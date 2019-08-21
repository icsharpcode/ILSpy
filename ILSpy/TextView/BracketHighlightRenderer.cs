// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace ICSharpCode.ILSpy.TextView
{
	/// <summary>
	/// Allows language specific search for matching brackets.
	/// </summary>
	public interface IBracketSearcher
	{
		/// <summary>
		/// Searches for a matching bracket from the given offset to the start of the document.
		/// </summary>
		/// <returns>A BracketSearchResult that contains the positions and lengths of the brackets. Return null if there is nothing to highlight.</returns>
		BracketSearchResult SearchBracket(IDocument document, int offset);
	}

	public class DefaultBracketSearcher : IBracketSearcher
	{
		public static readonly DefaultBracketSearcher DefaultInstance = new DefaultBracketSearcher();

		public BracketSearchResult SearchBracket(IDocument document, int offset)
		{
			return null;
		}
	}

	/// <summary>
	/// Describes a pair of matching brackets found by <see cref="IBracketSearcher"/>.
	/// </summary>
	public class BracketSearchResult
	{
		public int OpeningBracketOffset { get; private set; }

		public int OpeningBracketLength { get; private set; }

		public int ClosingBracketOffset { get; private set; }

		public int ClosingBracketLength { get; private set; }

		public BracketSearchResult(int openingBracketOffset, int openingBracketLength,
								   int closingBracketOffset, int closingBracketLength)
		{
			this.OpeningBracketOffset = openingBracketOffset;
			this.OpeningBracketLength = openingBracketLength;
			this.ClosingBracketOffset = closingBracketOffset;
			this.ClosingBracketLength = closingBracketLength;
		}
	}

	public class BracketHighlightRenderer : IBackgroundRenderer
	{
		BracketSearchResult result;
		Pen borderPen;
		Brush backgroundBrush;
		ICSharpCode.AvalonEdit.Rendering.TextView textView;

		public void SetHighlight(BracketSearchResult result)
		{
			if (this.result != result) {
				this.result = result;
				textView.InvalidateLayer(this.Layer);
			}
		}

		public BracketHighlightRenderer(ICSharpCode.AvalonEdit.Rendering.TextView textView)
		{
			if (textView == null)
				throw new ArgumentNullException("textView");

			this.borderPen = new Pen(new SolidColorBrush(Color.FromArgb(52, 0, 0, 255)), 1);
			this.borderPen.Freeze();

			this.backgroundBrush = new SolidColorBrush(Color.FromArgb(22, 0, 0, 255));
			this.backgroundBrush.Freeze();

			this.textView = textView;

			this.textView.BackgroundRenderers.Add(this);
		}

		public KnownLayer Layer {
			get {
				return KnownLayer.Selection;
			}
		}

		public void Draw(ICSharpCode.AvalonEdit.Rendering.TextView textView, DrawingContext drawingContext)
		{
			if (this.result == null)
				return;

			BackgroundGeometryBuilder builder = new BackgroundGeometryBuilder();

			builder.CornerRadius = 1;

			builder.AddSegment(textView, new TextSegment() { StartOffset = result.OpeningBracketOffset, Length = result.OpeningBracketLength });
			builder.CloseFigure(); // prevent connecting the two segments
			builder.AddSegment(textView, new TextSegment() { StartOffset = result.ClosingBracketOffset, Length = result.ClosingBracketLength });

			Geometry geometry = builder.CreateGeometry();
			if (geometry != null) {
				drawingContext.DrawGeometry(backgroundBrush, borderPen, geometry);
			}
		}
	}
}