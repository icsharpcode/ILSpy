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
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ILSpy.AppEnv;
using ILSpy.Metadata;
using ILSpy.Metadata.CorTables;
using ILSpy.TreeNodes;
using ILSpy.ViewModels;
using ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Metadata;

/// <summary>
/// TEMPORARY verification harness for the per-column filter wiring. Run with
/// <c>ILSPY_TESTS_VISIBLE=1</c> (or debugger attached) so each step captures a PNG and
/// pops it open in the OS image viewer; otherwise the assertions still execute headlessly
/// but no images are produced. Delete this file once the filter is confirmed working.
/// </summary>
[TestFixture]
public class MetadataFilterRowVerificationTest
{
	[AvaloniaTest]
	public async Task Step_By_Step_Verify_TypeDef_Name_Filter_With_Screenshots()
	{
		// === STEP 1: Boot the main window ===
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		Dispatcher.UIThread.RunJobs();
		window.CaptureAndShow(label: "step1_window_shown");

		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);
		Dispatcher.UIThread.RunJobs();
		window.CaptureAndShow(label: "step2_assemblies_loaded");

		// === STEP 2: Drill into TypeDef table ===
		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(coreLibName);
		assemblyNode.EnsureLazyChildren();
		var metadataNode = assemblyNode.Children.OfType<MetadataTreeNode>().Single();
		metadataNode.EnsureLazyChildren();
		var tablesNode = metadataNode.Children.OfType<MetadataTablesTreeNode>().Single();
		tablesNode.EnsureLazyChildren();
		var typeDefNode = tablesNode.Children.OfType<TypeDefTableTreeNode>().Single();
		vm.AssemblyTreeModel.SelectNode(typeDefNode);

		var page = await vm.DockWorkspace.WaitForMetadataTabAsync();
		Dispatcher.UIThread.RunJobs();
		window.CaptureAndShow(label: "step3_typedef_open");

		// === STEP 3: Wait for the live MetadataTablePage to materialise ===
		// ContentTabPage opts out of Dock's deferred-content presentation so the wrapper
		// view's children (DecompilerTextView + MetadataTablePage) are part of the visual
		// tree synchronously. Without that opt-out the headless test can't reach them.
		var metadataPage = await window.WaitForComponent<MetadataTablePage>();
		TestContext.Out.WriteLine("[VERIFY] MetadataTablePage materialised under the main window.");
		var grid = await metadataPage.WaitForComponent<DataGrid>();
		TestContext.Out.WriteLine("[VERIFY] Inner DataGrid materialised.");

		// Force a layout pass so DataGrid populates its column-headers panel.
		Dispatcher.UIThread.RunJobs();
		await grid.WaitForComponent<DataGridColumnHeader>();

		// === STEP 4: Find the rendered TextBox for the Name column header ===
		var headerBox = grid.GetVisualDescendants().OfType<TextBox>()
			.FirstOrDefault(tb => tb.FindAncestorOfType<DataGridColumnHeader>() is { } owner
				&& owner.Content is StackPanel sp
				&& sp.Children.OfType<TextBlock>().FirstOrDefault()?.Text == "Name");
		headerBox.Should().NotBeNull(
			"the column-builder bakes a TextBox into the Name column's header — it must reach the rendered visual tree");

		// === STEP 5: Confirm the rendered TextBox is the exact instance the model holds ===
		var nameColumn = page.Columns.Single(c => (string?)c.Tag == "Name");
		var modelHeaderBox = ((StackPanel)nameColumn.Header!).Children.OfType<TextBox>().Single();
		ReferenceEquals(headerBox, modelHeaderBox).Should().BeTrue(
			"DataGrid must render the StackPanel Header directly without re-templating; otherwise the TextProperty observable wired by the builder is on a different TextBox than the user types into");

		var nameFilter = page.ColumnFilters.Single(f => f.ColumnName == "Name");
		var totalRows = page.Items.Count;
		totalRows.Should().BeGreaterThan(0, "TypeDef must have rows for filtering to be observable");

		// === STEP 6: Type "System" into the rendered TextBox ===
		headerBox!.Text = "System";
		Dispatcher.UIThread.RunJobs();
		nameFilter.Text.Should().Be("System",
			"setting the rendered TextBox.Text must propagate to ColumnFilter.Text");
		var visibleByPredicate = page.Items.Count(e => MetadataTablePageModel.MatchesFilters(e, page.ColumnFilters));
		visibleByPredicate.Should().BeLessThan(totalRows,
			"the predicate must hide at least one TypeDef row when filtering Name on 'System'");
		TestContext.Out.WriteLine($"[VERIFY] After typing 'System': filter='{nameFilter.Text}' visible={visibleByPredicate}/{totalRows}");
		window.CaptureAndShow(label: "step6_typed_System");

		// === STEP 7: Clear the filter ===
		headerBox.Text = "";
		Dispatcher.UIThread.RunJobs();
		nameFilter.Text.Should().BeEmpty("clearing the TextBox must clear the filter");
		var visibleAfterClear = page.Items.Count(e => MetadataTablePageModel.MatchesFilters(e, page.ColumnFilters));
		visibleAfterClear.Should().Be(totalRows, "an empty filter must show every row again");
		TestContext.Out.WriteLine($"[VERIFY] After clearing: filter='{nameFilter.Text}' visible={visibleAfterClear}/{totalRows}");
		window.CaptureAndShow(label: "step7_cleared");
	}
}
