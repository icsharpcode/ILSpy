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

using System.Globalization;

using CommunityToolkit.Mvvm.ComponentModel;

namespace ICSharpCode.ILSpy.Bookmarks
{
	/// <summary>
	/// How a <see cref="Bookmark"/> is anchored to a decompiled member. Prefer semantic anchors
	/// (metadata token or method IL offset) and use the rendered line only when no stable semantic
	/// anchor exists for the clicked line.
	/// </summary>
	public enum BookmarkKind
	{
		/// <summary>A definition line (type, method, field, property, event): module + token.</summary>
		Token,

		/// <summary>A line inside a method body: module + method token + IL offset.</summary>
		Body,

		/// <summary>A displayed line without a stable semantic anchor: module + containing token + visible line number.</summary>
		Line
	}

	/// <summary>
	/// A single user bookmark in the flat bookmark list. The anchor fields are immutable
	/// (set when the bookmark is created); only <see cref="Name"/> and <see cref="Enabled"/>
	/// change at runtime, both bound two-way in the bookmarks pane.
	/// </summary>
	public sealed partial class Bookmark : ObservableObject
	{
		int lineNumber;

		[ObservableProperty]
		private string name = "";

		[ObservableProperty]
		private bool enabled = true;

		/// <summary>Path of the assembly file, used to reload it from disk on navigation.</summary>
		public required string FileName { get; init; }

		/// <summary>Assembly identity (also part of the duplicate/merge key).</summary>
		public required string AssemblyFullName { get; init; }

		/// <summary>Module file name, shown in the pane's "Module" column.</summary>
		public required string ModuleName { get; init; }

		/// <summary>Metadata token of the member (or, for <see cref="BookmarkKind.Body"/>, the enclosing method).</summary>
		public required uint Token { get; init; }

		public required BookmarkKind Kind { get; init; }

		/// <summary>IL offset within the method body; meaningful only for <see cref="BookmarkKind.Body"/>.</summary>
		public int ILOffset { get; init; }

		/// <summary>Visible document line number captured in the rendered text.</summary>
		public int LineNumber {
			get => lineNumber;
			init => lineNumber = value;
		}

		/// <summary>
		/// Display name of the anchored member. Navigation
		/// resolves purely by token and does not compare this name: a token can legitimately point at
		/// a compiler-generated member (e.g. a local function) whose resolved name differs from this
		/// stored display name, and a name check would wrongly reject that valid navigation.
		/// </summary>
		public required string MemberName { get; init; }

		/// <summary>Full display name of the tree node whose rendered text contains the bookmark.</summary>
		public string? LocationNodeName { get; init; }

		/// <summary>Decompiler editor view state captured when the bookmark was created.</summary>
		public BookmarkViewState? ViewState { get; set; }

		/// <summary>Location text shown in the bookmarks pane.</summary>
		public string DisplayLocation {
			get {
				var nodeName = !string.IsNullOrEmpty(LocationNodeName)
					? LocationNodeName
					: ViewState?.SelectedTreeNodePath is { Count: > 0 } path
					? path[^1]
					: MemberName;
				return LineNumber > 0
					? string.Create(CultureInfo.InvariantCulture, $"{nodeName}:{LineNumber}")
					: nodeName;
			}
		}

		/// <summary>Updates the rendered line shown in the bookmarks pane after the current document resolves it.</summary>
		public void UpdateRenderedLineNumber(int lineNumber)
		{
			if (lineNumber < 1 || this.lineNumber == lineNumber)
				return;
			this.lineNumber = lineNumber;
			OnPropertyChanged(nameof(DisplayLocation));
		}

		/// <summary>
		/// Identity used for toggle and merge-import deduplication: same assembly + same location
		/// (token, plus IL offset or rendered line for anchors that need one) means the same bookmark.
		/// </summary>
		public string AnchorKey => $"{AssemblyFullName}|{ModuleName}|{Kind}|{Token}|{Kind switch { BookmarkKind.Body => ILOffset, BookmarkKind.Line => LineNumber, _ => -1 }}";
	}
}
