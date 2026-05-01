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

using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;

namespace ILSpy.TreeNodes
{
	/// <summary>
	/// Single entry in the references folder. Minimal port: shows the reference name
	/// using ILAmbience escaping plus a generic assembly icon. Children (the
	/// per-reference type list and transitive references) are not yet implemented.
	/// </summary>
	sealed class AssemblyReferenceTreeNode : ILSpyTreeNode
	{
		readonly AssemblyReference reference;
		readonly AssemblyTreeNode parentAssembly;

		public AssemblyReferenceTreeNode(AssemblyReference reference, AssemblyTreeNode parentAssembly)
		{
			this.reference = reference ?? throw new ArgumentNullException(nameof(reference));
			this.parentAssembly = parentAssembly ?? throw new ArgumentNullException(nameof(parentAssembly));
		}

		public AssemblyReference AssemblyReference => reference;

		public override object Text => ILAmbience.EscapeName(reference.Name);

		public override object Icon => Images.Images.Assembly;
	}
}
