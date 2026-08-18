// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Composition;

using Avalonia;
using Avalonia.Controls;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpyX.AI;
using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpy.AI
{
	[ExportContextMenuEntry(Header = "Suggest Name with AI", Category = "AI", Order = 1020)]
	[Shared]
	public sealed class RenameAssistantContextMenuEntry : IContextMenuEntry
	{
		readonly SettingsService settingsService;
		readonly IAIProviderFactory providerFactory;

		[ImportingConstructor]
		public RenameAssistantContextMenuEntry(SettingsService settingsService, IAIProviderFactory providerFactory)
		{
			this.settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
			this.providerFactory = providerFactory ?? throw new ArgumentNullException(nameof(providerFactory));
		}

		public bool IsVisible(TextViewContext context) => ResolveEntity(context) is not null;

		public bool IsEnabled(TextViewContext context)
		{
			if (ResolveEntity(context) is not { } entity)
				return false;
			AISettings settings = settingsService.AISettings;
			return RenameSuggester.IsLikelyObfuscated(entity.Name) && IsConfigured(settings);
		}

		public void Execute(TextViewContext context)
		{
			if (!IsEnabled(context) || ResolveEntity(context) is not { } entity)
				return;
			var dialog = new RenameDialog(entity, settingsService.AISettings, providerFactory);
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

		internal static bool IsConfigured(AISettings settings)
			=> settings.PrivacyConsentAccepted
				&& AISettings.IsSupportedProvider(settings.Provider)
				&& !string.IsNullOrWhiteSpace(settings.BaseUrl)
				&& !string.IsNullOrWhiteSpace(settings.Model)
				&& (settings.Provider == "ollama" || !string.IsNullOrWhiteSpace(settings.ApiKey) || !string.IsNullOrWhiteSpace(settings.ApiKeyPlaceholder));
	}

	[ExportContextMenuEntry(Header = "Batch Rename All Members with AI", Category = "AI", Order = 1021)]
	[Shared]
	public sealed class BatchRenameContextMenuEntry : IContextMenuEntry
	{
		readonly SettingsService settingsService;
		readonly IAIProviderFactory providerFactory;

		[ImportingConstructor]
		public BatchRenameContextMenuEntry(SettingsService settingsService, IAIProviderFactory providerFactory)
		{
			this.settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
			this.providerFactory = providerFactory ?? throw new ArgumentNullException(nameof(providerFactory));
		}

		public bool IsVisible(TextViewContext context) => ResolveType(context) is not null;

		public bool IsEnabled(TextViewContext context)
			=> ResolveType(context) is not null && RenameAssistantContextMenuEntry.IsConfigured(settingsService.AISettings);

		public void Execute(TextViewContext context)
		{
			if (!IsEnabled(context) || ResolveType(context) is not { } type)
				return;
			var dialog = new BatchRenameDialog(type, settingsService.AISettings, providerFactory);
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
