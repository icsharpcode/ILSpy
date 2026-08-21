// Copyright (c) 2026 Dr. Masroor Ehsan

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.AI.Decompiler
{
	/// <summary>Rewrites annotated entity names in the generated AST without modifying the assembly.</summary>
	public sealed class RenameAnnotationTransform : IAstTransform
	{
		readonly RenameAnnotationManager manager;

		public RenameAnnotationTransform(RenameAnnotationManager manager)
		{
			this.manager = manager;
		}

		public void Run(AstNode rootNode, TransformContext context)
		{
			foreach (AstNode node in rootNode.DescendantsAndSelf)
			{
				if (node.GetSymbol() is not IEntity entity || manager.GetRename(entity) is not { } name)
					continue;

				switch (node)
				{
					case EntityDeclaration declaration when declaration.Name != name:
						declaration.Name = name;
						break;
					case MemberReferenceExpression memberReference when memberReference.MemberName != name:
						memberReference.MemberName = name;
						break;
					case IdentifierExpression identifier when identifier.Identifier != name:
						identifier.Identifier = name;
						break;
					case SimpleType simpleType when simpleType.Identifier != name:
						simpleType.Identifier = name;
						break;
				}
			}
		}
	}
}
