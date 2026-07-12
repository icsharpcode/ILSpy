// Copyright (c) 2024 Christoph Wille
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

namespace ICSharpCode.ILSpyX.TreeView.PlatformAbstractions
{
	//
	// Summary:
	//     Specifies the effects of a drag-and-drop operation.
	[Flags]
	public enum XPlatDragDropEffects
	{
		//
		// Summary:
		//     Scrolling is about to start or is currently occurring in the drop target.
		Scroll = int.MinValue,
		//
		// Summary:
		//     The data is copied, removed from the drag source, and scrolled in the drop target.
		All = -2147483645,
		//
		// Summary:
		//     The drop target does not accept the data.
		None = 0,
		//
		// Summary:
		//     The data is copied to the drop target.
		Copy = 1,
		//
		// Summary:
		//     The data from the drag source is moved to the drop target.
		Move = 2,
		//
		// Summary:
		//     The data from the drag source is linked to the drop target.
		Link = 4
	}
}
