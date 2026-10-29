// Copyright (c) 2026 Piero Viano
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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace ICSharpCode.ILSpy.FileStructure
{
	/// <summary>
	/// Content of the Go to File Member popup: a filter box over the member list. The filter box
	/// keeps the focus; Up/Down move the selection, Enter accepts, Escape cancels, and a click on
	/// a row accepts it.
	/// </summary>
	public partial class GoToFileMemberView : UserControl
	{
		public GoToFileMemberView()
		{
			InitializeComponent();
			AddHandler(KeyDownEvent, OnKeyDownTunnel, RoutingStrategies.Tunnel);
			ResultList.Tapped += OnResultTapped;
		}

		protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
		{
			base.OnAttachedToVisualTree(e);
			Dispatcher.UIThread.Post(() => FilterBox.Focus());
		}

		void OnKeyDownTunnel(object? sender, KeyEventArgs e)
		{
			if (DataContext is not GoToFileMemberViewModel model)
				return;
			switch (e.Key)
			{
				case Key.Down:
					model.MoveSelection(1);
					break;
				case Key.Up:
					model.MoveSelection(-1);
					break;
				case Key.PageDown:
					model.MoveSelection(10);
					break;
				case Key.PageUp:
					model.MoveSelection(-10);
					break;
				case Key.Enter:
					model.Accept();
					break;
				case Key.Escape:
					model.Cancel();
					break;
				default:
					return;
			}
			if (model.SelectedItem != null)
				ResultList.ScrollIntoView(model.SelectedItem);
			e.Handled = true;
		}

		void OnResultTapped(object? sender, TappedEventArgs e)
		{
			if (DataContext is GoToFileMemberViewModel model && model.Accept())
				e.Handled = true;
		}
	}
}
