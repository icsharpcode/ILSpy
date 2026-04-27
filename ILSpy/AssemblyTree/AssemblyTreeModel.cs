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
using System.Composition;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Serialization;

using CommunityToolkit.Mvvm.ComponentModel;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Settings;
using ICSharpCode.ILSpyX.TreeView;

using ILSpy;
using ILSpy.Languages;
using ILSpy.TreeNodes;
using ILSpy.ViewModels;

namespace ILSpy.AssemblyTree
{
	[Export]
	[Shared]
	public partial class AssemblyTreeModel : ToolPaneModel
	{
		public const string PaneContentId = "AssemblyTree";

		readonly SettingsService settingsService;
		readonly LanguageService languageService;
		AssemblyListManager? listManager;

		[ObservableProperty]
		[property: IgnoreDataMember]
		private SharpTreeNode? root;

		[ObservableProperty]
		[property: IgnoreDataMember]
		private SharpTreeNode? selectedItem;

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
			this.settingsService = settingsService;
			this.languageService = languageService;
			languageService.PropertyChanged += (_, e) => {
				if (e.PropertyName == nameof(LanguageService.CurrentLanguage) && Root != null)
					NotifyTextChanged(Root);
			};
			Id = PaneContentId;
			Title = "Assemblies";
			CanClose = false;
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

		public void Initialize()
		{
			var settings = ILSpySettings.Load();
			listManager = new AssemblyListManager(settings);
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

			AssemblyList = listManager.LoadList(value);

			if (AssemblyList.GetAssemblies().Length == 0 && value == AssemblyListManager.DefaultListName)
				LoadInitialAssemblies(AssemblyList);

			Root = new AssemblyListTreeNode(AssemblyList);

			settingsService.SessionSettings.ActiveAssemblyList = value;

			// Restore the previously-selected tree node, if any. Walks the tree by ToString()
			// matching, materialising lazy children as it goes.
			var savedPath = settingsService.SessionSettings.ActiveTreeViewPath;
			if (savedPath is { Length: > 0 })
			{
				var restored = FindNodeByPath(savedPath, returnBestMatch: true);
				if (restored != null && restored != Root)
					SelectedItem = restored;
			}
		}

		partial void OnSelectedItemChanged(SharpTreeNode? value)
		{
			settingsService.SessionSettings.ActiveTreeViewPath = GetPathForNode(value);
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

		/// <summary>
		/// Finds the tree node corresponding to <paramref name="reference"/> — used by
		/// hyperlink clicks in the decompiler view to route to the right entity.
		/// Mirrors <c>ICSharpCode.ILSpy.AssemblyTree.AssemblyTreeModel.FindTreeNode</c> but only
		/// covers the references the Avalonia tree currently knows how to model.
		/// </summary>
		public ILSpyTreeNode? FindTreeNode(object? reference)
		{
			if (Root is not AssemblyListTreeNode root)
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
					return FindTypeNode(root, type);

				case IMember member:
					return FindMemberNode(root, member);

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

			// Walk up the nesting chain so we can drill down through declaring types.
			var nesting = new System.Collections.Generic.Stack<ITypeDefinition>();
			for (var current = type; current != null; current = current.DeclaringTypeDefinition)
				nesting.Push(current);

			// Top-level: find namespace, then the outermost type.
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
				IMethod m => typeNode.Children.OfType<MethodTreeNode>().FirstOrDefault(n => n.MethodDefinition.MetadataToken == m.MetadataToken),
				IProperty p => typeNode.Children.OfType<PropertyTreeNode>().FirstOrDefault(n => n.PropertyDefinition.MetadataToken == p.MetadataToken),
				IEvent e => typeNode.Children.OfType<EventTreeNode>().FirstOrDefault(n => n.EventDefinition.MetadataToken == e.MetadataToken),
				_ => null,
			};
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

		public void OpenAssembly(string path)
		{
			AssemblyList?.Open(path);
		}
	}
}
