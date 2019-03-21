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
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpy.Controls;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.TreeView;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class ExportedTypeTableTreeNode : ILSpyTreeNode
	{
		private PEFile module;

		public ExportedTypeTableTreeNode(PEFile module)
		{
			this.module = module;
		}

		public override object Text => $"27 ExportedType ({module.Metadata.GetTableRowCount(TableIndex.ExportedType)})";

		public override object Icon => Images.Literal;

		public override bool View(DecompilerTextView textView)
		{
			ListView view = Helpers.CreateListView("ExportedTypesView");

			var metadata = module.Metadata;
			
			var list = new List<ExportedTypeEntry>();
			
			foreach (var row in metadata.ExportedTypes) {
				list.Add(new ExportedTypeEntry(module.Reader.PEHeaders.MetadataStartOffset, metadata, row, metadata.GetExportedType(row)));
			}

			view.ItemsSource = list;
			
			textView.ShowContent(new[] { this }, view);
			return true;
		}

		struct ExportedTypeEntry
		{
			readonly int metadataOffset;
			readonly MetadataReader metadata;
			readonly ExportedTypeHandle handle;
			readonly ExportedType type;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataOffset
				+ metadata.GetTableMetadataOffset(TableIndex.ExportedType)
				+ metadata.GetTableRowSize(TableIndex.ExportedType) * (RID-1);

			public int Attributes => (int)type.Attributes;

			public string AttributesTooltip => Helpers.AttributesToString(type.Attributes);

			public int TypeDefId => type.GetTypeDefinitionId();

			public int TypeName => MetadataTokens.GetHeapOffset(type.Name);

			public string TypeNameTooltip => metadata.GetString(type.Name);

			public int TypeNamespace => MetadataTokens.GetHeapOffset(type.Namespace);

			public string TypeNamespaceTooltip => metadata.GetString(type.Namespace);

			public int Implementation => MetadataTokens.GetToken(type.Implementation);

			public ExportedTypeEntry(int metadataOffset, MetadataReader metadata, ExportedTypeHandle handle, ExportedType type)
			{
				this.metadataOffset = metadataOffset;
				this.metadata = metadata;
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