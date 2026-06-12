// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata.CorTables
{
	/// <summary>
	/// View of the Module table — always exactly one row carrying the module name and the
	/// MVID (the GUID identifying this module's identity). Reads the GUID heap for MVID and
	/// the optional generation IDs (only present in EnC / hot-reload deltas).
	/// </summary>
	public sealed class ModuleTableTreeNode : MetadataTableTreeNode<ModuleTableTreeNode.ModuleEntry>
	{
		public ModuleTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.Module, metadataFile)
		{
		}

		protected override IReadOnlyList<ModuleEntry> LoadTable()
			=> [new ModuleEntry(metadataFile, (ModuleDefinitionHandle)EntityHandle.ModuleDefinition)];

		public sealed class ModuleEntry
		{
			readonly MetadataFile metadataFile;
			readonly ModuleDefinitionHandle handle;
			readonly ModuleDefinition moduleDef;

			public int RID => MetadataTokens.GetRowNumber(handle);

			[ColumnInfo("X8")]
			public int Token => MetadataTokens.GetToken(handle);

			[ColumnInfo("X8")]
			public int Offset => GetRowOffset(metadataFile, TableIndex.Module, RID);

			public int Generation => moduleDef.Generation;

			public string Name => metadataFile.Metadata.GetString(moduleDef.Name);

			public string NameTooltip => $"{MetadataTokens.GetHeapOffset(moduleDef.Name):X} \"{Name}\"";

			[ColumnInfo("X8")]
			public int Mvid => MetadataTokens.GetHeapOffset(moduleDef.Mvid);

			public string MvidTooltip => metadataFile.Metadata.GetGuid(moduleDef.Mvid).ToString();

			[ColumnInfo("X8")]
			public int GenerationId => MetadataTokens.GetHeapOffset(moduleDef.GenerationId);

			public string? GenerationIdTooltip => moduleDef.GenerationId.IsNil ? null : metadataFile.Metadata.GetGuid(moduleDef.GenerationId).ToString();

			[ColumnInfo("X8")]
			public int BaseGenerationId => MetadataTokens.GetHeapOffset(moduleDef.BaseGenerationId);

			public string? BaseGenerationIdTooltip => moduleDef.BaseGenerationId.IsNil ? null : metadataFile.Metadata.GetGuid(moduleDef.BaseGenerationId).ToString();

			public ModuleEntry(MetadataFile metadataFile, ModuleDefinitionHandle handle)
			{
				this.metadataFile = metadataFile;
				this.handle = handle;
				moduleDef = metadataFile.Metadata.GetModuleDefinition();
			}
		}
	}
}
