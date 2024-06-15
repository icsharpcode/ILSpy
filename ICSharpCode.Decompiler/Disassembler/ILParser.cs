// Copyright (c) 2018 Siegfried Pammer
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
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.Disassembler
{
	public static class ILParser
	{
		public static ILOpCode DecodeOpCode(this ref BlobReader blob)
		{
			byte opCodeByte = blob.ReadByte();
			if (opCodeByte == 0xFE && blob.RemainingBytes >= 1)
			{
				return (ILOpCode)(0xFE00 + blob.ReadByte());
			}
			else
			{
				return (ILOpCode)opCodeByte;
			}
		}

		internal static int OperandSize(this OperandType opType)
		{
			switch (opType)
			{
				// 64-bit
				case OperandType.I8:
				case OperandType.R:
					return 8;
				// 32-bit
				case OperandType.BrTarget:
				case OperandType.Field:
				case OperandType.Method:
				case OperandType.I:
				case OperandType.Sig:
				case OperandType.String:
				case OperandType.Tok:
				case OperandType.Type:
				case OperandType.ShortR:
					return 4;
				// (n + 1) * 32-bit
				case OperandType.Switch:
					return 4; // minimum 4, usually more
				case OperandType.Variable: // 16-bit
					return 2;
				// 8-bit
				case OperandType.ShortVariable:
				case OperandType.ShortBrTarget:
				case OperandType.ShortI:
					return 1;
				default:
					return 0;
			}
		}

		public static void SkipOperand(this ref BlobReader blob, ILOpCode opCode)
		{
			var opType = opCode.GetOperandType();
			int operandSize;
			if (opType == OperandType.Switch)
			{
				uint n = blob.RemainingBytes >= 4 ? blob.ReadUInt32() : uint.MaxValue;
				if (n < int.MaxValue / 4)
				{
					operandSize = (int)(n * 4);
				}
				else
				{
					operandSize = int.MaxValue;
				}
			}
			else
			{
				operandSize = opType.OperandSize();
			}
			if (operandSize <= blob.RemainingBytes)
			{
				blob.Offset += operandSize;
			}
			else
			{
				// ignore missing/partial operand at end of body
				blob.Offset = blob.Length;
			}
		}

		public static int DecodeBranchTarget(this ref BlobReader blob, ILOpCode opCode)
		{
			int opSize = opCode.GetBranchOperandSize();
			if (opSize <= blob.RemainingBytes)
			{
				int relOffset = opSize == 4 ? blob.ReadInt32() : blob.ReadSByte();
				return unchecked(relOffset + blob.Offset);
			}
			else
			{
				return int.MinValue;
			}
		}

		public static int[] DecodeSwitchTargets(this ref BlobReader blob)
		{
			if (blob.RemainingBytes < 4)
			{
				blob.Offset += blob.RemainingBytes;
				return new int[0];
			}
			uint numTargets = blob.ReadUInt32();
			bool numTargetOverflow = false;
			if (numTargets > blob.RemainingBytes / 4)
			{
				numTargets = (uint)(blob.RemainingBytes / 4);
				numTargetOverflow = true;
			}
			int[] targets = new int[numTargets];
			int offset = blob.Offset + 4 * targets.Length;
			for (int i = 0; i < targets.Length; i++)
			{
				targets[i] = unchecked(blob.ReadInt32() + offset);
			}
			if (numTargetOverflow)
			{
				blob.Offset += blob.RemainingBytes;
			}
			return targets;
		}

		public static string DecodeUserString(this ref BlobReader blob, MetadataReader metadata)
		{
			return metadata.GetUserString(MetadataTokens.UserStringHandle(blob.ReadInt32()));
		}

		public static int DecodeIndex(this ref BlobReader blob, ILOpCode opCode)
		{
			switch (opCode.GetOperandType())
			{
				case OperandType.ShortVariable:
					return blob.ReadByte();
				case OperandType.Variable:
					return blob.ReadUInt16();
				default:
					throw new ArgumentException($"{opCode} not supported!");
			}
		}

		public static bool IsReturn(this ILOpCode opCode)
		{
			return opCode == ILOpCode.Ret || opCode == ILOpCode.Endfilter || opCode == ILOpCode.Endfinally;
		}

		public static int GetHeaderSize(BlobReader bodyBlockReader)
		{
			byte header = bodyBlockReader.ReadByte();
			if ((header & 3) == 3)
			{
				// fat
				ushort largeHeader = (ushort)((bodyBlockReader.ReadByte() << 8) | header);
				return (byte)(largeHeader >> 12) * 4;
			}
			else
			{
				// tiny
				return 1;
			}
		}

		public static void SetBranchTargets(ref BlobReader blob, BitSet branchTargets)
		{
			while (blob.RemainingBytes > 0)
			{
				var opCode = DecodeOpCode(ref blob);
				if (opCode == ILOpCode.Switch)
				{
					foreach (var target in DecodeSwitchTargets(ref blob))
					{
						if (target >= 0 && target < blob.Length)
							branchTargets.Set(target);
					}
				}
				else if (opCode.IsBranch())
				{
					int target = DecodeBranchTarget(ref blob, opCode);
					if (target >= 0 && target < blob.Length)
						branchTargets.Set(target);
				}
				else
				{
					SkipOperand(ref blob, opCode);
				}
			}
		}
	}
}
