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

using System.Reflection.Metadata;
using System.Text;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ILSpy.TextView
{
	/// <summary>
	/// Phase-1 <see cref="ITextOutput"/> for the Avalonia text view: just accumulates text into a
	/// <see cref="StringBuilder"/> with the indentation contract the decompiler expects. Reference
	/// markers, fold ranges, and inline UI elements are intentionally dropped — those will land
	/// when we add hyperlinks and folding support.
	/// </summary>
	sealed class AvaloniaEditTextOutput : ITextOutput
	{
		readonly StringBuilder builder = new();
		int indent;
		bool needsIndent;

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
			// Folding support arrives in a later phase.
		}

		public void MarkFoldEnd()
		{
			// Folding support arrives in a later phase.
		}
	}
}
