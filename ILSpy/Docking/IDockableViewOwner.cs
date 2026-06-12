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

using Avalonia.Controls;

namespace ICSharpCode.ILSpy.Docking
{
	/// <summary>
	/// A dockable view-model that owns its realised view instance. The dock chrome must render
	/// one view per dockable identity (not one per dock slot) or two dockables sharing a slot
	/// render each other's content; <see cref="DockableViewRecycling"/> satisfies that by handing
	/// back the owner's <see cref="OwnedView"/> instead of pinning views in a global cache. The
	/// view then lives and dies with the dockable: singleton dockables (tool panes, the Options /
	/// About pages) keep one view for the app's life, and transient document tabs release theirs
	/// for GC the moment the tab is dropped.
	/// </summary>
	public interface IDockableViewOwner
	{
		/// <summary>
		/// The realised view for this dockable, created lazily by <see cref="DockableViewRecycling"/>
		/// on first resolution and reused for every subsequent resolution (tab switch, drag to a new
		/// slot). Null until first realised.
		/// </summary>
		Control? OwnedView { get; set; }
	}
}
