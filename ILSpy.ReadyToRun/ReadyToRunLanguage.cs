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
		private static readonly ConditionalWeakTable<PEFile, ReadyToRunReaderCacheEntry> readyToRunReaders = new ConditionalWeakTable<PEFile, ReadyToRunReaderCacheEntry>();
		public override string Name => "ReadyToRun";

		public override string FileExtension {
			get { return ".asm"; }
		}

		public override ProjectId DecompileAssembly(LoadedAssembly assembly, ITextOutput output, DecompilationOptions options)
		{
			PEFile module = assembly.GetPEFileOrNull();
			ReadyToRunReaderCacheEntry cacheEntry = GetReader(assembly, module);
			if (cacheEntry.readyToRunReader == null) {
				WriteCommentLine(output, cacheEntry.failureReason);
			} else {
				ReadyToRunReader reader = cacheEntry.readyToRunReader;
				WriteCommentLine(output, reader.Machine.ToString());
				WriteCommentLine(output, reader.OperatingSystem.ToString());
				WriteCommentLine(output, reader.CompilerIdentifier);
				WriteCommentLine(output, "TODO - display more header information");
			}

			return base.DecompileAssembly(assembly, output, options);
		}

		public override void DecompileMethod(IMethod method, ITextOutput output, DecompilationOptions options)
		{
			PEFile module = method.ParentModule.PEFile;
			ReadyToRunReaderCacheEntry cacheEntry = GetReader(module.GetLoadedAssembly(), module);
			if (cacheEntry.readyToRunReader == null) {
				WriteCommentLine(output, cacheEntry.failureReason);
			} else {
				ReadyToRunReader reader = cacheEntry.readyToRunReader;
				int bitness = -1;
				if (reader.Machine == Machine.Amd64) {
					bitness = 64;
				} else {
					Debug.Assert(reader.Machine == Machine.I386);
					bitness = 32;
				}
				foreach (ReadyToRunMethod readyToRunMethod in reader.Methods) {
					if (readyToRunMethod.MethodHandle == method.MetadataToken) {
						// TODO: Indexing
						foreach (RuntimeFunction runtimeFunction in readyToRunMethod.RuntimeFunctions) {
							WriteCommentLine(output, readyToRunMethod.SignatureString);
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

		private ReadyToRunReaderCacheEntry GetReader(LoadedAssembly assembly, PEFile module)
		{
			ReadyToRunReaderCacheEntry result;
			lock (readyToRunReaders) {
				if (!readyToRunReaders.TryGetValue(module, out result)) {
					result = new ReadyToRunReaderCacheEntry();
					try {
						// TODO: avoid eager parsing 
						result.readyToRunReader = new ReadyToRunReader(new ReadyToRunAssemblyResolver(assembly), module.Metadata, module.Reader, module.FileName);
						if (result.readyToRunReader.Machine != Machine.Amd64 && result.readyToRunReader.Machine != Machine.I386) {
							result.failureReason = $"Architecture {result.readyToRunReader.Machine} is not currently supported.";
							result.readyToRunReader = null;
						}
					} catch (BadImageFormatException e) {
						result.failureReason = e.Message;
					}
					readyToRunReaders.Add(module, result);
				}
			}
			return result;
		}

		private class ReadyToRunAssemblyResolver : ILCompiler.Reflection.ReadyToRun.IAssemblyResolver
		{
			private LoadedAssembly loadedAssembly;
			public ReadyToRunAssemblyResolver(LoadedAssembly loadedAssembly)
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

			public MetadataReader FindAssembly(string simpleName, string parentFile)
			{
				// This is called only for the composite R2R scenario, 
				// So it will never be called before the feature is released.
				throw new NotSupportedException("Composite R2R format is not currently supported");
			}
		}

		private class ReadyToRunReaderCacheEntry
		{
			public ReadyToRunReader readyToRunReader;
			public string failureReason;
		}
	}
}