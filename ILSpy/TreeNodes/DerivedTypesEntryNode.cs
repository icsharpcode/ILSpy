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
using System.Threading;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Output;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Abstractions;
using ICSharpCode.ILSpyX.TreeView.PlatformAbstractions;

using ICSharpCode.ILSpy.Languages;

namespace ICSharpCode.ILSpy.TreeNodes
{
	/// <summary>
	/// Single derived-type entry under a <see cref="DerivedTypesTreeNode"/>. Activating jumps
	/// to the corresponding <see cref="TypeTreeNode"/>. Itself lazy-recursive — expanding a
	/// derived-type entry walks one more level, surfacing types derived from <em>it</em>.
	/// </summary>
	public sealed class DerivedTypesEntryNode : ILSpyTreeNode, IMemberTreeNode
	{
		readonly AssemblyList list;
		readonly ITypeDefinition type;

		public DerivedTypesEntryNode(AssemblyList list, ITypeDefinition type)
		{
			this.list = list;
			this.type = type;
			LazyLoading = true;
		}

		public override bool ShowExpander => !type.IsSealed && base.ShowExpander;

		public override object Text => Language.TypeToString(type, ConversionFlags.None);

		public override object? NavigationText => $"{Text} ({ICSharpCode.ILSpy.Properties.Resources.DerivedTypes})";

		public override object Icon => type.Kind == TypeKind.Interface
			? Images.Interface
			: Images.Class;

		protected override void LoadChildren()
		{
			foreach (var entry in DerivedTypesTreeNode.FindDerivedTypes(list, LanguageService.CurrentLanguage, type, CancellationToken.None))
				Children.Add(entry);
		}

		public override bool IsPublicAPI => type.Accessibility switch {
			Accessibility.Public or Accessibility.Internal or Accessibility.ProtectedOrInternal => true,
			_ => false,
		};

		/// <summary>
		/// Mirrors WPF's filter — drop non-public entries under PublicOnly visibility, otherwise
		/// recurse so the user can drill into derived chains. The WPF overload also reads
		/// <c>SearchTermMatches</c> (a <see cref="LanguageSettings"/> helper that's not yet in
		/// the Avalonia port) to surface only entries whose name matches the active search term;
		/// reinstate that branch when the search infrastructure lands.
		/// </summary>
		public override FilterResult Filter(LanguageSettings settings)
		{
			if (settings.ShowApiLevel == ApiVisibility.PublicOnly && !IsPublicAPI)
				return FilterResult.Hidden;
			return FilterResult.Recurse;
		}

		public override void ActivateItem(IPlatformRoutedEventArgs e)
		{
			e.Handled = BaseTypesEntryNode.ActivateItem(this, type);
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, language.TypeToString(type, ConversionFlags.None));
		}

		IEntity IMemberTreeNode.Member => type;
	}
}
