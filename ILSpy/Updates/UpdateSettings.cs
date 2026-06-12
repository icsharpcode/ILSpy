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

using System;
using System.Xml.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpy.Updates
{
	/// <summary>
	/// Persisted user preferences for the auto-update checker. Default-on so the user
	/// gets notified about new releases without an explicit opt-in step. Schema matches
	/// WPF's <c>UpdateSettings</c> so saved settings round-trip across platforms.
	/// </summary>
	public sealed partial class UpdateSettings : ObservableObject, ISettingsSection
	{
		public XName SectionName => "UpdateSettings";

		[ObservableProperty]
		bool automaticUpdateCheckEnabled = true;

		[ObservableProperty]
		DateTime? lastSuccessfulUpdateCheck;

		public void LoadFromXml(XElement section)
		{
			AutomaticUpdateCheckEnabled = (bool?)section.Element(nameof(AutomaticUpdateCheckEnabled)) ?? true;
			try
			{
				LastSuccessfulUpdateCheck = (DateTime?)section.Element(nameof(LastSuccessfulUpdateCheck));
			}
			catch (FormatException)
			{
				// Avoid crashing on settings files with malformed DateTime values
				// (see ILSpy issue #2 for the original WPF-side bug this guard fixed).
			}
		}

		public XElement SaveToXml()
		{
			var section = new XElement(SectionName);
			section.Add(new XElement(nameof(AutomaticUpdateCheckEnabled), AutomaticUpdateCheckEnabled));
			if (LastSuccessfulUpdateCheck != null)
				section.Add(new XElement(nameof(LastSuccessfulUpdateCheck), LastSuccessfulUpdateCheck));
			return section;
		}
	}
}
