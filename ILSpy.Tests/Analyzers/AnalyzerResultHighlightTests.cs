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

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX.TreeView.PlatformAbstractions;

using ICSharpCode.ILSpy.Analyzers;
using ICSharpCode.ILSpy.Analyzers.TreeNodes;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Analyzers;

/// <summary>
/// Logged follow-up from earlier live-testing: when the user double-clicks an analyzer
/// result, the decompiled-output view should highlight the analysed symbol — the entity
/// at the root of that result chain. The mechanism is `DecompilerTabPageModel.HighlightedReference`:
/// `AnalyzerEntityTreeNode.ActivateItem` sets it to <c>SourceMember</c>; the text view
/// observes the property and re-runs <c>HighlightLocalReferences</c> against the current
/// (or next) References collection.
/// </summary>
[TestFixture]
public class AnalyzerResultHighlightTests
{
	[AvaloniaTest]
	public async Task ActivateItem_On_Analyzer_Result_Sets_HighlightedReference_To_SourceMember()
	{
		// The end-to-end path is: ActivateItem sends NavigateToReferenceEventArgs through
		// MessageBus → AssemblyTreeModel's subscription selects the Reference's tree node
		// AND pushes Source onto the active decompiler tab's HighlightedReference.

		var (_, vm) = await TestHarness.BootAsync();

		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var stringNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			coreLibName, "System", "System.String");
		stringNode.IsExpanded = true;
		var toStringNode = stringNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "ToString");
		var concatNode = stringNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Concat");

		// Materialise the decompiler tab by driving a real navigation first; without this
		// DockWorkspace.ActiveDecompilerTab is null and the highlight path has no sink.
		vm.AssemblyTreeModel.SelectNode(stringNode);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		TestCapture.Step("string-decompiled");
		var decompTab = vm.DockWorkspace.ActiveDecompilerTab;
		Assert.That(decompTab, Is.Not.Null, "selecting a type must materialise the decompiler tab");
		decompTab!.HighlightedReference = null;

		var resultNode = new AnalyzedMethodTreeNode(toStringNode.MethodDefinition, source: concatNode.MethodDefinition);
		resultNode.ActivateItem(new StubArgs());
		TestCapture.Step("highlighted-source-member");

		((object?)decompTab.HighlightedReference).Should().BeSameAs(concatNode.MethodDefinition,
			"NavigateToReferenceEventArgs.Source (= SourceMember) must land in HighlightedReference so the editor view can paint local-reference marks");
	}

	[AvaloniaTest]
	public async Task ActivateItem_Without_SourceMember_Leaves_HighlightedReference_Unchanged()
	{
		// The top-level analysed entity (the row directly under Root) has SourceMember == null.
		// Activating it is the "jump to declaration" gesture — there's nothing to highlight,
		// so the subscriber must skip the HighlightedReference assignment rather than blow
		// away an active highlight.

		var (_, vm) = await TestHarness.BootAsync();

		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var stringNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			coreLibName, "System", "System.String");
		stringNode.IsExpanded = true;
		var lengthProp = stringNode.Children.OfType<PropertyTreeNode>()
			.First(p => p.PropertyDefinition.Name == "Length");

		// Same materialisation as the first test.
		vm.AssemblyTreeModel.SelectNode(stringNode);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		TestCapture.Step("string-decompiled");
		var decompTab = vm.DockWorkspace.ActiveDecompilerTab!;
		decompTab.HighlightedReference = lengthProp.PropertyDefinition;

		var topLevel = new AnalyzedMethodTreeNode(
			stringNode.Children.OfType<MethodTreeNode>().First().MethodDefinition,
			source: null);
		topLevel.ActivateItem(new StubArgs());
		TestCapture.Step("activated-top-level-entity");

		((object?)decompTab.HighlightedReference).Should().BeSameAs(lengthProp.PropertyDefinition,
			"top-level analysed-entity rows have no SourceMember and must not overwrite the active highlight");
	}

	sealed class StubArgs : IPlatformRoutedEventArgs
	{
		public bool Handled { get; set; }
	}
}
