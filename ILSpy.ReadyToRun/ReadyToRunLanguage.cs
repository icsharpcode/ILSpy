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
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Resources;
using System.Runtime.CompilerServices;
using Iced.Intel;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Solution;
using ICSharpCode.Decompiler.TypeSystem;
using ILCompiler.Reflection.ReadyToRun;
using ILCompiler.Reflection.ReadyToRun.Amd64;

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
				if (cacheEntry.methodMap == null) {
					cacheEntry.methodMap = reader.Methods.Values
						.SelectMany(m => m)
						.GroupBy(m => m.MethodHandle)
						.ToDictionary(g => g.Key, g => g.ToArray());
				}
				bool showMetadataTokens = ILSpy.Options.DisplaySettingsPanel.CurrentDisplaySettings.ShowMetadataTokens;
				bool showMetadataTokensInBase10 = ILSpy.Options.DisplaySettingsPanel.CurrentDisplaySettings.ShowMetadataTokensInBase10;
				if (cacheEntry.methodMap.TryGetValue(method.MetadataToken, out var methods)) {
					foreach (var readyToRunMethod in methods) {
						foreach (RuntimeFunction runtimeFunction in readyToRunMethod.RuntimeFunctions) {
							Disassemble(method.ParentModule.PEFile, output, reader, readyToRunMethod, runtimeFunction, bitness, (ulong)runtimeFunction.StartAddress, showMetadataTokens, showMetadataTokensInBase10);
						}
					}
				}
			}
		}

		public override void WriteCommentLine(ITextOutput output, string comment)
		{
			output.WriteLine("; " + comment);
		}

		private Dictionary<VarLocType, HashSet<(DebugInfo debugInfo, NativeVarInfo varLoc)>> WriteDebugInfo(ReadyToRunMethod readyToRunMethod, ITextOutput output)
		{
			Dictionary<VarLocType, HashSet<(DebugInfo debugInfo, NativeVarInfo varLoc)>> debugInfoDict = new Dictionary<VarLocType, HashSet<(DebugInfo debugInfo, NativeVarInfo varLoc)>>();
			IReadOnlyList<RuntimeFunction> runTimeList = readyToRunMethod.RuntimeFunctions;
			foreach (RuntimeFunction runtimeFunction in runTimeList) {
				DebugInfo debugInfo = runtimeFunction.DebugInfo;
				if (debugInfo != null && debugInfo.BoundsList.Count > 0) {
					for (int i = 0; i < debugInfo.VariablesList.Count; ++i) {
						var varLoc = debugInfo.VariablesList[i];
						try {
							var typeSet = new HashSet<ValueTuple<DebugInfo, NativeVarInfo>>();
							bool found = debugInfoDict.TryGetValue(varLoc.VariableLocation.VarLocType, out typeSet);
							if (found) {
								(DebugInfo debugInfo, NativeVarInfo varLoc) newTuple = (debugInfo, varLoc);
								typeSet.Add(newTuple);
							} else {
								typeSet = new HashSet<ValueTuple<DebugInfo, NativeVarInfo>>();
								debugInfoDict.Add(varLoc.VariableLocation.VarLocType, typeSet);
								(DebugInfo debugInfo, NativeVarInfo varLoc) newTuple = (debugInfo, varLoc);
								typeSet.Add(newTuple);
							}
						} catch (ArgumentNullException) {
							output.WriteLine("Failed to find hash set of Debug info type");
						}

						if (varLoc.VariableLocation.VarLocType != VarLocType.VLT_REG && varLoc.VariableLocation.VarLocType != VarLocType.VLT_STK
							&& varLoc.VariableLocation.VarLocType != VarLocType.VLT_STK_BYREF) {
							output.WriteLine($"    Variable Number: {varLoc.VariableNumber}");
							output.WriteLine($"    Start Offset: 0x{varLoc.StartOffset:X}");
							output.WriteLine($"    End Offset: 0x{varLoc.EndOffset:X}");
							output.WriteLine($"    Loc Type: {varLoc.VariableLocation.VarLocType}");
							switch (varLoc.VariableLocation.VarLocType) {
								case VarLocType.VLT_REG:
								case VarLocType.VLT_REG_FP:
								case VarLocType.VLT_REG_BYREF:
									output.WriteLine($"    Register: {DebugInfo.GetPlatformSpecificRegister(debugInfo.Machine, varLoc.VariableLocation.Data1)}");
									break;
								case VarLocType.VLT_STK:
								case VarLocType.VLT_STK_BYREF:
									output.WriteLine($"    Base Register: {DebugInfo.GetPlatformSpecificRegister(debugInfo.Machine, varLoc.VariableLocation.Data1)}");
									output.WriteLine($"    Stack Offset: {varLoc.VariableLocation.Data2}");
									break;
								case VarLocType.VLT_REG_REG:
									output.WriteLine($"    Register 1: {DebugInfo.GetPlatformSpecificRegister(debugInfo.Machine, varLoc.VariableLocation.Data1)}");
									output.WriteLine($"    Register 2: {DebugInfo.GetPlatformSpecificRegister(debugInfo.Machine, varLoc.VariableLocation.Data2)}");
									break;
								case VarLocType.VLT_REG_STK:
									output.WriteLine($"    Register: {DebugInfo.GetPlatformSpecificRegister(debugInfo.Machine, varLoc.VariableLocation.Data1)}");
									output.WriteLine($"    Base Register: {DebugInfo.GetPlatformSpecificRegister(debugInfo.Machine, varLoc.VariableLocation.Data2)}");
									output.WriteLine($"    Stack Offset: {varLoc.VariableLocation.Data3}");
									break;
								case VarLocType.VLT_STK_REG:
									output.WriteLine($"    Stack Offset: {varLoc.VariableLocation.Data1}");
									output.WriteLine($"    Base Register: {DebugInfo.GetPlatformSpecificRegister(debugInfo.Machine, varLoc.VariableLocation.Data2)}");
									output.WriteLine($"    Register: {DebugInfo.GetPlatformSpecificRegister(debugInfo.Machine, varLoc.VariableLocation.Data3)}");
									break;
								case VarLocType.VLT_STK2:
									output.WriteLine($"    Base Register: {DebugInfo.GetPlatformSpecificRegister(debugInfo.Machine, varLoc.VariableLocation.Data1)}");
									output.WriteLine($"    Stack Offset: {varLoc.VariableLocation.Data2}");
									break;
								case VarLocType.VLT_FPSTK:
									output.WriteLine($"    Offset: {DebugInfo.GetPlatformSpecificRegister(debugInfo.Machine, varLoc.VariableLocation.Data1)}");
									break;
								case VarLocType.VLT_FIXED_VA:
									output.WriteLine($"    Offset: {DebugInfo.GetPlatformSpecificRegister(debugInfo.Machine, varLoc.VariableLocation.Data1)}");
									break;
								default:
									output.WriteLine("WRN: Unexpected variable location type");
									break;
							}
							output.WriteLine("");
						}
					}
				}
			}
			return debugInfoDict;
		}

		private Dictionary<ulong, UnwindCode> WriteUnwindInfo(RuntimeFunction runtimeFunction, ITextOutput output)
		{
			Dictionary<ulong, UnwindCode> unwindCodes = new Dictionary<ulong, UnwindCode>();
			if (runtimeFunction.UnwindInfo is UnwindInfo amd64UnwindInfo) {
				string parsedFlags = "";
				if ((amd64UnwindInfo.Flags & (int)UnwindFlags.UNW_FLAG_EHANDLER) != 0) {
					parsedFlags += " EHANDLER";
				}
				if ((amd64UnwindInfo.Flags & (int)UnwindFlags.UNW_FLAG_UHANDLER) != 0) {
					parsedFlags += " UHANDLER";
				}
				if ((amd64UnwindInfo.Flags & (int)UnwindFlags.UNW_FLAG_CHAININFO) != 0) {
					parsedFlags += " CHAININFO";
				}
				if (parsedFlags.Length == 0) {
					parsedFlags = " NHANDLER";
				}
				WriteCommentLine(output, $"UnwindInfo:");
				WriteCommentLine(output, $"Version:            {amd64UnwindInfo.Version}");
				WriteCommentLine(output, $"Flags:              0x{amd64UnwindInfo.Flags:X2}{parsedFlags}");
				WriteCommentLine(output, $"FrameRegister:      {((amd64UnwindInfo.FrameRegister == 0) ? "none" : amd64UnwindInfo.FrameRegister.ToString().ToLower())}");
				for (int unwindCodeIndex = 0; unwindCodeIndex < amd64UnwindInfo.CountOfUnwindCodes; unwindCodeIndex++) {
					unwindCodes.Add((ulong)(amd64UnwindInfo.UnwindCodes[unwindCodeIndex].CodeOffset), amd64UnwindInfo.UnwindCodes[unwindCodeIndex]);
				}
			}
			return unwindCodes;
		}

		private void Disassemble(PEFile currentFile, ITextOutput output, ReadyToRunReader reader, ReadyToRunMethod readyToRunMethod, RuntimeFunction runtimeFunction, int bitness, ulong address, bool showMetadataTokens, bool showMetadataTokensInBase10)
		{
			// TODO: Decorate the disassembly with GCInfo
			WriteCommentLine(output, readyToRunMethod.SignatureString);

			Dictionary<ulong, UnwindCode> unwindInfo = null;
			if (ReadyToRunOptions.GetIsShowUnwindInfo(null) && bitness == 64) {
				unwindInfo = WriteUnwindInfo(runtimeFunction, output);
			}

			bool isShowDebugInfo = ReadyToRunOptions.GetIsShowDebugInfo(null);
			Dictionary<VarLocType, HashSet<ValueTuple<DebugInfo, NativeVarInfo>>> debugInfo = null;
			if (isShowDebugInfo) {
				debugInfo = WriteDebugInfo(readyToRunMethod, output);
			}

			byte[] codeBytes = new byte[runtimeFunction.Size];
			for (int i = 0; i < runtimeFunction.Size; i++) {
				codeBytes[i] = reader.Image[reader.GetOffset(runtimeFunction.StartAddress) + i];
			}

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
			var tempOutput = new StringOutput();
			ulong baseInstrIP = instructions[0].IP;
			foreach (var instr in instructions) {
				int byteBaseIndex = (int)(instr.IP - address);
				if (isShowDebugInfo && runtimeFunction.DebugInfo != null) {
					foreach (var bound in runtimeFunction.DebugInfo.BoundsList) {
						if (bound.NativeOffset == byteBaseIndex) {
							if (bound.ILOffset == (uint)DebugInfoBoundsType.Prolog) {
								WriteCommentLine(output, "Prolog");
							} else if (bound.ILOffset == (uint)DebugInfoBoundsType.Epilog) {
								WriteCommentLine(output, "Epilog");
							} else {
								WriteCommentLine(output, $"IL_{bound.ILOffset:x4}");
							}
						}
					}
				}
				formatter.Format(instr, tempOutput);
				output.Write(instr.IP.ToString("X16"));
				output.Write(" ");
				int instrLen = instr.Length;
				for (int i = 0; i < instrLen; i++) {
					output.Write(codeBytes[byteBaseIndex + i].ToString("X2"));
				}
				int missingBytes = 10 - instrLen;
				for (int i = 0; i < missingBytes; i++) {
					output.Write("  ");
				}
				output.Write(" ");
				output.Write(tempOutput.ToStringAndReset());
				DecorateUnwindInfo(output, unwindInfo, baseInstrIP, instr);
				DecorateDebugInfo(output, instr, debugInfo, baseInstrIP);
				DecorateCallSite(currentFile, output, reader, showMetadataTokens, showMetadataTokensInBase10, instr);
			}
			output.WriteLine();
		}

		private static void DecorateUnwindInfo(ITextOutput output, Dictionary<ulong, UnwindCode> unwindInfo, ulong baseInstrIP, Instruction instr)
		{
			ulong nextInstructionOffset = instr.NextIP - baseInstrIP;
			if (unwindInfo != null && unwindInfo.ContainsKey(nextInstructionOffset)) {
				UnwindCode unwindCode = unwindInfo[nextInstructionOffset];
				output.Write($" ; {unwindCode.UnwindOp}({unwindCode.OpInfoStr})");
			}
		}

		private static void DecorateDebugInfo(ITextOutput output, Instruction instr, Dictionary<VarLocType, HashSet<(DebugInfo debugInfo, NativeVarInfo varLoc)>> debugInfoDict, ulong baseInstrIP)
		{
			if (debugInfoDict != null) {
				InstructionInfoFactory factory = new InstructionInfoFactory();
				InstructionInfo info = factory.GetInfo(instr);
				HashSet<ValueTuple<DebugInfo, NativeVarInfo>> stkSet = new HashSet<ValueTuple<DebugInfo, NativeVarInfo>>();
				if (debugInfoDict.ContainsKey(VarLocType.VLT_STK)) {
					stkSet.UnionWith(debugInfoDict[VarLocType.VLT_STK]);
				}
				if (debugInfoDict.ContainsKey(VarLocType.VLT_STK_BYREF)) {
					stkSet.UnionWith(debugInfoDict[VarLocType.VLT_STK_BYREF]);
				}
				if (stkSet != null) {
					foreach (UsedMemory usedMemInfo in info.GetUsedMemory()) { //for each time a [register +- value] is used
						foreach ((DebugInfo debugInfo, NativeVarInfo varLoc) tuple in stkSet) { //for each VLT_STK variable
							var debugInfo = tuple.debugInfo;
							var varInfo = tuple.varLoc;
							int stackOffset = varInfo.VariableLocation.Data2;
							ulong adjOffset;
							bool negativeOffset;
							if (stackOffset < 0) {
								int absValue = -1 * stackOffset;
								adjOffset = ulong.MaxValue - (ulong)absValue + 1;
								negativeOffset = true;
							} else {
								adjOffset = (ulong)stackOffset;
								negativeOffset = false;
							}
							if (varInfo.StartOffset < instr.IP - baseInstrIP && varInfo.EndOffset > instr.IP - baseInstrIP &&
								DebugInfo.GetPlatformSpecificRegister(debugInfo.Machine, varInfo.VariableLocation.Data1) == usedMemInfo.Base.ToString() &&
								adjOffset == usedMemInfo.Displacement) {
								output.Write($"; [{usedMemInfo.Base.ToString().ToLower()}{(negativeOffset ? '-' : '+')}{Math.Abs(stackOffset):X}h] = {varInfo.Variable.Type} {varInfo.Variable.Index}");
							}
						}
					}
				}
				HashSet<ValueTuple<DebugInfo, NativeVarInfo>> regSet = new HashSet<ValueTuple<DebugInfo, NativeVarInfo>>();
				if (debugInfoDict.ContainsKey(VarLocType.VLT_REG)) {
					regSet.UnionWith(debugInfoDict[VarLocType.VLT_REG]);
				}
				if (debugInfoDict.ContainsKey(VarLocType.VLT_REG_BYREF)) {
					regSet.UnionWith(debugInfoDict[VarLocType.VLT_REG_BYREF]);
				}
				if (debugInfoDict.ContainsKey(VarLocType.VLT_REG_FP)) {
					regSet.UnionWith(debugInfoDict[VarLocType.VLT_REG_FP]);
				}
				if (regSet != null) {
					foreach (UsedRegister usedMemInfo in info.GetUsedRegisters()) {
						foreach ((DebugInfo debugInfo, NativeVarInfo varLoc) tuple in regSet) {
							var debugInfo = tuple.debugInfo;
							var varInfo = tuple.varLoc;
							if (varInfo.StartOffset <= (instr.IP - baseInstrIP) && (instr.IP - baseInstrIP) < varInfo.EndOffset &&
								DebugInfo.GetPlatformSpecificRegister(debugInfo.Machine, varInfo.VariableLocation.Data1) == usedMemInfo.Register.ToString()) {
								output.Write($"; {usedMemInfo.Register.ToString().ToLower()} = {varInfo.Variable.Type} {varInfo.Variable.Index}");
							}
						}
					}
				}
			}
		}

		private static void DecorateCallSite(PEFile currentFile, ITextOutput output, ReadyToRunReader reader, bool showMetadataTokens, bool showMetadataTokensInBase10, Instruction instr)
		{
			int importCellAddress = (int)instr.IPRelativeMemoryAddress;
			if (instr.IsCallNearIndirect && reader.ImportCellNames.ContainsKey(importCellAddress)) {
				output.Write(" ; ");
				ReadyToRunSignature signature = reader.ImportSignatures[(int)instr.IPRelativeMemoryAddress];
				switch (signature) {
					case MethodDefEntrySignature methodDefSignature:
						var methodDefToken = MetadataTokens.EntityHandle(unchecked((int)methodDefSignature.MethodDefToken));
						if (showMetadataTokens) {
							if (showMetadataTokensInBase10) {
								output.WriteReference(currentFile, methodDefToken, $"({MetadataTokens.GetToken(methodDefToken)}) ", "metadata");
							} else {
								output.WriteReference(currentFile, methodDefToken, $"({MetadataTokens.GetToken(methodDefToken):X8}) ", "metadata");
							}
						}
						methodDefToken.WriteTo(currentFile, output, Decompiler.Metadata.GenericContext.Empty);
						break;
					case MethodRefEntrySignature methodRefSignature:
						var methodRefToken = MetadataTokens.EntityHandle(unchecked((int)methodRefSignature.MethodRefToken));
						if (showMetadataTokens) {
							if (showMetadataTokensInBase10) {
								output.WriteReference(currentFile, methodRefToken, $"({MetadataTokens.GetToken(methodRefToken)}) ", "metadata");
							} else {
								output.WriteReference(currentFile, methodRefToken, $"({MetadataTokens.GetToken(methodRefToken):X8}) ", "metadata");
							}
						}
						methodRefToken.WriteTo(currentFile, output, Decompiler.Metadata.GenericContext.Empty);
						break;
					default:
						output.WriteLine(reader.ImportCellNames[importCellAddress]);
						break;
				}
				output.WriteLine();
			} else {
				output.WriteLine();
			}
		}

		public override RichText GetRichTextTooltip(IEntity entity)
		{
			return Languages.ILLanguage.GetRichTextTooltip(entity);
		}

		private ReadyToRunReaderCacheEntry GetReader(LoadedAssembly assembly, PEFile module)
		{
			ReadyToRunReaderCacheEntry result;
			lock (readyToRunReaders) {
				if (!readyToRunReaders.TryGetValue(module, out result)) {
					result = new ReadyToRunReaderCacheEntry();
					try {
						result.readyToRunReader = new ReadyToRunReader(new ReadyToRunAssemblyResolver(assembly), new StandaloneAssemblyMetadata(module.Reader), module.Reader, module.FileName);
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
