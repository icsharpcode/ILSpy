using System.ComponentModel;
using System.Windows.Input;

namespace ICSharpCode.ILSpy.ViewModels
{
	public abstract class PaneModel : INotifyPropertyChanged
	{
		public abstract PanePosition DefaultPosition { get; }

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

		private bool isVisible = true;
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
