using System.IO;
using System.Linq;

using EnvDTE;

using Microsoft.VisualStudio.Shell;

namespace ICSharpCode.ILSpy.AddIn.Commands
{
	/// <summary>
	/// Represents a project item in Solution Explorer, which can be opened in ILSpy.
	/// </summary>
	class ProjectItemForILSpy
	{
		SelectedItem item;
		Project project;
		Microsoft.CodeAnalysis.Project roslynProject;

		ProjectItemForILSpy(Project project, Microsoft.CodeAnalysis.Project roslynProject, SelectedItem item)
		{
			this.project = project;
			this.roslynProject = roslynProject;
			this.item = item;
		}

		/// <summary>
		/// Detects whether the given <see cref="SelectedItem"/> represents a supported project.
		/// </summary>
		/// <param name="item">Selected item to check.</param>
		/// <returns><see cref="ProjectItemForILSpy"/> instance or <c>null</c>, if item is not a supported project.</returns>
		public static ProjectItemForILSpy Detect(ILSpyAddInPackage package, SelectedItem item)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			var project = item.Project;
			var roslynProject = package.Workspace.CurrentSolution.Projects.FirstOrDefault(p => p.FilePath == project.FileName);
			if (roslynProject == null)
				return null;

			return new ProjectItemForILSpy(project, roslynProject, item);
		}

		/// <summary>
		/// If possible retrieves parameters to use for launching ILSpy instance.
		/// </summary>
		/// <param name="package">Package instance.</param>
		/// <returns>Parameters object or <c>null, if not applicable.</c></returns>
		public ILSpyParameters GetILSpyParameters(ILSpyAddInPackage package)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			return new ILSpyParameters(new[] { Utils.GetProjectOutputAssembly(project, roslynProject) });
		}
	}
}
