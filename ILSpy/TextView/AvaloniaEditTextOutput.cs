// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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
using System.Text;

using AvaloniaEdit.Folding;
using AvaloniaEdit.Highlighting;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ILSpy.TextView
{
	/// <summary>
	/// Avalonia-side <see cref="ITextOutput"/> + <see cref="ISmartTextOutput"/>: accumulates text
	/// into a <see cref="StringBuilder"/> and tracks <c>BeginSpan</c>/<c>EndSpan</c> pairs into a
	/// <see cref="RichTextModel"/> that the text view renders via <c>RichTextColorizer</c>.
	/// Reference markers, fold ranges, and inline UI elements are intentionally dropped here —
	/// those land when we add hyperlinks and folding support.
	/// </summary>
	sealed class AvaloniaEditTextOutput : ISmartTextOutput
	{
		readonly StringBuilder builder = new();
		readonly Stack<(int Offset, HighlightingColor Color)> openSpans = new();
		readonly Stack<(NewFolding Folding, int StartLine)> openFoldings = new();
		readonly List<NewFolding> foldings = new();
		int indent;
		bool needsIndent;
		int lineNumber = 1;

		public RichTextModel HighlightingModel { get; } = new RichTextModel();

		/// <summary>Foldings collected during writing — only ones spanning more than one line.</summary>
		public IReadOnlyList<NewFolding> Foldings => foldings;

		public string Title { get; set; } = string.Empty;

		public string IndentationString { get; set; } = "\t";

		public int TextLength => builder.Length;

		public string GetText() => builder.ToString();

		public void Indent() => indent++;

		public void Unindent()
		{
			if (indent > 0)
				indent--;
		}

		void WriteIndentIfNeeded()
		{
			if (!needsIndent)
				return;
			needsIndent = false;
			for (int i = 0; i < indent; i++)
				builder.Append(IndentationString);
		}

		public void Write(char ch)
		{
			WriteIndentIfNeeded();
			builder.Append(ch);
		}

		public void Write(string text)
		{
			WriteIndentIfNeeded();
			builder.Append(text);
		}

		public void WriteLine()
		{
			builder.Append('\n');
			lineNumber++;
			needsIndent = true;
		}

		public void WriteReference(OpCodeInfo opCode, bool omitSuffix = false)
		{
			Write(omitSuffix ? opCode.Name.TrimEnd('.') : opCode.Name);
		}

		public void WriteReference(MetadataFile metadata, Handle handle, string text, string protocol = "decompile", bool isDefinition = false)
			=> Write(text);

		public void WriteReference(IType type, string text, bool isDefinition = false) => Write(text);

		public void WriteReference(IMember member, string text, bool isDefinition = false) => Write(text);

		public void WriteLocalReference(string text, object reference, bool isDefinition = false) => Write(text);

		public void MarkFoldStart(string collapsedText = "...", bool defaultCollapsed = false, bool isDefinition = false)
		{
			WriteIndentIfNeeded();
			openFoldings.Push((
				new NewFolding {
					StartOffset = builder.Length,
					Name = collapsedText,
					DefaultClosed = defaultCollapsed,
					IsDefinition = isDefinition,
				},
				lineNumber));
		}

		public void MarkFoldEnd()
		{
			if (openFoldings.Count == 0)
				return;
			var (folding, startLine) = openFoldings.Pop();
			folding.EndOffset = builder.Length;
			// Skip single-line foldings — they only add visual noise.
			if (startLine != lineNumber)
				foldings.Add(folding);
		}

		public void BeginSpan(HighlightingColor highlightingColor)
		{
			openSpans.Push((builder.Length, highlightingColor));
		}

		public void EndSpan()
		{
			if (openSpans.Count == 0)
				return;
			var (start, color) = openSpans.Pop();
			var length = builder.Length - start;
			if (length > 0)
				HighlightingModel.SetHighlighting(start, length, color);
		}
	}
}
