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
using System.Windows.Input;

namespace ICSharpCode.ILSpy.ViewModels
{
	public abstract class PaneModel : INotifyPropertyChanged
	{
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
				if (e.PropertyName == nameof(model.IsCloseable)) {
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
				Docking.DockWorkspace.Instance.Remove(model);
			}
		}

		public PaneModel()
		{
			this.closeCommand = new CloseCommandImpl(this);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void RaisePropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private bool isSelected = false;
		public bool IsSelected {
			get => isSelected;
			set {
				if (isSelected != value) {
					isSelected = value;
					RaisePropertyChanged(nameof(IsSelected));
				}
			}
		}

		private bool isActive = false;
		public bool IsActive {
			get => isActive;
			set {
				if (isActive != value) {
					isActive = value;
					RaisePropertyChanged(nameof(IsActive));
				}
			}
		}

		private bool isVisible;
		public bool IsVisible {
			get { return isVisible; }
			set {
				if (isVisible != value) {
					isVisible = value;
					RaisePropertyChanged(nameof(IsVisible));
				}
			}
		}

		private bool isCloseable = true;
		public bool IsCloseable {
			get { return isCloseable; }
			set {
				if (isCloseable != value) {
					isCloseable = value;
					RaisePropertyChanged(nameof(IsCloseable));
				}
			}
		}

		private ICommand closeCommand;
		public ICommand CloseCommand {
			get { return closeCommand; }
			set {
				if (closeCommand != value) {
					closeCommand = value;
					RaisePropertyChanged(nameof(CloseCommand));
				}
			}
		}

		private string contentId;
		public string ContentId {
			get => contentId;
			set {
				if (contentId != value) {
					contentId = value;
					RaisePropertyChanged(nameof(ContentId));
				}
			}
		}

		private string title;
		public string Title {
			get => title;
			set {
				if (title != value) {
					title = value;
					RaisePropertyChanged(nameof(Title));
				}
			}
		}
	}
}
