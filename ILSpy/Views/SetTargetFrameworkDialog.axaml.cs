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

using System.Collections.Generic;
using System.Linq;

using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ICSharpCode.ILSpy.Views
{
	/// <summary>
	/// Prompts for a target-framework moniker used to override how an assembly's references are
	/// resolved. A text box accepts the short TFMs users know (net48, net6.0, ...) for free-form
	/// entry, with the common ones offered in an adjacent list to pick from; input is
	/// validated/converted to the long FrameworkName form via <see cref="TargetFrameworkConverter"/>.
	/// Leaving it blank clears the override.
	/// </summary>
	public partial class SetTargetFrameworkDialog : Window
	{
		// Suggestions only; any valid TFM the user types is accepted.
		static readonly IReadOnlyList<string> CommonFrameworks = new[] {
			"net11.0", "net10.0", "net9.0", "net8.0", "net7.0", "net6.0", "net5.0",
			"netcoreapp3.1", "netcoreapp3.0", "netcoreapp2.1",
			"netstandard2.1", "netstandard2.0",
			"net48", "net472", "net471", "net47", "net462", "net461", "net46",
		};

		TextBox frameworkBox = null!;
		ListBox presetList = null!;
		TextBlock errorText = null!;

		public SetTargetFrameworkDialog()
		{
			InitializeComponent();
			frameworkBox = this.FindControl<TextBox>("FrameworkBox")!;
			presetList = this.FindControl<ListBox>("PresetList")!;
			errorText = this.FindControl<TextBlock>("ErrorText")!;
			this.FindControl<TextBlock>("PromptText")!.Text = Properties.Resources.TargetFramework;
			presetList.ItemsSource = CommonFrameworks;
			// Picking a preset fills the box; typing stays free-form, so any TFM the user knows is
			// accepted even when it isn't in the list.
			presetList.SelectionChanged += (_, _) => {
				if (presetList.SelectedItem is string preset)
					frameworkBox.Text = preset;
			};
			// Double-clicking a preset is "choose and confirm", like committing a combo selection.
			presetList.DoubleTapped += (_, _) => {
				if (presetList.SelectedItem is string)
					OnOk();
			};
			((Button)this.FindControl<Button>("OkButton")!).Click += (_, _) => OnOk();
			((Button)this.FindControl<Button>("CancelButton")!).Click += (_, _) => Close(null);
		}

		public SetTargetFrameworkDialog(string? currentFrameworkName) : this()
		{
			Title = Properties.Resources.SetTargetFramework;
			var shortName = TargetFrameworkConverter.ToShortFolderName(currentFrameworkName);
			if (!string.IsNullOrEmpty(shortName))
			{
				frameworkBox.Text = shortName;
				// Highlight the matching preset (no-op when the current value isn't one of them).
				presetList.SelectedItem = CommonFrameworks.FirstOrDefault(f => f == shortName);
			}
		}

		void OnOk()
		{
			var text = frameworkBox.Text;
			if (string.IsNullOrWhiteSpace(text))
			{
				// Blank clears the override (revert to auto-detection).
				Close(string.Empty);
				return;
			}
			if (!TargetFrameworkConverter.TryParseToFrameworkName(text, out var frameworkName, out var error))
			{
				errorText.Text = error;
				errorText.IsVisible = true;
				return;
			}
			Close(frameworkName);
		}

		void InitializeComponent() => AvaloniaXamlLoader.Load(this);
	}
}
