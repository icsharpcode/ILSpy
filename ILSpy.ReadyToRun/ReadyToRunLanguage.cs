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
using System.IO;
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
using ICSharpCode.ILSpyX;

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

		public void MarkFoldStart(string collapsedText = "...", bool defaultCollapsed = false, bool isDefinition = false)
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

		public override void WriteCommentLine(ITextOutput output, string comment)
		{
			output.WriteLine("; " + comment);
		}

		public override ProjectId DecompileAssembly(LoadedAssembly assembly, ITextOutput output, DecompilationOptions options)
		{
			PEFile module = assembly.GetPEFileAsync().GetAwaiter().GetResult();
			ReadyToRunReaderCacheEntry cacheEntry = GetReader(assembly, module);
			if (cacheEntry.readyToRunReader == null)
			{
				WriteCommentLine(output, cacheEntry.failureReason);
			}
			else
			{
				ReadyToRunReader reader = cacheEntry.readyToRunReader;
				WriteCommentLine(output, $"Machine                  : {reader.Machine}");
				WriteCommentLine(output, $"OperatingSystem          : {reader.OperatingSystem}");
				WriteCommentLine(output, $"CompilerIdentifier       : {reader.CompilerIdentifier}");
				if (reader.OwnerCompositeExecutable != null)
				{
					WriteCommentLine(output, $"OwnerCompositeExecutable : {reader.OwnerCompositeExecutable}");
				}
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
					IEnumerable<ReadyToRunMethod> readyToRunMethods = null;
					if (cacheEntry.compositeReadyToRunReader == null)
					{
						readyToRunMethods = reader.Methods;
					}
					else
					{
						readyToRunMethods = cacheEntry.compositeReadyToRunReader.Methods
							.Where(m => {
								MetadataReader mr = m.ComponentReader.MetadataReader;
								return string.Equals(mr.GetString(mr.GetAssemblyDefinition().Name), method.ParentModule.Name, StringComparison.OrdinalIgnoreCase);
							});
					}
					cacheEntry.methodMap = readyToRunMethods.ToList()
							.GroupBy(m => m.MethodHandle)
							.ToDictionary(g => g.Key, g => g.ToArray());
				}
				var displaySettings = MainWindow.Instance.CurrentDisplaySettings;
				bool showMetadataTokens = displaySettings.ShowMetadataTokens;
				bool showMetadataTokensInBase10 = displaySettings.ShowMetadataTokensInBase10;
#if STRESS
				ITextOutput originalOutput = output;
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
							PEFile file = null;
							ReadyToRunReader disassemblingReader = null;
							if (cacheEntry.compositeReadyToRunReader == null)
							{
								disassemblingReader = reader;
								file = method.ParentModule.PEFile;
							}
							else
							{
								disassemblingReader = cacheEntry.compositeReadyToRunReader;
								file = ((IlSpyAssemblyMetadata)readyToRunMethod.ComponentReader).Module;
							}

							new ReadyToRunDisassembler(output, disassemblingReader, runtimeFunction).Disassemble(file, bitness, (ulong)runtimeFunction.StartAddress, showMetadataTokens, showMetadataTokensInBase10);
						}
					}
				}
#if STRESS
				output = originalOutput;
				output.WriteLine("Passed");
#endif
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
						else if (result.readyToRunReader.OwnerCompositeExecutable != null)
						{
							string compositePath = Path.Combine(Path.GetDirectoryName(module.FileName), result.readyToRunReader.OwnerCompositeExecutable);
							result.compositeReadyToRunReader = new ReadyToRunReader(new ReadyToRunAssemblyResolver(assembly), compositePath);
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
			private Decompiler.Metadata.IAssemblyResolver assemblyResolver;

			public ReadyToRunAssemblyResolver(LoadedAssembly loadedAssembly)
			{
				this.loadedAssembly = loadedAssembly;
				assemblyResolver = loadedAssembly.GetAssemblyResolver();
			}

			public IAssemblyMetadata FindAssembly(MetadataReader metadataReader, AssemblyReferenceHandle assemblyReferenceHandle, string parentFile)
			{
				return GetAssemblyMetadata(assemblyResolver.Resolve(new Decompiler.Metadata.AssemblyReference(metadataReader, assemblyReferenceHandle)));
			}

			public IAssemblyMetadata FindAssembly(string simpleName, string parentFile)
			{
				return GetAssemblyMetadata(assemblyResolver.ResolveModule(loadedAssembly.GetPEFileOrNull(), simpleName + ".dll"));
			}

			private IAssemblyMetadata GetAssemblyMetadata(PEFile module)
			{
				if (module.Reader == null)
				{
					return null;
				}
				else
				{
					return new IlSpyAssemblyMetadata(module);
				}
			}
		}

		private class IlSpyAssemblyMetadata : StandaloneAssemblyMetadata
		{
			public PEFile Module { get; private set; }

			public IlSpyAssemblyMetadata(PEFile module) : base(module.Reader)
			{
				Module = module;
			}
		}

		private class ReadyToRunReaderCacheEntry
		{
			public ReadyToRunReader readyToRunReader;
			public ReadyToRunReader compositeReadyToRunReader;
			public string failureReason;
			public Dictionary<EntityHandle, ReadyToRunMethod[]> methodMap;
		}
	}
}
