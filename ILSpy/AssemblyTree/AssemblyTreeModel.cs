// Copyright (c) 2019 AlphaSierraPapa for the SharpDevelop Team
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
using System.Composition;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Threading;

using ICSharpCode.Decompiler.Documentation;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.Updates;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.TreeView;

using TomsToolbox.Composition;
using TomsToolbox.Essentials;
using TomsToolbox.Wpf;

#nullable enable

namespace ICSharpCode.ILSpy.AssemblyTree
{
	[ExportToolPane]
	[Shared]
	public class AssemblyTreeModel : ToolPaneModel
	{
		public const string PaneContentId = "assemblyListPane";

		private AssemblyListPane? activeView;
		private AssemblyListTreeNode? assemblyListTreeNode;
		private readonly DispatcherThrottle refreshThrottle;

		private readonly NavigationHistory<NavigationState> history = new();
		private bool isNavigatingHistory;
		private readonly SettingsService settingsService;
		private readonly LanguageService languageService;
		private readonly IExportProvider exportProvider;

		public AssemblyTreeModel(SettingsService settingsService, LanguageService languageService, IExportProvider exportProvider)
		{
			this.settingsService = settingsService;
			this.languageService = languageService;
			this.exportProvider = exportProvider;

			Title = Resources.Assemblies;
			ContentId = PaneContentId;
			IsCloseable = false;
			ShortcutKey = new KeyGesture(Key.F6);

			MessageBus<NavigateToReferenceEventArgs>.Subscribers += JumpToReference;
			MessageBus<SettingsChangedEventArgs>.Subscribers += (sender, e) => Settings_PropertyChanged(sender, e);
			MessageBus<ApplySessionSettingsEventArgs>.Subscribers += ApplySessionSettings;
			MessageBus<ActiveTabPageChangedEventArgs>.Subscribers += ActiveTabPageChanged;
			MessageBus<ResetLayoutEventArgs>.Subscribers += ResetLayout;
			MessageBus<NavigateToEventArgs>.Subscribers += (_, e) => NavigateTo(e.Request, e.InNewTabPage);
			MessageBus<MainWindowLoadedEventArgs>.Subscribers += (_, _) => {
				Initialize();
				Show();
			};

			EventManager.RegisterClassHandler(typeof(Window), Hyperlink.RequestNavigateEvent, new RequestNavigateEventHandler((_, e) => NavigateTo(e)));

			refreshThrottle = new(DispatcherPriority.Background, RefreshInternal);

			AssemblyList = settingsService.CreateEmptyAssemblyList();
		}

		private void Settings_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (sender is SessionSettings sessionSettings)
			{
				switch (e.PropertyName)
				{
					case nameof(SessionSettings.ActiveAssemblyList):
						ShowAssemblyList(sessionSettings.ActiveAssemblyList);
						RefreshDecompiledView();
						break;
					case nameof(SessionSettings.Theme):
						// update syntax highlighting and force reload (AvalonEdit does not automatically refresh on highlighting change)
						DecompilerTextView.RegisterHighlighting();
						RefreshDecompiledView();
						break;
					case nameof(SessionSettings.CurrentCulture):
						MessageBox.Show(Resources.SettingsChangeRestartRequired, "ILSpy");
						break;
				}
			}
			else if (sender is LanguageSettings)
			{
				switch (e.PropertyName)
				{
					case nameof(LanguageSettings.LanguageId) or nameof(LanguageSettings.LanguageVersionId):
						RefreshDecompiledView();
						break;
					default:
						Refresh();
						break;
				}
			}
		}

		public AssemblyList AssemblyList { get; private set; }

		private SharpTreeNode? root;
		public SharpTreeNode? Root {
			get => root;
			set => SetProperty(ref root, value);
		}

		public SharpTreeNode? SelectedItem {
			get => SelectedItems.FirstOrDefault();
			set => SelectedItems = value is null ? [] : [value];
		}

		private SharpTreeNode[] selectedItems = [];
		public SharpTreeNode[] SelectedItems {
			get => selectedItems;
			set {
				if (selectedItems.SequenceEqual(value))
					return;

				selectedItems = value;
				OnPropertyChanged();
				TreeView_SelectionChanged();
			}
		}

		public string[]? SelectedPath => GetPathForNode(SelectedItem);

		private readonly List<LoadedAssembly> commandLineLoadedAssemblies = [];

		private bool HandleCommandLineArguments(CommandLineArguments args)
		{
			LoadAssemblies(args.AssembliesToLoad, commandLineLoadedAssemblies, focusNode: false);
			if (args.Language != null)
				languageService.Language = languageService.GetLanguage(args.Language);
			return true;
		}

		/// <summary>
		/// Called on startup or when passed arguments via WndProc from a second instance.
		/// In the format case, updateSettings is non-null; in the latter it is null.
		/// </summary>
		private async Task HandleCommandLineArgumentsAfterShowList(CommandLineArguments args, UpdateSettings? updateSettings = null)
		{
			var sessionSettings = settingsService.SessionSettings;

			var relevantAssemblies = commandLineLoadedAssemblies.ToList();
			commandLineLoadedAssemblies.Clear(); // clear references once we don't need them anymore

			await NavigateOnLaunch(args.NavigateTo, sessionSettings.ActiveTreeViewPath, updateSettings, relevantAssemblies);

			if (args.Search != null)
			{
				MessageBus.Send(this, new ShowSearchPageEventArgs(args.Search));
			}
		}

		public async Task HandleSingleInstanceCommandLineArguments(string[] args)
		{
			var cmdArgs = CommandLineArguments.Create(args);

			await Dispatcher.InvokeAsync(async () => {

				if (!HandleCommandLineArguments(cmdArgs))
					return;

				var window = Application.Current.MainWindow;

				if (!cmdArgs.NoActivate && window is { WindowState: WindowState.Minimized })
				{
					window.WindowState = WindowState.Normal;
				}

				await HandleCommandLineArgumentsAfterShowList(cmdArgs);
			});
		}

		private async Task NavigateOnLaunch(string? navigateTo, string[]? activeTreeViewPath, UpdateSettings? updateSettings, List<LoadedAssembly> relevantAssemblies)
		{
			var initialSelection = SelectedItem;
			if (navigateTo != null)
			{
				bool found = false;
				if (navigateTo.StartsWith("N:", StringComparison.Ordinal))
				{
					string namespaceName = navigateTo.Substring(2);
					foreach (LoadedAssembly asm in relevantAssemblies)
					{
						var asmNode = assemblyListTreeNode?.FindAssemblyNode(asm);
						if (asmNode != null)
						{
							// FindNamespaceNode() blocks the UI if the assembly is not yet loaded,
							// so use an async wait instead.
							await asm.GetMetadataFileAsync().Catch<Exception>(_ => { });
							NamespaceTreeNode nsNode = asmNode.FindNamespaceNode(namespaceName);
							if (nsNode != null)
							{
								found = true;
								if (SelectedItem == initialSelection)
								{
									SelectNode(nsNode);
								}
								break;
							}
						}
					}
				}
				else if (navigateTo == "none")
				{
					// Don't navigate anywhere; start empty.
					// Used by ILSpy VS addin, it'll send us the real location to navigate to via IPC.
					found = true;
				}
				else
				{
					IEntity? mr = await Task.Run(() => FindEntityInRelevantAssemblies(navigateTo, relevantAssemblies));

					// Make sure we wait for assemblies being loaded...
					// BeginInvoke in LoadedAssembly.LookupReferencedAssemblyInternal
					await Dispatcher.InvokeAsync(delegate { }, DispatcherPriority.Normal);

					if (mr is { ParentModule.MetadataFile: not null })
					{
						found = true;
						if (SelectedItem == initialSelection)
						{
							await JumpToReferenceAsync(mr);
						}
					}
				}
				if (!found && SelectedItem == initialSelection)
				{
					AvalonEditTextOutput output = new AvalonEditTextOutput();
					output.Write($"Cannot find '{navigateTo}' in command line specified assemblies.");
					DockWorkspace.ShowText(output);
				}
			}
			else if (relevantAssemblies.Count == 1)
			{
				// NavigateTo == null and an assembly was given on the command-line:
				// Select the newly loaded assembly
				var asmNode = assemblyListTreeNode?.FindAssemblyNode(relevantAssemblies[0]);
				if (asmNode != null && SelectedItem == initialSelection)
				{
					SelectNode(asmNode);
				}
			}
			else if (updateSettings != null)
			{
				SharpTreeNode? node = null;
				if (activeTreeViewPath?.Length > 0)
				{
					foreach (var asm in AssemblyList.GetAssemblies())
					{
						if (asm.FileName == activeTreeViewPath[0])
						{
							// FindNodeByPath() blocks the UI if the assembly is not yet loaded,
							// so use an async wait instead.
							await asm.GetMetadataFileAsync().Catch<Exception>(_ => { });
						}
					}
					node = FindNodeByPath(activeTreeViewPath, true);
				}
				if (SelectedItem == initialSelection)
				{
					if (node != null)
					{
						SelectNode(node);

						// only if not showing the about page, perform the update check:
						MessageBus.Send(this, new CheckIfUpdateAvailableEventArgs());
					}
					else
					{
						MessageBus.Send(this, new ShowAboutPageEventArgs(DockWorkspace.ActiveTabPage));
					}
				}
			}
		}

		public static IEntity? FindEntityInRelevantAssemblies(string navigateTo, IEnumerable<LoadedAssembly> relevantAssemblies)
		{
			ITypeReference typeRef;
			IMemberReference? memberRef = null;
			if (navigateTo.StartsWith("T:", StringComparison.Ordinal))
			{
				typeRef = IdStringProvider.ParseTypeName(navigateTo);
			}
			else
			{
				memberRef = IdStringProvider.ParseMemberIdString(navigateTo);
				typeRef = memberRef.DeclaringTypeReference;
			}
			foreach (LoadedAssembly asm in relevantAssemblies.ToList())
			{
				var module = asm.GetMetadataFileOrNull();
				if (module != null && CanResolveTypeInPEFile(module, typeRef, out var typeHandle))
				{
					ICompilation compilation = typeHandle.Kind == HandleKind.ExportedType
						? new DecompilerTypeSystem(module, module.GetAssemblyResolver())
						: new SimpleCompilation((PEFile)module, MinimalCorlib.Instance);
					return memberRef == null
						? typeRef.Resolve(new SimpleTypeResolveContext(compilation)) as ITypeDefinition
						: memberRef.Resolve(new SimpleTypeResolveContext(compilation));
				}
			}
			return null;
		}

		private static bool CanResolveTypeInPEFile(MetadataFile module, ITypeReference typeRef, out EntityHandle typeHandle)
		{
			// We intentionally ignore reference assemblies, so that the loop continues looking for another assembly that might have a usable definition.
			if (module.IsReferenceAssembly())
			{
				typeHandle = default;
				return false;
			}

			switch (typeRef)
			{
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

		public void Initialize()
		{
			AssemblyList = settingsService.LoadInitialAssemblyList();

			HandleCommandLineArguments(App.CommandLineArguments);

			var loadPreviousAssemblies = settingsService.MiscSettings.LoadPreviousAssemblies;
			if (AssemblyList.GetAssemblies().Length == 0
				&& AssemblyList.ListName == AssemblyListManager.DefaultListName
				&& loadPreviousAssemblies)
			{
				LoadInitialAssemblies(AssemblyList);
			}

			ShowAssemblyList(AssemblyList);

			var sessionSettings = settingsService.SessionSettings;
			if (sessionSettings.ActiveAutoLoadedAssembly != null
				&& File.Exists(sessionSettings.ActiveAutoLoadedAssembly))
			{
				AssemblyList.Open(sessionSettings.ActiveAutoLoadedAssembly, true);
			}

			Dispatcher.BeginInvoke(DispatcherPriority.Loaded, OpenAssemblies);
		}

		private async Task OpenAssemblies()
		{
			await HandleCommandLineArgumentsAfterShowList(App.CommandLineArguments, settingsService.GetSettings<UpdateSettings>());

			if (FormatExceptions(App.StartupExceptions.ToArray(), out var output))
			{
				output.Title = "Startup errors";

				DockWorkspace.AddTabPage();
				DockWorkspace.ShowText(output);
			}
		}

		private static bool FormatExceptions(App.ExceptionData[] exceptions, [NotNullWhen(true)] out AvalonEditTextOutput? output)
		{
			output = null;

			var result = exceptions.FormatExceptions();
			if (result.IsNullOrEmpty())
				return false;

			output = new();
			output.Write(result);
			return true;

		}

		private void ShowAssemblyList(string name)
		{
			AssemblyList list = settingsService.AssemblyListManager.LoadList(name);
			//Only load a new list when it is a different one
			if (list.ListName != AssemblyList.ListName)
			{
				ShowAssemblyList(list);
				SelectNode(Root?.Children.FirstOrDefault());
			}
		}

		private void ShowAssemblyList(AssemblyList assemblyList)
		{
			history.Clear();

			AssemblyList.CollectionChanged -= assemblyList_CollectionChanged;
			AssemblyList = assemblyList;
			assemblyList.CollectionChanged += assemblyList_CollectionChanged;

			assemblyListTreeNode = new(assemblyList) {
				Select = x => SelectNode(x)
			};

			Root = assemblyListTreeNode;

			var mainWindow = Application.Current?.MainWindow;

			if (mainWindow == null)
				return;

			if (assemblyList.ListName == AssemblyListManager.DefaultListName)
#if DEBUG
				mainWindow.Title = $"ILSpy {DecompilerVersionInfo.FullVersion}";
#else
				mainWindow.Title = "ILSpy";
#endif
			else
#if DEBUG
				mainWindow.Title = $"ILSpy {DecompilerVersionInfo.FullVersion} - " + assemblyList.ListName;
#else
				mainWindow.Title = "ILSpy - " + assemblyList.ListName;
#endif
		}

		private void assemblyList_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				history.RemoveAll(_ => true);
			}
			if (e.OldItems != null)
			{
				var oldAssemblies = new HashSet<LoadedAssembly>(e.OldItems.Cast<LoadedAssembly>());
				history.RemoveAll(n => n.TreeNodes.Any(
					nd => nd.AncestorsAndSelf().OfType<AssemblyTreeNode>().Any(
						a => oldAssemblies.Contains(a.LoadedAssembly))));
			}

			MessageBus.Send(this, new CurrentAssemblyListChangedEventArgs(e));
		}

		private static void LoadInitialAssemblies(AssemblyList assemblyList)
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

		public AssemblyTreeNode? FindAssemblyNode(LoadedAssembly asm)
		{
			return assemblyListTreeNode?.FindAssemblyNode(asm);
		}

		#region Node Selection

		public void SelectNode(SharpTreeNode? node, bool inNewTabPage = false)
		{
			if (node == null)
				return;

			if (node.AncestorsAndSelf().Any(item => item.IsHidden))
			{
				MessageBox.Show(Resources.NavigationFailed, "ILSpy", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				return;
			}

			if (inNewTabPage)
			{
				DockWorkspace.AddTabPage();
				SelectedItem = null;
			}

			if (SelectedItem == node)
			{
				Dispatcher.BeginInvoke(RefreshDecompiledView);
			}
			else
			{
				activeView?.ScrollIntoView(node);
				SelectedItem = node;

				Dispatcher.BeginInvoke(DispatcherPriority.Background, () => {
					activeView?.ScrollIntoView(node);
				});
			}
		}

		public void SelectNodes(IEnumerable<SharpTreeNode> nodes)
		{
			// Ensure nodes exist
			var nodesList = nodes.Select(n => FindNodeByPath(GetPathForNode(n), true))
				.ExceptNullItems()
				.ToArray();

			if (!nodesList.Any() || nodesList.Any(n => n.AncestorsAndSelf().Any(a => a.IsHidden)))
			{
				return;
			}

			foreach (var node in nodesList)
			{
				activeView?.ScrollIntoView(node);
			}

			SelectedItems = nodesList.ToArray();
		}

		/// <summary>
		/// Retrieves a node using the .ToString() representations of its ancestors.
		/// </summary>
		public SharpTreeNode? FindNodeByPath(string[]? path, bool returnBestMatch)
		{
			if (path == null)
				return null;
			var node = Root;
			var bestMatch = node;
			foreach (var element in path)
			{
				if (node == null)
					break;
				bestMatch = node;
				node.EnsureLazyChildren();
				if (node is ILSpyTreeNode ilSpyTreeNode)
					ilSpyTreeNode.EnsureChildrenFiltered();
				node = node.Children.FirstOrDefault(c => c.ToString() == element);
			}

			return returnBestMatch ? node ?? bestMatch : node;
		}

		/// <summary>
		/// Gets the .ToString() representation of the node's ancestors.
		/// </summary>
		public static string[]? GetPathForNode(SharpTreeNode? node)
		{
			if (node == null)
				return null;
			List<string> path = new List<string>();
			while (node.Parent != null)
			{
				path.Add(node.ToString());
				node = node.Parent;
			}
			path.Reverse();
			return path.ToArray();
		}

		public ILSpyTreeNode? FindTreeNode(object? reference)
		{
			if (assemblyListTreeNode == null)
				return null;

			switch (reference)
			{
				case LoadedAssembly lasm:
					return assemblyListTreeNode.FindAssemblyNode(lasm);
				case MetadataFile asm:
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
					return assemblyListTreeNode.FindNamespaceNode(nd);
				default:
					return null;
			}
		}

		private void JumpToReference(object? sender, NavigateToReferenceEventArgs e)
		{
			JumpToReferenceAsync(e.Reference, e.InNewTabPage).HandleExceptions();
			IsActive = true;
		}

		/// <summary>
		/// Jumps to the specified reference.
		/// </summary>
		/// <returns>
		/// Returns a task that will signal completion when the decompilation of the jump target has finished.
		/// The task will be marked as canceled if the decompilation is canceled.
		/// </returns>
		private Task JumpToReferenceAsync(object? reference, bool inNewTabPage = false)
		{
			var decompilationTask = Task.CompletedTask;

			switch (reference)
			{
				case Decompiler.Disassembler.OpCodeInfo opCode:
					GlobalUtils.OpenLink(opCode.Link);
					break;
				case EntityReference unresolvedEntity:
					string protocol = unresolvedEntity.Protocol;
					var file = unresolvedEntity.ResolveAssembly(AssemblyList);
					if (file == null)
					{
						break;
					}
					if (protocol != "decompile")
					{
						foreach (var handler in exportProvider.GetExportedValues<IProtocolHandler>())
						{
							var node = handler.Resolve(protocol, file, unresolvedEntity.Handle, out bool newTabPage);
							if (node != null)
							{
								SelectNode(node, newTabPage || inNewTabPage);
								return decompilationTask;
							}
						}
					}
					var possibleToken = MetadataTokenHelpers.TryAsEntityHandle(MetadataTokens.GetToken(unresolvedEntity.Handle));
					if (possibleToken != null)
					{
						var typeSystem = new DecompilerTypeSystem(file, file.GetAssemblyResolver(), TypeSystemOptions.Default | TypeSystemOptions.Uncached);
						reference = typeSystem.MainModule.ResolveEntity(possibleToken.Value);
						goto default;
					}
					break;
				default:
					var treeNode = FindTreeNode(reference);
					if (treeNode != null)
						SelectNode(treeNode, inNewTabPage);
					break;
			}
			return decompilationTask;
		}

		#endregion

		private void LoadAssemblies(IEnumerable<string> fileNames, List<LoadedAssembly>? loadedAssemblies = null, bool focusNode = true)
		{
			using (Keyboard.FocusedElement.PreserveFocus(!focusNode))
			{
				AssemblyTreeNode? lastNode = null;

				var assemblyList = AssemblyList;

				foreach (string file in fileNames)
				{
					var assembly = assemblyList.OpenAssembly(file);

					if (loadedAssemblies != null)
					{
						loadedAssemblies.Add(assembly);
					}
					else
					{
						var node = assemblyListTreeNode?.FindAssemblyNode(assembly);
						if (node != null && focusNode)
						{
							lastNode = node;
							activeView?.ScrollIntoView(node);
							SelectedItems = [.. SelectedItems, node];
						}
					}
				}
				if (focusNode && lastNode != null)
				{
					activeView?.FocusNode(lastNode);
				}
			}
		}

		#region Decompile (TreeView_SelectionChanged)

		private void TreeView_SelectionChanged()
		{
			if (SelectedItems.Length <= 0)
			{
				// To cancel any pending decompilation requests and show an empty tab
				DecompileSelectedNodes();
			}
			else
			{
				var activeTabPage = DockWorkspace.ActiveTabPage;

				if (!isNavigatingHistory)
				{
					history.Record(new NavigationState(activeTabPage, SelectedItems));
				}

				var delayDecompilationRequestDueToContextMenu = Mouse.RightButton == MouseButtonState.Pressed;

				if (!delayDecompilationRequestDueToContextMenu)
				{
					var decompiledNodes = activeTabPage
						.GetState()
						?.DecompiledNodes
						?.Select(n => FindNodeByPath(GetPathForNode(n), true))
						.ExceptNullItems()
						.ToArray() ?? [];

					if (!decompiledNodes.SequenceEqual(SelectedItems))
					{
						DecompileSelectedNodes();
					}
				}
				else
				{
					// ensure that we are only connected once to the event, else we might get multiple notifications
					ContextMenuProvider.ContextMenuClosed -= ContextMenuClosed;
					ContextMenuProvider.ContextMenuClosed += ContextMenuClosed;
				}
			}

			MessageBus.Send(this, new AssemblyTreeSelectionChangedEventArgs());

			return;

			void ContextMenuClosed(object? sender, EventArgs e)
			{
				ContextMenuProvider.ContextMenuClosed -= ContextMenuClosed;

				Dispatcher.BeginInvoke(DispatcherPriority.Background, () => {
					if (Mouse.RightButton != MouseButtonState.Pressed)
					{
						RefreshDecompiledView();
					}
				});
			}
		}

		public void DecompileSelectedNodes(DecompilerTextViewState? newState = null)
		{
			var activeTabPage = DockWorkspace.ActiveTabPage;

			activeTabPage.SupportsLanguageSwitching = true;

			if (SelectedItems.Length == 1)
			{
				if (SelectedItem is ILSpyTreeNode node && node.View(activeTabPage))
					return;
			}
			if (newState?.ViewedUri != null)
			{
				NavigateTo(new(newState.ViewedUri, null));
				return;
			}

			var options = activeTabPage.CreateDecompilationOptions();
			options.TextViewState = newState;
			activeTabPage.ShowTextViewAsync(textView => textView.DecompileAsync(this.CurrentLanguage, this.SelectedNodes, options));
		}

		public void RefreshDecompiledView()
		{
			DecompileSelectedNodes(DockWorkspace.ActiveTabPage.GetState() as DecompilerTextViewState);
		}

		public Language CurrentLanguage => languageService.Language;

		public LanguageVersion? CurrentLanguageVersion => languageService.LanguageVersion;

		public IEnumerable<ILSpyTreeNode> SelectedNodes => GetTopLevelSelection().OfType<ILSpyTreeNode>();

		#endregion

		public void NavigateHistory(bool forward)
		{
			try
			{
				isNavigatingHistory = true;

				TabPageModel tabPage = DockWorkspace.ActiveTabPage;
				var state = tabPage.GetState();
				if (state != null)
					history.UpdateCurrent(new NavigationState(tabPage, state));
				var newState = forward ? history.GoForward() : history.GoBack();

				TabPageModel activeTabPage = newState.TabPage;

				if (!DockWorkspace.TabPages.Contains(activeTabPage))
					DockWorkspace.AddTabPage(activeTabPage);
				else
					DockWorkspace.ActiveTabPage = activeTabPage;

				SelectNodes(newState.TreeNodes);
			}
			finally
			{
				isNavigatingHistory = false;
			}
		}

		public bool CanNavigateBack => history.CanNavigateBack;

		public bool CanNavigateForward => history.CanNavigateForward;

		private void NavigateTo(RequestNavigateEventArgs e, bool inNewTabPage = false)
		{
			if (e.Uri.Scheme == "resource")
			{
				if (inNewTabPage)
				{
					DockWorkspace.AddTabPage();
				}

				if (e.Uri.Host == "aboutpage")
				{
					RecordHistory();
					MessageBus.Send(this, new ShowAboutPageEventArgs(DockWorkspace.ActiveTabPage));
					e.Handled = true;
					return;
				}

				AvalonEditTextOutput output = new AvalonEditTextOutput {
					Address = e.Uri,
					Title = e.Uri.AbsolutePath,
					EnableHyperlinks = true
				};
				using (Stream? s = typeof(App).Assembly.GetManifestResourceStream(typeof(App), e.Uri.AbsolutePath))
				{
					if (s != null)
					{
						using StreamReader r = new StreamReader(s);
						string? line;
						while ((line = r.ReadLine()) != null)
						{
							output.Write(line);
							output.WriteLine();
						}
					}
				}
				RecordHistory();
				DockWorkspace.ShowText(output);
				e.Handled = true;
			}

			void RecordHistory()
			{
				if (isNavigatingHistory)
					return;
				TabPageModel tabPage = DockWorkspace.ActiveTabPage;
				var currentState = tabPage.GetState();
				if (currentState != null)
					history.UpdateCurrent(new NavigationState(tabPage, currentState));

				UnselectAll();

				history.Record(new NavigationState(tabPage, new ViewState { ViewedUri = e.Uri }));
			}
		}

		public void Refresh()
		{
			refreshThrottle.Tick();
		}

		private void RefreshInternal()
		{
			using (Keyboard.FocusedElement.PreserveFocus())
			{
				var path = GetPathForNode(SelectedItem);

				ShowAssemblyList(settingsService.AssemblyListManager.LoadList(AssemblyList.ListName));
				SelectNode(FindNodeByPath(path, true), inNewTabPage: false);

				RefreshDecompiledView();
			}
		}

		private void UnselectAll()
		{
			SelectedItems = [];
		}

		private IEnumerable<SharpTreeNode> GetTopLevelSelection()
		{
			var selection = this.SelectedItems;
			var selectionHash = new HashSet<SharpTreeNode>(selection);

			return selection.Where(item => item.Ancestors().All(a => !selectionHash.Contains(a)));
		}

		public void SetActiveView(AssemblyListPane activeView)
		{
			this.activeView = activeView;
		}

		public void SortAssemblyList()
		{
			using (activeView?.LockUpdates())
			{
				AssemblyList.Sort(AssemblyComparer.Instance);
			}
		}

		private class AssemblyComparer : IComparer<LoadedAssembly>
		{
			public static readonly AssemblyComparer Instance = new();
			int IComparer<LoadedAssembly>.Compare(LoadedAssembly? x, LoadedAssembly? y)
			{
				return string.Compare(x?.ShortName, y?.ShortName, StringComparison.CurrentCulture);
			}
		}

		public void CollapseAll()
		{
			using (activeView?.LockUpdates())
			{
				CollapseChildren(Root);
			}
		}

		private static void CollapseChildren(SharpTreeNode? node)
		{
			if (node is null)
				return;

			foreach (var child in node.Children)
			{
				if (!child.IsExpanded)
					continue;

				CollapseChildren(child);
				child.IsExpanded = false;
			}
		}

		public void OpenFiles(string[] fileNames, bool focusNode = true)
		{
			if (fileNames == null)
				throw new ArgumentNullException(nameof(fileNames));

			if (focusNode)
				UnselectAll();

			LoadAssemblies(fileNames, focusNode: focusNode);
		}

		private void ApplySessionSettings(object? sender, ApplySessionSettingsEventArgs e)
		{
			var settings = e.SessionSettings;

			settings.ActiveAssemblyList = AssemblyList.ListName;
			settings.ActiveTreeViewPath = SelectedPath;
			settings.ActiveAutoLoadedAssembly = GetAutoLoadedAssemblyNode(SelectedItem);
		}

		private static string? GetAutoLoadedAssemblyNode(SharpTreeNode? node)
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

		private void ActiveTabPageChanged(object? sender, ActiveTabPageChangedEventArgs e)
		{
			if (e.ViewState is not { } state)
				return;

			if (state.DecompiledNodes != null)
			{
				SelectNodes(state.DecompiledNodes);
			}
			else
			{
				NavigateTo(new(state.ViewedUri, null));
			}

		}
		private void ResetLayout(object? sender, ResetLayoutEventArgs e)
		{
			RefreshDecompiledView();
		}
	}
}
