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

using System;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;

using AwesomeAssertions;

using ICSharpCode.ILSpy.TextView;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Themes;

/// <summary>
/// The About page and every other hyperlink in a decompiler view are AvaloniaEdit
/// <c>VisualLineLinkText</c> runs, which take their color from
/// <c>TextView.LinkTextForegroundBrush</c>. AvaloniaEdit's registered default is pure blue,
/// 1.9:1 against the dark editor canvas, so App.axaml restyles the property through
/// <c>ILSpy.LinkForeground</c>. Verified here because a typo in the selector or the xmlns
/// would silently fall back to that unreadable default.
/// </summary>
[TestFixture]
public class LinkColorTests
{
	[AvaloniaTest]
	public void Editor_TextView_Takes_Its_Link_Color_From_The_Theme()
	{
		var textView = HostedTextView();

		BrushColor(textView.LinkTextForegroundBrush)
			.Should().NotBe(Colors.Blue, "the App.axaml style must replace AvaloniaEdit's default link brush");
	}

	[AvaloniaTest]
	public void Link_Color_Follows_The_Theme_Variant()
	{
		var app = Application.Current ?? throw new InvalidOperationException("no Application");
		var previous = app.RequestedThemeVariant;
		try
		{
			var textView = HostedTextView();

			app.RequestedThemeVariant = ThemeVariant.Light;
			Dispatcher.UIThread.RunJobs();
			var light = BrushColor(textView.LinkTextForegroundBrush);

			app.RequestedThemeVariant = ThemeVariant.Dark;
			Dispatcher.UIThread.RunJobs();
			var dark = BrushColor(textView.LinkTextForegroundBrush);

			dark.Should().NotBe(light, "each theme dictionary defines its own ILSpy.LinkForeground");
		}
		finally
		{
			app.RequestedThemeVariant = previous;
		}
	}

	// Application-level styles only apply once the control is attached to a TopLevel.
	static AvaloniaEdit.Rendering.TextView HostedTextView()
	{
		var editor = new DecompilerTextEditor();
		var window = new Window { Content = editor, Width = 400, Height = 300 };
		window.Show();
		Dispatcher.UIThread.RunJobs();
		return editor.TextArea.TextView;
	}

	static Color BrushColor(IBrush? brush)
		=> (brush as ISolidColorBrush)?.Color ?? throw new InvalidOperationException("not a solid color brush");
}
