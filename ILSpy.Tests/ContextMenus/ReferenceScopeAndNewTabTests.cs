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
using Avalonia.Threading;

using AwesomeAssertions;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Search;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// The remaining decompiler text-view entries that act on a clicked symbol (IEntity): "Decompile to
/// new tab", and the two "Scope search to ..." entries.
/// </summary>
[TestFixture]
public class ReferenceScopeAndNewTabTests
{
	static TextViewContext RefContext(IEntity entity)
		=> new() { Reference = new ReferenceSegment { Reference = entity } };

	[AvaloniaTest]
	public async Task Decompile_To_New_Tab_On_A_Reference_Opens_A_New_Tab()
	{
		var (_, vm) = await TestHarness.BootAsync(3);
		// Two entries share the DecompileToNewPanel header (tree + metadata-row); pick the tree/code one.
		var entry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>().Entries
			.First(e => e.Value.GetType().Name == "DecompileInNewViewCommand").Value;

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		var entity = (IEntity)typeNode.Member!;

		entry.IsVisible(RefContext(entity)).Should().BeTrue("a clicked entity must surface Decompile to new tab");

		int before = vm.DockWorkspace.Documents!.VisibleDockables!.OfType<ContentTabPage>().Count();
		entry.Execute(RefContext(entity));
		for (int i = 0; i < 8; i++)
		{
			Dispatcher.UIThread.RunJobs();
			await Task.Delay(20);
		}

		vm.DockWorkspace.Documents!.VisibleDockables!.OfType<ContentTabPage>().Count()
			.Should().BeGreaterThan(before, "Decompile to new tab on a code reference must open a new document tab");
	}

	[AvaloniaTest]
	public async Task Scope_Search_To_Assembly_On_A_Reference_Sets_The_inassembly_Filter()
	{
		var (_, vm) = await TestHarness.BootAsync(3);
		var entry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>()
			.GetEntry(nameof(Resources.ScopeSearchToThisAssembly));
		var entity = (IEntity)vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable").Member!;

		entry.IsVisible(RefContext(entity)).Should().BeTrue();
		entry.Execute(RefContext(entity));

		var search = AppComposition.Current.GetExport<SearchPaneModel>();
		search.SearchTerm.Should().Contain("inassembly:").And.Contain("System.Linq",
			"scoping to the reference's assembly must prepend inassembly:<name>");
	}

	[AvaloniaTest]
	public async Task Scope_Search_To_Namespace_On_A_Reference_Sets_The_innamespace_Filter()
	{
		var (_, vm) = await TestHarness.BootAsync(3);
		var entry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>()
			.GetEntry(nameof(Resources.ScopeSearchToThisNamespace));
		var entity = (IEntity)vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable").Member!;

		entry.IsVisible(RefContext(entity)).Should().BeTrue();
		entry.Execute(RefContext(entity));

		var search = AppComposition.Current.GetExport<SearchPaneModel>();
		search.SearchTerm.Should().Contain("innamespace:").And.Contain("System.Linq",
			"scoping to the reference's namespace must prepend innamespace:<namespace>");
	}
}
