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
				SetChildInstruction(ref this.tryBlock, value);
			}
		}
		
		internal override ILInstruction Inline(InstructionFlags flagsBefore, Stack<ILInstruction> instructionStack, out bool finished)
		{
			finished = false;
			return this;
		}
	}
	
	partial class TryCatch : TryInstruction
	{
		public readonly InstructionCollection<TryCatchHandler> Handlers;
		
		public TryCatch(ILInstruction tryBlock) : base(OpCode.TryCatch, tryBlock)
		{
			this.Handlers = new InstructionCollection<TryCatchHandler>(this);
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
			var flags = TryBlock.Flags;
			foreach (var handler in Handlers)
				flags = IfInstruction.CombineFlags(flags, handler.Flags);
			return flags;
		}
		
		public override IEnumerable<ILInstruction> Children {
			get {
				yield return TryBlock;
				foreach (var handler in Handlers)
					yield return handler;
			}
		}
		
		public override void TransformChildren(ILVisitor<ILInstruction> visitor)
		{
			this.TryBlock = TryBlock.AcceptVisitor(visitor);
			for (int i = 0; i < Handlers.Count; i++) {
				if (Handlers[i].AcceptVisitor(visitor) != Handlers[i])
					throw new InvalidOperationException("Cannot transform a TryCatchHandler");
			}
		}
	}
	
	partial class TryCatchHandler
	{
		internal override ILInstruction Inline(InstructionFlags flagsBefore, Stack<ILInstruction> instructionStack, out bool finished)
		{
			finished = false;
			return this;
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
				SetChildInstruction(ref this.finallyBlock, value);
			}
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
			return TryBlock.Flags | finallyBlock.Flags;
		}
		
		public override IEnumerable<ILInstruction> Children {
			get {
				yield return TryBlock;
				yield return finallyBlock;
			}
		}
		
		public override void TransformChildren(ILVisitor<ILInstruction> visitor)
		{
			this.TryBlock = TryBlock.AcceptVisitor(visitor);
			this.FinallyBlock = finallyBlock.AcceptVisitor(visitor);
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
				SetChildInstruction(ref this.faultBlock, value);
			}
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
			return IfInstruction.CombineFlags(TryBlock.Flags, faultBlock.Flags);
		}
		
		public override IEnumerable<ILInstruction> Children {
			get {
				yield return TryBlock;
				yield return faultBlock;
			}
		}
		
		public override void TransformChildren(ILVisitor<ILInstruction> visitor)
		{
			this.TryBlock = TryBlock.AcceptVisitor(visitor);
			this.FaultBlock = faultBlock.AcceptVisitor(visitor);
		}
	}
}
