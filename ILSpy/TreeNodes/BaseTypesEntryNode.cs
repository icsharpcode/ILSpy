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

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Output;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX.TreeView;
using ICSharpCode.ILSpyX.TreeView.PlatformAbstractions;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Languages;

namespace ICSharpCode.ILSpy.TreeNodes
{
	/// <summary>
	/// Single base-class / interface entry under a <see cref="BaseTypesTreeNode"/>. Activating
	/// the entry navigates the assembly-tree selection to the corresponding <see cref="TypeTreeNode"/>.
	/// </summary>
	public sealed class BaseTypesEntryNode : ILSpyTreeNode, IMemberTreeNode
	{
		readonly ITypeDefinition type;

		public BaseTypesEntryNode(ITypeDefinition type)
		{
			this.type = type;
		}

		public override object Text => Language.TypeToString(type, ConversionFlags.None);

		public override object? NavigationText => $"{Text} ({ICSharpCode.ILSpy.Properties.Resources.BaseTypes})";

		public override object Icon => type.Kind == TypeKind.Interface
			? Images.Interface
			: Images.Class;

		public override void ActivateItem(IPlatformRoutedEventArgs e)
		{
			e.Handled = ActivateItem(this, type);
		}

		/// <summary>
		/// Shared activate helper used by <see cref="BaseTypesEntryNode"/> and
		/// <see cref="DerivedTypesEntryNode"/>. Mirrors WPF's matching helper. The
		/// <paramref name="node"/> parameter is unused on Avalonia (navigation routes through
		/// the MEF-resolved <see cref="AssemblyTreeModel"/> instead of walking up the tree to
		/// the assembly-list node), but the signature stays in lock-step with WPF so any future
		/// caller that needs the originating node has it on hand.
		/// </summary>
		internal static bool ActivateItem(SharpTreeNode node, ITypeDefinition? def)
		{
			if (def == null)
				return false;
			var atm = AppComposition.Current.GetExport<AssemblyTreeModel>();
			return atm.JumpToType(def);
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, language.TypeToString(type, ConversionFlags.None));
		}

		IEntity IMemberTreeNode.Member => type;
	}
}
