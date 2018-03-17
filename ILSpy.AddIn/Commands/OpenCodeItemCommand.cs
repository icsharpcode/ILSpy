using System;
using System.Linq;
using Microsoft.VisualStudio.Shell;

namespace ICSharpCode.ILSpy.AddIn.Commands
{
	class OpenCodeItemCommand : ILSpyCommand
	{
		static OpenCodeItemCommand instance;

		public OpenCodeItemCommand(ILSpyAddInPackage owner)
			: base(owner, PkgCmdIDList.cmdidOpenCodeItemInILSpy)
		{
		}

		protected override void OnBeforeQueryStatus(object sender, EventArgs e)
		{
			OleMenuCommand menuItem = sender as OleMenuCommand;
			if (menuItem != null) {
				var document = owner.DTE.ActiveDocument;
				menuItem.Enabled =
					(document != null) &&
					(document.ProjectItem != null) &&
					(document.ProjectItem.ContainingProject != null) &&
					(document.ProjectItem.ContainingProject.ConfigurationManager != null) &&
					!string.IsNullOrEmpty(document.ProjectItem.ContainingProject.FileName);
			}
		}

		protected override void OnExecute(object sender, EventArgs e)
		{
			var document = owner.DTE.ActiveDocument;
			var selection = (EnvDTE.TextPoint)((EnvDTE.TextSelection)document.Selection).ActivePoint;

			// Search code elements in desired order, working from innermost to outermost.
			// Should eventually find something, and if not we'll just open the assembly itself.
			var codeElement = GetSelectedCodeElement(selection,
				EnvDTE.vsCMElement.vsCMElementFunction,
				EnvDTE.vsCMElement.vsCMElementEvent,
				EnvDTE.vsCMElement.vsCMElementVariable,		// There is no vsCMElementField, fields are just variables outside of function scope.
				EnvDTE.vsCMElement.vsCMElementProperty,
				EnvDTE.vsCMElement.vsCMElementDelegate,
				EnvDTE.vsCMElement.vsCMElementEnum,
				EnvDTE.vsCMElement.vsCMElementInterface,
				EnvDTE.vsCMElement.vsCMElementStruct,
				EnvDTE.vsCMElement.vsCMElementClass,
				EnvDTE.vsCMElement.vsCMElementNamespace);

			if (codeElement != null) {
				OpenCodeItemInILSpy(codeElement);
			}
			else {
				OpenProjectInILSpy(document.ProjectItem.ContainingProject);
			}
		}

		private EnvDTE.CodeElement GetSelectedCodeElement(EnvDTE.TextPoint selection, params EnvDTE.vsCMElement[] elementTypes)
		{
			foreach (var elementType in elementTypes) {
				var codeElement = selection.CodeElement[elementType];
				if (codeElement != null) {
					return codeElement;
				}
			}

			return null;
		}

		private void OpenProjectInILSpy(EnvDTE.Project project, params string[] arguments)
		{
			var roslynProject = owner.Workspace.CurrentSolution.Projects.FirstOrDefault(p => p.FilePath == project.FileName);
			OpenAssembliesInILSpy(new[] { roslynProject.OutputFilePath }, arguments);
		}

		private void OpenCodeItemInILSpy(EnvDTE.CodeElement codeElement)
		{
			string codeElementKey = CodeElementXmlDocKeyProvider.GetKey(codeElement);
			OpenProjectInILSpy(codeElement.ProjectItem.ContainingProject, "/navigateTo:" + codeElementKey);
		}

		internal static void Register(ILSpyAddInPackage owner)
		{
			instance = new OpenCodeItemCommand(owner);
		}
	}
}
