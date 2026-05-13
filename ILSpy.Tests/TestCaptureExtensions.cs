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
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Threading;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// Deterministic-path screenshot helpers for the test-review workflow. Unlike
/// <see cref="WindowExtensions.CaptureAndShow"/> these don't open the OS image viewer —
/// they write to <c>%TEMP%/ilspy-test-captures/</c> with a filename predictable from
/// the (testName, step, label) triple so the audit workflow can read them back for
/// in-chat review. The <c>ILSPY_TEST_CAPTURES</c> environment variable overrides the
/// directory when a specific machine wants captures elsewhere. Only renders when
/// <c>ILSPY_TESTS_VISIBLE=1</c> (otherwise the headless platform returns a null frame
/// and the call is a no-op).
/// </summary>
public static class TestCaptureExtensions
{
	static readonly string OutputDirectory = Environment.GetEnvironmentVariable("ILSPY_TEST_CAPTURES")
		?? Path.Combine(Path.GetTempPath(), "ilspy-test-captures");

	public static void CaptureForReview(
		this Window window,
		int step,
		string label,
		[CallerMemberName] string? testName = null)
	{
		ArgumentNullException.ThrowIfNull(window);
		Dispatcher.UIThread.RunJobs();
		var frame = window.CaptureRenderedFrame();
		if (frame is null)
			return;

		Directory.CreateDirectory(OutputDirectory);
		var fileName = $"{Sanitize(testName)}-{step:D2}-{Sanitize(label)}.png";
		var path = Path.Combine(OutputDirectory, fileName);
		frame.Save(path);
	}

	static string Sanitize(string? label)
	{
		if (string.IsNullOrWhiteSpace(label))
			return "frame";
		return new string(label.Select(c => char.IsLetterOrDigit(c) ? c : '_').ToArray());
	}
}
