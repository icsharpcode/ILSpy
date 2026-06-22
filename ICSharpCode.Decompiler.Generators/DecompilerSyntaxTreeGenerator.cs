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
	// Enforces the slot-model invariant: a slot kind names one child position, so it must map to a
	// single declared child type across all nodes that use it. A kind used with two different types
	// would have to widen its typed Slots constant to AstNode, which makes the typed child accessors
	// (GetChildren in particular) unable to recover the real element type and throw at runtime. A
	// position that is genuinely either an expression or a statement (a lambda body) is still one
	// declared type -- AstNode -- and is fine; the error fires only on coincidental name reuse.
	static readonly DiagnosticDescriptor MultipleChildTypesForKind = new(
		id: "DSTG001",
		title: "Slot kind used with multiple child types",
		messageFormat: "Slot kind '{0}' is declared with multiple child types ({1}); each [Slot] kind must map to a single child type. Give the differing position its own [Slot] name.",
		category: "DecompilerSyntaxTreeGenerator",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true);

	record AstNodeAdditions(string NodeName, bool NeedsVisitor, bool IsAbstract, bool BaseHasDefaultConstructor, bool NeedsPatternPlaceholder, string VisitMethodName, string VisitMethodParamType, EquatableArray<MemberMatch>? MembersToMatch, EquatableArray<SlotInfo>? Slots, EquatableArray<NameAccessor>? NameAccessors, EquatableArray<CtorParam>? CtorParams);

	// One DoMatch comparison: Member is the property (or the "MatchAttributesAndModifiers" sentinel);
	// RecursiveMatch matches child nodes structurally, MatchAny compares an enum that has an "Any"
	// wildcard, and TypeName/Nullable pick the comparison form for the rest.
	readonly record struct MemberMatch(string Member, string TypeName, bool RecursiveMatch, bool MatchAny, bool Nullable);

	// One child slot's schema: a single AstNode-typed child, an AstNodeCollection, or the backing
	// Identifier token of a string name. KindName is the shared slot-kind name; IsPartial is false only for
	// the generator-owned token slot behind a name (the source does not declare that property).
	readonly record struct SlotInfo(bool IsCollection, string PropertyName, string PropertyType, string ElementType, bool IsOverride, bool IsNullable, string KindName, bool IsPartial);

	// A string name accessor (StringName) over its backing Identifier token slot (TokenName); IsOptional
	// when the name may be absent (the token is then a nullable slot).
	readonly record struct NameAccessor(string StringName, string TokenName, bool IsOptional);

	// One generated constructor parameter. ParamType is the full type for a non-collection; for a
	// collection it is empty and ElementType names the element. IsOptional (nullable / collection)
	// drives the required-parameter prefix.
	readonly record struct CtorParam(string PropertyName, string ParamType, string ElementType, bool IsCollection, bool IsOptional);

	AstNodeAdditions GetAstNodeAdditions(GeneratorAttributeSyntaxContext context, CancellationToken ct)
	{
		var targetSymbol = (INamedTypeSymbol)context.TargetSymbol;
		var attribute = context.Attributes.SingleOrDefault(ad => ad.AttributeClass?.Name == "DecompilerAstNodeAttribute")!;
		var (visitMethodName, paramTypeName) = targetSymbol.Name switch {
			"ErrorExpression" => ("ErrorNode", "AstNode"),
			string s when s.EndsWith("AstType") => (s.Replace("AstType", "Type"), s),
			_ => (targetSymbol.Name, targetSymbol.Name),
		};

		List<MemberMatch>? membersToMatch = null;

		// Abstract base nodes are never the dispatch target of DoMatch (concrete subclasses each emit
		// their own and do not chain to base), so emitting one here is dead code.
		if (!targetSymbol.IsAbstract && !targetSymbol.MemberNames.Contains("DoMatch"))
		{
			membersToMatch = new();

			var compilation = context.SemanticModel.Compilation;
			var astNodeType = compilation.GetTypeByMetadataName("ICSharpCode.Decompiler.CSharp.Syntax.AstNode")!;
			var entityDeclarationType = compilation.GetTypeByMetadataName("ICSharpCode.Decompiler.CSharp.Syntax.EntityDeclaration");

			// EntityDeclaration declares Name (a string [Slot] over its Identifier token), ReturnType, and the attributes/modifiers
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
					membersToMatch.Add(new("Name", "String", RecursiveMatch: false, MatchAny: false, Nullable: false));
				membersToMatch.Add(new("MatchAttributesAndModifiers", null!, RecursiveMatch: false, MatchAny: false, Nullable: false));
				membersToMatch.Add(new("ReturnType", "AstType", RecursiveMatch: true, MatchAny: false, Nullable: true));
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
						membersToMatch.Add(new(property.Name, named.Name, RecursiveMatch: true, MatchAny: false, Nullable: nullable));
						break;
					case INamedTypeSymbol { TypeKind: TypeKind.Enum } named when named.GetMembers().Any(_ => _.Name == "Any"):
						membersToMatch.Add(new(property.Name, named.Name, RecursiveMatch: false, MatchAny: true, Nullable: false));
						break;
					default:
						membersToMatch.Add(new(property.Name, property.Type.Name, RecursiveMatch: false, MatchAny: false, Nullable: false));
						break;
				}
			}
		}

		// Collect the slot schema: child properties tagged [Slot], in declaration order
		// (the order is the node's child layout, so it must follow the source, not be grouped by kind).
		// [Slot] names the slot kind and infers the shape from the property type: an AstNode (or collection)
		// is a child slot; a string X is a name -- only the convenience string is declared, and the generator
		// owns the backing Identifier XToken child slot (emitted like a [Slot], but non-partial since the
		// source does not declare it) plus the string body. DoMatch matches X (a string) and never sees XToken.
		List<SlotInfo>? slots = null;
		List<NameAccessor>? nameAccessors = null;
		// Constructor parameters in declaration order: single/collection [Slot] children, the string [Slot]
		// name, and settable enum-typed scalar properties (e.g. Operator, FieldDirection).
		List<CtorParam>? ctorParams = null;
		foreach (var m in targetSymbol.GetMembers())
		{
			if (m is not IPropertySymbol property)
				continue;
			var slotAttr = property.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == "SlotAttribute");
			if (slotAttr != null)
			{
				slots ??= new();
				// The [Slot] argument is already the bare slot-kind name (e.g. "Body", "Expression"); kinds
				// shared across nodes (aliases, or the same logical slot on different node types) deliberately
				// collapse to one name, so node.Slot.Kind is shared and consumers compare against Slots.X.
				string kindName = (string)slotAttr.ConstructorArguments[0].Value!;
				// A [Slot] on a string property is a name: a convenience string accessor over a backing
				// Identifier token slot. Child slots are AstNode-typed, so the property type disambiguates -
				// no separate attribute is needed. Optionality comes from the nullable annotation ('string?'
				// = the name may be absent); a 'string?' declaration already requires a #nullable context.
				if (property.Type.SpecialType == SpecialType.System_String)
				{
					nameAccessors ??= new();
					bool nameNullable = property.Type.NullableAnnotation == NullableAnnotation.Annotated;
					string tokenName = property.Name + "Token";
					// An optional name makes the backing token a real nullable slot: an absent name is a null token.
					slots.Add(new(IsCollection: false, tokenName, "Identifier", "Identifier", IsOverride: false, nameNullable, kindName, IsPartial: false));
					nameAccessors.Add(new(property.Name, tokenName, nameNullable));
					// The name is the primary construction value, so it is a required ctor param regardless of
					// optionality (which only governs the setter's empty-to-null behaviour and the property type).
					ctorParams ??= new();
					ctorParams.Add(new(property.Name, "string", "", IsCollection: false, IsOptional: false));
					continue;
				}
				bool isCollection = property.Type.MetadataName == "AstNodeCollection`1";
				bool isNullable = property.Type.NullableAnnotation == NullableAnnotation.Annotated;
				var unannotated = property.Type.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
				string propertyType = unannotated.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
				// For a collection slot, the element type is the single type argument of AstNodeCollection<T>.
				string elementType = isCollection
					? ((INamedTypeSymbol)property.Type).TypeArguments[0].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)
					: propertyType;
				slots.Add(new(isCollection, property.Name, propertyType, elementType, property.IsOverride, isNullable, kindName, IsPartial: true));
				ctorParams ??= new();
				ctorParams.Add(isCollection
					? new(property.Name, "", elementType, IsCollection: true, IsOptional: true)
					: new(property.Name, propertyType + (isNullable ? "?" : ""), "", IsCollection: false, IsOptional: isNullable));
				continue;
			}
			// A settable enum-typed scalar (e.g. Operator, FieldDirection) is part of construction. The enum may
			// live in another namespace, so fully-qualify it (the generated file has only a few usings).
			if (property.Type.TypeKind == TypeKind.Enum && property.SetMethod != null && !property.IsStatic)
			{
				ctorParams ??= new();
				ctorParams.Add(new(property.Name, CtorParamTypeName(property.Type), "", IsCollection: false, IsOptional: false));
			}
		}

		return new(targetSymbol.Name,
			NeedsVisitor: !targetSymbol.IsAbstract && targetSymbol.BaseType!.IsAbstract,
			IsAbstract: targetSymbol.IsAbstract,
			BaseHasDefaultConstructor: targetSymbol.BaseType is { } bt && bt.InstanceConstructors.Any(c => c.Parameters.Length == 0 && c.DeclaredAccessibility != Accessibility.Private),
			NeedsPatternPlaceholder: (bool)attribute.ConstructorArguments[0].Value!,
			visitMethodName, paramTypeName, membersToMatch?.ToEquatableArray(), slots?.ToEquatableArray(), nameAccessors?.ToEquatableArray(), ctorParams?.ToEquatableArray());
	}

	// Backing-field name for a slot property. Prefixed to avoid colliding with hand-written fields
	// in the same partial class; the generated file carries an auto-generated header, so naming-style
	// rules do not apply.
	static string FieldName(string propertyName) => "slot_" + propertyName;

	// Type name for an enum-typed constructor parameter. Fully-qualified so an enum in another
	// namespace resolves without a using, minus the generated file's own namespace prefix, which is
	// redundant there. Used only in parameter-type position, so no member-name shadowing applies
	// (unlike Identifier.Create in name-slot setters, which must stay global::-qualified).
	static string CtorParamTypeName(ITypeSymbol type)
	{
		const string ownNamespacePrefix = "global::ICSharpCode.Decompiler.CSharp.Syntax.";
		string name = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
		return name.StartsWith(ownNamespacePrefix) ? name.Substring(ownNamespacePrefix.Length) : name;
	}

	void WriteGeneratedMembers(SourceProductionContext context, AstNodeAdditions source)
	{
		var builder = new StringBuilder();

		builder.AppendLine("// <auto-generated/>");
		builder.AppendLine("#nullable enable");
		builder.AppendLine();

		bool hasSlots = source.Slots is not null;
		if (hasSlots)
			builder.AppendLine("using System.Collections.Generic;");
		if (source.NeedsPatternPlaceholder)
			builder.AppendLine("using ICSharpCode.Decompiler.CSharp.Syntax.PatternMatching;");
		if (hasSlots || source.NeedsPatternPlaceholder)
			builder.AppendLine();

		builder.AppendLine("namespace ICSharpCode.Decompiler.CSharp.Syntax;");
		builder.AppendLine();

		builder.AppendLine($"partial class {source.NodeName}");
		builder.AppendLine("{");

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
		$@"	public static implicit operator {source.NodeName}{returnQ}(PatternMatching.Pattern{paramQ} pattern)
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

		if (source.NeedsVisitor)
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
		return other is {source.NodeName} o");

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

			// Backing fields and the partial-property bodies. A single slot stores a nullable backing
			// field (returned null-forgiving for a required child, nullable for an optional one); a
			// collection slot owns a lazily created AstNodeCollection bound to this node. A slot re-declared from an inherited contract
			// member (Part I.3 flatten) is an override.
			// A collection can maintain its children's flattened indices incrementally only when it is the
			// node's sole collection and its last slot: then it owns the contiguous range [slotIndex, ..)
			// with nothing after it, so an element's index is slotIndex + its local position (all
			// preceding slots are single children, one index each).
			int collectionCount = slots.Count(s => s.IsCollection);
			for (int slotIndex = 0; slotIndex < slots.Count; slotIndex++)
			{
				var (isCollection, name, type, elementType, isOverride, isNullable, kindName, isPartial) = slots[slotIndex];
				string field = FieldName(name);
				string partialKw = isPartial ? "partial " : "";
				if (isCollection)
				{
					bool supportsIncremental = collectionCount == 1 && slotIndex == slots.Count - 1;
					builder.AppendLine($"\tAstNodeCollection<{elementType}>? {field};");
					builder.AppendLine($"\tpublic {(isOverride ? "override " : "")}{partialKw}AstNodeCollection<{elementType}> {name} => {field} ??= new AstNodeCollection<{elementType}>(this, Slots.{kindName}, {slotIndex}, {(supportsIncremental ? "true" : "false")});");
				}
				else
				{
					// A single slot occupies a fixed flattened index. When no collection precedes it, that
					// index is the constant slotIndex, so the setter can assign childIndex directly (the ILAst
					// pattern -- no invalidate, no renumber). After a collection the index is dynamic, so fall
					// back to the index-less setter (which invalidates on a set/clear).
					bool constIndex = !slots.Take(slotIndex).Any(s => s.IsCollection);
					builder.AppendLine($"\t{type}? {field};");
					builder.AppendLine($"\tpublic {(isOverride ? "override " : "")}{partialKw}{type}{(isNullable ? "?" : "")} {name}");
					builder.AppendLine("\t{");
					builder.AppendLine($"\t\tget => {field}{(isNullable ? "" : "!")};");
					if (constIndex)
						builder.AppendLine($"\t\tset => SetChildNode(ref {field}, value, {slotIndex});");
					else
						builder.AppendLine($"\t\tset => SetChildNode(ref {field}, value);");
					builder.AppendLine("\t}");
				}
				builder.AppendLine();
			}

			// A string [Slot] is a convenience accessor over its generated Identifier token slot.
			if (source.NameAccessors is { } nameAccessorsArray)
			{
				foreach (var (stringName, tokenName, isOptional) in nameAccessorsArray)
				{
					// The token factory is the type 'Identifier'. A [Slot] string property literally named
					// "Identifier" (e.g. SimpleType.Identifier) shadows that type inside its own setter, so the
					// bare name would bind to the string property; qualify with global:: only in that case.
					string createType = stringName == "Identifier"
						? "global::ICSharpCode.Decompiler.CSharp.Syntax.Identifier"
						: "Identifier";
					builder.AppendLine($"\tpublic partial string{(isOptional ? "?" : "")} {stringName}");
					builder.AppendLine("\t{");
					if (isOptional)
					{
						// Optional name: the backing token is a nullable slot. An absent name reads as null; an
						// empty or null name clears the token, so "" and null both mean "no name".
						builder.AppendLine($"\t\tget => {tokenName}?.Name;");
						builder.AppendLine($"\t\tset => {tokenName} = {createType}.CreateIfNotEmpty(value);");
					}
					else
					{
						builder.AppendLine($"\t\tget => {tokenName}.Name;");
						builder.AppendLine($"\t\tset => {tokenName} = {createType}.Create(value);");
					}
					builder.AppendLine("\t}");
					builder.AppendLine();
				}
			}

			// Constructors. Parameters follow member source order and cover single/collection [Slot] children,
			// the string [Slot], and settable enum scalars (Operator, FieldDirection, ...); a collection is
			// an IEnumerable<T> param in its declared position. We emit the empty ctor (for object-initializer
			// construction), then a ctor for the required prefix (through the last required param), one ending at
			// each collection, and one with all params. A params T[] overload is added when a ctor's last param
			// is the collection. Pure-scalar nodes (no [Slot], e.g. PrimitiveExpression) are excluded
			// because their non-enum state (a literal value) is invisible here; those keep hand-written ctors.
			if (!source.IsAbstract && source.BaseHasDefaultConstructor && slots.Count > 0 && source.CtorParams is { } ctorParamsArray)
			{
				var cp = ctorParamsArray.ToList();
				string ParamName(string n)
				{
					string p = char.ToLowerInvariant(n[0]) + n.Substring(1);
					return SyntaxFacts.GetKeywordKind(p) != SyntaxKind.None ? "@" + p : p;
				}
				string ParamType(int i) => cp[i].IsCollection
					? $"IEnumerable<{cp[i].ElementType}>"
					: cp[i].ParamType;

				builder.AppendLine($"\tpublic {source.NodeName}()");
				builder.AppendLine("\t{");
				builder.AppendLine("\t}");
				builder.AppendLine();

				// Required prefix: through the last non-optional param (an optional param before it is still
				// positionally included so the required param after it can be passed).
				int reqLen = 0;
				for (int i = 0; i < cp.Count; i++)
					if (!cp[i].IsOptional)
						reqLen = i + 1;

				// A normal prefix ctor forwards to the previous (shorter) emitted prefix via : this(...) and only
				// sets the params between them; the shortest sets its params directly. A params T[] overload
				// forwards to the IEnumerable overload of the same length.
				void EmitPrefix(int len, int chainLen, bool paramsForm)
				{
					var decls = new List<string>();
					for (int i = 0; i < len; i++)
					{
						if (paramsForm && i == len - 1)
							decls.Add($"params {cp[i].ElementType}[] {ParamName(cp[i].PropertyName)}");
						else
							decls.Add($"{ParamType(i)} {ParamName(cp[i].PropertyName)}");
					}
					builder.AppendLine($"\tpublic {source.NodeName}({string.Join(", ", decls)})");
					if (paramsForm)
					{
						var args = new List<string>();
						for (int i = 0; i < len; i++)
							args.Add(i == len - 1
								? $"(IEnumerable<{cp[i].ElementType}>){ParamName(cp[i].PropertyName)}"
								: ParamName(cp[i].PropertyName));
						builder.AppendLine($"\t\t: this({string.Join(", ", args)})");
						builder.AppendLine("\t{");
						builder.AppendLine("\t}");
					}
					else
					{
						if (chainLen > 0)
							builder.AppendLine($"\t\t: this({string.Join(", ", Enumerable.Range(0, chainLen).Select(i => ParamName(cp[i].PropertyName)))})");
						builder.AppendLine("\t{");
						for (int i = chainLen; i < len; i++)
						{
							if (cp[i].IsCollection)
								builder.AppendLine($"\t\tthis.{cp[i].PropertyName}.AddRange({ParamName(cp[i].PropertyName)});");
							else
								builder.AppendLine($"\t\tthis.{cp[i].PropertyName} = {ParamName(cp[i].PropertyName)};");
						}
						builder.AppendLine("\t}");
					}
					builder.AppendLine();
				}

				var lengths = new SortedSet<int>();
				if (reqLen > 0)
					lengths.Add(reqLen);
				for (int i = 0; i < cp.Count; i++)
					if (cp[i].IsCollection && i + 1 >= reqLen)
						lengths.Add(i + 1);
				lengths.Add(cp.Count);
				int prev = 0;
				foreach (int len in lengths)
				{
					if (len <= 0)
						continue;
					EmitPrefix(len, prev, paramsForm: false);
					if (cp[len - 1].IsCollection)
						EmitPrefix(len, len, paramsForm: true);
					prev = len;
				}
			}

			// Flattened child-index space: slots in declaration order, a single slot occupying one index
			// (even when empty), a collection slot a contiguous run of its current length.
			var countTerms = new List<string>();
			int singleSlotCount = slots.Count(s => !s.IsCollection);
			if (singleSlotCount > 0)
				countTerms.Add(singleSlotCount.ToString());
			foreach (var s in slots.Where(s => s.IsCollection))
				countTerms.Add($"({FieldName(s.PropertyName)}?.Count ?? 0)");
			builder.AppendLine($"\tinternal override int GetChildCount() => {string.Join(" + ", countTerms)};");
			builder.AppendLine();

			bool anyCollection = slots.Any(s => s.IsCollection);

			// Emits a method body that maps a flat child index to a slot and returns an expression for it.
			// With only single slots the index is a constant offset, so a switch reads best; once a
			// collection slot is present the widths are dynamic, so walk the slots subtracting each one's
			// length from a running index.
			void EmitReturnDispatch(Func<int, string> singleExpr, Func<int, string> collectionExpr)
			{
				if (!anyCollection)
				{
					builder.AppendLine("\t\tswitch (index)");
					builder.AppendLine("\t\t{");
					for (int k = 0; k < slots.Count; k++)
					{
						builder.AppendLine($"\t\t\tcase {k}:");
						builder.AppendLine($"\t\t\t\treturn {singleExpr(k)};");
					}
					builder.AppendLine("\t\t\tdefault:");
					builder.AppendLine("\t\t\t\tthrow new System.ArgumentOutOfRangeException(nameof(index));");
					builder.AppendLine("\t\t}");
					return;
				}
				builder.AppendLine("\t\tint i = index;");
				for (int k = 0; k < slots.Count; k++)
				{
					bool last = k == slots.Count - 1;
					if (slots[k].IsCollection)
					{
						string field = FieldName(slots[k].PropertyName);
						builder.AppendLine("\t\t{");
						builder.AppendLine($"\t\t\tint n = {field}?.Count ?? 0;");
						builder.AppendLine("\t\t\tif (i < n)");
						builder.AppendLine($"\t\t\t\treturn {collectionExpr(k)};");
						if (!last)
							builder.AppendLine("\t\t\ti -= n;");
						builder.AppendLine("\t\t}");
					}
					else
					{
						builder.AppendLine("\t\tif (i == 0)");
						builder.AppendLine($"\t\t\treturn {singleExpr(k)};");
						if (!last)
							builder.AppendLine("\t\ti--;");
					}
				}
				builder.AppendLine("\t\tthrow new System.ArgumentOutOfRangeException(nameof(index));");
			}

			builder.AppendLine("\tinternal override AstNode? GetChild(int index)");
			builder.AppendLine("\t{");
			EmitReturnDispatch(k => FieldName(slots[k].PropertyName), k => $"{FieldName(slots[k].PropertyName)}![i]");
			builder.AppendLine("\t}");
			builder.AppendLine();

			builder.AppendLine("\tinternal override void SetChild(int index, AstNode? value)");
			builder.AppendLine("\t{");
			if (!anyCollection)
			{
				builder.AppendLine("\t\tswitch (index)");
				builder.AppendLine("\t\t{");
				for (int k = 0; k < slots.Count; k++)
				{
					builder.AppendLine($"\t\t\tcase {k}:");
					builder.AppendLine($"\t\t\t\tSetChildNode(ref {FieldName(slots[k].PropertyName)}, ({slots[k].PropertyType}?)value, index);");
					builder.AppendLine("\t\t\t\treturn;");
				}
				builder.AppendLine("\t\t\tdefault:");
				builder.AppendLine("\t\t\t\tthrow new System.ArgumentOutOfRangeException(nameof(index));");
				builder.AppendLine("\t\t}");
			}
			else
			{
				builder.AppendLine("\t\tint i = index;");
				for (int k = 0; k < slots.Count; k++)
				{
					bool last = k == slots.Count - 1;
					var s = slots[k];
					string field = FieldName(s.PropertyName);
					if (s.IsCollection)
					{
						builder.AppendLine("\t\t{");
						builder.AppendLine($"\t\t\tint n = {field}?.Count ?? 0;");
						builder.AppendLine("\t\t\tif (i < n)");
						builder.AppendLine("\t\t\t{");
						builder.AppendLine($"\t\t\t\t{field}![i] = ({s.ElementType})value!;");
						builder.AppendLine("\t\t\t\treturn;");
						builder.AppendLine("\t\t\t}");
						if (!last)
							builder.AppendLine("\t\t\ti -= n;");
						builder.AppendLine("\t\t}");
					}
					else
					{
						builder.AppendLine("\t\tif (i == 0)");
						builder.AppendLine("\t\t{");
						builder.AppendLine($"\t\t\tSetChildNode(ref {field}, ({s.PropertyType}?)value, index);");
						builder.AppendLine("\t\t\treturn;");
						builder.AppendLine("\t\t}");
						if (!last)
							builder.AppendLine("\t\ti--;");
					}
				}
				builder.AppendLine("\t\tthrow new System.ArgumentOutOfRangeException(nameof(index));");
			}
			builder.AppendLine("\t}");
			builder.AppendLine();

			// One typed CSharpSlotInfo<T> static per slot; node.Slot compares against these by object
			// identity, and the typed child accessors infer the child type from the slot.
			foreach (var s in slots)
				builder.AppendLine($"\tpublic static readonly CSharpSlotInfo<{s.ElementType}> {s.PropertyName}Slot = new CSharpSlotInfo<{s.ElementType}>(\"{s.PropertyName}\", {(s.IsCollection ? "true" : "false")}, Slots.{s.KindName}, {(s.IsCollection || s.IsNullable ? "true" : "false")});");
			builder.AppendLine();

			builder.AppendLine("\tinternal override CSharpSlotInfo GetChildSlotInfo(int index)");
			builder.AppendLine("\t{");
			EmitReturnDispatch(k => $"{slots[k].PropertyName}Slot", k => $"{slots[k].PropertyName}Slot");
			builder.AppendLine("\t}");
			builder.AppendLine();

			if (slots.Any(s => s.IsCollection))
			{
				builder.AppendLine("\tinternal override AstNodeCollection? GetCollectionByKind(CSharpSlotInfo kind)");
				builder.AppendLine("\t{");
				foreach (var s in slots.Where(s => s.IsCollection))
					builder.AppendLine($"\t\tif (kind == Slots.{s.KindName}) return {s.PropertyName};");
				builder.AppendLine("\t\treturn base.GetCollectionByKind(kind);");
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
				{
					builder.AppendLine($"\t\tif ({field} != null)");
					builder.AppendLine($"\t\t\tforeach (var c in {field})");
					builder.AppendLine($"\t\t\t\tcopy.{s.PropertyName}.Add(({s.ElementType})c.Clone());");
				}
				else
					builder.AppendLine($"\t\tif ({field} != null) copy.{s.PropertyName} = ({s.PropertyType}){field}.Clone();");
			}
			builder.AppendLine("\t}");
			builder.AppendLine();
		}

		// Close the class, trimming the blank line the per-member spacer leaves before the brace.
		string body = builder.ToString().TrimEnd() + "\n}\n";

		context.AddSource(source.NodeName + ".g.cs", SourceText.From(body.Replace("\r\n", "\n"), Encoding.UTF8));
	}

	void WriteVisitors(SourceProductionContext context, ImmutableArray<AstNodeAdditions> source)
	{
		var builder = new StringBuilder();

		builder.AppendLine("// <auto-generated/>");
		builder.AppendLine("#nullable enable");
		builder.AppendLine();
		builder.AppendLine("namespace ICSharpCode.Decompiler.CSharp.Syntax;");
		builder.AppendLine();

		source = source
			.Concat([new("PatternPlaceholder", NeedsVisitor: true, IsAbstract: false, BaseHasDefaultConstructor: true, NeedsPatternPlaceholder: false, "PatternPlaceholder", "AstNode", null, null, null, null)])
			.ToImmutableArray();

		WriteInterface("IAstVisitor", "void", "");
		builder.AppendLine();
		WriteInterface("IAstVisitor<out S>", "S", "");
		builder.AppendLine();
		WriteInterface("IAstVisitor<in T, out S>", "S", ", T data");

		context.AddSource("IAstVisitor.g.cs", SourceText.From(builder.ToString().Replace("\r\n", "\n"), Encoding.UTF8));

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
		public DecompilerAstNodeAttribute(bool hasPatternPlaceholder = false) { }
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

	// Emits the Slots holder: one typed CSharpSlotInfo<T> constant per distinct slot kind across all
	// nodes. Each is its own canonical kind (constructed with a null Kind); a node's per-node slot points
	// back at it, and consumers compare node.Slot.Kind == Slots.X by object identity -- shared across node
	// types, replacing the old polymorphic node.Role == Roles.X comparisons.
	void WriteSlotKinds(SourceProductionContext context, ImmutableArray<AstNodeAdditions> source)
	{
		// Per kind: the element types seen (to choose a typed slot's T) and whether it is a collection.
		// kindIsCollection is null once a kind is seen as a collection on one node and a single child on
		// another (e.g. Expression, Initializer); it is resolved by agreement, so the emitted flag never
		// depends on iteration order (each kind's value is written exactly once it is determined).
		var kindTypes = new SortedDictionary<string, SortedSet<string>>(StringComparer.Ordinal);
		var kindIsCollection = new Dictionary<string, bool?>();
		foreach (var node in source)
		{
			if (node.Slots is { } slots)
			{
				foreach (var s in slots)
				{
					if (!kindTypes.TryGetValue(s.KindName, out var set))
						kindTypes[s.KindName] = set = new SortedSet<string>(StringComparer.Ordinal);
					set.Add(s.ElementType);
					if (!kindIsCollection.TryGetValue(s.KindName, out var arity))
						kindIsCollection[s.KindName] = s.IsCollection;
					else if (arity is bool b && b != s.IsCollection)
						kindIsCollection[s.KindName] = null;
				}
			}
		}

		// A slot kind names one child position, so it maps to a single child type (DSTG001 enforces this);
		// the typed Slots constant therefore always carries that precise type. A kind that is a collection
		// on one node and a single (or optional) child on another keeps a single type but no single arity,
		// so its hard-coded IsCollection/isOptional flags are not authoritative -- the precise per-position
		// flags live on the per-node slots, which is where consumers read them; the shared constant carries
		// identity (and the now-precise child type), not those flags.
		foreach (var kv in kindTypes)
		{
			if (kv.Value.Count > 1)
				context.ReportDiagnostic(Diagnostic.Create(MultipleChildTypesForKind, Location.None, kv.Key, string.Join(", ", kv.Value)));
		}

		var builder = new StringBuilder();
		builder.AppendLine("// <auto-generated/>");
		builder.AppendLine();
		builder.AppendLine("namespace ICSharpCode.Decompiler.CSharp.Syntax;");
		builder.AppendLine();
		// Each constant is its own canonical kind (null Kind): node.GetChild(Slots.X) infers the child
		// type, and node.Slot.Kind == Slots.X identifies a position polymorphically.
		builder.AppendLine("/// <summary>The shared slot kinds, one per distinct child position across the AST node types.</summary>");
		builder.AppendLine("public static class Slots");
		builder.AppendLine("{");
		foreach (var kv in kindTypes)
		{
			string type = kv.Value.Count == 1 ? kv.Value.Min : "AstNode";
			string isColl = kindIsCollection[kv.Key] == true ? "true" : "false";
			builder.AppendLine($"\tpublic static readonly CSharpSlotInfo<{type}> {kv.Key} = new CSharpSlotInfo<{type}>(\"{kv.Key}\", {isColl}, null, false);");
		}
		builder.AppendLine("}");

		context.AddSource("Slots.g.cs", SourceText.From(builder.ToString().Replace("\r\n", "\n"), Encoding.UTF8));
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
		return this.array.AsSpan().SequenceEqual(other.array.AsSpan());
	}

	public override bool Equals(object obj)
	{
		return obj is EquatableArray<T> other && Equals(other);
	}

	// Content-based hash so it agrees with the content-based Equals; the default struct
	// GetHashCode would hash the array reference and break the records that cache these as
	// incremental-generator keys. (System.HashCode is unavailable on netstandard2.0.)
	public override int GetHashCode()
	{
		if (array == null)
			return 0;
		unchecked
		{
			int hash = 17;
			foreach (T item in array)
				hash = hash * 31 + EqualityComparer<T>.Default.GetHashCode(item);
			return hash;
		}
	}

	public static bool operator ==(EquatableArray<T> left, EquatableArray<T> right) => left.Equals(right);

	public static bool operator !=(EquatableArray<T> left, EquatableArray<T> right) => !left.Equals(right);

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