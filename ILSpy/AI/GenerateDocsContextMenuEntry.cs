// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Collections.Generic;
using System.Composition;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpyX.AI;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpy.AI
{
	[ExportContextMenuEntry(Header = "Generate XML Documentation", Category = "AI", Order = 1020)]
	[Shared]
	public sealed class GenerateDocsContextMenuEntry : IContextMenuEntry
	{
		readonly IAIProviderFactory providerFactory;
		readonly AISelectionService selectionService;
		readonly AIOutputPaneModel outputPane;
		readonly DockWorkspace dockWorkspace;

		[ImportingConstructor]
		public GenerateDocsContextMenuEntry(SettingsService settingsService, IAIProviderFactory providerFactory, AIOutputPaneModel outputPane, DockWorkspace dockWorkspace, AISelectionService selectionService)
		{
			_ = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
			this.providerFactory = providerFactory ?? throw new ArgumentNullException(nameof(providerFactory));
			this.outputPane = outputPane ?? throw new ArgumentNullException(nameof(outputPane));
			this.dockWorkspace = dockWorkspace ?? throw new ArgumentNullException(nameof(dockWorkspace));
			this.selectionService = selectionService ?? throw new ArgumentNullException(nameof(selectionService));
		}

		public bool IsVisible(TextViewContext context) => ResolveEntity(context) is ITypeDefinition or IMethod;

		public bool IsEnabled(TextViewContext context)
			=> IsVisible(context) && selectionService.CanAttemptRequest;

		public void Execute(TextViewContext context)
		{
			if (!IsEnabled(context) || ResolveEntity(context) is not { } entity || entity.ParentModule?.MetadataFile is not MetadataFile module)
				return;
			dockWorkspace.ShowToolPane(AIOutputPaneModel.PaneContentId);
			_ = ExecuteAsync(entity, module);
		}

		async Task ExecuteAsync(IEntity entity, MetadataFile module)
		{
			AISelectionSnapshot snapshot;
			try
			{ snapshot = await selectionService.ResolveSnapshotAsync(); }
			catch (AIConfigurationException) { return; }
			var decompiler = new CSharpDecompiler(module, module.GetAssemblyResolver(true), new ICSharpCode.Decompiler.DecompilerSettings());
			var service = new AIExplanationService(snapshot, providerFactory);
			_ = outputPane.StartAsync(entity.FullName, token => GenerateAsync(entity, decompiler, service, snapshot, token));
		}

		static async IAsyncEnumerable<string> GenerateAsync(IEntity entity, CSharpDecompiler decompiler, AIExplanationService service, AISelectionSnapshot snapshot, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			var context = await Task.Run(() => new ContextBuilder(snapshot).Build(entity, decompiler), cancellationToken).ConfigureAwait(false);
			var chunks = new StringBuilder();
			await foreach (var chunk in service.CompleteStreamingAsync(
				AIPromptProvider.Instance.GetSystemPrompt("generate_docs", snapshot.Model),
				"Generate <summary>, <param>, <returns>, and exception documentation for this symbol:\n\n" + context.ToMarkdown(), cancellationToken).ConfigureAwait(false))
				chunks.Append(chunk);
			string formatted = FormatDocumentation(chunks.ToString());
			if (formatted.Length != 0)
				yield return formatted;
		}

		internal static string FormatDocumentation(string value)
		{
			value = value.Trim();
			if (value.StartsWith("```", StringComparison.Ordinal))
			{
				int firstNewline = value.IndexOf('\n');
				if (firstNewline >= 0)
					value = value[(firstNewline + 1)..];
				else
					value = string.Empty;
				if (value.EndsWith("```", StringComparison.Ordinal))
					value = value[..^3].TrimEnd();
			}
			var builder = new StringBuilder();
			foreach (string line in value.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None))
			{
				if (line.Trim().Length == 0)
					continue;
				builder.Append("/// ").AppendLine(line.Trim());
			}
			return builder.ToString().TrimEnd();
		}

		static IEntity? ResolveEntity(TextViewContext context)
		{
			IEntity? selected = context.SelectedTreeNodes is { Length: 1 } nodes ? (nodes[0] as IMemberTreeNode)?.Member : null;
			if (selected is ITypeDefinition or IMethod)
				return selected;
			object? reference = context.Reference?.Reference;
			return reference is ITypeDefinition || reference is IMethod ? (IEntity)reference : null;
		}
	}
}
