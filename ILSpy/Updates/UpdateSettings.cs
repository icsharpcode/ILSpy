// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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

using System;
using System.Xml.Linq;

using TomsToolbox.Wpf;

namespace ICSharpCode.ILSpy.Updates
{
	public sealed class UpdateSettings : ObservableObjectBase, ISettingsSection
	{
		public XName SectionName => "UpdateSettings";

		bool automaticUpdateCheckEnabled;

		public bool AutomaticUpdateCheckEnabled {
			get => automaticUpdateCheckEnabled;
			set => SetProperty(ref automaticUpdateCheckEnabled, value);
		}

		DateTime? lastSuccessfulUpdateCheck;

		public DateTime? LastSuccessfulUpdateCheck {
			get => lastSuccessfulUpdateCheck;
			set => SetProperty(ref lastSuccessfulUpdateCheck, value);
		}

		public void LoadFromXml(XElement section)
		{
			AutomaticUpdateCheckEnabled = (bool?)section.Element("AutomaticUpdateCheckEnabled") ?? true;
			try
			{
				LastSuccessfulUpdateCheck = (DateTime?)section.Element("LastSuccessfulUpdateCheck");
			}
			catch (FormatException)
			{
				// avoid crashing on settings files invalid due to
				// https://github.com/icsharpcode/ILSpy/issues/closed/#issue/2
			}
		}

		public XElement SaveToXml()
		{
			var section = new XElement(SectionName);

			section.Add(new XElement("AutomaticUpdateCheckEnabled", AutomaticUpdateCheckEnabled));

			if (LastSuccessfulUpdateCheck != null)
			{
				section.Add(new XElement("LastSuccessfulUpdateCheck", LastSuccessfulUpdateCheck));
			}

			return section;
		}
	}
}
