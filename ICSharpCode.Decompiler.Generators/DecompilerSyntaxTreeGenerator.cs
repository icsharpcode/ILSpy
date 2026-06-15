// Copyright (c) 2026 Siegfried Pammer
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System.Collections;
using System.Collections.Immutable;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace ICSharpCode.Decompiler.Generators;

[Generator]
internal class DecompilerSyntaxTreeGenerator : IIncrementalGenerator
{
	record AstNodeAdditions(string NodeName, bool NeedsAcceptImpls, bool NeedsVisitor, bool NeedsNullNode, bool NeedsPatternPlaceholder, int NullNodeBaseCtorParamCount, bool IsTypeNode, string VisitMethodName, string VisitMethodParamType, EquatableArray<(string Member, string TypeName, bool RecursiveMatch, bool MatchAny, bool Nullable)>? MembersToMatch, EquatableArray<(string RoleExpr, bool IsCollection, string PropertyName, string PropertyType, string ElementType, bool IsOverride, bool IsNullable, string KindName)>? Slots);

	// Derives the shared SlotKind name from a [Slot] role expression: the last dotted segment with a
	// trailing "Role" removed (e.g. "Roles.EmbeddedStatement" -> "EmbeddedStatement", "LeftRole" ->
	// "Left", "PropertyDeclaration.GetterRole" -> "Getter"). Roles that share a name (aliases, or the
	// same logical slot across node types) intentionally collapse to one kind, so node.Slot.Kind matches
	// node.Role comparisons; consumer sites derive the kind from their role the same way.
	static string SlotKindName(string roleExpr)
	{
		int dot = roleExpr.LastIndexOf('.');
		string name = dot >= 0 ? roleExpr.Substring(dot + 1) : roleExpr;
		if (name.EndsWith("Role") && name.Length > 4)
			name = name.Substring(0, name.Length - 4);
		return name;
	}

	AstNodeAdditions GetAstNodeAdditions(GeneratorAttributeSyntaxContext context, CancellationToken ct)
	{
		var targetSymbol = (INamedTypeSymbol)context.TargetSymbol;
		var attribute = context.Attributes.SingleOrDefault(ad => ad.AttributeClass?.Name == "DecompilerAstNodeAttribute")!;
		var (visitMethodName, paramTypeName) = targetSymbol.Name switch {
			"ErrorExpression" => ("ErrorNode", "AstNode"),
			string s when s.Contains("AstType") => (s.Replace("AstType", "Type"), s),
			_ => (targetSymbol.Name, targetSymbol.Name),
		};

		List<(string Member, string TypeName, bool RecursiveMatch, bool MatchAny, bool Nullable)>? membersToMatch = null;

		// Abstract base nodes are never the dispatch target of DoMatch (concrete subclasses and the
		// null-object each emit their own and do not chain to base), so emitting one here is dead code.
		if (!targetSymbol.IsAbstract && !targetSymbol.MemberNames.Contains("DoMatch"))
		{
			membersToMatch = new();

			var astNodeType = (INamedTypeSymbol)context.SemanticModel.GetSpeculativeSymbolInfo(context.TargetNode.Span.Start, SyntaxFactory.ParseTypeName("AstNode"), SpeculativeBindingOption.BindAsTypeOrNamespace).Symbol!;
			var entityDeclarationType = context.SemanticModel.GetSpeculativeSymbolInfo(context.TargetNode.Span.Start, SyntaxFactory.ParseTypeName("EntityDeclaration"), SpeculativeBindingOption.BindAsTypeOrNamespace).Symbol as INamedTypeSymbol;

			// EntityDeclaration declares Name (over Roles.Identifier), ReturnType, and the attributes/modifiers
			// helper as virtual members on the base; a subclass overrides them, so the property scan (which
			// skips overrides) misses them. Add them explicitly for every EntityDeclaration-derived node. The
			// NameToken slot is intentionally not matched, since the Name string already covers it.
			if (entityDeclarationType != null && targetSymbol.IsDerivedFrom(entityDeclarationType))
			{
				// A node opts out of the inherited Name match by marking its NameToken slot
				// [ExcludeFromMatch] (e.g. constructors, whose Name is just the declaring type name).
				bool excludeName = targetSymbol.GetMembers("NameToken").OfType<IPropertySymbol>()
					.Any(p => p.GetAttributes().Any(a => a.AttributeClass?.Name == "ExcludeFromMatchAttribute"));
				if (!excludeName)
					membersToMatch.Add(("Name", "String", false, false, false));
				membersToMatch.Add(("MatchAttributesAndModifiers", null!, false, false, false));
				membersToMatch.Add(("ReturnType", "AstType", true, false, false));
			}

			foreach (var m in targetSymbol.GetMembers())
			{
				if (m is not IPropertySymbol property || property.IsIndexer || property.IsOverride)
					continue;
				if (property.GetAttributes().Any(a => a.AttributeClass?.Name == "ExcludeFromMatchAttribute"))
					continue;
				if (property.Type.MetadataName is "CSharpTokenNode" or "TextLocation")
					continue;
				bool nullable = property.NullableAnnotation == NullableAnnotation.Annotated;
				switch (property.Type)
				{
					case INamedTypeSymbol named when named.IsDerivedFrom(astNodeType) || named.MetadataName == "AstNodeCollection`1":
						membersToMatch.Add((property.Name, named.Name, true, false, nullable));
						break;
					case INamedTypeSymbol { TypeKind: TypeKind.Enum } named when named.GetMembers().Any(_ => _.Name == "Any"):
						membersToMatch.Add((property.Name, named.Name, false, true, false));
						break;
					default:
						membersToMatch.Add((property.Name, property.Type.Name, false, false, false));
						break;
				}
			}
		}

		// Collect the slot schema: child properties tagged [Slot], in declaration order. The
		// attribute names the Role expression to use; single vs collection comes from the type.
		List<(string RoleExpr, bool IsCollection, string PropertyName, string PropertyType, string ElementType, bool IsOverride, bool IsNullable, string KindName)>? slots = null;
		foreach (var m in targetSymbol.GetMembers())
		{
			if (m is not IPropertySymbol property)
				continue;
			var slotAttr = property.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == "SlotAttribute");
			if (slotAttr == null)
				continue;
			slots ??= new();
			string roleExpr = (string)slotAttr.ConstructorArguments[0].Value!;
			bool isCollection = property.Type.MetadataName == "AstNodeCollection`1";
			bool isNullable = property.Type.NullableAnnotation == NullableAnnotation.Annotated;
			var unannotated = property.Type.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
			string propertyType = unannotated.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			// For a collection slot, the element type is the single type argument of AstNodeCollection<T>.
			string elementType = isCollection
				? ((INamedTypeSymbol)property.Type).TypeArguments[0].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)
				: propertyType;
			slots.Add((roleExpr, isCollection, property.Name, propertyType, elementType, property.IsOverride, isNullable, SlotKindName(roleExpr)));
		}

		return new(targetSymbol.Name, !targetSymbol.MemberNames.Contains("AcceptVisitor"),
			NeedsVisitor: !targetSymbol.IsAbstract && targetSymbol.BaseType!.IsAbstract,
			NeedsNullNode: (bool)attribute.ConstructorArguments[0].Value!,
			NeedsPatternPlaceholder: (bool)attribute.ConstructorArguments[1].Value!,
			NullNodeBaseCtorParamCount: targetSymbol.InstanceConstructors.Min(m => m.Parameters.Length),
			IsTypeNode: targetSymbol.Name == "AstType" || targetSymbol.BaseType?.Name == "AstType",
			visitMethodName, paramTypeName, membersToMatch?.ToEquatableArray(), slots?.ToEquatableArray());
	}

	// Backing-field name for a slot property. Prefixed to avoid colliding with hand-written fields
	// in the same partial class; the generated file carries an auto-generated header, so naming-style
	// rules do not apply.
	static string FieldName(string propertyName) => "slot_" + propertyName;

	void WriteGeneratedMembers(SourceProductionContext context, AstNodeAdditions source)
	{
		var builder = new StringBuilder();

		builder.AppendLine("// <auto-generated/>");
		builder.AppendLine("namespace ICSharpCode.Decompiler.CSharp.Syntax;");
		builder.AppendLine();

		if (source.NeedsPatternPlaceholder)
		{
			builder.AppendLine("using ICSharpCode.Decompiler.CSharp.Syntax.PatternMatching;");
		}

		builder.AppendLine();

		builder.AppendLine("#nullable enable");
		builder.AppendLine();

		builder.AppendLine($"partial class {source.NodeName}");
		builder.AppendLine("{");

		if (source.NeedsNullNode)
		{
			bool needsNew = source.NodeName != "AstNode";

			builder.AppendLine($"	{(needsNew ? "new " : "")}public static readonly {source.NodeName} Null = new Null{source.NodeName}();");

			builder.AppendLine($@"
	sealed class Null{source.NodeName} : {source.NodeName}
	{{
		public override bool IsNull => true;

		public override void AcceptVisitor(IAstVisitor visitor)
		{{
			visitor.VisitNullNode(this);
		}}

		public override T AcceptVisitor<T>(IAstVisitor<T> visitor)
		{{
			return visitor.VisitNullNode(this);
		}}

		public override S AcceptVisitor<T, S>(IAstVisitor<T, S> visitor, T data)
		{{
			return visitor.VisitNullNode(this, data);
		}}

		protected internal override bool DoMatch(AstNode? other, PatternMatching.Match match)
		{{
			return other == null || other.IsNull;
		}}");

			if (source.NullNodeBaseCtorParamCount > 0)
			{
				builder.AppendLine($@"

		public Null{source.NodeName}() : base({string.Join(", ", Enumerable.Repeat("default", source.NullNodeBaseCtorParamCount))}) {{ }}");
			}

			builder.AppendLine($@"
	}}

");

		}

		if (source.NeedsPatternPlaceholder)
		{
			// The placeholder conversion is part of the pattern-construction DSL, where a non-null
			// pattern is the invariant; the result is therefore non-nullable for the specific node
			// types. AstNode (the base) and ParameterDeclaration keep the nullable contract, and only
			// AstNode also accepts a nullable pattern.
			bool nullableReturn = source.NodeName is "AstNode" or "ParameterDeclaration";
			string returnQ = nullableReturn ? "?" : "";
			string paramQ = source.NodeName == "AstNode" ? "?" : "";
			string forgive = nullableReturn ? "" : "!";
			builder.Append(
		$@"		public static implicit operator {source.NodeName}{returnQ}(PatternMatching.Pattern{paramQ} pattern)
	{{
		return pattern != null ? new PatternPlaceholder(pattern) : null{forgive};
	}}

	sealed class PatternPlaceholder : {source.NodeName}, INode, PatternMatching.IPatternPlaceholder
	{{
		readonly PatternMatching.Pattern child;

		public PatternPlaceholder(PatternMatching.Pattern child)
		{{
			this.child = child;
		}}

		public override void AcceptVisitor(IAstVisitor visitor)
		{{
			visitor.VisitPatternPlaceholder(this, child);
		}}

		public override T AcceptVisitor<T>(IAstVisitor<T> visitor)
		{{
			return visitor.VisitPatternPlaceholder(this, child);
		}}

		public override S AcceptVisitor<T, S>(IAstVisitor<T, S> visitor, T data)
		{{
			return visitor.VisitPatternPlaceholder(this, child, data);
		}}

		protected internal override bool DoMatch(AstNode? other, PatternMatching.Match match)
		{{
			return child.DoMatch(other, match);
		}}

		bool PatternMatching.INode.DoMatchCollection(global::System.Collections.Generic.IReadOnlyList<PatternMatching.INode> other, int pos, PatternMatching.Match match, PatternMatching.BacktrackingInfo backtrackingInfo)
		{{
			return child.DoMatchCollection(other, pos, match, backtrackingInfo);
		}}
	}}
"
			);
		}

		if (source.NeedsAcceptImpls && source.NeedsVisitor)
		{
			builder.Append($@"	public override void AcceptVisitor(IAstVisitor visitor)
	{{
		visitor.Visit{source.VisitMethodName}(this);
	}}

	public override T AcceptVisitor<T>(IAstVisitor<T> visitor)
	{{
		return visitor.Visit{source.VisitMethodName}(this);
	}}

	public override S AcceptVisitor<T, S>(IAstVisitor<T, S> visitor, T data)
	{{
		return visitor.Visit{source.VisitMethodName}(this, data);
	}}

");
		}

		if (source.MembersToMatch != null)
		{
			builder.Append($@"	protected internal override bool DoMatch(AstNode? other, PatternMatching.Match match)
	{{
		return other is {source.NodeName} o && !o.IsNull");

			foreach (var (member, typeName, recursive, hasAny, nullable) in source.MembersToMatch)
			{
				if (member == "MatchAttributesAndModifiers")
				{
					builder.Append($"\r\n\t\t\t&& this.MatchAttributesAndModifiers(o, match)");
				}
				else if (recursive && nullable && typeName != "AstNodeCollection")
				{
					// An optional single-value child is null when absent; match null-safely.
					builder.Append($"\r\n\t\t\t&& MatchOptional(this.{member}, o.{member}, match)");
				}
				else if (recursive)
				{
					builder.Append($"\r\n\t\t\t&& this.{member}.DoMatch(o.{member}, match)");
				}
				else if (hasAny)
				{
					builder.Append($"\r\n\t\t\t&& (this.{member} == {typeName}.Any || this.{member} == o.{member})");
				}
				else if (typeName == "String")
				{
					builder.Append($"\r\n\t\t\t&& MatchString(this.{member}, o.{member})");
				}
				else
				{
					builder.Append($"\r\n\t\t\t&& this.{member} == o.{member}");
				}
			}

			builder.Append(@";
	}

");
		}

		if (source.Slots is { } slotsArray)
		{
			var slots = slotsArray.ToList();

			// Backing fields and the partial-property bodies. A single slot stores a nullable field and
			// substitutes the role's null object when empty (pre-NRT); a collection slot owns a lazily
			// created AstNodeCollection bound to this node. A slot re-declared from an inherited contract
			// member (Part I.3 flatten) is an override.
			foreach (var (roleExpr, isCollection, name, type, elementType, isOverride, isNullable, kindName) in slots)
			{
				string field = FieldName(name);
				if (isCollection)
				{
					builder.AppendLine($"\tAstNodeCollection<{elementType}>? {field};");
					builder.AppendLine($"\tpublic {(isOverride ? "override " : "")}partial AstNodeCollection<{elementType}> {name} => {field} ??= new AstNodeCollection<{elementType}>(this, {roleExpr});");
				}
				else
				{
					builder.AppendLine($"\t{type}? {field};");
					builder.AppendLine($"\tpublic {(isOverride ? "override " : "")}partial {type} {name}");
					builder.AppendLine("\t{");
					builder.AppendLine($"\t\tget => {field}{(isNullable ? "" : $" ?? {roleExpr}.NullObject")};");
					builder.AppendLine($"\t\tset => SetChildNode(ref {field}, value, {roleExpr});");
					builder.AppendLine("\t}");
				}
				builder.AppendLine();
			}

			// Flattened child-index space: slots in declaration order, a single slot occupying one index
			// (even when empty), a collection slot a contiguous run of its current length.
			builder.Append("\tinternal override int GetChildCount() => ");
			builder.Append(string.Join(" + ", slots.Select(s => s.IsCollection ? $"({FieldName(s.PropertyName)}?.Count ?? 0)" : "1")));
			builder.AppendLine(";");
			builder.AppendLine();

			builder.AppendLine("\tinternal override AstNode? GetChild(int index)");
			builder.AppendLine("\t{");
			builder.AppendLine("\t\tint i = index;");
			foreach (var s in slots)
			{
				if (s.IsCollection)
				{
					string field = FieldName(s.PropertyName);
					builder.AppendLine($"\t\t{{ int n = {field}?.Count ?? 0; if (i < n) return {field}![i]; i -= n; }}");
				}
				else
				{
					builder.AppendLine($"\t\tif (i == 0) return {FieldName(s.PropertyName)}; i -= 1;");
				}
			}
			builder.AppendLine("\t\tthrow new System.ArgumentOutOfRangeException(nameof(index));");
			builder.AppendLine("\t}");
			builder.AppendLine();

			builder.AppendLine("\tinternal override void SetChild(int index, AstNode? value)");
			builder.AppendLine("\t{");
			builder.AppendLine("\t\tint i = index;");
			foreach (var s in slots)
			{
				if (s.IsCollection)
				{
					string field = FieldName(s.PropertyName);
					builder.AppendLine($"\t\t{{ int n = {field}?.Count ?? 0; if (i < n) {{ {field}![i] = ({s.ElementType})value!; return; }} i -= n; }}");
				}
				else
				{
					builder.AppendLine($"\t\tif (i == 0) {{ SetChildNode(ref {FieldName(s.PropertyName)}, ({s.PropertyType}?)value, {s.RoleExpr}); return; }} i -= 1;");
				}
			}
			builder.AppendLine("\t\tthrow new System.ArgumentOutOfRangeException(nameof(index));");
			builder.AppendLine("\t}");
			builder.AppendLine();

			builder.AppendLine("\tinternal override Role GetChildSlot(int index)");
			builder.AppendLine("\t{");
			builder.AppendLine("\t\tint i = index;");
			foreach (var s in slots)
			{
				if (s.IsCollection)
					builder.AppendLine($"\t\t{{ int n = {FieldName(s.PropertyName)}?.Count ?? 0; if (i < n) return {s.RoleExpr}; i -= n; }}");
				else
					builder.AppendLine($"\t\tif (i == 0) return {s.RoleExpr}; i -= 1;");
			}
			builder.AppendLine("\t\tthrow new System.ArgumentOutOfRangeException(nameof(index));");
			builder.AppendLine("\t}");
			builder.AppendLine();

			// One CSharpSlotInfo static per slot; node.Slot compares against these by object identity.
			foreach (var s in slots)
				builder.AppendLine($"\tpublic static readonly CSharpSlotInfo {s.PropertyName}Slot = new CSharpSlotInfo(\"{s.PropertyName}\", typeof({s.ElementType}), {(s.IsCollection ? "true" : "false")}, SlotKind.{s.KindName});");
			builder.AppendLine();

			builder.AppendLine("\tinternal override CSharpSlotInfo GetChildSlotInfo(int index)");
			builder.AppendLine("\t{");
			builder.AppendLine("\t\tint i = index;");
			foreach (var s in slots)
			{
				if (s.IsCollection)
					builder.AppendLine($"\t\t{{ int n = {FieldName(s.PropertyName)}?.Count ?? 0; if (i < n) return {s.PropertyName}Slot; i -= n; }}");
				else
					builder.AppendLine($"\t\tif (i == 0) return {s.PropertyName}Slot; i -= 1;");
			}
			builder.AppendLine("\t\tthrow new System.ArgumentOutOfRangeException(nameof(index));");
			builder.AppendLine("\t}");
			builder.AppendLine();

			if (slots.Any(s => s.IsCollection))
			{
				builder.AppendLine("\tinternal override AstNodeCollection? GetCollectionByRole(Role role)");
				builder.AppendLine("\t{");
				foreach (var s in slots.Where(s => s.IsCollection))
					builder.AppendLine($"\t\tif (role == {s.RoleExpr}) return {s.PropertyName};");
				builder.AppendLine("\t\treturn base.GetCollectionByRole(role);");
				builder.AppendLine("\t}");
				builder.AppendLine();
			}

			builder.AppendLine("\tinternal override void CloneChildrenInto(AstNode copyNode)");
			builder.AppendLine("\t{");
			builder.AppendLine($"\t\tvar copy = ({source.NodeName})copyNode;");
			foreach (var s in slots)
				builder.AppendLine($"\t\tcopy.{FieldName(s.PropertyName)} = null;");
			foreach (var s in slots)
			{
				string field = FieldName(s.PropertyName);
				if (s.IsCollection)
					builder.AppendLine($"\t\tif ({field} != null) foreach (var c in {field}) copy.{s.PropertyName}.Add(({s.ElementType})c.Clone());");
				else
					builder.AppendLine($"\t\tif ({field} != null) copy.{s.PropertyName} = ({s.PropertyType}){field}.Clone();");
			}
			builder.AppendLine("\t}");
			builder.AppendLine();
		}

		builder.AppendLine("}");

		context.AddSource(source.NodeName + ".g.cs", SourceText.From(builder.ToString().Replace("\r\n", "\n"), Encoding.UTF8));
	}

	void WriteVisitors(SourceProductionContext context, ImmutableArray<AstNodeAdditions> source)
	{
		var builder = new StringBuilder();

		builder.AppendLine("namespace ICSharpCode.Decompiler.CSharp.Syntax;");

		source = source
			.Concat([new("NullNode", false, true, false, false, 0, false, "NullNode", "AstNode", null, null), new("PatternPlaceholder", false, true, false, false, 0, false, "PatternPlaceholder", "AstNode", null, null)])
			.ToImmutableArray();

		WriteInterface("IAstVisitor", "void", "");
		WriteInterface("IAstVisitor<out S>", "S", "");
		WriteInterface("IAstVisitor<in T, out S>", "S", ", T data");

		context.AddSource("IAstVisitor.g.cs", SourceText.From(builder.ToString(), Encoding.UTF8));

		void WriteInterface(string name, string ret, string param)
		{
			builder.AppendLine($"public interface {name}");
			builder.AppendLine("{");

			foreach (var type in source.OrderBy(t => t.VisitMethodName))
			{
				if (!type.NeedsVisitor)
					continue;

				string extParams, paramName;
				if (type.VisitMethodName == "PatternPlaceholder")
				{
					paramName = "placeholder";
					extParams = ", PatternMatching.Pattern pattern" + param;
				}
				else
				{
					paramName = char.ToLowerInvariant(type.VisitMethodName[0]) + type.VisitMethodName.Substring(1);
					extParams = param;
				}

				builder.AppendLine($"\t{ret} Visit{type.VisitMethodName}({type.VisitMethodParamType} {paramName}{extParams});");
			}

			builder.AppendLine("}");
		}
	}

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var astNodeAdditions = context.SyntaxProvider.ForAttributeWithMetadataName(
			"ICSharpCode.Decompiler.CSharp.Syntax.DecompilerAstNodeAttribute",
			(n, ct) => n is ClassDeclarationSyntax,
			GetAstNodeAdditions);

		var visitorMembers = astNodeAdditions.Collect();

		context
			.RegisterPostInitializationOutput(i => i.AddSource("DecompilerSyntaxTreeGeneratorAttributes.g.cs", @"

using System;

namespace Microsoft.CodeAnalysis
{
    internal sealed partial class EmbeddedAttribute : global::System.Attribute
    {
    }
}

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	[global::Microsoft.CodeAnalysis.EmbeddedAttribute]
	sealed class DecompilerAstNodeAttribute : global::System.Attribute
	{
		public DecompilerAstNodeAttribute(bool hasNullNode = false, bool hasPatternPlaceholder = false) { }
	}

	[global::Microsoft.CodeAnalysis.EmbeddedAttribute]
	sealed class ExcludeFromMatchAttribute : global::System.Attribute
	{
	}

	[global::Microsoft.CodeAnalysis.EmbeddedAttribute]
	[global::System.AttributeUsage(global::System.AttributeTargets.Property)]
	sealed class SlotAttribute : global::System.Attribute
	{
		public SlotAttribute(string role) { }
	}
}

"));

		context.RegisterSourceOutput(astNodeAdditions, WriteGeneratedMembers);
		context.RegisterSourceOutput(visitorMembers, WriteVisitors);
		context.RegisterSourceOutput(visitorMembers, WriteSlotKinds);
	}

	// Emits the SlotKind enum: one value per distinct slot kind across all nodes. A node's CSharpSlotInfo
	// carries its kind, and consumers compare node.Slot.Kind == SlotKind.X -- shared across node types, so
	// it replaces the old polymorphic node.Role == Roles.X comparisons.
	void WriteSlotKinds(SourceProductionContext context, ImmutableArray<AstNodeAdditions> source)
	{
		var kinds = new SortedSet<string>(StringComparer.Ordinal);
		foreach (var node in source)
		{
			if (node.Slots is { } slots)
			{
				foreach (var s in slots)
					kinds.Add(s.KindName);
			}
		}

		var builder = new StringBuilder();
		builder.AppendLine("namespace ICSharpCode.Decompiler.CSharp.Syntax;");
		builder.AppendLine();
		builder.AppendLine("/// <summary>Identifies the kind of an AST child slot, shared across node types.</summary>");
		builder.AppendLine("public enum SlotKind");
		builder.AppendLine("{");
		builder.AppendLine("\tNone,");
		foreach (var k in kinds)
			builder.AppendLine($"\t{k},");
		builder.AppendLine("}");

		context.AddSource("SlotKind.g.cs", SourceText.From(builder.ToString(), Encoding.UTF8));
	}
}

readonly struct EquatableArray<T> : IEquatable<EquatableArray<T>>, IEnumerable<T>
	where T : IEquatable<T>
{
	readonly T[] array;

	public EquatableArray(T[] array)
	{
		this.array = array ?? throw new ArgumentNullException(nameof(array));
	}

	public bool Equals(EquatableArray<T> other)
	{
		return other.array.AsSpan().SequenceEqual(this.array);
	}

	public IEnumerator<T> GetEnumerator()
	{
		return ((IEnumerable<T>)array).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return array.GetEnumerator();
	}
}

static class EquatableArrayExtensions
{
	public static EquatableArray<T> ToEquatableArray<T>(this List<T> array) where T : IEquatable<T>
	{
		return new EquatableArray<T>(array.ToArray());
	}
}