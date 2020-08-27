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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL
{
	class TupleTransform
	{
		/// <summary>
		/// Matches an 'ldflda' instruction accessing a tuple element.
		/// 
		/// E.g. matches:
		/// <c>ldflda Item1(ldflda Rest(target))</c>
		/// </summary>
		public static bool MatchTupleFieldAccess(LdFlda inst, out IType tupleType, out ILInstruction target, out int position)
		{
			tupleType = inst.Field.DeclaringType;
			target = inst.Target;
			if (!inst.Field.Name.StartsWith("Item", StringComparison.Ordinal))
			{
				position = 0;
				return false;
			}
			if (!int.TryParse(inst.Field.Name.Substring(4), out position))
				return false;
			if (!TupleType.IsTupleCompatible(tupleType, out _))
				return false;
			while (target is LdFlda ldflda && ldflda.Field.Name == "Rest" && TupleType.IsTupleCompatible(ldflda.Field.DeclaringType, out _))
			{
				tupleType = ldflda.Field.DeclaringType;
				target = ldflda.Target;
				position += TupleType.RestPosition - 1;
			}
			return true;
		}

		/// <summary>
		/// Matches 'newobj TupleType(...)'.
		/// Takes care of flattening long tuples.
		/// </summary>
		public static bool MatchTupleConstruction(NewObj newobj, out ILInstruction[] arguments)
		{
			arguments = null;
			if (newobj == null)
				return false;
			if (!TupleType.IsTupleCompatible(newobj.Method.DeclaringType, out int elementCount))
				return false;
			arguments = new ILInstruction[elementCount];
			int outIndex = 0;
			while (elementCount >= TupleType.RestPosition)
			{
				if (newobj.Arguments.Count != TupleType.RestPosition)
					return false;
				for (int pos = 1; pos < TupleType.RestPosition; pos++)
				{
					arguments[outIndex++] = newobj.Arguments[pos - 1];
				}
				elementCount -= TupleType.RestPosition - 1;
				Debug.Assert(outIndex + elementCount == arguments.Length);
				newobj = newobj.Arguments.Last() as NewObj;
				if (newobj == null)
					return false;
				if (!TupleType.IsTupleCompatible(newobj.Method.DeclaringType, out int restElementCount))
					return false;
				if (restElementCount != elementCount)
					return false;
			}
			Debug.Assert(outIndex + elementCount == arguments.Length);
			if (newobj.Arguments.Count != elementCount)
				return false;
			for (int i = 0; i < elementCount; i++)
			{
				arguments[outIndex++] = newobj.Arguments[i];
			}
			return true;
		}
	}
}
