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

using System.Composition;
using System.Linq;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Analyzers
{
	/// <summary>
	/// Right-click → "Analyze" — pushes every selected member (type, method, field, property,
	/// event) into the analyzer pane. The pane's <see cref="AnalyzerTreeViewModel.Analyze"/>
	/// dedupes entries by <see cref="IEntity.MetadataToken"/> + parent module so re-running
	/// the menu on the same entity just refocuses the existing row.
	/// </summary>
	[ExportContextMenuEntry(
		Header = nameof(Resources.Analyze),
		Category = nameof(Resources.Analyze),
		InputGestureText = "Ctrl+R",
		Order = 200)]
	[Shared]
	public sealed class AnalyzeContextMenuEntry : IContextMenuEntry
	{
		readonly AnalyzerTreeViewModel analyzerTreeViewModel;
		readonly DockWorkspace dockWorkspace;

		[ImportingConstructor]
		public AnalyzeContextMenuEntry(AnalyzerTreeViewModel analyzerTreeViewModel, DockWorkspace dockWorkspace)
		{
			this.analyzerTreeViewModel = analyzerTreeViewModel;
			this.dockWorkspace = dockWorkspace;
		}

		public bool IsVisible(TextViewContext context)
		{
			if (context.SelectedTreeNodes is { Length: > 0 } nodes)
				return nodes.All(n => n is IMemberTreeNode);
			// Right-clicking a resolved symbol in the decompiled code: the reference carries the entity.
			return context.Reference?.Reference is IEntity;
		}

		public bool IsEnabled(TextViewContext context)
		{
			if (context.SelectedTreeNodes is { Length: > 0 } nodes)
				return nodes.OfType<IMemberTreeNode>().All(n => IsAnalysable(n.Member));
			return context.Reference?.Reference is IEntity entity && IsAnalysable(entity);
		}

		public void Execute(TextViewContext context)
		{
			var analysable = MembersToAnalyse(context);
			foreach (var member in analysable)
				analyzerTreeViewModel.Analyze(member);
			// Bring the analyzer pane to the front so the user can see the entity they just
			// added. AnalyzerTreeViewModel.Analyze deliberately leaves dock-activation to its
			// caller — that's this entry's job.
			if (analysable.Count > 0)
				dockWorkspace.ShowToolPane(AnalyzerTreeViewModel.PaneContentId);
		}

		// The analysable entities for this invocation: a tree-node selection (assembly/analyzer tree),
		// or the single resolved entity under a right-clicked code reference (decompiler text view).
		static System.Collections.Generic.List<IEntity> MembersToAnalyse(TextViewContext context)
		{
			if (context.SelectedTreeNodes is { Length: > 0 } nodes)
			{
				return nodes.OfType<IMemberTreeNode>()
					.Select(n => n.Member)
					.Where(IsAnalysable)
					.Select(m => m!)
					.ToList();
			}
			if (context.Reference?.Reference is IEntity entity && IsAnalysable(entity))
				return new System.Collections.Generic.List<IEntity> { entity };
			return new System.Collections.Generic.List<IEntity>();
		}

		/// <summary>
		/// Const fields are textual literals at every use-site rather than entities the
		/// analyser can match against — exclude them so the entry stays disabled.
		/// </summary>
		static bool IsAnalysable(IEntity? entity) => entity is not null and not IField { IsConst: true };
	}
}
