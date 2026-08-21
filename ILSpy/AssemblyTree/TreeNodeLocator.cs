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
using System.Linq;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.AssemblyTree
{
	/// <summary>
	/// Resolves a metadata reference (entity, namespace, resource, assembly, ...) or a saved tree
	/// path to the matching <see cref="SharpTreeNode"/>, expanding lazy children along the way. This
	/// is the node-finding subsystem split out of <see cref="AssemblyTreeModel"/>; the model keeps
	/// thin instance/static wrappers that pass in its current tree root and assembly list.
	/// </summary>
	internal static class TreeNodeLocator
	{
		/// <summary>
		/// Walks down from <paramref name="root"/> matching each path segment against
		/// <see cref="object.ToString"/>, expanding lazy children along the way.
		/// </summary>
		public static SharpTreeNode? FindNodeByPath(SharpTreeNode? root, string[]? path, bool returnBestMatch)
		{
			if (path == null || root == null)
				return null;
			SharpTreeNode? node = root;
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
		/// Finds the tree node corresponding to <paramref name="reference"/> -- used by hyperlink
		/// clicks in the decompiler view and search-result activation to route to the right entity.
		/// Currently only covers the reference kinds the tree knows how to model.
		/// </summary>
		public static ILSpyTreeNode? FindTreeNode(AssemblyListTreeNode? root, AssemblyList? list, object? reference)
		{
			if (root == null)
				return null;

			switch (reference)
			{
				case EntityReference unresolved:
					return FindTreeNode(root, list, unresolved.Resolve(list!));

				case ITypeDefinition type:
					return FindTypeNode(root, type);

				case IMember member:
					return FindMemberNode(root, member);

				case LoadedAssembly lasm:
					return FindAssemblyNode(root, lasm);

				case MetadataFile metadataFile:
					return FindAssemblyNode(root, metadataFile);

				case Resource resource:
					return FindResourceNode(root, resource, null);

				case ValueTuple<Resource, string> resourceWithName:
					return FindResourceNode(root, resourceWithName.Item1, resourceWithName.Item2);

				case INamespace ns:
					return FindNamespaceNode(root, ns);

				default:
					return null;
			}
		}

		/// <summary>
		/// Finds the node for the assembly <paramref name="module"/> was loaded from, including
		/// assemblies nested inside a package or bundle. Package folders are expanded on the way
		/// down, so this resolves even when the user has never opened the package in the tree.
		/// </summary>
		public static AssemblyTreeNode? FindAssemblyNode(AssemblyListTreeNode root, MetadataFile? module)
			=> FindAssemblyNode(root, module?.GetLoadedAssemblyOrNull());

		/// <inheritdoc cref="FindAssemblyNode(AssemblyListTreeNode, MetadataFile?)"/>
		public static AssemblyTreeNode? FindAssemblyNode(AssemblyListTreeNode root, LoadedAssembly? assembly)
		{
			// A package child records the bundle it came from, so walking up that chain names every
			// package to descend into, outermost first, ending at the one top-level node.
			var nesting = new Stack<LoadedAssembly>();
			for (var current = assembly; current != null; current = current.ParentBundle)
				nesting.Push(current);
			if (nesting.Count == 0)
				return null;

			var node = root.FindAssemblyNode(nesting.Pop());
			while (node != null && nesting.Count > 0)
				node = FindNestedAssemblyNode(node, nesting.Pop());
			return node;
		}

		// Finds one assembly inside a package node, expanding only the folders on the path down to
		// it. Expanding a package folder resolves and extracts every .dll/.exe it holds, so the
		// path is taken from the package's in-memory folder graph first -- that costs no tree node
		// and reads no package entry.
		static AssemblyTreeNode? FindNestedAssemblyNode(AssemblyTreeNode packageNode, LoadedAssembly assembly)
		{
			var rootFolder = packageNode.LoadedAssembly.GetLoadResultAsync().GetAwaiter().GetResult().Package?.RootFolder;
			if (rootFolder == null)
				return null;
			var path = new HashSet<PackageFolder>();
			if (!rootFolder.HasResolved(assembly) && !CollectFolderPath(rootFolder, assembly, path))
				return null;

			SharpTreeNode node = packageNode;
			while (true)
			{
				node.EnsureLazyChildren();
				if (node.Children.OfType<AssemblyTreeNode>().FirstOrDefault(n => n.LoadedAssembly == assembly) is { } nested)
					return nested;
				// A folder node stands for the deepest link of a collapsed single-child chain
				// (a/b/c shows as one node), so match its folder against the whole path rather
				// than against one expected next segment.
				if (node.Children.OfType<PackageFolderTreeNode>().FirstOrDefault(f => path.Contains(f.Folder)) is not { } next)
					return null;
				node = next;
			}
		}

		// Fills <paramref name="path"/> with the folders between <paramref name="folder"/> (exclusive)
		// and the one that already resolved <paramref name="assembly"/>. Every package-nested
		// LoadedAssembly is produced by exactly one PackageFolder.ResolveEntry call, so the owning
		// folder's resolution cache is what identifies it.
		static bool CollectFolderPath(PackageFolder folder, LoadedAssembly assembly, HashSet<PackageFolder> path)
		{
			foreach (var subFolder in folder.Folders)
			{
				if (subFolder.HasResolved(assembly) || CollectFolderPath(subFolder, assembly, path))
				{
					path.Add(subFolder);
					return true;
				}
			}
			return false;
		}

		// Resolves a resource (optionally a named sub-entry) to its tree node. Mirrors the previous
		// version's AssemblyListTreeNode.FindResourceNode so resource search results / links navigate.
		static ILSpyTreeNode? FindResourceNode(AssemblyListTreeNode root, Resource resource, string? name)
		{
			if (resource == null)
				return null;
			ILSpyTreeNode? resourceNode = null;
			foreach (var node in root.Children.OfType<AssemblyTreeNode>())
			{
				if (!node.LoadedAssembly.IsLoaded)
					continue;
				node.EnsureLazyChildren();
				foreach (var list in node.Children.OfType<ResourceListTreeNode>())
				{
					resourceNode = list.Children.OfType<ResourceTreeNode>().FirstOrDefault(x => x.Resource == resource)
						?? (ILSpyTreeNode?)list.Children.OfType<ResourceEntryNode>().FirstOrDefault(x => resource.Name.Equals(x.Text));
					if (resourceNode != null)
						break;
				}
				if (resourceNode != null)
					break;
			}
			if (resourceNode == null || name == null || name.Equals(resourceNode.Text))
				return resourceNode;
			resourceNode.EnsureLazyChildren();
			return resourceNode.Children.OfType<ILSpyTreeNode>().FirstOrDefault(x => name.Equals(x.Text)) ?? resourceNode;
		}

		// Resolves a namespace to its tree node within its contributing assembly.
		static NamespaceTreeNode? FindNamespaceNode(AssemblyListTreeNode root, INamespace ns)
		{
			var module = ns.ContributingModules.FirstOrDefault();
			if (module?.MetadataFile == null)
				return null;
			// The assembly node indexes every namespace it built by full, unescaped name. Matching
			// against the node labels instead would fail in nested-namespace mode, where the node
			// for "A.B.C" is a descendant and its label is only the last segment.
			return FindAssemblyNode(root, module.MetadataFile)?.FindNamespaceNode(ns.FullName);
		}

		public static TypeTreeNode? FindTypeNode(AssemblyListTreeNode root, ITypeDefinition type)
		{
			var module = type.ParentModule?.MetadataFile;
			if (module == null)
				return null;
			var assembly = FindAssemblyNode(root, module);
			if (assembly == null)
				return null;

			var nesting = new Stack<ITypeDefinition>();
			for (var current = type; current != null; current = current.DeclaringTypeDefinition)
				nesting.Push(current);

			// The assembly node indexes every top-level type it built, so this resolves regardless of
			// how deep the type's namespace nests. Nested types are not in that index -- they are
			// loaded lazily by their declaring type's node -- so walk the remaining chain by handle.
			var typeNode = assembly.FindTypeNode(nesting.Pop());
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
	}
}
