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


using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

using Avalonia.Headless.NUnit;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Options;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.TextView;

/// <summary>
/// Highlight usages at caret (ReSharper-style): while the caret rests on a symbol, every
/// occurrence of that symbol in the document is marked. Independent of the click highlight
/// (<see cref="MemberReferenceHighlightTests"/>), which keeps its own marks.
/// </summary>
[TestFixture]
public class CaretUsageHighlightTests
{
	static async Task<(DecompilerTextView View, DecompilerTabPageModel Tab, DisplaySettings Settings)> SetupAsync(bool enabled = true)
	{
		var (window, vm) = await TestHarness.BootAsync();
		var settings = AppComposition.Current.GetExport<SettingsService>().DisplaySettings;
		settings.HighlightUsagesAtCaret = enabled;
		await vm.OpenAssemblyAsync(typeof(MemberHighlightSample).Assembly.Location);
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"ILSpy.Tests",
			"ICSharpCode.ILSpy.Tests.TextView",
			"ICSharpCode.ILSpy.Tests.TextView.MemberHighlightSample");
		vm.AssemblyTreeModel.SelectNode(typeNode);
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		var view = window.GetVisualDescendants().OfType<DecompilerTextView>().First();
		return (view, tab, settings);
	}

	static List<ReferenceSegment> MemberSegments(DecompilerTabPageModel tab, string name)
		=> tab.References!
			.Where(r => r.Kind == ReferenceMode.Link && r.Reference is IMember m && m.Name == name)
			.ToList();

	[Test]
	public void Setting_Defaults_To_On_And_Round_Trips()
	{
		new DisplaySettings().HighlightUsagesAtCaret.Should().BeTrue();

		var loaded = new DisplaySettings();
		loaded.LoadFromXml(new XElement("DisplaySettings"));
		loaded.HighlightUsagesAtCaret.Should().BeTrue("a missing attribute keeps the default");

		var off = new DisplaySettings { HighlightUsagesAtCaret = false };
		var reloaded = new DisplaySettings();
		reloaded.LoadFromXml(off.SaveToXml());
		reloaded.HighlightUsagesAtCaret.Should().BeFalse();
	}

	[AvaloniaTest]
	public async Task Caret_On_A_Member_Use_Marks_Every_Occurrence()
	{
		var (view, tab, _) = await SetupAsync();
		var fieldSegments = MemberSegments(tab, nameof(MemberHighlightSample.Field));
		var use = fieldSegments.First(r => !r.IsDefinition);

		view.Editor.TextArea.Caret.Offset = use.StartOffset + 1;

		view.CaretUsageMarks.Select(m => m.StartOffset).Should().BeEquivalentTo(
			fieldSegments.Select(s => s.StartOffset),
			"the definition and every use of the field under the caret are marked");
		view.LocalReferenceMarks.Should().BeEmpty("the caret highlight must not touch the click-highlight marks");
	}

	[AvaloniaTest]
	public async Task Caret_At_The_End_Of_An_Identifier_Still_Counts_As_On_It()
	{
		var (view, tab, _) = await SetupAsync();
		var fieldSegments = MemberSegments(tab, nameof(MemberHighlightSample.Field));
		var use = fieldSegments.First(r => !r.IsDefinition && r.Length > 1);

		view.Editor.TextArea.Caret.Offset = use.EndOffset;

		view.CaretUsageMarks.Should().NotBeEmpty();
	}

	[AvaloniaTest]
	public async Task Caret_On_A_Parameter_Marks_Its_Local_Occurrences()
	{
		var (view, tab, _) = await SetupAsync();
		var itemSegments = tab.References!
			.Where(r => r.Kind == ReferenceMode.LocalHighlight && tab.Text.Substring(r.StartOffset, r.Length) == "item")
			.ToList();
		itemSegments.Should().HaveCountGreaterThanOrEqualTo(2, "the parameter is declared and used once");

		view.Editor.TextArea.Caret.Offset = itemSegments[0].StartOffset;

		view.CaretUsageMarks.Select(m => m.StartOffset).Should().BeEquivalentTo(
			itemSegments.Select(s => s.StartOffset));
	}

	[AvaloniaTest]
	public async Task Moving_The_Caret_Off_A_Symbol_Clears_The_Marks()
	{
		var (view, tab, _) = await SetupAsync();
		var use = MemberSegments(tab, nameof(MemberHighlightSample.Field)).First(r => !r.IsDefinition);
		view.Editor.TextArea.Caret.Offset = use.StartOffset;
		view.CaretUsageMarks.Should().NotBeEmpty();

		// Offset 0 is the start of the leading comment/using block, which carries no reference.
		tab.References!.FindSegmentsContaining(0).Should().BeEmpty();
		view.Editor.TextArea.Caret.Offset = 0;

		view.CaretUsageMarks.Should().BeEmpty();
	}

	[AvaloniaTest]
	public async Task Disabled_Setting_Marks_Nothing_And_Turning_It_Off_Clears_Marks()
	{
		var (view, tab, settings) = await SetupAsync(enabled: false);
		var use = MemberSegments(tab, nameof(MemberHighlightSample.Field)).First(r => !r.IsDefinition);

		view.Editor.TextArea.Caret.Offset = use.StartOffset;
		view.CaretUsageMarks.Should().BeEmpty("the option is off");

		settings.HighlightUsagesAtCaret = true;
		view.CaretUsageMarks.Should().NotBeEmpty("turning the option on highlights the symbol already under the caret");

		settings.HighlightUsagesAtCaret = false;
		view.CaretUsageMarks.Should().BeEmpty("turning the option off clears the marks");
	}
}
