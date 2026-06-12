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

namespace ICSharpCode.ILSpy.AppEnv
{
	/// <summary>
	/// Trace listener that intercepts <c>Debug.Assert</c> failures and shows a developer dialog
	/// (Throw / Debug / Ignore / Ignore All) instead of the process-killing default fail-fast.
	/// The decompiler ships a number of asserts that fire on real-world (but unusual) IL — without
	/// this, hovering or decompiling such a method would terminate the process.
	/// </summary>
	sealed class ILSpyTraceListener : DefaultTraceListener
	{
		[Conditional("DEBUG")]
		public static void Install()
		{
			Trace.Listeners.Clear();
			Trace.Listeners.Add(new ILSpyTraceListener());
			// Don't serialize trace output through TraceInternal's global lock. A decompiler
			// Debug.Assert runs Fail() under that lock, and Fail() blocks on the UI thread to
			// show the dialog; meanwhile the UI thread emits its own trace output (Avalonia
			// logs binding errors during a layout pass) and would block acquiring the same
			// lock -> deadlock. With the global lock off and the listener declaring itself
			// thread-safe, TraceInternal calls the listener directly, so the asserting thread
			// and the UI thread no longer contend on it.
			Trace.UseGlobalLock = false;
		}

		// Tells TraceInternal it may invoke this listener without taking the global lock; the
		// dialog show is marshalled to the UI thread and guarded against re-entrancy, and the
		// listener's own mutable state is lock-protected below.
		public override bool IsThreadSafe => true;

		ILSpyTraceListener()
		{
			AssertUiEnabled = false;
		}

		// Call sites the developer chose "Ignore All" for; further asserts from these are dropped.
		readonly HashSet<string> ignoredTopFrames = new();
		// Guards against a second assert (e.g. raised while the dispatcher pumps during the dialog)
		// stacking another dialog on top of the open one.
		bool dialogIsOpen;

		public override void Fail(string? message)
			=> Fail(message, null);

		public override void Fail(string? message, string? detailMessage)
		{
			Debug.WriteLine("Assertion failed: " + message);
			if (!string.IsNullOrEmpty(detailMessage))
				Debug.WriteLine("  " + detailMessage);

			// Strip the ILSpyTraceListener + Debug.* frames so the dialog shows the assert's
			// actual call site at the top.
			var frames = new StackTrace(fNeedFileInfo: true).ToString()
				.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
				.SkipWhile(f => f.Contains("ILSpyTraceListener") || f.Contains("System.Diagnostics"))
				.ToArray();
			if (frames.Length == 0)
				return;
			var topFrame = frames[0];
			lock (ignoredTopFrames)
			{
				if (ignoredTopFrames.Contains(topFrame) || dialogIsOpen)
					return;
				dialogIsOpen = true;
			}
			try
			{
				var trimmedStack = string.Join(Environment.NewLine, frames);
				switch (AssertionFailedDialog.Show(message ?? "(no message)", detailMessage, trimmedStack))
				{
					case AssertionAction.Throw:
						throw new AssertionFailedException(message ?? "(no message)", detailMessage, trimmedStack);
					case AssertionAction.Debug:
						Debugger.Break();
						break;
					case AssertionAction.IgnoreAll:
						lock (ignoredTopFrames)
							ignoredTopFrames.Add(topFrame);
						break;
				}
			}
			finally
			{
				lock (ignoredTopFrames)
					dialogIsOpen = false;
			}
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
