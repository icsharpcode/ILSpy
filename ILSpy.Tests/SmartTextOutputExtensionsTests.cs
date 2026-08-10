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

using AwesomeAssertions;

using ICSharpCode.Decompiler;

using ICSharpCode.ILSpy.TextView;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class SmartTextOutputExtensionsTests
{
	/// <summary>
	/// A collapsed fold hides everything up to its end offset, so a fold reaching past the last
	/// frame takes the line after it - the one the reader needs to see - down with it.
	/// </summary>
	[Test]
	public void Exception_Fold_Stops_At_The_Last_Frame()
	{
		var output = new AvaloniaEditTextOutput();
		output.Write("Something failed:");
		output.WriteLine();
		output.WriteExceptionDetails(ExceptionWithTrace());
		output.Write("this line must stay visible");
		output.WriteLine();

		string text = output.GetText();
		var fold = output.Foldings.Should().ContainSingle().Subject;
		text[fold.StartOffset..fold.EndOffset].Should().NotEndWith("\n",
			"the fold must end with the last frame, not with the newline behind it");
		text.Should().Contain("this line must stay visible");
	}

	static Exception ExceptionWithTrace() => new TrailingNewlineException();

	/// <summary>
	/// Stands in for the exceptions that actually reach this helper: <see cref="DecompilerException"/>
	/// renders a trailing newline, which would push the fold one line past the last frame.
	/// </summary>
	sealed class TrailingNewlineException : Exception
	{
		public override string ToString() => "boom" + Environment.NewLine + "   at Frame1" + Environment.NewLine;
	}
}
