using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using ICSharpCode.Decompiler.Metadata;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Shell;
using Mono.Cecil;
using VSLangProj;

namespace ICSharpCode.ILSpy.AddIn.Commands
{
	class OpenReferenceCommand : ILSpyCommand
	{
		static OpenReferenceCommand instance;

		public OpenReferenceCommand(ILSpyAddInPackage owner)
			: base(owner, PkgCmdIDList.cmdidOpenReferenceInILSpy)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
		}

		protected override void OnBeforeQueryStatus(object sender, EventArgs e)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			if (sender is OleMenuCommand menuItem) {
				menuItem.Visible = false;

				var selectedItemData = owner.GetSelectedItemsData<object>().FirstOrDefault();
				if (selectedItemData == null)
					return;

				/*
				 * Assure that we only show the context menu item on items we intend:
				 * - Project references
				 * - NuGet package references
				 */
				if ((AssemblyReferenceForILSpy.Detect(selectedItemData) != null)
					|| (ProjectReferenceForILSpy.Detect(selectedItemData) != null)
					|| (NuGetReferenceForILSpy.Detect(selectedItemData) != null)) {
					menuItem.Visible = true;
				}
			}
		}

		protected override void OnExecute(object sender, EventArgs e)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			var itemObject = owner.GetSelectedItemsData<object>().FirstOrDefault();
			if (itemObject == null)
				return;

			var referenceItem = AssemblyReferenceForILSpy.Detect(itemObject);
			if (referenceItem != null) {
				Reference reference = itemObject as Reference;
				var project = reference.ContainingProject;
				var roslynProject = owner.Workspace.CurrentSolution.Projects.FirstOrDefault(p => p.FilePath == project.FileName);
				var references = GetReferences(roslynProject);
				var parameters = referenceItem.GetILSpyParameters(references);
				if (references.TryGetValue(reference.Name, out var path))
					OpenAssembliesInILSpy(parameters);
				else
					owner.ShowMessage("Could not find reference '{0}', please ensure the project and all references were built correctly!", reference.Name);
				return;
			}

			// Handle NuGet references
			var nugetRefItem = NuGetReferenceForILSpy.Detect(itemObject);
			if (nugetRefItem != null) {
				OpenAssembliesInILSpy(nugetRefItem.GetILSpyParameters());
				return;
			}

			// Handle project references
			var projectRefItem = ProjectReferenceForILSpy.Detect(itemObject);
			if (projectRefItem != null) {
				var projectItem = itemObject as ProjectItem;
				string fileName = projectItem.ContainingProject?.FileName;
				if (!string.IsNullOrEmpty(fileName)) {
					var roslynProject = owner.Workspace.CurrentSolution.Projects.FirstOrDefault(p => p.FilePath == fileName);
					var references = GetReferences(roslynProject);
					if (references.TryGetValue(projectItem.Name, out DetectedReference path)) {
						OpenAssembliesInILSpy(projectRefItem.GetILSpyParameters(references));
						return;
					}
				}

				OpenAssembliesInILSpy(projectRefItem.GetILSpyParameters());
				return;
			}
		}

		internal static void Register(ILSpyAddInPackage owner)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			instance = new OpenReferenceCommand(owner);
		}
	}

}
