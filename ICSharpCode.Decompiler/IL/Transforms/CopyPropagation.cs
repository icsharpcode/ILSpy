// Copyright (c) 2011-2015 Daniel Grunwald
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

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;
using System.Diagnostics;
using System.Linq;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Runs a very simple form of copy propagation.
	/// Copy propagation is used in two cases:
	/// 1) assignments from arguments to local variables
	///    If the target variable is assigned to only once (so always is that argument) and the argument is never changed (no ldarga/starg),
	///    then we can replace the variable with the argument.
	/// 2) assignments of address-loading instructions to local variables
	/// </summary>
	public class CopyPropagation : IBlockTransform, IILTransform
	{
		public static void Propagate(StLoc store, ILTransformContext context)
		{
			Debug.Assert(store.Variable.IsSingleDefinition);
			Block block = (Block)store.Parent;
			int i = store.ChildIndex;
			DoPropagate(store.Variable, store.Value, block, ref i, context);
		}

		public void Run(Block block, BlockTransformContext context)
		{
			RunOnBlock(block, context);
		}

		public void Run(ILFunction function, ILTransformContext context)
		{
			foreach (var block in function.Descendants.OfType<Block>()) {
				if (block.Kind != BlockKind.ControlFlow)
					continue;
				RunOnBlock(block, context);
			}
		}

		static void RunOnBlock(Block block, ILTransformContext context)
		{
			for (int i = 0; i < block.Instructions.Count; i++) {
				ILVariable v;
				ILInstruction copiedExpr;
				if (block.Instructions[i].MatchStLoc(out v, out copiedExpr)) {
					if (v.IsSingleDefinition && v.LoadCount == 0 && v.Kind == VariableKind.StackSlot) {
						// dead store to stack
						if (SemanticHelper.IsPure(copiedExpr.Flags)) {
							// no-op -> delete
							context.Step("remove dead store to stack: no-op -> delete", block.Instructions[i]);
							block.Instructions.RemoveAt(i);
							// This can open up new inlining opportunities:
							int c = ILInlining.InlineInto(block, i, InliningOptions.None, context: context);
							i -= c + 1;
						} else {
							// evaluate the value for its side-effects
							context.Step("remove dead store to stack: evaluate the value for its side-effects", block.Instructions[i]);
							copiedExpr.AddILRange(block.Instructions[i]);
							block.Instructions[i] = copiedExpr;
						}
					} else if (v.IsSingleDefinition && CanPerformCopyPropagation(v, copiedExpr)) {
						DoPropagate(v, copiedExpr, block, ref i, context);
					}
				}
			}
		}

		static bool CanPerformCopyPropagation(ILVariable target, ILInstruction value)
		{
			Debug.Assert(target.StackType == value.ResultType);
			if (target.Type.IsSmallIntegerType())
				return false;
			switch (value.OpCode) {
				case OpCode.LdLoca:
//				case OpCode.LdElema:
//				case OpCode.LdFlda:
				case OpCode.LdsFlda:
					// All address-loading instructions always return the same value for a given operand/argument combination,
					// so they can be safely copied.
					// ... except for LdElema and LdFlda, because those might throw an exception, and we don't want to
					// change the place where the exception is thrown.
					return true;
				case OpCode.LdLoc:
					var v = ((LdLoc)value).Variable;
					switch (v.Kind) {
						case VariableKind.Parameter:
							// Parameters can be copied only if they aren't assigned to (directly or indirectly via ldarga)
							// note: the initialization by the caller is the first store -> StoreCount must be 1
							return v.IsSingleDefinition;
						default:
							// Variables can be copied if both are single-definition.
							// To avoid removing too many variables, we do this only if the target
							// is either a stackslot or a ref local.
							Debug.Assert(target.IsSingleDefinition);
							return v.IsSingleDefinition && (target.Kind == VariableKind.StackSlot || target.StackType == StackType.Ref);
					}
				default:
					// All instructions without special behavior that target a stack-variable can be copied.
					return value.Flags == InstructionFlags.None && value.Children.Count == 0 && target.Kind == VariableKind.StackSlot;
			}
		}

		static void DoPropagate(ILVariable v, ILInstruction copiedExpr, Block block, ref int i, ILTransformContext context)
		{
			context.Step($"Copy propagate {v.Name}", copiedExpr);
			// un-inline the arguments of the ldArg instruction
			ILVariable[] uninlinedArgs = new ILVariable[copiedExpr.Children.Count];
			for (int j = 0; j < uninlinedArgs.Length; j++) {
				var arg = copiedExpr.Children[j];
				var type = context.TypeSystem.FindType(arg.ResultType.ToKnownTypeCode());
				uninlinedArgs[j] = new ILVariable(VariableKind.StackSlot, type, arg.ResultType) {
					Name = "C_" + arg.StartILOffset,
					HasGeneratedName = true,
				};
				block.Instructions.Insert(i++, new StLoc(uninlinedArgs[j], arg));
			}
			CollectionExtensions.AddRange(v.Function.Variables, uninlinedArgs);
			// perform copy propagation:
			foreach (var expr in v.LoadInstructions.ToArray()) {
				var clone = copiedExpr.Clone();
				for (int j = 0; j < uninlinedArgs.Length; j++) {
					clone.Children[j].ReplaceWith(new LdLoc(uninlinedArgs[j]));
				}
				expr.ReplaceWith(clone);
			}
			block.Instructions.RemoveAt(i);
			int c = ILInlining.InlineInto(block, i, InliningOptions.None, context: context);
			i -= c + 1;
		}
	}
}


