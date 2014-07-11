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
		public struct ArgumentCollection : IReadOnlyList<ILInstruction>
		{
			readonly CallInstruction inst;
			
			public ArgumentCollection(CallInstruction inst)
			{
				this.inst = inst;
			}
			
			public int Count {
				get { return inst.arguments.Length; }
			}
			
			public ILInstruction this[int index] {
				get { return inst.arguments[index]; }
				set {
					Debug.Assert(value.ResultType != StackType.Void);
					inst.arguments[index] = value;
					inst.InvalidateFlags();
				}
			}
			
			public ArgumentEnumerator GetEnumerator()
			{
				return new ArgumentEnumerator(inst.arguments);
			}
			
			IEnumerator<ILInstruction> IEnumerable<ILInstruction>.GetEnumerator()
			{
				IEnumerable<ILInstruction> arguments = inst.arguments;
				return arguments.GetEnumerator();
			}
			
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return inst.arguments.GetEnumerator();
			}
		}
		
		public struct ArgumentEnumerator
		{
			readonly ILInstruction[] arguments;
			int index;
			
			public ArgumentEnumerator(ILInstruction[] arguments)
			{
				this.arguments = arguments;
			}
			
			public bool MoveNext()
			{
				return ++index < arguments.Length;
			}
			
			public ILInstruction Current {
				get { return arguments[index]; }
			}
		}
		
		readonly ILInstruction[] arguments;
		public readonly MethodReference Method;
		
		public ArgumentCollection Arguments {
			get { return new ArgumentCollection(this); }
		}
		
		protected CallInstruction(OpCode opCode, MethodReference methodReference) : base(opCode)
		{
			this.Method = methodReference;
			int popCount = methodReference.Parameters.Count;
			if (opCode != OpCode.NewObj && methodReference.HasThis)
				popCount++;
			this.arguments = new ILInstruction[popCount];
			for (int i = 0; i < arguments.Length; i++) {
				arguments[i] = Pop;
			}
		}
		
		public override StackType ResultType {
			get {
				throw new NotImplementedException();
			}
		}
		
		internal override void CheckInvariant()
		{
			base.CheckInvariant();
			foreach (var op in arguments) {
				op.CheckInvariant();
				Debug.Assert(op.ResultType != StackType.Void);
			}
		}

		public override TAccumulate AggregateChildren<TSource, TAccumulate>(TAccumulate initial, ILVisitor<TSource> visitor, Func<TAccumulate, TSource, TAccumulate> func)
		{
			TAccumulate value = initial;
			foreach (var op in arguments)
				value = func(value, op.AcceptVisitor(visitor));
			return value;
		}
		
		protected override InstructionFlags ComputeFlags()
		{
			var flags = InstructionFlags.MayThrow | InstructionFlags.SideEffects;
			foreach (var op in arguments)
				flags |= op.Flags;
			return flags;
		}
		
		/*
		public override bool IsPeeking { get { return Operands.Length > 0 && Operands[0].IsPeeking; } }

		public override bool NoResult
		{
			get
			{
				return OpCode != OpCode.NewObj && Method.ReturnType.GetStackType() == StackType.Void;
			}
		}

		public override void TransformChildren(Func<ILInstruction, ILInstruction> transformFunc)
		{
			for (int i = 0; i < Operands.Length; i++) {
				Operands[i] = transformFunc(Operands[i]);
			}
		}*/

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
			for (int i = 0; i < Arguments.Length; i++) {
				if (i > 0)
					output.Write(", ");
				Arguments[i].WriteTo(output);
			}
			output.Write(')');
		}

		/*
		public override InstructionFlags Flags
		{
			get
			{
				InstructionFlags flags = InstructionFlags.SideEffects | InstructionFlags.MayThrow;
				for (int i = 0; i < Operands.Length; i++) {
					flags |= Operands[i].Flags;
				}
				return flags;
			}
		}

		internal override ILInstruction Inline(InstructionFlags flagsBefore, Stack<ILInstruction> instructionStack, out bool finished)
		{
			InstructionFlags operandFlags = InstructionFlags.None;
			for (int i = 0; i < Operands.Length - 1; i++) {
				operandFlags |= Operands[i].Flags;
			}
			flagsBefore |= operandFlags & ~(InstructionFlags.MayPeek | InstructionFlags.MayPop);
			finished = true;
			for (int i = Operands.Length - 1; i >= 0; i--) {
				Operands[i] = Operands[i].Inline(flagsBefore, instructionStack, out finished);
				if (!finished)
					break;
			}
			return this;
        }*/
	}
}
