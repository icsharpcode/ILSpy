using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ICSharpCode.ILSpy.TextView
{
	public class MainTabViewModel : INotifyPropertyChanged
	{
		public MainTabViewModel()
		{
			DecompilerTabs = new ObservableCollection<DecompilerTab>();
		}

		private int activeTab;

		public int ActiveTab {
			get {
				return activeTab;
			}
			set {
				activeTab = value;
				OnPropertyChanged();
			}
		}

		private ObservableCollection<DecompilerTab> decompilerTabs;

		public ObservableCollection<DecompilerTab> DecompilerTabs {
			get {
				return decompilerTabs;
			}
			set {
				decompilerTabs = value;
				OnPropertyChanged();
			}
		}

		public DecompilerTab CurrentDecompilerTab =>
			(DecompilerTabs.Count > 0 && ActiveTab < DecompilerTabs.Count) ? DecompilerTabs[ActiveTab] : null;

		public void AddDecompilerTab(DecompilerTextView decompilerTextView)
		{
			DecompilerTabs.Add(new DecompilerTab(this, decompilerTextView));
			ActiveTab = DecompilerTabs.Count - 1;
		}

		public void CloseDecompilerTab(DecompilerTab decompilerTab)
		{
			DecompilerTabs.Remove(decompilerTab);
		}

		/// <summary>
		/// Get current DecompilerTextView, create a new one if does not exist
		/// </summary>
		/// <returns></returns>
		public DecompilerTextView GetCurrentDecompilerTextView()
		{
			DecompilerTextView view;
			if (DecompilerTabs.Count > 0) {
				var currentDecompilerTab = this.CurrentDecompilerTab;
				view = currentDecompilerTab?.MainContent;
				// If tab is there but view is null
				if (view == null) {
					view = new DecompilerTextView();
					currentDecompilerTab.MainContent = view;
				}
			} else {
				view = new DecompilerTextView();
				this.AddDecompilerTab(view);
			}

			return view;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
