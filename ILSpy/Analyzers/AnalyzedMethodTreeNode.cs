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

using ICSharpCode.Decompiler.Output;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.Analyzers.TreeNodes
{
	internal class AnalyzedMethodTreeNode : AnalyzerEntityTreeNode
	{
		readonly IMethod analyzedMethod;
		readonly string prefix;

		public AnalyzedMethodTreeNode(IMethod analyzedMethod, IEntity? source, string prefix = "")
		{
			this.analyzedMethod = analyzedMethod ?? throw new ArgumentNullException(nameof(analyzedMethod));
			this.SourceMember = source;
			this.prefix = prefix;
			LazyLoading = true;
		}

		public override IEntity Member => analyzedMethod;

		public override object Text
			=> prefix + Language.EntityToString(analyzedMethod,
				ConversionFlags.ShowDeclaringType | ConversionFlags.UseFullyQualifiedEntityNames);

		public override object Icon => ResolveIcon(analyzedMethod);

		internal static object ResolveIcon(IMethod method)
		{
			var baseImage = method.IsConstructor
				? Images.Constructor
				: method.IsOperator
					? Images.Operator
					: Images.Method;
			return Images.GetIcon(baseImage,
				Images.GetOverlay(method.Accessibility),
				method.IsStatic,
				method.IsExtensionMethod);
		}

		protected override void LoadChildren() => AddAnalyzerChildren(analyzedMethod);
	}
}
