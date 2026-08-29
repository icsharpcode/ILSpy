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

#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;

using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Syntax.PatternMatching;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.CSharp.Transforms
{
	/// <summary>
	/// Finds the expanded form of using statements using pattern matching and replaces it with a UsingStatement.
	/// </summary>
	public sealed class PatternStatementTransform : ContextTrackingVisitor<AstNode>, IAstTransform
	{
		readonly DeclareVariables declareVariables = new DeclareVariables();
		[AllowNull]
		TransformContext context;

		public void Run(AstNode rootNode, TransformContext context)
		{
			if (this.context != null)
				throw new InvalidOperationException("Reentrancy in PatternStatementTransform.Run?");
			try
			{
				this.context = context;
				base.Initialize(context);
				declareVariables.Analyze(rootNode);
				rootNode.AcceptVisitor(this);
			}
			finally
			{
				this.context = null;
				base.Uninitialize();
				declareVariables.ClearAnalysisResults();
			}
		}

		#region Visitor Overrides
		protected override AstNode VisitChildren(AstNode node)
		{
			// Go through the children, and keep visiting a node as long as it changes.
			// Because some transforms delete/replace nodes before and after the node being transformed, we rely
			// on the transform's return value to know where we need to keep iterating.
			for (AstNode? child = node.FirstChild; child != null; child = child.NextSibling)
			{
				AstNode oldChild;
				do
				{
					oldChild = child;
					child = child.AcceptVisitor(this);
					Debug.Assert(child != null && child.Parent == node);
				} while (child != oldChild);
			}
			return node;
		}

		public override AstNode VisitExpressionStatement(ExpressionStatement expressionStatement)
		{
			AstNode? result = TransformForeachOnMultiDimArray(expressionStatement);
			if (result != null)
				return result;
			result = TransformFor(expressionStatement);
			if (result != null)
				return result;
			return base.VisitExpressionStatement(expressionStatement);
		}

		public override AstNode VisitForStatement(ForStatement forStatement)
		{
			AstNode? result = TransformForeachOnArray(forStatement);
			if (result != null)
				return result;
			result = TransformForeachOnInlineArray(forStatement);
			if (result != null)
				return result;
			return base.VisitForStatement(forStatement);
		}

		public override AstNode VisitIfElseStatement(IfElseStatement ifElseStatement)
		{
			AstNode? simplifiedIfElse = SimplifyCascadingIfElseStatements(ifElseStatement);
			if (simplifiedIfElse != null)
				return simplifiedIfElse;
			return base.VisitIfElseStatement(ifElseStatement);
		}

		public override AstNode VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration)
		{
			if (context.Settings.AutomaticProperties
				&& (propertyDeclaration.Setter is not null || context.Settings.GetterOnlyAutomaticProperties))
			{
				AstNode? result = TransformAutomaticProperty(propertyDeclaration);
				if (result != null)
					return result;
			}
			return base.VisitPropertyDeclaration(propertyDeclaration);
		}

		public override AstNode VisitEventDeclaration(EventDeclaration eventDeclaration)
		{
			// A field-like event declaration hides its backing field; remove the field declaration
			// if it was emitted because other members reference it. (This happens for events that
			// CSharpDecompiler.DoDecompile already recognized as automatic: the backing field is
			// hidden from the normal member list, but re-emitted via the work list when referenced.)
			if (context.Settings.AutomaticEvents && eventDeclaration.GetSymbol() is IEvent symbol)
			{
				var fieldDecl = eventDeclaration.Parent?.Children.OfType<FieldDeclaration>()
					.FirstOrDefault(fd => IsEventBackingFieldDeclaration(fd, symbol));
				fieldDecl?.Remove();
			}
			return base.VisitEventDeclaration(eventDeclaration);
		}

		public override AstNode VisitMethodDeclaration(MethodDeclaration methodDeclaration)
		{
			return TransformDestructor(methodDeclaration) ?? base.VisitMethodDeclaration(methodDeclaration);
		}

		public override AstNode VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration)
		{
			return TransformDestructorBody(destructorDeclaration) ?? base.VisitDestructorDeclaration(destructorDeclaration);
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
						"iterator",
						new ExpressionStatement(
							new AssignmentExpression {
								Left = new Backreference("ident"),
								Operator = AssignmentOperatorType.Any,
								Right = new AnyNode()
							}))
				}
			}
		};

		public ForStatement? TransformFor(ExpressionStatement node)
		{
			if (!context.Settings.ForStatement)
				return null;
			Match m1 = variableAssignPattern.Match(node);
			if (!m1.Success)
				return null;
			var variable = m1.Get<IdentifierExpression>("variable").Single().GetILVariable();
			AstNode? next = node.NextSibling;
			if (next == null)
				return null;
			if (next is ForStatement forStatement && ForStatementUsesVariable(forStatement, variable))
			{
				context.Step("Move declaration into for initializer", node);
				node.Remove();
				next.InsertChildAfter(null, node, Slots.ForInitializer);
				return (ForStatement)next;
			}
			Match m3 = forPattern.Match(next);
			if (!m3.Success)
				return null;
			// ensure the variable in the for pattern is the same as in the declaration
			if (variable != m3.Get<IdentifierExpression>("ident").Single().GetILVariable())
				return null;
			WhileStatement loop = (WhileStatement)next;
			// Cannot convert to for loop, if the iteration variable is a ref local used after the loop: its
			// declaration is hoisted in front, leaving a headless `for (; cond; v = ref ...)` whose only
			// initialization is the for-initializer ref-assignment -- which can't be split from a ref local
			// (CS8174). Keeping it a while-loop matches the source and keeps the initializer on the decl.
			if (variable != null && variable.Type.IsByRefLike && IsVariableUsedAfter(loop, variable))
				return null;
			// Cannot convert to for loop, if any variable that is used in the "iterator" part of the pattern,
			// will be declared in the body of the while-loop.
			var iteratorStatement = m3.Get<Statement>("iterator").Single();
			if (IteratorVariablesDeclaredInsideLoopBody(iteratorStatement))
				return null;
			// Cannot convert to for loop, because that would change the semantics of the program.
			// continue in while jumps to the condition block.
			// Whereas continue in for jumps to the increment block.
			if (loop.DescendantNodes(DescendIntoStatement).OfType<Statement>().Any(s => s is ContinueStatement))
				return null;
			context.Step("Transform while loop to for", loop);
			node.Remove();
			BlockStatement newBody = new BlockStatement();
			foreach (Statement stmt in m3.Get<Statement>("statement"))
				newBody.Add(stmt.Detach());
			forStatement = new ForStatement();
			forStatement.CopyAnnotationsFrom(loop);
			forStatement.Initializers.Add(node);
			forStatement.Condition = loop.Condition.Detach();
			forStatement.Iterators.Add(iteratorStatement.Detach());
			forStatement.EmbeddedStatement = newBody;
			loop.ReplaceWith(forStatement);
			context.EndStep(forStatement);
			return forStatement;
		}

		bool DescendIntoStatement(AstNode node)
		{
			if (node is Expression || node is ExpressionStatement)
				return false;
			if (node is WhileStatement || node is ForeachStatement || node is DoWhileStatement || node is ForStatement)
				return false;
			return true;
		}

		bool ForStatementUsesVariable(ForStatement statement, IL.ILVariable? variable)
		{
			if (statement.Condition?.DescendantsAndSelf.OfType<IdentifierExpression>().Any(ie => ie.GetILVariable() == variable) == true)
				return true;
			if (statement.Iterators.Any(i => i.DescendantsAndSelf.OfType<IdentifierExpression>().Any(ie => ie.GetILVariable() == variable)))
				return true;
			return false;
		}

		bool IsVariableUsedAfter(Statement loop, IL.ILVariable variable)
		{
			for (AstNode? sibling = loop.NextSibling; sibling != null; sibling = sibling.NextSibling)
			{
				if (sibling.DescendantsAndSelf.OfType<IdentifierExpression>().Any(ie => ie.GetILVariable() == variable))
					return true;
			}
			return false;
		}

		bool IteratorVariablesDeclaredInsideLoopBody(Statement iteratorStatement)
		{
			foreach (var id in iteratorStatement.DescendantsAndSelf.OfType<IdentifierExpression>())
			{
				var v = id.GetILVariable();
				if (v == null || !DeclareVariables.VariableNeedsDeclaration(v.Kind))
					continue;
				if (declareVariables.GetDeclarationPoint(v).Parent == iteratorStatement.Parent)
					return true;
			}
			return false;
		}
		#endregion

		#region foreach

		static readonly ForStatement forOnArrayPattern = new ForStatement {
			Initializers = {
				new ExpressionStatement(
				new AssignmentExpression(
					new NamedNode("indexVariable", new IdentifierExpression(Pattern.AnyString)),
					new PrimitiveExpression(0)
				))
			},
			Condition = new BinaryOperatorExpression(
				new IdentifierExpressionBackreference("indexVariable"),
				BinaryOperatorType.LessThan,
				new MemberReferenceExpression(new NamedNode("arrayVariable", new IdentifierExpression(Pattern.AnyString)), "Length")
			),
			Iterators = {
				new ExpressionStatement(
				new AssignmentExpression(
					new IdentifierExpressionBackreference("indexVariable"),
					new BinaryOperatorExpression(new IdentifierExpressionBackreference("indexVariable"), BinaryOperatorType.Add, new PrimitiveExpression(1))
				))
			},
			EmbeddedStatement = new BlockStatement {
				Statements = {
					new ExpressionStatement(new AssignmentExpression(
						new NamedNode("itemVariable", new IdentifierExpression(Pattern.AnyString)),
						new IndexerExpression(new IdentifierExpressionBackreference("arrayVariable"), new IdentifierExpressionBackreference("indexVariable"))
					)),
					new Repeat(new AnyNode("statements"))
				}
			}
		};

		bool VariableCanBeUsedAsForeachLocal(IL.ILVariable itemVar, Statement loop)
		{
			if (itemVar == null || !(itemVar.Kind == IL.VariableKind.Local || itemVar.Kind == IL.VariableKind.StackSlot))
			{
				// only locals/temporaries can be converted into foreach loop variable
				return false;
			}

			if (context.Settings.UserDefinedCompoundAssignmentOperators
				&& IL.UserDefinedCompoundAssign.IsCompoundAssignmentReceiver(itemVar))
			{
				// A foreach variable is read-only, but an instance compound assignment operator
				// requires an assignable target; the plain loop keeps the variable writable.
				return false;
			}

			var blockContainer = loop.Annotation<IL.BlockContainer>();

			if (!itemVar.IsSingleDefinition)
			{
				// foreach variable cannot be assigned to.
				// As a special case, we accept taking the address for a method call,
				// but only if the call is the only use, so that any mutation by the call
				// cannot be observed.
				if (!AddressUsedForSingleCall(itemVar, blockContainer))
				{
					return false;
				}
			}

			if (itemVar.CaptureScope != null && itemVar.CaptureScope != blockContainer)
			{
				// captured variables cannot be declared in the loop unless the loop is their capture scope
				return false;
			}

			AstNode declPoint = declareVariables.GetDeclarationPoint(itemVar);
			return declPoint.Ancestors.Contains(loop) && !declareVariables.WasMerged(itemVar);
		}

		static bool AddressUsedForSingleCall(IL.ILVariable v, IL.BlockContainer? loop)
		{
			if (v.StoreCount == 1 && v.AddressCount == 1 && v.LoadCount == 0 && v.Type.IsReferenceType == false)
			{
				if (v.AddressInstructions[0].Parent is IL.Call call
					&& v.AddressInstructions[0].ChildIndex == 0
					&& !call.Method.IsStatic)
				{
					// used as this pointer for a method call
					// this is OK iff the call is not within a nested loop
					for (var node = call.Parent; node != null; node = node.Parent)
					{
						if (node == loop)
							return true;
						else if (node is IL.BlockContainer)
							break;
					}
				}
			}
			return false;
		}

		Statement? TransformForeachOnArray(ForStatement forStatement)
		{
			if (!context.Settings.ForEachStatement)
				return null;
			Match m = forOnArrayPattern.Match(forStatement);
			if (!m.Success)
				return null;
			var itemVariable = m.Get<IdentifierExpression>("itemVariable").Single().GetILVariable();
			var indexVariable = m.Get<IdentifierExpression>("indexVariable").Single().GetILVariable();
			var arrayVariable = m.Get<IdentifierExpression>("arrayVariable").Single().GetILVariable();
			if (itemVariable == null || indexVariable == null || arrayVariable == null)
				return null;
			if (arrayVariable.Type.Kind != TypeKind.Array && !arrayVariable.Type.IsKnownType(KnownTypeCode.String))
				return null;
			if (!VariableCanBeUsedAsForeachLocal(itemVariable, forStatement))
				return null;
			if (indexVariable.StoreCount != 2 || indexVariable.LoadCount != 3 || indexVariable.AddressCount != 0)
				return null;
			context.Step("Introduce foreach over array", forStatement);
			var body = new BlockStatement();
			foreach (var statement in m.Get<Statement>("statements"))
				body.Statements.Add(statement.Detach());
			var foreachStmt = new ForeachStatement {
				VariableType = context.Settings.AnonymousTypes && itemVariable.Type.ContainsAnonymousType() ? new SimpleType("var") : context.TypeSystemAstBuilder.ConvertType(itemVariable.Type),
				VariableDesignation = new SingleVariableDesignation { Identifier = itemVariable.Name! },
				InExpression = m.Get<IdentifierExpression>("arrayVariable").Single().Detach(),
				EmbeddedStatement = body
			};
			foreachStmt.CopyAnnotationsFrom(forStatement);
			itemVariable.Kind = IL.VariableKind.ForeachLocal;
			// Add the variable annotation for highlighting (TokenTextWriter expects it directly on the ForeachStatement).
			foreachStmt.VariableDesignation.AddAnnotation(new ILVariableResolveResult(itemVariable, itemVariable.Type));
			// TODO : add ForeachAnnotation
			forStatement.ReplaceWith(foreachStmt);
			context.EndStep(foreachStmt);
			return foreachStmt;
		}

		static readonly ForStatement forOnInlineArrayPattern = new ForStatement {
			Initializers = {
				new ExpressionStatement(
				new AssignmentExpression(
					new NamedNode("indexVariable", new IdentifierExpression(Pattern.AnyString)),
					new PrimitiveExpression(0)
				))
			},
			Condition = new BinaryOperatorExpression(
				new IdentifierExpressionBackreference("indexVariable"),
				BinaryOperatorType.LessThan,
				new NamedNode("length", new PrimitiveExpression(PrimitiveExpression.AnyValue))
			),
			Iterators = {
				new ExpressionStatement(
				new AssignmentExpression(
					new IdentifierExpressionBackreference("indexVariable"),
					new BinaryOperatorExpression(new IdentifierExpressionBackreference("indexVariable"), BinaryOperatorType.Add, new PrimitiveExpression(1))
				))
			},
			EmbeddedStatement = new BlockStatement {
				Statements = {
					new ExpressionStatement(new AssignmentExpression(
						new NamedNode("itemVariable", new IdentifierExpression(Pattern.AnyString)),
						new NamedNode("elementAccess", new AnyNode())
					)),
					new Repeat(new AnyNode("statements"))
				}
			}
		};

		/// <summary>
		/// Reconstructs a <c>foreach</c> over an inline array from the <c>for</c> loop the compiler
		/// lowers it to: <c>for (i = 0; i &lt; N; i++) { item = &lt;PrivateImplementationDetails&gt;.InlineArrayElementRef(ref buffer, i); ... }</c>.
		/// The rewrite is only sound because the loop bound <c>N</c> equals the inline array length,
		/// which proves the index is always in range: <c>InlineArrayElementRef</c> is the compiler's
		/// unchecked element accessor, whereas the C# inline-array indexer <c>buffer[i]</c> is
		/// bounds-checked, so the two only agree when the index is provably in-bounds. A loop that
		/// does not match this exact shape keeps the (unnameable but faithful) helper call.
		/// </summary>
		Statement? TransformForeachOnInlineArray(ForStatement forStatement)
		{
			if (!context.Settings.ForEachStatement || !context.Settings.InlineArrays)
				return null;
			Match m = forOnInlineArrayPattern.Match(forStatement);
			if (!m.Success)
				return null;
			var itemVariable = m.Get<IdentifierExpression>("itemVariable").Single().GetILVariable();
			var indexVariable = m.Get<IdentifierExpression>("indexVariable").Single().GetILVariable();
			if (itemVariable == null || indexVariable == null)
				return null;

			// The loop body must start with `item = InlineArrayElementRef(ref buffer, index)`.
			if (m.Get<Expression>("elementAccess").Single() is not InvocationExpression elementAccess)
				return null;
			if (elementAccess.GetSymbol() is not IMethod { DeclaringType.FullName: "<PrivateImplementationDetails>" } helper)
				return null;
			if (helper.Name is not ("InlineArrayElementRef" or "InlineArrayElementRefReadOnly"))
				return null;
			if (elementAccess.Arguments.Count != 2)
				return null;
			// arg0: `ref buffer`, arg1: the loop index.
			if (elementAccess.Arguments.First() is not DirectionExpression { Expression: IdentifierExpression bufferIdentifier })
				return null;
			var bufferVariable = bufferIdentifier.GetILVariable();
			if (bufferVariable == null)
				return null;
			if (elementAccess.Arguments.Last() is not IdentifierExpression indexIdentifier
				|| indexIdentifier.GetILVariable() != indexVariable)
				return null;

			// Soundness: the loop counts 0..length-1 over exactly the inline array's length, so the
			// index is provably in range. Any other bound (or a non-inline-array buffer) is rejected.
			if (bufferVariable.Type.GetInlineArrayLength() is not int arrayLength)
				return null;
			if (m.Get<PrimitiveExpression>("length").Single().Value is not int loopBound || loopBound != arrayLength)
				return null;

			if (!VariableCanBeUsedAsForeachLocal(itemVariable, forStatement))
				return null;
			// The index is a pure counter: stored at init + increment, loaded at the condition,
			// the increment, and the element access; never captured by address.
			if (indexVariable.StoreCount != 2 || indexVariable.LoadCount != 3 || indexVariable.AddressCount != 0)
				return null;

			context.Step("Introduce foreach over inline array", forStatement);
			// Take the buffer reference for the `in` expression before dropping the element access.
			var inExpression = bufferIdentifier.Detach();
			// Reuse the loop body (preserving its annotations) after removing its leading
			// `item = <PrivateImplementationDetails>.InlineArrayElementRef(ref buffer, i)` statement.
			var body = (BlockStatement)forStatement.EmbeddedStatement;
			body.Statements.First().Remove();
			var foreachStmt = new ForeachStatement {
				VariableType = context.Settings.AnonymousTypes && itemVariable.Type.ContainsAnonymousType() ? new SimpleType("var") : context.TypeSystemAstBuilder.ConvertType(itemVariable.Type),
				VariableDesignation = new SingleVariableDesignation { Identifier = itemVariable.Name! },
				InExpression = inExpression,
				EmbeddedStatement = body.Detach()
			};
			foreachStmt.CopyAnnotationsFrom(forStatement);
			itemVariable.Kind = IL.VariableKind.ForeachLocal;
			foreachStmt.VariableDesignation.AddAnnotation(new ILVariableResolveResult(itemVariable, itemVariable.Type));
			forStatement.ReplaceWith(foreachStmt);
			context.EndStep(foreachStmt);
			return foreachStmt;
		}

		static readonly ForStatement forOnArrayMultiDimPattern = new ForStatement {
			Initializers = { },
			Condition = new BinaryOperatorExpression(
				new NamedNode("indexVariable", new IdentifierExpression(Pattern.AnyString)),
				BinaryOperatorType.LessThanOrEqual,
				new NamedNode("upperBoundVariable", new IdentifierExpression(Pattern.AnyString))
			),
			Iterators = {
				new ExpressionStatement(
				new AssignmentExpression(
					new IdentifierExpressionBackreference("indexVariable"),
					new BinaryOperatorExpression(new IdentifierExpressionBackreference("indexVariable"), BinaryOperatorType.Add, new PrimitiveExpression(1))
				))
			},
			EmbeddedStatement = new BlockStatement { Statements = { new AnyNode("lowerBoundAssign"), new Repeat(new AnyNode("statements")) } }
		};

		/// <summary>
		/// $variable = $collection.GetUpperBound($index);
		/// </summary>
		static readonly AstNode variableAssignUpperBoundPattern = new ExpressionStatement(
			new AssignmentExpression(
				new NamedNode("variable", new IdentifierExpression(Pattern.AnyString)),
				new InvocationExpression(
					new MemberReferenceExpression(
						new NamedNode("collection", new IdentifierExpression(Pattern.AnyString)),
						"GetUpperBound"
					),
					new NamedNode("index", new PrimitiveExpression(PrimitiveExpression.AnyValue))
			)));

		/// <summary>
		/// $variable = $collection.GetLowerBound($index);
		/// </summary>
		static readonly ExpressionStatement variableAssignLowerBoundPattern = new ExpressionStatement(
			new AssignmentExpression(
				new NamedNode("variable", new IdentifierExpression(Pattern.AnyString)),
				new InvocationExpression(
					new MemberReferenceExpression(
						new NamedNode("collection", new IdentifierExpression(Pattern.AnyString)),
						"GetLowerBound"
					),
					new NamedNode("index", new PrimitiveExpression(PrimitiveExpression.AnyValue))
			)));

		/// <summary>
		/// $variable = $collection[$index1, $index2, ...];
		/// </summary>
		static readonly ExpressionStatement foreachVariableOnMultArrayAssignPattern = new ExpressionStatement(
			new AssignmentExpression(
				new NamedNode("variable", new IdentifierExpression(Pattern.AnyString)),
				new IndexerExpression(
					new NamedNode("collection", new IdentifierExpression(Pattern.AnyString)),
					new Repeat(new NamedNode("index", new IdentifierExpression(Pattern.AnyString))
				)
			)));

		bool MatchLowerBound(int indexNum, [NotNullWhen(true)] out IL.ILVariable? index, IL.ILVariable collection, Statement statement)
		{
			index = null;
			var m = variableAssignLowerBoundPattern.Match(statement);
			if (!m.Success)
				return false;
			if (!int.TryParse(m.Get<PrimitiveExpression>("index").Single().Value.ToString(), out int i) || indexNum != i)
				return false;
			index = m.Get<IdentifierExpression>("variable").Single().GetILVariable();
			return m.Get<IdentifierExpression>("collection").Single().GetILVariable() == collection;
		}

		bool MatchForeachOnMultiDimArray(IL.ILVariable[] upperBounds, IL.ILVariable collection, Statement firstInitializerStatement, [NotNullWhen(true)] out IdentifierExpression? foreachVariable, [NotNullWhen(true)] out IList<Statement>? statements, out IL.ILVariable[] lowerBounds)
		{
			int i = 0;
			foreachVariable = null;
			statements = null;
			lowerBounds = new IL.ILVariable[upperBounds.Length];
			Statement stmt = firstInitializerStatement;
			Match m = default(Match);
			while (i < upperBounds.Length && MatchLowerBound(i, out var indexVariable, collection, stmt))
			{
				m = forOnArrayMultiDimPattern.Match(stmt.GetNextStatement());
				if (!m.Success)
					return false;
				var upperBound = m.Get<IdentifierExpression>("upperBoundVariable").Single().GetILVariable();
				if (upperBounds[i] != upperBound)
					return false;
				stmt = m.Get<Statement>("lowerBoundAssign").Single();
				lowerBounds[i] = indexVariable;
				i++;
			}
			if (collection.Type.Kind != TypeKind.Array)
				return false;
			var m2 = foreachVariableOnMultArrayAssignPattern.Match(stmt);
			if (!m2.Success)
				return false;
			var collection2 = m2.Get<IdentifierExpression>("collection").Single().GetILVariable();
			if (collection2 != collection)
				return false;
			foreachVariable = m2.Get<IdentifierExpression>("variable").Single();
			statements = m.Get<Statement>("statements").ToList();
			return true;
		}

		Statement? TransformForeachOnMultiDimArray(ExpressionStatement expressionStatement)
		{
			if (!context.Settings.ForEachStatement)
				return null;
			Match m;
			Statement? stmt = expressionStatement;
			IL.ILVariable? collection = null;
			IL.ILVariable[]? upperBounds = null;
			List<Statement> statementsToDelete = new List<Statement>();
			int i = 0;
			// first we look for all the upper bound initializations
			do
			{
				m = variableAssignUpperBoundPattern.Match(stmt);
				if (!m.Success)
					break;
				if (upperBounds == null)
				{
					collection = m.Get<IdentifierExpression>("collection").Single().GetILVariable();
					if (!(collection?.Type is Decompiler.TypeSystem.ArrayType arrayType))
						break;
					upperBounds = new IL.ILVariable[arrayType.Dimensions];
				}
				else
				{
					statementsToDelete.Add(stmt);
				}
				var nextCollection = m.Get<IdentifierExpression>("collection").Single().GetILVariable();
				if (nextCollection != collection)
					break;
				if (!int.TryParse(m.Get<PrimitiveExpression>("index").Single().Value?.ToString() ?? "", out int index) || index != i)
					break;
				upperBounds[i] = m.Get<IdentifierExpression>("variable").Single().GetILVariable()!;
				stmt = stmt.GetNextStatement();
				i++;
			} while (stmt != null && upperBounds != null && i < upperBounds.Length);

			if (upperBounds?.LastOrDefault() == null || collection == null || stmt == null)
				return null;
			if (!MatchForeachOnMultiDimArray(upperBounds, collection, stmt, out var foreachVariable, out var statements, out var lowerBounds))
				return null;
			statementsToDelete.Add(stmt);
			// The matched multi-dimensional foreach pattern guarantees a statement after stmt.
			statementsToDelete.Add(stmt.GetNextStatement()!);
			var itemVariable = foreachVariable.GetILVariable();
			if (itemVariable == null || !itemVariable.IsSingleDefinition
				|| (itemVariable.Kind != IL.VariableKind.Local && itemVariable.Kind != IL.VariableKind.StackSlot)
				|| (context.Settings.UserDefinedCompoundAssignmentOperators
					&& IL.UserDefinedCompoundAssign.IsCompoundAssignmentReceiver(itemVariable))
				|| !upperBounds.All(ub => ub.IsSingleDefinition && ub.LoadCount == 1)
				|| !lowerBounds.All(lb => lb.StoreCount == 2 && lb.LoadCount == 3 && lb.AddressCount == 0))
				return null;
			context.Step("Introduce foreach over multidimensional array", expressionStatement);
			var body = new BlockStatement();
			foreach (var statement in statements)
				body.Statements.Add(statement.Detach());
			var foreachStmt = new ForeachStatement {
				VariableType = context.Settings.AnonymousTypes && itemVariable.Type.ContainsAnonymousType() ? new SimpleType("var") : context.TypeSystemAstBuilder.ConvertType(itemVariable.Type),
				VariableDesignation = new SingleVariableDesignation { Identifier = itemVariable.Name! },
				InExpression = m.Get<IdentifierExpression>("collection").Single().Detach(),
				EmbeddedStatement = body
			};
			foreach (var statement in statementsToDelete)
				statement.Detach();
			//foreachStmt.CopyAnnotationsFrom(forStatement);
			itemVariable.Kind = IL.VariableKind.ForeachLocal;
			// Add the variable annotation for highlighting (TokenTextWriter expects it directly on the ForeachStatement).
			foreachStmt.VariableDesignation.AddAnnotation(new ILVariableResolveResult(itemVariable, itemVariable.Type));
			// TODO : add ForeachAnnotation
			expressionStatement.ReplaceWith(foreachStmt);
			context.EndStep(foreachStmt);
			return foreachStmt;
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
				}
			}
		};

		static readonly PropertyDeclaration automaticReadonlyPropertyPattern = new PropertyDeclaration {
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
			}
		};

		bool CanTransformToAutomaticProperty(IProperty property, bool accessorsMustBeCompilerGenerated)
		{
			if (!property.CanGet)
				return false;
			if (accessorsMustBeCompilerGenerated && !property.Getter.IsCompilerGenerated())
				return false;
			if (property.Setter is IMethod setter)
			{
				if (accessorsMustBeCompilerGenerated && !setter.IsCompilerGenerated())
					return false;
				if (setter.HasReadonlyModifier())
					return false;
			}
			return true;
		}

		PropertyDeclaration? TransformAutomaticProperty(PropertyDeclaration propertyDeclaration)
		{
			IProperty? property = propertyDeclaration.GetSymbol() as IProperty;
			if (property == null)
				return null;
			if (context.Settings.FieldKeyword)
				return TransformFieldBackedProperty(propertyDeclaration, property);
			if (!CanTransformToAutomaticProperty(property, !(property.DeclaringTypeDefinition?.Fields.Any(f => f.Name == "_" + property.Name && f.IsCompilerGenerated()) ?? false)))
				return null;
			IField? field = null;
			Match m = automaticPropertyPattern.Match(propertyDeclaration);
			if (m.Success)
			{
				field = m.Get<AstNode>("fieldReference").Single().GetSymbol() as IField;
			}
			else
			{
				Match m2 = automaticReadonlyPropertyPattern.Match(propertyDeclaration);
				if (m2.Success)
				{
					field = m2.Get<AstNode>("fieldReference").Single().GetSymbol() as IField;
				}
			}
			if (field == null || !NameCouldBeBackingFieldOfAutomaticProperty(field.Name, out _))
				return null;
			// In generic types the accessor bodies reference the field specialized by the
			// type's own type parameters; the field declaration's symbol is the definition.
			field = (IField)field.MemberDefinition;
			if (propertyDeclaration.Setter?.HasModifier(Modifiers.Readonly) == true || (propertyDeclaration.HasModifier(Modifiers.Readonly) && propertyDeclaration.Setter is not null))
				return null;
			if (field.IsCompilerGenerated() && field.DeclaringTypeDefinition == property.DeclaringTypeDefinition)
			{
				context.Step("Convert property to auto-property", propertyDeclaration);
				// Clearing the accessor body turns it into an auto-property accessor.
				var getter = propertyDeclaration.Getter;
				var setter = propertyDeclaration.Setter;
				if (getter is not null)
				{
					RemoveCompilerGeneratedAttribute(getter.Attributes);
					getter.Body = null;
				}
				if (setter is not null)
				{
					RemoveCompilerGeneratedAttribute(setter.Attributes);
					setter.Body = null;
				}
				propertyDeclaration.Modifiers &= ~Modifiers.Readonly;
				if (getter is not null)
					getter.Modifiers &= ~Modifiers.Readonly;

				var fieldDecl = propertyDeclaration.Parent?.Children.OfType<FieldDeclaration>()
					.FirstOrDefault(fd => field.Equals(fd.GetSymbol()));
				if (fieldDecl != null)
				{
					fieldDecl.Remove();
					// Add C# 7.3 attributes on backing field:
					CSharpDecompiler.RemoveAttribute(fieldDecl, KnownAttribute.CompilerGenerated);
					CSharpDecompiler.RemoveAttribute(fieldDecl, KnownAttribute.DebuggerBrowsable);
					foreach (var section in fieldDecl.Attributes)
					{
						section.AttributeTarget = "field";
						propertyDeclaration.Attributes.Add(section.Detach());
					}
				}
			}
			// Since the property instance is not changed, we can continue in the visitor as usual, so return null
			return null;
		}

		static readonly BlockStatement trivialFieldGetterBody = new BlockStatement {
			new ReturnStatement {
				Expression = new NamedNode("fieldReference", new IdentifierExpression("field"))
			}
		};

		static readonly BlockStatement trivialFieldSetterBody = new BlockStatement {
			new AssignmentExpression {
				Left = new NamedNode("fieldReference", new IdentifierExpression("field")),
				Right = new IdentifierExpression("value")
			}
		};

		/// <summary>
		/// Handles all field-backed properties when the C# 14 "field" keyword is available:
		/// accessor bodies already refer to the backing field as "field" (see
		/// ExpressionBuilder.ConvertField), so compiler-generated trivial accessors collapse
		/// individually to "get;"/"set;" and the backing-field declaration disappears, with its
		/// attributes re-hosted as "field:" sections on the property.
		/// </summary>
		PropertyDeclaration? TransformFieldBackedProperty(PropertyDeclaration propertyDeclaration, IProperty property)
		{
			if (!TryGetBackingField(property, out var field))
				return null;
			if (!OutsideReferencesAreExpressible(propertyDeclaration, field))
			{
				// The field stays declared, so the "field" keyword references emitted by
				// ExpressionBuilder.ConvertField have to become ordinary field references again.
				foreach (var identifierExpression in propertyDeclaration.Descendants.OfType<IdentifierExpression>())
				{
					if (identifierExpression.Identifier == "field"
						&& identifierExpression.GetSymbol() is IField referencedField
						&& field.Equals(referencedField.MemberDefinition))
					{
						identifierExpression.Identifier = field.Name;
					}
				}
				return null;
			}
			context.Step("Transform field-backed property", propertyDeclaration);
			var getter = propertyDeclaration.Getter;
			var setter = propertyDeclaration.Setter;
			if (context.Settings.AutomaticProperties)
			{
				// VB auto-properties do not mark their accessors [CompilerGenerated]; the
				// pre-C# 14 transform recognizes them by their "_<PropertyName>" backing field
				// instead. Keep that rule, or a VB auto-property grows explicit "field"
				// accessors where every other compiler's collapses to "{ get; set; }".
				bool accessorsMustBeCompilerGenerated = field.Name != "_" + property.Name;
				CollapseTrivialAccessor(getter, trivialFieldGetterBody, field, accessorsMustBeCompilerGenerated);
				// A readonly setter cannot become an auto-accessor (same rule as the pre-C# 14
				// transform); readonly getters collapse fine because auto-getters are
				// implicitly readonly.
				if (setter?.HasModifier(Modifiers.Readonly) != true
					&& !(propertyDeclaration.HasModifier(Modifiers.Readonly) && setter is not null))
				{
					CollapseTrivialAccessor(setter, trivialFieldSetterBody, field, accessorsMustBeCompilerGenerated);
				}
			}
			if (getter?.Body == null && setter?.Body == null)
			{
				// The property became a full auto-property; readonly is implied like before.
				propertyDeclaration.Modifiers &= ~Modifiers.Readonly;
				if (getter is not null)
					getter.Modifiers &= ~Modifiers.Readonly;
			}
			var fieldDecl = propertyDeclaration.Parent?.Children.OfType<FieldDeclaration>()
				.FirstOrDefault(fd => field.Equals(fd.GetSymbol()));
			if (fieldDecl != null)
			{
				fieldDecl.Remove();
				CSharpDecompiler.RemoveAttribute(fieldDecl, KnownAttribute.CompilerGenerated);
				CSharpDecompiler.RemoveAttribute(fieldDecl, KnownAttribute.DebuggerBrowsable);
				foreach (var section in fieldDecl.Attributes)
				{
					section.AttributeTarget = "field";
					propertyDeclaration.Attributes.Add(section.Detach());
				}
			}
			return null;
		}

		void CollapseTrivialAccessor(Accessor? accessor, BlockStatement pattern, IField field,
			bool accessorMustBeCompilerGenerated)
		{
			if (accessor?.Body is null)
				return;
			if (accessorMustBeCompilerGenerated
				&& (accessor.GetSymbol() is not IMethod method || !method.IsCompilerGenerated()))
			{
				return;
			}
			Match m = pattern.Match(accessor.Body);
			if (!m.Success)
				return;
			if (m.Get<AstNode>("fieldReference").Single().GetSymbol() is not IField referencedField
				|| !field.Equals(referencedField.MemberDefinition))
			{
				return;
			}
			RemoveCompilerGeneratedAttribute(accessor.Attributes);
			// Auto-accessors are implicitly readonly.
			accessor.Modifiers &= ~Modifiers.Readonly;
			// Clearing the accessor body turns it into an auto-property accessor.
			accessor.Body = null;
		}

		internal static bool TryGetBackingField(IProperty property, [NotNullWhen(true)] out IField? field)
		{
			field = null;
			if (property.Parameters.Count > 0 || property.DeclaringTypeDefinition == null)
				return false;
			// A type definition's fields are unspecialized, so compare against the property
			// DEFINITION's return type; a specialized property in a generic type would
			// otherwise never match its own backing field.
			var propertyType = ((IProperty)property.MemberDefinition).ReturnType;
			foreach (var candidate in property.DeclaringTypeDefinition.Fields)
			{
				if (candidate.IsCompilerGenerated()
					&& candidate.IsStatic == property.IsStatic
					// The trivial accessor bodies of a classic auto-property guaranteed this
					// structurally; arbitrary accessor bodies do not. A field of a different
					// type is not this property's storage, and removing it while printing
					// `field` would substitute storage of the property's type instead.
					&& candidate.Type.Equals(propertyType)
					&& NameCouldBeBackingFieldOfAutomaticProperty(candidate.Name, out var propertyName)
					&& propertyName == property.Name)
				{
					field = candidate;
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// The "field" keyword cannot express backing-field accesses outside the owning
		/// property's accessors. The only shapes C# can express are stores in a constructor of
		/// the declaring type (property initializers, or assignments to setter-less
		/// properties); anything else means the field declaration has to be kept.
		/// </summary>
		bool OutsideReferencesAreExpressible(AstNode nodeInTree, IField field)
		{
			var root = nodeInTree.Ancestors.LastOrDefault() ?? nodeInTree;
			if (outsideReferenceRoot != root)
			{
				outsideReferenceRoot = root;
				outsideReferenceVerdicts = BuildOutsideReferenceIndex(root);
			}
			// Absent means no reference to this field was found outside its own property.
			return !outsideReferenceVerdicts!.TryGetValue((IField)field.MemberDefinition, out bool expressible)
				|| expressible;
		}

		// The verdict per backing field for one syntax tree. Answering each property with its
		// own full walk made whole-module output quadratic in the number of properties, which
		// a property-heavy assembly feels as minutes instead of seconds. Later transforms only
		// ever REMOVE references to the field of the property they are rewriting, so a verdict
		// computed up front stays valid for every other property in the tree.
		AstNode? outsideReferenceRoot;
		Dictionary<IField, bool>? outsideReferenceVerdicts;

		/// <summary>
		/// Whether a constructor store to <paramref name="field"/> still has a home once the
		/// field declaration is gone. Something has to turn it into an initializer or a
		/// property assignment, and not every store qualifies.
		/// </summary>
		bool StoreSurvivesAsInitializer(IField field)
		{
			// A setter-less property's store is rewritten to a property assignment by
			// ReplaceBackingFieldUsage, in this same transform - no later transform involved.
			if (IsBackingFieldOfAutomaticProperty(field, out var property) && !property.CanSet)
				return true;
			// Everything else waits for TransformFieldAndConstructorInitializers, which runs
			// after this transform and declines to move non-constant stores out of an EXPLICIT
			// static constructor (see its `onlyMoveConstants`). Removing the declaration here
			// would leave that store referencing a field that no longer exists.
			if (field.IsStatic && !context.Settings.AlwaysMoveInitializer
				&& !IsBeforeFieldInit(field.DeclaringTypeDefinition))
			{
				return false;
			}
			return true;
		}

		bool IsBeforeFieldInit(ITypeDefinition? typeDefinition)
		{
			if (typeDefinition?.MetadataToken.IsNil != false)
				return false;
			var metadata = context.TypeSystem.MainModule.MetadataFile.Metadata;
			var td = metadata.GetTypeDefinition((TypeDefinitionHandle)typeDefinition.MetadataToken);
			return td.HasFlag(System.Reflection.TypeAttributes.BeforeFieldInit);
		}

		Dictionary<IField, bool> BuildOutsideReferenceIndex(AstNode root)
		{
			var verdicts = new Dictionary<IField, bool>();
			foreach (var node in root.Descendants)
			{
				if (node is not (IdentifierExpression or MemberReferenceExpression))
					continue;
				if (node.GetSymbol() is not IField referencedField)
					continue;
				var definition = (IField)referencedField.MemberDefinition;
				// A reference inside the accessors of the field's own property is exactly what
				// the "field" keyword expresses; anything else is an outside reference.
				if (node.Ancestors.OfType<PropertyDeclaration>().FirstOrDefault()?.GetSymbol() is IProperty owner
					&& TryGetBackingField(owner, out var ownerField)
					&& definition.Equals(ownerField.MemberDefinition))
				{
					continue;
				}
				var enclosingMethod = node.Ancestors.OfType<EntityDeclaration>().FirstOrDefault()?.GetSymbol() as IMethod;
				if (!IsConstructorStore(node, definition, enclosingMethod) || !StoreSurvivesAsInitializer(definition))
					verdicts[definition] = false;
				else if (!verdicts.ContainsKey(definition))
					verdicts[definition] = true;
			}
			return verdicts;
		}

		static void RemoveCompilerGeneratedAttribute(AstNodeCollection<AttributeSection> attributeSections)
		{
			RemoveCompilerGeneratedAttribute(attributeSections, "System.Runtime.CompilerServices.CompilerGeneratedAttribute");
		}

		static void RemoveCompilerGeneratedAttribute(AstNodeCollection<AttributeSection> attributeSections, params string[] attributesToRemove)
		{
			foreach (AttributeSection section in attributeSections)
			{
				foreach (var attr in section.Attributes)
				{
					var tr = attr.Type.GetSymbol() as IType;
					if (tr != null && attributesToRemove.Contains(tr.FullName))
					{
						attr.Remove();
					}
				}
				if (section.Attributes.Count == 0)
					section.Remove();
			}
		}
		#endregion

		public override AstNode VisitIdentifier(Identifier identifier)
		{
			if (context.Settings.AutomaticProperties || context.Settings.FieldKeyword)
			{
				var newIdentifier = ReplaceBackingFieldUsage(identifier);
				if (newIdentifier != null)
				{
					identifier.ReplaceWith(newIdentifier);
					context.EndStep(newIdentifier);
					return newIdentifier;
				}
			}
			return base.VisitIdentifier(identifier);
		}

		internal static bool IsBackingFieldOfAutomaticProperty(IField field, [NotNullWhen(true)] out IProperty? property)
		{
			property = null;
			if (!NameCouldBeBackingFieldOfAutomaticProperty(field.Name, out var propertyName))
				return false;
			if (!field.IsCompilerGenerated())
				return false;
			property = field.DeclaringTypeDefinition?
				.GetProperties(p => p.Name == propertyName, GetMemberOptions.IgnoreInheritedMembers)
				.FirstOrDefault();
			return property != null;
		}

		/// <summary>
		/// This matches the following patterns
		/// <list type="bullet">
		///		<item>&lt;Property&gt;k__BackingField (used by C#)</item>
		///		<item>_Property (used by VB)</item>
		/// </list>
		/// </summary>
		static readonly System.Text.RegularExpressions.Regex automaticPropertyBackingFieldNameRegex
			= new System.Text.RegularExpressions.Regex(@"^(<(?<name>.+)>k__BackingField|_(?<name>.+))$");

		internal static bool NameCouldBeBackingFieldOfAutomaticProperty(string name, [NotNullWhen(true)] out string? propertyName)
		{
			propertyName = null;
			var m = automaticPropertyBackingFieldNameRegex.Match(name);
			if (!m.Success)
				return false;
			propertyName = m.Groups["name"].Value;
			return true;
		}

		Identifier? ReplaceBackingFieldUsage(Identifier identifier)
		{
			if (NameCouldBeBackingFieldOfAutomaticProperty(identifier.Name, out _))
			{
				var parent = identifier.Parent;
				if (parent == null)
					return null;
				var mrr = parent.Annotation<MemberResolveResult>();
				if (mrr?.Member is IField field && IsBackingFieldOfAutomaticProperty(field, out var property)
					&& currentMethod?.AccessorOwner != property)
				{
					if (CanTransformToAutomaticProperty(property, !(field.IsCompilerGenerated() && field.Name == "_" + property.Name)))
					{
						if (!property.CanSet && !context.Settings.GetterOnlyAutomaticProperties && !context.Settings.FieldKeyword)
							return null;
					}
					else if (context.Settings.FieldKeyword && !property.CanSet && IsConstructorStoreTarget(parent, field)
						&& BackingFieldWillBeRemoved(property, field, parent))
					{
						// A direct store to the backing field of a setter-less field-backed
						// property is expressible as a property assignment in a constructor -
						// but only where the property declaration actually becomes field-backed.
						// If TransformFieldBackedProperty bails, the property keeps explicit
						// accessors and no setter, so assigning it would not compile (CS0200).
					}
					else
					{
						// Stores that initialize a field-backed property with a setter are left
						// as field references and lifted into the property initializer by
						// TransformFieldAndConstructorInitializers (a property assignment would
						// invoke the setter); everything else is inexpressible with the "field"
						// keyword and keeps the field declared.
						return null;
					}
					context.Step("Replace backing field use with property", identifier);
					parent.RemoveAnnotations<MemberResolveResult>();
					parent.AddAnnotation(new MemberResolveResult(mrr.TargetResult, property));
					return Identifier.Create(property.Name);
				}
			}
			return null;
		}

		bool IsConstructorStoreTarget(AstNode node, IField field)
			=> IsConstructorStore(node, field, currentMethod);

		/// <summary>
		/// Whether <see cref="TransformFieldBackedProperty"/> will remove this field's
		/// declaration. Rewriting a store before knowing that produces an assignment to a
		/// property that keeps explicit, setter-less accessors.
		/// </summary>
		bool BackingFieldWillBeRemoved(IProperty property, IField field, AstNode nodeInTree)
		{
			return TryGetBackingField(property, out var backingField)
				&& field.MemberDefinition.Equals(backingField.MemberDefinition)
				&& OutsideReferencesAreExpressible(nodeInTree, backingField);
		}

		/// <summary>
		/// True when <paramref name="node"/> is the left-hand side of a plain assignment to
		/// <paramref name="field"/> inside a constructor of the field's declaring type - the
		/// only outside reference the "field" keyword can still express (as a property
		/// initializer, or an assignment to a setter-less property).
		/// </summary>
		/// <remarks>
		/// Shared by <see cref="OutsideReferencesAreExpressible"/>, which decides whether the
		/// field declaration may be removed, and <see cref="ReplaceBackingFieldUsage"/>, which
		/// rewrites the store. The two must agree: if only one of them accepts a store, the
		/// output either references a removed field or assigns a property that stayed
		/// field-backed. They differ only in how the enclosing method is known, which is why
		/// it is a parameter here.
		/// </remarks>
		static bool IsConstructorStore(AstNode node, IField field, IMethod? enclosingMethod)
		{
			if (node.Parent is not AssignmentExpression { Operator: AssignmentOperatorType.Assign } assignment
				|| assignment.Left != node)
			{
				return false;
			}
			return enclosingMethod is { IsConstructor: true } ctor
				&& ctor.IsStatic == field.IsStatic
				&& ctor.DeclaringTypeDefinition == field.DeclaringTypeDefinition;
		}

		#region Automatic Events
		internal static readonly string[] attributeTypesToRemoveFromAutoProperties = new[] {
			"System.Runtime.CompilerServices.CompilerGeneratedAttribute",
			"System.Diagnostics.DebuggerBrowsableAttribute"
		};

		static bool IsEventBackingFieldDeclaration(FieldDeclaration fd, IEvent ev)
		{
			if (fd.Variables.Count > 1)
				return false;
			if (fd.GetSymbol() is not IField f)
				return false;
			if (f.ParentModule is not MetadataModule module)
				return false;
			return f.Accessibility == Accessibility.Private
				&& ev.ReturnType.Equals(f.ReturnType)
				&& module.MetadataFile.PropertyAndEventBackingFieldLookup.IsEventBackingField((FieldDefinitionHandle)f.MetadataToken, out _);
		}
		#endregion

		#region Destructor
		static readonly BlockStatement destructorBodyPattern = new BlockStatement {
			new TryCatchStatement {
				TryBlock = new AnyNode("body"),
				FinallyBlock = new BlockStatement {
					new InvocationExpression(new MemberReferenceExpression(new BaseReferenceExpression(), "Finalize"))
				}
			}
		};

		static readonly MethodDeclaration destructorPattern = new MethodDeclaration {
			Attributes = { new Repeat(new AnyNode()) },
			Modifiers = Modifiers.Any,
			ReturnType = new PrimitiveType("void"),
			Name = "Finalize",
			Body = destructorBodyPattern
		};

		DestructorDeclaration? TransformDestructor(MethodDeclaration methodDef)
		{
			Match m = destructorPattern.Match(methodDef);
			if (m.Success)
			{
				context.Step("Convert Finalize method to destructor", methodDef);
				DestructorDeclaration dd = new DestructorDeclaration();
				methodDef.Attributes.MoveTo(dd.Attributes);
				dd.CopyAnnotationsFrom(methodDef);
				dd.Modifiers = methodDef.Modifiers & ~(Modifiers.Protected | Modifiers.Override);
				dd.Body = m.Get<BlockStatement>("body").Single().Detach();
				// A destructor only appears inside a type declaration, so the context tracker
				// has an enclosing type at this point.
				dd.Name = currentTypeDefinition!.Name;
				methodDef.ReplaceWith(dd);
				context.EndStep(dd);
				return dd;
			}
			return null;
		}

		DestructorDeclaration? TransformDestructorBody(DestructorDeclaration dtorDef)
		{
			Match m = destructorBodyPattern.Match(dtorDef.Body);
			if (m.Success)
			{
				context.Step("Simplify destructor body", dtorDef);
				dtorDef.Body = m.Get<BlockStatement>("body").Single().Detach();
				return dtorDef;
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
		TryCatchStatement? TransformTryCatchFinally(TryCatchStatement tryFinally)
		{
			if (tryCatchFinallyPattern.IsMatch(tryFinally))
			{
				context.Step("Merge nested try-catch-finally", tryFinally);
				TryCatchStatement tryCatch = (TryCatchStatement)tryFinally.TryBlock.Statements.Single();
				tryFinally.TryBlock = tryCatch.TryBlock.Detach();
				tryCatch.CatchClauses.MoveTo(tryFinally.CatchClauses);
			}
			// Since the tryFinally instance is not changed, we can continue in the visitor as usual, so return null
			return null;
		}
		#endregion

		#region Simplify cascading if-else-if statements
		static readonly IfElseStatement cascadingIfElsePattern = new IfElseStatement {
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

		AstNode? SimplifyCascadingIfElseStatements(IfElseStatement node)
		{
			Match m = cascadingIfElsePattern.Match(node);
			if (m.Success)
			{
				context.Step("Simplify cascading if-else", node);
				IfElseStatement elseIf = m.Get<IfElseStatement>("nestedIfStatement").Single();
				node.FalseStatement = elseIf.Detach();
			}

			return null;
		}

		/// <summary>
		/// Use associativity of logic operators to avoid parentheses.
		/// </summary>
		public override AstNode VisitBinaryOperatorExpression(BinaryOperatorExpression expr)
		{
			switch (expr.Operator)
			{
				case BinaryOperatorType.ConditionalAnd:
				case BinaryOperatorType.ConditionalOr:
					// a && (b && c) ==> (a && b) && c
					var bAndC = expr.Right as BinaryOperatorExpression;
					if (bAndC != null && bAndC.Operator == expr.Operator)
					{
						context.Step("Reassociate conditional logic", expr);
						// make bAndC the parent and expr the child.
						// A conditional-and/or operator always has both operands present.
						var b = bAndC.Left!.Detach();
						var c = bAndC.Right!.Detach();
						expr.ReplaceWith(bAndC.Detach());
						bAndC.Left = expr;
						bAndC.Right = c;
						expr.Right = b;
						context.EndStep(bAndC);
						return base.VisitBinaryOperatorExpression(bAndC);
					}
					break;
			}
			return base.VisitBinaryOperatorExpression(expr);
		}

		public override AstNode VisitUnaryOperatorExpression(UnaryOperatorExpression expr)
		{
			if (expr.Operator == UnaryOperatorType.Not && expr.Expression is BinaryOperatorExpression { Operator: BinaryOperatorType.Equality } binary)
			{
				context.Step("Replace negated equality with inequality", expr);
				binary.Operator = BinaryOperatorType.InEquality;
				expr.ReplaceWith(binary.Detach());
				context.EndStep(binary);
				return VisitBinaryOperatorExpression(binary);
			}
			return base.VisitUnaryOperatorExpression(expr);
		}
		#endregion

		#region C# 7.3 pattern based fixed (for value types)
		// reference types are handled by DetectPinnedRegions.IsCustomRefPinPattern
		static readonly Expression addressOfPinnableReference = new UnaryOperatorExpression {
			Operator = UnaryOperatorType.AddressOf,
			Expression = new InvocationExpression {
				Target = new MemberReferenceExpression(new AnyNode("target"), "GetPinnableReference"),
				Arguments = { }
			}
		};

		public override AstNode VisitFixedStatement(FixedStatement fixedStatement)
		{
			if (context.Settings.PatternBasedFixedStatement)
			{
				foreach (var v in fixedStatement.Variables)
				{
					var m = addressOfPinnableReference.Match(v.Initializer);
					if (m.Success)
					{
						Expression target = m.Get<Expression>("target").Single();
						if (target.GetResolveResult().Type.IsReferenceType == false)
						{
							context.Step("Use pattern-based fixed statement", fixedStatement);
							v.Initializer = target.Detach();
						}
					}
				}
			}
			return base.VisitFixedStatement(fixedStatement);
		}
		#endregion

		#region C# 8.0 Using variables
		public override AstNode VisitUsingStatement(UsingStatement usingStatement)
		{
			usingStatement = (UsingStatement)base.VisitUsingStatement(usingStatement);
			if (!context.Settings.UseEnhancedUsing)
				return usingStatement;

			if (usingStatement.GetNextStatement() != null || !(usingStatement.Parent is BlockStatement))
				return usingStatement;

			if (!(usingStatement.ResourceAcquisition is VariableDeclarationStatement))
				return usingStatement;

			context.Step("Use enhanced using statement", usingStatement);
			usingStatement.IsEnhanced = true;
			return usingStatement;
		}
		#endregion
	}
}
