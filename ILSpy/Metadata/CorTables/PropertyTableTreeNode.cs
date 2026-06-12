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
	/// View of the Property table — every property the module's types declare. Method-level
	/// accessor binding lives in MethodSemantics; this table just carries each property's
	/// attributes, name, and signature blob.
	/// </summary>
	public sealed class PropertyTableTreeNode : MetadataTableTreeNode<PropertyTableTreeNode.PropertyDefEntry>
	{
		public PropertyTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.Property, metadataFile)
		{
		}

		protected override IReadOnlyList<PropertyDefEntry> LoadTable()
		{
			var list = new List<PropertyDefEntry>();
			foreach (var row in metadataFile.Metadata.PropertyDefinitions)
				list.Add(new PropertyDefEntry(metadataFile, row));
			return list;
		}

		public sealed class PropertyDefEntry
		{
			readonly MetadataFile metadataFile;
			readonly PropertyDefinitionHandle handle;
			readonly PropertyDefinition propertyDef;

			public int RID => MetadataTokens.GetRowNumber(handle);

			[ColumnInfo("X8")]
			public int Token => MetadataTokens.GetToken(handle);

			[ColumnInfo("X8")]
			public int Offset => GetRowOffset(metadataFile, TableIndex.Property, RID);

			[ColumnInfo("X8")]
			public PropertyAttributes Attributes => propertyDef.Attributes;

			public object AttributesTooltip => new FlagsTooltip {
				FlagGroup.CreateMultipleChoiceGroup(typeof(PropertyAttributes), selectedValue: (int)propertyDef.Attributes, includeAll: false),
			};

			public string Name => metadataFile.Metadata.GetString(propertyDef.Name);

			public string NameTooltip => $"{MetadataTokens.GetHeapOffset(propertyDef.Name):X} \"{Name}\"";

			[ColumnInfo("X8", Kind = ColumnKind.HeapOffset)]
			public int Signature => MetadataTokens.GetHeapOffset(propertyDef.Signature);

			string? signatureTooltip;
			public string? SignatureTooltip => GenerateTooltip(ref signatureTooltip, metadataFile, handle);

			public PropertyDefEntry(MetadataFile metadataFile, PropertyDefinitionHandle handle)
			{
				this.metadataFile = metadataFile;
				this.handle = handle;
				propertyDef = metadataFile.Metadata.GetPropertyDefinition(handle);
			}
		}
	}
}
