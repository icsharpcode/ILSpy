// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.IL.ControlFlow
{
	/// <summary>
	/// This exception is thrown when we find something else than we expect from the C# compiler.
	/// This aborts the analysis and makes the whole transform fail.
	/// </summary>
	class SymbolicAnalysisFailedException : Exception
	{
		public SymbolicAnalysisFailedException() { }
		public SymbolicAnalysisFailedException(string message) : base(message) { }
	}

	enum SymbolicValueType
	{
		/// <summary>
		/// Unknown value
		/// </summary>
		Unknown,
		/// <summary>
		/// int: Constant (result of ldc.i4)
		/// </summary>
		IntegerConstant,
		/// <summary>
		/// int: State + Constant
		/// </summary>
		State,
		/// <summary>
		/// This pointer (result of ldarg.0)
		/// </summary>
		This,
		/// <summary>
		/// bool: State == Constant
		/// </summary>
		StateEquals,
		/// <summary>
		/// bool: State != Constant
		/// </summary>
		StateInEquals
	}

	struct SymbolicValue
	{
		public readonly int Constant;
		public readonly SymbolicValueType Type;

		public SymbolicValue(SymbolicValueType type, int constant = 0)
		{
			this.Type = type;
			this.Constant = constant;
		}

		public SymbolicValue AsBool()
		{
			if (Type == SymbolicValueType.State) {
				// convert state integer to bool:
				// if (state + c) = if (state + c != 0) = if (state != -c)
				return new SymbolicValue(SymbolicValueType.StateInEquals, unchecked(-Constant));
			}
			return this;
		}
		public override string ToString()
		{
			return string.Format("[SymbolicValue {0}: {1}]", this.Type, this.Constant);
		}
	}

	class SymbolicEvaluationContext
	{
		readonly IField stateField;
		readonly List<ILVariable> stateVariables = new List<ILVariable>();

		public SymbolicEvaluationContext(IField stateField)
		{
			this.stateField = stateField;
		}

		public void AddStateVariable(ILVariable v)
		{
			if (!stateVariables.Contains(v))
				stateVariables.Add(v);
		}

		static readonly SymbolicValue Failed = new SymbolicValue(SymbolicValueType.Unknown);

		public SymbolicValue Eval(ILInstruction inst)
		{
			if (inst is BinaryNumericInstruction bni && bni.Operator == BinaryNumericOperator.Sub && !bni.CheckForOverflow) {
				var left = Eval(bni.Left);
				var right = Eval(bni.Right);
				if (left.Type != SymbolicValueType.State && left.Type != SymbolicValueType.IntegerConstant)
					return Failed;
				if (right.Type != SymbolicValueType.IntegerConstant)
					return Failed;
				return new SymbolicValue(left.Type, unchecked(left.Constant - right.Constant));
			} else if (inst.MatchLdFld(out var target, out var field)) {
				if (Eval(target).Type != SymbolicValueType.This)
					return Failed;
				if (field.MemberDefinition != stateField)
					return Failed;
				return new SymbolicValue(SymbolicValueType.State);
			} else if (inst.MatchLdLoc(out var loadedVariable)) {
				if (stateVariables.Contains(loadedVariable))
					return new SymbolicValue(SymbolicValueType.State);
				else if (loadedVariable.Kind == VariableKind.Parameter && loadedVariable.Index < 0)
					return new SymbolicValue(SymbolicValueType.This);
				else
					return Failed;
			} else if (inst.MatchLdcI4(out var value)) {
				return new SymbolicValue(SymbolicValueType.IntegerConstant, value);
			} else if (inst is Comp comp) {
				var left = Eval(comp.Left);
				var right = Eval(comp.Right);
				if (left.Type != SymbolicValueType.State || right.Type != SymbolicValueType.IntegerConstant)
					return Failed;
				// bool: (state + left.Constant == right.Constant)
				// bool: (state == right.Constant - left.Constant)
				if (comp.Kind == ComparisonKind.Equality)
					return new SymbolicValue(SymbolicValueType.StateEquals, unchecked(right.Constant - left.Constant));
				else if (comp.Kind == ComparisonKind.Inequality)
					return new SymbolicValue(SymbolicValueType.StateInEquals, unchecked(right.Constant - left.Constant));
				else
					return Failed;
			} else if (inst is LogicNot logicNot) {
				SymbolicValue val = Eval(logicNot.Argument).AsBool();
				if (val.Type == SymbolicValueType.StateEquals)
					return new SymbolicValue(SymbolicValueType.StateInEquals, val.Constant);
				else if (val.Type == SymbolicValueType.StateInEquals)
					return new SymbolicValue(SymbolicValueType.StateEquals, val.Constant);
				else
					return Failed;
			} else {
				return Failed;
			}
		}
	}
}
