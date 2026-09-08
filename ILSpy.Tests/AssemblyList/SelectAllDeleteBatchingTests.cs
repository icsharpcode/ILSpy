// Copyright (c) 2026 Siegfried Pammer
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

using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.Input;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Controls.TreeView;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// Ctrl+A followed by Delete must cost one batch, not one fan-out per row. Every selection
/// change reaches a full application-wide command re-query, a history prune, and a decompile
/// kick-off; repeating that per node (and again per removed assembly) is what made clearing an
/// expanded list freeze the UI. These pin the batching so the cost stays proportional to the
/// number of user actions rather than to the number of selected nodes.
/// </summary>
[TestFixture]
public class SelectAllDeleteBatchingTests
{
	static void RaiseKey(Control target, Key key, KeyModifiers modifiers)
	{
		target.RaiseEvent(new KeyEventArgs {
			Key = key,
			KeyModifiers = modifiers,
			RoutedEvent = InputElement.KeyDownEvent,
			Source = target,
		});
	}

	// Deliberately not WaitForIdleAsync: a select-all starts a decompile of everything selected,
	// and its progress spinner keeps posting to the dispatcher until that finishes, so the app is
	// never idle. Everything asserted here -- the selection, the fan-out count, the list change --
	// lands synchronously while the key is handled, so pumping once is the right synchronization
	// point. The tree is left collapsed for the same reason: expanding an assembly first would put
	// every type in it into the selection, and decompiling those dominates the run without making
	// the assertions any sharper. One fan-out versus one per row already separates the two
	// behaviours at three rows.
	static void SettleInput() => Waiters.PumpUI();

	[AvaloniaTest]
	public async Task Select_All_Fans_Out_Once_Not_Once_Per_Node()
	{
		var (window, vm) = await TestHarness.BootAsync(3);
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var tree = pane.FindControl<SharpTreeView>("Tree")!;
		var model = vm.AssemblyTreeModel;

		int rows = ((System.Collections.IList)tree.ItemsSource!).Count;
		rows.Should().BeGreaterThan(2, "the fan-out only shows up on a multi-row selection");

		int fanOuts = 0;
		void OnChanged(object? _, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(AssemblyTreeModel.SelectedItem))
				fanOuts++;
		}
		model.PropertyChanged += OnChanged;
		try
		{
			RaiseKey(tree, Key.A, KeyModifiers.Control);
			SettleInput();
		}
		finally
		{
			model.PropertyChanged -= OnChanged;
		}

		model.SelectedItems.Should().HaveCount(rows, "Ctrl+A selects every visible row");
		fanOuts.Should().Be(1,
			"selecting N rows fans out once, not once per row -- each fan-out runs a full command "
			+ "re-query, a session-settings write and a decompile of the selection");
	}

	[AvaloniaTest]
	public async Task Deleting_Every_Assembly_Raises_One_List_Change_Not_One_Per_Assembly()
	{
		var (window, vm) = await TestHarness.BootAsync(3);
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var tree = pane.FindControl<SharpTreeView>("Tree")!;
		var model = vm.AssemblyTreeModel;
		var list = model.AssemblyList!;

		int assemblies = list.Count;
		assemblies.Should().BeGreaterThan(2);

		int listChanges = 0;
		void OnListChanged(object? _, NotifyCollectionChangedEventArgs e) => listChanges++;
		list.CollectionChanged += OnListChanged;
		try
		{
			RaiseKey(tree, Key.A, KeyModifiers.Control);
			SettleInput();
			RaiseKey(tree, Key.Delete, KeyModifiers.None);
			SettleInput();
		}
		finally
		{
			list.CollectionChanged -= OnListChanged;
		}

		list.Count.Should().Be(0, "Delete on a select-all removes every assembly");
		listChanges.Should().Be(1,
			"removing N assemblies must raise one batched collection change, not one per assembly -- "
			+ "every consumer (history prune, search restart, command re-query, tab prune) runs per event");
	}
}
