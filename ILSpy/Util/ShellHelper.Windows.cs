// Copyright (c) 2025 sonyps5201314
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

// The shell-COM reveal below (SHParseDisplayName + SHOpenFolderAndSelectItems) is adapted from
// the pre-Avalonia, Windows-only ShellHelper contributed by sonyps5201314; its copyright notice
// is retained per the MIT license under which it was provided. It is invoked only on Windows
// (guarded by OperatingSystem.IsWindows() in RevealFiles); the P/Invokes resolve lazily, so the
// declarations are harmless to compile on the cross-platform target.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

#pragma warning disable CA1060 // Move pinvokes to native methods class

namespace ICSharpCode.ILSpy.Util
{
	static partial class ShellHelper
	{
		[LibraryImport("shell32.dll", StringMarshalling = StringMarshalling.Utf16)]
		private static partial int SHParseDisplayName(string pszName, IntPtr pbc, out IntPtr ppidl, uint sfgaoIn, out uint psfgaoOut);

		[LibraryImport("shell32.dll")]
		private static partial int SHOpenFolderAndSelectItems(IntPtr pidlFolder, uint cidl, ReadOnlySpan<IntPtr> apidl, uint dwFlags);

		[LibraryImport("shell32.dll")]
		private static partial IntPtr ILFindLastID(IntPtr pidl);

		[LibraryImport("ole32.dll")]
		private static partial void CoTaskMemFree(IntPtr pv);

		/// <summary>
		/// Opens <paramref name="folder"/> in Explorer with all of <paramref name="files"/> selected,
		/// reusing a window already open at that folder. Falls back to <c>explorer.exe /select,</c>
		/// (single item) and then to just opening the folder if the shell COM call fails.
		/// </summary>
		static void RevealInExplorer(string folder, IReadOnlyList<string> files)
		{
			IntPtr folderPidl = IntPtr.Zero;
			var itemPidlAllocs = new List<IntPtr>();
			var relativePidls = new List<IntPtr>();

			try
			{
				int hrFolder = SHParseDisplayName(folder, IntPtr.Zero, out folderPidl, 0, out _);
				Marshal.ThrowExceptionForHR(hrFolder);

				foreach (var file in files)
				{
					int hrItem = SHParseDisplayName(file, IntPtr.Zero, out var itemPidl, 0, out _);
					if (hrItem == 0 && itemPidl != IntPtr.Zero)
					{
						// ILFindLastID returns a pointer *into* itemPidl, so itemPidl must stay alive
						// until SHOpenFolderAndSelectItems has consumed it (freed in the finally).
						IntPtr relative = ILFindLastID(itemPidl);
						if (relative != IntPtr.Zero)
						{
							relativePidls.Add(relative);
							itemPidlAllocs.Add(itemPidl);
							continue;
						}
					}
					if (itemPidl != IntPtr.Zero)
						CoTaskMemFree(itemPidl);
				}

				if (relativePidls.Count > 0)
				{
					int hr = SHOpenFolderAndSelectItems(folderPidl, (uint)relativePidls.Count, CollectionsMarshal.AsSpan(relativePidls), 0);
					Marshal.ThrowExceptionForHR(hr);
				}
				else
				{
					// Nothing resolved to a selectable item: just open the folder.
					OpenFolder(folder);
				}
			}
			catch (Exception ex) when (ex is COMException or Win32Exception)
			{
				RevealWithExplorerSelect(folder, files);
			}
			finally
			{
				foreach (var p in itemPidlAllocs)
					CoTaskMemFree(p);
				if (folderPidl != IntPtr.Zero)
					CoTaskMemFree(folderPidl);
			}
		}

		// Primitive fallback when the shell COM call is unavailable: explorer.exe can only select a
		// single item from the command line, so reveal the first file (or open the folder).
		static void RevealWithExplorerSelect(string folder, IReadOnlyList<string> files)
		{
			try
			{
				if (files.Count > 0)
					Process.Start(new ProcessStartInfo("explorer.exe", $"/select,\"{files[0]}\"") { UseShellExecute = false });
				else
					OpenFolder(folder);
			}
			catch
			{
				// Best-effort: the user can navigate manually if even this fails.
			}
		}
	}
}
