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

using CommunityToolkit.Mvvm.ComponentModel;

namespace ICSharpCode.ILSpy.Bookmarks
{
	/// <summary>
	/// How a <see cref="Bookmark"/> is anchored to a decompiled member. The anchor is a metadata
	/// token (never a raw line number) so it survives re-decompilation and decompiler-setting
	/// changes that reflow the C# text.
	/// </summary>
	public enum BookmarkKind
	{
		/// <summary>A definition line (type, method, field, property, event): module + token.</summary>
		Token,

		/// <summary>A line inside a method body: module + method token + IL offset.</summary>
		Body
	}

	/// <summary>
	/// A single user bookmark in the flat bookmark list. The anchor fields are immutable
	/// (set when the bookmark is created); only <see cref="Name"/> and <see cref="Enabled"/>
	/// change at runtime, both bound two-way in the bookmarks pane.
	/// </summary>
	public sealed partial class Bookmark : ObservableObject
	{
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

		/// <summary>
		/// Display name of the anchored member, shown in the pane's "Location" column and used as a
		/// stale-token guard: if a reloaded module's token no longer resolves to this member, the
		/// bookmark is treated as missing rather than silently pointing at the wrong code.
		/// </summary>
		public required string MemberName { get; init; }

		/// <summary>
		/// Identity used for toggle and merge-import deduplication: same assembly + same location
		/// (token, plus IL offset for body anchors) means the same bookmark.
		/// </summary>
		public string AnchorKey => $"{AssemblyFullName}|{ModuleName}|{Token}|{(Kind == BookmarkKind.Body ? ILOffset : -1)}";
	}
}
