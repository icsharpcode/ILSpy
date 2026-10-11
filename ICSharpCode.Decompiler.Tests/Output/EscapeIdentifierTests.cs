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

using ICSharpCode.Decompiler.CSharp.OutputVisitor;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.Output
{
	[TestFixture]
	public class EscapeIdentifierTests
	{
		[Test]
		public void PlainIdentifierIsReturnedAsTheSameInstance()
		{
			// The overwhelmingly common case must not allocate at all.
			string identifier = "MyIdentifier_42";
			Assert.That(TextWriterTokenWriter.EscapeIdentifier(identifier), Is.SameAs(identifier));
		}

		[Test]
		public void EmptyAndNullAreReturnedUnchanged()
		{
			Assert.That(TextWriterTokenWriter.EscapeIdentifier(""), Is.EqualTo(""));
			Assert.That(TextWriterTokenWriter.EscapeIdentifier(null), Is.Null);
		}

		[Test]
		public void ControlCharIsEscaped()
		{
			Assert.That(TextWriterTokenWriter.EscapeIdentifier("a\u0001b"), Is.EqualTo(@"a\u0001b"));
		}

		[Test]
		public void BackslashIsEscaped()
		{
			Assert.That(TextWriterTokenWriter.EscapeIdentifier("a\\b"), Is.EqualTo(@"a\u005cb"));
		}

		[Test]
		public void PrintableSurrogatePairPassesThroughUnchanged()
		{
			// U+1D49C (MATHEMATICAL SCRIPT CAPITAL A) is a letter, i.e. printable.
			Assert.That(TextWriterTokenWriter.EscapeIdentifier("a\U0001D49Cb"), Is.EqualTo("a\U0001D49Cb"));
		}

		[Test]
		public void NonPrintableSurrogatePairIsEscapedAsUtf32()
		{
			// U+1D173 (MUSICAL SYMBOL BEGIN BEAM) is a format char, i.e. non-printable.
			Assert.That(TextWriterTokenWriter.EscapeIdentifier("a\U0001D173b"), Is.EqualTo(@"a\U0001d173b"));
		}
	}
}
