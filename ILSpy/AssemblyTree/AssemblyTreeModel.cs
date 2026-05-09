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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Composition;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Serialization;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Documentation;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.TreeView;

using ILSpy;
using ILSpy.Commands;
using ILSpy.Languages;
using ILSpy.TreeNodes;
using ILSpy.ViewModels;

namespace ILSpy.AssemblyTree
{
	[Export]
	[ExportToolPane(ContentId = PaneContentId, Alignment = ToolPaneAlignment.Left, Order = 0)]
	[Shared]
	public partial class AssemblyTreeModel : ToolPaneModel
	{
		public const string PaneContentId = "AssemblyTree";

		readonly SettingsService settingsService;
		readonly LanguageService languageService;
		AssemblyListManager? listManager;
		AssemblyListTreeNode? assemblyListTreeNode;

		[ObservableProperty]
		[property: IgnoreDataMember]
		private SharpTreeNode? root;

		/// <summary>
		/// Multi-selection set. Each entry is kept in sync with its
		/// <see cref="SharpTreeNode.IsSelected"/>. <see cref="SelectedItem"/> is a convenience
		/// wrapper around the *primary* (last-added) entry — drives decompilation, navigation
		/// history, and tree-view-path persistence — but the underlying state is single-sourced
		/// here.
		/// </summary>
		[IgnoreDataMember]
		public ObservableCollection<SharpTreeNode> SelectedItems { get; } = [];

		/// <summary>
		/// Primary (last) selection. Get returns the most recently selected entry of
		/// <see cref="SelectedItems"/>, or <c>null</c>; set replaces the entire selection
		/// with the supplied node (clears the collection then adds it). All
		/// <c>PropertyChanged(SelectedItem)</c> notifications are fired by
		/// <see cref="SelectedItems.CollectionChanged"/>.
		/// </summary>
		[IgnoreDataMember]
		public SharpTreeNode? SelectedItem {
			get => SelectedItems.Count > 0 ? SelectedItems[^1] : null;
			set {
				if (SelectedItem == value)
					return;
				SelectedItems.Clear();
				if (value != null)
					SelectedItems.Add(value);
			}
		}

		[ObservableProperty]
		[property: IgnoreDataMember]
		private string? activeListName;

		[IgnoreDataMember]
		public AssemblyList? AssemblyList { get; private set; }

		[IgnoreDataMember]
		public ObservableCollection<string> AssemblyLists { get; } = [];

		[ImportingConstructor]
		public AssemblyTreeModel(SettingsService settingsService, LanguageService languageService)
		{
			AppEnv.StartupLog.Mark("AssemblyTreeModel ctor entered");
			this.settingsService = settingsService;
			this.languageService = languageService;
			languageService.PropertyChanged += (_, e) => {
				if (e.PropertyName == nameof(LanguageService.CurrentLanguage) && Root != null)
					NotifyTextChanged(Root);
			};
			SelectedItems.CollectionChanged += OnSelectedItemsChanged;
			Id = PaneContentId;
			Title = "Assemblies";
			CanClose = false;
			AppEnv.StartupLog.Mark("AssemblyTreeModel ctor exited");
		}

		void OnSelectedItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
				foreach (SharpTreeNode n in e.NewItems)
					n.IsSelected = true;
			if (e.OldItems != null)
				foreach (SharpTreeNode n in e.OldItems)
					n.IsSelected = false;

			// SelectedItem is a wrapper over this collection — anyone bound to it must be
			// notified, and the saved path must follow the new primary.
			OnPropertyChanged(nameof(SelectedItem));
			settingsService.SessionSettings.ActiveTreeViewPath = GetPathForNode(SelectedItem);
		}

		// Walks already-materialized children and re-raises Text PropertyChanged so the cell
		// templates pick up the new language's formatting -- without collapsing the user's
		// expanded state. Lazy-loaded subtrees that haven't been opened yet are skipped (they'll
		// format with the active language the next time they get expanded).
		static void NotifyTextChanged(SharpTreeNode node)
		{
			node.RaisePropertyChanged(nameof(SharpTreeNode.Text));
			if (node.LazyLoading)
				return;
			foreach (var child in node.Children)
				NotifyTextChanged(child);
		}

		readonly TaskCompletionSource<bool> treeReadyTcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

		/// <summary>
		/// Completes when the assembly-tree view (<c>AssemblyListPane</c>) has fired its
		/// <c>Loaded</c> event for the first time. <c>RestoreSelectedPathAsync</c> awaits
		/// this before assigning <see cref="SelectedItem"/> so the saved-selection path
		/// (which kicks off a decompilation through the dock workspace) doesn't paint
		/// the document area before the tree itself is on screen.
		/// </summary>
		public Task TreeReady => treeReadyTcs.Task;

		/// <summary>
		/// Called by <c>AssemblyListPane</c> from its <c>Loaded</c> handler to resolve
		/// <see cref="TreeReady"/>. Idempotent — only the first call wins.
		/// </summary>
		internal void MarkTreeReady()
		{
			if (treeReadyTcs.TrySetResult(true))
				AppEnv.StartupLog.Mark("AssemblyTreeModel.TreeReady completed");
		}

		public void Initialize()
		{
			using var _ = AppEnv.StartupLog.Phase("AssemblyTreeModel.Initialize body");
			listManager = settingsService.AssemblyListManager;
			using (AppEnv.StartupLog.Phase("CreateDefaultAssemblyLists"))
				listManager.CreateDefaultAssemblyLists();

			SyncListNames();
			listManager.AssemblyLists.CollectionChanged += (_, _) => SyncListNames();

			var saved = settingsService.SessionSettings.ActiveAssemblyList;
			ActiveListName = !string.IsNullOrEmpty(saved) && AssemblyLists.Contains(saved)
				? saved
				: AssemblyListManager.DefaultListName;
		}

		void SyncListNames()
		{
			if (listManager == null)
				return;
			AssemblyLists.Clear();
			foreach (var name in listManager.AssemblyLists)
				AssemblyLists.Add(name);
		}

		partial void OnActiveListNameChanged(string? value)
		{
			if (listManager == null || string.IsNullOrEmpty(value))
				return;

			settingsService.SessionSettings.ActiveAssemblyList = value;
			ShowAssemblyList(value);

			// Restore the previously-selected tree node off the UI critical path. The walk
			// crosses an AssemblyTreeNode whose EnsureLazyChildren synchronously blocks on
			// GetLoadResultAsync — by going async here and awaiting the load, we let the
			// initial paint happen first and the UI stays responsive while metadata loads.
			var savedPath = settingsService.SessionSettings.ActiveTreeViewPath;
			if (savedPath is { Length: > 0 })
				_ = RestoreSelectedPathAsync(savedPath);
		}

		async Task RestoreSelectedPathAsync(string[] path)
		{
			using var _ = AppEnv.StartupLog.Phase($"RestoreSelectedPathAsync ({path.Length} segments)");
			try
			{
				if (Root == null)
					return;
				// Snapshot — if the user selects something else before the restore completes,
				// don't yank their selection out from under them.
				var initialSelection = SelectedItem;
				SharpTreeNode? node = Root;
				foreach (var element in path)
				{
					if (node == null)
						break;
					// Awaiting GetLoadResultAsync keeps EnsureLazyChildren — which itself does
					// GetAwaiter().GetResult() on the same task — from blocking the UI thread.
					// Once the load completes the .GetAwaiter().GetResult() returns instantly.
					if (node is AssemblyTreeNode asm)
					{
						using (AppEnv.StartupLog.Phase($"await GetLoadResultAsync ({asm.LoadedAssembly.ShortName})"))
							await asm.LoadedAssembly.GetLoadResultAsync().ConfigureAwait(true);
					}
					using (AppEnv.StartupLog.Phase($"EnsureLazyChildren ({node.GetType().Name} \"{element}\")"))
						node.EnsureLazyChildren();
					node = node.Children.FirstOrDefault(c => c.ToString() == element);
				}
				// Wait for the tree view to be Loaded before assigning SelectedItem. Without
				// this, the SelectedItem assignment runs ShowSelectedNode → CurrentNodes →
				// async decompilation, and the user can see the decompiled output appear
				// BEFORE the assembly tree has rendered — a confusing reverse order.
				// The 5-second timeout is a safety net for environments where the pane
				// never loads (headless tests, design-time previews).
				using (AppEnv.StartupLog.Phase("await TreeReady before SelectedItem assignment"))
				{
					await Task.WhenAny(TreeReady, Task.Delay(TimeSpan.FromSeconds(5)))
						.ConfigureAwait(true);
				}
				if (node != null && node != Root && ReferenceEquals(SelectedItem, initialSelection))
					SelectedItem = node;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"[AssemblyTreeModel] saved-path restore failed: {ex}");
			}
		}

		void ShowAssemblyList(string name)
		{
			if (listManager == null)
				return;
			AssemblyList list;
			using (AppEnv.StartupLog.Phase($"LoadList({name})"))
				list = listManager.LoadList(name);
			if (AssemblyList == null || list.ListName != AssemblyList.ListName)
				ShowAssemblyList(list);
		}

		void ShowAssemblyList(AssemblyList list)
		{
			using var _ = AppEnv.StartupLog.Phase("ShowAssemblyList(list)");
			AssemblyList = list;
			if (list.GetAssemblies().Length == 0 && list.ListName == AssemblyListManager.DefaultListName)
			{
				using (AppEnv.StartupLog.Phase("LoadInitialAssemblies"))
					LoadInitialAssemblies(list);
			}
			AppEnv.StartupLog.Mark($"AssemblyList contains {list.GetAssemblies().Length} assemblies");
			using (AppEnv.StartupLog.Phase("new AssemblyListTreeNode"))
				assemblyListTreeNode = new AssemblyListTreeNode(list);
			Root = assemblyListTreeNode;
			AppEnv.StartupLog.Mark("Root assigned");
			ScheduleBackgroundLoadSweep(list);
		}

		/// <summary>
		/// LoadedAssembly entries are now lazy — their <c>Task.Run(LoadAsync)</c> only kicks
		/// off when something asks for the metadata. The active assembly's load is awaited
		/// by <see cref="RestoreSelectedPathAsync"/>, so it gets a clean run. Everything else
		/// only loads on-demand (tree expansion, hyperlink follow, …) which can stretch
		/// quietly into "the user never sees a populated icon for assemblies they don't
		/// touch".
		///
		/// To strike a middle ground, schedule a one-shot sweep a couple of seconds after
		/// the list appears that nudges every <see cref="LoadedAssembly"/> to start loading
		/// in the background. By that point the active assembly's metadata is usually ready
		/// and the user has had a frame or two to interact with the tree.
		/// </summary>
		void ScheduleBackgroundLoadSweep(AssemblyList list)
		{
			_ = Task.Run(async () => {
				try
				{
					await Task.Delay(TimeSpan.FromSeconds(2)).ConfigureAwait(false);
					AppEnv.StartupLog.Mark("Background-load sweep starting");
					foreach (var assembly in list.GetAssemblies())
					{
						// Calling GetLoadResultAsync triggers the Lazy<Task> creation. We don't
						// await — fire-and-forget so all 122/200/whatever loads run in parallel
						// on the thread pool, just delayed past the active-assembly window.
						_ = assembly.GetLoadResultAsync();
					}
					AppEnv.StartupLog.Mark("Background-load sweep dispatched");
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine($"[AssemblyTreeModel] background load sweep failed: {ex}");
				}
			});
		}

		/// <summary>
		/// Walks down from <see cref="Root"/> matching each path segment against
		/// <see cref="object.ToString"/>, expanding lazy children along the way.
		/// </summary>
		public SharpTreeNode? FindNodeByPath(string[]? path, bool returnBestMatch)
		{
			if (path == null || Root == null)
				return null;
			SharpTreeNode? node = Root;
			SharpTreeNode? bestMatch = node;
			foreach (var element in path)
			{
				if (node == null)
					break;
				bestMatch = node;
				node.EnsureLazyChildren();
				node = node.Children.FirstOrDefault(c => c.ToString() == element);
			}
			return returnBestMatch ? node ?? bestMatch : node;
		}

		/// <summary>
		/// The path of <paramref name="node"/>'s ancestors (root excluded), in root-first order.
		/// </summary>
		public static string[]? GetPathForNode(SharpTreeNode? node)
		{
			if (node == null)
				return null;
			var path = new List<string>();
			while (node.Parent != null)
			{
				path.Add(node.ToString()!);
				node = node.Parent;
			}
			path.Reverse();
			return path.ToArray();
		}

		internal AssemblyTreeNode? FindAssemblyNode(LoadedAssembly asm)
			=> assemblyListTreeNode?.FindAssemblyNode(asm);

		/// <summary>
		/// Finds the tree node corresponding to <paramref name="reference"/> — used by
		/// hyperlink clicks in the decompiler view to route to the right entity. Currently
		/// only covers the reference kinds the tree knows how to model.
		/// </summary>
		public ILSpyTreeNode? FindTreeNode(object? reference)
		{
			if (assemblyListTreeNode == null)
				return null;

			switch (reference)
			{
				case EntityReference unresolved:
					var module = unresolved.ResolveAssembly(AssemblyList!);
					if (module == null)
						return null;
					var token = MetadataTokenHelpers.TryAsEntityHandle(MetadataTokens.GetToken(unresolved.Handle));
					if (token == null)
						return null;
					var typeSystem = new DecompilerTypeSystem(module, module.GetAssemblyResolver(), TypeSystemOptions.Default | TypeSystemOptions.Uncached);
					return FindTreeNode(typeSystem.MainModule.ResolveEntity(token.Value));

				case ITypeDefinition type:
					return FindTypeNode(assemblyListTreeNode, type);

				case IMember member:
					return FindMemberNode(assemblyListTreeNode, member);

				default:
					return null;
			}
		}

		static TypeTreeNode? FindTypeNode(AssemblyListTreeNode root, ITypeDefinition type)
		{
			var module = type.ParentModule?.MetadataFile;
			if (module == null)
				return null;
			var assembly = root.Children.OfType<AssemblyTreeNode>()
				.FirstOrDefault(a => a.LoadedAssembly.GetMetadataFileOrNull() == module);
			if (assembly == null)
				return null;
			assembly.EnsureLazyChildren();

			var nesting = new Stack<ITypeDefinition>();
			for (var current = type; current != null; current = current.DeclaringTypeDefinition)
				nesting.Push(current);

			var top = nesting.Pop();
			var ns = assembly.Children.OfType<NamespaceTreeNode>()
				.FirstOrDefault(n => n.Name == (top.Namespace ?? string.Empty));
			if (ns == null)
				return null;
			ns.EnsureLazyChildren();
			var typeNode = ns.Children.OfType<TypeTreeNode>()
				.FirstOrDefault(t => t.Handle == top.MetadataToken);
			while (typeNode != null && nesting.Count > 0)
			{
				typeNode.EnsureLazyChildren();
				var nested = nesting.Pop();
				typeNode = typeNode.Children.OfType<TypeTreeNode>()
					.FirstOrDefault(t => t.Handle == nested.MetadataToken);
			}
			return typeNode;
		}

		static ILSpyTreeNode? FindMemberNode(AssemblyListTreeNode root, IMember member)
		{
			var typeNode = member.DeclaringTypeDefinition is { } declaring ? FindTypeNode(root, declaring) : null;
			if (typeNode == null)
				return null;
			typeNode.EnsureLazyChildren();
			return member switch {
				IField f => typeNode.Children.OfType<FieldTreeNode>().FirstOrDefault(n => n.FieldDefinition.MetadataToken == f.MetadataToken),
				IMethod m => FindMethodNode(typeNode, m),
				IProperty p => typeNode.Children.OfType<PropertyTreeNode>().FirstOrDefault(n => n.PropertyDefinition.MetadataToken == p.MetadataToken),
				IEvent e => typeNode.Children.OfType<EventTreeNode>().FirstOrDefault(n => n.EventDefinition.MetadataToken == e.MetadataToken),
				_ => null,
			};
		}

		static ILSpyTreeNode? FindMethodNode(TypeTreeNode typeNode, IMethod method)
		{
			// Accessor methods (get_X / set_X / add_X / remove_X / invoke_X) live as children
			// of their owning PropertyTreeNode / EventTreeNode, not directly under the type.
			// Route through the owner so MMB on a metadata-grid accessor row finds its node.
			if (method.AccessorOwner is IProperty owningProperty)
			{
				var propNode = typeNode.Children.OfType<PropertyTreeNode>()
					.FirstOrDefault(n => n.PropertyDefinition.MetadataToken == owningProperty.MetadataToken);
				if (propNode != null)
					return propNode.Children.OfType<MethodTreeNode>()
						.FirstOrDefault(n => n.MethodDefinition.MetadataToken == method.MetadataToken);
			}
			if (method.AccessorOwner is IEvent owningEvent)
			{
				var eventNode = typeNode.Children.OfType<EventTreeNode>()
					.FirstOrDefault(n => n.EventDefinition.MetadataToken == owningEvent.MetadataToken);
				if (eventNode != null)
					return eventNode.Children.OfType<MethodTreeNode>()
						.FirstOrDefault(n => n.MethodDefinition.MetadataToken == method.MetadataToken);
			}
			return typeNode.Children.OfType<MethodTreeNode>()
				.FirstOrDefault(n => n.MethodDefinition.MetadataToken == method.MetadataToken);
		}

		static void LoadInitialAssemblies(AssemblyList assemblyList)
		{
			System.Reflection.Assembly[] initialAssemblies = {
				typeof(object).Assembly,
				typeof(Uri).Assembly,
				typeof(System.Linq.Enumerable).Assembly,
			};
			foreach (var asm in initialAssemblies)
			{
				if (!string.IsNullOrEmpty(asm.Location))
					assemblyList.OpenAssembly(asm.Location);
			}
		}

		public void SelectNode(SharpTreeNode? node)
		{
			if (node == null)
				return;
			SelectedItem = node;
		}

		public void OpenFiles(string[] fileNames, bool focusNode = true)
		{
			ArgumentNullException.ThrowIfNull(fileNames);
			LoadAssemblies(fileNames, focusNode: focusNode);
		}

		/// <summary>
		/// Applies parsed startup arguments: switches the active language, loads any assemblies
		/// passed positionally, then navigates to the requested entity / namespace if any. Safe
		/// to call before assemblies finish loading — awaits each one's metadata before resolving
		/// the navigation target. Search-string handling is deferred until the search pane lands.
		/// </summary>
		public async Task HandleCommandLineArgumentsAsync(AppEnv.CommandLineArguments args)
		{
			ArgumentNullException.ThrowIfNull(args);
			if (args.Language is { Length: > 0 } languageName)
				languageService.CurrentLanguage = languageService.GetLanguage(languageName);

			var newlyLoaded = new List<LoadedAssembly>();
			if (args.AssembliesToLoad is { Count: > 0 })
				LoadAssemblies(args.AssembliesToLoad, newlyLoaded, focusNode: false);

			// "all currently-loaded entries that the navigation target may live in" — newly
			// loaded ones first (matches WPF's "command-line files take precedence") plus the
			// existing list as fallback.
			var relevant = newlyLoaded.Count > 0
				? new List<LoadedAssembly>(newlyLoaded)
				: AssemblyList?.GetAssemblies().ToList() ?? new List<LoadedAssembly>();

			if (args.NavigateTo is { Length: > 0 } navigateTo)
				await NavigateOnLaunchAsync(navigateTo, relevant);
			else if (newlyLoaded.Count == 1 && FindAssemblyNode(newlyLoaded[0]) is { } singleNode)
				SelectNode(singleNode);

			// Search-pane wiring lands with task 6. Until then the arg parses but is a no-op
			// rather than crashing.
		}

		async Task NavigateOnLaunchAsync(string navigateTo, IList<LoadedAssembly> relevant)
		{
			// "none" is a sentinel used by the WPF VS add-in to suppress initial navigation —
			// the real target arrives later via IPC.
			if (navigateTo == "none")
				return;

			if (navigateTo.StartsWith("N:", StringComparison.Ordinal))
			{
				var namespaceName = navigateTo.Substring(2);
				foreach (var asm in relevant)
				{
					var assemblyNode = FindAssemblyNode(asm);
					if (assemblyNode == null)
						continue;
					await asm.GetMetadataFileAsync().ConfigureAwait(true);
					var nsNode = assemblyNode.FindNamespaceNode(namespaceName);
					if (nsNode != null)
					{
						SelectNode(nsNode);
						return;
					}
				}
				return;
			}

			foreach (var asm in relevant)
				await asm.GetMetadataFileAsync().ConfigureAwait(true);

			var entity = await Task.Run(() => FindEntityInRelevantAssemblies(navigateTo, relevant));
			if (entity != null)
			{
				var node = FindTreeNode(entity);
				if (node != null)
					SelectNode(node);
			}
		}

		static IEntity? FindEntityInRelevantAssemblies(string navigateTo, IEnumerable<LoadedAssembly> relevantAssemblies)
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
			foreach (var asm in relevantAssemblies)
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

		static bool CanResolveTypeInPEFile(MetadataFile module, ITypeReference typeRef, out EntityHandle typeHandle)
		{
			// Reference assemblies are skipped so the loop keeps looking for an actual definition.
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

		void LoadAssemblies(IEnumerable<string> fileNames, List<LoadedAssembly>? loadedAssemblies = null, bool focusNode = true)
		{
			if (AssemblyList == null)
				return;

			AssemblyTreeNode? lastNode = null;
			foreach (var file in fileNames)
			{
				var assembly = AssemblyList.OpenAssembly(file);
				if (loadedAssemblies != null)
				{
					loadedAssemblies.Add(assembly);
					continue;
				}
				var node = assemblyListTreeNode?.FindAssemblyNode(assembly);
				if (node != null && focusNode)
					lastNode = node;
			}

			if (focusNode && lastNode != null)
				SelectNode(lastNode);
		}

		public void SortAssemblyList()
			=> AssemblyList?.Sort(AssemblyComparer.Instance);

		sealed class AssemblyComparer : IComparer<LoadedAssembly>
		{
			public static readonly AssemblyComparer Instance = new();
			public int Compare(LoadedAssembly? x, LoadedAssembly? y)
				=> string.Compare(x?.ShortName, y?.ShortName, StringComparison.CurrentCulture);
		}

		public void CollapseAll() => CollapseChildren(Root);

		static void CollapseChildren(SharpTreeNode? node)
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

		public void Refresh() => RefreshInternal();

		void RefreshInternal()
		{
			if (AssemblyList == null || listManager == null)
				return;
			var path = GetPathForNode(SelectedItem);
			ShowAssemblyList(listManager.LoadList(AssemblyList.ListName));
			SelectNode(FindNodeByPath(path, returnBestMatch: true));
		}
	}
}
