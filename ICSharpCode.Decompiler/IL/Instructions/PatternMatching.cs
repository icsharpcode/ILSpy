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
		
		public bool MatchLdThis()
		{
			var inst = this as LdLoc;
			return inst != null && inst.Variable.Kind == VariableKind.This;
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
		
		public bool MatchLeave(out BlockContainer targetContainer)
		{
			var inst = this as Leave;
			if (inst != null) {
				targetContainer = inst.TargetContainer;
				return true;
			}
			targetContainer = null;
			return false;
		}
		
		public bool MatchLeave(BlockContainer targetContainer)
		{
			var inst = this as Leave;
			return inst != null && inst.TargetContainer == targetContainer;
		}
	}
}
