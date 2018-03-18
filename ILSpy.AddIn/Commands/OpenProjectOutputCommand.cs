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
			if (owner.DTE.SelectedItems.Count != 1) return;
			var project = owner.DTE.SelectedItems.Item(1).Project;
			var roslynProject = owner.Workspace.CurrentSolution.Projects.FirstOrDefault(p => p.FilePath == project.FileName);
			string outputFileName = Path.GetFileName(roslynProject.OutputFilePath);
			//get the directory path based on the project file.
			string projectPath = Path.GetDirectoryName(project.FullName);
			//get the output path based on the active configuration
			string projectOutputPath = project.ConfigurationManager.ActiveConfiguration.Properties.Item("OutputPath").Value.ToString();
			//combine the project path and output path to get the bin path
			OpenAssembliesInILSpy(new[] { Path.Combine(projectPath, projectOutputPath, outputFileName) });
		}

		internal static void Register(ILSpyAddInPackage owner)
		{
			instance = new OpenProjectOutputCommand(owner);
		}
	}
}
