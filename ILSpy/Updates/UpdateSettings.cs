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
using System.ComponentModel;
using System.Xml.Linq;

using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpy.Updates
{
	sealed class UpdateSettings : INotifyPropertyChanged
	{
		public UpdateSettings(ILSpySettings spySettings)
		{
			XElement s = spySettings["UpdateSettings"];
			this.automaticUpdateCheckEnabled = (bool?)s.Element("AutomaticUpdateCheckEnabled") ?? true;
			try
			{
				this.lastSuccessfulUpdateCheck = (DateTime?)s.Element("LastSuccessfulUpdateCheck");
			}
			catch (FormatException)
			{
				// avoid crashing on settings files invalid due to
				// https://github.com/icsharpcode/ILSpy/issues/closed/#issue/2
			}
		}

		bool automaticUpdateCheckEnabled;

		public bool AutomaticUpdateCheckEnabled {
			get { return automaticUpdateCheckEnabled; }
			set {
				if (automaticUpdateCheckEnabled != value)
				{
					automaticUpdateCheckEnabled = value;
					Save();
					OnPropertyChanged(nameof(AutomaticUpdateCheckEnabled));
				}
			}
		}

		DateTime? lastSuccessfulUpdateCheck;

		public DateTime? LastSuccessfulUpdateCheck {
			get { return lastSuccessfulUpdateCheck; }
			set {
				if (lastSuccessfulUpdateCheck != value)
				{
					lastSuccessfulUpdateCheck = value;
					Save();
					OnPropertyChanged(nameof(LastSuccessfulUpdateCheck));
				}
			}
		}

		public void Save()
		{
			XElement updateSettings = new XElement("UpdateSettings");
			updateSettings.Add(new XElement("AutomaticUpdateCheckEnabled", automaticUpdateCheckEnabled));
			if (lastSuccessfulUpdateCheck != null)
				updateSettings.Add(new XElement("LastSuccessfulUpdateCheck", lastSuccessfulUpdateCheck));
			ILSpySettings.SaveSettings(updateSettings);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

}
