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

using System;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using ICSharpCode.Decompiler;
using ICSharpCode.ILSpy.TextView;

using TomsToolbox.Composition;
using TomsToolbox.Wpf;

#nullable enable

namespace ICSharpCode.ILSpy.ViewModels
{
	[Export]
	[NonShared]
	public class TabPageModel : PaneModel
	{
		public IExportProvider ExportProvider { get; }

		public TabPageModel(IExportProvider exportProvider)
		{
			ExportProvider = exportProvider;
			Title = Properties.Resources.NewTab;
		}

		private bool supportsLanguageSwitching = true;

		public bool SupportsLanguageSwitching {
			get => supportsLanguageSwitching;
			set => SetProperty(ref supportsLanguageSwitching, value);
		}

		private object? content;

		public object? Content {
			get => content;
			set => SetProperty(ref content, value);
		}

		public ViewState? GetState()
		{
			return (Content as IHaveState)?.GetState();
		}
	}

	public static class TabPageModelExtensions
	{
		public static Task<T> ShowTextViewAsync<T>(this TabPageModel tabPage, Func<DecompilerTextView, Task<T>> action)
		{
			if (tabPage.Content is not DecompilerTextView textView)
			{
				textView = new DecompilerTextView(tabPage.ExportProvider);
				tabPage.Content = textView;
			}
			tabPage.Title = Properties.Resources.Decompiling;
			return action(textView);
		}

		public static Task ShowTextViewAsync(this TabPageModel tabPage, Func<DecompilerTextView, Task> action)
		{
			if (tabPage.Content is not DecompilerTextView textView)
			{
				textView = new DecompilerTextView(tabPage.ExportProvider);
				tabPage.Content = textView;
			}
			string oldTitle = tabPage.Title;
			tabPage.Title = Properties.Resources.Decompiling;
			try
			{
				return action(textView);
			}
			finally
			{
				if (tabPage.Title == Properties.Resources.Decompiling)
				{
					tabPage.Title = oldTitle;
				}
			}
		}

		public static void ShowTextView(this TabPageModel tabPage, Action<DecompilerTextView> action)
		{
			if (tabPage.Content is not DecompilerTextView textView)
			{
				textView = new DecompilerTextView(tabPage.ExportProvider);
				tabPage.Content = textView;
			}
			string oldTitle = tabPage.Title;
			tabPage.Title = Properties.Resources.Decompiling;
			action(textView);
			if (tabPage.Title == Properties.Resources.Decompiling)
			{
				tabPage.Title = oldTitle;
			}
		}

		public static void Focus(this TabPageModel tabPage)
		{
			if (tabPage.Content is not FrameworkElement content)
				return;

			var focusable = content
				.VisualDescendantsAndSelf()
				.OfType<FrameworkElement>()
				.FirstOrDefault(item => item.Focusable);

			focusable?.Focus();
		}

		public static DecompilationOptions CreateDecompilationOptions(this TabPageModel tabPage)
		{
			var exportProvider = tabPage.ExportProvider;
			var languageService = exportProvider.GetExportedValue<LanguageService>();
			var settingsService = exportProvider.GetExportedValue<SettingsService>();

			return new(languageService.LanguageVersion, settingsService.DecompilerSettings, settingsService.DisplaySettings) { Progress = tabPage.Content as IProgress<DecompilationProgress> };
		}
	}

	public interface IHaveState
	{
		ViewState? GetState();
	}
}