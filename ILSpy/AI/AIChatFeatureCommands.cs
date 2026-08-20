// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.Decompiler.TypeSystem;
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
	}

	[Export(typeof(IAIChatFeatureCommands))]
	[Shared]
	public sealed class AIChatFeatureCommands : IAIChatFeatureCommands
	{
		readonly AssemblyTreeModel assemblyTree;
		readonly AISelectionService selectionService;
		readonly IAIProviderFactory providerFactory;
		readonly DockWorkspace dockWorkspace;

		[ImportingConstructor]
		public AIChatFeatureCommands(AssemblyTreeModel assemblyTree, AISelectionService selectionService, IAIProviderFactory providerFactory, DockWorkspace dockWorkspace)
		{
			this.assemblyTree = assemblyTree ?? throw new ArgumentNullException(nameof(assemblyTree));
			this.selectionService = selectionService ?? throw new ArgumentNullException(nameof(selectionService));
			this.providerFactory = providerFactory ?? throw new ArgumentNullException(nameof(providerFactory));
			this.dockWorkspace = dockWorkspace ?? throw new ArgumentNullException(nameof(dockWorkspace));
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
			dockWorkspace.ShowToolPane(AIOutputPaneModel.PaneContentId);
			_ = outputPane.StartAsync(assembly.ShortName, token => AssemblySummaryContextMenuEntry.BuildAndCompleteAsync(assembly, service, token));
			return $"Assembly summary started in the AI Output pane for {assembly.ShortName}.";
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
