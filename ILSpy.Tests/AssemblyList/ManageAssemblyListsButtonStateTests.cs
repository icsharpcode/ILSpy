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

using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.Threading;

using AwesomeAssertions;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// Manage Assembly Lists: the buttons that act on the selected list (Clone / Rename / Delete /
/// Select) must be disabled while nothing is selected; the selection-independent ones
/// (New / Reset / Add preconfigured / Close) stay enabled.
/// </summary>
[TestFixture]
public class ManageAssemblyListsButtonStateTests
{
	static readonly string[] SelectionDependent = ["CloneButton", "RenameButton", "DeleteButton", "SelectButton"];
	static readonly string[] SelectionIndependent = ["NewButton", "ResetButton", "AddPreconfiguredButton", "CloseButton"];

	[AvaloniaTest]
	public void Selection_Dependent_Buttons_Are_Disabled_Until_A_List_Is_Selected()
	{
		var settingsService = AppComposition.Current.GetExport<SettingsService>();
		var manager = settingsService.AssemblyListManager;
		if (manager.AssemblyLists.Count == 0)
			manager.AddListIfNotExists(manager.CreateList("List " + Guid.NewGuid().ToString("N")));

		var dialog = new ManageAssemblyListsDialog(settingsService);
		dialog.Show();
		try
		{
			dialog.ListsControl.SelectedItem = null;
			Dispatcher.UIThread.RunJobs();

			foreach (var name in SelectionDependent)
				dialog.FindControl<Button>(name)!.IsEnabled.Should().BeFalse(
					$"{name} acts on the selected list, so it must be disabled when nothing is selected");

			foreach (var name in SelectionIndependent)
				dialog.FindControl<Button>(name)!.IsEnabled.Should().BeTrue(
					$"{name} does not depend on a selection and stays enabled");

			// Selecting a list enables the selection-dependent buttons.
			dialog.ListsControl.SelectedIndex = 0;
			Dispatcher.UIThread.RunJobs();

			foreach (var name in SelectionDependent)
				dialog.FindControl<Button>(name)!.IsEnabled.Should().BeTrue(
					$"{name} must be enabled once a list is selected");
		}
		finally
		{
			dialog.Close();
		}
	}
}
