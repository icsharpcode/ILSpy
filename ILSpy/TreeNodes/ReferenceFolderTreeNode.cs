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

using System.Linq;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpy.Properties;

namespace ILSpy.TreeNodes
{
	/// <summary>
	/// References folder under an assembly node. Children are one
	/// <see cref="AssemblyReferenceTreeNode"/> per metadata <c>AssemblyRef</c>.
	/// </summary>
	sealed class ReferenceFolderTreeNode : ILSpyTreeNode
	{
		readonly MetadataFile module;
		readonly AssemblyTreeNode parentAssembly;

		public ReferenceFolderTreeNode(MetadataFile module, AssemblyTreeNode parentAssembly)
		{
			this.module = module;
			this.parentAssembly = parentAssembly;
			LazyLoading = true;
		}

		public override object Text => Resources.References;

		public override object Icon => Images.Images.ReferenceFolder;

		protected override void LoadChildren()
		{
			foreach (var r in module.AssemblyReferences.OrderBy(r => r.Name))
				Children.Add(new AssemblyReferenceTreeNode(r, parentAssembly));
		}
	}
}
