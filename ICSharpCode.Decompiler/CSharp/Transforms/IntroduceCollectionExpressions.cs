// Copyright (c) 2026 sonyps5201314
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
using System.Linq;

using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.CSharp.Transforms
{
	public sealed class IntroduceCollectionExpressions : DepthFirstAstVisitor, IAstTransform
	{
		TransformContext context = null!;

		public void Run(AstNode node, TransformContext context)
		{
			this.context = context;
			if (context.Settings.CollectionExpressions)
				node.AcceptVisitor(this);
		}

		public override void VisitBlockStatement(BlockStatement blockStatement)
		{
			bool changed;
			do
			{
				changed = false;
				foreach (var statement in blockStatement.Statements.ToArray())
				{
					if (TryTransformListConstruction(blockStatement, statement)
						|| TryTransformInlineArrayConstruction(blockStatement, statement))
					{
						changed = true;
						break;
					}
				}
			}
			while (changed);
			base.VisitBlockStatement(blockStatement);
		}

		public override void VisitCastExpression(CastExpression castExpression)
		{
			base.VisitCastExpression(castExpression);
			var targetType = castExpression.Type.GetResolveResult().Type;
			if (!TryConvertExpression(castExpression.Expression, targetType, out var collection))
				return;
			context.Step("Use collection expression", castExpression.Expression);
			castExpression.Expression.ReplaceWith(collection);
			context.EndStep(collection);
		}

		public override void VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression)
		{
			base.VisitObjectCreateExpression(objectCreateExpression);
			var objectType = objectCreateExpression.GetResolveResult().Type;
			if (!IsReadOnlyCollectionHelper(objectType)
				|| objectType.DirectBaseTypes.FirstOrDefault(type =>
					type.GetDefinition()?.KnownTypeCode == KnownTypeCode.IReadOnlyListOfT) is not { } targetType)
			{
				return;
			}
			CollectionExpression collection;
			if (!TryConvertExpression(objectCreateExpression, targetType, out collection))
			{
				if (objectCreateExpression.Arguments.Count != 1)
					return;
				collection = CreateCollectionExpression(
					new[] { (objectCreateExpression.Arguments.Single(), true) }, targetType);
			}
			context.Step("Use collection expression", objectCreateExpression);
			objectCreateExpression.ReplaceWith(collection);
			context.EndStep(collection);
		}

		public override void VisitReturnStatement(ReturnStatement returnStatement)
		{
			base.VisitReturnStatement(returnStatement);
			if (returnStatement.Expression == null)
				return;
			var method = returnStatement.Ancestors.OfType<EntityDeclaration>()
				.Select(declaration => declaration.GetSymbol())
				.OfType<IMethod>()
				.FirstOrDefault();
			if (method == null || !TryConvertExpression(returnStatement.Expression, method.ReturnType, out var collection))
				return;
			context.Step("Use collection expression", returnStatement.Expression);
			returnStatement.Expression.ReplaceWith(collection);
			context.EndStep(collection);
		}

		public override void VisitVariableDeclarationStatement(VariableDeclarationStatement variableDeclarationStatement)
		{
			base.VisitVariableDeclarationStatement(variableDeclarationStatement);
			if (variableDeclarationStatement.Variables.Count != 1)
				return;
			var variable = variableDeclarationStatement.Variables.Single();
			if (variable.Initializer == null)
				return;
			var targetType = variableDeclarationStatement.Type.GetResolveResult().Type;
			if (targetType.Kind == TypeKind.Unknown
				|| !TryConvertExpression(variable.Initializer, targetType, out var collection))
			{
				return;
			}
			context.Step("Use collection expression", variable.Initializer);
			variable.Initializer.ReplaceWith(collection);
			context.EndStep(collection);
		}

		bool TryTransformListConstruction(BlockStatement block, Statement statement)
		{
			if (statement is not VariableDeclarationStatement { Variables.Count: 1 } listDeclaration)
				return false;
			var listVariable = listDeclaration.Variables.Single();
			if (listVariable.Initializer is not ObjectCreateExpression listCreation
				|| listCreation.GetResolveResult().Type.GetDefinition()?.FullName != "System.Collections.Generic.List")
			{
				return false;
			}

			var elements = new List<(Expression Expression, bool IsSpread)>();
			var setupStatements = new List<Statement> { statement };
			if (statement.PrevSibling is VariableDeclarationStatement { Variables.Count: 1 } countDeclaration
				&& listCreation.Arguments.Count == 1
				&& listCreation.Arguments.Single() is IdentifierExpression countReference
				&& countReference.Identifier == countDeclaration.Variables.Single().Name)
			{
				setupStatements.Insert(0, countDeclaration);
			}
			Statement? sink = statement.GetNextStatement();
			while (sink != null && TryMatchAdd(sink, listVariable.Name, out var element, out bool isSpread))
			{
				elements.Add((element, isSpread));
				setupStatements.Add(sink);
				sink = sink.GetNextStatement();
			}

			if (elements.Count == 0)
			{
				if (!TryParseSpanInitialization(listDeclaration, setupStatements, elements, ref sink))
					return false;
			}
			if (elements.Any(element => element.Expression.DescendantsAndSelf.OfType<IdentifierExpression>()
				.Any(identifier => identifier.Identifier == listVariable.Name)))
			{
				return false;
			}
			if (sink is not ReturnStatement { Expression: { } returnExpression }
				|| !TryMatchCollectionSink(returnExpression, listVariable.Name, out var replaceTarget))
			{
				return false;
			}
			var method = sink.Ancestors.OfType<EntityDeclaration>()
				.Select(declaration => declaration.GetSymbol())
				.OfType<IMethod>()
				.FirstOrDefault();
			if (method == null)
				return false;
			var collection = CreateCollectionExpression(elements, method.ReturnType);
			context.Step("Reconstruct collection expression", statement);
			replaceTarget.ReplaceWith(collection);
			foreach (var setupStatement in setupStatements)
				setupStatement.Remove();
			context.EndStep(collection);
			return true;
		}

		bool TryParseSpanInitialization(VariableDeclarationStatement listDeclaration,
			List<Statement> setupStatements, List<(Expression Expression, bool IsSpread)> elements,
			ref Statement? sink)
		{
			var listName = listDeclaration.Variables.Single().Name;
			var current = listDeclaration.GetNextStatement();
			bool hasSetCount = false;
			string? spanName = null;
			while (current != null)
			{
				if (current is ReturnStatement)
				{
					sink = current;
					return hasSetCount && spanName != null && elements.Count > 0;
				}
				if (current.Descendants.OfType<InvocationExpression>().Any(call =>
					call.Target is MemberReferenceExpression { MemberName: "SetCount" }))
				{
					hasSetCount = true;
					setupStatements.Add(current);
					current = current.GetNextStatement();
					continue;
				}
				if (current is VariableDeclarationStatement { Variables.Count: 1 } declaration)
				{
					var variable = declaration.Variables.Single();
					if (variable.Initializer is InvocationExpression {
						Target: MemberReferenceExpression { MemberName: "AsSpan" }
					})
					{
						spanName = variable.Name;
					}
					else if (variable.Initializer is ObjectCreateExpression { Arguments.Count: 1 } spanCreation
						&& spanCreation.GetResolveResult().Type.GetDefinition()?.FullName == "System.ReadOnlySpan")
					{
						elements.Add((spanCreation.Arguments.Single(), true));
					}
					else if (variable.Initializer?.GetResolveResult().IsCompileTimeConstant != true)
					{
						return false;
					}
					setupStatements.Add(current);
					current = current.GetNextStatement();
					continue;
				}
				if (current is ForeachStatement foreachStatement)
				{
					if (spanName == null || !IsSpanCopyLoop(foreachStatement, spanName))
						return false;
					elements.Add((foreachStatement.InExpression, true));
					setupStatements.Add(current);
					current = current.GetNextStatement();
					continue;
				}
				if (spanName != null && current is ExpressionStatement {
					Expression: AssignmentExpression {
						Operator: AssignmentOperatorType.Assign,
						Left: IndexerExpression { Target: IdentifierExpression spanTarget },
						Right: var value
					}
				} && spanTarget.Identifier == spanName)
				{
					elements.Add((value, false));
					setupStatements.Add(current);
					current = current.GetNextStatement();
					continue;
				}
				if (current is ExpressionStatement expressionStatement
					&& (expressionStatement.Descendants.OfType<InvocationExpression>().Any(call =>
						call.Target is MemberReferenceExpression { MemberName: "CopyTo" or "Slice" })
						|| expressionStatement.Expression is AssignmentExpression {
							Left: IdentifierExpression,
						}))
				{
					setupStatements.Add(current);
					current = current.GetNextStatement();
					continue;
				}
				return false;
			}
			return false;

			static bool IsSpanCopyLoop(ForeachStatement foreachStatement, string spanName)
			{
				var statements = foreachStatement.EmbeddedStatement is BlockStatement block
					? block.Statements.ToArray()
					: new[] { foreachStatement.EmbeddedStatement };
				if (statements.Any(statement => statement is not ExpressionStatement))
					return false;
				return statements.OfType<ExpressionStatement>().Any(statement =>
					statement.Expression is AssignmentExpression {
						Operator: AssignmentOperatorType.Assign,
						Left: IndexerExpression { Target: IdentifierExpression target }
					} && target.Identifier == spanName);
			}
		}

		bool TryTransformInlineArrayConstruction(BlockStatement block, Statement statement)
		{
			if (statement is not VariableDeclarationStatement { Variables.Count: 1 } declaration)
				return false;
			var variable = declaration.Variables.Single();
			var inlineArrayType = declaration.Type.GetResolveResult().Type;
			var definition = inlineArrayType.GetDefinition();
			if (definition == null || definition.GetInlineArrayLength() is not int length || length <= 0)
				return false;
			var values = new List<Expression>();
			var assignments = new List<Statement>();
			var current = statement.GetNextStatement();
			while (current is ExpressionStatement {
				Expression: AssignmentExpression {
					Operator: AssignmentOperatorType.Assign,
					Left: IndexerExpression { Target: IdentifierExpression target },
					Right: var value
				}
			} && target.Identifier == variable.Name)
			{
				values.Add(value);
				assignments.Add(current);
				current = current.GetNextStatement();
			}
			if (values.Count != length)
				return false;
			var spanDefinition = context.TypeSystem.FindType(KnownTypeCode.SpanOfT).GetDefinition();
			if (spanDefinition == null)
				return false;
			var elementType = inlineArrayType.GetInlineArrayElementType();
			var spanType = new ParameterizedType(spanDefinition, elementType);
			var collection = CreateCollectionExpression(values.Select(value => (value, false)), spanType);
			context.Step("Reconstruct span collection expression", statement);
			declaration.Type.ReplaceWith(context.TypeSystemAstBuilder.ConvertType(spanType));
			variable.Initializer?.ReplaceWith(collection);
			foreach (var assignment in assignments)
				assignment.Remove();
			context.EndStep(collection);
			return true;
		}

		bool TryConvertExpression(Expression expression, IType targetType, out CollectionExpression collection)
		{
			collection = null!;
			if (!TryExtractElements(expression, targetType, out var elements))
				return false;
			collection = CreateCollectionExpression(elements.Select(element => (element, false)), targetType);
			return true;
		}

		bool TryExtractElements(Expression expression, IType targetType, out List<Expression> elements)
		{
			elements = new List<Expression>();
			if (expression is InvocationExpression { Arguments.Count: 0 } invocation
				&& invocation.GetSymbol() is IMethod { Name: "Empty", IsStatic: true, DeclaringType.FullName: "System.Array" })
			{
				return true;
			}
			if (expression is CastExpression cast)
				return TryExtractElements(cast.Expression, targetType, out elements);
			if (expression is ArrayCreateExpression arrayCreation)
			{
				if (targetType is ArrayType { Dimensions: not 1 })
					return false;
				if (arrayCreation.Initializer == null)
				{
					return arrayCreation.Arguments.Count == 0
						|| (arrayCreation.Arguments.Count == 1
							&& arrayCreation.Arguments.Single().GetResolveResult().ConstantValue is int length
							&& length == 0);
				}
				return ExtractInitializer(arrayCreation.Initializer, elements);
			}
			if (expression is ObjectCreateExpression objectCreation)
			{
				var fullName = objectCreation.GetResolveResult().Type.GetDefinition()?.FullName;
				if (fullName == "System.Collections.Generic.List")
				{
					if (!ExtractInitializer(objectCreation.Initializer, elements))
						return false;
					return objectCreation.Arguments.Count == 0
						|| (objectCreation.Arguments.Count == 1
							&& objectCreation.Arguments.Single().GetResolveResult().ConstantValue is int capacity
							&& capacity == elements.Count);
				}
				if (fullName?.StartsWith("<>z__ReadOnlySingleElementList", StringComparison.Ordinal) == true
					&& objectCreation.Arguments.Count == 1)
				{
					elements.Add(objectCreation.Arguments.Single());
					return true;
				}
				if ((fullName?.StartsWith("<>z__ReadOnlyArray", StringComparison.Ordinal) == true
						|| fullName?.StartsWith("<>z__ReadOnlyList", StringComparison.Ordinal) == true)
					&& objectCreation.Arguments.Count == 1)
				{
					return TryExtractElements(objectCreation.Arguments.Single(), targetType, out elements);
				}
				if (fullName == "System.ReadOnlySpan" && objectCreation.Arguments.Count == 1)
					return TryExtractElements(objectCreation.Arguments.Single(), targetType, out elements);
			}
			if (expression is InvocationExpression builderCall
				&& targetType.GetDefinition()?.GetAttributes().Any(attribute =>
					attribute.AttributeType.ReflectionName == "System.Runtime.CompilerServices.CollectionBuilderAttribute")
					== true
				&& builderCall.Arguments.Count == 1)
			{
				return TryExtractElements(builderCall.Arguments.Single(), targetType, out elements);
			}
			return false;
		}

		static bool IsReadOnlyCollectionHelper(IType type)
		{
			return type.GetDefinition()?.FullName?.StartsWith("<>z__ReadOnly", StringComparison.Ordinal) == true;
		}

		static bool ExtractInitializer(ArrayInitializerExpression? initializer, List<Expression> elements)
		{
			if (initializer == null)
				return true;
			foreach (var element in initializer.Elements)
			{
				elements.Add(element is ArrayInitializerExpression { Elements.Count: 1 } nested
					? nested.Elements.Single()
					: element);
			}
			return true;
		}

		static bool TryMatchAdd(Statement statement, string variableName,
			out Expression element, out bool isSpread)
		{
			element = null!;
			isSpread = false;
			if (statement is not ExpressionStatement {
				Expression: InvocationExpression {
					Target: MemberReferenceExpression {
						Target: IdentifierExpression target,
						MemberName: var methodName
					},
					Arguments.Count: 1
				} invocation
			} || target.Identifier != variableName || methodName is not ("Add" or "AddRange"))
			{
				return false;
			}
			element = invocation.Arguments.Single();
			isSpread = methodName == "AddRange";
			return true;
		}

		static bool TryMatchCollectionSink(Expression expression, string variableName, out Expression replaceTarget)
		{
			replaceTarget = null!;
			if (expression is IdentifierExpression identifier && identifier.Identifier == variableName)
			{
				replaceTarget = expression;
				return true;
			}
			var references = expression.DescendantsAndSelf.OfType<IdentifierExpression>()
				.Where(identifierExpression => identifierExpression.Identifier == variableName)
				.ToArray();
			if (references.Length != 1)
				return false;
			if (expression is InvocationExpression {
				Target: MemberReferenceExpression { Target: IdentifierExpression target, MemberName: "ToArray" },
				Arguments.Count: 0
			} && target.Identifier == variableName)
			{
				replaceTarget = expression;
				return true;
			}
			if (expression is InvocationExpression builderCall
				&& builderCall.Descendants.OfType<InvocationExpression>().Any(call =>
					call.Target is MemberReferenceExpression { Target: IdentifierExpression list, MemberName: "ToArray" }
					&& list.Identifier == variableName))
			{
				replaceTarget = expression;
				return true;
			}
			return false;
		}

		static CollectionExpression CreateCollectionExpression(
			IEnumerable<(Expression Expression, bool IsSpread)> elements, IType targetType)
		{
			var collection = new CollectionExpression();
			foreach (var element in elements)
			{
				collection.Elements.Add(element.IsSpread
					? new SpreadElement { Expression = element.Expression.Detach() }
					: element.Expression.Detach());
			}
			collection.AddAnnotation(new ResolveResult(targetType));
			return collection;
		}
	}
}
