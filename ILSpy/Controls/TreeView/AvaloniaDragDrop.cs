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

using Avalonia.Input;

using ICSharpCode.ILSpyX.TreeView.PlatformAbstractions;

namespace ICSharpCode.ILSpy.Controls.TreeView
{
	/// <summary>
	/// In-process <see cref="IPlatformDataObject"/> -- a simple format-to-value map. A tree node's
	/// <c>Copy</c> builds one to describe a drag; <see cref="SharpTreeView"/> builds one wrapping an
	/// incoming external file list so the same node <c>Drop</c> code handles both.
	/// </summary>
	public sealed class AvaloniaDataObject : IPlatformDataObject
	{
		readonly Dictionary<string, object> data = new();

		public bool GetDataPresent(string format) => data.ContainsKey(format);
		public object GetData(string format) => data.TryGetValue(format, out var value) ? value : null!;
		public void SetData(string format, object value) => data[format] = value;
		public object UnderlyingDataObject => this;
	}

	/// <summary>
	/// Carries a drop's data + effect across the cross-platform <see cref="IPlatformDragEventArgs"/>
	/// contract that <c>SharpTreeNode.CanDrop/Drop</c> consume. <see cref="SharpTreeView"/> fills in
	/// <see cref="Data"/>, calls the node, then reads <see cref="Effects"/> back to set the cursor.
	/// </summary>
	public sealed class AvaloniaPlatformDragEventArgs : IPlatformDragEventArgs
	{
		public AvaloniaPlatformDragEventArgs(IPlatformDataObject data)
		{
			Data = data;
		}

		public XPlatDragDropEffects Effects { get; set; }
		public IPlatformDataObject Data { get; }
	}

	static class DragDropEffectsExtensions
	{
		public static DragDropEffects ToAvalonia(this XPlatDragDropEffects e)
		{
			var result = DragDropEffects.None;
			if ((e & XPlatDragDropEffects.Copy) != 0)
				result |= DragDropEffects.Copy;
			if ((e & XPlatDragDropEffects.Move) != 0)
				result |= DragDropEffects.Move;
			if ((e & XPlatDragDropEffects.Link) != 0)
				result |= DragDropEffects.Link;
			return result;
		}
	}
}
