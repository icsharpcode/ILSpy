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

namespace ILSpy.Util
{
	/// <summary>
	/// Cross-platform helpers for handing a path to the OS shell: open a folder, reveal a file in
	/// the file manager, or open a file with its default application. All calls are best-effort --
	/// a failed launch is swallowed, since the user can navigate manually and the gesture has
	/// already returned. Consolidates the explorer.exe / open / xdg-open switch that was copied
	/// across the export, save, PDB, diagram and package-extraction commands.
	/// </summary>
	public static class ShellHelper
	{
		/// <summary>Opens <paramref name="path"/> (a directory) in the OS file manager.</summary>
		public static void OpenFolder(string path) => Launch(path, selectItem: false);

		/// <summary>
		/// Reveals <paramref name="path"/> (a file) in the OS file manager, selecting it where the
		/// platform supports it (Windows <c>/select,</c>, macOS <c>-R</c>). On Linux there is no
		/// stable cross-distro "select file" hook, so the parent directory is opened instead.
		/// </summary>
		public static void RevealFile(string path) => Launch(path, selectItem: true);

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

		static void Launch(string path, bool selectItem)
		{
			try
			{
				if (OperatingSystem.IsWindows())
				{
					var args = selectItem ? $"/select,\"{path}\"" : $"\"{path}\"";
					Process.Start(new ProcessStartInfo("explorer.exe", args) { UseShellExecute = false });
				}
				else if (OperatingSystem.IsMacOS())
				{
					var args = selectItem ? $"-R \"{path}\"" : $"\"{path}\"";
					Process.Start(new ProcessStartInfo("open", args) { UseShellExecute = false });
				}
				else
				{
					// Linux + others: no universal "select item" command, so revealing a file opens
					// its parent directory.
					var target = selectItem ? (Path.GetDirectoryName(path) ?? path) : path;
					Process.Start(new ProcessStartInfo("xdg-open", target) { UseShellExecute = false });
				}
			}
			catch
			{
				// Best-effort: the user can navigate manually if the shell call fails.
			}
		}
	}
}
