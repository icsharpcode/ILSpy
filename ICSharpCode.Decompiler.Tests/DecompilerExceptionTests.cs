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

using ICSharpCode.Decompiler.Metadata;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests
{
	/// <summary>
	/// DecompilerException prints its own stack trace format (type, method, parameters, file name
	/// and line, inner exceptions first). Bug reports depend on the throwing method being named,
	/// so the format is pinned here for the managed runtime; the Native AOT gate
	/// (ICSharpCode.Decompiler.AotTest) checks the same for frames without reflection metadata.
	/// </summary>
	[TestFixture]
	public class DecompilerExceptionTests
	{
		static void ThrowFromHelper()
		{
			throw new InvalidOperationException("inner failure");
		}

		static DecompilerException WrapHelperException()
		{
			try
			{
				ThrowFromHelper();
			}
			catch (Exception ex)
			{
				using var file = new PEFile(typeof(DecompilerExceptionTests).Assembly.Location);
				return new DecompilerException(file, "outer failure", ex);
			}
			throw new InvalidOperationException("unreachable");
		}

		[Test]
		public void StackTraceNamesTheThrowingMethodAndItsSourceLocation()
		{
			string text = WrapHelperException().ToString();

			using (Assert.EnterMultipleScope())
			{
				Assert.That(text, Does.StartWith("outer failure"));
				Assert.That(text, Does.Contain("System.InvalidOperationException: inner failure"));
				Assert.That(text, Does.Contain("   at ICSharpCode.Decompiler.Tests.DecompilerExceptionTests.ThrowFromHelper()"));
				Assert.That(text, Does.Contain(" in DecompilerExceptionTests.cs:line "), "file name without path, with line number");
				Assert.That(text, Does.Contain("-- continuing with outer exception (ICSharpCode.Decompiler.DecompilerException) --"));
			}
		}

		static void ThrowWrapped()
		{
			throw WrapHelperException();
		}

		[Test]
		public void StackTracePropertyUsesTheSameFormat()
		{
			try
			{
				ThrowWrapped();
			}
			catch (DecompilerException ex)
			{
				Assert.That(ex.StackTrace, Does.Contain("   at ICSharpCode.Decompiler.Tests.DecompilerExceptionTests.ThrowWrapped()"));
				return;
			}
			Assert.Fail("expected a DecompilerException");
		}
	}
}
