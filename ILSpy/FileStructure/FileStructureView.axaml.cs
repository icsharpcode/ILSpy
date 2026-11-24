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

namespace ICSharpCode.ILSpy.FileStructure
{
	/// <summary>
	/// Renders <see cref="FileStructureViewModel"/> as a flat, indented list. A click or Enter on a
	/// row moves the editor caret to that member.
	/// </summary>
	public partial class FileStructureView : UserControl
	{
		public FileStructureView()
		{
			InitializeComponent();
			EntryList.Tapped += OnEntryTapped;
			EntryList.AddHandler(KeyDownEvent, OnEntryKeyDown, RoutingStrategies.Tunnel);
		}

		protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
		{
			base.OnAttachedToVisualTree(e);
			(DataContext as FileStructureViewModel)?.Attach();
		}

		void OnEntryTapped(object? sender, TappedEventArgs e)
		{
			if (ActivateSelection())
				e.Handled = true;
		}

		void OnEntryKeyDown(object? sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter && e.KeyModifiers == KeyModifiers.None && ActivateSelection())
				e.Handled = true;
		}

		bool ActivateSelection()
			=> DataContext is FileStructureViewModel model
				&& EntryList.SelectedItem is FileMemberEntry entry
				&& model.Activate(entry);
	}
}
