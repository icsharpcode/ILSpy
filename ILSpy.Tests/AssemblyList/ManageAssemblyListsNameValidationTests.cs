// Copyright (c) 2026 Siegfried Pammer
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
/// Manage Assembly Lists: a new list may not take the name of an existing one. The prompt has
/// to say so and keep OK disabled - the operations behind it silently do nothing for a name
/// that is taken, which reads as the dialog having accepted the name.
/// </summary>
[TestFixture]
public class ManageAssemblyListsNameValidationTests
{
	static (ManageAssemblyListsDialog Dialog, string TakenName) DialogWithOneList()
	{
		var settingsService = AppComposition.Current.GetExport<SettingsService>();
		var manager = settingsService.AssemblyListManager;
		var taken = "List " + Guid.NewGuid().ToString("N");
		manager.AddListIfNotExists(manager.CreateList(taken));
		return (new ManageAssemblyListsDialog(settingsService), taken);
	}

	static (TextBox Name, Button Ok, TextBlock Message) Controls(CreateListDialog prompt)
		=> (prompt.FindControl<TextBox>("ListNameBox")!,
			prompt.FindControl<Button>("OkButton")!,
			prompt.FindControl<TextBlock>("NameTakenText")!);

	[AvaloniaTest]
	public void Prompt_Rejects_The_Name_Of_An_Existing_List()
	{
		var (dialog, taken) = DialogWithOneList();
		var prompt = dialog.CreatePrompt("New Assembly List");
		prompt.Show();
		try
		{
			var (name, ok, message) = Controls(prompt);

			name.Text = taken;
			Dispatcher.UIThread.RunJobs();
			ok.IsEnabled.Should().BeFalse("the name is already in use");
			message.IsVisible.Should().BeTrue("the user has to be told why OK is disabled");

			name.Text = taken + " (2)";
			Dispatcher.UIThread.RunJobs();
			ok.IsEnabled.Should().BeTrue("the name is free");
			message.IsVisible.Should().BeFalse();
		}
		finally
		{
			prompt.Close();
			dialog.Close();
		}
	}

	[AvaloniaTest]
	public void Rename_Accepts_The_Name_The_List_Already_Has()
	{
		var (dialog, taken) = DialogWithOneList();
		// Renaming a list to its own name is a no-op, not a collision with itself.
		var prompt = dialog.CreatePrompt("Rename Assembly List", taken, allowedName: taken);
		prompt.Show();
		try
		{
			var (_, ok, message) = Controls(prompt);
			Dispatcher.UIThread.RunJobs();
			ok.IsEnabled.Should().BeTrue();
			message.IsVisible.Should().BeFalse();
		}
		finally
		{
			prompt.Close();
			dialog.Close();
		}
	}
}
