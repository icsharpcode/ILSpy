// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.ComponentModel;
using System.Composition;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

using Screen = System.Windows.Forms.Screen;

namespace ICSharpCode.ILSpy
{
	/// <summary>
	/// The main window of the application.
	/// </summary>
	[Export]
	[Shared]
#pragma warning disable MEF003 // Main window is a singleton
	partial class MainWindow
	{
		private readonly SettingsService settingsService;

		public MainWindow(MainWindowViewModel mainWindowViewModel, SettingsService settingsService)
		{
			this.settingsService = settingsService;

			// Make sure Images are initialized on the UI thread.
			this.Icon = Images.ILSpyIcon;

			this.DataContext = mainWindowViewModel;

			InitializeComponent();

			Dispatcher.BeginInvoke(DispatcherPriority.Background, () => {
				mainWindowViewModel.Workspace.InitializeLayout();
				MessageBus.Send(this, new MainWindowLoadedEventArgs());
			});
		}

		void SetWindowBounds(Rect bounds)
		{
			this.Left = bounds.Left;
			this.Top = bounds.Top;
			this.Width = bounds.Width;
			this.Height = bounds.Height;
		}

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			var source = PresentationSource.FromVisual(this);

			var sessionSettings = settingsService.SessionSettings;

			// Validate and Set Window Bounds
			var windowBounds = Rect.Transform(sessionSettings.WindowBounds, source?.CompositionTarget?.TransformToDevice ?? Matrix.Identity);
			var boundsRect = new Rectangle((int)windowBounds.Left, (int)windowBounds.Top, (int)windowBounds.Width, (int)windowBounds.Height);

			bool areBoundsValid = Screen.AllScreens.Any(screen => Rectangle.Intersect(boundsRect, screen.WorkingArea) is { Width: > 10, Height: > 10 });

			SetWindowBounds(areBoundsValid ? sessionSettings.WindowBounds : SessionSettings.DefaultWindowBounds);

			this.WindowState = sessionSettings.WindowState;
		}

		protected override void OnStateChanged(EventArgs e)
		{
			base.OnStateChanged(e);
			// store window state in settings only if it's not minimized
			if (this.WindowState != WindowState.Minimized)
				settingsService.SessionSettings.WindowState = this.WindowState;
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);

			var snapshot = settingsService.CreateSnapshot();

			var sessionSettings = snapshot.GetSettings<SessionSettings>();

			MessageBus.Send(this, new ApplySessionSettingsEventArgs(sessionSettings));

			sessionSettings.WindowBounds = this.RestoreBounds;
			sessionSettings.DockLayout.Serialize(new(DockManager));

			snapshot.Save();
		}
	}
}