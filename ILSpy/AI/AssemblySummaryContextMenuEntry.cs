// Copyright (c) 2026 Masroor
using System;
using System.Composition;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpyX.AI;
using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpy.AI
{
	[ExportContextMenuEntry(Header = "Summarize Assembly with AI", Category = "AI", Order = 1010)]
	[Shared]
	public sealed class AssemblySummaryContextMenuEntry : IContextMenuEntry
	{
		readonly SettingsService settingsService;
		readonly IAIProviderFactory providerFactory;
		readonly AIOutputPaneModel outputPane;
		readonly DockWorkspace dockWorkspace;

		[ImportingConstructor]
		public AssemblySummaryContextMenuEntry(SettingsService settingsService, IAIProviderFactory providerFactory, AIOutputPaneModel outputPane, DockWorkspace dockWorkspace)
		{
			this.settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
			this.providerFactory = providerFactory ?? throw new ArgumentNullException(nameof(providerFactory));
			this.outputPane = outputPane ?? throw new ArgumentNullException(nameof(outputPane));
			this.dockWorkspace = dockWorkspace ?? throw new ArgumentNullException(nameof(dockWorkspace));
		}

		public bool IsVisible(TextViewContext context)
			=> context.SelectedTreeNodes is { Length: 1 } && context.SelectedTreeNodes[0] is AssemblyTreeNode;

		public bool IsEnabled(TextViewContext context)
		{
			var settings = settingsService.AISettings;
			return IsVisible(context) && settings.PrivacyConsentAccepted && AISettings.IsSupportedProvider(settings.Provider)
				&& !string.IsNullOrWhiteSpace(settings.BaseUrl) && !string.IsNullOrWhiteSpace(settings.Model)
				&& (settings.Provider == "ollama" || !string.IsNullOrWhiteSpace(settings.ApiKey) || !string.IsNullOrWhiteSpace(settings.ApiKeyPlaceholder));
		}

		public void Execute(TextViewContext context)
		{
			if (!IsEnabled(context) || context.SelectedTreeNodes?[0] is not AssemblyTreeNode node)
				return;
			string markdown;
			try
			{
				markdown = AssemblySummaryContextBuilder.Build(node.LoadedAssembly);
			}
			catch (Exception)
			{
				return;
			}
			dockWorkspace.ShowToolPane(AIOutputPaneModel.PaneContentId);
			_ = outputPane.StartAsync(node.LoadedAssembly.ShortName, token => new AIExplanationService(settingsService.AISettings, providerFactory)
				.CompleteStreamingAsync(
					"You are analyzing a .NET assembly. Provide a 2-3 paragraph summary: what it is, what framework it targets, what it is probably used for.",
					"Summarize this assembly:\n\n" + markdown,
					token));
		}
	}
}
