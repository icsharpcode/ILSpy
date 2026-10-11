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
/// Pins reference click navigation: plain clicks navigate by default, and the display setting
/// can require Ctrl+Click for navigation.
/// </summary>
[TestFixture]
public class MemberReferenceHighlightTests
{
	static async Task<(MainWindow Window, DecompilerTextView View, DecompilerTabPageModel Tab)> SetupAsync(bool navigateOnCtrlClick)
	{
		var (window, vm) = await TestHarness.BootAsync();
		AppComposition.Current.GetExport<SettingsService>().DisplaySettings.NavigateOnCtrlClick = navigateOnCtrlClick;
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
	public async Task Clicking_A_Member_Navigates_By_Default()
	{
		var (_, view, tab) = await SetupAsync(navigateOnCtrlClick: false);

		var fieldSegments = SegmentsOf(tab, nameof(MemberHighlightSample.Field));
		var definition = fieldSegments.First(r => r.IsDefinition);
		var use = fieldSegments.First(r => !r.IsDefinition);
		view.OnReferenceClicked(use);

		view.LocalReferenceMarks.Should().BeEmpty(
			"plain clicks no longer create member reference highlights");
		view.Editor.TextArea.Caret.Offset.Should().BeInRange(definition.StartOffset, definition.EndOffset,
			"plain clicks navigate to an in-document definition by default");
	}

	[AvaloniaTest]
	public async Task Plain_Click_Does_Not_Navigate_When_Ctrl_Click_Is_Required()
	{
		var (_, view, tab) = await SetupAsync(navigateOnCtrlClick: true);

		var fieldSegments = SegmentsOf(tab, nameof(MemberHighlightSample.Field));
		var originalOffset = view.Editor.TextArea.Caret.Offset;
		var use = fieldSegments.First(r => !r.IsDefinition);
		view.OnReferenceClicked(use);

		view.Editor.TextArea.Caret.Offset.Should().Be(originalOffset);
		view.LocalReferenceMarks.Should().BeEmpty("plain clicks do not create member reference highlights");
	}

	[AvaloniaTest]
	public async Task Ctrl_Click_Navigates_When_Ctrl_Click_Is_Required()
	{
		var (_, view, tab) = await SetupAsync(navigateOnCtrlClick: true);

		var fieldSegments = SegmentsOf(tab, nameof(MemberHighlightSample.Field));
		var definition = fieldSegments.First(r => r.IsDefinition);
		var use = fieldSegments.First(r => !r.IsDefinition);
		view.OnReferenceClicked(use, DecompilerTextView.ReferenceClickKind.Navigate);

		view.LocalReferenceMarks.Should().BeEmpty(
			"Ctrl+Click navigates instead of highlighting");
		view.Editor.TextArea.Caret.Offset.Should().BeInRange(definition.StartOffset, definition.EndOffset);
	}

	[AvaloniaTest]
	public async Task Pointer_Click_Respects_Ctrl_Click_Requirement()
	{
		// Mirrors ReferenceClickTests.SetupAsync/FindVisibleReference: the System.String
		// view and its first visible link are the proven coordinate path for pointer
		// gestures.
		var (window, vm) = await TestHarness.BootAsync();
		AppComposition.Current.GetExport<SettingsService>().DisplaySettings.NavigateOnCtrlClick = true;
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
		tab.NavigateRequested += (_, _) => navigated = true;

		window.MouseDown(point, MouseButton.Left);
		window.MouseUp(point, MouseButton.Left);

		view.LocalReferenceMarks.Should().BeEmpty("plain clicks do not create member reference highlights");
		navigated.Should().BeFalse("plain clicks do not navigate when Ctrl+Click is required");

		window.MouseDown(point, MouseButton.Left, RawInputModifiers.Control);
		window.MouseUp(point, MouseButton.Left, RawInputModifiers.Control);

		navigated.Should().BeTrue("Ctrl+Click must navigate");
	}

	[AvaloniaTest]
	public async Task Cursor_Shows_Hand_Only_When_Click_Would_Navigate()
	{
		var (window, vm) = await TestHarness.BootAsync();
		AppComposition.Current.GetExport<SettingsService>().DisplaySettings.NavigateOnCtrlClick = true;
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

		// Without Ctrl a click does not navigate, so no hand cursor; with Ctrl it navigates.
		window.MouseMove(point);
		textView.Cursor?.ToString().Should().NotBe("Hand",
			"with the setting enabled, a plain click does not navigate");

		window.MouseMove(point, RawInputModifiers.Control);
		textView.Cursor?.ToString().Should().Be("Hand",
			"holding Ctrl makes the click navigate, so the link affordance must show");
	}
}
