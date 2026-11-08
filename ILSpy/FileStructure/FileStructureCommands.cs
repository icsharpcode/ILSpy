// Copyright (c) 2026 Piero Viano
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

using System.Composition;

using Avalonia.Controls;
using Avalonia.Controls.Primitives;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.Views;

namespace ICSharpCode.ILSpy.FileStructure
{
	/// <summary>Navigate &gt; File Structure: shows the outline pane for the active document.</summary>
	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._Navigate), Header = nameof(Resources.FileStructure),
		MenuCategory = "Structure", MenuOrder = 110, InputGestureText = Gesture)]
	[Shared]
	[method: ImportingConstructor]
	public sealed class FileStructureCommand(DockWorkspace dockWorkspace, FileStructureViewModel pane) : SimpleCommand
	{
		public const string Gesture = "Ctrl+Alt+F";

		public override void Execute(object? parameter)
		{
			pane.Attach();
			dockWorkspace.ShowToolPane(FileStructureViewModel.PaneContentId);
		}
	}

	/// <summary>
	/// Navigate &gt; Go to File Member: a searchable popup over the active document listing the
	/// same members as the File Structure pane; Enter jumps to the selected one.
	/// </summary>
	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._Navigate), Header = nameof(Resources.GoToFileMember),
		MenuCategory = "Structure", MenuOrder = 120, InputGestureText = Gesture)]
	[Shared]
	public sealed class GoToFileMemberCommand : SimpleCommand
	{
		/// <summary>Alt+\ -- the backslash key of US and most European layouts reports as OemPipe.</summary>
		public const string Gesture = "Alt+OemPipe";

		/// <summary>The popup currently shown, or null.</summary>
		internal Flyout? OpenFlyout { get; private set; }

		/// <summary>The model of the popup currently shown, or null.</summary>
		internal GoToFileMemberViewModel? OpenModel { get; private set; }

		public override bool CanExecute(object? parameter) => ActiveDocument.Get() != null;

		public override void Execute(object? parameter)
		{
			if (ActiveDocument.Get() is not { } document)
				return;
			if (AppComposition.TryGetExport<MainWindow>() is not { } window)
				return;
			OpenFlyout?.Hide();

			var model = new GoToFileMemberViewModel(document);
			var flyout = new Flyout {
				Content = new GoToFileMemberView { DataContext = model },
				Placement = PlacementMode.Center,
				ShowMode = FlyoutShowMode.Standard,
			};
			model.CloseRequested += (_, _) => flyout.Hide();
			flyout.Closed += (_, _) => {
				if (ReferenceEquals(OpenFlyout, flyout))
				{
					OpenFlyout = null;
					OpenModel = null;
				}
			};
			OpenFlyout = flyout;
			OpenModel = model;
			flyout.ShowAt(window);
		}
	}
}
