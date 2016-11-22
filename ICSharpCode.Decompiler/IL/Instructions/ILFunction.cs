// Copyright (c) 2014 Daniel Grunwald
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
using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.NRefactory.TypeSystem;
using Mono.Cecil;
using ICSharpCode.Decompiler.Disassembler;
using System.Linq;

namespace ICSharpCode.Decompiler.IL
{
	partial class ILFunction
	{
		public readonly MethodDefinition Method;
		
		public ILFunction(MethodDefinition method, ILInstruction body) : base(OpCode.ILFunction)
		{
			this.Body = body;
			this.Method = method;
		}
		
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			if (Method != null) {
				output.Write(' ');
				Method.WriteTo(output);
			}
			output.WriteLine(" {");
			output.Indent();
			
			output.MarkFoldStart(Variables.Count + " variable(s)", true);
			foreach (var variable in Variables) {
				variable.WriteDefinitionTo(output);
				output.WriteLine();
			}
			output.MarkFoldEnd();
			output.WriteLine();
			body.WriteTo(output);
			
			output.WriteLine();
			output.Unindent();
			output.WriteLine("}");
		}
		
		protected override InstructionFlags ComputeFlags()
		{
			// Creating a lambda may throw OutOfMemoryException
			// We intentionally don't propagate any flags from the lambda body!
			return InstructionFlags.MayThrow | InstructionFlags.ControlFlow;
		}
		
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.MayThrow | InstructionFlags.ControlFlow;
			}
		}
		
		/// <summary>
		/// Apply a list of transforms to this function.
		/// </summary>
		public void RunTransforms(IEnumerable<IILTransform> transforms, ILTransformContext context)
		{
			foreach (var transform in transforms) {
				context.CancellationToken.ThrowIfCancellationRequested();
				transform.Run(this, context);
				this.CheckInvariant(ILPhase.Normal);
			}
		}

		public ILVariable RegisterVariable(VariableKind kind, IType type, string name = null)
		{
			int index = Variables.Where(v => v.Kind == kind).MaxOrDefault(v => v.Index, -1) + 1;
			if (string.IsNullOrWhiteSpace(name)) {
				switch (kind) {
					case VariableKind.Local:
						name = "V_";
						break;
					case VariableKind.Parameter:
						name = "P_";
						break;
					case VariableKind.Exception:
						name = "E_";
						break;
					case VariableKind.StackSlot:
						name = "S_";
						break;
					default:
						throw new NotSupportedException();
				}
				name += index;
			}
			var variable = new ILVariable(kind, type, index);
			variable.Name = name;
			Variables.Add(variable);
			return variable;
		}
		
		public static ILFunction Read(IDecompilerTypeSystem context, IMethod method, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Read(context, (MethodDefinition)context.GetCecil(method), cancellationToken);
		}

		public static ILFunction Read(IDecompilerTypeSystem context, MethodDefinition methodDefinition, CancellationToken cancellationToken = default(CancellationToken))
		{
			var ilReader = new ILReader(context);
			var function = ilReader.ReadIL(methodDefinition.Body, cancellationToken);
			function.CheckInvariant(ILPhase.Normal);
			return function;
		}
	}
}
