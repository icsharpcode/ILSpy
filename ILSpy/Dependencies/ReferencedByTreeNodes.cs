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
using System.Linq;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Dependencies;
using ICSharpCode.ILSpyX.TreeView.PlatformAbstractions;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Dependencies
{
	/// <summary>
	/// "Referenced By" folder under an assembly node: the reverse of the References folder. Lists
	/// the assemblies of the current list that reference this assembly (matched by name and public
	/// key token), each of which expands to its own referencing assemblies.
	/// </summary>
	public sealed class ReferencedByFolderTreeNode : ILSpyTreeNode
	{
		readonly MetadataFile module;
		readonly AssemblyTreeNode parentAssembly;

		public ReferencedByFolderTreeNode(MetadataFile module, AssemblyTreeNode parentAssembly)
		{
			this.module = module ?? throw new ArgumentNullException(nameof(module));
			this.parentAssembly = parentAssembly ?? throw new ArgumentNullException(nameof(parentAssembly));
			LazyLoading = true;
		}

		public MetadataFile Module => module;

		public override object Text => Resources.ReferencedBy;

		public override object? NavigationText => $"{Text} ({module.Name})";

		public override object Icon => Images.ReferenceFolder;

		protected override void LoadChildren()
		{
			foreach (var child in ReferencingAssemblyTreeNode.CreateChildren(parentAssembly.LoadedAssembly.AssemblyList, module, this))
				Children.Add(child);
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			EnsureLazyChildren();
			output.WriteLine($"Assemblies in the list that reference {module.FullName}:");
			output.WriteLine();
			if (Children.Count == 0)
				output.WriteLine("(none)");
			foreach (var node in Children.OfType<ReferencingAssemblyTreeNode>())
				node.WriteSummary(output);
		}
	}

	/// <summary>
	/// One referencing assembly under a <see cref="ReferencedByFolderTreeNode"/> (or, recursively,
	/// under another referencing assembly). Activating it selects that assembly in the tree.
	/// </summary>
	public sealed class ReferencingAssemblyTreeNode : ILSpyTreeNode
	{
		readonly ReferencingAssembly referencing;

		ReferencingAssemblyTreeNode(ReferencingAssembly referencing)
		{
			this.referencing = referencing;
			LazyLoading = true;
		}

		public LoadedAssembly Assembly => referencing.Assembly;

		public AssemblyReference Reference => referencing.Reference;

		public bool IsVersionMismatch => referencing.IsVersionMismatch;

		public override object Text {
			get {
				var name = ILAmbience.EscapeName(referencing.Assembly.ShortName);
				var metadata = referencing.Module.Metadata;
				var version = metadata.IsAssembly ? metadata.GetAssemblyDefinition().Version : null;
				var text = version != null ? $"{name} ({version})" : name;
				if (referencing.IsVersionMismatch)
					text += $" - references v{referencing.Reference.Version}";
				return text;
			}
		}

		public override object? NavigationText => $"{Text} ({Resources.ReferencedBy})";

		public override object Icon => referencing.IsVersionMismatch ? Images.AssemblyWarning : Images.Assembly;

		public override object? ToolTip => referencing.Assembly.FileName;

		/// <summary>
		/// One node per assembly of <paramref name="list"/> that references <paramref name="target"/>,
		/// skipping assemblies already on the path from <paramref name="parent"/> up to the folder so
		/// that reference cycles do not recurse forever.
		/// </summary>
		internal static System.Collections.Generic.IEnumerable<ReferencingAssemblyTreeNode> CreateChildren(
			AssemblyList list, MetadataFile target, ILSpyTreeNode parent)
		{
			var onPath = parent.AncestorsAndSelf()
				.Select(n => n switch {
					ReferencingAssemblyTreeNode r => r.referencing.Module,
					ReferencedByFolderTreeNode f => f.Module,
					_ => null,
				})
				.Where(m => m != null)
				.ToHashSet();
			foreach (var referencing in ReferencedByFinder.FindReferencingAssemblies(list.GetAssemblies(), target))
			{
				if (!onPath.Contains(referencing.Module))
					yield return new ReferencingAssemblyTreeNode(referencing);
			}
		}

		protected override void LoadChildren()
		{
			foreach (var child in CreateChildren(referencing.Assembly.AssemblyList, referencing.Module, this))
				Children.Add(child);
		}

		public override void ActivateItem(IPlatformRoutedEventArgs e)
		{
			var model = AppComposition.TryGetExport<AssemblyTreeModel>();
			if (model?.Root is not AssemblyListTreeNode listNode)
				return;
			var node = listNode.FindAssemblyNode(referencing.Assembly);
			if (node == null)
				return;
			model.SelectedItem = node;
			e.Handled = true;
		}

		internal void WriteSummary(ITextOutput output)
		{
			output.WriteLine($"{Text}");
			output.Indent();
			output.WriteLine("File: " + referencing.Assembly.FileName);
			output.WriteLine("Reference: " + referencing.Reference.FullName);
			output.Unindent();
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			WriteSummary(output);
		}
	}
}
