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

using System.Linq;

using Avalonia.Threading;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy.Languages;

namespace ICSharpCode.ILSpy.TreeNodes
{
	/// <summary>
	/// Lists the embedded resources of an assembly. Children are <see cref="ResourceTreeNode"/>
	/// instances created via the <see cref="ResourceTreeNode.Create"/> factory.
	/// </summary>
	sealed class ResourceListTreeNode : ILSpyTreeNode
	{
		readonly MetadataFile module;

		public ResourceListTreeNode(MetadataFile module)
		{
			this.module = module;
			LazyLoading = true;
		}

		public override object Text => Resources._Resources;

		public override object? NavigationText => $"{Text} ({module.Name})";

		public override object Icon => IsExpanded ? Images.FolderOpen : Images.FolderClosed;

		protected override void OnExpanding()
		{
			base.OnExpanding();
			RaisePropertyChanged(nameof(Icon));
		}

		protected override void OnCollapsing()
		{
			base.OnCollapsing();
			RaisePropertyChanged(nameof(Icon));
		}

		protected override void LoadChildren()
		{
			foreach (var r in module.Resources.OrderBy(m => m.Name, NaturalStringComparer.Instance))
				Children.Add(ResourceTreeNode.Create(r));
		}

		public override FilterResult Filter(LanguageSettings settings)
		{
			// WPF's variant short-circuits on string.IsNullOrEmpty(SearchTerm) — when no term
			// is set the folder reads as a plain match, otherwise the tree walks into resources
			// to surface only matching ones. We use the same shape so a future search pane
			// behaves identically.
			if (string.IsNullOrEmpty(settings.SearchTerm))
				return FilterResult.MatchAndRecurse;
			else
				return FilterResult.Recurse;
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			Dispatcher.UIThread.Invoke(EnsureLazyChildren);
			foreach (var child in Children.OfType<ILSpyTreeNode>())
			{
				child.Decompile(language, output, options);
				output.WriteLine();
			}
		}
	}
}
