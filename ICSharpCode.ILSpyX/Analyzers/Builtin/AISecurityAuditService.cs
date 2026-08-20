using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX.AI;

namespace ICSharpCode.ILSpyX.Analyzers.Builtin
{
	/// <summary>Bounded, sequential bulk security audit. The normal analyzer never uses this path.</summary>
	public sealed class AISecurityAuditService
	{
		public const int MaximumTypesPerAudit = 50;

		public AISecurityAuditPlan CreatePlan(IEnumerable<ITypeDefinition> types, int maximumTypes = MaximumTypesPerAudit)
		{
			ArgumentNullException.ThrowIfNull(types);
			if (maximumTypes <= 0)
				throw new ArgumentOutOfRangeException(nameof(maximumTypes));
			var eligible = types.Where(static type => type.ParentModule?.MetadataFile != null)
				.OrderBy(static type => type.FullName, StringComparer.Ordinal)
				.ToArray();
			return new AISecurityAuditPlan(eligible, maximumTypes);
		}

		public async Task<AISecurityAuditResult> RunAsync(
			AISecurityAuditPlan plan,
			AISelectionSnapshot snapshot,
			IAIProviderFactory providerFactory,
			IProgress<AISecurityAuditProgress>? progress = null,
			CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(plan);
			ArgumentNullException.ThrowIfNull(snapshot);
			ArgumentNullException.ThrowIfNull(providerFactory);
			if (plan.IsOverLimit)
				throw new AISecurityAuditLimitException(plan.TotalEligible, plan.MaximumTypes);

			var findings = new List<AISecurityFinding>();
			int failed = 0;
			int completed = 0;
			progress?.Report(new AISecurityAuditProgress(0, plan.Types.Count, null, 0, 0, false));
			foreach (ITypeDefinition type in plan.Types)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					progress?.Report(new AISecurityAuditProgress(completed, plan.Types.Count, type.FullName, findings.Count, failed, true));
					return new AISecurityAuditResult(findings, failed, true);
				}
				progress?.Report(new AISecurityAuditProgress(completed, plan.Types.Count, type.FullName, findings.Count, failed, false));
				try
				{
					findings.AddRange(await AnalyzeTypeAsync(type, snapshot, providerFactory, cancellationToken).ConfigureAwait(false));
				}
				catch (AIConfigurationException)
				{
					throw;
				}
				catch (OperationCanceledException)
				{
					progress?.Report(new AISecurityAuditProgress(completed, plan.Types.Count, type.FullName, findings.Count, failed, true));
					return new AISecurityAuditResult(findings, failed, true);
				}
				catch
				{
					failed++;
				}
				completed++;
				progress?.Report(new AISecurityAuditProgress(completed, plan.Types.Count, type.FullName, findings.Count, failed, false));
			}
			return new AISecurityAuditResult(findings, failed, false);
		}

		static async Task<IReadOnlyList<AISecurityFinding>> AnalyzeTypeAsync(ITypeDefinition type, AISelectionSnapshot snapshot, IAIProviderFactory providerFactory, CancellationToken cancellationToken)
		{
			return await new AISecurityAnalyzer().AnalyzeSelectedTypeAsync(type, snapshot, providerFactory, cancellationToken: cancellationToken).ConfigureAwait(false);
		}
	}

	public sealed class AISecurityAuditPlan
	{
		internal AISecurityAuditPlan(IReadOnlyList<ITypeDefinition> types, int maximumTypes)
		{ Types = types; MaximumTypes = maximumTypes; TotalEligible = types.Count; }
		public IReadOnlyList<ITypeDefinition> Types { get; }
		public int TotalEligible { get; }
		public int MaximumTypes { get; }
		public bool IsOverLimit => TotalEligible > MaximumTypes;
	}

	public sealed record AISecurityAuditProgress(int Completed, int Total, string? CurrentType, int FindingCount, int FailedCount, bool IsPartial);
	public sealed record AISecurityAuditResult(IReadOnlyList<AISecurityFinding> Findings, int FailedCount, bool IsPartial);
	public sealed class AISecurityAuditLimitException : InvalidOperationException
	{
		public AISecurityAuditLimitException(int totalEligible, int maximumTypes) : base($"AI security audit contains {totalEligible} eligible types, exceeding the limit of {maximumTypes}. Reduce the selection or confirm a capped audit.") { TotalEligible = totalEligible; MaximumTypes = maximumTypes; }
		public int TotalEligible { get; }
		public int MaximumTypes { get; }
	}
}
