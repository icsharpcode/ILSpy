// Copyright (c) 2016 Daniel Grunwald
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

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Exception thrown when an IL transform runs into the <see cref="Stepper.MaxStepCount"/> limit.
	/// </summary>
	public class StepLimitReachedException : Exception
	{
	}

	/// <summary>
	/// Helper class that manages recording transform steps.
	/// </summary>
	public class Stepper
	{
		/// <summary>
		/// Gets whether stepping of built-in transforms is supported in this build of ICSharpCode.Decompiler.
		/// Usually only debug builds support transform stepping.
		/// </summary>
		public static bool SteppingAvailable {
			get {
#if STEP
				return true;
#else
				return false;
#endif
			}
		}
		
		/// <summary>
		/// Call this method immediately before performing a transform step.
		/// Used for debugging the IL transforms. Has no effect in release mode.
		/// 
		/// May throw <see cref="StepLimitReachedException"/> in debug mode.
		/// </summary>
		public void Step(string description, ILInstruction near)
		{
			// TODO: implement stepping
		}
	}
}
