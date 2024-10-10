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

using System.Collections.Generic;
using System.Xml.Linq;

using ICSharpCode.ILSpyX;

using TomsToolbox.Wpf;

#nullable enable

namespace ICSharpCode.ILSpy
{
	/// <summary>
	/// Represents the filters applied to the tree view.
	/// </summary>
	public class LanguageSettings : ObservableObject, IChildSettings
	{
		/// <summary>
		/// This dictionary is necessary to remember language versions across language changes. For example, 
		/// the user first select C# 10, then switches to IL, then switches back to C#. After that we must be
		/// able to restore the original selection (i.e., C# 10).
		/// </summary>
		private readonly Dictionary<Language, LanguageVersion> languageVersionHistory = new Dictionary<Language, LanguageVersion>();

		public LanguageSettings(XElement element, ISettingsSection parent)
		{
			Parent = parent;
			this.ShowApiLevel = (ApiVisibility?)(int?)element.Element("ShowAPILevel") ?? ApiVisibility.PublicAndInternal;
			this.LanguageId = (string?)element.Element("Language");
			this.LanguageVersionId = (string?)element.Element("LanguageVersion");
		}

		public ISettingsSection Parent { get; }

		public XElement SaveAsXml()
		{
			return new XElement(
				"FilterSettings",
				new XElement("ShowAPILevel", (int)this.ShowApiLevel),
				new XElement("Language", this.LanguageId),
				new XElement("LanguageVersion", this.LanguageVersionId)
			);
		}

		ApiVisibility showApiLevel;

		/// <summary>
		/// Gets/Sets whether public, internal or all API members should be shown.
		/// </summary>
		public ApiVisibility ShowApiLevel {
			get { return showApiLevel; }
			set {
				if (showApiLevel != value)
				{
					showApiLevel = value;
					OnPropertyChanged(nameof(ShowApiLevel));
				}
			}
		}

		public bool ApiVisPublicOnly {
			get { return showApiLevel == ApiVisibility.PublicOnly; }
			set {
				if (value == (showApiLevel == ApiVisibility.PublicOnly))
					return;
				ShowApiLevel = ApiVisibility.PublicOnly;
				OnPropertyChanged(nameof(ApiVisPublicOnly));
				OnPropertyChanged(nameof(ApiVisPublicAndInternal));
				OnPropertyChanged(nameof(ApiVisAll));
			}
		}

		public bool ApiVisPublicAndInternal {
			get { return showApiLevel == ApiVisibility.PublicAndInternal; }
			set {
				if (value == (showApiLevel == ApiVisibility.PublicAndInternal))
					return;
				ShowApiLevel = ApiVisibility.PublicAndInternal;
				OnPropertyChanged(nameof(ApiVisPublicOnly));
				OnPropertyChanged(nameof(ApiVisPublicAndInternal));
				OnPropertyChanged(nameof(ApiVisAll));
			}
		}

		public bool ApiVisAll {
			get { return showApiLevel == ApiVisibility.All; }
			set {
				if (value == (showApiLevel == ApiVisibility.All))
					return;
				ShowApiLevel = ApiVisibility.All;
				OnPropertyChanged(nameof(ApiVisPublicOnly));
				OnPropertyChanged(nameof(ApiVisPublicAndInternal));
				OnPropertyChanged(nameof(ApiVisAll));
			}
		}

		string? languageId;

		/// <summary>
		/// Gets/Sets the current language.
		/// </summary>
		/// <remarks>
		/// While this isn't related to filtering, having it as part of the FilterSettings
		/// makes it easy to pass it down into all tree nodes.
		/// </remarks>
		public string? LanguageId {
			get => languageId;
			set => SetProperty(ref languageId, value);
		}

		string? languageVersionId;

		/// <summary>
		/// Gets/Sets the current language version.
		/// </summary>
		/// <remarks>
		/// While this isn't related to filtering, having it as part of the FilterSettings
		/// makes it easy to pass it down into all tree nodes.
		/// </remarks>
		public string? LanguageVersionId {
			get { return languageVersionId; }
			set => SetProperty(ref languageVersionId, value);
		}

		// This class has been initially called FilterSettings, but then has been Hijacked to store language settings as well.
		// While the filter settings were some sort of local, the language settings are global. This is a bit of a mess.
		// There has been a lot of workarounds cloning the FilterSettings to pass them down to the tree nodes, without messing up the global language settings.
		// Finally, this filtering was not used at all, so this SearchTerm is just a placeholder to make the filtering code compile, in case someone wants to reactivate filtering in the future.
		public string SearchTerm => string.Empty;

		public bool SearchTermMatches(string value)
		{
			return true;
		}
	}
}
