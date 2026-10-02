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

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Input;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX.Analyzers;
using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy.Analyzers;
using ICSharpCode.ILSpy.Analyzers.TreeNodes;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Dependencies;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TextView;
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
			.Should().Contain([ModuleAnalyzerHeaders.ReferencedBy, ModuleAnalyzerHeaders.DependentCode]);

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
		row.AnalyzerHeader.Should().Be(ModuleAnalyzerHeaders.ReferencedBy);
		await Waiters.WaitForAsync(() => !row.IsLoading && row.Children.OfType<AnalyzedModuleTreeNode>().Any(),
			description: "Referenced By results");
		row.Children.OfType<AnalyzedModuleTreeNode>().Single().Module.AssemblyName.Should().Be(consumerName);
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
		row.AnalyzerHeader.Should().Be(ModuleAnalyzerHeaders.DependentCode);
		await Waiters.WaitForAsync(() => !row.IsLoading && row.Children.OfType<AnalyzerEntityTreeNode>().Any(),
			description: "Dependent Code results");
		var members = row.Children.OfType<AnalyzerEntityTreeNode>().Select(n => n.Member?.FullName).ToList();
		members.Should().Contain($"{consumerName}.{DependencyFixtures.BodyUserType}.{DependencyFixtures.BodyUserMethod}");
		members.Should().NotContain($"{consumerName}.{DependencyFixtures.IndependentType}.{DependencyFixtures.IndependentMethod}");
	}

	[AvaloniaTest]
	public async Task Dependency_Diagram_Opens_A_Tab_And_A_Node_Click_Selects_The_Assembly()
	{
		var (window, _) = await TestHarness.BootAsync();
		var vm = (MainWindowViewModel)window.DataContext!;
		var (libraryPath, consumerPath, libraryName, consumerName) = DependencyFixtures.EmitPair("Ui");
		await vm.OpenAssemblyAsync(libraryPath);
		await vm.OpenAssemblyAsync(consumerPath);
		var consumer = vm.AssemblyTreeModel.SelectNode<AssemblyTreeNode>(consumerName);

		var registry = AppComposition.Current.GetExport<MainMenuCommandRegistry>();
		registry.Commands.Single(c => c.Metadata.Header == nameof(Resources.AssemblyDependencyDiagram)).Metadata.ParentMenuID
			.Should().Be(nameof(Resources._Navigate));
		registry.GetCommand(nameof(Resources.AssemblyDependencyDiagram)).Execute(null);

		var dockWorkspace = AppComposition.Current.GetExport<DockWorkspace>();
		ContentTabPage? tab = null;
		await Waiters.WaitForAsync(() => {
			tab = dockWorkspace.Documents?.VisibleDockables?.OfType<ContentTabPage>()
				.FirstOrDefault(t => t.Content is DependencyGraphViewModel);
			return tab != null;
		}, description: "dependency diagram tab");
		var model = (DependencyGraphViewModel)tab!.Content!;
		model.Graph.Nodes[0].Name.Should().Be(consumerName);
		var libraryIndex = model.Graph.Nodes.Single(n => n.Name == libraryName).Index;

		var view = await window.WaitForComponent<DependencyGraphView>();
		await Waiters.WaitForAsync(() => view.GraphControl.Bounds.Width > 0 && view.GraphControl.Layout != null,
			description: "graph control laid out");
		window.Capture("dependency-diagram");

		var center = view.GraphControl.GetNodeCenter(libraryIndex);
		view.GraphControl.HitTestNode(center).Should().Be(libraryIndex);
		var point = view.GraphControl.TranslatePoint(center, window)!.Value;
		window.MouseDown(point, MouseButton.Left);
		window.MouseUp(point, MouseButton.Left);

		await Waiters.WaitForAsync(() => vm.AssemblyTreeModel.SelectedItem is AssemblyTreeNode { LoadedAssembly.ShortName: var n } && n == libraryName,
			description: "library selected in the tree after clicking its node");
		ReferenceEquals(consumer, vm.AssemblyTreeModel.SelectedItem).Should().BeFalse();
	}

	[AvaloniaTest]
	public async Task Mermaid_Export_Writes_An_Html_Page_With_The_Graph()
	{
		var (vm, libraryName, consumerName) = await OpenPairAsync();
		vm.AssemblyTreeModel.SelectNode<AssemblyTreeNode>(consumerName);
		var path = Path.Combine(DependencyFixtures.NewDirectory(), "deps.html");

		AppComposition.Current.GetExport<MainMenuCommandRegistry>()
			.GetCommand(nameof(Resources.ExportDependencyDiagramMermaid)).Execute(path);

		var dockWorkspace = AppComposition.Current.GetExport<DockWorkspace>();
		await Waiters.WaitForAsync(() => dockWorkspace.Documents?.VisibleDockables?.OfType<ContentTabPage>()
			.Any(t => t.Content is DecompilerTabPageModel { Title: var title } && title == Resources.ExportDependencyDiagramMermaid) == true,
			description: "export report tab offering to open the page");
		File.Exists(path).Should().BeTrue();
		var html = await File.ReadAllTextAsync(path);
		html.Should().Contain("graph LR");
		html.Should().Contain(consumerName);
		html.Should().Contain(libraryName);
	}

	[AvaloniaTest]
	public async Task Unresolved_References_Report_Lists_The_Missing_Reference_With_Its_Log()
	{
		var (_, vm) = await TestHarness.BootAsync();
		var (consumerPath, missingName) = DependencyFixtures.EmitConsumerWithMissingReference("Ui");
		var orphan = await vm.OpenAssemblyAsync(consumerPath);
		vm.AssemblyTreeModel.SelectNode<AssemblyTreeNode>(orphan.ShortName);

		var registry = AppComposition.Current.GetExport<MainMenuCommandRegistry>();
		registry.Commands.Single(c => c.Metadata.Header == nameof(Resources.UnresolvedReferencesReport)).Metadata.ParentMenuID
			.Should().Be(nameof(Resources._Navigate));
		registry.GetCommand(nameof(Resources.UnresolvedReferencesReport)).Execute(null);

		var dockWorkspace = AppComposition.Current.GetExport<DockWorkspace>();
		string? text = null;
		await Waiters.WaitForAsync(() => {
			var content = dockWorkspace.Documents?.VisibleDockables?.OfType<ContentTabPage>()
				.Select(t => t.Content).OfType<DecompilerTabPageModel>()
				.FirstOrDefault(c => c.Title == Resources.UnresolvedReferencesReport);
			text = content?.Text;
			return text?.Contains(missingName, StringComparison.Ordinal) == true;
		}, description: "unresolved references report text");
		text.Should().Contain(orphan.ShortName);
		text.Should().Contain("Could not find reference");
	}
}
