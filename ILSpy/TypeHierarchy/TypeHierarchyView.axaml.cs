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

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace ICSharpCode.ILSpy.TypeHierarchy
{
	/// <summary>
	/// Renders <see cref="TypeHierarchyViewModel"/> as a tree. Double-click or Enter on a type row
	/// navigates to that type.
	/// </summary>
	public partial class TypeHierarchyView : UserControl
	{
		public TypeHierarchyView()
		{
			InitializeComponent();
			HierarchyTree.DoubleTapped += OnTreeDoubleTapped;
			// TreeViewItem consumes Enter to toggle expansion before the event bubbles, so listen
			// in the tunnel phase to turn Enter on a type row into navigation.
			HierarchyTree.AddHandler(KeyDownEvent, OnTreeKeyDown, RoutingStrategies.Tunnel, handledEventsToo: true);
		}

		void OnTreeDoubleTapped(object? sender, TappedEventArgs e)
		{
			if (ActivateSelection())
				e.Handled = true;
		}

		void OnTreeKeyDown(object? sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter && e.KeyModifiers == KeyModifiers.None && ActivateSelection())
				e.Handled = true;
		}

		bool ActivateSelection()
			=> DataContext is TypeHierarchyViewModel model
				&& HierarchyTree.SelectedItem is TypeHierarchyNode node
				&& model.Activate(node);
	}
}
