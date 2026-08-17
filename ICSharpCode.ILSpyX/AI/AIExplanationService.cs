// Copyright (c) 2026 Masroor
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpyX.AI
{
	public sealed class AIExplanationService
	{
		public const string SystemPrompt = "You explain decompiled .NET code concisely. State uncertainty when context is incomplete. Never instruct the user to execute code.";

		readonly AISettings settings;
		readonly IAIProviderFactory providerFactory;

		public AIExplanationService(AISettings settings, IAIProviderFactory providerFactory)
		{
			this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
			this.providerFactory = providerFactory ?? throw new ArgumentNullException(nameof(providerFactory));
		}

		public async IAsyncEnumerable<string> ExplainStreamingAsync(
			IEntity entity,
			CSharpDecompiler decompiler,
			[EnumeratorCancellation] CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(entity);
			ArgumentNullException.ThrowIfNull(decompiler);
			DecompilationContext context = await Task.Run(
				() => new ContextBuilder(settings).Build(entity, decompiler),
				cancellationToken).ConfigureAwait(false);
			await foreach (string chunk in ExplainContextStreamingAsync(context, cancellationToken).ConfigureAwait(false))
				yield return chunk;
		}

		public async Task<string> ExplainAsync(IEntity entity, CSharpDecompiler decompiler, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(entity);
			ArgumentNullException.ThrowIfNull(decompiler);
			DecompilationContext context = new ContextBuilder(settings).Build(entity, decompiler);
			return await ExplainContextAsync(context, cancellationToken).ConfigureAwait(false);
		}

		public IAsyncEnumerable<string> ExplainContextStreamingAsync(
			DecompilationContext context,
			CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(context);
			return CompleteStreamingAsync(
				SystemPrompt,
				"Explain this selected symbol:\n\n" + context.ToMarkdown(),
				cancellationToken);
		}

		public IAsyncEnumerable<string> CompleteStreamingAsync(
			string systemPrompt,
			string userPrompt,
			CancellationToken cancellationToken = default)
		{
			return CompleteStreamingCoreAsync(systemPrompt, userPrompt, cancellationToken);
		}

		async IAsyncEnumerable<string> CompleteStreamingCoreAsync(
			string systemPrompt,
			string userPrompt,
			[EnumeratorCancellation] CancellationToken cancellationToken)
		{
			ILLMProvider provider;
			try
			{
				EnsureConsent();
				provider = await providerFactory.CreateAsync(settings, cancellationToken).ConfigureAwait(false);
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (AIConfigurationException)
			{
				throw;
			}
			catch (AIRequestException)
			{
				throw;
			}
			catch (Exception exception)
			{
				throw new AIRequestException(ClassifyError(exception), exception);
			}

			var request = new LLMRequest(systemPrompt, new[] { new LLMMessage("user", userPrompt) }, maxTokens: 2048, temperature: 0.2);
			IAsyncEnumerator<string> enumerator;
			try
			{
				enumerator = provider.CompleteAsync(request, cancellationToken).GetAsyncEnumerator(cancellationToken);
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (AIRequestException)
			{
				throw;
			}
			catch (Exception exception)
			{
				throw new AIRequestException(ClassifyError(exception), exception);
			}

			try
			{
				while (true)
				{
					bool hasChunk;
					try
					{
						hasChunk = await enumerator.MoveNextAsync().ConfigureAwait(false);
					}
					catch (OperationCanceledException)
					{
						throw;
					}
					catch (AIRequestException)
					{
						throw;
					}
					catch (Exception exception)
					{
						throw new AIRequestException(ClassifyError(exception), exception);
					}

					if (!hasChunk)
						break;
					if (!string.IsNullOrEmpty(enumerator.Current))
						yield return enumerator.Current;
				}
			}
			finally
			{
				await enumerator.DisposeAsync().ConfigureAwait(false);
			}
		}

		public async Task<string> ExplainContextAsync(DecompilationContext context, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(context);
			EnsureConsent();
			var chunks = new List<string>();
			try
			{
				await foreach (string chunk in ExplainContextStreamingAsync(context, cancellationToken).ConfigureAwait(false))
					chunks.Add(chunk);
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (AIRequestException)
			{
				throw;
			}
			catch (Exception exception)
			{
				throw new AIRequestException(ClassifyError(exception), exception);
			}
			return string.Concat(chunks);
		}

		public async Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default)
		{
			EnsureConsent();
			ILLMProvider provider = await providerFactory.CreateAsync(settings, cancellationToken).ConfigureAwait(false);
			try
			{
				return await provider.TestConnectionAsync(cancellationToken).ConfigureAwait(false);
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (Exception exception)
			{
				throw new AIRequestException(ClassifyError(exception), exception);
			}
		}

		void EnsureConsent()
		{
			if (!settings.PrivacyConsentAccepted)
				throw new AIConfigurationException("Accept the privacy notice before using AI.");
		}

		static string ClassifyError(Exception exception)
		{
			if (exception is AIConfigurationException)
				return exception.Message;
			if (exception is HttpRequestException http && http.StatusCode is { } status)
			{
				return (int)status switch {
					401 or 403 => "The AI provider rejected the API key.",
					404 => "The AI endpoint or model was not found.",
					408 or 429 => "The AI provider is busy or rate-limited. Try again later.",
					_ when (int)status >= 500 => "The AI provider reported a server error. Try again later.",
					_ => "The AI provider rejected the request. Check endpoint and model settings."
				};
			}
			return exception is FormatException or System.Text.Json.JsonException
				? "The AI provider returned an invalid response."
				: "The AI request failed. Check provider settings and try again.";
		}
	}

	public sealed class AIRequestException : Exception
	{
		public AIRequestException(string message, Exception innerException) : base(message, innerException) { }
	}
}
