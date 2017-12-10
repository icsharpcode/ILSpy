// Copyright (c) 2014 Daniel Grunwald
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
using System.Collections.Immutable;
using System.Diagnostics;
using Mono.Cecil;
using Cil = Mono.Cecil.Cil;
using System.Collections;
using System.Threading;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;
using ArrayType = ICSharpCode.Decompiler.TypeSystem.ArrayType;
using ByReferenceType = ICSharpCode.Decompiler.TypeSystem.ByReferenceType;

namespace ICSharpCode.Decompiler.IL
{
	public class ILReader
	{
		readonly ICompilation compilation;
		readonly IDecompilerTypeSystem typeSystem;

		public bool UseDebugSymbols { get; set; }
		public List<string> Warnings { get; } = new List<string>();

		public ILReader(IDecompilerTypeSystem typeSystem)
		{
			if (typeSystem == null)
				throw new ArgumentNullException(nameof(typeSystem));
			this.typeSystem = typeSystem;
			this.compilation = typeSystem.Compilation;
		}

		Cil.MethodBody body;
		Cil.MethodDebugInformation debugInfo;
		StackType methodReturnStackType;
		Cil.Instruction currentInstruction;
		int nextInstructionIndex;
		ImmutableStack<ILVariable> currentStack;
		ILVariable[] parameterVariables;
		ILVariable[] localVariables;
		BitArray isBranchTarget;
		BlockContainer mainContainer;
		List<ILInstruction> instructionBuilder;

		// Dictionary that stores stacks for each IL instruction
		Dictionary<int, ImmutableStack<ILVariable>> stackByOffset;
		Dictionary<Cil.ExceptionHandler, ILVariable> variableByExceptionHandler;
		UnionFind<ILVariable> unionFind;
		List<(ILVariable, ILVariable)> stackMismatchPairs;
		IEnumerable<ILVariable> stackVariables;
		
		void Init(Cil.MethodBody body)
		{
			if (body == null)
				throw new ArgumentNullException(nameof(body));
			this.body = body;
			this.debugInfo = body.Method.DebugInformation;
			this.currentInstruction = null;
			this.nextInstructionIndex = 0;
			this.currentStack = ImmutableStack<ILVariable>.Empty;
			this.unionFind = new UnionFind<ILVariable>();
			this.stackMismatchPairs = new List<(ILVariable, ILVariable)>();
			this.methodReturnStackType = typeSystem.Resolve(body.Method.ReturnType).GetStackType();
			InitParameterVariables();
			this.localVariables = body.Variables.SelectArray(CreateILVariable);
			if (body.InitLocals) {
				foreach (var v in localVariables) {
					v.HasInitialValue = true;
				}
			}
			this.mainContainer = new BlockContainer(expectedResultType: methodReturnStackType);
			this.instructionBuilder = new List<ILInstruction>();
			this.isBranchTarget = new BitArray(body.CodeSize);
			this.stackByOffset = new Dictionary<int, ImmutableStack<ILVariable>>();
			this.variableByExceptionHandler = new Dictionary<Cil.ExceptionHandler, ILVariable>();
		}

		IMetadataTokenProvider ReadAndDecodeMetadataToken()
		{
			return (IMetadataTokenProvider)currentInstruction.Operand;
		}

		IType ReadAndDecodeTypeReference()
		{
			var typeReference = (TypeReference)currentInstruction.Operand;
			return typeSystem.Resolve(typeReference);
		}

		IMethod ReadAndDecodeMethodReference()
		{
			var methodReference = (MethodReference)currentInstruction.Operand;
			return typeSystem.Resolve(methodReference);
		}

		IField ReadAndDecodeFieldReference()
		{
			var fieldReference = (FieldReference)currentInstruction.Operand;
			return typeSystem.Resolve(fieldReference);
		}

		void InitParameterVariables()
		{
			parameterVariables = new ILVariable[GetPopCount(OpCode.Call, body.Method)];
			int paramIndex = 0;
			if (body.Method.HasThis)
				parameterVariables[paramIndex++] = CreateILVariable(body.ThisParameter);
			foreach (var p in body.Method.Parameters)
				parameterVariables[paramIndex++] = CreateILVariable(p);
			Debug.Assert(paramIndex == parameterVariables.Length);
		}

		ILVariable CreateILVariable(Cil.VariableDefinition v)
		{
			VariableKind kind = IsPinned(v.VariableType) ? VariableKind.PinnedLocal : VariableKind.Local;
			ILVariable ilVar = new ILVariable(kind, typeSystem.Resolve(v.VariableType), v.Index);
			if (!UseDebugSymbols || debugInfo == null || !debugInfo.TryGetName(v, out string name)) {
				ilVar.Name = "V_" + v.Index;
				ilVar.HasGeneratedName = true;
			} else if (string.IsNullOrWhiteSpace(name)) {
				ilVar.Name = "V_" + v.Index;
				ilVar.HasGeneratedName = true;
			} else {
				ilVar.Name = name;
			}
			return ilVar;
		}

		bool IsPinned(TypeReference type)
		{
			while (type is OptionalModifierType || type is RequiredModifierType) {
				type = ((TypeSpecification)type).ElementType;
			}
			return type is PinnedType;
		}

		ILVariable CreateILVariable(ParameterDefinition p)
		{
			IType parameterType;
			if (p.Index == -1) {
				// Manually construct ctor parameter type due to Cecil bug:
				// https://github.com/jbevain/cecil/issues/275
				ITypeDefinition def = typeSystem.Resolve(body.Method.DeclaringType).GetDefinition();
				if (def != null && def.TypeParameterCount > 0) {
					parameterType = new ParameterizedType(def, def.TypeArguments);
					if (def.IsReferenceType == false) {
						parameterType = new ByReferenceType(parameterType);
					}
				} else {
					parameterType = typeSystem.Resolve(p.ParameterType);
				}
			} else {
				parameterType = typeSystem.Resolve(p.ParameterType);
			}
			Debug.Assert(!parameterType.IsUnbound());
			if (parameterType.IsUnbound()) {
				// parameter types should not be unbound, the only known cause for these is a Cecil bug:
				Debug.Assert(p.Index < 0); // cecil bug occurs only for "this"
				parameterType = new ParameterizedType(parameterType.GetDefinition(), parameterType.TypeArguments);
			}
			var ilVar = new ILVariable(VariableKind.Parameter, parameterType, p.Index);
			Debug.Assert(ilVar.StoreCount == 1); // count the initial store when the method is called with an argument
			if (p.Index < 0)
				ilVar.Name = "this";
			else if (string.IsNullOrEmpty(p.Name))
				ilVar.Name = "P_" + p.Index;
			else
				ilVar.Name = p.Name;
			return ilVar;
		}
		
		/// <summary>
		/// Warn when invalid IL is detected.
		/// ILSpy should be able to handle invalid IL; but this method can be helpful for debugging the ILReader,
		/// as this method should not get called when processing valid IL.
		/// </summary>
		void Warn(string message)
		{
			Warnings.Add(string.Format("IL_{0:x4}: {1}", currentInstruction.Offset, message));
		}

		ImmutableStack<ILVariable> MergeStacks(ImmutableStack<ILVariable> a, ImmutableStack<ILVariable> b)
		{
			if (CheckStackCompatibleWithoutAdjustments(a, b)) {
				// We only need to union the input variables, but can 
				// otherwise re-use the existing stack.
				ImmutableStack<ILVariable> output = a;
				while (!a.IsEmpty && !b.IsEmpty) {
					Debug.Assert(a.Peek().StackType == b.Peek().StackType);
					unionFind.Merge(a.Peek(), b.Peek());
					a = a.Pop();
					b = b.Pop();
				}
				return output;
			} else if (a.Count() != b.Count()) {
				// Let's not try to merge mismatched stacks.
				Warn("Incompatible stack heights: " + a.Count() + " vs " + b.Count());
				return a;
			} else {
				// The more complex case where the stacks don't match exactly.
				var output = new List<ILVariable>();
				while (!a.IsEmpty && !b.IsEmpty) {
					var varA = a.Peek();
					var varB = b.Peek();
					if (varA.StackType == varB.StackType) {
						unionFind.Merge(varA, varB);
						output.Add(varA);
					} else {
						if (!IsValidTypeStackTypeMerge(varA.StackType, varB.StackType)) {
							Warn("Incompatible stack types: " + varA.StackType + " vs " + varB.StackType);
						}
						if (varA.StackType > varB.StackType) {
							output.Add(varA);
							// every store to varB should also store to varA
							stackMismatchPairs.Add((varB, varA));
						} else {
							output.Add(varB);
							// every store to varA should also store to varB
							stackMismatchPairs.Add((varA, varB));
						}
					}
					a = a.Pop();
					b = b.Pop();
				}
				// because we built up output by popping from the input stacks, we need to reverse it to get back the original order
				output.Reverse();
				return ImmutableStack.CreateRange(output);
			}
		}

		static bool CheckStackCompatibleWithoutAdjustments(ImmutableStack<ILVariable> a, ImmutableStack<ILVariable> b)
		{
			while (!a.IsEmpty && !b.IsEmpty) {
				if (a.Peek().StackType != b.Peek().StackType)
					return false;
				a = a.Pop();
				b = b.Pop();
			}
			return a.IsEmpty && b.IsEmpty;
		}

		private bool IsValidTypeStackTypeMerge(StackType stackType1, StackType stackType2)
		{
			if (stackType1 == StackType.I && stackType2 == StackType.I4)
				return true;
			if (stackType1 == StackType.I4 && stackType2 == StackType.I)
				return true;
			if (stackType1 == StackType.F4 && stackType2 == StackType.F8)
				return true;
			if (stackType1 == StackType.F8 && stackType2 == StackType.F4)
				return true;
			// allow merging unknown type with any other type
			return stackType1 == StackType.Unknown || stackType2 == StackType.Unknown;
		}

		/// <summary>
		/// Stores the given stack for a branch to `offset`.
		/// 
		/// The stack may be modified if stack adjustments are necessary. (e.g. implicit I4->I conversion)
		/// </summary>
		void StoreStackForOffset(int offset, ref ImmutableStack<ILVariable> stack)
		{
			if (stackByOffset.TryGetValue(offset, out var existing)) {
				stack = MergeStacks(existing, stack);
				if (stack != existing)
					stackByOffset[offset] = stack;
			} else {
				stackByOffset.Add(offset, stack);
			}
		}
		
		void ReadInstructions(CancellationToken cancellationToken)
		{
			// Fill isBranchTarget and branchStackDict based on exception handlers
			foreach (var eh in body.ExceptionHandlers) {
				ImmutableStack<ILVariable> ehStack = null;
				if (eh.HandlerType == Cil.ExceptionHandlerType.Catch) {
					var v = new ILVariable(VariableKind.Exception, typeSystem.Resolve(eh.CatchType), eh.HandlerStart.Offset) {
						Name = "E_" + eh.HandlerStart.Offset,
						HasGeneratedName = true
					};
					variableByExceptionHandler.Add(eh, v);
					ehStack = ImmutableStack.Create(v);
				} else if (eh.HandlerType == Cil.ExceptionHandlerType.Filter) {
					var v = new ILVariable(VariableKind.Exception, typeSystem.Compilation.FindType(KnownTypeCode.Object), eh.HandlerStart.Offset) {
						Name = "E_" + eh.HandlerStart.Offset,
						HasGeneratedName = true
					};
					variableByExceptionHandler.Add(eh, v);
					ehStack = ImmutableStack.Create(v);
				} else {
					ehStack = ImmutableStack<ILVariable>.Empty;
				}
				if (eh.FilterStart != null) {
					isBranchTarget[eh.FilterStart.Offset] = true;
					StoreStackForOffset(eh.FilterStart.Offset, ref ehStack);
				}
				if (eh.HandlerStart != null) {
					isBranchTarget[eh.HandlerStart.Offset] = true;
					StoreStackForOffset(eh.HandlerStart.Offset, ref ehStack);
				}
			}
			
			while (nextInstructionIndex < body.Instructions.Count) {
				cancellationToken.ThrowIfCancellationRequested();
				int start = body.Instructions[nextInstructionIndex].Offset;
				StoreStackForOffset(start, ref currentStack);
				ILInstruction decodedInstruction = DecodeInstruction();
				if (decodedInstruction.ResultType == StackType.Unknown)
					Warn("Unknown result type (might be due to invalid IL or missing references)");
				decodedInstruction.CheckInvariant(ILPhase.InILReader);
				int end = currentInstruction.GetEndOffset();
				decodedInstruction.ILRange = new Interval(start, end);
				UnpackPush(decodedInstruction).ILRange = decodedInstruction.ILRange;
				instructionBuilder.Add(decodedInstruction);
				if (decodedInstruction.HasDirectFlag(InstructionFlags.EndPointUnreachable)) {
					if (!stackByOffset.TryGetValue(end, out currentStack)) {
						currentStack = ImmutableStack<ILVariable>.Empty;
					}
				}
				Debug.Assert(currentInstruction.Next == null || currentInstruction.Next.Offset == end);
			}
			
			var visitor = new CollectStackVariablesVisitor(unionFind);
			for (int i = 0; i < instructionBuilder.Count; i++) {
				instructionBuilder[i] = instructionBuilder[i].AcceptVisitor(visitor);
			}
			stackVariables = visitor.variables;
			InsertStackAdjustments();
		}

		void InsertStackAdjustments()
		{
			if (stackMismatchPairs.Count == 0)
				return;
			var dict = new MultiDictionary<ILVariable, ILVariable>();
			foreach (var (origA, origB) in stackMismatchPairs) {
				var a = unionFind.Find(origA);
				var b = unionFind.Find(origB);
				Debug.Assert(a.StackType < b.StackType);
				// For every store to a, insert a converting store to b.
				if (!dict[a].Contains(b))
					dict.Add(a, b);
			}
			var newInstructions = new List<ILInstruction>();
			foreach (var inst in instructionBuilder) {
				newInstructions.Add(inst);
				if (inst is StLoc store) {
					foreach (var additionalVar in dict[store.Variable]) {
						ILInstruction value = new LdLoc(store.Variable);
						value = new Conv(value, additionalVar.StackType.ToPrimitiveType(), false, Sign.Signed);
						newInstructions.Add(new StLoc(additionalVar, value) {
							IsStackAdjustment = true,
							ILRange = inst.ILRange
						});
					}
				}
			}
			instructionBuilder = newInstructions;
		}

		/// <summary>
		/// Debugging helper: writes the decoded instruction stream interleaved with the inferred evaluation stack layout.
		/// </summary>
		public void WriteTypedIL(Cil.MethodBody body, ITextOutput output, CancellationToken cancellationToken = default(CancellationToken))
		{
			Init(body);
			ReadInstructions(cancellationToken);
			foreach (var inst in instructionBuilder) {
				if (inst is StLoc stloc && stloc.IsStackAdjustment) {
					output.Write("          ");
					inst.WriteTo(output, new ILAstWritingOptions());
					output.WriteLine();
					continue;
				}
				output.Write("   [");
				bool isFirstElement = true;
				foreach (var element in stackByOffset[inst.ILRange.Start]) {
					if (isFirstElement)
						isFirstElement = false;
					else
						output.Write(", ");
					output.WriteReference(element.Name, element, isLocal: true);
					output.Write(":");
					output.Write(element.StackType);
				}
				output.Write(']');
				output.WriteLine();
				if (isBranchTarget[inst.ILRange.Start])
					output.Write('*');
				else
					output.Write(' ');
				output.WriteDefinition("IL_" + inst.ILRange.Start.ToString("x4"), inst.ILRange.Start);
				output.Write(": ");
				inst.WriteTo(output, new ILAstWritingOptions());
				output.WriteLine();
			}
			new Disassembler.MethodBodyDisassembler(output, cancellationToken) { DetectControlStructure = false }
				.WriteExceptionHandlers(body);
		}

		/// <summary>
		/// Decodes the specified method body and returns an ILFunction.
		/// </summary>
		public ILFunction ReadIL(Cil.MethodBody body, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			Init(body);
			ReadInstructions(cancellationToken);
			var blockBuilder = new BlockBuilder(body, typeSystem, variableByExceptionHandler);
			blockBuilder.CreateBlocks(mainContainer, instructionBuilder, isBranchTarget, cancellationToken);
			var method = typeSystem.Resolve(body.Method);
			var function = new ILFunction(method, body.Method, mainContainer);
			CollectionExtensions.AddRange(function.Variables, parameterVariables);
			CollectionExtensions.AddRange(function.Variables, localVariables);
			CollectionExtensions.AddRange(function.Variables, stackVariables);
			CollectionExtensions.AddRange(function.Variables, variableByExceptionHandler.Values);
			function.AddRef(); // mark the root node
			foreach (var c in function.Descendants.OfType<BlockContainer>()) {
				c.SortBlocks();
			}
			function.Warnings.AddRange(Warnings);
			return function;
		}

		static ILInstruction UnpackPush(ILInstruction inst)
		{
			ILVariable v;
			ILInstruction inner;
			if (inst.MatchStLoc(out v, out inner) && v.Kind == VariableKind.StackSlot)
				return inner;
			else
				return inst;
		}
		
		ILInstruction Neg()
		{
			switch (PeekStackType()) {
				case StackType.I4:
					return Push(new BinaryNumericInstruction(BinaryNumericOperator.Sub, new LdcI4(0), Pop(), checkForOverflow: false, sign: Sign.None));
				case StackType.I:
					return Push(new BinaryNumericInstruction(BinaryNumericOperator.Sub, new Conv(new LdcI4(0), PrimitiveType.I, false, Sign.None), Pop(), checkForOverflow: false, sign: Sign.None));
				case StackType.I8:
					return Push(new BinaryNumericInstruction(BinaryNumericOperator.Sub, new LdcI8(0), Pop(), checkForOverflow: false, sign: Sign.None));
				case StackType.F4:
					return Push(new BinaryNumericInstruction(BinaryNumericOperator.Sub, new LdcF4(0), Pop(), checkForOverflow: false, sign: Sign.None));
				case StackType.F8:
					return Push(new BinaryNumericInstruction(BinaryNumericOperator.Sub, new LdcF8(0), Pop(), checkForOverflow: false, sign: Sign.None));
				default:
					Warn("Unsupported input type for neg.");
					goto case StackType.I4;
			}
		}
		
		ILInstruction DecodeInstruction()
		{
			if (nextInstructionIndex >= body.Instructions.Count)
				return new InvalidBranch("Unexpected end of body");
			var cecilInst = body.Instructions[nextInstructionIndex++];
			currentInstruction = cecilInst;
			switch (cecilInst.OpCode.Code) {
				case Cil.Code.Constrained:
					return DecodeConstrainedCall();
				case Cil.Code.Readonly:
					return DecodeReadonly();
				case Cil.Code.Tail:
					return DecodeTailCall();
				case Cil.Code.Unaligned:
					return DecodeUnaligned();
				case Cil.Code.Volatile:
					return DecodeVolatile();
				case Cil.Code.Add:
					return BinaryNumeric(BinaryNumericOperator.Add);
				case Cil.Code.Add_Ovf:
					return BinaryNumeric(BinaryNumericOperator.Add, true, Sign.Signed);
				case Cil.Code.Add_Ovf_Un:
					return BinaryNumeric(BinaryNumericOperator.Add, true, Sign.Unsigned);
				case Cil.Code.And:
					return BinaryNumeric(BinaryNumericOperator.BitAnd);
				case Cil.Code.Arglist:
					return Push(new Arglist());
				case Cil.Code.Beq:
					return DecodeComparisonBranch(false, ComparisonKind.Equality);
				case Cil.Code.Beq_S:
					return DecodeComparisonBranch(true, ComparisonKind.Equality);
				case Cil.Code.Bge:
					return DecodeComparisonBranch(false, ComparisonKind.GreaterThanOrEqual);
				case Cil.Code.Bge_S:
					return DecodeComparisonBranch(true, ComparisonKind.GreaterThanOrEqual);
				case Cil.Code.Bge_Un:
					return DecodeComparisonBranch(false, ComparisonKind.GreaterThanOrEqual, un: true);
				case Cil.Code.Bge_Un_S:
					return DecodeComparisonBranch(true, ComparisonKind.GreaterThanOrEqual, un: true);
				case Cil.Code.Bgt:
					return DecodeComparisonBranch(false, ComparisonKind.GreaterThan);
				case Cil.Code.Bgt_S:
					return DecodeComparisonBranch(true, ComparisonKind.GreaterThan);
				case Cil.Code.Bgt_Un:
					return DecodeComparisonBranch(false, ComparisonKind.GreaterThan, un: true);
				case Cil.Code.Bgt_Un_S:
					return DecodeComparisonBranch(true, ComparisonKind.GreaterThan, un: true);
				case Cil.Code.Ble:
					return DecodeComparisonBranch(false, ComparisonKind.LessThanOrEqual);
				case Cil.Code.Ble_S:
					return DecodeComparisonBranch(true, ComparisonKind.LessThanOrEqual);
				case Cil.Code.Ble_Un:
					return DecodeComparisonBranch(false, ComparisonKind.LessThanOrEqual, un: true);
				case Cil.Code.Ble_Un_S:
					return DecodeComparisonBranch(true, ComparisonKind.LessThanOrEqual, un: true);
				case Cil.Code.Blt:
					return DecodeComparisonBranch(false, ComparisonKind.LessThan);
				case Cil.Code.Blt_S:
					return DecodeComparisonBranch(true, ComparisonKind.LessThan);
				case Cil.Code.Blt_Un:
					return DecodeComparisonBranch(false, ComparisonKind.LessThan, un: true);
				case Cil.Code.Blt_Un_S:
					return DecodeComparisonBranch(true, ComparisonKind.LessThan, un: true);
				case Cil.Code.Bne_Un:
					return DecodeComparisonBranch(false, ComparisonKind.Inequality, un: true);
				case Cil.Code.Bne_Un_S:
					return DecodeComparisonBranch(true, ComparisonKind.Inequality, un: true);
				case Cil.Code.Br:
					return DecodeUnconditionalBranch(false);
				case Cil.Code.Br_S:
					return DecodeUnconditionalBranch(true);
				case Cil.Code.Break:
					return new DebugBreak();
				case Cil.Code.Brfalse:
					return DecodeConditionalBranch(false, true);
				case Cil.Code.Brfalse_S:
					return DecodeConditionalBranch(true, true);
				case Cil.Code.Brtrue:
					return DecodeConditionalBranch(false, false);
				case Cil.Code.Brtrue_S:
					return DecodeConditionalBranch(true, false);
				case Cil.Code.Call:
					return DecodeCall(OpCode.Call);
				case Cil.Code.Callvirt:
					return DecodeCall(OpCode.CallVirt);
				case Cil.Code.Calli:
					return DecodeCallIndirect();
				case Cil.Code.Ceq:
					return Push(Comparison(ComparisonKind.Equality));
				case Cil.Code.Cgt:
					return Push(Comparison(ComparisonKind.GreaterThan));
				case Cil.Code.Cgt_Un:
					return Push(Comparison(ComparisonKind.GreaterThan, un: true));
				case Cil.Code.Clt:
					return Push(Comparison(ComparisonKind.LessThan));
				case Cil.Code.Clt_Un:
					return Push(Comparison(ComparisonKind.LessThan, un: true));
				case Cil.Code.Ckfinite:
					return new Ckfinite(Peek());
				case Cil.Code.Conv_I1:
					return Push(new Conv(Pop(), PrimitiveType.I1, false, Sign.None));
				case Cil.Code.Conv_I2:
					return Push(new Conv(Pop(), PrimitiveType.I2, false, Sign.None));
				case Cil.Code.Conv_I4:
					return Push(new Conv(Pop(), PrimitiveType.I4, false, Sign.None));
				case Cil.Code.Conv_I8:
					return Push(new Conv(Pop(), PrimitiveType.I8, false, Sign.None));
				case Cil.Code.Conv_R4:
					return Push(new Conv(Pop(), PrimitiveType.R4, false, Sign.Signed));
				case Cil.Code.Conv_R8:
					return Push(new Conv(Pop(), PrimitiveType.R8, false, Sign.Signed));
				case Cil.Code.Conv_U1:
					return Push(new Conv(Pop(), PrimitiveType.U1, false, Sign.None));
				case Cil.Code.Conv_U2:
					return Push(new Conv(Pop(), PrimitiveType.U2, false, Sign.None));
				case Cil.Code.Conv_U4:
					return Push(new Conv(Pop(), PrimitiveType.U4, false, Sign.None));
				case Cil.Code.Conv_U8:
					return Push(new Conv(Pop(), PrimitiveType.U8, false, Sign.None));
				case Cil.Code.Conv_I:
					return Push(new Conv(Pop(), PrimitiveType.I, false, Sign.None));
				case Cil.Code.Conv_U:
					return Push(new Conv(Pop(), PrimitiveType.U, false, Sign.None));
				case Cil.Code.Conv_R_Un:
					return Push(new Conv(Pop(), PrimitiveType.R8, false, Sign.Unsigned));
				case Cil.Code.Conv_Ovf_I1:
					return Push(new Conv(Pop(), PrimitiveType.I1, true, Sign.Signed));
				case Cil.Code.Conv_Ovf_I2:
					return Push(new Conv(Pop(), PrimitiveType.I2, true, Sign.Signed));
				case Cil.Code.Conv_Ovf_I4:
					return Push(new Conv(Pop(), PrimitiveType.I4, true, Sign.Signed));
				case Cil.Code.Conv_Ovf_I8:
					return Push(new Conv(Pop(), PrimitiveType.I8, true, Sign.Signed));
				case Cil.Code.Conv_Ovf_U1:
					return Push(new Conv(Pop(), PrimitiveType.U1, true, Sign.Signed));
				case Cil.Code.Conv_Ovf_U2:
					return Push(new Conv(Pop(), PrimitiveType.U2, true, Sign.Signed));
				case Cil.Code.Conv_Ovf_U4:
					return Push(new Conv(Pop(), PrimitiveType.U4, true, Sign.Signed));
				case Cil.Code.Conv_Ovf_U8:
					return Push(new Conv(Pop(), PrimitiveType.U8, true, Sign.Signed));
				case Cil.Code.Conv_Ovf_I:
					return Push(new Conv(Pop(), PrimitiveType.I, true, Sign.Signed));
				case Cil.Code.Conv_Ovf_U:
					return Push(new Conv(Pop(), PrimitiveType.U, true, Sign.Signed));
				case Cil.Code.Conv_Ovf_I1_Un:
					return Push(new Conv(Pop(), PrimitiveType.I1, true, Sign.Unsigned));
				case Cil.Code.Conv_Ovf_I2_Un:
					return Push(new Conv(Pop(), PrimitiveType.I2, true, Sign.Unsigned));
				case Cil.Code.Conv_Ovf_I4_Un:
					return Push(new Conv(Pop(), PrimitiveType.I4, true, Sign.Unsigned));
				case Cil.Code.Conv_Ovf_I8_Un:
					return Push(new Conv(Pop(), PrimitiveType.I8, true, Sign.Unsigned));
				case Cil.Code.Conv_Ovf_U1_Un:
					return Push(new Conv(Pop(), PrimitiveType.U1, true, Sign.Unsigned));
				case Cil.Code.Conv_Ovf_U2_Un:
					return Push(new Conv(Pop(), PrimitiveType.U2, true, Sign.Unsigned));
				case Cil.Code.Conv_Ovf_U4_Un:
					return Push(new Conv(Pop(), PrimitiveType.U4, true, Sign.Unsigned));
				case Cil.Code.Conv_Ovf_U8_Un:
					return Push(new Conv(Pop(), PrimitiveType.U8, true, Sign.Unsigned));
				case Cil.Code.Conv_Ovf_I_Un:
					return Push(new Conv(Pop(), PrimitiveType.I, true, Sign.Unsigned));
				case Cil.Code.Conv_Ovf_U_Un:
					return Push(new Conv(Pop(), PrimitiveType.U, true, Sign.Unsigned));
				case Cil.Code.Cpblk:
					return new Cpblk(size: Pop(StackType.I4), sourceAddress: PopPointer(), destAddress: PopPointer());
				case Cil.Code.Div:
					return BinaryNumeric(BinaryNumericOperator.Div, false, Sign.Signed);
				case Cil.Code.Div_Un:
					return BinaryNumeric(BinaryNumericOperator.Div, false, Sign.Unsigned);
				case Cil.Code.Dup:
					return Push(Peek());
				case Cil.Code.Endfilter:
					return new Leave(null, Pop());
				case Cil.Code.Endfinally:
					return new Leave(null);
				case Cil.Code.Initblk:
					return new Initblk(size: Pop(StackType.I4), value: Pop(StackType.I4), address: PopPointer());
				case Cil.Code.Jmp:
					return DecodeJmp();
				case Cil.Code.Ldarg:
				case Cil.Code.Ldarg_S:
					return Push(Ldarg(((ParameterDefinition)cecilInst.Operand).Sequence));
				case Cil.Code.Ldarg_0:
					return Push(Ldarg(0));
				case Cil.Code.Ldarg_1:
					return Push(Ldarg(1));
				case Cil.Code.Ldarg_2:
					return Push(Ldarg(2));
				case Cil.Code.Ldarg_3:
					return Push(Ldarg(3));
				case Cil.Code.Ldarga:
				case Cil.Code.Ldarga_S:
					return Push(Ldarga(((ParameterDefinition)cecilInst.Operand).Sequence));
				case Cil.Code.Ldc_I4:
					return Push(new LdcI4((int)cecilInst.Operand));
				case Cil.Code.Ldc_I8:
					return Push(new LdcI8((long)cecilInst.Operand));
				case Cil.Code.Ldc_R4:
					return Push(new LdcF4((float)cecilInst.Operand));
				case Cil.Code.Ldc_R8:
					return Push(new LdcF8((double)cecilInst.Operand));
				case Cil.Code.Ldc_I4_M1:
					return Push(new LdcI4(-1));
				case Cil.Code.Ldc_I4_0:
					return Push(new LdcI4(0));
				case Cil.Code.Ldc_I4_1:
					return Push(new LdcI4(1));
				case Cil.Code.Ldc_I4_2:
					return Push(new LdcI4(2));
				case Cil.Code.Ldc_I4_3:
					return Push(new LdcI4(3));
				case Cil.Code.Ldc_I4_4:
					return Push(new LdcI4(4));
				case Cil.Code.Ldc_I4_5:
					return Push(new LdcI4(5));
				case Cil.Code.Ldc_I4_6:
					return Push(new LdcI4(6));
				case Cil.Code.Ldc_I4_7:
					return Push(new LdcI4(7));
				case Cil.Code.Ldc_I4_8:
					return Push(new LdcI4(8));
				case Cil.Code.Ldc_I4_S:
					return Push(new LdcI4((sbyte)cecilInst.Operand));
				case Cil.Code.Ldnull:
					return Push(new LdNull());
				case Cil.Code.Ldstr:
					return Push(DecodeLdstr());
				case Cil.Code.Ldftn:
					return Push(new LdFtn(ReadAndDecodeMethodReference()));
				case Cil.Code.Ldind_I1:
					return Push(new LdObj(PopPointer(), compilation.FindType(KnownTypeCode.SByte)));
				case Cil.Code.Ldind_I2:
					return Push(new LdObj(PopPointer(), compilation.FindType(KnownTypeCode.Int16)));
				case Cil.Code.Ldind_I4:
					return Push(new LdObj(PopPointer(), compilation.FindType(KnownTypeCode.Int32)));
				case Cil.Code.Ldind_I8:
					return Push(new LdObj(PopPointer(), compilation.FindType(KnownTypeCode.Int64)));
				case Cil.Code.Ldind_U1:
					return Push(new LdObj(PopPointer(), compilation.FindType(KnownTypeCode.Byte)));
				case Cil.Code.Ldind_U2:
					return Push(new LdObj(PopPointer(), compilation.FindType(KnownTypeCode.UInt16)));
				case Cil.Code.Ldind_U4:
					return Push(new LdObj(PopPointer(), compilation.FindType(KnownTypeCode.UInt32)));
				case Cil.Code.Ldind_R4:
					return Push(new LdObj(PopPointer(), compilation.FindType(KnownTypeCode.Single)));
				case Cil.Code.Ldind_R8:
					return Push(new LdObj(PopPointer(), compilation.FindType(KnownTypeCode.Double)));
				case Cil.Code.Ldind_I:
					return Push(new LdObj(PopPointer(), compilation.FindType(KnownTypeCode.IntPtr)));
				case Cil.Code.Ldind_Ref:
					return Push(new LdObj(PopPointer(), compilation.FindType(KnownTypeCode.Object)));
				case Cil.Code.Ldloc:
				case Cil.Code.Ldloc_S:
					return Push(Ldloc(((Cil.VariableDefinition)cecilInst.Operand).Index));
				case Cil.Code.Ldloc_0:
					return Push(Ldloc(0));
				case Cil.Code.Ldloc_1:
					return Push(Ldloc(1));
				case Cil.Code.Ldloc_2:
					return Push(Ldloc(2));
				case Cil.Code.Ldloc_3:
					return Push(Ldloc(3));
				case Cil.Code.Ldloca:
				case Cil.Code.Ldloca_S:
					return Push(Ldloca(((Cil.VariableDefinition)cecilInst.Operand).Index));
				case Cil.Code.Leave:
					return DecodeUnconditionalBranch(false, isLeave: true);
				case Cil.Code.Leave_S:
					return DecodeUnconditionalBranch(true, isLeave: true);
				case Cil.Code.Localloc:
					return Push(new LocAlloc(Pop()));
				case Cil.Code.Mul:
					return BinaryNumeric(BinaryNumericOperator.Mul, false, Sign.None);
				case Cil.Code.Mul_Ovf:
					return BinaryNumeric(BinaryNumericOperator.Mul, true, Sign.Signed);
				case Cil.Code.Mul_Ovf_Un:
					return BinaryNumeric(BinaryNumericOperator.Mul, true, Sign.Unsigned);
				case Cil.Code.Neg:
					return Neg();
				case Cil.Code.Newobj:
					return DecodeCall(OpCode.NewObj);
				case Cil.Code.Nop:
					return new Nop();
				case Cil.Code.Not:
					return Push(new BitNot(Pop()));
				case Cil.Code.Or:
					return BinaryNumeric(BinaryNumericOperator.BitOr);
				case Cil.Code.Pop:
					Pop();
					return new Nop() { Kind = NopKind.Pop };
				case Cil.Code.Rem:
					return BinaryNumeric(BinaryNumericOperator.Rem, false, Sign.Signed);
				case Cil.Code.Rem_Un:
					return BinaryNumeric(BinaryNumericOperator.Rem, false, Sign.Unsigned);
				case Cil.Code.Ret:
					return Return();
				case Cil.Code.Shl:
					return BinaryNumeric(BinaryNumericOperator.ShiftLeft, false, Sign.None);
				case Cil.Code.Shr:
					return BinaryNumeric(BinaryNumericOperator.ShiftRight, false, Sign.Signed);
				case Cil.Code.Shr_Un:
					return BinaryNumeric(BinaryNumericOperator.ShiftRight, false, Sign.Unsigned);
				case Cil.Code.Starg:
				case Cil.Code.Starg_S:
					return Starg(((ParameterDefinition)cecilInst.Operand).Sequence);
				case Cil.Code.Stind_I1:
					return new StObj(value: Pop(StackType.I4), target: PopPointer(), type: compilation.FindType(KnownTypeCode.SByte));
				case Cil.Code.Stind_I2:
					return new StObj(value: Pop(StackType.I4), target: PopPointer(), type: compilation.FindType(KnownTypeCode.Int16));
				case Cil.Code.Stind_I4:
					return new StObj(value: Pop(StackType.I4), target: PopPointer(), type: compilation.FindType(KnownTypeCode.Int32));
				case Cil.Code.Stind_I8:
					return new StObj(value: Pop(StackType.I8), target: PopPointer(), type: compilation.FindType(KnownTypeCode.Int64));
				case Cil.Code.Stind_R4:
					return new StObj(value: Pop(StackType.F4), target: PopPointer(), type: compilation.FindType(KnownTypeCode.Single));
				case Cil.Code.Stind_R8:
					return new StObj(value: Pop(StackType.F8), target: PopPointer(), type: compilation.FindType(KnownTypeCode.Double));
				case Cil.Code.Stind_I:
					return new StObj(value: Pop(StackType.I), target: PopPointer(), type: compilation.FindType(KnownTypeCode.IntPtr));
				case Cil.Code.Stind_Ref:
					return new StObj(value: Pop(StackType.O), target: PopPointer(), type: compilation.FindType(KnownTypeCode.Object));
				case Cil.Code.Stloc:
				case Cil.Code.Stloc_S:
					return Stloc(((Cil.VariableDefinition)cecilInst.Operand).Index);
				case Cil.Code.Stloc_0:
					return Stloc(0);
				case Cil.Code.Stloc_1:
					return Stloc(1);
				case Cil.Code.Stloc_2:
					return Stloc(2);
				case Cil.Code.Stloc_3:
					return Stloc(3);
				case Cil.Code.Sub:
					return BinaryNumeric(BinaryNumericOperator.Sub, false, Sign.None);
				case Cil.Code.Sub_Ovf:
					return BinaryNumeric(BinaryNumericOperator.Sub, true, Sign.Signed);
				case Cil.Code.Sub_Ovf_Un:
					return BinaryNumeric(BinaryNumericOperator.Sub, true, Sign.Unsigned);
				case Cil.Code.Switch:
					return DecodeSwitch();
				case Cil.Code.Xor:
					return BinaryNumeric(BinaryNumericOperator.BitXor);
				case Cil.Code.Box:
					{
						var type = ReadAndDecodeTypeReference();
						return Push(new Box(Pop(type.GetStackType()), type));
					}
				case Cil.Code.Castclass:
					return Push(new CastClass(Pop(StackType.O), ReadAndDecodeTypeReference()));
				case Cil.Code.Cpobj:
					{
						var type = ReadAndDecodeTypeReference();
						var ld = new LdObj(PopPointer(), type);
						return new StObj(PopPointer(), ld, type);
					}
				case Cil.Code.Initobj:
					return InitObj(PopPointer(), ReadAndDecodeTypeReference());
				case Cil.Code.Isinst:
					return Push(new IsInst(Pop(StackType.O), ReadAndDecodeTypeReference()));
				case Cil.Code.Ldelem_Any:
					return LdElem(ReadAndDecodeTypeReference());
				case Cil.Code.Ldelem_I1:
					return LdElem(compilation.FindType(KnownTypeCode.SByte));
				case Cil.Code.Ldelem_I2:
					return LdElem(compilation.FindType(KnownTypeCode.Int16));
				case Cil.Code.Ldelem_I4:
					return LdElem(compilation.FindType(KnownTypeCode.Int32));
				case Cil.Code.Ldelem_I8:
					return LdElem(compilation.FindType(KnownTypeCode.Int64));
				case Cil.Code.Ldelem_U1:
					return LdElem(compilation.FindType(KnownTypeCode.Byte));
				case Cil.Code.Ldelem_U2:
					return LdElem(compilation.FindType(KnownTypeCode.UInt16));
				case Cil.Code.Ldelem_U4:
					return LdElem(compilation.FindType(KnownTypeCode.UInt32));
				case Cil.Code.Ldelem_R4:
					return LdElem(compilation.FindType(KnownTypeCode.Single));
				case Cil.Code.Ldelem_R8:
					return LdElem(compilation.FindType(KnownTypeCode.Double));
				case Cil.Code.Ldelem_I:
					return LdElem(compilation.FindType(KnownTypeCode.IntPtr));
				case Cil.Code.Ldelem_Ref:
					return LdElem(compilation.FindType(KnownTypeCode.Object));
				case Cil.Code.Ldelema:
					return Push(new LdElema(indices: Pop(), array: Pop(), type: ReadAndDecodeTypeReference()));
				case Cil.Code.Ldfld:
					{
						var field = ReadAndDecodeFieldReference();
						return Push(new LdObj(new LdFlda(PopLdFldTarget(field), field) { DelayExceptions = true }, field.Type));
					}
				case Cil.Code.Ldflda:
					{
						var field = ReadAndDecodeFieldReference();
						return Push(new LdFlda(PopFieldTarget(field), field));
					}
				case Cil.Code.Stfld:
					{
						var field = ReadAndDecodeFieldReference();
						return new StObj(value: Pop(field.Type.GetStackType()), target: new LdFlda(PopFieldTarget(field), field) { DelayExceptions = true }, type: field.Type);
					}
				case Cil.Code.Ldlen:
					return Push(new LdLen(StackType.I, Pop()));
				case Cil.Code.Ldobj:
					return Push(new LdObj(PopPointer(), ReadAndDecodeTypeReference()));
				case Cil.Code.Ldsfld:
					{
						var field = ReadAndDecodeFieldReference();
						return Push(new LdObj(new LdsFlda(field), field.Type));
					}
				case Cil.Code.Ldsflda:
					return Push(new LdsFlda(ReadAndDecodeFieldReference()));
				case Cil.Code.Stsfld:
					{
						var field = ReadAndDecodeFieldReference();
						return new StObj(value: Pop(field.Type.GetStackType()), target: new LdsFlda(field), type: field.Type);
					}
				case Cil.Code.Ldtoken:
					return Push(LdToken(ReadAndDecodeMetadataToken()));
				case Cil.Code.Ldvirtftn:
					return Push(new LdVirtFtn(Pop(), ReadAndDecodeMethodReference()));
				case Cil.Code.Mkrefany:
					return Push(new MakeRefAny(PopPointer(), ReadAndDecodeTypeReference()));
				case Cil.Code.Newarr:
					return Push(new NewArr(ReadAndDecodeTypeReference(), Pop()));
				case Cil.Code.Refanytype:
					return Push(new RefAnyType(Pop()));
				case Cil.Code.Refanyval:
					return Push(new RefAnyValue(Pop(), ReadAndDecodeTypeReference()));
				case Cil.Code.Rethrow:
					return new Rethrow();
				case Cil.Code.Sizeof:
					return Push(new SizeOf(ReadAndDecodeTypeReference()));
				case Cil.Code.Stelem_Any:
					return StElem(ReadAndDecodeTypeReference());
				case Cil.Code.Stelem_I1:
					return StElem(compilation.FindType(KnownTypeCode.SByte));
				case Cil.Code.Stelem_I2:
					return StElem(compilation.FindType(KnownTypeCode.Int16));
				case Cil.Code.Stelem_I4:
					return StElem(compilation.FindType(KnownTypeCode.Int32));
				case Cil.Code.Stelem_I8:
					return StElem(compilation.FindType(KnownTypeCode.Int64));
				case Cil.Code.Stelem_R4:
					return StElem(compilation.FindType(KnownTypeCode.Single));
				case Cil.Code.Stelem_R8:
					return StElem(compilation.FindType(KnownTypeCode.Double));
				case Cil.Code.Stelem_I:
					return StElem(compilation.FindType(KnownTypeCode.IntPtr));
				case Cil.Code.Stelem_Ref:
					return StElem(compilation.FindType(KnownTypeCode.Object));
				case Cil.Code.Stobj:
				{
					var type = ReadAndDecodeTypeReference();
					return new StObj(value: Pop(type.GetStackType()), target: PopPointer(), type: type);
				}
				case Cil.Code.Throw:
					return new Throw(Pop());
				case Cil.Code.Unbox:
					return Push(new Unbox(Pop(), ReadAndDecodeTypeReference()));
				case Cil.Code.Unbox_Any:
					return Push(new UnboxAny(Pop(), ReadAndDecodeTypeReference()));
				default:
					return new InvalidBranch("Unknown opcode: " + cecilInst.OpCode.ToString());
			}
		}
		
		StackType PeekStackType()
		{
			if (currentStack.IsEmpty)
				return StackType.Unknown;
			else
				return currentStack.Peek().StackType;
		}

		class CollectStackVariablesVisitor : ILVisitor<ILInstruction>
		{
			readonly UnionFind<ILVariable> unionFind;
			internal readonly HashSet<ILVariable> variables = new HashSet<ILVariable>();

			public CollectStackVariablesVisitor(UnionFind<ILVariable> unionFind)
			{
				Debug.Assert(unionFind != null);
				this.unionFind = unionFind;
			}

			protected override ILInstruction Default(ILInstruction inst)
			{
				foreach (var child in inst.Children) {
					var newChild = child.AcceptVisitor(this);
					if (newChild != child)
						child.ReplaceWith(newChild);
				}
				return inst;
			}

			protected internal override ILInstruction VisitLdLoc(LdLoc inst)
			{
				base.VisitLdLoc(inst);
				if (inst.Variable.Kind == VariableKind.StackSlot) {
					var variable = unionFind.Find(inst.Variable);
					if (variables.Add(variable))
						variable.Name = "S_" + (variables.Count - 1);
					return new LdLoc(variable) { ILRange = inst.ILRange };
				}
				return inst;
			}

			protected internal override ILInstruction VisitStLoc(StLoc inst)
			{
				base.VisitStLoc(inst);
				if (inst.Variable.Kind == VariableKind.StackSlot) {
					var variable = unionFind.Find(inst.Variable);
					if (variables.Add(variable))
						variable.Name = "S_" + (variables.Count - 1);
					return new StLoc(variable, inst.Value) { ILRange = inst.ILRange };
				}
				return inst;
			}
		}
		
		ILInstruction Push(ILInstruction inst)
		{
			Debug.Assert(inst.ResultType != StackType.Void);
			IType type = compilation.FindType(inst.ResultType.ToKnownTypeCode());
			var v = new ILVariable(VariableKind.StackSlot, type, inst.ResultType, inst.ILRange.Start);
			v.Name = "S_" + inst.ILRange.Start.ToString("x4");
			v.HasGeneratedName = true;
			currentStack = currentStack.Push(v);
			return new StLoc(v, inst);
		}
		
		Interval GetCurrentInstructionInterval()
		{
			return new Interval(currentInstruction.Offset, currentInstruction.GetEndOffset());
		}
		
		ILInstruction Peek()
		{
			if (currentStack.IsEmpty) {
				return new InvalidExpression("Stack underflow") { ILRange = GetCurrentInstructionInterval() };
			}
			return new LdLoc(currentStack.Peek());
		}
		
		ILInstruction Pop()
		{
			if (currentStack.IsEmpty) {
				return new InvalidExpression("Stack underflow") { ILRange = GetCurrentInstructionInterval() };
			}
			ILVariable v;
			currentStack = currentStack.Pop(out v);
			return new LdLoc(v);
		}

		ILInstruction Pop(StackType expectedType)
		{
			ILInstruction inst = Pop();
			if (expectedType != inst.ResultType) {
				if (inst is InvalidExpression) {
					((InvalidExpression)inst).ExpectedResultType = expectedType;
				} else if (expectedType == StackType.I && inst.ResultType == StackType.I4) {
					// IL allows implicit I4->I conversions
					inst = new Conv(inst, PrimitiveType.I, false, Sign.None);
				} else if (expectedType == StackType.I4 && inst.ResultType == StackType.I) {
					// C++/CLI also sometimes implicitly converts in the other direction:
					inst = new Conv(inst, PrimitiveType.I4, false, Sign.None);
				} else if (expectedType == StackType.Unknown) {
					inst = new Conv(inst, PrimitiveType.Unknown, false, Sign.None);
				} else if (inst.ResultType == StackType.Ref) {
					// Implicitly stop GC tracking; this occurs when passing the result of 'ldloca' or 'ldsflda'
					// to a method expecting a native pointer.
					inst = new Conv(inst, PrimitiveType.I, false, Sign.None);
					switch (expectedType) {
						case StackType.I4:
							inst = new Conv(inst, PrimitiveType.I4, false, Sign.None);
							break;
						case StackType.I:
							break;
						case StackType.I8:
							inst = new Conv(inst, PrimitiveType.I8, false, Sign.None);
							break;
						default:
							Warn($"Expected {expectedType}, but got {StackType.Ref}");
							inst = new Conv(inst, expectedType.ToPrimitiveType(), false, Sign.None);
							break;
					}
				} else if (expectedType == StackType.Ref) {
					// implicitly start GC tracking / object to interior
					if (!inst.ResultType.IsIntegerType() && inst.ResultType != StackType.O) {
						// We also handle the invalid to-ref cases here because the else case
						// below uses expectedType.ToKnownTypeCode(), which doesn't work for Ref.
						Warn($"Expected {expectedType}, but got {inst.ResultType}");
					}
					inst = new Conv(inst, PrimitiveType.Ref, false, Sign.None);
				} else if (expectedType == StackType.F8 && inst.ResultType == StackType.F4) {
					// IL allows implicit F4->F8 conversions, because in IL F4 and F8 are the same.
					inst = new Conv(inst, PrimitiveType.R8, false, Sign.Signed);
				} else if (expectedType == StackType.F4 && inst.ResultType == StackType.F8) {
					// IL allows implicit F8->F4 conversions, because in IL F4 and F8 are the same.
					inst = new Conv(inst, PrimitiveType.R4, false, Sign.Signed);
				} else {
					Warn($"Expected {expectedType}, but got {inst.ResultType}");
					inst = new Conv(inst, expectedType.ToPrimitiveType(), false, Sign.Signed);
				}
			}
			return inst;
		}
		
		ILInstruction PopPointer()
		{
			ILInstruction inst = Pop();
			switch (inst.ResultType) {
				case StackType.I4:
					return new Conv(inst, PrimitiveType.I, false, Sign.None);
				case StackType.I:
				case StackType.Ref:
				case StackType.Unknown:
					return inst;
				default:
					Warn("Expected native int or pointer, but got " + inst.ResultType);
					return inst;
			}
		}

		ILInstruction PopFieldTarget(IField field)
		{
			return field.DeclaringType.IsReferenceType == true ? Pop(StackType.O) : PopPointer();
		}

		ILInstruction PopLdFldTarget(IField field)
		{
			if (field.DeclaringType.IsReferenceType == true)
				return Pop(StackType.O);

			return PeekStackType() == StackType.O ? new AddressOf(Pop()) : PopPointer();
		}

		private ILInstruction Return()
		{
			if (methodReturnStackType == StackType.Void)
				return new IL.Leave(mainContainer);
			else
				return new IL.Leave(mainContainer, Pop(methodReturnStackType));
		}

		private ILInstruction DecodeLdstr()
		{
			return new LdStr((string)currentInstruction.Operand);
		}

		private ILInstruction Ldarg(int v)
		{
			return new LdLoc(parameterVariables[v]);
		}

		private ILInstruction Ldarga(int v)
		{
			return new LdLoca(parameterVariables[v]);
		}

		private ILInstruction Starg(int v)
		{
			return new StLoc(parameterVariables[v], Pop(parameterVariables[v].StackType));
		}

		private ILInstruction Ldloc(int v)
		{
			return new LdLoc(localVariables[v]);
		}

		private ILInstruction Ldloca(int v)
		{
			return new LdLoca(localVariables[v]);
		}

		private ILInstruction Stloc(int v)
		{
			return new StLoc(localVariables[v], Pop(localVariables[v].StackType));
		}
		
		private ILInstruction LdElem(IType type)
		{
			return Push(new LdObj(new LdElema(indices: Pop(), array: Pop(), type: type) { DelayExceptions = true }, type));
		}
		
		private ILInstruction StElem(IType type)
		{
			var value = Pop(type.GetStackType());
			var index = Pop();
			var array = Pop();
			return new StObj(new LdElema(type, array, index) { DelayExceptions = true }, value, type);
		}

		ILInstruction InitObj(ILInstruction target, IType type)
		{
			var value = new DefaultValue(type);
			value.ILStackWasEmpty = currentStack.IsEmpty;
			return new StObj(target, value, type);
		}

		IType constrainedPrefix;

		private ILInstruction DecodeConstrainedCall()
		{
			constrainedPrefix = ReadAndDecodeTypeReference();
			var inst = DecodeInstruction();
			var call = UnpackPush(inst) as CallInstruction;
			if (call != null)
				Debug.Assert(call.ConstrainedTo == constrainedPrefix);
			else
				Warn("Ignored invalid 'constrained' prefix");
			constrainedPrefix = null;
			return inst;
		}

		private ILInstruction DecodeTailCall()
		{
			var inst = DecodeInstruction();
			var call = UnpackPush(inst) as CallInstruction;
			if (call != null)
				call.IsTail = true;
			else
				Warn("Ignored invalid 'tail' prefix");
			return inst;
		}

		private ILInstruction DecodeUnaligned()
		{
			byte alignment = (byte)currentInstruction.Operand;
			var inst = DecodeInstruction();
			var sup = UnpackPush(inst) as ISupportsUnalignedPrefix;
			if (sup != null)
				sup.UnalignedPrefix = alignment;
			else
				Warn("Ignored invalid 'unaligned' prefix");
			return inst;
		}

		private ILInstruction DecodeVolatile()
		{
			var inst = DecodeInstruction();
			var svp = UnpackPush(inst) as ISupportsVolatilePrefix;
			if (svp != null)
				svp.IsVolatile = true;
			else
				Warn("Ignored invalid 'volatile' prefix");
			return inst;
		}

		private ILInstruction DecodeReadonly()
		{
			var inst = DecodeInstruction();
			var ldelema = UnpackPush(inst) as LdElema;
			if (ldelema != null)
				ldelema.IsReadOnly = true;
			else
				Warn("Ignored invalid 'readonly' prefix");
			return inst;
		}

		ILInstruction DecodeCall(OpCode opCode)
		{
			var method = ReadAndDecodeMethodReference();
			int firstArgument = (opCode != OpCode.NewObj && !method.IsStatic) ? 1 : 0;
			var arguments = new ILInstruction[firstArgument + method.Parameters.Count];
			for (int i = method.Parameters.Count - 1; i >= 0; i--) {
				arguments[firstArgument + i] = Pop(method.Parameters[i].Type.GetStackType());
			}
			if (firstArgument == 1) {
				arguments[0] = Pop(CallInstruction.ExpectedTypeForThisPointer(constrainedPrefix ?? method.DeclaringType));
			}
			switch (method.DeclaringType.Kind) {
				case TypeKind.Array:
					var elementType = ((ArrayType)method.DeclaringType).ElementType;
					if (opCode == OpCode.NewObj)
						return Push(new NewArr(elementType, arguments));
					if (method.Name == "Set") {
						var target = arguments[0];
						var value = arguments.Last();
						var indices = arguments.Skip(1).Take(arguments.Length - 2).ToArray();
						return new StObj(new LdElema(elementType, target, indices), value, elementType);
					}
					if (method.Name == "Get") {
						var target = arguments[0];
						var indices = arguments.Skip(1).ToArray();
						return Push(new LdObj(new LdElema(elementType, target, indices), elementType));
					}
					if (method.Name == "Address") {
						var target = arguments[0];
						var indices = arguments.Skip(1).ToArray();
						return Push(new LdElema(elementType, target, indices));
					}
					Warn("Unknown method called on array type: " + method.Name);
					goto default;
				default:
					var call = CallInstruction.Create(opCode, method);
					call.ILStackWasEmpty = currentStack.IsEmpty;
					call.ConstrainedTo = constrainedPrefix;
					call.Arguments.AddRange(arguments);
					if (call.ResultType != StackType.Void)
						return Push(call);
					return call;
			}
		}

		ILInstruction DecodeCallIndirect()
		{
			var functionPointer = Pop(StackType.I);
			var signature = (CallSite)currentInstruction.Operand;
			Debug.Assert(!signature.HasThis);
			var parameterTypes = new IType[signature.Parameters.Count];
			var arguments = new ILInstruction[parameterTypes.Length];
			for (int i = signature.Parameters.Count - 1; i >= 0; i--) {
				parameterTypes[i] = typeSystem.Resolve(signature.Parameters[i].ParameterType);
				arguments[i] = Pop(parameterTypes[i].GetStackType());
			}
			var call = new CallIndirect(
				signature.CallingConvention,
				typeSystem.Resolve(signature.ReturnType),
				parameterTypes.ToImmutableArray(),
				arguments,
				functionPointer
			);
			if (call.ResultType != StackType.Void)
				return Push(call);
			else
				return call;
		}

		static int GetPopCount(OpCode callCode, MethodReference methodReference)
		{
			int popCount = methodReference.Parameters.Count;
			if (callCode != OpCode.NewObj && methodReference.HasThis)
				popCount++;
			return popCount;
		}

		static int GetPopCount(OpCode callCode, IMethod method)
		{
			int popCount = method.Parameters.Count;
			if (callCode != OpCode.NewObj && !method.IsStatic)
				popCount++;
			return popCount;
		}

		ILInstruction Comparison(ComparisonKind kind, bool un = false)
		{
			var right = Pop();
			var left = Pop();
			// make the implicit I4->I conversion explicit:
			if (left.ResultType == StackType.I4 && right.ResultType == StackType.I) {
				left = new Conv(left, PrimitiveType.I, false, Sign.None);
			} else if (left.ResultType == StackType.I && right.ResultType == StackType.I4) {
				right = new Conv(right, PrimitiveType.I, false, Sign.None);
			}
			
			// Based on Table 4: Binary Comparison or Branch Operation
			if (left.ResultType.IsFloatType() && right.ResultType.IsFloatType()) {
				if (left.ResultType != right.ResultType) {
					// make the implicit F4->F8 conversion explicit:
					if (left.ResultType == StackType.F4) {
						left = new Conv(left, PrimitiveType.R8, false, Sign.Signed);
					} else if (right.ResultType == StackType.F4) {
						right = new Conv(right, PrimitiveType.R8, false, Sign.Signed);
					}
				}
				if (un) {
					// for floats, 'un' means 'unordered'
					return Comp.LogicNot(new Comp(kind.Negate(), Sign.None, left, right));
				} else {
					return new Comp(kind, Sign.None, left, right);
				}
			} else if (left.ResultType.IsIntegerType() && right.ResultType.IsIntegerType() && !kind.IsEqualityOrInequality()) {
				// integer comparison where the sign matters
				Debug.Assert(right.ResultType.IsIntegerType());
				return new Comp(kind, un ? Sign.Unsigned : Sign.Signed, left, right);
			} else if (left.ResultType == right.ResultType) {
				// integer equality, object reference or managed reference comparison
				return new Comp(kind, Sign.None, left, right);
			} else {
				Warn($"Invalid comparison between {left.ResultType} and {right.ResultType}");
				if (left.ResultType < right.ResultType) {
					left = new Conv(left, right.ResultType.ToPrimitiveType(), false, Sign.Signed);
				} else {
					right = new Conv(right, left.ResultType.ToPrimitiveType(), false, Sign.Signed);
				}
				return new Comp(kind, Sign.None, left, right);
			}
		}

		int? DecodeBranchTarget(bool shortForm)
		{
//			int target = shortForm ? reader.ReadSByte() : reader.ReadInt32();
//			target += reader.Position;
//			return target;
			return ((Cil.Instruction)currentInstruction.Operand)?.Offset;
		}
		
		ILInstruction DecodeComparisonBranch(bool shortForm, ComparisonKind kind, bool un = false)
		{
			int? target = DecodeBranchTarget(shortForm);
			var condition = Comparison(kind, un);
			condition.ILRange = GetCurrentInstructionInterval();
			if (target != null) {
				MarkBranchTarget(target.Value);
				return new IfInstruction(condition, new Branch(target.Value));
			} else {
				return new IfInstruction(condition, new InvalidBranch("Invalid branch target"));
			}
		}

		ILInstruction DecodeConditionalBranch(bool shortForm, bool negate)
		{
			int? target = DecodeBranchTarget(shortForm);
			ILInstruction condition = Pop();
			switch (condition.ResultType) {
				case StackType.O:
					// introduce explicit comparison with null
					condition = new Comp(
						negate ? ComparisonKind.Equality : ComparisonKind.Inequality,
						Sign.None, condition, new LdNull());
					break;
				case StackType.I:
					// introduce explicit comparison with 0
					condition = new Comp(
						negate ? ComparisonKind.Equality : ComparisonKind.Inequality,
						Sign.None, condition, new Conv(new LdcI4(0), PrimitiveType.I, false, Sign.None));
					break;
				case StackType.I8:
					// introduce explicit comparison with 0
					condition = new Comp(
						negate ? ComparisonKind.Equality : ComparisonKind.Inequality,
						Sign.None, condition, new LdcI8(0));
					break;
				case StackType.Ref:
					// introduce explicit comparison with null ref
					condition = new Comp(
						negate ? ComparisonKind.Equality : ComparisonKind.Inequality,
						Sign.None, new Conv(condition, PrimitiveType.I, false, Sign.None), new Conv(new LdcI4(0), PrimitiveType.I, false, Sign.None));
					break;
				case StackType.I4:
					if (negate) {
						condition = Comp.LogicNot(condition);
					}
					break;
				default:
					condition = new Conv(condition, PrimitiveType.I4, false, Sign.None);
					if (negate) {
						condition = Comp.LogicNot(condition);
					}
					break;
			}
			if (target != null) {
				MarkBranchTarget(target.Value);
				return new IfInstruction(condition, new Branch(target.Value));
			} else {
				return new IfInstruction(condition, new InvalidBranch("Invalid branch target"));
			}
		}

		ILInstruction DecodeUnconditionalBranch(bool shortForm, bool isLeave = false)
		{
			int? target = DecodeBranchTarget(shortForm);
			if (isLeave) {
				currentStack = currentStack.Clear();
			}
			if (target != null) {
				MarkBranchTarget(target.Value);
				return new Branch(target.Value);
			} else {
				return new InvalidBranch("Invalid branch target");
			}
		}

		void MarkBranchTarget(int targetILOffset)
		{
			isBranchTarget[targetILOffset] = true;
			StoreStackForOffset(targetILOffset, ref currentStack);
		}

		ILInstruction DecodeSwitch()
		{
//			uint length = reader.ReadUInt32();
//			int baseOffset = 4 * (int)length + reader.Position;
			var labels = (Cil.Instruction[])currentInstruction.Operand;
			var instr = new SwitchInstruction(Pop(StackType.I4));
			
			for (int i = 0; i < labels.Length; i++) {
				var section = new SwitchSection();
				section.Labels = new LongSet(i);
				int? target = labels[i]?.Offset; // baseOffset + reader.ReadInt32();
				if (target != null) {
					MarkBranchTarget(target.Value);
					section.Body = new Branch(target.Value);
				} else {
					section.Body = new InvalidBranch("Invalid branch target");
				}
				instr.Sections.Add(section);
			}
			var defaultSection = new SwitchSection();
			defaultSection.Labels = new LongSet(new LongInterval(0, labels.Length)).Invert();
			defaultSection.Body = new Nop();
			instr.Sections.Add(defaultSection);
			return instr;
		}
		
		ILInstruction BinaryNumeric(BinaryNumericOperator @operator, bool checkForOverflow = false, Sign sign = Sign.None)
		{
			var right = Pop();
			var left = Pop();
			if (@operator != BinaryNumericOperator.ShiftLeft && @operator != BinaryNumericOperator.ShiftRight) {
				// make the implicit I4->I conversion explicit:
				if (left.ResultType == StackType.I4 && right.ResultType == StackType.I) {
					left = new Conv(left, PrimitiveType.I, false, Sign.None);
				} else if (left.ResultType == StackType.I && right.ResultType == StackType.I4) {
					right = new Conv(right, PrimitiveType.I, false, Sign.None);
				}
			}
			return Push(new BinaryNumericInstruction(@operator, left, right, checkForOverflow, sign));
		}

		ILInstruction DecodeJmp()
		{
			IMethod method = ReadAndDecodeMethodReference();
			// Translate jmp into tail call:
			Call call = new Call(method);
			call.IsTail = true;
			call.ILStackWasEmpty = true;
			if (!method.IsStatic) {
				call.Arguments.Add(Ldarg(0));
			}
			foreach (var p in method.Parameters) {
				call.Arguments.Add(Ldarg(call.Arguments.Count));
			}
			return new Leave(mainContainer, call);
		}

		ILInstruction LdToken(IMetadataTokenProvider token)
		{
			if (token is TypeReference)
				return new LdTypeToken(typeSystem.Resolve((TypeReference)token));
			if (token is FieldReference)
				return new LdMemberToken(typeSystem.Resolve((FieldReference)token));
			if (token is MethodReference)
				return new LdMemberToken(typeSystem.Resolve((MethodReference)token));
			throw new NotImplementedException();
		}
	}
}
