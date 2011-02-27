using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Decompiler.Transforms;
using ICSharpCode.NRefactory.CSharp;

namespace ICSharpCode.Decompiler.Tests.Helpers
{
	class RemoveRedundantBaseConstructorInitializers : DepthFirstAstVisitor<object, object>, IAstTransform
	{
		public override object VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data)
		{
			System.Diagnostics.Debug.WriteLine("constructor");
			return base.VisitConstructorDeclaration(constructorDeclaration, data);
		}

		public override object VisitConstructorInitializer(ConstructorInitializer constructorInitializer, object data)
		{
			System.Diagnostics.Debug.WriteLine(constructorInitializer.ConstructorInitializerType.ToString() + " " + constructorInitializer.Arguments.Count);
			if (constructorInitializer.ConstructorInitializerType == ConstructorInitializerType.Base &&
				constructorInitializer.Arguments.Count == 0)
			{
				constructorInitializer.Remove();
			}
			return null;
		}

		public void Run(AstNode node)
		{
			node.AcceptVisitor(this, null);
		}
	}
}
