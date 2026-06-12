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
using System.Collections.ObjectModel;
using System.Composition;
using System.Linq;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy.Analyzers.TreeNodes;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Analyzers
{
	[Export]
	[ExportToolPane(ContentId = PaneContentId, Alignment = ToolPaneAlignment.Bottom, Order = 0, IsVisibleByDefault = false)]
	[Shared]
	public class AnalyzerTreeViewModel : ToolPaneModel
	{
		public const string PaneContentId = "Analyzer";

		public AnalyzerTreeViewModel()
		{
			Id = PaneContentId;
			Title = "Analyzer";
			// IsRoot is computed (Parent == null) — the node is the root because we never
			// add it to another node's Children collection.
			Root = new AnalyzerRootNode();
		}

		/// <summary>
		/// Root of the analyzer pane's tree. Pre-populated as a child-less node; entries are
		/// added through <see cref="Analyze(IEntity)"/>.
		/// </summary>
		public AnalyzerRootNode Root { get; }

		/// <summary>
		/// Two-way binding sink for the tree grid's selection. Drives the context-menu's
		/// <c>TextViewContext.SelectedTreeNodes</c> so menu entries see the live selection.
		/// </summary>
		public ObservableCollection<SharpTreeNode> SelectedItems { get; } = new();

		/// <summary>
		/// Adds <paramref name="entity"/> to the analyzer pane (reusing the existing row if
		/// the entity is already analysed) and selects it. The pane's focus / dock-activation
		/// is the caller's responsibility — typically the context-menu entry that triggered
		/// the analysis.
		/// </summary>
		public AnalyzerEntityTreeNode Analyze(IEntity entity)
		{
			ArgumentNullException.ThrowIfNull(entity);
			var existing = Root.Children
				.OfType<AnalyzerEntityTreeNode>()
				.FirstOrDefault(n => IsSameEntity(n.Member, entity));
			if (existing != null)
			{
				SyncSelection(existing);
				return existing;
			}
			var node = Wrap(entity);
			Root.Children.Add(node);
			// Auto-expand the freshly-added node so its analyzers (Used By, Uses, ...) are visible
			// immediately, instead of leaving the user to expand it by hand after every Analyze.
			node.IsExpanded = true;
			SyncSelection(node);
			return node;
		}

		static bool IsSameEntity(IEntity? a, IEntity b)
		{
			if (a == null)
				return false;
			return a.MetadataToken == b.MetadataToken
				&& ReferenceEquals(a.ParentModule, b.ParentModule);
		}

		void SyncSelection(SharpTreeNode node)
		{
			SelectedItems.Clear();
			SelectedItems.Add(node);
		}

		static AnalyzerEntityTreeNode Wrap(IEntity entity)
		{
			return entity switch {
				ITypeDefinition type => new AnalyzedTypeTreeNode(type, source: entity),
				IMethod method => new AnalyzedMethodTreeNode(method, source: entity),
				IField field => new AnalyzedFieldTreeNode(field, source: entity),
				IProperty property => new AnalyzedPropertyTreeNode(property, source: entity),
				IEvent ev => new AnalyzedEventTreeNode(ev, source: entity),
				_ => throw new ArgumentOutOfRangeException(nameof(entity),
					$"Entity {entity.GetType().FullName} is not supported by the analyzer pane.")
			};
		}
	}
}
