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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace ICSharpCode.ILSpy
{
	/// <summary>
	/// Represents the filters applied to the tree view.
	/// </summary>
	/// <remarks>
	/// This class is mutable; but the ILSpyTreeNode filtering assumes that filter settings are immutable.
	/// Thus, the main window will use one mutable instance (for data-binding), and will assign a new
	/// clone to the ILSpyTreeNodes whenever the main mutable instance changes.
	/// </remarks>
	public class FilterSettings : INotifyPropertyChanged
	{
		public FilterSettings(XElement element)
		{
			this.ShowApiLevel = (ApiVisibility?)(int?)element.Element("ShowAPILevel") ?? ApiVisibility.PublicAndInternal;
			this.Language = Languages.GetLanguage((string)element.Element("Language"));
			this.LanguageVersion = Language.LanguageVersions.FirstOrDefault(v => v.Version == (string)element.Element("LanguageVersion"));
			if (this.LanguageVersion == default(LanguageVersion))
				this.LanguageVersion = language.LanguageVersions.LastOrDefault();
		}

		public XElement SaveAsXml()
		{
			return new XElement(
				"FilterSettings",
				new XElement("ShowAPILevel", (int)this.ShowApiLevel),
				new XElement("Language", this.Language.Name),
				new XElement("LanguageVersion", this.LanguageVersion?.Version)
			);
		}

		string searchTerm;

		/// <summary>
		/// Gets/Sets the search term.
		/// Only tree nodes containing the search term will be shown.
		/// </summary>
		public string SearchTerm {
			get { return searchTerm; }
			set {
				if (searchTerm != value)
				{
					searchTerm = value;
					OnPropertyChanged(nameof(SearchTerm));
				}
			}
		}

		/// <summary>
		/// Gets whether a node with the specified text is matched by the current search term.
		/// </summary>
		public bool SearchTermMatches(string text)
		{
			if (string.IsNullOrEmpty(searchTerm))
				return true;
			return text.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0;
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

		Language language;

		/// <summary>
		/// Gets/Sets the current language.
		/// </summary>
		/// <remarks>
		/// While this isn't related to filtering, having it as part of the FilterSettings
		/// makes it easy to pass it down into all tree nodes.
		/// </remarks>
		public Language Language {
			get { return language; }
			set {
				if (language != value)
				{
					language = value;
					OnPropertyChanged();
				}
			}
		}

		LanguageVersion languageVersion;

		/// <summary>
		/// Gets/Sets the current language version.
		/// </summary>
		/// <remarks>
		/// While this isn't related to filtering, having it as part of the FilterSettings
		/// makes it easy to pass it down into all tree nodes.
		/// </remarks>
		public LanguageVersion LanguageVersion {
			get { return languageVersion; }
			set {
				if (languageVersion != value)
				{
					languageVersion = value;
					OnPropertyChanged();
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public FilterSettings Clone()
		{
			FilterSettings f = (FilterSettings)MemberwiseClone();
			f.PropertyChanged = null;
			return f;
		}
	}

	public enum ApiVisibility
	{
		PublicOnly,
		PublicAndInternal,
		All
	}
}
