// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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

using System.Composition;
using System.Linq;

using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Commands
{
	/// <summary>
	/// Right-click → "Decompile to new tab" — opens a fresh document tab and decompiles the
	/// selected tree node(s) into it without disturbing the existing tab. Useful for
	/// side-by-side comparison or to keep your bookmarked code visible while exploring
	/// somewhere else.
	/// </summary>
	[ExportContextMenuEntry(
		Header = nameof(Resources.DecompileToNewPanel),
		Category = "Navigate",
		Icon = "Images/ViewCode",
		InputGestureText = "MMB",
		Order = 100)]
	[Shared]
	internal sealed class DecompileInNewViewCommand : IContextMenuEntry
	{
		readonly DockWorkspace dockWorkspace;
		readonly LanguageService languageService;

		[ImportingConstructor]
		public DecompileInNewViewCommand(DockWorkspace dockWorkspace, LanguageService languageService)
		{
			this.dockWorkspace = dockWorkspace;
			this.languageService = languageService;
		}

		public bool IsVisible(TextViewContext context)
			=> SelectedNodes(context).Any() || ReferencedEntity(context) is not null;

		public bool IsEnabled(TextViewContext context) => IsVisible(context);

		public void Execute(TextViewContext context)
		{
			var nodes = SelectedNodes(context).ToArray();
			if (nodes.Length == 1)
			{
				// Route a single node through OpenNodeInNewTab so nodes with custom content
				// (metadata tables, resource viewers) open their own page-type instead of being
				// force-decompiled into an empty code tab. Plain nodes still get a decompiler tab,
				// sourced from the node so the tab/tree stay in lockstep when flipping tabs.
				dockWorkspace.OpenNodeInNewTab(nodes[0]);
				return;
			}
			if (nodes.Length > 1)
			{
				// Multi-node selections leave SourceNode null — no single tree row represents the
				// union — and always decompile (there is no custom-content union page).
				var content = new DecompilerTabPageModel { Language = languageService.CurrentLanguage };
				dockWorkspace.OpenNewTab(content, sourceNode: null);
				content.CurrentNodes = nodes;
				return;
			}
			// Right-click on a symbol in the decompiled code: open its definition in a new tab. The
			// navigate handler resolves the entity to its tree node and opens it via OpenNodeInNewTab.
			if (ReferencedEntity(context) is { } entity)
				Util.MessageBus.Send(this, new Util.NavigateToReferenceEventArgs(entity, inNewTabPage: true));
		}

		static System.Collections.Generic.IEnumerable<ILSpyTreeNode> SelectedNodes(TextViewContext context)
			=> context.SelectedTreeNodes?.OfType<ILSpyTreeNode>()
				?? System.Linq.Enumerable.Empty<ILSpyTreeNode>();

		static ICSharpCode.Decompiler.TypeSystem.IEntity? ReferencedEntity(TextViewContext context)
			=> context.Reference?.Reference as ICSharpCode.Decompiler.TypeSystem.IEntity;
	}
}
