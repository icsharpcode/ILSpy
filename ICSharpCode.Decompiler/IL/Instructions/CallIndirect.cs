#nullable enable
// Copyright (c) 2017 Daniel Grunwald
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

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL
{
	partial class CallIndirect
	{
		// Note: while in IL the arguments come first and the function pointer last;
		// in the ILAst we're handling it as in C#: the function pointer is evaluated first, the arguments later.
		public static readonly SlotInfo FunctionPointerSlot = new SlotInfo("FunctionPointer", canInlineInto: true);
		public static readonly SlotInfo ArgumentSlot = new SlotInfo("Argument", canInlineInto: true, isCollection: true);

		ILInstruction functionPointer = null!;
		public readonly InstructionCollection<ILInstruction> Arguments;
		public bool IsInstance { get; }
		public bool HasExplicitThis { get; }
		public FunctionPointerType FunctionPointerType { get; }

		public ILInstruction FunctionPointer {
			get {
				return functionPointer;
			}
			set {
				ValidateChild(value);
				SetChildInstruction(ref functionPointer, value, 0);
			}
		}

		public CallIndirect(bool isInstance, bool hasExplicitThis, FunctionPointerType functionPointerType,
			ILInstruction functionPointer, IEnumerable<ILInstruction> arguments) : base(OpCode.CallIndirect)
		{
			this.IsInstance = isInstance;
			this.HasExplicitThis = hasExplicitThis;
			this.FunctionPointerType = functionPointerType;
			this.FunctionPointer = functionPointer;
			this.Arguments = new InstructionCollection<ILInstruction>(this, 1);
			this.Arguments.AddRange(arguments);
		}

		public override ILInstruction Clone()
		{
			return new CallIndirect(IsInstance, HasExplicitThis, FunctionPointerType,
				functionPointer.Clone(), this.Arguments.Select(inst => inst.Clone())
			).WithILRange(this);
		}

		public override StackType ResultType => FunctionPointerType.ReturnType.GetStackType();

		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			Debug.Assert(Arguments.Count == FunctionPointerType.ParameterTypes.Length + (IsInstance ? 1 : 0));
		}

		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write("call.indirect ");
			FunctionPointerType.ReturnType.WriteTo(output);
			output.Write('(');
			functionPointer.WriteTo(output, options);
			int firstArgument = IsInstance ? 1 : 0;
			if (firstArgument == 1)
			{
				output.Write(", ");
				Arguments[0].WriteTo(output, options);
			}
			foreach (var (inst, type) in Arguments.Zip(FunctionPointerType.ParameterTypes, (a, b) => (a, b)))
			{
				output.Write(", ");
				inst.WriteTo(output, options);
				output.Write(" : ");
				type.WriteTo(output);
			}
			if (Arguments.Count > 0)
				output.Write(')');
		}

		protected override int GetChildCount()
		{
			return Arguments.Count + 1;
		}

		protected override ILInstruction GetChild(int index)
		{
			if (index == 0)
				return functionPointer;
			return Arguments[index - 1];
		}

		protected override void SetChild(int index, ILInstruction value)
		{
			if (index == 0)
				FunctionPointer = value;
			else
				Arguments[index - 1] = value;
		}

		protected override SlotInfo GetChildSlot(int index)
		{
			if (index == 0)
				return FunctionPointerSlot;
			else
				return ArgumentSlot;
		}

		protected override InstructionFlags ComputeFlags()
		{
			var flags = this.DirectFlags;
			flags |= functionPointer.Flags;
			foreach (var inst in Arguments)
			{
				flags |= inst.Flags;
			}
			return flags;
		}

		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.MayThrow | InstructionFlags.SideEffect;
			}
		}

		bool EqualSignature(CallIndirect other)
		{
			if (IsInstance != other.IsInstance)
				return false;
			if (HasExplicitThis != other.HasExplicitThis)
				return false;
			return FunctionPointerType.Equals(other.FunctionPointerType);
		}
	}
}
