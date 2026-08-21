// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Composition;
using System.Linq;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.AI.Decompiler;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpyX.AI;
using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpy.AI
{
	[ExportContextMenuEntry(Header = "Explain with AI", Category = "AI", Order = 1000)]
	[Shared]
	public sealed class ExplainContextMenuEntry : IContextMenuEntry
	{
		readonly AISelectionService selectionService;
		readonly AIOutputPaneModel outputPane;
		readonly DockWorkspace dockWorkspace;

		[ImportingConstructor]
		public ExplainContextMenuEntry(SettingsService settingsService, AIOutputPaneModel outputPane, DockWorkspace dockWorkspace, AISelectionService selectionService)
		{
			_ = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
			this.outputPane = outputPane ?? throw new ArgumentNullException(nameof(outputPane));
			this.dockWorkspace = dockWorkspace ?? throw new ArgumentNullException(nameof(dockWorkspace));
			this.selectionService = selectionService ?? throw new ArgumentNullException(nameof(selectionService));
		}

		public bool IsVisible(TextViewContext context) => ResolveEntity(context) is not null;

		public bool IsEnabled(TextViewContext context)
		{
			if (ResolveEntity(context) is null)
				return false;
			return selectionService.CanAttemptRequest;
		}

		public void Execute(TextViewContext context)
		{
			if (!IsEnabled(context) || ResolveEntity(context) is not { } entity)
				return;
			dockWorkspace.ShowToolPane(AIOutputPaneModel.PaneContentId);
			outputPane.StartAsync(entity).HandleExceptions();
		}

		public static IEntity? ResolveEntity(TextViewContext context)
		{
			ArgumentNullException.ThrowIfNull(context);
			IEntity? treeEntity = context.SelectedTreeNodes is { Length: 1 } nodes
				? (nodes[0] as IMemberTreeNode)?.Member
				: null;
			if (IsSupported(treeEntity))
				return treeEntity;

			IEntity? referenceEntity = context.Reference?.Reference as IEntity;
			return IsSupported(referenceEntity) ? referenceEntity : null;
		}

		static bool IsSupported(IEntity? entity)
			=> entity is ITypeDefinition or IMethod or IProperty or IField;
	}
}
