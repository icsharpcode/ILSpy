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

using System.Collections;
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class UseNestedNamespaceNodesGridVerification
{
	[AvaloniaTest]
	public async Task Toggling_UseNestedNamespaceNodes_Reshapes_The_Visible_Tree_In_Place()
	{
		// Toggling UseNestedNamespaceNodes rebuilds each loaded assembly's namespace subtree.
		// SharpTreeView's flattener observes node.Children mutations live, so the rebuilt shape
		// surfaces in the visible row list without any model re-bind (the ProDataGrid snapshot
		// problem that used to need a HierarchicalModel rebind is gone). This pins that the
		// flat-mode -> nested-mode switch is reflected in the SharpTreeView's ItemsSource.

		var settings = AppComposition.Current.GetExport<SettingsService>().DisplaySettings;
		settings.UseNestedNamespaceNodes = false;

		var (window, vm) = await TestHarness.BootAsync(3);
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var grid = await pane.WaitForComponent<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView>();

		// Expand an assembly so its namespace children become visible rows.
		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		assemblyNode.IsExpanded = true;
		await Waiters.WaitForAsync(() => assemblyNode.Children.OfType<NamespaceTreeNode>().Any());
		TestCapture.Step("system-linq-expanded-flat");

		var flat = (IList)grid.ItemsSource!;
		bool VisibleNamespace(string name) => flat.Cast<SharpTreeNode>()
			.OfType<NamespaceTreeNode>()
			.Any(n => string.Equals(n.Text?.ToString(), name, System.StringComparison.Ordinal));

		// In flat mode the full dotted namespace is a single visible row.
		await Waiters.WaitForAsync(() => VisibleNamespace("System.Collections.Generic"),
			description: "flat mode shows the dotted namespace as one row");

		try
		{
			settings.UseNestedNamespaceNodes = true;
			assemblyNode.IsExpanded = true;
			TestCapture.Step("nested-namespaces-rebuilt");

			// In nested mode the leaf segment ("Generic") becomes its own row nested under
			// "System" -> "Collections"; the live flattener must surface that without a re-bind.
			await Waiters.WaitForAsync(
				() => assemblyNode.Children.OfType<NamespaceTreeNode>()
						.Any(n => string.Equals(n.Text?.ToString(), "System", System.StringComparison.Ordinal)),
				System.TimeSpan.FromSeconds(5),
				"toggling UseNestedNamespaceNodes must rebuild the namespace subtree into nested nodes");

			VisibleNamespace("System").Should().BeTrue(
				"the rebuilt nested 'System' namespace node must appear in the SharpTreeView's visible rows "
				+ "without any model re-bind (the live flattener observes the child mutations directly)");
		}
		finally
		{
			settings.UseNestedNamespaceNodes = false;
		}
	}
}
