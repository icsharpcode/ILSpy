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
using System.Composition;

using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Options;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Commands
{
	/// <summary>
	/// Opens (or focuses, if already open) the Options document tab. Mounted under the View
	/// menu mirroring the WPF host's `ShowOptionsCommand`. Ensures a single open instance —
	/// re-invocation just reactivates the existing tab.
	/// </summary>
	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._View), Header = nameof(Resources._Options), MenuCategory = nameof(Resources.Options), MenuOrder = 999)]
	[Shared]
	[method: ImportingConstructor]
	internal sealed class ShowOptionsCommand(
		DockWorkspace dockWorkspace,
		SettingsService settingsService,
		// "OptionPages" is the named MEF contract every ExportOptionPageAttribute publishes
		// under (see ExportOptionPageAttribute ctor). [ImportMany] alone resolves to the
		// default contract for IOptionPage and would come up empty.
		[ImportMany("OptionPages")] IEnumerable<ExportFactory<IOptionPage, IOptionsMetadata>> optionPages) : SimpleCommand
	{
		public override void Execute(object? parameter)
		{
			// Singleton: one retained Options tab for the session. Reopening after close reuses
			// the same ContentTabPage (and its OptionsPageModel, so the selected page survives).
			dockWorkspace.OpenSingletonTab("options",
				() => dockWorkspace.OpenNewTab(new OptionsPageModel(settingsService, optionPages)));
		}
	}
}
