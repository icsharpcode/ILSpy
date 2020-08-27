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
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Windows.Controls;
using System.Windows.Threading;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class MethodTableTreeNode : MetadataTableTreeNode
	{
		public MethodTableTreeNode(PEFile module)
			: base(HandleKind.MethodDefinition, module)
		{
		}

		public override object Text => $"06 Method ({module.Metadata.GetTableRowCount(TableIndex.MethodDef)})";

		public override object Icon => Images.Literal;

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			var metadata = module.Metadata;
			var list = new List<MethodDefEntry>();
			MethodDefEntry scrollTargetEntry = default;

			foreach (var row in metadata.MethodDefinitions)
			{
				MethodDefEntry entry = new MethodDefEntry(module, row);
				if (entry.RID == scrollTarget)
				{
					scrollTargetEntry = entry;
				}
				list.Add(entry);
			}

			view.ItemsSource = list;

			tabPage.Content = view;

			if (scrollTargetEntry.RID > 1)
			{
				ScrollItemIntoView(view, scrollTargetEntry);
			}

			return true;
		}

		struct MethodDefEntry : IMemberTreeNode
		{
			readonly int metadataOffset;
			readonly PEFile module;
			readonly MetadataReader metadata;
			readonly MethodDefinitionHandle handle;
			readonly MethodDefinition methodDef;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataOffset
				+ metadata.GetTableMetadataOffset(TableIndex.MethodDef)
				+ metadata.GetTableRowSize(TableIndex.MethodDef) * (RID - 1);

			[StringFormat("X8")]
			public MethodAttributes Attributes => methodDef.Attributes;

			const MethodAttributes otherFlagsMask = ~(MethodAttributes.MemberAccessMask | MethodAttributes.VtableLayoutMask);

			public object AttributesTooltip => new FlagsTooltip {
				FlagGroup.CreateSingleChoiceGroup(typeof(MethodAttributes), "Member access: ", (int)MethodAttributes.MemberAccessMask, (int)(methodDef.Attributes & MethodAttributes.MemberAccessMask), new Flag("CompilerControlled (0000)", 0, false), includeAny: false),
				FlagGroup.CreateSingleChoiceGroup(typeof(MethodAttributes), "Vtable layout: ", (int)MethodAttributes.VtableLayoutMask, (int)(methodDef.Attributes & MethodAttributes.VtableLayoutMask), new Flag("ReuseSlot (0000)", 0, false), includeAny: false),
				FlagGroup.CreateMultipleChoiceGroup(typeof(MethodAttributes), "Flags:", (int)otherFlagsMask, (int)(methodDef.Attributes & otherFlagsMask), includeAll: false),
			};

			[StringFormat("X8")]
			public MethodImplAttributes ImplAttributes => methodDef.ImplAttributes;

			public object ImplAttributesTooltip => new FlagsTooltip {
				FlagGroup.CreateSingleChoiceGroup(typeof(MethodImplAttributes), "Code type: ", (int)MethodImplAttributes.CodeTypeMask, (int)(methodDef.ImplAttributes & MethodImplAttributes.CodeTypeMask), new Flag("IL (0000)", 0, false), includeAny: false),
				FlagGroup.CreateSingleChoiceGroup(typeof(MethodImplAttributes), "Managed type: ", (int)MethodImplAttributes.ManagedMask, (int)(methodDef.ImplAttributes & MethodImplAttributes.ManagedMask), new Flag("Managed (0000)", 0, false), includeAny: false),
			};

			public int RVA => methodDef.RelativeVirtualAddress;

			public string Name => metadata.GetString(methodDef.Name);

			public string NameTooltip => $"{MetadataTokens.GetHeapOffset(methodDef.Name):X} \"{Name}\"";

			[StringFormat("X")]
			public int Signature => MetadataTokens.GetHeapOffset(methodDef.Signature);

			string signatureTooltip;

			public string SignatureTooltip {
				get {
					if (signatureTooltip == null)
					{
						ITextOutput output = new PlainTextOutput();
						var context = new Decompiler.Metadata.GenericContext(default(TypeDefinitionHandle), module);
						((EntityHandle)handle).WriteTo(module, output, context);
						signatureTooltip = output.ToString();
					}
					return signatureTooltip;
				}
			}

			IEntity IMemberTreeNode.Member => ((MetadataModule)module.GetTypeSystemWithCurrentOptionsOrNull()?.MainModule).GetDefinition(handle);

			public MethodDefEntry(PEFile module, MethodDefinitionHandle handle)
			{
				this.metadataOffset = module.Reader.PEHeaders.MetadataStartOffset;
				this.module = module;
				this.metadata = module.Metadata;
				this.handle = handle;
				this.methodDef = metadata.GetMethodDefinition(handle);
				this.signatureTooltip = null;
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "MethodDefs");
		}
	}
}
