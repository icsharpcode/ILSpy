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
using System.Reflection.Metadata;
using System.Text;
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
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.ILSpy.Analyzers;
using ICSharpCode.ILSpy.Controls;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.TreeView;
using Microsoft.Win32;
using OSVersionHelper;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace ICSharpCode.ILSpy
{
	class MainWindowDataContext
	{
		public DockWorkspace Workspace { get; set; }
		public SessionSettings SessionSettings { get; set; }
	}

	/// <summary>
	/// The main window of the application.
	/// </summary>
	partial class MainWindow : Window
	{
		bool refreshInProgress;
		bool handlingNugetPackageSelection;
		readonly NavigationHistory<NavigationState> history = new NavigationHistory<NavigationState>();
		ILSpySettings spySettingsForMainWindow_Loaded;
		internal SessionSettings sessionSettings;

		internal AssemblyListManager assemblyListManager;
		AssemblyList assemblyList;
		AssemblyListTreeNode assemblyListTreeNode;

		static MainWindow instance;

		public static MainWindow Instance {
			get { return instance; }
		}

		public SessionSettings SessionSettings {
			get { return sessionSettings; }
		}

		public SharpTreeView treeView {
			get {
				return FindResource("TreeView") as SharpTreeView;
			}
		}

		public AnalyzerTreeView AnalyzerTreeView {
			get {
				return FindResource("AnalyzerTreeView") as AnalyzerTreeView;
			}
		}

		public SearchPane SearchPane {
			get {
				return FindResource("SearchPane") as SearchPane;
			}
		}

		public MainWindow()
		{
			instance = this;
			var spySettings = ILSpySettings.Load();
			this.spySettingsForMainWindow_Loaded = spySettings;
			this.sessionSettings = new SessionSettings(spySettings);
			this.assemblyListManager = new AssemblyListManager(spySettings);

			this.Icon = new BitmapImage(new Uri("pack://application:,,,/ILSpy;component/images/ILSpy.ico"));

			this.DataContext = new MainWindowDataContext {
				Workspace = DockWorkspace.Instance,
				SessionSettings = sessionSettings
			};

			DockWorkspace.Instance.LoadSettings(sessionSettings);
			InitializeComponent();
			DockWorkspace.Instance.InitializeLayout(DockManager);
			sessionSettings.FilterSettings.PropertyChanged += filterSettings_PropertyChanged;

			InitMainMenu();
			InitToolbar();
			ContextMenuProvider.Add(treeView);

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
			int navigationPos = 0;
			int openPos = 1;
			var toolbarCommands = App.ExportProvider.GetExports<ICommand, IToolbarCommandMetadata>("ToolbarCommand");
			foreach (var commandGroup in toolbarCommands.OrderBy(c => c.Metadata.ToolbarOrder).GroupBy(c => Properties.Resources.ResourceManager.GetString(c.Metadata.ToolbarCategory))) {
				if (commandGroup.Key == Properties.Resources.ResourceManager.GetString("Navigation")) {
					foreach (var command in commandGroup) {
						toolBar.Items.Insert(navigationPos++, MakeToolbarItem(command));
						openPos++;
					}
				} else if (commandGroup.Key == Properties.Resources.ResourceManager.GetString("Open")) {
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
				ToolTip = Properties.Resources.ResourceManager.GetString(command.Metadata.ToolTip),
				Tag = command.Metadata.Tag,
				Content = new Image {
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
			foreach (var topLevelMenu in mainMenuCommands.OrderBy(c => c.Metadata.MenuOrder).GroupBy(c => GetResourceString(c.Metadata.Menu))) {
				var topLevelMenuItem = mainMenu.Items.OfType<MenuItem>().FirstOrDefault(m => (GetResourceString(m.Header as string)) == topLevelMenu.Key);
				foreach (var category in topLevelMenu.GroupBy(c => GetResourceString(c.Metadata.MenuCategory))) {
					if (topLevelMenuItem == null) {
						topLevelMenuItem = new MenuItem();
						topLevelMenuItem.Header = GetResourceString(topLevelMenu.Key);
						mainMenu.Items.Add(topLevelMenuItem);
					} else if (topLevelMenuItem.Items.Count > 0) {
						topLevelMenuItem.Items.Add(new Separator());
					}
					foreach (var entry in category) {
						MenuItem menuItem = new MenuItem();
						menuItem.Command = CommandWrapper.Unwrap(entry.Value);
						if (!string.IsNullOrEmpty(GetResourceString(entry.Metadata.Header)))
							menuItem.Header = GetResourceString(entry.Metadata.Header);
						if (!string.IsNullOrEmpty(entry.Metadata.MenuIcon)) {
							menuItem.Icon = new Image {
								Width = 16,
								Height = 16,
								Source = Images.Load(entry.Value, entry.Metadata.MenuIcon)
							};
						}

						menuItem.IsEnabled = entry.Metadata.IsEnabled;
						menuItem.InputGestureText = entry.Metadata.InputGestureText;
						topLevelMenuItem.Items.Add(menuItem);
					}
				}
			}
		}

		internal static string GetResourceString(string key)
		{
			var str = !string.IsNullOrEmpty(key) ? Properties.Resources.ResourceManager.GetString(key) : null;
			return string.IsNullOrEmpty(key) || string.IsNullOrEmpty(str) ? key : str;
		}
		#endregion

		#region Message Hook
		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);
			PresentationSource source = PresentationSource.FromVisual(this);
			HwndSource hwndSource = source as HwndSource;
			if (hwndSource != null) {
				hwndSource.AddHook(WndProc);
			}
			// Validate and Set Window Bounds
			Rect bounds = Rect.Transform(sessionSettings.WindowBounds, source.CompositionTarget.TransformToDevice);
			var boundsRect = new System.Drawing.Rectangle((int)bounds.Left, (int)bounds.Top, (int)bounds.Width, (int)bounds.Height);
			bool boundsOK = false;
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
				CopyDataStruct* copyData = (CopyDataStruct*)lParam;
				string data = new string((char*)copyData->Buffer, 0, copyData->Size / sizeof(char));
				if (data.StartsWith("ILSpy:\r\n", StringComparison.Ordinal)) {
					if (handlingNugetPackageSelection)
						return (IntPtr)1;
					data = data.Substring(8);
					List<string> lines = new List<string>();
					using (StringReader r = new StringReader(data)) {
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

		public AssemblyList CurrentAssemblyList {
			get { return assemblyList; }
		}

		public event NotifyCollectionChangedEventHandler CurrentAssemblyListChanged;

		List<LoadedAssembly> commandLineLoadedAssemblies = new List<LoadedAssembly>();

		List<string> nugetPackagesToLoad = new List<string>();

		bool HandleCommandLineArguments(CommandLineArguments args)
		{
			int i = 0;
			while (i < args.AssembliesToLoad.Count) {
				var asm = args.AssembliesToLoad[i];
				if (Path.GetExtension(asm) == ".nupkg") {
					nugetPackagesToLoad.Add(asm);
					args.AssembliesToLoad.RemoveAt(i);
				} else {
					i++;
				}
			}
			LoadAssemblies(args.AssembliesToLoad, commandLineLoadedAssemblies, focusNode: false);
			if (args.Language != null)
				sessionSettings.FilterSettings.Language = Languages.GetLanguage(args.Language);
			return true;
		}

		/// <summary>
		/// Called on startup or when passed arguments via WndProc from a second instance.
		/// In the format case, spySettings is non-null; in the latter it is null.
		/// </summary>
		void HandleCommandLineArgumentsAfterShowList(CommandLineArguments args, ILSpySettings spySettings = null)
		{
			if (nugetPackagesToLoad.Count > 0) {
				var relevantPackages = nugetPackagesToLoad.ToArray();
				nugetPackagesToLoad.Clear();
				// Show the nuget package open dialog after the command line/window message was processed.
				Dispatcher.BeginInvoke(new Action(() => LoadAssemblies(relevantPackages, commandLineLoadedAssemblies, focusNode: false)), DispatcherPriority.Normal);
			}
			var relevantAssemblies = commandLineLoadedAssemblies.ToList();
			commandLineLoadedAssemblies.Clear(); // clear references once we don't need them anymore
			NavigateOnLaunch(args.NavigateTo, sessionSettings.ActiveTreeViewPath, spySettings, relevantAssemblies);
			if (args.Search != null) {
				SearchPane.SearchTerm = args.Search;
				SearchPane.Show();
			}
		}

		async void NavigateOnLaunch(string navigateTo, string[] activeTreeViewPath, ILSpySettings spySettings, List<LoadedAssembly> relevantAssemblies)
		{
			var initialSelection = treeView.SelectedItem;
			if (navigateTo != null) {
				bool found = false;
				if (navigateTo.StartsWith("N:", StringComparison.Ordinal)) {
					string namespaceName = navigateTo.Substring(2);
					foreach (LoadedAssembly asm in relevantAssemblies) {
						AssemblyTreeNode asmNode = assemblyListTreeNode.FindAssemblyNode(asm);
						if (asmNode != null) {
							// FindNamespaceNode() blocks the UI if the assembly is not yet loaded,
							// so use an async wait instead.
							await asm.GetPEFileAsync().Catch<Exception>(ex => { });
							NamespaceTreeNode nsNode = asmNode.FindNamespaceNode(namespaceName);
							if (nsNode != null) {
								found = true;
								if (treeView.SelectedItem == initialSelection) {
									SelectNode(nsNode);
								}
								break;
							}
						}
					}
				} else if (navigateTo == "none") {
					// Don't navigate anywhere; start empty.
					// Used by ILSpy VS addin, it'll send us the real location to navigate to via IPC.
					found = true;
				} else {
					IEntity mr = await Task.Run(() => FindEntityInRelevantAssemblies(navigateTo, relevantAssemblies));
					if (mr != null && mr.ParentModule.PEFile != null) {
						found = true;
						if (treeView.SelectedItem == initialSelection) {
							JumpToReference(mr);
						}
					}
				}
				if (!found && treeView.SelectedItem == initialSelection) {
					AvalonEditTextOutput output = new AvalonEditTextOutput();
					output.Write(string.Format("Cannot find '{0}' in command line specified assemblies.", navigateTo));
					DockWorkspace.Instance.ShowText(output);
				}
			} else if (relevantAssemblies.Count == 1) {
				// NavigateTo == null and an assembly was given on the command-line:
				// Select the newly loaded assembly
				AssemblyTreeNode asmNode = assemblyListTreeNode.FindAssemblyNode(relevantAssemblies[0]);
				if (asmNode != null && treeView.SelectedItem == initialSelection) {
					SelectNode(asmNode);
				}
			} else if (spySettings != null) {
				SharpTreeNode node = null;
				if (activeTreeViewPath?.Length > 0) {
					foreach (var asm in assemblyList.GetAssemblies()) {
						if (asm.FileName == activeTreeViewPath[0]) {
							// FindNodeByPath() blocks the UI if the assembly is not yet loaded,
							// so use an async wait instead.
							await asm.GetPEFileAsync().Catch<Exception>(ex => { });
						}
					}
					node = FindNodeByPath(activeTreeViewPath, true);
				}
				if (treeView.SelectedItem == initialSelection) {
					if (node != null) {
						SelectNode(node);

						// only if not showing the about page, perform the update check:
						await ShowMessageIfUpdatesAvailableAsync(spySettings);
					} else {
						AboutPage.Display(DockWorkspace.Instance.GetTextView());
					}
				}
			}
		}

		internal static IEntity FindEntityInRelevantAssemblies(string navigateTo, IEnumerable<LoadedAssembly> relevantAssemblies)
		{
			ITypeReference typeRef = null;
			IMemberReference memberRef = null;
			if (navigateTo.StartsWith("T:", StringComparison.Ordinal)) {
				typeRef = IdStringProvider.ParseTypeName(navigateTo);
			} else {
				memberRef = IdStringProvider.ParseMemberIdString(navigateTo);
				typeRef = memberRef.DeclaringTypeReference;
			}
			foreach (LoadedAssembly asm in relevantAssemblies.ToList()) {
				var module = asm.GetPEFileOrNull();
				if (CanResolveTypeInPEFile(module, typeRef, out var typeHandle)) {
					ICompilation compilation = typeHandle.Kind == HandleKind.ExportedType
						? new DecompilerTypeSystem(module, module.GetAssemblyResolver())
						: new SimpleCompilation(module, MinimalCorlib.Instance);
					return memberRef == null
						? typeRef.Resolve(new SimpleTypeResolveContext(compilation)) as ITypeDefinition
						: (IEntity)memberRef.Resolve(new SimpleTypeResolveContext(compilation));
				}
			}
			return null;
		}

		static bool CanResolveTypeInPEFile(PEFile module, ITypeReference typeRef, out EntityHandle typeHandle)
		{
			if (module == null) {
				typeHandle = default;
				return false;
			}

			switch (typeRef) {
				case GetPotentiallyNestedClassTypeReference topLevelType:
					typeHandle = topLevelType.ResolveInPEFile(module);
					return !typeHandle.IsNil;
				case NestedTypeReference nestedType:
					if (!CanResolveTypeInPEFile(module, nestedType.DeclaringTypeReference, out typeHandle))
						return false;
					if (typeHandle.Kind == HandleKind.ExportedType)
						return true;
					var typeDef = module.Metadata.GetTypeDefinition((TypeDefinitionHandle)typeHandle);
					typeHandle = typeDef.GetNestedTypes().FirstOrDefault(t => {
						var td = module.Metadata.GetTypeDefinition(t);
						var typeName = ReflectionHelper.SplitTypeParameterCountFromReflectionName(module.Metadata.GetString(td.Name), out int typeParameterCount);
						return nestedType.AdditionalTypeParameterCount == typeParameterCount && nestedType.Name == typeName;
					});
					return !typeHandle.IsNil;
				default:
					typeHandle = default;
					return false;
			}
		}

		void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			DockWorkspace.Instance.Documents.Add(new DecompiledDocumentModel() {
				Language = CurrentLanguage,
				LanguageVersion = CurrentLanguageVersion
			});
			DockWorkspace.Instance.ActiveDocument = DockWorkspace.Instance.Documents.First();

			ILSpySettings spySettings = this.spySettingsForMainWindow_Loaded;
			this.spySettingsForMainWindow_Loaded = null;
			var loadPreviousAssemblies = Options.MiscSettingsPanel.CurrentMiscSettings.LoadPreviousAssemblies;

			if (loadPreviousAssemblies) {
				// Load AssemblyList only in Loaded event so that WPF is initialized before we start the CPU-heavy stuff.
				// This makes the UI come up a bit faster.
				this.assemblyList = assemblyListManager.LoadList(spySettings, sessionSettings.ActiveAssemblyList);
			} else {
				this.assemblyList = new AssemblyList(AssemblyListManager.DefaultListName);
				assemblyListManager.ClearAll();
			}

			HandleCommandLineArguments(App.CommandLineArguments);

			if (assemblyList.GetAssemblies().Length == 0
				&& assemblyList.ListName == AssemblyListManager.DefaultListName
				&& loadPreviousAssemblies) {
				LoadInitialAssemblies();
			}

			ShowAssemblyList(this.assemblyList);

			if (sessionSettings.ActiveAutoLoadedAssembly != null) {
				this.assemblyList.Open(sessionSettings.ActiveAutoLoadedAssembly, true);
			}

			Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() => OpenAssemblies(spySettings)));
		}

		void OpenAssemblies(ILSpySettings spySettings)
		{
			HandleCommandLineArgumentsAfterShowList(App.CommandLineArguments, spySettings);

			AvalonEditTextOutput output = new AvalonEditTextOutput();
			if (FormatExceptions(App.StartupExceptions.ToArray(), output))
				DockWorkspace.Instance.ShowText(output);
		}

		bool FormatExceptions(App.ExceptionData[] exceptions, ITextOutput output)
		{
			var stringBuilder = new StringBuilder();
			var result = FormatExceptions(exceptions, stringBuilder);
			if (result) {
				output.Write(stringBuilder.ToString());
			}
			return result;
		}

		internal static bool FormatExceptions(App.ExceptionData[] exceptions, StringBuilder output)
		{
			if (exceptions.Length == 0) return false;
			bool first = true;

			foreach (var item in exceptions) {
				if (first)
					first = false;
				else
					output.AppendLine("-------------------------------------------------");
				output.AppendLine("Error(s) loading plugin: " + item.PluginName);
				if (item.Exception is System.Reflection.ReflectionTypeLoadException) {
					var e = (System.Reflection.ReflectionTypeLoadException)item.Exception;
					foreach (var ex in e.LoaderExceptions) {
						output.AppendLine(ex.ToString());
						output.AppendLine();
					}
				} else
					output.AppendLine(item.Exception.ToString());
			}

			return true;
		}

		#region Update Check
		string updateAvailableDownloadUrl;

		public async Task ShowMessageIfUpdatesAvailableAsync(ILSpySettings spySettings, bool forceCheck = false)
		{
			// Don't check for updates if we're in an MSIX since they work differently
			if (WindowsVersionHelper.HasPackageIdentity) {
				return;
			}

			string downloadUrl;
			if (forceCheck) {
				downloadUrl = await AboutPage.CheckForUpdatesAsync(spySettings);
			} else {
				downloadUrl = await AboutPage.CheckForUpdatesIfEnabledAsync(spySettings);
			}

			AdjustUpdateUIAfterCheck(downloadUrl, forceCheck);
		}

		void updatePanelCloseButtonClick(object sender, RoutedEventArgs e)
		{
			updatePanel.Visibility = Visibility.Collapsed;
		}

		async void downloadOrCheckUpdateButtonClick(object sender, RoutedEventArgs e)
		{
			if (updateAvailableDownloadUrl != null) {
				MainWindow.OpenLink(updateAvailableDownloadUrl);
			} else {
				updatePanel.Visibility = Visibility.Collapsed;
				string downloadUrl = await AboutPage.CheckForUpdatesAsync(ILSpySettings.Load());
				AdjustUpdateUIAfterCheck(downloadUrl, true);
			}
		}

		void AdjustUpdateUIAfterCheck(string downloadUrl, bool displayMessage)
		{
			updateAvailableDownloadUrl = downloadUrl;
			updatePanel.Visibility = displayMessage ? Visibility.Visible : Visibility.Collapsed;
			if (downloadUrl != null) {
				updatePanelMessage.Text = Properties.Resources.ILSpyVersionAvailable;
				downloadOrCheckUpdateButton.Content = Properties.Resources.Download;
			} else {
				updatePanelMessage.Text = Properties.Resources.UpdateILSpyFound;
				downloadOrCheckUpdateButton.Content = Properties.Resources.CheckAgain;
			}
		}
		#endregion

		public void ShowAssemblyList(string name)
		{
			ILSpySettings settings = ILSpySettings.Load();
			AssemblyList list = this.assemblyListManager.LoadList(settings, name);
			//Only load a new list when it is a different one
			if (list.ListName != CurrentAssemblyList.ListName) {
				ShowAssemblyList(list);
			}
		}

		void ShowAssemblyList(AssemblyList assemblyList)
		{
			history.Clear();
			this.assemblyList = assemblyList;

			assemblyList.assemblies.CollectionChanged += assemblyList_Assemblies_CollectionChanged;

			assemblyListTreeNode = new AssemblyListTreeNode(assemblyList);
			assemblyListTreeNode.FilterSettings = sessionSettings.FilterSettings.Clone();
			assemblyListTreeNode.Select = SelectNode;
			treeView.Root = assemblyListTreeNode;

			if (assemblyList.ListName == AssemblyListManager.DefaultListName)
#if DEBUG
				this.Title = $"ILSpy {RevisionClass.FullVersion}";
#else
				this.Title = "ILSpy";
#endif
			else
#if DEBUG
				this.Title = $"ILSpy {RevisionClass.FullVersion} - " + assemblyList.ListName;
#else
				this.Title = "ILSpy - " + assemblyList.ListName;
#endif
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
			foreach (System.Reflection.Assembly asm in initialAssemblies)
				assemblyList.OpenAssembly(asm.Location);
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
			if (assemblyListTreeNode != null)
				assemblyListTreeNode.FilterSettings = sessionSettings.FilterSettings.Clone();
		}

		internal AssemblyListTreeNode AssemblyListTreeNode {
			get { return assemblyListTreeNode; }
		}

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
			SharpTreeNode node = treeView.Root;
			SharpTreeNode bestMatch = node;
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
			List<string> path = new List<string>();
			while (node.Parent != null) {
				path.Add(node.ToString());
				node = node.Parent;
			}
			path.Reverse();
			return path.ToArray();
		}

		public ILSpyTreeNode FindTreeNode(object reference)
		{
			switch (reference) {
				case PEFile asm:
					return assemblyListTreeNode.FindAssemblyNode(asm);
				case Resource res:
					return assemblyListTreeNode.FindResourceNode(res);
				case ValueTuple<Resource, string> resName:
					return assemblyListTreeNode.FindResourceNode(resName.Item1, resName.Item2);
				case ITypeDefinition type:
					return assemblyListTreeNode.FindTypeNode(type);
				case IField fd:
					return assemblyListTreeNode.FindFieldNode(fd);
				case IMethod md:
					return assemblyListTreeNode.FindMethodNode(md);
				case IProperty pd:
					return assemblyListTreeNode.FindPropertyNode(pd);
				case IEvent ed:
					return assemblyListTreeNode.FindEventNode(ed);
				case INamespace nd:
					return AssemblyListTreeNode.FindNamespaceNode(nd);
				default:
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
			switch (reference) {
				case Decompiler.Disassembler.OpCodeInfo opCode:
					OpenLink(opCode.Link);
					break;
				case ValueTuple<PEFile, System.Reflection.Metadata.EntityHandle> unresolvedEntity:
					var typeSystem = new DecompilerTypeSystem(unresolvedEntity.Item1, unresolvedEntity.Item1.GetAssemblyResolver(),
						TypeSystemOptions.Default | TypeSystemOptions.Uncached);
					reference = typeSystem.MainModule.ResolveEntity(unresolvedEntity.Item2);
					goto default;
				default:
					ILSpyTreeNode treeNode = FindTreeNode(reference);
					if (treeNode != null)
						SelectNode(treeNode);
					break;
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

		public static void ExecuteCommand(string fileName, string arguments)
		{
			try {
				Process.Start(fileName, arguments);
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
			OpenFileDialog dlg = new OpenFileDialog();
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
			foreach (string file in fileNames) {
				switch (Path.GetExtension(file)) {
					case ".nupkg":
						this.handlingNugetPackageSelection = true;
						try {
							LoadedNugetPackage package = new LoadedNugetPackage(file);
							var selectionDialog = new NugetPackageBrowserDialog(package);
							selectionDialog.Owner = this;
							if (selectionDialog.ShowDialog() != true)
								break;
							foreach (var entry in selectionDialog.SelectedItems) {
								var nugetAsm = assemblyList.OpenAssembly("nupkg://" + file + ";" + entry.Name, entry.Stream, true);
								if (nugetAsm != null) {
									if (loadedAssemblies != null)
										loadedAssemblies.Add(nugetAsm);
									else {
										var node = assemblyListTreeNode.FindAssemblyNode(nugetAsm);
										if (node != null && focusNode) {
											treeView.SelectedItems.Add(node);
											lastNode = node;
										}
									}
								}
							}
						} finally {
							this.handlingNugetPackageSelection = false;
						}
						break;
					default:
						var asm = assemblyList.OpenAssembly(file);
						if (asm != null) {
							if (loadedAssemblies != null)
								loadedAssemblies.Add(asm);
							else {
								var node = assemblyListTreeNode.FindAssemblyNode(asm);
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
			try {
				refreshInProgress = true;
				var path = GetPathForNode(treeView.SelectedItem as SharpTreeNode);
				ShowAssemblyList(assemblyListManager.LoadList(ILSpySettings.Load(), assemblyList.ListName));
				SelectNode(FindNodeByPath(path, true));
			} finally {
				refreshInProgress = false;
			}
		}

		void SearchCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			SearchPaneModel.Instance.IsVisible = true;
		}
		#endregion

		#region Decompile (TreeView_SelectionChanged)
		void TreeView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			DecompileSelectedNodes();

			SelectionChanged?.Invoke(sender, e);
		}

		Task decompilationTask;
		bool ignoreDecompilationRequests;

		void DecompileSelectedNodes(DecompilerTextViewState state = null, bool recordHistory = true)
		{
			if (ignoreDecompilationRequests)
				return;

			if (treeView.SelectedItems.Count == 0 && refreshInProgress)
				return;

			if (recordHistory) {
				var dtState = DockWorkspace.Instance.GetState();
				if (dtState != null)
					history.UpdateCurrent(new NavigationState(dtState));
				history.Record(new NavigationState(treeView.SelectedItems.OfType<SharpTreeNode>()));
			}

			if (treeView.SelectedItems.Count == 1) {
				ILSpyTreeNode node = treeView.SelectedItem as ILSpyTreeNode;
				if (node != null && node.View(DockWorkspace.Instance.GetTextView()))
					return;
			}
			decompilationTask = DockWorkspace.Instance.GetTextView().DecompileAsync(this.CurrentLanguage, this.SelectedNodes, new DecompilationOptions() { TextViewState = state });
		}

		void SaveCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;
			e.CanExecute = SaveCodeContextMenuEntry.CanExecute(SelectedNodes.ToList());
		}

		void SaveCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			SaveCodeContextMenuEntry.Execute(SelectedNodes.ToList());
		}

		public void RefreshDecompiledView()
		{
			try {
				refreshInProgress = true;
				DecompileSelectedNodes();
			} finally {
				refreshInProgress = false;
			}
		}

		public Language CurrentLanguage => sessionSettings.FilterSettings.Language;
		public LanguageVersion CurrentLanguageVersion => sessionSettings.FilterSettings.LanguageVersion;

		public event SelectionChangedEventHandler SelectionChanged;

		public IEnumerable<ILSpyTreeNode> SelectedNodes {
			get {
				return treeView.GetTopLevelSelection().OfType<ILSpyTreeNode>();
			}
		}
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
			var dtState = DockWorkspace.Instance.GetState();
			if (dtState != null)
				history.UpdateCurrent(new NavigationState(dtState));
			var newState = forward ? history.GoForward() : history.GoBack();

			ignoreDecompilationRequests = true;
			treeView.SelectedItems.Clear();
			foreach (var node in newState.TreeNodes) {
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
			sessionSettings.ActiveAssemblyList = assemblyList.ListName;
			sessionSettings.ActiveTreeViewPath = GetPathForNode(treeView.SelectedItem as SharpTreeNode);
			sessionSettings.ActiveAutoLoadedAssembly = GetAutoLoadedAssemblyNode(treeView.SelectedItem as SharpTreeNode);
			sessionSettings.WindowBounds = this.RestoreBounds;
			sessionSettings.DockLayout.Serialize(new XmlLayoutSerializer(DockManager));
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
