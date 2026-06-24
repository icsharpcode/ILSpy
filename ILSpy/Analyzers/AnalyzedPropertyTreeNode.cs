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

using AvaloniaEdit.Highlighting;

using ICSharpCode.Decompiler.Output;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.Analyzers.TreeNodes
{
	internal sealed class AnalyzedPropertyTreeNode : AnalyzerEntityTreeNode
	{
		readonly IProperty analyzedProperty;
		readonly string prefix;

		public AnalyzedPropertyTreeNode(IProperty analyzedProperty, IEntity? source, string prefix = "")
		{
			this.analyzedProperty = analyzedProperty ?? throw new ArgumentNullException(nameof(analyzedProperty));
			this.SourceMember = source;
			this.prefix = prefix;
			LazyLoading = true;
		}

		public override IEntity Member => analyzedProperty;

		public override object Text => CreateRichText()?.Text
			?? prefix + Language.EntityToString(analyzedProperty,
				ConversionFlags.ShowDeclaringType | ConversionFlags.UseFullyQualifiedEntityNames);

		protected override RichText? BuildRichText() => CreateMemberRichText(prefix, MemberSignatureFlags);

		public override object Icon => Images.GetIcon(Images.Property,
			Images.GetOverlay(analyzedProperty.Accessibility), analyzedProperty.IsStatic);

		protected override void LoadChildren()
		{
			if (analyzedProperty.CanGet)
				this.Children.Add(new AnalyzedAccessorTreeNode(analyzedProperty.Getter!, this.SourceMember, "get"));
			if (analyzedProperty.CanSet)
				this.Children.Add(new AnalyzedAccessorTreeNode(analyzedProperty.Setter!, this.SourceMember, "set"));

			AddAnalyzerChildren(analyzedProperty);
		}
	}
}
