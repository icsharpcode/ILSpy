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

using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ILSpy;
using ILSpy.AppEnv;
using ILSpy.AssemblyTree;
using ILSpy.TreeNodes;
using ILSpy.ViewModels;
using ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class UseNestedNamespaceNodesGridVerification
{
	[AvaloniaTest]
	public async Task Toggling_UseNestedNamespaceNodes_Triggers_BindTree_So_The_Grid_Sees_The_New_Shape()
	{
		// The pane caches a snapshot of each expanded node's children via the
		// HierarchicalOptions.ChildrenSelector — mid-expand mutations to AssemblyTreeNode.
		// Children aren't observed by the live grid. The fix raises AssemblyTreeModel.Root
		// PropertyChanged so AssemblyListPane.BindTree fires and replaces the
		// HierarchicalModel; this test verifies that re-bind happens.

		var settings = AppComposition.Current.GetExport<SettingsService>().DisplaySettings;
		settings.UseNestedNamespaceNodes = false;

		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var grid = pane.FindControl<DataGrid>("TreeGrid")!;

		// Expand an assembly to force the ChildrenSelector to fire and cache its snapshot.
		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		assemblyNode.IsExpanded = true;
		await Waiters.WaitForAsync(() => grid.HierarchicalModel != null);

		try
		{
			var modelBefore = grid.HierarchicalModel;
			((object?)modelBefore).Should().NotBeNull("baseline: the grid must have a HierarchicalModel before the toggle");

			settings.UseNestedNamespaceNodes = true;

			await Waiters.WaitForAsync(
				() => !ReferenceEquals(grid.HierarchicalModel, modelBefore),
				System.TimeSpan.FromSeconds(5));

			grid.HierarchicalModel.Should().NotBeSameAs(modelBefore,
				"toggling UseNestedNamespaceNodes must force AssemblyListPane.BindTree, "
				+ "replacing the HierarchicalModel with one that reads the rebuilt children");
		}
		finally
		{
			settings.UseNestedNamespaceNodes = false;
		}
	}
}
