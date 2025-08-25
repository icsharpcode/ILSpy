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
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class MethodTableTreeNode : MetadataTableTreeNode<MethodTableTreeNode.MethodDefEntry>
	{
		public MethodTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.MethodDef, metadataFile)
		{
		}

		protected override IReadOnlyList<MethodDefEntry> LoadTable()
		{
			var list = new List<MethodDefEntry>();
			foreach (var row in metadataFile.Metadata.MethodDefinitions)
			{
				list.Add(new MethodDefEntry(metadataFile, row));
			}
			return list;
		}

		internal struct MethodDefEntry : IMemberTreeNode
		{
			readonly MetadataFile metadataFile;
			readonly MethodDefinitionHandle handle;
			readonly MethodDefinition methodDef;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataFile.MetadataOffset
				+ metadataFile.Metadata.GetTableMetadataOffset(TableIndex.MethodDef)
				+ metadataFile.Metadata.GetTableRowSize(TableIndex.MethodDef) * (RID - 1);

			[ColumnInfo("X8", Kind = ColumnKind.Other)]
			public MethodAttributes Attributes => methodDef.Attributes;

			const MethodAttributes otherFlagsMask = ~(MethodAttributes.MemberAccessMask | MethodAttributes.VtableLayoutMask);

			public object AttributesTooltip => new FlagsTooltip {
				FlagGroup.CreateSingleChoiceGroup(typeof(MethodAttributes), "Member access: ", (int)MethodAttributes.MemberAccessMask, (int)(methodDef.Attributes & MethodAttributes.MemberAccessMask), new Flag("CompilerControlled (0000)", 0, false), includeAny: false),
				FlagGroup.CreateSingleChoiceGroup(typeof(MethodAttributes), "Vtable layout: ", (int)MethodAttributes.VtableLayoutMask, (int)(methodDef.Attributes & MethodAttributes.VtableLayoutMask), new Flag("ReuseSlot (0000)", 0, false), includeAny: false),
				FlagGroup.CreateMultipleChoiceGroup(typeof(MethodAttributes), "Flags:", (int)otherFlagsMask, (int)(methodDef.Attributes & otherFlagsMask), includeAll: false),
			};

			[ColumnInfo("X8", Kind = ColumnKind.Other)]
			public MethodImplAttributes ImplAttributes => methodDef.ImplAttributes;

			public object ImplAttributesTooltip => new FlagsTooltip {
				FlagGroup.CreateSingleChoiceGroup(typeof(MethodImplAttributes), "Code type: ", (int)MethodImplAttributes.CodeTypeMask, (int)(methodDef.ImplAttributes & MethodImplAttributes.CodeTypeMask), new Flag("IL (0000)", 0, false), includeAny: false),
				FlagGroup.CreateSingleChoiceGroup(typeof(MethodImplAttributes), "Managed type: ", (int)MethodImplAttributes.ManagedMask, (int)(methodDef.ImplAttributes & MethodImplAttributes.ManagedMask), new Flag("Managed (0000)", 0, false), includeAny: false),
			};

			public int RVA => methodDef.RelativeVirtualAddress;

			public string Name => metadataFile.Metadata.GetString(methodDef.Name);

			public string NameTooltip => $"{MetadataTokens.GetHeapOffset(methodDef.Name):X} \"{Name}\"";

			[ColumnInfo("X8", Kind = ColumnKind.HeapOffset)]
			public int Signature => MetadataTokens.GetHeapOffset(methodDef.Signature);

			string signatureTooltip;

			public string SignatureTooltip => GenerateTooltip(ref signatureTooltip, metadataFile, handle);

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int ParamList => MetadataTokens.GetToken(methodDef.GetParameters().FirstOrDefault());

			public void OnParamListClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, methodDef.GetParameters().FirstOrDefault(), protocol: "metadata")));
			}

			string paramListTooltip;
			public string ParamListTooltip {
				get {
					var param = methodDef.GetParameters().FirstOrDefault();
					if (param.IsNil)
						return null;
					return GenerateTooltip(ref paramListTooltip, metadataFile, param);
				}
			}

			IEntity IMemberTreeNode.Member => ((MetadataModule)metadataFile.GetTypeSystemWithCurrentOptionsOrNull(SettingsService, AssemblyTreeModel.CurrentLanguageVersion)?.MainModule)?.GetDefinition(handle);

			public MethodDefEntry(MetadataFile metadataFile, MethodDefinitionHandle handle)
			{
				this.metadataFile = metadataFile;
				this.handle = handle;
				this.methodDef = metadataFile.Metadata.GetMethodDefinition(handle);
				this.signatureTooltip = null;
				this.paramListTooltip = null;
			}
		}
	}
}
