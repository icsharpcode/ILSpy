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
using System.IO;
using System.Linq;

using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Threading;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Views;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// Visual breakpoints for the headless suite. A <see cref="Step(string)"/> call snapshots the
/// live UI and writes it to
/// <c>&lt;captures&gt;/&lt;TestFixtureName&gt;/&lt;TestName&gt;_&lt;NN&gt;_&lt;ShortDescription&gt;.png</c>,
/// where the fixture and test names come from NUnit's <see cref="TestContext"/> and <c>NN</c> is a
/// per-test step counter that auto-increments (so inserting a breakpoint never renumbers the rest).
/// <para>
/// Drop <c>TestCapture.Step("selected-the-row")</c> after each action and before each assertion to
/// build a frame-by-frame filmstrip of a test, then flip <c>ILSPY_TESTS_VISIBLE=1</c> to render them
/// and eyeball whether the test exercises what it claims. Without that variable the headless platform
/// returns a null frame and every call is a no-op, so instrumented tests cost nothing in CI.
/// </para>
/// <para>
/// The capture root is <c>%TEMP%/ilspy-test-captures/</c>, overridable via the
/// <c>ILSPY_TEST_CAPTURES</c> environment variable.
/// </para>
/// </summary>
public static class TestCapture
{
	static readonly string OutputDirectory = Environment.GetEnvironmentVariable("ILSPY_TEST_CAPTURES")
		?? Path.Combine(Path.GetTempPath(), "ilspy-test-captures");

	// Rendering is only on when a debugger is attached or ILSPY_TESTS_VISIBLE=1 (see TestAppBuilder).
	// When it's off, a breakpoint must be a TRUE no-op -- not even Dispatcher.RunJobs() -- so that
	// instrumenting a test cannot perturb the timing of navigation/tab state it asserts on.
	static readonly bool Enabled = Debugger.IsAttached
		|| string.Equals(Environment.GetEnvironmentVariable("ILSPY_TESTS_VISIBLE"), "1", StringComparison.Ordinal);

	// The active test's identity, recorded by CaptureContextAttribute.BeforeTest from the real
	// ITest. We can't read TestContext.CurrentContext live during the test body: the NUnit
	// execution context doesn't reliably flow onto async continuations, so captures after an await
	// would otherwise fall back to NUnit's ad-hoc context and collide under one filename.
	// The dispatcher thread is single under AvaloniaTestIsolationLevel.PerAssembly, so the suite
	// runs tests serially on it -- plain static fields are safe (no cross-test races).
	static string fixtureName = "UnknownFixture";
	static string testName = "UnknownTest";
	static int step;

	/// <summary>
	/// Records the running test's identity and resets the step counter. Called by
	/// <see cref="CaptureContextAttribute"/> before each test, off the real <c>ITest</c>.
	/// </summary>
	internal static void BeginTest(string? className, string? name)
	{
		fixtureName = ShortName(className);
		testName = Sanitize(name);
		step = 0;
	}

	/// <summary>
	/// Snapshots the shared <see cref="MainWindow"/> as the next numbered step of the running test.
	/// The window is resolved from the MEF container, so callers that discarded their window handle
	/// (e.g. <c>var (_, vm) = await TestHarness.BootAsync()</c>) can still drop a breakpoint.
	/// </summary>
	public static void Step(string description)
	{
		if (!Enabled)
			return;
		var window = TryGetMainWindow();
		if (window is not null)
			window.Capture(description);
	}

	/// <summary>
	/// Snapshots a specific <paramref name="window"/> (use when the frame of interest is a dialog or
	/// secondary window rather than the main one) as the next numbered step of the running test.
	/// </summary>
	public static void Capture(this Window window, string description)
	{
		ArgumentNullException.ThrowIfNull(window);
		if (!Enabled)
			return;
		Dispatcher.UIThread.RunJobs();
		var frame = window.CaptureRenderedFrame();
		if (frame is null)
			return; // headless platform isn't rendering (ILSPY_TESTS_VISIBLE != 1) -> no-op.

		var directory = Path.Combine(OutputDirectory, fixtureName);
		Directory.CreateDirectory(directory);
		var fileName = $"{testName}_{step:D2}_{Sanitize(description)}.png";
		step++;
		frame.Save(Path.Combine(directory, fileName));
	}

	static Window? TryGetMainWindow()
	{
		// ApplicationLifetime is null in headless; the [Shared] MainWindow export is the same
		// instance the test resolved.
		try
		{
			return AppComposition.Current.GetExport<MainWindow>();
		}
		catch
		{
			return null;
		}
	}

	static string ShortName(string? className)
	{
		if (string.IsNullOrWhiteSpace(className))
			return "UnknownFixture";
		var lastDot = className.LastIndexOf('.');
		var shortName = lastDot >= 0 ? className[(lastDot + 1)..] : className;
		return Sanitize(shortName);
	}

	static string Sanitize(string? label)
	{
		if (string.IsNullOrWhiteSpace(label))
			return "frame";
		return new string(label.Select(c => char.IsLetterOrDigit(c) ? c : '_').ToArray());
	}
}
