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

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class ExportedTypeTableTreeNode : MetadataTableTreeNode<ExportedTypeTableTreeNode.ExportedTypeEntry>
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
			{
				list.Add(new ExportedTypeEntry(metadataFile, row, metadata.GetExportedType(row)));
			}
			return list;
		}

		internal struct ExportedTypeEntry
		{
			readonly MetadataFile metadataFile;
			readonly ExportedTypeHandle handle;
			readonly ExportedType type;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataFile.MetadataOffset
				+ metadataFile.Metadata.GetTableMetadataOffset(TableIndex.ExportedType)
				+ metadataFile.Metadata.GetTableRowSize(TableIndex.ExportedType) * (RID - 1);

			[ColumnInfo("X8", Kind = ColumnKind.Other)]
			public TypeAttributes Attributes => type.Attributes;

			const TypeAttributes otherFlagsMask = ~(TypeAttributes.VisibilityMask | TypeAttributes.LayoutMask | TypeAttributes.ClassSemanticsMask | TypeAttributes.StringFormatMask | TypeAttributes.CustomFormatMask);

			public object AttributesTooltip => new FlagsTooltip {
				FlagGroup.CreateSingleChoiceGroup(typeof(TypeAttributes), "Visibility: ", (int)TypeAttributes.VisibilityMask, (int)(type.Attributes & TypeAttributes.VisibilityMask), new Flag("NotPublic (0000)", 0, false), includeAny: false),
				FlagGroup.CreateSingleChoiceGroup(typeof(TypeAttributes), "Class layout: ", (int)TypeAttributes.LayoutMask, (int)(type.Attributes & TypeAttributes.LayoutMask), new Flag("AutoLayout (0000)", 0, false), includeAny: false),
				FlagGroup.CreateSingleChoiceGroup(typeof(TypeAttributes), "Class semantics: ", (int)TypeAttributes.ClassSemanticsMask, (int)(type.Attributes & TypeAttributes.ClassSemanticsMask), new Flag("Class (0000)", 0, false), includeAny: false),
				FlagGroup.CreateSingleChoiceGroup(typeof(TypeAttributes), "String format: ", (int)TypeAttributes.StringFormatMask, (int)(type.Attributes & TypeAttributes.StringFormatMask), new Flag("AnsiClass (0000)", 0, false), includeAny: false),
				FlagGroup.CreateSingleChoiceGroup(typeof(TypeAttributes), "Custom format: ", (int)TypeAttributes.CustomFormatMask, (int)(type.Attributes & TypeAttributes.CustomFormatMask), new Flag("Value0 (0000)", 0, false), includeAny: false),
				FlagGroup.CreateMultipleChoiceGroup(typeof(TypeAttributes), "Flags:", (int)otherFlagsMask, (int)(type.Attributes & otherFlagsMask), includeAll: false),
			};

			public int TypeDefId => type.GetTypeDefinitionId();

			public string TypeNameTooltip => $"{MetadataTokens.GetHeapOffset(type.Name):X} \"{TypeName}\"";

			public string TypeName => metadataFile.Metadata.GetString(type.Name);

			public string TypeNamespaceTooltip => $"{MetadataTokens.GetHeapOffset(type.Namespace):X} \"{TypeNamespace}\"";

			public string TypeNamespace => metadataFile.Metadata.GetString(type.Namespace);

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Implementation => MetadataTokens.GetToken(type.Implementation);

			public void OnImplementationClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, type.Implementation, protocol: "metadata")));
			}

			string implementationTooltip;
			public string ImplementationTooltip => GenerateTooltip(ref implementationTooltip, metadataFile, type.Implementation);

			public ExportedTypeEntry(MetadataFile metadataFile, ExportedTypeHandle handle, ExportedType type)
			{
				this.metadataFile = metadataFile;
				this.handle = handle;
				this.type = type;
				this.implementationTooltip = null;
			}
		}
	}
}