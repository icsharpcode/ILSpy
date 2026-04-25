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

using ILSpy.ViewModels;

namespace ILSpy.Views
{
	[Export]
	[Shared]
	public partial class MainWindow : Window
	{
		readonly SettingsService? settingsService;

		public MainWindow()
		{
			InitializeComponent();
		}

		[ImportingConstructor]
		public MainWindow(MainWindowViewModel viewModel, SettingsService settingsService) : this()
		{
			this.settingsService = settingsService;
			DataContext = viewModel;
			ApplySessionSettings(settingsService.SessionSettings);
			Opened += (_, _) => viewModel.AssemblyTreeModel.Initialize();
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
			base.OnClosing(e);
		}
	}
}
