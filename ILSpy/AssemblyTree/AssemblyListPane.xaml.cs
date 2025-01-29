// Copyright (c) 2024 Tom Englert for the SharpDevelop Team
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

using System.Composition;
using System.Windows;
using System.Windows.Threading;

using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpyX.TreeView;

using TomsToolbox.Wpf;
using TomsToolbox.Wpf.Composition.AttributedModel;

namespace ICSharpCode.ILSpy.AssemblyTree
{
	/// <summary>
	/// Interaction logic for AssemblyListPane.xaml
	/// </summary>
	[DataTemplate(typeof(AssemblyTreeModel))]
	[NonShared]
	public partial class AssemblyListPane
	{
		public AssemblyListPane()
		{
			InitializeComponent();

			ContextMenuProvider.Add(this);
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == DataContextProperty)
			{
				if (e.NewValue is not AssemblyTreeModel model)
					return;

				model.SetActiveView(this);

				// If there is already a selected item in the model, we need to scroll it into view, so it can be selected in the UI.
				var selected = model.SelectedItem;
				if (selected != null)
				{
					this.BeginInvoke(DispatcherPriority.Background, () => {
						ScrollIntoView(selected);
						this.SelectedItem = selected;
					});
				}
			}
			else if (e.Property == Pane.IsActiveProperty)
			{
				if (!true.Equals(e.NewValue))
					return;

				if (SelectedItem is SharpTreeNode selectedItem)
				{
					// defer focusing, so it does not interfere with selection via mouse click
					this.BeginInvoke(() => {
						if (this.SelectedItem == selectedItem)
							FocusNode(selectedItem);
					});
				}
				else
				{
					Focus();
				}
			}
		}
	}
}
