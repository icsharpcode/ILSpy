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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Threading;

using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

using ArrayType = ICSharpCode.Decompiler.TypeSystem.ArrayType;
using ByReferenceType = ICSharpCode.Decompiler.TypeSystem.ByReferenceType;
using PinnedType = ICSharpCode.Decompiler.TypeSystem.Implementation.PinnedType;

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// Reads IL bytecodes and converts them into ILAst instructions.
	/// </summary>
	/// <remarks>
	/// Instances of this class are not thread-safe. Use separate instances to decompile multiple members in parallel.
	/// </remarks>
	public class ILReader
	{
		readonly ICompilation compilation;
		readonly MetadataModule module;
		readonly MetadataReader metadata;

		public bool UseDebugSymbols { get; set; }
		public DebugInfo.IDebugInfoProvider DebugInfo { get; set; }
		public List<string> Warnings { get; } = new List<string>();

		// List of candidate locations for sequence points. Includes empty il stack locations, any nop instructions, and the instruction following
		// a call instruction. 
		public List<int> SequencePointCandidates { get; private set; }

		/// <summary>
		/// Creates a new ILReader instance.
		/// </summary>
		/// <param name="module">
		/// The module used to resolve metadata tokens in the type system.
		/// </param>
		public ILReader(MetadataModule module)
		{
			if (module == null)
				throw new ArgumentNullException(nameof(module));
			this.module = module;
			this.compilation = module.Compilation;
			this.metadata = module.metadata;
			this.SequencePointCandidates = new List<int>();
		}

		GenericContext genericContext;
		IMethod method;
		MethodBodyBlock body;
		StackType methodReturnStackType;
		BlobReader reader;
		ImmutableStack<ILVariable> currentStack;
		ILVariable[] parameterVariables;
		ILVariable[] localVariables;
		BitArray isBranchTarget;
		BlockContainer mainContainer;
		List<ILInstruction> instructionBuilder;
		int currentInstructionStart;

		// Dictionary that stores stacks for each IL instruction
		Dictionary<int, ImmutableStack<ILVariable>> stackByOffset;
		Dictionary<ExceptionRegion, ILVariable> variableByExceptionHandler;
		UnionFind<ILVariable> unionFind;
		List<(ILVariable, ILVariable)> stackMismatchPairs;
		IEnumerable<ILVariable> stackVariables;

		void Init(MethodDefinitionHandle methodDefinitionHandle, MethodBodyBlock body, GenericContext genericContext)
		{
			if (body == null)
				throw new ArgumentNullException(nameof(body));
			if (methodDefinitionHandle.IsNil)
				throw new ArgumentException("methodDefinitionHandle.IsNil");
			this.method = module.GetDefinition(methodDefinitionHandle);
			if (genericContext.ClassTypeParameters == null && genericContext.MethodTypeParameters == null)
			{
				// no generic context specified: use the method's own type parameters
				genericContext = new GenericContext(method);
			}
			else
			{
				// generic context specified, so specialize the method for it:
				this.method = this.method.Specialize(genericContext.ToSubstitution());
			}
			this.genericContext = genericContext;
			this.body = body;
			this.reader = body.GetILReader();
			this.currentStack = ImmutableStack<ILVariable>.Empty;
			this.unionFind = new UnionFind<ILVariable>();
			this.stackMismatchPairs = new List<(ILVariable, ILVariable)>();
			this.methodReturnStackType = method.ReturnType.GetStackType();
			InitParameterVariables();
			localVariables = InitLocalVariables();
			foreach (var v in localVariables)
			{
				v.InitialValueIsInitialized = body.LocalVariablesInitialized;
				v.UsesInitialValue = true;
			}
			this.mainContainer = new BlockContainer(expectedResultType: methodReturnStackType);
			this.instructionBuilder = new List<ILInstruction>();
			this.isBranchTarget = new BitArray(reader.Length);
			this.stackByOffset = new Dictionary<int, ImmutableStack<ILVariable>>();
			this.variableByExceptionHandler = new Dictionary<ExceptionRegion, ILVariable>();
		}

		EntityHandle ReadAndDecodeMetadataToken()
		{
			int token = reader.ReadInt32();
			if (token <= 0)
			{
				// SRM uses negative tokens as "virtual tokens" and can get confused
				// if we manually create them.
				// Row-IDs < 1 are always invalid.
				throw new BadImageFormatException("Invalid metadata token");
			}
			return MetadataTokens.EntityHandle(token);
		}

		IType ReadAndDecodeTypeReference()
		{
			var typeReference = ReadAndDecodeMetadataToken();
			return module.ResolveType(typeReference, genericContext);
		}

		IMethod ReadAndDecodeMethodReference()
		{
			var methodReference = ReadAndDecodeMetadataToken();
			return module.ResolveMethod(methodReference, genericContext);
		}

		IField ReadAndDecodeFieldReference()
		{
			var fieldReference = ReadAndDecodeMetadataToken();
			var f = module.ResolveEntity(fieldReference, genericContext) as IField;
			if (f == null)
				throw new BadImageFormatException("Invalid field token");
			return f;
		}

		ILVariable[] InitLocalVariables()
		{
			if (body.LocalSignature.IsNil)
				return Empty<ILVariable>.Array;
			ImmutableArray<IType> variableTypes;
			try
			{
				variableTypes = module.DecodeLocalSignature(body.LocalSignature, genericContext);
			}
			catch (BadImageFormatException ex)
			{
				Warnings.Add("Error decoding local variables: " + ex.Message);
				variableTypes = ImmutableArray<IType>.Empty;
			}
			var localVariables = new ILVariable[variableTypes.Length];
			foreach (var (index, type) in variableTypes.WithIndex())
			{
				localVariables[index] = CreateILVariable(index, type);
			}
			return localVariables;
		}

		void InitParameterVariables()
		{
			int popCount = method.Parameters.Count;
			if (!method.IsStatic)
				popCount++;
			if (method.Parameters.LastOrDefault()?.Type == SpecialType.ArgList)
				popCount--;
			parameterVariables = new ILVariable[popCount];
			int paramIndex = 0;
			int offset = 0;
			if (!method.IsStatic)
			{
				offset = 1;
				IType declaringType = method.DeclaringType;
				if (declaringType.IsUnbound())
				{
					// If method is a definition (and not specialized), the declaring type is also just a definition,
					// and needs to be converted into a normally usable type.
					declaringType = new ParameterizedType(declaringType, declaringType.TypeParameters);
				}
				ILVariable ilVar = CreateILVariable(-1, declaringType, "this");
				ilVar.IsRefReadOnly = method.ThisIsRefReadOnly;
				parameterVariables[paramIndex++] = ilVar;
			}
			while (paramIndex < parameterVariables.Length)
			{
				IParameter parameter = method.Parameters[paramIndex - offset];
				ILVariable ilVar = CreateILVariable(paramIndex - offset, parameter.Type, parameter.Name);
				ilVar.IsRefReadOnly = parameter.IsIn;
				parameterVariables[paramIndex] = ilVar;
				paramIndex++;
			}
			Debug.Assert(paramIndex == parameterVariables.Length);
		}

		ILVariable CreateILVariable(int index, IType type)
		{
			VariableKind kind;
			if (type.SkipModifiers() is PinnedType pinned)
			{
				kind = VariableKind.PinnedLocal;
				type = pinned.ElementType;
			}
			else
			{
				kind = VariableKind.Local;
			}
			ILVariable ilVar = new ILVariable(kind, type, index);
			if (!UseDebugSymbols || DebugInfo == null || !DebugInfo.TryGetName((MethodDefinitionHandle)method.MetadataToken, index, out string name))
			{
				ilVar.Name = "V_" + index;
				ilVar.HasGeneratedName = true;
			}
			else if (string.IsNullOrWhiteSpace(name))
			{
				ilVar.Name = "V_" + index;
				ilVar.HasGeneratedName = true;
			}
			else
			{
				ilVar.Name = name;
			}
			return ilVar;
		}

		ILVariable CreateILVariable(int index, IType parameterType, string name)
		{
			Debug.Assert(!parameterType.IsUnbound());
			ITypeDefinition def = parameterType.GetDefinition();
			if (def != null && index < 0 && def.IsReferenceType == false)
			{
				parameterType = new ByReferenceType(parameterType);
			}
			var ilVar = new ILVariable(VariableKind.Parameter, parameterType, index);
			Debug.Assert(ilVar.StoreCount == 1); // count the initial store when the method is called with an argument
			if (index < 0)
				ilVar.Name = "this";
			else if (string.IsNullOrEmpty(name))
				ilVar.Name = "P_" + index;
			else
				ilVar.Name = name;
			return ilVar;
		}

		/// <summary>
		/// Warn when invalid IL is detected.
		/// ILSpy should be able to handle invalid IL; but this method can be helpful for debugging the ILReader,
		/// as this method should not get called when processing valid IL.
		/// </summary>
		void Warn(string message)
		{
			Warnings.Add(string.Format("IL_{0:x4}: {1}", currentInstructionStart, message));
		}

		ImmutableStack<ILVariable> MergeStacks(ImmutableStack<ILVariable> a, ImmutableStack<ILVariable> b)
		{
			if (CheckStackCompatibleWithoutAdjustments(a, b))
			{
				// We only need to union the input variables, but can 
				// otherwise re-use the existing stack.
				ImmutableStack<ILVariable> output = a;
				while (!a.IsEmpty && !b.IsEmpty)
				{
					Debug.Assert(a.Peek().StackType == b.Peek().StackType);
					unionFind.Merge(a.Peek(), b.Peek());
					a = a.Pop();
					b = b.Pop();
				}
				return output;
			}
			else if (a.Count() != b.Count())
			{
				// Let's not try to merge mismatched stacks.
				Warn("Incompatible stack heights: " + a.Count() + " vs " + b.Count());
				return a;
			}
			else
			{
				// The more complex case where the stacks don't match exactly.
				var output = new List<ILVariable>();
				while (!a.IsEmpty && !b.IsEmpty)
				{
					var varA = a.Peek();
					var varB = b.Peek();
					if (varA.StackType == varB.StackType)
					{
						unionFind.Merge(varA, varB);
						output.Add(varA);
					}
					else
					{
						if (!IsValidTypeStackTypeMerge(varA.StackType, varB.StackType))
						{
							Warn("Incompatible stack types: " + varA.StackType + " vs " + varB.StackType);
						}
						if (varA.StackType > varB.StackType)
						{
							output.Add(varA);
							// every store to varB should also store to varA
							stackMismatchPairs.Add((varB, varA));
						}
						else
						{
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
			while (!a.IsEmpty && !b.IsEmpty)
			{
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
			if (stackByOffset.TryGetValue(offset, out var existing))
			{
				stack = MergeStacks(existing, stack);
				if (stack != existing)
					stackByOffset[offset] = stack;
			}
			else
			{
				stackByOffset.Add(offset, stack);
			}
		}

		void ReadInstructions(CancellationToken cancellationToken)
		{
			// Fill isBranchTarget and branchStackDict based on exception handlers
			foreach (var eh in body.ExceptionRegions)
			{
				ImmutableStack<ILVariable> ehStack = null;
				if (eh.Kind == ExceptionRegionKind.Catch)
				{
					var catchType = module.ResolveType(eh.CatchType, genericContext);
					var v = new ILVariable(VariableKind.ExceptionStackSlot, catchType, eh.HandlerOffset) {
						Name = "E_" + eh.HandlerOffset,
						HasGeneratedName = true
					};
					variableByExceptionHandler.Add(eh, v);
					ehStack = ImmutableStack.Create(v);
				}
				else if (eh.Kind == ExceptionRegionKind.Filter)
				{
					var v = new ILVariable(VariableKind.ExceptionStackSlot, compilation.FindType(KnownTypeCode.Object), eh.HandlerOffset) {
						Name = "E_" + eh.HandlerOffset,
						HasGeneratedName = true
					};
					variableByExceptionHandler.Add(eh, v);
					ehStack = ImmutableStack.Create(v);
				}
				else
				{
					ehStack = ImmutableStack<ILVariable>.Empty;
				}
				if (eh.FilterOffset != -1)
				{
					isBranchTarget[eh.FilterOffset] = true;
					StoreStackForOffset(eh.FilterOffset, ref ehStack);
				}
				if (eh.HandlerOffset != -1)
				{
					isBranchTarget[eh.HandlerOffset] = true;
					StoreStackForOffset(eh.HandlerOffset, ref ehStack);
				}
			}

			reader.Reset();
			while (reader.RemainingBytes > 0)
			{
				cancellationToken.ThrowIfCancellationRequested();
				int start = reader.Offset;
				StoreStackForOffset(start, ref currentStack);
				currentInstructionStart = start;
				bool startedWithEmptyStack = currentStack.IsEmpty;
				ILInstruction decodedInstruction;
				try
				{
					decodedInstruction = DecodeInstruction();
				}
				catch (BadImageFormatException ex)
				{
					decodedInstruction = new InvalidBranch(ex.Message);
				}
				if (decodedInstruction.ResultType == StackType.Unknown && decodedInstruction.OpCode != OpCode.InvalidBranch && UnpackPush(decodedInstruction).OpCode != OpCode.InvalidExpression)
					Warn("Unknown result type (might be due to invalid IL or missing references)");
				decodedInstruction.CheckInvariant(ILPhase.InILReader);
				int end = reader.Offset;
				decodedInstruction.AddILRange(new Interval(start, end));
				UnpackPush(decodedInstruction).AddILRange(decodedInstruction);
				instructionBuilder.Add(decodedInstruction);
				if (decodedInstruction.HasDirectFlag(InstructionFlags.EndPointUnreachable))
				{
					if (!stackByOffset.TryGetValue(end, out currentStack))
					{
						currentStack = ImmutableStack<ILVariable>.Empty;
					}
				}

				if (IsSequencePointInstruction(decodedInstruction) || startedWithEmptyStack)
				{
					this.SequencePointCandidates.Add(decodedInstruction.StartILOffset);
				}
			}

			var visitor = new CollectStackVariablesVisitor(unionFind);
			for (int i = 0; i < instructionBuilder.Count; i++)
			{
				instructionBuilder[i] = instructionBuilder[i].AcceptVisitor(visitor);
			}
			stackVariables = visitor.variables;
			InsertStackAdjustments();
		}

		private bool IsSequencePointInstruction(ILInstruction instruction)
		{
			if (instruction.OpCode == OpCode.Nop ||
				(this.instructionBuilder.Count > 0 &&
				this.instructionBuilder.Last().OpCode == OpCode.Call ||
				this.instructionBuilder.Last().OpCode == OpCode.CallIndirect ||
				this.instructionBuilder.Last().OpCode == OpCode.CallVirt))
			{

				return true;
			}
			else
			{
				return false;
			}
		}

		void InsertStackAdjustments()
		{
			if (stackMismatchPairs.Count == 0)
				return;
			var dict = new MultiDictionary<ILVariable, ILVariable>();
			foreach (var (origA, origB) in stackMismatchPairs)
			{
				var a = unionFind.Find(origA);
				var b = unionFind.Find(origB);
				Debug.Assert(a.StackType < b.StackType);
				// For every store to a, insert a converting store to b.
				if (!dict[a].Contains(b))
					dict.Add(a, b);
			}
			var newInstructions = new List<ILInstruction>();
			foreach (var inst in instructionBuilder)
			{
				newInstructions.Add(inst);
				if (inst is StLoc store)
				{
					foreach (var additionalVar in dict[store.Variable])
					{
						ILInstruction value = new LdLoc(store.Variable);
						value = new Conv(value, additionalVar.StackType.ToPrimitiveType(), false, Sign.Signed);
						newInstructions.Add(new StLoc(additionalVar, value) {
							IsStackAdjustment = true,
						}.WithILRange(inst));
					}
				}
			}
			instructionBuilder = newInstructions;
		}

		/// <summary>
		/// Debugging helper: writes the decoded instruction stream interleaved with the inferred evaluation stack layout.
		/// </summary>
		public void WriteTypedIL(MethodDefinitionHandle method, MethodBodyBlock body,
			ITextOutput output, GenericContext genericContext = default, CancellationToken cancellationToken = default)
		{
			Init(method, body, genericContext);
			ReadInstructions(cancellationToken);
			foreach (var inst in instructionBuilder)
			{
				if (inst is StLoc stloc && stloc.IsStackAdjustment)
				{
					output.Write("          ");
					inst.WriteTo(output, new ILAstWritingOptions());
					output.WriteLine();
					continue;
				}
				output.Write("   [");
				bool isFirstElement = true;
				foreach (var element in stackByOffset[inst.StartILOffset])
				{
					if (isFirstElement)
						isFirstElement = false;
					else
						output.Write(", ");
					output.WriteLocalReference(element.Name, element);
					output.Write(":");
					output.Write(element.StackType);
				}
				output.Write(']');
				output.WriteLine();
				if (isBranchTarget[inst.StartILOffset])
					output.Write('*');
				else
					output.Write(' ');
				output.WriteLocalReference("IL_" + inst.StartILOffset.ToString("x4"), inst.StartILOffset, isDefinition: true);
				output.Write(": ");
				inst.WriteTo(output, new ILAstWritingOptions());
				output.WriteLine();
			}
			new Disassembler.MethodBodyDisassembler(output, cancellationToken) { DetectControlStructure = false }
				.WriteExceptionHandlers(module.PEFile, method, body);
		}

		/// <summary>
		/// Decodes the specified method body and returns an ILFunction.
		/// </summary>
		public ILFunction ReadIL(MethodDefinitionHandle method, MethodBodyBlock body, GenericContext genericContext = default, ILFunctionKind kind = ILFunctionKind.TopLevelFunction, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Init(method, body, genericContext);
			ReadInstructions(cancellationToken);
			var blockBuilder = new BlockBuilder(body, variableByExceptionHandler, compilation);
			blockBuilder.CreateBlocks(mainContainer, instructionBuilder, isBranchTarget, cancellationToken);
			var function = new ILFunction(this.method, body.GetCodeSize(), this.genericContext, mainContainer, kind);
			function.Variables.AddRange(parameterVariables);
			function.Variables.AddRange(localVariables);
			function.Variables.AddRange(stackVariables);
			function.Variables.AddRange(variableByExceptionHandler.Values);
			function.Variables.AddRange(blockBuilder.OnErrorDispatcherVariables);
			function.AddRef(); // mark the root node
			var removedBlocks = new List<Block>();
			foreach (var c in function.Descendants.OfType<BlockContainer>())
			{
				var newOrder = c.TopologicalSort(deleteUnreachableBlocks: true);
				if (newOrder.Count < c.Blocks.Count)
				{
					removedBlocks.AddRange(c.Blocks.Except(newOrder));
				}
				c.Blocks.ReplaceList(newOrder);
			}
			if (removedBlocks.Count > 0)
			{
				removedBlocks.SortBy(b => b.StartILOffset);
				function.Warnings.Add("Discarded unreachable code: "
							+ string.Join(", ", removedBlocks.Select(b => $"IL_{b.StartILOffset:x4}")));
			}

			this.SequencePointCandidates.Sort();
			function.SequencePointCandidates = this.SequencePointCandidates;

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
			switch (PeekStackType())
			{
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
			if (reader.RemainingBytes == 0)
				return new InvalidBranch("Unexpected end of body");
			var opCode = ILParser.DecodeOpCode(ref reader);
			switch (opCode)
			{
				case ILOpCode.Constrained:
					return DecodeConstrainedCall();
				case ILOpCode.Readonly:
					return DecodeReadonly();
				case ILOpCode.Tail:
					return DecodeTailCall();
				case ILOpCode.Unaligned:
					return DecodeUnaligned();
				case ILOpCode.Volatile:
					return DecodeVolatile();
				case ILOpCode.Add:
					return BinaryNumeric(BinaryNumericOperator.Add);
				case ILOpCode.Add_ovf:
					return BinaryNumeric(BinaryNumericOperator.Add, true, Sign.Signed);
				case ILOpCode.Add_ovf_un:
					return BinaryNumeric(BinaryNumericOperator.Add, true, Sign.Unsigned);
				case ILOpCode.And:
					return BinaryNumeric(BinaryNumericOperator.BitAnd);
				case ILOpCode.Arglist:
					return Push(new Arglist());
				case ILOpCode.Beq:
					return DecodeComparisonBranch(opCode, ComparisonKind.Equality);
				case ILOpCode.Beq_s:
					return DecodeComparisonBranch(opCode, ComparisonKind.Equality);
				case ILOpCode.Bge:
					return DecodeComparisonBranch(opCode, ComparisonKind.GreaterThanOrEqual);
				case ILOpCode.Bge_s:
					return DecodeComparisonBranch(opCode, ComparisonKind.GreaterThanOrEqual);
				case ILOpCode.Bge_un:
					return DecodeComparisonBranch(opCode, ComparisonKind.GreaterThanOrEqual, un: true);
				case ILOpCode.Bge_un_s:
					return DecodeComparisonBranch(opCode, ComparisonKind.GreaterThanOrEqual, un: true);
				case ILOpCode.Bgt:
					return DecodeComparisonBranch(opCode, ComparisonKind.GreaterThan);
				case ILOpCode.Bgt_s:
					return DecodeComparisonBranch(opCode, ComparisonKind.GreaterThan);
				case ILOpCode.Bgt_un:
					return DecodeComparisonBranch(opCode, ComparisonKind.GreaterThan, un: true);
				case ILOpCode.Bgt_un_s:
					return DecodeComparisonBranch(opCode, ComparisonKind.GreaterThan, un: true);
				case ILOpCode.Ble:
					return DecodeComparisonBranch(opCode, ComparisonKind.LessThanOrEqual);
				case ILOpCode.Ble_s:
					return DecodeComparisonBranch(opCode, ComparisonKind.LessThanOrEqual);
				case ILOpCode.Ble_un:
					return DecodeComparisonBranch(opCode, ComparisonKind.LessThanOrEqual, un: true);
				case ILOpCode.Ble_un_s:
					return DecodeComparisonBranch(opCode, ComparisonKind.LessThanOrEqual, un: true);
				case ILOpCode.Blt:
					return DecodeComparisonBranch(opCode, ComparisonKind.LessThan);
				case ILOpCode.Blt_s:
					return DecodeComparisonBranch(opCode, ComparisonKind.LessThan);
				case ILOpCode.Blt_un:
					return DecodeComparisonBranch(opCode, ComparisonKind.LessThan, un: true);
				case ILOpCode.Blt_un_s:
					return DecodeComparisonBranch(opCode, ComparisonKind.LessThan, un: true);
				case ILOpCode.Bne_un:
					return DecodeComparisonBranch(opCode, ComparisonKind.Inequality, un: true);
				case ILOpCode.Bne_un_s:
					return DecodeComparisonBranch(opCode, ComparisonKind.Inequality, un: true);
				case ILOpCode.Br:
					return DecodeUnconditionalBranch(opCode);
				case ILOpCode.Br_s:
					return DecodeUnconditionalBranch(opCode);
				case ILOpCode.Break:
					return new DebugBreak();
				case ILOpCode.Brfalse:
					return DecodeConditionalBranch(opCode, true);
				case ILOpCode.Brfalse_s:
					return DecodeConditionalBranch(opCode, true);
				case ILOpCode.Brtrue:
					return DecodeConditionalBranch(opCode, false);
				case ILOpCode.Brtrue_s:
					return DecodeConditionalBranch(opCode, false);
				case ILOpCode.Call:
					return DecodeCall(OpCode.Call);
				case ILOpCode.Callvirt:
					return DecodeCall(OpCode.CallVirt);
				case ILOpCode.Calli:
					return DecodeCallIndirect();
				case ILOpCode.Ceq:
					return Push(Comparison(ComparisonKind.Equality));
				case ILOpCode.Cgt:
					return Push(Comparison(ComparisonKind.GreaterThan));
				case ILOpCode.Cgt_un:
					return Push(Comparison(ComparisonKind.GreaterThan, un: true));
				case ILOpCode.Clt:
					return Push(Comparison(ComparisonKind.LessThan));
				case ILOpCode.Clt_un:
					return Push(Comparison(ComparisonKind.LessThan, un: true));
				case ILOpCode.Ckfinite:
					return new Ckfinite(Peek());
				case ILOpCode.Conv_i1:
					return Push(new Conv(Pop(), PrimitiveType.I1, false, Sign.None));
				case ILOpCode.Conv_i2:
					return Push(new Conv(Pop(), PrimitiveType.I2, false, Sign.None));
				case ILOpCode.Conv_i4:
					return Push(new Conv(Pop(), PrimitiveType.I4, false, Sign.None));
				case ILOpCode.Conv_i8:
					return Push(new Conv(Pop(), PrimitiveType.I8, false, Sign.None));
				case ILOpCode.Conv_r4:
					return Push(new Conv(Pop(), PrimitiveType.R4, false, Sign.Signed));
				case ILOpCode.Conv_r8:
					return Push(new Conv(Pop(), PrimitiveType.R8, false, Sign.Signed));
				case ILOpCode.Conv_u1:
					return Push(new Conv(Pop(), PrimitiveType.U1, false, Sign.None));
				case ILOpCode.Conv_u2:
					return Push(new Conv(Pop(), PrimitiveType.U2, false, Sign.None));
				case ILOpCode.Conv_u4:
					return Push(new Conv(Pop(), PrimitiveType.U4, false, Sign.None));
				case ILOpCode.Conv_u8:
					return Push(new Conv(Pop(), PrimitiveType.U8, false, Sign.None));
				case ILOpCode.Conv_i:
					return Push(new Conv(Pop(), PrimitiveType.I, false, Sign.None));
				case ILOpCode.Conv_u:
					return Push(new Conv(Pop(), PrimitiveType.U, false, Sign.None));
				case ILOpCode.Conv_r_un:
					return Push(new Conv(Pop(), PrimitiveType.R, false, Sign.Unsigned));
				case ILOpCode.Conv_ovf_i1:
					return Push(new Conv(Pop(), PrimitiveType.I1, true, Sign.Signed));
				case ILOpCode.Conv_ovf_i2:
					return Push(new Conv(Pop(), PrimitiveType.I2, true, Sign.Signed));
				case ILOpCode.Conv_ovf_i4:
					return Push(new Conv(Pop(), PrimitiveType.I4, true, Sign.Signed));
				case ILOpCode.Conv_ovf_i8:
					return Push(new Conv(Pop(), PrimitiveType.I8, true, Sign.Signed));
				case ILOpCode.Conv_ovf_u1:
					return Push(new Conv(Pop(), PrimitiveType.U1, true, Sign.Signed));
				case ILOpCode.Conv_ovf_u2:
					return Push(new Conv(Pop(), PrimitiveType.U2, true, Sign.Signed));
				case ILOpCode.Conv_ovf_u4:
					return Push(new Conv(Pop(), PrimitiveType.U4, true, Sign.Signed));
				case ILOpCode.Conv_ovf_u8:
					return Push(new Conv(Pop(), PrimitiveType.U8, true, Sign.Signed));
				case ILOpCode.Conv_ovf_i:
					return Push(new Conv(Pop(), PrimitiveType.I, true, Sign.Signed));
				case ILOpCode.Conv_ovf_u:
					return Push(new Conv(Pop(), PrimitiveType.U, true, Sign.Signed));
				case ILOpCode.Conv_ovf_i1_un:
					return Push(new Conv(Pop(), PrimitiveType.I1, true, Sign.Unsigned));
				case ILOpCode.Conv_ovf_i2_un:
					return Push(new Conv(Pop(), PrimitiveType.I2, true, Sign.Unsigned));
				case ILOpCode.Conv_ovf_i4_un:
					return Push(new Conv(Pop(), PrimitiveType.I4, true, Sign.Unsigned));
				case ILOpCode.Conv_ovf_i8_un:
					return Push(new Conv(Pop(), PrimitiveType.I8, true, Sign.Unsigned));
				case ILOpCode.Conv_ovf_u1_un:
					return Push(new Conv(Pop(), PrimitiveType.U1, true, Sign.Unsigned));
				case ILOpCode.Conv_ovf_u2_un:
					return Push(new Conv(Pop(), PrimitiveType.U2, true, Sign.Unsigned));
				case ILOpCode.Conv_ovf_u4_un:
					return Push(new Conv(Pop(), PrimitiveType.U4, true, Sign.Unsigned));
				case ILOpCode.Conv_ovf_u8_un:
					return Push(new Conv(Pop(), PrimitiveType.U8, true, Sign.Unsigned));
				case ILOpCode.Conv_ovf_i_un:
					return Push(new Conv(Pop(), PrimitiveType.I, true, Sign.Unsigned));
				case ILOpCode.Conv_ovf_u_un:
					return Push(new Conv(Pop(), PrimitiveType.U, true, Sign.Unsigned));
				case ILOpCode.Cpblk:
					return new Cpblk(size: Pop(StackType.I4), sourceAddress: PopPointer(), destAddress: PopPointer());
				case ILOpCode.Div:
					return BinaryNumeric(BinaryNumericOperator.Div, false, Sign.Signed);
				case ILOpCode.Div_un:
					return BinaryNumeric(BinaryNumericOperator.Div, false, Sign.Unsigned);
				case ILOpCode.Dup:
					return Push(Peek());
				case ILOpCode.Endfilter:
					return new Leave(null, Pop());
				case ILOpCode.Endfinally:
					return new Leave(null);
				case ILOpCode.Initblk:
					return new Initblk(size: Pop(StackType.I4), value: Pop(StackType.I4), address: PopPointer());
				case ILOpCode.Jmp:
					return DecodeJmp();
				case ILOpCode.Ldarg:
				case ILOpCode.Ldarg_s:
					return Push(Ldarg(ILParser.DecodeIndex(ref reader, opCode)));
				case ILOpCode.Ldarg_0:
					return Push(Ldarg(0));
				case ILOpCode.Ldarg_1:
					return Push(Ldarg(1));
				case ILOpCode.Ldarg_2:
					return Push(Ldarg(2));
				case ILOpCode.Ldarg_3:
					return Push(Ldarg(3));
				case ILOpCode.Ldarga:
				case ILOpCode.Ldarga_s:
					return Push(Ldarga(ILParser.DecodeIndex(ref reader, opCode)));
				case ILOpCode.Ldc_i4:
					return Push(new LdcI4(reader.ReadInt32()));
				case ILOpCode.Ldc_i8:
					return Push(new LdcI8(reader.ReadInt64()));
				case ILOpCode.Ldc_r4:
					return Push(new LdcF4(reader.ReadSingle()));
				case ILOpCode.Ldc_r8:
					return Push(new LdcF8(reader.ReadDouble()));
				case ILOpCode.Ldc_i4_m1:
					return Push(new LdcI4(-1));
				case ILOpCode.Ldc_i4_0:
					return Push(new LdcI4(0));
				case ILOpCode.Ldc_i4_1:
					return Push(new LdcI4(1));
				case ILOpCode.Ldc_i4_2:
					return Push(new LdcI4(2));
				case ILOpCode.Ldc_i4_3:
					return Push(new LdcI4(3));
				case ILOpCode.Ldc_i4_4:
					return Push(new LdcI4(4));
				case ILOpCode.Ldc_i4_5:
					return Push(new LdcI4(5));
				case ILOpCode.Ldc_i4_6:
					return Push(new LdcI4(6));
				case ILOpCode.Ldc_i4_7:
					return Push(new LdcI4(7));
				case ILOpCode.Ldc_i4_8:
					return Push(new LdcI4(8));
				case ILOpCode.Ldc_i4_s:
					return Push(new LdcI4(reader.ReadSByte()));
				case ILOpCode.Ldnull:
					return Push(new LdNull());
				case ILOpCode.Ldstr:
					return Push(DecodeLdstr());
				case ILOpCode.Ldftn:
					return Push(new LdFtn(ReadAndDecodeMethodReference()));
				case ILOpCode.Ldind_i1:
					return Push(new LdObj(PopPointer(), compilation.FindType(KnownTypeCode.SByte)));
				case ILOpCode.Ldind_i2:
					return Push(new LdObj(PopPointer(), compilation.FindType(KnownTypeCode.Int16)));
				case ILOpCode.Ldind_i4:
					return Push(new LdObj(PopPointer(), compilation.FindType(KnownTypeCode.Int32)));
				case ILOpCode.Ldind_i8:
					return Push(new LdObj(PopPointer(), compilation.FindType(KnownTypeCode.Int64)));
				case ILOpCode.Ldind_u1:
					return Push(new LdObj(PopPointer(), compilation.FindType(KnownTypeCode.Byte)));
				case ILOpCode.Ldind_u2:
					return Push(new LdObj(PopPointer(), compilation.FindType(KnownTypeCode.UInt16)));
				case ILOpCode.Ldind_u4:
					return Push(new LdObj(PopPointer(), compilation.FindType(KnownTypeCode.UInt32)));
				case ILOpCode.Ldind_r4:
					return Push(new LdObj(PopPointer(), compilation.FindType(KnownTypeCode.Single)));
				case ILOpCode.Ldind_r8:
					return Push(new LdObj(PopPointer(), compilation.FindType(KnownTypeCode.Double)));
				case ILOpCode.Ldind_i:
					return Push(new LdObj(PopPointer(), compilation.FindType(KnownTypeCode.IntPtr)));
				case ILOpCode.Ldind_ref:
					return Push(new LdObj(PopPointer(), compilation.FindType(KnownTypeCode.Object)));
				case ILOpCode.Ldloc:
				case ILOpCode.Ldloc_s:
					return Push(Ldloc(ILParser.DecodeIndex(ref reader, opCode)));
				case ILOpCode.Ldloc_0:
					return Push(Ldloc(0));
				case ILOpCode.Ldloc_1:
					return Push(Ldloc(1));
				case ILOpCode.Ldloc_2:
					return Push(Ldloc(2));
				case ILOpCode.Ldloc_3:
					return Push(Ldloc(3));
				case ILOpCode.Ldloca:
				case ILOpCode.Ldloca_s:
					return Push(Ldloca(ILParser.DecodeIndex(ref reader, opCode)));
				case ILOpCode.Leave:
					return DecodeUnconditionalBranch(opCode, isLeave: true);
				case ILOpCode.Leave_s:
					return DecodeUnconditionalBranch(opCode, isLeave: true);
				case ILOpCode.Localloc:
					return Push(new LocAlloc(Pop()));
				case ILOpCode.Mul:
					return BinaryNumeric(BinaryNumericOperator.Mul, false, Sign.None);
				case ILOpCode.Mul_ovf:
					return BinaryNumeric(BinaryNumericOperator.Mul, true, Sign.Signed);
				case ILOpCode.Mul_ovf_un:
					return BinaryNumeric(BinaryNumericOperator.Mul, true, Sign.Unsigned);
				case ILOpCode.Neg:
					return Neg();
				case ILOpCode.Newobj:
					return DecodeCall(OpCode.NewObj);
				case ILOpCode.Nop:
					return new Nop();
				case ILOpCode.Not:
					return Push(new BitNot(Pop()));
				case ILOpCode.Or:
					return BinaryNumeric(BinaryNumericOperator.BitOr);
				case ILOpCode.Pop:
					Pop();
					return new Nop() { Kind = NopKind.Pop };
				case ILOpCode.Rem:
					return BinaryNumeric(BinaryNumericOperator.Rem, false, Sign.Signed);
				case ILOpCode.Rem_un:
					return BinaryNumeric(BinaryNumericOperator.Rem, false, Sign.Unsigned);
				case ILOpCode.Ret:
					return Return();
				case ILOpCode.Shl:
					return BinaryNumeric(BinaryNumericOperator.ShiftLeft, false, Sign.None);
				case ILOpCode.Shr:
					return BinaryNumeric(BinaryNumericOperator.ShiftRight, false, Sign.Signed);
				case ILOpCode.Shr_un:
					return BinaryNumeric(BinaryNumericOperator.ShiftRight, false, Sign.Unsigned);
				case ILOpCode.Starg:
				case ILOpCode.Starg_s:
					return Starg(ILParser.DecodeIndex(ref reader, opCode));
				case ILOpCode.Stind_i1:
					return new StObj(value: Pop(StackType.I4), target: PopPointer(), type: compilation.FindType(KnownTypeCode.SByte));
				case ILOpCode.Stind_i2:
					return new StObj(value: Pop(StackType.I4), target: PopPointer(), type: compilation.FindType(KnownTypeCode.Int16));
				case ILOpCode.Stind_i4:
					return new StObj(value: Pop(StackType.I4), target: PopPointer(), type: compilation.FindType(KnownTypeCode.Int32));
				case ILOpCode.Stind_i8:
					return new StObj(value: Pop(StackType.I8), target: PopPointer(), type: compilation.FindType(KnownTypeCode.Int64));
				case ILOpCode.Stind_r4:
					return new StObj(value: Pop(StackType.F4), target: PopPointer(), type: compilation.FindType(KnownTypeCode.Single));
				case ILOpCode.Stind_r8:
					return new StObj(value: Pop(StackType.F8), target: PopPointer(), type: compilation.FindType(KnownTypeCode.Double));
				case ILOpCode.Stind_i:
					return new StObj(value: Pop(StackType.I), target: PopPointer(), type: compilation.FindType(KnownTypeCode.IntPtr));
				case ILOpCode.Stind_ref:
					return new StObj(value: Pop(StackType.O), target: PopPointer(), type: compilation.FindType(KnownTypeCode.Object));
				case ILOpCode.Stloc:
				case ILOpCode.Stloc_s:
					return Stloc(ILParser.DecodeIndex(ref reader, opCode));
				case ILOpCode.Stloc_0:
					return Stloc(0);
				case ILOpCode.Stloc_1:
					return Stloc(1);
				case ILOpCode.Stloc_2:
					return Stloc(2);
				case ILOpCode.Stloc_3:
					return Stloc(3);
				case ILOpCode.Sub:
					return BinaryNumeric(BinaryNumericOperator.Sub, false, Sign.None);
				case ILOpCode.Sub_ovf:
					return BinaryNumeric(BinaryNumericOperator.Sub, true, Sign.Signed);
				case ILOpCode.Sub_ovf_un:
					return BinaryNumeric(BinaryNumericOperator.Sub, true, Sign.Unsigned);
				case ILOpCode.Switch:
					return DecodeSwitch();
				case ILOpCode.Xor:
					return BinaryNumeric(BinaryNumericOperator.BitXor);
				case ILOpCode.Box:
				{
					var type = ReadAndDecodeTypeReference();
					return Push(new Box(Pop(type.GetStackType()), type));
				}
				case ILOpCode.Castclass:
					return Push(new CastClass(Pop(StackType.O), ReadAndDecodeTypeReference()));
				case ILOpCode.Cpobj:
				{
					var type = ReadAndDecodeTypeReference();
					var ld = new LdObj(PopPointer(), type);
					return new StObj(PopPointer(), ld, type);
				}
				case ILOpCode.Initobj:
					return InitObj(PopPointer(), ReadAndDecodeTypeReference());
				case ILOpCode.Isinst:
					return Push(new IsInst(Pop(StackType.O), ReadAndDecodeTypeReference()));
				case ILOpCode.Ldelem:
					return LdElem(ReadAndDecodeTypeReference());
				case ILOpCode.Ldelem_i1:
					return LdElem(compilation.FindType(KnownTypeCode.SByte));
				case ILOpCode.Ldelem_i2:
					return LdElem(compilation.FindType(KnownTypeCode.Int16));
				case ILOpCode.Ldelem_i4:
					return LdElem(compilation.FindType(KnownTypeCode.Int32));
				case ILOpCode.Ldelem_i8:
					return LdElem(compilation.FindType(KnownTypeCode.Int64));
				case ILOpCode.Ldelem_u1:
					return LdElem(compilation.FindType(KnownTypeCode.Byte));
				case ILOpCode.Ldelem_u2:
					return LdElem(compilation.FindType(KnownTypeCode.UInt16));
				case ILOpCode.Ldelem_u4:
					return LdElem(compilation.FindType(KnownTypeCode.UInt32));
				case ILOpCode.Ldelem_r4:
					return LdElem(compilation.FindType(KnownTypeCode.Single));
				case ILOpCode.Ldelem_r8:
					return LdElem(compilation.FindType(KnownTypeCode.Double));
				case ILOpCode.Ldelem_i:
					return LdElem(compilation.FindType(KnownTypeCode.IntPtr));
				case ILOpCode.Ldelem_ref:
					return LdElem(compilation.FindType(KnownTypeCode.Object));
				case ILOpCode.Ldelema:
					return Push(new LdElema(indices: Pop(), array: Pop(), type: ReadAndDecodeTypeReference()));
				case ILOpCode.Ldfld:
				{
					var field = ReadAndDecodeFieldReference();
					return Push(new LdObj(new LdFlda(PopLdFldTarget(field), field) { DelayExceptions = true }, field.Type));
				}
				case ILOpCode.Ldflda:
				{
					var field = ReadAndDecodeFieldReference();
					return Push(new LdFlda(PopFieldTarget(field), field));
				}
				case ILOpCode.Stfld:
				{
					var field = ReadAndDecodeFieldReference();
					return new StObj(value: Pop(field.Type.GetStackType()), target: new LdFlda(PopFieldTarget(field), field) { DelayExceptions = true }, type: field.Type);
				}
				case ILOpCode.Ldlen:
					return Push(new LdLen(StackType.I, Pop(StackType.O)));
				case ILOpCode.Ldobj:
					return Push(new LdObj(PopPointer(), ReadAndDecodeTypeReference()));
				case ILOpCode.Ldsfld:
				{
					var field = ReadAndDecodeFieldReference();
					return Push(new LdObj(new LdsFlda(field), field.Type));
				}
				case ILOpCode.Ldsflda:
					return Push(new LdsFlda(ReadAndDecodeFieldReference()));
				case ILOpCode.Stsfld:
				{
					var field = ReadAndDecodeFieldReference();
					return new StObj(value: Pop(field.Type.GetStackType()), target: new LdsFlda(field), type: field.Type);
				}
				case ILOpCode.Ldtoken:
					return Push(LdToken(ReadAndDecodeMetadataToken()));
				case ILOpCode.Ldvirtftn:
					return Push(new LdVirtFtn(Pop(), ReadAndDecodeMethodReference()));
				case ILOpCode.Mkrefany:
					return Push(new MakeRefAny(PopPointer(), ReadAndDecodeTypeReference()));
				case ILOpCode.Newarr:
					return Push(new NewArr(ReadAndDecodeTypeReference(), Pop()));
				case ILOpCode.Refanytype:
					return Push(new RefAnyType(Pop()));
				case ILOpCode.Refanyval:
					return Push(new RefAnyValue(Pop(), ReadAndDecodeTypeReference()));
				case ILOpCode.Rethrow:
					return new Rethrow();
				case ILOpCode.Sizeof:
					return Push(new SizeOf(ReadAndDecodeTypeReference()));
				case ILOpCode.Stelem:
					return StElem(ReadAndDecodeTypeReference());
				case ILOpCode.Stelem_i1:
					return StElem(compilation.FindType(KnownTypeCode.SByte));
				case ILOpCode.Stelem_i2:
					return StElem(compilation.FindType(KnownTypeCode.Int16));
				case ILOpCode.Stelem_i4:
					return StElem(compilation.FindType(KnownTypeCode.Int32));
				case ILOpCode.Stelem_i8:
					return StElem(compilation.FindType(KnownTypeCode.Int64));
				case ILOpCode.Stelem_r4:
					return StElem(compilation.FindType(KnownTypeCode.Single));
				case ILOpCode.Stelem_r8:
					return StElem(compilation.FindType(KnownTypeCode.Double));
				case ILOpCode.Stelem_i:
					return StElem(compilation.FindType(KnownTypeCode.IntPtr));
				case ILOpCode.Stelem_ref:
					return StElem(compilation.FindType(KnownTypeCode.Object));
				case ILOpCode.Stobj:
				{
					var type = ReadAndDecodeTypeReference();
					return new StObj(value: Pop(type.GetStackType()), target: PopPointer(), type: type);
				}
				case ILOpCode.Throw:
					return new Throw(Pop());
				case ILOpCode.Unbox:
					return Push(new Unbox(Pop(), ReadAndDecodeTypeReference()));
				case ILOpCode.Unbox_any:
					return Push(new UnboxAny(Pop(), ReadAndDecodeTypeReference()));
				default:
					return new InvalidBranch($"Unknown opcode: 0x{(int)opCode:X2}");
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
				foreach (var child in inst.Children)
				{
					var newChild = child.AcceptVisitor(this);
					if (newChild != child)
						child.ReplaceWith(newChild);
				}
				return inst;
			}

			protected internal override ILInstruction VisitLdLoc(LdLoc inst)
			{
				base.VisitLdLoc(inst);
				if (inst.Variable.Kind == VariableKind.StackSlot)
				{
					var variable = unionFind.Find(inst.Variable);
					if (variables.Add(variable))
						variable.Name = "S_" + (variables.Count - 1);
					return new LdLoc(variable).WithILRange(inst);
				}
				return inst;
			}

			protected internal override ILInstruction VisitStLoc(StLoc inst)
			{
				base.VisitStLoc(inst);
				if (inst.Variable.Kind == VariableKind.StackSlot)
				{
					var variable = unionFind.Find(inst.Variable);
					if (variables.Add(variable))
						variable.Name = "S_" + (variables.Count - 1);
					return new StLoc(variable, inst.Value).WithILRange(inst);
				}
				return inst;
			}
		}

		ILInstruction Push(ILInstruction inst)
		{
			Debug.Assert(inst.ResultType != StackType.Void);
			IType type = compilation.FindType(inst.ResultType);
			var v = new ILVariable(VariableKind.StackSlot, type, inst.ResultType);
			v.HasGeneratedName = true;
			currentStack = currentStack.Push(v);
			return new StLoc(v, inst);
		}

		ILInstruction Peek()
		{
			if (currentStack.IsEmpty)
			{
				return new InvalidExpression("Stack underflow").WithILRange(new Interval(reader.Offset, reader.Offset));
			}
			return new LdLoc(currentStack.Peek());
		}

		ILInstruction Pop()
		{
			if (currentStack.IsEmpty)
			{
				return new InvalidExpression("Stack underflow").WithILRange(new Interval(reader.Offset, reader.Offset));
			}
			ILVariable v;
			currentStack = currentStack.Pop(out v);
			return new LdLoc(v);
		}

		ILInstruction Pop(StackType expectedType)
		{
			ILInstruction inst = Pop();
			return Cast(inst, expectedType, Warnings, reader.Offset);
		}

		internal static ILInstruction Cast(ILInstruction inst, StackType expectedType, List<string> warnings, int ilOffset)
		{
			if (expectedType != inst.ResultType)
			{
				if (inst is InvalidExpression)
				{
					((InvalidExpression)inst).ExpectedResultType = expectedType;
				}
				else if (expectedType == StackType.I && inst.ResultType == StackType.I4)
				{
					// IL allows implicit I4->I conversions
					inst = new Conv(inst, PrimitiveType.I, false, Sign.None);
				}
				else if (expectedType == StackType.I4 && inst.ResultType == StackType.I)
				{
					// C++/CLI also sometimes implicitly converts in the other direction:
					inst = new Conv(inst, PrimitiveType.I4, false, Sign.None);
				}
				else if (expectedType == StackType.Unknown)
				{
					inst = new Conv(inst, PrimitiveType.Unknown, false, Sign.None);
				}
				else if (inst.ResultType == StackType.Ref)
				{
					// Implicitly stop GC tracking; this occurs when passing the result of 'ldloca' or 'ldsflda'
					// to a method expecting a native pointer.
					inst = new Conv(inst, PrimitiveType.I, false, Sign.None);
					switch (expectedType)
					{
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
				}
				else if (expectedType == StackType.Ref)
				{
					// implicitly start GC tracking / object to interior
					if (!inst.ResultType.IsIntegerType() && inst.ResultType != StackType.O)
					{
						// We also handle the invalid to-ref cases here because the else case
						// below uses expectedType.ToKnownTypeCode(), which doesn't work for Ref.
						Warn($"Expected {expectedType}, but got {inst.ResultType}");
					}
					inst = new Conv(inst, PrimitiveType.Ref, false, Sign.None);
				}
				else if (expectedType == StackType.F8 && inst.ResultType == StackType.F4)
				{
					// IL allows implicit F4->F8 conversions, because in IL F4 and F8 are the same.
					inst = new Conv(inst, PrimitiveType.R8, false, Sign.Signed);
				}
				else if (expectedType == StackType.F4 && inst.ResultType == StackType.F8)
				{
					// IL allows implicit F8->F4 conversions, because in IL F4 and F8 are the same.
					inst = new Conv(inst, PrimitiveType.R4, false, Sign.Signed);
				}
				else
				{
					Warn($"Expected {expectedType}, but got {inst.ResultType}");
					inst = new Conv(inst, expectedType.ToPrimitiveType(), false, Sign.Signed);
				}
			}
			return inst;

			void Warn(string message)
			{
				if (warnings != null)
				{
					warnings.Add(string.Format("IL_{0:x4}: {1}", ilOffset, message));
				}
			}
		}

		ILInstruction PopPointer()
		{
			ILInstruction inst = Pop();
			switch (inst.ResultType)
			{
				case StackType.I4:
				case StackType.I8:
				case StackType.Unknown:
					return new Conv(inst, PrimitiveType.I, false, Sign.None);
				case StackType.I:
				case StackType.Ref:
					return inst;
				default:
					Warn("Expected native int or pointer, but got " + inst.ResultType);
					return new Conv(inst, PrimitiveType.I, false, Sign.None);
			}
		}

		ILInstruction PopFieldTarget(IField field)
		{
			switch (field.DeclaringType.IsReferenceType)
			{
				case true:
					return Pop(StackType.O);
				case false:
					return PopPointer();
				default:
					// field in unresolved type
					var stackType = PeekStackType();
					if (stackType == StackType.O || stackType == StackType.Unknown)
						return Pop();
					else
						return PopPointer();
			}
		}

		/// <summary>
		/// Like PopFieldTarget, but supports ldfld's special behavior for fields of temporary value types.
		/// </summary>
		ILInstruction PopLdFldTarget(IField field)
		{
			switch (field.DeclaringType.IsReferenceType)
			{
				case true:
					return Pop(StackType.O);
				case false:
					// field of value type: ldfld can handle temporaries
					if (PeekStackType() == StackType.O || PeekStackType() == StackType.Unknown)
						return new AddressOf(Pop(), field.DeclaringType);
					else
						return PopPointer();
				default:
					// field in unresolved type
					if (PeekStackType() == StackType.O || PeekStackType() == StackType.Unknown)
						return Pop();
					else
						return PopPointer();
			}
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
			return new LdStr(ILParser.DecodeUserString(ref reader, metadata));
		}

		private ILInstruction Ldarg(int v)
		{
			if (v >= 0 && v < parameterVariables.Length)
			{
				return new LdLoc(parameterVariables[v]);
			}
			else
			{
				return new InvalidExpression($"ldarg {v} (out-of-bounds)");
			}
		}

		private ILInstruction Ldarga(int v)
		{
			if (v >= 0 && v < parameterVariables.Length)
			{
				return new LdLoca(parameterVariables[v]);
			}
			else
			{
				return new InvalidExpression($"ldarga {v} (out-of-bounds)");
			}
		}

		private ILInstruction Starg(int v)
		{
			if (v >= 0 && v < parameterVariables.Length)
			{
				return new StLoc(parameterVariables[v], Pop(parameterVariables[v].StackType));
			}
			else
			{
				Pop();
				return new InvalidExpression($"starg {v} (out-of-bounds)");
			}
		}

		private ILInstruction Ldloc(int v)
		{
			if (v >= 0 && v < localVariables.Length)
			{
				return new LdLoc(localVariables[v]);
			}
			else
			{
				return new InvalidExpression($"ldloc {v} (out-of-bounds)");
			}
		}

		private ILInstruction Ldloca(int v)
		{
			if (v >= 0 && v < localVariables.Length)
			{
				return new LdLoca(localVariables[v]);
			}
			else
			{
				return new InvalidExpression($"ldloca {v} (out-of-bounds)");
			}
		}

		private ILInstruction Stloc(int v)
		{
			if (v >= 0 && v < localVariables.Length)
			{
				return new StLoc(localVariables[v], Pop(localVariables[v].StackType)) {
					ILStackWasEmpty = currentStack.IsEmpty
				};
			}
			else
			{
				Pop();
				return new InvalidExpression($"stloc {v} (out-of-bounds)");
			}
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
			byte alignment = reader.ReadByte();
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
			for (int i = method.Parameters.Count - 1; i >= 0; i--)
			{
				arguments[firstArgument + i] = Pop(method.Parameters[i].Type.GetStackType());
			}
			if (firstArgument == 1)
			{
				arguments[0] = Pop(CallInstruction.ExpectedTypeForThisPointer(constrainedPrefix ?? method.DeclaringType));
			}
			switch (method.DeclaringType.Kind)
			{
				case TypeKind.Array:
				{
					var elementType = ((ArrayType)method.DeclaringType).ElementType;
					if (opCode == OpCode.NewObj)
						return Push(new NewArr(elementType, arguments));
					if (method.Name == "Set")
					{
						var target = arguments[0];
						var value = arguments.Last();
						var indices = arguments.Skip(1).Take(arguments.Length - 2).ToArray();
						return new StObj(new LdElema(elementType, target, indices) { DelayExceptions = true }, value, elementType);
					}
					if (method.Name == "Get")
					{
						var target = arguments[0];
						var indices = arguments.Skip(1).ToArray();
						return Push(new LdObj(new LdElema(elementType, target, indices) { DelayExceptions = true }, elementType));
					}
					if (method.Name == "Address")
					{
						var target = arguments[0];
						var indices = arguments.Skip(1).ToArray();
						return Push(new LdElema(elementType, target, indices));
					}
					Warn("Unknown method called on array type: " + method.Name);
					goto default;
				}
				case TypeKind.Struct when method.IsConstructor && !method.IsStatic && opCode == OpCode.Call
					&& method.ReturnType.Kind == TypeKind.Void:
				{
					// "call Struct.ctor(target, ...)" doesn't exist in C#,
					// the next best equivalent is an assignment `*target = new Struct(...);`.
					// So we represent this call as "stobj Struct(target, newobj Struct.ctor(...))".
					// This needs to happen early (not as a transform) because the StObj.TargetSlot has
					// restricted inlining (doesn't accept ldflda when exceptions aren't delayed).
					var newobj = new NewObj(method);
					newobj.ILStackWasEmpty = currentStack.IsEmpty;
					newobj.ConstrainedTo = constrainedPrefix;
					newobj.Arguments.AddRange(arguments.Skip(1));
					return new StObj(arguments[0], newobj, method.DeclaringType);
				}
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
			var signatureHandle = (StandaloneSignatureHandle)ReadAndDecodeMetadataToken();
			var (header, fpt) = module.DecodeMethodSignature(signatureHandle, genericContext);
			var functionPointer = Pop(StackType.I);
			int firstArgument = header.IsInstance ? 1 : 0;
			var arguments = new ILInstruction[firstArgument + fpt.ParameterTypes.Length];
			for (int i = fpt.ParameterTypes.Length - 1; i >= 0; i--)
			{
				arguments[firstArgument + i] = Pop(fpt.ParameterTypes[i].GetStackType());
			}
			if (firstArgument == 1)
			{
				arguments[0] = Pop();
			}
			var call = new CallIndirect(
				header.IsInstance,
				header.HasExplicitThis,
				fpt,
				functionPointer,
				arguments
			);
			if (call.ResultType != StackType.Void)
				return Push(call);
			else
				return call;
		}

		ILInstruction Comparison(ComparisonKind kind, bool un = false)
		{
			var right = Pop();
			var left = Pop();

			if ((left.ResultType == StackType.O || left.ResultType == StackType.Ref) && right.ResultType.IsIntegerType())
			{
				// C++/CLI sometimes compares object references with integers.
				// Also happens with Ref==I in Unsafe.IsNullRef().
				if (right.ResultType == StackType.I4)
				{
					// ensure we compare at least native integer size
					right = new Conv(right, PrimitiveType.I, false, Sign.None);
				}
				left = new Conv(left, right.ResultType.ToPrimitiveType(), false, Sign.None);
			}
			else if ((right.ResultType == StackType.O || right.ResultType == StackType.Ref) && left.ResultType.IsIntegerType())
			{
				if (left.ResultType == StackType.I4)
				{
					left = new Conv(left, PrimitiveType.I, false, Sign.None);
				}
				right = new Conv(right, left.ResultType.ToPrimitiveType(), false, Sign.None);
			}

			// make implicit integer conversions explicit:
			MakeExplicitConversion(sourceType: StackType.I4, targetType: StackType.I, conversionType: PrimitiveType.I);
			MakeExplicitConversion(sourceType: StackType.I4, targetType: StackType.I8, conversionType: PrimitiveType.I8);
			MakeExplicitConversion(sourceType: StackType.I, targetType: StackType.I8, conversionType: PrimitiveType.I8);

			// Based on Table 4: Binary Comparison or Branch Operation
			if (left.ResultType.IsFloatType() && right.ResultType.IsFloatType())
			{
				if (left.ResultType != right.ResultType)
				{
					// make the implicit F4->F8 conversion explicit:
					MakeExplicitConversion(StackType.F4, StackType.F8, PrimitiveType.R8);
				}
				if (un)
				{
					// for floats, 'un' means 'unordered'
					return Comp.LogicNot(new Comp(kind.Negate(), Sign.None, left, right));
				}
				else
				{
					return new Comp(kind, Sign.None, left, right);
				}
			}
			else if (left.ResultType.IsIntegerType() && right.ResultType.IsIntegerType() && !kind.IsEqualityOrInequality())
			{
				// integer comparison where the sign matters
				Debug.Assert(right.ResultType.IsIntegerType());
				return new Comp(kind, un ? Sign.Unsigned : Sign.Signed, left, right);
			}
			else if (left.ResultType == right.ResultType)
			{
				// integer equality, object reference or managed reference comparison
				return new Comp(kind, Sign.None, left, right);
			}
			else
			{
				Warn($"Invalid comparison between {left.ResultType} and {right.ResultType}");
				if (left.ResultType < right.ResultType)
				{
					left = new Conv(left, right.ResultType.ToPrimitiveType(), false, Sign.Signed);
				}
				else
				{
					right = new Conv(right, left.ResultType.ToPrimitiveType(), false, Sign.Signed);
				}
				return new Comp(kind, Sign.None, left, right);
			}

			void MakeExplicitConversion(StackType sourceType, StackType targetType, PrimitiveType conversionType)
			{
				if (left.ResultType == sourceType && right.ResultType == targetType)
				{
					left = new Conv(left, conversionType, false, Sign.None);
				}
				else if (left.ResultType == targetType && right.ResultType == sourceType)
				{
					right = new Conv(right, conversionType, false, Sign.None);
				}
			}
		}

		bool IsInvalidBranch(int target) => target < 0 || target >= reader.Length;

		ILInstruction DecodeComparisonBranch(ILOpCode opCode, ComparisonKind kind, bool un = false)
		{
			int start = reader.Offset - 1; // opCode is always one byte in this case
			int target = ILParser.DecodeBranchTarget(ref reader, opCode);
			var condition = Comparison(kind, un);
			condition.AddILRange(new Interval(start, reader.Offset));
			if (!IsInvalidBranch(target))
			{
				MarkBranchTarget(target);
				return new IfInstruction(condition, new Branch(target));
			}
			else
			{
				return new IfInstruction(condition, new InvalidBranch("Invalid branch target"));
			}
		}

		ILInstruction DecodeConditionalBranch(ILOpCode opCode, bool negate)
		{
			int target = ILParser.DecodeBranchTarget(ref reader, opCode);
			ILInstruction condition = Pop();
			switch (condition.ResultType)
			{
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
					if (negate)
					{
						condition = Comp.LogicNot(condition);
					}
					break;
				default:
					condition = new Conv(condition, PrimitiveType.I4, false, Sign.None);
					if (negate)
					{
						condition = Comp.LogicNot(condition);
					}
					break;
			}
			if (!IsInvalidBranch(target))
			{
				MarkBranchTarget(target);
				return new IfInstruction(condition, new Branch(target));
			}
			else
			{
				return new IfInstruction(condition, new InvalidBranch("Invalid branch target"));
			}
		}

		ILInstruction DecodeUnconditionalBranch(ILOpCode opCode, bool isLeave = false)
		{
			int target = ILParser.DecodeBranchTarget(ref reader, opCode);
			if (isLeave)
			{
				currentStack = currentStack.Clear();
			}
			if (!IsInvalidBranch(target))
			{
				MarkBranchTarget(target);
				return new Branch(target);
			}
			else
			{
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
			var targets = ILParser.DecodeSwitchTargets(ref reader);
			var instr = new SwitchInstruction(Pop(StackType.I4));

			for (int i = 0; i < targets.Length; i++)
			{
				var section = new SwitchSection();
				section.Labels = new LongSet(i);
				int target = targets[i];
				if (!IsInvalidBranch(target))
				{
					MarkBranchTarget(target);
					section.Body = new Branch(target);
				}
				else
				{
					section.Body = new InvalidBranch("Invalid branch target");
				}
				instr.Sections.Add(section);
			}
			var defaultSection = new SwitchSection();
			defaultSection.Labels = new LongSet(new LongInterval(0, targets.Length)).Invert();
			defaultSection.Body = new Nop();
			instr.Sections.Add(defaultSection);
			return instr;
		}

		ILInstruction BinaryNumeric(BinaryNumericOperator @operator, bool checkForOverflow = false, Sign sign = Sign.None)
		{
			var right = Pop();
			var left = Pop();
			if (@operator != BinaryNumericOperator.Add && @operator != BinaryNumericOperator.Sub)
			{
				// we are treating all Refs as I, make the conversion explicit
				if (left.ResultType == StackType.Ref)
				{
					left = new Conv(left, PrimitiveType.I, false, Sign.None);
				}
				if (right.ResultType == StackType.Ref)
				{
					right = new Conv(right, PrimitiveType.I, false, Sign.None);
				}
			}
			if (@operator != BinaryNumericOperator.ShiftLeft && @operator != BinaryNumericOperator.ShiftRight)
			{
				// make the implicit I4->I conversion explicit:
				MakeExplicitConversion(sourceType: StackType.I4, targetType: StackType.I, conversionType: PrimitiveType.I);
				// I4->I8 conversion:
				MakeExplicitConversion(sourceType: StackType.I4, targetType: StackType.I8, conversionType: PrimitiveType.I8);
				// I->I8 conversion:
				MakeExplicitConversion(sourceType: StackType.I, targetType: StackType.I8, conversionType: PrimitiveType.I8);
				// F4->F8 conversion:
				MakeExplicitConversion(sourceType: StackType.F4, targetType: StackType.F8, conversionType: PrimitiveType.R8);
			}
			return Push(new BinaryNumericInstruction(@operator, left, right, checkForOverflow, sign));

			void MakeExplicitConversion(StackType sourceType, StackType targetType, PrimitiveType conversionType)
			{
				if (left.ResultType == sourceType && right.ResultType == targetType)
				{
					left = new Conv(left, conversionType, false, Sign.None);
				}
				else if (left.ResultType == targetType && right.ResultType == sourceType)
				{
					right = new Conv(right, conversionType, false, Sign.None);
				}
			}
		}

		ILInstruction DecodeJmp()
		{
			IMethod method = ReadAndDecodeMethodReference();
			// Translate jmp into tail call:
			Call call = new Call(method);
			call.IsTail = true;
			call.ILStackWasEmpty = true;
			if (!method.IsStatic)
			{
				call.Arguments.Add(Ldarg(0));
			}
			foreach (var p in method.Parameters)
			{
				call.Arguments.Add(Ldarg(call.Arguments.Count));
			}
			return new Leave(mainContainer, call);
		}

		ILInstruction LdToken(EntityHandle token)
		{
			if (token.Kind.IsTypeKind())
				return new LdTypeToken(module.ResolveType(token, genericContext));
			if (token.Kind.IsMemberKind())
			{
				var entity = module.ResolveEntity(token, genericContext);
				if (entity is IMember member)
					return new LdMemberToken(member);
			}
			throw new BadImageFormatException("Invalid metadata token for ldtoken instruction.");
		}
	}
}
