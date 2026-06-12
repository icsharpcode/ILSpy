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
using System.Collections.Specialized;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX.Abstractions;
using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.TreeNodes
{
	public abstract class ILSpyTreeNode : SharpTreeNode, ITreeNode
	{
		IEnumerable<ITreeNode> ITreeNode.Children => Children.OfType<ILSpyTreeNode>();

		/// <summary>
		/// Resolves the LanguageService from the MEF composition. Tree nodes use this to access
		/// the active <see cref="Language"/> for formatting their <see cref="SharpTreeNode.Text"/>.
		/// Resolved fresh each call: a static cache can hold a previous test's composition root in
		/// headless sweeps (the container is rebuilt per test), and a warm GetExport is a dictionary
		/// lookup. See <see cref="GetSuffixString(EntityHandle)"/> for the same rationale.
		/// </summary>
		protected static LanguageService LanguageService
			=> AppComposition.Current.GetExport<LanguageService>();

		/// <summary>
		/// The active <see cref="LanguageSettings"/>, or <see langword="null"/> when composition
		/// isn't available (design-time previews, minimal tests). Tree nodes that need to
		/// consult the filter on demand (e.g. <see cref="SharpTreeNode.ShowExpander"/> overrides
		/// that hide their chevron when every child is filtered out) use this without forcing
		/// the rest of their state to depend on a SettingsService injection.
		/// </summary>
		protected static LanguageSettings? CurrentLanguageSettings {
			get {
				try
				{
					// Resolve fresh each call -- a static cache can hold a previous test's composition
					// root in headless sweeps (the container is rebuilt per test, so a cached
					// SettingsService reports a stale ShowApiLevel and the filter cascade hides nodes
					// that the current settings would show). A warm GetExport is a dictionary lookup.
					return AppComposition.Current.GetExport<SettingsService>().SessionSettings.LanguageSettings;
				}
				catch
				{
					return null;
				}
			}
		}

		/// <summary>
		/// All MEF-discovered <see cref="IResourceNodeFactory"/> instances. Resource-tree nodes
		/// walk this list to dispatch a <see cref="ICSharpCode.Decompiler.Metadata.Resource"/> to
		/// the right specialised node (image / xml / .resources / …) before falling back to the
		/// generic node.
		/// </summary>
		protected static ICollection<IResourceNodeFactory> ResourceNodeFactories { get; }
			= AppComposition.Current.GetExports<IResourceNodeFactory>().ToArray();

		public Language Language => LanguageService.CurrentLanguage;

		/// <summary>
		/// Optional " @xNNNNNNNN" (hex) or " @NNNNN" (decimal) suffix appended to entity tree-node
		/// <see cref="SharpTreeNode.Text"/> values when the user enables Display Settings →
		/// "Show metadata tokens". Mirrors WPF's <c>ILSpyTreeNode.GetSuffixString</c>; format
		/// matches byte-for-byte so cross-tool grep on token strings keeps working.
		/// </summary>
		protected static string GetSuffixString(IMember member) => GetSuffixString(member.MetadataToken);

		protected static string GetSuffixString(EntityHandle handle)
		{
			try
			{
				// Resolve fresh each call — the static cachedSettingsService can hold a
				// previous test's composition root in headless test sweeps, and the cost of
				// MEF GetExport on a warm composition is a dictionary lookup.
				var settings = AppComposition.Current.GetExport<SettingsService>().DisplaySettings;
				if (!settings.ShowMetadataTokens)
					return string.Empty;
				int token = MetadataTokens.GetToken(handle);
				if (settings.ShowMetadataTokensInBase10)
					return " @" + token;
				return " @" + token.ToString("x8");
			}
			catch
			{
				return string.Empty;
			}
		}

		/// <summary>
		/// Long-form label used in navigation surfaces — back/forward history dropdowns, future
		/// breadcrumb rendering, etc. Defaults to <see cref="SharpTreeNode.Text"/>; override on
		/// nodes whose <see cref="SharpTreeNode.Text"/> is too generic to be useful out of
		/// context (e.g. "Base Types" → "Base Types (System.Exception)") so the user can tell
		/// dropdown entries apart at a glance.
		/// </summary>
		public override object? NavigationText => Text;

		/// <summary>
		/// True for nodes that represent assemblies the user did not open explicitly — the
		/// decompiler resolved them as part of another assembly's references and added them to
		/// the tree as a side effect. The list pane renders these in a distinct foreground so
		/// the user can tell auto-loaded entries from manually opened ones.
		/// </summary>
		public virtual bool IsAutoLoaded => false;

		/// <summary>
		/// Renders this node's decompiled representation to <paramref name="output"/> using
		/// <paramref name="language"/>. Default writes a stub comment so any node we forgot to
		/// override still produces *something*.
		/// </summary>
		public virtual void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, Text?.ToString() ?? GetType().Name);
		}

		/// <summary>
		/// Special "Save" handling for nodes that aren't textual code. Default returns false,
		/// meaning the host should use the regular decompile-to-file path. Override and return
		/// true after writing the node's content to handle Save inline (e.g. raw byte copy for
		/// embedded resources).
		/// </summary>
		public virtual bool Save() => false;

		/// <summary>
		/// True for nodes whose member is part of the assembly's public surface
		/// (Public / Protected / ProtectedOrInternal). Consulted by <see cref="Filter"/>
		/// to honour the <see cref="ApiVisibility.PublicOnly"/> setting.
		/// </summary>
		public virtual bool IsPublicAPI => true;

		/// <summary>Convenience inverse of <see cref="IsPublicAPI"/> for XAML class-bindings.</summary>
		public bool IsNonPublicAPI => !IsPublicAPI;

		/// <summary>
		/// Decides whether this node is visible under the current <paramref name="settings"/>.
		/// Overrides on member tree nodes consult <see cref="IsPublicAPI"/> plus
		/// <see cref="Languages.Language.ShowMember"/> (the compiler-generated cut) to drop
		/// non-matching entries. Default treats every node as visible.
		/// </summary>
		public virtual FilterResult Filter(LanguageSettings settings) => FilterResult.Match;

		/// <summary>
		/// When non-null, the docking host opens this tab for the node instead of routing it
		/// through the decompiler-text path. Lets metadata-table nodes show their own
		/// DataGrid view while the rest of the tree keeps decompiling.
		/// </summary>
		public virtual ContentPageModel? CreateTab() => null;

		/// <summary>
		/// Applies <see cref="Filter"/> to every newly-added child and writes the result
		/// into <see cref="SharpTreeNode.IsHidden"/>. Mirrors WPF's filter cascade — it's
		/// what makes accessor children of properties / events read as hidden under the
		/// default ShowApiLevel without overriding <see cref="SharpTreeNode.ShowExpander"/>
		/// per node-type. Only fires while this parent is visible (matches WPF) so the
		/// filter pass doesn't run for descendants whose parent chain isn't realised yet.
		/// </summary>
		public override void OnChildrenChanged(NotifyCollectionChangedEventArgs e)
		{
			base.OnChildrenChanged(e);
			if (e.NewItems == null || !IsVisible)
				return;
			foreach (var item in e.NewItems)
			{
				if (item is ILSpyTreeNode child)
					ApplyFilterToChild(child);
			}
		}

		/// <summary>
		/// Re-applies the filter cascade to every realised child when this node becomes
		/// visible. <see cref="OnChildrenChanged"/> only fires for newly-added children, and
		/// only when the parent was visible at the time — so children added under a hidden
		/// parent inherit stale <see cref="SharpTreeNode.IsHidden"/> states. Without this
		/// trigger, expanding a node that ShowApiLevel had hidden leaves its accessor
		/// children reading as visible even though the cascade would mark them hidden.
		/// Mirrors WPF's identical override on <c>ILSpyTreeNode</c>.
		/// </summary>
		protected override void OnIsVisibleChanged()
		{
			base.OnIsVisibleChanged();
			if (!IsVisible)
				return;
			foreach (var child in Children.OfType<ILSpyTreeNode>())
				ApplyFilterToChild(child);
		}

		void ApplyFilterToChild(ILSpyTreeNode child)
		{
			var settings = CurrentLanguageSettings;
			if (settings == null)
				return;
			var result = child.Filter(settings);
			switch (result)
			{
				case FilterResult.Hidden:
					child.IsHidden = true;
					break;
				case FilterResult.Match:
					child.IsHidden = false;
					break;
				case FilterResult.Recurse:
				case FilterResult.MatchAndRecurse:
					child.EnsureChildrenFiltered();
					child.IsHidden = child.Children.All(c => c.IsHidden);
					break;
			}
		}

		/// <summary>
		/// Forces this node's children to be materialised (when <see cref="SharpTreeNode.LazyLoading"/>
		/// is true) and runs <see cref="ApplyFilterToChild"/> on each one. Matches WPF's
		/// internal helper so the same filter cascade reaches Recurse / MatchAndRecurse
		/// branches.
		/// </summary>
		internal void EnsureChildrenFiltered()
		{
			EnsureLazyChildren();
			foreach (var child in Children.OfType<ILSpyTreeNode>())
				ApplyFilterToChild(child);
		}

		/// <summary>
		/// Re-applies the filter to already-realised children (without forcing lazy children to
		/// load) and recurses into realised subtrees. Called when ShowApiLevel changes so the
		/// visible tree updates in place -- a <c>TreeFlattener</c> then drops anything newly hidden.
		/// </summary>
		internal void RefreshRealizedFilter()
		{
			if (LazyLoading)
				return;
			foreach (var child in Children.OfType<ILSpyTreeNode>())
			{
				ApplyFilterToChild(child);
				child.RefreshRealizedFilter();
			}
		}
	}
}
