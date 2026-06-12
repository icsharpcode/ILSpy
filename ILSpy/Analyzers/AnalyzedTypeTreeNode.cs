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

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.Analyzers.TreeNodes
{
	internal sealed class AnalyzedTypeTreeNode : AnalyzerEntityTreeNode
	{
		readonly ITypeDefinition analyzedType;

		public AnalyzedTypeTreeNode(ITypeDefinition analyzedType, IEntity? source)
		{
			this.analyzedType = analyzedType ?? throw new ArgumentNullException(nameof(analyzedType));
			this.SourceMember = source;
			LazyLoading = true;
		}

		public override IEntity Member => analyzedType;

		public override object Text => Language.TypeToString(analyzedType);

		public override object Icon => ResolveIcon(analyzedType);

		static object ResolveIcon(ITypeDefinition type)
		{
			var baseImage = type.Kind switch {
				TypeKind.Interface => Images.Interface,
				TypeKind.Struct or TypeKind.Void => Images.Struct,
				TypeKind.Delegate => Images.Delegate,
				TypeKind.Enum => Images.Enum,
				_ => Images.Class,
			};
			return Images.GetIcon(baseImage,
				Images.GetOverlay(type.Accessibility), type.IsStatic);
		}

		protected override void LoadChildren() => AddAnalyzerChildren(analyzedType);
	}
}
