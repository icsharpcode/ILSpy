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
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class MemberRefTableTreeNode : MetadataTableTreeNode
	{
		public MemberRefTableTreeNode(PEFile module)
			: base(HandleKind.MemberReference, module)
		{
		}

		public override object Text => $"0A MemberRef ({module.Metadata.GetTableRowCount(TableIndex.MemberRef)})";

		public override object Icon => Images.Literal;

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			var metadata = module.Metadata;

			var list = new List<MemberRefEntry>();
			MemberRefEntry scrollTargetEntry = default;

			foreach (var row in metadata.MemberReferences)
			{
				MemberRefEntry entry = new MemberRefEntry(module, row);
				if (entry.RID == this.scrollTarget)
				{
					scrollTargetEntry = entry;
				}
				list.Add(entry);
			}

			view.ItemsSource = list;

			tabPage.Content = view;

			if (scrollTargetEntry.RID > 0)
			{
				ScrollItemIntoView(view, scrollTargetEntry);
			}

			return true;
		}

		struct MemberRefEntry
		{
			readonly int metadataOffset;
			readonly PEFile module;
			readonly MetadataReader metadata;
			readonly MemberReferenceHandle handle;
			readonly MemberReference memberRef;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataOffset
				+ metadata.GetTableMetadataOffset(TableIndex.MemberRef)
				+ metadata.GetTableRowSize(TableIndex.MemberRef) * (RID - 1);

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Parent => MetadataTokens.GetToken(memberRef.Parent);

			public void OnParentClick()
			{
				MainWindow.Instance.JumpToReference(new EntityReference(module, memberRef.Parent, protocol: "metadata"));
			}

			string parentTooltip;
			public string ParentTooltip => GenerateTooltip(ref parentTooltip, module, memberRef.Parent);

			public string Name => metadata.GetString(memberRef.Name);

			public string NameTooltip => $"{MetadataTokens.GetHeapOffset(memberRef.Name):X} \"{Name}\"";

			[ColumnInfo("X8", Kind = ColumnKind.HeapOffset)]
			public int Signature => MetadataTokens.GetHeapOffset(memberRef.Signature);

			string signatureTooltip;
			public string SignatureTooltip => GenerateTooltip(ref signatureTooltip, module, handle);

			public MemberRefEntry(PEFile module, MemberReferenceHandle handle)
			{
				this.metadataOffset = module.Reader.PEHeaders.MetadataStartOffset;
				this.module = module;
				this.metadata = module.Metadata;
				this.handle = handle;
				this.memberRef = metadata.GetMemberReference(handle);
				this.signatureTooltip = null;
				this.parentTooltip = null;
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "MemberRefs");
		}
	}
}
