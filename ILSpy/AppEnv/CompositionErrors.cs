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

using ICSharpCode.Decompiler;

namespace ICSharpCode.ILSpy.AppEnv
{
	/// <summary>A single non-fatal composition failure: which part failed, and why.</summary>
	public sealed record CompositionError(string Source, Exception Exception);

	/// <summary>
	/// Collects non-fatal MEF composition failures -- a plugin assembly that fails to load, or an
	/// exported command whose constructor throws when the menu/toolbar builder materialises it.
	/// Unlike <see cref="StartupExceptions"/> (fatal startup errors that replace the app with an
	/// error window), these are recoverable: the offending part is skipped, the app keeps running,
	/// and the collected errors are surfaced to the user in a document tab (see <see cref="WriteTo"/>)
	/// and, when <see cref="AppLog.Category.Composition"/> is enabled, written to the log file.
	/// </summary>
	public static class CompositionErrors
	{
		static readonly List<CompositionError> items = new();

		public static IReadOnlyList<CompositionError> Items => items;

		public static bool Any => items.Count > 0;

		/// <summary>Records a composition failure and logs it under the Composition category.</summary>
		public static void Report(string source, Exception exception)
		{
			items.Add(new CompositionError(source, exception));
			AppLog.Write(AppLog.Category.Composition, $"{source}: {exception}");
		}

		/// <summary>Clears the collected errors. Intended for test isolation.</summary>
		public static void Clear() => items.Clear();

		/// <summary>Renders every collected error into <paramref name="output"/> for display.</summary>
		public static void WriteTo(ITextOutput output)
		{
			ArgumentNullException.ThrowIfNull(output);
			foreach (var error in items)
			{
				output.WriteLine($"[{error.Source}]");
				output.WriteLine(error.Exception.ToString());
				output.WriteLine();
			}
		}
	}
}
