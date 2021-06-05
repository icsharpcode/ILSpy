#nullable enable
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

using System.Diagnostics;

namespace ICSharpCode.Decompiler.IL
{
	public sealed partial class LdFlda
	{
		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			switch (field.DeclaringType.IsReferenceType)
			{
				case true:
					Debug.Assert(target.ResultType == StackType.O,
						"Class fields can only be accessed with an object on the stack");
					break;
				case false:
					Debug.Assert(target.ResultType == StackType.I || target.ResultType == StackType.Ref,
						"Struct fields can only be accessed with a pointer on the stack");
					break;
				case null:
					// field of unresolved type
					Debug.Assert(target.ResultType == StackType.O || target.ResultType == StackType.I
						|| target.ResultType == StackType.Ref || target.ResultType == StackType.Unknown,
						"Field of unresolved type with invalid target");
					break;
			}
		}
	}

	public sealed partial class StObj
	{
		/// <summary>
		/// For a store to a field or array element, C# will only throw NullReferenceException/IndexOfBoundsException
		/// after the value-to-be-stored has been computed.
		/// This means a LdFlda/LdElema used as target for StObj must have DelayExceptions==true to allow a translation to C#
		/// without changing the program semantics. See https://github.com/icsharpcode/ILSpy/issues/2050
		/// </summary>
		public bool CanInlineIntoTargetSlot(ILInstruction inst)
		{
			switch (inst.OpCode)
			{
				case OpCode.LdElema:
				case OpCode.LdFlda:
					Debug.Assert(inst.HasDirectFlag(InstructionFlags.MayThrow));
					// If the ldelema/ldflda may throw a non-delayed exception, inlining will cause it
					// to turn into a delayed exception after the translation to C#.
					// This is only valid if the value computation doesn't involve any side effects.
					return SemanticHelper.IsPure(this.Value.Flags);
				// Note that after inlining such a ldelema/ldflda, the normal inlining rules will
				// prevent us from inlining an effectful instruction into the value slot.
				default:
					return true;
			}
		}

		/// <summary>
		/// called as part of CheckInvariant()
		/// </summary>
		void CheckTargetSlot()
		{
			switch (this.Target.OpCode)
			{
				case OpCode.LdElema:
				case OpCode.LdFlda:
					if (this.Target.HasDirectFlag(InstructionFlags.MayThrow))
					{
						Debug.Assert(SemanticHelper.IsPure(this.Value.Flags));
					}
					break;
			}
		}
	}
}
