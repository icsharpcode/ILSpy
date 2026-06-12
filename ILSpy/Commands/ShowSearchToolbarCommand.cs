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
using System.Composition;

using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy.Docking;

namespace ICSharpCode.ILSpy.Commands
{
	/// <summary>
	/// Toolbar-surface adapter for the existing <see cref="DockWorkspace.ShowSearchCommand"/>.
	/// Wraps it so the [ExportToolbarCommand] discovery picks up a single ICommand entry; both
	/// the magnifier toolbar button and the Ctrl+Shift+F / Ctrl+E key bindings end up routed
	/// through the same workspace command.
	/// </summary>
	[ExportToolbarCommand(
		ToolTip = nameof(Resources.SearchCtrlShiftFOrCtrlE),
		ToolbarIcon = "Images/Search",
		ToolbarCategory = nameof(Resources.View),
		ToolbarOrder = 100)]
	[Shared]
	sealed class ShowSearchToolbarCommand : SimpleCommand
	{
		readonly DockWorkspace dockWorkspace;

		[ImportingConstructor]
		public ShowSearchToolbarCommand(DockWorkspace dockWorkspace)
		{
			this.dockWorkspace = dockWorkspace;
			dockWorkspace.ShowSearchCommand.CanExecuteChanged += (_, _) => RaiseCanExecuteChanged();
		}

		public override bool CanExecute(object? parameter)
			=> dockWorkspace.ShowSearchCommand.CanExecute(parameter);

		public override void Execute(object? parameter)
			=> dockWorkspace.ShowSearchCommand.Execute(parameter);
	}
}
