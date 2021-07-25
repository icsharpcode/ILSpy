// Copyright (c) 2021 Daniel Grunwald, Siegfried Pammer
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

#nullable enable

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	class PatternMatchingRefTypesTransform : IStatementTransform
	{
		/// <summary>
		/// stloc V(isinst T(testedOperand))
		/// call Use(..., comp.o(ldloc V != ldnull))
		/// =>
		/// call Use(..., match.type[T].notnull(V = testedOperand))
		/// 
		/// - or -
		/// 
		/// stloc S(isinst T(testedOperand))
		/// stloc V(ldloc S)
		/// call Use(..., comp.o(ldloc S != ldnull))
		/// =>
		/// call Use(..., match.type[T].notnull(V = testedOperand))
		/// </summary>
		void IStatementTransform.Run(Block block, int pos, StatementTransformContext context)
		{
			if (!context.Settings.PatternMatching)
				return;
			int startPos = pos;
			if (!block.Instructions[pos].MatchStLoc(out var s, out var value))
				return;
			IType? unboxType;
			if (value is UnboxAny unboxAny)
			{
				// stloc S(unbox.any T(isinst T(testedOperand)))
				unboxType = unboxAny.Type;
				value = unboxAny.Argument;
			}
			else
			{
				unboxType = null;
			}
			if (value is not IsInst { Argument: var testedOperand, Type: var type })
				return;
			if (type.IsReferenceType != true)
				return;
			if (!(unboxType == null || type.Equals(unboxType)))
				return;
			if (!s.IsSingleDefinition)
				return;
			if (s.Kind is not (VariableKind.Local or VariableKind.StackSlot))
				return;
			pos++;
			ILVariable v;
			if (block.Instructions.ElementAtOrDefault(pos) is StLoc stloc && stloc.Value.MatchLdLoc(s))
			{
				v = stloc.Variable;
				pos++;
				if (!v.IsSingleDefinition)
					return;
				if (v.Kind is not (VariableKind.Local or VariableKind.StackSlot))
					return;
				if (s.LoadCount != 2)
					return;
			}
			else
			{
				v = s;
			}

			if (!v.Type.Equals(type))
				return;
			if (pos >= block.Instructions.Count)
				return;
			var result = ILInlining.FindLoadInNext(block.Instructions[pos], s, testedOperand, InliningOptions.None);
			if (result.Type != ILInlining.FindResultType.Found)
				return;
			if (result.LoadInst is not LdLoc)
				return;
			bool invertCondition;
			if (result.LoadInst.Parent!.MatchCompNotEqualsNull(out _))
			{
				invertCondition = false;

			}
			else if (result.LoadInst.Parent!.MatchCompEqualsNull(out _))
			{
				invertCondition = true;
			}
			else
			{
				return;
			}

			context.Step($"Type pattern matching {v.Name}", block.Instructions[pos]);
			// call Use(..., match.type[T](V = testedOperand))

			var target = result.LoadInst.Parent;
			ILInstruction matchInstruction = new MatchInstruction(v, testedOperand) {
				CheckNotNull = true,
				CheckType = true
			};
			if (invertCondition)
			{
				matchInstruction = Comp.LogicNot(matchInstruction);
			}
			target.ReplaceWith(matchInstruction.WithILRange(target));
			block.Instructions.RemoveRange(startPos, pos - startPos);
			v.Kind = VariableKind.PatternLocal;
		}
	}
}
