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

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Syntax.PatternMatching;
using ICSharpCode.Decompiler.TypeSystem;
using SRM = System.Reflection.Metadata;

namespace ICSharpCode.Decompiler.CSharp.Transforms
{
	/// <summary>
	/// If the first element of a constructor is a chained constructor call, convert it into a constructor initializer.
	/// </summary>
	public class ConvertConstructorCallIntoInitializer : IAstTransform
	{
		public void Run(AstNode node, TransformContext context)
		{
			var visitor = new ConvertConstructorCallIntoInitializerVisitor(context);
			
			// If we're viewing some set of members (fields are direct children of SyntaxTree),
			// we also need to handle those:
			visitor.HandleInstanceFieldInitializers(node.Children);
			visitor.HandleStaticFieldInitializers(node.Children);
			
			node.AcceptVisitor(visitor);

			visitor.RemoveSingleEmptyConstructor(node.Children, context.CurrentTypeDefinition);
		}
	}
	
	sealed class ConvertConstructorCallIntoInitializerVisitor : DepthFirstAstVisitor
	{
		readonly TransformContext context;
		
		public ConvertConstructorCallIntoInitializerVisitor(TransformContext context)
		{
			Debug.Assert(context != null);
			this.context = context;
		}
		
		public override void VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration)
		{
			if (!(constructorDeclaration.Body.Statements.FirstOrDefault() is ExpressionStatement stmt))
				return;
			var currentCtor = (IMethod)constructorDeclaration.GetSymbol();
			ConstructorInitializer ci;
			switch (stmt.Expression) {
				// Pattern for reference types:
				// this..ctor(...);
				case InvocationExpression invocation:
					if (!(invocation.Target is MemberReferenceExpression mre) || mre.MemberName != ".ctor")
						return;
					if (!(invocation.GetSymbol() is IMethod ctor && ctor.IsConstructor))
						return;
					ci = new ConstructorInitializer();
					var target = mre.Target;
					// Ignore casts, those might be added if references are missing.
					if (target is CastExpression cast)
						target = cast.Expression;
					if (target is ThisReferenceExpression)
						ci.ConstructorInitializerType = ConstructorInitializerType.This;
					else if (target is BaseReferenceExpression)
						ci.ConstructorInitializerType = ConstructorInitializerType.Base;
					else
						return;
					// Move arguments from invocation to initializer:
					invocation.Arguments.MoveTo(ci.Arguments);
					// Add the initializer: (unless it is the default 'base()')
					if (!(ci.ConstructorInitializerType == ConstructorInitializerType.Base && ci.Arguments.Count == 0))
						constructorDeclaration.Initializer = ci.CopyAnnotationsFrom(invocation);
					// Remove the statement:
					stmt.Remove();
					break;
				// Pattern for value types:
				// this = new TSelf(...);
				case AssignmentExpression assignment:
					if (!(assignment.Right is ObjectCreateExpression oce && oce.GetSymbol() is IMethod ctor2 && ctor2.DeclaringTypeDefinition == currentCtor.DeclaringTypeDefinition))
						return;
					ci = new ConstructorInitializer();
					if (assignment.Left is ThisReferenceExpression)
						ci.ConstructorInitializerType = ConstructorInitializerType.This;
					else
						return;
					// Move arguments from invocation to initializer:
					oce.Arguments.MoveTo(ci.Arguments);
					// Add the initializer: (unless it is the default 'base()')
					if (!(ci.ConstructorInitializerType == ConstructorInitializerType.Base && ci.Arguments.Count == 0))
						constructorDeclaration.Initializer = ci.CopyAnnotationsFrom(oce);
					// Remove the statement:
					stmt.Remove();
					break;
			}
		}
		
		static readonly ExpressionStatement fieldInitializerPattern = new ExpressionStatement {
			Expression = new AssignmentExpression {
				Left = new Choice {
					new NamedNode("fieldAccess", new MemberReferenceExpression {
										 Target = new ThisReferenceExpression(),
										 MemberName = Pattern.AnyString
									 }),
					new NamedNode("fieldAccess", new IdentifierExpression(Pattern.AnyString))
				},
				Operator = AssignmentOperatorType.Assign,
				Right = new AnyNode("initializer")
			}
		};
		
		static readonly AstNode thisCallPattern = new ExpressionStatement(new InvocationExpression(new MemberReferenceExpression(new ThisReferenceExpression(), ".ctor"), new Repeat(new AnyNode())));
		
		public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
		{
			// Handle initializers on instance fields
			HandleInstanceFieldInitializers(typeDeclaration.Members);
			
			// Now convert base constructor calls to initializers:
			base.VisitTypeDeclaration(typeDeclaration);
			
			// Remove single empty constructor:
			RemoveSingleEmptyConstructor(typeDeclaration.Members, (ITypeDefinition)typeDeclaration.GetSymbol());
			
			// Handle initializers on static fields:
			HandleStaticFieldInitializers(typeDeclaration.Members);
		}
		
		internal void HandleInstanceFieldInitializers(IEnumerable<AstNode> members)
		{
			var instanceCtors = members.OfType<ConstructorDeclaration>().Where(c => (c.Modifiers & Modifiers.Static) == 0).ToArray();
			var instanceCtorsNotChainingWithThis = instanceCtors.Where(ctor => !thisCallPattern.IsMatch(ctor.Body.Statements.FirstOrDefault())).ToArray();
			if (instanceCtorsNotChainingWithThis.Length > 0) {
				var ctorMethodDef = instanceCtorsNotChainingWithThis[0].GetSymbol() as IMethod;
				if (ctorMethodDef != null && ctorMethodDef.DeclaringType.IsReferenceType == false)
					return;

				bool ctorIsUnsafe = instanceCtorsNotChainingWithThis.All(c => c.HasModifier(Modifiers.Unsafe));
				
				// Recognize field or property initializers:
				// Translate first statement in all ctors (if all ctors have the same statement) into an initializer.
				bool allSame;
				do {
					Match m = fieldInitializerPattern.Match(instanceCtorsNotChainingWithThis[0].Body.FirstOrDefault());
					if (!m.Success)
						break;
					IMember fieldOrPropertyOrEvent = (m.Get<AstNode>("fieldAccess").Single().GetSymbol() as IMember)?.MemberDefinition;
					if (!(fieldOrPropertyOrEvent is IField) && !(fieldOrPropertyOrEvent is IProperty) && !(fieldOrPropertyOrEvent is IEvent))
						break;
					var fieldOrPropertyOrEventDecl = members.FirstOrDefault(f => f.GetSymbol() == fieldOrPropertyOrEvent) as EntityDeclaration;
					// Cannot transform if member is not found or if it is a custom event.
					if (fieldOrPropertyOrEventDecl == null || fieldOrPropertyOrEventDecl is CustomEventDeclaration)
						break;
					Expression initializer = m.Get<Expression>("initializer").Single();
					// 'this'/'base' cannot be used in initializers
					if (initializer.DescendantsAndSelf.Any(n => n is ThisReferenceExpression || n is BaseReferenceExpression))
						break;
					
					allSame = true;
					for (int i = 1; i < instanceCtorsNotChainingWithThis.Length; i++) {
						var otherMatch = fieldInitializerPattern.Match(instanceCtorsNotChainingWithThis[i].Body.FirstOrDefault());
						if (!otherMatch.Success) {
							allSame = false;
							break;
						}
						var otherMember = (otherMatch.Get<AstNode>("fieldAccess").Single().GetSymbol() as IMember)?.MemberDefinition;
						if (!otherMember.Equals(fieldOrPropertyOrEvent))
							allSame = false;
						if (!initializer.IsMatch(otherMatch.Get<AstNode>("initializer").Single()))
							allSame = false;
					}
					if (allSame) {
						foreach (var ctor in instanceCtorsNotChainingWithThis)
							ctor.Body.First().Remove();
						if (ctorIsUnsafe && IntroduceUnsafeModifier.IsUnsafe(initializer)) {
							fieldOrPropertyOrEventDecl.Modifiers |= Modifiers.Unsafe;
						}
						if (fieldOrPropertyOrEventDecl is PropertyDeclaration pd) {
							pd.Initializer = initializer.Detach();
						} else {
							fieldOrPropertyOrEventDecl.GetChildrenByRole(Roles.Variable).Single().Initializer = initializer.Detach();
						}
					}
				} while (allSame);
			}
		}

		internal void RemoveSingleEmptyConstructor(IEnumerable<AstNode> members, ITypeDefinition contextTypeDefinition)
		{
			if (contextTypeDefinition == null) return;
			var instanceCtors = members.OfType<ConstructorDeclaration>().Where(c => (c.Modifiers & Modifiers.Static) == 0).ToArray();
			if (instanceCtors.Length == 1 && (members.Skip(1).Any() || instanceCtors[0].Parent is TypeDeclaration)) {
				ConstructorDeclaration emptyCtor = new ConstructorDeclaration();
				emptyCtor.Modifiers = contextTypeDefinition.IsAbstract ? Modifiers.Protected : Modifiers.Public;
				if (instanceCtors[0].HasModifier(Modifiers.Unsafe))
					emptyCtor.Modifiers |= Modifiers.Unsafe;
				emptyCtor.Body = new BlockStatement();
				if (emptyCtor.IsMatch(instanceCtors[0]))
					instanceCtors[0].Remove();
			}
		}
		
		internal void HandleStaticFieldInitializers(IEnumerable<AstNode> members)
		{
			// Translate static constructor into field initializers if the class is BeforeFieldInit
			var staticCtor = members.OfType<ConstructorDeclaration>().FirstOrDefault(c => (c.Modifiers & Modifiers.Static) == Modifiers.Static);
			if (staticCtor != null) {
				bool ctorIsUnsafe = staticCtor.HasModifier(Modifiers.Unsafe);
				IMethod ctorMethod = staticCtor.GetSymbol() as IMethod;
				if (!ctorMethod.MetadataToken.IsNil) {
					var metadata = context.TypeSystem.MainModule.PEFile.Metadata;
					SRM.MethodDefinition ctorMethodDef = metadata.GetMethodDefinition((SRM.MethodDefinitionHandle)ctorMethod.MetadataToken);
					SRM.TypeDefinition declaringType = metadata.GetTypeDefinition(ctorMethodDef.GetDeclaringType());
					if (declaringType.HasFlag(System.Reflection.TypeAttributes.BeforeFieldInit)) {
						while (true) {
							ExpressionStatement es = staticCtor.Body.Statements.FirstOrDefault() as ExpressionStatement;
							if (es == null)
								break;
							AssignmentExpression assignment = es.Expression as AssignmentExpression;
							if (assignment == null || assignment.Operator != AssignmentOperatorType.Assign)
								break;
							IMember fieldOrProperty = (assignment.Left.GetSymbol() as IMember)?.MemberDefinition;
							if (!(fieldOrProperty is IField || fieldOrProperty is IProperty) || !fieldOrProperty.IsStatic)
								break;
							var fieldOrPropertyDecl = members.FirstOrDefault(f => f.GetSymbol() == fieldOrProperty) as EntityDeclaration;
							if (fieldOrPropertyDecl == null)
								break;
							if (ctorIsUnsafe && IntroduceUnsafeModifier.IsUnsafe(assignment.Right)) {
								fieldOrPropertyDecl.Modifiers |= Modifiers.Unsafe;
							}
							if (fieldOrPropertyDecl is FieldDeclaration fd)
								fd.Variables.Single().Initializer = assignment.Right.Detach();
							else if (fieldOrPropertyDecl is PropertyDeclaration pd)
								pd.Initializer = assignment.Right.Detach();
							else
								break;
							es.Remove();
						}
						if (staticCtor.Body.Statements.Count == 0)
							staticCtor.Remove();
					}
				}
			}
		}
	}
}
