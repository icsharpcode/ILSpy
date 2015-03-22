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

namespace ICSharpCode.Decompiler.IL
{
	public abstract class TryInstruction : ILInstruction
	{
		protected TryInstruction(OpCode opCode, ILInstruction tryBlock) : base(opCode)
		{
			this.TryBlock = tryBlock;
		}
		
		ILInstruction tryBlock;
		public ILInstruction TryBlock {
			get { return this.tryBlock; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.tryBlock, value, 0);
			}
		}
		
		internal override ILInstruction Inline(InstructionFlags flagsBefore, IInlineContext context)
		{
			// Inlining into exception-handling constructs would be madness.
			// To keep phase-1 execution semantics consistent with inlining, there's a
			// phase-1-boundary around every try/catch/finally/fault block.
			return this;
		}
	}
	
	/// <summary>
	/// Try-catch statement.
	/// </summary>
	/// <remarks>
	/// The evaluation stack does not need to be empty when entering or leaving a try-catch-block.
	/// All try or catch blocks with reachable endpoint must produce compatible stacks.
	/// The return value of the try or catch blocks is ignored, the TryCatch always returns void.
	/// </remarks>
	partial class TryCatch : TryInstruction
	{
		public readonly InstructionCollection<TryCatchHandler> Handlers;
		
		public TryCatch(ILInstruction tryBlock) : base(OpCode.TryCatch, tryBlock)
		{
			this.Handlers = new InstructionCollection<TryCatchHandler>(this, 1);
		}
		
		public override ILInstruction Clone()
		{
			var clone = new TryCatch(TryBlock.Clone());
			clone.ILRange = this.ILRange;
			clone.Handlers.AddRange(this.Handlers.Select(h => (TryCatchHandler)h.Clone()));
			return clone;
		}
		
		public override void WriteTo(ITextOutput output)
		{
			output.Write(".try ");
			TryBlock.WriteTo(output);
			foreach (var handler in Handlers) {
				output.Write(' ');
				handler.WriteTo(output);
			}
		}
		
		public override StackType ResultType {
			get { return StackType.Void; }
		}
		
		protected override InstructionFlags ComputeFlags()
		{
			var flags = Block.Phase1Boundary(TryBlock.Flags);
			foreach (var handler in Handlers)
				flags = IfInstruction.CombineFlags(flags, handler.Flags);
			return flags;
		}
		
		protected override int GetChildCount()
		{
			return 1 + Handlers.Count;
		}
		
		protected override ILInstruction GetChild(int index)
		{
			if (index == 0)
				return TryBlock;
			else
				return Handlers[index - 1];
		}
		
		protected override void SetChild(int index, ILInstruction value)
		{
			if (index == 0)
				TryBlock = value;
			else
				Handlers[index - 1] = (TryCatchHandler)value;
		}
		
		internal override void TransformStackIntoVariables(TransformStackIntoVariablesState state)
		{
			throw new NotImplementedException();
		}
	}
	
	/// <summary>
	/// Catch handler within a try-catch statement.
	/// 
	/// When an exception occurs in the try block of the parent try.catch statement, the runtime searches
	/// the nearest enclosing TryCatchHandler with a matching variable type and
	/// assigns the exception object to the <see cref="Variable"/>.
	/// Then, the evaluation stack is cleared and the <see cref="Filter"/> is executed
	/// (first phase-1 execution, which should be a no-op given the empty stack, then phase-2 execution).
	/// If the filter evaluates to 0, the exception is not caught and the runtime looks for the next catch handler.
	/// If the filter evaluates to 1, the stack is unwound, the exception caught and assigned to the <see cref="Variable"/>,
	/// the evaluation stack is cleared again, and the <see cref="Body"/> is executed (again, phase-1 + phase-2).
	/// </summary>
	partial class TryCatchHandler
	{
		internal override ILInstruction Inline(InstructionFlags flagsBefore, IInlineContext context)
		{
			// should never happen as TryCatchHandler only appears within TryCatch instructions
			throw new InvalidOperationException();
		}
		
		internal override void CheckInvariant()
		{
			base.CheckInvariant();
			Debug.Assert(Parent is TryCatch);
			Debug.Assert(filter.ResultType == StackType.I4);
		}
		
		public override StackType ResultType {
			get { return StackType.Void; }
		}
		
		protected override InstructionFlags ComputeFlags()
		{
			return Block.Phase1Boundary(filter.Flags) | Block.Phase1Boundary(body.Flags);
		}

		internal override void TransformStackIntoVariables(TransformStackIntoVariablesState state)
		{
		}
		
		public override void WriteTo(ITextOutput output)
		{
			output.Write("catch ");
			if (variable != null) {
				output.WriteDefinition(variable.Name, variable);
				output.Write(" : ");
				Disassembler.DisassemblerHelpers.WriteOperand(output, variable.Type);
			}
			output.Write(" if (");
			filter.WriteTo(output);
			output.Write(')');
			output.Write(' ');
			body.WriteTo(output);
		}
		
		protected override void Connected()
		{
			base.Connected();
			variable.StoreCount++;
		}
		
		protected override void Disconnected()
		{
			variable.StoreCount--;
			base.Disconnected();
		}
	}
	
	partial class TryFinally
	{
		public TryFinally(ILInstruction tryBlock, ILInstruction finallyBlock) : base(OpCode.TryFinally, tryBlock)
		{
			this.FinallyBlock = finallyBlock;
		}
		
		ILInstruction finallyBlock;
		public ILInstruction FinallyBlock {
			get { return this.finallyBlock; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.finallyBlock, value, 1);
			}
		}
		
		public override ILInstruction Clone()
		{
			return new TryFinally(TryBlock.Clone(), finallyBlock.Clone()) {
				ILRange = this.ILRange
			};
		}
		
		public override void WriteTo(ITextOutput output)
		{
			output.Write(".try ");
			TryBlock.WriteTo(output);
			output.Write(" finally ");
			finallyBlock.WriteTo(output);
		}

		public override StackType ResultType {
			get {
				return TryBlock.ResultType;
			}
		}

		protected override InstructionFlags ComputeFlags()
		{
			// if the endpoint of either the try or the finally is unreachable, the endpoint of the try-finally will be unreachable
			return Block.Phase1Boundary(TryBlock.Flags) | Block.Phase1Boundary(finallyBlock.Flags);
		}
		
		protected override int GetChildCount()
		{
			return 2;
		}
		
		protected override ILInstruction GetChild(int index)
		{
			switch (index) {
				case 0:
					return TryBlock;
				case 1:
					return finallyBlock;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		
		protected override void SetChild(int index, ILInstruction value)
		{
			switch (index) {
				case 0:
					TryBlock = value;
					break;
				case 1:
					FinallyBlock = value;
					break;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		
		internal override void TransformStackIntoVariables(TransformStackIntoVariablesState state)
		{
			throw new NotImplementedException();
		}
	}
	
	partial class TryFault
	{
		public TryFault(ILInstruction tryBlock, ILInstruction faultBlock) : base(OpCode.TryFinally, tryBlock)
		{
			this.FaultBlock = faultBlock;
		}
		
		ILInstruction faultBlock;
		public ILInstruction FaultBlock {
			get { return this.faultBlock; }
			set {
				ValidateChild(value);
				SetChildInstruction(ref this.faultBlock, value, 1);
			}
		}
		
		public override ILInstruction Clone()
		{
			return new TryFault(TryBlock.Clone(), faultBlock.Clone()) {
				ILRange = this.ILRange
			};
		}
		
		public override void WriteTo(ITextOutput output)
		{
			output.Write(".try ");
			TryBlock.WriteTo(output);
			output.Write(" fault ");
			faultBlock.WriteTo(output);
		}
		
		public override StackType ResultType {
			get { return TryBlock.ResultType; }
		}
		
		protected override InstructionFlags ComputeFlags()
		{
			// The endpoint of the try-fault is unreachable only if both endpoints are unreachable
			return IfInstruction.CombineFlags(Block.Phase1Boundary(TryBlock.Flags), Block.Phase1Boundary(faultBlock.Flags));
		}
		
		protected override int GetChildCount()
		{
			return 2;
		}
		
		protected override ILInstruction GetChild(int index)
		{
			switch (index) {
				case 0:
					return TryBlock;
				case 1:
					return faultBlock;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		
		protected override void SetChild(int index, ILInstruction value)
		{
			switch (index) {
				case 0:
					TryBlock = value;
					break;
				case 1:
					FaultBlock = value;
					break;
				default:
					throw new IndexOutOfRangeException();
			}
		}
		
		internal override void TransformStackIntoVariables(TransformStackIntoVariablesState state)
		{
			throw new NotImplementedException();
		}
	}
}
