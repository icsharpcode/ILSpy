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
using System.IO;
using System.Linq;

// The Windows-only part of this partial class declares shell32/ole32 P/Invokes; CA1060 reports on
// the class declaration here in the primary file rather than at the DllImport site.
#pragma warning disable CA1060 // Move pinvokes to native methods class

namespace ICSharpCode.ILSpy.Util
{
	/// <summary>
	/// Cross-platform helpers for handing a path to the OS shell: open a folder, reveal one or more
	/// files in the file manager, or open a file with its default application. All calls are
	/// best-effort -- a failed launch is swallowed, since the user can navigate manually and the
	/// gesture has already returned. Consolidates the explorer.exe / open / xdg-open switch that was
	/// copied across the export, save, PDB, diagram and package-extraction commands.
	///
	/// On Windows, revealing files goes through the shell COM API (see the Windows-specific part of
	/// this class) so that several selected files collapse into a single Explorer window -- reusing
	/// one already open at that folder -- instead of spawning a fresh explorer.exe per file.
	/// </summary>
	public static partial class ShellHelper
	{
		/// <summary>Opens <paramref name="path"/> (a directory) in the OS file manager.</summary>
		public static void OpenFolder(string path)
		{
			try
			{
				if (OperatingSystem.IsWindows())
					Process.Start(new ProcessStartInfo("explorer.exe", Quote(path)) { UseShellExecute = false });
				else if (OperatingSystem.IsMacOS())
					Process.Start(new ProcessStartInfo("open", Quote(path)) { UseShellExecute = false });
				else
					Process.Start(new ProcessStartInfo("xdg-open", path) { UseShellExecute = false });
			}
			catch
			{
				// Best-effort: the user can navigate manually if the shell call fails.
			}
		}

		/// <summary>
		/// Reveals <paramref name="path"/> (a file) in the OS file manager, selecting it where the
		/// platform supports it. See <see cref="RevealFiles"/> for the multi-file behaviour.
		/// </summary>
		public static void RevealFile(string path) => RevealFiles(new[] { path });

		/// <summary>
		/// Reveals several files in the OS file manager. Files are grouped by containing folder so
		/// that each folder is shown in a single window with all of its files selected, rather than
		/// one window per file. On Windows this reuses an Explorer window already open at the folder
		/// (shell COM); on macOS Finder's <c>open -R</c> reveals and selects; on Linux there is no
		/// portable "select item" hook, so each distinct parent folder is opened once.
		/// </summary>
		public static void RevealFiles(IEnumerable<string> paths)
		{
			var groups = GroupByFolder(paths);
			if (groups.Count == 0)
				return;

			if (OperatingSystem.IsWindows())
			{
				foreach (var (folder, files) in groups)
					RevealInExplorer(folder, files);
			}
			else if (OperatingSystem.IsMacOS())
			{
				// Finder reveals and selects every passed file in one invocation.
				var allFiles = groups.SelectMany(g => g.Files).Select(Quote);
				try
				{
					Process.Start(new ProcessStartInfo("open", "-R " + string.Join(' ', allFiles)) { UseShellExecute = false });
				}
				catch
				{
					// Best-effort: fall through silently.
				}
			}
			else
			{
				// Linux + others: open each distinct parent folder once (deduped by GroupByFolder).
				foreach (var (folder, _) in groups)
					OpenFolder(folder);
			}
		}

		/// <summary>Opens <paramref name="path"/> with its default application (image viewer,
		/// browser, ...).</summary>
		public static void OpenWithDefaultApplication(string path)
		{
			try
			{
				// UseShellExecute resolves the default app on Windows and macOS; Linux needs xdg-open.
				if (OperatingSystem.IsWindows() || OperatingSystem.IsMacOS())
					Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
				else
					Process.Start(new ProcessStartInfo("xdg-open", path) { UseShellExecute = false });
			}
			catch
			{
				// Best-effort: the user can open the file manually if the shell call fails.
			}
		}

		/// <summary>
		/// Groups paths by their containing directory, preserving the order in which folders are
		/// first seen and deduping paths case-insensitively. Entries that are null/empty or have no
		/// containing directory are dropped. Exposed for testing the reveal grouping without
		/// launching the OS file manager.
		/// </summary>
		internal static IReadOnlyList<(string Folder, IReadOnlyList<string> Files)> GroupByFolder(IEnumerable<string?>? paths)
		{
			if (paths is null)
				return Array.Empty<(string, IReadOnlyList<string>)>();

			var groups = new List<(string Folder, List<string> Files)>();
			var folderIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
			var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

			foreach (var path in paths)
			{
				if (string.IsNullOrEmpty(path) || !seen.Add(path))
					continue;
				var folder = Path.GetDirectoryName(path);
				if (string.IsNullOrEmpty(folder))
					continue;
				if (!folderIndex.TryGetValue(folder, out int i))
				{
					i = groups.Count;
					folderIndex.Add(folder, i);
					groups.Add((folder, new List<string>()));
				}
				groups[i].Files.Add(path);
			}

			return groups.Select(g => (g.Folder, (IReadOnlyList<string>)g.Files)).ToList();
		}

		static string Quote(string value) => "\"" + value + "\"";
	}
}
