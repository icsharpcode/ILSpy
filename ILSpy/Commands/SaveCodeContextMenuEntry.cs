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

using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpyX.TreeView;

using ILSpy.TreeNodes;

namespace ILSpy.Commands
{
	/// <summary>
	/// Right-click → "Save Code". Delegates to the selected node's
	/// <see cref="ILSpyTreeNode.Save"/> override so the context-menu entry and File →
	/// Save Code take exactly the same path: <c>AssemblyTreeNode</c> drives project /
	/// single-file selection, <c>ResourceTreeNode</c> writes raw bytes, every other
	/// node falls through to the generic single-file decompile in the existing
	/// <c>FileCommands.SaveCommand</c>.
	/// </summary>
	[ExportContextMenuEntry(Header = nameof(Resources._SaveCode), Category = nameof(Resources.Save))]
	[Shared]
	public sealed class SaveCodeContextMenuEntry : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context) => GetTarget(context) != null;

		public bool IsEnabled(TextViewContext context) => GetTarget(context) != null;

		public void Execute(TextViewContext context)
		{
			GetTarget(context)?.Save();
		}

		static ILSpyTreeNode? GetTarget(TextViewContext context)
		{
			if (context.SelectedTreeNodes is not { Length: 1 } nodes)
				return null;
			return nodes[0] as ILSpyTreeNode;
		}
	}
}
