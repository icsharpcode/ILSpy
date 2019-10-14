using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Mono.Cecil;

namespace ICSharpCode.ILSpy.AddIn.Commands
{
	/// <summary>
	/// Represents a project reference item in Solution Explorer, which can be opened in ILSpy.
	/// </summary>
	class ProjectReferenceForILSpy
	{
		ProjectItem projectItem;
		string fusionName;
		string resolvedPath;

		ProjectReferenceForILSpy(ProjectItem projectItem, string fusionName, string resolvedPath)
		{
			this.projectItem = projectItem;
			this.fusionName = fusionName;
			this.resolvedPath = resolvedPath;
		}

		/// <summary>
		/// Detects whether the given selected item represents a supported project.
		/// </summary>
		/// <param name="itemData">Data object of selected item to check.</param>
		/// <returns><see cref="ProjectReferenceForILSpy"/> instance or <c>null</c>, if item is not a supported project.</returns>
		public static ProjectReferenceForILSpy Detect(object itemData)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			if (itemData is ProjectItem projectItem) {
				var properties = Utils.GetProperties(projectItem.Properties, "FusionName", "ResolvedPath");
				string fusionName = properties[0] as string;
				string resolvedPath = properties[1] as string;
				if ((fusionName != null) || (resolvedPath != null)) {
					return new ProjectReferenceForILSpy(projectItem, fusionName, resolvedPath);
				}
			}

			return null;
		}

		/// <summary>
		/// If possible retrieves parameters to use for launching ILSpy instance.
		/// </summary>
		/// <param name="projectReferences">List of current project's references.</param>
		/// <returns>Parameters object or <c>null, if not applicable.</c></returns>
		public ILSpyParameters GetILSpyParameters(Dictionary<string, DetectedReference> projectReferences)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			string fileName = projectItem.ContainingProject?.FileName;
			if (!string.IsNullOrEmpty(fileName)) {
				if (projectReferences.TryGetValue(projectItem.Name, out DetectedReference path)) {
					return new ILSpyParameters(new[] { path.AssemblyFile });
				}
			}

			return null;
		}

		/// <summary>
		/// If possible retrieves parameters to use for launching ILSpy instance.
		/// </summary>
		/// <returns>Parameters object or <c>null, if not applicable.</c></returns>
		public ILSpyParameters GetILSpyParameters()
		{
			if (resolvedPath != null) {
				return new ILSpyParameters(new[] { $"{resolvedPath}" });
			} else if (!string.IsNullOrWhiteSpace(fusionName)) {
				return new ILSpyParameters(new string[] { GacInterop.FindAssemblyInNetGac(Decompiler.Metadata.AssemblyNameReference.Parse(fusionName)) });
			}

			return null;
		}
	}
}
