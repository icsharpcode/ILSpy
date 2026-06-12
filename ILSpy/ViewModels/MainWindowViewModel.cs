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

using Dock.Model.Controls;

using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Languages;

namespace ICSharpCode.ILSpy.ViewModels
{
	[Export]
	[Shared]
	public partial class MainWindowViewModel : ViewModelBase
	{
		public AssemblyTreeModel AssemblyTreeModel { get; }

		public LanguageService LanguageService { get; }

		public DockWorkspace DockWorkspace { get; }

		public LanguageSettings LanguageSettings { get; }

		public UpdatePanelViewModel UpdatePanel { get; }

		public IRootDock DockLayout => DockWorkspace.Layout;

		public System.Collections.Generic.IReadOnlyList<ToolPaneMenuItem> ToolPaneMenuItems => DockWorkspace.ToolPaneMenuItems;

		public string Title =>
#if DEBUG
			$"ILSpy {DecompilerVersionInfo.FullVersion}";
#else
			"ILSpy";
#endif

		[ImportingConstructor]
		public MainWindowViewModel(
			AssemblyTreeModel assemblyTreeModel,
			LanguageService languageService,
			DockWorkspace dockWorkspace,
			SettingsService settingsService,
			UpdatePanelViewModel updatePanel)
		{
			AppEnv.AppLog.Mark("MainWindowViewModel ctor entered (deps already resolved)");
			AssemblyTreeModel = assemblyTreeModel;
			LanguageService = languageService;
			DockWorkspace = dockWorkspace;
			LanguageSettings = settingsService.SessionSettings.LanguageSettings;
			UpdatePanel = updatePanel;
			// Auto-throttled background update check; respects user's
			// AutomaticUpdateCheckEnabled preference + 7-day cooldown. Fire-and-forget so
			// startup isn't blocked on the HTTP call. The panel stays hidden unless an
			// actual update is available.
			_ = updatePanel.CheckIfUpdatesAvailableAsync();
			AppEnv.AppLog.Mark("MainWindowViewModel ctor exited");
		}
	}
}
