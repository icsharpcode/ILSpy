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
using System.Linq;

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
		/// Returns StackType.Ref if constrainedTo is not null,
		/// StackType.O for reference types (this pointer passed as object reference),
		/// and StackType.Ref for type parameters and value types (this pointer passed as managed reference).
		/// 
		/// Returns StackType.Unknown if the input type is unknown.
		/// </summary>
		internal static StackType ExpectedTypeForThisPointer(IType declaringType, IType? constrainedTo)
		{
			if (constrainedTo != null)
				return StackType.Ref;
			if (declaringType.Kind == TypeKind.TypeParameter)
				return StackType.Ref;
			switch (declaringType.IsReferenceType)
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
				if (!(Arguments[0].ResultType == ExpectedTypeForThisPointer(Method.DeclaringType, ConstrainedTo)))
					Debug.Fail($"Stack type mismatch in 'this' argument in call to {Method.Name}()");
			}
			for (int i = 0; i < Method.Parameters.Count; ++i)
			{
				if (!(Arguments[firstArgument + i].ResultType == Method.Parameters[i].Type.GetStackType()))
					Debug.Fail($"Stack type mismatch in parameter {i} in call to {Method.Name}()");
			}
		}

		protected override void WriteToCore(ITextOutput output, ILAstWritingOptions options)
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

		internal override bool SatisfiesSlotRestrictionForInlining(int childIndex, ILInstruction newChild)
		{
			// The receiver of a call to a C# 14 instance compound assignment operator becomes the
			// target of "x op= y", so the expression taking its place has to be one C# accepts
			// there and one that still binds the operator this call names. Classification already
			// implies the corresponding decompiler setting is on.
			if (childIndex == 0 && Method.IsOperator && !Method.IsStatic
				&& !CanBeCompoundAssignmentReceiver(newChild))
			{
				return false;
			}
			return base.SatisfiesSlotRestrictionForInlining(childIndex, newChild);
		}

		/// <summary>
		/// Gets whether <paramref name="replacement"/> can stand in for the receiver of this call
		/// to an instance compound assignment operator. The receiver becomes the target of
		/// "x op= y", so the replacement has to denote a storage location C# accepts there, and it
		/// has to bind the operator the call names rather than one a more derived type brings into
		/// scope. Consulted by inlining through the slot restriction above, and by copy
		/// propagation, which substitutes receivers the same way.
		/// </summary>
		internal bool CanBeCompoundAssignmentReceiver(ILInstruction replacement)
		{
			var contextMethod = this.Ancestors.OfType<ILFunction>().FirstOrDefault()?.Method;
			if (Transforms.ILInlining.IsReadonlyCompoundAssignmentTarget(replacement, contextMethod))
			{
				// The target is not an assignable variable, so it cannot take the place of
				// "x" in "x op= y"; the copy the receiver slot holds is what keeps the
				// operator form legal.
				return false;
			}
			switch (replacement.OpCode)
			{
				case OpCode.LdLoc:
				case OpCode.LdObj:
				case OpCode.LdFlda:
				case OpCode.LdsFlda:
					break;
				default:
					// anything else would turn the target into "GetX() op= y"
					return false;
			}
			return !ReplacementMayRebindOperator(Method, GetReceiverType(replacement));
		}

		/// <summary>
		/// Gets whether "x op= y" with x of type <paramref name="receiverType"/> could bind an
		/// operator other than <paramref name="op"/>, the one the call being rewritten names.
		/// The form selects its operator from the static type of x, so an operator introduced
		/// anywhere between that type and the type declaring <paramref name="op"/> can take the
		/// call. This is a declaration-existence check, deliberately one-sided: overloads next to
		/// the operator itself cannot be selected by the receiver's type, and an override of a
		/// virtual operator occupies the slot of the operator the call names. Where it errs it
		/// only refuses a substitution, which costs a local copy in the output, never its
		/// correctness. The exact form of the question, argument applicability included, is
		/// CSharpResolver.WouldRebindOperator, which the C# transforms use; this check stays
		/// approximate because the IL layer does not bind.
		/// </summary>
		static bool ReplacementMayRebindOperator(IMethod op, IType receiverType)
		{
			if (op.DeclaringType.Kind == TypeKind.Interface)
			{
				// An operator declared in an interface is only reachable from a receiver of
				// interface (or type-parameter) type: class member lookup does not see interface
				// members. System.Object stays permissive - it is the stack-type placeholder
				// several ILAst nodes carry.
				return receiverType.Kind is not (TypeKind.Interface or TypeKind.TypeParameter)
					&& !receiverType.IsKnownType(KnownTypeCode.Object);
			}
			// Both the checked and the unchecked operator can take the call: which of them applies
			// depends on the checked context the assignment ends up in.
			string siblingName = UserDefinedCompoundAssign.GetCheckedSiblingName(op.Name);
			foreach (var type in receiverType.GetAllBaseTypeDefinitions())
			{
				if (type == op.DeclaringTypeDefinition)
					continue;
				if (!type.GetAllBaseTypeDefinitions().Contains(op.DeclaringTypeDefinition))
					continue;
				foreach (var m in type.Methods)
				{
					if (m.IsOperator && !m.IsStatic && !m.IsOverride
						&& m.Accessibility == Accessibility.Public
						&& (m.Name == op.Name || m.Name == siblingName))
					{
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Gets the type that decides which operator a receiver expression binds.
		/// </summary>
		/// <remarks>
		/// A stack slot standing in for a value carries whatever type the reader gave it - for a
		/// flushed expression stack that is just the stack type, which says nothing about the
		/// operators in play. Such a slot is a pure alias, so the value stored into it is what
		/// really ends up as the receiver.
		/// </remarks>
		IType GetReceiverType(ILInstruction expr)
		{
			while (expr is LdLoc { Variable: { Kind: VariableKind.StackSlot, IsSingleDefinition: true } v }
				&& v.StoreInstructions.SingleOrDefault() is StLoc store)
			{
				expr = store.Value;
			}
			var type = expr.InferType(Method.Compilation);
			// An address-taking receiver denotes the storage location itself.
			return type is ByReferenceType byRef ? byRef.ElementType : type;
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
