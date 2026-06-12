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

using System.Linq;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class UseNestedNamespaceNodesLiveTests
{
	[AvaloniaTest]
	public async Task Toggling_UseNestedNamespaceNodes_Live_Refreshes_The_Tree_Shape()
	{
		// Drives the live-reactivity path: toggle the Display-Settings flag and observe
		// that the AssemblyTreeNode's namespace children switch between flat and nested
		// layouts without a manual rebuild.
		var settings = AppComposition.Current.GetExport<SettingsService>().DisplaySettings;
		settings.UseNestedNamespaceNodes = false;

		var (_, vm) = await TestHarness.BootAsync(3);

		try
		{
			var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
			assemblyNode.EnsureLazyChildren();

			var flatNamespaces = assemblyNode.Children.OfType<NamespaceTreeNode>()
				.Select(n => n.Name).ToList();
			flatNamespaces.Should().Contain("System.Linq",
				"baseline: flat mode lists 'System.Linq' as a top-level sibling");

			// Toggle the live setting — should fan out through MessageBus<SettingsChangedEventArgs>
			// to AssemblyTreeModel.OnSettingsChanged and rebuild the namespace subtrees.
			settings.UseNestedNamespaceNodes = true;

			await Waiters.WaitForAsync(() =>
				assemblyNode.Children.OfType<NamespaceTreeNode>().Any(n => n.Name == "System"),
				System.TimeSpan.FromSeconds(5));

			var nestedNames = assemblyNode.Children.OfType<NamespaceTreeNode>()
				.Select(n => n.Name).ToList();
			nestedNames.Should().Contain("System",
				"after live toggle: nested mode must surface 'System' as top-level");
			nestedNames.Should().NotContain("System.Linq",
				"after live toggle: the flat 'System.Linq' sibling must disappear");
		}
		finally
		{
			settings.UseNestedNamespaceNodes = false;
		}
	}
}
