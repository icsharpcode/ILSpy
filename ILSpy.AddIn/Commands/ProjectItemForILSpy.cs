using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;

namespace ICSharpCode.ILSpy.AddIn.Commands
{
	/// <summary>
	/// Represents a project item in Solution Explorer, which can be opened in ILSpy.
	/// </summary>
	class ProjectItemForILSpy
	{
		SelectedItem item;

		ProjectItemForILSpy(SelectedItem item)
		{
			this.item = item;
		}

		/// <summary>
		/// Detects whether the given <see cref="SelectedItem"/> represents a supported project.
		/// </summary>
		/// <param name="item">Selected item to check.</param>
		/// <returns><see cref="ProjectItemForILSpy"/> instance or <c>null</c>, if item is not a supported project.</returns>
		public static ProjectItemForILSpy Detect(SelectedItem item)
		{
			return new ProjectItemForILSpy(item);
		}

		/// <summary>
		/// If possible retrieves parameters to use for launching ILSpy instance.
		/// </summary>
		/// <param name="owner">Package instance.</param>
		/// <returns>Parameters object or <c>null, if not applicable.</c></returns>
		public ILSpyParameters GetILSpyParameters(ILSpyAddInPackage owner)
		{
			var project = item.Project;
			var roslynProject = owner.Workspace.CurrentSolution.Projects.FirstOrDefault(p => p.FilePath == project.FileName);
			string outputFileName = Path.GetFileName(roslynProject.OutputFilePath);

			// Get the directory path based on the project file.
			string projectPath = Path.GetDirectoryName(project.FullName);

			// Get the output path based on the active configuration
			string projectOutputPath = project.ConfigurationManager.ActiveConfiguration.Properties.Item("OutputPath").Value.ToString();

			// Combine the project path and output path to get the bin path
			return new ILSpyParameters(new[] { Path.Combine(projectPath, projectOutputPath, outputFileName) });
		}
	}
}
