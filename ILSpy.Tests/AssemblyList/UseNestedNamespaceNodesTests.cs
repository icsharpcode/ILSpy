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
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.TreeNodes;

using ICSharpCode.ILSpyX.TreeView;

using NUnit.Framework;

using SharpTreeView = ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// The "Use nested namespace structure" display setting, from the setting through to the
/// rows on screen. Flat mode keeps every dotted namespace as its own sibling of the assembly
/// node; nested mode splits them, so "System.Linq" becomes "Linq" under "System". The switch
/// happens live: toggling rebuilds each loaded assembly's namespace subtree, and because
/// SharpTreeView's flattener observes node.Children directly, the rebuilt shape reaches the
/// visible rows with no model re-bind.
/// </summary>
[TestFixture]
public class UseNestedNamespaceNodesTests
{
	[AvaloniaTest]
	public async Task Toggling_The_Setting_Reshapes_The_Tree_Live_Down_To_The_Visible_Rows()
	{
		var settings = AppComposition.Current.GetExport<SettingsService>().DisplaySettings;
		settings.UseNestedNamespaceNodes = false;

		var (window, vm) = await TestHarness.BootAsync(3);
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var grid = await pane.WaitForComponent<SharpTreeView>();

		try
		{
			// System.Linq's assembly carries both "System" and "System.Linq", so one assembly
			// shows the difference between the two layouts.
			var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
			assemblyNode.IsExpanded = true;
			await Waiters.WaitForAsync(() => assemblyNode.Children.OfType<NamespaceTreeNode>().Any());

			var visibleRows = (IList)grid.ItemsSource!;
			bool VisibleNamespace(string name) => visibleRows.Cast<SharpTreeNode>()
				.OfType<NamespaceTreeNode>()
				.Any(n => string.Equals(n.Text?.ToString(), name, StringComparison.Ordinal));

			NamespaceNames().Should().Contain("System.Linq",
				"flat mode lists the whole dotted namespace as one sibling of the assembly node");
			await Waiters.WaitForAsync(() => VisibleNamespace("System.Collections.Generic"),
				description: "flat mode shows the dotted namespace as a single visible row");
			TestCapture.Step("flat-mode");

			// The setting fans out through MessageBus<SettingsChangedEventArgs> to
			// AssemblyTreeModel.OnSettingsChanged, which rebuilds the namespace subtrees.
			settings.UseNestedNamespaceNodes = true;
			assemblyNode.IsExpanded = true;

			await Waiters.WaitForAsync(() => NamespaceNames().Contains("System"),
				TimeSpan.FromSeconds(5),
				"toggling the setting must rebuild the namespace subtree into nested nodes");
			TestCapture.Step("nested-mode");

			NamespaceNames().Should().NotContain("System.Linq",
				"the flat dotted sibling must be gone once its segments are nested");
			assemblyNode.Children.OfType<NamespaceTreeNode>().Single(n => n.Name == "System")
				.Children.OfType<NamespaceTreeNode>().Select(n => n.Name).Should().Contain("Linq",
				"the trailing segment must hang under the node for the leading one");
			VisibleNamespace("System").Should().BeTrue(
				"the rebuilt node must reach the visible rows without a model re-bind - the live "
				+ "flattener observes the child mutations directly");

			string[] NamespaceNames() => assemblyNode.Children.OfType<NamespaceTreeNode>()
				.Select(n => n.Name).ToArray();
		}
		finally
		{
			settings.UseNestedNamespaceNodes = false;
		}
	}
}
