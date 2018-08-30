using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ICSharpCode.ILSpy.Commands;

namespace ICSharpCode.ILSpy.TextView
{
	public class DecompilerTab : INotifyPropertyChanged
	{
		private readonly MainTabViewModel Container;

		public DecompilerTab(MainTabViewModel Container, DecompilerTextView decompilerTextView)
		{
			this.Container = Container;
			this.MainContent = decompilerTextView;
			this.TabName = $"Tab {Container.DecompilerTabs.Count}";
			this.CloseTabCommand = new CloseTabCommand(this);
		}

		private string tabName;

		public string TabName {
			get {
				return tabName;
			}
			set {
				tabName = value;
				OnPropertyChanged();
			}
		}

		public ICommand CloseTabCommand { get; }

		private DecompilerTextView mainContent;

		public DecompilerTextView MainContent {
			get {
				return mainContent;
			}
			set {
				mainContent = value;
				OnPropertyChanged();
			}
		}

		public void Close()
		{
			this.Container.CloseDecompilerTab(this);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
