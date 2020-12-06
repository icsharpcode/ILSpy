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

// #define STRESS

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;

using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Solution;
using ICSharpCode.Decompiler.TypeSystem;

using ILCompiler.Reflection.ReadyToRun;

namespace ICSharpCode.ILSpy.ReadyToRun
{
#if STRESS
	class DummyOutput : ITextOutput
	{
		public string IndentationString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public void Indent()
		{
		}

		public void MarkFoldEnd()
		{
		}

		public void MarkFoldStart(string collapsedText = "...", bool defaultCollapsed = false)
		{
		}

		public void Unindent()
		{
		}

		public void Write(char ch)
		{
		}

		public void Write(string text)
		{
		}

		public void WriteLine()
		{
		}

		public void WriteLocalReference(string text, object reference, bool isDefinition = false)
		{
		}

		public void WriteReference(OpCodeInfo opCode, bool omitSuffix = false)
		{
		}

		public void WriteReference(PEFile module, Handle handle, string text, string protocol = "decompile", bool isDefinition = false)
		{
		}

		public void WriteReference(IType type, string text, bool isDefinition = false)
		{
		}

		public void WriteReference(IMember member, string text, bool isDefinition = false)
		{
		}
	}
#endif

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
			if (cacheEntry.readyToRunReader == null)
			{
				WriteCommentLine(output, cacheEntry.failureReason);
			}
			else
			{
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
			if (cacheEntry.readyToRunReader == null)
			{
				WriteCommentLine(output, cacheEntry.failureReason);
			}
			else
			{
				ReadyToRunReader reader = cacheEntry.readyToRunReader;
				int bitness = -1;
				if (reader.Machine == Machine.Amd64)
				{
					bitness = 64;
				}
				else
				{
					Debug.Assert(reader.Machine == Machine.I386);
					bitness = 32;
				}
				if (cacheEntry.methodMap == null)
				{
					cacheEntry.methodMap = reader.Methods.ToList()
						.GroupBy(m => m.MethodHandle)
						.ToDictionary(g => g.Key, g => g.ToArray());
				}
				bool showMetadataTokens = ILSpy.Options.DisplaySettingsPanel.CurrentDisplaySettings.ShowMetadataTokens;
				bool showMetadataTokensInBase10 = ILSpy.Options.DisplaySettingsPanel.CurrentDisplaySettings.ShowMetadataTokensInBase10;
#if STRESS
				output = new DummyOutput();
				{
					foreach (var readyToRunMethod in reader.Methods)
					{ 
#else
				if (cacheEntry.methodMap.TryGetValue(method.MetadataToken, out var methods))
				{
					foreach (var readyToRunMethod in methods)
					{
#endif
						foreach (RuntimeFunction runtimeFunction in readyToRunMethod.RuntimeFunctions)
						{
							new ReadyToRunDisassembler(output, reader, runtimeFunction).Disassemble(method.ParentModule.PEFile, bitness, (ulong)runtimeFunction.StartAddress, showMetadataTokens, showMetadataTokensInBase10);
						}
					}
				}
			}
		}

		public override RichText GetRichTextTooltip(IEntity entity)
		{
			return Languages.ILLanguage.GetRichTextTooltip(entity);
		}

		private ReadyToRunReaderCacheEntry GetReader(LoadedAssembly assembly, PEFile module)
		{
			ReadyToRunReaderCacheEntry result;
			lock (readyToRunReaders)
			{
				if (!readyToRunReaders.TryGetValue(module, out result))
				{
					result = new ReadyToRunReaderCacheEntry();
					try
					{
						result.readyToRunReader = new ReadyToRunReader(new ReadyToRunAssemblyResolver(assembly), new StandaloneAssemblyMetadata(module.Reader), module.Reader, module.FileName);
						if (result.readyToRunReader.Machine != Machine.Amd64 && result.readyToRunReader.Machine != Machine.I386)
						{
							result.failureReason = $"Architecture {result.readyToRunReader.Machine} is not currently supported.";
							result.readyToRunReader = null;
						}
					}
					catch (BadImageFormatException e)
					{
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

			public IAssemblyMetadata FindAssembly(MetadataReader metadataReader, AssemblyReferenceHandle assemblyReferenceHandle, string parentFile)
			{
				LoadedAssembly loadedAssembly = this.loadedAssembly.LookupReferencedAssembly(new Decompiler.Metadata.AssemblyReference(metadataReader, assemblyReferenceHandle));
				PEReader reader = loadedAssembly?.GetPEFileOrNull()?.Reader;
				return reader == null ? null : new StandaloneAssemblyMetadata(reader);
			}

			public IAssemblyMetadata FindAssembly(string simpleName, string parentFile)
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
			public Dictionary<EntityHandle, ReadyToRunMethod[]> methodMap;
		}
	}
}
