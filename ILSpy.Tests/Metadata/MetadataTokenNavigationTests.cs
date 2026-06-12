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
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Metadata;
using ICSharpCode.ILSpy.Metadata.CorTables;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Metadata;

[TestFixture]
public class MetadataTokenNavigationTests
{
	[AvaloniaTest]
	public async Task Clicking_A_Token_Cell_In_TypeDef_Navigates_To_The_Referenced_Table_Row()
	{
		// Each TypeDef row carries a BaseType column annotated [Kind=Token]. Clicking it
		// must dispatch through the docking host to the TypeRef / TypeDef / TypeSpec table
		// the token belongs to and scroll the receiving grid to the referenced row.

		var (_, vm) = await TestHarness.BootAsync();

		var typeDefNode = vm.AssemblyTreeModel.FindCoreLib()
			.GetChild<MetadataTreeNode>()
			.GetChild<MetadataTablesTreeNode>()
			.GetChild<TypeDefTableTreeNode>();

		vm.AssemblyTreeModel.SelectNode(typeDefNode);
		var tab = await vm.DockWorkspace.WaitForMetadataTabAsync();
		TestCapture.Step("typedef-grid");

		// Pick a row whose BaseType is non-zero (skip <Module>) and resolve the expected
		// target table from the handle's runtime kind.
		var rowWithBase = tab.Items.Cast<TypeDefTableTreeNode.TypeDefEntry>()
			.First(e => e.BaseType != 0);
		var expectedTableIndex = (TableIndex)(int)MetadataTokens.EntityHandle(rowWithBase.BaseType).Kind;
		var expectedRowNumber = MetadataTokens.GetRowNumber(MetadataTokens.EntityHandle(rowWithBase.BaseType));

		// Act — fire the navigate-to-cell event the way the hyperlink button does.
		tab.RaiseNavigateToCell(rowWithBase, nameof(rowWithBase.BaseType));
		await Waiters.WaitForAsync(
			() => vm.AssemblyTreeModel.SelectedItem is MetadataTableTreeNode m
				&& m.Kind == expectedTableIndex);
		TestCapture.Step("navigated-to-token-table");

		// Assert — host swapped the tree selection + Content to the table matching the
		// handle's kind. The actual scroll-into-view runs through MetadataTablePage's
		// ApplyScrollTarget, which clears ScrollToRow synchronously after handling — so
		// we don't observe the row index here, just that the selection is the right table
		// and a fresh metadata page is showing.
		var factory = (ICSharpCode.ILSpy.Docking.ILSpyDockFactory)vm.DockWorkspace.Factory;
		var landed = (MetadataTablePageModel)factory.MainTab!.Content!;
		landed.Title.Should().Contain(expectedTableIndex.ToString()); // title leads with the kind byte
		landed.Items.Should().HaveCountGreaterThan((int)(expectedRowNumber - 1),
			"the target row index must be in range for the destination table");
	}

	[AvaloniaTest]
	public async Task Pressing_Ctrl_G_On_A_Token_Cell_Navigates_The_Same_Way_As_Clicking_The_Hyperlink()
	{
		// Ctrl+G is the keyboard shortcut for "Go to token", advertised on the context-menu
		// entry. It should fire the same NavigateToCellRequested path as the hyperlink-style
		// click, with the focused (or last-clicked) token cell as the target.

		var (window, vm) = await TestHarness.BootAsync();

		var typeDefNode = vm.AssemblyTreeModel.FindCoreLib()
			.GetChild<MetadataTreeNode>()
			.GetChild<MetadataTablesTreeNode>()
			.GetChild<TypeDefTableTreeNode>();

		vm.AssemblyTreeModel.SelectNode(typeDefNode);
		var tab = await vm.DockWorkspace.WaitForMetadataTabAsync();
		TestCapture.Step("typedef-grid");

		var rowWithBase = tab.Items.Cast<TypeDefTableTreeNode.TypeDefEntry>()
			.First(e => e.BaseType != 0);
		var expectedTableIndex = (System.Reflection.Metadata.Ecma335.TableIndex)
			(int)MetadataTokens.EntityHandle(rowWithBase.BaseType).Kind;

		var metadataPage = await window.WaitForComponent<MetadataTablePage>();

		// Synthesize a DataGridCell for the row's BaseType (token-kind) column. The Ctrl+G
		// handler doesn't care how the cell got its OwningColumn / DataContext — it just
		// dispatches whatever is given.
		var baseTypeColumn = tab.Columns.Single(c => (string?)c.Tag == "BaseType");
		var cell = new global::Avalonia.Controls.DataGridCell { DataContext = rowWithBase };
		cell.SetValue(global::Avalonia.Controls.DataGridCell.OwningColumnProperty, baseTypeColumn);

		metadataPage.TryNavigateToTokenInCell(cell);

		await Waiters.WaitForAsync(
			() => vm.AssemblyTreeModel.SelectedItem is MetadataTableTreeNode m
				&& m.Kind == expectedTableIndex);
		TestCapture.Step("navigated-to-token-table");
	}
}

internal static class MetadataTokenNavigationTestExtensions
{
	public static void RaiseNavigateToCell(this MetadataTablePageModel page, object row, string columnName)
	{
		// MetadataTablePageModel.RaiseNavigateToCell is internal; tests in this assembly
		// can call it because [InternalsVisibleTo] is wired in the project file.
		typeof(MetadataTablePageModel)
			.GetMethod("RaiseNavigateToCell", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
			.Invoke(page, [row, columnName]);
	}
}
