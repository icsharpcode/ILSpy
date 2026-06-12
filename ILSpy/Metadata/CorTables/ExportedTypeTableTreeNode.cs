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
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata.CorTables
{
	/// <summary>
	/// View of the ExportedType table — types this assembly forwards to (or re-exports
	/// from) another assembly. Common in type-forwarding scenarios where mscorlib.dll
	/// exposes types that physically live in System.Runtime.dll.
	/// </summary>
	public sealed class ExportedTypeTableTreeNode : MetadataTableTreeNode<ExportedTypeTableTreeNode.ExportedTypeEntry>
	{
		public ExportedTypeTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.ExportedType, metadataFile)
		{
		}

		protected override IReadOnlyList<ExportedTypeEntry> LoadTable()
		{
			var list = new List<ExportedTypeEntry>();
			var metadata = metadataFile.Metadata;
			foreach (var row in metadata.ExportedTypes)
				list.Add(new ExportedTypeEntry(metadataFile, row, metadata.GetExportedType(row)));
			return list;
		}

		public sealed class ExportedTypeEntry
		{
			readonly MetadataFile metadataFile;
			readonly ExportedTypeHandle handle;
			readonly ExportedType type;

			public int RID => MetadataTokens.GetRowNumber(handle);

			[ColumnInfo("X8")]
			public int Token => MetadataTokens.GetToken(handle);

			[ColumnInfo("X8")]
			public int Offset => GetRowOffset(metadataFile, TableIndex.ExportedType, RID);

			[ColumnInfo("X8")]
			public TypeAttributes Attributes => type.Attributes;

			public object AttributesTooltip => FlagsTooltip.ForTypeAttributes(type.Attributes);

			[ColumnInfo("X8")]
			public int TypeDefId => type.GetTypeDefinitionId();

			public string TypeNameTooltip => $"{MetadataTokens.GetHeapOffset(type.Name):X} \"{TypeName}\"";

			public string TypeName => metadataFile.Metadata.GetString(type.Name);

			public string TypeNamespaceTooltip => $"{MetadataTokens.GetHeapOffset(type.Namespace):X} \"{TypeNamespace}\"";

			public string TypeNamespace => metadataFile.Metadata.GetString(type.Namespace);

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Implementation => MetadataTokens.GetToken(type.Implementation);

			string? implementationTooltip;
			public string? ImplementationTooltip => GenerateTooltip(ref implementationTooltip, metadataFile, type.Implementation);

			public ExportedTypeEntry(MetadataFile metadataFile, ExportedTypeHandle handle, ExportedType type)
			{
				this.metadataFile = metadataFile;
				this.handle = handle;
				this.type = type;
			}
		}
	}
}
