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

using ILSpy.AppEnv;
using ILSpy.Metadata;
using ILSpy.Metadata.CorTables;
using ILSpy.TreeNodes;
using ILSpy.ViewModels;
using ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Metadata;

[TestFixture]
public class MetadataFilterTests
{
	sealed class SampleEntry
	{
		public int RID { get; set; }
		public string Name { get; set; } = "";
		public string Culture { get; set; } = "";
	}

	[Test]
	public void MatchesFilter_Returns_True_For_Empty_Or_Null_Filter()
	{
		// An empty filter must show every row — the no-filter state is the identity case.
		var entry = new SampleEntry { RID = 1, Name = "System.Runtime" };
		MetadataTablePageModel.MatchesFilter(entry, null).Should().BeTrue();
		MetadataTablePageModel.MatchesFilter(entry, "").Should().BeTrue();
	}

	[Test]
	public void MatchesFilter_Returns_True_When_Any_Property_Contains_The_Substring_Case_Insensitively()
	{
		// Filter is a case-insensitive Contains over every property's stringified value, so
		// "system" matches "System.Runtime" and "neutral" matches the Culture column.
		var entry = new SampleEntry { RID = 1, Name = "System.Runtime", Culture = "neutral" };
		MetadataTablePageModel.MatchesFilter(entry, "system").Should().BeTrue();
		MetadataTablePageModel.MatchesFilter(entry, "RUNTIME").Should().BeTrue();
		MetadataTablePageModel.MatchesFilter(entry, "neutral").Should().BeTrue();
	}

	[Test]
	public void MatchesFilter_Returns_False_When_No_Property_Contains_The_Substring()
	{
		var entry = new SampleEntry { RID = 1, Name = "System.Runtime" };
		MetadataTablePageModel.MatchesFilter(entry, "Foo").Should().BeFalse();
	}

	[Test]
	public void MatchesFilter_Stringifies_Numeric_Values_So_Filter_Hits_Numeric_Columns()
	{
		// Without stringifying, filtering "42" against a numeric RID would silently miss.
		var entry = new SampleEntry { RID = 42, Name = "X" };
		MetadataTablePageModel.MatchesFilter(entry, "42").Should().BeTrue();
	}

	[AvaloniaTest]
	public async Task FilterText_Reduces_Visible_Rows_To_Those_Whose_Property_Contains_The_Substring()
	{
		// Integration check: navigate to the TypeDef table (guaranteed populated for any
		// loaded module) and confirm setting FilterText='System' shrinks the visible-row
		// set, and clearing it restores the full set.

		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);

		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(coreLibName);
		assemblyNode.EnsureLazyChildren();
		var metadataNode = assemblyNode.Children.OfType<MetadataTreeNode>().Single();
		metadataNode.EnsureLazyChildren();
		var tablesNode = metadataNode.Children.OfType<MetadataTablesTreeNode>().Single();
		tablesNode.EnsureLazyChildren();
		var typeDefNode = tablesNode.Children.OfType<TypeDefTableTreeNode>().Single();

		vm.AssemblyTreeModel.SelectNode(typeDefNode);
		var tab = await vm.DockWorkspace.WaitForMetadataTabAsync();

		var totalCount = tab.Items.Count;
		totalCount.Should().BeGreaterThan(0);

		tab.FilterText = "System";
		var visible = tab.Items.Where(e => MetadataTablePageModel.MatchesFilter(e, tab.FilterText)).ToList();
		visible.Should().NotBeEmpty("at least one TypeDef row should mention 'System'");
		visible.Count.Should().BeLessThan(totalCount, "the filter must hide at least one row");

		tab.FilterText = "";
		var afterClear = tab.Items.Where(e => MetadataTablePageModel.MatchesFilter(e, tab.FilterText)).Count();
		afterClear.Should().Be(totalCount);
	}
}
