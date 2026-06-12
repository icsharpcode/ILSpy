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

using System;

using AwesomeAssertions;

using ICSharpCode.ILSpy.TextView;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.TextView;

/// <summary>
/// The decompiler text output stops once it exceeds its LengthLimit, so a runaway decompile (a huge
/// type/namespace) can't hang or OOM the UI thread. (Regression: the Avalonia port had no limit.)
/// </summary>
[TestFixture]
public class OutputLengthLimitTests
{
	[Test]
	public void Write_Throws_Once_The_Length_Limit_Is_Exceeded()
	{
		var output = new AvaloniaEditTextOutput { LengthLimit = 10 };

		var act = () => {
			for (int i = 0; i < 100; i++)
				output.Write("0123456789");
		};

		act.Should().Throw<OutputLengthExceededException>();
	}

	[Test]
	public void Write_Does_Not_Throw_Below_The_Limit()
	{
		var output = new AvaloniaEditTextOutput { LengthLimit = 1000 };

		var act = () => {
			output.Write("short output");
			output.WriteLine();
		};

		act.Should().NotThrow();
	}

	[Test]
	public void Default_Limit_Is_Unlimited()
	{
		var output = new AvaloniaEditTextOutput();
		output.LengthLimit.Should().Be(int.MaxValue);
	}
}
