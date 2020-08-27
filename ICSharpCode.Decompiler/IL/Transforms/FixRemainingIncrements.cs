// Copyright (c) 2019 Daniel Grunwald
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

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	public class FixRemainingIncrements : IILTransform
	{
		void IILTransform.Run(ILFunction function, ILTransformContext context)
		{
			var callsToFix = new List<Call>();
			foreach (var call in function.Descendants.OfType<Call>())
			{
				if (!(call.Method.IsOperator && (call.Method.Name == "op_Increment" || call.Method.Name == "op_Decrement")))
					continue;
				if (call.Arguments.Count != 1)
					continue;
				if (call.Method.DeclaringType.IsKnownType(KnownTypeCode.Decimal))
				{
					// For decimal, legacy csc can optimize "d + 1m" to "op_Increment(d)".
					// We can handle these calls in ReplaceMethodCallsWithOperators.
					continue;
				}
				callsToFix.Add(call);
			}
			foreach (var call in callsToFix)
			{
				// A user-defined increment/decrement that was not handled by TransformAssignment.
				// This can happen because the variable-being-incremented was optimized out by Roslyn,
				// e.g.
				//   public void Issue1552Pre(UserType a, UserType b)
				//   {
				//      UserType num = a + b;
				//      Console.WriteLine(++num);
				//   }
				// can end up being compiled to:
				//      Console.WriteLine(UserType.op_Increment(a + b));
				if (call.SlotInfo == StLoc.ValueSlot && call.Parent.SlotInfo == Block.InstructionSlot)
				{
					var store = (StLoc)call.Parent;
					var block = (Block)store.Parent;
					context.Step($"Fix {call.Method.Name} call at 0x{call.StartILOffset:x4} using {store.Variable.Name}", call);
					// stloc V(call op_Increment(...))
					// ->
					// stloc V(...)
					// compound.assign op_Increment(V)
					call.ReplaceWith(call.Arguments[0]);
					block.Instructions.Insert(store.ChildIndex + 1,
						new UserDefinedCompoundAssign(call.Method, CompoundEvalMode.EvaluatesToNewValue,
						new LdLoca(store.Variable), CompoundTargetKind.Address, new LdcI4(1)).WithILRange(call));
				}
				else
				{
					context.Step($"Fix {call.Method.Name} call at 0x{call.StartILOffset:x4} using new local", call);
					var newVariable = call.Arguments[0].Extract();
					if (newVariable == null)
					{
						Debug.Fail("Failed to extract argument of remaining increment/decrement");
						continue;
					}
					newVariable.Type = call.GetParameter(0).Type;
					Debug.Assert(call.Arguments[0].MatchLdLoc(newVariable));
					call.ReplaceWith(new UserDefinedCompoundAssign(call.Method, CompoundEvalMode.EvaluatesToNewValue,
						new LdLoca(newVariable), CompoundTargetKind.Address, new LdcI4(1)).WithILRange(call));
				}
			}
		}
	}
}
