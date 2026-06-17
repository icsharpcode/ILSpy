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

using AvaloniaEdit;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.TextView;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

// The line-highlight flash that plays alongside the gutter-icon pulse on a bookmark navigation.
// The animation curve / timing are eyeballed; this pins the register-then-unregister lifecycle.
[TestFixture]
public class LineHighlightAdornerTests
{
	[AvaloniaTest]
	public void DisplayLineHighlight_registers_and_Dismiss_unregisters()
	{
		var editor = new TextEditor { Document = new AvaloniaEdit.Document.TextDocument("line1\nline2\nline3") };
		var renderers = editor.TextArea.TextView.BackgroundRenderers;
		renderers.Should().NotContain(r => r is LineHighlightAdorner);

		LineHighlightAdorner.DisplayLineHighlight(editor.TextArea, 2);
		renderers.Should().Contain(r => r is LineHighlightAdorner, "DisplayLineHighlight adds the adorner");

		renderers.OfType<LineHighlightAdorner>().Single().Dismiss();
		renderers.Should().NotContain(r => r is LineHighlightAdorner, "Dismiss unregisters the adorner");
	}
}
