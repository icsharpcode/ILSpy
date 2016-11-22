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
using System.Threading;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Parameter class holding various arguments for <see cref="IILTransform.Run(ILFunction, ILTransformContext)"/>.
	/// </summary>
	public class ILTransformContext
	{
		public IDecompilerTypeSystem TypeSystem { get; set; }
		public DecompilerSettings Settings { get; set; }
		public CancellationToken CancellationToken { get; set; }
	}
	
	public interface IILTransform
	{
		void Run(ILFunction function, ILTransformContext context);
	}

	/// <summary>
	/// Interface for transforms that can be "single-stepped" for debug purposes.
	/// 
	/// After the transform has performed MaxStepCount steps, it throws a
	/// <see cref="StepLimitReachedException"/>.
	/// </summary>
	public interface ISingleStep
	{
		/// <summary>
		/// Limits the number of steps performed by the transform.
		/// 
		/// The default is int.MaxValue = unlimited.
		/// </summary>
		int MaxStepCount { get; set; }
	}

	/// <summary>
	/// Exception thrown when an IL transform runs into the <see cref="ISingleStep.MaxStepCount"/> limit.
	/// </summary>
	public class StepLimitReachedException : Exception
	{
	}

	/// <summary>
	/// Helper struct for use in transforms that implement ISingleStep.
	/// </summary>
	internal struct Stepper
	{
		public readonly int MaxStepCount;
		public int StepCount;

		public Stepper(int maxStepCount)
		{
			this.StepCount = 0;
			this.MaxStepCount = maxStepCount;
			if (maxStepCount == 0)
				throw new StepLimitReachedException();
		}

		/// <summary>
		/// Called immediately after a transform step was taken.
		/// </summary>
		public void Stepped()
		{
			if (++StepCount == MaxStepCount)
				throw new StepLimitReachedException();
		}
	}
}
