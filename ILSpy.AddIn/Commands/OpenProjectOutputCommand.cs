using System;
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
			if (owner.DTE.SelectedItems.Count != 1) return;
			var project = owner.DTE.SelectedItems.Item(1).Project;
			var roslynProject = owner.Workspace.CurrentSolution.Projects.FirstOrDefault(p => p.FilePath == project.FileName);
			OpenAssembliesInILSpy(new[] { roslynProject.OutputFilePath });
		}

		internal static void Register(ILSpyAddInPackage owner)
		{
			instance = new OpenProjectOutputCommand(owner);
		}
	}
}
