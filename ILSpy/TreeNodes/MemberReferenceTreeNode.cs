// Copyright (c) 2023 James May
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

#nullable enable

using System;
using System.Reflection.Metadata;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.TreeNodes
{
	/// <summary>
	/// Reference to member from assembly reference list.
	/// </summary>
	public sealed class MemberReferenceTreeNode : ILSpyTreeNode
	{
		readonly MetadataModule module;
		readonly MemberReferenceMetadata r;
		readonly IMember resolvedMember;

		public MemberReferenceTreeNode(MetadataModule module, MemberReferenceMetadata r)
		{
			this.module = module ?? throw new ArgumentNullException(nameof(module));
			this.r = r;
			this.resolvedMember = r.MemberReferenceKind switch {
				MemberReferenceKind.Method => module.ResolveMethod(r.Handle, default),
				MemberReferenceKind.Field => (IMember)module.ResolveEntity(r.Handle),
				_ => throw new NotSupportedException(),
			};
		}

		public override object Text => Signature + GetSuffixString(r.Handle);

		public override object Icon => r.MemberReferenceKind switch {
			MemberReferenceKind.Method => Images.MethodReference,
			MemberReferenceKind.Field => Images.FieldReference,
			_ => throw new NotSupportedException(),
		};

		public string Signature => resolvedMember is IMethod m ? Language.MethodToString(m, false, false, false) : Language.FieldToString((IField)resolvedMember, false, false, false);

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, Signature);
		}

		public override object ToolTip => Language.GetRichTextTooltip(resolvedMember);
	}
}
