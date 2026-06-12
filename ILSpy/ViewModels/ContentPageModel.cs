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

namespace ICSharpCode.ILSpy.ViewModels
{
	/// <summary>
	/// Base for the concrete viewmodels that occupy a <see cref="ContentTabPage"/>'s
	/// <see cref="ContentTabPage.Content"/> slot: the decompiler text view, the metadata grid,
	/// the compare view, and the Options panel. Collapsing them under one base lets
	/// <c>ContentTabPage</c> type its <c>Content</c> strongly and read <c>Title</c> /
	/// <c>SupportsLanguageSwitching</c> / <see cref="IsStaticContent"/> directly instead of by
	/// reflection or runtime type-tests scattered across the dock router.
	/// </summary>
	public abstract class ContentPageModel : TabPageModel
	{
		/// <summary>
		/// True for content a tree-node selection must NOT replace in the preview tab -- the
		/// static pages (Options, About, embedded resources). Defaults false (ordinary decompile
		/// / metadata content); static pages set it true. The dock router's "is this the writable
		/// preview?" and "what's the current decompile target?" checks key off this.
		/// </summary>
		public bool IsStaticContent { get; set; }
	}
}
