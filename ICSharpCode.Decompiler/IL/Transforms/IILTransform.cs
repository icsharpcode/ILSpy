// Copyright (c) 2015 Daniel Grunwald
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
using System.Threading;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Per-function IL transform.
	/// </summary>
	public interface IILTransform
	{
		void Run(ILFunction function, ILTransformContext context);
	}

	/// <summary>
	/// Parameter class holding various arguments for <see cref="IILTransform.Run(ILFunction, ILTransformContext)"/>.
	/// </summary>
	public class ILTransformContext
	{
		public ILFunction Function { get; }
		public IDecompilerTypeSystem TypeSystem { get; }
		public DecompilerSettings Settings { get; }
		public CancellationToken CancellationToken { get; set; }
		public Stepper Stepper { get; set; }

		public ILTransformContext(ILFunction function, IDecompilerTypeSystem typeSystem, DecompilerSettings settings = null)
		{
			this.Function = function ?? throw new ArgumentNullException(nameof(function));
			this.TypeSystem = typeSystem ?? throw new ArgumentNullException(nameof(typeSystem));
			this.Settings = settings ?? new DecompilerSettings();
			Stepper = new Stepper();
		}

		public ILTransformContext(ILTransformContext context)
		{
			this.Function = context.Function;
			this.TypeSystem = context.TypeSystem;
			this.Settings = context.Settings;
			this.CancellationToken = context.CancellationToken;
			this.Stepper = context.Stepper;
		}

		/// <summary>
		/// Call this method immediately before performing a transform step.
		/// Unlike <c>context.Stepper.Step()</c>, calls to this method are only compiled in debug builds.
		/// </summary>
		[Conditional("STEP")]
		internal void Step(string description, ILInstruction near)
		{
			Stepper.Step(description, near);
		}
		
		[Conditional("STEP")]
		internal void StepStartGroup(string description, ILInstruction near = null)
		{
			Stepper.StartGroup(description, near);
		}

		[Conditional("STEP")]
		internal void StepEndGroup(bool keepIfEmpty = false)
		{
			Stepper.EndGroup(keepIfEmpty);
		}
	}
}
