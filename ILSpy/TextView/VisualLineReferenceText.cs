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

using Avalonia.Input;

using AvaloniaEdit.Rendering;

namespace ILSpy.TextView
{
	/// <summary>
	/// A clickable piece of text that maps back to a <see cref="ReferenceSegment"/>. Sets a hand
	/// cursor for cross-document references and an arrow for in-document ones. The actual click
	/// is handled by the parent generator, which routes the segment to the document's navigator.
	/// </summary>
	sealed class VisualLineReferenceText : VisualLineText
	{
		readonly ReferenceElementGenerator parent;
		readonly ReferenceSegment referenceSegment;

		public ReferenceSegment ReferenceSegment => referenceSegment;

		public VisualLineReferenceText(VisualLine parentVisualLine, int length, ReferenceElementGenerator parent, ReferenceSegment referenceSegment)
			: base(parentVisualLine, length)
		{
			this.parent = parent;
			this.referenceSegment = referenceSegment;
		}

		protected override VisualLineText CreateInstance(int length)
			=> new VisualLineReferenceText(ParentVisualLine, length, parent, referenceSegment);

		protected override void OnQueryCursor(PointerEventArgs e)
		{
			if (e.Source is InputElement inputElement)
			{
				inputElement.Cursor = new Cursor(referenceSegment.IsLocal
					? StandardCursorType.Arrow
					: StandardCursorType.Hand);
			}
			e.Handled = true;
		}

		protected override void OnPointerPressed(PointerPressedEventArgs e)
		{
			if (e.Handled || !e.GetCurrentPoint(null).Properties.IsLeftButtonPressed)
				return;
			parent.OnReferenceClicked(referenceSegment);
			e.Handled = true;
		}
	}
}
