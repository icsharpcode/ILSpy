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

namespace ICSharpCode.Decompiler.IL
{
	partial class LdLoc
	{
		ILVariable variable;
		
		public ILVariable Variable {
			get { return variable; }
			set {
				Debug.Assert(value != null);
				if (IsConnected)
					variable.LoadCount--;
				variable = value;
				if (IsConnected)
					variable.LoadCount++;
			}
		}
		
		protected override void Connected()
		{
			base.Connected();
			variable.LoadCount++;
		}
		
		protected override void Disconnected()
		{
			variable.LoadCount--;
			base.Disconnected();
		}
		
		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			Debug.Assert(phase <= ILPhase.InILReader || this.IsDescendantOf(variable.Scope));
		}
	}
	
	partial class LdLoca
	{
		ILVariable variable;
		
		public ILVariable Variable {
			get { return variable; }
			set {
				Debug.Assert(value != null);
				if (IsConnected)
					variable.AddressCount--;
				variable = value;
				if (IsConnected)
					variable.AddressCount++;
			}
		}
		
		protected override void Connected()
		{
			base.Connected();
			variable.AddressCount++;
		}
		
		protected override void Disconnected()
		{
			variable.AddressCount--;
			base.Disconnected();
		}
		
		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			Debug.Assert(phase <= ILPhase.InILReader || this.IsDescendantOf(variable.Scope));
		}
	}
	
	partial class StLoc
	{
		ILVariable variable;
		
		public ILVariable Variable {
			get { return variable; }
			set {
				Debug.Assert(value != null);
				if (IsConnected)
					variable.StoreCount--;
				variable = value;
				if (IsConnected)
					variable.StoreCount++;
			}
		}
		
		protected override void Connected()
		{
			base.Connected();
			variable.StoreCount++;
		}
		
		protected override void Disconnected()
		{
			variable.StoreCount--;
			base.Disconnected();
		}
		
		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			Debug.Assert(phase <= ILPhase.InILReader || this.IsDescendantOf(variable.Scope));
		}
	}
}
