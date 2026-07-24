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

#nullable enable

using System.Diagnostics.CodeAnalysis;

using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.CSharp.Transforms
{
	/// <summary>
	/// Replaces "default(T)" with the default literal "default" (C# 7.1) where the target type
	/// of the expression is explicit in the surrounding syntax and identical to T, so the
	/// shorter form cannot change semantics: initializers of variable and field declarations
	/// with an explicit type, right-hand sides of simple assignments, and return statements.
	/// Arguments and operands are left alone, because there the default literal participates
	/// in overload resolution, operator resolution and type inference.
	/// </summary>
	/// <remarks>
	/// Must run after DeclareVariables and TransformFieldAndConstructorInitializers, which
	/// produce the declaration initializers this transform inspects.
	/// </remarks>
	class IntroduceDefaultLiterals : DepthFirstAstVisitor, IAstTransform
	{
		[AllowNull]
		TransformContext context;
		[AllowNull]
		CSharpConversions conversions;

		public void Run(AstNode rootNode, TransformContext context)
		{
			if (!context.Settings.DefaultLiterals)
				return;
			this.context = context;
			this.conversions = CSharpConversions.Get(context.TypeSystem);
			rootNode.AcceptVisitor(this);
		}

		public override void VisitDefaultValueExpression(DefaultValueExpression defaultValueExpression)
		{
			base.VisitDefaultValueExpression(defaultValueExpression);
			if (defaultValueExpression.Type is null)
				return;
			IType type = defaultValueExpression.Type.GetResolveResult().Type;
			if (type.Kind is TypeKind.Unknown or TypeKind.None)
				return;
			IType? targetType = GetTargetType(defaultValueExpression);
			if (targetType == null)
				return;
			// Only an identity conversion guarantees that the value is unchanged:
			// e.g. in "object o = default(SomeStruct);" the struct is boxed (non-null),
			// while "object o = default;" would be null.
			if (!conversions.IdentityConversion(type, targetType))
				return;
			context.Step("Replace default(" + type.Name + ") with default literal", defaultValueExpression);
			defaultValueExpression.Type = null;
		}

		IType? GetTargetType(DefaultValueExpression defaultValueExpression)
		{
			switch (defaultValueExpression.Parent)
			{
				case VariableInitializer { Parent: VariableDeclarationStatement declaration }:
					if (declaration.Type is SimpleType { Identifier: "var" })
						return null;
					return declaration.Type.GetResolveResult().Type;
				case VariableInitializer { Parent: FieldDeclaration field }:
					return field.ReturnType.GetResolveResult().Type;
				case AssignmentExpression { Operator: AssignmentOperatorType.Assign } assignment when assignment.Right == defaultValueExpression:
					return assignment.Left.GetResolveResult().Type;
				case ReturnStatement returnStatement:
					return GetEnclosingReturnType(returnStatement);
				default:
					return null;
			}
		}

		static IType? GetEnclosingReturnType(ReturnStatement returnStatement)
		{
			for (AstNode? node = returnStatement.Parent; node != null; node = node.Parent)
			{
				switch (node)
				{
					case LambdaExpression:
					case AnonymousMethodExpression:
						// The return type of an anonymous function is inferred from its body,
						// so replacing default(T) could change the function's type.
						return null;
					case EntityDeclaration entity:
						if (entity.GetSymbol() is not IMethod method)
							return null;
						IType returnType = method.ReturnType;
						if ((entity.Modifiers & Modifiers.Async) != 0)
						{
							if (returnType.TypeParameterCount == 1
								&& (TaskType.IsTask(returnType) || TaskType.IsCustomTask(returnType, out _)))
							{
								returnType = returnType.TypeArguments[0];
							}
							else
							{
								return null;
							}
						}
						return returnType;
				}
			}
			return null;
		}
	}
}
