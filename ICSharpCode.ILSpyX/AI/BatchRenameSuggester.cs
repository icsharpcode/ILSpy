// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.AI;
using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpyX.AI
{
	/// <summary>Progress for one batch-rename suggestion run.</summary>
	public sealed record BatchRenameProgress(
		int Completed,
		int Total,
		string? CurrentMember,
		int SkippedOrErrorCount);

	public sealed record BatchRenameItem(
		IEntity Entity,
		string OldName,
		IReadOnlyList<RenameSuggestion> Suggestions,
		string? Error = null)
	{
		public bool HasSuggestions => Suggestions.Count != 0;
	}

	/// <summary>Generates reviewed rename candidates in a stable dependency-aware order.</summary>
	public sealed class BatchRenameSuggester
	{
		readonly RenameSuggester suggester;

		///<summary>Creates a batch suggester bound to an immutable request target.</summary>
		public BatchRenameSuggester(AISelectionSnapshot snapshot, IAIProviderFactory providerFactory)
		{
			suggester = new RenameSuggester(snapshot ?? throw new ArgumentNullException(nameof(snapshot)), providerFactory ?? throw new ArgumentNullException(nameof(providerFactory)));
		}

		public async Task<IReadOnlyList<BatchRenameItem>> SuggestAsync(
			ITypeDefinition type,
			CSharpDecompiler decompiler,
			IProgress<BatchRenameProgress>? progress = null,
			CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(type);
			ArgumentNullException.ThrowIfNull(decompiler);

			var eligibleMembers = OrderMembers(type)
				.Where(member => RenameSuggester.IsLikelyObfuscated(member.Name))
				.ToArray();
			var items = new List<BatchRenameItem>(eligibleMembers.Length);
			var proposedRenames = new List<string>();
			int skippedOrErrorCount = 0;
			int completed = 0;
			progress?.Report(new BatchRenameProgress(0, eligibleMembers.Length, null, 0));
			foreach (IEntity member in eligibleMembers)
			{
				cancellationToken.ThrowIfCancellationRequested();
				progress?.Report(new BatchRenameProgress(completed, eligibleMembers.Length, member.FullName, skippedOrErrorCount));

				try
				{
					string relatedContext = string.Join("\n", proposedRenames);
					IReadOnlyList<RenameSuggestion> suggestions = await suggester
						.SuggestAsync(member, decompiler, relatedContext, cancellationToken)
						.ConfigureAwait(false);
					items.Add(new BatchRenameItem(member, member.Name, suggestions));
					if (suggestions.Count > 0)
						proposedRenames.Add($"{member.FullName} -> {suggestions[0].Name}");
				}
				catch (RenameSuggestionParseException exception)
				{
					items.Add(new BatchRenameItem(member, member.Name, Array.Empty<RenameSuggestion>(), exception.Message));
					skippedOrErrorCount++;
				}
				completed++;
				progress?.Report(new BatchRenameProgress(completed, eligibleMembers.Length, member.FullName, skippedOrErrorCount));
			}
			return items;
		}

		public static IReadOnlyList<IEntity> OrderMembers(ITypeDefinition type)
		{
			ArgumentNullException.ThrowIfNull(type);
			var result = new List<IEntity>();
			result.AddRange(type.Fields.OrderBy(GetToken));
			result.AddRange(type.Properties.OrderBy(GetToken));

			IMethod[] methods = type.Methods.Where(method => !method.IsAccessor).OrderBy(GetToken).ToArray();
			var methodsByToken = methods.ToDictionary(method => method.MetadataToken);
			var state = new Dictionary<IMethod, VisitState>();
			foreach (IMethod method in methods)
				Visit(method, methodsByToken, state, result);
			return result;
		}

		static void Visit(
			IMethod method,
			IReadOnlyDictionary<System.Reflection.Metadata.EntityHandle, IMethod> methodsByToken,
			IDictionary<IMethod, VisitState> state,
			ICollection<IEntity> result)
		{
			if (state.TryGetValue(method, out VisitState current))
			{
				if (current is VisitState.Visited or VisitState.Visiting)
					return;
			}
			state[method] = VisitState.Visiting;
			if (method.ParentModule is MetadataModule module)
			{
				foreach (IMethod dependency in ContextBuilder.ScanMethodReferences(method, module).OfType<IMethod>())
				{
					if (methodsByToken.TryGetValue(dependency.MetadataToken, out IMethod? localDependency))
						Visit(localDependency, methodsByToken, state, result);
				}
			}
			state[method] = VisitState.Visited;
			result.Add(method);
		}

		static int GetToken(IEntity entity) => System.Reflection.Metadata.Ecma335.MetadataTokens.GetToken(entity.MetadataToken);

		enum VisitState
		{
			Visiting,
			Visited
		}
	}
}
