// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Collections.Generic;
using System.Composition;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.AI;
using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpy.AI
{
	[ExportContextMenuEntry(Header = "Summarize Assembly with AI", Category = "AI", Order = 1010)]
	[Shared]
	public sealed class AssemblySummaryContextMenuEntry : IContextMenuEntry
	{
		readonly IAIProviderFactory providerFactory;
		readonly AISelectionService selectionService;
		readonly AIOutputPaneModel outputPane;
		readonly DockWorkspace dockWorkspace;

		[ImportingConstructor]
		public AssemblySummaryContextMenuEntry(SettingsService settingsService, IAIProviderFactory providerFactory, AIOutputPaneModel outputPane, DockWorkspace dockWorkspace, AISelectionService selectionService)
		{
			_ = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
			this.providerFactory = providerFactory ?? throw new ArgumentNullException(nameof(providerFactory));
			this.outputPane = outputPane ?? throw new ArgumentNullException(nameof(outputPane));
			this.dockWorkspace = dockWorkspace ?? throw new ArgumentNullException(nameof(dockWorkspace));
			this.selectionService = selectionService ?? throw new ArgumentNullException(nameof(selectionService));
		}

		public bool IsVisible(TextViewContext context)
			=> context.SelectedTreeNodes is { Length: 1 } && context.SelectedTreeNodes[0] is AssemblyTreeNode;

		public bool IsEnabled(TextViewContext context)
			=> IsVisible(context) && selectionService.CanAttemptRequest;

		public void Execute(TextViewContext context)
		{
			if (!IsEnabled(context) || context.SelectedTreeNodes?[0] is not AssemblyTreeNode node)
				return;
			dockWorkspace.ShowToolPane(AIOutputPaneModel.PaneContentId);
			_ = ExecuteAsync(node);
		}

		async Task ExecuteAsync(AssemblyTreeNode node)
		{
			AISelectionSnapshot snapshot;
			try
			{ snapshot = await selectionService.ResolveSnapshotAsync(); }
			catch (AIConfigurationException) { return; }
			var service = new AIExplanationService(snapshot, providerFactory);
			_ = outputPane.StartAsync(node.LoadedAssembly.ShortName, token => BuildAndCompleteAsync(node.LoadedAssembly, service, token));
		}

		static async IAsyncEnumerable<string> BuildAndCompleteAsync(
			LoadedAssembly assembly,
			AIExplanationService service,
			[EnumeratorCancellation] CancellationToken cancellationToken)
		{
			string markdown = await Task.Run(() => AssemblySummaryContextBuilder.Build(assembly), cancellationToken).ConfigureAwait(false);
			await foreach (string chunk in service.CompleteStreamingAsync(
				"You are analyzing a .NET assembly. Provide a 2-3 paragraph summary: what it is, what framework it targets, what it is probably used for.",
				"Summarize this assembly:\n\n" + markdown,
				cancellationToken).ConfigureAwait(false))
			{
				yield return chunk;
			}
		}
	}
}
