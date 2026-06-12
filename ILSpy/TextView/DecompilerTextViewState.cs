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

namespace ICSharpCode.ILSpy.TextView
{
	/// <summary>
	/// Snapshot of the decompiler editor's view state at a moment in time: caret offset, scroll
	/// offsets, and the set of expanded foldings. Pulled from the live editor when a navigation
	/// record is written, and applied back when Back/Forward re-decompiles the node.
	///
	/// <para>
	/// This replaces the per-axis LastKnown*/Pending* fields that used to be pushed onto the
	/// view-model on every caret/scroll event. That push mistook AvaloniaEdit's programmatic
	/// caret-to-end (performed when the document text is replaced) for a user move and recorded
	/// it, poisoning the captured position. Reading on demand at capture time avoids that.
	/// </para>
	/// </summary>
	public readonly record struct DecompilerTextViewState(
		int CaretOffset,
		double VerticalOffset,
		double HorizontalOffset,
		FoldingsViewState.Snapshot? Foldings);
}
