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
using System.Diagnostics;
using System.Linq;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Syntax.PatternMatching;
using ICSharpCode.Decompiler.CSharp.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.CSharp.Analysis;
using Mono.Cecil;

namespace ICSharpCode.Decompiler.CSharp.Transforms
{
	/// <summary>
	/// Finds the expanded form of using statements using pattern matching and replaces it with a UsingStatement.
	/// </summary>
	public sealed class PatternStatementTransform : ContextTrackingVisitor<AstNode>, IAstTransform
	{
		TransformContext context;
		
		public void Run(AstNode rootNode, TransformContext context)
		{
			this.context = context;
			base.Initialize(context);
			rootNode.AcceptVisitor(this);
		}

		#region Visitor Overrides
		protected override AstNode VisitChildren(AstNode node)
		{
			// Go through the children, and keep visiting a node as long as it changes.
			// Because some transforms delete/replace nodes before and after the node being transformed, we rely
			// on the transform's return value to know where we need to keep iterating.
			for (AstNode child = node.FirstChild; child != null; child = child.NextSibling) {
				AstNode oldChild;
				do {
					oldChild = child;
					child = child.AcceptVisitor(this);
					Debug.Assert(child != null && child.Parent == node);
				} while (child != oldChild);
			}
			return node;
		}
		
		public override AstNode VisitExpressionStatement(ExpressionStatement expressionStatement)
		{
			AstNode result;
			if (context.Settings.UsingStatement)
			{
				result = TransformNonGenericForEach(expressionStatement);
				if (result != null)
					return result;
				result = TransformUsings(expressionStatement);
				if (result != null)
					return result;
			}
			result = TransformFor(expressionStatement);
			if (result != null)
				return result;
			if (context.Settings.LockStatement) {
				result = TransformLock(expressionStatement);
				if (result != null)
					return result;
			}
			return base.VisitExpressionStatement(expressionStatement);
		}
		
		public override AstNode VisitUsingStatement(UsingStatement usingStatement)
		{
			if (context.Settings.ForEachStatement) {
				AstNode result = TransformForeach(usingStatement);
				if (result != null)
					return result;
			}
			return base.VisitUsingStatement(usingStatement);
		}
		
		public override AstNode VisitWhileStatement(WhileStatement whileStatement)
		{
			return TransformDoWhile(whileStatement) ?? base.VisitWhileStatement(whileStatement);
		}
		
		public override AstNode VisitIfElseStatement(IfElseStatement ifElseStatement)
		{
			if (context.Settings.SwitchStatementOnString) {
				AstNode result = TransformSwitchOnString(ifElseStatement);
				if (result != null)
					return result;
			}
			AstNode simplifiedIfElse = SimplifyCascadingIfElseStatements(ifElseStatement);
			if (simplifiedIfElse != null)
				return simplifiedIfElse;
			return base.VisitIfElseStatement(ifElseStatement);
		}
		
		public override AstNode VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration)
		{
			if (context.Settings.AutomaticProperties) {
				AstNode result = TransformAutomaticProperties(propertyDeclaration);
				if (result != null)
					return result;
			}
			return base.VisitPropertyDeclaration(propertyDeclaration);
		}
		
		public override AstNode VisitCustomEventDeclaration(CustomEventDeclaration eventDeclaration)
		{
			// first apply transforms to the accessor bodies
			base.VisitCustomEventDeclaration(eventDeclaration);
			if (context.Settings.AutomaticEvents) {
				AstNode result = TransformAutomaticEvents(eventDeclaration);
				if (result != null)
					return result;
			}
			return eventDeclaration;
		}
		
		public override AstNode VisitMethodDeclaration(MethodDeclaration methodDeclaration)
		{
			return TransformDestructor(methodDeclaration) ?? base.VisitMethodDeclaration(methodDeclaration);
		}
		
		public override AstNode VisitTryCatchStatement(TryCatchStatement tryCatchStatement)
		{
			return TransformTryCatchFinally(tryCatchStatement) ?? base.VisitTryCatchStatement(tryCatchStatement);
		}
		#endregion
		
		/// <summary>
		/// $variable = $initializer;
		/// </summary>
		static readonly AstNode variableAssignPattern = new ExpressionStatement(
			new AssignmentExpression(
				new NamedNode("variable", new IdentifierExpression(Pattern.AnyString)),
				new AnyNode("initializer")
			));
		
		#region using
		static Expression InvokeDispose(Expression identifier)
		{
			return new Choice {
				new InvocationExpression(new MemberReferenceExpression(identifier, "Dispose")),
				new InvocationExpression(new MemberReferenceExpression(new CastExpression(new TypePattern(typeof(IDisposable)), identifier.Clone()), "Dispose"))
			};
		}
		
		static readonly AstNode usingTryCatchPattern = new Choice {
			{ "c#/vb",
			new TryCatchStatement {
			TryBlock = new AnyNode(),
			FinallyBlock = new BlockStatement {
				new Choice {
					{ "valueType",
						new ExpressionStatement(InvokeDispose(new NamedNode("ident", new IdentifierExpression(Pattern.AnyString))))
					},
					{ "referenceType",
						new IfElseStatement {
							Condition = new BinaryOperatorExpression(
								new NamedNode("ident", new IdentifierExpression(Pattern.AnyString)),
								BinaryOperatorType.InEquality,
								new NullReferenceExpression()
							),
							TrueStatement = new BlockStatement {
								new ExpressionStatement(InvokeDispose(new Backreference("ident")))
							}
						}
					}
				}.ToStatement()
			}
		}
		},
		{ "f#",
			new TryCatchStatement {
			TryBlock = new AnyNode(),
			FinallyBlock =
					new BlockStatement {
						new ExpressionStatement(
							new AssignmentExpression(left: new NamedNode("disposable", new IdentifierExpression(Pattern.AnyString)),
														right: new AsExpression(expression: new NamedNode("ident", new IdentifierExpression(Pattern.AnyString)),
																				type: new TypePattern(typeof(IDisposable))
																				)
							)
						),
						new IfElseStatement {
							Condition = new BinaryOperatorExpression(
								new Backreference("disposable"),
								BinaryOperatorType.InEquality,
								new NullReferenceExpression()
							),
							TrueStatement = new BlockStatement {
								new ExpressionStatement(InvokeDispose(new Backreference("disposable")))
							}
						}
					}
				}
			}
		};
		
		public UsingStatement TransformUsings(ExpressionStatement node)
		{
			// Conditions:
			// 1. CaptureScope of the resource-variable must be either null or the BlockContainer of the using block.
			// 2. The variable must not be used outside of the block.
			// 3. The variable is only assigned once, right before the try-block.
			Match m1 = variableAssignPattern.Match(node);
			if (!m1.Success) return null;
			TryCatchStatement tryCatch = node.NextSibling as TryCatchStatement;
			Match m2 = usingTryCatchPattern.Match(tryCatch);
			if (!m2.Success) return null;
			IL.ILVariable variable = m1.Get<IdentifierExpression>("variable").Single().GetILVariable();
			string variableName = m1.Get<IdentifierExpression>("variable").Single().Identifier;
			if (variable == null || variableName != m2.Get<IdentifierExpression>("ident").Single().Identifier)
				return null;
			if (m2.Has("valueType")) {
				// if there's no if(x!=null), then it must be a value type
				if (variable.Type.IsReferenceType != false)
					return null;
			}

			if (variable.StoreCount > 1 || !variable.Type.GetAllBaseTypes().Any(t => t.IsKnownType(KnownTypeCode.IDisposable)))
				return null;

			//if (m2.Has("f#")) {
			//	string variableNameDisposable = m2.Get<IdentifierExpression>("disposable").Single().Identifier;
			//	VariableDeclarationStatement varDeclDisposable = FindVariableDeclaration(node, variableNameDisposable);
			//	if (varDeclDisposable == null || !(varDeclDisposable.Parent is BlockStatement))
			//		return null;

			//	// Validate that the variable is not used after the using statement:
			//	if (!IsVariableValueUnused(varDeclDisposable, tryCatch))
			//		return null;
			//}

			node.Remove();

			UsingStatement usingStatement = new UsingStatement();
			usingStatement.EmbeddedStatement = tryCatch.TryBlock.Detach();
			tryCatch.ReplaceWith(usingStatement);
			
			// If possible, we'll eliminate the variable completely:
			if (usingStatement.EmbeddedStatement.Descendants.OfType<IdentifierExpression>().Any(ident => ident.Identifier == variableName)) {
				// variable is used, so we'll create a variable declaration
				variable.Kind = IL.VariableKind.UsingLocal;
				usingStatement.ResourceAcquisition = new VariableDeclarationStatement {
					Type = variable.Type.ContainsAnonymousType() ? new SimpleType("var") : context.TypeSystemAstBuilder.ConvertType(variable.Type),
					Variables = {
						new VariableInitializer {
							Name = variableName,
							Initializer = m1.Get<Expression>("initializer").Single().Detach()
						}.CopyAnnotationsFrom(node.Expression)
						 .WithILVariable(variable)
					}
				}.CopyAnnotationsFrom(node);
			} else {
				// the variable is never used; eliminate it:
				usingStatement.ResourceAcquisition = m1.Get<Expression>("initializer").Single().Detach();
			}
			return usingStatement;
		}
		
		internal static VariableDeclarationStatement FindVariableDeclaration(AstNode node, string identifier)
		{
			while (node != null) {
				while (node.PrevSibling != null) {
					node = node.PrevSibling;
					VariableDeclarationStatement varDecl = node as VariableDeclarationStatement;
					if (varDecl != null && varDecl.Variables.Count == 1 && varDecl.Variables.Single().Name == identifier) {
						return varDecl;
					}
				}
				node = node.Parent;
			}
			return null;
		}
		
		/// <summary>
		/// Gets whether the old variable value (assigned inside 'targetStatement' or earlier)
		/// is read anywhere in the remaining scope of the variable declaration.
		/// </summary>
		bool IsVariableValueUnused(VariableDeclarationStatement varDecl, Statement targetStatement)
		{
			Debug.Assert(targetStatement.Ancestors.Contains(varDecl.Parent));
			BlockStatement block = (BlockStatement)varDecl.Parent;
			DefiniteAssignmentAnalysis daa = CreateDAA(block);
			daa.SetAnalyzedRange(targetStatement, block, startInclusive: false);
			daa.Analyze(varDecl.Variables.Single().Name);
			return daa.UnassignedVariableUses.Count == 0;
		}
		
		// I used this in the first implementation of the using-statement transform, but now no longer
		// because there were problems when multiple using statements were using the same variable
		// - no single using statement could be transformed without making the C# code invalid,
		// but transforming both would work.
		// We now use 'IsVariableValueUnused' which will perform the transform
		// even if it results in two variables with the same name and overlapping scopes.
		// (this issue could be fixed later by renaming one of the variables)
		
		private DefiniteAssignmentAnalysis CreateDAA(BlockStatement block)
		{
			var typeResolveContext = new CSharpTypeResolveContext(context.TypeSystem.MainAssembly);
			return new DefiniteAssignmentAnalysis(block, (node, ct) => node.GetResolveResult(), typeResolveContext, context.CancellationToken);
		}

		/// <summary>
		/// Gets whether there is an assignment to 'variableName' anywhere within the given node.
		/// </summary>
		bool HasAssignment(AstNode root, string variableName)
		{
			foreach (AstNode node in root.DescendantsAndSelf) {
				IdentifierExpression ident = node as IdentifierExpression;
				if (ident != null && ident.Identifier == variableName) {
					if (ident.Parent is AssignmentExpression && ident.Role == AssignmentExpression.LeftRole
					    || ident.Parent is DirectionExpression)
					{
						return true;
					}
				}
			}
			return false;
		}
		#endregion
		
		#region foreach (generic)
		static readonly UsingStatement genericForeachPattern = new UsingStatement {
			ResourceAcquisition = new VariableDeclarationStatement {
				Type = new AnyNode("enumeratorType"),
				Variables = {
					new NamedNode(
						"enumeratorVariable",
						new VariableInitializer {
							Name = Pattern.AnyString,
							Initializer = new InvocationExpression(new MemberReferenceExpression(new AnyNode("collection").ToExpression(), "GetEnumerator"))
						}
					)
				}
			},
			EmbeddedStatement = new BlockStatement {
				new Repeat(
					new VariableDeclarationStatement { Type = new AnyNode(), Variables = { new VariableInitializer(Pattern.AnyString) } }.WithName("variablesOutsideLoop")
				).ToStatement(),
				new WhileStatement {
					Condition = new InvocationExpression(new MemberReferenceExpression(new IdentifierExpressionBackreference("enumeratorVariable").ToExpression(), "MoveNext")),
					EmbeddedStatement = new BlockStatement {
						new Repeat(
							new VariableDeclarationStatement { 
								Type = new AnyNode(), 
								Variables = { new VariableInitializer(Pattern.AnyString) }
							}.WithName("variablesInsideLoop")
						).ToStatement(),
						new AssignmentExpression {
							Left = new IdentifierExpression(Pattern.AnyString).WithName("itemVariable"),
							Operator = AssignmentOperatorType.Assign,
							Right = new MemberReferenceExpression(new IdentifierExpressionBackreference("enumeratorVariable").ToExpression(), "Current")
						},
						new Repeat(new AnyNode("statement")).ToStatement()
					}
				}.WithName("loop")
			}};
		
		public ForeachStatement TransformForeach(UsingStatement node)
		{
			Match m = genericForeachPattern.Match(node);
			if (!m.Success)
				return null;
			if (!(node.Parent is BlockStatement) && m.Has("variablesOutsideLoop")) {
				// if there are variables outside the loop, we need to put those into the parent block, and that won't work if the direct parent isn't a block
				return null;
			}
			VariableInitializer enumeratorVar = m.Get<VariableInitializer>("enumeratorVariable").Single();
			var itemVar = m.Get<IdentifierExpression>("itemVariable").Single().GetILVariable();
			WhileStatement loop = m.Get<WhileStatement>("loop").Single();

			if (!VariableCanBeDeclaredInLoop(itemVar, loop)) {
				return null;
			}

			// Make sure that the enumerator variable is not used inside the body
			var enumeratorId = Identifier.Create(enumeratorVar.Name);
			foreach (Statement stmt in m.Get<Statement>("statement")) {
				if (stmt.Descendants.OfType<Identifier>().Any(id => enumeratorId.IsMatch(id)))
					return null;
			}
			
			BlockStatement newBody = new BlockStatement();
			foreach (Statement stmt in m.Get<Statement>("variablesInsideLoop"))
				newBody.Add(stmt.Detach());
			foreach (Statement stmt in m.Get<Statement>("statement"))
				newBody.Add(stmt.Detach());

			itemVar.Kind = IL.VariableKind.ForeachLocal;
			ForeachStatement foreachStatement = new ForeachStatement {
				VariableType = itemVar.Type.ContainsAnonymousType() ? new SimpleType("var") : context.TypeSystemAstBuilder.ConvertType(itemVar.Type),
				VariableName = itemVar.Name,
				InExpression = m.Get<Expression>("collection").Single().Detach(),
				EmbeddedStatement = newBody
			}.WithILVariable(itemVar);
			foreachStatement.CopyAnnotationsFrom(loop);
			if (foreachStatement.InExpression is BaseReferenceExpression) {
				foreachStatement.InExpression = new ThisReferenceExpression().CopyAnnotationsFrom(foreachStatement.InExpression);
			}
			node.ReplaceWith(foreachStatement);
			foreach (Statement stmt in m.Get<Statement>("variablesOutsideLoop")) {
				((BlockStatement)foreachStatement.Parent).Statements.InsertAfter(null, stmt.Detach());
			}
			return foreachStatement;
		}

		static bool VariableCanBeDeclaredInLoop(IL.ILVariable itemVar, WhileStatement loop)
		{
			if (itemVar == null || !(itemVar.Kind == IL.VariableKind.Local || itemVar.Kind == IL.VariableKind.StackSlot)) {
				// only locals/temporaries can be converted into foreach loop variable
				return false;
			}

			if (!itemVar.IsSingleDefinition) {
				// foreach variable cannot be assigned to
				return false;
			}

			if (itemVar.CaptureScope != null && itemVar.CaptureScope != loop.Annotation<IL.BlockContainer>()) {
				// captured variables cannot be declared in the loop unless the loop is their capture scope
				return false;
			}
			return true;
		}
		#endregion
		
		#region foreach (non-generic)
		ExpressionStatement getEnumeratorPattern = new ExpressionStatement(
			new AssignmentExpression(
				new NamedNode("left", new IdentifierExpression(Pattern.AnyString)),
				new InvocationExpression(new MemberReferenceExpression(new AnyNode("collection").ToExpression(), "GetEnumerator"))
			));
		
		TryCatchStatement nonGenericForeachPattern = new TryCatchStatement {
			TryBlock = new BlockStatement {
				new WhileStatement {
					Condition = new InvocationExpression(new MemberReferenceExpression(new IdentifierExpression(Pattern.AnyString).WithName("enumerator"), "MoveNext")),
					EmbeddedStatement = new BlockStatement {
						new AssignmentExpression(
							new IdentifierExpression(Pattern.AnyString).WithName("itemVar"),
							new Choice {
								new MemberReferenceExpression(new Backreference("enumerator").ToExpression(), "Current"),
								new CastExpression {
									Type = new AnyNode("castType"),
									Expression = new MemberReferenceExpression(new Backreference("enumerator").ToExpression(), "Current")
								}
							}
						),
						new Repeat(new AnyNode("stmt")).ToStatement()
					}
				}.WithName("loop")
			},
			FinallyBlock = new BlockStatement {
				new AssignmentExpression(
					new IdentifierExpression(Pattern.AnyString).WithName("disposable"),
					new AsExpression(new Backreference("enumerator").ToExpression(), new TypePattern(typeof(IDisposable)))
				),
				new IfElseStatement {
					Condition = new BinaryOperatorExpression {
						Left = new Backreference("disposable"),
						Operator = BinaryOperatorType.InEquality,
						Right = new NullReferenceExpression()
					},
					TrueStatement = new BlockStatement {
						new InvocationExpression(new MemberReferenceExpression(new Backreference("disposable").ToExpression(), "Dispose"))
					}
				}
			}};
		
		public ForeachStatement TransformNonGenericForEach(ExpressionStatement node)
		{
			Match m1 = getEnumeratorPattern.Match(node);
			if (!m1.Success) return null;
			AstNode tryCatch = node.NextSibling;
			Match m2 = nonGenericForeachPattern.Match(tryCatch);
			if (!m2.Success) return null;
			
			IdentifierExpression enumeratorVar = m2.Get<IdentifierExpression>("enumerator").Single();
			var itemVar = m2.Get<IdentifierExpression>("itemVar").Single().GetILVariable();
			WhileStatement loop = m2.Get<WhileStatement>("loop").Single();
			
			// verify that the getEnumeratorPattern assigns to the same variable as the nonGenericForeachPattern is reading from
			if (!enumeratorVar.IsMatch(m1.Get("left").Single()))
				return null;

			if (!VariableCanBeDeclaredInLoop(itemVar, loop))
				return null;

			itemVar.Kind = IL.VariableKind.ForeachLocal;
			ForeachStatement foreachStatement = new ForeachStatement
			{
				VariableType = itemVar.Type.ContainsAnonymousType() ? new SimpleType("var") : context.TypeSystemAstBuilder.ConvertType(itemVar.Type),
				VariableName = itemVar.Name,
			}.WithILVariable(itemVar);
			BlockStatement body = new BlockStatement();
			foreachStatement.EmbeddedStatement = body;
			foreachStatement.CopyAnnotationsFrom(loop);
			((BlockStatement)node.Parent).Statements.InsertBefore(node, foreachStatement);
			
			body.Add(node.Detach());
			body.Add((Statement)tryCatch.Detach());
			
			/*
			// Now that we moved the whole try-catch into the foreach loop; verify that we can
			// move the enumerator into the foreach loop:
			CanMoveVariableDeclarationIntoStatement(enumeratorVarDecl, foreachStatement, out declarationPoint);
			if (declarationPoint != foreachStatement) {
				// oops, the enumerator variable can't be moved into the foreach loop
				// Undo our AST changes:
				((BlockStatement)foreachStatement.Parent).Statements.InsertBefore(foreachStatement, node.Detach());
				foreachStatement.ReplaceWith(tryCatch);
				return null;
			}
			*/

			// Now create the correct body for the foreach statement:
			foreachStatement.InExpression = m1.Get<Expression>("collection").Single().Detach();
			if (foreachStatement.InExpression is BaseReferenceExpression) {
				foreachStatement.InExpression = new ThisReferenceExpression().CopyAnnotationsFrom(foreachStatement.InExpression);
			}
			body.Statements.Clear();
			body.Statements.AddRange(m2.Get<Statement>("stmt").Select(stmt => stmt.Detach()));
			
			return foreachStatement;
		}
		#endregion

		#region for
		static readonly WhileStatement forPattern = new WhileStatement {
			Condition = new BinaryOperatorExpression {
				Left = new NamedNode("ident", new IdentifierExpression(Pattern.AnyString)),
				Operator = BinaryOperatorType.Any,
				Right = new AnyNode("endExpr")
			},
			EmbeddedStatement = new BlockStatement {
				Statements = {
					new Repeat(new AnyNode("statement")),
					new NamedNode(
						"increment",
						new ExpressionStatement(
							new AssignmentExpression {
								Left = new Backreference("ident"),
								Operator = AssignmentOperatorType.Any,
								Right = new AnyNode()
							}))
				}
			}};

		static readonly ForStatement forLoopWithoutInitializer = new ForStatement {
			Condition = new BinaryOperatorExpression {
				Left = new NamedNode("ident", new IdentifierExpression(Pattern.AnyString)),
				Operator = BinaryOperatorType.Any,
				Right = new AnyNode("endExpr")
			},
			Iterators = {
				new NamedNode(
					"increment",
					new ExpressionStatement(
						new AssignmentExpression {
							Left = new Backreference("ident"),
							Operator = AssignmentOperatorType.Any,
							Right = new AnyNode()
						}))
			},
			EmbeddedStatement = new AnyNode()
		};

		public ForStatement TransformFor(ExpressionStatement node)
		{
			Match m1 = variableAssignPattern.Match(node);
			if (!m1.Success) return null;
			AstNode next = node.NextSibling;
			Match m2 = forLoopWithoutInitializer.Match(next);
			ForStatement forStatement;
			if (m2.Success) {
				node.Remove();
				forStatement = (ForStatement)next;
				forStatement.InsertChildAfter(null, node, ForStatement.InitializerRole);
				return forStatement;
			}
			Match m3 = forPattern.Match(next);
			if (!m3.Success) return null;
			// ensure the variable in the for pattern is the same as in the declaration
			if (m1.Get<IdentifierExpression>("variable").Single().Identifier != m3.Get<IdentifierExpression>("ident").Single().Identifier)
				return null;
			WhileStatement loop = (WhileStatement)next;
			node.Remove();
			BlockStatement newBody = new BlockStatement();
			foreach (Statement stmt in m3.Get<Statement>("statement"))
				newBody.Add(stmt.Detach());
			forStatement = new ForStatement();
			forStatement.CopyAnnotationsFrom(loop);
			forStatement.Initializers.Add(node);
			forStatement.Condition = loop.Condition.Detach();
			forStatement.Iterators.Add(m3.Get<Statement>("increment").Single().Detach());
			forStatement.EmbeddedStatement = newBody;
			loop.ReplaceWith(forStatement);
			return forStatement;
		}
		#endregion
		
		#region doWhile
		static readonly WhileStatement doWhilePattern = new WhileStatement {
			Condition = new PrimitiveExpression(true),
			EmbeddedStatement = new BlockStatement {
				Statements = {
					new Repeat(new AnyNode("statement")),
					new IfElseStatement {
						Condition = new AnyNode("condition"),
						TrueStatement = new BlockStatement { new BreakStatement() }
					}
				}
			}};
		
		public DoWhileStatement TransformDoWhile(WhileStatement whileLoop)
		{
			Match m = doWhilePattern.Match(whileLoop);
			if (m.Success) {
				DoWhileStatement doLoop = new DoWhileStatement();
				doLoop.Condition = new UnaryOperatorExpression(UnaryOperatorType.Not, m.Get<Expression>("condition").Single().Detach());
				//doLoop.Condition.AcceptVisitor(new PushNegation(), null);
				BlockStatement block = (BlockStatement)whileLoop.EmbeddedStatement;
				block.Statements.Last().Remove(); // remove if statement
				doLoop.EmbeddedStatement = block.Detach();
				doLoop.CopyAnnotationsFrom(whileLoop);
				whileLoop.ReplaceWith(doLoop);
				
				// we may have to extract variable definitions out of the loop if they were used in the condition:
				foreach (var varDecl in block.Statements.OfType<VariableDeclarationStatement>()) {
					VariableInitializer v = varDecl.Variables.Single();
					if (doLoop.Condition.DescendantsAndSelf.OfType<IdentifierExpression>().Any(i => i.Identifier == v.Name)) {
						AssignmentExpression assign = new AssignmentExpression(new IdentifierExpression(v.Name), v.Initializer.Detach());
						// move annotations from v to assign:
						assign.CopyAnnotationsFrom(v);
						v.RemoveAnnotations<object>();
						// remove varDecl with assignment; and move annotations from varDecl to the ExpressionStatement:
						varDecl.ReplaceWith(new ExpressionStatement(assign).CopyAnnotationsFrom(varDecl));
						varDecl.RemoveAnnotations<object>();
						
						// insert the varDecl above the do-while loop:
						doLoop.Parent.InsertChildBefore(doLoop, varDecl, BlockStatement.StatementRole);
					}
				}
				return doLoop;
			}
			return null;
		}
		#endregion
		
		#region lock
		static readonly AstNode lockFlagInitPattern = new ExpressionStatement(
			new AssignmentExpression(
				new NamedNode("variable", new IdentifierExpression(Pattern.AnyString)),
				new PrimitiveExpression(false)
			));
		
		static readonly AstNode lockTryCatchPattern = new TryCatchStatement {
			TryBlock = new BlockStatement {
				new OptionalNode(new VariableDeclarationStatement()).ToStatement(),
				new InvocationExpression(new MemberReferenceExpression(new TypeReferenceExpression(new TypePattern(typeof(System.Threading.Monitor)).ToType()), "Enter"),
					new AnyNode("enter"),
					new DirectionExpression {
						FieldDirection = FieldDirection.Ref,
						Expression = new NamedNode("flag", new IdentifierExpression(Pattern.AnyString))
					}),
				new Repeat(new AnyNode()).ToStatement()
			},
			FinallyBlock = new BlockStatement {
				new IfElseStatement {
					Condition = new Backreference("flag"),
					TrueStatement = new BlockStatement {
						new InvocationExpression(new MemberReferenceExpression(new TypeReferenceExpression(new TypePattern(typeof(System.Threading.Monitor)).ToType()), "Exit"), new AnyNode("exit"))
					}
				}
			}};

		static readonly AstNode oldMonitorCallPattern = new ExpressionStatement(
			new InvocationExpression(new MemberReferenceExpression(new TypeReferenceExpression(new TypePattern(typeof(System.Threading.Monitor)).ToType()), "Enter"), new AnyNode("enter"))
		);

		static readonly AstNode oldLockTryCatchPattern = new TryCatchStatement
		{
			TryBlock = new BlockStatement {
				new Repeat(new AnyNode()).ToStatement()
			},
			FinallyBlock = new BlockStatement {
				new InvocationExpression(new MemberReferenceExpression(new TypeReferenceExpression(new TypePattern(typeof(System.Threading.Monitor)).ToType()), "Exit"), new AnyNode("exit"))
			}
		};

		bool AnalyzeLockV2(ExpressionStatement node, out Expression enter, out Expression exit)
		{
			enter = null;
			exit = null;
			Match m1 = oldMonitorCallPattern.Match(node);
			if (!m1.Success) return false;
			Match m2 = oldLockTryCatchPattern.Match(node.NextSibling);
			if (!m2.Success) return false;
			enter = m1.Get<Expression>("enter").Single();
			exit = m2.Get<Expression>("exit").Single();
			return true;
		}

		bool AnalyzeLockV4(ExpressionStatement node, out Expression enter, out Expression exit)
		{
			enter = null;
			exit = null;
			Match m1 = lockFlagInitPattern.Match(node);
			if (!m1.Success) return false;
			Match m2 = lockTryCatchPattern.Match(node.NextSibling);
			if (!m2.Success) return false;
			enter = m2.Get<Expression>("enter").Single();
			exit = m2.Get<Expression>("exit").Single();
			return m1.Get<IdentifierExpression>("variable").Single().Identifier == m2.Get<IdentifierExpression>("flag").Single().Identifier;
		}
		
		public LockStatement TransformLock(ExpressionStatement node)
		{
			Expression enter, exit;
			bool isV2 = AnalyzeLockV2(node, out enter, out exit);
			if (isV2 || AnalyzeLockV4(node, out enter, out exit)) {
				AstNode tryCatch = node.NextSibling;
				if (!exit.IsMatch(enter)) {
					// If exit and enter are not the same, then enter must be "exit = ..."
					AssignmentExpression assign = enter as AssignmentExpression;
					if (assign == null)
						return null;
					if (!exit.IsMatch(assign.Left))
						return null;
					enter = assign.Right;
					// TODO: verify that 'obj' variable can be removed
				}
				// TODO: verify that 'flag' variable can be removed
				// transform the code into a lock statement:
				LockStatement l = new LockStatement();
				l.Expression = enter.Detach();
				l.EmbeddedStatement = ((TryCatchStatement)tryCatch).TryBlock.Detach();
				if (!isV2) // Remove 'Enter()' call
					((BlockStatement)l.EmbeddedStatement).Statements.First().Remove(); 
				tryCatch.ReplaceWith(l);
				node.Remove(); // remove flag variable
				return l;
			}
			return null;
		}
		#endregion
		
		#region switch on strings
		static readonly IfElseStatement switchOnStringPattern = new IfElseStatement {
			Condition = new BinaryOperatorExpression {
				Left = new AnyNode("switchExpr"),
				Operator = BinaryOperatorType.InEquality,
				Right = new NullReferenceExpression()
			},
			TrueStatement = new BlockStatement {
				new IfElseStatement {
					Condition = new BinaryOperatorExpression {
						Left = new AnyNode("cachedDict"),
						Operator = BinaryOperatorType.Equality,
						Right = new NullReferenceExpression()
					},
					TrueStatement = new AnyNode("dictCreation")
				},
				new IfElseStatement {
					Condition = new InvocationExpression(new MemberReferenceExpression(new Backreference("cachedDict").ToExpression(), "TryGetValue"),
						new NamedNode("switchVar", new IdentifierExpression(Pattern.AnyString)),
						new DirectionExpression {
							FieldDirection = FieldDirection.Out,
							Expression = new IdentifierExpression(Pattern.AnyString).WithName("intVar")
						}),
					TrueStatement = new BlockStatement {
						Statements = {
							new NamedNode(
								"switch", new SwitchStatement {
									Expression = new IdentifierExpressionBackreference("intVar"),
									SwitchSections = { new Repeat(new AnyNode()) }
								})
						}
					}
				},
				new Repeat(new AnyNode("nonNullDefaultStmt")).ToStatement()
			},
			FalseStatement = new OptionalNode("nullStmt", new BlockStatement { Statements = { new Repeat(new AnyNode()) } })
		};
		
		public SwitchStatement TransformSwitchOnString(IfElseStatement node)
		{
			Match m = switchOnStringPattern.Match(node);
			if (!m.Success)
				return null;
			// switchVar must be the same as switchExpr; or switchExpr must be an assignment and switchVar the left side of that assignment
			if (!m.Get("switchVar").Single().IsMatch(m.Get("switchExpr").Single())) {
				AssignmentExpression assign = m.Get("switchExpr").Single() as AssignmentExpression;
				if (!(assign != null && m.Get("switchVar").Single().IsMatch(assign.Left)))
					return null;
			}
			FieldReference cachedDictField = m.Get<AstNode>("cachedDict").Single().Annotation<FieldReference>();
			if (cachedDictField == null)
				return null;
			List<Statement> dictCreation = m.Get<BlockStatement>("dictCreation").Single().Statements.ToList();
			List<KeyValuePair<string, int>> dict = BuildDictionary(dictCreation);
			SwitchStatement sw = m.Get<SwitchStatement>("switch").Single();
			sw.Expression = m.Get<Expression>("switchExpr").Single().Detach();
			foreach (SwitchSection section in sw.SwitchSections) {
				List<CaseLabel> labels = section.CaseLabels.ToList();
				section.CaseLabels.Clear();
				foreach (CaseLabel label in labels) {
					PrimitiveExpression expr = label.Expression as PrimitiveExpression;
					if (expr == null || !(expr.Value is int))
						continue;
					int val = (int)expr.Value;
					foreach (var pair in dict) {
						if (pair.Value == val)
							section.CaseLabels.Add(new CaseLabel { Expression = new PrimitiveExpression(pair.Key) });
					}
				}
			}
			if (m.Has("nullStmt")) {
				SwitchSection section = new SwitchSection();
				section.CaseLabels.Add(new CaseLabel { Expression = new NullReferenceExpression() });
				BlockStatement block = m.Get<BlockStatement>("nullStmt").Single();
				block.Statements.Add(new BreakStatement());
				section.Statements.Add(block.Detach());
				sw.SwitchSections.Add(section);
			} else if (m.Has("nonNullDefaultStmt")) {
				sw.SwitchSections.Add(
					new SwitchSection {
						CaseLabels = { new CaseLabel { Expression = new NullReferenceExpression() } },
						Statements = { new BlockStatement { new BreakStatement() } }
					});
			}
			if (m.Has("nonNullDefaultStmt")) {
				SwitchSection section = new SwitchSection();
				section.CaseLabels.Add(new CaseLabel());
				BlockStatement block = new BlockStatement();
				block.Statements.AddRange(m.Get<Statement>("nonNullDefaultStmt").Select(s => s.Detach()));
				block.Add(new BreakStatement());
				section.Statements.Add(block);
				sw.SwitchSections.Add(section);
			}
			node.ReplaceWith(sw);
			return sw;
		}
		
		List<KeyValuePair<string, int>> BuildDictionary(List<Statement> dictCreation)
		{
			if (context.Settings.ObjectOrCollectionInitializers && dictCreation.Count == 1)
				return BuildDictionaryFromInitializer(dictCreation[0]);

			return BuildDictionaryFromAddMethodCalls(dictCreation);
		}

		static readonly Statement assignInitializedDictionary = new ExpressionStatement {
			Expression = new AssignmentExpression {
				Left = new AnyNode().ToExpression(),
				Right = new ObjectCreateExpression {
					Type = new AnyNode(),
					Arguments = { new Repeat(new AnyNode()) },
					Initializer = new ArrayInitializerExpression {
						Elements = { new Repeat(new AnyNode("dictJumpTable")) }
					}
				},
			},
		};

		private List<KeyValuePair<string, int>> BuildDictionaryFromInitializer(Statement statement)
		{
			List<KeyValuePair<string, int>> dict = new List<KeyValuePair<string, int>>();
			Match m = assignInitializedDictionary.Match(statement);
			if (!m.Success)
				return dict;

			foreach (ArrayInitializerExpression initializer in m.Get<ArrayInitializerExpression>("dictJumpTable")) {
				KeyValuePair<string, int> pair;
				if (TryGetPairFrom(initializer.Elements, out pair))
					dict.Add(pair);
			}

			return dict;
		}

		private static List<KeyValuePair<string, int>> BuildDictionaryFromAddMethodCalls(List<Statement> dictCreation)
		{
			List<KeyValuePair<string, int>> dict = new List<KeyValuePair<string, int>>();
			for (int i = 0; i < dictCreation.Count; i++) {
				ExpressionStatement es = dictCreation[i] as ExpressionStatement;
				if (es == null)
					continue;
				InvocationExpression ie = es.Expression as InvocationExpression;
				if (ie == null)
					continue;

				KeyValuePair<string, int> pair;
				if (TryGetPairFrom(ie.Arguments, out pair))
					dict.Add(pair);
			}
			return dict;
		}

		private static bool TryGetPairFrom(AstNodeCollection<Expression> expressions, out KeyValuePair<string, int> pair)
		{
			PrimitiveExpression arg1 = expressions.ElementAtOrDefault(0) as PrimitiveExpression;
			PrimitiveExpression arg2 = expressions.ElementAtOrDefault(1) as PrimitiveExpression;
			if (arg1 != null && arg2 != null && arg1.Value is string && arg2.Value is int) {
				pair = new KeyValuePair<string, int>((string)arg1.Value, (int)arg2.Value);
				return true;
			}

			pair = default(KeyValuePair<string, int>);
			return false;
		}

		#endregion
		
		#region Automatic Properties
		static readonly PropertyDeclaration automaticPropertyPattern = new PropertyDeclaration {
			Attributes = { new Repeat(new AnyNode()) },
			Modifiers = Modifiers.Any,
			ReturnType = new AnyNode(),
			PrivateImplementationType = new OptionalNode(new AnyNode()),
			Name = Pattern.AnyString,
			Getter = new Accessor {
				Attributes = { new Repeat(new AnyNode()) },
				Modifiers = Modifiers.Any,
				Body = new BlockStatement {
					new ReturnStatement {
						Expression = new AnyNode("fieldReference")
					}
				}
			},
			Setter = new Accessor {
				Attributes = { new Repeat(new AnyNode()) },
				Modifiers = Modifiers.Any,
				Body = new BlockStatement {
					new AssignmentExpression {
						Left = new Backreference("fieldReference"),
						Right = new IdentifierExpression("value")
					}
				}}};
		
		PropertyDeclaration TransformAutomaticProperties(PropertyDeclaration property)
		{
			PropertyDefinition cecilProperty = context.TypeSystem.GetCecil(property.GetSymbol() as IProperty) as PropertyDefinition;
			if (cecilProperty == null || cecilProperty.GetMethod == null || cecilProperty.SetMethod == null)
				return null;
			if (!(cecilProperty.GetMethod.IsCompilerGenerated() && cecilProperty.SetMethod.IsCompilerGenerated()))
				return null;
			Match m = automaticPropertyPattern.Match(property);
			if (m.Success) {
				var fieldInfo = m.Get<AstNode>("fieldReference").Single().GetSymbol() as IField;
				if (fieldInfo == null)
					return null;
				FieldDefinition field = context.TypeSystem.GetCecil(fieldInfo) as FieldDefinition;
				if (field.IsCompilerGenerated() && field.DeclaringType == cecilProperty.DeclaringType) {
					RemoveCompilerGeneratedAttribute(property.Getter.Attributes);
					RemoveCompilerGeneratedAttribute(property.Setter.Attributes);
					property.Getter.Body = null;
					property.Setter.Body = null;
				}
			}
			// Since the property instance is not changed, we can continue in the visitor as usual, so return null
			return null;
		}
		
		void RemoveCompilerGeneratedAttribute(AstNodeCollection<AttributeSection> attributeSections)
		{
			foreach (AttributeSection section in attributeSections) {
				foreach (var attr in section.Attributes) {
					var tr = attr.Type.GetSymbol() as IType;
					if (tr != null && tr.Namespace == "System.Runtime.CompilerServices" && tr.Name == "CompilerGeneratedAttribute") {
						attr.Remove();
					}
				}
				if (section.Attributes.Count == 0)
					section.Remove();
			}
		}
		#endregion
		
		#region Automatic Events
		static readonly Accessor automaticEventPatternV4 = new Accessor {
			Attributes = { new Repeat(new AnyNode()) },
			Body = new BlockStatement {
				new VariableDeclarationStatement { Type = new AnyNode("type"), Variables = { new AnyNode() } },
				new VariableDeclarationStatement { Type = new Backreference("type"), Variables = { new AnyNode() } },
				new VariableDeclarationStatement { Type = new Backreference("type"), Variables = { new AnyNode() } },
				new AssignmentExpression {
					Left = new NamedNode("var1", new IdentifierExpression(Pattern.AnyString)),
					Operator = AssignmentOperatorType.Assign,
					Right = new NamedNode(
						"field",
						new MemberReferenceExpression {
							Target = new Choice { new ThisReferenceExpression(), new TypeReferenceExpression { Type = new AnyNode() } },
							MemberName = Pattern.AnyString
						})
				},
				new DoWhileStatement {
					EmbeddedStatement = new BlockStatement {
						new AssignmentExpression(new NamedNode("var2", new IdentifierExpression(Pattern.AnyString)), new IdentifierExpressionBackreference("var1")),
						new AssignmentExpression {
							Left = new NamedNode("var3", new IdentifierExpression(Pattern.AnyString)),
							Operator = AssignmentOperatorType.Assign,
							Right = new CastExpression(new Backreference("type"), new InvocationExpression(new AnyNode("delegateCombine").ToExpression(), new IdentifierExpressionBackreference("var2"),
								new IdentifierExpression("value")
							))
						},
						new AssignmentExpression {
							Left = new IdentifierExpressionBackreference("var1"),
							Right = new InvocationExpression(new MemberReferenceExpression(new TypeReferenceExpression(new TypePattern(typeof(System.Threading.Interlocked)).ToType()),
								"CompareExchange",
								new AstType[] { new Backreference("type") }), // type argument
								new Expression[] { // arguments
									new DirectionExpression { FieldDirection = FieldDirection.Ref, Expression = new Backreference("field") },
									new IdentifierExpressionBackreference("var3"),
									new IdentifierExpressionBackreference("var2")
								}
							)}
					},
					Condition = new BinaryOperatorExpression {
						Left = new IdentifierExpressionBackreference("var1"),
						Operator = BinaryOperatorType.InEquality,
						Right = new IdentifierExpressionBackreference("var2")
					}}
			}};
		
		bool CheckAutomaticEventV4Match(Match m, CustomEventDeclaration ev, bool isAddAccessor)
		{
			if (!m.Success)
				return false;
			if (m.Get<MemberReferenceExpression>("field").Single().MemberName != ev.Name)
				return false; // field name must match event name
			if (!ev.ReturnType.IsMatch(m.Get("type").Single()))
				return false; // variable types must match event type
			var combineMethod = m.Get<AstNode>("delegateCombine").Single().Parent.Annotation<MethodReference>();
			if (combineMethod == null || combineMethod.Name != (isAddAccessor ? "Combine" : "Remove"))
				return false;
			return combineMethod.DeclaringType.FullName == "System.Delegate";
		}
		
		EventDeclaration TransformAutomaticEvents(CustomEventDeclaration ev)
		{
			Match m1 = automaticEventPatternV4.Match(ev.AddAccessor);
			if (!CheckAutomaticEventV4Match(m1, ev, true))
				return null;
			Match m2 = automaticEventPatternV4.Match(ev.RemoveAccessor);
			if (!CheckAutomaticEventV4Match(m2, ev, false))
				return null;
			EventDeclaration ed = new EventDeclaration();
			ev.Attributes.MoveTo(ed.Attributes);
			foreach (var attr in ev.AddAccessor.Attributes) {
				attr.AttributeTarget = "method";
				ed.Attributes.Add(attr.Detach());
			}
			ed.ReturnType = ev.ReturnType.Detach();
			ed.Modifiers = ev.Modifiers;
			ed.Variables.Add(new VariableInitializer(ev.Name));
			ed.CopyAnnotationsFrom(ev);
			
			EventDefinition eventDef = ev.Annotation<EventDefinition>();
			if (eventDef != null) {
				FieldDefinition field = eventDef.DeclaringType.Fields.FirstOrDefault(f => f.Name == ev.Name);
				if (field != null) {
					ed.AddAnnotation(field);
					// TODO AstBuilder.ConvertAttributes(ed, field, "field");
				}
			}
			
			ev.ReplaceWith(ed);
			return ed;
		}
		#endregion
		
		#region Destructor
		static readonly MethodDeclaration destructorPattern = new MethodDeclaration {
			Attributes = { new Repeat(new AnyNode()) },
			Modifiers = Modifiers.Any,
			ReturnType = new PrimitiveType("void"),
			Name = "Finalize",
			Body = new BlockStatement {
				new TryCatchStatement {
					TryBlock = new AnyNode("body"),
					FinallyBlock = new BlockStatement {
						new InvocationExpression(new MemberReferenceExpression(new BaseReferenceExpression(), "Finalize"))
					}
				}
			}
		};
		
		DestructorDeclaration TransformDestructor(MethodDeclaration methodDef)
		{
			Match m = destructorPattern.Match(methodDef);
			if (m.Success) {
				DestructorDeclaration dd = new DestructorDeclaration();
				methodDef.Attributes.MoveTo(dd.Attributes);
				dd.Modifiers = methodDef.Modifiers & ~(Modifiers.Protected | Modifiers.Override);
				dd.Body = m.Get<BlockStatement>("body").Single().Detach();
				dd.Name = currentTypeDefinition?.Name;
				methodDef.ReplaceWith(dd);
				return dd;
			}
			return null;
		}
		#endregion
		
		#region Try-Catch-Finally
		static readonly TryCatchStatement tryCatchFinallyPattern = new TryCatchStatement {
			TryBlock = new BlockStatement {
				new TryCatchStatement {
					TryBlock = new AnyNode(),
					CatchClauses = { new Repeat(new AnyNode()) }
				}
			},
			FinallyBlock = new AnyNode()
		};
		
		/// <summary>
		/// Simplify nested 'try { try {} catch {} } finally {}'.
		/// This transformation must run after the using/lock tranformations.
		/// </summary>
		TryCatchStatement TransformTryCatchFinally(TryCatchStatement tryFinally)
		{
			if (tryCatchFinallyPattern.IsMatch(tryFinally)) {
				TryCatchStatement tryCatch = (TryCatchStatement)tryFinally.TryBlock.Statements.Single();
				tryFinally.TryBlock = tryCatch.TryBlock.Detach();
				tryCatch.CatchClauses.MoveTo(tryFinally.CatchClauses);
			}
			// Since the tryFinally instance is not changed, we can continue in the visitor as usual, so return null
			return null;
		}
		#endregion

		#region Simplify cascading if-else-if statements
		static readonly IfElseStatement cascadingIfElsePattern = new IfElseStatement
		{
			Condition = new AnyNode(),
			TrueStatement = new AnyNode(),
			FalseStatement = new BlockStatement {
				Statements = {
					new NamedNode(
						"nestedIfStatement",
						new IfElseStatement {
							Condition = new AnyNode(),
							TrueStatement = new AnyNode(),
							FalseStatement = new OptionalNode(new AnyNode())
						}
					)
				}
			}
		};

		AstNode SimplifyCascadingIfElseStatements(IfElseStatement node)
		{
			Match m = cascadingIfElsePattern.Match(node);
			if (m.Success) {
				IfElseStatement elseIf = m.Get<IfElseStatement>("nestedIfStatement").Single();
				node.FalseStatement = elseIf.Detach();
			}
			
			return null;
		}

		/// <summary>
		/// Use associativity of logic operators to avoid parentheses.
		/// </summary>
		public override AstNode VisitBinaryOperatorExpression(BinaryOperatorExpression boe1)
		{
			switch (boe1.Operator) {
				case BinaryOperatorType.ConditionalAnd:
				case BinaryOperatorType.ConditionalOr:
					// a && (b && c) ==> (a && b) && c
					var boe2 = boe1.Right as BinaryOperatorExpression;
					if (boe2 != null && boe2.Operator == boe1.Operator) {
						// make boe2 the parent and boe1 the child
						var b = boe2.Left.Detach();
						boe1.ReplaceWith(boe2.Detach());
						boe2.Left = boe1;
						boe1.Right = b;
						return base.VisitBinaryOperatorExpression(boe2);
					}
					break;
			}
			return base.VisitBinaryOperatorExpression(boe1);
		}
		#endregion
	}
}
