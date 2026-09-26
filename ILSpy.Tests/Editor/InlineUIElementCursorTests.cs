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
using Avalonia.Headless.NUnit;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.ILSpy.TextView;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class InlineUIElementCursorTests
{
	// Controls embedded in the text via ISmartTextOutput.AddUIElement are visual children of the
	// AvaloniaEdit TextView, so they inherit the text area's I-beam unless they carry their own
	// cursor. They are widgets (buttons, toggles, images), not text, so they must show the arrow.
	// The About page's update section is the canonical example: a button and a checkbox.
	[AvaloniaTest]
	public async Task Inline_Controls_Use_The_Arrow_Cursor_Not_The_Editors_I_Beam()
	{
		var (window, vm) = await TestHarness.BootAsync(1);
		await Waiters.WaitForAsync(() => vm.DockWorkspace.IsWelcomePageVisible,
			description: "the startup welcome page (About) to be showing");
		var view = await window.WaitForComponent<DecompilerTextView>();
		window.UpdateLayout();

		var textView = view.Editor.TextArea.TextView;
		await Waiters.WaitForAsync(() => textView.GetVisualDescendants().OfType<CheckBox>().Any(),
			description: "the About page's inline update section to be realised");

		var checkBox = textView.GetVisualDescendants().OfType<CheckBox>().Should().ContainSingle().Subject;
		// CheckBox derives from Button, so match the exact type to get the update button alone.
		var button = textView.GetVisualDescendants().Where(v => v.GetType() == typeof(Button)).Cast<Button>()
			.Should().ContainSingle().Subject;

		checkBox.Cursor.Should().BeSameAs(DecompilerTextView.ArrowCursor,
			"the auto-update checkbox is a widget, so it must show the arrow rather than the text I-beam");
		button.Cursor.Should().BeSameAs(DecompilerTextView.ArrowCursor,
			"the update button is a widget, so it must show the arrow rather than the text I-beam");

		view.Editor.TextArea.Cursor.Should().NotBeSameAs(DecompilerTextView.ArrowCursor,
			"the text itself keeps the editor's I-beam");
	}
}
