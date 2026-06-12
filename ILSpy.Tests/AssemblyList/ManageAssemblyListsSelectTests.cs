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

using System.Linq;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// Selecting a list in Manage Assembly Lists (the Select button and the double-click shortcut both
/// call the same activation path) must switch the LIVE active list, not merely persist it for the
/// next launch — i.e. it must drive AssemblyTreeModel.ActiveListName, which reloads the tree.
/// </summary>
[TestFixture]
public class ManageAssemblyListsSelectTests
{
	[AvaloniaTest]
	public async Task Selecting_A_List_Switches_The_Live_Active_List()
	{
		var (_, vm) = await TestHarness.BootAsync();
		var settingsService = AppComposition.Current.GetExport<SettingsService>();
		var manager = settingsService.AssemblyListManager;

		var current = vm.AssemblyTreeModel.ActiveListName;

		// Ensure there is a second, empty list to switch to (the headless fixture seeds one).
		var other = "Switch Target " + System.Guid.NewGuid().ToString("N");
		manager.AddListIfNotExists(manager.CreateList(other));
		other.Should().NotBe(current);

		var dialog = new ManageAssemblyListsDialog(settingsService);
		dialog.ListsControl.SelectedItem = other;
		dialog.ActivateSelectedList();

		vm.AssemblyTreeModel.ActiveListName.Should().Be(other,
			"selecting a list must switch the live active list (via AssemblyTreeModel.ActiveListName), not just the persisted setting");
		settingsService.SessionSettings.ActiveAssemblyList.Should().Be(other,
			"and the choice is persisted as a side effect of the model switch");
	}
}
