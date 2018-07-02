using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Mono.Cecil;

namespace ICSharpCode.ILSpy.AddIn.Commands
{
	public class ILSpyParameters
	{
		public ILSpyParameters(IEnumerable<string> assemblyFileNames, params string[] arguments)
		{
			this.AssemblyFileNames = assemblyFileNames;
			this.Arguments = arguments;
		}

		public IEnumerable<string> AssemblyFileNames { get; private set; }
		public string[] Arguments { get; private set; }
	}

	public class DetectedReference
	{
		public DetectedReference(string name, string assemblyFile, bool isProjectReference)
		{
			this.Name = name;
			this.AssemblyFile = assemblyFile;
			this.IsProjectReference = isProjectReference;
		}

		public string Name { get; private set; }
		public string AssemblyFile { get; private set; }
		public bool IsProjectReference { get; private set; }
	}

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

		protected void OpenAssembliesInILSpy(ILSpyParameters parameters)
		{
			if (parameters == null)
				return;

			foreach (string assemblyFileName in parameters.AssemblyFileNames) {
				if (!File.Exists(assemblyFileName)) {
					owner.ShowMessage("Could not find assembly '{0}', please ensure the project and all references were built correctly!", assemblyFileName);
					return;
				}
			}

			string commandLineArguments = Utils.ArgumentArrayToCommandLine(parameters.AssemblyFileNames.ToArray());
			if (parameters.Arguments != null) {
				commandLineArguments = string.Concat(commandLineArguments, " ", Utils.ArgumentArrayToCommandLine(parameters.Arguments));
			}

			System.Diagnostics.Process.Start(GetILSpyPath(), commandLineArguments);
		}

		protected Dictionary<string, DetectedReference> GetReferences(Microsoft.CodeAnalysis.Project parentProject)
		{
			var dict = new Dictionary<string, DetectedReference>();
			foreach (var reference in parentProject.MetadataReferences) {
				using (var assemblyDef = AssemblyDefinition.ReadAssembly(reference.Display)) {
					string assemblyName = assemblyDef.Name.Name;
					if (AssemblyFileFinder.IsReferenceAssembly(assemblyDef, reference.Display)) {
						string resolvedAssemblyFile = AssemblyFileFinder.FindAssemblyFile(assemblyDef, reference.Display);
						dict.Add(assemblyName, 
							new DetectedReference(assemblyName, resolvedAssemblyFile, false));
					} else {
						dict.Add(assemblyName, 
							new DetectedReference(assemblyName, reference.Display, false));
					}
				}
			}
			foreach (var projectReference in parentProject.ProjectReferences) {
				var roslynProject = owner.Workspace.CurrentSolution.GetProject(projectReference.ProjectId);
				var project = FindProject(owner.DTE.Solution.Projects.OfType<EnvDTE.Project>(), roslynProject.FilePath);
				if (roslynProject != null && project != null)
					dict.Add(roslynProject.AssemblyName, 
						new DetectedReference(roslynProject.AssemblyName, Utils.GetProjectOutputAssembly(project, roslynProject), true));
			}
			return dict;
		}

		protected EnvDTE.Project FindProject(IEnumerable<EnvDTE.Project> projects, string projectFile)
		{
			foreach (var project in projects) {
				if (project.Kind == ProjectKinds.vsProjectKindSolutionFolder) {
					// This is a solution folder -> search in sub-projects
					var subProject = FindProject(
						project.ProjectItems.OfType<EnvDTE.ProjectItem>().Select(pi => pi.SubProject).OfType<EnvDTE.Project>(), 
						projectFile);
					if (subProject != null)
						return subProject;
				} else {
					if (project.FileName == projectFile)
						return project;
				}
			}

			return null;
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
