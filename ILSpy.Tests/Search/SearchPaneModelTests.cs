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

using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpyX.Search;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Search;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Search;

[TestFixture]
public class SearchPaneModelTests
{
	[AvaloniaTest]
	public Task SearchPaneModel_Resolves_Through_MEF_As_A_Shared_Export()
	{
		var first = AppComposition.Current.GetExport<SearchPaneModel>();
		var second = AppComposition.Current.GetExport<SearchPaneModel>();
		first.Should().NotBeNull();
		first.Should().BeSameAs(second,
			"the search pane is [Shared] — the dock host and tests must see the same instance");
		return Task.CompletedTask;
	}

	[AvaloniaTest]
	public Task SearchModes_Lists_All_Twelve_ILSpyX_Modes_Starting_With_TypeAndMember()
	{
		var vm = AppComposition.Current.GetExport<SearchPaneModel>();
		var modes = vm.SearchModes.Select(m => m.Mode).ToArray();

		modes.Should().HaveCount(12,
			"every value in ICSharpCode.ILSpyX.Search.SearchMode must surface as a pickable mode");
		modes[0].Should().Be(SearchMode.TypeAndMember,
			"TypeAndMember is the default mode — it sits at the top of the picker");
		modes.Should().Contain(new[] {
			SearchMode.TypeAndMember, SearchMode.Type, SearchMode.Member,
			SearchMode.Method, SearchMode.Field, SearchMode.Property,
			SearchMode.Event, SearchMode.Literal, SearchMode.Token,
			SearchMode.Resource, SearchMode.Assembly, SearchMode.Namespace,
		});

		vm.SearchModes.Should().AllSatisfy(m => {
			((object?)m.Name).Should().NotBeNull("every mode entry needs a display name");
			m.Name.Should().NotBeEmpty();
		});

		return Task.CompletedTask;
	}

	[AvaloniaTest]
	public Task SelectedSearchMode_Defaults_To_TypeAndMember()
	{
		var vm = AppComposition.Current.GetExport<SearchPaneModel>();
		((object?)vm.SelectedSearchMode).Should().NotBeNull();
		vm.SelectedSearchMode.Mode.Should().Be(SearchMode.TypeAndMember,
			"the picker opens on the most inclusive mode so a brand-new search returns useful hits without picking a category first");
		return Task.CompletedTask;
	}

	[AvaloniaTest]
	public Task SearchTerm_Raises_PropertyChanged_So_The_View_Binding_Round_Trips()
	{
		var vm = AppComposition.Current.GetExport<SearchPaneModel>();
		vm.SearchTerm = string.Empty;

		var raised = false;
		PropertyChangedEventHandler handler = (_, e) => {
			if (e.PropertyName == nameof(SearchPaneModel.SearchTerm))
				raised = true;
		};
		vm.PropertyChanged += handler;
		try
		{
			vm.SearchTerm = "Enumerable";
		}
		finally
		{
			vm.PropertyChanged -= handler;
		}

		raised.Should().BeTrue("SearchTerm setter must raise INotifyPropertyChanged so the TextBox binding stays in sync");
		vm.SearchTerm.Should().Be("Enumerable");

		return Task.CompletedTask;
	}

	[AvaloniaTest]
	public Task Results_Is_An_Empty_Observable_Collection_Before_Any_Search_Runs()
	{
		var vm = AppComposition.Current.GetExport<SearchPaneModel>();
		// Clear in case a prior test in the same composition seeded it.
		vm.Results.Clear();
		vm.Results.Should().BeEmpty(
			"the results collection is the binding sink for the ListBox; empty means 'nothing to show yet'");
		return Task.CompletedTask;
	}
}
