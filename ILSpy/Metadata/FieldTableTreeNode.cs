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
	internal class FieldTableTreeNode : ILSpyTreeNode
	{
		private PEFile module;

		public FieldTableTreeNode(PEFile module)
		{
			this.module = module;
		}

		public override object Text => $"04 Field ({module.Metadata.GetTableRowCount(TableIndex.Field)})";

		public override object Icon => Images.Literal;

		public override bool View(DecompilerTextView textView)
		{
			ListView view = Helpers.CreateListView("FieldDefsView");
			var metadata = module.Metadata;

			var list = new List<FieldDefEntry>();

			foreach (var row in metadata.FieldDefinitions)
				list.Add(new FieldDefEntry(module, row));

			view.ItemsSource = list;

			textView.ShowContent(new[] { this }, view);
			return true;
		}

		struct FieldDefEntry : IMemberTreeNode
		{
			readonly int metadataOffset;
			readonly PEFile module;
			readonly MetadataReader metadata;
			readonly FieldDefinitionHandle handle;
			readonly FieldDefinition fieldDef;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataOffset
				+ metadata.GetTableMetadataOffset(TableIndex.Field)
				+ metadata.GetTableRowSize(TableIndex.Field) * (RID - 1);

			public int Attributes => (int)fieldDef.Attributes;

			public string AttributesTooltip => null; //Helpers.AttributesToString(fieldDef.Attributes);

			public int NameStringHandle => MetadataTokens.GetHeapOffset(fieldDef.Name);

			public string Name => metadata.GetString(fieldDef.Name);

			IEntity IMemberTreeNode.Member => ((MetadataModule)module.GetTypeSystemOrNull()?.MainModule).GetDefinition(handle);

			public int Signature => MetadataTokens.GetHeapOffset(fieldDef.Signature);

			public string SignatureTooltip {
				get {
					ITextOutput output = new PlainTextOutput();
					var context = new Decompiler.Metadata.GenericContext(default(TypeDefinitionHandle), module);
					((EntityHandle)handle).WriteTo(module, output, context);
					return output.ToString();
				}
			}

			public FieldDefEntry(PEFile module, FieldDefinitionHandle handle)
			{
				this.metadataOffset = module.Reader.PEHeaders.MetadataStartOffset;
				this.module = module;
				this.metadata = module.Metadata;
				this.handle = handle;
				this.fieldDef = metadata.GetFieldDefinition(handle);
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "FieldDefs");
		}
	}
}
