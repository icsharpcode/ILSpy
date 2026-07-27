// Copyright (c) 2026 Siegfried Pammer
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

using Avalonia.Headless.NUnit;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.TextView;

/// <summary>
/// Sample type decompiled by <see cref="FoldingGroupTests"/>. Its XML documentation is
/// supplied by a hand-written documentation file placed next to the test assembly, so the
/// decompiled view renders a '///' fold above <see cref="Documented"/>.
/// </summary>
public class FoldGroupSample
{
	public void Documented(int value)
	{
		Console.WriteLine(value);
	}

	public void Plain()
	{
		Console.WriteLine();
	}
}

/// <summary>
/// Pins the grouped fold-toggle behavior (issue #749): a member and its XML documentation
/// comment toggle as one logical unit, the toggle targets the member (not the enclosing
/// type) when invoked on the header line, the doc fold alone toggles when invoked inside
/// it, and Toggle All uses VS parity (mixed state expands everything).
/// </summary>
[TestFixture]
public class FoldingGroupTests
{
	[OneTimeSetUp]
	public void WriteDocumentationFile()
	{
		string assemblyPath = typeof(FoldGroupSample).Assembly.Location;
		string xmlPath = Path.ChangeExtension(assemblyPath, ".xml");
		string assemblyName = Path.GetFileNameWithoutExtension(assemblyPath);
		File.WriteAllText(xmlPath, $"""
			<?xml version="1.0"?>
			<doc>
				<assembly><name>{assemblyName}</name></assembly>
				<members>
					<member name="M:ICSharpCode.ILSpy.Tests.TextView.FoldGroupSample.Documented(System.Int32)">
						<summary>Summary line used by FoldingGroupTests.</summary>
						<param name="value">Parameter line used by FoldingGroupTests.</param>
					</member>
				</members>
			</doc>
			""");
	}

	static async Task<(DecompilerTextView View, string Text)> SetupAsync()
	{
		var (window, vm) = await TestHarness.BootAsync();
		// Start from a fully expanded document so every toggle direction is deterministic.
		var displaySettings = AppComposition.Current.GetExport<SettingsService>().DisplaySettings;
		displaySettings.ExpandMemberDefinitions = true;
		displaySettings.ExpandXmlDocumentationComments = true;
		await vm.OpenAssemblyAsync(typeof(FoldGroupSample).Assembly.Location);
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"ILSpy.Tests",
			"ICSharpCode.ILSpy.Tests.TextView",
			"ICSharpCode.ILSpy.Tests.TextView.FoldGroupSample");
		vm.AssemblyTreeModel.SelectNode(typeNode);
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		var view = window.GetVisualDescendants().OfType<DecompilerTextView>().First();
		string text = view.Editor.Document.Text;
		text.Should().Contain("Summary line used by FoldingGroupTests",
			"the hand-written documentation file must be picked up for these tests to be meaningful");
		return (view, text);
	}

	static int OffsetOf(string text, string needle)
	{
		int offset = text.IndexOf(needle, StringComparison.Ordinal);
		offset.Should().BeGreaterThan(-1, $"the decompiled text must contain '{needle}'");
		return offset;
	}

	[AvaloniaTest]
	public async Task Toggling_In_The_Body_Folds_Member_And_Documentation_Together()
	{
		var (view, text) = await SetupAsync();
		int docOffset = OffsetOf(text, "Summary line");
		int bodyOffset = OffsetOf(text, "Console.WriteLine(value)");
		int plainBodyOffset = OffsetOf(text, "Console.WriteLine()");

		view.ToggleFoldingAt(bodyOffset);

		view.IsFoldedAt(bodyOffset).Should().BeTrue("the member body must fold");
		view.IsFoldedAt(docOffset).Should().BeTrue("the documentation comment must fold together with its member");
		view.IsFoldedAt(plainBodyOffset).Should().BeFalse("the sibling member is not part of the group");

		view.ToggleFoldingAt(bodyOffset);

		view.IsFoldedAt(bodyOffset).Should().BeFalse("toggling again must unfold the member body");
		view.IsFoldedAt(docOffset).Should().BeFalse("toggling again must unfold the documentation comment");
	}

	[AvaloniaTest]
	public async Task Toggling_On_The_Header_Line_Targets_The_Member_Not_The_Type()
	{
		var (view, text) = await SetupAsync();
		int headerOffset = OffsetOf(text, "void Documented");
		int bodyOffset = OffsetOf(text, "Console.WriteLine(value)");
		int plainBodyOffset = OffsetOf(text, "Console.WriteLine()");

		view.ToggleFoldingAt(headerOffset);

		view.IsFoldedAt(bodyOffset).Should().BeTrue("the member body must fold from its header line");
		view.IsFoldedAt(plainBodyOffset).Should().BeFalse(
			"the enclosing type must not fold when the toggle targets a member header");
	}

	[AvaloniaTest]
	public async Task Toggling_Inside_The_Documentation_Folds_Only_The_Documentation()
	{
		var (view, text) = await SetupAsync();
		int docOffset = OffsetOf(text, "Summary line");
		int bodyOffset = OffsetOf(text, "Console.WriteLine(value)");

		view.ToggleFoldingAt(docOffset);

		view.IsFoldedAt(docOffset).Should().BeTrue("the documentation fold must collapse");
		view.IsFoldedAt(bodyOffset).Should().BeFalse("the member body stays open when only the docs are toggled");
	}

	[AvaloniaTest]
	public async Task Toggle_All_Expands_Everything_From_A_Mixed_State()
	{
		var (view, text) = await SetupAsync();
		int bodyOffset = OffsetOf(text, "Console.WriteLine(value)");

		// Create a mixed state: one group folded, the rest open.
		view.ToggleFoldingAt(bodyOffset);
		view.FoldedFoldingCount.Should().BeGreaterThan(0);
		view.GetFoldingsForTest().Should().Contain(f => !f.IsFolded, "the state must be mixed for this test");

		view.ToggleAllFoldings();

		view.FoldedFoldingCount.Should().Be(0, "VS parity: a mixed state expands all folds");

		view.ToggleAllFoldings();

		view.GetFoldingsForTest().Should().OnlyContain(f => f.IsFolded,
			"a uniformly expanded document collapses everything");
	}
}
