using System;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
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

		protected override async void OnExecute(object sender, EventArgs e)
		{
			var document = owner.DTE.ActiveDocument;
			var selection = (EnvDTE.TextPoint)((EnvDTE.TextSelection)document.Selection).ActivePoint;
			var id = owner.Workspace.CurrentSolution.GetDocumentIdsWithFilePath(document.FullName).FirstOrDefault();

			if (id == null) return;
			var roslynDocument = owner.Workspace.CurrentSolution.GetDocument(id);
			var ast = await roslynDocument.GetSyntaxRootAsync().ConfigureAwait(false);
			var model = await roslynDocument.GetSemanticModelAsync().ConfigureAwait(false);
			var node = ast.FindNode(new Microsoft.CodeAnalysis.Text.TextSpan(selection.AbsoluteCharOffset, 1));
			if (node == null)
				return;
			var symbol = model.GetSymbolInfo(node).Symbol;
			if (symbol == null)
				return;
			var refs = GetReferences(roslynDocument.Project).Select(fn => fn.Value).Where(f => File.Exists(f)).ToArray();
			OpenAssembliesInILSpy(refs, "/navigateTo:" + symbol.GetDocumentationCommentId());
		}

		internal static void Register(ILSpyAddInPackage owner)
		{
			instance = new OpenCodeItemCommand(owner);
		}
	}
}
