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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Windows;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.TreeView;

namespace ICSharpCode.ILSpy.TreeNodes
{
	/// <summary>
	/// Represents a list of assemblies.
	/// This is used as (invisible) root node of the tree view.
	/// </summary>
	sealed class AssemblyListTreeNode : ILSpyTreeNode
	{
		readonly AssemblyList assemblyList;

		public AssemblyList AssemblyList {
			get { return assemblyList; }
		}

		public AssemblyListTreeNode(AssemblyList assemblyList)
		{
			this.assemblyList = assemblyList ?? throw new ArgumentNullException(nameof(assemblyList));
			BindToObservableCollection(assemblyList.assemblies);
		}

		public override object Text {
			get { return assemblyList.ListName; }
		}

		void BindToObservableCollection(ObservableCollection<LoadedAssembly> collection)
		{
			this.Children.Clear();
			this.Children.AddRange(collection.Select(a => new AssemblyTreeNode(a)));
			collection.CollectionChanged += delegate (object sender, NotifyCollectionChangedEventArgs e) {
				switch (e.Action)
				{
					case NotifyCollectionChangedAction.Add:
						this.Children.InsertRange(e.NewStartingIndex, e.NewItems.Cast<LoadedAssembly>().Select(a => new AssemblyTreeNode(a)));
						break;
					case NotifyCollectionChangedAction.Remove:
						this.Children.RemoveRange(e.OldStartingIndex, e.OldItems.Count);
						break;
					case NotifyCollectionChangedAction.Replace:
					case NotifyCollectionChangedAction.Move:
						throw new NotImplementedException();
					case NotifyCollectionChangedAction.Reset:
						this.Children.Clear();
						this.Children.AddRange(collection.Select(a => new AssemblyTreeNode(a)));
						break;
					default:
						throw new NotSupportedException("Invalid value for NotifyCollectionChangedAction");
				}
			};
		}

		public override bool CanDrop(DragEventArgs e, int index)
		{
			e.Effects = DragDropEffects.Move | DragDropEffects.Copy | DragDropEffects.Link;
			if (e.Data.GetDataPresent(AssemblyTreeNode.DataFormat))
				return true;
			else if (e.Data.GetDataPresent(DataFormats.FileDrop))
				return true;
			else
			{
				e.Effects = DragDropEffects.None;
				return false;
			}
		}

		public override void Drop(DragEventArgs e, int index)
		{
			string[] files = e.Data.GetData(AssemblyTreeNode.DataFormat) as string[];
			if (files == null)
				files = e.Data.GetData(DataFormats.FileDrop) as string[];
			if (files != null)
			{
				lock (assemblyList.assemblies)
				{
					var assemblies = files
						.Where(file => file != null)
						.SelectMany(file => OpenAssembly(assemblyList, file))
						.Where(asm => asm != null)
						.Distinct()
						.ToArray();
					foreach (LoadedAssembly asm in assemblies)
					{
						int nodeIndex = assemblyList.assemblies.IndexOf(asm);
						if (nodeIndex < index)
							index--;
						assemblyList.assemblies.RemoveAt(nodeIndex);
					}
					Array.Reverse(assemblies);
					foreach (LoadedAssembly asm in assemblies)
					{
						assemblyList.assemblies.Insert(index, asm);
					}
					var nodes = assemblies.SelectArray(MainWindow.Instance.FindTreeNode);
					MainWindow.Instance.SelectNodes(nodes);
				}
			}
		}

		private IEnumerable<LoadedAssembly> OpenAssembly(AssemblyList assemblyList, string file)
		{
			if (file.EndsWith(".nupkg"))
			{
				LoadedNugetPackage package = new LoadedNugetPackage(file);
				var selectionDialog = new NugetPackageBrowserDialog(package);
				selectionDialog.Owner = Application.Current.MainWindow;
				if (selectionDialog.ShowDialog() != true)
					yield break;
				foreach (var entry in selectionDialog.SelectedItems)
				{
					var nugetAsm = assemblyList.OpenAssembly("nupkg://" + file + ";" + entry.Name, entry.Stream, true);
					if (nugetAsm != null)
					{
						yield return nugetAsm;
					}
				}
				yield break;
			}
			yield return assemblyList.OpenAssembly(file);
		}

		public Action<SharpTreeNode> Select = delegate { };

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "List: " + assemblyList.ListName);
			output.WriteLine();
			foreach (AssemblyTreeNode asm in this.Children)
			{
				language.WriteCommentLine(output, new string('-', 60));
				output.WriteLine();
				asm.Decompile(language, output, options);
			}
		}

		#region Find*Node
		public ILSpyTreeNode FindResourceNode(Resource resource)
		{
			if (resource == null || resource.IsNil)
				return null;
			foreach (AssemblyTreeNode node in this.Children)
			{
				if (node.LoadedAssembly.IsLoaded)
				{
					node.EnsureLazyChildren();
					foreach (var item in node.Children.OfType<ResourceListTreeNode>())
					{
						var founded = item.Children.OfType<ResourceTreeNode>().Where(x => x.Resource == resource).FirstOrDefault();
						if (founded != null)
							return founded;

						var foundedResEntry = item.Children.OfType<ResourceEntryNode>().Where(x => resource.Name.Equals(x.Text)).FirstOrDefault();
						if (foundedResEntry != null)
							return foundedResEntry;
					}
				}
			}
			return null;
		}

		public ILSpyTreeNode FindResourceNode(Resource resource, string name)
		{
			var resourceNode = FindResourceNode(resource);
			if (resourceNode == null || name == null || name.Equals(resourceNode.Text))
				return resourceNode;

			resourceNode.EnsureLazyChildren();
			return resourceNode.Children.OfType<ILSpyTreeNode>().Where(x => name.Equals(x.Text)).FirstOrDefault() ?? resourceNode;
		}

		public AssemblyTreeNode FindAssemblyNode(IModule module)
		{
			return FindAssemblyNode(module.PEFile);
		}

		public AssemblyTreeNode FindAssemblyNode(PEFile module)
		{
			if (module == null)
				return null;
			App.Current.Dispatcher.VerifyAccess();
			foreach (AssemblyTreeNode node in this.Children)
			{
				if (node.LoadedAssembly.IsLoaded && node.LoadedAssembly.GetPEFileOrNull()?.FileName == module.FileName)
					return node;
			}
			return null;
		}

		public AssemblyTreeNode FindAssemblyNode(LoadedAssembly asm)
		{
			if (asm == null)
				return null;
			App.Current.Dispatcher.VerifyAccess();
			foreach (AssemblyTreeNode node in this.Children)
			{
				if (node.LoadedAssembly == asm)
					return node;
			}
			return null;
		}

		/// <summary>
		/// Looks up the type node corresponding to the type definition.
		/// Returns null if no matching node is found.
		/// </summary>
		public TypeTreeNode FindTypeNode(ITypeDefinition def)
		{
			if (def == null)
				return null;
			var declaringType = def.DeclaringTypeDefinition;
			if (declaringType != null)
			{
				TypeTreeNode decl = FindTypeNode(declaringType);
				if (decl != null)
				{
					decl.EnsureLazyChildren();
					return decl.Children.OfType<TypeTreeNode>().FirstOrDefault(t => t.TypeDefinition.MetadataToken == def.MetadataToken && !t.IsHidden);
				}
			}
			else
			{
				AssemblyTreeNode asm = FindAssemblyNode(def.ParentModule);
				if (asm != null)
				{
					return asm.FindTypeNode(def);
				}
			}
			return null;
		}

		/// <summary>
		/// Looks up the method node corresponding to the method definition.
		/// Returns null if no matching node is found.
		/// </summary>
		public ILSpyTreeNode FindMethodNode(IMethod def)
		{
			TypeTreeNode typeNode = FindTypeNode(def.DeclaringTypeDefinition);
			if (typeNode == null)
				return null;
			// method might be an accessor, must look for parent node
			ILSpyTreeNode parentNode = typeNode;
			MethodTreeNode methodNode;
			parentNode.EnsureLazyChildren();
			switch (def.AccessorOwner)
			{
				case IProperty p:
					parentNode = parentNode.Children.OfType<PropertyTreeNode>().FirstOrDefault(m => m.PropertyDefinition.MetadataToken == p.MetadataToken && !m.IsHidden);
					if (parentNode == null)
						return null;
					parentNode.EnsureLazyChildren();
					methodNode = parentNode.Children.OfType<MethodTreeNode>().FirstOrDefault(m => m.MethodDefinition.MetadataToken == def.MetadataToken && !m.IsHidden);
					if (methodNode == null || methodNode.IsHidden)
						return parentNode;
					return methodNode;
				case IEvent e:
					parentNode = parentNode.Children.OfType<EventTreeNode>().FirstOrDefault(m => m.EventDefinition.MetadataToken == e.MetadataToken && !m.IsHidden);
					if (parentNode == null)
						return null;
					parentNode.EnsureLazyChildren();
					methodNode = parentNode.Children.OfType<MethodTreeNode>().FirstOrDefault(m => m.MethodDefinition.MetadataToken == def.MetadataToken && !m.IsHidden);
					if (methodNode == null || methodNode.IsHidden)
						return parentNode;
					return methodNode;
				default:
					methodNode = typeNode.Children.OfType<MethodTreeNode>().FirstOrDefault(m => m.MethodDefinition.MetadataToken == def.MetadataToken && !m.IsHidden);
					if (methodNode != null)
						return methodNode;
					return null;
			}
		}

		/// <summary>
		/// Looks up the field node corresponding to the field definition.
		/// Returns null if no matching node is found.
		/// </summary>
		public FieldTreeNode FindFieldNode(IField def)
		{
			TypeTreeNode typeNode = FindTypeNode(def.DeclaringTypeDefinition);
			if (typeNode == null)
				return null;
			typeNode.EnsureLazyChildren();
			return typeNode.Children.OfType<FieldTreeNode>().FirstOrDefault(m => m.FieldDefinition.MetadataToken == def.MetadataToken && !m.IsHidden);
		}

		/// <summary>
		/// Looks up the property node corresponding to the property definition.
		/// Returns null if no matching node is found.
		/// </summary>
		public PropertyTreeNode FindPropertyNode(IProperty def)
		{
			TypeTreeNode typeNode = FindTypeNode(def.DeclaringTypeDefinition);
			if (typeNode == null)
				return null;
			typeNode.EnsureLazyChildren();
			return typeNode.Children.OfType<PropertyTreeNode>().FirstOrDefault(m => m.PropertyDefinition.MetadataToken == def.MetadataToken && !m.IsHidden);
		}

		/// <summary>
		/// Looks up the event node corresponding to the event definition.
		/// Returns null if no matching node is found.
		/// </summary>
		public EventTreeNode FindEventNode(IEvent def)
		{
			TypeTreeNode typeNode = FindTypeNode(def.DeclaringTypeDefinition);
			if (typeNode == null)
				return null;
			typeNode.EnsureLazyChildren();
			return typeNode.Children.OfType<EventTreeNode>().FirstOrDefault(m => m.EventDefinition.MetadataToken == def.MetadataToken && !m.IsHidden);
		}

		/// <summary>
		/// Looks up the event node corresponding to the namespace definition.
		/// Returns null if no matching node is found.
		/// </summary>
		public NamespaceTreeNode FindNamespaceNode(INamespace def)
		{
			var module = def.ContributingModules.FirstOrDefault();
			if (module == null)
				return null;

			AssemblyTreeNode assemblyNode = FindAssemblyNode(module);
			if (assemblyNode == null)
				return null;

			assemblyNode.EnsureLazyChildren();
			return assemblyNode.Children.OfType<NamespaceTreeNode>().FirstOrDefault(n => def.FullName.Length == 0 || def.FullName.Equals(n.Text));
		}
		#endregion
	}
}
