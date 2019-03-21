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
	internal class ModuleTableTreeNode : ILSpyTreeNode
	{
		private PEFile module;

		public ModuleTableTreeNode(PEFile module)
		{
			this.module = module;
		}

		public override object Text => $"00 Module ({module.Metadata.GetTableRowCount(TableIndex.Module)})";

		public override object Icon => Images.Literal;

		public override bool View(DecompilerTextView textView)
		{
			ListView view = Helpers.CreateListView("ModulesView");
			var metadata = module.Metadata;
			
			var list = new List<ModuleEntry>();
			
			list.Add(new ModuleEntry(module.Reader.PEHeaders.MetadataStartOffset, metadata, EntityHandle.ModuleDefinition, metadata.GetModuleDefinition()));

			view.ItemsSource = list;
			
			textView.ShowContent(new[] { this }, view);
			return true;
		}

		struct ModuleEntry
		{
			readonly int metadataOffset;
			readonly MetadataReader metadata;
			readonly ModuleDefinitionHandle handle;
			readonly ModuleDefinition module;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataOffset
				+ metadata.GetTableMetadataOffset(TableIndex.Module)
				+ metadata.GetTableRowSize(TableIndex.Module) * (RID-1);

			public int Generation => module.Generation;

			public int NameStringHandle => MetadataTokens.GetHeapOffset(module.Name);

			public string Name => metadata.GetString(module.Name);

			public int Mvid => MetadataTokens.GetHeapOffset(module.Mvid);

			public string MvidTooltip => metadata.GetGuid(module.Mvid).ToString();

			public int GenerationId => MetadataTokens.GetHeapOffset(module.GenerationId);

			public string GenerationIdTooltip => module.GenerationId.IsNil ? null : metadata.GetGuid(module.GenerationId).ToString();

			public int BaseGenerationId => MetadataTokens.GetHeapOffset(module.BaseGenerationId);

			public string BaseGenerationIdTooltip => module.BaseGenerationId.IsNil ? null : metadata.GetGuid(module.BaseGenerationId).ToString();

			public ModuleEntry(int metadataOffset, MetadataReader metadata, ModuleDefinitionHandle handle, ModuleDefinition module)
			{
				this.metadataOffset = metadataOffset;
				this.metadata = metadata;
				this.handle = handle;
				this.module = module;
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "Modules");
		}
	}
}