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

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class ExportedTypeTableTreeNode : MetadataTableTreeNode
	{
		public ExportedTypeTableTreeNode(PEFile module)
			: base(HandleKind.ExportedType, module)
		{
		}

		public override object Text => $"27 ExportedType ({module.Metadata.GetTableRowCount(TableIndex.ExportedType)})";

		public override object Icon => Images.Literal;

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			var metadata = module.Metadata;
			var list = new List<ExportedTypeEntry>();
			ExportedTypeEntry scrollTargetEntry = default;

			foreach (var row in metadata.ExportedTypes) {
				ExportedTypeEntry entry = new ExportedTypeEntry(module.Reader.PEHeaders.MetadataStartOffset, module, row, metadata.GetExportedType(row));
				if (entry.RID == this.scrollTarget) {
					scrollTargetEntry = entry;
				}
				list.Add(entry);
			}

			view.ItemsSource = list;

			tabPage.Content = view;

			if (scrollTargetEntry.RID > 0) {
				ScrollItemIntoView(view, scrollTargetEntry);
			}

			return true;
		}

		struct ExportedTypeEntry
		{
			readonly int metadataOffset;
			readonly PEFile module;
			readonly MetadataReader metadata;
			readonly ExportedTypeHandle handle;
			readonly ExportedType type;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataOffset
				+ metadata.GetTableMetadataOffset(TableIndex.ExportedType)
				+ metadata.GetTableRowSize(TableIndex.ExportedType) * (RID-1);

			public TypeAttributes Attributes => type.Attributes;

			public object AttributesTooltip => new FlagsTooltip((int)type.Attributes, typeof(TypeAttributes));

			public int TypeDefId => type.GetTypeDefinitionId();

			public string TypeNameTooltip => $"{MetadataTokens.GetHeapOffset(type.Name):X} \"{TypeName}\"";

			public string TypeName => metadata.GetString(type.Name);

			public string TypeNamespaceTooltip => $"{MetadataTokens.GetHeapOffset(type.Name):X} \"{TypeNamespace}\"";

			public string TypeNamespace => metadata.GetString(type.Name);

			[StringFormat("X8")]
			public int Implementation => MetadataTokens.GetToken(type.Implementation);

			public string ImplementationTooltip {
				get {
					if (type.Implementation.IsNil)
						return null;
					ITextOutput output = new PlainTextOutput();
					var context = new GenericContext(default(TypeDefinitionHandle), module);
					type.Implementation.WriteTo(module, output, context);
					return output.ToString();
				}
			}

			public ExportedTypeEntry(int metadataOffset, PEFile module, ExportedTypeHandle handle, ExportedType type)
			{
				this.metadataOffset = metadataOffset;
				this.module = module;
				this.metadata = module.Metadata;
				this.handle = handle;
				this.type = type;
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "ExportedType");
		}
	}
}