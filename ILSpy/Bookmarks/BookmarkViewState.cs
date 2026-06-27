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
using System.Linq;

using ICSharpCode.ILSpy.Options;
using ICSharpCode.ILSpy.TextView;

namespace ICSharpCode.ILSpy.Bookmarks
{
	/// <summary>A serializable folding range stored inside a bookmark view-state payload.</summary>
	public sealed record BookmarkFoldingRange(int Start, int End);

	/// <summary>Display settings that affect the rendered C# text layout.</summary>
	public sealed record BookmarkRenderedLayoutSettings(
		bool FoldBraces,
		bool ExpandMemberDefinitions,
		bool ExpandUsingDeclarations,
		bool ShowDebugInfo,
		bool IndentationUseTabs,
		int IndentationSize,
		int IndentationTabSize)
	{
		public static BookmarkRenderedLayoutSettings From(DisplaySettings display)
			=> new(
				display.FoldBraces,
				display.ExpandMemberDefinitions,
				display.ExpandUsingDeclarations,
				display.ShowDebugInfo,
				display.IndentationUseTabs,
				display.IndentationSize,
				display.IndentationTabSize);

		public void ApplyTo(DisplaySettings display)
		{
			display.FoldBraces = FoldBraces;
			display.ExpandMemberDefinitions = ExpandMemberDefinitions;
			display.ExpandUsingDeclarations = ExpandUsingDeclarations;
			display.ShowDebugInfo = ShowDebugInfo;
			display.IndentationUseTabs = IndentationUseTabs;
			display.IndentationSize = IndentationSize;
			display.IndentationTabSize = IndentationTabSize;
		}
	}

	/// <summary>
	/// Serializable editor view state captured with a bookmark. The version is part of the payload so
	/// future schema changes can be handled without changing the bookmark file version.
	/// </summary>
	public sealed record BookmarkViewState(
		int Version,
		int CaretOffset,
		double VerticalOffset,
		double HorizontalOffset,
		int? FoldingChecksum,
		IReadOnlyList<BookmarkFoldingRange>? ExpandedFoldings,
		BookmarkRenderedLayoutSettings? RenderedLayoutSettings = null,
		IReadOnlyList<string>? SelectedTreeNodePath = null)
	{
		public static BookmarkViewState From(DecompilerTextViewState state, DisplaySettings? displaySettings = null,
			IReadOnlyList<string>? selectedTreeNodePath = null)
			=> new(
				1,
				state.CaretOffset,
				state.VerticalOffset,
				state.HorizontalOffset,
				state.Foldings?.Checksum,
				state.Foldings?.Expanded.Select(r => new BookmarkFoldingRange(r.Start, r.End)).ToArray(),
				displaySettings != null ? BookmarkRenderedLayoutSettings.From(displaySettings) : null,
				selectedTreeNodePath);

		public DecompilerTextViewState ToDecompilerTextViewState()
		{
			FoldingsViewState.Snapshot? foldings = null;
			if (FoldingChecksum is { } checksum)
			{
				var expanded = ExpandedFoldings?.Select(r => (r.Start, r.End)).ToArray()
					?? System.Array.Empty<(int Start, int End)>();
				foldings = new FoldingsViewState.Snapshot(expanded, checksum);
			}
			return new DecompilerTextViewState(CaretOffset, VerticalOffset, HorizontalOffset, foldings);
		}
	}
}
