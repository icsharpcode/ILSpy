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
	sealed class DebugDirectoryEntryTreeNode : ILSpyTreeNode
	{
		readonly PEFile module;
		readonly PEReader reader;
		readonly DebugDirectoryEntry entry;
		public DebugDirectoryEntryTreeNode(PEFile module, DebugDirectoryEntry entry)
		{
			this.module = module;
			this.reader = module.Reader;
			this.entry = entry;
		}

		override public object Text => $"{entry.Type}";

		public override object Icon => Images.Literal;

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, Text.ToString());
			if (entry.DataSize > 0)
			{
				language.WriteCommentLine(output, $"Raw Data ({entry.DataSize}):");
				int dataOffset = module.Reader.IsLoadedImage ? entry.DataRelativeVirtualAddress : entry.DataPointer;
				var data = module.Reader.GetEntireImage().GetContent(dataOffset, entry.DataSize);
				language.WriteCommentLine(output, data.ToHexString(data.Length));
			}
			else
			{
				language.WriteCommentLine(output, $"(no data)");
			} 
		}
	}
}
