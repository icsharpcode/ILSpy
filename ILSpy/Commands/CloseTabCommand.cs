using System;
using System.Windows.Input;
using ICSharpCode.ILSpy.TextView;

namespace ICSharpCode.ILSpy.Commands
{
	class CloseTabCommand : ICommand
	{
		private readonly DecompilerTab decompilerTab;

		public CloseTabCommand(DecompilerTab decompilerTab)
		{
			this.decompilerTab = decompilerTab;
		}

		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			this.decompilerTab.Close();
		}
	}
}
