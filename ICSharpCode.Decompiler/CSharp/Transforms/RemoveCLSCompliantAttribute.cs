// Copyright (c) 2017 Siegfried Pammer
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.Semantics;

namespace ICSharpCode.Decompiler.CSharp.Transforms
{
	/// <summary>
	/// This transform is used to remove CLSCompliant attributes added by the compiler. We remove them in order to get rid of many warnings.
	/// </summary>
	/// <remarks>This transform is only enabled, when exporting a full assembly as project.</remarks>
	public class RemoveCLSCompliantAttribute : IAstTransform
	{
		public void Run(AstNode rootNode, TransformContext context)
		{
			foreach (var section in rootNode.Children.OfType<AttributeSection>())
			{
				if (section.AttributeTarget == "assembly")
					continue;
				foreach (var attribute in section.Attributes)
				{
					var trr = attribute.Type.Annotation<TypeResolveResult>();
					if (trr != null && trr.Type.FullName == "System.CLSCompliantAttribute")
						attribute.Remove();
				}
				if (section.Attributes.Count == 0)
					section.Remove();
			}
		}
	}
}
