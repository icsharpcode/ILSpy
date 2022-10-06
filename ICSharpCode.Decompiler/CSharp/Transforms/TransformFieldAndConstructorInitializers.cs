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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Syntax.PatternMatching;
using ICSharpCode.Decompiler.TypeSystem;

using SRM = System.Reflection.Metadata;

namespace ICSharpCode.Decompiler.CSharp.Transforms
{
	/// <summary>
	/// This transform moves field initializers at the start of constructors to their respective field declarations
	/// and transforms this-/base-ctor calls in constructors to constructor initializers.
	/// </summary>
	public class TransformFieldAndConstructorInitializers : DepthFirstAstVisitor, IAstTransform
	{
		TransformContext context;

		public void Run(AstNode node, TransformContext context)
		{
			this.context = context;

			try
			{
				// If we're viewing some set of members (fields are direct children of SyntaxTree),
				// we also need to handle those:
				HandleInstanceFieldInitializers(node.Children);
				HandleStaticFieldInitializers(node.Children);

				node.AcceptVisitor(this);

				RemoveSingleEmptyConstructor(node.Children, context.CurrentTypeDefinition);
			}
			finally
			{
				this.context = null;
			}
		}

		public override void VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration)
		{
			var currentCtor = (IMethod)constructorDeclaration.GetSymbol();
			ConstructorInitializer ci = null;
			if (constructorDeclaration.Body.Statements.FirstOrDefault() is ExpressionStatement stmt)
			{
				switch (stmt.Expression)
				{
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
						if (target is ThisReferenceExpression or BaseReferenceExpression)
						{
							if (ctor.DeclaringTypeDefinition == currentCtor.DeclaringTypeDefinition)
								ci.ConstructorInitializerType = ConstructorInitializerType.This;
							else
								ci.ConstructorInitializerType = ConstructorInitializerType.Base;
						}
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
					default:
						return;
				}
			}
			if (context.DecompileRun.RecordDecompilers.TryGetValue(currentCtor.DeclaringTypeDefinition, out var record)
				&& currentCtor.Equals(record.PrimaryConstructor))
			{
				if (record.IsInheritedRecord &&
					ci?.ConstructorInitializerType == ConstructorInitializerType.Base &&
					constructorDeclaration.Parent is TypeDeclaration { BaseTypes: { Count: >= 1 } } typeDecl)
				{
					var baseType = typeDecl.BaseTypes.First();
					var newBaseType = new InvocationAstType();
					baseType.ReplaceWith(newBaseType);
					newBaseType.BaseType = baseType;
					ci.Arguments.MoveTo(newBaseType.Arguments);
				}
				constructorDeclaration.Remove();
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

		void HandleInstanceFieldInitializers(IEnumerable<AstNode> members)
		{
			var instanceCtors = members.OfType<ConstructorDeclaration>().Where(c => (c.Modifiers & Modifiers.Static) == 0).ToArray();
			var instanceCtorsNotChainingWithThis = instanceCtors.Where(ctor => !thisCallPattern.IsMatch(ctor.Body.Statements.FirstOrDefault())).ToArray();
			if (instanceCtorsNotChainingWithThis.Length > 0)
			{
				var ctorMethodDef = instanceCtorsNotChainingWithThis[0].GetSymbol() as IMethod;
				ITypeDefinition declaringTypeDefinition = ctorMethodDef?.DeclaringTypeDefinition;
				if (ctorMethodDef != null && declaringTypeDefinition?.IsReferenceType == false && !declaringTypeDefinition.IsRecord)
					return;

				bool ctorIsUnsafe = instanceCtorsNotChainingWithThis.All(c => c.HasModifier(Modifiers.Unsafe));

				if (!context.DecompileRun.RecordDecompilers.TryGetValue(ctorMethodDef.DeclaringTypeDefinition, out var record))
					record = null;

				// Filter out copy constructor of records
				if (record != null)
					instanceCtorsNotChainingWithThis = instanceCtorsNotChainingWithThis.Where(ctor => !record.IsCopyConstructor(ctor.GetSymbol() as IMethod)).ToArray();

				// Recognize field or property initializers:
				// Translate first statement in all ctors (if all ctors have the same statement) into an initializer.
				bool allSame;
				bool isStructPrimaryCtor = false;
				do
				{
					Match m = fieldInitializerPattern.Match(instanceCtorsNotChainingWithThis[0].Body.FirstOrDefault());
					if (!m.Success)
						break;
					IMember fieldOrPropertyOrEvent = (m.Get<AstNode>("fieldAccess").Single().GetSymbol() as IMember)?.MemberDefinition;
					if (!(fieldOrPropertyOrEvent is IField) && !(fieldOrPropertyOrEvent is IProperty) && !(fieldOrPropertyOrEvent is IEvent))
						break;
					var fieldOrPropertyOrEventDecl = members.FirstOrDefault(f => f.GetSymbol() == fieldOrPropertyOrEvent) as EntityDeclaration;
					// Cannot transform if it is a custom event.
					if (fieldOrPropertyOrEventDecl is CustomEventDeclaration)
						break;


					Expression initializer = m.Get<Expression>("initializer").Single();
					// 'this'/'base' cannot be used in initializers
					if (initializer.DescendantsAndSelf.Any(n => n is ThisReferenceExpression || n is BaseReferenceExpression))
						break;

					if (initializer.Annotation<ILVariableResolveResult>()?.Variable.Kind == IL.VariableKind.Parameter)
					{
						// remove record ctor parameter assignments
						if (!IsPropertyDeclaredByPrimaryCtor(fieldOrPropertyOrEvent as IProperty, record))
							break;
						isStructPrimaryCtor = true;
					}
					else
					{
						// cannot transform if member is not found
						if (fieldOrPropertyOrEventDecl == null)
							break;
						// or if this is a struct record, but not the primary ctor
						if (declaringTypeDefinition.IsRecord && declaringTypeDefinition.IsReferenceType == false && !isStructPrimaryCtor)
							break;
					}

					allSame = true;
					for (int i = 1; i < instanceCtorsNotChainingWithThis.Length; i++)
					{
						var otherMatch = fieldInitializerPattern.Match(instanceCtorsNotChainingWithThis[i].Body.FirstOrDefault());
						if (!otherMatch.Success)
						{
							allSame = false;
							break;
						}
						var otherMember = (otherMatch.Get<AstNode>("fieldAccess").Single().GetSymbol() as IMember)?.MemberDefinition;
						if (!otherMember.Equals(fieldOrPropertyOrEvent))
							allSame = false;
						if (!initializer.IsMatch(otherMatch.Get<AstNode>("initializer").Single()))
							allSame = false;
					}
					if (allSame)
					{
						foreach (var ctor in instanceCtorsNotChainingWithThis)
							ctor.Body.First().Remove();
						if (fieldOrPropertyOrEventDecl == null)
							continue;
						if (ctorIsUnsafe && IntroduceUnsafeModifier.IsUnsafe(initializer))
						{
							fieldOrPropertyOrEventDecl.Modifiers |= Modifiers.Unsafe;
						}
						if (fieldOrPropertyOrEventDecl is PropertyDeclaration pd)
						{
							pd.Initializer = initializer.Detach();
						}
						else
						{
							fieldOrPropertyOrEventDecl.GetChildrenByRole(Roles.Variable).Single().Initializer = initializer.Detach();
						}
					}
				} while (allSame);
			}
		}

		bool IsPropertyDeclaredByPrimaryCtor(IProperty p, RecordDecompiler record)
		{
			if (p == null || record == null)
				return false;
			return record.IsPropertyDeclaredByPrimaryConstructor(p);
		}

		void RemoveSingleEmptyConstructor(IEnumerable<AstNode> members, ITypeDefinition contextTypeDefinition)
		{
			// if we're outside of a type definition skip this altogether
			if (contextTypeDefinition == null)
				return;
			// first get non-static constructor declarations from the AST
			var instanceCtors = members.OfType<ConstructorDeclaration>().Where(c => (c.Modifiers & Modifiers.Static) == 0).ToArray();
			// if there's exactly one ctor and it's part of a type declaration or there's more than one member in the current selection
			// we can remove the constructor. (We do not want to hide the constructor if the user explicitly selected it in the tree view.) 
			if (instanceCtors.Length == 1 && (instanceCtors[0].Parent is TypeDeclaration || members.Skip(1).Any()))
			{
				var ctor = instanceCtors[0];
				// dynamically create a pattern of an empty ctor
				ConstructorDeclaration emptyCtorPattern = new ConstructorDeclaration();
				emptyCtorPattern.Modifiers = contextTypeDefinition.IsAbstract ? Modifiers.Protected : Modifiers.Public;
				if (ctor.HasModifier(Modifiers.Unsafe))
					emptyCtorPattern.Modifiers |= Modifiers.Unsafe;
				emptyCtorPattern.Body = new BlockStatement();

				if (emptyCtorPattern.IsMatch(ctor))
				{
					bool retainBecauseOfDocumentation = ctor.GetSymbol() is IMethod ctorMethod
						&& context.Settings.ShowXmlDocumentation
						&& context.DecompileRun.DocumentationProvider?.GetDocumentation(ctorMethod) != null;
					if (!retainBecauseOfDocumentation)
						ctor.Remove();
				}
			}
		}

		void HandleStaticFieldInitializers(IEnumerable<AstNode> members)
		{
			// Translate static constructor into field initializers if the class is BeforeFieldInit
			var staticCtor = members.OfType<ConstructorDeclaration>().FirstOrDefault(c => (c.Modifiers & Modifiers.Static) == Modifiers.Static);
			if (staticCtor != null)
			{
				bool ctorIsUnsafe = staticCtor.HasModifier(Modifiers.Unsafe);
				IMethod ctorMethod = staticCtor.GetSymbol() as IMethod;
				if (!ctorMethod.MetadataToken.IsNil)
				{
					var metadata = context.TypeSystem.MainModule.PEFile.Metadata;
					SRM.MethodDefinition ctorMethodDef = metadata.GetMethodDefinition((SRM.MethodDefinitionHandle)ctorMethod.MetadataToken);
					SRM.TypeDefinition declaringType = metadata.GetTypeDefinition(ctorMethodDef.GetDeclaringType());
					bool declaringTypeIsBeforeFieldInit = declaringType.HasFlag(TypeAttributes.BeforeFieldInit);
					int pos = 0;
					while (pos < staticCtor.Body.Statements.Count)
					{
						ExpressionStatement es = staticCtor.Body.Statements.ElementAtOrDefault(pos) as ExpressionStatement;
						if (es == null)
							break;
						AssignmentExpression assignment = es.Expression as AssignmentExpression;
						if (assignment == null || assignment.Operator != AssignmentOperatorType.Assign)
							break;
						IMember fieldOrProperty = (assignment.Left.GetSymbol() as IMember)?.MemberDefinition;
						if (!(fieldOrProperty is IField || fieldOrProperty is IProperty) || !fieldOrProperty.IsStatic)
							break;
						// Only move fields that are constants, if the declaring type is not marked beforefieldinit.
						if (!declaringTypeIsBeforeFieldInit && fieldOrProperty is not IField { IsConst: true })
						{
							pos++;
							continue;
						}
						var fieldOrPropertyDecl = members.FirstOrDefault(f => f.GetSymbol() == fieldOrProperty) as EntityDeclaration;
						if (fieldOrPropertyDecl == null)
							break;
						if (ctorIsUnsafe && IntroduceUnsafeModifier.IsUnsafe(assignment.Right))
						{
							fieldOrPropertyDecl.Modifiers |= Modifiers.Unsafe;
						}
						if (fieldOrPropertyDecl is FieldDeclaration fd)
						{
							var v = fd.Variables.Single();
							if (v.Initializer.IsNull)
							{
								v.Initializer = assignment.Right.Detach();
							}
							else
							{
								var constant = v.Initializer.GetResolveResult();
								var expression = assignment.Right.GetResolveResult();
								if (!(constant.IsCompileTimeConstant &&
									TryEvaluateDecimalConstant(expression, out decimal value) &&
									value.Equals(constant.ConstantValue)))
								{
									// decimal values do not match, abort transformation
									break;
								}
							}
						}
						else if (fieldOrPropertyDecl is PropertyDeclaration pd)
						{
							pd.Initializer = assignment.Right.Detach();
						}
						else
						{
							break;
						}
						es.Remove();
					}
					if (declaringTypeIsBeforeFieldInit && staticCtor.Body.Statements.Count == 0)
					{
						staticCtor.Remove();
					}
				}
			}
		}

		/// <summary>
		/// Evaluates a call to the decimal-ctor.
		/// </summary>
		private bool TryEvaluateDecimalConstant(Semantics.ResolveResult expression, out decimal value)
		{
			value = 0;
			if (!expression.Type.IsKnownType(KnownTypeCode.Decimal))
			{
				return false;
			}
			switch (expression)
			{
				case CSharpInvocationResolveResult rr:
					if (!(rr.GetSymbol() is IMethod { SymbolKind: SymbolKind.Constructor } ctor))
						return false;
					var args = rr.GetArgumentsForCall();
					if (args.Count == 1)
					{
						switch (args[0].ConstantValue)
						{
							case double d:
								value = new decimal(d);
								return true;
							case float f:
								value = new decimal(f);
								return true;
							case long l:
								value = new decimal(l);
								return true;
							case int i:
								value = new decimal(i);
								return true;
							case ulong ul:
								value = new decimal(ul);
								return true;
							case uint ui:
								value = new decimal(ui);
								return true;
							case int[] bits when bits.Length == 4 && (bits[3] & 0x7F00FFFF) == 0 && (bits[3] & 0xFF000000) <= 0x1C000000:
								value = new decimal(bits);
								return true;
							default:
								return false;
						}
					}
					else if (args.Count == 5 &&
						args[0].ConstantValue is int lo &&
						args[1].ConstantValue is int mid &&
						args[2].ConstantValue is int hi &&
						args[3].ConstantValue is bool isNegative &&
						args[4].ConstantValue is byte scale)
					{
						value = new decimal(lo, mid, hi, isNegative, scale);
						return true;
					}
					return false;
				default:
					if (expression.ConstantValue is decimal v)
					{
						value = v;
						return true;
					}
					return false;
			}
		}
	}
}
