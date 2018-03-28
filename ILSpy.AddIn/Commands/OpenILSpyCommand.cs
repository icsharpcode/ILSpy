using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using Mono.Cecil;

namespace ICSharpCode.ILSpy.AddIn.Commands
{
	abstract class ILSpyCommand
	{
		protected ILSpyAddInPackage owner;

		protected ILSpyCommand(ILSpyAddInPackage owner, uint id)
		{
			this.owner = owner;
			CommandID menuCommandID = new CommandID(GuidList.guidILSpyAddInCmdSet, (int)id);
			OleMenuCommand menuItem = new OleMenuCommand(OnExecute, menuCommandID);
			menuItem.BeforeQueryStatus += OnBeforeQueryStatus;
			owner.MenuService.AddCommand(menuItem);
		}

		protected virtual void OnBeforeQueryStatus(object sender, EventArgs e)
		{
		}

		protected abstract void OnExecute(object sender, EventArgs e);

		protected string GetILSpyPath()
		{
			var basePath = Path.GetDirectoryName(typeof(ILSpyAddInPackage).Assembly.Location);
			return Path.Combine(basePath, "ILSpy.exe");
		}

		protected void OpenAssembliesInILSpy(IEnumerable<string> assemblyFileNames, params string[] arguments)
		{
			foreach (string assemblyFileName in assemblyFileNames) {
				if (!File.Exists(assemblyFileName)) {
					owner.ShowMessage("Could not find assembly '{0}', please ensure the project and all references were built correctly!", assemblyFileName);
					return;
				}
			}

			string commandLineArguments = Utils.ArgumentArrayToCommandLine(assemblyFileNames.ToArray());
			if (arguments != null) {
				commandLineArguments = string.Concat(commandLineArguments, " ", Utils.ArgumentArrayToCommandLine(arguments));
			}

			System.Diagnostics.Process.Start(GetILSpyPath(), commandLineArguments);
		}

		protected string GetProjectOutputPath(EnvDTE.Project project, Microsoft.CodeAnalysis.Project roslynProject)
		{
			string outputFileName = Path.GetFileName(roslynProject.OutputFilePath);
			//get the directory path based on the project file.
			string projectPath = Path.GetDirectoryName(project.FullName);
			//get the output path based on the active configuration
			string projectOutputPath = project.ConfigurationManager.ActiveConfiguration.Properties.Item("OutputPath").Value.ToString();
			//combine the project path and output path to get the bin path
			return Path.Combine(projectPath, projectOutputPath, outputFileName);
		}

		protected Dictionary<string, string> GetReferences(Microsoft.CodeAnalysis.Project parentProject)
		{
			var dict = new Dictionary<string, string>();
			foreach (var reference in parentProject.MetadataReferences) {
				using (var assemblyDef = AssemblyDefinition.ReadAssembly(reference.Display)) {
					if (IsReferenceAssembly(assemblyDef)) {
						dict.Add(assemblyDef.Name.Name, GacInterop.FindAssemblyInNetGac(assemblyDef.Name));
					} else {
						dict.Add(assemblyDef.Name.Name, reference.Display);
					}
				}
			}
			foreach (var projectReference in parentProject.ProjectReferences) {
				var roslynProject = owner.Workspace.CurrentSolution.GetProject(projectReference.ProjectId);
				var project = owner.DTE.Solution.Projects.OfType<EnvDTE.Project>().FirstOrDefault(p => p.FileName == roslynProject.FilePath);
				if (roslynProject != null && project != null)
					dict.Add(roslynProject.AssemblyName, GetProjectOutputPath(project, roslynProject));
			}
			return dict;
		}

		protected bool IsReferenceAssembly(AssemblyDefinition assemblyDef)
		{
			return assemblyDef.CustomAttributes.Any(ca => ca.AttributeType.FullName == "System.Runtime.CompilerServices.ReferenceAssemblyAttribute");
		}
	}

	class OpenILSpyCommand : ILSpyCommand
	{
		static OpenILSpyCommand instance;

		public OpenILSpyCommand(ILSpyAddInPackage owner)
			: base(owner, PkgCmdIDList.cmdidOpenILSpy)
		{
		}

		protected override void OnExecute(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start(GetILSpyPath());
		}

		internal static void Register(ILSpyAddInPackage owner)
		{
			instance = new OpenILSpyCommand(owner);
		}
	}
}
