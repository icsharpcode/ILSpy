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

using System.Composition;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input.Platform;

using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Commands
{
	/// <summary>
	/// Right-click → "Copy fully qualified name". Visible whenever the selection is exactly
	/// one tree node that wraps a TypeSystem entity (Type/Method/Field/Property/Event).
	/// Copies the entity's <see cref="ICSharpCode.Decompiler.TypeSystem.IEntity.ReflectionName"/>
	/// — the language-independent identifier used by FindNodeByPath etc. — to the clipboard.
	/// </summary>
	[ExportContextMenuEntry(Header = nameof(Resources.CopyName), Category = "Edit", Icon = "Images/Copy", Order = 600)]
	[Shared]
	public sealed class CopyFullyQualifiedNameContextMenuEntry : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context) => GetMemberNodeFromContext(context) != null;

		public bool IsEnabled(TextViewContext context) => true;

		public void Execute(TextViewContext context)
		{
			var text = GetTextToCopy(context);
			if (text == null)
				return;
			// The clipboard is owned by the TopLevel that hosts the source visual; for the
			// assembly tree that's the main window. SetTextAsync runs on the UI thread and we
			// don't await — the menu item already returned by then.
			var topLevel = context.OriginalSource is Visual src ? TopLevel.GetTopLevel(src)
				: context.TreeGrid is { } grid ? TopLevel.GetTopLevel(grid)
				: null;
			topLevel?.Clipboard?.SetTextAsync(text);
		}

		/// <summary>Public for testing: returns the text the entry would write to the clipboard
		/// for the given context, or <see langword="null"/> when the entry isn't applicable.</summary>
		public string? GetTextToCopy(TextViewContext context)
			=> GetMemberNodeFromContext(context)?.Member?.ReflectionName;

		static IMemberTreeNode? GetMemberNodeFromContext(TextViewContext context)
			=> context.SelectedTreeNodes is { Length: 1 } nodes
				? nodes[0] as IMemberTreeNode
				: null;
	}
}
