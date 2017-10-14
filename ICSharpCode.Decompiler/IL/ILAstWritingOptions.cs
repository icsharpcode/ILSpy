using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace ICSharpCode.Decompiler.IL
{
	public class ILAstWritingOptions : INotifyPropertyChanged
	{
		private bool useLogicOperationSugar;
		private bool useFieldSugar;
		private bool showILRanges;

		/// <summary>
		/// Sugar for logic.not/and/or.
		/// </summary>
		public bool UseLogicOperationSugar {
			get { return useLogicOperationSugar; }
			set {
				if (useLogicOperationSugar != value) {
					useLogicOperationSugar = value;
					OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// Sugar for ldfld/stfld.
		/// </summary>
		public bool UseFieldSugar {
			get { return useFieldSugar; }
			set {
				if (useFieldSugar != value) {
					useFieldSugar = value;
					OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// Show IL ranges in ILAst output.
		/// </summary>
		public bool ShowILRanges {
			get { return showILRanges; }
			set {
				if (showILRanges != value) {
					showILRanges = value;
					OnPropertyChanged();
				}
			}
		}

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
		}

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			PropertyChanged?.Invoke(this, e);
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
