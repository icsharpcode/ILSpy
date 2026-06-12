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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;

namespace ICSharpCode.ILSpy.AppEnv
{
	/// <summary>
	/// Accessors for the desktop UI context. The classic-desktop lifetime cast was repeated at every
	/// dialog-owner / clipboard / shutdown call site; centralising it here keeps those short and
	/// consistent. Each member returns null / no-ops on non-desktop or design-time hosts where there
	/// is no main window.
	/// </summary>
	public static class UiContext
	{
		/// <summary>The desktop main window, or null when there is no classic-desktop lifetime.</summary>
		public static Window? MainWindow
			=> (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

		/// <summary>The main window's clipboard, or null when unavailable.</summary>
		public static IClipboard? Clipboard => MainWindow?.Clipboard;

		/// <summary>Requests application shutdown on the desktop lifetime; a no-op elsewhere.</summary>
		public static void Shutdown()
			=> (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.Shutdown();
	}
}
