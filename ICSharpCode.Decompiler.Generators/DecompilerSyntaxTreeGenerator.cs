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
	record AstNodeAdditions(string NodeName, bool NeedsAcceptImpls, bool NeedsVisitor, bool NeedsNullNode, bool NeedsPatternPlaceholder, int NullNodeBaseCtorParamCount, bool IsTypeNode, string VisitMethodName, string VisitMethodParamType, EquatableArray<(string Member, string TypeName, bool RecursiveMatch, bool MatchAny)>? MembersToMatch, EquatableArray<(string RoleExpr, bool IsCollection, string PropertyName, string PropertyType, bool IsOverride)>? Slots);

	AstNodeAdditions GetAstNodeAdditions(GeneratorAttributeSyntaxContext context, CancellationToken ct)
	{
		var targetSymbol = (INamedTypeSymbol)context.TargetSymbol;
		var attribute = context.Attributes.SingleOrDefault(ad => ad.AttributeClass?.Name == "DecompilerAstNodeAttribute")!;
		var (visitMethodName, paramTypeName) = targetSymbol.Name switch {
			"ErrorExpression" => ("ErrorNode", "AstNode"),
			string s when s.Contains("AstType") => (s.Replace("AstType", "Type"), s),
			_ => (targetSymbol.Name, targetSymbol.Name),
		};

		List<(string Member, string TypeName, bool RecursiveMatch, bool MatchAny)>? membersToMatch = null;

		if (!targetSymbol.MemberNames.Contains("DoMatch"))
		{
			membersToMatch = new();

			var astNodeType = (INamedTypeSymbol)context.SemanticModel.GetSpeculativeSymbolInfo(context.TargetNode.Span.Start, SyntaxFactory.ParseTypeName("AstNode"), SpeculativeBindingOption.BindAsTypeOrNamespace).Symbol!;

			if (targetSymbol.BaseType!.MemberNames.Contains("MatchAttributesAndModifiers"))
				membersToMatch.Add(("MatchAttributesAndModifiers", null!, false, false));

			foreach (var m in targetSymbol.GetMembers())
			{
				if (m is not IPropertySymbol property || property.IsIndexer || property.IsOverride)
					continue;
				if (property.GetAttributes().Any(a => a.AttributeClass?.Name == "ExcludeFromMatchAttribute"))
					continue;
				if (property.Type.MetadataName is "CSharpTokenNode" or "TextLocation")
					continue;
				switch (property.Type)
				{
					case INamedTypeSymbol named when named.IsDerivedFrom(astNodeType) || named.MetadataName == "AstNodeCollection`1":
						membersToMatch.Add((property.Name, named.Name, true, false));
						break;
					case INamedTypeSymbol { TypeKind: TypeKind.Enum } named when named.GetMembers().Any(_ => _.Name == "Any"):
						membersToMatch.Add((property.Name, named.Name, false, true));
						break;
					default:
						membersToMatch.Add((property.Name, property.Type.Name, false, false));
						break;
				}
			}
		}

		// Collect the slot schema: child properties tagged [Slot], in declaration order. The
		// attribute names the Role expression to use; single vs collection comes from the type.
		List<(string RoleExpr, bool IsCollection, string PropertyName, string PropertyType, bool IsOverride)>? slots = null;
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
			string propertyType = property.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			slots.Add((roleExpr, isCollection, property.Name, propertyType, property.IsOverride));
		}

		return new(targetSymbol.Name, !targetSymbol.MemberNames.Contains("AcceptVisitor"),
			NeedsVisitor: !targetSymbol.IsAbstract && targetSymbol.BaseType!.IsAbstract,
			NeedsNullNode: (bool)attribute.ConstructorArguments[0].Value!,
			NeedsPatternPlaceholder: (bool)attribute.ConstructorArguments[1].Value!,
			NullNodeBaseCtorParamCount: targetSymbol.InstanceConstructors.Min(m => m.Parameters.Length),
			IsTypeNode: targetSymbol.Name == "AstType" || targetSymbol.BaseType?.Name == "AstType",
			visitMethodName, paramTypeName, membersToMatch?.ToEquatableArray(), slots?.ToEquatableArray());
	}

	void WriteGeneratedMembers(SourceProductionContext context, AstNodeAdditions source)
	{
		var builder = new StringBuilder();

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
		public override NodeType NodeType => NodeType.Unknown;

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
			builder.Append(
		$@"		public static implicit operator {source.NodeName}?(PatternMatching.Pattern? pattern)
	{{
		return pattern != null ? new PatternPlaceholder(pattern) : null;
	}}

	sealed class PatternPlaceholder : {source.NodeName}, INode
	{{
		readonly PatternMatching.Pattern child;

		public PatternPlaceholder(PatternMatching.Pattern child)
		{{
			this.child = child;
		}}

		public override NodeType NodeType {{
			get {{ return NodeType.Pattern; }}
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

		bool PatternMatching.INode.DoMatchCollection(Role? role, PatternMatching.INode? pos, PatternMatching.Match match, PatternMatching.BacktrackingInfo backtrackingInfo)
		{{
			return child.DoMatchCollection(role, pos, match, backtrackingInfo);
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

			foreach (var (member, typeName, recursive, hasAny) in source.MembersToMatch)
			{
				if (member == "MatchAttributesAndModifiers")
				{
					builder.Append($"\r\n\t\t\t&& this.MatchAttributesAndModifiers(o, match)");
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

			// Implementing half of each [Slot] partial property: stored over the linked list for now.
			// A slot re-declared from an inherited contract member (Part I.3 flatten) is an override.
			foreach (var (roleExpr, isCollection, name, type, isOverride) in slots)
			{
				builder.AppendLine($"\tpublic {(isOverride ? "override " : "")}partial {type} {name}");
				builder.AppendLine("\t{");
				builder.AppendLine($"\t\tget {{ return GetChild{(isCollection ? "ren" : "")}ByRole({roleExpr}); }}");
				if (!isCollection)
					builder.AppendLine($"\t\tset {{ SetChildByRole({roleExpr}, value); }}");
				builder.AppendLine("\t}");
			}

			// Slot schema (transitional bridge): source-ordered slots mapped to roles.
			builder.AppendLine($"\tinternal override int SlotCount => {slots.Count};");
			builder.AppendLine("\tinternal override Role GetSlotRole(int slotIndex)");
			builder.AppendLine("\t{");
			builder.AppendLine("\t\tswitch (slotIndex)");
			builder.AppendLine("\t\t{");
			for (int i = 0; i < slots.Count; i++)
				builder.AppendLine($"\t\t\tcase {i}: return {slots[i].RoleExpr};");
			builder.AppendLine("\t\t\tdefault: throw new System.ArgumentOutOfRangeException(nameof(slotIndex));");
			builder.AppendLine("\t\t}");
			builder.AppendLine("\t}");
			builder.AppendLine("\tinternal override bool IsCollectionSlot(int slotIndex)");
			builder.AppendLine("\t{");
			builder.AppendLine("\t\tswitch (slotIndex)");
			builder.AppendLine("\t\t{");
			for (int i = 0; i < slots.Count; i++)
				builder.AppendLine($"\t\t\tcase {i}: return {(slots[i].IsCollection ? "true" : "false")};");
			builder.AppendLine("\t\t\tdefault: throw new System.ArgumentOutOfRangeException(nameof(slotIndex));");
			builder.AppendLine("\t\t}");
			builder.AppendLine("\t}");
		}

		builder.AppendLine("}");

		context.AddSource(source.NodeName + ".g.cs", SourceText.From(builder.ToString(), Encoding.UTF8));
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