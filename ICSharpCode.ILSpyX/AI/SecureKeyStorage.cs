// Copyright (c) 2026 Masroor
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace ICSharpCode.ILSpyX.AI
{
	internal interface ISecureKeyStorageBackend
	{
		Task SaveAsync(string provider, string key, CancellationToken cancellationToken);

		Task<SecureKeyStorageBackendReadResult> LoadAsync(string provider, CancellationToken cancellationToken);

		Task DeleteAsync(string provider, CancellationToken cancellationToken);
	}

	internal enum SecureKeyStorageBackendReadStatus
	{
		Found,
		NotFound
	}

	internal readonly record struct SecureKeyStorageBackendReadResult(
		SecureKeyStorageBackendReadStatus Status,
		string? Value)
	{
		public static SecureKeyStorageBackendReadResult Found(string value)
		{
			return new(SecureKeyStorageBackendReadStatus.Found, value ?? throw new ArgumentNullException(nameof(value)));
		}

		public static SecureKeyStorageBackendReadResult NotFound => new(SecureKeyStorageBackendReadStatus.NotFound, null);
	}

	public enum SecureKeyLookupStatus
	{
		Found,
		NotFound,
		Unavailable
	}

	public readonly record struct SecureKeyLookupResult(SecureKeyLookupStatus Status, string? Value)
	{
		public static SecureKeyLookupResult Found(string value)
		{
			return new(SecureKeyLookupStatus.Found, value ?? throw new ArgumentNullException(nameof(value)));
		}

		public static SecureKeyLookupResult NotFound => new(SecureKeyLookupStatus.NotFound, null);

		public static SecureKeyLookupResult Unavailable => new(SecureKeyLookupStatus.Unavailable, null);
	}

	public sealed class SecureKeyStorageUnavailableException : Exception
	{
		public SecureKeyStorageUnavailableException(string message)
			: base(message)
		{
		}

		public SecureKeyStorageUnavailableException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}

	public sealed class SecureKeyStorage
	{
		private readonly ISecureKeyStorageBackend backend;

		public SecureKeyStorage()
			: this(SecureKeyStorageBackendFactory.CreateDefault())
		{
		}

		internal SecureKeyStorage(ISecureKeyStorageBackend backend)
		{
			this.backend = backend ?? throw new ArgumentNullException(nameof(backend));
		}

		public Task SaveKeyAsync(string provider, string key, CancellationToken cancellationToken = default)
		{
			return backend.SaveAsync(CanonicalizeProvider(provider), ValidateKey(key), cancellationToken);
		}

		public Task<string?> LoadKeyAsync(string provider, CancellationToken cancellationToken = default)
		{
			return LoadKeyCoreAsync(CanonicalizeProvider(provider), cancellationToken);
		}

		public Task<SecureKeyLookupResult> TryLoadKeyAsync(string provider, CancellationToken cancellationToken = default)
		{
			return TryLoadKeyCoreAsync(CanonicalizeProvider(provider), cancellationToken);
		}

		private async Task<string?> LoadKeyCoreAsync(string provider, CancellationToken cancellationToken)
		{
			SecureKeyLookupResult result = await TryLoadKeyCoreAsync(provider, cancellationToken).ConfigureAwait(false);
			if (result.Status == SecureKeyLookupStatus.Unavailable)
				throw new SecureKeyStorageUnavailableException("Secure key storage is unavailable.");
			return result.Value;
		}

		private async Task<SecureKeyLookupResult> TryLoadKeyCoreAsync(string provider, CancellationToken cancellationToken)
		{
			SecureKeyStorageBackendReadResult result;
			try
			{
				result = await backend.LoadAsync(provider, cancellationToken).ConfigureAwait(false);
			}
			catch (SecureKeyStorageUnavailableException)
			{
				return SecureKeyLookupResult.Unavailable;
			}

			return result.Status switch
			{
				SecureKeyStorageBackendReadStatus.Found => SecureKeyLookupResult.Found(result.Value!),
				SecureKeyStorageBackendReadStatus.NotFound => SecureKeyLookupResult.NotFound,
				_ => throw new InvalidOperationException("The secure key storage backend returned an unknown read status.")
			};
		}

		public Task DeleteKeyAsync(string provider, CancellationToken cancellationToken = default)
		{
			return backend.DeleteAsync(CanonicalizeProvider(provider), cancellationToken);
		}

		internal static string CanonicalizeProvider(string provider)
		{
			if (provider is null)
				throw new ArgumentException("Provider identifier cannot be null.", nameof(provider));

			provider = provider.Trim();
			if (provider.Length == 0 || provider is "." or "..")
				throw new ArgumentException("Provider identifier is invalid.", nameof(provider));
			if (provider.Length > 64)
				throw new ArgumentException("Provider identifier is too long.", nameof(provider));

			foreach (char c in provider)
			{
				if (!IsAllowedProviderCharacter(c))
					throw new ArgumentException("Provider identifier contains an unsupported character.", nameof(provider));
			}

			return provider.ToLowerInvariant();
		}

		private static string ValidateKey(string key)
		{
			if (key is null)
				throw new ArgumentNullException(nameof(key));
			if (string.IsNullOrWhiteSpace(key))
				throw new ArgumentException("API key cannot be empty.", nameof(key));
			return key;
		}

		private static bool IsAllowedProviderCharacter(char c)
		{
			return c is >= 'a' and <= 'z'
				or >= 'A' and <= 'Z'
				or >= '0' and <= '9'
				or '.' or '-' or '_';
		}
	}
}
