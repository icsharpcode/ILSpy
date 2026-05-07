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

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.Metadata;

namespace ILSpy.Metadata.CorTables
{
	/// <summary>
	/// View of the Assembly table — at most one row, present only when the metadata file is
	/// itself an assembly manifest (vs. a plain .netmodule). Carries the assembly's name,
	/// culture, version, hash algorithm, and signing flags.
	/// </summary>
	public sealed class AssemblyTableTreeNode : MetadataTableTreeNode<AssemblyTableTreeNode.AssemblyEntry>
	{
		public AssemblyTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.Assembly, metadataFile)
		{
		}

		protected override IReadOnlyList<AssemblyEntry> LoadTable()
			=> metadataFile.Metadata.IsAssembly
				? [new AssemblyEntry(metadataFile.Metadata, metadataFile.MetadataOffset)]
				: [];

		public sealed class AssemblyEntry
		{
			readonly int metadataOffset;
			readonly MetadataReader metadata;
			readonly AssemblyDefinition assembly;

			public int RID => MetadataTokens.GetRowNumber(EntityHandle.AssemblyDefinition);

			[ColumnInfo("X8")]
			public int Token => MetadataTokens.GetToken(EntityHandle.AssemblyDefinition);

			[ColumnInfo("X8")]
			public int Offset => metadataOffset
				+ metadata.GetTableMetadataOffset(TableIndex.Assembly)
				+ metadata.GetTableRowSize(TableIndex.Assembly) * (RID - 1);

			[ColumnInfo("X4")]
			public AssemblyHashAlgorithm HashAlgorithm => assembly.HashAlgorithm;

			[ColumnInfo("X4")]
			public AssemblyFlags Flags => assembly.Flags;

			public Version Version => assembly.Version;

			public string Name => metadata.GetString(assembly.Name);

			public string Culture => metadata.GetString(assembly.Culture);

			public AssemblyEntry(MetadataReader metadata, int metadataOffset)
			{
				this.metadata = metadata;
				this.metadataOffset = metadataOffset;
				assembly = metadata.GetAssemblyDefinition();
			}
		}
	}
}
