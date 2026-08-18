// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using AwesomeAssertions;

using ICSharpCode.ILSpyX.AI;

using NUnit.Framework;

namespace ICSharpCode.ILSpyX.Tests.AI
{
	[TestFixture]
	public class SecureKeyStorageTests
	{
		[Test]
		public void BackendContract_IsNotPartOfThePublicApi()
		{
			typeof(ISecureKeyStorageBackend).IsNotPublic.Should().BeTrue();
			typeof(SecureKeyStorageBackendReadResult).IsNotPublic.Should().BeTrue();
		}

		[Test]
		public async Task SaveAndLoad_CanonicalizesProvider()
		{
			var backend = new FakeBackend();
			var storage = new SecureKeyStorage(backend);

			await storage.SaveKeyAsync(" OpenAI ", "sk-test");

			Assert.That(await storage.LoadKeyAsync("openai"), Is.EqualTo("sk-test"));
			Assert.That(backend.SavedProvider, Is.EqualTo("openai"));
		}

		[Test]
		public async Task Delete_UsesCanonicalProvider()
		{
			var backend = new FakeBackend();
			var storage = new SecureKeyStorage(backend);

			await storage.DeleteKeyAsync(" Anthropic ");

			Assert.That(backend.DeletedProvider, Is.EqualTo("anthropic"));
		}

		[TestCase(null)]
		[TestCase("")]
		[TestCase(" ")]
		[TestCase("provider/name")]
		[TestCase("provider\\name")]
		[TestCase("provider\nname")]
		[TestCase(".")]
		[TestCase("..")]
		public void InvalidProvider_IsRejected(string? provider)
		{
			var storage = new SecureKeyStorage(new FakeBackend());

			Assert.That(() => storage.LoadKeyAsync(provider!), Throws.ArgumentException);
		}

		[Test]
		public void EmptyKey_IsRejected()
		{
			var storage = new SecureKeyStorage(new FakeBackend());

			Assert.That(() => storage.SaveKeyAsync("openai", " "), Throws.ArgumentException);
		}

		[Test]
		public async Task TryLoad_NotFound_IsDistinctFromFound()
		{
			var storage = new SecureKeyStorage(new FakeBackend());

			SecureKeyLookupResult result = await storage.TryLoadKeyAsync("openai");

			Assert.That(result.Status, Is.EqualTo(SecureKeyLookupStatus.NotFound));
			Assert.That(result.Value, Is.Null);
			Assert.That(await storage.LoadKeyAsync("openai"), Is.Null);
		}

		[Test]
		public async Task TryLoad_Unavailable_IsDistinctFromNotFound()
		{
			var storage = new SecureKeyStorage(new FakeBackend { IsUnavailable = true });

			SecureKeyLookupResult result = await storage.TryLoadKeyAsync("openai");

			Assert.That(result.Status, Is.EqualTo(SecureKeyLookupStatus.Unavailable));
			Assert.That(result.Value, Is.Null);
			Assert.ThrowsAsync<SecureKeyStorageUnavailableException>(
				async () => await storage.LoadKeyAsync("openai"));
		}

		[Test]
		public async Task Cancellation_IsForwardedToBackend()
		{
			var backend = new FakeBackend();
			var storage = new SecureKeyStorage(backend);
			using var cancellationTokenSource = new CancellationTokenSource();

			await storage.SaveKeyAsync("openai", "sk-test", cancellationTokenSource.Token);

			Assert.That(backend.LastCancellationToken, Is.EqualTo(cancellationTokenSource.Token));
		}

		private sealed class FakeBackend : ISecureKeyStorageBackend
		{
			private readonly Dictionary<string, string> keys = new(StringComparer.Ordinal);

			public bool IsUnavailable { get; init; }

			public string? SavedProvider { get; private set; }

			public string? DeletedProvider { get; private set; }

			public CancellationToken LastCancellationToken { get; private set; }

			public Task SaveAsync(string provider, string key, CancellationToken cancellationToken)
			{
				ThrowIfUnavailable();
				LastCancellationToken = cancellationToken;
				SavedProvider = provider;
				keys[provider] = key;
				return Task.CompletedTask;
			}

			public Task<SecureKeyStorageBackendReadResult> LoadAsync(string provider, CancellationToken cancellationToken)
			{
				ThrowIfUnavailable();
				LastCancellationToken = cancellationToken;
				return Task.FromResult(keys.TryGetValue(provider, out string? key)
					? SecureKeyStorageBackendReadResult.Found(key)
					: SecureKeyStorageBackendReadResult.NotFound);
			}

			public Task DeleteAsync(string provider, CancellationToken cancellationToken)
			{
				ThrowIfUnavailable();
				LastCancellationToken = cancellationToken;
				DeletedProvider = provider;
				keys.Remove(provider);
				return Task.CompletedTask;
			}

			private void ThrowIfUnavailable()
			{
				if (IsUnavailable)
					throw new SecureKeyStorageUnavailableException("test backend unavailable");
			}
		}
	}
}
