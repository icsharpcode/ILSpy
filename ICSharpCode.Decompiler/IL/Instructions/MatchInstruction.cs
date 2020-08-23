// Copyright (c) 2020 Siegfried Pammer
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
	partial class MatchInstruction : ILInstruction
	{
		/* Pseudo-Code for interpreting a MatchInstruction:
			bool Eval()
			{
				var value = this.TestedOperand.Eval();
				if (this.CheckNotNull && value == null)
					return false;
				if (this.CheckType && !(value is this.Variable.Type))
					return false;
				if (this.IsDeconstructCall) {
					deconstructResult = new[numArgs];
					EvalCall(this.Method, value, out deconstructResult[0], .., out deconstructResult[numArgs-1]);
					// any occurrences of 'deconstruct.result' in the subPatterns will refer
					// to the values provided by evaluating the call.
				}
				Variable.Value = value;
				foreach (var subPattern in this.SubPatterns) {
					if (!subPattern.Eval())
						return false;
				}
				return true;
			}
		*/
		/* Examples of MatchInstructions:
			expr is var x:
				match(x = expr)

			expr is {} x:
				match.notnull(x = expr)

			expr is T x:
				match.type[T](x = expr)

			expr is C { A: var x } z:
				match.type[C](z = expr) {
				   match(x = z.A)
				}

			expr is C { A: var x, B: 42, C: { A: 4 } } z:
				match.type[C](z = expr) {
					match(x = z.A),
					comp (z.B == 42),
					match.notnull(temp2 = z.C) {
						comp (temp2.A == 4)
					}
				}

			expr is C(var x, var y, <4):
				match.type[C].deconstruct[C.Deconstruct](tmp1 = expr) {
					match(x = deconstruct.result1(tmp1)),
					match(y = deconstruct.result2(tmp1)),
					comp(deconstruct.result3(tmp1) < 4),
				}
			
			expr is C(1, D(2, 3)):
				match.type[C].deconstruct(c = expr) {
					comp(deconstruct.result1(c) == 1),
					match.type[D].deconstruct(d = deconstruct.result2(c)) {
						comp(deconstruct.result1(d) == 2),
						comp(deconstruct.result2(d) == 2),
					}
				}
		 */

		public bool IsVar => !CheckType && !CheckNotNull && !IsDeconstructCall && !IsDeconstructTuple && SubPatterns.Count == 0;

		public bool HasDesignator => Variable.LoadCount + Variable.AddressCount > SubPatterns.Count;

		public int NumPositionalPatterns {
			get {
				if (IsDeconstructCall)
					return method.Parameters.Count - (method.IsStatic ? 1 : 0);
				else if (IsDeconstructTuple)
					return TupleType.GetTupleElementTypes(variable.Type).Length;
				else
					return 0;
			}
		}

		public MatchInstruction(ILVariable variable, ILInstruction testedOperand)
			: this(variable, method: null, testedOperand)
		{
		}

		/// <summary>
		/// Checks whether the input instruction can represent a pattern matching operation.
		/// 
		/// Any pattern matching instruction will first evaluate the `testedOperand` (a descendant of `inst`),
		/// and then match the value of that operand against the pattern encoded in the instruction.
		/// The matching may have side-effects on the newly-initialized pattern variables
		/// (even if the pattern fails to match!).
		/// The pattern matching instruction evaluates to 1 (as I4) if the pattern matches, or 0 otherwise.
		/// </summary>
		public static bool IsPatternMatch(ILInstruction inst, out ILInstruction testedOperand)
		{
			switch (inst) {
				case MatchInstruction m:
					testedOperand = m.testedOperand;
					return true;
				case Comp comp:
					testedOperand = comp.Left;
					return IsConstant(comp.Right);
				case ILInstruction logicNot when logicNot.MatchLogicNot(out var operand):
					return IsPatternMatch(operand, out testedOperand);
				default:
					testedOperand = null;
					return false;
			}
		}

		private static bool IsConstant(ILInstruction inst)
		{
			return inst.OpCode switch
			{
				OpCode.LdcDecimal => true,
				OpCode.LdcF4 => true,
				OpCode.LdcF8 => true,
				OpCode.LdcI4 => true,
				OpCode.LdcI8 => true,
				OpCode.LdNull => true,
				_ => false
			};
		}

		internal IType GetDeconstructResultType(int index)
		{
			if (this.IsDeconstructCall) {
				int firstOutParam = (method.IsStatic ? 1 : 0);
				var outParamType = this.Method.Parameters[firstOutParam + index].Type;
				if (!(outParamType is ByReferenceType brt))
					throw new InvalidOperationException("deconstruct out param must be by reference");
				return brt.ElementType;
			}
			if (this.IsDeconstructTuple) {
				var elementTypes = TupleType.GetTupleElementTypes(this.variable.Type);
				return elementTypes[index];
			}
			throw new InvalidOperationException("GetDeconstructResultType requires a deconstruct pattern");
		}

		void AdditionalInvariants()
		{
			Debug.Assert(variable.Kind == VariableKind.PatternLocal);
			if (this.IsDeconstructCall) {
				Debug.Assert(IsDeconstructMethod(method));
			} else {
				Debug.Assert(method == null);
			}
			if (this.IsDeconstructTuple) {
				Debug.Assert(variable.Type.Kind == TypeKind.Tuple);
			}
			Debug.Assert(SubPatterns.Count >= NumPositionalPatterns);
			foreach (var subPattern in SubPatterns) {
				if (!IsPatternMatch(subPattern, out ILInstruction operand))
					Debug.Fail("Sub-Pattern must be a valid pattern");
				// the first child is TestedOperand
				int subPatternIndex = subPattern.ChildIndex - 1;
				if (subPatternIndex < NumPositionalPatterns) {
					// positional pattern
					Debug.Assert(operand is DeconstructResultInstruction  result && result.Index == subPatternIndex);
				} else if (operand.MatchLdFld(out var target, out _)) {
					Debug.Assert(target.MatchLdLoc(variable));
				} else if (operand is CallInstruction call) {
					Debug.Assert(call.Method.AccessorKind == System.Reflection.MethodSemanticsAttributes.Getter);
					Debug.Assert(call.Arguments[0].MatchLdLoc(variable));
				} else {
					Debug.Fail("Tested operand of sub-pattern is invalid.");
				}
			}
		}

		internal static bool IsDeconstructMethod(IMethod method)
		{
			if (method.Name != "Deconstruct")
				return false;
			if (method.ReturnType.Kind != TypeKind.Void)
				return false;
			int firstOutParam = (method.IsStatic ? 1 : 0);
			if (method.IsStatic) {
				if (!method.IsExtensionMethod)
					return false;
				// TODO : check whether all type arguments can be inferred from the first argument
			} else {
				if (method.TypeParameters.Count != 0)
					return false;
			}

			// TODO : check whether the method is ambigious

			if (method.Parameters.Count < firstOutParam)
				return false;

			for (int i = firstOutParam; i < method.Parameters.Count; i++) {
				if (!method.Parameters[i].IsOut)
					return false;
			}

			return true;
		}

		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			if (CheckNotNull) {
				output.Write(".notnull");
			}
			if (CheckType) {
				output.Write(".type[");
				variable.Type.WriteTo(output);
				output.Write(']');
			}
			if (IsDeconstructCall) {
				output.Write(".deconstruct[");
				method.WriteTo(output);
				output.Write(']');
			}
			if (IsDeconstructTuple) {
				output.Write(".tuple");
			}
			output.Write(' ');
			output.Write('(');
			Variable.WriteTo(output);
			output.Write(" = ");
			TestedOperand.WriteTo(output, options);
			output.Write(')');
			if (SubPatterns.Count > 0) {
				output.MarkFoldStart("{...}");
				output.WriteLine("{");
				output.Indent();
				foreach (var pattern in SubPatterns) {
					pattern.WriteTo(output, options);
					output.WriteLine();
				}
				output.Unindent();
				output.Write('}');
				output.MarkFoldEnd();
			}
		}
	}
}
