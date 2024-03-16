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

namespace ICSharpCode.ILSpy.TreeNodes
{
	/// <summary>
	/// Reference to member from assembly reference list.
	/// </summary>
	public sealed class MemberReferenceTreeNode : ILSpyTreeNode
	{
		readonly PEFile module;
		private readonly MemberReferenceMetadata r;

		public MemberReferenceTreeNode(PEFile module, MemberReferenceMetadata r)
		{
			this.module = module ?? throw new ArgumentNullException(nameof(module));
			this.r = r;
		}

		public override object Text => r.Name + GetSuffixString(r.Handle);

		public override object Icon => r.MemberReferenceKind switch {
			MemberReferenceKind.Method => Images.Method,
			MemberReferenceKind.Field => Images.Field,
			_ => Images.Class,
		};

		public string Signature => Language.GetEntityName(module, r.Handle, fullName: true, omitGenerics: false);

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, Signature);
		}

		public override object ToolTip => Signature;
	}
}
