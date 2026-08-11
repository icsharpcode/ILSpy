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

using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.NUnit;
using Avalonia.LogicalTree;
using Avalonia.Media;

using AvaloniaEdit.Highlighting;

using AwesomeAssertions;

using ICSharpCode.Decompiler.CSharp.OutputVisitor;

using ICSharpCode.ILSpy.TextView;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.TextView;

/// <summary>
/// Pins the rendering shape of method-signature tooltips so a long signature isn't clipped
/// horizontally: the signature <see cref="SelectableTextBlock"/> must wrap. Created with
/// <see cref="TextWrapping.NoWrap"/>, text past the popup's MaxWidth is simply cut off,
/// because the outer ScrollViewer disables horizontal scrolling. WPF parity is
/// wrap-friendly, so we mirror that.
/// </summary>
[TestFixture]
public class DocumentationRendererTooltipWidthTests
{
	[AvaloniaTest]
	public void Signature_Block_Wraps_So_Long_Method_Signatures_Are_Not_Clipped()
	{
		var renderer = new DocumentationRenderer(
			new CSharpAmbience(),
			new FontFamily("Consolas, Menlo, Monospace"),
			12);

		// A real C# Zip overload — long enough that it exceeds the previous 600px cap at
		// the popup's monospace 12pt size. The bug shows up on any method whose rendered
		// signature is wider than the popup's outer MaxWidth.
		var longSignature = new RichText(
			"public static IEnumerable<TResult> Zip<TFirst, TSecond, TThird, TResult>("
			+ "this IEnumerable<TFirst> first, IEnumerable<TSecond> second, "
			+ "IEnumerable<TThird> third, "
			+ "Func<TFirst, TSecond, TThird, TResult> resultSelector)");

		renderer.AddSignatureBlock(longSignature);

		var view = renderer.CreateView();
		var signature = view.GetLogicalDescendants()
			.OfType<SelectableTextBlock>()
			.FirstOrDefault();
		signature.Should().NotBeNull(
			"DocumentationRenderer must materialise the signature as a SelectableTextBlock");
		signature!.TextWrapping.Should().Be(TextWrapping.Wrap,
			"the signature must wrap inside the popup — without wrapping a long signature is "
			+ "clipped at the outer MaxWidth because the ScrollViewer disables horizontal scrolling");
	}
}
