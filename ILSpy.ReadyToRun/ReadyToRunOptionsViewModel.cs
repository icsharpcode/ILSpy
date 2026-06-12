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
using System.Xml.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.Options;

namespace ICSharpCode.ILSpy.ReadyToRun
{
	[ExportOptionPage(Order = 40)]
	[Shared]
	internal sealed partial class ReadyToRunOptionsViewModel : ObservableObject, IOptionPage
	{
		[ObservableProperty]
		ReadyToRunOptions options = null!;

		public string Title => global::ILSpy.ReadyToRun.Properties.Resources.ReadyToRun;

		public void Load(SettingsService service)
		{
			Options = service.GetSettings<ReadyToRunOptions>();
		}

		public void LoadDefaults()
		{
			Options.LoadFromXml(new XElement("ReadyToRunOptions"));
		}
	}
}
