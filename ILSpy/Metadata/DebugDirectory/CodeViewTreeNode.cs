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

#nullable enable

using System.Reflection.PortableExecutable;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Metadata
{
	sealed class CodeViewTreeNode : ILSpyTreeNode
	{
		readonly CodeViewDebugDirectoryData entry;
		public CodeViewTreeNode(CodeViewDebugDirectoryData entry)
		{
			this.entry = entry;
		}

		override public object Text => nameof(DebugDirectoryEntryType.CodeView);

		public override object ToolTip => "Associated PDB file description.";

		public override object Icon => Images.Literal;

		public override bool View(TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var dataGrid = Helpers.PrepareDataGrid(tabPage, this);

			dataGrid.ItemsSource = new[] { entry };

			tabPage.Content = dataGrid;

			return true;
		}

		sealed class PdbChecksumDebugDirectoryDataEntry
		{
			readonly CodeViewDebugDirectoryData entry;
			public PdbChecksumDebugDirectoryDataEntry(CodeViewDebugDirectoryData entry)
			{
				this.entry = entry;
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, Text.ToString());
			language.WriteCommentLine(output, $"GUID: {entry.Guid}");
			language.WriteCommentLine(output, $"Age: {entry.Age}");
			language.WriteCommentLine(output, $"Path: {entry.Path}");
		}
	}
}
