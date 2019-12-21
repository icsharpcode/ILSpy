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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Metadata
{
	class PropertyMapTableTreeNode : ILSpyTreeNode
	{
		private PEFile module;

		public PropertyMapTableTreeNode(PEFile module)
		{
			this.module = module;
		}

		public override object Text => $"15 PropertyMap ({module.Metadata.GetTableRowCount(TableIndex.PropertyMap)})";

		public override object Icon => Images.Literal;

		public unsafe override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage);
			var metadata = module.Metadata;

			var list = new List<PropertyMapEntry>();

			var length = metadata.GetTableRowCount(TableIndex.PropertyMap);
			byte* ptr = metadata.MetadataPointer;
			int metadataOffset = module.Reader.PEHeaders.MetadataStartOffset;
			for (int rid = 1; rid <= length; rid++) {
				list.Add(new PropertyMapEntry(module, ptr, metadataOffset, rid));
			}

			view.ItemsSource = list;

			tabPage.Content = view;
			return true;
		}

		readonly struct PropertyMap
		{
			public readonly TypeDefinitionHandle Parent;
			public readonly PropertyDefinitionHandle PropertyList;

			public unsafe PropertyMap(byte *ptr, int typeDefSize, int propertyDefSize)
			{
				Parent = MetadataTokens.TypeDefinitionHandle(Helpers.GetValue(ptr, typeDefSize));
				PropertyList = MetadataTokens.PropertyDefinitionHandle(Helpers.GetValue(ptr + typeDefSize, propertyDefSize));
			}
		}

		unsafe struct PropertyMapEntry
		{
			readonly PEFile module;
			readonly MetadataReader metadata;
			readonly PropertyMap propertyMap;

			public int RID { get; }

			public int Token => 0x15000000 | RID;

			public int Offset { get; }

			[StringFormat("X8")]
			public int Parent => MetadataTokens.GetToken(propertyMap.Parent);

			public string ParentTooltip {
				get {
					ITextOutput output = new PlainTextOutput();
					var context = new GenericContext(default(TypeDefinitionHandle), module);
					((EntityHandle)propertyMap.Parent).WriteTo(module, output, context);
					return output.ToString();
				}
			}

			[StringFormat("X8")]
			public int EventList => MetadataTokens.GetToken(propertyMap.PropertyList);

			public string PropertyListTooltip {
				get {
					ITextOutput output = new PlainTextOutput();
					var context = new GenericContext(default(TypeDefinitionHandle), module);
					((EntityHandle)propertyMap.PropertyList).WriteTo(module, output, context);
					return output.ToString();
				}
			}

			public PropertyMapEntry(PEFile module, byte* ptr, int metadataOffset, int row)
			{
				this.module = module;
				this.metadata = module.Metadata;
				this.RID = row;
				var rowOffset = metadata.GetTableMetadataOffset(TableIndex.PropertyMap)
					+ metadata.GetTableRowSize(TableIndex.PropertyMap) * (row - 1);
				this.Offset = metadataOffset + rowOffset;
				int typeDefSize = metadata.GetTableRowCount(TableIndex.TypeDef) < ushort.MaxValue ? 2 : 4;
				int propertyDefSize = metadata.GetTableRowCount(TableIndex.Property) < ushort.MaxValue ? 2 : 4;
				this.propertyMap = new PropertyMap(ptr + rowOffset, typeDefSize, propertyDefSize);
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "PropertyMap");
		}
	}
}
