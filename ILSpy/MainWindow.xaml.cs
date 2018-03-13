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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Documentation;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.TreeView;
using Microsoft.Win32;
using Mono.Cecil;

namespace ICSharpCode.ILSpy
{
	/// <summary>
	/// The main window of the application.
	/// </summary>
	partial class MainWindow : Window
	{
		readonly NavigationHistory<NavigationState> history = new NavigationHistory<NavigationState>();
		ILSpySettings spySettings;
		internal SessionSettings sessionSettings;
		
		internal AssemblyListManager assemblyListManager;

		public static MainWindow Instance { get; private set; }

		public SessionSettings SessionSettings => sessionSettings;

		public MainWindow()
		{
			Instance = this;
			spySettings = ILSpySettings.Load();
			this.sessionSettings = new SessionSettings(spySettings);
			this.assemblyListManager = new AssemblyListManager(spySettings);
			
			this.Icon = new BitmapImage(new Uri("pack://application:,,,/ILSpy;component/images/ILSpy.ico"));
			
			this.DataContext = sessionSettings;
			
			InitializeComponent();
			TextView = App.ExportProvider.GetExportedValue<DecompilerTextView>();
			mainPane.Content = TextView;
			
			if (sessionSettings.SplitterPosition > 0 && sessionSettings.SplitterPosition < 1) {
				leftColumn.Width = new GridLength(sessionSettings.SplitterPosition, GridUnitType.Star);
				rightColumn.Width = new GridLength(1 - sessionSettings.SplitterPosition, GridUnitType.Star);
			}
			sessionSettings.FilterSettings.PropertyChanged += filterSettings_PropertyChanged;
			
			InitMainMenu();
			InitToolbar();
			ContextMenuProvider.Add(treeView, TextView);
			
			this.Loaded += MainWindow_Loaded;
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
			var navigationPos = 0;
			var openPos = 1;
			var toolbarCommands = App.ExportProvider.GetExports<ICommand, IToolbarCommandMetadata>("ToolbarCommand");
			foreach (var commandGroup in toolbarCommands.OrderBy(c => c.Metadata.ToolbarOrder).GroupBy(c => c.Metadata.ToolbarCategory)) {
				if (commandGroup.Key == "Navigation") {
					foreach (var command in commandGroup) {
						toolBar.Items.Insert(navigationPos++, MakeToolbarItem(command));
						openPos++;
					}
				} else if (commandGroup.Key == "Open") {
					foreach (var command in commandGroup) {
						toolBar.Items.Insert(openPos++, MakeToolbarItem(command));
					}
				} else {
					toolBar.Items.Add(new Separator());
					foreach (var command in commandGroup) {
						toolBar.Items.Add(MakeToolbarItem(command));
					}
				}
			}
			
		}
		
		Button MakeToolbarItem(Lazy<ICommand, IToolbarCommandMetadata> command)
		{
			return new Button {
				Command = CommandWrapper.Unwrap(command.Value),
				ToolTip = command.Metadata.ToolTip,
				Tag = command.Metadata.Tag,
				Content = new Image {
					Width = 16,
					Height = 16,
					Source = Images.LoadImage(command.Value, command.Metadata.ToolbarIcon)
				}
			};
		}
		#endregion
		
		#region Main Menu extensibility
		
		void InitMainMenu()
		{
			var mainMenuCommands = App.ExportProvider.GetExports<ICommand, IMainMenuCommandMetadata>("MainMenuCommand");
			foreach (var topLevelMenu in mainMenuCommands.OrderBy(c => c.Metadata.MenuOrder).GroupBy(c => c.Metadata.Menu)) {
				var topLevelMenuItem = mainMenu.Items.OfType<MenuItem>().FirstOrDefault(m => (m.Header as string) == topLevelMenu.Key);
				foreach (var category in topLevelMenu.GroupBy(c => c.Metadata.MenuCategory)) {
					if (topLevelMenuItem == null) {
						topLevelMenuItem = new MenuItem();
						topLevelMenuItem.Header = topLevelMenu.Key;
						mainMenu.Items.Add(topLevelMenuItem);
					} else if (topLevelMenuItem.Items.Count > 0) {
						topLevelMenuItem.Items.Add(new Separator());
					}
					foreach (var entry in category) {
						var menuItem = new MenuItem();
						menuItem.Command = CommandWrapper.Unwrap(entry.Value);
						if (!string.IsNullOrEmpty(entry.Metadata.Header))
							menuItem.Header = entry.Metadata.Header;
						if (!string.IsNullOrEmpty(entry.Metadata.MenuIcon)) {
							menuItem.Icon = new Image {
								Width = 16,
								Height = 16,
								Source = Images.LoadImage(entry.Value, entry.Metadata.MenuIcon)
							};
						}
						
						menuItem.IsEnabled = entry.Metadata.IsEnabled;
						menuItem.InputGestureText = entry.Metadata.InputGestureText;
						topLevelMenuItem.Items.Add(menuItem);
					}
				}
			}
		}
		#endregion
		
		#region Message Hook
		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);
			var source = PresentationSource.FromVisual(this);
			var hwndSource = source as HwndSource;
			if (hwndSource != null) {
				hwndSource.AddHook(WndProc);
			}
			// Validate and Set Window Bounds
			var bounds = Rect.Transform(sessionSettings.WindowBounds, source.CompositionTarget.TransformToDevice);
			var boundsRect = new System.Drawing.Rectangle((int)bounds.Left, (int)bounds.Top, (int)bounds.Width, (int)bounds.Height);
			var boundsOK = false;
			foreach (var screen in System.Windows.Forms.Screen.AllScreens) {
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
		
		unsafe IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == NativeMethods.WM_COPYDATA) {
				var copyData = (CopyDataStruct*)lParam;
				var data = new string((char*)copyData->Buffer, 0, copyData->Size / sizeof(char));
				if (data.StartsWith("ILSpy:\r\n", StringComparison.Ordinal)) {
					data = data.Substring(8);
					var lines = new List<string>();
					using (var r = new StringReader(data)) {
						string line;
						while ((line = r.ReadLine()) != null)
							lines.Add(line);
					}
					var args = new CommandLineArguments(lines);
					if (HandleCommandLineArguments(args)) {
						if (!args.NoActivate && WindowState == WindowState.Minimized)
							WindowState = WindowState.Normal;
						HandleCommandLineArgumentsAfterShowList(args);
						handled = true;
						return (IntPtr)1;
					}
				}
			}
			return IntPtr.Zero;
		}
		#endregion
		
		public AssemblyList CurrentAssemblyList { get; private set; }

		public event NotifyCollectionChangedEventHandler CurrentAssemblyListChanged;

		readonly List<LoadedAssembly> commandLineLoadedAssemblies = new List<LoadedAssembly>();
		
		bool HandleCommandLineArguments(CommandLineArguments args)
		{
			LoadAssemblies(args.AssembliesToLoad, commandLineLoadedAssemblies, false);
			if (args.Language != null)
				sessionSettings.FilterSettings.Language = Languages.GetLanguage(args.Language);
			return true;
		}
		
		void HandleCommandLineArgumentsAfterShowList(CommandLineArguments args)
		{
			if (args.NavigateTo != null) {
				var found = false;
				if (args.NavigateTo.StartsWith("N:", StringComparison.Ordinal)) {
					var namespaceName = args.NavigateTo.Substring(2);
					foreach (var asm in commandLineLoadedAssemblies) {
						var asmNode = AssemblyListTreeNode.FindAssemblyNode(asm);
						if (asmNode != null) {
							var nsNode = asmNode.FindNamespaceNode(namespaceName);
							if (nsNode != null) {
								found = true;
								SelectNode(nsNode);
								break;
							}
						}
					}
				} else {
					foreach (var asm in commandLineLoadedAssemblies) {
						var def = asm.GetModuleDefinitionOrNull();
						if (def != null) {
							var mr = XmlDocKeyProvider.FindMemberByKey(def, args.NavigateTo);
							if (mr != null) {
								found = true;
								JumpToReference(mr);
								break;
							}
						}
					}
				}
				if (!found) {
					var output = new AvalonEditTextOutput();
					output.Write(string.Format("Cannot find '{0}' in command line specified assemblies.", args.NavigateTo));
					TextView.ShowText(output);
				}
			} else if (commandLineLoadedAssemblies.Count == 1) {
				// NavigateTo == null and an assembly was given on the command-line:
				// Select the newly loaded assembly
				JumpToReference(commandLineLoadedAssemblies[0].GetModuleDefinitionOrNull());
			}
			if (args.Search != null)
			{
				SearchPane.Instance.SearchTerm = args.Search;
				SearchPane.Instance.Show();
			}
			commandLineLoadedAssemblies.Clear(); // clear references once we don't need them anymore
		}

		void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			var spySettings = this.spySettings;
			this.spySettings = null;

			// Load AssemblyList only in Loaded event so that WPF is initialized before we start the CPU-heavy stuff.
			// This makes the UI come up a bit faster.
			this.CurrentAssemblyList = assemblyListManager.LoadList(spySettings, sessionSettings.ActiveAssemblyList);

			HandleCommandLineArguments(App.CommandLineArguments);

			if (CurrentAssemblyList.GetAssemblies().Length == 0
				&& CurrentAssemblyList.ListName == AssemblyListManager.DefaultListName) {
				LoadInitialAssemblies();
			}

			ShowAssemblyList(this.CurrentAssemblyList);

			if (sessionSettings.ActiveAutoLoadedAssembly != null) {
				this.CurrentAssemblyList.Open(sessionSettings.ActiveAutoLoadedAssembly, true);
			}

			Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() => OpenAssemblies(spySettings)));
		}

		void OpenAssemblies(ILSpySettings spySettings)
		{
			HandleCommandLineArgumentsAfterShowList(App.CommandLineArguments);
			if (App.CommandLineArguments.NavigateTo == null && App.CommandLineArguments.AssembliesToLoad.Count != 1) {
				SharpTreeNode node = null;
				if (sessionSettings.ActiveTreeViewPath != null) {
					node = FindNodeByPath(sessionSettings.ActiveTreeViewPath, true);
					if (node == this.AssemblyListTreeNode && sessionSettings.ActiveAutoLoadedAssembly != null) {
						node = FindNodeByPath(sessionSettings.ActiveTreeViewPath, true);
					}
				}
				if (node != null) {
					SelectNode(node);
					
					// only if not showing the about page, perform the update check:
					ShowMessageIfUpdatesAvailableAsync(spySettings);
				} else {
					AboutPage.Display(TextView);
				}
			}
			
			var output = new AvalonEditTextOutput();
			if (FormatExceptions(App.StartupExceptions.ToArray(), output))
				TextView.ShowText(output);
		}
		
		bool FormatExceptions(App.ExceptionData[] exceptions, ITextOutput output)
		{
			if (exceptions.Length == 0) return false;
			var first = true;
			
			foreach (var item in exceptions) {
				if (first)
					first = false;
				else
					output.WriteLine("-------------------------------------------------");
				output.WriteLine("Error(s) loading plugin: " + item.PluginName);
				if (item.Exception is System.Reflection.ReflectionTypeLoadException) {
					var e = (System.Reflection.ReflectionTypeLoadException)item.Exception;
					foreach (var ex in e.LoaderExceptions) {
						output.WriteLine(ex.ToString());
						output.WriteLine();
					}
				} else
					output.WriteLine(item.Exception.ToString());
			}
			
			return true;
		}
		
		#region Update Check
		string updateAvailableDownloadUrl;
		
		public void ShowMessageIfUpdatesAvailableAsync(ILSpySettings spySettings, bool forceCheck = false)
		{
			Task<string> result;
			if (forceCheck) {
				result = AboutPage.CheckForUpdatesAsync(spySettings);
			} else {
				result = AboutPage.CheckForUpdatesIfEnabledAsync(spySettings);
			}
			result.ContinueWith(task => AdjustUpdateUIAfterCheck(task, forceCheck), TaskScheduler.FromCurrentSynchronizationContext());
		}
		
		void updatePanelCloseButtonClick(object sender, RoutedEventArgs e)
		{
			updatePanel.Visibility = Visibility.Collapsed;
		}
		
		void downloadOrCheckUpdateButtonClick(object sender, RoutedEventArgs e)
		{
			if (updateAvailableDownloadUrl != null) {
				MainWindow.OpenLink(updateAvailableDownloadUrl);
			} else {
				updatePanel.Visibility = Visibility.Collapsed;
				AboutPage.CheckForUpdatesAsync(spySettings ?? ILSpySettings.Load())
					.ContinueWith(task => AdjustUpdateUIAfterCheck(task, true), TaskScheduler.FromCurrentSynchronizationContext());
			}
		}

		void AdjustUpdateUIAfterCheck(Task<string> task, bool displayMessage)
		{
			updateAvailableDownloadUrl = task.Result;
			updatePanel.Visibility = displayMessage ? Visibility.Visible : Visibility.Collapsed;
			if (task.Result != null) {
				updatePanelMessage.Text = "A new ILSpy version is available.";
				downloadOrCheckUpdateButton.Content = "Download";
			} else {
				updatePanelMessage.Text = "No update for ILSpy found.";
				downloadOrCheckUpdateButton.Content = "Check again";
			}
		}
		#endregion
		
		public void ShowAssemblyList(string name)
		{
			var settings = this.spySettings;
			if (settings == null)
			{
				settings = ILSpySettings.Load();
			}
			var list = this.assemblyListManager.LoadList(settings, name);
			//Only load a new list when it is a different one
			if (list.ListName != CurrentAssemblyList.ListName)
			{
				ShowAssemblyList(list);
			}
		}
		
		void ShowAssemblyList(AssemblyList assemblyList)
		{
			history.Clear();
			this.CurrentAssemblyList = assemblyList;
			
			assemblyList.assemblies.CollectionChanged += assemblyList_Assemblies_CollectionChanged;
			
			AssemblyListTreeNode = new AssemblyListTreeNode(assemblyList);
			AssemblyListTreeNode.FilterSettings = sessionSettings.FilterSettings.Clone();
			AssemblyListTreeNode.Select = SelectNode;
			treeView.Root = AssemblyListTreeNode;
			
			if (assemblyList.ListName == AssemblyListManager.DefaultListName)
				this.Title = "ILSpy";
			else
				this.Title = "ILSpy - " + assemblyList.ListName;
		}
		
		void assemblyList_Assemblies_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Reset) {
				history.RemoveAll(_ => true);
			}
			if (e.OldItems != null) {
				var oldAssemblies = new HashSet<LoadedAssembly>(e.OldItems.Cast<LoadedAssembly>());
				history.RemoveAll(n => n.TreeNodes.Any(
					nd => nd.AncestorsAndSelf().OfType<AssemblyTreeNode>().Any(
						a => oldAssemblies.Contains(a.LoadedAssembly))));
			}
			if (CurrentAssemblyListChanged != null)
				CurrentAssemblyListChanged(this, e);
		}
		
		void LoadInitialAssemblies()
		{
			// Called when loading an empty assembly list; so that
			// the user can see something initially.
			System.Reflection.Assembly[] initialAssemblies = {
				typeof(object).Assembly,
				typeof(Uri).Assembly,
				typeof(System.Linq.Enumerable).Assembly,
				typeof(System.Xml.XmlDocument).Assembly,
				typeof(System.Windows.Markup.MarkupExtension).Assembly,
				typeof(System.Windows.Rect).Assembly,
				typeof(System.Windows.UIElement).Assembly,
				typeof(System.Windows.FrameworkElement).Assembly
			};
			foreach (var asm in initialAssemblies)
				CurrentAssemblyList.OpenAssembly(asm.Location);
		}
		
		void filterSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			RefreshTreeViewFilter();
			if (e.PropertyName == "Language" || e.PropertyName == "LanguageVersion") {
				DecompileSelectedNodes(recordHistory: false);
			}
		}
		
		public void RefreshTreeViewFilter()
		{
			// filterSettings is mutable; but the ILSpyTreeNode filtering assumes that filter settings are immutable.
			// Thus, the main window will use one mutable instance (for data-binding), and assign a new clone to the ILSpyTreeNodes whenever the main
			// mutable instance changes.
			if (AssemblyListTreeNode != null)
				AssemblyListTreeNode.FilterSettings = sessionSettings.FilterSettings.Clone();
		}
		
		internal AssemblyListTreeNode AssemblyListTreeNode { get; private set; }

		#region Node Selection

		public void SelectNode(SharpTreeNode obj)
		{
			if (obj != null) {
				if (!obj.AncestorsAndSelf().Any(node => node.IsHidden)) {
					// Set both the selection and focus to ensure that keyboard navigation works as expected.
					treeView.FocusNode(obj);
					treeView.SelectedItem = obj;
				} else {
					MessageBox.Show("Navigation failed because the target is hidden or a compiler-generated class.\n" +
						"Please disable all filters that might hide the item (i.e. activate " +
						"\"View > Show internal types and members\") and try again.",
						"ILSpy", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				}
			}
		}

		public void SelectNodes(IEnumerable<SharpTreeNode> nodes)
		{
			if (nodes.Any() && nodes.All(n => !n.AncestorsAndSelf().Any(a => a.IsHidden))) {
				treeView.FocusNode(nodes.First());
				treeView.SetSelectedNodes(nodes);
			}
		}
		
		/// <summary>
		/// Retrieves a node using the .ToString() representations of its ancestors.
		/// </summary>
		public SharpTreeNode FindNodeByPath(string[] path, bool returnBestMatch)
		{
			if (path == null)
				return null;
			var node = treeView.Root;
			var bestMatch = node;
			foreach (var element in path) {
				if (node == null)
					break;
				bestMatch = node;
				node.EnsureLazyChildren();
				var ilSpyTreeNode = node as ILSpyTreeNode;
				if (ilSpyTreeNode != null)
					ilSpyTreeNode.EnsureChildrenFiltered();
				node = node.Children.FirstOrDefault(c => c.ToString() == element);
			}
			if (returnBestMatch)
				return node ?? bestMatch;
			else
				return node;
		}
		
		/// <summary>
		/// Gets the .ToString() representation of the node's ancestors.
		/// </summary>
		public static string[] GetPathForNode(SharpTreeNode node)
		{
			if (node == null)
				return null;
			var path = new List<string>();
			while (node.Parent != null) {
				path.Add(node.ToString());
				node = node.Parent;
			}
			path.Reverse();
			return path.ToArray();
		}
		
		public ILSpyTreeNode FindTreeNode(object reference)
		{
			if (reference is TypeReference)
			{
				return AssemblyListTreeNode.FindTypeNode(((TypeReference)reference).Resolve());
			}
			else if (reference is MethodReference)
			{
				return AssemblyListTreeNode.FindMethodNode(((MethodReference)reference).Resolve());
			}
			else if (reference is FieldReference)
			{
				return AssemblyListTreeNode.FindFieldNode(((FieldReference)reference).Resolve());
			}
			else if (reference is PropertyReference)
			{
				return AssemblyListTreeNode.FindPropertyNode(((PropertyReference)reference).Resolve());
			}
			else if (reference is EventReference)
			{
				return AssemblyListTreeNode.FindEventNode(((EventReference)reference).Resolve());
			}
			else if (reference is AssemblyDefinition)
			{
				return AssemblyListTreeNode.FindAssemblyNode((AssemblyDefinition)reference);
			}
			else if (reference is ModuleDefinition)
			{
				return AssemblyListTreeNode.FindAssemblyNode((ModuleDefinition)reference);
			}
			else if (reference is Resource)
			{
				return AssemblyListTreeNode.FindResourceNode((Resource)reference);
			}
			else
			{
				return null;
			}
		}
		
		public void JumpToReference(object reference)
		{
			JumpToReferenceAsync(reference).HandleExceptions();
		}
		
		/// <summary>
		/// Jumps to the specified reference.
		/// </summary>
		/// <returns>
		/// Returns a task that will signal completion when the decompilation of the jump target has finished.
		/// The task will be marked as canceled if the decompilation is canceled.
		/// </returns>
		public Task JumpToReferenceAsync(object reference)
		{
			decompilationTask = TaskHelper.CompletedTask;
			var treeNode = FindTreeNode(reference);
			if (treeNode != null) {
				SelectNode(treeNode);
			} else if (reference is Mono.Cecil.Cil.OpCode) {
				var link = "http://msdn.microsoft.com/library/system.reflection.emit.opcodes." + ((Mono.Cecil.Cil.OpCode)reference).Code.ToString().ToLowerInvariant() + ".aspx";
				OpenLink(link);
			}
			return decompilationTask;
		}

		public static void OpenLink(string link)
		{
			try {
				Process.Start(link);
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
			} catch (Exception) {
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
				// Process.Start can throw several errors (not all of them documented),
				// just ignore all of them.
			}
		}
		#endregion
		
		#region Open/Refresh
		void OpenCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;
			var dlg = new OpenFileDialog();
			dlg.Filter = ".NET assemblies|*.dll;*.exe;*.winmd|Nuget Packages (*.nupkg)|*.nupkg|All files|*.*";
			dlg.Multiselect = true;
			dlg.RestoreDirectory = true;
			if (dlg.ShowDialog() == true) {
				OpenFiles(dlg.FileNames);
			}
		}
		
		public void OpenFiles(string[] fileNames, bool focusNode = true)
		{
			if (fileNames == null)
				throw new ArgumentNullException(nameof(fileNames));
			
			if (focusNode)
				treeView.UnselectAll();

			LoadAssemblies(fileNames, focusNode: focusNode);
		}

		void LoadAssemblies(IEnumerable<string> fileNames, List<LoadedAssembly> loadedAssemblies = null, bool focusNode = true)
		{
			SharpTreeNode lastNode = null;
			foreach (var file in fileNames) {
				switch (Path.GetExtension(file)) {
					case ".nupkg":
						var package = new LoadedNugetPackage(file);
						var selectionDialog = new NugetPackageBrowserDialog(package);
						selectionDialog.Owner = this;
						if (selectionDialog.ShowDialog() != true)
							break;
						foreach (var entry in selectionDialog.SelectedItems) {
							var nugetAsm = CurrentAssemblyList.OpenAssembly("nupkg://" + file + ";" + entry.Name, entry.Stream, true);
							if (nugetAsm != null) {
								if (loadedAssemblies != null)
									loadedAssemblies.Add(nugetAsm);
								else {
									var node = AssemblyListTreeNode.FindAssemblyNode(nugetAsm);
									if (node != null && focusNode) {
										treeView.SelectedItems.Add(node);
										lastNode = node;
									}
								}
							}
						}
						break;
					default:
						var asm = CurrentAssemblyList.OpenAssembly(file);
						if (asm != null) {
							if (loadedAssemblies != null)
								loadedAssemblies.Add(asm);
							else {
								var node = AssemblyListTreeNode.FindAssemblyNode(asm);
								if (node != null && focusNode) {
									treeView.SelectedItems.Add(node);
									lastNode = node;
								}
							}
						}
						break;
				}

				if (lastNode != null && focusNode)
					treeView.FocusNode(lastNode);
			}
		}
		
		void RefreshCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			var path = GetPathForNode(treeView.SelectedItem as SharpTreeNode);
			ShowAssemblyList(assemblyListManager.LoadList(ILSpySettings.Load(), CurrentAssemblyList.ListName));
			SelectNode(FindNodeByPath(path, true));
		}
		
		void SearchCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			SearchPane.Instance.Show();
		}
		#endregion
		
		#region Decompile (TreeView_SelectionChanged)
		void TreeView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			DecompileSelectedNodes();

			if (SelectionChanged != null)
				SelectionChanged(sender, e);
		}
		
		Task decompilationTask;
		bool ignoreDecompilationRequests;
		
		void DecompileSelectedNodes(DecompilerTextViewState state = null, bool recordHistory = true)
		{
			if (ignoreDecompilationRequests)
				return;
			
			if (recordHistory) {
				var dtState = TextView.GetState();
				if(dtState != null)
					history.UpdateCurrent(new NavigationState(dtState));
				history.Record(new NavigationState(treeView.SelectedItems.OfType<SharpTreeNode>()));
			}
			
			if (treeView.SelectedItems.Count == 1) {
				var node = treeView.SelectedItem as ILSpyTreeNode;
				if (node != null && node.View(TextView))
					return;
			}
			decompilationTask = TextView.DecompileAsync(this.CurrentLanguage, this.SelectedNodes, new DecompilationOptions() { TextViewState = state });
		}
		
		void SaveCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			if (this.SelectedNodes.Count() == 1) {
				if (this.SelectedNodes.Single().Save(this.TextView))
					return;
			}
			this.TextView.SaveToDisk(this.CurrentLanguage,
				this.SelectedNodes,
				new DecompilationOptions() { FullDecompilation = true });
		}
		
		public void RefreshDecompiledView()
		{
			DecompileSelectedNodes();
		}
		
		public DecompilerTextView TextView { get; }

		public Language CurrentLanguage => sessionSettings.FilterSettings.Language;
		public LanguageVersion CurrentLanguageVersion => sessionSettings.FilterSettings.LanguageVersion;

		public event SelectionChangedEventHandler SelectionChanged;

		public IEnumerable<ILSpyTreeNode> SelectedNodes => treeView.GetTopLevelSelection().OfType<ILSpyTreeNode>();

		#endregion

		#region Back/Forward navigation
		void BackCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;
			e.CanExecute = history.CanNavigateBack;
		}
		
		void BackCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			if (history.CanNavigateBack) {
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
			if (history.CanNavigateForward) {
				e.Handled = true;
				NavigateHistory(true);
			}
		}
		
		void NavigateHistory(bool forward)
		{
			var dtState = TextView.GetState();
			if(dtState != null)
				history.UpdateCurrent(new NavigationState(dtState));
			var newState = forward ? history.GoForward() : history.GoBack();
			
			ignoreDecompilationRequests = true;
			treeView.SelectedItems.Clear();
			foreach (var node in newState.TreeNodes)
			{
				treeView.SelectedItems.Add(node);
			}
			if (newState.TreeNodes.Any())
				treeView.FocusNode(newState.TreeNodes.First());
			ignoreDecompilationRequests = false;
			DecompileSelectedNodes(newState.ViewState, false);
		}
		
		#endregion
		
		protected override void OnStateChanged(EventArgs e)
		{
			base.OnStateChanged(e);
			// store window state in settings only if it's not minimized
			if (this.WindowState != System.Windows.WindowState.Minimized)
				sessionSettings.WindowState = this.WindowState;
		}
		
		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			sessionSettings.ActiveAssemblyList = CurrentAssemblyList.ListName;
			sessionSettings.ActiveTreeViewPath = GetPathForNode(treeView.SelectedItem as SharpTreeNode);
			sessionSettings.ActiveAutoLoadedAssembly = GetAutoLoadedAssemblyNode(treeView.SelectedItem as SharpTreeNode);
			sessionSettings.WindowBounds = this.RestoreBounds;
			sessionSettings.SplitterPosition = leftColumn.Width.Value / (leftColumn.Width.Value + rightColumn.Width.Value);
			if (topPane.Visibility == Visibility.Visible)
				sessionSettings.TopPaneSplitterPosition = topPaneRow.Height.Value / (topPaneRow.Height.Value + textViewRow.Height.Value);
			if (bottomPane.Visibility == Visibility.Visible)
				sessionSettings.BottomPaneSplitterPosition = bottomPaneRow.Height.Value / (bottomPaneRow.Height.Value + textViewRow.Height.Value);
			sessionSettings.Save();
		}

		private string GetAutoLoadedAssemblyNode(SharpTreeNode node)
		{
			if (node == null)
				return null;
			while (!(node is TreeNodes.AssemblyTreeNode) && node.Parent != null) {
				node = node.Parent;
			}
			//this should be an assembly node
			var assyNode = node as TreeNodes.AssemblyTreeNode;
			var loadedAssy = assyNode.LoadedAssembly;
			if (!(loadedAssy.IsLoaded && loadedAssy.IsAutoLoaded))
				return null;

			return loadedAssy.FileName;
		}
		
		#region Top/Bottom Pane management

		/// <summary>
		///   When grid is resized using splitter, row height value could become greater than 1.
		///   As result, when a new pane is shown, both textView and pane could become very small.
		///   This method normalizes two rows and ensures that height is less then 1.
		/// </summary>
		void NormalizePaneRowHeightValues(RowDefinition pane1Row, RowDefinition pane2Row)
		{
			var pane1Height = pane1Row.Height;
			var pane2Height = pane2Row.Height;

			//only star height values are normalized.
			if (!pane1Height.IsStar || !pane2Height.IsStar)
			{
				return;
			}

			var totalHeight = pane1Height.Value + pane2Height.Value;
			if (totalHeight == 0)
			{
				return;
			}

			pane1Row.Height = new GridLength(pane1Height.Value / totalHeight, GridUnitType.Star);
			pane2Row.Height = new GridLength(pane2Height.Value / totalHeight, GridUnitType.Star);
		}

		public void ShowInTopPane(string title, object content)
		{
			topPaneRow.MinHeight = 100;
			if (sessionSettings.TopPaneSplitterPosition > 0 && sessionSettings.TopPaneSplitterPosition < 1) {
				//Ensure all 3 blocks are in fair conditions
				NormalizePaneRowHeightValues(bottomPaneRow, textViewRow);

				textViewRow.Height = new GridLength(1 - sessionSettings.TopPaneSplitterPosition, GridUnitType.Star);
				topPaneRow.Height = new GridLength(sessionSettings.TopPaneSplitterPosition, GridUnitType.Star);
			}
			topPane.Title = title;
			if (topPane.Content != content) {
				var pane = topPane.Content as IPane;
				if (pane != null)
					pane.Closed();
				topPane.Content = content;
			}
			topPane.Visibility = Visibility.Visible;
		}
		
		void TopPane_CloseButtonClicked(object sender, EventArgs e)
		{
			sessionSettings.TopPaneSplitterPosition = topPaneRow.Height.Value / (topPaneRow.Height.Value + textViewRow.Height.Value);
			topPaneRow.MinHeight = 0;
			topPaneRow.Height = new GridLength(0);
			topPane.Visibility = Visibility.Collapsed;
			
			var pane = topPane.Content as IPane;
			topPane.Content = null;
			if (pane != null)
				pane.Closed();
		}
		
		public void ShowInBottomPane(string title, object content)
		{
			bottomPaneRow.MinHeight = 100;
			if (sessionSettings.BottomPaneSplitterPosition > 0 && sessionSettings.BottomPaneSplitterPosition < 1) {
				//Ensure all 3 blocks are in fair conditions
				NormalizePaneRowHeightValues(topPaneRow, textViewRow);

				textViewRow.Height = new GridLength(1 - sessionSettings.BottomPaneSplitterPosition, GridUnitType.Star);
				bottomPaneRow.Height = new GridLength(sessionSettings.BottomPaneSplitterPosition, GridUnitType.Star);
			}
			bottomPane.Title = title;
			if (bottomPane.Content != content) {
				var pane = bottomPane.Content as IPane;
				if (pane != null)
					pane.Closed();
				bottomPane.Content = content;
			}
			bottomPane.Visibility = Visibility.Visible;
		}
		
		void BottomPane_CloseButtonClicked(object sender, EventArgs e)
		{
			sessionSettings.BottomPaneSplitterPosition = bottomPaneRow.Height.Value / (bottomPaneRow.Height.Value + textViewRow.Height.Value);
			bottomPaneRow.MinHeight = 0;
			bottomPaneRow.Height = new GridLength(0);
			bottomPane.Visibility = Visibility.Collapsed;
			
			var pane = bottomPane.Content as IPane;
			bottomPane.Content = null;
			if (pane != null)
				pane.Closed();
		}
		#endregion
		
		public void UnselectAll()
		{
			treeView.UnselectAll();
		}
		
		public void SetStatus(string status, Brush foreground)
		{
			if (this.statusBar.Visibility == Visibility.Collapsed)
				this.statusBar.Visibility = Visibility.Visible;
			this.StatusLabel.Foreground = foreground;
			this.StatusLabel.Text = status;
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
