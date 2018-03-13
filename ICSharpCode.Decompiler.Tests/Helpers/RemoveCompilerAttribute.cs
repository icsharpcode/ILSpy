using System.Collections.Generic;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.Tests.Helpers
{
	class RemoveCompilerAttribute : DepthFirstAstVisitor<object, object>, IAstTransform
	{
		public override object VisitAttribute(CSharp.Syntax.Attribute attribute, object data)
		{
			var section = (AttributeSection)attribute.Parent;
			var type = attribute.Type as SimpleType;
			if (section.AttributeTarget == "assembly" &&
				(type.Identifier == "CompilationRelaxations" || type.Identifier == "RuntimeCompatibility" || type.Identifier == "SecurityPermission" || type.Identifier == "PermissionSet" || type.Identifier == "AssemblyVersion" || type.Identifier == "Debuggable"))
			{
				attribute.Remove();
				if (section.Attributes.Count == 0)
					section.Remove();
			}
			if (section.AttributeTarget == "module" && type.Identifier == "UnverifiableCode")
			{
				attribute.Remove();
				if (section.Attributes.Count == 0)
					section.Remove();
			}
			return null;
		}

		public void Run(AstNode rootNode, TransformContext context)
		{
			rootNode.AcceptVisitor(this, null);
		}
	}

	public class RemoveEmbeddedAtttributes : DepthFirstAstVisitor<object, object>, IAstTransform
	{
		readonly HashSet<string> attributeNames = new HashSet<string>() {
			"System.Runtime.CompilerServices.IsReadOnlyAttribute",
			"System.Runtime.CompilerServices.IsByRefLikeAttribute",
			"Microsoft.CodeAnalysis.EmbeddedAttribute",
		};

		public override object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			var typeDefinition = typeDeclaration.GetSymbol() as ITypeDefinition;
			if (typeDefinition == null || !attributeNames.Contains(typeDefinition.FullName))
				return null;
			if (typeDeclaration.Parent is NamespaceDeclaration ns && ns.Members.Count == 1)
				ns.Remove();
			else
				typeDeclaration.Remove();
			return null;
		}

		public void Run(AstNode rootNode, TransformContext context)
		{
			rootNode.AcceptVisitor(this, null);
		}
	}
}
