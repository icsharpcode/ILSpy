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

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using AwesomeAssertions;

using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.Views;

using Avalonia.Headless.NUnit;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// Selecting the assemblies that were just dropped / reordered must go through the model's batched
/// SelectNodes, not a raw Clear()+Add(). A raw swap flashes a transient EMPTY selection, which the
/// model documents as poisoning the grid-sync deferred guard (the tree stops following tab
/// activation). This pins that the drop-selection callback never emits an empty selection mid-change.
/// </summary>
[TestFixture]
public class DropSelectionBatchingTests
{
	[AvaloniaTest]
	public async Task Drop_Selection_Does_Not_Flash_An_Empty_Selection()
	{
		var (window, vm) = await TestHarness.BootAsync(3);
		await window.WaitForComponent<AssemblyListPane>();
		var model = vm.AssemblyTreeModel;
		var listRoot = (AssemblyListTreeNode)model.Root!;

		listRoot.SelectAssembliesAfterDrop.Should().NotBeNull(
			"the pane wires the after-drop selection callback when it binds");

		var assemblies = model.AssemblyList!.GetAssemblies();
		assemblies.Length.Should().BeGreaterThanOrEqualTo(2);

		// Start from a non-empty selection so a raw Clear() would transit through empty.
		model.SelectedItem = listRoot.FindAssemblyNode(assemblies[0]);

		var observed = new List<SharpTreeNode?>();
		void OnChanged(object? _, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(AssemblyTreeModel.SelectedItem))
				observed.Add(model.SelectedItem);
		}
		model.PropertyChanged += OnChanged;
		try
		{
			// Drive the after-drop selection the way AssemblyListTreeNode.Drop does.
			listRoot.SelectAssembliesAfterDrop!(new[] { assemblies[0], assemblies[1] });
		}
		finally
		{
			model.PropertyChanged -= OnChanged;
		}

		observed.Should().NotContainNulls(
			"the drop selection must not flash an empty SelectedItem mid-change");
		observed.Should().ContainSingle(
			"the batched selection change fans out exactly once, with the final set");
		model.SelectedItems.Should().HaveCount(2, "both dropped assemblies end up selected");
	}
}
