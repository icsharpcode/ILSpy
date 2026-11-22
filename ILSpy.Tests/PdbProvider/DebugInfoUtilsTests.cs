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

using System;
using System.IO;
using System.Text;

using ICSharpCode.ILSpyX.PdbProvider;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.PdbProvider;

[TestFixture]
public class DebugInfoUtilsTests
{
	[Test]
	public void IsWindowsPdbHandlesShortReads()
	{
		byte[] signature = Encoding.ASCII.GetBytes("Microsoft C/C++ MSF 7.00");
		using var stream = new ShortReadStream(signature);

		Assert.That(DebugInfoUtils.IsWindowsPdb(stream), Is.True);
		Assert.That(stream.Position, Is.EqualTo(signature.Length));
	}

	sealed class ShortReadStream(byte[] data) : MemoryStream(data)
	{
		public override int Read(byte[] buffer, int offset, int count)
		{
			return base.Read(buffer, offset, Math.Min(count, 1));
		}
	}
}
