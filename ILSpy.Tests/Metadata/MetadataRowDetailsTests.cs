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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Metadata;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Metadata;

[TestFixture]
public class MetadataRowDetailsTests
{
	[AvaloniaTest]
	public void Shell_Rebuilds_Its_Content_Whenever_The_DataContext_Changes()
	{
		// The DataGrid builds a row-details template once per details element and then reuses
		// the built control across row recycling, swapping only the DataContext. Per-item
		// content (text blob vs. sub-grid) therefore has to be produced by a control that
		// re-runs its factory on every DataContext change — that is the shell's contract.
		var factoryInputs = new List<object?>();
		var shell = new MetadataRowDetailsControl(item => {
			factoryInputs.Add(item);
			return item switch {
				string s => new TextBox { Text = s },
				IList<BitEntry> bits => new DataGrid { ItemsSource = bits },
				_ => null,
			};
		});

		shell.DataContext = "blob text";
		shell.Content.Should().BeOfType<TextBox>()
			.Which.Text.Should().Be("blob text");

		var bits = new List<BitEntry> { new(true, "<0001> bit") };
		shell.DataContext = bits;
		shell.Content.Should().BeOfType<DataGrid>()
			.Which.ItemsSource.Should().BeSameAs(bits);

		shell.DataContext = null;
		shell.Content.Should().BeNull("a recycled details element gets its DataContext nulled and must drop stale content");

		factoryInputs.Should().Equal("blob text", bits, null);
	}

	[AvaloniaTest]
	public void CreateTemplate_Builds_A_DataContext_Tracking_Shell()
	{
		var template = MetadataRowDetails.CreateTemplate(
			item => item is string s ? new TextBox { Text = s } : null);

		var control = template.Build("ignored-build-parameter");

		var shell = control.Should().BeOfType<MetadataRowDetailsControl>().Subject;
		shell.DataContext = "hello";
		shell.Content.Should().BeOfType<TextBox>().Which.Text.Should().Be("hello");
	}

	[AvaloniaTest]
	public async Task Coff_Characteristics_Row_Renders_Its_Flag_Bits_In_Expanded_Row_Details()
	{
		// The COFF header view keeps the Characteristics flags word permanently expanded:
		// the row's details area lists all 16 bits with their meanings, while every other
		// row of the header stays detail-less.
		var (window, vm) = await TestHarness.BootAsync();

		var coffNode = vm.AssemblyTreeModel.FindCoreLib()
			.GetChild<MetadataTreeNode>()
			.GetChild<CoffHeaderTreeNode>();

		vm.AssemblyTreeModel.SelectNode(coffNode);
		var tab = await vm.DockWorkspace.WaitForMetadataTabAsync();
		TestCapture.Step("coff-header-grid");

		tab.RowDetailsTemplate.Should().NotBeNull("the COFF header page carries a flags details template");
		tab.IsRowDetailsVisible.Should().NotBeNull();

		var metadataPage = await window.WaitForComponent<MetadataTablePage>();
		var grid = metadataPage.FindControl<DataGrid>("Grid")!;
		await grid.WaitForComponent<DataGridRow>();

		await Waiters.WaitForAsync(() => grid.GetVisualDescendants().OfType<DataGridRow>()
			.Any(r => r.DataContext is Entry { Member: "Characteristics" } && r.AreDetailsVisible));
		TestCapture.Step("characteristics-row-expanded");

		var rows = grid.GetVisualDescendants().OfType<DataGridRow>().ToList();
		var characteristicsRow = rows.Single(r => r.DataContext is Entry { Member: "Characteristics" });
		characteristicsRow.AreDetailsVisible.Should().BeTrue();
		rows.Where(r => r != characteristicsRow).Should()
			.OnlyContain(r => !r.AreDetailsVisible, "only the flags row carries details");

		var shell = await characteristicsRow.WaitForComponent<MetadataRowDetailsControl>();
		var flagsGrid = shell.Content.Should().BeOfType<DataGrid>().Subject;
		((IEnumerable)flagsGrid.ItemsSource!).Cast<BitEntry>().Should().HaveCount(16);
		flagsGrid.HeadersVisibility.Should().Be(DataGridHeadersVisibility.None);
	}

	[AvaloniaTest]
	public async Task Optional_Header_Expands_Details_For_The_Dll_Characteristics_Row_Only()
	{
		var (_, vm) = await TestHarness.BootAsync();

		var optionalNode = vm.AssemblyTreeModel.FindCoreLib()
			.GetChild<MetadataTreeNode>()
			.GetChild<OptionalHeaderTreeNode>();

		vm.AssemblyTreeModel.SelectNode(optionalNode);
		var tab = await vm.DockWorkspace.WaitForMetadataTabAsync();
		TestCapture.Step("optional-header-grid");

		tab.RowDetailsTemplate.Should().NotBeNull();
		tab.RowDetailsVisibilityMode.Should().Be(DataGridRowDetailsVisibilityMode.Collapsed,
			"non-flag rows must never grow a details area, not even on selection");
		tab.IsRowDetailsVisible.Should().NotBeNull();

		var entries = tab.Items.Cast<Entry>().ToList();
		var dllCharacteristics = entries.Single(e => e.Member == "DLL Characteristics");
		tab.IsRowDetailsVisible!(dllCharacteristics).Should().BeTrue();
		entries.Where(e => e != dllCharacteristics).Should()
			.OnlyContain(e => !tab.IsRowDetailsVisible!(e));
	}

	[AvaloniaTest]
	public async Task Double_Tap_Inside_The_Details_Area_Does_Not_Resolve_To_An_Activatable_Row()
	{
		// Row activation navigates away from the metadata view. A double-click inside the
		// details area (e.g. word-selection in an embedded-source TextBox, or a click in the
		// flags sub-grid) is interacting with the details content, not requesting navigation,
		// so the row-resolution walk must reject sources under the details presenter.
		var (window, vm) = await TestHarness.BootAsync();

		var coffNode = vm.AssemblyTreeModel.FindCoreLib()
			.GetChild<MetadataTreeNode>()
			.GetChild<CoffHeaderTreeNode>();

		vm.AssemblyTreeModel.SelectNode(coffNode);
		await vm.DockWorkspace.WaitForMetadataTabAsync();

		var metadataPage = await window.WaitForComponent<MetadataTablePage>();
		var grid = metadataPage.FindControl<DataGrid>("Grid")!;
		await grid.WaitForComponent<DataGridRow>();
		await Waiters.WaitForAsync(() => grid.GetVisualDescendants().OfType<DataGridRow>()
			.Any(r => r.DataContext is Entry { Member: "Characteristics" } && r.AreDetailsVisible));

		var characteristicsRow = grid.GetVisualDescendants().OfType<DataGridRow>()
			.Single(r => r.DataContext is Entry { Member: "Characteristics" });
		var shell = await characteristicsRow.WaitForComponent<MetadataRowDetailsControl>();

		MetadataTablePage.FindActivatableRow(shell).Should().BeNull(
			"visuals under the details presenter must not trigger row activation");

		var cell = characteristicsRow.GetVisualDescendants().OfType<DataGridCell>().First();
		MetadataTablePage.FindActivatableRow(cell).Should().BeSameAs(characteristicsRow,
			"regular cells keep resolving to their owning row");
	}
}
