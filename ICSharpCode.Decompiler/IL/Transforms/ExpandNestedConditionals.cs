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


using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Expands a statement-level assignment of a nested conditional operator back into if-else.
	///
	/// `ExpressionTransforms.HandleConditionalOperator` collapses `if (c) a = x; else a = y;` into a
	/// conditional operator, innermost first, and keeps going for as long as the chain does. A source
	/// else-if ladder therefore comes back as one expression, however long it was: the sample in
	/// issue #2027 decompiles to a single 2095-character statement.
	///
	/// This undoes the collapse past <see cref="MaxNesting"/> levels, so a statement keeps at most
	/// that many conditional operators.
	/// </summary>
	/// <remarks>
	/// This runs at the end of the pipeline, not inside ExpressionTransforms, because every transform
	/// that needs its input to be a single expression has to see the collapsed form first: object and
	/// collection initializers, `with`, switch expressions, interpolated string handlers, and the
	/// query lambdas the C# stage later rewrites into clauses. Cutting the chain earlier leaves an
	/// if-else between the statements they pattern-match on, and they silently stop matching.
	///
	/// Only a store to a declared local is expanded. Such a local carries its own type from metadata,
	/// so the branches can be split without the result widening to the stack type - which is what
	/// makes a separate type check unnecessary here.
	/// </remarks>
	public class ExpandNestedConditionals : IILTransform
	{
		/// <summary>
		/// How many conditional operators a single statement may keep.
		/// </summary>
		const int MaxNesting = 1;

		ILTransformContext context;

		public void Run(ILFunction function, ILTransformContext context)
		{
			this.context = context;
			if (context.Settings.AggressiveInlining)
				return;
			foreach (var f in function.Descendants.OfType<ILFunction>())
			{
				if (!IsExpandableFunction(f))
					continue;
				foreach (var block in f.Descendants.OfType<Block>().ToArray())
				{
					if (block.Ancestors.OfType<ILFunction>().FirstOrDefault() != f)
						continue;
					if (!IsExpandableBlock(block))
						continue;
					for (int i = 0; i < block.Instructions.Count; i++)
					{
						context.CancellationToken.ThrowIfCancellationRequested();
						ExtractInto(block, i);
						Expand(f, block, i);
					}
				}
			}
		}

		/// <summary>
		/// A lambda body may have to stay a single expression: the query-expression stage rewrites
		/// one into a clause, and an expression tree is built from the expression itself. A
		/// constructor is matched as a whole, so that its leading stores become field initializers
		/// and a record's primary constructor is recognized.
		/// </summary>
		static bool IsExpandableFunction(ILFunction function)
		{
			return function.Kind is ILFunctionKind.TopLevelFunction or ILFunctionKind.LocalFunction
				&& function.Method?.IsConstructor != true;
		}

		/// <summary>
		/// Only plain control-flow blocks hold statements. The other block kinds are expressions
		/// spelled as blocks - an initializer, a named-argument call - and an if-else inside one of
		/// them is not a statement the C# stage can print.
		/// </summary>
		static bool IsExpandableBlock(Block block)
		{
			if (block.Kind != BlockKind.ControlFlow)
				return false;
			if (ILInlining.IsCatchWhenBlock(block))
				return false;
			// A branch body is a block hanging off the if, not off a container; it holds statements
			// either way. Only a container's own layout reserves blocks for a loop or switch header.
			if (block.Parent is not BlockContainer container)
				return true;
			return container.Kind switch {
				// the entry point carries the loop condition or the switch value
				ContainerKind.While or ContainerKind.Switch => block != container.EntryPoint,
				// and for a for-loop the last block carries the increment
				ContainerKind.For => block != container.EntryPoint
					&& block != container.Blocks[container.Blocks.Count - 1],
				ContainerKind.DoWhile => block != container.Blocks[container.Blocks.Count - 1],
				_ => true,
			};
		}

		/// <summary>
		/// A conditional that is not stored to a variable - an argument, a return value, a
		/// condition - has nothing to expand into. ILExtraction gives it one, vetting the move
		/// through PrepareExtract so the order of evaluation is preserved.
		/// </summary>
		/// <remarks>
		/// Extraction is only done where the position the value flows into names a type. The
		/// temporary ILExtraction creates is typed from the stack type, and `I4` is `int`, `bool`,
		/// `char` and every enum at once; the consumer knows better, because a parameter, a return
		/// type or a field carries its type in metadata.
		/// </remarks>
		void ExtractInto(Block block, int pos)
		{
			if (block.Instructions[pos] is StLoc { Value: IfInstruction })
				return;
			foreach (var inst in block.Instructions[pos].Descendants.OfType<IfInstruction>().ToArray())
			{
				if (inst.Parent is StLoc || !ExceedsNesting(inst, MaxNesting))
					continue;
				var expected = inst.Parent?.InferExpectedType(inst.ChildIndex, context.TypeSystem);
				if (expected == null || expected.Kind == TypeKind.Unknown
					|| expected.GetStackType() != inst.ResultType)
				{
					continue;
				}
				context.Step("Extract nested conditional operator", inst);
				var v = inst.Extract(context);
				if (v != null)
				{
					v.Type = expected;
					context.EndStep(block.Instructions[pos]);
					return;
				}
				context.EndStep(inst);
			}
		}

		void Expand(ILFunction function, Block block, int pos)
		{
			// Only one branch of a chain nests further, so the expansion walks down it. A chain is
			// bounded only by the size of the method it came from, hence the loop over recursion.
			var worklist = new Stack<(Block Block, int Pos)>();
			worklist.Push((block, pos));
			while (worklist.Count > 0)
			{
				var (current, index) = worklist.Pop();
				if (current.Instructions[index] is not StLoc { Variable: var v, Value: IfInstruction ifInst } stloc)
					continue;
				if (v.Kind is not (VariableKind.Local or VariableKind.StackSlot) || v.Type is ByReferenceType)
					continue;
				if (ILInlining.IsInConstructorInitializer(function, stloc))
					continue;
				if (!ExceedsNesting(ifInst, MaxNesting))
					continue;

				context.Step("Expand nested conditional operator", stloc);
				// HandleConditionalOperator built the conditional by negating the condition and
				// swapping the branches; undoing that here keeps the source's own polarity, and
				// makes the expansion the exact inverse of the collapse rather than an equivalent
				// of it - without which a pretty-test fixture would never reach a fixed point.
				var condition = ifInst.Condition;
				var (trueValue, falseValue) = (ifInst.TrueInst, ifInst.FalseInst);
				while (condition.MatchLogicNot(out var withoutNot))
				{
					condition = withoutNot;
					(trueValue, falseValue) = (falseValue, trueValue);
				}
				// Each new store stands where its value did, so the statement keeps a sequence point
				// of its own instead of inheriting the one the value already carries.
				var trueStore = new StLoc(v, trueValue).WithILRange(trueValue);
				var falseStore = new StLoc(v, falseValue).WithILRange(falseValue);
				var trueBlock = new Block { Instructions = { trueStore } }.WithILRange(trueStore);
				var falseBlock = new Block { Instructions = { falseStore } }.WithILRange(falseStore);
				var expanded = new IfInstruction(condition, trueBlock, falseBlock);
				expanded.AddILRange(ifInst);
				expanded.AddILRange(stloc);
				stloc.ReplaceWith(expanded);
				context.EndStep(expanded);

				// the branches are statements of their own now, so each may need expanding in turn
				worklist.Push((trueBlock, 0));
				worklist.Push((falseBlock, 0));
			}
		}

		/// <summary>
		/// Gets whether the branches of <paramref name="inst"/> nest conditional operators
		/// <paramref name="depth"/> levels deep. The short-circuit logic operators share the
		/// IfInstruction representation, but render as operator chains without visible nesting.
		/// </summary>
		static bool ExceedsNesting(ILInstruction inst, int depth)
		{
			if (inst is not IfInstruction ifInst)
				return false;
			if (ifInst.MatchLogicAnd(out _, out _) || ifInst.MatchLogicOr(out _, out _))
				return false;
			return depth <= 0
				|| ExceedsNesting(ifInst.TrueInst, depth - 1)
				|| ExceedsNesting(ifInst.FalseInst, depth - 1);
		}
	}
}
