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

using System;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.TreeNodes
{
	/// <summary>
	/// Referenced Types node in assembly reference list
	/// </summary>
	public sealed class AssemblyReferenceReferencedTypesTreeNode : ILSpyTreeNode
	{
		readonly MetadataModule module;
		readonly AssemblyReference r;

		public AssemblyReferenceReferencedTypesTreeNode(MetadataModule module, AssemblyReference r)
		{
			this.module = module ?? throw new ArgumentNullException(nameof(module));
			this.r = r ?? throw new ArgumentNullException(nameof(r));

			this.LazyLoading = true;
		}

		public override object Text => $"{Properties.Resources.ReferencedTypes} ({r.TypeReferences.Length + r.ExportedTypes.Length})";
		public override object Icon => Images.MetadataTable;

		protected override void LoadChildren()
		{
			foreach (var typeRef in r.TypeReferences)
				this.Children.Add(new TypeReferenceTreeNode(module, typeRef));

			foreach (var exportedType in r.ExportedTypes)
				this.Children.Add(new ExportedTypeTreeNode(module, exportedType));
		}

		public override bool ShowExpander => !r.TypeReferences.IsEmpty || !r.ExportedTypes.IsEmpty;

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			EnsureLazyChildren();
			foreach (ILSpyTreeNode child in Children)
				child.Decompile(language, output, options);
		}
	}
}
