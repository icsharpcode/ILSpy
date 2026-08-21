// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Threading;
using System.Threading.Tasks;

namespace ICSharpCode.ILSpy.AI
{
	public interface IAIProviderFactory
	{
		/// <summary>
		/// Creates the provider for an immutable resolved target. No mutable settings are read;
		/// in-flight requests are unaffected by later configuration changes.
		/// </summary>
		Task<ILLMProvider> CreateAsync(AISelectionSnapshot snapshot, CancellationToken cancellationToken = default);
	}

	public sealed class AIConfigurationException : Exception
	{
		public AIConfigurationException(string message) : base(message) { }
	}
}
