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
			for (AstNode child = node.FirstChild; child != null; child = child.NextSibling)
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
			AstNode result = TransformForeachOnMultiDimArray(expressionStatement);
			if (result != null)
				return result;
			result = TransformFor(expressionStatement);
			if (result != null)
				return result;
			return base.VisitExpressionStatement(expressionStatement);
		}

		public override AstNode VisitForStatement(ForStatement forStatement)
		{
			AstNode result = TransformForeachOnArray(forStatement);
			if (result != null)
				return result;
			return base.VisitForStatement(forStatement);
		}

		public override AstNode VisitIfElseStatement(IfElseStatement ifElseStatement)
		{
			AstNode simplifiedIfElse = SimplifyCascadingIfElseStatements(ifElseStatement);
			if (simplifiedIfElse != null)
				return simplifiedIfElse;
			return base.VisitIfElseStatement(ifElseStatement);
		}

		public override AstNode VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration)
		{
			if (context.Settings.AutomaticProperties
				&& (!propertyDeclaration.Setter.IsNull || context.Settings.GetterOnlyAutomaticProperties))
			{
				AstNode result = TransformAutomaticProperty(propertyDeclaration);
				if (result != null)
					return result;
			}
			return base.VisitPropertyDeclaration(propertyDeclaration);
		}

		public override AstNode VisitCustomEventDeclaration(CustomEventDeclaration eventDeclaration)
		{
			// first apply transforms to the accessor bodies
			base.VisitCustomEventDeclaration(eventDeclaration);
			if (context.Settings.AutomaticEvents)
			{
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

		public ForStatement TransformFor(ExpressionStatement node)
		{
			if (!context.Settings.ForStatement)
				return null;
			Match m1 = variableAssignPattern.Match(node);
			if (!m1.Success)
				return null;
			var variable = m1.Get<IdentifierExpression>("variable").Single().GetILVariable();
			AstNode next = node.NextSibling;
			if (next is ForStatement forStatement && ForStatementUsesVariable(forStatement, variable))
			{
				node.Remove();
				next.InsertChildAfter(null, node, ForStatement.InitializerRole);
				return (ForStatement)next;
			}
			Match m3 = forPattern.Match(next);
			if (!m3.Success)
				return null;
			// ensure the variable in the for pattern is the same as in the declaration
			if (variable != m3.Get<IdentifierExpression>("ident").Single().GetILVariable())
				return null;
			WhileStatement loop = (WhileStatement)next;
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

		bool ForStatementUsesVariable(ForStatement statement, IL.ILVariable variable)
		{
			if (statement.Condition.DescendantsAndSelf.OfType<IdentifierExpression>().Any(ie => ie.GetILVariable() == variable))
				return true;
			if (statement.Iterators.Any(i => i.DescendantsAndSelf.OfType<IdentifierExpression>().Any(ie => ie.GetILVariable() == variable)))
				return true;
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

		static bool AddressUsedForSingleCall(IL.ILVariable v, IL.BlockContainer loop)
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

		Statement TransformForeachOnArray(ForStatement forStatement)
		{
			if (!context.Settings.ForEachStatement)
				return null;
			Match m = forOnArrayPattern.Match(forStatement);
			if (!m.Success)
				return null;
			var itemVariable = m.Get<IdentifierExpression>("itemVariable").Single().GetILVariable();
			var indexVariable = m.Get<IdentifierExpression>("indexVariable").Single().GetILVariable();
			var arrayVariable = m.Get<IdentifierExpression>("arrayVariable").Single().GetILVariable();
			var loopContainer = forStatement.Annotation<IL.BlockContainer>();
			if (itemVariable == null || indexVariable == null || arrayVariable == null)
				return null;
			if (arrayVariable.Type.Kind != TypeKind.Array)
				return null;
			if (!VariableCanBeUsedAsForeachLocal(itemVariable, forStatement))
				return null;
			if (indexVariable.StoreCount != 2 || indexVariable.LoadCount != 3 || indexVariable.AddressCount != 0)
				return null;
			var body = new BlockStatement();
			foreach (var statement in m.Get<Statement>("statements"))
				body.Statements.Add(statement.Detach());
			var foreachStmt = new ForeachStatement {
				VariableType = context.Settings.AnonymousTypes && itemVariable.Type.ContainsAnonymousType() ? new SimpleType("var") : context.TypeSystemAstBuilder.ConvertType(itemVariable.Type),
				VariableDesignation = new SingleVariableDesignation { Identifier = itemVariable.Name },
				InExpression = m.Get<IdentifierExpression>("arrayVariable").Single().Detach(),
				EmbeddedStatement = body
			};
			foreachStmt.CopyAnnotationsFrom(forStatement);
			itemVariable.Kind = IL.VariableKind.ForeachLocal;
			// Add the variable annotation for highlighting (TokenTextWriter expects it directly on the ForeachStatement).
			foreachStmt.VariableDesignation.AddAnnotation(new ILVariableResolveResult(itemVariable, itemVariable.Type));
			// TODO : add ForeachAnnotation
			forStatement.ReplaceWith(foreachStmt);
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

		bool MatchLowerBound(int indexNum, out IL.ILVariable index, IL.ILVariable collection, Statement statement)
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

		bool MatchForeachOnMultiDimArray(IL.ILVariable[] upperBounds, IL.ILVariable collection, Statement firstInitializerStatement, out IdentifierExpression foreachVariable, out IList<Statement> statements, out IL.ILVariable[] lowerBounds)
		{
			int i = 0;
			foreachVariable = null;
			statements = null;
			lowerBounds = new IL.ILVariable[upperBounds.Length];
			Statement stmt = firstInitializerStatement;
			Match m = default(Match);
			while (i < upperBounds.Length && MatchLowerBound(i, out IL.ILVariable indexVariable, collection, stmt))
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

		Statement TransformForeachOnMultiDimArray(ExpressionStatement expressionStatement)
		{
			if (!context.Settings.ForEachStatement)
				return null;
			Match m;
			Statement stmt = expressionStatement;
			IL.ILVariable collection = null;
			IL.ILVariable[] upperBounds = null;
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
				upperBounds[i] = m.Get<IdentifierExpression>("variable").Single().GetILVariable();
				stmt = stmt.GetNextStatement();
				i++;
			} while (stmt != null && i < upperBounds.Length);

			if (upperBounds?.LastOrDefault() == null || collection == null)
				return null;
			if (!MatchForeachOnMultiDimArray(upperBounds, collection, stmt, out var foreachVariable, out var statements, out var lowerBounds))
				return null;
			statementsToDelete.Add(stmt);
			statementsToDelete.Add(stmt.GetNextStatement());
			var itemVariable = foreachVariable.GetILVariable();
			if (itemVariable == null || !itemVariable.IsSingleDefinition
				|| (itemVariable.Kind != IL.VariableKind.Local && itemVariable.Kind != IL.VariableKind.StackSlot)
				|| !upperBounds.All(ub => ub.IsSingleDefinition && ub.LoadCount == 1)
				|| !lowerBounds.All(lb => lb.StoreCount == 2 && lb.LoadCount == 3 && lb.AddressCount == 0))
				return null;
			var body = new BlockStatement();
			foreach (var statement in statements)
				body.Statements.Add(statement.Detach());
			var foreachStmt = new ForeachStatement {
				VariableType = context.Settings.AnonymousTypes && itemVariable.Type.ContainsAnonymousType() ? new SimpleType("var") : context.TypeSystemAstBuilder.ConvertType(itemVariable.Type),
				VariableDesignation = new SingleVariableDesignation { Identifier = itemVariable.Name },
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

		bool CanTransformToAutomaticProperty(IProperty property)
		{
			if (!property.CanGet)
				return false;
			if (!property.Getter.IsCompilerGenerated())
				return false;
			if (property.Setter is IMethod setter)
			{
				if (!setter.IsCompilerGenerated())
					return false;
				if (setter.HasReadonlyModifier())
					return false;
			}
			return true;
		}

		PropertyDeclaration TransformAutomaticProperty(PropertyDeclaration propertyDeclaration)
		{
			IProperty property = propertyDeclaration.GetSymbol() as IProperty;
			if (!CanTransformToAutomaticProperty(property))
				return null;
			IField field = null;
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
			if (propertyDeclaration.Setter.HasModifier(Modifiers.Readonly) || (propertyDeclaration.HasModifier(Modifiers.Readonly) && !propertyDeclaration.Setter.IsNull))
				return null;
			if (field.IsCompilerGenerated() && field.DeclaringTypeDefinition == property.DeclaringTypeDefinition)
			{
				RemoveCompilerGeneratedAttribute(propertyDeclaration.Getter.Attributes);
				RemoveCompilerGeneratedAttribute(propertyDeclaration.Setter.Attributes);
				propertyDeclaration.Getter.Body = null;
				propertyDeclaration.Setter.Body = null;
				propertyDeclaration.Modifiers &= ~Modifiers.Readonly;
				propertyDeclaration.Getter.Modifiers &= ~Modifiers.Readonly;

				// Add C# 7.3 attributes on backing field:
				var attributes = field.GetAttributes()
					.Where(a => !attributeTypesToRemoveFromAutoProperties.Contains(a.AttributeType.FullName))
					.Select(context.TypeSystemAstBuilder.ConvertAttribute).ToArray();
				if (attributes.Length > 0)
				{
					var section = new AttributeSection {
						AttributeTarget = "field"
					};
					section.Attributes.AddRange(attributes);
					propertyDeclaration.Attributes.Add(section);
				}
			}
			// Since the property instance is not changed, we can continue in the visitor as usual, so return null
			return null;
		}

		void RemoveCompilerGeneratedAttribute(AstNodeCollection<AttributeSection> attributeSections)
		{
			RemoveCompilerGeneratedAttribute(attributeSections, "System.Runtime.CompilerServices.CompilerGeneratedAttribute");
		}

		void RemoveCompilerGeneratedAttribute(AstNodeCollection<AttributeSection> attributeSections, params string[] attributesToRemove)
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
			if (context.Settings.AutomaticProperties)
			{
				var newIdentifier = ReplaceBackingFieldUsage(identifier);
				if (newIdentifier != null)
				{
					identifier.ReplaceWith(newIdentifier);
					return newIdentifier;
				}
			}
			if (context.Settings.AutomaticEvents)
			{
				var newIdentifier = ReplaceEventFieldAnnotation(identifier);
				if (newIdentifier != null)
					return newIdentifier;
			}
			return base.VisitIdentifier(identifier);
		}

		internal static bool IsBackingFieldOfAutomaticProperty(IField field, out IProperty property)
		{
			property = null;
			if (!NameCouldBeBackingFieldOfAutomaticProperty(field.Name, out string propertyName))
				return false;
			if (!field.IsCompilerGenerated())
				return false;
			property = field.DeclaringTypeDefinition
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

		static bool NameCouldBeBackingFieldOfAutomaticProperty(string name, out string propertyName)
		{
			propertyName = null;
			var m = automaticPropertyBackingFieldNameRegex.Match(name);
			if (!m.Success)
				return false;
			propertyName = m.Groups["name"].Value;
			return true;
		}

		Identifier ReplaceBackingFieldUsage(Identifier identifier)
		{
			if (NameCouldBeBackingFieldOfAutomaticProperty(identifier.Name, out _))
			{
				var parent = identifier.Parent;
				var mrr = parent.Annotation<MemberResolveResult>();
				var field = mrr?.Member as IField;
				if (field != null && IsBackingFieldOfAutomaticProperty(field, out var property)
					&& CanTransformToAutomaticProperty(property) && currentMethod.AccessorOwner != property)
				{
					if (!property.CanSet && !context.Settings.GetterOnlyAutomaticProperties)
						return null;
					parent.RemoveAnnotations<MemberResolveResult>();
					parent.AddAnnotation(new MemberResolveResult(mrr.TargetResult, property));
					return Identifier.Create(property.Name);
				}
			}
			return null;
		}

		Identifier ReplaceEventFieldAnnotation(Identifier identifier)
		{
			var parent = identifier.Parent;
			var mrr = parent.Annotation<MemberResolveResult>();
			var field = mrr?.Member as IField;
			if (field == null)
				return null;
			var @event = field.DeclaringType.GetEvents(ev => ev.Name == field.Name, GetMemberOptions.IgnoreInheritedMembers).SingleOrDefault();
			if (@event != null && currentMethod.AccessorOwner != @event)
			{
				parent.RemoveAnnotations<MemberResolveResult>();
				parent.AddAnnotation(new MemberResolveResult(mrr.TargetResult, @event));
				return identifier;
			}
			return null;
		}

		#region Automatic Events
		static readonly Expression fieldReferencePattern = new Choice {
			new IdentifierExpression(Pattern.AnyString),
			new MemberReferenceExpression {
				Target = new Choice { new ThisReferenceExpression(), new TypeReferenceExpression { Type = new AnyNode() } },
				MemberName = Pattern.AnyString
			}
		};

		static readonly Accessor automaticEventPatternV2 = new Accessor {
			Attributes = { new Repeat(new AnyNode()) },
			Body = new BlockStatement {
				new AssignmentExpression {
					Left = new NamedNode("field", fieldReferencePattern),
					Operator = AssignmentOperatorType.Assign,
					Right = new CastExpression(
						new AnyNode("type"),
						new InvocationExpression(new AnyNode("delegateCombine").ToExpression(), new Backreference("field"), new IdentifierExpression("value"))
					)
				},
			}
		};

		static readonly Accessor automaticEventPatternV4 = new Accessor {
			Attributes = { new Repeat(new AnyNode()) },
			Body = new BlockStatement {
				new AssignmentExpression {
					Left = new NamedNode("var1", new IdentifierExpression(Pattern.AnyString)),
					Operator = AssignmentOperatorType.Assign,
					Right = new NamedNode("field", fieldReferencePattern)
				},
				new DoWhileStatement {
					EmbeddedStatement = new BlockStatement {
						new AssignmentExpression(new NamedNode("var2", new IdentifierExpression(Pattern.AnyString)), new IdentifierExpressionBackreference("var1")),
						new AssignmentExpression {
							Left = new NamedNode("var3", new IdentifierExpression(Pattern.AnyString)),
							Operator = AssignmentOperatorType.Assign,
							Right = new CastExpression(new AnyNode("type"), new InvocationExpression(new AnyNode("delegateCombine").ToExpression(), new IdentifierExpressionBackreference("var2"), new IdentifierExpression("value")))
						},
						new AssignmentExpression {
							Left = new IdentifierExpressionBackreference("var1"),
							Right = new InvocationExpression(new MemberReferenceExpression(new TypeReferenceExpression(new TypePattern(typeof(System.Threading.Interlocked)).ToType()),
								"CompareExchange"),
								new Expression[] { // arguments
									new DirectionExpression { FieldDirection = FieldDirection.Ref, Expression = new Backreference("field") },
									new IdentifierExpressionBackreference("var3"),
									new IdentifierExpressionBackreference("var2")
								}
							)}
					},
					Condition = new BinaryOperatorExpression {
						Left = new CastExpression(new TypePattern(typeof(object)), new IdentifierExpressionBackreference("var1")),
						Operator = BinaryOperatorType.InEquality,
						Right = new IdentifierExpressionBackreference("var2")
					},
				}
			}
		};

		static readonly Accessor automaticEventPatternV4AggressivelyInlined = new Accessor {
			Attributes = { new Repeat(new AnyNode()) },
			Body = new BlockStatement {
				new AssignmentExpression {
					Left = new NamedNode("var1", new IdentifierExpression(Pattern.AnyString)),
					Operator = AssignmentOperatorType.Assign,
					Right = new NamedNode("field", fieldReferencePattern)
				},
				new DoWhileStatement {
					EmbeddedStatement = new BlockStatement {
						new AssignmentExpression(new NamedNode("var2", new IdentifierExpression(Pattern.AnyString)), new IdentifierExpressionBackreference("var1")),
						new AssignmentExpression {
							Left = new IdentifierExpressionBackreference("var1"),
							Right = new InvocationExpression(new MemberReferenceExpression(new TypeReferenceExpression(new TypePattern(typeof(System.Threading.Interlocked)).ToType()),
								"CompareExchange"),
								new Expression[] { // arguments
									new NamedArgumentExpression("value", new CastExpression(new AnyNode("type"), new InvocationExpression(new AnyNode("delegateCombine").ToExpression(), new IdentifierExpressionBackreference("var2"), new IdentifierExpression("value")))),
									new NamedArgumentExpression("location1", new DirectionExpression { FieldDirection = FieldDirection.Ref, Expression = new Backreference("field") }),
									new NamedArgumentExpression("comparand", new IdentifierExpressionBackreference("var2"))
								}
							)}
					},
					Condition = new BinaryOperatorExpression {
						Left = new CastExpression(new TypePattern(typeof(object)), new IdentifierExpressionBackreference("var1")),
						Operator = BinaryOperatorType.InEquality,
						Right = new IdentifierExpressionBackreference("var2")
					},
				}
			}
		};

		static readonly Accessor automaticEventPatternV4MCS = new Accessor {
			Attributes = { new Repeat(new AnyNode()) },
			Body = new BlockStatement {
				new AssignmentExpression {
					Left = new NamedNode("var1", new IdentifierExpression(Pattern.AnyString)),
					Operator = AssignmentOperatorType.Assign,
					Right = new NamedNode(
						"field",
						new MemberReferenceExpression {
								Target = new Choice { new ThisReferenceExpression(), new TypeReferenceExpression { Type = new AnyNode() } },
								MemberName = Pattern.AnyString
						}
					)
				},
				new DoWhileStatement {
					EmbeddedStatement = new BlockStatement {
						new AssignmentExpression(new NamedNode("var2", new IdentifierExpression(Pattern.AnyString)), new IdentifierExpressionBackreference("var1")),
						new AssignmentExpression {
							Left = new IdentifierExpressionBackreference("var1"),
							Right = new InvocationExpression(new MemberReferenceExpression(new TypeReferenceExpression(new TypePattern(typeof(System.Threading.Interlocked)).ToType()),
								"CompareExchange",
								new AstType[] { new Repeat(new AnyNode()) }), // optional type arguments
								new Expression[] { // arguments
									new DirectionExpression { FieldDirection = FieldDirection.Ref, Expression = new Backreference("field") },
									new CastExpression(new AnyNode("type"), new InvocationExpression(new AnyNode("delegateCombine").ToExpression(), new IdentifierExpressionBackreference("var2"), new IdentifierExpression("value"))),
									new IdentifierExpressionBackreference("var1")
								}
							)
						}
					},
					Condition = new BinaryOperatorExpression {
						Left = new CastExpression(new TypePattern(typeof(object)), new IdentifierExpressionBackreference("var1")),
						Operator = BinaryOperatorType.InEquality,
						Right = new IdentifierExpressionBackreference("var2")
					},
				}
			}
		};

		bool CheckAutomaticEventMatch(Match m, CustomEventDeclaration ev, bool isAddAccessor)
		{
			if (!m.Success)
				return false;
			Expression fieldExpression = m.Get<Expression>("field").Single();
			// field name must match event name
			switch (fieldExpression)
			{
				case IdentifierExpression identifier:
					if (identifier.Identifier != ev.Name)
						return false;
					break;
				case MemberReferenceExpression memberRef:
					if (memberRef.MemberName != ev.Name)
						return false;
					break;
				default:
					return false;
			}
			var returnType = ev.ReturnType.GetResolveResult().Type;
			var eventType = m.Get<AstType>("type").Single().GetResolveResult().Type;
			// ignore tuple element names, dynamic and nullability
			if (!NormalizeTypeVisitor.TypeErasure.EquivalentTypes(returnType, eventType))
				return false;
			var combineMethod = m.Get<AstNode>("delegateCombine").Single().Parent.GetSymbol() as IMethod;
			if (combineMethod == null || combineMethod.Name != (isAddAccessor ? "Combine" : "Remove"))
				return false;
			return combineMethod.DeclaringType.FullName == "System.Delegate";
		}

		static readonly string[] attributeTypesToRemoveFromAutoEvents = new[] {
			"System.Runtime.CompilerServices.CompilerGeneratedAttribute",
			"System.Diagnostics.DebuggerBrowsableAttribute",
			"System.Runtime.CompilerServices.MethodImplAttribute"
		};

		static readonly string[] attributeTypesToRemoveFromAutoProperties = new[] {
			"System.Runtime.CompilerServices.CompilerGeneratedAttribute",
			"System.Diagnostics.DebuggerBrowsableAttribute"
		};

		bool CheckAutomaticEventV4(CustomEventDeclaration ev)
		{
			Match addMatch = automaticEventPatternV4.Match(ev.AddAccessor);
			if (!CheckAutomaticEventMatch(addMatch, ev, isAddAccessor: true))
				return false;
			Match removeMatch = automaticEventPatternV4.Match(ev.RemoveAccessor);
			if (!CheckAutomaticEventMatch(removeMatch, ev, isAddAccessor: false))
				return false;
			return true;
		}

		bool CheckAutomaticEventV4AggressivelyInlined(CustomEventDeclaration ev)
		{
			if (!context.Settings.AggressiveInlining)
				return false;
			Match addMatch = automaticEventPatternV4AggressivelyInlined.Match(ev.AddAccessor);
			if (!CheckAutomaticEventMatch(addMatch, ev, isAddAccessor: true))
				return false;
			Match removeMatch = automaticEventPatternV4AggressivelyInlined.Match(ev.RemoveAccessor);
			if (!CheckAutomaticEventMatch(removeMatch, ev, isAddAccessor: false))
				return false;
			return true;
		}

		bool CheckAutomaticEventV2(CustomEventDeclaration ev)
		{
			Match addMatch = automaticEventPatternV2.Match(ev.AddAccessor);
			if (!CheckAutomaticEventMatch(addMatch, ev, isAddAccessor: true))
				return false;
			Match removeMatch = automaticEventPatternV2.Match(ev.RemoveAccessor);
			if (!CheckAutomaticEventMatch(removeMatch, ev, isAddAccessor: false))
				return false;
			return true;
		}

		bool CheckAutomaticEventV4MCS(CustomEventDeclaration ev)
		{
			Match addMatch = automaticEventPatternV4MCS.Match(ev.AddAccessor);
			if (!CheckAutomaticEventMatch(addMatch, ev, true))
				return false;
			Match removeMatch = automaticEventPatternV4MCS.Match(ev.RemoveAccessor);
			if (!CheckAutomaticEventMatch(removeMatch, ev, false))
				return false;
			return true;
		}

		EventDeclaration TransformAutomaticEvents(CustomEventDeclaration ev)
		{
			if (!ev.PrivateImplementationType.IsNull)
				return null;
			const Modifiers withoutBody = Modifiers.Abstract | Modifiers.Extern;
			if ((ev.Modifiers & withoutBody) == 0 && ev.GetSymbol() is IEvent symbol && symbol.DeclaringType.Kind != TypeKind.Interface)
			{
				if (!CheckAutomaticEventV4AggressivelyInlined(ev) && !CheckAutomaticEventV4(ev) && !CheckAutomaticEventV2(ev) && !CheckAutomaticEventV4MCS(ev))
					return null;
			}
			RemoveCompilerGeneratedAttribute(ev.AddAccessor.Attributes, attributeTypesToRemoveFromAutoEvents);
			EventDeclaration ed = new EventDeclaration();
			ev.Attributes.MoveTo(ed.Attributes);
			foreach (var attr in ev.AddAccessor.Attributes)
			{
				attr.AttributeTarget = "method";
				ed.Attributes.Add(attr.Detach());
			}
			ed.ReturnType = ev.ReturnType.Detach();
			ed.Modifiers = ev.Modifiers;
			ed.Variables.Add(new VariableInitializer(ev.Name));
			ed.CopyAnnotationsFrom(ev);

			if (ev.GetSymbol() is IEvent eventDef)
			{
				IField field = eventDef.DeclaringType.GetFields(f => f.Name == ev.Name, GetMemberOptions.IgnoreInheritedMembers).SingleOrDefault();
				if (field != null)
				{
					ed.AddAnnotation(field);
					var attributes = field.GetAttributes()
							.Where(a => !attributeTypesToRemoveFromAutoEvents.Contains(a.AttributeType.FullName))
							.Select(context.TypeSystemAstBuilder.ConvertAttribute).ToArray();
					if (attributes.Length > 0)
					{
						var section = new AttributeSection {
							AttributeTarget = "field"
						};
						section.Attributes.AddRange(attributes);
						ed.Attributes.Add(section);
					}
				}
			}

			ev.ReplaceWith(ed);
			return ed;
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

		DestructorDeclaration TransformDestructor(MethodDeclaration methodDef)
		{
			Match m = destructorPattern.Match(methodDef);
			if (m.Success)
			{
				DestructorDeclaration dd = new DestructorDeclaration();
				methodDef.Attributes.MoveTo(dd.Attributes);
				dd.CopyAnnotationsFrom(methodDef);
				dd.Modifiers = methodDef.Modifiers & ~(Modifiers.Protected | Modifiers.Override);
				dd.Body = m.Get<BlockStatement>("body").Single().Detach();
				dd.Name = currentTypeDefinition?.Name;
				methodDef.ReplaceWith(dd);
				return dd;
			}
			return null;
		}

		DestructorDeclaration TransformDestructorBody(DestructorDeclaration dtorDef)
		{
			Match m = destructorBodyPattern.Match(dtorDef.Body);
			if (m.Success)
			{
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
		TryCatchStatement TransformTryCatchFinally(TryCatchStatement tryFinally)
		{
			if (tryCatchFinallyPattern.IsMatch(tryFinally))
			{
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

		AstNode SimplifyCascadingIfElseStatements(IfElseStatement node)
		{
			Match m = cascadingIfElsePattern.Match(node);
			if (m.Success)
			{
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
						// make bAndC the parent and expr the child
						var b = bAndC.Left.Detach();
						var c = bAndC.Right.Detach();
						expr.ReplaceWith(bAndC.Detach());
						bAndC.Left = expr;
						bAndC.Right = c;
						expr.Right = b;
						return base.VisitBinaryOperatorExpression(bAndC);
					}
					break;
			}
			return base.VisitBinaryOperatorExpression(expr);
		}
		#endregion

		#region C# 7.3 pattern based fixed
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
						v.Initializer = m.Get<Expression>("target").Single().Detach();
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

			usingStatement.IsEnhanced = true;
			return usingStatement;
		}
		#endregion
	}
}
