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
public class UseNestedNamespaceNodesTests
{
	[AvaloniaTest]
	public async Task When_UseNestedNamespaceNodes_True_Namespaces_Are_Hierarchical()
	{
		// With the setting on, "System" becomes a single root node holding "Collections",
		// "IO", "Linq", … as descendants — not the flat "System", "System.Collections",
		// "System.IO" siblings the default flat mode produces.

		var settings = AppComposition.Current.GetExport<SettingsService>().DisplaySettings;
		var (_, vm) = await TestHarness.BootAsync(3);

		try
		{
			settings.UseNestedNamespaceNodes = true;

			// Use System.Linq's assembly — it has System and System.Linq as namespaces.
			var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
			assemblyNode.Children.Clear();
			assemblyNode.LazyLoading = true;
			assemblyNode.EnsureLazyChildren();

			var systemNode = assemblyNode.Children.OfType<NamespaceTreeNode>()
				.SingleOrDefault(ns => ns.Name == "System");
			((object?)systemNode).Should().NotBeNull(
				"in nested mode the top-level node for the System namespace must exist as 'System' (last segment), not 'System.Linq'");

			var nestedLinq = systemNode!.Children.OfType<NamespaceTreeNode>()
				.SingleOrDefault(ns => ns.Name == "Linq");
			((object?)nestedLinq).Should().NotBeNull(
				"the System.Linq namespace must nest under the System node in nested mode");

			// Sanity: there is NO sibling "System.Linq" at the assembly-node level.
			assemblyNode.Children.OfType<NamespaceTreeNode>()
				.Select(n => n.Name).Should().NotContain("System.Linq",
				"flat-style 'System.Linq' sibling must not appear when nesting is on");
		}
		finally
		{
			settings.UseNestedNamespaceNodes = false;
		}
	}

	[AvaloniaTest]
	public async Task When_UseNestedNamespaceNodes_False_Namespaces_Are_Flat()
	{
		// Baseline: the default flat layout keeps every distinct namespace string as a
		// sibling under the AssemblyTreeNode.
		var settings = AppComposition.Current.GetExport<SettingsService>().DisplaySettings;
		settings.UseNestedNamespaceNodes = false;

		var (_, vm) = await TestHarness.BootAsync(3);

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		assemblyNode.Children.Clear();
		assemblyNode.LazyLoading = true;
		assemblyNode.EnsureLazyChildren();

		var namespaceNames = assemblyNode.Children.OfType<NamespaceTreeNode>()
			.Select(n => n.Name).ToList();

		namespaceNames.Should().Contain("System.Linq",
			"in flat mode 'System.Linq' must appear as a top-level sibling");
	}
}
