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

using Avalonia.Controls;
using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpyX.Search;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Search;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Search;

/// <summary>
/// Right-clicking a search result opens the same registry-driven context menu the trees use.
/// (Regression: the Avalonia port dropped the menu the WPF host attached via
/// <c>ContextMenuProvider.Add(listBox)</c>.) The selected result's entity reaches the entries
/// through <c>TextViewContext.Reference</c>, so entity entries such as Analyze and the
/// scope-search entries light up.
/// </summary>
[TestFixture]
public class SearchResultContextMenuTests
{
	[AvaloniaTest]
	public async Task Right_Click_Menu_Surfaces_Entity_Entries_For_The_Selected_Result()
	{
		var (window, vm) = await TestHarness.BootAsync();
		AppComposition.Current.GetExport<DockWorkspace>().ShowToolPane(SearchPaneModel.PaneContentId);
		var pane = await window.WaitForComponent<SearchPane>();
		var grid = pane.FindControl<DataGrid>("SearchResults")!;
		var model = (SearchPaneModel)pane.DataContext!;

		// A real member result: its Reference is the entity the menu entries read.
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		var entity = (IEntity)typeNode.Member!;
		var result = new MemberSearchResult {
			Member = entity,
			Name = entity.Name,
			Location = entity.Namespace,
			Assembly = entity.ParentModule?.AssemblyName ?? "",
			Image = "", LocationImage = "", AssemblyImage = "",
		};
		model.Results.Add(result);
		grid.SelectedItem = result;

		grid.ContextMenu.Should().NotBeNull("a context menu must be attached to the results grid");

		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();
		var menu = pane.BuildContextMenuForCurrentState(registry.Entries);

		menu.Should().NotBeNull("the selected entity result must produce a populated menu");
		var headers = menu!.Items.OfType<MenuItem>().Select(i => i.Header?.ToString()).ToList();
		headers.Should().Contain(Resources.Analyze, "Analyze must be offered for an entity result");
		headers.Should().Contain(Resources.ScopeSearchToThisNamespace,
			"a namespaced entity result offers scope-search-to-namespace");
	}

	[AvaloniaTest]
	public async Task Menu_Is_Cancelled_When_No_Result_Is_Selected()
	{
		var (window, _) = await TestHarness.BootAsync();
		AppComposition.Current.GetExport<DockWorkspace>().ShowToolPane(SearchPaneModel.PaneContentId);
		var pane = await window.WaitForComponent<SearchPane>();

		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();
		// Nothing selected -> no entity context -> the entity entries hide, so the menu is empty.
		var menu = pane.BuildContextMenuForCurrentState(registry.Entries);
		(menu == null || !menu.Items.OfType<MenuItem>().Any())
			.Should().BeTrue("with no result selected there are no entity entries to show");
	}
}
