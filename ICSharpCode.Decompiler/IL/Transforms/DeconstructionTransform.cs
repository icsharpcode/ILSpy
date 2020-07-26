// Copyright (c) 2020 Siegfried Pammer
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
using System.Linq.Expressions;
using System.Text;

using ICSharpCode.Decompiler.CSharp.Syntax.PatternMatching;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// 
	/// </summary>
	class DeconstructionTransform : IStatementTransform
	{
		StatementTransformContext context;
		readonly Dictionary<ILVariable, int> deconstructionResultsLookup = new Dictionary<ILVariable, int>();

		/*
			stloc tuple(call MakeIntIntTuple(ldloc this))
		----
			stloc myInt(call op_Implicit(ldfld Item2(ldloca tuple)))
			stloc a(ldfld Item1(ldloca tuple))
			stloc b(ldloc myInt)
		==>
			deconstruct {
				init:
					<empty>
				deconstruct:
					match.deconstruct(temp = ldloca tuple) {
						match(result0 = deconstruct.result 0(temp)),
						match(result1 = deconstruct.result 1(temp))
					}
				conversions: {
					stloc conv2(call op_Implicit(ldloc result1))
				}
				assignments: {
					stloc a(ldloc result0)
					stloc b(ldloc conv2)
				}
			}
		 * */
		void IStatementTransform.Run(Block block, int pos, StatementTransformContext context)
		{
			//if (!context.Settings.Deconstruction)
			//	return;

			try {
				this.context = context;
				this.deconstructionResultsLookup.Clear();
				int startPos = pos;
				if (!MatchDeconstruction(block, ref pos, out var deconstructMethod, out var rootTestedOperand, out var deconstructionResults))
					return;
				if (!MatchConversion(block, ref pos))
					return;
				if (!MatchAssignments(block, ref pos, out var assignments))
					return;
				context.Step("Deconstruction", block.Instructions[startPos]);
				DeconstructInstruction replacement = new DeconstructInstruction();
				IType deconstructedType;
				if (deconstructMethod.IsStatic) {
					deconstructedType = deconstructMethod.Parameters[0].Type;
				} else {
					deconstructedType = deconstructMethod.DeclaringType;
				}
				var rootTempVariable = context.Function.RegisterVariable(VariableKind.PatternLocal, deconstructedType);
				replacement.Pattern = new MatchInstruction(rootTempVariable, deconstructMethod, rootTestedOperand) {
					IsDeconstructCall = true
				};
				int index = 0;
				foreach (var result in deconstructionResults) {
					result.Kind = VariableKind.PatternLocal;
					replacement.Pattern.SubPatterns.Add(new MatchInstruction(result, new DeconstructResultInstruction(index, result.StackType, new LdLoc(rootTempVariable))));
					index++;
				}
				replacement.Conversions = new Block(BlockKind.DeconstructionConversions);
				replacement.Assignments = new Block(BlockKind.DeconstructionAssignments);
				replacement.Assignments.Instructions.AddRange(assignments);
				block.Instructions[startPos] = replacement;
				block.Instructions.RemoveRange(startPos + 1, pos - startPos - 1);
			} finally {
				this.context = null;
				this.deconstructionResultsLookup.Clear();
			}
		}

		bool MatchDeconstruction(Block block, ref int pos, out IMethod deconstructMethod, out ILInstruction testedOperand, out List<ILVariable> deconstructionResults)
		{
			testedOperand = null;
			deconstructMethod = null;
			deconstructionResults = null;
			// TODO nested deconstruction / tuple deconstruction
			if (!(block.Instructions[pos] is CallInstruction call))
				return false;
			if (!MatchInstruction.IsDeconstructMethod(call.Method))
				return false;
			if (call.Method.IsStatic == call is CallVirt)
				return false;
			if (call.Arguments.Count < 3)
				return false;
			deconstructionResults = new List<ILVariable>();
			for (int i = 1; i < call.Arguments.Count; i++) {
				if (!call.Arguments[i].MatchLdLoca(out var v))
					return false;
				// TODO v.LoadCount may be 2 if the deconstruction is assigned to a tuple variable
				// or 0? because of discards
				if (!(v.StoreCount == 0 && v.AddressCount == 1 && v.LoadCount == 1))
					return false;
				deconstructionResultsLookup.Add(v, i - 1);
				deconstructionResults.Add(v);
			}
			testedOperand = call.Arguments[0];
			deconstructMethod = call.Method;
			pos++;
			return true;
		}

		bool MatchConversion(Block block, ref int pos)
		{
			// TODO
			return true;
		}

		bool MatchAssignments(Block block, ref int pos, out List<ILInstruction> assignments)
		{
			assignments = new List<ILInstruction>();
			while (MatchAssignment(block.Instructions.ElementAtOrDefault(pos))) {
				assignments.Add(block.Instructions[pos]);
				pos++;
			}
			return assignments.Count > 0;
		}

		bool MatchAssignment(ILInstruction inst)
		{
			if (!DeconstructInstruction.IsAssignment(inst, out var resultVariable))
				return false;
			if (!deconstructionResultsLookup.ContainsKey(resultVariable))
				return false;
			// TODO check order of use

			return true;
		}
	}
}
