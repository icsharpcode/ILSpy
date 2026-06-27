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

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.IL.Transforms;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Languages
{
	/// <summary>
	/// Debug Steps support for the C# language, shown in the Debug Steps pane like the ILAst
	/// language already does for IL transforms. A full decompile records AST transform groups with
	/// individual mutation steps inside; a selected step's index is replayed by re-decompiling with <see
	/// cref="DecompilationOptions.StepLimit"/>.
	/// </summary>
	partial class CSharpLanguage : IDebugStepProvider
	{
		Stepper stepper = new Stepper();

		public Stepper Stepper => stepper;

		public event EventHandler? StepperUpdated;

		// The C# AST step view has no options of its own (yet); the pane shows nothing above the
		// tree for C#, unlike ILAst's writing-options checkboxes.
		public object? StepOptions => null;

		partial void OnCSharpDecompiled(CSharpDecompiler decompiler, ITextOutput output, DecompilationOptions options)
		{
			// The button always shows so the pane is one click away; mirrors the ILAst language.
			// DockWorkspace is resolved lazily (an ImportingConstructor import would form a MEF
			// cycle via LanguageService -> Languages).
			(output as ISmartTextOutput)?.AddButton(Images.ViewCode, "Show Steps", delegate {
				AppComposition.TryGetExport<DockWorkspace>()?.ShowToolPane(DebugStepsPaneModel.PaneContentId);
			});
			// Only a full run refreshes the step list; a step-limited re-decompile (triggered by the
			// pane itself) must leave the tree and the user's selection intact.
			if (options.StepLimit == int.MaxValue)
			{
				stepper = decompiler.Stepper;
				StepperUpdated?.Invoke(this, EventArgs.Empty);
			}
		}
	}
}

#endif
