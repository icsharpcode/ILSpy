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
using System.Runtime.CompilerServices;

using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Threading;

namespace ICSharpCode.ILSpy.Tests;

public static class WindowExtensions
{
	/// <summary>Snapshots the window with Skia, writes a temp PNG, and opens it in the OS
	/// image viewer. No-op when <c>UseHeadlessDrawing</c> is true (CI default).</summary>
	public static void CaptureAndShow(this Window window, [CallerMemberName] string? label = null)
	{
		ArgumentNullException.ThrowIfNull(window);
		Dispatcher.UIThread.RunJobs();

		var frame = window.CaptureRenderedFrame();
		if (frame is null)
			return;

		var fileName = $"ILSpy.Tests-{Sanitize(label)}-{DateTime.Now:HHmmss-fff}.png";
		var path = Path.Combine(Path.GetTempPath(), fileName);
		frame.Save(path);

		try
		{
			Process.Start(new ProcessStartInfo {
				FileName = path,
				UseShellExecute = true,
			});
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine($"CaptureAndShow: failed to launch viewer for {path}: {ex.Message}");
		}
	}

	static string Sanitize(string? label)
	{
		if (string.IsNullOrWhiteSpace(label))
			return "frame";
		return new string(label.Select(c => char.IsLetterOrDigit(c) ? c : '_').ToArray());
	}
}
