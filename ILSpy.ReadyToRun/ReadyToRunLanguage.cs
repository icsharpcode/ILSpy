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
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using Iced.Intel;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Solution;
using ICSharpCode.Decompiler.TypeSystem;
using ILCompiler.Reflection.ReadyToRun;

namespace ICSharpCode.ILSpy.ReadyToRun
{
	[Export(typeof(Language))]
	internal class ReadyToRunLanguage : Language
	{
		private static readonly ConditionalWeakTable<PEFile, R2RReaderCacheEntry> r2rReaders = new ConditionalWeakTable<PEFile, R2RReaderCacheEntry>();
		public override string Name => "ReadyToRun";

		public override string FileExtension {
			get { return ".asm"; }
		}

		public override ProjectId DecompileAssembly(LoadedAssembly assembly, ITextOutput output, DecompilationOptions options)
		{
			PEFile module = assembly.GetPEFileOrNull();
			R2RReaderCacheEntry r2rReaderCacheEntry = GetReader(assembly, module);
			if (r2rReaderCacheEntry.r2rReader == null) {
				WriteCommentLine(output, r2rReaderCacheEntry.failureReason);
			} else {
				R2RReader reader = r2rReaderCacheEntry.r2rReader;
				WriteCommentLine(output, "TODO - display ready to run information");
				// TODO: display other header information
				foreach (var method in reader.R2RMethods) {
					WriteCommentLine(output, method.SignatureString);
				}
			}

			return base.DecompileAssembly(assembly, output, options);
		}

		public override void DecompileMethod(IMethod method, ITextOutput output, DecompilationOptions options)
		{
			PEFile module = method.ParentModule.PEFile;
			R2RReaderCacheEntry r2rReaderCacheEntry = GetReader(module.GetLoadedAssembly(), module);
			if (r2rReaderCacheEntry.r2rReader == null) {
				WriteCommentLine(output, r2rReaderCacheEntry.failureReason);
			} else {
				R2RReader reader = r2rReaderCacheEntry.r2rReader;
				int bitness = -1;
				if (reader.Machine == Machine.Amd64) {
					bitness = 64;
				} else {
					Debug.Assert(reader.Machine == Machine.I386);
					bitness = 32;
				}
				foreach (var m in reader.R2RMethods) {
					if (m.MethodHandle == method.MetadataToken) {
						// TODO: Indexing
						foreach (RuntimeFunction runtimeFunction in m.RuntimeFunctions) {
							WriteCommentLine(output, m.SignatureString);
							byte[] code = new byte[runtimeFunction.Size];
							for (int i = 0; i < runtimeFunction.Size; i++) {
								code[i] = reader.Image[reader.GetOffset(runtimeFunction.StartAddress) + i];
							}
							Disassemble(output, code, bitness, (ulong)runtimeFunction.StartAddress);
							output.WriteLine();
						}
					}
				}
			}
		}

		public override void WriteCommentLine(ITextOutput output, string comment)
		{
			output.WriteLine("; " + comment);
		}

		private void Disassemble(ITextOutput output, byte[] codeBytes, int bitness, ulong address)
		{
			// TODO: Decorate the disassembly with Unwind, GC and debug info
			var codeReader = new ByteArrayCodeReader(codeBytes);
			var decoder = Decoder.Create(bitness, codeReader);
			decoder.IP = address;
			ulong endRip = decoder.IP + (uint)codeBytes.Length;

			var instructions = new InstructionList();
			while (decoder.IP < endRip) {
				decoder.Decode(out instructions.AllocUninitializedElement());
			}

			string disassemblyFormat = ReadyToRunOptions.GetDisassemblyFormat(null);
			Formatter formatter = null;
			if (disassemblyFormat.Equals(ReadyToRunOptions.intel)) {
				formatter = new NasmFormatter();
			} else {
				Debug.Assert(disassemblyFormat.Equals(ReadyToRunOptions.gas));
				formatter = new GasFormatter();
			}
			formatter.Options.DigitSeparator = "`";
			formatter.Options.FirstOperandCharIndex = 10;
			var tempOutput = new StringBuilderFormatterOutput();
			foreach (var instr in instructions) {
				formatter.Format(instr, tempOutput);
				output.Write(instr.IP.ToString("X16"));
				output.Write(" ");
				int instrLen = instr.ByteLength;
				int byteBaseIndex = (int)(instr.IP - address);
				for (int i = 0; i < instrLen; i++)
					output.Write(codeBytes[byteBaseIndex + i].ToString("X2"));
				int missingBytes = 10 - instrLen;
				for (int i = 0; i < missingBytes; i++)
					output.Write("  ");
				output.Write(" ");
				output.WriteLine(tempOutput.ToStringAndReset());
			}
		}

		private R2RReaderCacheEntry GetReader(LoadedAssembly assembly, PEFile module)
		{
			R2RReaderCacheEntry result;
			lock (r2rReaders) {
				if (!r2rReaders.TryGetValue(module, out result)) {
					result = new R2RReaderCacheEntry();
					try {
						// TODO: avoid eager parsing 
						result.r2rReader = new R2RReader(new R2RAssemblyResolver(assembly), module.Metadata, module.Reader, module.FileName);
						if (result.r2rReader.Machine != Machine.Amd64 && result.r2rReader.Machine != Machine.I386) {
							result.failureReason = $"Architecture {result.r2rReader.Machine} is not currently supported.";
							result.r2rReader = null;
						}
					} catch (BadImageFormatException e) {
						result.failureReason = e.Message;
					}
					r2rReaders.Add(module, result);
				}
			}
			return result;
		}

		private class R2RAssemblyResolver : ILCompiler.Reflection.ReadyToRun.IAssemblyResolver
		{
			private LoadedAssembly loadedAssembly;
			public R2RAssemblyResolver(LoadedAssembly loadedAssembly)
			{
				this.loadedAssembly = loadedAssembly;
			}
			public bool Naked => false;

			public bool SignatureBinary => false;

			public bool InlineSignatureBinary => false;

			public MetadataReader FindAssembly(MetadataReader metadataReader, AssemblyReferenceHandle assemblyReferenceHandle, string parentFile)
			{
				LoadedAssembly loadedAssembly = this.loadedAssembly.LookupReferencedAssembly(new Decompiler.Metadata.AssemblyReference(metadataReader, assemblyReferenceHandle));
				return loadedAssembly?.GetPEFileOrNull()?.Metadata;
			}
		}

		private class R2RReaderCacheEntry
		{
			public R2RReader r2rReader;
			public string failureReason;
		}
	}
}