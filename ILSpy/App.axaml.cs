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
using System.Composition.Hosting;
using System.Diagnostics;
using System.IO;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using ICSharpCode.ILSpyX.Settings;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Themes;
using ICSharpCode.ILSpy.Views;

namespace ICSharpCode.ILSpy
{
	public partial class App : Application
	{
		public static CommandLineArguments? CommandLineArguments { get; private set; }
		public static CompositionHost? Composition { get; private set; }

		// On first run the default assembly list is seeded with the entire shared-framework
		// directory the running runtime lives in (~150 assemblies). Headless tests flip this
		// off so each test boots with a minimal default instead of re-seeding the whole
		// framework per test (which is ~10x slower); see TestApp.
		public static bool SeedFullFrameworkDefaultList { get; set; } = true;

		public override void Initialize()
		{
			AvaloniaXamlLoader.Load(this);
		}

		public override void OnFrameworkInitializationCompleted()
		{
			AppLog.Mark("App.OnFrameworkInitializationCompleted entered");
			ILSpyTraceListener.Install();
			GlobalExceptionHandler.Install();

			CommandLineArguments = CommandLineArguments.Create(Environment.GetCommandLineArgs()[1..]);

			ILSpySettings.SettingsFilePathProvider = () => {
				if (App.CommandLineArguments.ConfigFile != null)
					return App.CommandLineArguments.ConfigFile;

				var assemblyLocation = typeof(MainWindow).Assembly.Location;
				if (!String.IsNullOrWhiteSpace(assemblyLocation))
				{
					var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
					Debug.Assert(assemblyDirectory != null);
					string localPath = Path.Combine(assemblyDirectory, "ILSpy.xml");
					if (File.Exists(localPath))
						return localPath;
				}

				return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ICSharpCode", "ILSpy.xml");
			};

			try
			{
				using var _ = AppLog.Phase("AppComposition.Initialize");
				Composition = AppComposition.Initialize();
			}
			catch (Exception ex)
			{
				StartupExceptions.Items.Add(new ExceptionData(ex));
			}

			try
			{
				if (Composition?.GetExport<SettingsService>() is { } settingsService)
				{
					ThemeManager.Current.Attach(settingsService.SessionSettings);
					ApplyCulture(settingsService.SessionSettings.CurrentCulture);
				}
			}
			catch (Exception ex)
			{
				StartupExceptions.Items.Add(new ExceptionData(ex));
			}

			if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
			{
				MainWindow? mainWindow = null;
				try
				{
					AppLog.Mark("MainWindow about to be resolved from MEF");
					mainWindow = Composition?.GetExport<MainWindow>();
				}
				catch (Exception ex)
				{
					StartupExceptions.Items.Add(new ExceptionData(ex));
				}

				// Without this fallback, anything thrown by MEF resolution above propagates out of
				// OnFrameworkInitializationCompleted before the dispatcher pump starts -- so the
				// AppDomain.UnhandledException dialog posted by GlobalExceptionHandler never gets
				// a chance to run, and the user sees a silent exit. Hand the user a window that
				// shows the captured exceptions instead.
				desktop.MainWindow = StartupExceptions.Items.Count > 0
					? new StartupErrorWindow(StartupExceptions.Items)
					: mainWindow ?? new MainWindow();
				AppLog.Mark("MainWindow assigned to desktop.MainWindow");
				desktop.Exit += (_, _) => {
					try
					{ Composition?.GetExport<SettingsService>().Save(); }
					catch { /* persistence must never block shutdown */ }
					Composition?.Dispose();
				};
			}

			base.OnFrameworkInitializationCompleted();
		}

		// Effective UI culture is process-wide. Setting it on startup means a changed CurrentCulture
		// only takes effect after restart, matching the WPF behaviour.
		static void ApplyCulture(string? culture)
		{
			if (string.IsNullOrEmpty(culture))
				return;
			try
			{
				var info = System.Globalization.CultureInfo.GetCultureInfo(culture);
				System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = info;
				System.Threading.Thread.CurrentThread.CurrentUICulture = info;
				ICSharpCode.ILSpy.Properties.Resources.Culture = info;
			}
			catch (System.Globalization.CultureNotFoundException)
			{
			}
		}
	}
}