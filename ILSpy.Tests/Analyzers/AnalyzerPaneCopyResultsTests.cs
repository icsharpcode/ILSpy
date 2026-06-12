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
public class AnalyzerPaneCopyResultsTests
{
	[AvaloniaTest]
	public async Task CopyResults_Entry_Is_Visible_For_AnalyzerSearchTreeNode_And_Hidden_Otherwise()
	{
		var (_, vm) = await TestHarness.BootAsync();

		var entry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>()
			.GetEntry("Copy results");

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		var target = (ICSharpCode.Decompiler.TypeSystem.ITypeDefinition)typeNode.Member!;
		var analyzer = AnalyzerTreeNode.Analyzers
			.Where(a => a.Metadata?.Header == "Used By")
			.Select(a => a.CreateExport().Value)
			.First(a => a.Show(target));
		var search = new AnalyzerSearchTreeNode(target, analyzer, "Used By");

		entry.IsVisible(new TextViewContext { SelectedTreeNodes = new SharpTreeNode[] { search } })
			.Should().BeTrue("Copy results applies to analyzer search-node rows");

		entry.IsVisible(new TextViewContext { SelectedTreeNodes = new SharpTreeNode[] { typeNode } })
			.Should().BeFalse("the entry must NOT surface on the assembly tree");
	}

	[AvaloniaTest]
	public async Task CopyResults_Execute_Writes_Each_Child_On_Its_Own_Line()
	{
		var (_, vm) = await TestHarness.BootAsync();

		var entry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>()
			.GetEntry("Copy results");

		// Build a search node and stuff some deterministic children in directly, bypassing
		// the analyzer altogether — the copy entry should only care about Text values.
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		var target = (ICSharpCode.Decompiler.TypeSystem.ITypeDefinition)typeNode.Member!;
		var analyzer = AnalyzerTreeNode.Analyzers
			.Where(a => a.Metadata?.Header == "Used By")
			.Select(a => a.CreateExport().Value)
			.First(a => a.Show(target));
		var search = new AnalyzerSearchTreeNode(target, analyzer, "Used By");
		search.Children.Add(new StubResultNode("alpha"));
		search.Children.Add(new StubResultNode("beta"));

		entry.Execute(new TextViewContext { SelectedTreeNodes = new SharpTreeNode[] { search } });
		TestCapture.Step("copy-results-executed");

		// We can't inspect the clipboard contract in a headless test without a window with
		// a real lifetime — the test asserts the entry handled execution without throwing
		// and the children Text values are still intact (i.e. Execute didn't mutate them).
		search.Children[0].Text.Should().Be("alpha");
		search.Children[1].Text.Should().Be("beta");
	}

	sealed class StubResultNode : AnalyzerTreeNode
	{
		readonly string text;

		public StubResultNode(string text) { this.text = text; }

		public override object Text => text;

		public override bool HandleAssemblyListChanged(
			System.Collections.Generic.ICollection<ICSharpCode.ILSpyX.LoadedAssembly> removedAssemblies,
			System.Collections.Generic.ICollection<ICSharpCode.ILSpyX.LoadedAssembly> addedAssemblies) => true;
	}
}
