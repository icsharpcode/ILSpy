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

using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;

namespace ICSharpCode.ILSpy.TextView
{
	/// <summary>
	/// Creates clickable hyperlink elements in the text view from a
	/// <see cref="TextSegmentCollection{ReferenceSegment}"/> emitted by the decompiler.
	/// </summary>
	sealed class ReferenceElementGenerator : VisualLineElementGenerator
	{
		readonly Predicate<ReferenceSegment> isLink;

		public TextSegmentCollection<ReferenceSegment>? References { get; set; }

		public ReferenceElementGenerator(Predicate<ReferenceSegment> isLink)
		{
			this.isLink = isLink ?? throw new ArgumentNullException(nameof(isLink));
		}

		public override int GetFirstInterestedOffset(int startOffset)
		{
			if (References == null)
				return -1;
			var segment = References.FindFirstSegmentWithStartAfter(startOffset);
			return segment != null ? segment.StartOffset : -1;
		}

		public override VisualLineElement? ConstructElement(int offset)
		{
			if (References == null)
				return null;
			foreach (var segment in References.FindSegmentsContaining(offset))
			{
				if (!isLink(segment))
					continue;
				// Hyperlinks can't span line breaks — clamp to the current line.
				int endOffset = Math.Min(segment.EndOffset, CurrentContext.VisualLine.LastDocumentLine.EndOffset);
				if (offset < endOffset)
					return new VisualLineReferenceText(CurrentContext.VisualLine, endOffset - offset, this, segment);
			}
			return null;
		}
	}
}
