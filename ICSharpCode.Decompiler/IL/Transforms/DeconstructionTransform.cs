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
using System.Linq;
using System.Resources;

using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// 
	/// </summary>
	class DeconstructionTransform : IStatementTransform
	{
		StatementTransformContext context;

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
			if (!context.Settings.Deconstruction)
				return;

			try {
				this.context = context;

				if (TransformDeconstruction(block, pos))
					return;
				if (InlineDeconstructionInitializer(block, pos))
					return;
			} finally {
				this.context = null;
			}
		}

		struct ConversionInfo
		{
			public IType inputType;
			public Conv conv;
		}

		/// <summary>
		/// stloc v(lhs)
		/// expr(..., deconstruct { ... }, ...)
		/// =>
		/// expr(..., deconstruct { init: stloc v(lhs) ... }, ...)
		/// </summary>
		bool InlineDeconstructionInitializer(Block block, int pos)
		{
			if (!block.Instructions[pos].MatchStLoc(out var v, out var value))
				return false;
			if (!(v.IsSingleDefinition && v.LoadCount == 1))
				return false;
			if (pos + 1 >= block.Instructions.Count)
				return false;
			var result = ILInlining.FindLoadInNext(block.Instructions[pos + 1], v, value, InliningOptions.FindDeconstruction);
			if (result.Type != ILInlining.FindResultType.Deconstruction)
				return false;
			var deconstruction = (DeconstructInstruction)result.LoadInst;
			if (!v.LoadInstructions[0].IsDescendantOf(deconstruction.Assignments))
				return false;
			if (deconstruction.Init.Count > 0) {
				var a = deconstruction.Init[0].Variable.LoadInstructions.Single();
				var b = v.LoadInstructions.Single();
				if (!b.IsBefore(a))
					return false;
			}
			context.Step("InlineDeconstructionInitializer", block.Instructions[pos]);
			deconstruction.Init.Insert(0, (StLoc)block.Instructions[pos]);
			block.Instructions.RemoveAt(pos);
			v.Kind = VariableKind.DeconstructionInitTemporary;
			return true;
		}

		bool TransformDeconstruction(Block block, int pos)
		{
			int startPos = pos;
			Action<DeconstructInstruction> delayedActions = null;
			if (MatchDeconstruction(block.Instructions[pos], out var deconstructMethod,
				out var rootTestedOperand, out var deconstructionResults,
				out var deconstructionResultsLookup))
			{
				pos++;
			}
			else {
				return false;
			}
			if (!MatchConversions(block, ref pos, deconstructionResultsLookup, out var conversions, out var conversionStLocs))
				return false;

			if (!MatchAssignments(block, ref pos, deconstructionResultsLookup, conversions, conversionStLocs, ref delayedActions))
				return false;
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
				replacement.Pattern.SubPatterns.Add(
					new MatchInstruction(
						result,
						new DeconstructResultInstruction(index, result.StackType, new LdLoc(rootTempVariable))
					)
				);
				index++;
			}
			replacement.Conversions = new Block(BlockKind.DeconstructionConversions);
			foreach (var convInst in conversionStLocs) {
				replacement.Conversions.Instructions.Add(convInst);
			}
			replacement.Assignments = new Block(BlockKind.DeconstructionAssignments);
			delayedActions?.Invoke(replacement);
			block.Instructions[startPos] = replacement;
			block.Instructions.RemoveRange(startPos + 1, pos - startPos - 1);
			return true;
		}

		bool MatchDeconstruction(ILInstruction inst, out IMethod deconstructMethod,
			out ILInstruction testedOperand, out List<ILVariable> deconstructionResults,
			out Dictionary<ILVariable, int> deconstructionResultsLookup)
		{
			testedOperand = null;
			deconstructMethod = null;
			deconstructionResults = null;
			deconstructionResultsLookup = null;
			if (!(inst is CallInstruction call))
				return false;
			if (!MatchInstruction.IsDeconstructMethod(call.Method))
				return false;
			if (call.Method.IsStatic == call is CallVirt)
				return false;
			if (call.Arguments.Count < 3)
				return false;
			deconstructionResults = new List<ILVariable>();
			deconstructionResultsLookup = new Dictionary<ILVariable, int>();
			for (int i = 1; i < call.Arguments.Count; i++) {
				if (!call.Arguments[i].MatchLdLoca(out var v))
					return false;
				// TODO v.LoadCount may be 2 if the deconstruction is assigned to a tuple variable
				// or 0? because of discards
				if (!(v.StoreCount == 0 && v.AddressCount == 1 && v.LoadCount <= 1))
					return false;
				deconstructionResultsLookup.Add(v, i - 1);
				deconstructionResults.Add(v);
			}
			testedOperand = call.Arguments[0];
			deconstructMethod = call.Method;
			return true;
		}

		bool MatchConversions(Block block, ref int pos,
			Dictionary<ILVariable, int> deconstructionResultsLookup,
			out Dictionary<ILVariable, ConversionInfo> conversions,
			out List<StLoc> conversionStLocs)
		{
			conversions = new Dictionary<ILVariable, ConversionInfo>();
			conversionStLocs = new List<StLoc>();
			int previousIndex = -1;
			while (MatchConversion(
				block.Instructions.ElementAtOrDefault(pos), out var inputInstruction,
				out var outputVariable, out var info))
			{
				if (!inputInstruction.MatchLdLoc(out var inputVariable))
					return false;
				if (!deconstructionResultsLookup.TryGetValue(inputVariable, out int index))
					return false;
				if (index <= previousIndex)
					return false;
				if (!(outputVariable.IsSingleDefinition && outputVariable.LoadCount == 1))
					return false;
				deconstructionResultsLookup.Remove(inputVariable);
				deconstructionResultsLookup.Add(outputVariable, index);
				conversions.Add(outputVariable, info);
				conversionStLocs.Add((StLoc)block.Instructions[pos]);
				pos++;
				previousIndex = index;
			}
			return true;
		}

		bool MatchConversion(ILInstruction inst, out ILInstruction inputInstruction,
			out ILVariable outputVariable, out ConversionInfo info)
		{
			info = default;
			inputInstruction = null;
			if (!inst.MatchStLoc(out outputVariable, out var value))
				return false;
			if (!(value is Conv conv))
				return false;
			info = new ConversionInfo {
				inputType = conv.Argument.InferType(context.TypeSystem),
				conv = conv
			};
			inputInstruction = conv.Argument;
			return true;
		}

		bool MatchAssignments(Block block, ref int pos,
			Dictionary<ILVariable, int> deconstructionResultsLookup,
			Dictionary<ILVariable, ConversionInfo> conversions,
			List<StLoc> conversionStLocs,
			ref Action<DeconstructInstruction> delayedActions)
		{
			int previousIndex = -1;
			int conversionStLocIndex = 0;
			int startPos = pos;
			while (MatchAssignment(block.Instructions.ElementAtOrDefault(pos), out var targetType, out var valueInst, out var addAssignment)) {
				if (!valueInst.MatchLdLoc(out var resultVariable))
					return false;
				if (!deconstructionResultsLookup.TryGetValue(resultVariable, out int index))
					return false;
				if (index <= previousIndex)
					return false;
				AddMissingAssignmentsForConversions(index, ref delayedActions);
				if (!conversions.TryGetValue(resultVariable, out var conversionInfo)) {
					conversionInfo = new ConversionInfo {
						inputType = resultVariable.Type
					};
				}
				if (block.Instructions[pos].MatchStLoc(out var assignmentTarget, out _)
					&& assignmentTarget.Kind == VariableKind.StackSlot
					&& assignmentTarget.IsSingleDefinition
					&& conversionInfo.conv == null)
				{
					delayedActions += _ => {
						assignmentTarget.Type = conversionInfo.inputType;
					};
				}
				else
				{
					if (!IsCompatibleImplicitConversion(targetType, conversionInfo))
						return false;
				}
				delayedActions += addAssignment;
				pos++;
				previousIndex = index;
			}
			AddMissingAssignmentsForConversions(int.MaxValue, ref delayedActions);
			return startPos != pos;

			void AddMissingAssignmentsForConversions(int index, ref Action<DeconstructInstruction> delayedActions)
			{
				while (conversionStLocIndex < conversionStLocs.Count) {
					var stLoc = conversionStLocs[conversionStLocIndex];
					int conversionResultIndex = deconstructionResultsLookup[stLoc.Variable];

					if (conversionResultIndex >= index)
						break;
					if (conversionResultIndex > previousIndex) {
						delayedActions += (DeconstructInstruction deconstructInst) => {
							var freshVar = context.Function.RegisterVariable(VariableKind.StackSlot, stLoc.Variable.Type);
							deconstructInst.Assignments.Instructions.Add(new StLoc(stLoc.Variable, new LdLoc(freshVar)));
							stLoc.Variable = freshVar;
						};
						
					}
					conversionStLocIndex++;
				}
			}
		}

		bool MatchAssignment(ILInstruction inst, out IType targetType, out ILInstruction valueInst, out Action<DeconstructInstruction> addAssignment)
		{
			targetType = null;
			valueInst = null;
			addAssignment = null;
			if (inst == null)
				return false;
			if (inst.MatchStLoc(out var v, out var value)
				&& value is Block block && block.MatchInlineAssignBlock(out var call, out valueInst)) {
				if (!DeconstructInstruction.IsAssignment(call, out targetType, out _))
					return false;
				if (!(v.IsSingleDefinition && v.LoadCount == 0))
					return false;
				var valueInstCopy = valueInst;
				addAssignment = (DeconstructInstruction deconstructInst) => {
					call.Arguments[call.Arguments.Count - 1] = valueInstCopy;
					deconstructInst.Assignments.Instructions.Add(call);
				};
				return true;
			} else if (DeconstructInstruction.IsAssignment(inst, out targetType, out valueInst)) {
				// OK - use the assignment as is
				addAssignment = (DeconstructInstruction deconstructInst) => {
					deconstructInst.Assignments.Instructions.Add(inst);
				};
				return true;
			} else {
				return false;
			}
		}

		bool IsCompatibleImplicitConversion(IType targetType, ConversionInfo conversionInfo)
		{
			var c = CSharpConversions.Get(context.TypeSystem)
				.ImplicitConversion(conversionInfo.inputType, targetType);
			if (!c.IsValid)
				return false;
			var inputType = conversionInfo.inputType;
			var conv = conversionInfo.conv;
			if (c.IsIdentityConversion) {
				return conv == null || conv.Kind == ConversionKind.Nop;
			}
			if (c.IsNumericConversion) {
				switch (conv.Kind) {
					case ConversionKind.IntToFloat:
						return inputType.GetSign() == conv.InputSign;
					case ConversionKind.FloatPrecisionChange:
						return true;
					case ConversionKind.SignExtend:
						return inputType.GetSign() == Sign.Signed;
					case ConversionKind.ZeroExtend:
						return inputType.GetSign() == Sign.Unsigned;
					default:
						return false;
				}
			}
			return false;
		}
	}
}
