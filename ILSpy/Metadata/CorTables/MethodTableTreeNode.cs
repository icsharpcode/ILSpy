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
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.Metadata;

namespace ILSpy.Metadata.CorTables
{
	/// <summary>
	/// View of the MethodDef table — every method body the module ships. The RVA points
	/// into the .text section's IL stream (0 for abstract / interface methods); the
	/// ParamList token is the first row in the Param table covering this method's parameters.
	/// </summary>
	public sealed class MethodTableTreeNode : MetadataTableTreeNode<MethodTableTreeNode.MethodDefEntry>
	{
		public MethodTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.MethodDef, metadataFile)
		{
		}

		protected override IReadOnlyList<MethodDefEntry> LoadTable()
		{
			var list = new List<MethodDefEntry>();
			foreach (var row in metadataFile.Metadata.MethodDefinitions)
				list.Add(new MethodDefEntry(metadataFile, row));
			return list;
		}

		public sealed class MethodDefEntry
		{
			readonly MetadataFile metadataFile;
			readonly MethodDefinitionHandle handle;
			readonly MethodDefinition methodDef;

			public int RID => MetadataTokens.GetRowNumber(handle);

			[ColumnInfo("X8")]
			public int Token => MetadataTokens.GetToken(handle);

			[ColumnInfo("X8")]
			public int Offset => metadataFile.MetadataOffset
				+ metadataFile.Metadata.GetTableMetadataOffset(TableIndex.MethodDef)
				+ metadataFile.Metadata.GetTableRowSize(TableIndex.MethodDef) * (RID - 1);

			[ColumnInfo("X8")]
			public MethodAttributes Attributes => methodDef.Attributes;

			[ColumnInfo("X8")]
			public MethodImplAttributes ImplAttributes => methodDef.ImplAttributes;

			[ColumnInfo("X8")]
			public int RVA => methodDef.RelativeVirtualAddress;

			public string Name => metadataFile.Metadata.GetString(methodDef.Name);

			[ColumnInfo("X8", Kind = ColumnKind.HeapOffset)]
			public int Signature => MetadataTokens.GetHeapOffset(methodDef.Signature);

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int ParamList => MetadataTokens.GetToken(methodDef.GetParameters().FirstOrDefault());

			public MethodDefEntry(MetadataFile metadataFile, MethodDefinitionHandle handle)
			{
				this.metadataFile = metadataFile;
				this.handle = handle;
				methodDef = metadataFile.Metadata.GetMethodDefinition(handle);
			}
		}
	}
}
