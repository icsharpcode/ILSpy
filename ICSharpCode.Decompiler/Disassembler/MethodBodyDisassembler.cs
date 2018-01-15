// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Threading;

namespace ICSharpCode.Decompiler.Disassembler
{
	/// <summary>
	/// Disassembles a method body.
	/// </summary>
	public class MethodBodyDisassembler
	{
		readonly ITextOutput output;
		readonly MetadataReader metadata;
		readonly PEReader reader;
		readonly CancellationToken cancellationToken;

		/// <summary>
		/// Show .try/finally as blocks in IL code; indent loops.
		/// </summary>
		public bool DetectControlStructure { get; set; } = true;

		/// <summary>
		/// Show sequence points if debug information is loaded in Cecil.
		/// </summary>
		public bool ShowSequencePoints { get; set; }

		IList<SequencePoint> sequencePoints;
		int nextSequencePointIndex;

		public MethodBodyDisassembler(ITextOutput output, PEReader reader, CancellationToken cancellationToken)
		{
			this.output = output ?? throw new ArgumentNullException(nameof(output));
			this.cancellationToken = cancellationToken;
			this.reader = reader;
			this.metadata = reader.GetMetadataReader();
		}

		public unsafe virtual void Disassemble(MethodDefinitionHandle handle)
		{
			// start writing IL code
			MethodDefinition method = metadata.GetMethodDefinition(handle);
			output.WriteLine("// Method begins at RVA 0x{0:x4}", method.RelativeVirtualAddress);
			if (method.RelativeVirtualAddress == 0) {
				output.WriteLine("// Code size {0} (0x{0:x})", 0);
				output.WriteLine(".maxstack {0}", 0);
				output.WriteLine();
				return;
			}
			var body = reader.GetMethodBody(method.RelativeVirtualAddress);
			output.WriteLine("// Code size {0} (0x{0:x})", body.Size); // wrong
			output.WriteLine(".maxstack {0}", body.MaxStack);
			ulong address = (ulong)reader.PEHeaders.PEHeader.AddressOfEntryPoint;
			output.WriteLine($"// Entrypoint: 0x{address:x}");
			int token = MetadataTokens.GetToken(handle);

			var entrypointHandle = MetadataTokens.MethodDefinitionHandle(reader.PEHeaders.CorHeader.EntryPointTokenOrRelativeVirtualAddress);
			if (handle == entrypointHandle)
				output.WriteLine(".entrypoint");

			DisassembleLocalsBlock(handle, body);
			output.WriteLine();

			WriteInstructions(body.GetILReader());

			/*
			sequencePoints = method.DebugInformation?.SequencePoints;
			nextSequencePointIndex = 0;
			if (DetectControlStructure && body.Instructions.Count > 0) {
				Instruction inst = body.Instructions[0];
				HashSet<int> branchTargets = GetBranchTargets(body.Instructions);
				WriteStructureBody(new ILStructure(body), branchTargets, ref inst, method.Body.CodeSize);
			} else {
				foreach (var inst in method.Body.Instructions) {
					WriteInstruction(output, inst);
					output.WriteLine();
				}
				WriteExceptionHandlers(body);
			}
			sequencePoints = null;*/
		}

		void WriteInstructions(BlobReader blob)
		{
			while (blob.RemainingBytes > 0) {
				WriteInstruction(blob.Offset, blob.ReadByte(), ref blob);
			}
		}

		void WriteInstruction(int offset, byte instructionByte, ref BlobReader blob)
		{
			output.WriteDefinition(DisassemblerHelpers.OffsetToString(offset), offset);
			output.Write(": ");
			if (instructionByte == 0xFE) {
				instructionByte = blob.ReadByte();
				switch (instructionByte) {
					case 0x00:
						output.WriteReference("arglist", new OpCodeInfo(0xFE00, "arglist"));
						output.WriteLine();
						break;
					case 0x01:
						output.WriteReference("ceq", new OpCodeInfo(0xFE01, "ceq"));
						output.WriteLine();
						break;
					case 0x02:
						output.WriteReference("cgt", new OpCodeInfo(0xFE02, "cgt"));
						output.WriteLine();
						break;
					case 0x03:
						output.WriteReference("cgt.un", new OpCodeInfo(0xFE03, "cgt.un"));
						output.WriteLine();
						break;
					case 0x04:
						output.WriteReference("clt", new OpCodeInfo(0xFE04, "clt"));
						output.WriteLine();
						break;
					case 0x05:
						output.WriteReference("clt.un", new OpCodeInfo(0xFE05, "clt.un"));
						output.WriteLine();
						break;
					case 0x06:
						var token = MetadataTokens.EntityHandle(blob.ReadInt32());
						output.WriteReference("ldftn", new OpCodeInfo(0xFE06, "ldftn"));
						output.Write(" ");
						token.WriteTo(metadata, output);
						output.WriteLine();
						break;
					case 0x07:
						token = MetadataTokens.EntityHandle(blob.ReadInt32());
						output.WriteReference("ldvirtftn", new OpCodeInfo(0xFE07, "ldvirtftn"));
						output.Write(" ");
						token.WriteTo(metadata, output);
						output.WriteLine();
						break;
					case 0x09:
						ushort arg = blob.ReadUInt16();
						output.WriteReference("ldarg", new OpCodeInfo(0xFE09, "ldarg"));
						output.WriteLine($" {arg}");
						break;
					case 0x0A:
						arg = blob.ReadUInt16();
						output.WriteReference("ldarga", new OpCodeInfo(0xFE0A, "ldarga"));
						output.WriteLine($" {arg}");
						break;
					case 0x0B:
						arg = blob.ReadUInt16();
						output.WriteReference("starg", new OpCodeInfo(0xFE0B, "starg"));
						output.WriteLine($" {arg}");
						break;
					case 0x0C:
						arg = blob.ReadUInt16();
						output.WriteReference("ldloc", new OpCodeInfo(0xFE0C, "ldloc"));
						output.WriteLine($" {arg}");
						break;
					case 0x0D:
						arg = blob.ReadUInt16();
						output.WriteReference("ldloca", new OpCodeInfo(0xFE0D, "ldloca"));
						output.WriteLine($" {arg}");
						break;
					case 0x0E:
						arg = blob.ReadUInt16();
						output.WriteReference("stloc", new OpCodeInfo(0xFE0E, "stloc"));
						output.WriteLine($" {arg}");
						break;
					case 0x0F:
						output.WriteReference("localloc", new OpCodeInfo(0xFE0F, "localloc"));
						output.WriteLine();
						break;
					case 0x11:
						output.WriteReference("endfilter", new OpCodeInfo(0xFE11, "endfilter"));
						output.WriteLine();
						break;
					case 0x12:
						var alignment = blob.ReadByte();
						output.WriteReference("unaligned.", new OpCodeInfo(0xFE12, "unaligned."));
						output.WriteLine($" {alignment}");
						break;
					case 0x13:
						output.WriteReference("volatile.", new OpCodeInfo(0xFE13, "volatile."));
						output.WriteLine();
						break;
					case 0x14:
						output.WriteReference("tail.", new OpCodeInfo(0xFE14, "tail."));
						output.WriteLine();
						break;
					case 0x15:
						token = MetadataTokens.EntityHandle(blob.ReadInt32());
						output.WriteReference("initobj", new OpCodeInfo(0xFE15, "initobj"));
						output.Write(" ");
						token.WriteTo(metadata, output);
						output.WriteLine();
						break;
					case 0x16:
						token = MetadataTokens.EntityHandle(blob.ReadInt32());
						output.WriteReference("constrained.", new OpCodeInfo(0xFE16, "constrained."));
						output.Write(" ");
						token.WriteTo(metadata, output);
						output.WriteLine();
						break;
					case 0x17:
						token = MetadataTokens.EntityHandle(blob.ReadInt32());
						output.WriteReference("cpblk", new OpCodeInfo(0xFE17, "cpblk"));
						output.Write(" ");
						token.WriteTo(metadata, output);
						output.WriteLine();
						break;
					case 0x18:
						token = MetadataTokens.EntityHandle(blob.ReadInt32());
						output.WriteReference("initblk", new OpCodeInfo(0xFE18, "initblk"));
						output.Write(" ");
						token.WriteTo(metadata, output);
						output.WriteLine();
						break;
					case 0x19:
						var type = blob.ReadByte();
						output.WriteReference("no.", new OpCodeInfo(0xFE19, "no."));
						if ((type & 1) == 1)
							output.Write(" typecheck");
						if ((type & 2) == 2)
							output.Write(" rangecheck");
						if ((type & 4) == 4)
							output.Write(" nullcheck");
						output.WriteLine();
						break;
					case 0x1A:
						output.WriteReference("rethrow", new OpCodeInfo(0xFE1A, "rethrow"));
						output.WriteLine();
						break;
					case 0x1C:
						output.WriteReference("sizeof", new OpCodeInfo(0xFE1C, "sizeof"));
						output.WriteLine();
						break;
					case 0x1D:
						output.WriteReference("refanytype", new OpCodeInfo(0xFE1D, "refanytype"));
						output.WriteLine();
						break;
					case 0x1E:
						output.WriteReference("readonly.", new OpCodeInfo(0xFE1E, "readonly."));
						output.WriteLine();
						break;
					default:
						output.WriteLine($".emitbyte 0xfe");
						output.WriteLine($".emitbyte 0x{instructionByte:x2}");
						break;
				}
			} else {
				switch (instructionByte) {
					case 0x00:
						output.WriteReference("nop", new OpCodeInfo(0x0, "nop"));
						output.WriteLine();
						break;
					case 0x01:
						output.WriteReference("break", new OpCodeInfo(0x1, "break"));
						output.WriteLine();
						break;
					case 0x02:
					case 0x03:
					case 0x04:
					case 0x05:
						output.WriteReference($"ldarg.{instructionByte - 0x2}", new OpCodeInfo(instructionByte, $"ldarg.{instructionByte - 0x2}"));
						output.WriteLine();
						break;
					case 0x06:
					case 0x07:
					case 0x08:
					case 0x09:
						output.WriteReference($"ldloc.{instructionByte - 0x6}", new OpCodeInfo(instructionByte, $"ldloc.{instructionByte - 0x6}"));
						output.WriteLine();
						break;
					case 0x0A:
					case 0x0B:
					case 0x0C:
					case 0x0D:
						output.WriteReference($"stloc.{instructionByte - 0xA}", new OpCodeInfo(instructionByte, $"stloc.{instructionByte - 0xA}"));
						output.WriteLine();
						break;
					case 0x0E:
						byte arg = blob.ReadByte();
						output.WriteReference("ldarg.s", new OpCodeInfo(instructionByte, "ldarg.s"));
						output.WriteLine(" {0}", arg);
						break;
					case 0x0F:
						arg = blob.ReadByte();
						output.WriteReference("ldarga.s", new OpCodeInfo(instructionByte, "ldarga.s"));
						output.WriteLine(" {0}", arg);
						break;
					case 0x10:
						arg = blob.ReadByte();
						output.WriteReference("starg.s", new OpCodeInfo(instructionByte, "starg.s"));
						output.WriteLine(" {0}", arg);
						break;
					case 0x11:
						arg = blob.ReadByte();
						output.WriteReference("ldloc.s", new OpCodeInfo(instructionByte, "ldloc.s"));
						output.WriteLine(" {0}", arg);
						break;
					case 0x12:
						arg = blob.ReadByte();
						output.WriteReference("ldloca.s", new OpCodeInfo(instructionByte, "ldloca.s"));
						output.WriteLine(" {0}", arg);
						break;
					case 0x13:
						arg = blob.ReadByte();
						output.WriteReference("stloc.s", new OpCodeInfo(instructionByte, "stloc.s"));
						output.WriteLine(" {0}", arg);
						break;
					case 0x14:
						output.WriteReference("ldnull", new OpCodeInfo(instructionByte, "ldnull"));
						output.WriteLine();
						break;
					case 0x15:
						output.WriteReference("ldc.i4.m1", new OpCodeInfo(instructionByte, "ldc.i4.m1"));
						output.WriteLine();
						break;
					case 0x16:
					case 0x17:
					case 0x18:
					case 0x19:
					case 0x1A:
					case 0x1B:
					case 0x1C:
					case 0x1D:
					case 0x1E:
						output.WriteReference($"ldc.i4.{instructionByte - 0x16}", new OpCodeInfo(instructionByte, $"ldc.i4.{instructionByte - 0x16}"));
						output.WriteLine();
						break;
					case 0x1F:
						sbyte sbarg = blob.ReadSByte();
						output.WriteReference("ldc.i4.s", new OpCodeInfo(instructionByte, "ldc.i4.s"));
						output.WriteLine(" {0}", sbarg);
						break;
					case 0x20:
						int intarg = blob.ReadInt32();
						output.WriteReference("ldc.i4", new OpCodeInfo(instructionByte, "ldc.i4"));
						output.WriteLine(" {0}", intarg);
						break;
					case 0x21:
						long longarg = blob.ReadInt64();
						output.WriteReference("ldc.i8", new OpCodeInfo(instructionByte, "ldc.i8"));
						output.WriteLine(" {0}", longarg);
						break;
					case 0x22:
						float farg = blob.ReadSingle();
						output.WriteReference("ldc.r4", new OpCodeInfo(instructionByte, "ldc.r4"));
						output.WriteLine(" {0}", farg);
						break;
					case 0x23:
						double darg = blob.ReadDouble();
						output.WriteReference("ldc.r8", new OpCodeInfo(instructionByte, "ldc.r8"));
						output.WriteLine(" {0}", darg);
						break;
					case 0x25:
						output.WriteReference("dup", new OpCodeInfo(instructionByte, "dup"));
						output.WriteLine();
						break;
					case 0x26:
						output.WriteReference("pop", new OpCodeInfo(instructionByte, "pop"));
						output.WriteLine();
						break;
					case 0x27:
						var token = MetadataTokens.EntityHandle(blob.ReadInt32());
						output.WriteReference("jmp", new OpCodeInfo(instructionByte, "jmp"));
						output.Write(" ");
						token.WriteTo(metadata, output);
						output.WriteLine();
						break;
					case 0x28:
						token = MetadataTokens.EntityHandle(blob.ReadInt32());
						output.WriteReference("call", new OpCodeInfo(instructionByte, "call"));
						output.Write(" ");
						token.WriteTo(metadata, output);
						output.WriteLine();
						break;
					case 0x29:
						token = MetadataTokens.EntityHandle(blob.ReadInt32());
						output.WriteReference("calli", new OpCodeInfo(instructionByte, "calli"));
						output.Write(" ");
						token.WriteTo(metadata, output);
						output.WriteLine();
						break;
					case 0x2A:
						output.WriteReference("ret", new OpCodeInfo(instructionByte, "ret"));
						output.WriteLine();
						break;
					case 0x2B:
						int targetOffset = blob.ReadByte() + blob.Offset;
						output.WriteReference("br.s", new OpCodeInfo(instructionByte, "br.s"));
						output.Write(" ");
						output.WriteReference($"IL_{targetOffset:x4}", targetOffset, true);
						output.WriteLine();
						break;
					case 0x2C:
						targetOffset = blob.ReadByte() + blob.Offset;
						output.WriteReference("brfalse.s", new OpCodeInfo(instructionByte, "brfalse.s"));
						output.Write(" ");
						output.WriteReference($"IL_{targetOffset:x4}", targetOffset, true);
						output.WriteLine();
						break;
					case 0x2D:
						targetOffset = blob.ReadByte() + blob.Offset;
						output.WriteReference("brtrue.s", new OpCodeInfo(instructionByte, "brtrue.s"));
						output.Write(" ");
						output.WriteReference($"IL_{targetOffset:x4}", targetOffset, true);
						output.WriteLine();
						break;
					case 0x2E:
						targetOffset = blob.ReadByte() + blob.Offset;
						output.WriteReference("beq.s", new OpCodeInfo(instructionByte, "beq.s"));
						output.Write(" ");
						output.WriteReference($"IL_{targetOffset:x4}", targetOffset, true);
						output.WriteLine();
						break;
					case 0x2F:
						targetOffset = blob.ReadByte() + blob.Offset;
						output.WriteReference("bge.s", new OpCodeInfo(instructionByte, "bge.s"));
						output.Write(" ");
						output.WriteReference($"IL_{targetOffset:x4}", targetOffset, true);
						output.WriteLine();
						break;
					case 0x30:
						targetOffset = blob.ReadByte() + blob.Offset;
						output.WriteReference("bgt.s", new OpCodeInfo(instructionByte, "bgt.s"));
						output.Write(" ");
						output.WriteReference($"IL_{targetOffset:x4}", targetOffset, true);
						output.WriteLine();
						break;
					case 0x31:
						targetOffset = blob.ReadByte() + blob.Offset;
						output.WriteReference("ble.s", new OpCodeInfo(instructionByte, "ble.s"));
						output.Write(" ");
						output.WriteReference($"IL_{targetOffset:x4}", targetOffset, true);
						output.WriteLine();
						break;
					case 0x32:
						targetOffset = blob.ReadByte() + blob.Offset;
						output.WriteReference("blt.s", new OpCodeInfo(instructionByte, "blt.s"));
						output.Write(" ");
						output.WriteReference($"IL_{targetOffset:x4}", targetOffset, true);
						output.WriteLine();
						break;
					case 0x33:
						targetOffset = blob.ReadByte() + blob.Offset;
						output.WriteReference("bne.un.s", new OpCodeInfo(instructionByte, "bne.un.s"));
						output.Write(" ");
						output.WriteReference($"IL_{targetOffset:x4}", targetOffset, true);
						output.WriteLine();
						break;
					case 0x34:
						targetOffset = blob.ReadByte() + blob.Offset;
						output.WriteReference("bge.un.s", new OpCodeInfo(instructionByte, "bge.un.s"));
						output.Write(" ");
						output.WriteReference($"IL_{targetOffset:x4}", targetOffset, true);
						output.WriteLine();
						break;
					case 0x35:
						targetOffset = blob.ReadByte() + blob.Offset;
						output.WriteReference("bgt.un.s", new OpCodeInfo(instructionByte, "bgt.un.s"));
						output.Write(" ");
						output.WriteReference($"IL_{targetOffset:x4}", targetOffset, true);
						output.WriteLine();
						break;
					case 0x36:
						targetOffset = blob.ReadByte() + blob.Offset;
						output.WriteReference("ble.un.s", new OpCodeInfo(instructionByte, "ble.un.s"));
						output.Write(" ");
						output.WriteReference($"IL_{targetOffset:x4}", targetOffset, true);
						output.WriteLine();
						break;
					case 0x37:
						targetOffset = blob.ReadByte() + blob.Offset;
						output.WriteReference("blt.un.s", new OpCodeInfo(instructionByte, "blt.un.s"));
						output.Write(" ");
						output.WriteReference($"IL_{targetOffset:x4}", targetOffset, true);
						output.WriteLine();
						break;
					case 0x38:
						targetOffset = blob.ReadInt32() + blob.Offset;
						output.WriteReference("br", new OpCodeInfo(instructionByte, "br"));
						output.Write(" ");
						output.WriteReference($"IL_{targetOffset:x4}", targetOffset, true);
						output.WriteLine();
						break;
					case 0x39:
						targetOffset = blob.ReadInt32() + blob.Offset;
						output.WriteReference("brfalse", new OpCodeInfo(instructionByte, "brfalse"));
						output.Write(" ");
						output.WriteReference($"IL_{targetOffset:x4}", targetOffset, true);
						output.WriteLine();
						break;
					case 0x3A:
						targetOffset = blob.ReadInt32() + blob.Offset;
						output.WriteReference("brtrue", new OpCodeInfo(instructionByte, "brtrue"));
						output.Write(" ");
						output.WriteReference($"IL_{targetOffset:x4}", targetOffset, true);
						output.WriteLine();
						break;
					case 0x3B:
						targetOffset = blob.ReadInt32() + blob.Offset;
						output.WriteReference("beq", new OpCodeInfo(instructionByte, "beq"));
						output.Write(" ");
						output.WriteReference($"IL_{targetOffset:x4}", targetOffset, true);
						output.WriteLine();
						break;
					case 0x3C:
						targetOffset = blob.ReadInt32() + blob.Offset;
						output.WriteReference("bge", new OpCodeInfo(instructionByte, "bge"));
						output.Write(" ");
						output.WriteReference($"IL_{targetOffset:x4}", targetOffset, true);
						output.WriteLine();
						break;
					case 0x3D:
						targetOffset = blob.ReadInt32() + blob.Offset;
						output.WriteReference("bgt", new OpCodeInfo(instructionByte, "bgt"));
						output.Write(" ");
						output.WriteReference($"IL_{targetOffset:x4}", targetOffset, true);
						output.WriteLine();
						break;
					case 0x3E:
						targetOffset = blob.ReadInt32() + blob.Offset;
						output.WriteReference("ble", new OpCodeInfo(instructionByte, "ble"));
						output.Write(" ");
						output.WriteReference($"IL_{targetOffset:x4}", targetOffset, true);
						output.WriteLine();
						break;
					case 0x3F:
						targetOffset = blob.ReadInt32() + blob.Offset;
						output.WriteReference("blt", new OpCodeInfo(instructionByte, "blt"));
						output.Write(" ");
						output.WriteReference($"IL_{targetOffset:x4}", targetOffset, true);
						output.WriteLine();
						break;
					case 0x40:
						targetOffset = blob.ReadInt32() + blob.Offset;
						output.WriteReference("bne.un", new OpCodeInfo(instructionByte, "bne.un"));
						output.Write(" ");
						output.WriteReference($"IL_{targetOffset:x4}", targetOffset, true);
						output.WriteLine();
						break;
					case 0x41:
						targetOffset = blob.ReadInt32() + blob.Offset;
						output.WriteReference("bge.un", new OpCodeInfo(instructionByte, "bge.un"));
						output.Write(" ");
						output.WriteReference($"IL_{targetOffset:x4}", targetOffset, true);
						output.WriteLine();
						break;
					case 0x42:
						targetOffset = blob.ReadInt32() + blob.Offset;
						output.WriteReference("bgt.un", new OpCodeInfo(instructionByte, "bgt.un"));
						output.Write(" ");
						output.WriteReference($"IL_{targetOffset:x4}", targetOffset, true);
						output.WriteLine();
						break;
					case 0x43:
						targetOffset = blob.ReadInt32() + blob.Offset;
						output.WriteReference("ble.un", new OpCodeInfo(instructionByte, "ble.un"));
						output.Write(" ");
						output.WriteReference($"IL_{targetOffset:x4}", targetOffset, true);
						output.WriteLine();
						break;
					case 0x44:
						targetOffset = blob.ReadInt32() + blob.Offset;
						output.WriteReference("blt.un", new OpCodeInfo(instructionByte, "blt.un"));
						output.Write(" ");
						output.WriteReference($"IL_{targetOffset:x4}", targetOffset, true);
						output.WriteLine();
						break;
					case 0x45:
						uint count = blob.ReadUInt32();
						int[] targetOffsets = new int[count];
						for (int i = 0; i < count; i++) {
							targetOffsets[i] = blob.ReadInt32();
						}
						output.WriteReference("switch", new OpCodeInfo(instructionByte, "switch"));
						output.Write(" (");
						for (int i = 0; i < count; i++) {
							if (i > 0)
								output.Write(", ");
							targetOffset = targetOffsets[i] + blob.Offset;
							output.WriteReference($"IL_{targetOffset:x4}", targetOffset, true);
						}
						output.WriteLine(")");
						break;
					case 0x46:
						output.WriteReference("ldind.i1", new OpCodeInfo(instructionByte, "ldind.i1"));
						output.WriteLine();
						break;
					case 0x47:
						output.WriteReference("ldind.u1", new OpCodeInfo(instructionByte, "ldind.u1"));
						output.WriteLine();
						break;
					case 0x48:
						output.WriteReference("ldind.i2", new OpCodeInfo(instructionByte, "ldind.i2"));
						output.WriteLine();
						break;
					case 0x49:
						output.WriteReference("ldind.u2", new OpCodeInfo(instructionByte, "ldind.u2"));
						output.WriteLine();
						break;
					case 0x4A:
						output.WriteReference("ldind.i4", new OpCodeInfo(instructionByte, "ldind.i4"));
						output.WriteLine();
						break;
					case 0x4B:
						output.WriteReference("ldind.u4", new OpCodeInfo(instructionByte, "ldind.u4"));
						output.WriteLine();
						break;
					case 0x4C:
						output.WriteReference("ldind.i8", new OpCodeInfo(instructionByte, "ldind.i8"));
						output.WriteLine();
						break;
					case 0x4D:
						output.WriteReference("ldind.i", new OpCodeInfo(instructionByte, "ldind.i"));
						output.WriteLine();
						break;
					case 0x4E:
						output.WriteReference("ldind.r4", new OpCodeInfo(instructionByte, "ldind.r4"));
						output.WriteLine();
						break;
					case 0x4F:
						output.WriteReference("ldind.r8", new OpCodeInfo(instructionByte, "ldind.r8"));
						output.WriteLine();
						break;
					case 0x50:
						output.WriteReference("ldind.ref", new OpCodeInfo(instructionByte, "ldind.ref"));
						output.WriteLine();
						break;
					case 0x51:
						output.WriteReference("stind.ref", new OpCodeInfo(instructionByte, "stind.ref"));
						output.WriteLine();
						break;
					case 0x52:
						output.WriteReference("stind.i1", new OpCodeInfo(instructionByte, "stind.i1"));
						output.WriteLine();
						break;
					case 0x53:
						output.WriteReference("stind.i2", new OpCodeInfo(instructionByte, "stind.i2"));
						output.WriteLine();
						break;
					case 0x54:
						output.WriteReference("stind.i4", new OpCodeInfo(instructionByte, "stind.i4"));
						output.WriteLine();
						break;
					case 0x55:
						output.WriteReference("stind.i8", new OpCodeInfo(instructionByte, "stind.i8"));
						output.WriteLine();
						break;
					case 0x56:
						output.WriteReference("stind.r4", new OpCodeInfo(instructionByte, "stind.r4"));
						output.WriteLine();
						break;
					case 0x57:
						output.WriteReference("stind.r8", new OpCodeInfo(instructionByte, "stind.r8"));
						output.WriteLine();
						break;
					case 0x58:
						output.WriteReference("add", new OpCodeInfo(instructionByte, "add"));
						output.WriteLine();
						break;
					case 0x59:
						output.WriteReference("sub", new OpCodeInfo(instructionByte, "sub"));
						output.WriteLine();
						break;
					case 0x5A:
						output.WriteReference("mul", new OpCodeInfo(instructionByte, "mul"));
						output.WriteLine();
						break;
					case 0x5B:
						output.WriteReference("div", new OpCodeInfo(instructionByte, "div"));
						output.WriteLine();
						break;
					case 0x5C:
						output.WriteReference("div.un", new OpCodeInfo(instructionByte, "div.un"));
						output.WriteLine();
						break;
					case 0x5D:
						output.WriteReference("rem", new OpCodeInfo(instructionByte, "rem"));
						output.WriteLine();
						break;
					case 0x5E:
						output.WriteReference("rem.un", new OpCodeInfo(instructionByte, "rem.un"));
						output.WriteLine();
						break;
					case 0x5F:
						output.WriteReference("and", new OpCodeInfo(instructionByte, "and"));
						output.WriteLine();
						break;
					case 0x60:
						output.WriteReference("or", new OpCodeInfo(instructionByte, "or"));
						output.WriteLine();
						break;
					case 0x61:
						output.WriteReference("xor", new OpCodeInfo(instructionByte, "xor"));
						output.WriteLine();
						break;
					case 0x62:
						output.WriteReference("shl", new OpCodeInfo(instructionByte, "shl"));
						output.WriteLine();
						break;
					case 0x63:
						output.WriteReference("shr", new OpCodeInfo(instructionByte, "shr"));
						output.WriteLine();
						break;
					case 0x64:
						output.WriteReference("shr.un", new OpCodeInfo(instructionByte, "shr.un"));
						output.WriteLine();
						break;
					case 0x65:
						output.WriteReference("neg", new OpCodeInfo(instructionByte, "neg"));
						output.WriteLine();
						break;
					case 0x66:
						output.WriteReference("not", new OpCodeInfo(instructionByte, "not"));
						output.WriteLine();
						break;
					case 0x67:
						output.WriteReference("conv.i1", new OpCodeInfo(instructionByte, "conv.i1"));
						output.WriteLine();
						break;
					case 0x68:
						output.WriteReference("conv.i2", new OpCodeInfo(instructionByte, "conv.i2"));
						output.WriteLine();
						break;
					case 0x69:
						output.WriteReference("conv.i4", new OpCodeInfo(instructionByte, "conv.i4"));
						output.WriteLine();
						break;
					case 0x6A:
						output.WriteReference("conv.i8", new OpCodeInfo(instructionByte, "conv.i8"));
						output.WriteLine();
						break;
					case 0x6B:
						output.WriteReference("conv.r4", new OpCodeInfo(instructionByte, "conv.r4"));
						output.WriteLine();
						break;
					case 0x6C:
						output.WriteReference("conv.r8", new OpCodeInfo(instructionByte, "conv.r8"));
						output.WriteLine();
						break;
					case 0x6D:
						output.WriteReference("conv.u4", new OpCodeInfo(instructionByte, "conv.u4"));
						output.WriteLine();
						break;
					case 0x6E:
						output.WriteReference("conv.u8", new OpCodeInfo(instructionByte, "conv.u8"));
						output.WriteLine();
						break;
					case 0x6F:
						token = MetadataTokens.EntityHandle(blob.ReadInt32());
						output.WriteReference("callvirt", new OpCodeInfo(instructionByte, "callvirt"));
						output.Write(" ");
						token.WriteTo(metadata, output);
						output.WriteLine();
						break;
					case 0x70:
						token = MetadataTokens.EntityHandle(blob.ReadInt32());
						output.WriteReference("cpobj", new OpCodeInfo(instructionByte, "cpobj"));
						output.Write(" ");
						token.WriteTo(metadata, output);
						output.WriteLine();
						break;
					case 0x71:
						token = MetadataTokens.EntityHandle(blob.ReadInt32());
						output.WriteReference("ldobj", new OpCodeInfo(instructionByte, "ldobj"));
						output.Write(" ");
						token.WriteTo(metadata, output);
						output.WriteLine();
						break;
					case 0x72:
						var str = metadata.GetUserString(MetadataTokens.UserStringHandle(blob.ReadInt32()));
						output.WriteReference("ldstr", new OpCodeInfo(instructionByte, "ldstr"));
						output.WriteLine($" \"{DisassemblerHelpers.EscapeString(str)}\"");
						break;
					case 0x73:
						token = MetadataTokens.EntityHandle(blob.ReadInt32());
						output.WriteReference("newobj", new OpCodeInfo(instructionByte, "newobj"));
						output.Write(" ");
						token.WriteTo(metadata, output);
						output.WriteLine();
						break;
					case 0x74:
						token = MetadataTokens.EntityHandle(blob.ReadInt32());
						output.WriteReference("castclass", new OpCodeInfo(instructionByte, "castclass"));
						output.Write(" ");
						token.WriteTo(metadata, output);
						output.WriteLine();
						break;
					case 0x75:
						token = MetadataTokens.EntityHandle(blob.ReadInt32());
						output.WriteReference("isinst", new OpCodeInfo(instructionByte, "isinst"));
						output.Write(" ");
						token.WriteTo(metadata, output);
						output.WriteLine();
						break;
					case 0x76:
						output.WriteReference("conv.r.un", new OpCodeInfo(instructionByte, "conv.r.un"));
						output.WriteLine();
						break;
					case 0x79:
						token = MetadataTokens.EntityHandle(blob.ReadInt32());
						output.WriteReference("unbox", new OpCodeInfo(instructionByte, "unbox"));
						output.Write(" ");
						token.WriteTo(metadata, output);
						output.WriteLine();
						break;
					case 0x7A:
						output.WriteReference("throw", new OpCodeInfo(instructionByte, "throw"));
						output.WriteLine();
						break;
					case 0x7B:
						token = MetadataTokens.EntityHandle(blob.ReadInt32());
						output.WriteReference("ldfld", new OpCodeInfo(instructionByte, "ldfld"));
						output.Write(" ");
						token.WriteTo(metadata, output);
						output.WriteLine();
						break;
					case 0x7C:
						token = MetadataTokens.EntityHandle(blob.ReadInt32());
						output.WriteReference("ldflda", new OpCodeInfo(instructionByte, "ldflda"));
						output.Write(" ");
						token.WriteTo(metadata, output);
						output.WriteLine();
						break;
					case 0x7D:
						token = MetadataTokens.EntityHandle(blob.ReadInt32());
						output.WriteReference("stfld", new OpCodeInfo(instructionByte, "stfld"));
						output.Write(" ");
						token.WriteTo(metadata, output);
						output.WriteLine();
						break;
					case 0x7E:
						token = MetadataTokens.EntityHandle(blob.ReadInt32());
						output.WriteReference("ldsfld", new OpCodeInfo(instructionByte, "ldsfld"));
						output.Write(" ");
						token.WriteTo(metadata, output);
						output.WriteLine();
						break;
					case 0x7F:
						token = MetadataTokens.EntityHandle(blob.ReadInt32());
						output.WriteReference("ldsflda", new OpCodeInfo(instructionByte, "ldsflda"));
						output.Write(" ");
						token.WriteTo(metadata, output);
						output.WriteLine();
						break;
					case 0x80:
						token = MetadataTokens.EntityHandle(blob.ReadInt32());
						output.WriteReference("stsfld", new OpCodeInfo(instructionByte, "stsfld"));
						output.Write(" ");
						token.WriteTo(metadata, output);
						output.WriteLine();
						break;
					case 0x81:
						token = MetadataTokens.EntityHandle(blob.ReadInt32());
						output.WriteReference("stobj", new OpCodeInfo(instructionByte, "stobj"));
						output.Write(" ");
						token.WriteTo(metadata, output);
						output.WriteLine();
						break;
					case 0x82:
						output.WriteReference("conv.ovf.i1.un", new OpCodeInfo(instructionByte, "conv.ovf.i1.un"));
						output.WriteLine();
						break;
					case 0x83:
						output.WriteReference("conv.ovf.i2.un", new OpCodeInfo(instructionByte, "conv.ovf.i2.un"));
						output.WriteLine();
						break;
					case 0x84:
						output.WriteReference("conv.ovf.i4.un", new OpCodeInfo(instructionByte, "conv.ovf.i4.un"));
						output.WriteLine();
						break;
					case 0x85:
						output.WriteReference("conv.ovf.i8.un", new OpCodeInfo(instructionByte, "conv.ovf.i8.un"));
						output.WriteLine();
						break;
					case 0x86:
						output.WriteReference("conv.ovf.u1.un", new OpCodeInfo(instructionByte, "conv.ovf.u1.un"));
						output.WriteLine();
						break;
					case 0x87:
						output.WriteReference("conv.ovf.u2.un", new OpCodeInfo(instructionByte, "conv.ovf.u2.un"));
						output.WriteLine();
						break;
					case 0x88:
						output.WriteReference("conv.ovf.u4.un", new OpCodeInfo(instructionByte, "conv.ovf.u4.un"));
						output.WriteLine();
						break;
					case 0x89:
						output.WriteReference("conv.ovf.u8.un", new OpCodeInfo(instructionByte, "conv.ovf.u8.un"));
						output.WriteLine();
						break;
					case 0x8A:
						output.WriteReference("conv.ovf.i.un", new OpCodeInfo(instructionByte, "conv.ovf.i.un"));
						output.WriteLine();
						break;
					case 0x8B:
						output.WriteReference("conv.ovf.u.un", new OpCodeInfo(instructionByte, "conv.ovf.u.un"));
						output.WriteLine();
						break;
					case 0x8C:
						token = MetadataTokens.EntityHandle(blob.ReadInt32());
						output.WriteReference("box", new OpCodeInfo(instructionByte, "box"));
						output.Write(" ");
						token.WriteTo(metadata, output);
						output.WriteLine();
						break;
					case 0x8D:
						token = MetadataTokens.EntityHandle(blob.ReadInt32());
						output.WriteReference("newarr", new OpCodeInfo(instructionByte, "newarr"));
						output.Write(" ");
						token.WriteTo(metadata, output);
						output.WriteLine();
						break;
					case 0x8E:
						output.WriteReference("ldlen", new OpCodeInfo(instructionByte, "ldlen"));
						output.WriteLine();
						break;
					case 0x8F:
						token = MetadataTokens.EntityHandle(blob.ReadInt32());
						output.WriteReference("ldelema", new OpCodeInfo(instructionByte, "ldelema"));
						output.Write(" ");
						token.WriteTo(metadata, output);
						output.WriteLine();
						break;
					case 0x90:
						output.WriteReference("ldelem.i1", new OpCodeInfo(instructionByte, "ldelem.i1"));
						output.WriteLine();
						break;
					case 0x91:
						output.WriteReference("ldelem.u1", new OpCodeInfo(instructionByte, "ldelem.u1"));
						output.WriteLine();
						break;
					case 0x92:
						output.WriteReference("ldelem.i2", new OpCodeInfo(instructionByte, "ldelem.i2"));
						output.WriteLine();
						break;
					case 0x93:
						output.WriteReference("ldelem.u2", new OpCodeInfo(instructionByte, "ldelem.u2"));
						output.WriteLine();
						break;
					case 0x94:
						output.WriteReference("ldelem.i4", new OpCodeInfo(instructionByte, "ldelem.i4"));
						output.WriteLine();
						break;
					case 0x95:
						output.WriteReference("ldelem.u4", new OpCodeInfo(instructionByte, "ldelem.u4"));
						output.WriteLine();
						break;
					case 0x96:
						output.WriteReference("ldelem.i8", new OpCodeInfo(instructionByte, "ldelem.i8"));
						output.WriteLine();
						break;
					case 0x97:
						output.WriteReference("ldelem.i", new OpCodeInfo(instructionByte, "ldelem.i"));
						output.WriteLine();
						break;
					case 0x98:
						output.WriteReference("ldelem.r4", new OpCodeInfo(instructionByte, "ldelem.r4"));
						output.WriteLine();
						break;
					case 0x99:
						output.WriteReference("ldelem.r8", new OpCodeInfo(instructionByte, "ldelem.r8"));
						output.WriteLine();
						break;
					case 0x9A:
						output.WriteReference("ldelem.ref", new OpCodeInfo(instructionByte, "ldelem.ref"));
						output.WriteLine();
						break;
					case 0x9B:
						output.WriteReference("stelem.i", new OpCodeInfo(instructionByte, "stelem.i"));
						output.WriteLine();
						break;
					case 0x9C:
						output.WriteReference("stelem.i1", new OpCodeInfo(instructionByte, "stelem.i1"));
						output.WriteLine();
						break;
					case 0x9D:
						output.WriteReference("stelem.i2", new OpCodeInfo(instructionByte, "stelem.i2"));
						output.WriteLine();
						break;
					case 0x9E:
						output.WriteReference("stelem.i4", new OpCodeInfo(instructionByte, "stelem.i4"));
						output.WriteLine();
						break;
					case 0x9F:
						output.WriteReference("stelem.i8", new OpCodeInfo(instructionByte, "stelem.i8"));
						output.WriteLine();
						break;
					case 0xA0:
						output.WriteReference("stelem.r4", new OpCodeInfo(instructionByte, "stelem.r4"));
						output.WriteLine();
						break;
					case 0xA1:
						output.WriteReference("stelem.r8", new OpCodeInfo(instructionByte, "stelem.r8"));
						output.WriteLine();
						break;
					case 0xA2:
						output.WriteReference("stelem.ref", new OpCodeInfo(instructionByte, "stelem.ref"));
						output.WriteLine();
						break;
					case 0xA3:
						token = MetadataTokens.EntityHandle(blob.ReadInt32());
						output.WriteReference("ldelem", new OpCodeInfo(instructionByte, "ldelem"));
						output.Write(" ");
						token.WriteTo(metadata, output);
						output.WriteLine();
						break;
					case 0xA4:
						token = MetadataTokens.EntityHandle(blob.ReadInt32());
						output.WriteReference("stelem", new OpCodeInfo(instructionByte, "stelem"));
						output.Write(" ");
						token.WriteTo(metadata, output);
						output.WriteLine();
						break;
					case 0xC3:
						output.WriteReference("ckfinite", new OpCodeInfo(instructionByte, "ckfinite"));
						output.WriteLine();
						break;
					case 0xD0:
						token = MetadataTokens.EntityHandle(blob.ReadInt32());
						output.WriteReference("ldtoken", new OpCodeInfo(instructionByte, "ldtoken"));
						output.Write(" ");
						token.WriteTo(metadata, output);
						output.WriteLine();
						break;
					case 0xD1:
						output.WriteReference("conv.u2", new OpCodeInfo(instructionByte, "conv.u2"));
						output.WriteLine();
						break;
					case 0xD2:
						output.WriteReference("conv.u1", new OpCodeInfo(instructionByte, "conv.u1"));
						output.WriteLine();
						break;
					case 0xD3:
						output.WriteReference("conv.i", new OpCodeInfo(instructionByte, "conv.i"));
						output.WriteLine();
						break;
					case 0xD4:
						output.WriteReference("conv.ovf.i", new OpCodeInfo(instructionByte, "conv.ovf.i"));
						output.WriteLine();
						break;
					case 0xD5:
						output.WriteReference("conv.ovf.u", new OpCodeInfo(instructionByte, "conv.ovf.u"));
						output.WriteLine();
						break;
					case 0xD6:
						output.WriteReference("add.ovf", new OpCodeInfo(instructionByte, "add.ovf"));
						output.WriteLine();
						break;
					case 0xD7:
						output.WriteReference("add.ovf.un", new OpCodeInfo(instructionByte, "add.ovf.un"));
						output.WriteLine();
						break;
					case 0xD8:
						output.WriteReference("mul.ovf", new OpCodeInfo(instructionByte, "mul.ovf"));
						output.WriteLine();
						break;
					case 0xD9:
						output.WriteReference("mul.ovf.un", new OpCodeInfo(instructionByte, "mul.ovf.un"));
						output.WriteLine();
						break;
					case 0xDA:
						output.WriteReference("sub.ovf", new OpCodeInfo(instructionByte, "sub.ovf"));
						output.WriteLine();
						break;
					case 0xDB:
						output.WriteReference("sub.ovf.un", new OpCodeInfo(instructionByte, "sub.ovf.un"));
						output.WriteLine();
						break;
					case 0xDC:
						output.WriteReference("endfinally", new OpCodeInfo(instructionByte, "endfinally"));
						output.WriteLine();
						break;
					case 0xDD:
						targetOffset = blob.ReadInt32() + blob.Offset;
						output.WriteReference("leave", new OpCodeInfo(instructionByte, "leave"));
						output.Write(" ");
						output.WriteReference($"IL_{targetOffset:x4}", targetOffset, true);
						output.WriteLine();
						break;
					case 0xDE:
						targetOffset = blob.ReadByte() + blob.Offset;
						output.WriteReference("leave.s", new OpCodeInfo(instructionByte, "leave.s"));
						output.Write(" ");
						output.WriteReference($"IL_{targetOffset:x4}", targetOffset, true);
						output.WriteLine();
						break;
					case 0xDF:
						output.WriteReference("stind.i", new OpCodeInfo(instructionByte, "stind.i"));
						output.WriteLine();
						break;
					case 0xE0:
						output.WriteReference("conv.u", new OpCodeInfo(instructionByte, "conv.u"));
						output.WriteLine();
						break;
					default:
						output.WriteLine($".emitbyte 0x{instructionByte:x2}");
						break;
				}
			}
		}

		void DisassembleLocalsBlock(MethodDefinitionHandle handle, MethodBodyBlock body)
		{
			MethodDefinition method = metadata.GetMethodDefinition(handle);
			var declaringType = metadata.GetTypeDefinition(method.GetDeclaringType());
			if (body.LocalSignature.IsNil) return;
			var signature = metadata.GetStandaloneSignature(body.LocalSignature).DecodeLocalSignature(new MethodSignatureProvider(metadata, output), (declaringType.GetGenericParameters(), method.GetGenericParameters()));
			if (!signature.IsEmpty) {
				output.Write(".locals ");
				if (body.LocalVariablesInitialized)
					output.Write("init ");
				output.WriteLine("(");
				output.Indent();
				int index = 0;
				foreach (var v in signature) {
					output.WriteDefinition("[" + index + "] ", v);
					v(ILNameSyntax.TypeName);
					if (index + 1 < signature.Length)
						output.Write(',');
					output.WriteLine();
					index++;
				}
				output.Unindent();
				output.WriteLine(")");
			}
		}

		internal void WriteExceptionHandlers(Mono.Cecil.Cil.MethodBody body)
		{
			if (body.HasExceptionHandlers) {
				output.WriteLine();
				foreach (var eh in body.ExceptionHandlers) {
					eh.WriteTo(output);
					output.WriteLine();
				}
			}
		}

		HashSet<int> GetBranchTargets(IEnumerable<Mono.Cecil.Cil.Instruction> instructions)
		{
			HashSet<int> branchTargets = new HashSet<int>();
			foreach (var inst in instructions) {
				var target = inst.Operand as Mono.Cecil.Cil.Instruction;
				if (target != null)
					branchTargets.Add(target.Offset);
				var targets = inst.Operand as Mono.Cecil.Cil.Instruction[];
				if (targets != null)
					foreach (var t in targets)
						branchTargets.Add(t.Offset);
			}
			return branchTargets;
		}

		void WriteStructureHeader(ILStructure s)
		{
			switch (s.Type) {
				case ILStructureType.Loop:
					output.Write("// loop start");
					if (s.LoopEntryPoint != null) {
						output.Write(" (head: ");
						DisassemblerHelpers.WriteOffsetReference(output, s.LoopEntryPoint);
						output.Write(')');
					}
					output.WriteLine();
					break;
				case ILStructureType.Try:
					output.WriteLine(".try");
					output.WriteLine("{");
					break;
				case ILStructureType.Handler:
					switch (s.ExceptionHandler.HandlerType) {
						case Mono.Cecil.Cil.ExceptionHandlerType.Catch:
						case Mono.Cecil.Cil.ExceptionHandlerType.Filter:
							output.Write("catch");
							if (s.ExceptionHandler.CatchType != null) {
								output.Write(' ');
								s.ExceptionHandler.CatchType.WriteTo(output, ILNameSyntax.TypeName);
							}
							output.WriteLine();
							break;
						case Mono.Cecil.Cil.ExceptionHandlerType.Finally:
							output.WriteLine("finally");
							break;
						case Mono.Cecil.Cil.ExceptionHandlerType.Fault:
							output.WriteLine("fault");
							break;
						default:
							throw new NotSupportedException();
					}
					output.WriteLine("{");
					break;
				case ILStructureType.Filter:
					output.WriteLine("filter");
					output.WriteLine("{");
					break;
				default:
					throw new NotSupportedException();
			}
			output.Indent();
		}

		void WriteStructureBody(ILStructure s, HashSet<int> branchTargets, ref Mono.Cecil.Cil.Instruction inst, int codeSize)
		{
			bool isFirstInstructionInStructure = true;
			bool prevInstructionWasBranch = false;
			int childIndex = 0;
			while (inst != null && inst.Offset < s.EndOffset) {
				int offset = inst.Offset;
				if (childIndex < s.Children.Count && s.Children[childIndex].StartOffset <= offset && offset < s.Children[childIndex].EndOffset) {
					ILStructure child = s.Children[childIndex++];
					WriteStructureHeader(child);
					WriteStructureBody(child, branchTargets, ref inst, codeSize);
					WriteStructureFooter(child);
				} else {
					if (!isFirstInstructionInStructure && (prevInstructionWasBranch || branchTargets.Contains(offset))) {
						output.WriteLine();	// put an empty line after branches, and in front of branch targets
					}
					WriteInstruction(output, inst);
					output.WriteLine();

					prevInstructionWasBranch = inst.OpCode.FlowControl == Mono.Cecil.Cil.FlowControl.Branch
						|| inst.OpCode.FlowControl == Mono.Cecil.Cil.FlowControl.Cond_Branch
						|| inst.OpCode.FlowControl == Mono.Cecil.Cil.FlowControl.Return
						|| inst.OpCode.FlowControl == Mono.Cecil.Cil.FlowControl.Throw;

					inst = inst.Next;
				}
				isFirstInstructionInStructure = false;
			}
		}

		void WriteStructureFooter(ILStructure s)
		{
			output.Unindent();
			switch (s.Type) {
				case ILStructureType.Loop:
					output.WriteLine("// end loop");
					break;
				case ILStructureType.Try:
					output.WriteLine("} // end .try");
					break;
				case ILStructureType.Handler:
					output.WriteLine("} // end handler");
					break;
				case ILStructureType.Filter:
					output.WriteLine("} // end filter");
					break;
				default:
					throw new NotSupportedException();
			}
		}

		protected virtual void WriteInstruction(ITextOutput output, Mono.Cecil.Cil.Instruction instruction)
		{
			/*if (ShowSequencePoints && nextSequencePointIndex < sequencePoints?.Count) {
				SequencePoint sp = sequencePoints[nextSequencePointIndex];
				if (sp.Offset <= instruction.Offset) {
					output.Write("// sequence point: ");
					if (sp.Offset != instruction.Offset) {
						output.Write("!! at " + DisassemblerHelpers.OffsetToString(sp.Offset) + " !!");
					}
					if (sp.IsHidden) {
						output.WriteLine("hidden");
					} else {
						output.WriteLine($"(line {sp.StartLine}, col {sp.StartColumn}) to (line {sp.EndLine}, col {sp.EndColumn}) in {sp.Document?.Url}");
					}
					nextSequencePointIndex++;
				}
			}*/
			instruction.WriteTo(output);
		}
	}
}
