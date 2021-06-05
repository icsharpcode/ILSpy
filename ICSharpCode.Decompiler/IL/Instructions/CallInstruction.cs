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

using System;
using System.Diagnostics;

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL
{
	public abstract partial class CallInstruction : ILInstruction
	{
		public static CallInstruction Create(OpCode opCode, IMethod method)
		{
			switch (opCode)
			{
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

		public readonly IMethod Method;

		/// <summary>
		/// Gets/Sets whether the call has the 'tail.' prefix.
		/// </summary>
		public bool IsTail;

		/// <summary>
		/// Gets/Sets the type specified in the 'constrained.' prefix.
		/// Returns null if no 'constrained.' prefix exists for this call.
		/// </summary>
		public IType? ConstrainedTo;

		/// <summary>
		/// Gets whether the IL stack was empty at the point of this call.
		/// (not counting the arguments/return value of the call itself)
		/// </summary>
		public bool ILStackWasEmpty;

		protected CallInstruction(OpCode opCode, IMethod method) : base(opCode)
		{
			this.Method = method ?? throw new ArgumentNullException(nameof(method));
			this.Arguments = new InstructionCollection<ILInstruction>(this, 0);
		}

		/// <summary>
		/// Gets whether this is an instance call (i.e. whether the first argument is the 'this' pointer).
		/// </summary>
		public bool IsInstanceCall {
			get { return !(Method.IsStatic || OpCode == OpCode.NewObj); }
		}

		/// <summary>
		/// Gets the parameter for the argument with the specified index.
		/// Returns null for the <c>this</c> parameter.
		/// </summary>
		public IParameter? GetParameter(int argumentIndex)
		{
			int firstParamIndex = (Method.IsStatic || OpCode == OpCode.NewObj) ? 0 : 1;
			if (argumentIndex < firstParamIndex)
			{
				return null; // asking for 'this' parameter
			}
			return Method.Parameters[argumentIndex - firstParamIndex];
		}

		public override StackType ResultType {
			get {
				if (OpCode == OpCode.NewObj)
					return Method.DeclaringType.GetStackType();
				else
					return Method.ReturnType.GetStackType();
			}
		}

		/// <summary>
		/// Gets the expected stack type for passing the this pointer in a method call.
		/// Returns StackType.O for reference types (this pointer passed as object reference),
		/// and StackType.Ref for type parameters and value types (this pointer passed as managed reference).
		/// 
		/// Returns StackType.Unknown if the input type is unknown.
		/// </summary>
		internal static StackType ExpectedTypeForThisPointer(IType type)
		{
			if (type.Kind == TypeKind.TypeParameter)
				return StackType.Ref;
			switch (type.IsReferenceType)
			{
				case true:
					return StackType.O;
				case false:
					return StackType.Ref;
				default:
					return StackType.Unknown;
			}
		}

		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			int firstArgument = (OpCode != OpCode.NewObj && !Method.IsStatic) ? 1 : 0;
			Debug.Assert(Method.Parameters.Count + firstArgument == Arguments.Count);
			if (firstArgument == 1)
			{
				if (!(Arguments[0].ResultType == ExpectedTypeForThisPointer(ConstrainedTo ?? Method.DeclaringType)))
					Debug.Fail($"Stack type mismatch in 'this' argument in call to {Method.Name}()");
			}
			for (int i = 0; i < Method.Parameters.Count; ++i)
			{
				if (!(Arguments[firstArgument + i].ResultType == Method.Parameters[i].Type.GetStackType()))
					Debug.Fail($"Stack type mismatch in parameter {i} in call to {Method.Name}()");
			}
		}

		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			if (ConstrainedTo != null)
			{
				output.Write("constrained[");
				ConstrainedTo.WriteTo(output);
				output.Write("].");
			}
			if (IsTail)
				output.Write("tail.");
			output.Write(OpCode);
			output.Write(' ');
			Method.WriteTo(output);
			output.Write('(');
			for (int i = 0; i < Arguments.Count; i++)
			{
				if (i > 0)
					output.Write(", ");
				Arguments[i].WriteTo(output, options);
			}
			output.Write(')');
		}

		protected internal sealed override bool PerformMatch(ILInstruction? other, ref Patterns.Match match)
		{
			CallInstruction? o = other as CallInstruction;
			return o != null && this.OpCode == o.OpCode && this.Method.Equals(o.Method) && this.IsTail == o.IsTail
				&& object.Equals(this.ConstrainedTo, o.ConstrainedTo)
				&& Patterns.ListMatch.DoMatch(this.Arguments, o.Arguments, ref match);
		}
	}

	partial class Call : ILiftableInstruction
	{
		/// <summary>
		/// Calls can only be lifted when calling a lifted operator.
		/// Note that the semantics of such a lifted call depend on the type of operator:
		/// we follow C# semantics here.
		/// </summary>
		public bool IsLifted => Method is CSharp.Resolver.ILiftedOperator;

		public StackType UnderlyingResultType {
			get {
				if (Method is CSharp.Resolver.ILiftedOperator liftedOp)
					return liftedOp.NonLiftedReturnType.GetStackType();
				else
					return Method.ReturnType.GetStackType();
			}
		}
	}
}
