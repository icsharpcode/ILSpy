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
	internal class GenericParamConstraintTableTreeNode : MetadataTableTreeNode
	{
		public GenericParamConstraintTableTreeNode(PEFile module)
			: base(HandleKind.GenericParameterConstraint, module)
		{
		}

		public override object Text => $"2C GenericParamConstraint ({module.Metadata.GetTableRowCount(TableIndex.GenericParamConstraint)})";

		public override object Icon => Images.Literal;

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			var metadata = module.Metadata;

			var list = new List<GenericParamConstraintEntry>();
			GenericParamConstraintEntry scrollTargetEntry = default;

			for (int row = 1; row <= metadata.GetTableRowCount(TableIndex.GenericParamConstraint); row++)
			{
				GenericParamConstraintEntry entry = new GenericParamConstraintEntry(module, MetadataTokens.GenericParameterConstraintHandle(row));
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
			readonly int metadataOffset;
			readonly PEFile module;
			readonly MetadataReader metadata;
			readonly GenericParameterConstraintHandle handle;
			readonly GenericParameterConstraint genericParamConstraint;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataOffset
				+ metadata.GetTableMetadataOffset(TableIndex.GenericParamConstraint)
				+ metadata.GetTableRowSize(TableIndex.GenericParamConstraint) * (RID - 1);

			[StringFormat("X8")]
			[LinkToTable]
			public int Owner => MetadataTokens.GetToken(genericParamConstraint.Parameter);

			public void OnOwnerClick()
			{
				MainWindow.Instance.JumpToReference(new EntityReference(module, genericParamConstraint.Parameter, protocol: "metadata"));
			}

			string ownerTooltip;

			public string OwnerTooltip {
				get {
					if (ownerTooltip == null)
					{
						ITextOutput output = new PlainTextOutput();
						var p = metadata.GetGenericParameter(genericParamConstraint.Parameter);
						output.Write("parameter " + p.Index + (p.Name.IsNil ? "" : " (" + metadata.GetString(p.Name) + ")") + " of ");
						p.Parent.WriteTo(module, output, default);
						ownerTooltip = output.ToString();
					}
					return ownerTooltip;
				}
			}

			[StringFormat("X8")]
			[LinkToTable]
			public int Type => MetadataTokens.GetToken(genericParamConstraint.Type);

			public void OnTypeClick()
			{
				MainWindow.Instance.JumpToReference(new EntityReference(module, genericParamConstraint.Type, protocol: "metadata"));
			}

			public string TypeTooltip {
				get {
					ITextOutput output = new PlainTextOutput();
					genericParamConstraint.Type.WriteTo(module, output, default);
					return output.ToString();
				}
			}

			public GenericParamConstraintEntry(PEFile module, GenericParameterConstraintHandle handle)
			{
				this.metadataOffset = module.Reader.PEHeaders.MetadataStartOffset;
				this.module = module;
				this.metadata = module.Metadata;
				this.handle = handle;
				this.genericParamConstraint = metadata.GetGenericParameterConstraint(handle);
				this.ownerTooltip = null;
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "GenericParamConstraints");
		}
	}
}