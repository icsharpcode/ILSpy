// Copyright (c) 2026 Siegfried Pammer
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

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.Tests.Helpers
{
	/// <summary>
	/// Shared setup for the tests that drive the transform pipeline itself rather than the code it
	/// produces: they decompile this assembly, which is on disk next to the tests and large enough to
	/// span many members, and they need a transform that fails on demand.
	/// </summary>
	static class StepperTesting
	{
		public const string SimulatedFailure = "Simulated transform failure";

		public static CSharpDecompiler CreateDecompiler()
		{
			var module = new PEFile("ICSharpCode.Decompiler.dll");
			var settings = new DecompilerSettings();
			var typeSystem = new DecompilerTypeSystem(module, new UniversalAssemblyResolver(null, false, null), settings);
			return new CSharpDecompiler(typeSystem, settings);
		}

		/// <summary>
		/// Throws while transforming the named method's top-level function, leaving the pipeline to
		/// unwind out of whatever step groups it had opened.
		/// </summary>
		public sealed class ThrowingILTransform(string methodName) : IILTransform
		{
			public void Run(ILFunction function, ILTransformContext context)
			{
				if (function.Parent == null && function.Method?.Name == methodName)
					throw new InvalidOperationException(SimulatedFailure);
			}
		}
	}
}
