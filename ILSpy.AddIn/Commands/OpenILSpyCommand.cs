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

using DTEConstants = EnvDTE.Constants;

namespace ICSharpCode.ILSpy.AddIn.Commands
{
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
			ThreadHelper.ThrowIfNotOnUIThread();

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
			return Path.Combine(basePath, "ILSpy", "ILSpy.exe");
		}

		protected void OpenAssembliesInILSpy(ILSpyParameters parameters)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			if (parameters == null)
				return;

			foreach (string assemblyFileName in parameters.AssemblyFileNames)
			{
				if (!File.Exists(assemblyFileName))
				{
					owner.ShowMessage("Could not find assembly '{0}', please ensure the project and all references were built correctly!", assemblyFileName);
					return;
				}
			}

			var ilspyExe = new ILSpyInstance(parameters);
			ilspyExe.Start();
		}

		protected Dictionary<string, DetectedReference> GetReferences(Microsoft.CodeAnalysis.Project parentProject)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			var dict = new Dictionary<string, DetectedReference>();
			foreach (var reference in parentProject.MetadataReferences)
			{
				using (var assemblyDef = AssemblyDefinition.ReadAssembly(reference.Display))
				{
					string assemblyName = assemblyDef.Name.Name;
					string resolvedAssemblyFile = AssemblyFileFinder.FindAssemblyFile(assemblyDef, reference.Display);
					dict.Add(assemblyName, new DetectedReference(assemblyName, resolvedAssemblyFile, false));
				}
			}
			foreach (var projectReference in parentProject.ProjectReferences)
			{
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
			ThreadHelper.ThrowIfNotOnUIThread();

			foreach (var project in projects)
			{
				switch (project.Kind)
				{
					case DTEConstants.vsProjectKindSolutionItems:
						// This is a solution folder -> search in sub-projects
						var subProject = FindProject(
							project.ProjectItems.OfType<EnvDTE.ProjectItem>().Select(pi => pi.SubProject).OfType<EnvDTE.Project>(),
							projectFile);
						if (subProject != null)
							return subProject;
						break;

					case DTEConstants.vsProjectKindUnmodeled:
						// Skip unloaded projects completely
						break;

					default:
						// Match by project's file name
						if (project.FileName == projectFile)
							return project;
						break;
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
			ThreadHelper.ThrowIfNotOnUIThread();
		}

		protected override void OnExecute(object sender, EventArgs e)
		{
			new ILSpyInstance().Start();
		}

		internal static void Register(ILSpyAddInPackage owner)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			instance = new OpenILSpyCommand(owner);
		}
	}
}
