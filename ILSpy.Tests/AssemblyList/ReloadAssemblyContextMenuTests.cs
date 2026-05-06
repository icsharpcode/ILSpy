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

using ILSpy;
using ILSpy.AppEnv;
using ILSpy.TreeNodes;
using ILSpy.ViewModels;
using ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class ReloadAssemblyContextMenuTests
{
	[AvaloniaTest]
	public async Task Reload_Entry_Is_Registered()
	{
		// AssemblyTreeNode contributes a [ExportContextMenuEntry] for "_Reload" so the user
		// can re-read an assembly from disk after rebuilding it elsewhere. Verifies the entry
		// shows up in the live MEF registry.

		// Arrange + Act — boot so composition is initialised.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();

		// Assert — registry contains an entry whose header is the _Reload resource.
		registry.Entries.Should().Contain(
			e => e.Metadata.Header == nameof(Resources._Reload),
			"AssemblyTreeNode must export a Reload context-menu entry");
	}

	[AvaloniaTest]
	public async Task Reload_Entry_Is_Visible_Only_For_AssemblyTreeNode_Selections()
	{
		// IsVisible: same shape as Remove — only when every selected node is an
		// AssemblyTreeNode. Hidden on member/namespace/resource selections so it doesn't
		// pollute their right-click menus.

		// Arrange — boot, locate the registered entry.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();
		var reloadEntry = registry.Entries
			.Single(e => e.Metadata.Header == nameof(Resources._Reload))
			.Value;

		var asm = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");

		// Assert — visible for assembly-only selections, hidden otherwise.
		reloadEntry.IsVisible(new TextViewContext {
			SelectedTreeNodes = new[] { (SharpTreeNode)asm }
		}).Should().BeTrue();

		reloadEntry.IsVisible(new TextViewContext {
			SelectedTreeNodes = new[] { (SharpTreeNode)asm, typeNode }
		}).Should().BeFalse();

		reloadEntry.IsVisible(new TextViewContext { SelectedTreeNodes = null })
			.Should().BeFalse();
	}

	[AvaloniaTest]
	public async Task Reload_Entry_Execute_Replaces_The_LoadedAssembly_For_Selected_Files()
	{
		// Execute fires AssemblyList.ReloadAssembly on each selected assembly's path. Since
		// reload re-creates the LoadedAssembly object (forcing the metadata cache to refresh),
		// after Execute the AssemblyList contains a *different* LoadedAssembly instance for
		// the same file name.

		// Arrange — boot, snapshot the LoadedAssembly we'll reload.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();
		var reloadEntry = registry.Entries
			.Single(e => e.Metadata.Header == nameof(Resources._Reload))
			.Value;

		var node = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		var originalAssembly = node.LoadedAssembly;
		var fileName = originalAssembly.FileName;

		// Act — fire Execute with the assembly node in the context.
		reloadEntry.Execute(new TextViewContext {
			SelectedTreeNodes = new[] { (SharpTreeNode)node }
		});

		// Assert — the LoadedAssembly for that file name is a different instance now.
		await Waiters.WaitForAsync(() =>
			vm.AssemblyTreeModel.AssemblyList!.GetAssemblies().Any(a =>
				a.FileName == fileName && !ReferenceEquals(a, originalAssembly)));
	}
}
