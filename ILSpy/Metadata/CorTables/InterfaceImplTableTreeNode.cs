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
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class InterfaceImplTableTreeNode : MetadataTableTreeNode<InterfaceImplTableTreeNode.InterfaceImplEntry>
	{
		public InterfaceImplTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.InterfaceImpl, metadataFile)
		{
		}

		protected override IReadOnlyList<InterfaceImplEntry> LoadTable()
		{
			var list = new List<InterfaceImplEntry>();
			var length = metadataFile.Metadata.GetTableRowCount(TableIndex.InterfaceImpl);
			ReadOnlySpan<byte> ptr = metadataFile.Metadata.AsReadOnlySpan();
			for (int rid = 1; rid <= length; rid++)
			{
				list.Add(new InterfaceImplEntry(metadataFile, ptr, rid));
			}
			return list;
		}

		readonly struct InterfaceImpl
		{
			public readonly EntityHandle Class;
			public readonly EntityHandle Interface;

			public InterfaceImpl(ReadOnlySpan<byte> ptr, int classSize, int interfaceSize)
			{
				Class = MetadataTokens.TypeDefinitionHandle(Helpers.GetValueLittleEndian(ptr, classSize));
				Interface = Helpers.FromTypeDefOrRefTag((uint)Helpers.GetValueLittleEndian(ptr.Slice(classSize, interfaceSize)));
			}
		}

		internal struct InterfaceImplEntry
		{
			readonly MetadataFile metadataFile;
			readonly InterfaceImpl interfaceImpl;

			public int RID { get; }

			public int Token => 0x09000000 | RID;

			public int Offset { get; }

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Class => MetadataTokens.GetToken(interfaceImpl.Class);

			public void OnClassClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, interfaceImpl.Class, protocol: "metadata")));
			}

			string classTooltip;
			public string ClassTooltip => GenerateTooltip(ref classTooltip, metadataFile, interfaceImpl.Class);

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Interface => MetadataTokens.GetToken(interfaceImpl.Interface);

			public void OnInterfaceClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, interfaceImpl.Interface, protocol: "metadata")));
			}

			string interfaceTooltip;
			public string InterfaceTooltip => GenerateTooltip(ref interfaceTooltip, metadataFile, interfaceImpl.Interface);

			public InterfaceImplEntry(MetadataFile metadataFile, ReadOnlySpan<byte> ptr, int row)
			{
				this.metadataFile = metadataFile;
				this.RID = row;
				var rowOffset = metadataFile.Metadata.GetTableMetadataOffset(TableIndex.InterfaceImpl)
					+ metadataFile.Metadata.GetTableRowSize(TableIndex.InterfaceImpl) * (row - 1);
				this.Offset = metadataFile.MetadataOffset + rowOffset;
				this.interfaceImpl = new InterfaceImpl(ptr.Slice(rowOffset), metadataFile.Metadata.GetTableRowCount(TableIndex.TypeDef) < ushort.MaxValue ? 2 : 4, metadataFile.Metadata.ComputeCodedTokenSize(16384, TableMask.TypeDef | TableMask.TypeRef | TableMask.TypeSpec));
				this.interfaceTooltip = null;
				this.classTooltip = null;
			}
		}
	}
}
