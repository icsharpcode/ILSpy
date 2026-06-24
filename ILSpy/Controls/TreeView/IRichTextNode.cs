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

using AvaloniaEdit.Highlighting;

namespace ICSharpCode.ILSpy.Controls.TreeView
{
	/// <summary>
	/// Implemented by tree nodes whose label should render as syntax-highlighted rich text
	/// (coloured runs, bold type names) instead of the plain string from
	/// <c>SharpTreeNode.Text</c>. The cell template renders <see cref="CreateRichText"/> when
	/// it returns a value and falls back to the plain <c>Text</c> otherwise, so a node's
	/// <c>Text</c> stays the authoritative plain-string representation for search, copy and
	/// keyboard navigation.
	/// </summary>
	public interface IRichTextNode
	{
		/// <summary>
		/// The highlighted label, or <see langword="null"/> to fall back to the plain
		/// <c>Text</c> (e.g. when no entity is available or the active language has no
		/// highlighting). The returned text must equal the plain <c>Text</c>.
		/// </summary>
		RichText? CreateRichText();
	}
}
