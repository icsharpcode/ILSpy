#nullable enable
// Copyright (c) 2018 Daniel Grunwald
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

using System.Diagnostics;
using System.Linq;

using ICSharpCode.Decompiler.IL.Transforms;

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// For a nullable input, gets the underlying value.
	/// 
	/// There are three possible input types:
	///  * reference type: if input!=null, evaluates to the input
	///  * nullable value type: if input.Has_Value, evaluates to input.GetValueOrDefault()
	///  * generic type: behavior depends on the type at runtime.
	///    If non-nullable value type, unconditionally evaluates to the input.
	///
	/// If the input is null, control-flow is tranferred to the nearest surrounding nullable.rewrap
	/// instruction.
	/// </summary>
	partial class NullableUnwrap
	{
		/// <summary>
		/// Whether the argument is dereferenced before checking for a null input.
		/// If true, the argument must be a managed reference to a valid input type.
		/// </summary>
		/// <remarks>
		/// This mode exists because the C# compiler sometimes avoids copying the whole Nullable{T} struct
		/// before the null-check.
		/// The underlying struct T is still copied by the GetValueOrDefault() call, but only in the non-null case.
		/// </remarks>
		public readonly bool RefInput;

		/// <summary>
		/// Consider the following code generated for <code>t?.Method()</code> on a generic t:
		/// <code>if (comp(box ``0(ldloc t) != ldnull)) newobj Nullable..ctor(constrained[``0].callvirt Method(ldloca t)) else default.value Nullable</code>
		/// Here, the method is called on the original reference, and any mutations performed by the method will be visible in the original variable.
		/// 
		/// To represent this, we use a nullable.unwrap with ResultType==Ref: instead of returning the input value,
		/// the input reference is returned in the non-null case.
		/// Note that in case the generic type ends up being <c>Nullable{T}</c>, this means methods will end up being called on
		/// the nullable type, not on the underlying type. However, this ends up making no difference, because the only methods
		/// that can be called that way are those on System.Object. All the virtual methods are overridden in <c>Nullable{T}</c>
		/// and end up forwarding to <c>T</c>; and the non-virtual methods cause boxing which strips the <c>Nullable{T}</c> wrapper.
		/// 
		/// RefOutput can only be used if RefInput is also used.
		/// </summary>
		public bool RefOutput { get => ResultType == StackType.Ref; }

		public NullableUnwrap(StackType unwrappedType, ILInstruction argument, bool refInput = false)
			: base(OpCode.NullableUnwrap, argument)
		{
			this.ResultType = unwrappedType;
			this.RefInput = refInput;
			if (unwrappedType == StackType.Ref)
			{
				Debug.Assert(refInput);
			}
		}

		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			if (this.RefInput)
			{
				Debug.Assert(Argument.ResultType == StackType.Ref, "nullable.unwrap expects reference to nullable type as input");
			}
			else
			{
				Debug.Assert(Argument.ResultType == StackType.O, "nullable.unwrap expects nullable type as input");
			}
			Debug.Assert(Ancestors.Any(a => a is NullableRewrap));
		}

		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			output.Write("nullable.unwrap.");
			if (RefInput)
			{
				output.Write("refinput.");
			}
			output.Write(ResultType);
			output.Write('(');
			Argument.WriteTo(output, options);
			output.Write(')');
		}

		public override StackType ResultType { get; }
	}

	partial class NullableRewrap
	{
		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			Debug.Assert(Argument.HasFlag(InstructionFlags.MayUnwrapNull));
		}

		public override InstructionFlags DirectFlags => InstructionFlags.ControlFlow;

		protected override InstructionFlags ComputeFlags()
		{
			// Convert MayUnwrapNull flag to ControlFlow flag.
			// Also, remove EndpointUnreachable flag, because the end-point is reachable through
			// the implicit nullable.unwrap branch.
			const InstructionFlags flagsToRemove = InstructionFlags.MayUnwrapNull | InstructionFlags.EndPointUnreachable;
			return (Argument.Flags & ~flagsToRemove) | InstructionFlags.ControlFlow;
		}

		public override StackType ResultType {
			get {
				if (Argument.ResultType == StackType.Void)
					return StackType.Void;
				else
					return StackType.O;
			}
		}

		internal override bool PrepareExtract(int childIndex, ExtractionContext ctx)
		{
			return base.PrepareExtract(childIndex, ctx)
				&& (ctx.FlagsBeingMoved & InstructionFlags.MayUnwrapNull) == 0;
		}

		internal override bool CanInlineIntoSlot(int childIndex, ILInstruction expressionBeingMoved)
		{
			// Inlining into nullable.rewrap is OK unless the expression being inlined
			// contains a nullable.wrap that isn't being re-wrapped within the expression being inlined.
			return base.CanInlineIntoSlot(childIndex, expressionBeingMoved)
				&& !expressionBeingMoved.HasFlag(InstructionFlags.MayUnwrapNull);
		}
	}
}
