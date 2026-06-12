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

using System.Xml.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpy
{
	/// <summary>
	/// Holds the API visibility filter and the active output language id. The View menu
	/// binds checkmarks straight to the three Api* boolean projections of
	/// <see cref="ShowApiLevel"/>.
	/// </summary>
	public sealed partial class LanguageSettings : ObservableObject, IChildSettings
	{
		public LanguageSettings(XElement element, ISettingsSection parent)
		{
			Parent = parent;
			showApiLevel = (ApiVisibility?)(int?)element.Element("ShowAPILevel") ?? ApiVisibility.PublicAndInternal;
			languageId = (string?)element.Element("Language");
			languageVersionId = (string?)element.Element("LanguageVersion");
		}

		public ISettingsSection Parent { get; }

		public XElement SaveAsXml()
		{
			return new XElement(
				"FilterSettings",
				new XElement("ShowAPILevel", (int)ShowApiLevel),
				new XElement("Language", LanguageId),
				new XElement("LanguageVersion", LanguageVersionId));
		}

		ApiVisibility showApiLevel;

		public ApiVisibility ShowApiLevel {
			get => showApiLevel;
			set {
				if (SetProperty(ref showApiLevel, value))
				{
					OnPropertyChanged(nameof(ApiVisPublicOnly));
					OnPropertyChanged(nameof(ApiVisPublicAndInternal));
					OnPropertyChanged(nameof(ApiVisAll));
				}
			}
		}

		public bool ApiVisPublicOnly {
			get => showApiLevel == ApiVisibility.PublicOnly;
			set { if (value) ShowApiLevel = ApiVisibility.PublicOnly; }
		}

		public bool ApiVisPublicAndInternal {
			get => showApiLevel == ApiVisibility.PublicAndInternal;
			set { if (value) ShowApiLevel = ApiVisibility.PublicAndInternal; }
		}

		public bool ApiVisAll {
			get => showApiLevel == ApiVisibility.All;
			set { if (value) ShowApiLevel = ApiVisibility.All; }
		}

		[ObservableProperty]
		string? languageId;

		[ObservableProperty]
		string? languageVersionId;

		/// <summary>
		/// Active search-pane query. The assembly tree's <c>Filter</c> cascade calls
		/// <see cref="SearchTermMatches"/> on every visible row; setting this property
		/// triggers the cascade through <see cref="ObservableObject.PropertyChanged"/>.
		/// An empty string disables the filter (everything matches).
		/// </summary>
		[ObservableProperty]
		public partial string SearchTerm { get; set; } = string.Empty;

		/// <summary>
		/// Tree-filter no-op. The search pane runs against the loaded assemblies directly
		/// (see <see cref="ICSharpCode.ILSpyX.Search.MemberSearchStrategy"/> + friends);
		/// piping its term through the assembly-tree filter cascade would hide member rows
		/// whose names don't contain the term — even when their parent type *does* match —
		/// because the cascade only resets the "match" bit one level deep. The matcher
		/// stays as a parameter-taking shim so existing <c>FieldTreeNode.Filter</c> /
		/// <c>MethodTreeNode.Filter</c> / etc. overrides keep their structure (the
		/// <c>ShowApiLevel</c> + <c>ShowMember</c> checks they wrap are still meaningful).
		/// </summary>
		public bool SearchTermMatches(string value) => true;
	}
}
