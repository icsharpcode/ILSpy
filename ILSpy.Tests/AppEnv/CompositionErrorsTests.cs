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

using ICSharpCode.Decompiler;

using ICSharpCode.ILSpy.AppEnv;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.AppEnv;

/// <summary>
/// Composition errors are collected non-fatally and can be rendered to an <see cref="ITextOutput"/>
/// for display to the user (the source label and the exception text both appear).
/// </summary>
[TestFixture]
public class CompositionErrorsTests
{
	// The sink is a process-global static. Leaving entries behind would make every later test that
	// boots MainWindow pop a "Composition Errors" tab, so clear it after each test here.
	[TearDown]
	public void TearDown() => CompositionErrors.Clear();

	[Test]
	public void Report_Collects_The_Error_And_WriteTo_Renders_It()
	{
		var marker = "Plugin 'Marker_" + Guid.NewGuid().ToString("N") + "'";
		CompositionErrors.Report(marker, new InvalidOperationException("boom-message"));

		CompositionErrors.Any.Should().BeTrue();

		var output = new PlainTextOutput();
		CompositionErrors.WriteTo(output);
		var text = output.ToString();

		text.Should().Contain(marker, "the failing part's label is shown");
		text.Should().Contain("boom-message", "the exception detail is shown");
	}
}
