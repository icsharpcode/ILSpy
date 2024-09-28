﻿// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.Linq;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpyX.Analyzers;

namespace ICSharpCode.ILSpy.Analyzers.TreeNodes
{
	internal class AnalyzedTypeTreeNode : AnalyzerEntityTreeNode
	{
		readonly ITypeDefinition analyzedType;

		public AnalyzedTypeTreeNode(ITypeDefinition analyzedType)
		{
			this.analyzedType = analyzedType ?? throw new ArgumentNullException(nameof(analyzedType));
			this.LazyLoading = true;
		}

		public override object Icon => TypeTreeNode.GetIcon(analyzedType);

		public override object Text => Language.TypeToString(analyzedType, includeNamespace: true);

		protected override void LoadChildren()
		{
			var analyzers = App.ExportProvider.GetExports<IAnalyzer, IAnalyzerMetadata>("Analyzer");
			foreach (var lazy in analyzers.OrderBy(item => item.Metadata.Order))
			{
				var analyzer = lazy.Value;
				if (analyzer.Show(analyzedType))
				{
					this.Children.Add(new AnalyzerSearchTreeNode(analyzedType, analyzer, lazy.Metadata.Header));
				}
			}
		}

		public override IEntity? Member => analyzedType;
	}
}
