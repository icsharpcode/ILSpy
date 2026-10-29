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

using ICSharpCode.ILSpy.Util;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Windows;

/// <summary>
/// Verifies the NONCLIENTMETRICS message-font reader used to follow the Windows system UI font.
/// The exact values depend on the machine (the accessibility "Text size" setting scales them),
/// so the assertions only pin down what must hold everywhere: a face name is present and the
/// size is a sane DIP value (metrics are requested at 96 DPI, so 12 on a default install,
/// larger when text scaling is active).
/// </summary>
[TestFixture]
public class WindowsSystemFontTests
{
	[Test]
	public void MessageFontIsReadable()
	{
		bool available = WindowsSystemFont.TryGetMessageFont(out var faceName, out var fontSize);

		Assert.That(available, Is.True);
		Assert.That(faceName, Is.Not.Null.And.Not.Empty);
		Assert.That(fontSize, Is.InRange(9.0, 48.0));
	}
}
