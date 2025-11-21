using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;
using System.Collections.Generic;

namespace ICSharpCode.ILSpy.Util
{
	static class ShellHelper
	{
		[DllImport("shell32.dll", CharSet = CharSet.Unicode)]
		static extern int SHParseDisplayName([MarshalAs(UnmanagedType.LPWStr)] string pszName, IntPtr pbc, out IntPtr ppidl, uint sfgaoIn, out uint psfgaoOut);

		[DllImport("shell32.dll")]
		static extern int SHOpenFolderAndSelectItems(IntPtr pidlFolder, uint cidl, [MarshalAs(UnmanagedType.LPArray)] IntPtr[] apidl, uint dwFlags);

		[DllImport("shell32.dll")]
		static extern IntPtr ILFindLastID(IntPtr pidl);

		[DllImport("ole32.dll")]
		static extern void CoTaskMemFree(IntPtr pv);

		public static void OpenFolder(string folderPath)
		{
			try
			{
				if (string.IsNullOrEmpty(folderPath))
					return;
				if (!Directory.Exists(folderPath))
					return;

				IntPtr folderPidl = IntPtr.Zero;
				uint attrs;
				int hr = SHParseDisplayName(folderPath, IntPtr.Zero, out folderPidl, 0, out attrs);
				if (hr == 0 && folderPidl != IntPtr.Zero)
				{
					SHOpenFolderAndSelectItems(folderPidl, 0, null, 0);
					CoTaskMemFree(folderPidl);
				}
				else
				{
					// fallback
					Process.Start(new ProcessStartInfo { FileName = folderPath, UseShellExecute = true });
				}
			}
			catch
			{
				// ignore
			}
		}

		public static void OpenFolderAndSelectItem(string path)
		{
			// Reuse the multi-item implementation for single item selection to avoid duplication.
			try
			{
				if (string.IsNullOrEmpty(path))
					return;
				if (Directory.Exists(path))
				{
					OpenFolder(path);
					return;
				}

				if (!File.Exists(path))
					return;

				OpenFolderAndSelectItems(new[] { path });
			}
			catch
			{
				// ignore
			}
		}

		public static void OpenFolderAndSelectItems(IEnumerable<string> paths)
		{
			try
			{
				if (paths == null)
					return;
				// Group by containing folder
				var files = paths.Where(p => !string.IsNullOrEmpty(p) && File.Exists(p)).ToList();
				if (files.Count == 0)
					return;

				var groups = files.GroupBy(p => Path.GetDirectoryName(p));
				foreach (var group in groups)
				{
					string folder = group.Key;
					if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder))
						continue;

					IntPtr folderPidl = IntPtr.Zero;
					uint attrs;
					int hrFolder = SHParseDisplayName(folder, IntPtr.Zero, out folderPidl, 0, out attrs);
					if (hrFolder != 0 || folderPidl == IntPtr.Zero)
					{
						// fallback: open folder normally
						OpenFolder(folder);
						continue;
					}

					var itemPidlAllocs = new List<IntPtr>();
					var relativePidls = new List<IntPtr>();
					try
					{
						foreach (var file in group)
						{
							IntPtr itemPidl = IntPtr.Zero;
							int hrItem = SHParseDisplayName(file, IntPtr.Zero, out itemPidl, 0, out attrs);
							if (hrItem == 0 && itemPidl != IntPtr.Zero)
							{
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
							SHOpenFolderAndSelectItems(folderPidl, (uint)relativePidls.Count, relativePidls.ToArray(), 0);
						}
						else
						{
							// nothing to select - open folder
							OpenFolder(folder);
						}
					}
					finally
					{
						foreach (var p in itemPidlAllocs)
							CoTaskMemFree(p);
						if (folderPidl != IntPtr.Zero)
							CoTaskMemFree(folderPidl);
					}
				}
			}
			catch
			{
				// ignore
			}
		}
	}
}
