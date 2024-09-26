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
	internal class MethodSemanticsTableTreeNode : MetadataTableTreeNode
	{
		public MethodSemanticsTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.MethodSemantics, metadataFile)
		{
		}

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);

			var list = new List<MethodSemanticsEntry>();
			MethodSemanticsEntry scrollTargetEntry = default;

			foreach (var row in metadataFile.Metadata.GetMethodSemantics())
			{
				MethodSemanticsEntry entry = new MethodSemanticsEntry(metadataFile, row.Handle, row.Semantics, row.Method, row.Association);
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

		struct MethodSemanticsEntry
		{
			readonly MetadataFile metadataFile;
			readonly Handle handle;
			readonly MethodSemanticsAttributes semantics;
			readonly MethodDefinitionHandle method;
			readonly EntityHandle association;

			public int RID => MetadataTokens.GetToken(handle) & 0xFFFFFF;

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataFile.MetadataOffset
				+ metadataFile.Metadata.GetTableMetadataOffset(TableIndex.MethodDef)
				+ metadataFile.Metadata.GetTableRowSize(TableIndex.MethodDef) * (RID - 1);

			[ColumnInfo("X8", Kind = ColumnKind.Other)]
			public MethodSemanticsAttributes Semantics => semantics;

			public string SemanticsTooltip => semantics.ToString();

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Method => MetadataTokens.GetToken(method);

			public void OnMethodClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, method, protocol: "metadata")));
			}

			string methodTooltip;
			public string MethodTooltip => GenerateTooltip(ref methodTooltip, metadataFile, method);

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Association => MetadataTokens.GetToken(association);

			public void OnAssociationClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, association, protocol: "metadata")));
			}

			string associationTooltip;
			public string AssociationTooltip => GenerateTooltip(ref associationTooltip, metadataFile, association);

			public MethodSemanticsEntry(MetadataFile metadataFile, Handle handle, MethodSemanticsAttributes semantics, MethodDefinitionHandle method, EntityHandle association)
			{
				this.metadataFile = metadataFile;
				this.handle = handle;
				this.semantics = semantics;
				this.method = method;
				this.association = association;
				this.methodTooltip = null;
				this.associationTooltip = null;
			}
		}
	}
}
