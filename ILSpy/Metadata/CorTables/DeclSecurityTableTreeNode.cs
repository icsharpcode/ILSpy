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

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata
{
	class DeclSecurityTableTreeNode : MetadataTableTreeNode
	{
		public DeclSecurityTableTreeNode(PEFile module)
			: base(HandleKind.DeclarativeSecurityAttribute, module)
		{
		}

		public override object Text => $"0E DeclSecurity ({module.Metadata.GetTableRowCount(TableIndex.DeclSecurity)})";

		public override object Icon => Images.Literal;

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			var metadata = module.Metadata;

			var list = new List<DeclSecurityEntry>();
			DeclSecurityEntry scrollTargetEntry = default;

			foreach (var row in metadata.DeclarativeSecurityAttributes)
			{
				var entry = new DeclSecurityEntry(module, row);
				if (scrollTarget == MetadataTokens.GetRowNumber(row))
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

		struct DeclSecurityEntry
		{
			readonly int metadataOffset;
			readonly PEFile module;
			readonly MetadataReader metadata;
			readonly DeclarativeSecurityAttributeHandle handle;
			readonly DeclarativeSecurityAttribute declSecAttr;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataOffset
				+ metadata.GetTableMetadataOffset(TableIndex.DeclSecurity)
				+ metadata.GetTableRowSize(TableIndex.DeclSecurity) * (RID - 1);

			[StringFormat("X8")]
			[LinkToTable]
			public int Parent => MetadataTokens.GetToken(declSecAttr.Parent);

			public void OnParentClick()
			{
				MainWindow.Instance.JumpToReference(new EntityReference(module, declSecAttr.Parent, protocol: "metadata"));
			}

			public string ParentTooltip {
				get {
					ITextOutput output = new PlainTextOutput();
					var context = new MetadataGenericContext(default(TypeDefinitionHandle), module);
					declSecAttr.Parent.WriteTo(module, output, context);
					return output.ToString();
				}
			}

			[StringFormat("X8")]
			public DeclarativeSecurityAction Action => declSecAttr.Action;

			public string ActionTooltip {
				get {
					return declSecAttr.Action.ToString();
				}
			}

			[StringFormat("X")]
			public int PermissionSet => MetadataTokens.GetHeapOffset(declSecAttr.PermissionSet);

			public string PermissionSetTooltip {
				get {
					return null;
				}
			}

			public DeclSecurityEntry(PEFile module, DeclarativeSecurityAttributeHandle handle)
			{
				this.metadataOffset = module.Reader.PEHeaders.MetadataStartOffset;
				this.module = module;
				this.metadata = module.Metadata;
				this.handle = handle;
				this.declSecAttr = metadata.GetDeclarativeSecurityAttribute(handle);
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "DeclSecurityAttrs");
		}
	}
}
