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


using System.Linq;

using ICSharpCode.ILSpy.Navigation;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Docking
{
	/// <summary>
	/// Recent Files / Recent Locations bookkeeping and navigation. The lists are fed from the
	/// navigation-history recorder, so they see exactly the navigations Back/Forward see.
	/// </summary>
	public partial class DockWorkspace
	{
		/// <summary>Most-recently-used tree nodes and caret locations of this workspace.</summary>
		public RecentNavigation RecentNavigation { get; } = new();

		// Called while a history entry is recorded: the outgoing entry has just been stamped with the
		// editor's view state, and its tab still holds the outgoing document text (the decompile of
		// the incoming node lands asynchronously), so the location and its preview line match.
		void RecordRecentNavigation(NavigationEntry? outgoing, NavigationEntry incoming)
		{
			if (outgoing is TreeNodeEntry { CaretOffset: { } caret } left
				&& UnwrapDecompilerTab(left.Tab) is { IsStaticContent: false } tab)
			{
				var state = new DecompilerTextViewState(caret, left.VerticalOffset ?? 0, left.HorizontalOffset ?? 0, left.Foldings);
				RecentNavigation.RecordLocation(RecentLocation.Create(left.Node, left.Tab, state, tab.Text));
			}
			if (incoming is TreeNodeEntry entered)
				RecentNavigation.RecordFile(entered.Node);
		}

		/// <summary>
		/// Snapshot of where the caret is right now in the document of the current history entry, or
		/// null when that document is not a decompiler view showing the entry's node.
		/// </summary>
		public RecentLocation? CaptureCurrentLocation()
		{
			if (history.Current is not TreeNodeEntry current
				|| UnwrapDecompilerTab(current.Tab) is not { IsStaticContent: false } tab
				|| !ReferenceEquals(tab.CurrentNode, current.Node)
				|| tab.CaptureViewState?.Invoke() is not { } state)
			{
				return null;
			}
			return RecentLocation.Create(current.Node, current.Tab, state, tab.Text);
		}

		/// <summary>Navigates to a recent file by selecting its node in the assembly tree.</summary>
		public void NavigateToRecentFile(RecentFile file)
		{
			System.ArgumentNullException.ThrowIfNull(file);
			assemblyTreeModel.SelectedItem = file.Node;
		}

		/// <summary>
		/// Navigates to a recent location: re-opens its node (in the frozen tab that recorded it when
		/// that tab is still open and showing it, otherwise through the tree selection) and restores
		/// the recorded caret and scroll position.
		/// </summary>
		public void NavigateToRecentLocation(RecentLocation location)
		{
			System.ArgumentNullException.ThrowIfNull(location);
			if (location.Tab is ContentTabPage recordedTab
				&& !ReferenceEquals(recordedTab, factory.MainTab)
				&& factory.Documents is { VisibleDockables: { } docs } documents
				&& docs.Contains(recordedTab)
				&& UnwrapDecompilerTab(recordedTab) is { } frozen
				&& ReferenceEquals(frozen.CurrentNode, location.Node))
			{
				if (!ReferenceEquals(documents.ActiveDockable, recordedTab))
					factory.SetActiveDockable(recordedTab);
				RestoreViewState(frozen, location.State);
				return;
			}

			if (ActiveDecompilerTab is { IsDecompiling: false } shown
				&& ReferenceEquals(shown.CurrentNode, location.Node)
				&& ReferenceEquals(assemblyTreeModel.SelectedItem, location.Node))
			{
				// Already on screen: no decompile will run to consume a pending state.
				RestoreViewState(shown, location.State);
				return;
			}

			assemblyTreeModel.SelectedItem = location.Node;
			// The selection started an asynchronous decompile; the editor applies the pending state
			// once the new text lands.
			if (ActiveDecompilerTab is { } target && ReferenceEquals(target.CurrentNode, location.Node))
				target.PendingViewState = location.State;
		}

		static void RestoreViewState(DecompilerTabPageModel tab, DecompilerTextViewState state)
		{
			if (tab.ApplyViewState is { } apply)
				apply(state);
			else
				tab.PendingViewState = state;
		}
	}
}
