// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AI.Decompiler;
using ICSharpCode.ILSpy.AI;
using ICSharpCode.ILSpy.AI.Providers;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.AI.Decompiler.Tests.AI
{
	[TestFixture]
	public class AIExplanationServiceTests
	{
		[Test]
		public async Task ExplainContext_ConcatenatesChunksAndSnapshotsRequest()
		{
			var provider = new FakeProvider("first", " second");
			var factory = new FakeFactory(provider);
			var snapshot = Snapshot(maxContextTokens: 4000);
			var service = new AIExplanationService(snapshot, factory);
			var context = new DecompilationContext {
				FullyQualifiedName = "Sample.Type.Method",
				AssemblyName = "Sample",
				DecompiledCSharp = "void Method() { }"
			};

			string explanation = await service.ExplainContextAsync(context);

			explanation.Should().Be("first second");
			factory.LastSnapshot.Should().Be(snapshot);
			provider.LastRequest.Should().NotBeNull();
			provider.LastRequest!.SystemPrompt.Should().Be(AIPromptProvider.Instance.GetSystemPrompt("explanation", snapshot.Model));
			provider.LastRequest.Messages.Should().ContainSingle();
			provider.LastRequest.Messages[0].Content.Should().Contain("Sample.Type.Method");
			provider.LastRequest.Messages[0].Content.Should().Contain("void Method() { }");
		}

		[Test]
		public async Task ExplainContextStreaming_YieldsProviderChunksInOrder()
		{
			var provider = new FakeProvider("one", "two", "three");
			var service = new AIExplanationService(
				Snapshot(),
				new FakeFactory(provider));
			var chunks = new List<string>();
			await foreach (string chunk in service.ExplainContextStreamingAsync(new DecompilationContext { DecompiledCSharp = "class C {}" }))
				chunks.Add(chunk);

			chunks.Should().Equal("one", "two", "three");
		}

		[Test]
		public async Task ExplainContextStreaming_HandlesDelayedProviderChunksInOrder()
		{
			var provider = new FakeProvider(TimeSpan.FromMilliseconds(1), "one", "two", "three");
			var service = new AIExplanationService(
				Snapshot(),
				new FakeFactory(provider));
			var chunks = new List<string>();

			await foreach (string chunk in service.ExplainContextStreamingAsync(new DecompilationContext { DecompiledCSharp = "class C {}" }))
				chunks.Add(chunk);

			chunks.Should().Equal("one", "two", "three");
		}

		[Test]
		public async Task ExplainContextStreaming_WithFocusText_IncludesFocusInPrompt()
		{
			var provider = new FakeProvider("answer");
			var service = new AIExplanationService(
				Snapshot(),
				new FakeFactory(provider));
			var context = new DecompilationContext { DecompiledCSharp = "class C {}" };

			await foreach (string _ in service.ExplainContextStreamingAsync(context, "thread safety"))
			{
			}

			provider.LastRequest!.Messages.Should().ContainSingle();
			provider.LastRequest.Messages[0].Content.Should().StartWith("Explain this selected symbol. Focus on: thread safety:\n\n");
		}

		[Test]
		public async Task ExplainContextStreaming_WithoutFocusText_KeepsStandardHeading()
		{
			var provider = new FakeProvider("answer");
			var service = new AIExplanationService(
				Snapshot(),
				new FakeFactory(provider));
			var context = new DecompilationContext { DecompiledCSharp = "class C {}" };

			await foreach (string _ in service.ExplainContextStreamingAsync(context, focusText: null))
			{
			}

			provider.LastRequest!.Messages.Should().ContainSingle();
			provider.LastRequest.Messages[0].Content.Should().StartWith("Explain this selected symbol:\n\n");
		}

		[Test]
		public void ExplainContext_RequiresConsentBeforeProviderCreation()
		{
			var factory = new FakeFactory(new FakeProvider("unused"));
			var service = new AIExplanationService(new AISelectionSnapshot(), factory);
			var context = new DecompilationContext { DecompiledCSharp = "class C {}" };

			Assert.ThrowsAsync<AIConfigurationException>(async () => await service.ExplainContextAsync(context));
			factory.CreateCount.Should().Be(0);
		}

		[Test]
		public async Task ProviderHttpFailure_IsMappedWithoutLeakingBody()
		{
			var provider = new FakeProvider(new HttpRequestException("secret-api-key", null, HttpStatusCode.Unauthorized));
			var service = new AIExplanationService(
				Snapshot(),
				new FakeFactory(provider));

			AIRequestException exception = Assert.ThrowsAsync<AIRequestException>(
				async () => await service.ExplainContextAsync(new DecompilationContext { DecompiledCSharp = "class C {}" }))!;
			exception.Message.Should().Be("The AI provider rejected the API key.");
			exception.Message.Should().NotContain("secret-api-key");
		}

		[Test]
		public void StreamingProviderHttpFailure_IsMappedWithoutLeakingBody()
		{
			var provider = new FakeProvider(new HttpRequestException("secret-api-key", null, HttpStatusCode.Unauthorized));
			var service = new AIExplanationService(
				Snapshot(),
				new FakeFactory(provider));

			AIRequestException exception = Assert.ThrowsAsync<AIRequestException>(async () => {
				await foreach (string _ in service.ExplainContextStreamingAsync(new DecompilationContext { DecompiledCSharp = "class C {}" }))
				{
				}
			})!;
			exception.Message.Should().Be("The AI provider rejected the API key.");
			exception.Message.Should().NotContain("secret-api-key");
		}

		[Test]
		public async Task Cancellation_IsForwardedToProvider()
		{
			using var cancellation = new CancellationTokenSource();
			var provider = new FakeProvider(async token => {
				cancellation.Cancel();
				await Task.Delay(Timeout.InfiniteTimeSpan, token);
			});
			var service = new AIExplanationService(
				Snapshot(),
				new FakeFactory(provider));

			Assert.ThrowsAsync<TaskCanceledException>(
				async () => await service.ExplainContextAsync(new DecompilationContext { DecompiledCSharp = "class C {}" }, cancellation.Token));
			provider.LastCancellationToken.CanBeCanceled.Should().BeTrue();
		}

		[Test]
		public async Task ProviderFactory_CreatesAnthropicProviderWithoutNetworkAccess()
		{
			using var factory = new AIProviderFactory(loggerFactory: null);
			ILLMProvider provider = await factory.CreateAsync(Snapshot("anthropic", "test-key"));
			Assert.That(provider, Is.TypeOf<AnthropicProvider>());
		}

		sealed class FakeFactory : IAIProviderFactory
		{
			readonly ILLMProvider provider;
			public FakeFactory(ILLMProvider provider) => this.provider = provider;
			public int CreateCount { get; private set; }
			public Task<ILLMProvider> CreateAsync(AISelectionSnapshot snapshot, CancellationToken cancellationToken = default)
			{
				CreateCount++;
				LastSnapshot = snapshot;
				return Task.FromResult(provider);
			}

			public AISelectionSnapshot? LastSnapshot { get; private set; }
		}

		static AISelectionSnapshot Snapshot(string provider = "openai", string? apiKey = "test-key", int maxContextTokens = 32000)
			=> new() { ProfileId = "p1", ProfileName = "Default", ProviderType = provider, Endpoint = "https://api.openai.com", Model = "gpt-4o", ApiKey = apiKey, CredentialId = "cred-1", MaxContextTokens = maxContextTokens };

		sealed class FakeProvider : ILLMProvider
		{
			readonly string[] chunks = Array.Empty<string>();
			readonly TimeSpan chunkDelay;
			readonly Exception? exception;
			readonly Func<CancellationToken, Task>? wait;
			public FakeProvider(params string[] chunks) => this.chunks = chunks;
			public FakeProvider(TimeSpan chunkDelay, params string[] chunks)
			{
				this.chunkDelay = chunkDelay;
				this.chunks = chunks;
			}
			public FakeProvider(Exception exception) => this.exception = exception;
			public FakeProvider(Func<CancellationToken, Task> wait) => this.wait = wait;
			public LLMRequest? LastRequest { get; private set; }
			public CancellationToken LastCancellationToken { get; private set; }

			public async IAsyncEnumerable<string> CompleteAsync(LLMRequest request, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
			{
				LastRequest = request;
				LastCancellationToken = cancellationToken;
				if (exception is not null)
					throw exception;
				if (wait is not null)
				{
					await wait(cancellationToken);
					yield break;
				}
				foreach (string chunk in chunks)
				{
					cancellationToken.ThrowIfCancellationRequested();
					if (chunkDelay > TimeSpan.Zero)
						await Task.Delay(chunkDelay, cancellationToken);
					yield return chunk;
				}
			}

			public Task<bool> TestConnectionAsync(CancellationToken cancellationToken) => Task.FromResult(true);
		}
	}
}
