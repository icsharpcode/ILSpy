// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.AI;
using ICSharpCode.ILSpy.AI.Decompiler;
using ICSharpCode.ILSpyX.Analyzers;

namespace ICSharpCode.ILSpy.Analyzers
{
	/// <summary>
	/// Desktop MEF adapter for the decompiler-module security analyzer. The analyzer export remains
	/// in the application assembly because the generic <see cref="IAnalyzer"/> contract and
	/// <see cref="AnalyzerContext"/> belong to ILSpyX; the implementation itself stays portable.
	/// </summary>
	[ExportAnalyzer(Header = "Security Risks (AI)", Order = 1000)]
	[Shared]
	public sealed class AISecurityAnalyzerAdapter : IAnalyzer
	{
		readonly AISelectionService selectionService;
		readonly IAIProviderFactory providerFactory;
		readonly AISecurityAnalyzer analyzer;

		[ImportingConstructor]
		public AISecurityAnalyzerAdapter(AISelectionService selectionService, IAIProviderFactory providerFactory)
		{
			this.selectionService = selectionService ?? throw new ArgumentNullException(nameof(selectionService));
			this.providerFactory = providerFactory ?? throw new ArgumentNullException(nameof(providerFactory));
			analyzer = new AISecurityAnalyzer();
		}

		public bool Show(ISymbol? symbol) => symbol is ITypeDefinition or IMethod;

		public IEnumerable<ISymbol> Analyze(ISymbol analyzedSymbol, AnalyzerContext context)
		{
			ITypeDefinition type = analyzedSymbol switch {
				ITypeDefinition definition => definition,
				IMethod method when method.DeclaringTypeDefinition is { } declaringType => declaringType,
				_ => throw new InvalidOperationException("Security analysis requires a type or method.")
			};
			AISelectionSnapshot snapshot = selectionService.ResolveSnapshotAsync(context.CancellationToken).GetAwaiter().GetResult();
			return analyzer.AnalyzeSelectedTypeAsync(type, snapshot, providerFactory,
				cancellationToken: context.CancellationToken).GetAwaiter().GetResult().Cast<ISymbol>();
		}
	}
}
