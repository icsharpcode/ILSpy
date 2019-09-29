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
using System.Reflection.Metadata;
using System.Text;
using System.Windows;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy
{
	public class RichTextModelOutput : ISmartTextOutput
	{
		readonly Stack<HighlightingColor> colorStack = new Stack<HighlightingColor>();
		HighlightingColor currentColor = new HighlightingColor();
		int currentColorBegin = -1;
		ILocatable textOutput;

		public string IndentationString { get; set; } = "\t";
		public RichTextModel Model { get; set; } = new RichTextModel();

		public RichTextModelOutput(ILocatable textOutput)
		{
			this.textOutput = textOutput;
		}

		public void AddUIElement(Func<UIElement> element)
		{
			throw new NotSupportedException();
		}

		public void BeginSpan(HighlightingColor highlightingColor)
		{
			if (currentColorBegin > -1)
				Model.SetHighlighting(currentColorBegin, textOutput.Length - currentColorBegin, currentColor);
			colorStack.Push(currentColor);
			currentColor = currentColor.Clone();
			currentColorBegin = textOutput.Length;
			currentColor.MergeWith(highlightingColor);
			currentColor.Freeze();
		}

		public void EndSpan()
		{
			Model.SetHighlighting(currentColorBegin, textOutput.Length - currentColorBegin, currentColor);
			currentColor = colorStack.Pop();
			currentColorBegin = textOutput.Length;
		}

		public void Indent()
		{
		}

		public void Unindent()
		{
		}

		public void Write(char ch)
		{
		}

		public void Write(string text)
		{
		}

		public void WriteLine()
		{
		}

		public void WriteReference(Decompiler.Disassembler.OpCodeInfo opCode, bool omitSuffix = false)
		{
		}

		public void WriteReference(PEFile module, EntityHandle handle, string text, bool isDefinition = false)
		{
		}

		public void WriteReference(IType type, string text, bool isDefinition = false)
		{
		}

		public void WriteReference(IMember member, string text, bool isDefinition = false)
		{
		}

		public void WriteLocalReference(string text, object reference, bool isDefinition = false)
		{
		}

		public void MarkFoldStart(string collapsedText = "...", bool defaultCollapsed = false)
		{
		}

		public void MarkFoldEnd()
		{
		}
	}
}
