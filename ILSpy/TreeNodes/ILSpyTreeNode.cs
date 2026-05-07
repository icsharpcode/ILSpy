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
using System.Linq;

using ICSharpCode.Decompiler;
using ICSharpCode.ILSpyX.Abstractions;
using ICSharpCode.ILSpyX.TreeView;

using ILSpy.AppEnv;
using ILSpy.Languages;
using ILSpy.ViewModels;

namespace ILSpy.TreeNodes
{
	public abstract class ILSpyTreeNode : SharpTreeNode, ITreeNode
	{
		IEnumerable<ITreeNode> ITreeNode.Children => Children.OfType<ILSpyTreeNode>();

		static LanguageService? cachedLanguageService;

		/// <summary>
		/// Resolves the LanguageService from the MEF composition. Tree nodes use this to access
		/// the active <see cref="Language"/> for formatting their <see cref="SharpTreeNode.Text"/>.
		/// </summary>
		protected static LanguageService LanguageService
			=> cachedLanguageService ??= AppComposition.Current.GetExport<LanguageService>();

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
		public virtual TabPageModel? CreateTab() => null;
	}
}
