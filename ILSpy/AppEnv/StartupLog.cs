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

namespace ILSpy.AppEnv
{
	/// <summary>
	/// Lightweight timing log for the startup path. Entries land on
	/// <see cref="Debug.WriteLine(string)"/> so they show up in the IDE Debug Output
	/// when running under a debugger; release builds elide them. Times are millisecond
	/// elapsed since the first call (process-wide), so different log lines are directly
	/// comparable.
	/// </summary>
	internal static class StartupLog
	{
		static readonly Stopwatch sw = Stopwatch.StartNew();

		public static void Mark(string message)
			=> Debug.WriteLine($"[startup +{sw.ElapsedMilliseconds,5}ms] {message}");

		/// <summary>
		/// Brackets a scope with BEGIN/END markers carrying the duration spent inside.
		/// Use with <c>using var _ = StartupLog.Phase("...");</c>.
		/// </summary>
		public static IDisposable Phase(string name)
		{
			Mark($"BEGIN {name}");
			var local = Stopwatch.StartNew();
			return new PhaseScope(name, local);
		}

		sealed class PhaseScope(string name, Stopwatch local) : IDisposable
		{
			public void Dispose() => Mark($"END   {name} ({local.ElapsedMilliseconds}ms)");
		}
	}
}
