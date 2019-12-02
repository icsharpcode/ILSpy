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
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class FieldLayoutTableTreeNode : ILSpyTreeNode
	{
		private PEFile module;

		public FieldLayoutTableTreeNode(PEFile module)
		{
			this.module = module;
		}

		public override object Text => $"10 FieldLayout ({module.Metadata.GetTableRowCount(TableIndex.FieldLayout)})";

		public override object Icon => Images.Literal;

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.SupportsLanguageSwitching = false;
			ListView view = Helpers.CreateListView("FieldLayoutsView");
			var metadata = module.Metadata;

			var list = new List<FieldLayoutEntry>();

			foreach (var row in metadata.GetFieldLayouts()) {
				FieldDefinition d = default;
				d.GetOffset();
				list.Add(new FieldLayoutEntry(module, row));
			}

			view.ItemsSource = list;

			tabPage.Content = view;
			return true;
		}

		struct FieldLayoutEntry
		{
			readonly int metadataOffset;
			readonly PEFile module;
			readonly MetadataReader metadata;
			readonly EntityHandle handle;
			//readonly  fieldDef;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataOffset
				+ metadata.GetTableMetadataOffset(TableIndex.FieldLayout)
				+ metadata.GetTableRowSize(TableIndex.FieldLayout) * (RID - 1);

			//public int Attributes => (int)fieldDef.Attributes;

			//public string AttributesTooltip => null; //Helpers.AttributesToString(fieldDef.Attributes);

			//public int NameStringHandle => MetadataTokens.GetHeapOffset(fieldDef.Name);

			//public string Name => metadata.GetString(fieldDef.Name);

			//public int Signature => MetadataTokens.GetHeapOffset(fieldDef.Signature);

			public string SignatureTooltip {
				get {
					ITextOutput output = new PlainTextOutput();
					var context = new Decompiler.Metadata.GenericContext(default(TypeDefinitionHandle), module);
					((EntityHandle)handle).WriteTo(module, output, context);
					return output.ToString();
				}
			}

			public FieldLayoutEntry(PEFile module, EntityHandle handle)
			{
				this.metadataOffset = module.Reader.PEHeaders.MetadataStartOffset;
				this.module = module;
				this.metadata = module.Metadata;
				this.handle = handle;
				//this.fieldDef = metadata.GetFieldLayoutinition(handle);
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "FieldLayouts");
		}
	}
}
