// Copyright (c) 2026 Dr. Masroor Ehsan

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICSharpCode.ILSpyX.AI
{
	public interface ILLMProvider
	{
		IAsyncEnumerable<string> CompleteAsync(LLMRequest request, CancellationToken cancellationToken);

		Task<bool> TestConnectionAsync(CancellationToken cancellationToken);
	}
}
