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
using System.Diagnostics;

using Avalonia;
using Avalonia.Headless;

using ICSharpCode.ILSpy.Tests;

[assembly: AvaloniaTestApplication(typeof(TestAppBuilder))]
[assembly: AvaloniaTestIsolation(AvaloniaTestIsolationLevel.PerAssembly)]
[assembly: ResetAppState]
[assembly: CaptureContext]

namespace ICSharpCode.ILSpy.Tests;

public static class TestAppBuilder
{
	// Visible mode (debugger attached or ILSPY_TESTS_VISIBLE=1) flips Skia drawing on so
	// CaptureAndShow can save a real PNG; otherwise headless drawing keeps tests cheap.
	public static AppBuilder BuildAvaloniaApp()
	{
		var visible = Debugger.IsAttached
			|| string.Equals(Environment.GetEnvironmentVariable("ILSPY_TESTS_VISIBLE"), "1", StringComparison.Ordinal);
		return AppBuilder.Configure<TestApp>()
			.UseSkia()
			.UseHeadless(new AvaloniaHeadlessPlatformOptions {
				UseHeadlessDrawing = !visible,
			});
	}
}
