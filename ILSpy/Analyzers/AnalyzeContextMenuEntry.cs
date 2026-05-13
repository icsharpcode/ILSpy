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

using ILSpy.TreeNodes;

namespace ILSpy.Analyzers
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
		Order = 100)]
	[Shared]
	public sealed class AnalyzeContextMenuEntry : IContextMenuEntry
	{
		readonly AnalyzerTreeViewModel analyzerTreeViewModel;

		[ImportingConstructor]
		public AnalyzeContextMenuEntry(AnalyzerTreeViewModel analyzerTreeViewModel)
		{
			this.analyzerTreeViewModel = analyzerTreeViewModel;
		}

		public bool IsVisible(TextViewContext context)
		{
			if (context.SelectedTreeNodes is not { Length: > 0 } nodes)
				return false;
			return nodes.All(n => n is IMemberTreeNode);
		}

		public bool IsEnabled(TextViewContext context)
		{
			if (context.SelectedTreeNodes is not { Length: > 0 } nodes)
				return false;
			return nodes.OfType<IMemberTreeNode>().All(n => IsAnalysable(n.Member));
		}

		public void Execute(TextViewContext context)
		{
			if (context.SelectedTreeNodes is not { Length: > 0 } nodes)
				return;
			foreach (var member in nodes.OfType<IMemberTreeNode>()
				.Select(n => n.Member)
				.Where(IsAnalysable))
			{
				analyzerTreeViewModel.Analyze(member!);
			}
		}

		/// <summary>
		/// Const fields are textual literals at every use-site rather than entities the
		/// analyser can match against — exclude them so the entry stays disabled.
		/// </summary>
		static bool IsAnalysable(IEntity? entity) => entity is not null and not IField { IsConst: true };
	}
}
