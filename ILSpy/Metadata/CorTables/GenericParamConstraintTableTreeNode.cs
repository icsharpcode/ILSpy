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
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class GenericParamConstraintTableTreeNode : MetadataTableTreeNode
	{
		public GenericParamConstraintTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.GenericParamConstraint, metadataFile)
		{
		}

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			var metadata = metadataFile.Metadata;

			var list = new List<GenericParamConstraintEntry>();
			GenericParamConstraintEntry scrollTargetEntry = default;

			for (int row = 1; row <= metadata.GetTableRowCount(TableIndex.GenericParamConstraint); row++)
			{
				GenericParamConstraintEntry entry = new GenericParamConstraintEntry(metadataFile, MetadataTokens.GenericParameterConstraintHandle(row));
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

		struct GenericParamConstraintEntry
		{
			readonly MetadataFile metadataFile;
			readonly GenericParameterConstraintHandle handle;
			readonly GenericParameterConstraint genericParamConstraint;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataFile.MetadataOffset
				+ metadataFile.Metadata.GetTableMetadataOffset(TableIndex.GenericParamConstraint)
				+ metadataFile.Metadata.GetTableRowSize(TableIndex.GenericParamConstraint) * (RID - 1);

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Owner => MetadataTokens.GetToken(genericParamConstraint.Parameter);

			public void OnOwnerClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, genericParamConstraint.Parameter, protocol: "metadata")));
			}

			string ownerTooltip;

			public string OwnerTooltip {
				get {
					if (ownerTooltip == null)
					{
						ITextOutput output = new PlainTextOutput();
						var p = metadataFile.Metadata.GetGenericParameter(genericParamConstraint.Parameter);
						output.Write("parameter " + p.Index + (p.Name.IsNil ? "" : " (" + metadataFile.Metadata.GetString(p.Name) + ")") + " of ");
						p.Parent.WriteTo(metadataFile, output, default);
						ownerTooltip = output.ToString();
					}
					return ownerTooltip;
				}
			}

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Type => MetadataTokens.GetToken(genericParamConstraint.Type);

			public void OnTypeClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, genericParamConstraint.Type, protocol: "metadata")));
			}

			string typeTooltip;
			public string TypeTooltip => GenerateTooltip(ref typeTooltip, metadataFile, genericParamConstraint.Type);

			public GenericParamConstraintEntry(MetadataFile metadataFile, GenericParameterConstraintHandle handle)
			{
				this.metadataFile = metadataFile;
				this.handle = handle;
				this.genericParamConstraint = metadataFile.Metadata.GetGenericParameterConstraint(handle);
				this.ownerTooltip = null;
				this.typeTooltip = null;
			}
		}
	}
}