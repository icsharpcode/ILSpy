// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System.Composition;

using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Commands;

namespace TestPlugin
{
	// ParentMenuID: menu into which the item is added
	// MenuIcon: optional, icon to use for the menu item. Embedded as an AvaloniaResource in this assembly.
	// Header: text on the menu item
	// MenuCategory: optional, used for grouping related menu items together. A separator is added between different groups.
	// MenuOrder: controls the order in which the items appear (items are sorted by this value)
	[ExportMainMenuCommand(ParentMenuID = "_File", MenuIcon = "Clear.png", Header = "_Clear List", MenuCategory = "Open", MenuOrder = 1.5)]
	// ToolTip: the tool tip
	// ToolbarIcon: the icon, embedded as an AvaloniaResource in this assembly.
	// ToolbarCategory: optional, used for grouping related toolbar items together. A separator is added between different groups.
	// ToolbarOrder: controls the order in which the items appear (items are sorted by this value)
	[ExportToolbarCommand(ToolTip = "Clears the current assembly list", ToolbarIcon = "Clear.png", ToolbarCategory = "Open", ToolbarOrder = 1.5)]
	[Shared]
	// System.Composition needs the importing constructor marked explicitly; without this the menu/
	// toolbar builder can't instantiate the command and the whole menu build fails.
	[method: ImportingConstructor]
	public class UnloadAllAssembliesCommand(AssemblyTreeModel assemblyTreeModel) : SimpleCommand
	{
		public override void Execute(object? parameter)
		{
			if (assemblyTreeModel.AssemblyList is not { } assemblyList)
				return;
			foreach (var loadedAssembly in assemblyList.GetAssemblies())
			{
				loadedAssembly.AssemblyList.Unload(loadedAssembly);
			}
		}
	}
}
