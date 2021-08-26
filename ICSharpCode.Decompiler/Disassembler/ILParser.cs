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

namespace ICSharpCode.Decompiler.Disassembler
{
	public static class ILParser
	{
		public static ILOpCode DecodeOpCode(this ref BlobReader blob)
		{
			byte opCodeByte = blob.ReadByte();
			return (ILOpCode)(opCodeByte == 0xFE ? 0xFE00 + blob.ReadByte() : opCodeByte);
		}

		public static void SkipOperand(this ref BlobReader blob, ILOpCode opCode)
		{
			switch (opCode.GetOperandType())
			{
				// 64-bit
				case OperandType.I8:
				case OperandType.R:
					blob.Offset += 8;
					break;
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
					blob.Offset += 4;
					break;
				// (n + 1) * 32-bit
				case OperandType.Switch:
					uint n = blob.ReadUInt32();
					blob.Offset += (int)(n * 4);
					break;
				// 16-bit
				case OperandType.Variable:
					blob.Offset += 2;
					break;
				// 8-bit
				case OperandType.ShortVariable:
				case OperandType.ShortBrTarget:
				case OperandType.ShortI:
					blob.Offset++;
					break;
			}
		}

		public static int DecodeBranchTarget(this ref BlobReader blob, ILOpCode opCode)
		{
			return (opCode.GetBranchOperandSize() == 4 ? blob.ReadInt32() : blob.ReadSByte()) + blob.Offset;
		}

		public static int[] DecodeSwitchTargets(this ref BlobReader blob)
		{
			int[] targets = new int[blob.ReadUInt32()];
			int offset = blob.Offset + 4 * targets.Length;
			for (int i = 0; i < targets.Length; i++)
				targets[i] = blob.ReadInt32() + offset;
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
	}
}
