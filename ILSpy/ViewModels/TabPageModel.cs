// Copyright (c) 2019 AlphaSierraPapa for the SharpDevelop Team
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
using System.Linq;
using System.Windows;

using ICSharpCode.ILSpy.TextView;

using TomsToolbox.Composition;
using TomsToolbox.Wpf;

#nullable enable

namespace ICSharpCode.ILSpy.ViewModels
{
	[Export]
	[NonShared]
#if CROSS_PLATFORM
	public class TabPageModel : Dock.Model.TomsToolbox.Controls.Document
	{
		protected static DockWorkspace DockWorkspace => App.ExportProvider.GetExportedValue<DockWorkspace>();
#else
	public class TabPageModel : PaneModel
	{
#endif
		public IExportProvider ExportProvider { get; }

		public TabPageModel(IExportProvider exportProvider)
		{
			ExportProvider = exportProvider;
			Title = Properties.Resources.NewTab;
		}

		private bool supportsLanguageSwitching = true;

		public bool SupportsLanguageSwitching {
			get => supportsLanguageSwitching;
			set => SetProperty(ref supportsLanguageSwitching, value);
		}

		private bool frozenContent;

		public bool FrozenContent {
			get => frozenContent;
			set => SetProperty(ref frozenContent, value);
		}

		private object? content;

		public object? Content {
			get => content;
			set => SetProperty(ref content, value);
		}

		public ViewState? GetState()
		{
			return (Content as IHaveState)?.GetState();
		}
	}

	public interface IHaveState
	{
		ViewState? GetState();
	}
}