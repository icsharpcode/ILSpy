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

#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Detects that a run of statements is a lowered deconstruction assignment - rooted in a
	/// Deconstruct call or in tuple element reads, including nested designations - and folds
	/// it into a single DeconstructInstruction.
	/// </summary>
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

		A nested designation over Deconstruct calls (var (x, (a, b)) = o;) chains the calls,
		with a defensive copy for struct elements:
			call Deconstruct(ldloc o, ldloca x', ldloca inner)
			call Deconstruct(ldloca inner, ldloca a', ldloca b')
			...conversions/assignments over the leaves x', a', b'...

		A nested designation over tuples (var (x, (a, b)) = t;) is lowered to one temporary
		per nested designation, followed by element reads in depth-first leaf order:
			stloc inner(ldobj(ldflda Item2(ldloca t)))
			stloc x(ldobj(ldflda Item1(ldloca t)))
			stloc a(ldobj(ldflda Item1(ldloca inner)))
			stloc b(ldobj(ldflda Item2(ldloca inner)))
	 * */
	class DeconstructionTransform : IStatementTransform
	{
		StatementTransformContext context = null!;
		readonly Dictionary<ILVariable, int> deconstructionResultsLookup = new Dictionary<ILVariable, int>();
		ILVariable?[] deconstructionResults = null!;
		ILVariable? tupleVariable;
		TupleType? tupleType;

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
				this.context = null!;
				Reset();
			}
		}

		private void Reset()
		{
			this.deconstructionResultsLookup.Clear();
			this.tupleVariable = null;
			this.tupleType = null;
			this.deconstructionResults = null!;
		}

		/// <summary>
		/// call Deconstruct(target, ldloca out0, ...) [+ nested Deconstruct calls]
		///   | stloc temp(ldobj(ldflda ItemN(ldloca tuple))) ... [nested tuple designations]
		/// stloc conv0(conv(...)) ...
		/// assignments ...
		/// =>
		/// deconstruct { init: pattern: conversions: assignments: }   (see class comment)
		/// </summary>
		bool TransformDeconstruction(Block block, int pos)
		{
			int startPos = pos;
			// Blocks are processed back to front, so the inner parts of a nested deconstruction
			// are visited before the position its matching starts at; matching them on their own
			// would consume the pattern piecemeal. Defer to the enclosing attempt where one
			// exists (see the guard for the precision guarantees).
			if (IsConsumableByEnclosingDeconstruction(block, pos))
				return false;
			if (!MatchDeconstructionSequence(block, startPos, out pos, out var rootCall,
				out var rootTestedOperand, out var conversionStLocs, out var delayedActions))
			{
				return false;
			}
			context.Step("Deconstruction", block.Instructions[startPos]);
			DeconstructInstruction replacement = new DeconstructInstruction();
			IMethod? deconstructMethod = rootCall?.Method;
			IType deconstructedType;
			if (deconstructMethod == null)
			{
				deconstructedType = this.tupleType!;
				rootTestedOperand = new LdLoc(this.tupleVariable!);
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
			if (rootCall != null)
			{
				replacement.Pattern = BuildPatternMatch(rootCall, rootTempVariable, rootTestedOperand!);
			}
			else
			{
				replacement.Pattern = new MatchInstruction(rootTempVariable, method: null, rootTestedOperand!) {
					IsDeconstructTuple = true
				};
				for (int i = 0; i < deconstructionResults.Length; i++)
				{
					var result = deconstructionResults[i];
					if (result == null)
					{
						var freshVar = new ILVariable(VariableKind.PatternLocal, this.tupleType!.ElementTypes[i]) { Name = "E_" + i };
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
							new DeconstructResultInstruction(i, result.StackType, new LdLoc(rootTempVariable))
						)
					);
				}
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
			context.EndStep(replacement);
			return true;
		}

		/// <summary>
		/// Matches the full statement sequence of one deconstruction, starting at startPos:
		///   [Deconstruct call + nested calls | nested tuple designation temporaries]
		///   [conversions]
		///   [assignments]
		/// On success, endPos is the position after the last consumed statement.
		/// The block is not modified; all rewrites are accumulated in delayedActions.
		/// </summary>
		bool MatchDeconstructionSequence(Block block, int startPos, out int endPos,
			out DeconstructionCall? rootCall, out ILInstruction? rootTestedOperand,
			out List<StLoc> conversionStLocs, out Action<DeconstructInstruction>? delayedActions)
		{
			Reset();
			endPos = startPos;
			int pos = startPos;
			delayedActions = null;
			MatchDeconstruction(block, ref pos, out rootCall, out rootTestedOperand);
			if (!MatchConversions(block, ref pos, out var conversions, out conversionStLocs, ref delayedActions))
				return false;
			if (!MatchAssignments(block, ref pos, conversions, conversionStLocs, ref delayedActions,
				allowUnrelatedAssignments: rootCall != null, out bool anyAssignments))
			{
				return false;
			}
			// Without any assignment the statement is a plain Deconstruct call, unless a nested
			// deconstruction was consumed: then all leaves are single-use elements handled by
			// the forwarding fixup in MatchAssignments.
			if (!anyAssignments && !(rootCall != null && rootCall.NestedCalls.Any(c => c != null)))
				return false;
			// first tuple element may not be discarded,
			// otherwise we would run this transform on a suffix of the actual pattern.
			if (deconstructionResults[0] == null)
				return false;
			endPos = pos;
			return true;
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
			if (!(v.IsSingleDefinition && v.LoadInstructions is [var loadInst]))
				return false;
			if (pos + 1 >= block.Instructions.Count)
				return false;
			var result = ILInlining.FindLoadInNext(block.Instructions[pos + 1], v, value, InliningOptions.FindDeconstruction);
			if (result.Type != ILInlining.FindResultType.Deconstruction)
				return false;
			var deconstruction = (DeconstructInstruction)result.LoadInst;
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
				if (!loadInst.IsBefore(a))
					return false;
			}
			context.Step("InlineDeconstructionInitializer", block.Instructions[pos]);
			deconstruction.Init.Insert(0, (StLoc)block.Instructions[pos]);
			block.Instructions.RemoveAt(pos);
			v.Kind = VariableKind.DeconstructionInitTemporary;
			return true;
		}

		/// <summary>
		/// Whether the statement at pos belongs to a deconstruction whose matching starts at an
		/// earlier position in the block, in either nesting shape:
		///
		///   call Deconstruct(..., ldloca inner, ...)             at enclosingPos
		///   ...
		///   call Deconstruct(ldloc(a) inner, ...)                at pos
		///
		///   stloc temp(ldobj(ldflda ItemN(ldloc(a) outer)))      at enclosingPos
		///   ...
		///   stloc x([conv](ldobj(ldflda ItemK(ldloc(a) temp))))  at pos
		///
		/// Both shapes are decided by the same dry run of the enclosing match: only a match that
		/// reaches beyond pos absorbs the statement there. A barrier statement between the two
		/// positions, an element with uses the nesting cannot consume, or a conversion or
		/// assignment the enclosing pattern does not account for makes the dry run stop short,
		/// and the deconstruction at pos is then still transformed on its own. What the dry run
		/// cannot promise is that the enclosing attempt still matches once the walk reaches it:
		/// the positions in between are visited first and may rewrite the block. The back-to-front
		/// walk gives this position no second chance, but losing the match there only costs
		/// sugar, never correctness.
		/// </summary>
		bool IsConsumableByEnclosingDeconstruction(Block block, int pos)
		{
			if (!TryFindEnclosingDeconstructionCall(block, pos, out int enclosingPos))
				return false;
			// The dry run leaves the matcher state behind, which is safe because it runs before
			// the attempt at this position, and both that attempt and Run reset it. It does not
			// modify the block: all rewrites are delayed actions.
			return MatchDeconstructionSequence(block, enclosingPos, out int endPos, out _, out _, out _, out _)
				&& endPos > pos;
		}

		/// <summary>
		/// call Deconstruct(..., ldloca v, ...)                  at enclosingPos
		/// [stloc copy(ldloc v)]                                 defensive copy of a struct element
		/// ...
		/// call Deconstruct(ldloc(a) v|copy, ...)                at pos
		/// </summary>
		static bool TryFindEnclosingDeconstructionCall(Block block, int pos, out int enclosingPos)
		{
			enclosingPos = -1;
			if (!(block.Instructions[pos] is CallInstruction call))
				return false;
			if (!MatchInstruction.IsDeconstructMethod(call.Method) || call.Arguments.Count == 0)
				return false;
			var target = call.Arguments[0];
			if (!MatchLdLocOrLdLoca(target, out var v))
				return false;
			// look through the defensive copy of a struct element
			if (v.StoreInstructions is [StLoc copy] && copy.Value.MatchLdLoc(out var copySource))
			{
				v = copySource;
			}
			// StoreCount also counts the initial value of parameters, on purpose
			if (v.StoreCount != 0)
				return false;
			if (!(v.AddressInstructions is [{ Parent: CallInstruction enclosingCall } addressLoad]
				&& addressLoad.ChildIndex > 0
				&& MatchInstruction.IsDeconstructMethod(enclosingCall.Method)))
			{
				return false;
			}
			if (enclosingCall.Parent != block)
				return false;
			enclosingPos = enclosingCall.ChildIndex;
			return enclosingPos >= 0 && enclosingPos < pos;
		}

		/// <summary>
		/// A matched Deconstruct call: one node of the (possibly nested) deconstruction pattern.
		/// </summary>
		sealed class DeconstructionCall
		{
			public IMethod Method = null!;
			/// <summary>Pattern variable of this match node; null for the root (which gets a fresh temp).</summary>
			public ILVariable? Receiver;
			/// <summary>The out-argument variable per element.</summary>
			public ILVariable[] Results = null!;
			/// <summary>Nested deconstruction per element; null = leaf element.</summary>
			public DeconstructionCall?[] NestedCalls = null!;
		}

		/// <summary>
		/// call Deconstruct(target, ldloca x, ldloca inner)      the root call, at pos
		/// [nested Deconstruct calls, see MatchNestedDeconstructions]
		/// On success, the leaf out-variables carry flat indices in depth-first order: this is
		/// the order in which StatementBuilder/ExpressionBuilder pair pattern variables with
		/// assignments, so the index checks in MatchConversions/MatchAssignments work unchanged
		/// for nested patterns.
		/// </summary>
		void MatchDeconstruction(Block block, ref int pos, out DeconstructionCall? rootCall,
			out ILInstruction? testedOperand)
		{
			rootCall = MatchDeconstructionCall(block.Instructions[pos], out testedOperand);
			if (rootCall == null)
				return;
			pos++;
			MatchNestedDeconstructions(block, ref pos, rootCall);
			// Assign flat indices to the leaves in depth-first order: this is the order in which
			// StatementBuilder/ExpressionBuilder pair pattern variables with assignments, so the
			// index checks in MatchConversions/MatchAssignments work unchanged for nested patterns.
			var leaves = new List<ILVariable>();
			CollectLeaves(rootCall, leaves);
			deconstructionResults = leaves.ToArray();
			for (int i = 0; i < deconstructionResults.Length; i++)
			{
				deconstructionResultsLookup.Add(deconstructionResults[i]!, i);
			}

			static void CollectLeaves(DeconstructionCall call, List<ILVariable> leaves)
			{
				for (int i = 0; i < call.Results.Length; i++)
				{
					if (call.NestedCalls[i] is DeconstructionCall nested)
						CollectLeaves(nested, leaves);
					else
						leaves.Add(call.Results[i]);
				}
			}
		}

		/// <summary>
		/// call(virt) Deconstruct(target, ldloca out0, ldloca out1, ...)
		/// where every out-argument is a single-use temporary.
		/// </summary>
		DeconstructionCall? MatchDeconstructionCall(ILInstruction inst, out ILInstruction? testedOperand)
		{
			testedOperand = null;
			if (!(inst is CallInstruction call))
				return null;
			if (!MatchInstruction.IsDeconstructMethod(call.Method))
				return null;
			if (call.Method.IsStatic || call.Method.DeclaringType.IsReferenceType == false)
			{
				if (!(call is Call))
					return null;
			}
			else
			{
				if (!(call is CallVirt))
					return null;
			}
			if (call.Arguments.Count < 3)
				return null;
			var results = new ILVariable[call.Arguments.Count - 1];
			for (int i = 0; i < results.Length; i++)
			{
				if (!call.Arguments[i + 1].MatchLdLoca(out var v))
					return null;
				// TODO v.LoadCount may be 2 if the deconstruction is assigned to a tuple variable
				// or 0? because of discards
				if (!(v.StoreCount == 0 && v.AddressCount == 1 && v.LoadCount <= 1))
					return null;
				results[i] = v;
			}
			testedOperand = call.Arguments[0];
			return new DeconstructionCall {
				Method = call.Method,
				Results = results,
				NestedCalls = new DeconstructionCall[results.Length]
			};
		}

		/// <summary>
		/// Per element of the parent call, in order:
		///   [stloc copy(ldloc result)]                          defensive copy for a struct element
		///   call Deconstruct(ldloc(a) result|copy, ldloca ...)  recursing into its elements
		/// C# evaluates nested Deconstruct calls left-to-right, directly after the parent call,
		/// before any conversions or assignments: the elements are visited depth-first, and the
		/// stack of pending elements takes the place of recursing into a matched nested call.
		/// </summary>
		void MatchNestedDeconstructions(Block block, ref int pos, DeconstructionCall rootCall)
		{
			var pendingElements = new Stack<(DeconstructionCall Call, int ElementIndex)>();
			pendingElements.Push((rootCall, 0));
			while (pendingElements.Count > 0)
			{
				var (parent, i) = pendingElements.Pop();
				if (i + 1 < parent.Results.Length)
					pendingElements.Push((parent, i + 1));
				ILVariable result = parent.Results[i];
				int savedPos = pos;
				ILVariable receiver = result;
				var inst = block.Instructions.ElementAtOrDefault(pos);
				if (inst != null && inst.MatchStLoc(out var copy, out var copiedValue)
					&& copiedValue.MatchLdLoc(result)
					&& copy.StoreCount == 1
					&& copy.LoadCount + copy.AddressCount == 1)
				{
					receiver = copy;
					pos++;
					inst = block.Instructions.ElementAtOrDefault(pos);
				}
				var nested = inst == null ? null : MatchDeconstructionCall(inst, out _);
				if (nested == null || !IsReceiverReference(((CallInstruction)inst!).Arguments[0], receiver))
				{
					pos = savedPos;
					continue;
				}
				if (receiver != result && result.LoadCount != 1)
				{
					// the copy must be the element's only use
					pos = savedPos;
					continue;
				}
				if (!BindsOnElementType(nested.Method, result.Type))
				{
					// A nested designation rebinds Deconstruct on the element's static type
					// when recompiled; if that picks a different method (member hiding), the
					// call must stay explicit, where a cast can preserve the binding.
					pos = savedPos;
					continue;
				}
				pos++;
				nested.Receiver = receiver;
				parent.NestedCalls[i] = nested;
				// its elements are evaluated before the parent's remaining ones
				pendingElements.Push((nested, 0));
			}

			static bool IsReceiverReference(ILInstruction target, ILVariable receiver)
			{
				return MatchLdLocOrLdLoca(target, out var v) && v == receiver;
			}

			static bool BindsOnElementType(IMethod method, IType elementType)
			{
				int outParamCount = method.Parameters.Count - (method.IsStatic ? 1 : 0);
				IType type = elementType;
				while (type != null)
				{
					if (!method.IsStatic && NormalizeTypeVisitor.TypeErasure.EquivalentTypes(type, method.DeclaringType))
						return true;
					if (type.GetMethods(m => m.Name == "Deconstruct", GetMemberOptions.IgnoreInheritedMembers)
						.Any(m => !m.IsStatic && m.Parameters.Count == outParamCount))
					{
						// An instance Deconstruct of the same arity is declared on a type more
						// derived than the called method's declaring type: it hides the called
						// method (and wins over a called extension method).
						return false;
					}
					type = type.DirectBaseTypes.FirstOrDefault(t => t.Kind == TypeKind.Class)!;
				}
				// The chain ended without seeing the declaring type, so an instance method's
				// binding cannot be verified. An extension method is reached by its receiver
				// type, and one declared on a more derived type wins over it; which extensions
				// are in scope where the output is compiled is not known here, so the binding
				// is only certain when the element type is the receiver type itself.
				return method.IsStatic
					&& NormalizeTypeVisitor.TypeErasure.EquivalentTypes(elementType, method.Parameters[0].Type);
			}
		}

		struct ConversionInfo
		{
			public IType? inputType;
			public Conv? conv;
		}

		/// <summary>
		/// stloc conv0(conv(FindIndex-resolvable value))
		/// stloc conv1(conv(...))
		/// ...
		/// The run of single-use conversion temporaries following the deconstruction, in flat
		/// leaf index order.
		/// </summary>
		bool MatchConversions(Block block, ref int pos,
			out Dictionary<ILVariable, ConversionInfo> conversions,
			out List<StLoc> conversionStLocs,
			ref Action<DeconstructInstruction>? delayedActions)
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

		/// <summary>
		/// stloc output(conv(input))
		/// </summary>
		bool MatchConversion(ILInstruction? inst, [NotNullWhen(true)] out ILInstruction? inputInstruction,
			[NotNullWhen(true)] out ILVariable? outputVariable, out ConversionInfo info)
		{
			info = default;
			inputInstruction = null;
			outputVariable = null;
			if (inst == null)
				return false;
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

		/// <summary>
		/// assignment(FindIndex-resolvable value)                see MatchAssignment
		/// ...
		/// The run of assignments following the conversions, in flat leaf index order.
		/// Single-use elements without an assignment are forwarded through a fresh variable
		/// assigned inside the deconstruction.
		/// </summary>
		bool MatchAssignments(Block block, ref int pos,
			Dictionary<ILVariable, ConversionInfo> conversions,
			List<StLoc> conversionStLocs,
			ref Action<DeconstructInstruction>? delayedActions,
			bool allowUnrelatedAssignments,
			out bool anyAssignments)
		{
			anyAssignments = false;
			int previousIndex = -1;
			int conversionStLocIndex = 0;
			int startPos = pos;
			while (MatchAssignment(block.Instructions.ElementAtOrDefault(pos), out var targetType, out var valueInst, out var addAssignment))
			{
				int index = FindIndex(valueInst, out var tupleAccessAdjustment);
				if (index < 0 && allowUnrelatedAssignments)
				{
					// For a Deconstruct call the element list is fixed by the call's
					// out-arguments, so an assignment whose value is unrelated to the
					// deconstruction just ends the pattern and stays after the deconstruct
					// instruction. (For tuples the elements are discovered from the
					// assignments, so ending early would misread a suffix as the pattern:
					// keep rejecting there.)
					break;
				}
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
						assignmentTarget.Type = conversionInfo.inputType!;
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

			if (deconstructionResults != null)
			{
				foreach (var v in deconstructionResults)
				{
					// In optimized code a deconstruction element is not stored to a temporary,
					// if it is used directly (and only once!) after the deconstruction. This
					// happens for trailing elements, but also for leading elements, e.g., when
					// a nested deconstruction copies the inner element to a temporary before
					// the elements preceding it are used. Forward such elements through a fresh
					// variable assigned inside the deconstruction, so that every pattern
					// variable's load is a descendant of the deconstruct instruction.
					// The assignment is inserted in pattern order, because StatementBuilder and
					// ExpressionBuilder pair pattern variables with assignments positionally.
					// LoadCount must be read eagerly, at match time: for a tuple deconstruction
					// the elements are the fresh "E_i" variables created in FindIndex, whose
					// loads only materialize when the delayed ReplaceWith actions run, so
					// LoadCount is still 0 here and forwarding never fires on that path. That
					// is load-bearing, not incidental: the fresh variables are never registered
					// in deconstructionResultsLookup, so GetAssignmentIndex could not position
					// a forwarding assignment among a tuple's assignments.
					if (v?.LoadCount != 1)
						continue;
					delayedActions += (DeconstructInstruction deconstructInst) => {
						var load = v.LoadInstructions[0];
						if (load.IsDescendantOf(deconstructInst))
							return;
						// MatchDeconstruction registered every deconstruction result in the
						// lookup, and the tuple path never gets here (see above); a miss would
						// leave the load outside the deconstruct instruction, i.e. a malformed
						// pattern, because the transform is already committed at this point.
						bool isDeconstructionResult = deconstructionResultsLookup.TryGetValue(v, out int index);
						Debug.Assert(isDeconstructionResult);
						var freshVar = context.Function.RegisterVariable(VariableKind.StackSlot, v.Type);
						var instructions = deconstructInst.Assignments.Instructions;
						int insertPos = 0;
						while (insertPos < instructions.Count && GetAssignmentIndex(instructions[insertPos]) < index)
							insertPos++;
						instructions.Insert(insertPos, new StLoc(freshVar, new LdLoc(v)));
						load.Variable = freshVar;
					};
				}
			}

			anyAssignments = startPos != pos;
			return true;

			int GetAssignmentIndex(ILInstruction inst)
			{
				if (DeconstructInstruction.IsAssignment(inst, context.TypeSystem, out _, out var value)
					&& value.MatchLdLoc(out var inputVariable))
				{
					if (deconstructionResultsLookup.TryGetValue(inputVariable, out int index))
						return index;
					// Forwarding assignments produced for conversions load a fresh variable;
					// their pattern index is that of the conversion output they store to.
					if (inst is StLoc stLoc && deconstructionResultsLookup.TryGetValue(stLoc.Variable, out index))
						return index;
				}
				return int.MaxValue;
			}

			void AddMissingAssignmentsForConversions(int index, ref Action<DeconstructInstruction>? delayedActions)
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
					previousIndex = conversionResultIndex;
					conversionStLocIndex++;
				}
			}
		}

		/// <summary>
		/// stloc v(value) | stobj(target, value) | call set_Property(target, value)
		/// or the result-used form
		///   stloc s(Block CallInlineAssign { call set_Property(target, stloc tmp(value)); final: ldloc tmp })
		/// where the setter call is moved into the assignments block.
		/// </summary>
		bool MatchAssignment(ILInstruction? inst, [NotNullWhen(true)] out IType? targetType, [NotNullWhen(true)] out ILInstruction? valueInst, [NotNullWhen(true)] out Action<DeconstructInstruction>? addAssignment)
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

		/// <summary>
		/// ldloc result                                          a registered result or conversion output
		/// ldobj(ldflda ItemN(ldloc(a) v))                       an element read of the tuple
		/// Resolves the value of a conversion or assignment to its element index.
		/// Returns -1 on failure.
		/// </summary>
		int FindIndex(ILInstruction inst, out Action<DeconstructInstruction>? delayedActions)
		{
			delayedActions = null;
			if (inst.MatchLdLoc(out var v))
			{
				if (!deconstructionResultsLookup.TryGetValue(v, out int index))
					return -1;
				return index;
			}
			if (!MatchTupleElementRead(inst, out var container, out var containerType, out int elementIndex))
				return -1;
			var normalizedType = TupleType.FromUnderlyingType(context.TypeSystem, containerType);
			if (this.tupleVariable == null)
			{
				this.tupleVariable = container;
				this.tupleType = (TupleType)normalizedType;
				this.deconstructionResults = new ILVariable[this.tupleType.Cardinality];
			}
			if (this.tupleType!.Cardinality < 2)
				return -1;
			if (container != tupleVariable || !this.tupleType.Equals(normalizedType))
				return -1;
			if (this.deconstructionResults[elementIndex] == null)
			{
				var freshVar = new ILVariable(VariableKind.StackSlot, this.tupleType.ElementTypes[elementIndex]) { Name = "E_" + elementIndex };
				delayedActions += _ => context.Function.Variables.Add(freshVar);
				this.deconstructionResults[elementIndex] = freshVar;
			}
			delayedActions += _ => {
				inst.ReplaceWith(new LdLoc(this.deconstructionResults[elementIndex]!));
			};
			return elementIndex;
		}

		/// <summary>
		/// Gets whether the matched conv instruction (or its absence) is the lowering of the
		/// implicit conversion from the input type to the assignment's target type.
		/// </summary>
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
			if (c.IsNumericConversion && conv != null)
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

		/// <summary>
		/// Builds, recursing into nested calls:
		/// match.deconstruct[Method] (matchVariable = testedOperand) {
		///     match(result_i = deconstruct.result i(ldloc matchVariable)),
		///     match.deconstruct[...] (receiver_j = deconstruct.result j(ldloc matchVariable)) { ... }
		/// }
		/// </summary>
		MatchInstruction BuildPatternMatch(DeconstructionCall call, ILVariable matchVariable, ILInstruction testedOperand)
		{
			matchVariable.Kind = VariableKind.PatternLocal;
			var match = new MatchInstruction(matchVariable, call.Method, testedOperand) {
				IsDeconstructCall = true
			};
			for (int i = 0; i < call.Results.Length; i++)
			{
				var nested = call.NestedCalls[i];
				if (nested != null)
				{
					var receiver = nested.Receiver!;
					match.SubPatterns.Add(BuildPatternMatch(nested, receiver,
						new DeconstructResultInstruction(i, receiver.StackType, new LdLoc(matchVariable))));
				}
				else
				{
					var result = call.Results[i];
					result.Kind = VariableKind.PatternLocal;
					match.SubPatterns.Add(
						new MatchInstruction(
							result,
							new DeconstructResultInstruction(i, result.StackType, new LdLoc(matchVariable))
						)
					);
				}
			}
			return match;
		}

		/// <summary>
		/// ldobj(ldflda ItemN(ldloc(a) container))
		/// The returned index is zero-based; Rest chains of long tuples are flattened.
		/// Non-escaping element reads may have been rewritten from ldloca to ldloc,
		/// so both load kinds are accepted.
		/// </summary>
		static bool MatchTupleElementRead(ILInstruction inst, [NotNullWhen(true)] out ILVariable? container, [NotNullWhen(true)] out IType? containerType, out int index)
		{
			container = null;
			containerType = null;
			index = -1;
			if (!(inst is LdObj ldobj && ldobj.Target is LdFlda ldflda))
				return false;
			if (ldobj.UnalignedPrefix != 0 || ldobj.IsVolatile)
				return false;
			if (!TupleTransform.MatchTupleFieldAccess(ldflda, out containerType, out var target, out int position))
				return false;
			// Item fields are one-based, we use zero-based indexing.
			index = position - 1;
			return MatchLdLocOrLdLoca(target, out container);
		}

		/// <summary>
		/// ldloc variable | ldloca variable
		/// </summary>
		static bool MatchLdLocOrLdLoca(ILInstruction inst, [NotNullWhen(true)] out ILVariable? variable)
		{
			return inst.MatchLdLoc(out variable) || inst.MatchLdLoca(out variable);
		}
	}
}
