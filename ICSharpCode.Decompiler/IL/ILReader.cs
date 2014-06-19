using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;
using System.Diagnostics;
using Mono.Cecil;
using System.Collections;
using System.Threading;

namespace ICSharpCode.Decompiler.IL
{
	public class ILReader(private readonly Mono.Cecil.Cil.MethodBody body, private readonly CancellationToken cancellationToken)
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

        readonly TypeSystem typeSystem = body.Method.Module.TypeSystem;
		BlobReader reader = body.GetILReader();
		Stack<StackType> stack = new Stack<StackType>(body.MaxStackSize);
		ILVariable[] parameterVariables = InitParameterVariables(body);
		ILVariable[] localVariables = body.Variables.Select(v => new ILVariable(v)).ToArray();
		BitArray isBranchTarget;
		List<ILInstruction> instructionBuilder;

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

		void ReadInstructions(Dictionary<int, ImmutableArray<StackType>> outputStacks)
		{
			instructionBuilder = new List<ILInstruction>();
			isBranchTarget = new BitArray(body.CodeSize);
			stack.Clear();
			// Dictionary that stores stacks for forward jumps
			var branchStackDict = new Dictionary<int, ImmutableArray<StackType>>();

			while (reader.Position < reader.Length) {
				cancellationToken.ThrowIfCancellationRequested();
				int start = reader.Position;
				if (outputStacks != null)
					outputStacks.Add(start, stack.ToImmutableArray());
				ILInstruction decodedInstruction = DecodeInstruction();
				decodedInstruction.ILRange = new Interval(start, reader.Position);
				instructionBuilder.Add(decodedInstruction);
				if ((var branch = decodedInstruction as Branch) != null) {
					isBranchTarget[branch.TargetILOffset] = true;
					if (branch.TargetILOffset >= reader.Position) {
						branchStackDict[branch.TargetILOffset] = stack.ToImmutableArray();
					}
				}
				if (!decodedInstruction.IsEndReachable) {
					stack.Clear();
					if (branchStackDict.TryGetValue(reader.Position, out var stackFromBranch)) {
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

		BlockContainer CreateBlocks(bool instructionInlining)
		{
			if (instructionBuilder == null)
				ReadInstructions(null);
			return new BlockBuilder(body, instructionInlining).CreateBlocks(instructionBuilder, isBranchTarget);
        }

		ILInstruction DecodeInstruction()
		{
			var ilOpCode = ReadOpCode(ref reader);
			switch (ilOpCode) {
				case ILOpCode.Constrained:
					return DecodeConstrainedCall();
				case ILOpCode.Readonly:
					throw new NotImplementedException(); // needs ldelema
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
					stack.Push(StackType.O);
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
					return DecodeUnconditionalBranch(false, OpCode.Branch);
				case ILOpCode.Br_S:
					return DecodeUnconditionalBranch(true, OpCode.Branch);
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
					return Conv(PrimitiveType.I1, OverflowMode.None);
				case ILOpCode.Conv_I2:
					return Conv(PrimitiveType.I2, OverflowMode.None);
				case ILOpCode.Conv_I4:
					return Conv(PrimitiveType.I4, OverflowMode.None);
				case ILOpCode.Conv_I8:
					return Conv(PrimitiveType.I8, OverflowMode.None);
				case ILOpCode.Conv_R4:
					return Conv(PrimitiveType.R4, OverflowMode.None);
				case ILOpCode.Conv_R8:
					return Conv(PrimitiveType.R8, OverflowMode.None);
				case ILOpCode.Conv_U1:
					return Conv(PrimitiveType.U1, OverflowMode.None);
				case ILOpCode.Conv_U2:
					return Conv(PrimitiveType.U2, OverflowMode.None);
				case ILOpCode.Conv_U4:
					return Conv(PrimitiveType.U4, OverflowMode.None);
				case ILOpCode.Conv_U8:
					return Conv(PrimitiveType.U8, OverflowMode.None);
				case ILOpCode.Conv_I:
					return Conv(PrimitiveType.I, OverflowMode.None);
				case ILOpCode.Conv_U:
					return Conv(PrimitiveType.U, OverflowMode.None);
				case ILOpCode.Conv_R_Un:
					return Conv(PrimitiveType.R8, OverflowMode.Un);
				case ILOpCode.Conv_Ovf_I1:
					return Conv(PrimitiveType.I1, OverflowMode.Ovf);
				case ILOpCode.Conv_Ovf_I2:
					return Conv(PrimitiveType.I2, OverflowMode.Ovf);
				case ILOpCode.Conv_Ovf_I4:
					return Conv(PrimitiveType.I4, OverflowMode.Ovf);
				case ILOpCode.Conv_Ovf_I8:
					return Conv(PrimitiveType.I8, OverflowMode.Ovf);
				case ILOpCode.Conv_Ovf_U1:
					return Conv(PrimitiveType.U1, OverflowMode.Ovf);
				case ILOpCode.Conv_Ovf_U2:
					return Conv(PrimitiveType.U2, OverflowMode.Ovf);
				case ILOpCode.Conv_Ovf_U4:
					return Conv(PrimitiveType.U4, OverflowMode.Ovf);
				case ILOpCode.Conv_Ovf_U8:
					return Conv(PrimitiveType.U8, OverflowMode.Ovf);
				case ILOpCode.Conv_Ovf_I:
					return Conv(PrimitiveType.I, OverflowMode.Ovf);
				case ILOpCode.Conv_Ovf_U:
					return Conv(PrimitiveType.U, OverflowMode.Ovf);
				case ILOpCode.Conv_Ovf_I1_Un:
					return Conv(PrimitiveType.I1, OverflowMode.Ovf_Un);
				case ILOpCode.Conv_Ovf_I2_Un:
					return Conv(PrimitiveType.I2, OverflowMode.Ovf_Un);
				case ILOpCode.Conv_Ovf_I4_Un:
					return Conv(PrimitiveType.I4, OverflowMode.Ovf_Un);
				case ILOpCode.Conv_Ovf_I8_Un:
					return Conv(PrimitiveType.I8, OverflowMode.Ovf_Un);
				case ILOpCode.Conv_Ovf_U1_Un:
					return Conv(PrimitiveType.U1, OverflowMode.Ovf_Un);
				case ILOpCode.Conv_Ovf_U2_Un:
					return Conv(PrimitiveType.U2, OverflowMode.Ovf_Un);
				case ILOpCode.Conv_Ovf_U4_Un:
					return Conv(PrimitiveType.U4, OverflowMode.Ovf_Un);
				case ILOpCode.Conv_Ovf_U8_Un:
					return Conv(PrimitiveType.U8, OverflowMode.Ovf_Un);
				case ILOpCode.Conv_Ovf_I_Un:
					return Conv(PrimitiveType.I, OverflowMode.Ovf_Un);
				case ILOpCode.Conv_Ovf_U_Un:
					return Conv(PrimitiveType.U, OverflowMode.Ovf_Un);
				case ILOpCode.Cpblk:
					throw new NotImplementedException();
				case ILOpCode.Div:
					return BinaryNumeric(OpCode.Div, OverflowMode.None);
				case ILOpCode.Div_Un:
					return BinaryNumeric(OpCode.Div, OverflowMode.Un);
				case ILOpCode.Dup:
					return new Peek();
				case ILOpCode.Endfilter:
					throw new NotImplementedException();
				case ILOpCode.Endfinally:
					throw new NotImplementedException();
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
					return LdcI4(reader.ReadInt32());
				case ILOpCode.Ldc_I8:
					return LdcI8(reader.ReadInt64());
				case ILOpCode.Ldc_R4:
					return LdcF(reader.ReadSingle());
				case ILOpCode.Ldc_R8:
					return LdcF(reader.ReadDouble());
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
					return LdcI4((int)ilOpCode - (int)ILOpCode.Ldc_I4_0);
				case ILOpCode.Ldc_I4_S:
					return LdcI4(reader.ReadSByte());
				case ILOpCode.Ldnull:
					stack.Push(StackType.O);
					return new ConstantNull();
				case ILOpCode.Ldstr:
					return DecodeLdstr();
				case ILOpCode.Ldftn:
					throw new NotImplementedException();
				case ILOpCode.Ldind_I1:
					return LdInd(typeSystem.SByte);
				case ILOpCode.Ldind_I2:
					return LdInd(typeSystem.Int16);
				case ILOpCode.Ldind_I4:
					return LdInd(typeSystem.Int32);
				case ILOpCode.Ldind_I8:
					return LdInd(typeSystem.Int64);
				case ILOpCode.Ldind_U1:
					return LdInd(typeSystem.Byte);
				case ILOpCode.Ldind_U2:
					return LdInd(typeSystem.UInt16);
				case ILOpCode.Ldind_U4:
					return LdInd(typeSystem.UInt32);
				case ILOpCode.Ldind_R4:
					return LdInd(typeSystem.Single);
				case ILOpCode.Ldind_R8:
					return LdInd(typeSystem.Double);
				case ILOpCode.Ldind_I:
					return LdInd(typeSystem.IntPtr);
				case ILOpCode.Ldind_Ref:
					return LdInd(typeSystem.Object);
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
					return DecodeUnconditionalBranch(false, OpCode.Leave);
				case ILOpCode.Leave_S:
					return DecodeUnconditionalBranch(true, OpCode.Leave);
				case ILOpCode.Localloc:
					throw new NotImplementedException();
				case ILOpCode.Mul:
					return BinaryNumeric(OpCode.Mul, OverflowMode.None);
				case ILOpCode.Mul_Ovf:
					return BinaryNumeric(OpCode.Mul, OverflowMode.Ovf);
				case ILOpCode.Mul_Ovf_Un:
					return BinaryNumeric(OpCode.Mul, OverflowMode.Ovf_Un);
				case ILOpCode.Neg:
					return UnaryNumeric(OpCode.Neg);
				case ILOpCode.Newobj:
					return DecodeCall(OpCode.NewObj);
				case ILOpCode.Nop:
					return new Nop();
				case ILOpCode.Not:
					return UnaryNumeric(OpCode.BitNot);
				case ILOpCode.Or:
					return BinaryNumeric(OpCode.BitOr);
				case ILOpCode.Pop:
					stack.PopOrDefault();
					return new VoidInstruction();
				case ILOpCode.Rem:
					return BinaryNumeric(OpCode.Rem, OverflowMode.None);
				case ILOpCode.Rem_Un:
					return BinaryNumeric(OpCode.Rem, OverflowMode.Un);
				case ILOpCode.Ret:
					return Return();
				case ILOpCode.Shl:
					return Shift(OpCode.Shl);
				case ILOpCode.Shr:
					return Shift(OpCode.Shr);
				case ILOpCode.Shr_Un:
					return Shift(OpCode.Shl, OverflowMode.Un);
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
					throw new NotImplementedException();
				case ILOpCode.Castclass:
					throw new NotImplementedException();
				case ILOpCode.Cpobj:
					throw new NotImplementedException();
				case ILOpCode.Initobj:
					throw new NotImplementedException();
				case ILOpCode.Isinst:
					stack.PopOrDefault();
					stack.Push(StackType.O);
					return new IsInst((TypeReference)ReadAndDecodeMetadataToken());
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
					throw new NotImplementedException();
				case ILOpCode.Ldfld:
					{
						stack.PopOrDefault();
						FieldReference field = (FieldReference)ReadAndDecodeMetadataToken();
						stack.Push(field.FieldType.GetStackType());
						return new LoadInstanceField(field);
					}
				case ILOpCode.Ldflda:
					stack.PopOrDefault();
					stack.Push(StackType.Ref);
					return new LoadInstanceField((FieldReference)ReadAndDecodeMetadataToken(), OpCode.Ldflda);
				case ILOpCode.Stfld:
					stack.PopOrDefault();
					stack.PopOrDefault();
					return new StoreInstanceField((FieldReference)ReadAndDecodeMetadataToken());
				case ILOpCode.Ldlen:
					throw new NotImplementedException();
				case ILOpCode.Ldobj:
					throw new NotImplementedException();
				case ILOpCode.Ldsfld:
					{
						FieldReference field = (FieldReference)ReadAndDecodeMetadataToken();
						stack.Push(field.FieldType.GetStackType());
						return new LoadStaticField(field);
					}
				case ILOpCode.Ldsflda:
					stack.Push(StackType.Ref);
					return new LoadStaticField((FieldReference)ReadAndDecodeMetadataToken(), OpCode.Ldsflda);
				case ILOpCode.Stsfld:
					stack.PopOrDefault();
					return new StoreStaticField((FieldReference)ReadAndDecodeMetadataToken());
				case ILOpCode.Ldtoken:
					throw new NotImplementedException();
				case ILOpCode.Ldvirtftn:
					throw new NotImplementedException();
				case ILOpCode.Mkrefany:
					throw new NotImplementedException();
				case ILOpCode.Newarr:
					throw new NotImplementedException();
				case ILOpCode.Refanytype:
					throw new NotImplementedException();
				case ILOpCode.Refanyval:
					throw new NotImplementedException();
				case ILOpCode.Rethrow:
					throw new NotImplementedException();
				case ILOpCode.Sizeof:
					throw new NotImplementedException();
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
					throw new NotImplementedException();
				case ILOpCode.Throw:
					stack.PopOrDefault();
					return new ThrowInstruction();
				case ILOpCode.Unbox:
					throw new NotImplementedException();
				case ILOpCode.Unbox_Any:
					return DecodeUnboxAny();
				default:
					throw new NotImplementedException(ilOpCode.ToString());
			}
		}

		private ILInstruction DecodeUnboxAny()
		{
			TypeReference typeRef = (TypeReference)ReadAndDecodeMetadataToken();
			stack.PopOrDefault();
			stack.Push(typeRef.GetStackType());
			return new UnboxAny(typeRef);
		}

		private ILInstruction LdInd(TypeReference typeRef)
		{
			stack.PopOrDefault();	// pointer
			stack.Push(typeRef.GetStackType());
			return new LoadIndirect(typeRef);
		}

		private ILInstruction Shift(OpCode opCode, OverflowMode overflowMode = OverflowMode.None)
		{
			var shiftAmountType = stack.PopOrDefault();
			var valueType = stack.PopOrDefault();
			stack.Push(valueType);
			return new BinaryNumericInstruction(opCode, valueType, overflowMode);
		}

		private ILInstruction Return()
		{
			if (body.Method.ReturnType.GetStackType() == StackType.Void)
				return new ReturnVoidInstruction();
			else
				return new ReturnInstruction();
		}

		private ILInstruction UnaryNumeric(OpCode opCode)
		{
			var opType = stack.PopOrDefault();
			stack.Push(opType);
			return new UnaryNumericInstruction(opCode, opType);
		}

		private ILInstruction DecodeLdstr()
		{
			stack.Push(StackType.O);
			var metadataToken = ReadMetadataToken(ref reader);
			return new ConstantString(body.LookupStringToken(metadataToken));
		}

		private ILInstruction LdcI4(int val)
		{
			stack.Push(StackType.I4);
			return new ConstantI4(val);
		}

		private ILInstruction LdcI8(long val)
		{
			stack.Push(StackType.I8);
			return new ConstantI8(val);
		}

		private ILInstruction LdcF(double val)
		{
			stack.Push(StackType.F);
			return new ConstantFloat(val);
		}

		private ILInstruction Ldarg(ushort v)
		{
			stack.Push(parameterVariables[v].Type.GetStackType());
			return new LdLoc(parameterVariables[v]);
		}

		private ILInstruction Ldarga(ushort v)
		{
			stack.Push(StackType.Ref);
			return new LdLoca(parameterVariables[v]);
		}

		private ILInstruction Starg(ushort v)
		{
			stack.PopOrDefault();
			return new StLoc(parameterVariables[v]);
		}

		private ILInstruction Ldloc(ushort v)
		{
			stack.Push(localVariables[v].Type.GetStackType());
			return new LdLoc(localVariables[v]);
		}

		private ILInstruction Ldloca(ushort v)
		{
			stack.Push(StackType.Ref);
			return new LdLoca(localVariables[v]);
		}

		private ILInstruction Stloc(ushort v)
		{
			stack.PopOrDefault();
			return new StLoc(localVariables[v]);
		}

		private ILInstruction DecodeConstrainedCall()
		{
			var typeRef = ReadAndDecodeMetadataToken() as TypeReference;
			var inst = DecodeInstruction();
			if ((var call = inst as CallInstruction) != null)
				call.ConstrainedTo = typeRef;
			return inst;
		}

		private ILInstruction DecodeTailCall()
		{
			var inst = DecodeInstruction();
			if ((var call = inst as CallInstruction) != null)
				call.IsTail = true;
			return inst;
		}

		private ILInstruction DecodeUnaligned()
		{
			byte alignment = reader.ReadByte();
			var inst = DecodeInstruction();
			if ((var smp = inst as ISupportsMemoryPrefix) != null)
				smp.UnalignedPrefix = alignment;
			return inst;
		}

		private ILInstruction DecodeVolatile()
		{
			var inst = DecodeInstruction();
			if ((var smp = inst as ISupportsMemoryPrefix) != null)
				smp.IsVolatile = true;
			return inst;
		}

		private ILInstruction Conv(PrimitiveType targetType, OverflowMode mode)
		{
			StackType from = stack.PopOrDefault();
			stack.Push(targetType.GetStackType());
			return new ConvInstruction(from, targetType, mode);
		}

		ILInstruction DecodeCall(OpCode opCode)
		{
			var method = (MethodReference)ReadAndDecodeMetadataToken();
			var inst = new CallInstruction(opCode, method);
			for (int i = 0; i < inst.Operands.Length; i++) {
				stack.Pop();
			}
			var returnType = (opCode == OpCode.NewObj ? method.DeclaringType : method.ReturnType).GetStackType();
			if (returnType != StackType.Void)
				stack.Push(returnType);
			return new CallInstruction(opCode, method);
		}

		ILInstruction Comparison(OpCode opCode_I, OpCode opCode_F)
		{
			StackType right = stack.PopOrDefault();
			StackType left = stack.PopOrDefault();
			stack.Push(StackType.I4);
			// Based on Table 4: Binary Comparison or Branch Operation
			if (left == StackType.F && right == StackType.F)
				return new BinaryNumericInstruction(opCode_F, StackType.F, OverflowMode.None);
			if (left == StackType.I || right == StackType.I)
				return new BinaryNumericInstruction(opCode_I, StackType.I, OverflowMode.None);
			Debug.Assert(left == right); // this should hold in all valid IL
			return new BinaryNumericInstruction(opCode_I, left, OverflowMode.None);
		}

		ILInstruction DecodeComparisonBranch(bool shortForm, OpCode comparisonOpCodeForInts, OpCode comparisonOpCodeForFloats, bool negate)
		{
			int start = reader.Position - 1;
			var condition = Comparison(comparisonOpCodeForInts, comparisonOpCodeForFloats);
			int target = shortForm ? reader.ReadSByte() : reader.ReadInt32();
			target += reader.Position;
			condition.ILRange = new Interval(start, reader.Position);
			if (negate) {
				condition = new LogicNotInstruction { Operand = condition };
			}
			stack.PopOrDefault();
			return new ConditionalBranch(condition, target);
		}

		ILInstruction DecodeConditionalBranch(bool shortForm, bool negate)
		{
			int target = shortForm ? reader.ReadSByte() : reader.ReadInt32();
			target += reader.Position;
			ILInstruction condition = ILInstruction.Pop;
			if (negate) {
				condition = new LogicNotInstruction { Operand = condition };
			}
			stack.PopOrDefault();
			return new ConditionalBranch(condition, target);
		}

		ILInstruction DecodeUnconditionalBranch(bool shortForm, OpCode opCode)
		{
			int target = shortForm ? reader.ReadSByte() : reader.ReadInt32();
			target += reader.Position;
			return new Branch(opCode, target);
		}

		ILInstruction BinaryNumeric(OpCode opCode, OverflowMode overflowMode = OverflowMode.None)
		{
			StackType right = stack.PopOrDefault();
			StackType left = stack.PopOrDefault();
			// Based on Table 2: Binary Numeric Operations
			// also works for Table 5: Integer Operations
			// and for Table 7: Overflow Arithmetic Operations
			if (left == right) {
				stack.Push(left);
				return new BinaryNumericInstruction(opCode, left, overflowMode);
			}
			if (left == StackType.Ref || right == StackType.Ref) {
				if (left == StackType.Ref && right == StackType.Ref) {
					// sub(&, &) = I
					stack.Push(StackType.I);
				} else {
					// add/sub with I or I4 and &
					stack.Push(StackType.Ref);
				}
				return new BinaryNumericInstruction(opCode, StackType.Ref, overflowMode);
			}
			if (left == StackType.I || right == StackType.I) {
				stack.Push(left);
				return new BinaryNumericInstruction(opCode, StackType.I, overflowMode);
			}
			stack.Push(StackType.Unknown);
			return new BinaryNumericInstruction(opCode, StackType.Unknown, overflowMode);
		}
	}
}
