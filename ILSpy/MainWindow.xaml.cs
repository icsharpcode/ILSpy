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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

using AvalonDock.Layout.Serialization;

using ICSharpCode.Decompiler;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Search;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.Themes;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.Updates;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpyX.FileLoaders;
using ICSharpCode.ILSpyX.Settings;
using ICSharpCode.ILSpyX.Extensions;

using Microsoft.Win32;
using ICSharpCode.ILSpyX.TreeView;

using TomsToolbox.Composition;

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

		public DecompilationOptions CreateDecompilationOptions()
		{
			var decompilerView = DockWorkspace.Instance.ActiveTabPage.Content as IProgress<DecompilationProgress>;

			return new(AssemblyTreeModel.CurrentLanguageVersion, SettingsService.Instance.DecompilerSettings, SettingsService.Instance.DisplaySettings) { Progress = decompilerView };
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
			InitToolPanes();
			DockWorkspace.Instance.InitializeLayout(dockManager);

			MessageBus<DockWorkspaceActiveTabPageChangedEventArgs>.Subscribers += DockWorkspace_ActiveTabPageChanged;

			InitMainMenu();
			InitWindowMenu();
			InitToolbar();
			InitFileLoaders();

			this.Loaded += MainWindow_Loaded;
		}

		private void DockWorkspace_ActiveTabPageChanged(object sender, EventArgs e)
		{
			DockWorkspace dock = DockWorkspace.Instance;

			var windowMenuItem = mainMenu.Items.OfType<MenuItem>().First(m => (string)m.Tag == nameof(Properties.Resources._Window));
			foreach (MenuItem menuItem in windowMenuItem.Items.OfType<MenuItem>())
			{
				if (menuItem.IsCheckable && menuItem.Tag is TabPageModel)
				{
					menuItem.IsChecked = menuItem.Tag == dock.ActiveTabPage;
				}
			}
		}

		void SetWindowBounds(Rect bounds)
		{
			this.Left = bounds.Left;
			this.Top = bounds.Top;
			this.Width = bounds.Width;
			this.Height = bounds.Height;
		}

		#region Toolbar extensibility

		void InitToolbar()
		{
			int navigationPos = 0;
			int openPos = 1;
			var toolbarCommands = App.ExportProvider.GetExports<ICommand, IToolbarCommandMetadata>("ToolbarCommand");
			foreach (var commandGroup in toolbarCommands.OrderBy(c => c.Metadata.ToolbarOrder).GroupBy(c => Properties.Resources.ResourceManager.GetString(c.Metadata.ToolbarCategory)))
			{
				if (commandGroup.Key == Properties.Resources.ResourceManager.GetString("Navigation"))
				{
					foreach (var command in commandGroup)
					{
						toolBar.Items.Insert(navigationPos++, MakeToolbarItem(command));
						openPos++;
					}
				}
				else if (commandGroup.Key == Properties.Resources.ResourceManager.GetString("Open"))
				{
					foreach (var command in commandGroup)
					{
						toolBar.Items.Insert(openPos++, MakeToolbarItem(command));
					}
				}
				else
				{
					toolBar.Items.Add(new Separator());
					foreach (var command in commandGroup)
					{
						toolBar.Items.Add(MakeToolbarItem(command));
					}
				}
			}

		}

		Button MakeToolbarItem(IExport<ICommand, IToolbarCommandMetadata> command)
		{
			return new Button {
				Style = ThemeManager.Current.CreateToolBarButtonStyle(),
				Command = CommandWrapper.Unwrap(command.Value),
				ToolTip = Properties.Resources.ResourceManager.GetString(command.Metadata.ToolTip),
				Tag = command.Metadata.Tag,
				Content = new System.Windows.Controls.Image {
					Width = 16,
					Height = 16,
					Source = Images.Load(command.Value, command.Metadata.ToolbarIcon)
				}
			};
		}
		#endregion

		#region Main Menu extensibility

		void InitMainMenu()
		{
			var mainMenuCommands = App.ExportProvider.GetExports<ICommand, IMainMenuCommandMetadata>("MainMenuCommand");
			// Start by constructing the individual flat menus
			var parentMenuItems = new Dictionary<string, MenuItem>();
			var menuGroups = mainMenuCommands.OrderBy(c => c.Metadata.MenuOrder).GroupBy(c => c.Metadata.ParentMenuID);
			foreach (var menu in menuGroups)
			{
				// Get or add the target menu item and add all items grouped by menu category
				var parentMenuItem = GetOrAddParentMenuItem(menu.Key, menu.Key);
				foreach (var category in menu.GroupBy(c => c.Metadata.MenuCategory))
				{
					if (parentMenuItem.Items.Count > 0)
					{
						parentMenuItem.Items.Add(new Separator { Tag = category.Key });
					}
					foreach (var entry in category)
					{
						if (menuGroups.Any(g => g.Key == entry.Metadata.MenuID))
						{
							var menuItem = GetOrAddParentMenuItem(entry.Metadata.MenuID, entry.Metadata.Header);
							// replace potential dummy text with real name
							menuItem.Header = GetResourceString(entry.Metadata.Header);
							parentMenuItem.Items.Add(menuItem);
						}
						else
						{
							MenuItem menuItem = new MenuItem();
							menuItem.Command = CommandWrapper.Unwrap(entry.Value);
							menuItem.Tag = entry.Metadata.MenuID;
							menuItem.Header = GetResourceString(entry.Metadata.Header);
							if (!string.IsNullOrEmpty(entry.Metadata.MenuIcon))
							{
								menuItem.Icon = new Image {
									Width = 16,
									Height = 16,
									Source = Images.Load(entry.Value, entry.Metadata.MenuIcon)
								};
							}

							menuItem.IsEnabled = entry.Metadata.IsEnabled;
							if (entry.Value is ToggleableCommand toggle)
							{
								menuItem.IsCheckable = true;
								menuItem.SetBinding(MenuItem.IsCheckedProperty, new Binding("IsChecked") { Source = entry.Value, Mode = BindingMode.OneWay });
							}

							menuItem.InputGestureText = entry.Metadata.InputGestureText;
							parentMenuItem.Items.Add(menuItem);
						}
					}
				}
			}

			foreach (var (key, item) in parentMenuItems)
			{
				if (item.Parent == null)
				{
					mainMenu.Items.Add(item);
				}
			}

			MenuItem GetOrAddParentMenuItem(string menuID, string resourceKey)
			{
				if (!parentMenuItems.TryGetValue(menuID, out var parentMenuItem))
				{
					var topLevelMenuItem = mainMenu.Items.OfType<MenuItem>().FirstOrDefault(m => (string)m.Tag == menuID);
					if (topLevelMenuItem == null)
					{
						parentMenuItem = new MenuItem();
						parentMenuItem.Header = GetResourceString(resourceKey);
						parentMenuItem.Tag = menuID;
						parentMenuItems.Add(menuID, parentMenuItem);
					}
					else
					{
						parentMenuItems.Add(menuID, topLevelMenuItem);
						parentMenuItem = topLevelMenuItem;
					}
				}
				return parentMenuItem;
			}
		}

		internal static string GetResourceString(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}
			string value = Properties.Resources.ResourceManager.GetString(key);
			if (!string.IsNullOrEmpty(value))
			{
				return value;
			}
			return key;
		}
		#endregion

		#region Tool Pane extensibility

		private void InitToolPanes()
		{
			var toolPanes = App.ExportProvider.GetExportedValues<ToolPaneModel>("ToolPane").OrderBy(item => item.Title);

			DockWorkspace.Instance.ToolPanes.AddRange(toolPanes);
		}

		private void InitWindowMenu()
		{
			var windowMenuItem = mainMenu.Items.OfType<MenuItem>().First(m => (string)m.Tag == nameof(Properties.Resources._Window));
			Separator separatorBeforeTools, separatorBeforeDocuments;
			windowMenuItem.Items.Add(separatorBeforeTools = new Separator());
			windowMenuItem.Items.Add(separatorBeforeDocuments = new Separator());

			var dock = DockWorkspace.Instance;
			dock.ToolPanes.CollectionChanged += ToolsChanged;
			dock.TabPages.CollectionChanged += TabsChanged;

			ToolsChanged(dock.ToolPanes, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			TabsChanged(dock.TabPages, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

			void ToolsChanged(object sender, NotifyCollectionChangedEventArgs e)
			{
				int endIndex = windowMenuItem.Items.IndexOf(separatorBeforeDocuments);
				int startIndex = windowMenuItem.Items.IndexOf(separatorBeforeTools) + 1;
				int insertionIndex;
				switch (e.Action)
				{
					case NotifyCollectionChangedAction.Add:
						insertionIndex = Math.Min(endIndex, startIndex + e.NewStartingIndex);
						foreach (ToolPaneModel pane in e.NewItems)
						{
							MenuItem menuItem = CreateMenuItem(pane);
							windowMenuItem.Items.Insert(insertionIndex, menuItem);
							insertionIndex++;
						}
						break;
					case NotifyCollectionChangedAction.Remove:
						foreach (ToolPaneModel pane in e.OldItems)
						{
							for (int i = endIndex - 1; i >= startIndex; i--)
							{
								MenuItem item = (MenuItem)windowMenuItem.Items[i];
								if (pane == item.Tag)
								{
									windowMenuItem.Items.RemoveAt(i);
									item.Tag = null;
									endIndex--;
									break;
								}
							}
						}
						break;
					case NotifyCollectionChangedAction.Replace:
						break;
					case NotifyCollectionChangedAction.Move:
						break;
					case NotifyCollectionChangedAction.Reset:
						for (int i = endIndex - 1; i >= startIndex; i--)
						{
							MenuItem item = (MenuItem)windowMenuItem.Items[0];
							item.Tag = null;
							windowMenuItem.Items.RemoveAt(i);
							endIndex--;
						}
						insertionIndex = endIndex;
						foreach (ToolPaneModel pane in dock.ToolPanes)
						{
							MenuItem menuItem = CreateMenuItem(pane);
							windowMenuItem.Items.Insert(insertionIndex, menuItem);
							insertionIndex++;
						}
						break;
				}

				MenuItem CreateMenuItem(ToolPaneModel pane)
				{
					MenuItem menuItem = new MenuItem();
					menuItem.Command = pane.AssociatedCommand ?? new ToolPaneCommand(pane.ContentId);
					menuItem.Header = pane.Title;
					menuItem.Tag = pane;
					var shortcutKey = pane.ShortcutKey;
					if (shortcutKey != null)
					{
						InputBindings.Add(new InputBinding(menuItem.Command, shortcutKey));
						menuItem.InputGestureText = shortcutKey.GetDisplayStringForCulture(CultureInfo.CurrentUICulture);
					}
					if (!string.IsNullOrEmpty(pane.Icon))
					{
						menuItem.Icon = new Image {
							Width = 16,
							Height = 16,
							Source = Images.Load(pane, pane.Icon)
						};
					}

					return menuItem;
				}
			}

			void TabsChanged(object sender, NotifyCollectionChangedEventArgs e)
			{
				int endIndex = windowMenuItem.Items.Count;
				int startIndex = windowMenuItem.Items.IndexOf(separatorBeforeDocuments) + 1;
				int insertionIndex;
				switch (e.Action)
				{
					case NotifyCollectionChangedAction.Add:
						insertionIndex = Math.Min(endIndex, startIndex + e.NewStartingIndex);
						foreach (TabPageModel pane in e.NewItems)
						{
							MenuItem menuItem = CreateMenuItem(pane);
							pane.PropertyChanged += TabPageChanged;
							windowMenuItem.Items.Insert(insertionIndex, menuItem);
							insertionIndex++;
						}
						break;
					case NotifyCollectionChangedAction.Remove:
						foreach (TabPageModel pane in e.OldItems)
						{
							for (int i = endIndex - 1; i >= startIndex; i--)
							{
								MenuItem item = (MenuItem)windowMenuItem.Items[i];
								if (pane == item.Tag)
								{
									windowMenuItem.Items.RemoveAt(i);
									pane.PropertyChanged -= TabPageChanged;
									item.Tag = null;
									endIndex--;
									break;
								}
							}
						}
						break;
					case NotifyCollectionChangedAction.Replace:
						break;
					case NotifyCollectionChangedAction.Move:
						break;
					case NotifyCollectionChangedAction.Reset:
						for (int i = endIndex - 1; i >= startIndex; i--)
						{
							MenuItem item = (MenuItem)windowMenuItem.Items[i];
							windowMenuItem.Items.RemoveAt(i);
							((TabPageModel)item.Tag).PropertyChanged -= TabPageChanged;
							endIndex--;
						}
						insertionIndex = endIndex;
						foreach (TabPageModel pane in dock.TabPages)
						{
							MenuItem menuItem = CreateMenuItem(pane);
							pane.PropertyChanged += TabPageChanged;
							windowMenuItem.Items.Insert(insertionIndex, menuItem);
							insertionIndex++;
						}
						break;
				}

				MenuItem CreateMenuItem(TabPageModel pane)
				{
					MenuItem menuItem = new MenuItem();
					menuItem.Command = new TabPageCommand(pane);
					menuItem.Header = pane.Title.Length > 20 ? pane.Title.Substring(20) + "..." : pane.Title;
					menuItem.Tag = pane;
					menuItem.IsCheckable = true;

					return menuItem;
				}
			}

			static void TabPageChanged(object sender, PropertyChangedEventArgs e)
			{
				var windowMenuItem = Instance.mainMenu.Items.OfType<MenuItem>().First(m => (string)m.Tag == nameof(Properties.Resources._Window));
				foreach (MenuItem menuItem in windowMenuItem.Items.OfType<MenuItem>())
				{
					if (menuItem.IsCheckable && menuItem.Tag == sender)
					{
						string title = ((TabPageModel)sender).Title;
						menuItem.Header = title.Length > 20 ? title.Substring(0, 20) + "..." : title;
					}
				}
			}
		}

		public void ShowInTopPane(string title, object content)
		{
			var model = DockWorkspace.Instance.ToolPanes.OfType<LegacyToolPaneModel>().FirstOrDefault(p => p.Content == content);
			if (model == null)
			{
				model = new LegacyToolPaneModel(title, content, LegacyToolPaneLocation.Top);
				DockWorkspace.Instance.ToolPanes.Add(model);
			}
			model.Show();
		}

		public void ShowInBottomPane(string title, object content)
		{
			var model = DockWorkspace.Instance.ToolPanes.OfType<LegacyToolPaneModel>().FirstOrDefault(p => p.Content == content);
			if (model == null)
			{
				model = new LegacyToolPaneModel(title, content, LegacyToolPaneLocation.Bottom);
				DockWorkspace.Instance.ToolPanes.Add(model);
			}
			model.Show();
		}
		#endregion

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
			PresentationSource source = PresentationSource.FromVisual(this);

			var sessionSettings = SettingsService.Instance.SessionSettings;

			// Validate and Set Window Bounds
			Rect bounds = Rect.Transform(sessionSettings.WindowBounds, source.CompositionTarget.TransformToDevice);
			var boundsRect = new System.Drawing.Rectangle((int)bounds.Left, (int)bounds.Top, (int)bounds.Width, (int)bounds.Height);
			bool boundsOK = false;
			foreach (var screen in System.Windows.Forms.Screen.AllScreens)
			{
				var intersection = System.Drawing.Rectangle.Intersect(boundsRect, screen.WorkingArea);
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

		public void SetStatus(string status, Brush foreground)
		{
			if (this.statusBar.Visibility == Visibility.Collapsed)
				this.statusBar.Visibility = Visibility.Visible;
			this.statusLabel.Foreground = foreground;
			this.statusLabel.Text = status;
		}

		public ItemCollection GetMainMenuItems()
		{
			return mainMenu.Items;
		}

		public ItemCollection GetToolBarItems()
		{
			return toolBar.Items;
		}
	}
}
