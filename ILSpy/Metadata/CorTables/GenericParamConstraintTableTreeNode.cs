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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpy.Controls;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.TreeView;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class GenericParamConstraintTableTreeNode : ILSpyTreeNode
	{
		private PEFile module;

		public GenericParamConstraintTableTreeNode(PEFile module)
		{
			this.module = module;
		}

		public override object Text => $"2C GenericParamConstraint ({module.Metadata.GetTableRowCount(TableIndex.GenericParamConstraint)})";

		public override object Icon => Images.Literal;

		public override bool View(DecompilerTextView textView)
		{
			ListView view = Helpers.CreateListView("GenericParamConstraintsView");
			var metadata = module.Metadata;
			
			var list = new List<GenericParamConstraintEntry>();

			for (int row = 1; row <= module.Metadata.GetTableRowCount(TableIndex.GenericParamConstraint); row++) {
				list.Add(new GenericParamConstraintEntry(module, MetadataTokens.GenericParameterConstraintHandle(row)));
			}
			view.ItemsSource = list;
			
			textView.ShowContent(new[] { this }, view);
			return true;
		}

		struct GenericParamConstraintEntry
		{
			readonly int metadataOffset;
			readonly PEFile module;
			readonly MetadataReader metadata;
			readonly GenericParameterConstraintHandle handle;
			readonly GenericParameterConstraint genericParamConstraint;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataOffset
				+ metadata.GetTableMetadataOffset(TableIndex.GenericParamConstraint)
				+ metadata.GetTableRowSize(TableIndex.GenericParamConstraint) * (RID-1);

			public int OwnerHandle => MetadataTokens.GetToken(genericParamConstraint.Parameter);

			public string Owner {
				get {
					ITextOutput output = new PlainTextOutput();
					((EntityHandle)genericParamConstraint.Parameter).WriteTo(module, output, GenericContext.Empty);
					return output.ToString();
				}
			}

			public int Type => MetadataTokens.GetToken(genericParamConstraint.Type);

			public string TypeTooltip {
				get {
					ITextOutput output = new PlainTextOutput();
					genericParamConstraint.Type.WriteTo(module, output, GenericContext.Empty);
					return output.ToString();
				}
			}

			public GenericParamConstraintEntry(PEFile module, GenericParameterConstraintHandle handle)
			{
				this.metadataOffset = module.Reader.PEHeaders.MetadataStartOffset;
				this.module = module;
				this.metadata = module.Metadata;
				this.handle = handle;
				this.genericParamConstraint = metadata.GetGenericParameterConstraint(handle);
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "GenericParamConstraints");
		}
	}
}