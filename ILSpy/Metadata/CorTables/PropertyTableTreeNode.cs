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
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class PropertyTableTreeNode : ILSpyTreeNode
	{
		private PEFile module;

		public PropertyTableTreeNode(PEFile module)
		{
			this.module = module;
		}

		public override object Text => $"17 Property ({module.Metadata.GetTableRowCount(TableIndex.Property)})";

		public override object Icon => Images.Literal;

		public override bool View(DecompilerTextView textView)
		{
			ListView view = Helpers.CreateListView("PropertyDefsView");
			var metadata = module.Metadata;

			var list = new List<PropertyDefEntry>();

			foreach (var row in metadata.PropertyDefinitions)
				list.Add(new PropertyDefEntry(module, row));

			view.ItemsSource = list;

			textView.ShowContent(new[] { this }, view);
			return true;
		}

		struct PropertyDefEntry : IMemberTreeNode
		{
			readonly int metadataOffset;
			readonly PEFile module;
			readonly MetadataReader metadata;
			readonly PropertyDefinitionHandle handle;
			readonly PropertyDefinition propertyDef;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataOffset
				+ metadata.GetTableMetadataOffset(TableIndex.Property)
				+ metadata.GetTableRowSize(TableIndex.Property) * (RID - 1);

			public int Attributes => (int)propertyDef.Attributes;

			public string AttributesTooltip => null; //Helpers.AttributesToString(PropertyDef.Attributes);

			public int NameStringHandle => MetadataTokens.GetHeapOffset(propertyDef.Name);

			public string Name => metadata.GetString(propertyDef.Name);

			IEntity IMemberTreeNode.Member => ((MetadataModule)module.GetTypeSystemOrNull()?.MainModule).GetDefinition(handle);

			public int Signature => MetadataTokens.GetHeapOffset(propertyDef.Signature);

			public string SignatureTooltip {
				get {
					ITextOutput output = new PlainTextOutput();
					((EntityHandle)handle).WriteTo(module, output, Decompiler.Metadata.GenericContext.Empty);
					return output.ToString();
				}
			}

			public PropertyDefEntry(PEFile module, PropertyDefinitionHandle handle)
			{
				this.metadataOffset = module.Reader.PEHeaders.MetadataStartOffset;
				this.module = module;
				this.metadata = module.Metadata;
				this.handle = handle;
				this.propertyDef = metadata.GetPropertyDefinition(handle);
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "PropertyDefs");
		}
	}
}
