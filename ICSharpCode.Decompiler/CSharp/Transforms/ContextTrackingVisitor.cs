// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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

using System.Diagnostics;

using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.CSharp.Transforms
{
	/// <summary>
	/// Base class for AST visitors that need the current type/method context info.
	/// </summary>
	public abstract class ContextTrackingVisitor<TResult> : DepthFirstAstVisitor<TResult>
	{
		protected ITypeDefinition currentTypeDefinition;
		protected IMethod currentMethod;

		protected void Initialize(TransformContext context)
		{
			currentTypeDefinition = context.CurrentTypeDefinition;
			currentMethod = context.CurrentMember as IMethod;
		}

		protected void Uninitialize()
		{
			currentTypeDefinition = null;
			currentMethod = null;
		}

		public override TResult VisitTypeDeclaration(TypeDeclaration typeDeclaration)
		{
			ITypeDefinition oldType = currentTypeDefinition;
			try
			{
				currentTypeDefinition = typeDeclaration.GetSymbol() as ITypeDefinition;
				return base.VisitTypeDeclaration(typeDeclaration);
			}
			finally
			{
				currentTypeDefinition = oldType;
			}
		}

		public override TResult VisitMethodDeclaration(MethodDeclaration methodDeclaration)
		{
			var oldMethod = currentMethod;
			try
			{
				currentMethod = methodDeclaration.GetSymbol() as IMethod;
				return base.VisitMethodDeclaration(methodDeclaration);
			}
			finally
			{
				currentMethod = oldMethod;
			}
		}

		public override TResult VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration)
		{
			var oldMethod = currentMethod;
			try
			{
				currentMethod = constructorDeclaration.GetSymbol() as IMethod;
				return base.VisitConstructorDeclaration(constructorDeclaration);
			}
			finally
			{
				currentMethod = oldMethod;
			}
		}

		public override TResult VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration)
		{
			var oldMethod = currentMethod;
			try
			{
				currentMethod = destructorDeclaration.GetSymbol() as IMethod;
				return base.VisitDestructorDeclaration(destructorDeclaration);
			}
			finally
			{
				currentMethod = oldMethod;
			}
		}

		public override TResult VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration)
		{
			var oldMethod = currentMethod;
			try
			{
				currentMethod = operatorDeclaration.GetSymbol() as IMethod;
				return base.VisitOperatorDeclaration(operatorDeclaration);
			}
			finally
			{
				currentMethod = oldMethod;
			}
		}

		public override TResult VisitAccessor(Accessor accessor)
		{
			var oldMethod = currentMethod;
			try
			{
				currentMethod = accessor.GetSymbol() as IMethod;
				return base.VisitAccessor(accessor);
			}
			finally
			{
				currentMethod = oldMethod;
			}
		}
	}
}
