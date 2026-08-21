// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.AI.Decompiler;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.AI;
using ICSharpCode.ILSpyX.Analyzers;
using ICSharpCode.ILSpyX.Analyzers.Builtin;

namespace ICSharpCode.ILSpy.AI
{
	public interface IAIChatFeatureCommands
	{
		Task<string> RunAuditAsync(CancellationToken cancellationToken);
		Task<string> RunSummaryAsync(CancellationToken cancellationToken);
		Task<IAsyncEnumerable<string>?> RunExplainAsync(string? focusText, CancellationToken cancellationToken);
		Task<string> RunRenameAsync(string? namingHint, CancellationToken cancellationToken);
	}

	[Export(typeof(IAIChatFeatureCommands))]
	[Shared]
	public sealed class AIChatFeatureCommands : IAIChatFeatureCommands
	{
		readonly AssemblyTreeModel assemblyTree;
		readonly AISelectionService selectionService;
		readonly IAIProviderFactory providerFactory;

		[ImportingConstructor]
		public AIChatFeatureCommands(AssemblyTreeModel assemblyTree, AISelectionService selectionService, IAIProviderFactory providerFactory)
		{
			this.assemblyTree = assemblyTree ?? throw new ArgumentNullException(nameof(assemblyTree));
			this.selectionService = selectionService ?? throw new ArgumentNullException(nameof(selectionService));
			this.providerFactory = providerFactory ?? throw new ArgumentNullException(nameof(providerFactory));
		}

		public async Task<string> RunAuditAsync(CancellationToken cancellationToken)
		{
			ITypeDefinition? type = ResolveSelectedType();
			if (type is null)
				return "/audit requires a selected type or method. Select one in the assembly tree and try again.";

			AISelectionSnapshot snapshot = await selectionService.ResolveSnapshotAsync(cancellationToken).ConfigureAwait(false);
			IReadOnlyList<AISecurityFinding> findings = await new AISecurityAnalyzer().AnalyzeSelectedTypeAsync(type, snapshot, providerFactory, cancellationToken: cancellationToken).ConfigureAwait(false);
			if (findings.Count == 0)
				return $"/audit completed for {type.FullName}: no findings met the 70% confidence threshold.";

			string formatted = string.Join(Environment.NewLine, findings.Select(f => $"- [{f.Severity}] {f.Issue} ({f.Confidence * 100:0}% confidence)"));
			return $"/audit completed for {type.FullName}:{Environment.NewLine}{formatted}";
		}

		public async Task<string> RunSummaryAsync(CancellationToken cancellationToken)
		{
			LoadedAssembly? assembly = ResolveSelectedAssembly();
			if (assembly is null)
				return "/summary requires a selected assembly or symbol. Select one in the assembly tree and try again.";

			AISelectionSnapshot snapshot = await selectionService.ResolveSnapshotAsync(cancellationToken).ConfigureAwait(false);
			var service = new AIExplanationService(snapshot, providerFactory);
			AIOutputPaneModel outputPane = AppComposition.TryGetExport<AIOutputPaneModel>()
				?? throw new InvalidOperationException("The AI Output pane is unavailable. Open it from the View menu and try again.");
			DockWorkspace dockWorkspace = AppComposition.TryGetExport<DockWorkspace>()
				?? throw new InvalidOperationException("The docking workspace is unavailable. Try again after the main window is ready.");
			dockWorkspace.ShowToolPane(AIOutputPaneModel.PaneContentId);
			_ = outputPane.StartAsync(assembly.ShortName, token => AssemblySummaryContextMenuEntry.BuildAndCompleteAsync(assembly, service, snapshot, token));
			return $"Assembly summary started in the AI Output pane for {assembly.ShortName}.";
		}

		public async Task<IAsyncEnumerable<string>?> RunExplainAsync(string? focusText, CancellationToken cancellationToken)
		{
			IEntity? entity = ResolveSelectedEntity();
			if (entity is null)
				return null;

			var decompiler = AIEntityDecompilation.CreateDecompiler(entity);
			IEntity resolvedEntity = AIEntityDecompilation.ResolveEntity(entity, decompiler)
				?? throw new InvalidOperationException($"Failed to resolve entity '{entity.FullName}' in the decompiler type system.");
			AISelectionSnapshot snapshot = await selectionService.ResolveSnapshotAsync(cancellationToken).ConfigureAwait(false);
			DecompilationContext context = await Task.Run(
				() => new ContextBuilder(snapshot).Build(resolvedEntity, decompiler),
				cancellationToken).ConfigureAwait(false);
			return new AIExplanationService(snapshot, providerFactory).ExplainContextStreamingAsync(context, focusText, cancellationToken);
		}

		public async Task<string> RunRenameAsync(string? namingHint, CancellationToken cancellationToken)
		{
			IEntity? entity = ResolveSelectedEntity();
			if (entity is null)
				return "/rename requires a selected type, method, property, or field. Select one in the assembly tree and try again.";
			if (!RenameSuggester.IsLikelyObfuscated(entity.Name))
				return $"/rename targets obfuscated names (e.g. method_1234); '{entity.Name}' does not look obfuscated. Ask in plain chat for general naming advice.";

			var decompiler = AIEntityDecompilation.CreateDecompiler(entity);
			IEntity resolvedEntity = AIEntityDecompilation.ResolveEntity(entity, decompiler)
				?? throw new InvalidOperationException($"Failed to resolve entity '{entity.FullName}' in the decompiler type system.");
			AISelectionSnapshot snapshot = await selectionService.ResolveSnapshotAsync(cancellationToken).ConfigureAwait(false);
			IReadOnlyList<RenameSuggestion> suggestions = await new RenameSuggester(snapshot, providerFactory)
				.SuggestAsync(resolvedEntity, decompiler, additionalContext: null, namingHint, cancellationToken)
				.ConfigureAwait(false);
			return FormatRenameSuggestions(entity, suggestions);
		}

		public static string FormatRenameSuggestions(IEntity entity, IReadOnlyList<RenameSuggestion> suggestions)
		{
			ArgumentNullException.ThrowIfNull(entity);
			ArgumentNullException.ThrowIfNull(suggestions);
			var builder = new StringBuilder();
			builder.Append("Rename candidates for ").Append(entity.FullName).AppendLine(":");
			foreach (RenameSuggestion suggestion in suggestions)
			{
				builder.AppendLine();
				builder.Append("- **").Append(suggestion.Name).Append("** (").Append(suggestion.ConfidencePercent).Append("%) — ")
					.Append(string.IsNullOrWhiteSpace(suggestion.Reasoning) ? "no reasoning provided" : suggestion.Reasoning);
			}
			builder.AppendLine();
			builder.AppendLine();
			builder.Append("Apply one via the assembly tree context menu → \"Suggest Name with AI\".");
			return builder.ToString();
		}

		IEntity? ResolveSelectedEntity()
		{
			var entity = (assemblyTree.SelectedItem as IMemberTreeNode)?.Member;
			return entity is ITypeDefinition or IMethod or IProperty or IField ? entity : null;
		}

		ITypeDefinition? ResolveSelectedType()
		{
			IEntity? entity = (assemblyTree.SelectedItem as IMemberTreeNode)?.Member;
			return entity switch { ITypeDefinition type => type, IMethod method => method.DeclaringTypeDefinition, _ => null };
		}

		LoadedAssembly? ResolveSelectedAssembly()
		{
			if (assemblyTree.SelectedItem is AssemblyTreeNode assemblyNode)
				return assemblyNode.LoadedAssembly;
			return (assemblyTree.SelectedItem as ILSpyTreeNode)?.AncestorsAndSelf().OfType<AssemblyTreeNode>().FirstOrDefault()?.LoadedAssembly;
		}
	}
}
