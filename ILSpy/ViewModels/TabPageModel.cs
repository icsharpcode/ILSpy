﻿// Copyright (c) 2019 AlphaSierraPapa for the SharpDevelop Team
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
using System.Threading.Tasks;

using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.Util;
using ICSharpCode.ILSpyX;

namespace ICSharpCode.ILSpy.ViewModels
{
	public class TabPageModel : PaneModel
	{
		public TabPageModel()
		{
			this.Title = Properties.Resources.NewTab;
		}

		private bool supportsLanguageSwitching = true;

		public bool SupportsLanguageSwitching {
			get => supportsLanguageSwitching;
			set {
				if (supportsLanguageSwitching != value)
				{
					supportsLanguageSwitching = value;
					OnPropertyChanged(nameof(SupportsLanguageSwitching));
				}
			}
		}

		private object content;

		public object Content {
			get => content;
			set {
				if (content != value)
				{
					content = value;
					OnPropertyChanged(nameof(Content));
				}
			}
		}

		public ViewState GetState()
		{
			return (Content as IHaveState)?.GetState();
		}
	}

	public static class TabPageModelExtensions
	{
		public static Task<T> ShowTextViewAsync<T>(this TabPageModel tabPage, Func<DecompilerTextView, Task<T>> action)
		{
			if (!(tabPage.Content is DecompilerTextView textView))
			{
				textView = new DecompilerTextView();
				tabPage.Content = textView;
			}
			tabPage.Title = Properties.Resources.Decompiling;
			return action(textView);
		}

		public static Task ShowTextViewAsync(this TabPageModel tabPage, Func<DecompilerTextView, Task> action)
		{
			if (!(tabPage.Content is DecompilerTextView textView))
			{
				textView = new DecompilerTextView();
				tabPage.Content = textView;
			}
			tabPage.Title = Properties.Resources.Decompiling;
			return action(textView);
		}

		public static void ShowTextView(this TabPageModel tabPage, Action<DecompilerTextView> action)
		{
			if (!(tabPage.Content is DecompilerTextView textView))
			{
				textView = new DecompilerTextView();
				tabPage.Content = textView;
			}
			tabPage.Title = Properties.Resources.Decompiling;
			action(textView);
		}
	}

	public interface IHaveState
	{
		ViewState GetState();
	}
}