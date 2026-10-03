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

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.CSharp.Transforms
{
	/// <summary>
	/// Rewrites VB WithEvents control initialization into the field-and-subscribe shape the
	/// Windows Forms designer recognizes, while leaving non-initializer assignments on the
	/// property so reassignment keeps VB's detach/attach semantics.
	/// </summary>
	public sealed class RewriteVisualBasicWithEventsInInitializeComponent : ContextTrackingVisitor<object?>, IAstTransform
	{
		TransformContext? context;
		readonly Dictionary<IProperty, AutoEventDecompiler.VisualBasicWithEventsInfo> withEventsProperties = new();
		bool isInWindowsFormsInitializeComponent;

		public void Run(AstNode rootNode, TransformContext context)
		{
			this.context = context;
			try
			{
				base.Initialize(context);
				CollectWithEventsProperties(rootNode);
				if (withEventsProperties.Count > 0)
				{
					rootNode.AcceptVisitor(this);
				}
			}
			finally
			{
				withEventsProperties.Clear();
				base.Uninitialize();
				this.context = null;
			}
		}

		void CollectWithEventsProperties(AstNode rootNode)
		{
			foreach (var property in rootNode.Descendants.OfType<PropertyDeclaration>())
			{
				if (AutoEventDecompiler.TryAnalyzeVisualBasicWithEventsProperty(property, out var info))
				{
					withEventsProperties[info.Property] = info;
				}
				else if (property.GetSymbol() is IProperty symbol
					&& PatternStatementTransform.TryGetBackingField(symbol, out var backingField)
					&& PatternStatementTransform.HasAccessedThroughPropertyAttribute(backingField))
				{
					withEventsProperties[(IProperty)symbol.MemberDefinition] = new AutoEventDecompiler.VisualBasicWithEventsInfo(
						(IProperty)symbol.MemberDefinition,
						backingField,
						Array.Empty<AutoEventDecompiler.VisualBasicWithEventsSubscription>());
				}
			}
		}

		public override object? VisitMethodDeclaration(MethodDeclaration methodDeclaration)
		{
			if (methodDeclaration.GetSymbol() is not IMethod method || !CSharpDecompiler.IsWindowsFormsInitializeComponentMethod(method))
				return null;
			bool oldIsInWindowsFormsInitializeComponent = isInWindowsFormsInitializeComponent;
			isInWindowsFormsInitializeComponent = true;
			try
			{
				return base.VisitMethodDeclaration(methodDeclaration);
			}
			finally
			{
				isInWindowsFormsInitializeComponent = oldIsInWindowsFormsInitializeComponent;
			}
		}

		public override object? VisitExpressionStatement(ExpressionStatement expressionStatement)
		{
			if (!isInWindowsFormsInitializeComponent)
			{
				base.VisitExpressionStatement(expressionStatement);
				return null;
			}
			if (expressionStatement.Expression is not AssignmentExpression assignment)
			{
				base.VisitExpressionStatement(expressionStatement);
				return null;
			}
			if (assignment.Operator != AssignmentOperatorType.Assign)
			{
				base.VisitExpressionStatement(expressionStatement);
				return null;
			}
			if (assignment.Left.GetSymbol() is not IProperty property)
			{
				base.VisitExpressionStatement(expressionStatement);
				return null;
			}
			property = (IProperty)property.MemberDefinition;
			if (!withEventsProperties.TryGetValue(property, out var info))
			{
				base.VisitExpressionStatement(expressionStatement);
				return null;
			}

			context!.Step("Rewrite VB WithEvents initialization", expressionStatement);
			assignment.Left.ReplaceWith(CreateFieldReference(info.BackingField));
			base.VisitExpressionStatement(expressionStatement);
			if (expressionStatement.Parent is not BlockStatement block)
				return null;
			Statement insertionPoint = expressionStatement;
			foreach (var subscription in info.Subscriptions)
			{
				var statement = CreateSubscriptionStatement(info.BackingField, subscription);
				block.Statements.InsertAfter(insertionPoint, statement);
				insertionPoint = statement;
			}
			return null;
		}

		public override object? VisitIdentifierExpression(IdentifierExpression identifierExpression)
		{
			if (TryRewritePropertyReference(identifierExpression))
				return null;
			return base.VisitIdentifierExpression(identifierExpression);
		}

		public override object? VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression)
		{
			if (TryRewritePropertyReference(memberReferenceExpression))
				return null;
			return base.VisitMemberReferenceExpression(memberReferenceExpression);
		}

		bool TryRewritePropertyReference(Expression expression)
		{
			if (!isInWindowsFormsInitializeComponent)
				return false;
			if (expression.GetSymbol() is not IProperty property)
				return false;
			property = (IProperty)property.MemberDefinition;
			if (!withEventsProperties.TryGetValue(property, out var info))
				return false;
			context!.Step("Rewrite VB WithEvents reference", expression);
			expression.ReplaceWith(CreateFieldReference(info.BackingField).CopyAnnotationsFrom(expression));
			return true;
		}

		static Expression CreateFieldReference(IField field)
		{
			if (field.IsStatic)
			{
				return new IdentifierExpression(field.Name)
					.WithRR(new MemberResolveResult(null, field));
			}
			return new MemberReferenceExpression(new ThisReferenceExpression(), field.Name)
				.WithRR(new MemberResolveResult(null, field));
		}

		ExpressionStatement CreateSubscriptionStatement(IField field, AutoEventDecompiler.VisualBasicWithEventsSubscription subscription)
		{
			Expression eventTarget = CreateFieldReference(field);
			var left = new MemberReferenceExpression(eventTarget, subscription.Event.Name)
				.WithRR(new MemberResolveResult(null, subscription.Event));
			var handler = subscription.Handler.IsStatic
				? (Expression)new IdentifierExpression(subscription.Handler.Name)
				: new MemberReferenceExpression(new ThisReferenceExpression(), subscription.Handler.Name);
			handler = handler.WithRR(new MemberResolveResult(null, subscription.Handler));
			var right = new ObjectCreateExpression {
				Type = context!.TypeSystemAstBuilder.ConvertType(subscription.Event.ReturnType)
			};
			right.Arguments.Add(handler);
			var assignment = new AssignmentExpression(left, right) {
				Operator = AssignmentOperatorType.Add
			}.WithRR(new TypeResolveResult(subscription.Event.ReturnType));
			return new ExpressionStatement { Expression = assignment };
		}
	}
}
