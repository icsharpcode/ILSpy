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
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

using AvalonDock.Layout.Serialization;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Search;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.Updates;
using ICSharpCode.ILSpy.ViewModels;
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

			this.Loaded += MainWindow_Loaded;
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
			Rect bounds = Rect.Transform(sessionSettings.WindowBounds, source?.CompositionTarget?.TransformToDevice ?? Matrix.Identity);
			var boundsRect = new Rectangle((int)bounds.Left, (int)bounds.Top, (int)bounds.Width, (int)bounds.Height);
			bool boundsOK = false;
			foreach (var screen in Screen.AllScreens)
			{
				var intersection = Rectangle.Intersect(boundsRect, screen.WorkingArea);
				if (intersection.Width > 10 && intersection.Height > 10)
					boundsOK = true;
			}
			if (boundsOK)
				SetWindowBounds(sessionSettings.WindowBounds);
			else
				SetWindowBounds(SessionSettings.DefaultWindowBounds);

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

		internal async Task HandleSingleInstanceCommandLineArguments(string[] args)
		{
			var cmdArgs = CommandLineArguments.Create(args);

			await Dispatcher.InvokeAsync(() => {
				if (AssemblyTreeModel.HandleCommandLineArguments(cmdArgs))
				{
					if (!cmdArgs.NoActivate && WindowState == WindowState.Minimized)
						WindowState = WindowState.Normal;

					AssemblyTreeModel.HandleCommandLineArgumentsAfterShowList(cmdArgs);
				}
			});
		}

		void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			DockWorkspace.Instance.TabPages.Add();

			AssemblyTreeModel.Initialize();
		}

		#region Update Check
		string updateAvailableDownloadUrl;

		public async Task ShowMessageIfUpdatesAvailableAsync(ILSpySettings spySettings, bool forceCheck = false)
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
				string downloadUrl = await NotifyOfUpdatesStrategy.CheckForUpdatesAsync(ILSpySettings.Load());
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
				NavigateHistory(false);
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
				NavigateHistory(true);
			}
		}

		void NavigateHistory(bool forward)
		{
			TabPageModel tabPage = DockWorkspace.Instance.ActiveTabPage;
			var state = tabPage.GetState();
			if (state != null)
				history.UpdateCurrent(new NavigationState(tabPage, state));
			var newState = forward ? history.GoForward() : history.GoBack();

			DockWorkspace.Instance.ActiveTabPage = newState.TabPage;

			AssemblyTreeModel.SelectNodes(newState.TreeNodes, ignoreCompilationRequests: true);
			AssemblyTreeModel.DecompileSelectedNodes(newState.ViewState as DecompilerTextViewState, false);
		}
		#endregion

		internal void NavigateTo(RequestNavigateEventArgs e, bool recordHistory = true, bool inNewTabPage = false)
		{
			if (e.Uri.Scheme == "resource")
			{
				if (inNewTabPage)
				{
					DockWorkspace.Instance.TabPages.Add();
				}

				if (e.Uri.Host == "aboutpage")
				{
					RecordHistory();
					DockWorkspace.Instance.ActiveTabPage.ShowTextView(AboutPage.Display);
					e.Handled = true;
					return;
				}
				AvalonEditTextOutput output = new AvalonEditTextOutput {
					Address = e.Uri,
					Title = e.Uri.AbsolutePath,
					EnableHyperlinks = true
				};
				using (Stream s = typeof(App).Assembly.GetManifestResourceStream(typeof(App), e.Uri.AbsolutePath))
				{
					using (StreamReader r = new StreamReader(s))
					{
						string line;
						while ((line = r.ReadLine()) != null)
						{
							output.Write(line);
							output.WriteLine();
						}
					}
				}
				RecordHistory();
				DockWorkspace.Instance.ShowText(output);
				e.Handled = true;
			}

			void RecordHistory()
			{
				if (!recordHistory)
					return;
				TabPageModel tabPage = DockWorkspace.Instance.ActiveTabPage;
				var currentState = tabPage.GetState();
				if (currentState != null)
					history.UpdateCurrent(new NavigationState(tabPage, currentState));

				AssemblyTreeModel.UnselectAll(ignoreCompilationRequests: true);
				history.Record(new NavigationState(tabPage, new ViewState { ViewedUri = e.Uri }));
			}
		}

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
			var sessionSettings = SettingsService.Instance.SessionSettings;

			sessionSettings.ActiveAssemblyList = AssemblyTreeModel.CurrentAssemblyList.ListName;
			sessionSettings.ActiveTreeViewPath = AssemblyTreeModel.SelectedPath;
			sessionSettings.ActiveAutoLoadedAssembly = GetAutoLoadedAssemblyNode(AssemblyTreeModel.SelectedItem);
			sessionSettings.WindowBounds = this.RestoreBounds;
			sessionSettings.DockLayout.Serialize(new XmlLayoutSerializer(dockManager));
			sessionSettings.Save();
		}

		private string GetAutoLoadedAssemblyNode(SharpTreeNode node)
		{
			if (node == null)
				return null;
			while (!(node is AssemblyTreeNode) && node.Parent != null)
			{
				node = node.Parent;
			}
			//this should be an assembly node
			var assyNode = node as AssemblyTreeNode;
			var loadedAssy = assyNode.LoadedAssembly;
			if (!(loadedAssy.IsLoaded && loadedAssy.IsAutoLoaded))
				return null;

			return loadedAssy.FileName;
		}
	}
}
