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

using System.Collections.Generic;
using System.Composition;

using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Analyzers;
using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Languages;

namespace ICSharpCode.ILSpy.Analyzers
{
	/// <summary>
	/// Base for every node in the analyzer pane. The pane root, the per-entity wrappers
	/// (<see cref="AnalyzerEntityTreeNode"/>), and the per-analyzer "search" headers
	/// (<see cref="AnalyzerSearchTreeNode"/>) all derive from this so the root's
	/// <c>HandleAssemblyListChanged</c> sweep can recurse uniformly.
	/// </summary>
	public abstract class AnalyzerTreeNode : SharpTreeNode
	{
		static LanguageService? cachedLanguageService;
		static AnalyzerRegistry? cachedRegistry;
		static AssemblyTreeModel? cachedAssemblyTreeModel;

		/// <summary>
		/// The active language used to format entity text. Resolved lazily through the
		/// composition host so design-time previews (no MEF) don't NRE during XAML reload.
		/// </summary>
		protected static Languages.Language Language
			=> (cachedLanguageService ??= AppComposition.Current.GetExport<LanguageService>()).CurrentLanguage;

		/// <summary>
		/// The active <see cref="AssemblyList"/> backing the assembly tree. Search nodes pass
		/// it into <c>AnalyzerContext</c> so each analyser can iterate the loaded modules.
		/// </summary>
		protected static AssemblyList? CurrentAssemblyList
			=> (cachedAssemblyTreeModel ??= AppComposition.Current.GetExport<AssemblyTreeModel>()).AssemblyList;

		/// <summary>
		/// All MEF-registered <see cref="IAnalyzer"/> exports, ordered by their declared
		/// <see cref="AnalyzerMetadata.Order"/>. Each <see cref="AnalyzerEntityTreeNode"/>
		/// walks this list on <c>LoadChildren</c> and instantiates an
		/// <see cref="AnalyzerSearchTreeNode"/> for every entry whose
		/// <see cref="IAnalyzer.Show"/> returns true for the wrapped entity.
		/// </summary>
		public static IReadOnlyList<ExportFactory<IAnalyzer, AnalyzerMetadata>> Analyzers
			=> (cachedRegistry ??= AppComposition.Current.GetExport<AnalyzerRegistry>()).Analyzers;

		public override bool CanDelete() => Parent is { IsRoot: true };

		public override void DeleteCore() => Parent?.Children.Remove(this);

		public override void Delete() => DeleteCore();

		/// <summary>
		/// Reacts to add / remove events on the active <see cref="AssemblyList"/>. Each
		/// node decides whether it (and its subtree) is still relevant — typically by
		/// checking the removed list against its source module — and returns <c>false</c>
		/// to ask its parent to drop it.
		/// </summary>
		public abstract bool HandleAssemblyListChanged(
			ICollection<LoadedAssembly> removedAssemblies,
			ICollection<LoadedAssembly> addedAssemblies);
	}
}
