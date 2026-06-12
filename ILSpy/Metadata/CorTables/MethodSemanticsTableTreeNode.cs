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
	/// View of the MethodSemantics table — links accessor methods (get/set/add/remove/raise)
	/// to the property or event they implement. <see cref="MetadataReader.GetMethodSemantics"/>
	/// returns a typed enumeration so byte-level reading isn't required.
	/// </summary>
	public sealed class MethodSemanticsTableTreeNode : MetadataTableTreeNode<MethodSemanticsTableTreeNode.MethodSemanticsEntry>
	{
		public MethodSemanticsTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.MethodSemantics, metadataFile)
		{
		}

		protected override IReadOnlyList<MethodSemanticsEntry> LoadTable()
		{
			var list = new List<MethodSemanticsEntry>();
			foreach (var row in metadataFile.Metadata.GetMethodSemantics())
				list.Add(new MethodSemanticsEntry(metadataFile, row.Handle, row.Semantics, row.Method, row.Association));
			return list;
		}

		public sealed class MethodSemanticsEntry
		{
			public int RID => MetadataTokens.GetToken(handle) & 0xFFFFFF;

			[ColumnInfo("X8")]
			public int Token => MetadataTokens.GetToken(handle);

			[ColumnInfo("X8")]
			public MethodSemanticsAttributes Semantics { get; }

			public string SemanticsTooltip => Semantics.ToString();

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Method => MetadataTokens.GetToken(method);

			string? methodTooltip;
			public string? MethodTooltip => GenerateTooltip(ref methodTooltip, metadataFile, method);

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Association => MetadataTokens.GetToken(association);

			string? associationTooltip;
			public string? AssociationTooltip => GenerateTooltip(ref associationTooltip, metadataFile, association);

			readonly MetadataFile metadataFile;
			readonly Handle handle;
			readonly MethodDefinitionHandle method;
			readonly EntityHandle association;

			public MethodSemanticsEntry(MetadataFile metadataFile, Handle handle, MethodSemanticsAttributes semantics, MethodDefinitionHandle method, EntityHandle association)
			{
				this.metadataFile = metadataFile;
				this.handle = handle;
				Semantics = semantics;
				this.method = method;
				this.association = association;
			}
		}
	}
}
