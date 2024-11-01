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
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpyX.Search;

namespace ICSharpCode.ILSpy.Search
{
	public class SearchModeModel
	{
		public SearchMode Mode { get; init; }
		public string Name { get; init; }
		public ImageSource Image { get; init; }
	}

	[ExportToolPane]
	[Shared]
	public class SearchPaneModel : ToolPaneModel
	{
		public const string PaneContentId = "searchPane";

		private readonly SettingsService settingsService;
		private string searchTerm;

		public SearchPaneModel(SettingsService settingsService)
		{
			this.settingsService = settingsService;
			ContentId = PaneContentId;
			Title = Properties.Resources.SearchPane_Search;
			Icon = "Images/Search";
			ShortcutKey = new(Key.F, ModifierKeys.Control | ModifierKeys.Shift);
			IsCloseable = true;

			MessageBus<ShowSearchPageEventArgs>.Subscribers += (_, e) => {
				SearchTerm = e.SearchTerm;
				Show();
			};
		}

		public SearchModeModel[] SearchModes { get; } = [
			new() { Mode = SearchMode.TypeAndMember, Image = Images.Library, Name = "Types and Members" },
			new() { Mode = SearchMode.Type, Image = Images.Class, Name = "Type" },
			new() { Mode = SearchMode.Member, Image = Images.Property, Name = "Member" },
			new() { Mode = SearchMode.Method, Image = Images.Method, Name = "Method" },
			new() { Mode = SearchMode.Field, Image = Images.Field, Name = "Field" },
			new() { Mode = SearchMode.Property, Image = Images.Property, Name = "Property" },
			new() { Mode = SearchMode.Event, Image = Images.Event, Name = "Event" },
			new() { Mode = SearchMode.Literal, Image = Images.Literal, Name = "Constant" },
			new() { Mode = SearchMode.Token, Image = Images.Library, Name = "Metadata Token" },
			new() { Mode = SearchMode.Resource, Image = Images.Resource, Name = "Resource" },
			new() { Mode = SearchMode.Assembly, Image = Images.Assembly, Name = "Assembly" },
			new() { Mode = SearchMode.Namespace, Image = Images.Namespace, Name = "Namespace" }
		];

		public SessionSettings SessionSettings => settingsService.SessionSettings;

		public string SearchTerm {
			get => searchTerm;
			set => SetProperty(ref searchTerm, value);
		}
	}
}
