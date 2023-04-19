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
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Windows;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

using TextLocation = ICSharpCode.Decompiler.CSharp.Syntax.TextLocation;

namespace ICSharpCode.ILSpy.TextView
{
	/// <summary>
	/// A text segment that references some object. Used for hyperlinks in the editor.
	/// </summary>
	public sealed class ReferenceSegment : TextSegment
	{
		public object Reference;
		public bool IsLocal;
		public bool IsDefinition;
	}

	/// <summary>
	/// Stores the positions of the definitions that were written to the text output.
	/// </summary>
	sealed class DefinitionLookup
	{
		internal Dictionary<object, int> definitions = new Dictionary<object, int>();

		public int GetDefinitionPosition(object definition)
		{
			int val;
			if (definitions.TryGetValue(definition, out val))
				return val;
			else
				return -1;
		}

		public void AddDefinition(object definition, int offset)
		{
			definitions[definition] = offset;
		}
	}

	/// <summary>
	/// Text output implementation for AvalonEdit.
	/// </summary>
	public sealed class AvalonEditTextOutput : ISmartTextOutput
	{
		int lastLineStart = 0;
		int lineNumber = 1;
		readonly StringBuilder b = new StringBuilder();

		/// <summary>Current indentation level</summary>
		int indent;
		/// <summary>Whether indentation should be inserted on the next write</summary>
		bool needsIndent;

		public string IndentationString { get; set; } = "\t";

		internal bool IgnoreNewLineAndIndent { get; set; }

		public string Title { get; set; } = Properties.Resources.NewTab;

		/// <summary>
		/// Gets/sets the <see cref="Uri"/> that is displayed by this view.
		/// Used to identify the AboutPage and other views built into ILSpy in the navigation history.
		/// </summary>
		public Uri Address { get; set; }

		internal readonly List<VisualLineElementGenerator> elementGenerators = new List<VisualLineElementGenerator>();

		/// <summary>List of all references that were written to the output</summary>
		TextSegmentCollection<ReferenceSegment> references = new TextSegmentCollection<ReferenceSegment>();

		/// <summary>Stack of the fold markers that are open but not closed yet</summary>
		Stack<(NewFolding, int startLine)> openFoldings = new Stack<(NewFolding, int startLine)>();

		/// <summary>List of all foldings that were written to the output</summary>
		internal readonly List<NewFolding> Foldings = new List<NewFolding>();

		internal readonly DefinitionLookup DefinitionLookup = new DefinitionLookup();

		internal bool EnableHyperlinks { get; set; }

		/// <summary>Embedded UIElements, see <see cref="UIElementGenerator"/>.</summary>
		internal readonly List<KeyValuePair<int, Lazy<UIElement>>> UIElements = new List<KeyValuePair<int, Lazy<UIElement>>>();

		public RichTextModel HighlightingModel { get; } = new RichTextModel();

		public AvalonEditTextOutput()
		{
		}

		/// <summary>
		/// Gets the list of references (hyperlinks).
		/// </summary>
		internal TextSegmentCollection<ReferenceSegment> References {
			get { return references; }
		}

		public void AddVisualLineElementGenerator(VisualLineElementGenerator elementGenerator)
		{
			elementGenerators.Add(elementGenerator);
		}

		/// <summary>
		/// Controls the maximum length of the text.
		/// When this length is exceeded, an <see cref="OutputLengthExceededException"/> will be thrown,
		/// thus aborting the decompilation.
		/// </summary>
		public int LengthLimit = int.MaxValue;

		public int TextLength {
			get { return b.Length; }
		}

		public TextLocation Location {
			get {
				return new TextLocation(lineNumber, b.Length - lastLineStart + 1 + (needsIndent ? indent : 0));
			}
		}

		#region Text Document
		TextDocument textDocument;

		/// <summary>
		/// Prepares the TextDocument.
		/// This method may be called by the background thread writing to the output.
		/// Once the document is prepared, it can no longer be written to.
		/// </summary>
		/// <remarks>
		/// Calling this method on the background thread ensures the TextDocument's line tokenization
		/// runs in the background and does not block the GUI.
		/// </remarks>
		public void PrepareDocument()
		{
			if (textDocument == null)
			{
				textDocument = new TextDocument(b.ToString());
				textDocument.SetOwnerThread(null); // release ownership
			}
		}

		/// <summary>
		/// Retrieves the TextDocument.
		/// Once the document is retrieved, it can no longer be written to.
		/// </summary>
		public TextDocument GetDocument()
		{
			PrepareDocument();
			textDocument.SetOwnerThread(System.Threading.Thread.CurrentThread); // acquire ownership
			return textDocument;
		}
		#endregion

		public void Indent()
		{
			if (IgnoreNewLineAndIndent)
				return;
			indent++;
		}

		public void Unindent()
		{
			if (IgnoreNewLineAndIndent)
				return;
			indent--;
		}

		void WriteIndent()
		{
			if (IgnoreNewLineAndIndent)
				return;
			Debug.Assert(textDocument == null);
			if (needsIndent)
			{
				needsIndent = false;
				for (int i = 0; i < indent; i++)
				{
					b.Append(IndentationString);
				}
			}
		}

		public void Write(char ch)
		{
			WriteIndent();
			b.Append(ch);
		}

		public void Write(string text)
		{
			WriteIndent();
			b.Append(text);
		}

		public void WriteLine()
		{
			Debug.Assert(textDocument == null);
			if (IgnoreNewLineAndIndent)
			{
				b.Append(' ');
			}
			else
			{
				b.AppendLine();
				needsIndent = true;
				lastLineStart = b.Length;
				lineNumber++;
			}
			if (this.TextLength > LengthLimit)
			{
				throw new OutputLengthExceededException();
			}
		}

		public void WriteReference(Decompiler.Disassembler.OpCodeInfo opCode, bool omitSuffix = false)
		{
			WriteIndent();
			int start = this.TextLength;
			if (omitSuffix)
			{
				int lastDot = opCode.Name.LastIndexOf('.');
				if (lastDot > 0)
				{
					b.Append(opCode.Name.Remove(lastDot + 1));
				}
			}
			else
			{
				b.Append(opCode.Name);
			}
			int end = this.TextLength - 1;
			references.Add(new ReferenceSegment { StartOffset = start, EndOffset = end, Reference = opCode });
		}

		public void WriteReference(PEFile module, Handle handle, string text, string protocol = "decompile", bool isDefinition = false)
		{
			WriteIndent();
			int start = this.TextLength;
			b.Append(text);
			int end = this.TextLength;
			if (isDefinition)
			{
				this.DefinitionLookup.AddDefinition((module, handle), this.TextLength);
			}
			references.Add(new ReferenceSegment { StartOffset = start, EndOffset = end, Reference = new EntityReference(module, handle, protocol), IsDefinition = isDefinition });
		}

		public void WriteReference(IType type, string text, bool isDefinition = false)
		{
			WriteIndent();
			int start = this.TextLength;
			b.Append(text);
			int end = this.TextLength;
			if (isDefinition)
			{
				this.DefinitionLookup.AddDefinition(type, this.TextLength);
			}
			references.Add(new ReferenceSegment { StartOffset = start, EndOffset = end, Reference = type, IsDefinition = isDefinition });
		}

		public void WriteReference(IMember member, string text, bool isDefinition = false)
		{
			WriteIndent();
			int start = this.TextLength;
			b.Append(text);
			int end = this.TextLength;
			if (isDefinition)
			{
				this.DefinitionLookup.AddDefinition(member, this.TextLength);
			}
			references.Add(new ReferenceSegment { StartOffset = start, EndOffset = end, Reference = member, IsDefinition = isDefinition });
		}

		public void WriteLocalReference(string text, object reference, bool isDefinition = false)
		{
			WriteIndent();
			int start = this.TextLength;
			b.Append(text);
			int end = this.TextLength;
			if (isDefinition)
			{
				this.DefinitionLookup.AddDefinition(reference, this.TextLength);
			}
			references.Add(new ReferenceSegment { StartOffset = start, EndOffset = end, Reference = reference, IsLocal = true, IsDefinition = isDefinition });
		}

		public void MarkFoldStart(string collapsedText = "...", bool defaultCollapsed = false, bool isDefinition = false)
		{
			WriteIndent();
			openFoldings.Push((
				new NewFolding {
					StartOffset = this.TextLength,
					Name = collapsedText,
					DefaultClosed = defaultCollapsed,
					IsDefinition = isDefinition,
				}, lineNumber));
		}

		public void MarkFoldEnd()
		{
			var (f, startLine) = openFoldings.Pop();
			f.EndOffset = this.TextLength;
			if (startLine != lineNumber)
				this.Foldings.Add(f);
		}

		public void AddUIElement(Func<UIElement> element)
		{
			if (element != null)
			{
				if (this.UIElements.Count > 0 && this.UIElements.Last().Key == this.TextLength)
					throw new InvalidOperationException("Only one UIElement is allowed for each position in the document");
				this.UIElements.Add(new KeyValuePair<int, Lazy<UIElement>>(this.TextLength, new Lazy<UIElement>(element)));
			}
		}

		readonly Stack<HighlightingColor> colorStack = new Stack<HighlightingColor>();
		HighlightingColor currentColor = new HighlightingColor();
		int currentColorBegin = -1;

		public void BeginSpan(HighlightingColor highlightingColor)
		{
			WriteIndent();
			if (currentColorBegin > -1)
				HighlightingModel.SetHighlighting(currentColorBegin, b.Length - currentColorBegin, currentColor);
			colorStack.Push(currentColor);
			currentColor = currentColor.Clone();
			currentColorBegin = b.Length;
			currentColor.MergeWith(highlightingColor);
			currentColor.Freeze();
		}

		public void EndSpan()
		{
			HighlightingModel.SetHighlighting(currentColorBegin, b.Length - currentColorBegin, currentColor);
			currentColor = colorStack.Pop();
			currentColorBegin = b.Length;
		}
	}
}
