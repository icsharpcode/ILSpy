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

using System.ComponentModel;
using System.Composition;

using Avalonia;
using Avalonia.Controls;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Views
{
	[Export]
	[Shared]
	public partial class MainWindow : Window
	{
		readonly SettingsService? settingsService;

		public MainWindow()
		{
			// Parameterless ctor — design-time / preview only. The runtime path uses the
			// ImportingConstructor below, which sets DataContext BEFORE inflating XAML so
			// that DockControl's Layout binding (and the cascade of Layout.Id, Layout.Title,
			// Layout.CanDrag, Layout.CanDrop, Layout.DockGroup template bindings) sees a
			// non-null source on first evaluation. Without that ordering, every binding
			// throws + logs at startup, which adds up to ~30 errors per launch.
			AppLog.Mark("MainWindow parameterless ctor entered (XAML inflation about to start)");
			InitializeComponent();
			AppLog.Mark("MainWindow parameterless ctor exited (XAML inflation done)");
		}

		[ImportingConstructor]
		public MainWindow(MainWindowViewModel viewModel, SettingsService settingsService)
		{
			AppLog.Mark("MainWindow ctor entered");
			this.settingsService = settingsService;
			DataContext = viewModel;
			AppLog.Mark("MainWindow XAML inflation about to start (DataContext set)");
			InitializeComponent();
			AppLog.Mark("MainWindow XAML inflation done");
			// Records every mouse/keyboard interaction under ILSPY_LOG=DBUSDEBUG so an unobserved
			// DBus error (which surfaces later on the finalizer thread) can be traced back to the
			// gesture that triggered the DBus call. No-op unless that category is enabled.
			InputDiagnostics.Attach(this);
			ICSharpCode.ILSpy.MainMenu.Attach(this);
			ApplySessionSettings(settingsService.SessionSettings);
			Opened += async (_, _) => {
				AppLog.Mark("MainWindow.Opened fired");
				using (AppLog.Phase("AssemblyTreeModel.Initialize"))
					viewModel.AssemblyTreeModel.Initialize();
				AppLog.Mark("MainWindow.Opened handler returning");
				if (App.CommandLineArguments is { } args)
					await viewModel.AssemblyTreeModel.HandleCommandLineArgumentsAsync(args);
				// Surface any non-fatal composition failures (failed plugins, uninstantiable menu/
				// toolbar commands) once the menu and toolbar have finished building. Deferred to a
				// later dispatcher turn so those builders' own Loaded handlers have run first.
				Avalonia.Threading.Dispatcher.UIThread.Post(SurfaceCompositionErrors,
					Avalonia.Threading.DispatcherPriority.Background);
			};
			AppLog.Mark("MainWindow ctor exited");
		}

		static void SurfaceCompositionErrors()
		{
			if (!AppEnv.CompositionErrors.Any)
				return;
			try
			{
				var output = new TextView.AvaloniaEditTextOutput { Title = "Composition Errors" };
				AppEnv.CompositionErrors.WriteTo(output);
				AppEnv.AppComposition.Current
					.GetExport<ICSharpCode.ILSpy.Docking.DockWorkspace>()
					.ShowTextInNewTab("Composition Errors", output);
			}
			catch (System.Exception ex)
			{
				// Surfacing the report must never itself crash startup.
				System.Diagnostics.Debug.WriteLine($"[MainWindow] SurfaceCompositionErrors failed: {ex}");
			}
		}

		void ApplySessionSettings(SessionSettings session)
		{
			Position = session.WindowPosition;
			Width = session.WindowSize.Width;
			Height = session.WindowSize.Height;
			WindowState = session.WindowState;
		}

		protected override void OnClosing(WindowClosingEventArgs e)
		{
			if (settingsService != null)
			{
				var session = settingsService.SessionSettings;
				session.WindowState = WindowState;
				// Only update bounds when the window is in Normal state. Otherwise Position/Width/
				// Height reflect the maximized rect, which would clobber the last "restore" bounds.
				if (WindowState == WindowState.Normal)
				{
					session.WindowPosition = Position;
					session.WindowSize = new Size(Width, Height);
				}
			}
			// Persist the dock layout to ILSpy.Layout.json so the user's pane positions
			// + splitter ratios survive the next launch. Resolved via composition so the
			// window doesn't need to be wired with a direct DockWorkspace reference.
			try
			{
				ICSharpCode.ILSpy.AppEnv.AppComposition.Current
					.GetExport<ICSharpCode.ILSpy.Docking.DockWorkspace>()
					.SaveLayout();
			}
			catch (System.Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"[MainWindow] SaveLayout on close failed: {ex}");
			}
			base.OnClosing(e);
		}
	}
}
