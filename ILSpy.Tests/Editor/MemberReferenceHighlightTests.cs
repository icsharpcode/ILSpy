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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Input;
using Avalonia.VisualTree;

using AvaloniaEdit;
using AvaloniaEdit.Rendering;

using AwesomeAssertions;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.TextView;

/// <summary>
/// Sample type decompiled by <see cref="MemberReferenceHighlightTests"/>: a field with
/// multiple uses, a generic method whose call site carries a specialized instance while
/// the declaration carries the definition, and two differently-parameterized uses of the
/// same generic type.
/// </summary>
public class MemberHighlightSample
{
	public int Field;
	public List<int>? Other;

	public List<T> Make<T>(T item)
	{
		return new List<T> { item };
	}

	public int UseAll()
	{
		Field = 1;
		Other = Make(2);
		return Field + Other.Count;
	}
}

/// <summary>
/// Pins the member click-highlight behavior (issue #781): with the setting enabled, a
/// single click on a member or type reference highlights all its occurrences in the view
/// and Ctrl+Click navigates; with the setting disabled (the default), plain click keeps
/// navigating.
/// </summary>
[TestFixture]
public class MemberReferenceHighlightTests
{
	static async Task<(MainWindow Window, DecompilerTextView View, DecompilerTabPageModel Tab)> SetupAsync(bool highlightMemberReferences)
	{
		var (window, vm) = await TestHarness.BootAsync();
		AppComposition.Current.GetExport<SettingsService>().DisplaySettings.HighlightMemberReferences = highlightMemberReferences;
		await vm.OpenAssemblyAsync(typeof(MemberHighlightSample).Assembly.Location);
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"ILSpy.Tests",
			"ICSharpCode.ILSpy.Tests.TextView",
			"ICSharpCode.ILSpy.Tests.TextView.MemberHighlightSample");
		vm.AssemblyTreeModel.SelectNode(typeNode);
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		var view = window.GetVisualDescendants().OfType<DecompilerTextView>().First();
		return (window, view, tab);
	}

	static List<ReferenceSegment> SegmentsOf(DecompilerTabPageModel tab, string memberName)
	{
		return tab.References!
			.Where(r => r.Kind == ReferenceMode.Link && r.Reference is IMember m && m.Name == memberName)
			.ToList();
	}

	[AvaloniaTest]
	public async Task Clicking_A_Member_Navigates_When_The_Setting_Is_Off()
	{
		var (_, view, tab) = await SetupAsync(highlightMemberReferences: false);

		var fieldSegments = SegmentsOf(tab, nameof(MemberHighlightSample.Field));
		var use = fieldSegments.First(r => !r.IsDefinition);
		view.OnReferenceClicked(use);

		view.LocalReferenceMarks.Should().BeEmpty(
			"with the setting off, a plain click must keep navigating instead of highlighting");
	}

	[AvaloniaTest]
	public async Task Clicking_A_Member_Highlights_All_Occurrences_When_Enabled()
	{
		var (_, view, tab) = await SetupAsync(highlightMemberReferences: true);

		var fieldSegments = SegmentsOf(tab, nameof(MemberHighlightSample.Field));
		// Besides the definition and the two uses, punctuation tokens of the declaration
		// carry the member reference as well; all of them belong to the highlight group.
		fieldSegments.Count(r => r.IsDefinition).Should().Be(1);
		fieldSegments.Should().HaveCountGreaterThanOrEqualTo(3, "the field has one definition and two uses");
		bool navigated = false;
		tab.NavigateRequested += _ => navigated = true;

		var use = fieldSegments.First(r => !r.IsDefinition);
		view.OnReferenceClicked(use);

		view.LocalReferenceMarks.Select(m => m.StartOffset).Should().BeEquivalentTo(
			fieldSegments.Select(s => s.StartOffset),
			"clicking a use must highlight the definition and every use");
		navigated.Should().BeFalse("a highlighting click must not navigate");
	}

	[AvaloniaTest]
	public async Task Highlight_Matches_Specialized_Member_Uses()
	{
		var (_, view, tab) = await SetupAsync(highlightMemberReferences: true);

		var makeSegments = SegmentsOf(tab, nameof(MemberHighlightSample.Make));
		// The identifier segments plus the call parentheses, which carry the member
		// reference as well.
		makeSegments.Count(r => r.IsDefinition).Should().Be(1);
		makeSegments.Should().HaveCountGreaterThanOrEqualTo(2, "the generic method has one definition and one call");
		// The call site carries a specialized method instance, the declaration the definition.
		var use = makeSegments.First(r => !r.IsDefinition && r.Length > 1);
		view.OnReferenceClicked(use);

		view.LocalReferenceMarks.Select(m => m.StartOffset).Should().BeEquivalentTo(
			makeSegments.Select(s => s.StartOffset),
			"clicking the specialized call site must also highlight the definition");
	}

	[AvaloniaTest]
	public async Task Clicking_A_Type_Highlights_All_Its_Parameterizations()
	{
		var (_, view, tab) = await SetupAsync(highlightMemberReferences: true);

		var listTypeSegments = tab.References!
			.Where(r => r.Kind == ReferenceMode.Link && r.Reference is IType { Name: "List" })
			.ToList();
		listTypeSegments.Should().HaveCountGreaterThanOrEqualTo(2,
			"List<T> and List<int> both occur in the view");

		view.OnReferenceClicked(listTypeSegments[0]);

		view.LocalReferenceMarks.Select(m => m.StartOffset).Should().BeEquivalentTo(
			listTypeSegments.Select(s => s.StartOffset),
			"clicking a type reference must highlight all its occurrences regardless of type arguments");
	}

	[AvaloniaTest]
	public async Task Ctrl_Click_Navigates_When_Enabled()
	{
		var (_, view, tab) = await SetupAsync(highlightMemberReferences: true);

		var fieldSegments = SegmentsOf(tab, nameof(MemberHighlightSample.Field));
		var use = fieldSegments.First(r => !r.IsDefinition);
		view.OnReferenceClicked(use, ctrlHeld: true);

		view.LocalReferenceMarks.Should().BeEmpty(
			"Ctrl+Click must navigate even with the setting enabled");
	}

	[AvaloniaTest]
	public async Task Pointer_Click_Highlights_And_Ctrl_Click_Navigates()
	{
		// Mirrors ReferenceClickTests.SetupAsync/FindVisibleReference: the System.String
		// view and its first visible link are the proven coordinate path for pointer
		// gestures; Stationary_Click_On_A_Link_Navigates pins that this very click
		// navigates when the setting is off.
		var (window, vm) = await TestHarness.BootAsync();
		AppComposition.Current.GetExport<SettingsService>().DisplaySettings.HighlightMemberReferences = true;
		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var stringNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(coreLibName, "System", "System.String");
		vm.AssemblyTreeModel.SelectNode(stringNode);
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		var view = window.GetVisualDescendants().OfType<DecompilerTextView>().First();
		AvaloniaHeadlessPlatform.ForceRenderTimerTick();
		Avalonia.Threading.Dispatcher.UIThread.RunJobs();
		window.UpdateLayout();

		var textView = view.Editor.TextArea.TextView;
		var segment = tab.References!
			.First(r => r.Reference != null && r.Kind == ReferenceMode.Link && !r.IsDefinition);
		var line = view.Editor.Document.GetLineByOffset(segment.StartOffset);
		view.Editor.ScrollTo(line.LineNumber, segment.StartOffset - line.Offset + 1);
		window.UpdateLayout();
		textView.EnsureVisualLines();
		var position = new TextViewPosition(line.LineNumber, segment.StartOffset - line.Offset + 2);
		var visual = textView.GetVisualPosition(position, VisualYPosition.LineMiddle) - textView.ScrollOffset;
		var point = textView.TranslatePoint(visual, window)!.Value;

		bool navigated = false;
		tab.NavigateRequested += _ => navigated = true;

		window.MouseDown(point, MouseButton.Left);
		window.MouseUp(point, MouseButton.Left);

		view.LocalReferenceMarks.Should().NotBeEmpty("a plain click must highlight, not navigate");
		navigated.Should().BeFalse();

		window.MouseDown(point, MouseButton.Left, RawInputModifiers.Control);
		window.MouseUp(point, MouseButton.Left, RawInputModifiers.Control);

		navigated.Should().BeTrue("Ctrl+Click must navigate to the definition");
	}

	[AvaloniaTest]
	public async Task Cursor_Shows_Hand_Only_When_A_Click_Would_Navigate()
	{
		var (window, vm) = await TestHarness.BootAsync();
		AppComposition.Current.GetExport<SettingsService>().DisplaySettings.HighlightMemberReferences = true;
		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var stringNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(coreLibName, "System", "System.String");
		vm.AssemblyTreeModel.SelectNode(stringNode);
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		var view = window.GetVisualDescendants().OfType<DecompilerTextView>().First();
		AvaloniaHeadlessPlatform.ForceRenderTimerTick();
		Avalonia.Threading.Dispatcher.UIThread.RunJobs();
		window.UpdateLayout();

		var textView = view.Editor.TextArea.TextView;
		var segment = tab.References!
			.First(r => r.Reference != null && r.Kind == ReferenceMode.Link && !r.IsDefinition);
		var line = view.Editor.Document.GetLineByOffset(segment.StartOffset);
		view.Editor.ScrollTo(line.LineNumber, segment.StartOffset - line.Offset + 1);
		window.UpdateLayout();
		textView.EnsureVisualLines();
		var position = new TextViewPosition(line.LineNumber, segment.StartOffset - line.Offset + 2);
		var visual = textView.GetVisualPosition(position, VisualYPosition.LineMiddle) - textView.ScrollOffset;
		var point = textView.TranslatePoint(visual, window)!.Value;

		// Without Ctrl a click highlights, so no hand cursor; with Ctrl it navigates.
		window.MouseMove(point);
		textView.Cursor?.ToString().Should().NotBe("Hand",
			"with the setting enabled, a plain click highlights instead of navigating");

		window.MouseMove(point, RawInputModifiers.Control);
		textView.Cursor?.ToString().Should().Be("Hand",
			"holding Ctrl makes the click navigate, so the link affordance must show");
	}

	[AvaloniaTest]
	public async Task Clicking_Empty_Space_Clears_The_Member_Highlight()
	{
		var (_, view, tab) = await SetupAsync(highlightMemberReferences: true);

		var use = SegmentsOf(tab, nameof(MemberHighlightSample.Field)).First(r => !r.IsDefinition);
		view.OnReferenceClicked(use);
		view.LocalReferenceMarks.Should().NotBeEmpty();

		// A subsequent navigating click (Ctrl) on another member clears the previous marks.
		view.OnReferenceClicked(use, ctrlHeld: true);
		view.LocalReferenceMarks.Should().BeEmpty();
	}
}
