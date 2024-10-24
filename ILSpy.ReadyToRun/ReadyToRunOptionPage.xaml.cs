// Copyright (c) 2018 Siegfried Pammer
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

using ICSharpCode.ILSpy.Options;
using ICSharpCode.ILSpy.Util;

using TomsToolbox.Wpf;
using TomsToolbox.Wpf.Composition.AttributedModel;

namespace ICSharpCode.ILSpy.ReadyToRun
{
	[DataTemplate(typeof(ReadyToRunOptionsViewModel))]
	[NonShared]
	partial class ReadyToRunOptionPage
	{
		public ReadyToRunOptionPage()
		{
			InitializeComponent();
		}
	}

	[ExportOptionPage(Order = 40)]
	[NonShared]
	class ReadyToRunOptionsViewModel : ObservableObject, IOptionPage
	{
		private ReadyToRunOptions options;

		public ReadyToRunOptions Options {
			get => options;
			set => SetProperty(ref options, value);
		}

		public string Title => global::ILSpy.ReadyToRun.Properties.Resources.ReadyToRun;

		public void Load(SettingsSnapshot snapshot)
		{
			Options = snapshot.GetSettings<ReadyToRunOptions>();
		}

		public void LoadDefaults()
		{
			Options.LoadFromXml(new("empty"));
		}
	}
}