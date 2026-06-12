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

using Avalonia.Controls;

using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Rendering;

using ICSharpCode.Decompiler;

namespace ICSharpCode.ILSpy.TextView
{
	/// <summary>
	/// Adds Avalonia-specific output features to <see cref="ITextOutput"/>: span-based
	/// highlighting + inline UI elements rendered by <see cref="UIElementGenerator"/>.
	/// </summary>
	public interface ISmartTextOutput : ITextOutput
	{
		/// <summary>
		/// Inserts an interactive UI element at the current position. The factory runs lazily
		/// on the UI thread when the element generator first realises the row.
		/// </summary>
		void AddUIElement(Func<Control> element);

		/// <summary>
		/// Contributes a <see cref="VisualLineElementGenerator"/> (typically a regex-based
		/// hyperlink generator) that the text view installs alongside the document.
		/// </summary>
		void AddVisualLineElementGenerator(VisualLineElementGenerator generator);

		void BeginSpan(HighlightingColor highlightingColor);
		void EndSpan();

		/// <summary>
		/// Title displayed in the document tab's header.
		/// </summary>
		string Title { get; set; }
	}
}
