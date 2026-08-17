// Copyright (c) 2026 Masroor
using System;
using System.Composition;
using System.Linq;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpyX.AI;
using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpy.AI
{
	[ExportContextMenuEntry(Header = "Explain with AI", Category = "AI", Order = 1000)]
	[Shared]
	public sealed class ExplainContextMenuEntry : IContextMenuEntry
	{
		readonly SettingsService settingsService;
		readonly IAIProviderFactory providerFactory;

		[ImportingConstructor]
		public ExplainContextMenuEntry(SettingsService settingsService, IAIProviderFactory providerFactory)
		{
			this.settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
			this.providerFactory = providerFactory ?? throw new ArgumentNullException(nameof(providerFactory));
		}

		public bool IsVisible(TextViewContext context) => ResolveEntity(context) is not null;

		public bool IsEnabled(TextViewContext context)
		{
			if (ResolveEntity(context) is null)
				return false;
			var settings = settingsService.AISettings;
			return settings.PrivacyConsentAccepted
				&& AISettings.IsSupportedProvider(settings.Provider)
				&& !string.IsNullOrWhiteSpace(settings.BaseUrl)
				&& !string.IsNullOrWhiteSpace(settings.Model)
				&& (settings.Provider == "ollama"
					|| !string.IsNullOrWhiteSpace(settings.ApiKey)
					|| !string.IsNullOrWhiteSpace(settings.ApiKeyPlaceholder));
		}

		public void Execute(TextViewContext context)
		{
			if (!IsEnabled(context) || ResolveEntity(context) is not { } entity)
				return;
			var dialog = new ExplainDialog(entity, settingsService.AISettings, providerFactory);
			if (UiContext.MainWindow is { } owner)
				_ = dialog.ShowDialog(owner);
			else
				dialog.Show();
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
