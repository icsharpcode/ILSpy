// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ICSharpCode.NRefactory.CSharp;

namespace Decompiler.Transforms
{
	public interface IAstTransform
	{
		void Run(AstNode node);
	}

	public class VisitorTransform<T, S> : IAstTransform
	{
		private IAstVisitor<T, S> visitor;
		private T data;

		public VisitorTransform(IAstVisitor<T, S> visitor, T data) {
			if (visitor == null)
				throw new ArgumentException("visitor");
			this.visitor = visitor;
			this.data = data;
		}

		public void Run(AstNode node)
		{
			if (node == null)
				throw new ArgumentNullException("node");
			node.AcceptVisitor(visitor, data);
		}
	}

	public static class VisitorTransform
	{
		public static VisitorTransform<T, S> Create<T, S>(IAstVisitor<T, S> visitor, T data)
		{
			return new VisitorTransform<T, S>(visitor, data);
		}
	}
	
	public static class TransformationPipeline
	{
		public static IAstTransform[] CreatePipeline(DecompilerContext context)
		{
			return new IAstTransform[] {
				new PushNegation(),
				new DelegateConstruction(context),
				new PatternStatementTransform(),
				new ConvertConstructorCallIntoInitializer(),
				new ReplaceMethodCallsWithOperators(),
			};
		}

		public static void RunTransformationsUntil(AstNode node, Predicate<IAstTransform> abortCondition, DecompilerContext context) {
			if (node == null)
				return;

			RunTransformations(node, CreateTransformationPipeline(context).TakeWhile(tr => abortCondition == null || !abortCondition(tr)), context);
		}

		public static void RunTransformations(AstNode node, IEnumerable<IAstTransform> transformations, DecompilerContext context)
		{
			if (node == null)
				return;

			foreach (var transform in transformations) {
				context.CancellationToken.ThrowIfCancellationRequested();
				transform.Run(node);
			}
		}

		public static IEnumerable<IAstTransform> CreateTransformationPipeline(DecompilerContext context) {
			for (int i = 0; i < 4; i++) {
				if (Options.ReduceAstJumps) {
					yield return VisitorTransform.Create(new Transforms.Ast.RemoveGotos(), null);
					yield return VisitorTransform.Create(new Transforms.Ast.RemoveDeadLabels(), null);
				}
				if (Options.ReduceAstLoops) {
					yield return VisitorTransform.Create(new Transforms.Ast.RestoreLoop(), null);
				}
				if (Options.ReduceAstOther) {
					yield return VisitorTransform.Create(new Transforms.Ast.RemoveEmptyElseBody(), null);
				}
			}
			
			foreach (var transform in CreatePipeline(context)) {
				yield return transform;
			}
		}
	}
}
