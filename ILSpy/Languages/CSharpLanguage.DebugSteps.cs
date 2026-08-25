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
using ICSharpCode.Decompiler.DebugSteps;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Languages
{
	/// <summary>
	/// Debug Steps support for the C# language: the pipeline the pane walks. A full decompile records
	/// the IL transforms of each member, the ILAst-to-C# seam, and the C# AST transforms into one
	/// <see cref="Stepper"/>; a selected step's index is replayed by re-decompiling with
	/// <see cref="DecompilationOptions.StepLimit"/>. A step that halts the IL phase leaves no C# to
	/// print, so the replay renders the halted ILAst instead - see <see cref="TryWriteILAst"/>.
	/// </summary>
	partial class CSharpLanguage
	{
		Stepper stepper = new Stepper();

		public Stepper Stepper => stepper;

		public event EventHandler? StepperUpdated;

		/// <summary>
		/// Writes the ILAst the IL transforms were halted in, in place of the C# the caller would
		/// otherwise print. The step limit stops the pipeline before that member ever reaches the C#
		/// builders, so there is nothing else worth showing.
		/// When the halt lands mid-type, the whole document becomes that one member's ILAst and the C#
		/// of the members already decompiled is dropped: one document in one language keeps the
		/// highlighting unambiguous, and which member the limit lands in follows decompilation order,
		/// not the user's selection.
		/// </summary>
		static partial void TryWriteILAst(ITextOutput output, DecompilationOptions options, CSharpDecompiler decompiler, ref bool handled)
		{
			if (decompiler.StepLimitHaltedFunction is not { } function)
				return;
			if (output is AvaloniaEditTextOutput avaloniaOutput)
			{
				// The dump is IL, not C#; without this the editor would highlight it as C#.
				avaloniaOutput.SyntaxExtensionOverride = ".il";
			}
			output.WriteLine();
			function.WriteTo(output, DebugStepsPaneModel.WritingOptions);
			handled = true;
		}

		/// <summary>
		/// Points the decompiler's IL phase at the shared stepper, but only while the Debug Steps pane
		/// is there to display what it records - see <see cref="DebugStepsPaneModel.IsRecording"/>.
		/// </summary>
		static partial void ConfigureStepRecording(CSharpDecompiler decompiler)
		{
			decompiler.RecordILTransformSteps = DebugStepsPaneModel.IsRecording;
		}

		/// <summary>
		/// Drops the recorded steps of the last run. The stepper pins every ILAst its steps captured, so
		/// this is what actually releases that memory once nothing is displaying it.
		/// </summary>
		internal void ReleaseSteps()
		{
			stepper = new Stepper();
		}

		partial void OnCSharpDecompiled(CSharpDecompiler decompiler, ITextOutput output, DecompilationOptions options)
		{
			// The button always shows so the pane is one click away.
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
