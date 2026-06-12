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

using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Languages;

namespace ICSharpCode.ILSpy.Commands
{
	/// <summary>
	/// Right-click one or more assemblies -> "Export Project/Solution...". Opens the configuration
	/// dialog (project mode for a single assembly, solution mode for several). Sits alongside the
	/// quick "Save Code" entry, which keeps its no-dialog behaviour.
	/// </summary>
	[ExportContextMenuEntry(Header = nameof(Resources.ExportProjectSolution), Category = nameof(Resources.Save), Icon = "Images/Save", Order = 301)]
	[Shared]
	public sealed class ExportProjectContextMenuEntry : IContextMenuEntry
	{
		readonly LanguageService languageService;
		readonly DockWorkspace dockWorkspace;
		readonly SettingsService settingsService;

		[ImportingConstructor]
		public ExportProjectContextMenuEntry(LanguageService languageService,
			DockWorkspace dockWorkspace, SettingsService settingsService)
		{
			this.languageService = languageService;
			this.dockWorkspace = dockWorkspace;
			this.settingsService = settingsService;
		}

		public bool IsVisible(TextViewContext context)
			=> ProjectExport.TryGetExportableAssemblies(context.SelectedTreeNodes, out _, out _);

		public bool IsEnabled(TextViewContext context)
			=> ProjectExport.TryGetExportableAssemblies(context.SelectedTreeNodes, out _, out _);

		public void Execute(TextViewContext context)
		{
			if (!ProjectExport.TryGetExportableAssemblies(context.SelectedTreeNodes, out var assemblies, out var solutionMode))
				return;
			ProjectExport.PromptAndExportAsync(assemblies, solutionMode,
				languageService.CurrentLanguage, dockWorkspace, settingsService).HandleExceptions();
		}
	}
}
