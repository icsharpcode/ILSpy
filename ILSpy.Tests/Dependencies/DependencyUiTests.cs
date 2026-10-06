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

using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Input;
using Avalonia.VisualTree;

using AwesomeAssertions;

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
}
