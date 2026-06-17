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

using System.Collections.Generic;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Bookmarks;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Bookmarks;

// The IL-offset <-> line map is the heart of the in-method (body) anchor. These tests pin the
// line->offset (save) and offset->line (display/navigate) round trip, including the "nearest
// statement at or before the offset" fallback that lets a bookmark survive a reflow.
[TestFixture]
public class MethodDebugInfoTests
{
	const uint Token = 0x06000001;

	static MethodDebugInfo Method() => new(
		Token, @"C:\asm\Sample.dll", "Sample", "Sample.dll", "Sample.Type.M",
		new List<(int Line, int ILOffset)> { (5, 0), (6, 8), (7, 16) });

	[Test]
	public void Line_maps_to_the_statement_offset()
	{
		Method().TryGetOffsetForLine(6, out var offset).Should().BeTrue();
		offset.Should().Be(8);
	}

	[Test]
	public void A_line_without_a_statement_has_no_offset()
	{
		Method().TryGetOffsetForLine(99, out _).Should().BeFalse();
	}

	[Test]
	public void Exact_offset_maps_back_to_its_line()
	{
		Method().TryGetLineForOffset(8, out var line).Should().BeTrue();
		line.Should().Be(6);
	}

	[Test]
	public void Offset_between_statements_snaps_to_the_one_before()
	{
		Method().TryGetLineForOffset(12, out var line).Should().BeTrue();
		line.Should().Be(6);
	}

	[Test]
	public void Document_resolves_a_body_anchor_and_back()
	{
		var info = new DecompiledDebugInfo(new List<MethodDebugInfo> { Method() });

		info.TryGetBodyAnchor(7, out var method, out var offset).Should().BeTrue();
		method.Token.Should().Be(Token);
		offset.Should().Be(16);

		info.TryGetLine(Token, offset, out var line).Should().BeTrue();
		line.Should().Be(7);
	}

	[Test]
	public void Document_has_no_body_anchor_off_any_statement()
	{
		var info = new DecompiledDebugInfo(new List<MethodDebugInfo> { Method() });
		info.TryGetBodyAnchor(1, out _, out _).Should().BeFalse();
	}
}
