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

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL
{
	partial class ILInstruction
	{
		public bool MatchLdcI4(int val)
		{
			return OpCode == OpCode.LdcI4 && ((LdcI4)this).Value == val;
		}

		public bool MatchLdcF4(float value)
		{
			return MatchLdcF4(out var v) && v == value;
		}

		public bool MatchLdcF8(double value)
		{
			return MatchLdcF8(out var v) && v == value;
		}

		/// <summary>
		/// Matches ldc.i4, ldc.i8, and extending conversions.
		/// </summary>
		public bool MatchLdcI(out long val)
		{
			if (MatchLdcI8(out val))
				return true;
			if (MatchLdcI4(out int intVal)) {
				val = intVal;
				return true;
			}
			if (this is Conv conv) {
				if (conv.Kind == ConversionKind.SignExtend) {
					return conv.Argument.MatchLdcI(out val);
				} else if (conv.Kind == ConversionKind.ZeroExtend && conv.InputType == StackType.I4) {
					if (conv.Argument.MatchLdcI(out val)) {
						// clear top 32 bits
						val &= uint.MaxValue;
						return true;
					}
				}
			}
			return false;
		}

		public bool MatchLdcI(long val)
		{
			return MatchLdcI(out long v) && v == val;
		}

		public bool MatchLdLoc(ILVariable variable)
		{
			var inst = this as LdLoc;
			return inst != null && inst.Variable == variable;
		}

		public bool MatchLdLoca(ILVariable variable)
		{
			var inst = this as LdLoca;
			return inst != null && inst.Variable == variable;
		}

		/// <summary>
		/// Matches either ldloc (if the variable is a reference type), or ldloca (otherwise).
		/// </summary>
		public bool MatchLdLocRef(ILVariable variable)
		{
			return MatchLdLocRef(out var v) && v == variable;
		}

		/// <summary>
		/// Matches either ldloc (if the variable is a reference type), or ldloca (otherwise).
		/// </summary>
		public bool MatchLdLocRef(out ILVariable variable)
		{
			switch (this) {
				case LdLoc ldloc:
					variable = ldloc.Variable;
					return variable.Type.IsReferenceType == true;
				case LdLoca ldloca:
					variable = ldloca.Variable;
					return variable.Type.IsReferenceType != true || variable.Type.Kind == TypeKind.TypeParameter;
				default:
					variable = null;
					return false;
			}
		}

		public bool MatchLdThis()
		{
			var inst = this as LdLoc;
			return inst != null && inst.Variable.Kind == VariableKind.Parameter && inst.Variable.Index < 0;
		}

		public bool MatchStLoc(out ILVariable variable)
		{
			var inst = this as StLoc;
			if (inst != null) {
				variable = inst.Variable;
				return true;
			}
			variable = null;
			return false;
		}

		public bool MatchStLoc(ILVariable variable, out ILInstruction value)
		{
			var inst = this as StLoc;
			if (inst != null && inst.Variable == variable) {
				value = inst.Value;
				return true;
			}
			value = null;
			return false;
		}

		public bool MatchLdLen(StackType type, out ILInstruction array)
		{
			var inst = this as LdLen;
			if (inst != null && inst.ResultType == type) {
				array = inst.Array;
				return true;
			}
			array = null;
			return false;
		}

		public bool MatchReturn(out ILInstruction value)
		{
			var inst = this as Leave;
			if (inst != null && inst.IsLeavingFunction) {
				value = inst.Value;
				return true;
			}
			value = default(ILInstruction);
			return false;
		}

		public bool MatchBranch(out Block targetBlock)
		{
			var inst = this as Branch;
			if (inst != null) {
				targetBlock = inst.TargetBlock;
				return true;
			}
			targetBlock = null;
			return false;
		}

		public bool MatchBranch(Block targetBlock)
		{
			var inst = this as Branch;
			return inst != null && inst.TargetBlock == targetBlock;
		}

		public bool MatchLeave(out BlockContainer targetContainer, out ILInstruction value)
		{
			var inst = this as Leave;
			if (inst != null) {
				targetContainer = inst.TargetContainer;
				value = inst.Value;
				return true;
			}
			targetContainer = null;
			value = null;
			return false;
		}

		public bool MatchLeave(BlockContainer targetContainer, out ILInstruction value)
		{
			var inst = this as Leave;
			if (inst != null && targetContainer == inst.TargetContainer) {
				value = inst.Value;
				return true;
			}
			value = null;
			return false;
		}

		public bool MatchLeave(out BlockContainer targetContainer)
		{
			var inst = this as Leave;
			if (inst != null && inst.Value.MatchNop()) {
				targetContainer = inst.TargetContainer;
				return true;
			}
			targetContainer = null;
			return false;
		}

		public bool MatchLeave(BlockContainer targetContainer)
		{
			var inst = this as Leave;
			return inst != null && inst.TargetContainer == targetContainer && inst.Value.MatchNop();
		}

		public bool MatchIfInstruction(out ILInstruction condition, out ILInstruction trueInst, out ILInstruction falseInst)
		{
			var inst = this as IfInstruction;
			if (inst != null) {
				condition = inst.Condition;
				trueInst = inst.TrueInst;
				falseInst = inst.FalseInst;
				return true;
			}
			condition = null;
			trueInst = null;
			falseInst = null;
			return false;
		}

		public bool MatchIfInstructionPositiveCondition(out ILInstruction condition, out ILInstruction trueInst, out ILInstruction falseInst)
		{
			if (MatchIfInstruction(out condition, out trueInst, out falseInst)) {
				// Swap trueInst<>falseInst for every logic.not in the condition.
				while (condition.MatchLogicNot(out var arg)) {
					condition = arg;
					ILInstruction tmp = trueInst;
					trueInst = falseInst;
					falseInst = tmp;
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Matches an if instruction where the false instruction is a nop.
		/// </summary>
		public bool MatchIfInstruction(out ILInstruction condition, out ILInstruction trueInst)
		{
			var inst = this as IfInstruction;
			if (inst != null && inst.FalseInst.MatchNop()) {
				condition = inst.Condition;
				trueInst = inst.TrueInst;
				return true;
			}
			condition = null;
			trueInst = null;
			return false;
		}

		/// <summary>
		/// Matches a 'logic and' instruction ("if (a) b else ldc.i4 0").
		/// Note: unlike C# '&amp;&amp;', this instruction is not limited to booleans,
		/// but allows passing through arbitrary I4 values on the rhs (but not on the lhs).
		/// </summary>
		public bool MatchLogicAnd(out ILInstruction lhs, out ILInstruction rhs)
		{
			var inst = this as IfInstruction;
			if (inst != null && inst.FalseInst.MatchLdcI4(0)) {
				lhs = inst.Condition;
				rhs = inst.TrueInst;
				return true;
			}
			lhs = null;
			rhs = null;
			return false;
		}

		/// <summary>
		/// Matches a 'logic or' instruction ("if (a) ldc.i4 1 else b").
		/// Note: unlike C# '||', this instruction is not limited to booleans,
		/// but allows passing through arbitrary I4 values on the rhs (but not on the lhs).
		/// </summary>
		public bool MatchLogicOr(out ILInstruction lhs, out ILInstruction rhs)
		{
			var inst = this as IfInstruction;
			if (inst != null && inst.TrueInst.MatchLdcI4(1)) {
				lhs = inst.Condition;
				rhs = inst.FalseInst;
				return true;
			}
			lhs = null;
			rhs = null;
			return false;
		}

		/// <summary>
		/// Matches an logical negation.
		/// </summary>
		public bool MatchLogicNot(out ILInstruction arg)
		{
			if (this is Comp comp && comp.Kind == ComparisonKind.Equality
				&& comp.LiftingKind == ComparisonLiftingKind.None
				&& comp.Right.MatchLdcI4(0)) {
				arg = comp.Left;
				return true;
			}
			arg = null;
			return false;
		}

		public bool MatchTryCatchHandler(out ILVariable variable)
		{
			var inst = this as TryCatchHandler;
			if (inst != null) {
				variable = inst.Variable;
				return true;
			}
			variable = null;
			return false;
		}

		/// <summary>
		/// Matches comp(left == right) or logic.not(comp(left != right)).
		/// </summary>
		public bool MatchCompEquals(out ILInstruction left, out ILInstruction right)
		{
			ILInstruction thisInst = this;
			var compKind = ComparisonKind.Equality;
			while (thisInst.MatchLogicNot(out var arg) && arg is Comp) {
				thisInst = arg;
				if (compKind == ComparisonKind.Equality)
					compKind = ComparisonKind.Inequality;
				else
					compKind = ComparisonKind.Equality;
			}
			if (thisInst is Comp comp && comp.Kind == compKind && !comp.IsLifted) {
				left = comp.Left;
				right = comp.Right;
				return true;
			} else {
				left = null;
				right = null;
				return false;
			}
		}

		/// <summary>
		/// Matches 'comp(arg == ldnull)'
		/// </summary>
		public bool MatchCompEqualsNull(out ILInstruction arg)
		{
			if (!MatchCompEquals(out var left, out var right)) {
				arg = null;
				return false;
			}
			if (right.MatchLdNull()) {
				arg = left;
				return true;
			} else if (left.MatchLdNull()) {
				arg = right;
				return true;
			} else {
				arg = null;
				return false;
			}
		}

		/// <summary>
		/// Matches 'comp(arg != ldnull)'
		/// </summary>
		public bool MatchCompNotEqualsNull(out ILInstruction arg)
		{
			if (!MatchCompNotEquals(out var left, out var right)) {
				arg = null;
				return false;
			}
			if (right.MatchLdNull()) {
				arg = left;
				return true;
			} else if (left.MatchLdNull()) {
				arg = right;
				return true;
			} else {
				arg = null;
				return false;
			}
		}

		/// <summary>
		/// Matches comp(left != right) or logic.not(comp(left == right)).
		/// </summary>
		public bool MatchCompNotEquals(out ILInstruction left, out ILInstruction right)
		{
			ILInstruction thisInst = this;
			var compKind = ComparisonKind.Inequality;
			while (thisInst.MatchLogicNot(out var arg) && arg is Comp) {
				thisInst = arg;
				if (compKind == ComparisonKind.Equality)
					compKind = ComparisonKind.Inequality;
				else
					compKind = ComparisonKind.Equality;
			}
			if (thisInst is Comp comp && comp.Kind == compKind && !comp.IsLifted) {
				left = comp.Left;
				right = comp.Right;
				return true;
			} else {
				left = null;
				right = null;
				return false;
			}
		}

		public bool MatchLdFld(out ILInstruction target, out IField field)
		{
			if (this is LdObj ldobj && ldobj.Target is LdFlda ldflda && ldobj.UnalignedPrefix == 0 && !ldobj.IsVolatile) {
				field = ldflda.Field;
				if (field.DeclaringType.IsReferenceType == true || !ldflda.Target.MatchAddressOf(out target, out _)) {
					target = ldflda.Target;
				}
				return true;
			}
			target = null;
			field = null;
			return false;
		}
		
		public bool MatchLdsFld(out IField field)
		{
			if (this is LdObj ldobj && ldobj.Target is LdsFlda ldsflda && ldobj.UnalignedPrefix == 0 && !ldobj.IsVolatile) {
				field = ldsflda.Field;
				return true;
			}
			field = null;
			return false;
		}

		public bool MatchLdsFld(IField field)
		{
			return MatchLdsFld(out var f) && f.Equals(field);
		}

		public bool MatchStsFld(out IField field, out ILInstruction value)
		{
			if (this is StObj stobj && stobj.Target is LdsFlda ldsflda && stobj.UnalignedPrefix == 0 && !stobj.IsVolatile) {
				field = ldsflda.Field;
				value = stobj.Value;
				return true;
			}
			field = null;
			value = null;
			return false;
		}

		public bool MatchStFld(out ILInstruction target, out IField field, out ILInstruction value)
		{
			if (this is StObj stobj && stobj.Target is LdFlda ldflda && stobj.UnalignedPrefix == 0 && !stobj.IsVolatile) {
				target = ldflda.Target;
				field = ldflda.Field;
				value = stobj.Value;
				return true;
			}
			target = null;
			field = null;
			value = null;
			return false;
		}
		
		public bool MatchBinaryNumericInstruction(BinaryNumericOperator @operator)
		{
			var op = this as BinaryNumericInstruction;
			return op != null && op.Operator == @operator;
		}
		
		public bool MatchBinaryNumericInstruction(BinaryNumericOperator @operator, out ILInstruction left, out ILInstruction right)
		{
			var op = this as BinaryNumericInstruction;
			if (op != null && op.Operator == @operator) {
				left = op.Left;
				right = op.Right;
				return true;
			}
			left = null;
			right = null;
			return false;
		}
		
		public bool MatchBinaryNumericInstruction(out BinaryNumericOperator @operator, out ILInstruction left, out ILInstruction right)
		{
			var op = this as BinaryNumericInstruction;
			if (op != null) {
				@operator = op.Operator;
				left = op.Left;
				right = op.Right;
				return true;
			}
			@operator = BinaryNumericOperator.None;
			left = null;
			right = null;
			return false;
		}
		
		/// <summary>
		/// If this instruction is a conversion of the specified kind, return its argument.
		/// Otherwise, return the instruction itself.
		/// </summary>
		/// <remarks>
		/// Does not unwrap lifted conversions.
		/// </remarks>
		public virtual ILInstruction UnwrapConv(ConversionKind kind)
		{
			return this;
		}
	}
}
