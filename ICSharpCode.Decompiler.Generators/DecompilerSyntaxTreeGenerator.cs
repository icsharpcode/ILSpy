using System.Collections;
using System.Collections.Immutable;
using System.Text;

using ICSharpCode.Decompiler.CSharp.Syntax;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace ICSharpCode.Decompiler.Generators;

[Generator]
internal class DecompilerSyntaxTreeGenerator : IIncrementalGenerator
{
	record AstNodeAdditions(string NodeName, bool NeedsAcceptImpls, bool NeedsVisitor, bool NeedsNullNode, bool NeedsPatternPlaceholder, int NullNodeBaseCtorParamCount, bool IsTypeNode, string VisitMethodName, string VisitMethodParamType, EquatableArray<(string Member, string TypeName, bool RecursiveMatch, bool MatchAny)>? MembersToMatch);

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
				if (property.GetAttributes().Any(a => a.AttributeClass?.Name == nameof(ExcludeFromMatchAttribute)))
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

		return new(targetSymbol.Name, !targetSymbol.MemberNames.Contains("AcceptVisitor"),
			NeedsVisitor: !targetSymbol.IsAbstract && targetSymbol.BaseType!.IsAbstract,
			NeedsNullNode: (bool)attribute.ConstructorArguments[0].Value!,
			NeedsPatternPlaceholder: (bool)attribute.ConstructorArguments[1].Value!,
			NullNodeBaseCtorParamCount: targetSymbol.InstanceConstructors.Min(m => m.Parameters.Length),
			IsTypeNode: targetSymbol.Name == "AstType" || targetSymbol.BaseType?.Name == "AstType",
			visitMethodName, paramTypeName, membersToMatch?.ToEquatableArray());
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

			if (source.IsTypeNode)
			{
				builder.AppendLine(
					$@"

		public override Decompiler.TypeSystem.ITypeReference ToTypeReference(Resolver.NameLookupMode lookupMode, Decompiler.TypeSystem.InterningProvider? interningProvider = null)
		{{
			return Decompiler.TypeSystem.SpecialType.UnknownType;
		}}"
				);
			}

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
			const string toTypeReferenceCode = @"

		public override Decompiler.TypeSystem.ITypeReference ToTypeReference(Resolver.NameLookupMode lookupMode, Decompiler.TypeSystem.InterningProvider? interningProvider = null)
		{
			throw new System.NotSupportedException();
		}";

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
		}}{(source.NodeName == "AstType" ? toTypeReferenceCode : "")}
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

		builder.AppendLine("}");

		context.AddSource(source.NodeName + ".g.cs", SourceText.From(builder.ToString(), Encoding.UTF8));
	}

	private void WriteVisitors(SourceProductionContext context, ImmutableArray<AstNodeAdditions> source)
	{
		var builder = new StringBuilder();

		builder.AppendLine("namespace ICSharpCode.Decompiler.CSharp.Syntax;");

		source = source
			.Concat([new("NullNode", false, true, false, false, 0, false, "NullNode", "AstNode", null), new("PatternPlaceholder", false, true, false, false, 0, false, "PatternPlaceholder", "AstNode", null)])
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