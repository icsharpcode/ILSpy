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

using Avalonia;

namespace ICSharpCode.ILSpy
{
	sealed class Program
	{
		// Initialization code. Don't use any Avalonia, third-party APIs or any
		// SynchronizationContext-reliant code before AppMain is called: things aren't initialized
		// yet and stuff might break.
		[STAThread]
		public static void Main(string[] args) => BuildAvaloniaApp()
			.StartWithClassicDesktopLifetime(args);

		// Avalonia configuration, don't remove; also used by visual designer.
		public static AppBuilder BuildAvaloniaApp()
		{
			var builder = AppBuilder.Configure<App>()
				.UsePlatformDetect()
				// Render popups (menus, flyouts, tooltips) in the window's own overlay layer instead of
				// separate native child windows. On X11/XWayland native popups can lose focus and
				// self-dismiss within ~100ms of opening (observed on the document-strip overflow
				// dropdown: it opened and closed before becoming visible).
				.With(new X11PlatformOptions { OverlayPopups = true });
#if DEBUG
			// Enables and attaches the Avalonia 12 DevTools bridge (it calls AttachDeveloperTools
			// internally, so no separate App-level attach is needed -- a second call throws
			// "already attached"). The avdt global tool connects over this bridge. Gated on DEBUG
			// because the DiagnosticsSupport assets are excluded from Release.
			builder = builder.WithDeveloperTools();
#endif
			return builder.LogToTrace();
		}
	}
}