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

using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Input;
using Avalonia.VisualTree;

using AvaloniaEdit.Editing;

using AwesomeAssertions;

using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Editor;

/// <summary>
/// The decompiled view is read-only, so Tab has nothing to indent; it must move the keyboard
/// focus out of the editor like it does for every other control (WPF removed the editor's
/// TabForward/TabBackward commands for the same reason).
/// </summary>
[TestFixture]
public class TabFocusNavigationTests
{
	static async Task<(ICSharpCode.ILSpy.Views.MainWindow Window, TextArea TextArea)> FocusEditorAsync()
	{
		var (window, vm) = await TestHarness.BootAsync();
		var coreLibName = typeof(object).Assembly.GetName().Name!;
		vm.AssemblyTreeModel.SelectNode(vm.AssemblyTreeModel.FindNode<TypeTreeNode>(coreLibName, "System", "System.String"));
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		var textArea = window.GetVisualDescendants().OfType<DecompilerTextView>().First().Editor.TextArea;
		textArea.Focus();
		Avalonia.Threading.Dispatcher.UIThread.RunJobs();
		var focused = TopLevel.GetTopLevel(window)!.FocusManager!.GetFocusedElement();
		focused.Should().BeSameAs(textArea, "precondition: the text area must own the keyboard focus");
		return (window, textArea);
	}

	[AvaloniaTest]
	public Task Tab_Moves_The_Keyboard_Focus_Out_Of_The_Editor()
		=> AssertTabLeavesTheEditorAsync(RawInputModifiers.None);

	[AvaloniaTest]
	public Task Shift_Tab_Moves_The_Keyboard_Focus_Out_Of_The_Editor()
		=> AssertTabLeavesTheEditorAsync(RawInputModifiers.Shift);

	static async Task AssertTabLeavesTheEditorAsync(RawInputModifiers modifiers)
	{
		var (window, textArea) = await FocusEditorAsync();

		window.KeyPress(Key.Tab, modifiers, PhysicalKey.Tab, null);
		Avalonia.Threading.Dispatcher.UIThread.RunJobs();

		var focused = TopLevel.GetTopLevel(window)!.FocusManager!.GetFocusedElement();
		focused.Should().NotBeNull("Tab must hand the focus to another control, not drop it");
		focused.Should().NotBeSameAs(textArea, "Tab must leave the read-only editor");
		((Avalonia.Visual)focused!).GetVisualAncestors().Should().NotContain(textArea,
			"Tab must leave the editor entirely, not land on one of its parts");
	}
}
