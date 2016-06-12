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

using System;
using System.Linq;

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// Runs a very simple form of copy propagation.
	/// Copy propagation is used in two cases:
	/// 1) assignments from arguments to local variables
	///    If the target variable is assigned to only once (so always is that argument) and the argument is never changed (no ldarga/starg),
	///    then we can replace the variable with the argument.
	/// 2) assignments of address-loading instructions to local variables
	/// </summary>
	public class CopyPropagation : IILTransform
	{
		public void Run(ILFunction function, ILTransformContext context)
		{
			foreach (var block in function.Descendants.OfType<Block>()) {
				for (int i = 0; i < block.Instructions.Count; i++) {
					ILVariable v;
					ILInstruction copiedExpr;
					if (block.Instructions[i].MatchStLoc(out v, out copiedExpr)) {
						if (v.IsSingleDefinition && v.Kind != VariableKind.Parameter && CanPerformCopyPropagation(v, copiedExpr)) {
							// un-inline the arguments of the ldArg instruction
							ILVariable[] uninlinedArgs = new ILVariable[copiedExpr.Children.Count];
							for (int j = 0; j < uninlinedArgs.Length; j++) {
								var arg = copiedExpr.Children[j];
								var type = context.TypeSystem.Compilation.FindType(arg.ResultType.ToKnownTypeCode());
								uninlinedArgs[j] = new ILVariable(VariableKind.StackSlot, type, arg.ResultType, arg.ILRange.Start) {
									Name = "C_" + arg.ILRange.Start
								};
								block.Instructions.Insert(i++, new StLoc(uninlinedArgs[j], arg));
							}
							// perform copy propagation:
							foreach (var expr in function.Descendants) {
								if (expr.MatchLdLoc(v)) {
									var clone = copiedExpr.Clone();
									for (int j = 0; j < uninlinedArgs.Length; j++) {
										clone.Children[j].ReplaceWith(new LdLoc(uninlinedArgs[j]));
									}
									expr.ReplaceWith(clone);
								}
							}
							block.Instructions.RemoveAt(i);
							int c = new ILInlining().InlineInto(block, i, aggressive: false);
							i -= uninlinedArgs.Length + c + 1;
						}
					}
				}
			}
		}

		static bool CanPerformCopyPropagation(ILVariable target, ILInstruction value)
		{
			switch (value.OpCode) {
				case OpCode.LdLoca:
				case OpCode.LdElema:
				case OpCode.LdFlda:
				case OpCode.LdsFlda:
					// All address-loading instructions always return the same value for a given operand/argument combination,
					// so they can be safely copied.
					return true;
				case OpCode.LdLoc:
					var v = ((LdLoc)value).Variable;
					switch (v.Kind) {
						case VariableKind.This:
						case VariableKind.Parameter:
							// Parameters can be copied only if they aren't assigned to (directly or indirectly via ldarga)
							// note: the initialization by the caller is the first store -> StoreCount must be 1
							return v.IsSingleDefinition;
						case VariableKind.StackSlot:
							// Variables are be copied only if both they and the target copy variable are generated,
							// and if the variable has only a single assignment
							return v.IsSingleDefinition && v.Kind == VariableKind.StackSlot && target.Kind == VariableKind.StackSlot;
						default:
							return false;
					}
				default:
					return false;
			}
		}
	}
}


