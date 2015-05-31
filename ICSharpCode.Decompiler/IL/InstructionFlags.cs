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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.IL
{
	[Flags]
	public enum InstructionFlags
	{
		None = 0,
		/// <summary>
		/// The instruction may read from local variables.
		/// </summary>
		MayReadLocals  = 0x10,
		/// <summary>
		/// The instruction may write to local variables.
		/// </summary>
		/// <remarks>
		/// This flag is not set for indirect write to local variables through pointers.
		/// Ensure you also check the SideEffect flag.
		/// </remarks>
		MayWriteLocals = 0x20,
		/// <summary>
		/// The instruction may have side effects, such as accessing heap memory,
		/// performing system calls, writing to local variables through pointers, etc.
		/// </summary>
		/// <remarks>
		/// Throwing an exception or directly writing to local variables or the evaluation stack
		/// is not considered a side effect, and is modeled by separate flags.
		/// </remarks>
		SideEffect = 0x40,
		
		/// <summary>
		/// The instruction may throw an exception.
		/// </summary>
		MayThrow = 0x100,
		/// <summary>
		/// The instruction may exit with a branch or return.
		/// </summary>
		MayBranch = 0x200,
		/// <summary>
		/// The instruction performs unconditional control flow, so that its endpoint is unreachable.
		/// </summary>
		/// <remarks>
		/// If EndPointUnreachable is set, either MayThrow or MayBranch should also be set
		/// (unless the instruction represents an infinite loop).
		/// </remarks>
		EndPointUnreachable = 0x400,
	}
}
