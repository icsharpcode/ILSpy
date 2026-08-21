// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Composition;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Controls;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.AI.Decompiler;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpyX.AI;

namespace ICSharpCode.ILSpy.AI
{
	[ExportContextMenuEntry(Header = "Suggest Name with AI", Category = "AI", Order = 1020)]
	[Shared]
	public sealed class RenameAssistantContextMenuEntry : IContextMenuEntry
	{
		readonly IAIProviderFactory providerFactory;
		readonly AISelectionService selectionService;

		[ImportingConstructor]
		public RenameAssistantContextMenuEntry(IAIProviderFactory providerFactory, AISelectionService selectionService)
		{
			this.providerFactory = providerFactory ?? throw new ArgumentNullException(nameof(providerFactory));
			this.selectionService = selectionService ?? throw new ArgumentNullException(nameof(selectionService));
		}

		public bool IsVisible(TextViewContext context) => ResolveEntity(context) is not null;

		public bool IsEnabled(TextViewContext context)
		{
			if (ResolveEntity(context) is not { } entity)
				return false;
			return RenameSuggester.IsLikelyObfuscated(entity.Name) && selectionService.CanAttemptRequest;
		}

		public void Execute(TextViewContext context)
		{
			if (!IsEnabled(context) || ResolveEntity(context) is not { } entity)
				return;
			_ = ShowRenameAsync(entity, context);
		}

		async Task ShowRenameAsync(IEntity entity, TextViewContext context)
		{
			AISelectionSnapshot snapshot;
			try
			{ snapshot = await selectionService.ResolveSnapshotAsync(); }
			catch (AIConfigurationException) { return; }
			var dialog = new RenameDialog(entity, snapshot, providerFactory);
			Window? owner = context.OriginalSource is Visual visual ? TopLevel.GetTopLevel(visual) as Window : null;
			if (owner is not null)
				_ = dialog.ShowDialog(owner);
			else
				dialog.Show();
		}

		public static IEntity? ResolveEntity(TextViewContext context)
		{
			ArgumentNullException.ThrowIfNull(context);
			IEntity? entity = context.SelectedTreeNodes is { Length: 1 } nodes
				? (nodes[0] as IMemberTreeNode)?.Member
				: context.Reference?.Reference as IEntity;
			return entity is ITypeDefinition or IMethod or IField or IProperty ? entity : null;
		}

	}

	[ExportContextMenuEntry(Header = "Batch Rename All Members with AI", Category = "AI", Order = 1021)]
	[Shared]
	public sealed class BatchRenameContextMenuEntry : IContextMenuEntry
	{
		readonly IAIProviderFactory providerFactory;
		readonly AISelectionService selectionService;

		[ImportingConstructor]
		public BatchRenameContextMenuEntry(IAIProviderFactory providerFactory, AISelectionService selectionService)
		{
			this.providerFactory = providerFactory ?? throw new ArgumentNullException(nameof(providerFactory));
			this.selectionService = selectionService ?? throw new ArgumentNullException(nameof(selectionService));
		}

		public bool IsVisible(TextViewContext context) => ResolveType(context) is not null;

		public bool IsEnabled(TextViewContext context)
			=> ResolveType(context) is not null && selectionService.CanAttemptRequest;

		public void Execute(TextViewContext context)
		{
			if (!IsEnabled(context) || ResolveType(context) is not { } type)
				return;
			_ = ShowBatchRenameAsync(type, context);
		}

		async Task ShowBatchRenameAsync(ITypeDefinition type, TextViewContext context)
		{
			AISelectionSnapshot snapshot;
			try
			{ snapshot = await selectionService.ResolveSnapshotAsync(); }
			catch (AIConfigurationException) { return; }
			var dialog = new BatchRenameDialog(type, snapshot, providerFactory);
			Window? owner = context.OriginalSource is Visual visual ? TopLevel.GetTopLevel(visual) as Window : null;
			if (owner is not null)
				_ = dialog.ShowDialog(owner);
			else
				dialog.Show();
		}

		static ITypeDefinition? ResolveType(TextViewContext context)
		{
			ArgumentNullException.ThrowIfNull(context);
			IEntity? entity = context.SelectedTreeNodes is { Length: 1 } nodes
				? (nodes[0] as IMemberTreeNode)?.Member
				: context.Reference?.Reference as IEntity;
			return entity as ITypeDefinition;
		}
	}
}
