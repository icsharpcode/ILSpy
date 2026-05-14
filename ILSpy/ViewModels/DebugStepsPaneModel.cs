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

using System.Composition;

using ICSharpCode.Decompiler.IL;

using ILSpy.Commands;

namespace ILSpy.ViewModels
{
	/// <summary>
	/// Bottom-aligned tool pane that surfaces the Stepper output from
	/// <see cref="Languages.BlockILLanguage"/>. Compiled only in Debug builds — Release
	/// users don't see the pane or the languages that populate it.
	/// </summary>
	[Export]
	[ExportToolPane(ContentId = PaneContentId, Alignment = ToolPaneAlignment.Bottom, Order = 1, IsVisibleByDefault = false)]
	[Shared]
	public sealed class DebugStepsPaneModel : ToolPaneModel
	{
		public const string PaneContentId = "DebugSteps";

		/// <summary>
		/// App-wide ILAst writing options shared between the BlockIL language (which reads
		/// them while emitting the transformed IL) and the DebugSteps view (whose four
		/// checkboxes toggle their values). Static singleton state because the language is
		/// MEF-shared and decompiles on background tasks that have no view-model reference.
		/// </summary>
		public static ILAstWritingOptions WritingOptions { get; } = new() {
			UseFieldSugar = true,
			UseLogicOperationSugar = true,
		};

		public DebugStepsPaneModel()
		{
			Id = PaneContentId;
			Title = "Debug Steps";
		}

		/// <summary>Instance accessor for XAML binding (Avalonia's static-source binding is awkward).</summary>
		public ILAstWritingOptions Options => WritingOptions;
	}
}

#endif
