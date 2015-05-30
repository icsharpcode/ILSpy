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
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;
using System.Diagnostics;
using ICSharpCode.NRefactory.TypeSystem;
using Mono.Cecil;
using Cil = Mono.Cecil.Cil;
using System.Collections;
using System.Threading;

namespace ICSharpCode.Decompiler.IL
{
	public class ILReader
	{
		internal static ILOpCode ReadOpCode(ref BlobReader reader)
		{
			byte b = reader.ReadByte();
			if (b == 0xfe)
				return (ILOpCode)(0x100 | reader.ReadByte());
			else
				return (ILOpCode)b;
		}

		internal static MetadataToken ReadMetadataToken(ref BlobReader reader)
		{
			return new MetadataToken(reader.ReadUInt32());
		}
		
		readonly ICompilation compilation;
		readonly IDecompilerTypeSystem typeSystem;

		public ILReader(IDecompilerTypeSystem typeSystem)
		{
			if (typeSystem == null)
				throw new ArgumentNullException("typeSystem");
			this.typeSystem = typeSystem;
			this.compilation = typeSystem.Compilation;
		}

		Cil.MethodBody body;
		BlobReader reader;
		Stack<StackType> stack;
		ILVariable[] parameterVariables;
		ILVariable[] localVariables;
		BitArray isBranchTarget;
		List<ILInstruction> instructionBuilder;

		// Dictionary that stores stacks for forward jumps
		Dictionary<int, ImmutableArray<StackType>> branchStackDict;
		
		void Init(Cil.MethodBody body)
		{
			if (body == null)
				throw new ArgumentNullException("body");
			this.body = body;
			this.reader = body.GetILReader();
			this.stack = new Stack<StackType>(body.MaxStackSize);
			InitParameterVariables();
			this.localVariables = body.Variables.SelectArray(CreateILVariable);
			this.instructionBuilder = new List<ILInstruction>();
			this.isBranchTarget = new BitArray(body.CodeSize);
			this.branchStackDict = new Dictionary<int, ImmutableArray<StackType>>();
		}

		IMetadataTokenProvider ReadAndDecodeMetadataToken()
		{
			var token = ReadMetadataToken(ref reader);
			return body.LookupToken(token);
		}

		IType ReadAndDecodeTypeReference()
		{
			var token = ReadMetadataToken(ref reader);
			var typeReference = body.LookupToken(token) as TypeReference;
			return typeSystem.Resolve(typeReference);
		}

		IMethod ReadAndDecodeMethodReference()
		{
			var token = ReadMetadataToken(ref reader);
			var methodReference = body.LookupToken(token) as MethodReference;
			return typeSystem.Resolve(methodReference);
		}

		IField ReadAndDecodeFieldReference()
		{
			var token = ReadMetadataToken(ref reader);
			var fieldReference = body.LookupToken(token) as FieldReference;
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
			var ilVar = new ILVariable(VariableKind.Local, typeSystem.Resolve(v.VariableType), v.Index);
			if (string.IsNullOrEmpty(v.Name))
				ilVar.Name = "V_" + v.Index;
			else
				ilVar.Name = v.Name;
			return ilVar;
		}

		ILVariable CreateILVariable(ParameterDefinition p)
		{
			var variableKind = p.Index == -1 ? VariableKind.This : VariableKind.Parameter;
			var ilVar = new ILVariable(variableKind, typeSystem.Resolve(p.ParameterType), p.Index);
			ilVar.StoreCount = 1; // count the initial store when the method is called with an argument
			if (variableKind == VariableKind.This)
				ilVar.Name = "this";
			else if (string.IsNullOrEmpty(p.Name))
				ilVar.Name = "P_" + p.Index;
			else
				ilVar.Name = p.Name;
			return ilVar;
		}
		
		/// <summary>
		/// Warn when invalid IL is detected.
		/// ILSpy should be able to handle invalid IL; but this method can be helpful for debugging the ILReader, as this method should not get called when processing valid IL.
		/// </summary>
		void Warn(string message)
		{
			Debug.Fail(message);
		}

		void ReadInstructions(Dictionary<int, ImmutableArray<StackType>> outputStacks, CancellationToken cancellationToken)
		{
			// Fill isBranchTarget and branchStackDict based on exception handlers
			foreach (var eh in body.ExceptionHandlers) {
				if (eh.FilterStart != null) {
					isBranchTarget[eh.FilterStart.Offset] = true;
					branchStackDict[eh.FilterStart.Offset] = ImmutableArray.Create(eh.CatchType.GetStackType());
				}
				if (eh.HandlerStart != null) {
					isBranchTarget[eh.HandlerStart.Offset] = true;
					if (eh.HandlerType == Cil.ExceptionHandlerType.Catch || eh.HandlerType == Cil.ExceptionHandlerType.Filter)
						branchStackDict[eh.HandlerStart.Offset] = ImmutableArray.Create(eh.CatchType.GetStackType());
					else
						branchStackDict[eh.HandlerStart.Offset] = ImmutableArray<StackType>.Empty;
				}
			}
			
			while (reader.Position < reader.Length) {
				cancellationToken.ThrowIfCancellationRequested();
				int start = reader.Position;
				if (outputStacks != null)
					outputStacks.Add(start, stack.ToImmutableArray());
				ILInstruction decodedInstruction = DecodeInstruction();
				if (decodedInstruction.ResultType == StackType.Unknown)
					Warn("Unknown result type (might be due to invalid IL)");
				decodedInstruction.CheckInvariant();
				if (decodedInstruction.ResultType != StackType.Void)
					stack.Push(decodedInstruction.ResultType);
				decodedInstruction.ILRange = new Interval(start, reader.Position);
				instructionBuilder.Add(decodedInstruction);
				if (decodedInstruction.HasFlag(InstructionFlags.EndPointUnreachable)) {
					stack.Clear();
					ImmutableArray<StackType> stackFromBranch;
					if (branchStackDict.TryGetValue(reader.Position, out stackFromBranch)) {
						for (int i = stackFromBranch.Length - 1; i >= 0; i--) {
							stack.Push(stackFromBranch[i]);
						}
					}
				}
			}
		}

		/// <summary>
		/// Debugging helper: writes the decoded instruction stream interleaved with the inferred evaluation stack layout.
		/// </summary>
		public void WriteTypedIL(Cil.MethodBody body, ITextOutput output, CancellationToken cancellationToken = default(CancellationToken))
		{
			Init(body);
			var outputStacks = new Dictionary<int, ImmutableArray<StackType>>();
			ReadInstructions(outputStacks, cancellationToken);
			foreach (var inst in instructionBuilder) {
				output.Write("   [");
				bool isFirstElement = true;
				foreach (var element in outputStacks[inst.ILRange.Start]) {
					if (isFirstElement)
						isFirstElement = false;
					else
						output.Write(", ");
					output.Write(element);
				}
				output.Write(']');
				output.WriteLine();
				if (isBranchTarget[inst.ILRange.Start])
					output.Write('*');
				else
					output.Write(' ');
				output.WriteDefinition("IL_" + inst.ILRange.Start.ToString("x4"), inst.ILRange.Start);
				output.Write(": ");
				inst.WriteTo(output);
				output.WriteLine();
			}
			new Disassembler.MethodBodyDisassembler(output, false, cancellationToken).WriteExceptionHandlers(body);
		}

		/// <summary>
		/// Decodes the specified method body and returns an ILFunction.
		/// </summary>
		public ILFunction ReadIL(Cil.MethodBody body, CancellationToken cancellationToken = default(CancellationToken))
		{
			Init(body);
			ReadInstructions(null, cancellationToken);
			var container = new BlockBuilder(body, typeSystem).CreateBlocks(instructionBuilder, isBranchTarget);
			var function = new ILFunction(body.Method, container);
			function.Variables.AddRange(parameterVariables);
			function.Variables.AddRange(localVariables);
			function.AddRef(); // mark the root node
			return function;
		}

		ILInstruction Neg()
		{
			switch (stack.PeekOrDefault()) {
				case StackType.I4:
				case StackType.I:
					return new Sub(new LdcI4(0), Pop(), checkForOverflow: false, sign: Sign.None);
				case StackType.I8:
					return new Sub(new LdcI8(0), Pop(), checkForOverflow: false, sign: Sign.None);
				case StackType.F:
					return new Sub(new LdcF(0), Pop(), checkForOverflow: false, sign: Sign.None);
				default:
					Warn("Unsupported input type for neg: ");
					goto case StackType.I4;
			}
		}
		
		ILInstruction DecodeInstruction()
		{
			var ilOpCode = ReadOpCode(ref reader);
			switch (ilOpCode) {
				case ILOpCode.Constrained:
					return DecodeConstrainedCall();
				case ILOpCode.Readonly:
					return DecodeReadonly();
				case ILOpCode.Tailcall:
					return DecodeTailCall();
				case ILOpCode.Unaligned:
					return DecodeUnaligned();
				case ILOpCode.Volatile:
					return DecodeVolatile();
				case ILOpCode.Add:
					return BinaryNumeric(OpCode.Add);
				case ILOpCode.Add_Ovf:
					return BinaryNumeric(OpCode.Add, true, Sign.Signed);
				case ILOpCode.Add_Ovf_Un:
					return BinaryNumeric(OpCode.Add, true, Sign.Unsigned);
				case ILOpCode.And:
					return BinaryNumeric(OpCode.BitAnd);
				case ILOpCode.Arglist:
					return new Arglist();
				case ILOpCode.Beq:
					return DecodeComparisonBranch(false, OpCode.Ceq, OpCode.Ceq, false);
				case ILOpCode.Beq_S:
					return DecodeComparisonBranch(true, OpCode.Ceq, OpCode.Ceq, false);
				case ILOpCode.Bge:
					return DecodeComparisonBranch(false, OpCode.Clt, OpCode.Clt_Un, true);
				case ILOpCode.Bge_S:
					return DecodeComparisonBranch(true, OpCode.Clt, OpCode.Clt_Un, true);
				case ILOpCode.Bge_Un:
					return DecodeComparisonBranch(false, OpCode.Clt_Un, OpCode.Clt, true);
				case ILOpCode.Bge_Un_S:
					return DecodeComparisonBranch(true, OpCode.Clt_Un, OpCode.Clt, true);
				case ILOpCode.Bgt:
					return DecodeComparisonBranch(false, OpCode.Cgt, OpCode.Cgt, false);
				case ILOpCode.Bgt_S:
					return DecodeComparisonBranch(true, OpCode.Cgt, OpCode.Cgt, false);
				case ILOpCode.Bgt_Un:
					return DecodeComparisonBranch(false, OpCode.Cgt_Un, OpCode.Clt_Un, false);
				case ILOpCode.Bgt_Un_S:
					return DecodeComparisonBranch(true, OpCode.Cgt_Un, OpCode.Clt_Un, false);
				case ILOpCode.Ble:
					return DecodeComparisonBranch(false, OpCode.Cgt, OpCode.Cgt_Un, true);
				case ILOpCode.Ble_S:
					return DecodeComparisonBranch(true, OpCode.Cgt, OpCode.Cgt_Un, true);
				case ILOpCode.Ble_Un:
					return DecodeComparisonBranch(false, OpCode.Cgt_Un, OpCode.Cgt, true);
				case ILOpCode.Ble_Un_S:
					return DecodeComparisonBranch(true, OpCode.Cgt_Un, OpCode.Cgt, true);
				case ILOpCode.Blt:
					return DecodeComparisonBranch(false, OpCode.Clt, OpCode.Clt, false);
				case ILOpCode.Blt_S:
					return DecodeComparisonBranch(true, OpCode.Clt, OpCode.Clt, false);
				case ILOpCode.Blt_Un:
					return DecodeComparisonBranch(false, OpCode.Clt_Un, OpCode.Clt_Un, false);
				case ILOpCode.Blt_Un_S:
					return DecodeComparisonBranch(true, OpCode.Clt_Un, OpCode.Clt_Un, false);
				case ILOpCode.Bne_Un:
					return DecodeComparisonBranch(false, OpCode.Ceq, OpCode.Ceq, true);
				case ILOpCode.Bne_Un_S:
					return DecodeComparisonBranch(true, OpCode.Ceq, OpCode.Ceq, true);
				case ILOpCode.Br:
					return DecodeUnconditionalBranch(false);
				case ILOpCode.Br_S:
					return DecodeUnconditionalBranch(true);
				case ILOpCode.Break:
					return new DebugBreak();
				case ILOpCode.Brfalse:
					return DecodeConditionalBranch(false, true);
				case ILOpCode.Brfalse_S:
					return DecodeConditionalBranch(true, true);
				case ILOpCode.Brtrue:
					return DecodeConditionalBranch(false, false);
				case ILOpCode.Brtrue_S:
					return DecodeConditionalBranch(true, false);
				case ILOpCode.Call:
					return DecodeCall(OpCode.Call);
				case ILOpCode.Callvirt:
					return DecodeCall(OpCode.CallVirt);
				case ILOpCode.Calli:
					throw new NotImplementedException();
				case ILOpCode.Ceq:
					return Comparison(OpCode.Ceq, OpCode.Ceq);
				case ILOpCode.Cgt:
					return Comparison(OpCode.Cgt, OpCode.Cgt);
				case ILOpCode.Cgt_Un:
					return Comparison(OpCode.Cgt_Un, OpCode.Cgt_Un);
				case ILOpCode.Clt:
					return Comparison(OpCode.Clt, OpCode.Clt);
				case ILOpCode.Clt_Un:
					return Comparison(OpCode.Clt_Un, OpCode.Clt_Un);
				case ILOpCode.Ckfinite:
					return new Ckfinite(new Peek(stack.Count > 0 ? stack.Peek() : StackType.Unknown));
				case ILOpCode.Conv_I1:
					return new Conv(Pop(), PrimitiveType.I1, false, Sign.None);
				case ILOpCode.Conv_I2:
					return new Conv(Pop(), PrimitiveType.I2, false, Sign.None);
				case ILOpCode.Conv_I4:
					return new Conv(Pop(), PrimitiveType.I4, false, Sign.None);
				case ILOpCode.Conv_I8:
					return new Conv(Pop(), PrimitiveType.I8, false, Sign.None);
				case ILOpCode.Conv_R4:
					return new Conv(Pop(), PrimitiveType.R4, false, Sign.Signed);
				case ILOpCode.Conv_R8:
					return new Conv(Pop(), PrimitiveType.R8, false, Sign.Signed);
				case ILOpCode.Conv_U1:
					return new Conv(Pop(), PrimitiveType.U1, false, Sign.None);
				case ILOpCode.Conv_U2:
					return new Conv(Pop(), PrimitiveType.U2, false, Sign.None);
				case ILOpCode.Conv_U4:
					return new Conv(Pop(), PrimitiveType.U4, false, Sign.None);
				case ILOpCode.Conv_U8:
					return new Conv(Pop(), PrimitiveType.U8, false, Sign.None);
				case ILOpCode.Conv_I:
					return new Conv(Pop(), PrimitiveType.I, false, Sign.None);
				case ILOpCode.Conv_U:
					return new Conv(Pop(), PrimitiveType.U, false, Sign.None);
				case ILOpCode.Conv_R_Un:
					return new Conv(Pop(), PrimitiveType.R8, false, Sign.Unsigned);
				case ILOpCode.Conv_Ovf_I1:
					return new Conv(Pop(), PrimitiveType.I1, true, Sign.Signed);
				case ILOpCode.Conv_Ovf_I2:
					return new Conv(Pop(), PrimitiveType.I2, true, Sign.Signed);
				case ILOpCode.Conv_Ovf_I4:
					return new Conv(Pop(), PrimitiveType.I4, true, Sign.Signed);
				case ILOpCode.Conv_Ovf_I8:
					return new Conv(Pop(), PrimitiveType.I8, true, Sign.Signed);
				case ILOpCode.Conv_Ovf_U1:
					return new Conv(Pop(), PrimitiveType.U1, true, Sign.Signed);
				case ILOpCode.Conv_Ovf_U2:
					return new Conv(Pop(), PrimitiveType.U2, true, Sign.Signed);
				case ILOpCode.Conv_Ovf_U4:
					return new Conv(Pop(), PrimitiveType.U4, true, Sign.Signed);
				case ILOpCode.Conv_Ovf_U8:
					return new Conv(Pop(), PrimitiveType.U8, true, Sign.Signed);
				case ILOpCode.Conv_Ovf_I:
					return new Conv(Pop(), PrimitiveType.I, true, Sign.Signed);
				case ILOpCode.Conv_Ovf_U:
					return new Conv(Pop(), PrimitiveType.U, true, Sign.Signed);
				case ILOpCode.Conv_Ovf_I1_Un:
					return new Conv(Pop(), PrimitiveType.I1, true, Sign.Unsigned);
				case ILOpCode.Conv_Ovf_I2_Un:
					return new Conv(Pop(), PrimitiveType.I2, true, Sign.Unsigned);
				case ILOpCode.Conv_Ovf_I4_Un:
					return new Conv(Pop(), PrimitiveType.I4, true, Sign.Unsigned);
				case ILOpCode.Conv_Ovf_I8_Un:
					return new Conv(Pop(), PrimitiveType.I8, true, Sign.Unsigned);
				case ILOpCode.Conv_Ovf_U1_Un:
					return new Conv(Pop(), PrimitiveType.U1, true, Sign.Unsigned);
				case ILOpCode.Conv_Ovf_U2_Un:
					return new Conv(Pop(), PrimitiveType.U2, true, Sign.Unsigned);
				case ILOpCode.Conv_Ovf_U4_Un:
					return new Conv(Pop(), PrimitiveType.U4, true, Sign.Unsigned);
				case ILOpCode.Conv_Ovf_U8_Un:
					return new Conv(Pop(), PrimitiveType.U8, true, Sign.Unsigned);
				case ILOpCode.Conv_Ovf_I_Un:
					return new Conv(Pop(), PrimitiveType.I, true, Sign.Unsigned);
				case ILOpCode.Conv_Ovf_U_Un:
					return new Conv(Pop(), PrimitiveType.U, true, Sign.Unsigned);
				case ILOpCode.Cpblk:
					throw new NotImplementedException();
				case ILOpCode.Div:
					return BinaryNumeric(OpCode.Div, false, Sign.Signed);
				case ILOpCode.Div_Un:
					return BinaryNumeric(OpCode.Div, false, Sign.Unsigned);
				case ILOpCode.Dup:
					return new Peek(stack.Count > 0 ? stack.Peek() : StackType.Unknown);
				case ILOpCode.Endfilter:
				case ILOpCode.Endfinally:
					return new Leave(null);
				case ILOpCode.Initblk:
					throw new NotImplementedException();
				case ILOpCode.Jmp:
					throw new NotImplementedException();
				case ILOpCode.Ldarg:
					return Ldarg(reader.ReadUInt16());
				case ILOpCode.Ldarg_S:
					return Ldarg(reader.ReadByte());
				case ILOpCode.Ldarg_0:
				case ILOpCode.Ldarg_1:
				case ILOpCode.Ldarg_2:
				case ILOpCode.Ldarg_3:
					return Ldarg(ilOpCode - ILOpCode.Ldarg_0);
				case ILOpCode.Ldarga:
					return Ldarga(reader.ReadUInt16());
				case ILOpCode.Ldarga_S:
					return Ldarga(reader.ReadByte());
				case ILOpCode.Ldc_I4:
					return new LdcI4(reader.ReadInt32());
				case ILOpCode.Ldc_I8:
					return new LdcI8(reader.ReadInt64());
				case ILOpCode.Ldc_R4:
					return new LdcF(reader.ReadSingle());
				case ILOpCode.Ldc_R8:
					return new LdcF(reader.ReadDouble());
				case ILOpCode.Ldc_I4_M1:
				case ILOpCode.Ldc_I4_0:
				case ILOpCode.Ldc_I4_1:
				case ILOpCode.Ldc_I4_2:
				case ILOpCode.Ldc_I4_3:
				case ILOpCode.Ldc_I4_4:
				case ILOpCode.Ldc_I4_5:
				case ILOpCode.Ldc_I4_6:
				case ILOpCode.Ldc_I4_7:
				case ILOpCode.Ldc_I4_8:
					return new LdcI4((int)ilOpCode - (int)ILOpCode.Ldc_I4_0);
				case ILOpCode.Ldc_I4_S:
					return new LdcI4(reader.ReadSByte());
				case ILOpCode.Ldnull:
					return new LdNull();
				case ILOpCode.Ldstr:
					return DecodeLdstr();
				case ILOpCode.Ldftn:
					return new LdFtn(ReadAndDecodeMethodReference());
				case ILOpCode.Ldind_I1:
					return new LdObj(Pop(), compilation.FindType(KnownTypeCode.SByte));
				case ILOpCode.Ldind_I2:
					return new LdObj(Pop(), compilation.FindType(KnownTypeCode.Int16));
				case ILOpCode.Ldind_I4:
					return new LdObj(Pop(), compilation.FindType(KnownTypeCode.Int32));
				case ILOpCode.Ldind_I8:
					return new LdObj(Pop(), compilation.FindType(KnownTypeCode.Int64));
				case ILOpCode.Ldind_U1:
					return new LdObj(Pop(), compilation.FindType(KnownTypeCode.Byte));
				case ILOpCode.Ldind_U2:
					return new LdObj(Pop(), compilation.FindType(KnownTypeCode.UInt16));
				case ILOpCode.Ldind_U4:
					return new LdObj(Pop(), compilation.FindType(KnownTypeCode.UInt32));
				case ILOpCode.Ldind_R4:
					return new LdObj(Pop(), compilation.FindType(KnownTypeCode.Single));
				case ILOpCode.Ldind_R8:
					return new LdObj(Pop(), compilation.FindType(KnownTypeCode.Double));
				case ILOpCode.Ldind_I:
					return new LdObj(Pop(), compilation.FindType(KnownTypeCode.IntPtr));
				case ILOpCode.Ldind_Ref:
					return new LdObj(Pop(), compilation.FindType(KnownTypeCode.Object));
				case ILOpCode.Ldloc:
					return Ldloc(reader.ReadUInt16());
				case ILOpCode.Ldloc_S:
					return Ldloc(reader.ReadByte());
				case ILOpCode.Ldloc_0:
				case ILOpCode.Ldloc_1:
				case ILOpCode.Ldloc_2:
				case ILOpCode.Ldloc_3:
					return Ldloc(ilOpCode - ILOpCode.Ldloc_0);
				case ILOpCode.Ldloca:
					return Ldloca(reader.ReadUInt16());
				case ILOpCode.Ldloca_S:
					return Ldloca(reader.ReadByte());
				case ILOpCode.Leave:
					return DecodeUnconditionalBranch(false, isLeave: true);
				case ILOpCode.Leave_S:
					return DecodeUnconditionalBranch(true, isLeave: true);
				case ILOpCode.Localloc:
					return new LocAlloc(Pop());
				case ILOpCode.Mul:
					return BinaryNumeric(OpCode.Mul, false, Sign.None);
				case ILOpCode.Mul_Ovf:
					return BinaryNumeric(OpCode.Mul, true, Sign.Signed);
				case ILOpCode.Mul_Ovf_Un:
					return BinaryNumeric(OpCode.Mul, true, Sign.Unsigned);
				case ILOpCode.Neg:
					return Neg();
				case ILOpCode.Newobj:
					return DecodeCall(OpCode.NewObj);
				case ILOpCode.Nop:
					return new Nop();
				case ILOpCode.Not:
					return new BitNot(Pop());
				case ILOpCode.Or:
					return BinaryNumeric(OpCode.BitOr);
				case ILOpCode.Pop:
					return new Void(Pop());
				case ILOpCode.Rem:
					return BinaryNumeric(OpCode.Rem, false, Sign.Signed);
				case ILOpCode.Rem_Un:
					return BinaryNumeric(OpCode.Rem, false, Sign.Unsigned);
				case ILOpCode.Ret:
					return Return();
				case ILOpCode.Shl:
					return BinaryNumeric(OpCode.Shl, false, Sign.None);
				case ILOpCode.Shr:
					return BinaryNumeric(OpCode.Shr, false, Sign.Signed);
				case ILOpCode.Shr_Un:
					return BinaryNumeric(OpCode.Shr, false, Sign.Unsigned);
				case ILOpCode.Starg:
					return Starg(reader.ReadUInt16());
				case ILOpCode.Starg_S:
					return Starg(reader.ReadByte());
				case ILOpCode.Stind_I1:
					return new Void(new StObj(value: Pop(), target: Pop(), type: compilation.FindType(KnownTypeCode.SByte)));
				case ILOpCode.Stind_I2:
					return new Void(new StObj(value: Pop(), target: Pop(), type: compilation.FindType(KnownTypeCode.Int16)));
				case ILOpCode.Stind_I4:
					return new Void(new StObj(value: Pop(), target: Pop(), type: compilation.FindType(KnownTypeCode.Int32)));
				case ILOpCode.Stind_I8:
					return new Void(new StObj(value: Pop(), target: Pop(), type: compilation.FindType(KnownTypeCode.Int64)));
				case ILOpCode.Stind_R4:
					return new Void(new StObj(value: Pop(), target: Pop(), type: compilation.FindType(KnownTypeCode.Single)));
				case ILOpCode.Stind_R8:
					return new Void(new StObj(value: Pop(), target: Pop(), type: compilation.FindType(KnownTypeCode.Double)));
				case ILOpCode.Stind_I:
					return new Void(new StObj(value: Pop(), target: Pop(), type: compilation.FindType(KnownTypeCode.IntPtr)));
				case ILOpCode.Stind_Ref:
					return new Void(new StObj(value: Pop(), target: Pop(), type: compilation.FindType(KnownTypeCode.Object)));
				case ILOpCode.Stloc:
					return Stloc(reader.ReadUInt16());
				case ILOpCode.Stloc_S:
					return Stloc(reader.ReadByte());
				case ILOpCode.Stloc_0:
				case ILOpCode.Stloc_1:
				case ILOpCode.Stloc_2:
				case ILOpCode.Stloc_3:
					return Stloc(ilOpCode - ILOpCode.Stloc_0);
				case ILOpCode.Sub:
					return BinaryNumeric(OpCode.Sub, false, Sign.None);
				case ILOpCode.Sub_Ovf:
					return BinaryNumeric(OpCode.Sub, true, Sign.Signed);
				case ILOpCode.Sub_Ovf_Un:
					return BinaryNumeric(OpCode.Sub, true, Sign.Unsigned);
				case ILOpCode.Switch:
					return DecodeSwitch();
				case ILOpCode.Xor:
					return BinaryNumeric(OpCode.BitXor);
				case ILOpCode.Box:
					return new Box(Pop(), ReadAndDecodeTypeReference());
				case ILOpCode.Castclass:
					return new CastClass(Pop(), ReadAndDecodeTypeReference());
				case ILOpCode.Cpobj:
					{
						var type = ReadAndDecodeTypeReference();
						var ld = new LdObj(Pop(), type);
						return new Void(new StObj(Pop(), ld, type));
					}
				case ILOpCode.Initobj:
					return new InitObj(Pop(), ReadAndDecodeTypeReference());
				case ILOpCode.Isinst:
					return new IsInst(Pop(), ReadAndDecodeTypeReference());
				case ILOpCode.Ldelem:
					return LdElem(index: Pop(), array: Pop(), type: ReadAndDecodeTypeReference());
				case ILOpCode.Ldelem_I1:
					return LdElem(index: Pop(), array: Pop(), type: compilation.FindType(KnownTypeCode.SByte));
				case ILOpCode.Ldelem_I2:
					return LdElem(index: Pop(), array: Pop(), type: compilation.FindType(KnownTypeCode.Int16));
				case ILOpCode.Ldelem_I4:
					return LdElem(index: Pop(), array: Pop(), type: compilation.FindType(KnownTypeCode.Int32));
				case ILOpCode.Ldelem_I8:
					return LdElem(index: Pop(), array: Pop(), type: compilation.FindType(KnownTypeCode.Int64));
				case ILOpCode.Ldelem_U1:
					return LdElem(index: Pop(), array: Pop(), type: compilation.FindType(KnownTypeCode.Byte));
				case ILOpCode.Ldelem_U2:
					return LdElem(index: Pop(), array: Pop(), type: compilation.FindType(KnownTypeCode.UInt16));
				case ILOpCode.Ldelem_U4:
					return LdElem(index: Pop(), array: Pop(), type: compilation.FindType(KnownTypeCode.UInt32));
				case ILOpCode.Ldelem_R4:
					return LdElem(index: Pop(), array: Pop(), type: compilation.FindType(KnownTypeCode.Single));
				case ILOpCode.Ldelem_R8:
					return LdElem(index: Pop(), array: Pop(), type: compilation.FindType(KnownTypeCode.Double));
				case ILOpCode.Ldelem_I:
					return LdElem(index: Pop(), array: Pop(), type: compilation.FindType(KnownTypeCode.IntPtr));
				case ILOpCode.Ldelem_Ref:
					return LdElem(index: Pop(), array: Pop(), type: compilation.FindType(KnownTypeCode.Object));
				case ILOpCode.Ldelema:
					return new LdElema(index: Pop(), array: Pop(), type: ReadAndDecodeTypeReference());
				case ILOpCode.Ldfld:
					return new LdFld(Pop(), ReadAndDecodeFieldReference());
				case ILOpCode.Ldflda:
					return new LdFlda(Pop(), ReadAndDecodeFieldReference());
				case ILOpCode.Stfld:
					return new Void(new StFld(value: Pop(), target: Pop(), field: ReadAndDecodeFieldReference()));
				case ILOpCode.Ldlen:
					return new LdLen(Pop());
				case ILOpCode.Ldobj:
					return new LdObj(Pop(), ReadAndDecodeTypeReference());
				case ILOpCode.Ldsfld:
					return new LdsFld(ReadAndDecodeFieldReference());
				case ILOpCode.Ldsflda:
					return new LdsFlda(ReadAndDecodeFieldReference());
				case ILOpCode.Stsfld:
					return new Void(new StsFld(Pop(), ReadAndDecodeFieldReference()));
				case ILOpCode.Ldtoken:
					var memberReference = ReadAndDecodeMetadataToken() as MemberReference;
					if (memberReference is TypeReference)
						return new LdTypeToken(typeSystem.Resolve((TypeReference)memberReference));
					if (memberReference is FieldReference)
						return new LdMemberToken(typeSystem.Resolve((FieldReference)memberReference));
					if (memberReference is MethodReference)
						return new LdMemberToken(typeSystem.Resolve((MethodReference)memberReference));
					throw new NotImplementedException();
				case ILOpCode.Ldvirtftn:
					return new LdVirtFtn(Pop(), ReadAndDecodeMethodReference());
				case ILOpCode.Mkrefany:
					throw new NotImplementedException();
				case ILOpCode.Newarr:
					return new NewArr(ReadAndDecodeTypeReference(), Pop());
				case ILOpCode.Refanytype:
					throw new NotImplementedException();
				case ILOpCode.Refanyval:
					throw new NotImplementedException();
				case ILOpCode.Rethrow:
					return new Rethrow();
				case ILOpCode.Sizeof:
					return new SizeOf(ReadAndDecodeTypeReference());
				case ILOpCode.Stelem:
					return StElem(value: Pop(), index: Pop(), array: Pop(), type: ReadAndDecodeTypeReference());
				case ILOpCode.Stelem_I1:
					return StElem(value: Pop(), index: Pop(), array: Pop(), type: compilation.FindType(KnownTypeCode.SByte));
				case ILOpCode.Stelem_I2:
					return StElem(value: Pop(), index: Pop(), array: Pop(), type: compilation.FindType(KnownTypeCode.Int16));
				case ILOpCode.Stelem_I4:
					return StElem(value: Pop(), index: Pop(), array: Pop(), type: compilation.FindType(KnownTypeCode.Int32));
				case ILOpCode.Stelem_I8:
					return StElem(value: Pop(), index: Pop(), array: Pop(), type: compilation.FindType(KnownTypeCode.Int64));
				case ILOpCode.Stelem_R4:
					return StElem(value: Pop(), index: Pop(), array: Pop(), type: compilation.FindType(KnownTypeCode.Single));
				case ILOpCode.Stelem_R8:
					return StElem(value: Pop(), index: Pop(), array: Pop(), type: compilation.FindType(KnownTypeCode.Double));
				case ILOpCode.Stelem_I:
					return StElem(value: Pop(), index: Pop(), array: Pop(), type: compilation.FindType(KnownTypeCode.IntPtr));
				case ILOpCode.Stelem_Ref:
					return StElem(value: Pop(), index: Pop(), array: Pop(), type: compilation.FindType(KnownTypeCode.Object));
				case ILOpCode.Stobj:
					return new Void(new StObj(value: Pop(), target: Pop(), type: ReadAndDecodeTypeReference()));
				case ILOpCode.Throw:
					return new Throw(Pop());
				case ILOpCode.Unbox:
					return new Unbox(Pop(), ReadAndDecodeTypeReference());
				case ILOpCode.Unbox_Any:
					return new UnboxAny(Pop(), ReadAndDecodeTypeReference());
				default:
					throw new NotImplementedException(ilOpCode.ToString());
			}
		}

		private IL.Pop Pop()
		{
			if (stack.Count > 0) {
				return new IL.Pop(stack.Pop());
			} else {
				Warn("Stack underflow");
				return new IL.Pop(StackType.Unknown);
			}
		}


		private ILInstruction Return()
		{
			if (body.Method.ReturnType.GetStackType() == StackType.Void)
				return new ICSharpCode.Decompiler.IL.Return();
			else
				return new ICSharpCode.Decompiler.IL.Return(Pop());
		}

		private ILInstruction DecodeLdstr()
		{
			var metadataToken = ReadMetadataToken(ref reader);
			return new LdStr(body.LookupStringToken(metadataToken));
		}

		private ILInstruction Ldarg(ushort v)
		{
			return new LdLoc(parameterVariables[v]);
		}

		private ILInstruction Ldarga(ushort v)
		{
			return new LdLoca(parameterVariables[v]);
		}

		private ILInstruction Starg(ushort v)
		{
			return new Void(new StLoc(Pop(), parameterVariables[v]));
		}

		private ILInstruction Ldloc(ushort v)
		{
			return new LdLoc(localVariables[v]);
		}

		private ILInstruction Ldloca(ushort v)
		{
			return new LdLoca(localVariables[v]);
		}

		private ILInstruction Stloc(ushort v)
		{
			return new Void(new StLoc(Pop(), localVariables[v]));
		}
		
		private ILInstruction LdElem(ILInstruction array, ILInstruction index, IType type)
		{
			return new LdObj(new LdElema(array, index, type), type);
		}
		
		private ILInstruction StElem(ILInstruction array, ILInstruction index, ILInstruction value, IType type)
		{
			return new Void(new StObj(new LdElema(array, index, type), value, type));
		}

		private ILInstruction DecodeConstrainedCall()
		{
			var typeRef = ReadAndDecodeTypeReference();
			var inst = DecodeInstruction();
			var call = inst as CallInstruction;
			if (call != null)
				call.ConstrainedTo = typeRef;
			return inst;
		}

		private ILInstruction DecodeTailCall()
		{
			var inst = DecodeInstruction();
			var call = inst as CallInstruction;
			if (call != null)
				call.IsTail = true;
			return inst;
		}

		private ILInstruction DecodeUnaligned()
		{
			byte alignment = reader.ReadByte();
			var inst = DecodeInstruction();
			var sup = UnpackVoid(inst) as ISupportsUnalignedPrefix;
			if (sup != null)
				sup.UnalignedPrefix = alignment;
			else
				Warn("Ignored invalid 'unaligned' prefix");
			return inst;
		}

		private ILInstruction DecodeVolatile()
		{
			var inst = DecodeInstruction();
			var svp = UnpackVoid(inst) as ISupportsVolatilePrefix;
			if (svp != null)
				svp.IsVolatile = true;
			else
				Warn("Ignored invalid 'volatile' prefix");
			return inst;
		}

		ILInstruction UnpackVoid(ILInstruction inst)
		{
			Void v = inst as Void;
			return v != null ? v.Argument : inst;
		}
		
		private ILInstruction DecodeReadonly()
		{
			var inst = DecodeInstruction();
			var ldelema = inst as LdElema;
			if (ldelema != null)
				ldelema.IsReadOnly = true;
			else
				Warn("Ignored invalid 'readonly' prefix");
			return inst;
		}

		ILInstruction DecodeCall(OpCode opCode)
		{
			var method = ReadAndDecodeMethodReference();
			var arguments = new ILInstruction[GetPopCount(opCode, method)];
			for (int i = arguments.Length - 1; i >= 0; i--) {
				arguments[i] = Pop();
			}
			var call = CallInstruction.Create(opCode, method);
			call.Arguments.AddRange(arguments);
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

		ILInstruction Comparison(OpCode opCode_I, OpCode opCode_F)
		{
			var right = Pop();
			var left = Pop();
			// Based on Table 4: Binary Comparison or Branch Operation
			if (left.ResultType == StackType.F && right.ResultType == StackType.F)
				return BinaryComparisonInstruction.Create(opCode_F, left, right);
			else
				return BinaryComparisonInstruction.Create(opCode_I, left, right);
		}

		ILInstruction DecodeComparisonBranch(bool shortForm, OpCode comparisonOpCodeForInts, OpCode comparisonOpCodeForFloats, bool negate)
		{
			int start = reader.Position - 1;
			var condition = Comparison(comparisonOpCodeForInts, comparisonOpCodeForFloats);
			int target = shortForm ? reader.ReadSByte() : reader.ReadInt32();
			target += reader.Position;
			condition.ILRange = new Interval(start, reader.Position);
			if (negate) {
				condition = new LogicNot(condition);
			}
			MarkBranchTarget(target);
			return new IfInstruction(condition, new Branch(target));
		}

		ILInstruction DecodeConditionalBranch(bool shortForm, bool negate)
		{
			int target = shortForm ? reader.ReadSByte() : reader.ReadInt32();
			target += reader.Position;
			ILInstruction condition = Pop();
			switch (condition.ResultType) {
				case StackType.O:
					// introduce explicit comparison with null
					condition = new Ceq(condition, new LdNull());
					negate = !negate;
					break;
				case StackType.I:
					// introduce explicit comparison with 0
					condition = new Ceq(condition, new LdcI4(0));
					negate = !negate;
					break;
				case StackType.I8:
					// introduce explicit comparison with 0
					condition = new Ceq(condition, new LdcI8(0));
					negate = !negate;
					break;
			}
			if (negate) {
				condition = new LogicNot(condition);
			}
			MarkBranchTarget(target);
			return new IfInstruction(condition, new Branch(target));
		}

		ILInstruction DecodeUnconditionalBranch(bool shortForm, bool isLeave = false)
		{
			int target = shortForm ? reader.ReadSByte() : reader.ReadInt32();
			target += reader.Position;
			int popCount = 0;
			if (isLeave) {
				popCount = stack.Count;
				stack.Clear();
			}
			MarkBranchTarget(target);
			return new Branch(target) { PopCount = popCount };
		}

		void MarkBranchTarget(int targetILOffset)
		{
			isBranchTarget[targetILOffset] = true;
			if (targetILOffset >= reader.Position) {
				branchStackDict[targetILOffset] = stack.ToImmutableArray();
			}
		}

		ILInstruction DecodeSwitch()
		{
			uint length = reader.ReadUInt32();
			int baseOffset = 4 * (int)length + reader.Position;
			var instr = new SwitchInstruction(Pop());
			
			for (uint i = 0; i < length; i++) {
				var section = new SwitchSection();
				section.Labels = new LongSet(i);
				int target = baseOffset + reader.ReadInt32();
				MarkBranchTarget(target);
				section.Body = new Branch(target);
				instr.Sections.Add(section);
			}
			
			return instr;
		}
		
		ILInstruction BinaryNumeric(OpCode opCode, bool checkForOverflow = false, Sign sign = Sign.None)
		{
			var right = Pop();
			var left = Pop();
			return BinaryNumericInstruction.Create(opCode, left, right, checkForOverflow, sign);
		}
	}
}
