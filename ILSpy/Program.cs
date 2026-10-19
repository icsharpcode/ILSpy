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
using System.Threading.Tasks;

using Avalonia;

using ICSharpCode.ILSpy.AppEnv;

namespace ICSharpCode.ILSpy
{
	sealed class Program
	{
		// Initialization code. Don't use any Avalonia, third-party APIs or any
		// SynchronizationContext-reliant code before AppMain is called: things aren't initialized
		// yet and stuff might break.
		[STAThread]
		public static void Main(string[] args)
		{
			// Single-instance gate runs before Avalonia starts: a second launch forwards its
			// arguments to the running instance over a named pipe and exits here, so no second
			// window opens.
			if (!ShouldStartNewInstance(args))
				return;

			AppLog.Mark("Program.Main: starting");
			// Pre-pay the first layout (de)serialization's JIT and metadata cost off the UI thread
			// (see ILSpyDockJson.WarmUp). It needs neither Avalonia nor the container, only the
			// plugin assemblies loaded so its dockable-type scan sees every pane type, and it
			// competes for the JIT with everything else that starts up. DockWorkspace loads the
			// user's layout roughly 300 ms after Avalonia's framework initialization completes;
			// started here, ahead of Avalonia's own platform setup, the warm-up is done by then.
			AppComposition.RegisterPluginResolver();
			_ = Task.Run(Docking.ILSpyDockJson.WarmUp);

			BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
		}

		static bool ShouldStartNewInstance(string[] args)
		{
			try
			{
				var commandLineArguments = CommandLineArguments.Create(args);
				App.ConfigureSettingsFilePathProvider(commandLineArguments);
				return SingleInstance.TrySignalFirstInstance(commandLineArguments);
			}
			catch (Exception)
			{
				// Single-instance coordination must never prevent a normal launch.
				return true;
			}
		}

		// Avalonia configuration, don't remove; also used by visual designer.
		public static AppBuilder BuildAvaloniaApp()
		{
			var builder = AppBuilder.Configure<App>()
				.UsePlatformDetect()
				// Parse the icon set off the UI thread as soon as the asset loader exists, which is
				// before App.axaml and the themes load; the views that need icons are inflated well
				// after that (see Images.PreloadSvgs).
				.AfterPlatformServicesSetup(_ => Task.Run(Images.PreloadSvgs))
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