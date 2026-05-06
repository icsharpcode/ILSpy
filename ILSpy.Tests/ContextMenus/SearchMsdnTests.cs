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
using ILSpy.Commands;
using ILSpy.TreeNodes;
using ILSpy.ViewModels;
using ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class SearchMsdnTests
{
	[AvaloniaTest]
	public async Task SearchMsdn_Entry_Is_Registered()
	{
		// "Search Microsoft Docs..." is exported as a [ExportContextMenuEntry] tagged with
		// the SearchMSDN resource. Verifies it shows up via MEF.

		// Arrange + Act — boot.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();

		// Assert
		registry.Entries.Should().Contain(
			e => e.Metadata.Header == nameof(Resources.SearchMSDN));
	}

	[AvaloniaTest]
	public async Task SearchMsdn_Is_Visible_Only_For_Type_Member_Or_Namespace_Selections()
	{
		// IsVisible: only the entity-bearing tree-node kinds plus NamespaceTreeNode (which has
		// its own MS-Docs landing page). Hidden for assembly nodes, references folders, etc.

		// Arrange — boot, locate entry + sample nodes.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();
		var entry = (SearchMsdnContextMenuEntry)registry.Entries
			.Single(e => e.Metadata.Header == nameof(Resources.SearchMSDN))
			.Value;

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");

		entry.IsVisible(new TextViewContext { SelectedTreeNodes = new[] { (SharpTreeNode)typeNode } })
			.Should().BeTrue();
		entry.IsVisible(new TextViewContext { SelectedTreeNodes = new[] { (SharpTreeNode)assemblyNode } })
			.Should().BeFalse("assembly nodes don't have an MS-Docs page");
		entry.IsVisible(new TextViewContext { SelectedTreeNodes = null })
			.Should().BeFalse();
	}

	[AvaloniaTest]
	public async Task SearchMsdn_Builds_The_Expected_Microsoft_Docs_URL()
	{
		// The URL is `https://learn.microsoft.com/dotnet/api/<reflection-name>`, lowercased,
		// with backticks turned into hyphens (generic arity) and `+` into `.` (nested types).
		// Constructors get a `..ctor` → `-ctor` rename so the docs site resolves them.

		// Arrange — boot.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();
		var entry = (SearchMsdnContextMenuEntry)registry.Entries
			.Single(e => e.Metadata.Header == nameof(Resources.SearchMSDN))
			.Value;

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");

		// Assert — the type's docs URL.
		entry.GetMsdnUrl(typeNode)
			.Should().Be("https://learn.microsoft.com/dotnet/api/system.linq.enumerable");
	}
}
