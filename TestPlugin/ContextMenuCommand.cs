// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System.Composition;

using ICSharpCode.ILSpy;

namespace TestPlugin
{
	// Demonstrates contributing an entry to the tree's context menu. (The real "Save Assembly"
	// command ships with the app; the concrete assembly-node types are internal to ILSpy, so this
	// sample only shows the extension point and operates on the selected nodes generically.)
	[ExportContextMenuEntry(Header = "_Test Plugin Command")]
	[Shared]
	public class TestPluginContextCommand : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context)
		{
			return context.SelectedTreeNodes is { Length: > 0 };
		}

		public bool IsEnabled(TextViewContext context)
		{
			return context.SelectedTreeNodes is { Length: 1 };
		}

		public void Execute(TextViewContext context)
		{
			// A real plugin would act on context.SelectedTreeNodes here.
		}
	}
}
