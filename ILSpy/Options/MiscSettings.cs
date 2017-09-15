using System.ComponentModel;

namespace ICSharpCode.ILSpy.Options
{
	public class MiscSettings : INotifyPropertyChanged
	{
		bool allowMultipleInstances = false;

		/// <summary>
		/// Decompile anonymous methods/lambdas.
		/// </summary>
		public bool AllowMultipleInstances
		{
			get
			{
				return allowMultipleInstances;
			}
			set
			{
				if (allowMultipleInstances != value)
				{
					allowMultipleInstances = value;
					OnPropertyChanged("AllowMultipleInstance");
				}
			}
		}

		#region INotifyPropertyChanged Implementation

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			PropertyChanged?.Invoke(this, e);
		}

		protected void OnPropertyChanged(string propertyName)
		{
			OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
		}

		#endregion
	}
}
