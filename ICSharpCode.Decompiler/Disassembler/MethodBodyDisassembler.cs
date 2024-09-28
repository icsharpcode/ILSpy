﻿// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.Collections.Immutable;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Threading;

using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.Disassembler
{
	/// <summary>
	/// Disassembles a method body.
	/// </summary>
	public class MethodBodyDisassembler
	{
		readonly ITextOutput output;
		readonly CancellationToken cancellationToken;

		/// <summary>
		/// Show .try/finally as blocks in IL code; indent loops.
		/// </summary>
		public bool DetectControlStructure { get; set; } = true;

		/// <summary>
		/// Show sequence points if debug information is loaded in Cecil.
		/// </summary>
		public bool ShowSequencePoints { get; set; }

		/// <summary>
		/// Show metadata tokens for instructions with token operands.
		/// </summary>
		public bool ShowMetadataTokens { get; set; }

		/// <summary>
		/// Show metadata tokens for instructions with token operands in base 10.
		/// </summary>
		public bool ShowMetadataTokensInBase10 { get; set; }

		/// <summary>
		/// Show raw RVA offset and bytes before each instruction.
		/// </summary>
		public bool ShowRawRVAOffsetAndBytes { get; set; }

		/// <summary>
		/// Optional provider for sequence points.
		/// </summary>
		public IDebugInfoProvider DebugInfo { get; set; }

		IList<DebugInfo.SequencePoint>? sequencePoints;
		int nextSequencePointIndex;

		// cache info
		MetadataFile module;
		MetadataReader metadata;
		MetadataGenericContext genericContext;
		DisassemblerSignatureTypeProvider signatureDecoder;

		public MethodBodyDisassembler(ITextOutput output, CancellationToken cancellationToken)
		{
			this.output = output ?? throw new ArgumentNullException(nameof(output));
			this.cancellationToken = cancellationToken;
		}

		public virtual void Disassemble(MetadataFile module, MethodDefinitionHandle handle)
		{
			this.module = module ?? throw new ArgumentNullException(nameof(module));
			metadata = module.Metadata;
			genericContext = new MetadataGenericContext(handle, module);
			signatureDecoder = new DisassemblerSignatureTypeProvider(module, output);
			var methodDefinition = metadata.GetMethodDefinition(handle);

			// start writing IL code
			output.WriteLine("// Method begins at RVA 0x{0:x4}", methodDefinition.RelativeVirtualAddress);
			if (methodDefinition.RelativeVirtualAddress == 0)
			{
				output.WriteLine("// Header size: {0}", 0);
				output.WriteLine("// Code size: {0} (0x{0:x})", 0);
				output.WriteLine(".maxstack {0}", 0);
				output.WriteLine();
				return;
			}
			MethodBodyBlock body;
			BlobReader bodyBlockReader;
			try
			{
				body = module.GetMethodBody(methodDefinition.RelativeVirtualAddress);
				bodyBlockReader = module.GetSectionData(methodDefinition.RelativeVirtualAddress).GetReader();
			}
			catch (BadImageFormatException ex)
			{
				output.WriteLine("// {0}", ex.Message);
				return;
			}
			var blob = body.GetILReader();
			int headerSize = ILParser.GetHeaderSize(bodyBlockReader);
			output.WriteLine("// Header size: {0}", headerSize);
			output.WriteLine("// Code size: {0} (0x{0:x})", blob.Length);
			output.WriteLine(".maxstack {0}", body.MaxStack);

			var entrypointHandle = MetadataTokens.MethodDefinitionHandle(module.CorHeader?.EntryPointTokenOrRelativeVirtualAddress ?? 0);
			if (handle == entrypointHandle)
				output.WriteLine(".entrypoint");

			DisassembleLocalsBlock(handle, body);
			output.WriteLine();

			sequencePoints = DebugInfo?.GetSequencePoints(handle) ?? EmptyList<DebugInfo.SequencePoint>.Instance;
			nextSequencePointIndex = 0;
			if (DetectControlStructure && blob.Length > 0)
			{
				blob.Reset();
				BitSet branchTargets = new(blob.Length);
				ILParser.SetBranchTargets(ref blob, branchTargets);
				blob.Reset();
				WriteStructureBody(new ILStructure(module, handle, genericContext, body), branchTargets, ref blob, methodDefinition.RelativeVirtualAddress + headerSize);
			}
			else
			{
				while (blob.RemainingBytes > 0)
				{
					cancellationToken.ThrowIfCancellationRequested();
					WriteInstruction(output, module, handle, ref blob, methodDefinition.RelativeVirtualAddress);
				}
				WriteExceptionHandlers(module, handle, body);
			}
			sequencePoints = null;
		}

		void DisassembleLocalsBlock(MethodDefinitionHandle method, MethodBodyBlock body)
		{
			if (body.LocalSignature.IsNil)
				return;
			output.Write(".locals");
			WriteMetadataToken(body.LocalSignature, spaceBefore: true);
			if (body.LocalVariablesInitialized)
				output.Write(" init");
			var blob = metadata.GetStandaloneSignature(body.LocalSignature);
			var signature = ImmutableArray<Action<ILNameSyntax>>.Empty;
			try
			{
				if (blob.GetKind() == StandaloneSignatureKind.LocalVariables)
				{
					signature = blob.DecodeLocalSignature(signatureDecoder, genericContext);
				}
				else
				{
					output.Write(" /* wrong signature kind */");
				}
			}
			catch (BadImageFormatException ex)
			{
				output.Write($" /* {ex.Message} */");
			}
			output.Write(' ');
			output.WriteLine("(");
			output.Indent();
			int index = 0;
			foreach (var v in signature)
			{
				output.WriteLocalReference("[" + index + "]", "loc_" + index, isDefinition: true);
				output.Write(' ');
				v(ILNameSyntax.TypeName);
				if (DebugInfo != null && DebugInfo.TryGetName(method, index, out var name))
				{
					output.Write(" " + DisassemblerHelpers.Escape(name));
				}
				if (index + 1 < signature.Length)
					output.Write(',');
				output.WriteLine();
				index++;
			}
			output.Unindent();
			output.WriteLine(")");
		}

		internal void WriteExceptionHandlers(MetadataFile module, MethodDefinitionHandle handle, MethodBodyBlock body)
		{
			this.module = module;
			metadata = module.Metadata;
			genericContext = new MetadataGenericContext(handle, module);
			signatureDecoder = new DisassemblerSignatureTypeProvider(module, output);
			var handlers = body.ExceptionRegions;
			if (!handlers.IsEmpty)
			{
				output.WriteLine();
				foreach (var eh in handlers)
				{
					eh.WriteTo(module, genericContext, output);
					output.WriteLine();
				}
			}
		}

		void WriteStructureHeader(ILStructure s)
		{
			switch (s.Type)
			{
				case ILStructureType.Loop:
					output.Write("// loop start");
					if (s.LoopEntryPointOffset >= 0)
					{
						output.Write(" (head: ");
						DisassemblerHelpers.WriteOffsetReference(output, s.LoopEntryPointOffset);
						output.Write(')');
					}
					output.WriteLine();
					break;
				case ILStructureType.Try:
					output.WriteLine(".try");
					output.WriteLine("{");
					break;
				case ILStructureType.Handler:
					switch (s.ExceptionHandler.Kind)
					{
						case ExceptionRegionKind.Filter:
							// handler block of filter block has no header
							break;
						case ExceptionRegionKind.Catch:
							output.Write("catch");
							if (!s.ExceptionHandler.CatchType.IsNil)
							{
								output.Write(' ');
								s.ExceptionHandler.CatchType.WriteTo(s.Module, output, s.GenericContext, ILNameSyntax.TypeName);
							}
							output.WriteLine();
							break;
						case ExceptionRegionKind.Finally:
							output.WriteLine("finally");
							break;
						case ExceptionRegionKind.Fault:
							output.WriteLine("fault");
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					output.WriteLine("{");
					break;
				case ILStructureType.Filter:
					output.WriteLine("filter");
					output.WriteLine("{");
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			output.Indent();
		}

		void WriteStructureBody(ILStructure s, BitSet branchTargets, ref BlobReader body, int methodRva)
		{
			bool isFirstInstructionInStructure = true;
			bool prevInstructionWasBranch = false;
			int childIndex = 0;
			while (body.RemainingBytes > 0 && body.Offset < s.EndOffset)
			{
				cancellationToken.ThrowIfCancellationRequested();
				int offset = body.Offset;
				if (childIndex < s.Children.Count && s.Children[childIndex].StartOffset <= offset && offset < s.Children[childIndex].EndOffset)
				{
					ILStructure child = s.Children[childIndex++];
					WriteStructureHeader(child);
					WriteStructureBody(child, branchTargets, ref body, methodRva);
					WriteStructureFooter(child);
				}
				else
				{
					if (!isFirstInstructionInStructure && (prevInstructionWasBranch || branchTargets[offset]))
					{
						output.WriteLine(); // put an empty line after branches, and in front of branch targets
					}
					var currentOpCode = ILParser.DecodeOpCode(ref body);
					body.Offset = offset; // reset IL stream
					WriteInstruction(output, module, s.MethodHandle, ref body, methodRva);
					prevInstructionWasBranch = currentOpCode.IsBranch()
						|| currentOpCode.IsReturn()
						|| currentOpCode == ILOpCode.Throw
						|| currentOpCode == ILOpCode.Rethrow
						|| currentOpCode == ILOpCode.Switch;
				}
				isFirstInstructionInStructure = false;
			}
		}

		void WriteStructureFooter(ILStructure s)
		{
			output.Unindent();
			switch (s.Type)
			{
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
					throw new ArgumentOutOfRangeException();
			}
		}

		protected virtual void WriteInstruction(ITextOutput output, MetadataFile metadataFile, MethodDefinitionHandle methodHandle, ref BlobReader blob, int methodRva)
		{
			var metadata = metadataFile.Metadata;
			int offset = blob.Offset;
			if (ShowSequencePoints && nextSequencePointIndex < sequencePoints?.Count)
			{
				var sp = sequencePoints[nextSequencePointIndex];
				if (sp.Offset <= offset)
				{
					output.Write("// sequence point: ");
					if (sp.Offset != offset)
					{
						output.Write("!! at " + DisassemblerHelpers.OffsetToString(sp.Offset) + " !!");
					}
					if (sp.IsHidden)
					{
						output.WriteLine("hidden");
					}
					else
					{
						output.WriteLine($"(line {sp.StartLine}, col {sp.StartColumn}) to (line {sp.EndLine}, col {sp.EndColumn}) in {sp.DocumentUrl}");
					}
					nextSequencePointIndex++;
				}
			}
			ILOpCode opCode = ILParser.DecodeOpCode(ref blob);
			OperandType opType = opCode.GetOperandType();
			if (opCode.IsDefined() && opType.OperandSize() <= blob.RemainingBytes)
			{
				WriteRVA(blob, offset + methodRva, opCode);
				output.WriteLocalReference(DisassemblerHelpers.OffsetToString(offset), offset, isDefinition: true);
				output.Write(": ");
				WriteOpCode(opCode);
				switch (opCode.GetOperandType())
				{
					case OperandType.BrTarget:
					case OperandType.ShortBrTarget:
						output.Write(' ');
						int targetOffset = ILParser.DecodeBranchTarget(ref blob, opCode);
						output.WriteLocalReference($"IL_{targetOffset:x4}", targetOffset);
						break;
					case OperandType.Field:
					case OperandType.Method:
					case OperandType.Sig:
					case OperandType.Type:
						output.Write(' ');
						int metadataToken = blob.ReadInt32();
						EntityHandle? handle = MetadataTokenHelpers.TryAsEntityHandle(metadataToken);
						try
						{
							handle?.WriteTo(module, output, genericContext);
						}
						catch (BadImageFormatException)
						{
							handle = null;
						}
						WriteMetadataToken(handle, metadataToken, spaceBefore: true);
						break;
					case OperandType.Tok:
						output.Write(' ');
						metadataToken = blob.ReadInt32();
						handle = MetadataTokenHelpers.TryAsEntityHandle(metadataToken);
						switch (handle?.Kind)
						{
							case HandleKind.MemberReference:
								switch (metadata.GetMemberReference((MemberReferenceHandle)handle).GetKind())
								{
									case MemberReferenceKind.Method:
										output.Write("method ");
										break;
									case MemberReferenceKind.Field:
										output.Write("field ");
										break;
								}
								break;
							case HandleKind.FieldDefinition:
								output.Write("field ");
								break;
							case HandleKind.MethodDefinition:
								output.Write("method ");
								break;
						}
						try
						{
							handle?.WriteTo(module, output, genericContext);
						}
						catch (BadImageFormatException)
						{
							handle = null;
						}
						WriteMetadataToken(handle, metadataToken, spaceBefore: true);
						break;
					case OperandType.ShortI:
						output.Write(' ');
						DisassemblerHelpers.WriteOperand(output, blob.ReadSByte());
						break;
					case OperandType.I:
						output.Write(' ');
						DisassemblerHelpers.WriteOperand(output, blob.ReadInt32());
						break;
					case OperandType.I8:
						output.Write(' ');
						DisassemblerHelpers.WriteOperand(output, blob.ReadInt64());
						break;
					case OperandType.ShortR:
						output.Write(' ');
						DisassemblerHelpers.WriteOperand(output, blob.ReadSingle());
						break;
					case OperandType.R:
						output.Write(' ');
						DisassemblerHelpers.WriteOperand(output, blob.ReadDouble());
						break;
					case OperandType.String:
						metadataToken = blob.ReadInt32();
						output.Write(' ');
						UserStringHandle? userString;
						string? text;
						try
						{
							userString = MetadataTokens.UserStringHandle(metadataToken);
							text = metadata.GetUserString(userString.Value);
						}
						catch (BadImageFormatException)
						{
							userString = null;
							text = null;
						}
						if (userString != null)
						{
							DisassemblerHelpers.WriteOperand(output, text);
						}
						WriteMetadataToken(userString, metadataToken, spaceBefore: true);
						break;
					case OperandType.Switch:
						var tmp = blob;
						int[] targets = ILParser.DecodeSwitchTargets(ref blob);
						if (ShowRawRVAOffsetAndBytes)
						{
							output.WriteLine(" (");
						}
						else
						{
							output.Write(" (");
						}
						tmp.ReadInt32();
						for (int i = 0; i < targets.Length; i++)
						{
							if (i > 0)
							{
								if (ShowRawRVAOffsetAndBytes)
								{
									output.WriteLine(",");
								}
								else
								{
									output.Write(", ");
								}
							}
							if (ShowRawRVAOffsetAndBytes)
							{
								output.Write("/*              ");
								output.Write($"{tmp.ReadByte():X2}{tmp.ReadByte():X2}{tmp.ReadByte():X2}{tmp.ReadByte():X2}");
								output.Write("         */ ");
							}
							if (ShowRawRVAOffsetAndBytes)
							{
								output.Write("                 ");
							}
							output.WriteLocalReference($"IL_{targets[i]:x4}", targets[i]);
						}
						output.Write(")");
						break;
					case OperandType.Variable:
						output.Write(' ');
						int index = blob.ReadUInt16();
						if (opCode == ILOpCode.Ldloc || opCode == ILOpCode.Ldloca || opCode == ILOpCode.Stloc)
						{
							DisassemblerHelpers.WriteVariableReference(output, metadata, methodHandle, index);
						}
						else
						{
							DisassemblerHelpers.WriteParameterReference(output, metadata, methodHandle, index);
						}
						break;
					case OperandType.ShortVariable:
						output.Write(' ');
						index = blob.ReadByte();
						if (opCode == ILOpCode.Ldloc_s || opCode == ILOpCode.Ldloca_s || opCode == ILOpCode.Stloc_s)
						{
							DisassemblerHelpers.WriteVariableReference(output, metadata, methodHandle, index);
						}
						else
						{
							DisassemblerHelpers.WriteParameterReference(output, metadata, methodHandle, index);
						}
						break;
				}
			}
			else
			{
				ushort opCodeValue = (ushort)opCode;
				if (opCodeValue > 0xFF)
				{
					if (ShowRawRVAOffsetAndBytes)
					{
						output.Write("/* ");
						output.Write($"0x{offset + methodRva:X8} {(ushort)opCode >> 8:X2}");
						output.Write("                 */ ");
					}
					output.WriteLocalReference(DisassemblerHelpers.OffsetToString(offset), offset, isDefinition: true);
					output.Write(": ");
					// split 16-bit value into two emitbyte directives
					output.WriteLine($".emitbyte 0x{(byte)(opCodeValue >> 8):x}");
					if (ShowRawRVAOffsetAndBytes)
					{
						output.Write("/* ");
						output.Write($"0x{offset + methodRva + 1:X8} {(ushort)opCode & 0xFF:X2}");
						output.Write("                 */ ");
					}
					// add label
					output.WriteLocalReference(DisassemblerHelpers.OffsetToString(offset + 1), offset + 1, isDefinition: true);
					output.Write(": ");
					output.Write($".emitbyte 0x{(byte)(opCodeValue & 0xFF):x}");
				}
				else
				{
					if (ShowRawRVAOffsetAndBytes)
					{
						output.Write("/* ");
						output.Write($"0x{offset + methodRva:X8} {(ushort)opCode & 0xFF:X2}");
						output.Write("                 */ ");
					}
					output.WriteLocalReference(DisassemblerHelpers.OffsetToString(offset), offset, isDefinition: true);
					output.Write(": ");
					output.Write($".emitbyte 0x{(byte)opCodeValue:x}");
				}
			}
			output.WriteLine();
		}

		void WriteRVA(BlobReader blob, int offset, ILOpCode opCode)
		{
			if (ShowRawRVAOffsetAndBytes)
			{
				output.Write("/* ");
				var tmp = blob;
				if (opCode == ILOpCode.Switch)
				{
					tmp.ReadInt32();
				}
				else
				{
					ILParser.SkipOperand(ref tmp, opCode);
				}
				output.Write($"0x{offset:X8} {(ushort)opCode:X2}");
				int appendSpaces = (ushort)opCode > 0xFF ? 14 : 16;
				while (blob.Offset < tmp.Offset)
				{
					output.Write($"{blob.ReadByte():X2}");
					appendSpaces -= 2;
				}
				if (appendSpaces > 0)
				{
					output.Write(new string(' ', appendSpaces));
				}
				output.Write(" */ ");
			}
		}

		private void WriteOpCode(ILOpCode opCode)
		{
			var opCodeInfo = new OpCodeInfo(opCode, opCode.GetDisplayName());
			string index;
			switch (opCode)
			{
				case ILOpCode.Ldarg_0:
				case ILOpCode.Ldarg_1:
				case ILOpCode.Ldarg_2:
				case ILOpCode.Ldarg_3:
					output.WriteReference(opCodeInfo, omitSuffix: true);
					index = opCodeInfo.Name.Substring(6);
					output.WriteLocalReference(index, "param_" + index);
					break;
				case ILOpCode.Ldloc_0:
				case ILOpCode.Ldloc_1:
				case ILOpCode.Ldloc_2:
				case ILOpCode.Ldloc_3:
				case ILOpCode.Stloc_0:
				case ILOpCode.Stloc_1:
				case ILOpCode.Stloc_2:
				case ILOpCode.Stloc_3:
					output.WriteReference(opCodeInfo, omitSuffix: true);
					index = opCodeInfo.Name.Substring(6);
					output.WriteLocalReference(index, "loc_" + index);
					break;
				default:
					output.WriteReference(opCodeInfo);
					break;
			}
		}

		private void WriteMetadataToken(EntityHandle handle, bool spaceBefore)
		{
			WriteMetadataToken(handle, MetadataTokens.GetToken(handle), spaceBefore);
		}

		private void WriteMetadataToken(Handle? handle, int metadataToken, bool spaceBefore)
		{
			ReflectionDisassembler.WriteMetadataToken(output, module, handle, metadataToken,
				spaceAfter: false, spaceBefore, ShowMetadataTokens, ShowMetadataTokensInBase10);
		}
	}
}
