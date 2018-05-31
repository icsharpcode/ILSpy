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
using System.Reflection;
using System.Reflection.Metadata;
using ICSharpCode.Decompiler;

namespace ICSharpCode.ILSpy.TreeNodes.Analyzer
{
	internal class AnalyzedMethodTreeNode : AnalyzerEntityTreeNode
	{
		readonly Decompiler.Metadata.PEFile module;
		readonly MethodDefinitionHandle analyzedMethod;
		readonly string prefix;

		public AnalyzedMethodTreeNode(Decompiler.Metadata.PEFile module, MethodDefinitionHandle analyzedMethod, string prefix = "")
		{
			if (analyzedMethod.IsNil)
				throw new ArgumentNullException(nameof(analyzedMethod));
			this.module = module;
			this.analyzedMethod = analyzedMethod;
			this.prefix = prefix;
			this.LazyLoading = true;
		}

		public override object Icon => MethodTreeNode.GetIcon(new Decompiler.Metadata.MethodDefinition(module, analyzedMethod));

		public override object Text => prefix + Language.MethodToString(new Decompiler.Metadata.MethodDefinition(module, analyzedMethod), true, true);

		protected override void LoadChildren()
		{
			if (analyzedMethod.HasBody(module.Metadata))
				this.Children.Add(new AnalyzedMethodUsesTreeNode(module, analyzedMethod));

			/*if (analyzedMethod.HasFlag(MethodAttributes.Virtual) && !(analyzedMethod.HasFlag(MethodAttributes.NewSlot) && analyzedMethod.HasFlag(MethodAttributes.Final)))
				this.Children.Add(new AnalyzedVirtualMethodUsedByTreeNode(analyzedMethod));
			else
				this.Children.Add(new AnalyzedMethodUsedByTreeNode(analyzedMethod));

			if (AnalyzedMethodOverridesTreeNode.CanShow(analyzedMethod))
				this.Children.Add(new AnalyzedMethodOverridesTreeNode(analyzedMethod));

			if (AnalyzedInterfaceMethodImplementedByTreeNode.CanShow(analyzedMethod))
				this.Children.Add(new AnalyzedInterfaceMethodImplementedByTreeNode(analyzedMethod));*/
		}

		public override Decompiler.Metadata.IMetadataEntity Member => new Decompiler.Metadata.MethodDefinition(module, analyzedMethod);
	}
}
