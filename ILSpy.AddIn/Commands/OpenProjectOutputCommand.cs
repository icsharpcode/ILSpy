using System;
using System.IO;
using System.Linq;

namespace ICSharpCode.ILSpy.AddIn.Commands
{
	class OpenProjectOutputCommand : ILSpyCommand
	{
		static OpenProjectOutputCommand instance;

		public OpenProjectOutputCommand(ILSpyAddInPackage owner)
			: base(owner, PkgCmdIDList.cmdidOpenProjectOutputInILSpy)
		{
		}

		protected override void OnExecute(object sender, EventArgs e)
		{
			if (owner.DTE.SelectedItems.Count != 1)
				return;
			var projectItemWrapper = ProjectItemForILSpy.Detect(owner.DTE.SelectedItems.Item(1));
			if (projectItemWrapper != null) {
				OpenAssembliesInILSpy(projectItemWrapper.GetILSpyParameters(owner));
			}
		}

		internal static void Register(ILSpyAddInPackage owner)
		{
			instance = new OpenProjectOutputCommand(owner);
		}
	}
}
