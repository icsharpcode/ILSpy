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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using ICSharpCode.Decompiler.Disassembler;

namespace ICSharpCode.Decompiler.IL
{
	public abstract class CallInstruction : ILInstruction
	{
		public static CallInstruction Create(OpCode opCode, MethodReference method)
		{
			switch (opCode) {
				case OpCode.Call:
					return new Call(method);
				case OpCode.CallVirt:
					return new CallVirt(method);
				case OpCode.NewObj:
					return new NewObj(method);
				default:
					throw new ArgumentException("Not a valid call opcode");
			}
		}

		public readonly InstructionCollection<ILInstruction> Arguments;
		public readonly MethodReference Method;
		
		protected CallInstruction(OpCode opCode, MethodReference methodReference) : base(opCode)
		{
			this.Method = methodReference;
			this.Arguments = new InstructionCollection<ILInstruction>(this);
		}
		
		public static int GetPopCount(OpCode callCode, MethodReference methodReference)
		{
			int popCount = methodReference.Parameters.Count;
			if (callCode != OpCode.NewObj && methodReference.HasThis)
				popCount++;
			return popCount;
		}
		
		public override StackType ResultType {
			get {
				throw new NotImplementedException();
			}
		}
		
		public override TAccumulate AggregateChildren<TSource, TAccumulate>(TAccumulate initial, ILVisitor<TSource> visitor, Func<TAccumulate, TSource, TAccumulate> func)
		{
			TAccumulate value = initial;
			foreach (var op in Arguments)
				value = func(value, op.AcceptVisitor(visitor));
			return value;
		}
		
		public override void TransformChildren(ILVisitor<ILInstruction> visitor)
		{
			for (int i = 0; i < Arguments.Count; i++) {
				Arguments[i] = Arguments[i].AcceptVisitor(visitor);
			}
		}
		
		protected override InstructionFlags ComputeFlags()
		{
			var flags = InstructionFlags.MayThrow | InstructionFlags.SideEffect;
			foreach (var op in Arguments)
				flags |= op.Flags;
			return flags;
		}
		
		/// <summary>
		/// Gets/Sets whether the call has the 'tail.' prefix.
		/// </summary>
		public bool IsTail;

		/// <summary>
		/// Gets/Sets the type specified in the 'constrained.' prefix.
		/// Returns null if no 'constrained.' prefix exists for this call.
		/// </summary>
		public TypeReference ConstrainedTo;

		public override void WriteTo(ITextOutput output)
		{
			if (ConstrainedTo != null) {
				output.Write("constrained.");
				ConstrainedTo.WriteTo(output, ILNameSyntax.ShortTypeName);
			}
			if (IsTail)
				output.Write("tail.");
			output.Write(OpCode);
			output.Write(' ');
			Method.WriteTo(output);
			output.Write('(');
			for (int i = 0; i < Arguments.Count; i++) {
				if (i > 0)
					output.Write(", ");
				Arguments[i].WriteTo(output);
			}
			output.Write(')');
		}

		internal override ILInstruction Inline(InstructionFlags flagsBefore, Stack<ILInstruction> instructionStack, out bool finished)
		{
			InstructionFlags operandFlags = InstructionFlags.None;
			for (int i = 0; i < Arguments.Count - 1; i++) {
				operandFlags |= Arguments[i].Flags;
			}
			flagsBefore |= operandFlags & ~(InstructionFlags.MayPeek | InstructionFlags.MayPop);
			finished = true;
			for (int i = Arguments.Count - 1; i >= 0; i--) {
				Arguments[i] = Arguments[i].Inline(flagsBefore, instructionStack, out finished);
				if (!finished)
					break;
			}
			return this;
		}
	}
}
