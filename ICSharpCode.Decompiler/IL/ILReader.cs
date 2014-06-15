using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;
using System.Diagnostics;
using Mono.Cecil;

namespace ICSharpCode.Decompiler.IL
{
	class ILReader(MethodDefinition method, private Mono.Cecil.Cil.MethodBody body)
	{
		public static ILOpCode ReadOpCode(ref BlobReader reader)
		{
			byte b = reader.ReadByte();
			if (b == 0xfe)
				return (ILOpCode)(0x100 | reader.ReadByte());
			else
				return (ILOpCode)b;
		}

		public static MetadataToken ReadMetadataToken(ref BlobReader reader)
		{
			return new MetadataToken(reader.ReadUInt32());
		}

		ImmutableArray<ILInstruction>.Builder instructionBuilder = ImmutableArray.CreateBuilder<ILInstruction>();
		BlobReader reader = body.GetILReader();
		Stack<StackType> stack = new Stack<StackType>(body.MaxStackSize);

		IMetadataTokenProvider ReadAndDecodeMetadataToken()
		{
			var token = ReadMetadataToken(ref reader);
			return body.LookupToken(token);
		}

		void ReadInstructions()
		{
			//var body = method.GetBody();
			//var methodSignature = new MethodSignature(method.Signature);
			//var localVariableTypes = GetStackTypes(body.GetLocalVariableTypes(method.ContainingModule.metadata));
			//var parameterTypes = GetStackTypes(methodSignature.ParameterTypes);
			//var stack = new Stack<StackType>(body.MaxStack);

			// Dictionary that stores stacks for forward jumps
			var branchStackDict = new Dictionary<int, ImmutableArray<StackType>>();

			while (reader.Position < reader.Length) {
				int start = reader.Position;
				ILInstruction decodedInstruction = DecodeInstruction();
				decodedInstruction.ILRange = new Interval(start, reader.Position);
				instructionBuilder.Add(decodedInstruction);
				if ((var branch = decodedInstruction as BranchInstruction) != null) {
					if (branch.TargetILOffset >= reader.Position) {
						branchStackDict[branch.TargetILOffset] = stack.ToImmutableArray();
					}
				}
				if (IsUnconditionalBranch(decodedInstruction.OpCode)) {
					stack.Clear();
					if (branchStackDict.TryGetValue(reader.Position, out var stackFromBranch)) {
						for (int i = stackFromBranch.Length - 1; i >= 0; i--) {
							stack.Push(stackFromBranch[i]);
						}
					}
				}
			}
		}

		private bool IsUnconditionalBranch(OpCode opCode)
		{
			return opCode == OpCode.Branch;
		}

		ILInstruction DecodeInstruction()
		{
			var ilOpCode = ReadOpCode(ref reader);
			switch (ilOpCode) {
				case ILOpCode.Constrained:
					return DecodeConstrainedCall();
				case ILOpCode.Nop:
					return DecodeNoPrefix();
				case ILOpCode.Readonly:
					throw new NotImplementedException(); // needs ldelema
				case ILOpCode.Tailcall:
					return DecodeTailCall();
				case ILOpCode.Unaligned:
					return DecodeUnaligned();
				case ILOpCode.Volatile:
					return DecodeVolatile();
				case ILOpCode.Add:
					return DecodeBinaryNumericInstruction(OpCode.Add);
				case ILOpCode.Add_Ovf:
					return DecodeBinaryNumericInstruction(OpCode.Add, OverflowMode.Ovf);
				case ILOpCode.Add_Ovf_Un:
					return DecodeBinaryNumericInstruction(OpCode.Add, OverflowMode.Ovf_Un);
				case ILOpCode.And:
					return DecodeBinaryNumericInstruction(OpCode.BitAnd);
				case ILOpCode.Arglist:
					stack.Push(StackType.O);
					return new SimpleInstruction(OpCode.Arglist);
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
					return new SimpleInstruction(OpCode.Break);
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
				case ILOpCode.Calli:
					throw new NotImplementedException();
				case ILOpCode.Ceq:
					return DecodeComparison(OpCode.Ceq, OpCode.Ceq);
				case ILOpCode.Cgt:
					return DecodeComparison(OpCode.Cgt, OpCode.Cgt);
				case ILOpCode.Cgt_Un:
					return DecodeComparison(OpCode.Cgt_Un, OpCode.Cgt_Un);
				case ILOpCode.Clt:
					return DecodeComparison(OpCode.Clt, OpCode.Clt);
				case ILOpCode.Clt_Un:
					return DecodeComparison(OpCode.Clt_Un, OpCode.Clt_Un);
				case ILOpCode.Ckfinite:
					return new PeekInstruction(OpCode.Ckfinite);
				case ILOpCode.Conv_I1:
					return DecodeConv(PrimitiveType.I1, OverflowMode.None);
				case ILOpCode.Conv_I2:
					return DecodeConv(PrimitiveType.I2, OverflowMode.None);
				case ILOpCode.Conv_I4:
					return DecodeConv(PrimitiveType.I4, OverflowMode.None);
				case ILOpCode.Conv_I8:
					return DecodeConv(PrimitiveType.I8, OverflowMode.None);
				case ILOpCode.Conv_R4:
					return DecodeConv(PrimitiveType.R4, OverflowMode.None);
				case ILOpCode.Conv_R8:
					return DecodeConv(PrimitiveType.R8, OverflowMode.None);
				case ILOpCode.Conv_U1:
					return DecodeConv(PrimitiveType.U1, OverflowMode.None);
				case ILOpCode.Conv_U2:
					return DecodeConv(PrimitiveType.U2, OverflowMode.None);
				case ILOpCode.Conv_U4:
					return DecodeConv(PrimitiveType.U4, OverflowMode.None);
				case ILOpCode.Conv_U8:
					return DecodeConv(PrimitiveType.U8, OverflowMode.None);
				case ILOpCode.Conv_I:
					return DecodeConv(PrimitiveType.I, OverflowMode.None);
				case ILOpCode.Conv_U:
					return DecodeConv(PrimitiveType.U, OverflowMode.None);
				case ILOpCode.Conv_R_Un:
					return DecodeConv(PrimitiveType.R8, OverflowMode.Un);
				case ILOpCode.Conv_Ovf_I1:
					return DecodeConv(PrimitiveType.I1, OverflowMode.Ovf);
				case ILOpCode.Conv_Ovf_I2:
					return DecodeConv(PrimitiveType.I2, OverflowMode.Ovf);
				case ILOpCode.Conv_Ovf_I4:
					return DecodeConv(PrimitiveType.I4, OverflowMode.Ovf);
				case ILOpCode.Conv_Ovf_I8:
					return DecodeConv(PrimitiveType.I8, OverflowMode.Ovf);
				case ILOpCode.Conv_Ovf_U1:
					return DecodeConv(PrimitiveType.U1, OverflowMode.Ovf);
				case ILOpCode.Conv_Ovf_U2:
					return DecodeConv(PrimitiveType.U2, OverflowMode.Ovf);
				case ILOpCode.Conv_Ovf_U4:
					return DecodeConv(PrimitiveType.U4, OverflowMode.Ovf);
				case ILOpCode.Conv_Ovf_U8:
					return DecodeConv(PrimitiveType.U8, OverflowMode.Ovf);
				case ILOpCode.Conv_Ovf_I:
					return DecodeConv(PrimitiveType.I, OverflowMode.Ovf);
				case ILOpCode.Conv_Ovf_U:
					return DecodeConv(PrimitiveType.U, OverflowMode.Ovf);
				case ILOpCode.Conv_Ovf_I1_Un:
					return DecodeConv(PrimitiveType.I1, OverflowMode.Ovf_Un);
				case ILOpCode.Conv_Ovf_I2_Un:
					return DecodeConv(PrimitiveType.I2, OverflowMode.Ovf_Un);
				case ILOpCode.Conv_Ovf_I4_Un:
					return DecodeConv(PrimitiveType.I4, OverflowMode.Ovf_Un);
				case ILOpCode.Conv_Ovf_I8_Un:
					return DecodeConv(PrimitiveType.I8, OverflowMode.Ovf_Un);
				case ILOpCode.Conv_Ovf_U1_Un:
					return DecodeConv(PrimitiveType.U1, OverflowMode.Ovf_Un);
				case ILOpCode.Conv_Ovf_U2_Un:
					return DecodeConv(PrimitiveType.U2, OverflowMode.Ovf_Un);
				case ILOpCode.Conv_Ovf_U4_Un:
					return DecodeConv(PrimitiveType.U4, OverflowMode.Ovf_Un);
				case ILOpCode.Conv_Ovf_U8_Un:
					return DecodeConv(PrimitiveType.U8, OverflowMode.Ovf_Un);
				case ILOpCode.Conv_Ovf_I_Un:
					return DecodeConv(PrimitiveType.I, OverflowMode.Ovf_Un);
				case ILOpCode.Conv_Ovf_U_Un:
					return DecodeConv(PrimitiveType.U, OverflowMode.Ovf_Un);
				case ILOpCode.Cpblk:
					throw new NotImplementedException();
				case ILOpCode.Div:
					return DecodeBinaryNumericInstruction(OpCode.Div, OverflowMode.None);
				case ILOpCode.Div_Un:
					return DecodeBinaryNumericInstruction(OpCode.Div, OverflowMode.Un);
				case ILOpCode.Dup:
					return new PeekInstruction(OpCode.Peek);
				case ILOpCode.Endfilter:
					throw new NotImplementedException();
				case ILOpCode.Endfinally:
					throw new NotImplementedException();
				case ILOpCode.Initblk:
					throw new NotImplementedException();
				case ILOpCode.Jmp:
					throw new NotImplementedException();
				default:
					return InvalidInstruction();
			}
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

		/// <summary>no prefix -- possibly skip a fault check</summary>
		private ILInstruction DecodeNoPrefix()
		{
			var flags = (NoPrefixFlags)reader.ReadByte();
			var inst = DecodeInstruction();
			if ((var snp = inst as ISupportsNoPrefix) != null)
				snp.NoPrefix = flags;
			return inst;
		}

		private ILInstruction DecodeConv(PrimitiveType targetType, OverflowMode mode)
		{
			StackType from = stack.PopOrDefault();
			return new ConvInstruction(from, targetType, mode);
		}

		ILInstruction DecodeCall(OpCode call)
		{
			var token = ReadMetadataToken(ref reader);
			var methodRef = body.LookupToken(token);
			// TODO: we need the decoded + (in case of generic calls) type substituted 
			throw new NotImplementedException();
		}

		ILInstruction DecodeComparison(OpCode opCode_I, OpCode opCode_F)
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
			var condition = DecodeComparison(comparisonOpCodeForInts, comparisonOpCodeForFloats);
			int target = start + (shortForm ? reader.ReadSByte() : reader.ReadInt32());
			condition.ILRange = new Interval(start, reader.Position);
			if (negate) {
				condition = new LogicNotInstruction { Operand = condition };
			}
			return new ConditionalBranchInstruction(condition, target);
		}

		ILInstruction DecodeConditionalBranch(bool shortForm, bool negate)
		{
			int start = reader.Position - 1;
			int target = start + (shortForm ? reader.ReadSByte() : reader.ReadInt32());
			ILInstruction condition = ILInstruction.Pop;
			if (negate) {
				condition = new LogicNotInstruction { Operand = condition };
			}
			return new ConditionalBranchInstruction(condition, target);
		}

		ILInstruction DecodeUnconditionalBranch(bool shortForm)
		{
			int start = reader.Position - 1;
			int target = start + (shortForm ? reader.ReadSByte() : reader.ReadInt32());
			return new BranchInstruction(OpCode.Branch, target);
		}

		ILInstruction DecodeBinaryNumericInstruction(OpCode opCode, OverflowMode overflowMode = OverflowMode.None)
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

		ILInstruction InvalidInstruction()
		{
			Debug.Fail("This should only happen in obfuscated code");
			return new SimpleInstruction(OpCode.Invalid);
		}
	}
}
