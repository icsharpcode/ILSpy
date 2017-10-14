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
using System.Threading;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

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

		Collection<SequencePoint> sequencePoints;
		int nextSequencePointIndex;

		public MethodBodyDisassembler(ITextOutput output, CancellationToken cancellationToken)
		{
			this.output = output ?? throw new ArgumentNullException(nameof(output));
			this.cancellationToken = cancellationToken;
		}

		public virtual void Disassemble(MethodBody body)
		{
			// start writing IL code
			MethodDefinition method = body.Method;
			output.WriteLine("// Method begins at RVA 0x{0:x4}", method.RVA);
			output.WriteLine("// Code size {0} (0x{0:x})", body.CodeSize);
			output.WriteLine(".maxstack {0}", body.MaxStackSize);
			if (method.DeclaringType.Module.Assembly != null && method.DeclaringType.Module.Assembly.EntryPoint == method)
				output.WriteLine(".entrypoint");

			DisassembleLocalsBlock(body);
			output.WriteLine();

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
			sequencePoints = null;
		}

		private void DisassembleLocalsBlock(MethodBody body)
		{
			if (body.HasVariables) {
				output.Write(".locals ");
				if (body.InitLocals)
					output.Write("init ");
				output.WriteLine("(");
				output.Indent();
				foreach (var v in body.Variables) {
					output.WriteDefinition("[" + v.Index + "] ", v);
					v.VariableType.WriteTo(output);
					if (v.Index + 1 < body.Variables.Count)
						output.Write(',');
					output.WriteLine();
				}
				output.Unindent();
				output.WriteLine(")");
			}
		}

		internal void WriteExceptionHandlers(MethodBody body)
		{
			if (body.HasExceptionHandlers) {
				output.WriteLine();
				foreach (var eh in body.ExceptionHandlers) {
					eh.WriteTo(output);
					output.WriteLine();
				}
			}
		}

		HashSet<int> GetBranchTargets(IEnumerable<Instruction> instructions)
		{
			HashSet<int> branchTargets = new HashSet<int>();
			foreach (var inst in instructions) {
				Instruction target = inst.Operand as Instruction;
				if (target != null)
					branchTargets.Add(target.Offset);
				Instruction[] targets = inst.Operand as Instruction[];
				if (targets != null)
					foreach (Instruction t in targets)
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

		void WriteStructureBody(ILStructure s, HashSet<int> branchTargets, ref Instruction inst, int codeSize)
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

					prevInstructionWasBranch = inst.OpCode.FlowControl == FlowControl.Branch
						|| inst.OpCode.FlowControl == FlowControl.Cond_Branch
						|| inst.OpCode.FlowControl == FlowControl.Return
						|| inst.OpCode.FlowControl == FlowControl.Throw;

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

		protected virtual void WriteInstruction(ITextOutput output, Instruction instruction)
		{
			if (ShowSequencePoints && nextSequencePointIndex < sequencePoints?.Count) {
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
			}
			instruction.WriteTo(output);
		}
	}
}
