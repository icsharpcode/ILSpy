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

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Output;
using ICSharpCode.Decompiler.TypeSystem;

using ICSharpCode.ILSpy.Languages;

namespace ICSharpCode.ILSpy.TreeNodes
{
	/// <summary>
	/// One <c>TypeRef</c> entry inside <see cref="AssemblyReferenceReferencedTypesTreeNode"/>.
	/// Recurses into nested type references and member references.
	/// </summary>
	public sealed class TypeReferenceTreeNode : ILSpyTreeNode
	{
		readonly MetadataModule module;
		readonly TypeReferenceMetadata r;
		readonly IType resolvedType;

		public TypeReferenceTreeNode(MetadataModule module, TypeReferenceMetadata r)
		{
			this.module = module ?? throw new ArgumentNullException(nameof(module));
			this.r = r ?? throw new ArgumentNullException(nameof(r));
			this.resolvedType = module.ResolveType(r.Handle, default);
			LazyLoading = true;
		}

		public override object Text => Language.TypeToString(resolvedType, ConversionFlags.None);

		public override object NavigationText => $"{Text} ({ICSharpCode.ILSpy.Properties.Resources.ReferencedTypes})";

		public override object Icon => Images.TypeReference;

		protected override void LoadChildren()
		{
			foreach (var typeRef in r.TypeReferences)
				Children.Add(new TypeReferenceTreeNode(module, typeRef));
			foreach (var memberRef in r.MemberReferences)
				Children.Add(new MemberReferenceTreeNode(module, memberRef));
		}

		public override bool ShowExpander => !r.TypeReferences.IsEmpty || !r.MemberReferences.IsEmpty;

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, Language.TypeToString(resolvedType));
			EnsureLazyChildren();
			foreach (ILSpyTreeNode child in Children)
			{
				output.Indent();
				child.Decompile(language, output, options);
				output.Unindent();
			}
		}
	}
}
