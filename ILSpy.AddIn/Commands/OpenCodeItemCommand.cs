using System;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;

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
			if (sender is OleMenuCommand menuItem) {
				menuItem.Visible = false;

				// Enable this item only if this is a .cs file!
				if (Utils.GetCurrentViewHost(owner, f => f.EndsWith(".cs")) == null)
					return;

				// Enable this item only if this is a Roslyn document
				if (GetRoslynDocument() == null)
					return;

				var document = owner.DTE.ActiveDocument;
				menuItem.Visible =
					(document != null) &&
					(document.ProjectItem != null) &&
					(document.ProjectItem.ContainingProject != null) &&
					(document.ProjectItem.ContainingProject.ConfigurationManager != null) &&
					!string.IsNullOrEmpty(document.ProjectItem.ContainingProject.FileName);
			}
		}

		Document GetRoslynDocument()
		{
			var document = owner.DTE.ActiveDocument;
			var selection = (EnvDTE.TextPoint)((EnvDTE.TextSelection)document.Selection).ActivePoint;
			var id = owner.Workspace.CurrentSolution.GetDocumentIdsWithFilePath(document.FullName).FirstOrDefault();
			if (id == null)
				return null;

			return owner.Workspace.CurrentSolution.GetDocument(id);
		}

		EnvDTE.TextPoint GetEditorSelection()
		{
			var document = owner.DTE.ActiveDocument;
			return ((EnvDTE.TextSelection)document.Selection).ActivePoint;
		}

		protected override async void OnExecute(object sender, EventArgs e)
		{
			var textView = Utils.GetCurrentViewHost(owner)?.TextView;
			if (textView == null)
				return;

			SnapshotPoint caretPosition = textView.Caret.Position.BufferPosition;
			var roslynDocument = GetRoslynDocument();
			var ast = await roslynDocument.GetSyntaxRootAsync().ConfigureAwait(false);
			var model = await roslynDocument.GetSemanticModelAsync().ConfigureAwait(false);
			var node = ast.FindNode(new TextSpan(caretPosition.Position, 0), false, true);
			if (node == null) {
				owner.ShowMessage("Can't show ILSpy for this code element!");
				return;
			}

			var symbol = GetSymbolResolvableByILSpy(model, node);
			if (symbol == null) {
				owner.ShowMessage("Can't show ILSpy for this code element!");
				return;
			}


			var roslynProject = roslynDocument.Project;
			var refsmap = GetReferences(roslynProject);

			// Add our own project as well (not among references)
			var project = owner.DTE.Solution.Projects.OfType<EnvDTE.Project>()
				.FirstOrDefault(p => p.FileName == roslynProject.FilePath);
			if (project != null) {
				string projectOutputPath = GetProjectOutputPath(project, roslynProject);
				refsmap.Add(roslynDocument.Project.AssemblyName, projectOutputPath);
			}

			var refs = refsmap.Select(fn => fn.Value).Where(f => File.Exists(f));
			OpenAssembliesInILSpy(new ILSpyParameters(refs, "/navigateTo:" + symbol.GetDocumentationCommentId()));
		}

		ISymbol GetSymbolResolvableByILSpy(SemanticModel model, SyntaxNode node)
		{
			var current = node;
			while (current != null) {
				var symbol = model.GetSymbolInfo(current).Symbol;
				if (symbol == null) {
					symbol = model.GetDeclaredSymbol(current);
				}

				// ILSpy can only resolve some symbol types, so allow them, discard everything else
				if (symbol != null) {
					switch (symbol.Kind) {
						case SymbolKind.ArrayType:
						case SymbolKind.Event:
						case SymbolKind.Field:
						case SymbolKind.Method:
						case SymbolKind.NamedType:
						case SymbolKind.Namespace:
						case SymbolKind.PointerType:
						case SymbolKind.Property:
							break;
						default:
							symbol = null;
							break;
					}
				}

				if (symbol != null)
					return symbol;

				current = current is IStructuredTriviaSyntax
					? ((IStructuredTriviaSyntax)current).ParentTrivia.Token.Parent
					: current.Parent;
			}

			return null;
		}

		internal static void Register(ILSpyAddInPackage owner)
		{
			instance = new OpenCodeItemCommand(owner);
		}
	}
}
