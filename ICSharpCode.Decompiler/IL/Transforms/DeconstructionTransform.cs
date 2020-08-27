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
using System.Collections.Immutable;
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
		readonly Dictionary<ILVariable, int> deconstructionResultsLookup = new Dictionary<ILVariable, int>();
		ILVariable[] deconstructionResults;
		ILVariable tupleVariable;
		TupleType tupleType;

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

			try
			{
				this.context = context;
				Reset();

				if (TransformDeconstruction(block, pos))
					return;
				if (InlineDeconstructionInitializer(block, pos))
					return;
			}
			finally
			{
				this.context = null;
				Reset();
			}
		}

		private void Reset()
		{
			this.deconstructionResultsLookup.Clear();
			this.tupleVariable = null;
			this.tupleType = null;
			this.deconstructionResults = null;
		}

		struct ConversionInfo
		{
			public IType inputType;
			public Conv conv;
		}

		/// <summary>
		/// Get index of deconstruction result or tuple element
		/// Returns -1 on failure.
		/// </summary>
		int FindIndex(ILInstruction inst, out Action<DeconstructInstruction> delayedActions)
		{
			delayedActions = null;
			if (inst.MatchLdLoc(out var v))
			{
				if (!deconstructionResultsLookup.TryGetValue(v, out int index))
					return -1;
				return index;
			}
			if (inst.MatchLdFld(out _, out _))
			{
				if (!TupleTransform.MatchTupleFieldAccess((LdFlda)((LdObj)inst).Target, out var tupleType, out var target, out int index))
					return -1;
				// Item fields are one-based, we use zero-based indexing.
				index--;
				// normalize tuple type
				tupleType = TupleType.FromUnderlyingType(context.TypeSystem, tupleType);
				if (!target.MatchLdLoca(out v))
					return -1;
				if (this.tupleVariable == null)
				{
					this.tupleVariable = v;
					this.tupleType = (TupleType)tupleType;
					this.deconstructionResults = new ILVariable[this.tupleType.Cardinality];
				}
				if (this.tupleType.Cardinality < 2)
					return -1;
				if (v != tupleVariable || !this.tupleType.Equals(tupleType))
					return -1;
				if (this.deconstructionResults[index] == null)
				{
					var freshVar = new ILVariable(VariableKind.StackSlot, this.tupleType.ElementTypes[index]) { Name = "E_" + index };
					delayedActions += _ => context.Function.Variables.Add(freshVar);
					this.deconstructionResults[index] = freshVar;
				}
				delayedActions += _ => {
					inst.ReplaceWith(new LdLoc(this.deconstructionResults[index]));
				};
				return index;
			}
			return -1;
		}

		/// <summary>
		/// stloc v(value)
		/// expr(..., deconstruct { ... }, ...)
		/// =>
		/// expr(..., deconstruct { init: stloc v(value) ... }, ...)
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
			LdLoc loadInst = v.LoadInstructions[0];
			if (!loadInst.IsDescendantOf(deconstruction.Assignments))
				return false;
			if (loadInst.SlotInfo == StObj.TargetSlot)
			{
				if (value.OpCode == OpCode.LdFlda || value.OpCode == OpCode.LdElema)
					return false;
			}
			if (deconstruction.Init.Count > 0)
			{
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
			if (MatchDeconstruction(block.Instructions[pos], out IMethod deconstructMethod,
				out ILInstruction rootTestedOperand))
			{
				pos++;
			}
			if (!MatchConversions(block, ref pos, out var conversions, out var conversionStLocs, ref delayedActions))
				return false;

			if (!MatchAssignments(block, ref pos, conversions, conversionStLocs, ref delayedActions))
				return false;
			// first tuple element may not be discarded,
			// otherwise we would run this transform on a suffix of the actual pattern.
			if (deconstructionResults[0] == null)
				return false;
			context.Step("Deconstruction", block.Instructions[startPos]);
			DeconstructInstruction replacement = new DeconstructInstruction();
			IType deconstructedType;
			if (deconstructMethod == null)
			{
				deconstructedType = this.tupleType;
				rootTestedOperand = new LdLoc(this.tupleVariable);
			}
			else
			{
				if (deconstructMethod.IsStatic)
				{
					deconstructedType = deconstructMethod.Parameters[0].Type;
				}
				else
				{
					deconstructedType = deconstructMethod.DeclaringType;
				}
			}
			var rootTempVariable = context.Function.RegisterVariable(VariableKind.PatternLocal, deconstructedType);
			replacement.Pattern = new MatchInstruction(rootTempVariable, deconstructMethod, rootTestedOperand) {
				IsDeconstructCall = deconstructMethod != null,
				IsDeconstructTuple = this.tupleType != null
			};
			int index = 0;
			foreach (ILVariable v in deconstructionResults)
			{
				var result = v;
				if (result == null)
				{
					var freshVar = new ILVariable(VariableKind.PatternLocal, this.tupleType.ElementTypes[index]) { Name = "E_" + index };
					context.Function.Variables.Add(freshVar);
					result = freshVar;
				}
				else
				{
					result.Kind = VariableKind.PatternLocal;
				}
				replacement.Pattern.SubPatterns.Add(
					new MatchInstruction(
						result,
						new DeconstructResultInstruction(index, result.StackType, new LdLoc(rootTempVariable))
					)
				);
				index++;
			}
			replacement.Conversions = new Block(BlockKind.DeconstructionConversions);
			foreach (var convInst in conversionStLocs)
			{
				replacement.Conversions.Instructions.Add(convInst);
			}
			replacement.Assignments = new Block(BlockKind.DeconstructionAssignments);
			delayedActions?.Invoke(replacement);
			block.Instructions[startPos] = replacement;
			block.Instructions.RemoveRange(startPos + 1, pos - startPos - 1);
			return true;
		}

		bool MatchDeconstruction(ILInstruction inst, out IMethod deconstructMethod,
			out ILInstruction testedOperand)
		{
			testedOperand = null;
			deconstructMethod = null;
			deconstructionResults = null;
			if (!(inst is CallInstruction call))
				return false;
			if (!MatchInstruction.IsDeconstructMethod(call.Method))
				return false;
			if (call.Method.IsStatic == call is CallVirt)
				return false;
			if (call.Arguments.Count < 3)
				return false;
			deconstructionResults = new ILVariable[call.Arguments.Count - 1];
			for (int i = 0; i < deconstructionResults.Length; i++)
			{
				if (!call.Arguments[i + 1].MatchLdLoca(out var v))
					return false;
				// TODO v.LoadCount may be 2 if the deconstruction is assigned to a tuple variable
				// or 0? because of discards
				if (!(v.StoreCount == 0 && v.AddressCount == 1 && v.LoadCount <= 1))
					return false;
				deconstructionResultsLookup.Add(v, i);
				deconstructionResults[i] = v;
			}
			testedOperand = call.Arguments[0];
			deconstructMethod = call.Method;
			return true;
		}

		bool MatchConversions(Block block, ref int pos,
			out Dictionary<ILVariable, ConversionInfo> conversions,
			out List<StLoc> conversionStLocs,
			ref Action<DeconstructInstruction> delayedActions)
		{
			conversions = new Dictionary<ILVariable, ConversionInfo>();
			conversionStLocs = new List<StLoc>();
			int previousIndex = -1;
			while (MatchConversion(
				block.Instructions.ElementAtOrDefault(pos), out var inputInstruction,
				out var outputVariable, out var info))
			{
				int index = FindIndex(inputInstruction, out var tupleAccessAdjustment);
				if (index <= previousIndex)
					return false;
				if (!(outputVariable.IsSingleDefinition && outputVariable.LoadCount == 1))
					return false;
				delayedActions += tupleAccessAdjustment;
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
			Dictionary<ILVariable, ConversionInfo> conversions,
			List<StLoc> conversionStLocs,
			ref Action<DeconstructInstruction> delayedActions)
		{
			int previousIndex = -1;
			int conversionStLocIndex = 0;
			int startPos = pos;
			while (MatchAssignment(block.Instructions.ElementAtOrDefault(pos), out var targetType, out var valueInst, out var addAssignment))
			{
				int index = FindIndex(valueInst, out var tupleAccessAdjustment);
				if (index <= previousIndex)
					return false;
				AddMissingAssignmentsForConversions(index, ref delayedActions);
				if (!(valueInst.MatchLdLoc(out var resultVariable)
					&& conversions.TryGetValue(resultVariable, out var conversionInfo)))
				{
					conversionInfo = new ConversionInfo {
						inputType = valueInst.InferType(context.TypeSystem)
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
				delayedActions += tupleAccessAdjustment;
				pos++;
				previousIndex = index;
			}
			AddMissingAssignmentsForConversions(int.MaxValue, ref delayedActions);
			return startPos != pos;

			void AddMissingAssignmentsForConversions(int index, ref Action<DeconstructInstruction> delayedActions)
			{
				while (conversionStLocIndex < conversionStLocs.Count)
				{
					var stLoc = conversionStLocs[conversionStLocIndex];
					int conversionResultIndex = deconstructionResultsLookup[stLoc.Variable];

					if (conversionResultIndex >= index)
						break;
					if (conversionResultIndex > previousIndex)
					{
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
				&& value is Block block && block.MatchInlineAssignBlock(out var call, out valueInst))
			{
				if (!DeconstructInstruction.IsAssignment(call, context.TypeSystem, out targetType, out _))
					return false;
				if (!(v.IsSingleDefinition && v.LoadCount == 0))
					return false;
				var valueInstCopy = valueInst;
				addAssignment = (DeconstructInstruction deconstructInst) => {
					call.Arguments[call.Arguments.Count - 1] = valueInstCopy;
					deconstructInst.Assignments.Instructions.Add(call);
				};
				return true;
			}
			else if (DeconstructInstruction.IsAssignment(inst, context.TypeSystem, out targetType, out valueInst))
			{
				// OK - use the assignment as is
				addAssignment = (DeconstructInstruction deconstructInst) => {
					deconstructInst.Assignments.Instructions.Add(inst);
				};
				return true;
			}
			else
			{
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
			if (c.IsIdentityConversion || c.IsReferenceConversion)
			{
				return conv == null || conv.Kind == ConversionKind.Nop;
			}
			if (c.IsNumericConversion)
			{
				switch (conv.Kind)
				{
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
