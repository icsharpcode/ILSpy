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
					return root.FindAssemblyNode(lasm);

				case MetadataFile metadataFile:
					return root.Children.OfType<AssemblyTreeNode>()
						.FirstOrDefault(a => a.LoadedAssembly.GetMetadataFileOrNull() == metadataFile);

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

		// Resolves a namespace to its tree node within its contributing assembly. Mirrors the previous
		// version's AssemblyListTreeNode.FindNamespaceNode.
		static NamespaceTreeNode? FindNamespaceNode(AssemblyListTreeNode root, INamespace ns)
		{
			var module = ns.ContributingModules.FirstOrDefault();
			if (module?.MetadataFile == null)
				return null;
			var assembly = root.Children.OfType<AssemblyTreeNode>()
				.FirstOrDefault(a => a.LoadedAssembly.GetMetadataFileOrNull() == module.MetadataFile);
			if (assembly == null)
				return null;
			assembly.EnsureLazyChildren();
			return assembly.Children.OfType<NamespaceTreeNode>()
				.FirstOrDefault(n => ns.FullName.Length == 0 || ns.FullName.Equals(n.Text));
		}

		public static TypeTreeNode? FindTypeNode(AssemblyListTreeNode root, ITypeDefinition type)
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
	}
}
