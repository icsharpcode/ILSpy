// Copyright (c) 2018 Andreas Weizel
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
