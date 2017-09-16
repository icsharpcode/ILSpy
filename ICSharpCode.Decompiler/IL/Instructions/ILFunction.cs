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
using Mono.Cecil;
using ICSharpCode.Decompiler.Disassembler;
using System.Linq;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;
using System.Diagnostics;

namespace ICSharpCode.Decompiler.IL
{
	partial class ILFunction
	{
		public readonly MethodDefinition Method;
		public readonly ILVariableCollection Variables;

		/// <summary>
		/// Gets whether this function is a decompiled iterator (is using yield).
		/// This flag gets set by the YieldReturnDecompiler.
		/// 
		/// If set, the 'return' instruction has the semantics of 'yield break;'
		/// instead of a normal return.
		/// </summary>
		public bool IsIterator;

		public bool StateMachineCompiledWithMono;

		/// <summary>
		/// Gets whether this function is async.
		/// This flag gets set by the AsyncAwaitDecompiler.
		/// </summary>
		public bool IsAsync { get => AsyncReturnType != null; }

		/// <summary>
		/// Return element type -- if the async method returns Task{T}, this field stores T.
		/// If the async method returns Task or void, this field stores void.
		/// </summary>
		public IType AsyncReturnType;

		public ILFunction(MethodDefinition method, ILInstruction body) : base(OpCode.ILFunction)
		{
			this.Body = body;
			this.Method = method;
			this.Variables = new ILVariableCollection(this);
		}

		internal override void CheckInvariant(ILPhase phase)
		{
			for (int i = 0; i < Variables.Count; i++) {
				Debug.Assert(Variables[i].Function == this);
				Debug.Assert(Variables[i].IndexInFunction == i);
			}
			base.CheckInvariant(phase);
		}

		void CloneVariables()
		{
			throw new NotImplementedException();
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

			if (IsAsync) {
				output.WriteLine(".async");
			}
			if (IsIterator) {
				output.WriteLine(".iterator");
			}

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
			this.CheckInvariant(ILPhase.Normal);
			foreach (var transform in transforms) {
				context.CancellationToken.ThrowIfCancellationRequested();
				if (transform is BlockILTransform blockTransform) {
					context.StepStartGroup(blockTransform.ToString());
				} else {
					context.StepStartGroup(transform.GetType().Name);
				}
				transform.Run(this, context);
				this.CheckInvariant(ILPhase.Normal);
				context.StepEndGroup(keepIfEmpty: true);
			}
		}

		public ILVariable RegisterVariable(VariableKind kind, IType type, string name = null)
		{
			int index = Variables.Where(v => v.Kind == kind).MaxOrDefault(v => v.Index, -1) + 1;
			var variable = new ILVariable(kind, type, index);
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
					case VariableKind.InitializerTarget:
						name = "I_";
						break;
					default:
						throw new NotSupportedException();
				}
				name += index;
				variable.HasGeneratedName = true;
			}
			variable.Name = name;
			Variables.Add(variable);
			return variable;
		}
	}
}
