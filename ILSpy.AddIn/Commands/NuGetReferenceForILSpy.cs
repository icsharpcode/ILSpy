using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EnvDTE;

using Microsoft.VisualStudio.Shell;

namespace ICSharpCode.ILSpy.AddIn.Commands
{
	/// <summary>
	/// Represents a NuGet package item in Solution Explorer, which can be opened in ILSpy.
	/// </summary>
	class NuGetReferenceForILSpy
	{
		ProjectItem projectItem;

		NuGetReferenceForILSpy(ProjectItem projectItem)
		{
			this.projectItem = projectItem;
		}

		/// <summary>
		/// Detects whether the given selected item represents a supported project.
		/// </summary>
		/// <param name="itemData">Data object of selected item to check.</param>
		/// <returns><see cref="NuGetReferenceForILSpy"/> instance or <c>null</c>, if item is not a supported project.</returns>
		public static NuGetReferenceForILSpy Detect(object itemData)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			if (itemData is ProjectItem projectItem)
			{
				var properties = Utils.GetProperties(projectItem.Properties, "Type");
				if ((properties[0] as string) == "Package")
				{
					return new NuGetReferenceForILSpy(projectItem);
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
			ThreadHelper.ThrowIfNotOnUIThread();

			var properties = Utils.GetProperties(projectItem.Properties, "Name", "Version", "Path");
			if (properties[0] != null && properties[1] != null && properties[2] != null)
			{
				return new ILSpyParameters(new[] { $"{properties[2]}\\{properties[0]}.{properties[1]}.nupkg" });
			}

			return null;
		}
	}
}
