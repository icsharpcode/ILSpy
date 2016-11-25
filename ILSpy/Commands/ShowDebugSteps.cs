using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.ILSpy.Commands
{
	[ExportMainMenuCommand(Menu = "_View", Header = "_Show debug steps", MenuOrder = 5000)]
	class ShowDebugSteps : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			DebugSteps.Show();
		}
	}
}
