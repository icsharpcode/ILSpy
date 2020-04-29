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
using System.Diagnostics;
using System.Linq;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL
{
	public abstract class TryInstruction : ILInstruction
	{
		public static readonly SlotInfo TryBlockSlot = new SlotInfo("TryBlock");
		
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
	}
	
	/// <summary>
	/// Try-catch statement.
	/// </summary>
	/// <remarks>
	/// The return value of the try or catch blocks is ignored, the TryCatch always returns void.
	/// </remarks>
	partial class TryCatch : TryInstruction
	{
		public static readonly SlotInfo HandlerSlot = new SlotInfo("Handler", isCollection: true);
		public readonly InstructionCollection<TryCatchHandler> Handlers;
		
		public TryCatch(ILInstruction tryBlock) : base(OpCode.TryCatch, tryBlock)
		{
			this.Handlers = new InstructionCollection<TryCatchHandler>(this, 1);
		}
		
		public override ILInstruction Clone()
		{
			var clone = new TryCatch(TryBlock.Clone());
			clone.AddILRange(this);
			clone.Handlers.AddRange(this.Handlers.Select(h => (TryCatchHandler)h.Clone()));
			return clone;
		}
		
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(".try ");
			TryBlock.WriteTo(output, options);
			foreach (var handler in Handlers) {
				output.Write(' ');
				handler.WriteTo(output, options);
			}
		}
		
		public override StackType ResultType {
			get { return StackType.Void; }
		}
		
		protected override InstructionFlags ComputeFlags()
		{
			var flags = TryBlock.Flags;
			foreach (var handler in Handlers)
				flags = SemanticHelper.CombineBranches(flags, handler.Flags);
			return flags | InstructionFlags.ControlFlow;
		}
		
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.ControlFlow;
			}
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
		
		protected override SlotInfo GetChildSlot(int index)
		{
			if (index == 0)
				return TryBlockSlot;
			else
				return HandlerSlot;
		}
	}
	
	/// <summary>
	/// Catch handler within a try-catch statement.
	/// 
	/// When an exception occurs in the try block of the parent try.catch statement, the runtime searches
	/// the nearest enclosing TryCatchHandler with a matching variable type and
	/// assigns the exception object to the <see cref="Variable"/>, and executes the <see cref="Filter"/>.
	/// If the filter evaluates to 0, the exception is not caught and the runtime looks for the next catch handler.
	/// If the filter evaluates to 1, the stack is unwound, the exception caught and assigned to the <see cref="Variable"/>,
	/// and the <see cref="Body"/> is executed.
	/// </summary>
	partial class TryCatchHandler
	{
		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			Debug.Assert(Parent is TryCatch);
			Debug.Assert(filter.ResultType == StackType.I4);
			Debug.Assert(this.IsDescendantOf(variable.Function));
		}
		
		public override StackType ResultType {
			get { return StackType.Void; }
		}
		
		protected override InstructionFlags ComputeFlags()
		{
			return filter.Flags | body.Flags | InstructionFlags.ControlFlow | InstructionFlags.MayWriteLocals;
		}
		
		public override InstructionFlags DirectFlags {
			get {
				// the body is not evaluated if the filter returns 0
				return InstructionFlags.ControlFlow | InstructionFlags.MayWriteLocals;
			}
		}

		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write("catch ");
			if (variable != null) {
				output.WriteLocalReference(variable.Name, variable, isDefinition: true);
				output.Write(" : ");
				Disassembler.DisassemblerHelpers.WriteOperand(output, variable.Type);
			}
			output.Write(" when (");
			filter.WriteTo(output, options);
			output.Write(')');
			output.Write(' ');
			body.WriteTo(output, options);
		}

		/// <summary>
		/// Gets the ILRange of the instructions at the start of the catch-block,
		/// that take the exception object and store it in the exception variable slot.
		/// Note: This range is empty, if Filter is not empty, i.e., ldloc 1.
		/// </summary>
		public Interval ExceptionSpecifierILRange { get; private set; }

		public void AddExceptionSpecifierILRange(Interval newRange)
		{
			ExceptionSpecifierILRange = CombineILRange(ExceptionSpecifierILRange, newRange);
		}
	}
	
	partial class TryFinally
	{
		public static readonly SlotInfo FinallyBlockSlot = new SlotInfo("FinallyBlock");
		
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
			return new TryFinally(TryBlock.Clone(), finallyBlock.Clone()).WithILRange(this);
		}

		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(".try ");
			TryBlock.WriteTo(output, options);
			output.Write(" finally ");
			finallyBlock.WriteTo(output, options);
		}

		public override StackType ResultType {
			get {
				return TryBlock.ResultType;
			}
		}

		protected override InstructionFlags ComputeFlags()
		{
			// if the endpoint of either the try or the finally is unreachable, the endpoint of the try-finally will be unreachable
			return TryBlock.Flags | finallyBlock.Flags | InstructionFlags.ControlFlow;
		}
		
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.ControlFlow;
			}
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
		
		protected override SlotInfo GetChildSlot(int index)
		{
			switch (index) {
				case 0:
					return TryBlockSlot;
				case 1:
					return FinallyBlockSlot;
				default:
					throw new IndexOutOfRangeException();
			}
		}
	}
	
	partial class TryFault
	{
		public static readonly SlotInfo FaultBlockSlot = new SlotInfo("FaultBlock");
		
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
			return new TryFault(TryBlock.Clone(), faultBlock.Clone()).WithILRange(this);
		}
		
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(".try ");
			TryBlock.WriteTo(output, options);
			output.Write(" fault ");
			faultBlock.WriteTo(output, options);
		}
		
		public override StackType ResultType {
			get { return TryBlock.ResultType; }
		}
		
		protected override InstructionFlags ComputeFlags()
		{
			// The endpoint of the try-fault is unreachable iff the try endpoint is unreachable
			return TryBlock.Flags | (faultBlock.Flags & ~InstructionFlags.EndPointUnreachable) | InstructionFlags.ControlFlow;
		}
		
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.ControlFlow;
			}
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
		
		protected override SlotInfo GetChildSlot(int index)
		{
			switch (index) {
				case 0:
					return TryBlockSlot;
				case 1:
					return FaultBlockSlot;
				default:
					throw new IndexOutOfRangeException();
			}
		}
	}

	public partial class Throw
	{
		internal StackType resultType = StackType.Void;
	}
}
