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
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class LocalScopeTableTreeNode : DebugMetadataTableTreeNode
	{
		private readonly bool isEmbedded;

		public LocalScopeTableTreeNode(PEFile module, MetadataReader metadata, bool isEmbedded)
			: base(HandleKind.LocalScope, module, metadata)
		{
			this.isEmbedded = isEmbedded;
		}

		public override object Text => $"32 LocalScope ({metadata.GetTableRowCount(TableIndex.LocalScope)})";

		public override object Icon => Images.Literal;

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			var list = new List<LocalScopeEntry>();
			LocalScopeEntry scrollTargetEntry = default;

			foreach (var row in metadata.LocalScopes)
			{
				LocalScopeEntry entry = new LocalScopeEntry(module, metadata, isEmbedded, row);
				if (entry.RID == scrollTarget)
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

		struct LocalScopeEntry
		{
			readonly int? offset;
			readonly PEFile module;
			readonly MetadataReader metadata;
			readonly LocalScopeHandle handle;
			readonly LocalScope localScope;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public object Offset => offset == null ? "n/a" : (object)offset;

			[StringFormat("X8")]
			[LinkToTable]
			public int Method => MetadataTokens.GetToken(localScope.Method);

			public void OnMethodClick()
			{
				MainWindow.Instance.JumpToReference(new EntityReference(module, localScope.Method, protocol: "metadata"));
			}

			public string MethodTooltip {
				get {
					ITextOutput output = new PlainTextOutput();
					((EntityHandle)localScope.Method).WriteTo(module, output, default);
					return output.ToString();
				}
			}

			[StringFormat("X8")]
			[LinkToTable]
			public int ImportScope => MetadataTokens.GetToken(localScope.ImportScope);

			public void OnImportScopeClick()
			{
				MainWindow.Instance.JumpToReference(new EntityReference(module, localScope.ImportScope, protocol: "metadata"));
			}

			[StringFormat("X8")]
			[LinkToTable]
			public int VariableList => MetadataTokens.GetToken(localScope.GetLocalVariables().FirstOrDefault());

			public void OnVariableListClick()
			{
				MainWindow.Instance.JumpToReference(new EntityReference(module, localScope.GetLocalVariables().FirstOrDefault(), protocol: "metadata"));
			}

			[StringFormat("X8")]
			[LinkToTable]
			public int ConstantList => MetadataTokens.GetToken(localScope.GetLocalConstants().FirstOrDefault());

			public void OnConstantListClick()
			{
				MainWindow.Instance.JumpToReference(new EntityReference(module, localScope.GetLocalConstants().FirstOrDefault(), protocol: "metadata"));
			}

			public int StartOffset => localScope.StartOffset;

			public int Length => localScope.Length;

			public LocalScopeEntry(PEFile module, MetadataReader metadata, bool isEmbedded, LocalScopeHandle handle)
			{
				this.offset = isEmbedded ? null : (int?)metadata.GetTableMetadataOffset(TableIndex.LocalScope)
					+ metadata.GetTableRowSize(TableIndex.LocalScope) * (MetadataTokens.GetRowNumber(handle) - 1);
				this.module = module;
				this.metadata = metadata;
				this.handle = handle;
				this.localScope = metadata.GetLocalScope(handle);
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "LocalScope");
		}
	}
}