// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.Diagnostics;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.TreeNodes;

#nullable enable

namespace ICSharpCode.ILSpy.Analyzers.TreeNodes
{
	class AnalyzedFieldTreeNode : AnalyzerEntityTreeNode
	{
		readonly IField analyzedField;

		public AnalyzedFieldTreeNode(IField analyzedField, IEntity? source)
		{
			this.analyzedField = analyzedField ?? throw new ArgumentNullException(nameof(analyzedField));
			this.SourceMember = source;
			this.LazyLoading = true;
		}

		public override object Icon => FieldTreeNode.GetIcon(analyzedField);

		public override object Text => Language.FieldToString(analyzedField, true, false, true);

		protected override void LoadChildren()
		{
			foreach (var lazy in Analyzers)
			{
				var analyzer = lazy.Value;
				Debug.Assert(analyzer != null);
				if (analyzer.Show(analyzedField))
				{
					this.Children.Add(new AnalyzerSearchTreeNode(analyzedField, analyzer, lazy.Metadata?.Header));
				}
			}
		}

		public override IEntity Member => analyzedField;
	}
}
