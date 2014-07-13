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
		
		readonly Cil.MethodBody body;
		readonly CancellationToken cancellationToken;
		readonly TypeSystem typeSystem;

		BlobReader reader;
		readonly Stack<StackType> stack;
		ILVariable[] parameterVariables;
		ILVariable[] localVariables;
		BitArray isBranchTarget;
		List<ILInstruction> instructionBuilder;

		public ILReader(Cil.MethodBody body, CancellationToken cancellationToken)
		{
			if (body == null)
				throw new ArgumentNullException("body");
			this.body = body;
			this.cancellationToken = cancellationToken;
			this.typeSystem = body.Method.Module.TypeSystem;
			this.reader = body.GetILReader();
			this.stack = new Stack<StackType>(body.MaxStackSize);
			this.parameterVariables = InitParameterVariables(body);
			this.localVariables = body.Variables.Select(v => new ILVariable(v)).ToArray();
		}

		IMetadataTokenProvider ReadAndDecodeMetadataToken()
		{
			var token = ReadMetadataToken(ref reader);
			return body.LookupToken(token);
		}

		static ILVariable[] InitParameterVariables(Mono.Cecil.Cil.MethodBody body)
		{
			var parameterVariables = new ILVariable[body.Method.GetPopAmount()];
			int paramIndex = 0;
			if (body.Method.HasThis)
				parameterVariables[paramIndex++] = new ILVariable(body.ThisParameter);
			foreach (var p in body.Method.Parameters)
				parameterVariables[paramIndex++] = new ILVariable(p);
			Debug.Assert(paramIndex == parameterVariables.Length);
			return parameterVariables;
		}

		void Warn(string message)
		{
			Debug.Fail(message);
		}
		
		// Dictionary that stores stacks for forward jumps
		Dictionary<int, ImmutableArray<StackType>> branchStackDict = new Dictionary<int, ImmutableArray<StackType>>();
		
		void ReadInstructions(Dictionary<int, ImmutableArray<StackType>> outputStacks)
		{
			instructionBuilder = new List<ILInstruction>();
			isBranchTarget = new BitArray(body.CodeSize);
			stack.Clear();
			branchStackDict.Clear();
			
			// Fill isBranchTarget and branchStackDict based on exception handlers
			foreach (var eh in body.ExceptionHandlers) {
				isBranchTarget[eh.TryStart.Offset] = true;
				if (eh.FilterStart != null) {
					isBranchTarget[eh.FilterStart.Offset] = true;
					branchStackDict[eh.FilterStart.Offset] = ImmutableArray.Create(StackType.O);
				}
				if (eh.HandlerStart != null) {
					isBranchTarget[eh.HandlerStart.Offset] = true;
					if (eh.HandlerType == Cil.ExceptionHandlerType.Catch || eh.HandlerType == Cil.ExceptionHandlerType.Filter)
						branchStackDict[eh.HandlerStart.Offset] = ImmutableArray.Create(StackType.O);
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

		public void WriteTypedIL(ITextOutput output)
		{
			var outputStacks = new Dictionary<int, ImmutableArray<StackType>>();
			ReadInstructions(outputStacks);
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

		public void WriteBlocks(ITextOutput output, bool instructionInlining)
		{
			CreateBlocks(instructionInlining).WriteTo(output);
		}

		internal BlockContainer CreateBlocks(bool instructionInlining)
		{
			if (instructionBuilder == null)
				ReadInstructions(null);
			return new BlockBuilder(body, instructionInlining).CreateBlocks(instructionBuilder, isBranchTarget);
		}

		ILInstruction Neg()
		{
			switch (stack.PeekOrDefault()) {
				case StackType.I4:
				case StackType.I:
					return new Sub(new LdcI4(0), Pop(), OverflowMode.None);
				case StackType.I8:
					return new Sub(new LdcI8(0), Pop(), OverflowMode.None);
				case StackType.F:
					return new Sub(new LdcF(0), Pop(), OverflowMode.None);
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
					return BinaryNumeric(OpCode.Add, OverflowMode.Ovf);
				case ILOpCode.Add_Ovf_Un:
					return BinaryNumeric(OpCode.Add, OverflowMode.Ovf_Un);
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
					return new Ckfinite();
				case ILOpCode.Conv_I1:
					return new Conv(Pop(), PrimitiveType.I1, OverflowMode.None);
				case ILOpCode.Conv_I2:
					return new Conv(Pop(), PrimitiveType.I2, OverflowMode.None);
				case ILOpCode.Conv_I4:
					return new Conv(Pop(), PrimitiveType.I4, OverflowMode.None);
				case ILOpCode.Conv_I8:
					return new Conv(Pop(), PrimitiveType.I8, OverflowMode.None);
				case ILOpCode.Conv_R4:
					return new Conv(Pop(), PrimitiveType.R4, OverflowMode.None);
				case ILOpCode.Conv_R8:
					return new Conv(Pop(), PrimitiveType.R8, OverflowMode.None);
				case ILOpCode.Conv_U1:
					return new Conv(Pop(), PrimitiveType.U1, OverflowMode.None);
				case ILOpCode.Conv_U2:
					return new Conv(Pop(), PrimitiveType.U2, OverflowMode.None);
				case ILOpCode.Conv_U4:
					return new Conv(Pop(), PrimitiveType.U4, OverflowMode.None);
				case ILOpCode.Conv_U8:
					return new Conv(Pop(), PrimitiveType.U8, OverflowMode.None);
				case ILOpCode.Conv_I:
					return new Conv(Pop(), PrimitiveType.I, OverflowMode.None);
				case ILOpCode.Conv_U:
					return new Conv(Pop(), PrimitiveType.U, OverflowMode.None);
				case ILOpCode.Conv_R_Un:
					return new Conv(Pop(), PrimitiveType.R8, OverflowMode.Un);
				case ILOpCode.Conv_Ovf_I1:
					return new Conv(Pop(), PrimitiveType.I1, OverflowMode.Ovf);
				case ILOpCode.Conv_Ovf_I2:
					return new Conv(Pop(), PrimitiveType.I2, OverflowMode.Ovf);
				case ILOpCode.Conv_Ovf_I4:
					return new Conv(Pop(), PrimitiveType.I4, OverflowMode.Ovf);
				case ILOpCode.Conv_Ovf_I8:
					return new Conv(Pop(), PrimitiveType.I8, OverflowMode.Ovf);
				case ILOpCode.Conv_Ovf_U1:
					return new Conv(Pop(), PrimitiveType.U1, OverflowMode.Ovf);
				case ILOpCode.Conv_Ovf_U2:
					return new Conv(Pop(), PrimitiveType.U2, OverflowMode.Ovf);
				case ILOpCode.Conv_Ovf_U4:
					return new Conv(Pop(), PrimitiveType.U4, OverflowMode.Ovf);
				case ILOpCode.Conv_Ovf_U8:
					return new Conv(Pop(), PrimitiveType.U8, OverflowMode.Ovf);
				case ILOpCode.Conv_Ovf_I:
					return new Conv(Pop(), PrimitiveType.I, OverflowMode.Ovf);
				case ILOpCode.Conv_Ovf_U:
					return new Conv(Pop(), PrimitiveType.U, OverflowMode.Ovf);
				case ILOpCode.Conv_Ovf_I1_Un:
					return new Conv(Pop(), PrimitiveType.I1, OverflowMode.Ovf_Un);
				case ILOpCode.Conv_Ovf_I2_Un:
					return new Conv(Pop(), PrimitiveType.I2, OverflowMode.Ovf_Un);
				case ILOpCode.Conv_Ovf_I4_Un:
					return new Conv(Pop(), PrimitiveType.I4, OverflowMode.Ovf_Un);
				case ILOpCode.Conv_Ovf_I8_Un:
					return new Conv(Pop(), PrimitiveType.I8, OverflowMode.Ovf_Un);
				case ILOpCode.Conv_Ovf_U1_Un:
					return new Conv(Pop(), PrimitiveType.U1, OverflowMode.Ovf_Un);
				case ILOpCode.Conv_Ovf_U2_Un:
					return new Conv(Pop(), PrimitiveType.U2, OverflowMode.Ovf_Un);
				case ILOpCode.Conv_Ovf_U4_Un:
					return new Conv(Pop(), PrimitiveType.U4, OverflowMode.Ovf_Un);
				case ILOpCode.Conv_Ovf_U8_Un:
					return new Conv(Pop(), PrimitiveType.U8, OverflowMode.Ovf_Un);
				case ILOpCode.Conv_Ovf_I_Un:
					return new Conv(Pop(), PrimitiveType.I, OverflowMode.Ovf_Un);
				case ILOpCode.Conv_Ovf_U_Un:
					return new Conv(Pop(), PrimitiveType.U, OverflowMode.Ovf_Un);
				case ILOpCode.Cpblk:
					throw new NotImplementedException();
				case ILOpCode.Div:
					return BinaryNumeric(OpCode.Div, OverflowMode.None);
				case ILOpCode.Div_Un:
					return BinaryNumeric(OpCode.Div, OverflowMode.Un);
				case ILOpCode.Dup:
					return new Peek(stack.Count > 0 ? stack.Peek() : StackType.Unknown);
				case ILOpCode.Endfilter:
				case ILOpCode.Endfinally:
					return new EndFinally();
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
					return new LdFtn((MethodReference)ReadAndDecodeMetadataToken());
				case ILOpCode.Ldind_I1:
					return new LdObj(Pop(), typeSystem.SByte);
				case ILOpCode.Ldind_I2:
					return new LdObj(Pop(), typeSystem.Int16);
				case ILOpCode.Ldind_I4:
					return new LdObj(Pop(), typeSystem.Int32);
				case ILOpCode.Ldind_I8:
					return new LdObj(Pop(), typeSystem.Int64);
				case ILOpCode.Ldind_U1:
					return new LdObj(Pop(), typeSystem.Byte);
				case ILOpCode.Ldind_U2:
					return new LdObj(Pop(), typeSystem.UInt16);
				case ILOpCode.Ldind_U4:
					return new LdObj(Pop(), typeSystem.UInt32);
				case ILOpCode.Ldind_R4:
					return new LdObj(Pop(), typeSystem.Single);
				case ILOpCode.Ldind_R8:
					return new LdObj(Pop(), typeSystem.Double);
				case ILOpCode.Ldind_I:
					return new LdObj(Pop(), typeSystem.IntPtr);
				case ILOpCode.Ldind_Ref:
					return new LdObj(Pop(), typeSystem.Object);
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
					return BinaryNumeric(OpCode.Mul, OverflowMode.None);
				case ILOpCode.Mul_Ovf:
					return BinaryNumeric(OpCode.Mul, OverflowMode.Ovf);
				case ILOpCode.Mul_Ovf_Un:
					return BinaryNumeric(OpCode.Mul, OverflowMode.Ovf_Un);
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
					return BinaryNumeric(OpCode.Rem, OverflowMode.None);
				case ILOpCode.Rem_Un:
					return BinaryNumeric(OpCode.Rem, OverflowMode.Un);
				case ILOpCode.Ret:
					return Return();
				case ILOpCode.Shl:
					return BinaryNumeric(OpCode.Shl, OverflowMode.None);
				case ILOpCode.Shr:
					return BinaryNumeric(OpCode.Shr, OverflowMode.None);
				case ILOpCode.Shr_Un:
					return BinaryNumeric(OpCode.Shr, OverflowMode.Un);
				case ILOpCode.Starg:
					return Starg(reader.ReadUInt16());
				case ILOpCode.Starg_S:
					return Starg(reader.ReadByte());
				case ILOpCode.Stind_I1:
				case ILOpCode.Stind_I2:
				case ILOpCode.Stind_I4:
				case ILOpCode.Stind_I8:
				case ILOpCode.Stind_R4:
				case ILOpCode.Stind_R8:
				case ILOpCode.Stind_I:
				case ILOpCode.Stind_Ref:
					throw new NotImplementedException();
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
					return BinaryNumeric(OpCode.Sub);
				case ILOpCode.Sub_Ovf:
					return BinaryNumeric(OpCode.Sub, OverflowMode.Ovf);
				case ILOpCode.Sub_Ovf_Un:
					return BinaryNumeric(OpCode.Sub, OverflowMode.Ovf_Un);
				case ILOpCode.Switch:
					throw new NotImplementedException();
				case ILOpCode.Xor:
					return BinaryNumeric(OpCode.BitXor);
				case ILOpCode.Box:
					return new Box(Pop(), (TypeReference)ReadAndDecodeMetadataToken());
				case ILOpCode.Castclass:
					return new CastClass(Pop(), (TypeReference)ReadAndDecodeMetadataToken());
				case ILOpCode.Cpobj:
					{
						var type = (TypeReference)ReadAndDecodeMetadataToken();
						var ld = new LdObj(Pop(), type);
						return new StObj(Pop(), ld, type);
					}
				case ILOpCode.Initobj:
					return new InitObj(Pop(), (TypeReference)ReadAndDecodeMetadataToken());
				case ILOpCode.Isinst:
					return new IsInst(Pop(), (TypeReference)ReadAndDecodeMetadataToken());
				case ILOpCode.Ldelem:
				case ILOpCode.Ldelem_I1:
				case ILOpCode.Ldelem_I2:
				case ILOpCode.Ldelem_I4:
				case ILOpCode.Ldelem_I8:
				case ILOpCode.Ldelem_U1:
				case ILOpCode.Ldelem_U2:
				case ILOpCode.Ldelem_U4:
				case ILOpCode.Ldelem_R4:
				case ILOpCode.Ldelem_R8:
				case ILOpCode.Ldelem_I:
				case ILOpCode.Ldelem_Ref:
					throw new NotImplementedException();
				case ILOpCode.Ldelema:
					return new LdElema(index: Pop(), array: Pop(), type: (TypeReference)ReadAndDecodeMetadataToken());
				case ILOpCode.Ldfld:
					return new LdFld(Pop(), (FieldReference)ReadAndDecodeMetadataToken());
				case ILOpCode.Ldflda:
					return new LdFlda(Pop(), (FieldReference)ReadAndDecodeMetadataToken());
				case ILOpCode.Stfld:
					return new StFld(value: Pop(), target: Pop(), field: (FieldReference)ReadAndDecodeMetadataToken());
				case ILOpCode.Ldlen:
					return new LdLen(Pop());
				case ILOpCode.Ldobj:
					throw new NotImplementedException();
				case ILOpCode.Ldsfld:
					return new LdsFld((FieldReference)ReadAndDecodeMetadataToken());
				case ILOpCode.Ldsflda:
					return new LdsFlda((FieldReference)ReadAndDecodeMetadataToken());
				case ILOpCode.Stsfld:
					return new StsFld(Pop(), (FieldReference)ReadAndDecodeMetadataToken());
				case ILOpCode.Ldtoken:
					return new LdToken((MemberReference)ReadAndDecodeMetadataToken());
				case ILOpCode.Ldvirtftn:
					return new LdVirtFtn(Pop(), (MethodReference)ReadAndDecodeMetadataToken());
				case ILOpCode.Mkrefany:
					throw new NotImplementedException();
				case ILOpCode.Newarr:
					throw new NotImplementedException();
				case ILOpCode.Refanytype:
					throw new NotImplementedException();
				case ILOpCode.Refanyval:
					throw new NotImplementedException();
				case ILOpCode.Rethrow:
					return new Rethrow();
				case ILOpCode.Sizeof:
					return new SizeOf((TypeReference)ReadAndDecodeMetadataToken());
				case ILOpCode.Stelem:
				case ILOpCode.Stelem_I1:
				case ILOpCode.Stelem_I2:
				case ILOpCode.Stelem_I4:
				case ILOpCode.Stelem_I8:
				case ILOpCode.Stelem_R4:
				case ILOpCode.Stelem_R8:
				case ILOpCode.Stelem_I:
				case ILOpCode.Stelem_Ref:
					throw new NotImplementedException();
				case ILOpCode.Stobj:
					return new StObj(value: Pop(), target: Pop(), type: (TypeReference)ReadAndDecodeMetadataToken());
				case ILOpCode.Throw:
					return new Throw(Pop());
				case ILOpCode.Unbox:
					return new Unbox(Pop(), (TypeReference)ReadAndDecodeMetadataToken());
				case ILOpCode.Unbox_Any:
					return new UnboxAny(Pop(), (TypeReference)ReadAndDecodeMetadataToken());
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
			return new StLoc(Pop(), parameterVariables[v]);
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
			return new StLoc(Pop(), localVariables[v]);
		}

		private ILInstruction DecodeConstrainedCall()
		{
			var typeRef = ReadAndDecodeMetadataToken() as TypeReference;
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
			var sup = inst as ISupportsUnalignedPrefix;
			if (sup != null)
				sup.UnalignedPrefix = alignment;
			else
				Warn("Ignored invalid 'unaligned' prefix");
			return inst;
		}

		private ILInstruction DecodeVolatile()
		{
			var inst = DecodeInstruction();
			var svp = inst as ISupportsVolatilePrefix;
			if (svp != null)
				svp.IsVolatile = true;
			else
				Warn("Ignored invalid 'volatile' prefix");
			return inst;
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
			var method = (MethodReference)ReadAndDecodeMetadataToken();
			var arguments = new ILInstruction[CallInstruction.GetPopCount(opCode, method)];
			for (int i = arguments.Length - 1; i >= 0; i--) {
				arguments[i] = Pop();
			}
			var call = CallInstruction.Create(opCode, method);
			call.Arguments.AddRange(arguments);
			return call;
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
		
		ILInstruction BinaryNumeric(OpCode opCode, OverflowMode overflowMode = OverflowMode.None)
		{
			var right = Pop();
			var left = Pop();
			return BinaryNumericInstruction.Create(opCode, left, right, overflowMode);
		}
	}
}
