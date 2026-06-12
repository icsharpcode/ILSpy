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
using Avalonia.Input.Platform;

using AvaloniaEdit;
using AvaloniaEdit.Search;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class EditorCommandsTests
{
	[AvaloniaTest]
	public async Task Copy_Entry_Is_Registered_Under_The_Editor_Category()
	{
		// Mirrors WPF's CopyContextMenuEntry — exported with Header=Copy / Category=Editor so
		// it lands under the editor's right-click menu next to other text-view actions.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();

		registry.Entries.Should().Contain(
			e => e.Metadata.Header == nameof(Resources.Copy)
				&& e.Metadata.Category == nameof(Resources.Editor));
	}

	[AvaloniaTest]
	public async Task SelectAll_Entry_Is_Registered_Under_The_Editor_Category()
	{
		// Mirrors WPF's SelectAllContextMenuEntry — Header=Select / Category=Editor.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();

		registry.Entries.Should().Contain(
			e => e.Metadata.Header == nameof(Resources.Select)
				&& e.Metadata.Category == nameof(Resources.Editor));
	}

	[AvaloniaTest]
	public async Task Copy_Entry_Reflects_Editor_Selection_State()
	{
		// Visible whenever a text view is the source; enabled only when the editor has a
		// non-empty selection. Mirrors WPF: SelectionLength > 0 gates the "Copy" item.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);
		// Single-method target — decompiling the full Enumerable class can exceed
		// Waiters.WaitForAsync's 15s timeout (300+ LINQ methods); a single method body
		// is sub-second. Uses the strong completion signal (IsDecompiling: false +
		// non-empty Text) instead of polling the editor's TextLength, which races the
		// model→view binding and can latch on stale state from previous tests.
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.IsExpanded = true;
		var methodNode = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "AsEnumerable");
		vm.AssemblyTreeModel.SelectNode(methodNode);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		var view = await window.WaitForComponent<DecompilerTextView>();
		await view.WaitForComponent<TextEditor>();

		var copy = LocateEntry(nameof(Resources.Copy));
		var ctx = new TextViewContext { TextView = view };

		copy.IsVisible(ctx).Should().BeTrue();
		view.Editor.Select(0, 0);
		copy.IsEnabled(ctx).Should().BeFalse();
		view.Editor.Select(0, 5);
		copy.IsEnabled(ctx).Should().BeTrue();

		// Without a text view, the entry must hide entirely.
		var blank = new TextViewContext();
		copy.IsVisible(blank).Should().BeFalse();
	}

	[AvaloniaTest]
	public async Task Copy_Execute_Writes_The_Selection_To_The_Clipboard()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.IsExpanded = true;
		var methodNode = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "AsEnumerable");
		vm.AssemblyTreeModel.SelectNode(methodNode);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		var view = await window.WaitForComponent<DecompilerTextView>();
		await view.WaitForComponent<TextEditor>();

		var copy = LocateEntry(nameof(Resources.Copy));
		view.Editor.Select(0, 6);
		var expected = view.Editor.Document.GetText(0, 6);

		copy.Execute(new TextViewContext { TextView = view });

		var clipboard = global::Avalonia.Controls.TopLevel.GetTopLevel(view)?.Clipboard;
		var actual = await clipboard!.TryGetTextAsync();
		actual.Should().Be(expected);
	}

	[AvaloniaTest]
	public async Task SelectAll_Execute_Highlights_The_Entire_Document()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.IsExpanded = true;
		var methodNode = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "AsEnumerable");
		vm.AssemblyTreeModel.SelectNode(methodNode);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		var view = await window.WaitForComponent<DecompilerTextView>();
		await view.WaitForComponent<TextEditor>();

		var selectAll = LocateEntry(nameof(Resources.Select));
		view.Editor.Select(0, 0);

		selectAll.Execute(new TextViewContext { TextView = view });

		view.Editor.SelectionLength.Should().Be(view.Editor.Document.TextLength);
	}

	[AvaloniaTest]
	public async Task DecompilerTextView_Installs_AvaloniaEdits_SearchPanel()
	{
		// Ctrl+F opens AvaloniaEdit's built-in search overlay; the panel must be installed on
		// the editor's TextArea. SearchPanel is an Avalonia control; SearchInputHandler is
		// what wires up Ctrl+F + nested handler registration.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		vm.AssemblyTreeModel.SelectNode(typeNode);
		var view = await window.WaitForComponent<DecompilerTextView>();
		await view.WaitForComponent<TextEditor>();

		view.SearchPanel.Should().NotBeNull(
			"DecompilerTextView must install AvaloniaEdit's SearchPanel so Ctrl+F brings up the find bar");
	}

	static IContextMenuEntry LocateEntry(string header)
	{
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();
		return registry.Entries.Single(e => e.Metadata.Header == header).Value;
	}
}
