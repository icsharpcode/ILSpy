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

using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Search;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Search;

[TestFixture]
public class ScopeSearchToTests
{
	[AvaloniaTest]
	public async Task Scope_Search_To_Assembly_Prepends_InAssembly_To_The_Search_Term()
	{
		var (_, vm) = await TestHarness.BootAsync();

		var entry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>()
			.GetEntry(nameof(Resources.ScopeSearchToThisAssembly));

		var asm = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		var search = AppComposition.Current.GetExport<SearchPaneModel>();
		search.SearchTerm = string.Empty;

		entry.IsVisible(new TextViewContext { SelectedTreeNodes = new SharpTreeNode[] { asm } })
			.Should().BeTrue("the entry must surface on AssemblyTreeNode selections");
		entry.Execute(new TextViewContext { SelectedTreeNodes = new SharpTreeNode[] { asm } });
		TestCapture.Step("scoped-to-assembly");

		search.SearchTerm.Should().Contain("inassembly:",
			"Execute must inject the inassembly: prefix into the live search term");
		search.SearchTerm.Should().Contain("System.Linq");

		search.SearchTerm = string.Empty;
	}

	[AvaloniaTest]
	public async Task Scope_Search_To_Namespace_Prepends_InNamespace_To_The_Search_Term()
	{
		var (_, vm) = await TestHarness.BootAsync();

		var entry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>()
			.GetEntry(nameof(Resources.ScopeSearchToThisNamespace));

		var ns = vm.AssemblyTreeModel.FindNode<NamespaceTreeNode>("System.Linq", "System.Linq");
		var search = AppComposition.Current.GetExport<SearchPaneModel>();
		search.SearchTerm = string.Empty;

		entry.IsVisible(new TextViewContext { SelectedTreeNodes = new SharpTreeNode[] { ns } })
			.Should().BeTrue("the entry must surface on NamespaceTreeNode selections");
		entry.Execute(new TextViewContext { SelectedTreeNodes = new SharpTreeNode[] { ns } });
		TestCapture.Step("scoped-to-namespace");

		search.SearchTerm.Should().Contain("innamespace:");
		search.SearchTerm.Should().Contain("System.Linq");

		search.SearchTerm = string.Empty;
	}
}
