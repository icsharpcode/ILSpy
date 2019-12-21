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

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage);
			var metadata = module.Metadata;
			
			var list = new List<ModuleEntry>();
			
			list.Add(new ModuleEntry(module, EntityHandle.ModuleDefinition));

			view.ItemsSource = list;
			
			tabPage.Content = view;
			return true;
		}

		struct ModuleEntry
		{
			readonly int metadataOffset;
			readonly PEFile module;
			readonly MetadataReader metadata;
			readonly ModuleDefinitionHandle handle;
			readonly ModuleDefinition moduleDef;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataOffset
				+ metadata.GetTableMetadataOffset(TableIndex.Module)
				+ metadata.GetTableRowSize(TableIndex.Module) * (RID-1);

			public int Generation => moduleDef.Generation;

			public string Name => metadata.GetString(moduleDef.Name);

			public string NameTooltip => $"{MetadataTokens.GetHeapOffset(moduleDef.Name):X} \"{Name}\"";

			public int Mvid => MetadataTokens.GetHeapOffset(moduleDef.Mvid);

			public string MvidTooltip => metadata.GetGuid(moduleDef.Mvid).ToString();

			public int GenerationId => MetadataTokens.GetHeapOffset(moduleDef.GenerationId);

			public string GenerationIdTooltip => moduleDef.GenerationId.IsNil ? null : metadata.GetGuid(moduleDef.GenerationId).ToString();

			public int BaseGenerationId => MetadataTokens.GetHeapOffset(moduleDef.BaseGenerationId);

			public string BaseGenerationIdTooltip => moduleDef.BaseGenerationId.IsNil ? null : metadata.GetGuid(moduleDef.BaseGenerationId).ToString();

			public ModuleEntry(PEFile module, ModuleDefinitionHandle handle)
			{
				this.metadataOffset = module.Reader.PEHeaders.MetadataStartOffset;
				this.module = module;
				this.metadata = module.Metadata;
				this.handle = handle;
				this.moduleDef = metadata.GetModuleDefinition();
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "Modules");
		}
	}
}