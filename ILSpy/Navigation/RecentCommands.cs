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


using System.Collections.Generic;
using System.Composition;
using System.Linq;

using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Properties;

namespace ICSharpCode.ILSpy.Navigation
{
	/// <summary>
	/// Navigate &gt; Recent Files (Ctrl+Comma, the ReSharper Visual Studio scheme; dotPeek's Ctrl+E
	/// is ILSpy's search). Opens a searchable list of recently viewed tree nodes, most recent first.
	/// </summary>
	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._Navigate), Header = nameof(Resources.RecentFiles), MenuCategory = "Recent", MenuOrder = 50, InputGestureText = "Ctrl+OemComma")]
	[Shared]
	[method: ImportingConstructor]
	sealed class RecentFilesCommand(DockWorkspace dockWorkspace) : SimpleCommand
	{
		public override void Execute(object? parameter)
		{
			if (ActiveTextViewLocator.MainWindow is not { } owner)
				return;
			var items = dockWorkspace.RecentNavigation.Files
				.Select(f => new QuickPickItem(f.DisplayText, RecentNavigation.GetAssemblyName(f.Node), f.Icon, f))
				.ToList();
			var model = new QuickPickModel(items, item => dockWorkspace.NavigateToRecentFile((RecentFile)item.Payload));
			QuickPickWindow.Show(owner, Resources.RecentFiles.TrimEnd('.'), model);
		}
	}

	/// <summary>
	/// Navigate &gt; Recent Locations (Ctrl+Shift+Comma). Lists the caret positions recently left
	/// behind -- the current one first -- with a preview of the code at each; choosing one re-opens
	/// the node with the caret and scroll position restored.
	/// </summary>
	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._Navigate), Header = nameof(Resources.RecentLocations), MenuCategory = "Recent", MenuOrder = 51, InputGestureText = "Ctrl+Shift+OemComma")]
	[Shared]
	[method: ImportingConstructor]
	sealed class RecentLocationsCommand(DockWorkspace dockWorkspace) : SimpleCommand
	{
		public override void Execute(object? parameter)
		{
			if (ActiveTextViewLocator.MainWindow is not { } owner)
				return;
			var locations = new List<RecentLocation>();
			if (dockWorkspace.CaptureCurrentLocation() is { } current)
				locations.Add(current);
			locations.AddRange(dockWorkspace.RecentNavigation.Locations.Where(l => !locations.Any(c => c.IsSamePlace(l))));
			var items = locations
				.Select(l => new QuickPickItem($"{l.DisplayText} ({l.Line}:{l.Column})", l.Preview, l.Icon, l))
				.ToList();
			var model = new QuickPickModel(items, item => dockWorkspace.NavigateToRecentLocation((RecentLocation)item.Payload));
			QuickPickWindow.Show(owner, Resources.RecentLocations.TrimEnd('.'), model);
		}
	}
}
