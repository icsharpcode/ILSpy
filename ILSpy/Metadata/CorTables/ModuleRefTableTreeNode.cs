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
	/// View of the ModuleRef table — every external .netmodule the assembly P/Invokes into
	/// or otherwise references by name. Carries just the module's name string handle.
	/// </summary>
	public sealed class ModuleRefTableTreeNode : MetadataTableTreeNode<ModuleRefTableTreeNode.ModuleRefEntry>
	{
		public ModuleRefTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.ModuleRef, metadataFile)
		{
		}

		protected override IReadOnlyList<ModuleRefEntry> LoadTable()
		{
			var list = new List<ModuleRefEntry>();
			foreach (var row in metadataFile.Metadata.GetModuleReferences())
				list.Add(new ModuleRefEntry(metadataFile, row));
			return list;
		}

		public sealed class ModuleRefEntry
		{
			readonly MetadataFile metadataFile;
			readonly ModuleReferenceHandle handle;
			readonly ModuleReference moduleRef;

			public int RID => MetadataTokens.GetRowNumber(handle);

			[ColumnInfo("X8")]
			public int Token => MetadataTokens.GetToken(handle);

			[ColumnInfo("X8")]
			public int Offset => GetRowOffset(metadataFile, TableIndex.ModuleRef, RID);

			public string Name => metadataFile.Metadata.GetString(moduleRef.Name);

			public string NameTooltip => $"{MetadataTokens.GetHeapOffset(moduleRef.Name):X} \"{Name}\"";

			public ModuleRefEntry(MetadataFile metadataFile, ModuleReferenceHandle handle)
			{
				this.metadataFile = metadataFile;
				this.handle = handle;
				moduleRef = metadataFile.Metadata.GetModuleReference(handle);
			}
		}
	}
}
