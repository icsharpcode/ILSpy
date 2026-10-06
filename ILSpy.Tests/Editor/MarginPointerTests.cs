// Copyright (c) 2026 Christoph Wille
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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.Input;
using Avalonia.VisualTree;

using AvaloniaEdit.Editing;
using AvaloniaEdit.Folding;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Options;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Editor;

/// <summary>
/// The gutter margins react to the left button only, as they did in WPF: AvaloniaEdit's line-number
/// margin otherwise selects the line and a fold marker toggles on any button, so a middle press
/// (which opens references in a new tab over the text) would also move the caret or fold code when
/// it lands on the gutter.
/// </summary>
[TestFixture]
public class MarginPointerTests
{
	static async Task<(ICSharpCode.ILSpy.Views.MainWindow Window, DecompilerTextView View)> SetupAsync()
	{
		var (window, vm) = await TestHarness.BootAsync(1);
		AppComposition.Current.GetExport<SettingsService>().DisplaySettings.ShowLineNumbers = true;

		// A type (not a single method) so the decompiled body carries foldings and the folding margin
		// is installed.
		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var objectNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(coreLibName, "System", "System.Object");
		vm.AssemblyTreeModel.SelectNode(objectNode);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		var view = await window.WaitForComponent<DecompilerTextView>();
		await Waiters.WaitForAsync(() => view.Editor.TextArea.LeftMargins.OfType<FoldingMargin>().Any(),
			description: "the folding margin to be installed for the decompiled type");
		await Waiters.WaitForIdleAsync();
		return (window, view);
	}

	[AvaloniaTest]
	public async Task Middle_Press_On_The_Line_Number_Margin_Does_Not_Select_The_Line()
	{
		// The line-number margin draws text only, which the headless hit test does not see, so the
		// press is raised on the margin directly; it still tunnels down from the window through the
		// text area, which is where the margins' button filter sits.
		var (window, view) = await SetupAsync();
		var margin = view.Editor.TextArea.LeftMargins.OfType<LineNumberMargin>().Single();
		var caretBefore = view.Editor.CaretOffset;

		margin.RaiseEvent(MiddlePress(window, margin, new Point(margin.Bounds.Width / 2, margin.Bounds.Height / 4)));
		await Waiters.WaitForIdleAsync();

		view.Editor.TextArea.Selection.IsEmpty.Should().BeTrue("a middle press on the line numbers must not select the line");
		view.Editor.CaretOffset.Should().Be(caretBefore, "a middle press on the line numbers must not move the caret");

		margin.RaiseEvent(LeftPress(window, margin, new Point(margin.Bounds.Width / 2, margin.Bounds.Height / 4)));
		await Waiters.WaitForIdleAsync();
		view.Editor.CaretOffset.Should().NotBe(caretBefore, "the same press with the left button moves the caret, so the middle press really reached the margin");
	}

	static PointerPressedEventArgs MiddlePress(Window window, Control target, Avalonia.Point position)
		=> Press(window, target, position, RawInputModifiers.MiddleMouseButton, PointerUpdateKind.MiddleButtonPressed);

	static PointerPressedEventArgs LeftPress(Window window, Control target, Avalonia.Point position)
		=> Press(window, target, position, RawInputModifiers.LeftMouseButton, PointerUpdateKind.LeftButtonPressed);

	static PointerPressedEventArgs Press(Window window, Control target, Avalonia.Point position,
		RawInputModifiers button, PointerUpdateKind kind)
		=> new(target, new Pointer(Pointer.GetNextFreeId(), PointerType.Mouse, isPrimary: true), window,
			target.TranslatePoint(position, window)!.Value, 0, new PointerPointProperties(button, kind), KeyModifiers.None);

	[AvaloniaTest]
	public async Task Middle_Press_On_A_Fold_Marker_Does_Not_Toggle_The_Fold()
	{
		var (window, view) = await SetupAsync();
		var foldingMargin = view.Editor.TextArea.LeftMargins.OfType<FoldingMargin>().Single();
		Control? Marker() => foldingMargin.GetVisualChildren().OfType<Control>().FirstOrDefault(c => c.IsVisible);
		Marker().Should().NotBeNull("the folding margin must show a marker for the decompiled type");
		var foldedBefore = view.FoldedFoldingCount;

		await window.ClickAsync(Marker, MouseButton.Middle);
		await Waiters.WaitForIdleAsync();
		view.FoldedFoldingCount.Should().Be(foldedBefore, "a middle press on a fold marker must not toggle the fold");

		// The same marker toggles on a left click, so the middle press above really reached a marker.
		await window.ClickAsync(Marker, MouseButton.Left);
		await Waiters.WaitForAsync(() => view.FoldedFoldingCount != foldedBefore,
			description: "a left click on the fold marker to toggle the fold");
	}
}
