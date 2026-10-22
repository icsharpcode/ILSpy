// Copyright (c) 2026 Christoph Wille
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
using System.IO;
using System.Runtime.InteropServices;

#pragma warning disable CA1060 // Move pinvokes to native methods class

namespace ICSharpCode.ILSpy.TextView
{
	/// <summary>
	/// What the desktop says about mouse-wheel scrolling. Avalonia delivers raw wheel notches and
	/// does not expose the system settings, so the editor's scroll metrics read them here: Windows'
	/// "lines to scroll" via SystemParametersInfo, and KDE's equivalent in kdeglobals, which Qt
	/// applications honor. GNOME has no such setting, and macOS applies scroll speed and
	/// acceleration to the deltas before an application sees them. Read once at startup; a changed
	/// setting applies after a restart.
	/// </summary>
	static partial class SystemScrollSettings
	{
		const uint DefaultWheelScrollLines = 3;

		// Windows' "one screen at a time" choice for the wheel (WHEEL_PAGESCROLL).
		const uint WheelPageScroll = uint.MaxValue;

		// Which desktop's conventions the scroll metrics follow. Production code leaves these at
		// the OS values; the headless tests, which run on every desktop, pin the platform they
		// assert against.
		internal static bool IsWindows { get; set; } = OperatingSystem.IsWindows();
		internal static bool IsMacOS { get; set; } = OperatingSystem.IsMacOS();

		static readonly uint wheelScrollLinesSetting = ReadWheelScrollLines();

		/// <summary>Lines scrolled per wheel notch; 3 where the desktop has no such setting.</summary>
		internal static double WheelScrollLines => wheelScrollLinesSetting;

		/// <summary>True when Windows is set to scroll one screen per wheel notch.</summary>
		internal static bool WheelScrollsOnePage => wheelScrollLinesSetting == WheelPageScroll;

		static uint ReadWheelScrollLines()
		{
			const uint SPI_GETWHEELSCROLLLINES = 0x0068;
			if (OperatingSystem.IsWindows())
				return SystemParametersInfoW(SPI_GETWHEELSCROLLLINES, 0, out uint lines, 0) ? lines : DefaultWheelScrollLines;
			if (OperatingSystem.IsLinux())
				return ReadKdeWheelScrollLines() ?? DefaultWheelScrollLines;
			return DefaultWheelScrollLines;
		}

		// Plasma stores "wheel scroll lines" in kdeglobals under [KDE]; both plasma-integration and
		// Qt's built-in KDE theme read that key (default 3), so Qt applications and this editor
		// move the same number of lines per notch. The file is simply absent on other desktops.
		static uint? ReadKdeWheelScrollLines()
		{
			string? configDir = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
			if (string.IsNullOrEmpty(configDir))
				configDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config");
			string path = Path.Combine(configDir, "kdeglobals");
			try
			{
				return File.Exists(path) ? ParseKdeWheelScrollLines(File.ReadAllText(path)) : null;
			}
			catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
			{
				return null;
			}
		}

		/// <summary>
		/// The <c>WheelScrollLines</c> value of the <c>[KDE]</c> group in kdeglobals INI text, or null
		/// when the group or the key is absent or not a number.
		/// </summary>
		internal static uint? ParseKdeWheelScrollLines(string kdeglobals)
		{
			const string key = "WheelScrollLines=";
			bool inKdeGroup = false;
			foreach (string rawLine in kdeglobals.Split('\n'))
			{
				string line = rawLine.Trim();
				if (line.StartsWith('['))
				{
					inKdeGroup = line == "[KDE]";
					continue;
				}
				if (inKdeGroup && line.StartsWith(key, StringComparison.Ordinal))
					return uint.TryParse(line.AsSpan(key.Length), out uint lines) ? lines : null;
			}
			return null;
		}

		[LibraryImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static partial bool SystemParametersInfoW(uint uiAction, uint uiParam, out uint pvParam, uint fWinIni);
	}
}
