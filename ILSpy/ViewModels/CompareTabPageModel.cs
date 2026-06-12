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

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ICSharpCode.ILSpyX;

using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Compare
{
	/// <summary>
	/// Tab content for the assembly-comparison view. Constructed by
	/// <see cref="Commands.CompareContextMenuEntry"/> when the user right-clicks two
	/// assemblies and picks "Compare…". Static content (no language switching, no
	/// re-decompile) — the merge is computed once at construction.
	/// </summary>
	public sealed partial class CompareTabPageModel : ContentPageModel
	{
		readonly LoadedAssembly leftAssembly;
		readonly LoadedAssembly rightAssembly;

		[ObservableProperty]
		private ComparisonEntryTreeNode rootEntry;

		/// <summary>
		/// When true, identical rows surface in the tree. The default false hides them so
		/// only the actual differences are visible — the typical use case for "what changed
		/// between these two versions?"
		/// </summary>
		[ObservableProperty]
		private bool showIdentical;

		public LoadedAssembly LeftAssembly => leftAssembly;
		public LoadedAssembly RightAssembly => rightAssembly;

		public CompareTabPageModel(LoadedAssembly left, LoadedAssembly right)
		{
			// Compare view shows a structural metadata diff — language-agnostic, so the
			// toolbar's Language / Language-Version pickers shouldn't affect anything while
			// this tab is active. Mirrors WPF's CompareViewModel.SupportsLanguageSwitching=false.
			SupportsLanguageSwitching = false;
			leftAssembly = left;
			rightAssembly = right;
			Title = $"Compare {left.Text} - {right.Text}";

			var leftTree = CompareEngine.CreateEntityTree(left.GetTypeSystemOrNull()!);
			var rightTree = CompareEngine.CreateEntityTree(right.GetTypeSystemOrNull()!);
			var merged = CompareEngine.MergeTrees(leftTree.Root, rightTree.Root);
			rootEntry = new ComparisonEntryTreeNode(merged, this);

			ExpandAllCommand = new RelayCommand(OnExpandAll);
		}

		public IRelayCommand ExpandAllCommand { get; }

		void OnExpandAll()
		{
			ExpandRecursive(RootEntry);
		}

		static void ExpandRecursive(ComparisonEntryTreeNode node)
		{
			node.IsExpanded = true;
			node.EnsureLazyChildren();
			foreach (var child in node.Children)
				if (child is ComparisonEntryTreeNode c)
					ExpandRecursive(c);
		}

		partial void OnShowIdenticalChanged(bool value)
		{
			// Re-apply the filter cascade to refresh hidden/visible state — the existing
			// ApplyFilterToChild path looks at the new ShowIdentical flag through
			// ComparisonEntryTreeNode.Filter.
			RootEntry.EnsureChildrenFiltered();
		}
	}
}
