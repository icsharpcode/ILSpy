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
using Mono.Cecil;
using ICSharpCode.Decompiler.Semantics;

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
			try {
				this.context = context;
				base.Initialize(context);
				declareVariables.Analyze(rootNode);
				rootNode.AcceptVisitor(this);
			} finally {
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

		public ForStatement TransformFor(ExpressionStatement node)
		{
			Match m1 = variableAssignPattern.Match(node);
			if (!m1.Success) return null;
			var variable = m1.Get<IdentifierExpression>("variable").Single().GetILVariable();
			AstNode next = node.NextSibling;
			if (next is ForStatement forStatement && ForStatementUsesVariable(forStatement, variable)) {
				node.Remove();
				next.InsertChildAfter(null, node, ForStatement.InitializerRole);
				return (ForStatement)next;
			}
			Match m3 = forPattern.Match(next);
			if (!m3.Success) return null;
			// ensure the variable in the for pattern is the same as in the declaration
			if (variable != m3.Get<IdentifierExpression>("ident").Single().GetILVariable())
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

		bool ForStatementUsesVariable(ForStatement statement, IL.ILVariable variable)
		{
			if (statement.Condition.DescendantsAndSelf.OfType<IdentifierExpression>().Any(ie => ie.GetILVariable() == variable))
				return true;
			if (statement.Iterators.Any(i => i.DescendantsAndSelf.OfType<IdentifierExpression>().Any(ie => ie.GetILVariable() == variable)))
				return true;
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

		Statement TransformForeachOnArray(ForStatement forStatement)
		{
			Match m = forOnArrayPattern.Match(forStatement);
			if (!m.Success) return null;
			var itemVariable = m.Get<IdentifierExpression>("itemVariable").Single().GetILVariable();
			var indexVariable = m.Get<IdentifierExpression>("indexVariable").Single().GetILVariable();
			var arrayVariable = m.Get<IdentifierExpression>("arrayVariable").Single().GetILVariable();
			var loopContainer = forStatement.Annotation<IL.BlockContainer>();
			if (!itemVariable.IsSingleDefinition || (itemVariable.CaptureScope != null && itemVariable.CaptureScope != loopContainer))
				return null;
			if (indexVariable.StoreCount != 2 || indexVariable.LoadCount != 3 || indexVariable.AddressCount != 0)
				return null;
			var body = new BlockStatement();
			foreach (var statement in m.Get<Statement>("statements"))
				body.Statements.Add(statement.Detach());
			var foreachStmt = new ForeachStatement {
				VariableType = context.Settings.AnonymousTypes && itemVariable.Type.ContainsAnonymousType() ? new SimpleType("var") : context.TypeSystemAstBuilder.ConvertType(itemVariable.Type),
				VariableName = itemVariable.Name,
				InExpression = m.Get<IdentifierExpression>("arrayVariable").Single().Detach(),
				EmbeddedStatement = body
			};
			foreachStmt.CopyAnnotationsFrom(forStatement);
			itemVariable.Kind = IL.VariableKind.ForeachLocal;
			// Add the variable annotation for highlighting (TokenTextWriter expects it directly on the ForeachStatement).
			foreachStmt.AddAnnotation(new ILVariableResolveResult(itemVariable, itemVariable.Type));
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
			if (!m.Success) return false;
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
			while (i < upperBounds.Length && MatchLowerBound(i, out IL.ILVariable indexVariable, collection, stmt)) {
				m = forOnArrayMultiDimPattern.Match(stmt.GetNextStatement());
				if (!m.Success) return false;
				var upperBound = m.Get<IdentifierExpression>("upperBoundVariable").Single().GetILVariable();
				if (upperBounds[i] != upperBound)
					return false;
				stmt = m.Get<Statement>("lowerBoundAssign").Single();
				lowerBounds[i] = indexVariable;
				i++;
			}
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
			Match m;
			Statement stmt = expressionStatement;
			IL.ILVariable collection = null;
			IL.ILVariable[] upperBounds = null;
			List<Statement> statementsToDelete = new List<Statement>();
			int i = 0;
			// first we look for all the upper bound initializations
			do {
				m = variableAssignUpperBoundPattern.Match(stmt);
				if (!m.Success) break;
				if (upperBounds == null) {
					collection = m.Get<IdentifierExpression>("collection").Single().GetILVariable();
					if (!(collection.Type is Decompiler.TypeSystem.ArrayType arrayType))
						break;
					upperBounds = new IL.ILVariable[arrayType.Dimensions];
				} else {
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
			if (!itemVariable.IsSingleDefinition
				|| !upperBounds.All(ub => ub.IsSingleDefinition && ub.LoadCount == 1)
				|| !lowerBounds.All(lb => lb.StoreCount == 2 && lb.LoadCount == 3 && lb.AddressCount == 0))
				return null;
			var body = new BlockStatement();
			foreach (var statement in statements)
				body.Statements.Add(statement.Detach());
			var foreachStmt = new ForeachStatement {
				VariableType = context.Settings.AnonymousTypes && itemVariable.Type.ContainsAnonymousType() ? new SimpleType("var") : context.TypeSystemAstBuilder.ConvertType(itemVariable.Type),
				VariableName = itemVariable.Name,
				InExpression = m.Get<IdentifierExpression>("collection").Single().Detach(),
				EmbeddedStatement = body
			};
			foreach (var statement in statementsToDelete)
				statement.Detach();
			//foreachStmt.CopyAnnotationsFrom(forStatement);
			itemVariable.Kind = IL.VariableKind.ForeachLocal;
			// Add the variable annotation for highlighting (TokenTextWriter expects it directly on the ForeachStatement).
			foreachStmt.AddAnnotation(new ILVariableResolveResult(itemVariable, itemVariable.Type));
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
				}}
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

		PropertyDeclaration TransformAutomaticProperties(PropertyDeclaration property)
		{
			PropertyDefinition cecilProperty = context.TypeSystem.GetCecil(property.GetSymbol() as IProperty) as PropertyDefinition;
			if (cecilProperty == null || cecilProperty.GetMethod == null)
				return null;
			if (!cecilProperty.GetMethod.IsCompilerGenerated() && (cecilProperty.SetMethod?.IsCompilerGenerated() == false))
				return null;
			IField fieldInfo = null;
			Match m = automaticPropertyPattern.Match(property);
			if (m.Success) {
				fieldInfo = m.Get<AstNode>("fieldReference").Single().GetSymbol() as IField;
			} else {
				Match m2 = automaticReadonlyPropertyPattern.Match(property);
				if (m2.Success) {
					fieldInfo = m2.Get<AstNode>("fieldReference").Single().GetSymbol() as IField;
				}
			}
			if (fieldInfo == null)
				return null;
			FieldDefinition field = context.TypeSystem.GetCecil(fieldInfo) as FieldDefinition;
			if (field.IsCompilerGenerated() && field.DeclaringType == cecilProperty.DeclaringType) {
				RemoveCompilerGeneratedAttribute(property.Getter.Attributes);
				RemoveCompilerGeneratedAttribute(property.Setter.Attributes);
				property.Getter.Body = null;
				property.Setter.Body = null;
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
			foreach (AttributeSection section in attributeSections) {
				foreach (var attr in section.Attributes) {
					var tr = attr.Type.GetSymbol() as IType;
					if (tr != null && attributesToRemove.Contains(tr.FullName)) {
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
			if (context.Settings.AutomaticProperties) {
				var newIdentifier = ReplaceBackingFieldUsage(identifier);
				if (newIdentifier != null) {
					identifier.ReplaceWith(newIdentifier);
					return newIdentifier;
				}
			}
			if (context.Settings.AutomaticEvents) {
				var newIdentifier = ReplaceEventFieldAnnotation(identifier);
				if (newIdentifier != null)
					return newIdentifier;
			}
			return base.VisitIdentifier(identifier);
		}

		Identifier ReplaceBackingFieldUsage(Identifier identifier)
		{
			if (identifier.Name.StartsWith("<") && identifier.Name.EndsWith(">k__BackingField")) {
				var parent = identifier.Parent;
				var mrr = parent.Annotation<MemberResolveResult>();
				var field = mrr?.Member as IField;
				if (field != null && field.IsCompilerGenerated()) {
					var propertyName = identifier.Name.Substring(1, identifier.Name.Length - 1 - ">k__BackingField".Length);
					var property = field.DeclaringTypeDefinition.GetProperties(p => p.Name == propertyName, GetMemberOptions.IgnoreInheritedMembers).FirstOrDefault();
					if (property != null) {
						parent.RemoveAnnotations<MemberResolveResult>();
						parent.AddAnnotation(new MemberResolveResult(mrr.TargetResult, property));
						return Identifier.Create(propertyName);
					}
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
			if (@event != null) {
				parent.RemoveAnnotations<MemberResolveResult>();
				parent.AddAnnotation(new MemberResolveResult(mrr.TargetResult, @event));
				return identifier;
			}
			return null;
		}

		#region Automatic Events
		static readonly Accessor automaticEventPatternV2 = new Accessor {
			Attributes = { new Repeat(new AnyNode()) },
			Body = new BlockStatement {
				new AssignmentExpression {
					Left = new NamedNode(
						"field",
						new MemberReferenceExpression {
							Target = new Choice { new ThisReferenceExpression(), new TypeReferenceExpression { Type = new AnyNode() } },
							MemberName = Pattern.AnyString
						}),
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
							Right = new CastExpression(new AnyNode("type"), new InvocationExpression(new AnyNode("delegateCombine").ToExpression(), new IdentifierExpressionBackreference("var2"), new IdentifierExpression("value")))
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
			if (m.Get<MemberReferenceExpression>("field").Single().MemberName != ev.Name)
				return false; // field name must match event name
			if (!ev.ReturnType.IsMatch(m.Get("type").Single()))
				return false; // variable types must match event type
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

		bool CheckAutomaticEventV4(CustomEventDeclaration ev, out Match addMatch, out Match removeMatch)
		{
			addMatch = removeMatch = default(Match);
			addMatch = automaticEventPatternV4.Match(ev.AddAccessor);
			if (!CheckAutomaticEventMatch(addMatch, ev, true))
				return false;
			removeMatch = automaticEventPatternV4.Match(ev.RemoveAccessor);
			if (!CheckAutomaticEventMatch(removeMatch, ev, false))
				return false;
			return true;
		}

		bool CheckAutomaticEventV2(CustomEventDeclaration ev, out Match addMatch, out Match removeMatch)
		{
			addMatch = removeMatch = default(Match);
			addMatch = automaticEventPatternV2.Match(ev.AddAccessor);
			if (!CheckAutomaticEventMatch(addMatch, ev, true))
				return false;
			removeMatch = automaticEventPatternV2.Match(ev.RemoveAccessor);
			if (!CheckAutomaticEventMatch(removeMatch, ev, false))
				return false;
			return true;
		}

		EventDeclaration TransformAutomaticEvents(CustomEventDeclaration ev)
		{
			Match m1, m2;
			if (!CheckAutomaticEventV4(ev, out m1, out m2) && !CheckAutomaticEventV2(ev, out m1, out m2))
				return null;
			RemoveCompilerGeneratedAttribute(ev.AddAccessor.Attributes, attributeTypesToRemoveFromAutoEvents);
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
			
			IEvent eventDef = ev.GetSymbol() as IEvent;
			if (eventDef != null) {
				IField field = eventDef.DeclaringType.GetFields(f => f.Name == ev.Name, GetMemberOptions.IgnoreInheritedMembers).SingleOrDefault();
				if (field != null) {
					ed.AddAnnotation(field);
					var attributes = field.Attributes
							.Where(a => !attributeTypesToRemoveFromAutoEvents.Any(t => t == a.AttributeType.FullName))
							.Select(context.TypeSystemAstBuilder.ConvertAttribute).ToArray();
					if (attributes.Length > 0) {
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
				dd.CopyAnnotationsFrom(methodDef);
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
