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

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class MethodImplTableTreeNode : MetadataTableTreeNode
	{
		public MethodImplTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.MethodImpl, metadataFile)
		{
		}

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);

			var list = new List<MethodImplEntry>();
			MethodImplEntry scrollTargetEntry = default;

			for (int row = 1; row <= metadataFile.Metadata.GetTableRowCount(TableIndex.MethodImpl); row++)
			{
				MethodImplEntry entry = new MethodImplEntry(metadataFile, MetadataTokens.MethodImplementationHandle(row));
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

		struct MethodImplEntry
		{
			readonly MetadataFile metadataFile;
			readonly MethodImplementationHandle handle;
			readonly MethodImplementation methodImpl;

			public int RID => MetadataTokens.GetToken(handle) & 0xFFFFFF;

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataFile.MetadataOffset
				+ metadataFile.Metadata.GetTableMetadataOffset(TableIndex.MethodDef)
				+ metadataFile.Metadata.GetTableRowSize(TableIndex.MethodDef) * (RID - 1);

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int MethodDeclaration => MetadataTokens.GetToken(methodImpl.MethodDeclaration);

			public void OnMethodDeclarationClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, methodImpl.MethodDeclaration, protocol: "metadata")));
			}

			string methodDeclarationTooltip;
			public string MethodDeclarationTooltip => GenerateTooltip(ref methodDeclarationTooltip, metadataFile, methodImpl.MethodDeclaration);

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int MethodBody => MetadataTokens.GetToken(methodImpl.MethodBody);

			public void OnMethodBodyClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, methodImpl.MethodBody, protocol: "metadata")));
			}

			string methodBodyTooltip;
			public string MethodBodyTooltip => GenerateTooltip(ref methodBodyTooltip, metadataFile, methodImpl.MethodBody);

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Type => MetadataTokens.GetToken(methodImpl.Type);

			public void OnTypeClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, methodImpl.Type, protocol: "metadata")));
			}

			string typeTooltip;
			public string TypeTooltip => GenerateTooltip(ref typeTooltip, metadataFile, methodImpl.Type);

			public MethodImplEntry(MetadataFile metadataFile, MethodImplementationHandle handle)
			{
				this.metadataFile = metadataFile;
				this.handle = handle;
				this.methodImpl = metadataFile.Metadata.GetMethodImplementation(handle);
				this.typeTooltip = null;
				this.methodBodyTooltip = null;
				this.methodDeclarationTooltip = null;
			}
		}
	}
}
