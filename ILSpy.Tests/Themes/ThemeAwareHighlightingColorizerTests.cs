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

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.TextView;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Themes;

/// <summary>
/// Verifies the editor → colorizer wiring: <see cref="DecompilerTextEditor.CreateColorizer"/>
/// returns a <see cref="ThemeAwareHighlightingColorizer"/>, and the AvaloniaEdit
/// pipeline installs it into the text view's <c>LineTransformers</c> on
/// <c>SyntaxHighlighting</c> assignment. Without this seam the theme-aware colour math
/// in <c>ThemeManager</c> never reaches the rendered output.
/// </summary>
[TestFixture]
public class ThemeAwareHighlightingColorizerTests
{
	[AvaloniaTest]
	public void DecompilerTextEditor_Installs_ThemeAwareHighlightingColorizer_For_CSharp_Highlighting()
	{
		var editor = new DecompilerTextEditor();
		editor.SyntaxHighlighting = HighlightingService.GetByExtension(".cs");

		editor.TextArea.TextView.LineTransformers
			.Any(t => t is ThemeAwareHighlightingColorizer)
			.Should().BeTrue(
				"setting SyntaxHighlighting must route through CreateColorizer, which produces the theme-aware variant");
	}

	[AvaloniaTest]
	public void DecompilerTextEditor_Replaces_Old_Colorizer_When_SyntaxHighlighting_Changes()
	{
		// AvaloniaEdit's setter removes the previous CreateColorizer-produced transformer
		// before installing the new one. If that contract regresses (older theme-aware
		// colorizers leaking onto every navigation), the editor would slowly accumulate
		// colorizers and render colour decisions from stale highlighting definitions.
		var editor = new DecompilerTextEditor();
		editor.SyntaxHighlighting = HighlightingService.GetByExtension(".cs");
		editor.SyntaxHighlighting = HighlightingService.GetByExtension(".xml");

		editor.TextArea.TextView.LineTransformers
			.Count(t => t is ThemeAwareHighlightingColorizer)
			.Should().Be(1, "exactly one theme-aware colorizer is alive at a time");
	}
}
