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
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

using ICSharpCode.ILSpy.Docking;

using TomsToolbox.Wpf;

namespace ICSharpCode.ILSpy.ViewModels
{
	public abstract class PaneModel : ObservableObject
	{
		private readonly Throttle titleChangeThrottle;

		protected static DockWorkspace DockWorkspace => App.ExportProvider.GetExportedValue<DockWorkspace>();

		protected PaneModel()
		{
			titleChangeThrottle = new Throttle(() => OnPropertyChanged(nameof(Title)));
		}

		class CloseCommandImpl : ICommand
		{
			readonly PaneModel model;

			public CloseCommandImpl(PaneModel model)
			{
				this.model = model;
				this.model.PropertyChanged += Model_PropertyChanged;
			}

			private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
			{
				if (e.PropertyName == nameof(model.IsCloseable))
				{
					CanExecuteChanged?.Invoke(this, EventArgs.Empty);
				}
			}

			public event EventHandler CanExecuteChanged;

			public bool CanExecute(object parameter)
			{
				return model.IsCloseable;
			}

			public void Execute(object parameter)
			{
				DockWorkspace.Remove(model);
			}
		}

		private bool isSelected;

		public bool IsSelected {
			get => isSelected;
			set => SetProperty(ref isSelected, value);
		}

		private bool isActive;

		public bool IsActive {
			get => isActive;
			set => SetProperty(ref isActive, value);
		}

		private bool isVisible;

		public bool IsVisible {
			get { return isVisible; }
			set {
				if (SetProperty(ref isVisible, value) && !value)
				{
					// When the pane is hidden, it should no longer be marked as active, else it won't raise an event when it is activated again.
					IsActive = false;
				}
			}
		}

		private bool isCloseable = true;

		public bool IsCloseable {
			get => isCloseable;
			set => SetProperty(ref isCloseable, value);
		}

		public ICommand CloseCommand => new CloseCommandImpl(this);

		private string contentId;

		public string ContentId {
			get => contentId;
			set => SetProperty(ref contentId, value);
		}

		private string title;

		public string Title {
			get => title;
			set {
				title = value;
				titleChangeThrottle.Tick();
			}
		}
	}

	public static class Pane
	{
		// Helper properties to enable binding state properties from the model to the view.

		public static readonly DependencyProperty IsActiveProperty = DependencyProperty.RegisterAttached(
			"IsActive", typeof(bool), typeof(Pane), new FrameworkPropertyMetadata(default(bool)));
		public static void SetIsActive(DependencyObject element, bool value)
		{
			element.SetValue(IsActiveProperty, value);
		}
		public static bool GetIsActive(DependencyObject element)
		{
			return (bool)element.GetValue(IsActiveProperty);
		}

		public static readonly DependencyProperty IsVisibleProperty = DependencyProperty.RegisterAttached(
			"IsVisible", typeof(bool), typeof(Pane), new FrameworkPropertyMetadata(default(bool)));
		public static void SetIsVisible(DependencyObject element, bool value)
		{
			element.SetValue(IsVisibleProperty, value);
		}
		public static bool GetIsVisible(DependencyObject element)
		{
			return (bool)element.GetValue(IsVisibleProperty);
		}
	}
}
