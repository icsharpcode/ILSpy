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

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL
{
	partial class CallIndirect
	{
		public static readonly SlotInfo ArgumentSlot = new SlotInfo("Argument", canInlineInto: true, isCollection: true);
		public static readonly SlotInfo FunctionPointerSlot = new SlotInfo("FunctionPointer", canInlineInto: true);

		public readonly InstructionCollection<ILInstruction> Arguments;
		ILInstruction functionPointer;
		public bool IsInstance { get; }
		public bool HasExplicitThis { get; }
		public System.Reflection.Metadata.SignatureCallingConvention CallingConvention { get; }
		public IType ReturnType { get; }
		public ImmutableArray<IType> ParameterTypes { get; }

		/// <summary>
		/// A 'final instruction' that gets executed after the <c>Instructions</c> collection.
		/// Provides the return value for the block.
		/// </summary>
		/// <remarks>
		/// Blocks in containers must have 'Nop' as a final instruction.
		/// </remarks>
		public ILInstruction FunctionPointer {
			get {
				return functionPointer;
			}
			set {
				ValidateChild(value);
				SetChildInstruction(ref functionPointer, value, Arguments.Count);
			}
		}

		protected internal override void InstructionCollectionUpdateComplete()
		{
			base.InstructionCollectionUpdateComplete();
			if (functionPointer?.Parent == this)
				functionPointer.ChildIndex = Arguments.Count;
		}

		public CallIndirect(bool isInstance, bool hasExplicitThis, System.Reflection.Metadata.SignatureCallingConvention callingConvention, IType returnType, ImmutableArray<IType> parameterTypes,
			IEnumerable<ILInstruction> arguments, ILInstruction functionPointer) : base(OpCode.CallIndirect)
		{
			this.IsInstance = isInstance;
			this.HasExplicitThis = hasExplicitThis;
			this.CallingConvention = callingConvention;
			this.ReturnType = returnType ?? throw new ArgumentNullException("returnType");
			this.ParameterTypes = parameterTypes.ToImmutableArray();
			this.Arguments = new InstructionCollection<ILInstruction>(this, 0);
			this.Arguments.AddRange(arguments);
			this.FunctionPointer = functionPointer;
		}

		public override ILInstruction Clone()
		{
			return new CallIndirect(IsInstance, HasExplicitThis, CallingConvention, ReturnType, ParameterTypes,
				this.Arguments.Select(inst => inst.Clone()), functionPointer.Clone()
			).WithILRange(this);
		}

		public override StackType ResultType => ReturnType.GetStackType();

		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			Debug.Assert(Arguments.Count == ParameterTypes.Length + (IsInstance ? 1 : 0));
		}

		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write("call.indirect ");
			ReturnType.WriteTo(output);
			output.Write('(');
			bool first = true;
			int firstArgument = IsInstance ? 1 : 0;
			if (firstArgument == 1) {
				Arguments[0].WriteTo(output, options);
				first = false;
			}
			foreach (var (inst, type) in Arguments.Skip(firstArgument).Zip(ParameterTypes, (a,b) => (a,b))) {
				if (first)
					first = false;
				else
					output.Write(", ");
				inst.WriteTo(output, options);
				output.Write(" : ");
				type.WriteTo(output);
			}
			if (Arguments.Count > 0)
				output.Write(", ");
			functionPointer.WriteTo(output, options);
			output.Write(')');
		}

		protected override int GetChildCount()
		{
			return Arguments.Count + 1;
		}

		protected override ILInstruction GetChild(int index)
		{
			if (index == Arguments.Count)
				return functionPointer;
			return Arguments[index];
		}

		protected override void SetChild(int index, ILInstruction value)
		{
			if (index == Arguments.Count)
				FunctionPointer = value;
			else
				Arguments[index] = value;
		}

		protected override SlotInfo GetChildSlot(int index)
		{
			if (index == Arguments.Count)
				return FunctionPointerSlot;
			else
				return ArgumentSlot;
		}

		protected override InstructionFlags ComputeFlags()
		{
			var flags = this.DirectFlags;
			foreach (var inst in Arguments) {
				flags |= inst.Flags;
			}
			flags |= functionPointer.Flags;
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
			if (CallingConvention != other.CallingConvention)
				return false;
			if (ParameterTypes.Length != other.ParameterTypes.Length)
				return false;
			for (int i = 0; i < ParameterTypes.Length; i++) {
				if (!ParameterTypes[i].Equals(other.ParameterTypes[i]))
					return false;
			}
			return ReturnType.Equals(other.ReturnType);
		}
	}
}
