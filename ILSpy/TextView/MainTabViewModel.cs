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

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
