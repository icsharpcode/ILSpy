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
using System.IO;
using System.Reflection.Metadata;

using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler
{
	public sealed class PlainTextOutput : ITextOutput
	{
		readonly TextWriter writer;
		int indent;
		bool needsIndent;

		int line = 1;
		int column = 1;

		public string IndentationString { get; set; } = "\t";

		public PlainTextOutput(TextWriter writer)
		{
			if (writer == null)
				throw new ArgumentNullException(nameof(writer));
			this.writer = writer;
		}

		public PlainTextOutput()
		{
			this.writer = new StringWriter();
		}

		public TextLocation Location {
			get {
				return new TextLocation(line, column + (needsIndent ? indent : 0));
			}
		}

		public override string ToString()
		{
			return writer.ToString();
		}

		public void Indent()
		{
			indent++;
		}

		public void Unindent()
		{
			indent--;
		}

		void WriteIndent()
		{
			if (needsIndent)
			{
				needsIndent = false;
				for (int i = 0; i < indent; i++)
				{
					writer.Write(IndentationString);
				}
				column += indent;
			}
		}

		public void Write(char ch)
		{
			WriteIndent();
			writer.Write(ch);
			column++;
		}

		public void Write(string text)
		{
			WriteIndent();
			writer.Write(text);
			column += text.Length;
		}

		public void WriteLine()
		{
			writer.WriteLine();
			needsIndent = true;
			line++;
			column = 1;
		}

		public void WriteReference(Disassembler.OpCodeInfo opCode, bool omitSuffix = false)
		{
			if (omitSuffix)
			{
				int lastDot = opCode.Name.LastIndexOf('.');
				if (lastDot > 0)
				{
					Write(opCode.Name.Remove(lastDot + 1));
				}
			}
			else
			{
				Write(opCode.Name);
			}
		}

		public void WriteReference(PEFile module, Handle handle, string text, string protocol = "decompile", bool isDefinition = false)
		{
			Write(text);
		}

		public void WriteReference(IType type, string text, bool isDefinition = false)
		{
			Write(text);
		}

		public void WriteReference(IMember member, string text, bool isDefinition = false)
		{
			Write(text);
		}

		public void WriteLocalReference(string text, object reference, bool isDefinition = false)
		{
			Write(text);
		}

		void ITextOutput.MarkFoldStart(string collapsedText, bool defaultCollapsed)
		{
		}

		void ITextOutput.MarkFoldEnd()
		{
		}
	}

	internal class TextOutputWithRollback : ITextOutput
	{
		List<Action<ITextOutput>> actions;
		ITextOutput target;

		public TextOutputWithRollback(ITextOutput target)
		{
			this.target = target;
			this.actions = new List<Action<ITextOutput>>();
		}

		string ITextOutput.IndentationString {
			get {
				return target.IndentationString;
			}
			set {
				target.IndentationString = value;
			}
		}

		public void Commit()
		{
			foreach (var action in actions)
			{
				action(target);
			}
		}

		public void Indent()
		{
			actions.Add(target => target.Indent());
		}

		public void MarkFoldEnd()
		{
			actions.Add(target => target.MarkFoldEnd());
		}

		public void MarkFoldStart(string collapsedText = "...", bool defaultCollapsed = false)
		{
			actions.Add(target => target.MarkFoldStart(collapsedText, defaultCollapsed));
		}

		public void Unindent()
		{
			actions.Add(target => target.Unindent());
		}

		public void Write(char ch)
		{
			actions.Add(target => target.Write(ch));
		}

		public void Write(string text)
		{
			actions.Add(target => target.Write(text));
		}

		public void WriteLine()
		{
			actions.Add(target => target.WriteLine());
		}

		public void WriteLocalReference(string text, object reference, bool isDefinition = false)
		{
			actions.Add(target => target.WriteLocalReference(text, reference, isDefinition));
		}

		public void WriteReference(OpCodeInfo opCode, bool omitSuffix = false)
		{
			actions.Add(target => target.WriteReference(opCode));
		}

		public void WriteReference(PEFile module, Handle handle, string text, string protocol = "decompile", bool isDefinition = false)
		{
			actions.Add(target => target.WriteReference(module, handle, text, protocol, isDefinition));
		}

		public void WriteReference(IType type, string text, bool isDefinition = false)
		{
			actions.Add(target => target.WriteReference(type, text, isDefinition));
		}

		public void WriteReference(IMember member, string text, bool isDefinition = false)
		{
			actions.Add(target => target.WriteReference(member, text, isDefinition));
		}
	}
}
