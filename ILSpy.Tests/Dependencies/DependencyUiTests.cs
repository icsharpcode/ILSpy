// Copyright (c) 2026 Piero Viano
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

using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy.Analyzers;
using ICSharpCode.ILSpy.Analyzers.TreeNodes;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Dependencies;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Dependencies;

[TestFixture]
public class DependencyUiTests
{
	static async Task<(MainWindowViewModel Vm, string LibraryName, string ConsumerName)> OpenPairAsync(Avalonia.Controls.Window? _ = null)
	{
		var (_, vm) = await TestHarness.BootAsync();
		var (libraryPath, consumerPath, libraryName, consumerName) = DependencyFixtures.EmitPair("Ui");
		await vm.OpenAssemblyAsync(libraryPath);
		await vm.OpenAssemblyAsync(consumerPath);
		return (vm, libraryName, consumerName);
	}

	static TextViewContext Select(SharpTreeNode node) => new() { SelectedTreeNodes = [node] };

	[AvaloniaTest]
	public async Task Assembly_Node_Has_A_Referenced_By_Folder_Listing_Its_Consumers()
	{
		var (vm, libraryName, consumerName) = await OpenPairAsync();
		var library = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(libraryName);

		var folder = library.GetChild<ReferencedByFolderTreeNode>();
		folder.Text.Should().Be(Resources.ReferencedBy);
		var child = folder.GetChild<ReferencingAssemblyTreeNode>();
		child.Assembly.ShortName.Should().Be(consumerName);
		child.IsVersionMismatch.Should().BeFalse();
		((string)child.Text).Should().StartWith(consumerName);

		// The consumer itself has no consumers: its folder is empty.
		var consumer = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(consumerName);
		var consumerFolder = consumer.GetChild<ReferencedByFolderTreeNode>();
		consumerFolder.EnsureLazyChildren();
		consumerFolder.Children.Should().BeEmpty();
	}

	[AvaloniaTest]
	public async Task Analyze_Accepts_An_Assembly_And_Shows_The_Module_Analyzers()
	{
		var (vm, libraryName, _) = await OpenPairAsync();
		var library = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(libraryName);
		var entry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>().GetEntry(nameof(Resources.Analyze));

		entry.IsVisible(Select(library)).Should().BeTrue();
		entry.IsEnabled(Select(library)).Should().BeTrue();
		entry.Execute(Select(library));

		var analyzerVm = AppComposition.Current.GetExport<AnalyzerTreeViewModel>();
		var moduleRow = analyzerVm.Root.Children.OfType<AnalyzedModuleTreeNode>().Single();
		moduleRow.Module.MetadataFile.Should().BeSameAs(library.LoadedAssembly.GetMetadataFileOrNull());
		moduleRow.EnsureLazyChildren();
		moduleRow.Children.OfType<AnalyzerSearchTreeNode>().Select(r => r.AnalyzerHeader)
			.Should().Contain([ReferencedByContextMenuEntry.AnalyzerHeader, DependentCodeContextMenuEntry.AnalyzerHeader]);

		entry.Execute(Select(library));
		analyzerVm.Root.Children.OfType<AnalyzedModuleTreeNode>().Should().ContainSingle("re-analyzing reuses the row");
	}

	[AvaloniaTest]
	public async Task Referenced_By_Context_Entry_Lists_The_Consumer_In_The_Analyzer_Pane()
	{
		var (vm, libraryName, consumerName) = await OpenPairAsync();
		var library = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(libraryName);
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();
		registry.Entries.Single(e => e.Metadata.Header == nameof(Resources.ReferencedBy)).Metadata.Category
			.Should().Be("Navigation");
		var entry = registry.GetEntry(nameof(Resources.ReferencedBy));

		entry.IsVisible(Select(library)).Should().BeTrue();
		entry.Execute(Select(library));

		var analyzerVm = AppComposition.Current.GetExport<AnalyzerTreeViewModel>();
		var row = (AnalyzerSearchTreeNode)analyzerVm.SelectedItems.Single();
		row.AnalyzerHeader.Should().Be(ReferencedByContextMenuEntry.AnalyzerHeader);
		await Waiters.WaitForAsync(() => !row.IsLoading && row.Children.OfType<AnalyzedModuleTreeNode>().Any(),
			description: "Referenced By results");
		row.Children.OfType<AnalyzedModuleTreeNode>().Single().Module.AssemblyName.Should().Be(consumerName);
	}

	[AvaloniaTest]
	public async Task Module_Entries_Are_Disabled_For_An_Unresolved_Assembly_Reference()
	{
		var (_, vm) = await TestHarness.BootAsync();
		var (consumerPath, missingName) = DependencyFixtures.EmitConsumerWithMissingReference("Ui");
		var consumer = await vm.OpenAssemblyAsync(consumerPath);
		var consumerNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(consumer.ShortName);
		consumerNode.EnsureLazyChildren();
		var folder = consumerNode.Children.OfType<ReferenceFolderTreeNode>().Single();
		folder.EnsureLazyChildren();
		var reference = folder.Children.OfType<AssemblyReferenceTreeNode>()
			.Single(r => r.AssemblyReference.Name == missingName);
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();

		foreach (var header in new[] { nameof(Resources.ReferencedBy), nameof(Resources.DependentCode) })
		{
			var entry = registry.GetEntry(header);
			entry.IsVisible(Select(reference)).Should().BeTrue();
			entry.IsEnabled(Select(reference)).Should().BeFalse($"{header} has no module to analyze");
		}
	}

	[AvaloniaTest]
	public async Task Dependent_Code_Context_Entry_Lists_The_Using_Members()
	{
		var (vm, libraryName, consumerName) = await OpenPairAsync();
		var library = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(libraryName);
		var entry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>().GetEntry(nameof(Resources.DependentCode));

		entry.IsVisible(Select(library)).Should().BeTrue();
		entry.Execute(Select(library));

		var analyzerVm = AppComposition.Current.GetExport<AnalyzerTreeViewModel>();
		var row = (AnalyzerSearchTreeNode)analyzerVm.SelectedItems.Single();
		row.AnalyzerHeader.Should().Be(DependentCodeContextMenuEntry.AnalyzerHeader);
		await Waiters.WaitForAsync(() => !row.IsLoading && row.Children.OfType<AnalyzerEntityTreeNode>().Any(),
			description: "Dependent Code results");
		var members = row.Children.OfType<AnalyzerEntityTreeNode>().Select(n => n.Member?.FullName).ToList();
		members.Should().Contain($"{consumerName}.{DependencyFixtures.BodyUserType}.{DependencyFixtures.BodyUserMethod}");
		members.Should().NotContain($"{consumerName}.{DependencyFixtures.IndependentType}.{DependencyFixtures.IndependentMethod}");
	}
}
