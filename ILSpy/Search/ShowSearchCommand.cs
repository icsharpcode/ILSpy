// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.Windows.Input;

using ICSharpCode.ILSpy.Docking;

namespace ICSharpCode.ILSpy.Search
{
	[ExportToolbarCommand(ToolTip = nameof(Properties.Resources.SearchCtrlShiftFOrCtrlE), ToolbarIcon = "Images/Search", ToolbarCategory = nameof(Properties.Resources.View), ToolbarOrder = 100)]
	[Shared]
	sealed class ShowSearchCommand : CommandWrapper
	{
		private readonly DockWorkspace dockWorkspace;

		public ShowSearchCommand(DockWorkspace dockWorkspace)
			: base(NavigationCommands.Search)
		{
			this.dockWorkspace = dockWorkspace;
			var gestures = NavigationCommands.Search.InputGestures;

			gestures.Clear();
			gestures.Add(new KeyGesture(Key.F, ModifierKeys.Control | ModifierKeys.Shift));
			gestures.Add(new KeyGesture(Key.E, ModifierKeys.Control));
		}

		protected override void OnExecute(object sender, ExecutedRoutedEventArgs e)
		{
			Console.WriteLine($"ShowSearchCommand: Executing ShowToolPane for SearchPaneModel.PaneContentId using dockWorkspace type: {dockWorkspace?.GetType().FullName}");
			var result = dockWorkspace.ShowToolPane(SearchPaneModel.PaneContentId);
			Console.WriteLine($"ShowSearchCommand: ShowToolPane returned: {result}");
		}
	}
}