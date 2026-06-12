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

using System;
using System.Threading;

using ICSharpCode.Decompiler;

namespace ICSharpCode.ILSpy
{
	/// <summary>
	/// Options passed to <see cref="Languages.Language"/>'s Decompile* methods. Just enough
	/// to drive a single decompilation into a text view — project-export and view-state
	/// restoration aren't wired up yet.
	/// </summary>
	public sealed class DecompilationOptions
	{
		public DecompilerSettings DecompilerSettings { get; }
		public CancellationToken CancellationToken { get; set; }

		/// <summary>Decompile the whole module rather than just the selected member.</summary>
		public bool FullDecompilation { get; set; }

		/// <summary>Target directory for project export. Always null today since no save
		/// dialog is wired up; kept on the signature for future use.</summary>
		public string? SaveAsProjectDirectory { get; set; }

		/// <summary>Escape invalid C# identifiers so the output compiles.</summary>
		public bool EscapeInvalidIdentifiers { get; set; }

		/// <summary>
		/// Path to a strong-name key (<c>.snk</c>) for the exported project. When set, the project
		/// export copies the key next to the project file and emits an
		/// <c>&lt;AssemblyOriginatorKeyFile&gt;</c>. Honoured only on the project-export path
		/// (<see cref="SaveAsProjectDirectory"/> set); ignored otherwise. The key file itself
		/// lives on <see cref="ICSharpCode.Decompiler.CSharp.ProjectDecompiler.WholeProjectDecompiler.StrongNameKeyFile"/>.
		/// </summary>
		public string? StrongNameKeyFile { get; set; }

		/// <summary>
		/// Stop the IL-transform pipeline after this many steps. <see cref="int.MaxValue"/>
		/// means "run all transforms". The Debug Steps pane sets this to the index of a
		/// chosen step so it can show the partial state at that point. Honoured by
		/// <see cref="Languages.BlockILLanguage"/>; ignored by every other language.
		/// </summary>
		public int StepLimit { get; set; } = int.MaxValue;

		/// <summary>
		/// When true, transforms emit verbose debug information about their behaviour. Only
		/// meaningful in combination with <see cref="StepLimit"/> — the Debug Steps pane sets
		/// it on the "Debug this step" context-menu action.
		/// </summary>
		public bool IsDebug { get; set; }

		/// <summary>
		/// Optional sink for whole-project decompilation progress. Wired onto
		/// <see cref="ICSharpCode.Decompiler.CSharp.ProjectDecompiler.WholeProjectDecompiler.ProgressIndicator"/>
		/// on the project-export path so the export tab can show a determinate progress bar and the
		/// file currently being written; ignored on single-member decompiles.
		/// </summary>
		public IProgress<DecompilationProgress>? ProgressIndicator { get; set; }

		// Deliberately no parameterless constructor: every decompilation must make an explicit
		// choice of settings. Callers inside the app want the user's current settings (see
		// SettingsService.CreateEffectiveDecompilerSettings), and a silent new DecompilerSettings()
		// default has repeatedly masked exactly that bug.
		public DecompilationOptions(DecompilerSettings settings)
		{
			DecompilerSettings = settings;
		}
	}
}
