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

namespace ICSharpCode.Decompiler.IL
{
	partial class ILInstruction
	{
		public bool MatchLdcI4(int val)
		{
			return OpCode == OpCode.LdcI4 && ((LdcI4)this).Value == val;
		}
		
		public bool MatchLdcI4(out int val)
		{
			var inst = this as LdcI4;
			if (inst != null) {
				val = inst.Value;
				return true;
			}
			val = 0;
			return false;
		}
		
		public bool MatchLdLoc(ILVariable variable)
		{
			var inst = this as LdLoc;
			return inst != null && inst.Variable == variable;
		}
		
		public bool MatchLdLoc(out ILVariable variable)
		{
			var inst = this as LdLoc;
			if (inst != null) {
				variable = inst.Variable;
				return true;
			}
			variable = null;
			return false;
		}
		
		public bool MatchLdThis()
		{
			var inst = this as LdLoc;
			return inst != null && inst.Variable.Kind == VariableKind.This;
		}
	}
}
