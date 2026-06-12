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

using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

using Avalonia.Controls;

using AvaloniaEdit.Document;
using AvaloniaEdit.Folding;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Rendering;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.TextView
{
	/// <summary>
	/// Avalonia-side <see cref="ITextOutput"/> + <see cref="ISmartTextOutput"/>: accumulates text
	/// into a <see cref="StringBuilder"/> and tracks <c>BeginSpan</c>/<c>EndSpan</c> pairs into a
	/// <see cref="RichTextModel"/> that the text view renders via <c>RichTextColorizer</c>.
	/// Reference markers, fold ranges, and inline UI elements are intentionally dropped here —
	/// those land when we add hyperlinks and folding support.
	/// </summary>
	public sealed class AvaloniaEditTextOutput : ISmartTextOutput
	{
		readonly StringBuilder builder = new();

		/// <summary>
		/// When the accumulated text exceeds this many characters, the next write throws
		/// <see cref="OutputLengthExceededException"/> so a runaway decompile is stopped before it
		/// hangs/OOMs the UI thread. Defaults to unlimited; the decompiler tab sets a real limit.
		/// </summary>
		public int LengthLimit { get; set; } = int.MaxValue;

		readonly Stack<(int Offset, HighlightingColor Color)> openSpans = new();
		readonly Stack<(NewFolding Folding, int StartLine)> openFoldings = new();
		readonly List<NewFolding> foldings = new();
		int indent;
		bool needsIndent;
		int lineNumber = 1;

		public RichTextModel HighlightingModel { get; } = new RichTextModel();

		// The same spans that build HighlightingModel, but holding the SHARED named
		// HighlightingColor instances (RichTextModel.SetHighlighting clones them, so the model
		// itself freezes the colours at decompile time). Kept so the view can rebuild the model
		// on a theme switch -- the shared instances are re-coloured in place by ThemeManager.
		readonly List<(int Start, int Length, HighlightingColor Color)> highlightingSpans = new();

		/// <summary>The highlighting spans referencing the live named colours; see the field note.</summary>
		public IReadOnlyList<(int Start, int Length, HighlightingColor Color)> HighlightingSpans => highlightingSpans;

		/// <summary>Foldings collected during writing — only ones spanning more than one line.</summary>
		public IReadOnlyList<NewFolding> Foldings => foldings;

		/// <summary>
		/// Hyperlink ranges and their reference targets. Sorted by start offset; safe to query
		/// with <see cref="TextSegmentCollection{T}.FindSegmentsContaining"/> after construction.
		/// </summary>
		public TextSegmentCollection<ReferenceSegment> References { get; } = new();

		/// <summary>Maps reference targets to their definition offsets in the rendered text.</summary>
		public DefinitionLookup DefinitionLookup { get; } = new();

		readonly List<KeyValuePair<int, Func<Control>>> uiElements = new();

		/// <summary>Inline UI elements collected during writing, in offset order. Fed to
		/// <see cref="UIElementGenerator"/> by the text view.</summary>
		public IReadOnlyList<KeyValuePair<int, Func<Control>>> UIElements => uiElements;

		readonly List<VisualLineElementGenerator> elementGenerators = new();

		/// <summary>
		/// Custom <see cref="VisualLineElementGenerator"/>s contributed by the writer (e.g. a
		/// regex-based hyperlink generator). Installed on the text view alongside the
		/// document and torn down again when the document is replaced.
		/// </summary>
		public IReadOnlyList<VisualLineElementGenerator> ElementGenerators => elementGenerators;

		public string Title { get; set; } = string.Empty;

		/// <summary>
		/// Optional file extension override that decides syntax highlighting for the output.
		/// Defaults to null (use the active language's extension). Resource entry nodes set this
		/// to e.g. <c>".xml"</c> so an embedded XML resource still highlights as XML even though
		/// the active language is C#.
		/// </summary>
		public string? SyntaxExtensionOverride { get; set; }

		public string IndentationString { get; set; } = "\t";

		/// <summary>
		/// When set, newlines are written as single spaces and indentation is suppressed, so
		/// multi-line writer output (e.g. a disassembled member header) collapses to one
		/// line. Used for single-line surfaces like the hover tooltip.
		/// </summary>
		public bool IgnoreNewLineAndIndent { get; set; }

		public int TextLength => builder.Length;

		public string GetText() => builder.ToString();

		public void Indent()
		{
			if (!IgnoreNewLineAndIndent)
				indent++;
		}

		public void Unindent()
		{
			if (!IgnoreNewLineAndIndent && indent > 0)
				indent--;
		}

		void WriteIndentIfNeeded()
		{
			if (IgnoreNewLineAndIndent || !needsIndent)
				return;
			needsIndent = false;
			for (int i = 0; i < indent; i++)
				builder.Append(IndentationString);
		}

		public void Write(char ch)
		{
			WriteIndentIfNeeded();
			builder.Append(ch);
			CheckLength();
		}

		public void Write(string text)
		{
			WriteIndentIfNeeded();
			builder.Append(text);
			CheckLength();
		}

		public void WriteLine()
		{
			if (IgnoreNewLineAndIndent)
			{
				builder.Append(' ');
				CheckLength();
				return;
			}
			builder.Append('\n');
			lineNumber++;
			needsIndent = true;
			CheckLength();
		}

		void CheckLength()
		{
			if (builder.Length > LengthLimit)
				throw new OutputLengthExceededException();
		}

		public void WriteReference(OpCodeInfo opCode, bool omitSuffix = false)
		{
			WriteIndentIfNeeded();
			int start = builder.Length;
			var name = omitSuffix ? opCode.Name.TrimEnd('.') : opCode.Name;
			builder.Append(name);
			References.Add(new ReferenceSegment {
				StartOffset = start,
				EndOffset = builder.Length,
				Reference = opCode,
			});
		}

		public void WriteReference(MetadataFile metadata, Handle handle, string text, string protocol = "decompile", bool isDefinition = false)
			=> AddReference(text, new EntityReference(metadata, handle, protocol), local: false, isDefinition);

		public void WriteReference(IType type, string text, bool isDefinition = false)
			=> AddReference(text, type, local: false, isDefinition);

		public void WriteReference(IMember member, string text, bool isDefinition = false)
			=> AddReference(text, member, local: false, isDefinition);

		public void WriteLocalReference(string text, object reference, bool isDefinition = false)
			=> AddReference(text, reference, local: true, isDefinition);

		void AddReference(string text, object reference, bool local, bool isDefinition)
		{
			WriteIndentIfNeeded();
			int start = builder.Length;
			builder.Append(text);
			int end = builder.Length;
			if (isDefinition)
				DefinitionLookup.AddDefinition(reference, end);
			References.Add(new ReferenceSegment {
				StartOffset = start,
				EndOffset = end,
				Reference = reference,
				IsLocal = local,
				IsDefinition = isDefinition,
			});
		}

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

		public void AddUIElement(Func<Control> element)
		{
			if (element == null)
				return;
			if (uiElements.Count > 0 && uiElements[uiElements.Count - 1].Key == builder.Length)
				throw new InvalidOperationException("Only one UIElement is allowed for each position in the document.");
			// Store the factory itself (not a cached Lazy): each editor's UIElementGenerator
			// builds and caches its own control instance, so the same control is never shared
			// across two editors -- which would make AvaloniaEdit throw "already has a visual
			// parent" when a reused/reopened tab renders into a different TextView.
			uiElements.Add(new KeyValuePair<int, Func<Control>>(builder.Length, element));
		}

		public void AddVisualLineElementGenerator(VisualLineElementGenerator generator)
		{
			ArgumentNullException.ThrowIfNull(generator);
			elementGenerators.Add(generator);
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
			{
				HighlightingModel.SetHighlighting(start, length, color);
				highlightingSpans.Add((start, length, color));
			}
		}
	}
}
