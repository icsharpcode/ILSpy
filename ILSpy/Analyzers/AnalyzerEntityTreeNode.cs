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

using AvaloniaEdit.Highlighting;

using ICSharpCode.Decompiler.Output;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.TreeView;
using ICSharpCode.ILSpyX.TreeView.PlatformAbstractions;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Controls.TreeView;
using ICSharpCode.ILSpy.Themes;
using ICSharpCode.ILSpy.Util;

namespace ICSharpCode.ILSpy.Analyzers
{
	/// <summary>
	/// Base for nodes that wrap an <see cref="IEntity"/> in the analyzer pane (the
	/// per-entity root row plus every analyser result row underneath an
	/// <see cref="AnalyzerSearchTreeNode"/>). Concrete subclasses supply the entity, its
	/// icon, and its text; this base owns the navigation hook and the assembly-change
	/// pruning logic.
	/// </summary>
	public abstract class AnalyzerEntityTreeNode : AnalyzerTreeNode, IRichTextNode
	{
		// Flags reproducing the plain signature the pane used before highlighting: the member's
		// declaring type, fully-qualified names, and the usual return-type/parameter detail.
		protected const ConversionFlags MemberSignatureFlags =
			ConversionFlags.ShowDeclaringType
			| ConversionFlags.UseFullyQualifiedEntityNames
			| ConversionFlags.ShowReturnType
			| ConversionFlags.ShowParameterList
			| ConversionFlags.ShowParameterModifiers
			| ConversionFlags.ShowTypeParameterList
			| ConversionFlags.PlaceReturnTypeAfterParameterList;

		// Type rows show a fully-qualified type signature with its type parameters.
		protected const ConversionFlags TypeSignatureFlags =
			ConversionFlags.UseFullyQualifiedEntityNames
			| ConversionFlags.UseFullyQualifiedTypeNames
			| ConversionFlags.ShowTypeParameterList;

		RichText? richText;
		// The build key: the signature's colours depend on the active theme and its formatting on the
		// active language, so the cache is rebuilt when either changes (not just once).
		(string? theme, object language)? richTextKey;

		/// <summary>
		/// The highlighted, type-name-emboldened signature for this row, cached until the theme or
		/// language changes. Falls back to <see langword="null"/> (plain <c>Text</c>) for non-entity rows.
		/// </summary>
		public RichText? CreateRichText()
		{
			var key = (ThemeManager.Current.Theme, (object)Language);
			if (richTextKey != key)
			{
				richText = BuildRichText();
				richTextKey = key;
			}
			return richText;
		}

		protected virtual RichText? BuildRichText() => null;

		/// <summary>
		/// Builds the coloured signature for <see cref="Member"/> with the given flags, bolding
		/// type names, and prepends <paramref name="prefix"/> as plain (uncoloured) text.
		/// </summary>
		protected RichText? CreateMemberRichText(string prefix, ConversionFlags flags)
		{
			var member = Member;
			if (member == null)
				return null;
			var body = Language.GetRichText(member, flags, boldTypeNames: true);
			return prefix.Length == 0 ? body : new RichText(prefix) + body;
		}

		public override void ActivateItem(IPlatformRoutedEventArgs e)
		{
			e.Handled = true;
			if (Member == null || Member.MetadataToken.IsNil)
				return;
			// One message carries both halves: Reference drives the assembly-tree navigation,
			// Source carries the originally-analysed entity so the receiving subscriber can
			// paint local-reference marks on it in the navigated-to body. The subscription
			// lives in AssemblyTreeModel (mirrors WPF) so the highlight stays decoupled from
			// the tree-node code path.
			MessageBus.Send(this, new NavigateToReferenceEventArgs(Member, SourceMember));
		}

		/// <summary>
		/// The entity this row analyses (or represents as an analyser result). Subclasses
		/// return <see langword="null"/> only for non-entity rows such as
		/// <see cref="ICSharpCode.ILSpy.Analyzers.TreeNodes.AnalyzedModuleTreeNode"/>.
		/// </summary>
		public abstract IEntity? Member { get; }

		/// <summary>
		/// The entity that the user originally clicked "Analyze" on. Used by analyser
		/// result rows to reverse-look-up the analysis they belong to (e.g. for the
		/// "Remove" context menu entry).
		/// </summary>
		public IEntity? SourceMember { get; protected set; }

		public override object? ToolTip => Member?.ParentModule?.MetadataFile?.FileName;

		/// <summary>
		/// Appends one <see cref="AnalyzerSearchTreeNode"/> per registered analyzer that
		/// applies to <paramref name="analyzedSymbol"/>. Subclasses with extra rows
		/// (accessors, backing fields) add those first, then call this.
		/// </summary>
		protected void AddAnalyzerChildren(ISymbol analyzedSymbol)
		{
			foreach (var factory in Analyzers)
			{
				var analyzer = factory.CreateExport().Value;
				if (analyzer.Show(analyzedSymbol))
				{
					this.Children.Add(
						new AnalyzerSearchTreeNode(analyzedSymbol, analyzer, factory.Metadata?.Header));
				}
			}
		}

		public override bool HandleAssemblyListChanged(
			ICollection<LoadedAssembly> removedAssemblies,
			ICollection<LoadedAssembly> addedAssemblies)
		{
			if (Member == null)
				return true;
			foreach (var asm in removedAssemblies)
			{
				if (this.Member.ParentModule?.MetadataFile == asm.GetMetadataFileOrNull())
					return false; // ask parent to drop me — my module is gone
			}
			this.Children.RemoveAll(node =>
				node is not AnalyzerTreeNode an
				|| !an.HandleAssemblyListChanged(removedAssemblies, addedAssemblies));
			return true;
		}
	}
}
