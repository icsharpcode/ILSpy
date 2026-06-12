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

using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Compare;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Compare;

/// <summary>
/// Pins the right-click → Compare flow: visibility gate (exactly two valid assembly
/// nodes), and that <c>Execute</c> opens a fresh tab containing a
/// <see cref="CompareTabPageModel"/> with a populated root.
/// </summary>
[TestFixture]
public class CompareContextMenuTests
{
	[AvaloniaTest]
	public async Task Compare_Is_Visible_When_Two_Assemblies_Are_Selected()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 2);

		var entry = AppComposition.Current.GetExport<ICSharpCode.ILSpy.ContextMenuEntryRegistry>()
			.Entries.Single(e => e.Metadata.Header == "Compare...").Value;
		var assemblies = vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
			.Where(a => a.IsLoadedAsValidAssembly).Take(2).ToList();
		assemblies.Should().HaveCount(2, "the fixture must load at least two valid assemblies");

		var nodes = assemblies.Select(a =>
			(SharpTreeNode)vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(a.ShortName)).ToArray();
		entry.IsVisible(new TextViewContext { SelectedTreeNodes = nodes }).Should().BeTrue();
		entry.IsEnabled(new TextViewContext { SelectedTreeNodes = nodes }).Should().BeTrue();
	}

	[AvaloniaTest]
	public async Task Compare_Is_Hidden_For_Single_Selection()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);

		var entry = AppComposition.Current.GetExport<ICSharpCode.ILSpy.ContextMenuEntryRegistry>()
			.Entries.Single(e => e.Metadata.Header == "Compare...").Value;
		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var node = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(coreLibName);
		entry.IsVisible(new TextViewContext {
			SelectedTreeNodes = new[] { (SharpTreeNode)node }
		}).Should().BeFalse(
			"Compare needs exactly two assemblies — single selection must hide the entry");
	}

	[AvaloniaTest]
	public async Task Execute_Opens_A_Tab_Containing_A_CompareTabPageModel_With_Populated_Tree()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 2);

		var entry = AppComposition.Current.GetExport<ICSharpCode.ILSpy.ContextMenuEntryRegistry>()
			.Entries.Single(e => e.Metadata.Header == "Compare...").Value;
		var dockWorkspace = AppComposition.Current.GetExport<ICSharpCode.ILSpy.Docking.DockWorkspace>();
		var assemblies = vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
			.Where(a => a.IsLoadedAsValidAssembly).Take(2).ToList();
		var nodes = assemblies.Select(a =>
			(SharpTreeNode)vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(a.ShortName)).ToArray();

		entry.Execute(new TextViewContext { SelectedTreeNodes = nodes });

		// The new tab's Content is the CompareTabPageModel. Walk the dock's tabs and find it.
		var page = dockWorkspace.Documents?.VisibleDockables?
			.OfType<ContentTabPage>()
			.Select(t => t.Content)
			.OfType<CompareTabPageModel>()
			.FirstOrDefault();
		Assert.That(page, Is.Not.Null, "a ContentTabPage with CompareTabPageModel content must exist");
		ReferenceEquals(page!.LeftAssembly, assemblies[0]).Should().BeTrue(
			"first-selected node becomes the left-hand assembly of the comparison");
		ReferenceEquals(page.RightAssembly, assemblies[1]).Should().BeTrue(
			"second-selected node becomes the right-hand assembly");
		((object?)page.RootEntry).Should().NotBeNull("the merged-tree root must be set at construction");
	}
}
