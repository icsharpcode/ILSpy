using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;

namespace ICSharpCode.ILSpy.AddIn.Commands
{
	class OpenCodeItemCommand : ILSpyCommand
	{
		static OpenCodeItemCommand instance;

		public OpenCodeItemCommand(ILSpyAddInPackage owner)
			: base(owner, PkgCmdIDList.cmdidOpenCodeItemInILSpy)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
		}

		protected override void OnBeforeQueryStatus(object sender, EventArgs e)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			if (sender is OleMenuCommand menuItem)
			{
				menuItem.Visible = false;

				// Enable this item only if this is a .cs file!
				if (Utils.GetCurrentViewHost(owner, f => f.EndsWith(".cs")) == null)
					return;

				// Enable this item only if this is a Roslyn document
				if (GetRoslynDocument() == null)
					return;

				var document = owner.DTE.ActiveDocument;
				menuItem.Visible =
					(document?.ProjectItem?.ContainingProject?.ConfigurationManager != null) &&
					!string.IsNullOrEmpty(document.ProjectItem.ContainingProject.FileName);
			}
		}

		Document GetRoslynDocument()
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			var document = owner.DTE.ActiveDocument;
			var id = owner.Workspace.CurrentSolution.GetDocumentIdsWithFilePath(document.FullName).FirstOrDefault();
			if (id == null)
				return null;

			return owner.Workspace.CurrentSolution.GetDocument(id);
		}

		protected override async void OnExecute(object sender, EventArgs e)
		{
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

			var textView = Utils.GetCurrentViewHost(owner)?.TextView;
			if (textView == null)
				return;

			SnapshotPoint caretPosition = textView.Caret.Position.BufferPosition;
			var roslynDocument = GetRoslynDocument();
			if (roslynDocument == null)
			{
				owner.ShowMessage("This element is not analyzable in current view.");
				return;
			}
			var ast = await roslynDocument.GetSyntaxRootAsync().ConfigureAwait(false);
			var model = await roslynDocument.GetSemanticModelAsync().ConfigureAwait(false);
			var node = ast.FindNode(new TextSpan(caretPosition.Position, 0), false, true);
			if (node == null)
			{
				owner.ShowMessage(OLEMSGICON.OLEMSGICON_WARNING, "Can't show ILSpy for this code element!");
				return;
			}

			var symbol = GetSymbolResolvableByILSpy(model, node);
			if (symbol == null)
			{
				owner.ShowMessage(OLEMSGICON.OLEMSGICON_WARNING, "Can't show ILSpy for this code element!");
				return;
			}

			var roslynProject = roslynDocument.Project;
			var refsmap = GetReferences(roslynProject);
			var symbolAssemblyName = symbol.ContainingAssembly?.Identity?.Name;

			// Add our own project as well (not among references)
			var project = FindProject(owner.DTE.Solution.Projects.OfType<EnvDTE.Project>(), roslynProject.FilePath);

			if (project == null)
			{
				owner.ShowMessage(OLEMSGICON.OLEMSGICON_WARNING, "Can't show ILSpy for this code element!");
				return;
			}

			string assemblyName = roslynDocument.Project.AssemblyName;
			string projectOutputPath = Utils.GetProjectOutputAssembly(project, roslynProject);
			refsmap.Add(assemblyName, new DetectedReference(assemblyName, projectOutputPath, true));

			// Divide into valid and invalid (= not found) referenced assemblies
			CheckAssemblies(refsmap, out var validRefs, out var invalidRefs);
			var invalidSymbolReference = invalidRefs.FirstOrDefault(r => r.IsProjectReference && (r.Name == symbolAssemblyName));
			if (invalidSymbolReference != null)
			{
				if (string.IsNullOrEmpty(invalidSymbolReference.AssemblyFile))
				{
					// No assembly file given at all. This has been seen while project is still loading after opening...
					owner.ShowMessage(OLEMSGICON.OLEMSGICON_WARNING,
						"Symbol can't be opened. This might happen while project is loading.",
						Environment.NewLine, invalidSymbolReference.AssemblyFile);
				}
				else if (invalidSymbolReference.IsProjectReference)
				{
					// Some project references don't have assemblies, maybe not compiled yet?
					if (owner.ShowMessage(
						OLEMSGBUTTON.OLEMSGBUTTON_YESNO, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST, OLEMSGICON.OLEMSGICON_WARNING,
						"The project output for '{1}' could not be found for analysis.{0}{0}Expected path:{0}{0}{2}{0}{0}Would you like to build the solution?",
						Environment.NewLine, symbolAssemblyName, invalidSymbolReference.AssemblyFile
						) == (int)MessageButtonResult.IDYES)
					{
						owner.DTE.ExecuteCommand("Build.BuildSolution");
					}
				}
				else
				{
					// External assembly is missing, we should abort
					owner.ShowMessage(OLEMSGICON.OLEMSGICON_WARNING,
						"Referenced assembly{0}{0}'{1}'{0}{0} could not be found.",
						Environment.NewLine, invalidSymbolReference.AssemblyFile);
				}

				return;
			}

			OpenAssembliesInILSpy(new ILSpyParameters(validRefs.Select(r => r.AssemblyFile), "/navigateTo:" +
				(symbol.OriginalDefinition ?? symbol).GetDocumentationCommentId()));
		}

		void CheckAssemblies(Dictionary<string, DetectedReference> inputReferenceList,
			out List<DetectedReference> validRefs,
			out List<DetectedReference> invalidRefs)
		{
			validRefs = new List<DetectedReference>();
			invalidRefs = new List<DetectedReference>();

			foreach (var reference in inputReferenceList.Select(r => r.Value))
			{
				if ((reference.AssemblyFile == null) || !File.Exists(reference.AssemblyFile))
				{
					invalidRefs.Add(reference);
				}
				else
				{
					validRefs.Add(reference);
				}
			}
		}

		ISymbol GetSymbolResolvableByILSpy(SemanticModel model, SyntaxNode node)
		{
			var current = node;
			while (current != null)
			{
				var symbol = model.GetSymbolInfo(current).Symbol;
				if (symbol == null)
				{
					symbol = model.GetDeclaredSymbol(current);
				}

				// ILSpy can only resolve some symbol types, so allow them, discard everything else
				if (symbol != null)
				{
					switch (symbol.Kind)
					{
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
			ThreadHelper.ThrowIfNotOnUIThread();

			instance = new OpenCodeItemCommand(owner);
		}
	}
}
