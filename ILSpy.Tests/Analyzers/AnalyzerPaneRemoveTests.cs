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
using ICSharpCode.ILSpy.Analyzers;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Analyzers;

[TestFixture]
public class AnalyzerPaneRemoveTests
{
	[AvaloniaTest]
	public async Task Remove_Entry_Is_Visible_Only_For_Top_Level_Analysed_Entities()
	{
		var (_, vm) = await TestHarness.BootAsync();

		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();
		var entry = registry.Entries
			.Single(e => e.Metadata.Header == "Remove"
				&& e.Value is RemoveAnalyzeContextMenuEntry)
			.Value;

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		var analyzerVm = AppComposition.Current.GetExport<AnalyzerTreeViewModel>();
		var added = analyzerVm.Analyze((ICSharpCode.Decompiler.TypeSystem.IEntity)typeNode.Member!);
		TestCapture.Step("entity-analysed");

		entry.IsVisible(new TextViewContext { SelectedTreeNodes = new SharpTreeNode[] { added } })
			.Should().BeTrue("Remove applies to top-level analysed entities");

		entry.IsVisible(new TextViewContext { SelectedTreeNodes = new SharpTreeNode[] { typeNode } })
			.Should().BeFalse("Remove must not surface on the assembly tree");
	}

	[AvaloniaTest]
	public async Task Remove_Execute_Drops_The_Selected_Root_Children()
	{
		var (_, vm) = await TestHarness.BootAsync();

		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();
		var entry = registry.Entries
			.Single(e => e.Metadata.Header == "Remove"
				&& e.Value is RemoveAnalyzeContextMenuEntry)
			.Value;

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		var analyzerVm = AppComposition.Current.GetExport<AnalyzerTreeViewModel>();
		var added = analyzerVm.Analyze((ICSharpCode.Decompiler.TypeSystem.IEntity)typeNode.Member!);
		var beforeCount = analyzerVm.Root.Children.Count;

		entry.Execute(new TextViewContext { SelectedTreeNodes = new SharpTreeNode[] { added } });
		TestCapture.Step("entity-removed-from-pane");

		analyzerVm.Root.Children.Should().NotContain(added,
			"Execute on Remove must drop the row from the pane root");
		analyzerVm.Root.Children.Count.Should().Be(beforeCount - 1);
	}
}
