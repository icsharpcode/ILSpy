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

using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Settings;
using ICSharpCode.ILSpyX.TreeView;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using ICSharpCode.Decompiler.Metadata;

using System.Reflection.Metadata.Ecma335;
using System.Windows;
using ICSharpCode.Decompiler.Documentation;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using System.Reflection.Metadata;

using ICSharpCode.ILSpyX.Extensions;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Search;
using ICSharpCode.Decompiler;
using System.Text;

using TomsToolbox.Wpf;

namespace ICSharpCode.ILSpy.AssemblyTree
{
	[ExportToolPane]
	[PartCreationPolicy(CreationPolicy.Shared)]
	[Export]
	public class AssemblyListPaneModel : ToolPaneModel
	{
		public const string PaneContentId = "assemblyListPane";

		AssemblyListPane activeView;
		AssemblyList assemblyList;
		AssemblyListTreeNode assemblyListTreeNode;

		bool refreshInProgress;
		bool changingActiveTab;

		readonly NavigationHistoryService history = NavigationHistoryService.Instance;

		public AssemblyListPaneModel()
		{
			Title = Resources.Assemblies;
			ContentId = PaneContentId;
			IsCloseable = false;
			ShortcutKey = new KeyGesture(Key.F6);

			MessageBus<NavigateToReferenceEventArgs>.Subscribers += JumpToReference;
			MessageBus<SessionSettingsChangedEventArgs>.Subscribers += (sender, e) => SessionSettings_PropertyChanged(sender, e);
			MessageBus<LanguageSettingsChangedEventArgs>.Subscribers += (sender, e) => LanguageSettings_PropertyChanged(sender, e);

			var selectionChangeThrottle = new DispatcherThrottle(DispatcherPriority.Background, TreeView_SelectionChanged);
			SelectedItems.CollectionChanged += (_, _) => selectionChangeThrottle.Tick();
		}

		private void SessionSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var sessionSettings = SettingsService.Instance.SessionSettings;

			switch (e.PropertyName)
			{
				case nameof(SessionSettings.ActiveAssemblyList):
					ShowAssemblyList(sessionSettings.ActiveAssemblyList);
					break;
				case nameof(SessionSettings.Theme):
					// update syntax highlighting and force reload (AvalonEdit does not automatically refresh on highlighting change)
					DecompilerTextView.RegisterHighlighting();
					DecompileSelectedNodes(DockWorkspace.Instance.ActiveTabPage.GetState() as DecompilerTextViewState);
					break;
				case nameof(SessionSettings.CurrentCulture):
					MessageBox.Show(Properties.Resources.SettingsChangeRestartRequired, "ILSpy");
					break;
			}
		}

		public AssemblyList CurrentAssemblyList {
			get { return assemblyList; }
		}

		private SharpTreeNode root;
		public SharpTreeNode Root {
			get => root;
			set => SetProperty(ref root, value);
		}

		private SharpTreeNode selectedItem;
		public SharpTreeNode SelectedItem {
			get => selectedItem;
			set => SetProperty(ref selectedItem, value);
		}

		public ObservableCollection<SharpTreeNode> SelectedItems { get; } = [];

		public string[] SelectedPath => GetPathForNode(SelectedItem);

		readonly List<LoadedAssembly> commandLineLoadedAssemblies = [];

		public bool HandleCommandLineArguments(CommandLineArguments args)
		{
			LoadAssemblies(args.AssembliesToLoad, commandLineLoadedAssemblies, focusNode: false);
			if (args.Language != null)
				SettingsService.Instance.SessionSettings.LanguageSettings.Language = Languages.GetLanguage(args.Language);
			return true;
		}

		/// <summary>
		/// Called on startup or when passed arguments via WndProc from a second instance.
		/// In the format case, spySettings is non-null; in the latter it is null.
		/// </summary>
		public void HandleCommandLineArgumentsAfterShowList(CommandLineArguments args, ILSpySettings spySettings = null)
		{
			var sessionSettings = SettingsService.Instance.SessionSettings;

			var relevantAssemblies = commandLineLoadedAssemblies.ToList();
			commandLineLoadedAssemblies.Clear(); // clear references once we don't need them anymore
			NavigateOnLaunch(args.NavigateTo, sessionSettings.ActiveTreeViewPath, spySettings, relevantAssemblies);
			if (args.Search != null)
			{
				var searchPane = App.ExportProvider.GetExportedValue<SearchPaneModel>();

				searchPane.SearchTerm = args.Search;
				searchPane.Show();
			}
		}

		public async void NavigateOnLaunch(string navigateTo, string[] activeTreeViewPath, ILSpySettings spySettings, List<LoadedAssembly> relevantAssemblies)
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
						AssemblyTreeNode asmNode = assemblyListTreeNode.FindAssemblyNode(asm);
						if (asmNode != null)
						{
							// FindNamespaceNode() blocks the UI if the assembly is not yet loaded,
							// so use an async wait instead.
							await asm.GetMetadataFileAsync().Catch<Exception>(ex => { });
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
					IEntity mr = await Task.Run(() => FindEntityInRelevantAssemblies(navigateTo, relevantAssemblies));
					// Make sure we wait for assemblies being loaded...
					// BeginInvoke in LoadedAssembly.LookupReferencedAssemblyInternal
					await Dispatcher.InvokeAsync(delegate { }, DispatcherPriority.Normal);
					if (mr != null && mr.ParentModule?.MetadataFile != null)
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
					DockWorkspace.Instance.ShowText(output);
				}
			}
			else if (relevantAssemblies.Count == 1)
			{
				// NavigateTo == null and an assembly was given on the command-line:
				// Select the newly loaded assembly
				AssemblyTreeNode asmNode = assemblyListTreeNode.FindAssemblyNode(relevantAssemblies[0]);
				if (asmNode != null && SelectedItem == initialSelection)
				{
					SelectNode(asmNode);
				}
			}
			else if (spySettings != null)
			{
				SharpTreeNode node = null;
				if (activeTreeViewPath?.Length > 0)
				{
					foreach (var asm in CurrentAssemblyList.GetAssemblies())
					{
						if (asm.FileName == activeTreeViewPath[0])
						{
							// FindNodeByPath() blocks the UI if the assembly is not yet loaded,
							// so use an async wait instead.
							await asm.GetMetadataFileAsync().Catch<Exception>(ex => { });
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
						await MainWindow.Instance.ShowMessageIfUpdatesAvailableAsync(spySettings);
					}
					else
					{
						DockWorkspace.Instance.ActiveTabPage.ShowTextView(AboutPage.Display);
					}
				}
			}
		}

		public static IEntity FindEntityInRelevantAssemblies(string navigateTo, IEnumerable<LoadedAssembly> relevantAssemblies)
		{
			ITypeReference typeRef;
			IMemberReference memberRef = null;
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
				if (CanResolveTypeInPEFile(module, typeRef, out var typeHandle))
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

		static bool CanResolveTypeInPEFile(MetadataFile module, ITypeReference typeRef, out EntityHandle typeHandle)
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
			var loadPreviousAssemblies = Options.MiscSettingsPanel.CurrentMiscSettings.LoadPreviousAssemblies;

			var sessionSettings = SettingsService.Instance.SessionSettings;

			if (loadPreviousAssemblies)
			{
				this.assemblyList = SettingsService.Instance.AssemblyListManager.LoadList(sessionSettings.ActiveAssemblyList);
			}
			else
			{
				SettingsService.Instance.AssemblyListManager.ClearAll();
				this.assemblyList = SettingsService.Instance.AssemblyListManager.CreateList(AssemblyListManager.DefaultListName);
			}

			HandleCommandLineArguments(App.CommandLineArguments);

			if (assemblyList.GetAssemblies().Length == 0
				&& assemblyList.ListName == AssemblyListManager.DefaultListName
				&& loadPreviousAssemblies)
			{
				LoadInitialAssemblies();
			}

			ShowAssemblyList(this.assemblyList);

			if (sessionSettings.ActiveAutoLoadedAssembly != null
				&& File.Exists(sessionSettings.ActiveAutoLoadedAssembly))
			{
				this.assemblyList.Open(sessionSettings.ActiveAutoLoadedAssembly, true);
			}

			Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() => OpenAssemblies(SettingsService.Instance.SpySettings)));
		}

		void OpenAssemblies(ILSpySettings spySettings)
		{
			HandleCommandLineArgumentsAfterShowList(App.CommandLineArguments, spySettings);

			AvalonEditTextOutput output = new();
			if (FormatExceptions(App.StartupExceptions.ToArray(), output))
				DockWorkspace.Instance.ShowText(output);
		}

		static bool FormatExceptions(App.ExceptionData[] exceptions, ITextOutput output)
		{
			var stringBuilder = new StringBuilder();
			var result = exceptions.FormatExceptions(stringBuilder);
			if (result)
			{
				output.Write(stringBuilder.ToString());
			}
			return result;
		}

		public void ShowAssemblyList(string name)
		{
			AssemblyList list = SettingsService.Instance.AssemblyListManager.LoadList(name);
			//Only load a new list when it is a different one
			if (list.ListName != CurrentAssemblyList.ListName)
			{
				ShowAssemblyList(list);
				SelectNode(Root);
			}
		}

		void ShowAssemblyList(AssemblyList assemblyList)
		{
			history.Clear();
			if (this.assemblyList != null)
			{
				this.assemblyList.CollectionChanged -= assemblyList_CollectionChanged;
			}

			this.assemblyList = assemblyList;

			assemblyList.CollectionChanged += assemblyList_CollectionChanged;

			assemblyListTreeNode = new(assemblyList) {
				Select = x => SelectNode(x)
			};

			Root = assemblyListTreeNode;

			if (assemblyList.ListName == AssemblyListManager.DefaultListName)
#if DEBUG
				this.Title = $"ILSpy {DecompilerVersionInfo.FullVersion}";
#else
				this.Title = "ILSpy";
#endif
			else
#if DEBUG
				this.Title = $"ILSpy {DecompilerVersionInfo.FullVersion} - " + assemblyList.ListName;
#else
				this.Title = "ILSpy - " + assemblyList.ListName;
#endif
		}

		void assemblyList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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

		void LanguageSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Language" || e.PropertyName == "LanguageVersion")
			{
				DecompileSelectedNodes(recordHistory: false);
			}
		}

		public AssemblyTreeNode FindAssemblyNode(LoadedAssembly asm)
		{
			return assemblyListTreeNode.FindAssemblyNode(asm);
		}

		#region Node Selection

		public void SelectNode(SharpTreeNode node, bool inNewTabPage = false)
		{
			if (node == null)
				return;

			if (node.AncestorsAndSelf().Any(item => item.IsHidden))
			{
				MessageBox.Show(Properties.Resources.NavigationFailed, "ILSpy", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				return;
			}

			if (inNewTabPage)
			{
				DockWorkspace.Instance.TabPages.Add();
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
			}
		}

		internal void SelectNodes(IEnumerable<SharpTreeNode> nodes, bool inNewTabPage = false, bool setFocus = true, bool changingTab = false, bool ignoreCompilationRequests = false)
		{
			this.ignoreDecompilationRequests = ignoreCompilationRequests;

			try
			{

				if (inNewTabPage)
				{
					DockWorkspace.Instance.TabPages.Add();
				}

				// Ensure nodes exist
				var nodesList = nodes.Select(n => FindNodeByPath(GetPathForNode(n), true))
					.Where(n => n != null)
					.ToArray();

				if (!nodesList.Any() || nodesList.Any(n => n.AncestorsAndSelf().Any(a => a.IsHidden)))
				{
					return;
				}

				this.changingActiveTab = changingTab || inNewTabPage;

				var currentFocused = Keyboard.FocusedElement;

				try
				{
					if (SelectedItems.SequenceEqual(nodesList))
					{
						Dispatcher.BeginInvoke(RefreshDecompiledView);
						return;
					}

					this.SelectedItems.Clear();
					this.SelectedItems.AddRange(nodesList);
				}
				finally
				{
					if (!setFocus)
						currentFocused.Focus();

					this.changingActiveTab = false;
				}
			}
			finally
			{
				this.ignoreDecompilationRequests = false;
			}

		}

		/// <summary>
		/// Retrieves a node using the .ToString() representations of its ancestors.
		/// </summary>
		public SharpTreeNode FindNodeByPath(string[] path, bool returnBestMatch)
		{
			if (path == null)
				return null;
			SharpTreeNode node = Root;
			SharpTreeNode bestMatch = node;
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
			while (node.Parent != null)
			{
				path.Add(node.ToString());
				node = node.Parent;
			}
			path.Reverse();
			return path.ToArray();
		}

		public ILSpyTreeNode FindTreeNode(object reference)
		{
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

		private void JumpToReference(object sender, NavigateToReferenceEventArgs e)
		{
			JumpToReferenceAsync(e.Reference, e.InNewTabPage).HandleExceptions();
		}

		/// <summary>
		/// Jumps to the specified reference.
		/// </summary>
		/// <returns>
		/// Returns a task that will signal completion when the decompilation of the jump target has finished.
		/// The task will be marked as canceled if the decompilation is canceled.
		/// </returns>
		private Task JumpToReferenceAsync(object reference, bool inNewTabPage = false)
		{
			var decompilationTask = Task.CompletedTask;

			switch (reference)
			{
				case Decompiler.Disassembler.OpCodeInfo opCode:
					MainWindow.OpenLink(opCode.Link);
					break;
				case EntityReference unresolvedEntity:
					string protocol = unresolvedEntity.Protocol;
					var file = unresolvedEntity.ResolveAssembly(assemblyList);
					if (file == null)
					{
						break;
					}
					if (protocol != "decompile")
					{
						var protocolHandlers = App.ExportProvider.GetExportedValues<IProtocolHandler>();
						foreach (var handler in protocolHandlers)
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
					ILSpyTreeNode treeNode = FindTreeNode(reference);
					if (treeNode != null)
						SelectNode(treeNode, inNewTabPage);
					break;
			}
			return decompilationTask;
		}

		#endregion

		public void LoadAssemblies(IEnumerable<string> fileNames, List<LoadedAssembly> loadedAssemblies = null, bool focusNode = true)
		{
			var currentFocus = Keyboard.FocusedElement;
			AssemblyTreeNode lastNode = null;

			foreach (string file in fileNames)
			{
				var assembly = assemblyList.OpenAssembly(file);

				if (loadedAssemblies != null)
				{
					loadedAssemblies.Add(assembly);
				}
				else
				{
					var node = assemblyListTreeNode.FindAssemblyNode(assembly);
					if (node != null && focusNode)
					{
						lastNode = node;
						SelectedItems.Add(node);
					}
				}
			}

			if (!focusNode)
			{
				currentFocus?.Focus();
			}
			else if (lastNode != null)
			{
				activeView?.FocusNode(lastNode);
			}
		}

		#region Decompile (TreeView_SelectionChanged)

		void TreeView_SelectionChanged()
		{
			DecompilerTextViewState state = null;

			// These are probably no longer needed and can be removed!
			Debug.Assert(!refreshInProgress);
			Debug.Assert(!changingActiveTab);

			if (refreshInProgress || changingActiveTab)
			{
				state = DockWorkspace.Instance.ActiveTabPage.GetState() as DecompilerTextViewState;
			}

			var delayDecompilationRequestDueToContextMenu = Mouse.RightButton == MouseButtonState.Pressed;

			if (!changingActiveTab && !delayDecompilationRequestDueToContextMenu)
			{
				DecompileSelectedNodes(state);
			}
			else
			{
				ContextMenuProvider.ContextMenuClosed += ContextMenuClosed;
			}

			MessageBus.Send(this, new AssemblyTreeSelectionChangedEventArgs());

			return;

			void ContextMenuClosed(object sender, EventArgs e)
			{
				ContextMenuProvider.ContextMenuClosed -= ContextMenuClosed;

				Dispatcher.BeginInvoke(DispatcherPriority.Background, () => {
					if (Mouse.RightButton != MouseButtonState.Pressed)
					{
						DecompileSelectedNodes(DockWorkspace.Instance.ActiveTabPage.GetState() as DecompilerTextViewState);
					}
				});
			}
		}

		private bool ignoreDecompilationRequests;

		public void DecompileSelectedNodes(DecompilerTextViewState newState = null, bool recordHistory = true)
		{
			if (ignoreDecompilationRequests)
				return;

			if (SelectedItems.Count == 0 && refreshInProgress)
				return;

			if (recordHistory)
			{
				var tabPage = DockWorkspace.Instance.ActiveTabPage;
				var currentState = tabPage.GetState();
				if (currentState != null)
					history.UpdateCurrent(new NavigationState(tabPage, currentState));
				history.Record(new NavigationState(tabPage, SelectedItems));
			}

			DockWorkspace.Instance.ActiveTabPage.SupportsLanguageSwitching = true;

			if (SelectedItems.Count == 1)
			{
				if (SelectedItem is ILSpyTreeNode node && node.View(DockWorkspace.Instance.ActiveTabPage))
					return;
			}
			if (newState?.ViewedUri != null)
			{
				MainWindow.Instance.NavigateTo(new(newState.ViewedUri, null), recordHistory: false);
				return;
			}
			var options = MainWindow.Instance.CreateDecompilationOptions();
			options.TextViewState = newState;
			DockWorkspace.Instance.ActiveTabPage.ShowTextViewAsync(textView => textView.DecompileAsync(this.CurrentLanguage, this.SelectedNodes, options));
		}

		public void RefreshDecompiledView()
		{
			try
			{
				refreshInProgress = true;
				DecompileSelectedNodes();
			}
			finally
			{
				refreshInProgress = false;
			}
		}

		public Language CurrentLanguage => SettingsService.Instance.SessionSettings.LanguageSettings.Language;

		public LanguageVersion CurrentLanguageVersion => SettingsService.Instance.SessionSettings.LanguageSettings.LanguageVersion;

		public IEnumerable<ILSpyTreeNode> SelectedNodes {
			get {
				return GetTopLevelSelection().OfType<ILSpyTreeNode>();
			}
		}

		#endregion


		public void Refresh()
		{
			var currentFocus = Keyboard.FocusedElement;

			try
			{
				refreshInProgress = true;
				var path = GetPathForNode(SelectedItem);
				ShowAssemblyList(SettingsService.Instance.AssemblyListManager.LoadList(assemblyList.ListName));
				SelectNode(FindNodeByPath(path, true), inNewTabPage: false);
			}
			finally
			{
				refreshInProgress = false;
				currentFocus?.Focus();
			}
		}

		public void UnselectAll(bool ignoreCompilationRequests = false)
		{
			this.ignoreDecompilationRequests = ignoreCompilationRequests;
			SelectedItems.Clear();
			this.ignoreDecompilationRequests = false;
		}

		public IEnumerable<SharpTreeNode> GetTopLevelSelection()
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
				CurrentAssemblyList.Sort(AssemblyComparer.Instance);
			}
		}

		private class AssemblyComparer : IComparer<LoadedAssembly>
		{
			public static readonly AssemblyComparer Instance = new();
			int IComparer<LoadedAssembly>.Compare(LoadedAssembly x, LoadedAssembly y)
			{
				return string.Compare(x?.ShortName, y?.ShortName, StringComparison.CurrentCulture);
			}
		}

		public void CollapseAll()
		{
			using (activeView.LockUpdates())
			{
				CollapseChildren(Root);
			}
		}

		static void CollapseChildren(SharpTreeNode node)
		{
			foreach (var child in node.Children)
			{
				if (!child.IsExpanded)
					continue;

				CollapseChildren(child);
				child.IsExpanded = false;
			}
		}
	}
}
