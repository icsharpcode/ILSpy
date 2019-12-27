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
using System.ComponentModel.Composition;
using System.Reflection.PortableExecutable;
using Iced.Intel;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Solution;
using ICSharpCode.Decompiler.TypeSystem;
using ILCompiler.Reflection.ReadyToRun;

namespace ICSharpCode.ILSpy
{
	[Export(typeof(Language))]
	class ReadyToRunLanguage : Language
	{
		public override string Name => "ReadyToRun";

		public override string FileExtension {
			get { return ".asm"; }
		}

		public override ProjectId DecompileAssembly(LoadedAssembly assembly, ITextOutput output, DecompilationOptions options)
		{
			PEFile module = assembly.GetPEFileOrNull();
			// TODO: avoid eager parsing 
			R2RReader reader = new R2RReader(new R2RAssemblyResolver(), module.Metadata, module.Reader, module.FileName);

			output.WriteLine("// TODO - display ready to run information");
			// TODO: display other header information
			foreach (var method in reader.R2RMethods) {
				output.WriteLine(method.SignatureString);
			}

			return base.DecompileAssembly(assembly, output, options);
		}

		public override void DecompileMethod(IMethod method, ITextOutput output, DecompilationOptions options)
		{
			PEFile module = method.ParentModule.PEFile;
			// TODO: avoid eager parsing in R2RReader
			R2RReader reader = new R2RReader(new R2RAssemblyResolver(), module.Metadata, module.Reader, module.FileName);
			int bitness = -1;
			if (reader.Machine == Machine.Amd64) {
				bitness = 64;
			} else if (reader.Machine == Machine.I386) {
				bitness = 32;
			}
			else {
				// TODO: Architecture other than x86/amd64
				throw new NotImplementedException("");
			}
			foreach (var m in reader.R2RMethods) {
				if (m.MethodHandle == method.MetadataToken) {
					// TODO: Indexing
					foreach (RuntimeFunction runtimeFunction in m.RuntimeFunctions) {
						byte[] code = new byte[runtimeFunction.Size];
						for (int i = 0; i < runtimeFunction.Size; i++) {
							code[i] = reader.Image[reader.GetOffset(runtimeFunction.StartAddress) + i];
						}
						DecoderFormatterExample(output, code, bitness, (ulong)runtimeFunction.StartAddress);
					}

				}
			}
		}
		private void DecoderFormatterExample(ITextOutput output, byte[] exampleCode, int exampleCodeBitness, ulong exampleCodeRIP)
		{
			// TODO: Decorate the disassembly with Unwind, GC and debug info
			// You can also pass in a hex string, eg. "90 91 929394", or you can use your own CodeReader
			// reading data from a file or memory etc
			var codeBytes = exampleCode;
			var codeReader = new ByteArrayCodeReader(codeBytes);
			var decoder = Decoder.Create(exampleCodeBitness, codeReader);
			decoder.IP = exampleCodeRIP;
			ulong endRip = decoder.IP + (uint)codeBytes.Length;

			// This list is faster than List<Instruction> since it uses refs to the Instructions
			// instead of copying them (each Instruction is 32 bytes in size). It has a ref indexer,
			// and a ref iterator. Add() uses 'in' (ref readonly).
			var instructions = new InstructionList();
			while (decoder.IP < endRip) {
				// The method allocates an uninitialized element at the end of the list and
				// returns a reference to it which is initialized by Decode().
				decoder.Decode(out instructions.AllocUninitializedElement());
			}

			// Formatters: Masm*, Nasm*, Gas* (AT&T) and Intel* (XED)
			// TODO: DecompilationOptions?
			var formatter = new NasmFormatter();
			formatter.Options.DigitSeparator = "`";
			formatter.Options.FirstOperandCharIndex = 10;
			var tempOutput = new StringBuilderFormatterOutput();
			// Use InstructionList's ref iterator (C# 7.3) to prevent copying 32 bytes every iteration
			foreach (var instr in instructions) {
				// Don't use instr.ToString(), it allocates more, uses masm syntax and default options
				formatter.Format(instr, tempOutput);
				output.Write(instr.IP.ToString("X16"));
				output.Write(" ");
				int instrLen = instr.ByteLength;
				int byteBaseIndex = (int)(instr.IP - exampleCodeRIP);
				for (int i = 0; i < instrLen; i++)
					output.Write(codeBytes[byteBaseIndex + i].ToString("X2"));
				int missingBytes = 10 - instrLen;
				for (int i = 0; i < missingBytes; i++)
					output.Write("  ");
				output.Write(" ");
				output.WriteLine(tempOutput.ToStringAndReset());
			}
		}

		private class R2RAssemblyResolver : ILCompiler.Reflection.ReadyToRun.IAssemblyResolver
		{
			public bool Naked => false;

			public bool SignatureBinary => false;

			public bool InlineSignatureBinary => false;

			public string FindAssembly(string name, string filename)
			{
				throw new NotImplementedException();
			}
		}
	}
}