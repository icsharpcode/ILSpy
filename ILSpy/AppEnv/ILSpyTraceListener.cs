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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ILSpy.AppEnv
{
	/// <summary>
	/// Trace listener that downgrades <c>Debug.Assert</c> failures from process-killing fail-fasts
	/// to a regular surfaced exception. The decompiler ships a number of asserts that fire on
	/// real-world (but unusual) IL — without this, hovering or decompiling such a method would
	/// terminate the process. WPF ILSpy ships <c>ILSpyTraceListener</c> for the same reason.
	///
	/// We route through <see cref="GlobalExceptionHandler.Show"/> so asserts land in the same
	/// dialog as any other unhandled exception.
	/// </summary>
	sealed class ILSpyTraceListener : DefaultTraceListener
	{
		[Conditional("DEBUG")]
		public static void Install()
		{
			Trace.Listeners.Clear();
			Trace.Listeners.Add(new ILSpyTraceListener());
		}

		ILSpyTraceListener()
		{
			AssertUiEnabled = false;
		}

		// Dedup repeated asserts by call site so a decompile that hits the same assert in 200
		// methods only opens one dialog. Mirrors WPF ILSpyTraceListener's "ignoredStacks".
		readonly HashSet<string> seenTopFrames = new();

		public override void Fail(string? message)
			=> Fail(message, null);

		public override void Fail(string? message, string? detailMessage)
		{
			Debug.WriteLine("Assertion failed: " + message);
			if (!string.IsNullOrEmpty(detailMessage))
				Debug.WriteLine("  " + detailMessage);

			// Strip the ILSpyTraceListener + Debug.* frames so the dialog shows the assert's
			// actual call site at the top, like the WPF ILSpyTraceListener does.
			var frames = new StackTrace(fNeedFileInfo: true).ToString()
				.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
				.SkipWhile(f => f.Contains("ILSpyTraceListener") || f.Contains("System.Diagnostics"))
				.ToArray();
			if (frames.Length == 0)
				return;
			var topFrame = frames[0];
			lock (seenTopFrames)
			{
				if (!seenTopFrames.Add(topFrame))
					return;
			}
			var trimmedStack = string.Join(Environment.NewLine, frames);
			GlobalExceptionHandler.Show(new AssertionFailedException(message ?? "(no message)", detailMessage, trimmedStack));
		}
	}

	sealed class AssertionFailedException : Exception
	{
		readonly string trimmedStack;

		public AssertionFailedException(string message, string? detailMessage, string trimmedStack)
			: base(string.IsNullOrEmpty(detailMessage) ? message : message + Environment.NewLine + detailMessage)
		{
			this.trimmedStack = trimmedStack;
		}

		// The dialog renders exception.ToString() in its details box; override so the stack
		// it sees is the assert site, not the ILSpyTraceListener itself.
		public override string ToString()
			=> GetType().FullName + ": " + Message + Environment.NewLine + trimmedStack;

		public override string? StackTrace => trimmedStack;
	}
}
