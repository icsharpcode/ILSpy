// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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

#if DEBUG

using System;

using ICSharpCode.Decompiler.IL.Transforms;

namespace ICSharpCode.ILSpy.Languages
{
	/// <summary>
	/// A language that surfaces a <see cref="Stepper"/> of decompiler-pipeline steps for the
	/// Debug Steps pane to visualise. Implemented by <see cref="ILAstLanguage"/> (one node per IL
	/// transform step) and <see cref="CSharpLanguage"/> (one node per C# AST transform). The pane
	/// binds to whichever current language is an <see cref="IDebugStepProvider"/>, so it no longer has to
	/// know the concrete language type.
	/// </summary>
	internal interface IDebugStepProvider
	{
		/// <summary>The step tree produced by the most recent full decompile.</summary>
		Stepper Stepper { get; }

		/// <summary>Raised after a full (non step-limited) decompile refreshes <see cref="Stepper"/>.</summary>
		event EventHandler? StepperUpdated;

		/// <summary>
		/// Language-specific options object the Debug Steps pane renders above the step tree
		/// (e.g. ILAst's writing-options checkboxes), or <see langword="null"/> when the language
		/// has no step options. The pane picks a template by the object's runtime type, so each
		/// language contributes its own controls.
		/// </summary>
		object? StepOptions { get; }
	}
}

#endif
