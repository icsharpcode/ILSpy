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
using System.Threading;

using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;

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

		IList<Metadata.SequencePoint> sequencePoints;
		int nextSequencePointIndex;

		public MethodBodyDisassembler(ITextOutput output, CancellationToken cancellationToken)
		{
			this.output = output ?? throw new ArgumentNullException(nameof(output));
			this.cancellationToken = cancellationToken;
		}

		public unsafe virtual void Disassemble(Metadata.MethodDefinition method)
		{
			// start writing IL code
			var metadata = method.Module.GetMetadataReader();
			var methodDefinition = metadata.GetMethodDefinition(method.Handle);
			output.WriteLine("// Method begins at RVA 0x{0:x4}", methodDefinition.RelativeVirtualAddress);
			if (methodDefinition.RelativeVirtualAddress == 0) {
				output.WriteLine("// Code size {0} (0x{0:x})", 0);
				output.WriteLine(".maxstack {0}", 0);
				output.WriteLine();
				return;
			}
			var body = method.Module.Reader.GetMethodBody(methodDefinition.RelativeVirtualAddress);
			var blob = body.GetILReader();
			output.WriteLine("// Code size {0} (0x{0:x})", blob.Length);
			output.WriteLine(".maxstack {0}", body.MaxStack);

			var entrypointHandle = MetadataTokens.MethodDefinitionHandle(method.Module.Reader.PEHeaders.CorHeader.EntryPointTokenOrRelativeVirtualAddress);
			if (method.Handle == entrypointHandle)
				output.WriteLine(".entrypoint");

			DisassembleLocalsBlock(method, body);
			output.WriteLine();


			sequencePoints = method.GetSequencePoints();
			nextSequencePointIndex = 0;
			if (DetectControlStructure && blob.Length > 0) {
				blob.Reset();
				HashSet<int> branchTargets = GetBranchTargets(blob);
				blob.Reset();
				WriteStructureBody(new ILStructure(method, body), branchTargets, ref blob);
			} else {
				while (blob.RemainingBytes > 0) {
					WriteInstruction(output, method, ref blob);
				}
				WriteExceptionHandlers(method, body);
			}
			sequencePoints = null;
		}

		void DisassembleLocalsBlock(Metadata.MethodDefinition method, MethodBodyBlock body)
		{
			if (body.LocalSignature.IsNil) return;
			var metadata = method.Module.GetMetadataReader();
			var blob = metadata.GetStandaloneSignature(body.LocalSignature);
			if (blob.GetKind() != StandaloneSignatureKind.LocalVariables)
				return;
			var reader = metadata.GetBlobReader(blob.Signature);
			if (reader.Length < 2)
				return;
			reader.Offset = 1;
			if (reader.ReadCompressedInteger() == 0)
				return;
			var signature = blob.DecodeLocalSignature(new DisassemblerSignatureProvider(method.Module, output), new GenericContext(method));
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

		internal void WriteExceptionHandlers(Metadata.MethodDefinition method, MethodBodyBlock body)
		{
			var handlers = body.ExceptionRegions;
			if (!handlers.IsEmpty) {
				output.WriteLine();
				foreach (var eh in handlers) {
					eh.WriteTo(method, output);
					output.WriteLine();
				}
			}
		}

		HashSet<int> GetBranchTargets(BlobReader blob)
		{
			HashSet<int> branchTargets = new HashSet<int>();
			while (blob.RemainingBytes > 0) {
				var opCode = ILParser.DecodeOpCode(ref blob);
				if (opCode == ILOpCode.Switch) {
					branchTargets.UnionWith(ILParser.DecodeSwitchTargets(ref blob));
				} else if (opCode.IsBranch()) {
					branchTargets.Add(ILParser.DecodeBranchTarget(ref blob, opCode));
				} else {
					ILParser.SkipOperand(ref blob, opCode);
				}
			}
			return branchTargets;
		}

		void WriteStructureHeader(ILStructure s)
		{
			switch (s.Type) {
				case ILStructureType.Loop:
					output.Write("// loop start");
					if (s.LoopEntryPointOffset >= 0) {
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
					switch (s.ExceptionHandler.Kind) {
						case ExceptionRegionKind.Catch:
						case ExceptionRegionKind.Filter:
							output.Write("catch");
							if (!s.ExceptionHandler.CatchType.IsNil) {
								output.Write(' ');
								s.ExceptionHandler.CatchType.WriteTo(s.Method.Module, output, new Metadata.GenericContext(s.Method), ILNameSyntax.TypeName);
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

		void WriteStructureBody(ILStructure s, HashSet<int> branchTargets, ref BlobReader body)
		{
			bool isFirstInstructionInStructure = true;
			bool prevInstructionWasBranch = false;
			int childIndex = 0;
			while (body.RemainingBytes > 0 && body.Offset < s.EndOffset) {
				int offset = body.Offset;
				if (childIndex < s.Children.Count && s.Children[childIndex].StartOffset <= offset && offset < s.Children[childIndex].EndOffset) {
					ILStructure child = s.Children[childIndex++];
					WriteStructureHeader(child);
					WriteStructureBody(child, branchTargets, ref body);
					WriteStructureFooter(child);
				} else {
					if (!isFirstInstructionInStructure && (prevInstructionWasBranch || branchTargets.Contains(offset))) {
						output.WriteLine(); // put an empty line after branches, and in front of branch targets
					}
					var currentOpCode = ILParser.DecodeOpCode(ref body);
					body.Offset = offset; // reset IL stream
					WriteInstruction(output, s.Method, ref body);
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

		protected virtual void WriteInstruction(ITextOutput output, Metadata.MethodDefinition method, ref BlobReader blob)
		{
			var metadata = method.Module.GetMetadataReader();
			int offset = blob.Offset;
			if (ShowSequencePoints && nextSequencePointIndex < sequencePoints?.Count) {
				Metadata.SequencePoint sp = sequencePoints[nextSequencePointIndex];
				if (sp.Offset <= offset) {
					output.Write("// sequence point: ");
					if (sp.Offset != offset) {
						output.Write("!! at " + DisassemblerHelpers.OffsetToString(sp.Offset) + " !!");
					}
					if (sp.IsHidden) {
						output.WriteLine("hidden");
					} else {
						output.WriteLine($"(line {sp.StartLine}, col {sp.StartColumn}) to (line {sp.EndLine}, col {sp.EndColumn}) in {sp.DocumentUrl}");
					}
					nextSequencePointIndex++;
				}
			}
			ILOpCode opCode = ILParser.DecodeOpCode(ref blob);
			output.WriteDefinition(DisassemblerHelpers.OffsetToString(offset), offset);
			output.Write(": ");
			if (opCode.IsDefined()) {
				output.WriteReference(opCode.GetDisplayName(), new OpCodeInfo(opCode, opCode.GetDisplayName()));
				switch (opCode.GetOperandType()) {
					case OperandType.BrTarget:
					case OperandType.ShortBrTarget:
						output.Write(' ');
						int targetOffset = ILParser.DecodeBranchTarget(ref blob, opCode);
						output.WriteReference($"IL_{targetOffset:x4}", targetOffset, true);
						break;
					case OperandType.Field:
					case OperandType.Method:
					case OperandType.Sig:
					case OperandType.Type:
						output.Write(' ');
						var handle = MetadataTokens.EntityHandle(blob.ReadInt32());
						handle.WriteTo(method.Module, output, new GenericContext(method));
						break;
					case OperandType.Tok:
						output.Write(' ');
						handle = MetadataTokens.EntityHandle(blob.ReadInt32());
						switch (handle.Kind) {
							case HandleKind.MemberReference:
								switch (metadata.GetMemberReference((MemberReferenceHandle)handle).GetKind()) {
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
						}
						handle.WriteTo(method.Module, output, new GenericContext(method));
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
						var userString = metadata.GetUserString(MetadataTokens.UserStringHandle(blob.ReadInt32()));
						output.Write(' ');
						DisassemblerHelpers.WriteOperand(output, userString);
						break;
					case OperandType.Switch:
						int[] targets = ILParser.DecodeSwitchTargets(ref blob);
						output.Write(" (");
						for (int i = 0; i < targets.Length; i++) {
							if (i > 0)
								output.Write(", ");
							output.WriteReference($"IL_{targets[i]:x4}", targets[i], true);
						}
						output.Write(")");
						break;
					case OperandType.Variable:
						output.Write(' ');
						int index = blob.ReadUInt16();
						if (opCode == ILOpCode.Ldloc || opCode == ILOpCode.Ldloca || opCode == ILOpCode.Stloc) {
							DisassemblerHelpers.WriteVariableReference(output, method, index);
						} else {
							DisassemblerHelpers.WriteParameterReference(output, method, index);
						}
						break;
					case OperandType.ShortVariable:
						output.Write(' ');
						index = blob.ReadByte();
						if (opCode == ILOpCode.Ldloc_s || opCode == ILOpCode.Ldloca_s || opCode == ILOpCode.Stloc_s) {
							DisassemblerHelpers.WriteVariableReference(output, method, index);
						} else {
							DisassemblerHelpers.WriteParameterReference(output, method, index);
						}
						break;
				}
			} else {
				output.Write($".emitbyte 0x{opCode:x}");
			}
			output.WriteLine();
		}
	}
}
