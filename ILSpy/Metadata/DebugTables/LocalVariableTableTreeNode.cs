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

namespace ICSharpCode.ILSpy.Metadata.DebugTables
{
	/// <summary>
	/// View of the LocalVariable table — debug-info for named local slots inside method
	/// bodies. Each row carries the variable's name, slot index, and PDB-only attributes
	/// (e.g. "is compiler-generated").
	/// </summary>
	public sealed class LocalVariableTableTreeNode : MetadataTableTreeNode<LocalVariableTableTreeNode.LocalVariableEntry>
	{
		public LocalVariableTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.LocalVariable, metadataFile)
		{
		}

		protected override IReadOnlyList<LocalVariableEntry> LoadTable()
		{
			var list = new List<LocalVariableEntry>();
			foreach (var row in metadataFile.Metadata.LocalVariables)
				list.Add(new LocalVariableEntry(metadataFile, row));
			return list;
		}

		public sealed class LocalVariableEntry
		{
			readonly MetadataFile metadataFile;
			readonly LocalVariableHandle handle;
			readonly LocalVariable localVar;

			public int RID => MetadataTokens.GetRowNumber(handle);

			[ColumnInfo("X8")]
			public int Token => MetadataTokens.GetToken(handle);

			[ColumnInfo("X8")]
			public LocalVariableAttributes Attributes => localVar.Attributes;

			public object AttributesTooltip => new FlagsTooltip() {
				FlagGroup.CreateMultipleChoiceGroup(typeof(LocalVariableAttributes)),
			};

			public int Index => localVar.Index;

			public string Name => metadataFile.Metadata.GetString(localVar.Name);

			public string NameTooltip => $"{MetadataTokens.GetHeapOffset(localVar.Name):X} \"{Name}\"";

			public LocalVariableEntry(MetadataFile metadataFile, LocalVariableHandle handle)
			{
				this.metadataFile = metadataFile;
				this.handle = handle;
				localVar = metadataFile.Metadata.GetLocalVariable(handle);
			}
		}
	}
}
