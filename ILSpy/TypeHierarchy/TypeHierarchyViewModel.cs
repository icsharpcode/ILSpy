// Copyright (c) 2026 Piero Viano
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
using System.Threading;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Analyzers;

namespace ICSharpCode.ILSpy.TypeHierarchy
{
	/// <summary>
	/// The Type Hierarchy tool pane: for one target type, a tree with its supertypes (base class
	/// and implemented interfaces, each expandable further up) and its subtypes (derived classes
	/// and implementations across the loaded assembly list, each expandable further down).
	/// The subtype search runs off the UI thread and is cancelled when the pane is retargeted.
	/// </summary>
	[Export]
	[ExportToolPane(ContentId = PaneContentId, Alignment = ToolPaneAlignment.Bottom, Order = 2, IsVisibleByDefault = false)]
	[Shared]
	public sealed partial class TypeHierarchyViewModel : ToolPaneModel
	{
		public const string PaneContentId = "TypeHierarchy";

		readonly AssemblyTreeModel assemblyTreeModel;
		readonly LanguageService languageService;
		CancellationTokenSource searchCts = new();

		[ObservableProperty]
		private TypeHierarchyNode? target;

		[ObservableProperty]
		private TypeHierarchyNode? selectedNode;

		[ImportingConstructor]
		public TypeHierarchyViewModel(AssemblyTreeModel assemblyTreeModel, LanguageService languageService)
		{
			this.assemblyTreeModel = assemblyTreeModel;
			this.languageService = languageService;
			Id = PaneContentId;
			Title = Resources.TypeHierarchy;
		}

		/// <summary>The tree's top level: the target node alone, or nothing before the first use.</summary>
		public ObservableCollection<TypeHierarchyNode> Roots { get; } = new();

		/// <summary>Token of the subtype searches started for the current target.</summary>
		internal CancellationToken SearchCancellationToken => searchCts.Token;

		/// <summary>
		/// Replaces the tree with the hierarchy of <paramref name="type"/>. Any subtype search still
		/// running for the previous target is cancelled.
		/// </summary>
		public void ShowHierarchy(ITypeDefinition type)
		{
			ArgumentNullException.ThrowIfNull(type);
			searchCts.Cancel();
			searchCts = new CancellationTokenSource();
			var token = searchCts.Token;

			var root = CreateTypeNode(TypeHierarchyNodeKind.Target, type, loader: null, token);
			var bases = new TypeHierarchyNode(TypeHierarchyNodeKind.BaseTypesGroup, Resources.BaseTypes,
				Images.SuperTypes, type, loader: null, token);
			foreach (var baseNode in CreateBaseTypeNodes(type, token))
				bases.Children.Add(baseNode);
			var derived = new TypeHierarchyNode(TypeHierarchyNodeKind.DerivedTypesGroup, Resources.DerivedTypes,
				Images.SubTypes, type, ct => LoadDerivedTypesAsync(type, ct), token);
			root.Children.Add(bases);
			root.Children.Add(derived);

			Roots.Clear();
			Roots.Add(root);
			Target = root;
			SelectedNode = root;
			root.IsExpanded = true;
			bases.IsExpanded = true;
			derived.IsExpanded = true;
		}

		/// <summary>
		/// Navigates to the type behind <paramref name="node"/> by selecting it in the assembly tree,
		/// which records a navigation-history entry like any other selection. Returns false when the
		/// node has no type or the type's assembly is not in the loaded list.
		/// </summary>
		public bool Activate(TypeHierarchyNode? node)
		{
			if (node is not { IsNavigable: true, Type: { } type })
				return false;
			return assemblyTreeModel.JumpToType(type);
		}

		/// <summary>
		/// The type a hierarchy request is about: a type itself, the declaring type of a member, the
		/// entity behind a tree node, or the entity an unresolved metadata reference points at.
		/// Null for anything else (locals, opcodes, namespaces, ...).
		/// </summary>
		public static ITypeDefinition? ResolveType(object? reference, AssemblyList? assemblyList = null)
		{
			switch (reference)
			{
				case ITypeDefinition definition:
					return definition;
				case IType type:
					return type.GetDefinition();
				case IMember member:
					return member.DeclaringTypeDefinition;
				case IMemberTreeNode node:
					return ResolveType(node.Member, assemblyList);
				case EntityReference entityReference when assemblyList != null:
					var entity = entityReference.Resolve(assemblyList);
					return entity == null ? null : ResolveType(entity, assemblyList);
				default:
					return null;
			}
		}

		TypeHierarchyNode CreateTypeNode(TypeHierarchyNodeKind kind, ITypeDefinition type,
			Func<CancellationToken, Task<IReadOnlyList<TypeHierarchyNode>>>? loader, CancellationToken token)
			=> new(kind, FormatType(type), TypeTreeNode.GetIcon(type), type, loader, token);

		string FormatType(ITypeDefinition type)
		{
			try
			{
				return languageService.CurrentLanguage?.TypeToString(type) ?? type.FullName;
			}
			catch (Exception)
			{
				return type.FullName;
			}
		}

		// The direct supertypes of a type are cheap to read, so they are produced synchronously;
		// each one again expands to its own supertypes.
		IEnumerable<TypeHierarchyNode> CreateBaseTypeNodes(ITypeDefinition type, CancellationToken token)
		{
			foreach (var baseType in type.DirectBaseTypes)
			{
				if (baseType.GetDefinition() is not { } definition)
					continue;
				// The type system reports System.Object as a base of every interface; an interface's
				// supertypes are only the interfaces it extends (as in the assembly tree's Base Types).
				if (type.Kind == TypeKind.Interface && definition.Kind != TypeKind.Interface)
					continue;
				yield return CreateTypeNode(TypeHierarchyNodeKind.BaseType, definition,
					ct => Task.FromResult<IReadOnlyList<TypeHierarchyNode>>(CreateBaseTypeNodes(definition, ct).ToList()),
					token);
			}
		}

		async Task<IReadOnlyList<TypeHierarchyNode>> LoadDerivedTypesAsync(ITypeDefinition type, CancellationToken token)
		{
			var assemblyList = assemblyTreeModel.AssemblyList;
			if (assemblyList == null)
				return Array.Empty<TypeHierarchyNode>();
			var found = await Task.Run(() => FindDirectSubtypes(assemblyList, type, token), token).ConfigureAwait(true);
			token.ThrowIfCancellationRequested();
			return found
				.Select(t => CreateTypeNode(TypeHierarchyNodeKind.DerivedType, t, ct => LoadDerivedTypesAsync(t, ct), token))
				.ToList();
		}

		/// <summary>
		/// Every type in the scope that can see <paramref name="type"/> whose direct base list names
		/// it: derived classes for a class, implementations and derived interfaces for an interface.
		/// Mirrors the matching done by the assembly tree's Derived Types node.
		/// </summary>
		internal static List<ITypeDefinition> FindDirectSubtypes(AssemblyList assemblyList, ITypeDefinition type, CancellationToken token)
		{
			var result = new List<ITypeDefinition>();
			var scope = new AnalyzerScope(assemblyList, type);
			foreach (var candidate in scope.GetTypesInScope(token))
			{
				token.ThrowIfCancellationRequested();
				foreach (var baseType in candidate.DirectBaseTypes)
				{
					if (baseType.FullName == type.FullName && baseType.TypeParameterCount == type.TypeParameterCount)
					{
						result.Add(candidate);
						break;
					}
				}
			}
			result.Sort((a, b) => string.CompareOrdinal(a.FullName, b.FullName));
			return result;
		}
	}
}
