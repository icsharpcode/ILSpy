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
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;

namespace ICSharpCode.ILSpy.AppEnv
{
	/// <summary>
	/// Categorized diagnostic log. All categories are off by default; production runs
	/// incur near-zero cost from existing call sites because <see cref="IsEnabled"/>
	/// short-circuits before allocations.
	/// <list type="bullet">
	/// <item><see cref="Mark"/> / <see cref="Phase"/> — startup-timing helpers. Each line
	/// gets a <c>[startup +Nms]</c> prefix where N is milliseconds since the first
	/// <see cref="AppLog"/> reference (process-wide). Gated on the
	/// <see cref="Category.Startup"/> category; emits nothing unless that category is
	/// enabled.</item>
	/// <item><see cref="Write(string, string)"/> / <see cref="Write(string, Func{string})"/> —
	/// general category-based logging. Use the <see cref="Func{T}"/> overload for
	/// messages that interpolate non-trivial state — the factory is only invoked when
	/// the category is enabled.</item>
	/// </list>
	/// Enable categories with the <c>ILSPY_LOG=Cat1,Cat2</c> environment variable at
	/// process start (e.g. <c>ILSPY_LOG=Startup</c> to capture the startup timeline) or
	/// call <see cref="Enable(string)"/> at runtime. Output goes to both
	/// <see cref="Debug.WriteLine(string)"/> (so DbgView / IDE Debug Output captures it)
	/// and <c>%TEMP%\ilspy-avalonia.log</c> (created on first emit; truncated at the
	/// first write of each process).
	/// </summary>
	public static class AppLog
	{
		/// <summary>
		/// Well-known category names. Free-form strings work too — these constants exist
		/// so call sites are typo-resistant and so adding a new category doesn't require
		/// updating callers (just add a constant + start writing).
		/// </summary>
		public static class Category
		{
			/// <summary>Process startup timeline. Off by default — opt in with <c>ILSPY_LOG=Startup</c>.</summary>
			public const string Startup = "Startup";

			/// <summary>Dock chrome activity — view-recycling cache, layout save/load, drag/drop transitions.</summary>
			public const string Docking = "Docking";

			/// <summary>
			/// Mouse/keyboard interaction trail plus unwrapped DBus exception reports, for
			/// correlating an unobserved <c>Tmds.DBus</c> error (which surfaces asynchronously on
			/// the finalizer thread) with the interaction that triggered the DBus call. Off by
			/// default; opt in with <c>ILSPY_LOG=DBUSDEBUG</c> (matched case-insensitively).
			/// </summary>
			public const string DBusDebug = "DBusDebug";

			/// <summary>
			/// Non-fatal MEF composition failures collected by <see cref="CompositionErrors"/> — a
			/// plugin assembly that won't load, or an exported command whose constructor throws when
			/// the menu/toolbar builder materialises it. Errors are always shown to the user in a
			/// document tab; opt in with <c>ILSPY_LOG=Composition</c> to also log them to file.
			/// </summary>
			public const string Composition = "Composition";
		}

		static readonly Stopwatch sw = Stopwatch.StartNew();
		static readonly ConcurrentDictionary<string, bool> enabled = LoadFromEnvironment();
		static readonly object fileLock = new();
		static readonly string logFilePath = Path.Combine(Path.GetTempPath(), "ilspy-avalonia.log");
		static bool fileTruncated;

		/// <summary>Returns true if the given category is currently enabled.</summary>
		public static bool IsEnabled(string category)
			=> enabled.TryGetValue(category, out var on) && on;

		/// <summary>Enables a category. Idempotent.</summary>
		public static void Enable(string category) => enabled[category] = true;

		/// <summary>Disables a category. Idempotent.</summary>
		public static void Disable(string category) => enabled[category] = false;

		/// <summary>
		/// Writes a message under <paramref name="category"/> if enabled. Prefer the
		/// <see cref="Write(string, Func{string})"/> overload for messages built with
		/// string interpolation — this overload pays formatting cost at the call site
		/// regardless of whether the category is on.
		/// </summary>
		public static void Write(string category, string message)
		{
			if (!IsEnabled(category))
				return;
			EmitLine($"[{DateTime.Now:HH:mm:ss.fff}] [{category}] {message}");
		}

		/// <summary>
		/// Writes a message under <paramref name="category"/> if enabled. The factory
		/// delegate is only invoked when the category is on, so the caller's string
		/// interpolation is zero-cost when off.
		/// </summary>
		public static void Write(string category, Func<string> messageFactory)
		{
			if (!IsEnabled(category))
				return;
			EmitLine($"[{DateTime.Now:HH:mm:ss.fff}] [{category}] {messageFactory()}");
		}

		/// <summary>
		/// Emits a single startup-timing line: <c>[startup +Nms] message</c>, where N
		/// is milliseconds since the process started. No-op if <see cref="Category.Startup"/>
		/// has been disabled.
		/// </summary>
		public static void Mark(string message)
		{
			if (!IsEnabled(Category.Startup))
				return;
			EmitLine($"[startup +{sw.ElapsedMilliseconds,5}ms] {message}");
		}

		/// <summary>
		/// Brackets a scope with BEGIN/END startup markers carrying the duration spent
		/// inside. Use with <c>using var _ = AppLog.Phase("...");</c>. No-op if
		/// <see cref="Category.Startup"/> has been disabled (the returned disposable is
		/// still safe to use; its Dispose is a no-op).
		/// </summary>
		public static IDisposable Phase(string name)
		{
			Mark($"BEGIN {name}");
			return new PhaseScope(name, Stopwatch.StartNew());
		}

		sealed class PhaseScope(string name, Stopwatch local) : IDisposable
		{
			public void Dispose() => Mark($"END   {name} ({local.ElapsedMilliseconds}ms)");
		}

		static void EmitLine(string line)
		{
			Debug.WriteLine(line);
			lock (fileLock)
			{
				try
				{
					if (!fileTruncated)
					{
						File.WriteAllText(logFilePath, $"[AppLog started {DateTime.Now:O}]" + Environment.NewLine);
						fileTruncated = true;
					}
					File.AppendAllText(logFilePath, line + Environment.NewLine);
				}
				catch
				{
					// Logging failures must not bubble — diagnostic only.
				}
			}
		}

		static ConcurrentDictionary<string, bool> LoadFromEnvironment()
		{
			var dict = new ConcurrentDictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
			// All categories default off; production runs incur zero cost from the existing
			// Mark/Phase call sites because IsEnabled short-circuits before any allocation.
			// Opt in via ILSPY_LOG=Startup,Docking,... at process launch, or call
			// AppLog.Enable("Startup") at runtime. The %TEMP%\ilspy-avalonia.log file is
			// only created on the first emitted line.
			var env = Environment.GetEnvironmentVariable("ILSPY_LOG");
			if (string.IsNullOrWhiteSpace(env))
				return dict;
			foreach (var raw in env.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
				dict[raw] = true;
			return dict;
		}
	}
}
