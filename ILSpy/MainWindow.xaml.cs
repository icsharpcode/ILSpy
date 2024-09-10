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
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using AvalonDock.Layout.Serialization;

using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Search;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.Themes;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.Updates;
using ICSharpCode.ILSpyX.FileLoaders;
using ICSharpCode.ILSpyX.Settings;
using ICSharpCode.ILSpyX.TreeView;

using Microsoft.Win32;

using Screen = System.Windows.Forms.Screen;

namespace ICSharpCode.ILSpy
{
	/// <summary>
	/// The main window of the application.
	/// </summary>
	partial class MainWindow : Window
	{
		readonly NavigationHistoryService history = NavigationHistoryService.Instance;

		static MainWindow instance;

		public static MainWindow Instance {
			get { return instance; }
		}

		public AssemblyListPaneModel AssemblyTreeModel {
			get {
				return App.ExportProvider.GetExportedValue<AssemblyListPaneModel>();
			}
		}

		public MainWindow()
		{
			instance = this;

			var sessionSettings = SettingsService.Instance.SessionSettings;
			ThemeManager.Current.Theme = sessionSettings.Theme;

			// Make sure Images are initialized on the UI thread.
			this.Icon = Images.ILSpyIcon;

			this.DataContext = new MainWindowViewModel {
				Workspace = DockWorkspace.Instance,
				SessionSettings = sessionSettings,
				AssemblyListManager = SettingsService.Instance.AssemblyListManager
			};

			SettingsService.Instance.AssemblyListManager.CreateDefaultAssemblyLists();
			if (!string.IsNullOrEmpty(sessionSettings.CurrentCulture))
			{
				Thread.CurrentThread.CurrentUICulture = new CultureInfo(sessionSettings.CurrentCulture);
			}

			InitializeComponent();

			DockWorkspace.Instance.InitializeLayout(dockManager);

			MenuService.Instance.Init(mainMenu, toolBar, InputBindings);

			InitFileLoaders();
		}

		void SetWindowBounds(Rect bounds)
		{
			this.Left = bounds.Left;
			this.Top = bounds.Top;
			this.Width = bounds.Width;
			this.Height = bounds.Height;
		}

		#region File Loader extensibility

		void InitFileLoaders()
		{
			// TODO
			foreach (var loader in App.ExportProvider.GetExportedValues<IFileLoader>())
			{

			}
		}

		#endregion

		#region Message Hook

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			var source = PresentationSource.FromVisual(this);

			var sessionSettings = SettingsService.Instance.SessionSettings;

			// Validate and Set Window Bounds
			var windowBounds = Rect.Transform(sessionSettings.WindowBounds, source?.CompositionTarget?.TransformToDevice ?? Matrix.Identity);
			var boundsRect = new Rectangle((int)windowBounds.Left, (int)windowBounds.Top, (int)windowBounds.Width, (int)windowBounds.Height);

			bool areBoundsValid = Screen.AllScreens.Any(screen => Rectangle.Intersect(boundsRect, screen.WorkingArea) is { Width: > 10, Height: > 10 });

			SetWindowBounds(areBoundsValid ? sessionSettings.WindowBounds : SessionSettings.DefaultWindowBounds);

			this.WindowState = sessionSettings.WindowState;
		}

		#endregion

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (!e.Handled && e.KeyboardDevice.Modifiers == ModifierKeys.Alt && e.Key == Key.System)
			{
				switch (e.SystemKey)
				{
					case Key.A:
						assemblyListComboBox.Focus();
						e.Handled = true;
						break;
					case Key.L:
						languageComboBox.Focus();
						e.Handled = true;
						break;
					case Key.E: // Alt+V was already taken by _View menu
						languageVersionComboBox.Focus();
						e.Handled = true;
						break;
				}
			}
		}

		#region Update Check
		string updateAvailableDownloadUrl;

		public async Task ShowMessageIfUpdatesAvailableAsync(ISettingsProvider spySettings, bool forceCheck = false)
		{
			string downloadUrl;
			if (forceCheck)
			{
				downloadUrl = await NotifyOfUpdatesStrategy.CheckForUpdatesAsync(spySettings);
			}
			else
			{
				downloadUrl = await NotifyOfUpdatesStrategy.CheckForUpdatesIfEnabledAsync(spySettings);
			}

			// The Update Panel is only available for NotifyOfUpdatesStrategy, AutoUpdate will have differing UI requirements
			AdjustUpdateUIAfterCheck(downloadUrl, forceCheck);
		}

		void UpdatePanelCloseButtonClick(object sender, RoutedEventArgs e)
		{
			updatePanel.Visibility = Visibility.Collapsed;
		}

		async void downloadOrCheckUpdateButtonClick(object sender, RoutedEventArgs e)
		{
			if (updateAvailableDownloadUrl != null)
			{
				OpenLink(updateAvailableDownloadUrl);
			}
			else
			{
				updatePanel.Visibility = Visibility.Collapsed;
				string downloadUrl = await NotifyOfUpdatesStrategy.CheckForUpdatesAsync(SettingsService.Instance.SpySettings);
				AdjustUpdateUIAfterCheck(downloadUrl, true);
			}
		}

		void AdjustUpdateUIAfterCheck(string downloadUrl, bool displayMessage)
		{
			updateAvailableDownloadUrl = downloadUrl;
			updatePanel.Visibility = displayMessage ? Visibility.Visible : Visibility.Collapsed;
			if (downloadUrl != null)
			{
				updatePanelMessage.Text = Properties.Resources.ILSpyVersionAvailable;
				downloadOrCheckUpdateButton.Content = Properties.Resources.Download;
			}
			else
			{
				updatePanelMessage.Text = Properties.Resources.UpdateILSpyFound;
				downloadOrCheckUpdateButton.Content = Properties.Resources.CheckAgain;
			}
		}
		#endregion

		public static void OpenLink(string link)
		{
			try
			{
				Process.Start(new ProcessStartInfo { FileName = link, UseShellExecute = true });
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
			}
			catch (Exception)
			{
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
				// Process.Start can throw several errors (not all of them documented),
				// just ignore all of them.
			}
		}

		public static void ExecuteCommand(string fileName, string arguments)
		{
			try
			{
				Process.Start(fileName, arguments);
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
			}
			catch (Exception)
			{
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
				// Process.Start can throw several errors (not all of them documented),
				// just ignore all of them.
			}
		}

		#region Open/Refresh
		void OpenCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = ".NET assemblies|*.dll;*.exe;*.winmd;*.wasm|Nuget Packages (*.nupkg)|*.nupkg|Portable Program Database (*.pdb)|*.pdb|All files|*.*";
			dlg.Multiselect = true;
			dlg.RestoreDirectory = true;
			if (dlg.ShowDialog() == true)
			{
				OpenFiles(dlg.FileNames);
			}
		}

		public void OpenFiles(string[] fileNames, bool focusNode = true)
		{
			if (fileNames == null)
				throw new ArgumentNullException(nameof(fileNames));

			if (focusNode)
				AssemblyTreeModel.UnselectAll();

			AssemblyTreeModel.LoadAssemblies(fileNames, focusNode: focusNode);
		}

		void RefreshCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			AssemblyTreeModel.Refresh();
		}

		void SearchCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			DockWorkspace.Instance.ShowToolPane(SearchPaneModel.PaneContentId);
		}
		#endregion

		void SaveCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;
			e.CanExecute = SaveCodeContextMenuEntry.CanExecute(AssemblyTreeModel.SelectedNodes.ToList());
		}

		void SaveCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			SaveCodeContextMenuEntry.Execute(AssemblyTreeModel.SelectedNodes.ToList());
		}

		#region Back/Forward navigation
		void BackCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;
			e.CanExecute = history.CanNavigateBack;
		}

		void BackCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			if (history.CanNavigateBack)
			{
				e.Handled = true;
				AssemblyTreeModel.NavigateHistory(false);
			}
		}

		void ForwardCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;
			e.CanExecute = history.CanNavigateForward;
		}

		void ForwardCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			if (history.CanNavigateForward)
			{
				e.Handled = true;
				AssemblyTreeModel.NavigateHistory(true);
			}
		}
		#endregion

		protected override void OnStateChanged(EventArgs e)
		{
			base.OnStateChanged(e);
			// store window state in settings only if it's not minimized
			if (this.WindowState != WindowState.Minimized)
				SettingsService.Instance.SessionSettings.WindowState = this.WindowState;
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);

			var snapshot = SettingsService.Instance.CreateSnapshot();

			var sessionSettings = snapshot.GetSettings<SessionSettings>();

			sessionSettings.ActiveAssemblyList = AssemblyTreeModel.AssemblyList.ListName;
			sessionSettings.ActiveTreeViewPath = AssemblyTreeModel.SelectedPath;
			sessionSettings.ActiveAutoLoadedAssembly = GetAutoLoadedAssemblyNode(AssemblyTreeModel.SelectedItem);
			sessionSettings.WindowBounds = this.RestoreBounds;
			sessionSettings.DockLayout.Serialize(new XmlLayoutSerializer(dockManager));

			snapshot.Save();
		}

		private static string GetAutoLoadedAssemblyNode(SharpTreeNode node)
		{
			var assemblyTreeNode = node?
				.AncestorsAndSelf()
				.OfType<AssemblyTreeNode>()
				.FirstOrDefault();

			var loadedAssembly = assemblyTreeNode?.LoadedAssembly;

			return loadedAssembly is not { IsLoaded: true, IsAutoLoaded: true }
				? null
				: loadedAssembly.FileName;
		}
	}
}
