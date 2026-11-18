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
using System.Runtime.InteropServices;

#pragma warning disable CA1060 // Move pinvokes to native methods class

namespace ICSharpCode.ILSpy.Util
{
	/// <summary>
	/// Reads the Windows system UI font (the NONCLIENTMETRICS message font). Avalonia's built-in
	/// default (Segoe UI at 12 DIP) matches a stock Windows install, but the accessibility
	/// "Text size" setting (Settings &gt; Accessibility &gt; Text size) scales these metrics
	/// without changing DPI, and only apps that read them follow it.
	/// </summary>
	public static partial class WindowsSystemFont
	{
		const uint SPI_GETNONCLIENTMETRICS = 0x0029;

		[StructLayout(LayoutKind.Sequential)]
		unsafe struct LOGFONTW
		{
			public int lfHeight;
			public int lfWidth;
			public int lfEscapement;
			public int lfOrientation;
			public int lfWeight;
			public byte lfItalic;
			public byte lfUnderline;
			public byte lfStrikeOut;
			public byte lfCharSet;
			public byte lfOutPrecision;
			public byte lfClipPrecision;
			public byte lfQuality;
			public byte lfPitchAndFamily;
			public fixed char lfFaceName[32];
		}

		[StructLayout(LayoutKind.Sequential)]
		struct NONCLIENTMETRICSW
		{
			public uint cbSize;
			public int iBorderWidth;
			public int iScrollWidth;
			public int iScrollHeight;
			public int iCaptionWidth;
			public int iCaptionHeight;
			public LOGFONTW lfCaptionFont;
			public int iSmCaptionWidth;
			public int iSmCaptionHeight;
			public LOGFONTW lfSmCaptionFont;
			public int iMenuWidth;
			public int iMenuHeight;
			public LOGFONTW lfMenuFont;
			public LOGFONTW lfStatusFont;
			public LOGFONTW lfMessageFont;
			public int iPaddedBorderWidth;
		}

		[LibraryImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static partial bool SystemParametersInfoForDpi(uint uiAction, uint uiParam, ref NONCLIENTMETRICSW pvParam, uint fWinIni, uint dpi);

		/// <summary>
		/// Gets the system message font as an Avalonia-ready (family name, size in DIPs) pair.
		/// Returns false on non-Windows platforms or if the metrics cannot be read. The metrics
		/// are requested at 96 DPI, so the size is in device-independent pixels regardless of
		/// display scaling (which Avalonia applies separately).
		/// </summary>
		public static unsafe bool TryGetMessageFont(out string faceName, out double fontSize)
		{
			faceName = string.Empty;
			fontSize = 0;

			// SystemParametersInfoForDpi requires Windows 10 1607.
			if (!OperatingSystem.IsWindowsVersionAtLeast(10, 0, 14393))
				return false;

			var metrics = new NONCLIENTMETRICSW { cbSize = (uint)sizeof(NONCLIENTMETRICSW) };
			if (!SystemParametersInfoForDpi(SPI_GETNONCLIENTMETRICS, metrics.cbSize, ref metrics, 0, 96))
				return false;

			// A negative lfHeight is the character height (the usual case for the message font);
			// a positive one is the cell height. Either way the magnitude is the pixel size at
			// the requested DPI.
			int height = Math.Abs(metrics.lfMessageFont.lfHeight);
			if (height <= 0)
				return false;

			var nameBuffer = new ReadOnlySpan<char>(metrics.lfMessageFont.lfFaceName, 32);
			int terminator = nameBuffer.IndexOf('\0');
			faceName = new string(terminator >= 0 ? nameBuffer[..terminator] : nameBuffer);
			if (faceName.Length == 0)
				return false;

			fontSize = height;
			return true;
		}
	}
}
