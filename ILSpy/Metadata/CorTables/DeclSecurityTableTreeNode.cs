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
	/// View of the DeclSecurity table — declarative security attributes (CAS-era
	/// <c>[SecurityPermission(...)]</c>) attached to assemblies, types, or methods. The
	/// PermissionSet blob holds a serialised XML / binary permission descriptor.
	/// </summary>
	public sealed class DeclSecurityTableTreeNode : MetadataTableTreeNode<DeclSecurityTableTreeNode.DeclSecurityEntry>
	{
		public DeclSecurityTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.DeclSecurity, metadataFile)
		{
		}

		protected override IReadOnlyList<DeclSecurityEntry> LoadTable()
		{
			var list = new List<DeclSecurityEntry>();
			foreach (var row in metadataFile.Metadata.DeclarativeSecurityAttributes)
				list.Add(new DeclSecurityEntry(metadataFile, row));
			return list;
		}

		public sealed class DeclSecurityEntry
		{
			readonly MetadataFile metadataFile;
			readonly DeclarativeSecurityAttributeHandle handle;
			readonly DeclarativeSecurityAttribute declSecAttr;

			public int RID => MetadataTokens.GetRowNumber(handle);

			[ColumnInfo("X8")]
			public int Token => MetadataTokens.GetToken(handle);

			[ColumnInfo("X8")]
			public int Offset => GetRowOffset(metadataFile, TableIndex.DeclSecurity, RID);

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Parent => MetadataTokens.GetToken(declSecAttr.Parent);

			string? parentTooltip;
			public string? ParentTooltip => GenerateTooltip(ref parentTooltip, metadataFile, declSecAttr.Parent);

			[ColumnInfo("X8")]
			public DeclarativeSecurityAction Action => declSecAttr.Action;

			public string ActionTooltip {
				get {
					return declSecAttr.Action.ToString();
				}
			}

			[ColumnInfo("X8", Kind = ColumnKind.HeapOffset)]
			public int PermissionSet => MetadataTokens.GetHeapOffset(declSecAttr.PermissionSet);

			public string? PermissionSetTooltip {
				get {
					return null;
				}
			}

			public DeclSecurityEntry(MetadataFile metadataFile, DeclarativeSecurityAttributeHandle handle)
			{
				this.metadataFile = metadataFile;
				this.handle = handle;
				declSecAttr = metadataFile.Metadata.GetDeclarativeSecurityAttribute(handle);
			}
		}
	}
}
