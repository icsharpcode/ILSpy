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
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ICSharpCode.ILSpyX.AI.Providers
{
	/// <summary>
	/// OpenAI-compatible API provider (supports OpenAI, Ollama, and custom endpoints).
	/// </summary>
	public sealed class OpenAIProvider : ILLMProvider
	{
		private const int MaxErrorBodyLength = 4096;
		private const int MaxSseEventLength = 256 * 1024;

		private readonly Uri endpoint;
		private readonly string? apiKey;
		private readonly string model;
		private readonly HttpClient httpClient;

		public OpenAIProvider(string baseUrl, string? apiKey, string model, HttpClient httpClient)
		{
			if (string.IsNullOrWhiteSpace(baseUrl))
				throw new ArgumentException("Base URL cannot be empty.", nameof(baseUrl));
			if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out Uri? parsedBaseUri)
				|| (parsedBaseUri.Scheme != Uri.UriSchemeHttp && parsedBaseUri.Scheme != Uri.UriSchemeHttps))
				throw new ArgumentException("Base URL must be an absolute HTTP or HTTPS URI.", nameof(baseUrl));
			if (parsedBaseUri.Scheme == Uri.UriSchemeHttp && !parsedBaseUri.IsLoopback)
				throw new ArgumentException("HTTP is only allowed for loopback endpoints.", nameof(baseUrl));
			if (string.IsNullOrWhiteSpace(model))
				throw new ArgumentException("Model cannot be empty.", nameof(model));

			if (!string.IsNullOrEmpty(parsedBaseUri.Query) || !string.IsNullOrEmpty(parsedBaseUri.Fragment))
				throw new ArgumentException("Base URL cannot contain a query or fragment.", nameof(baseUrl));

			var endpointBuilder = new UriBuilder(parsedBaseUri);
			string path = endpointBuilder.Path.TrimEnd('/');
			endpointBuilder.Path = path.EndsWith("/v1", StringComparison.OrdinalIgnoreCase)
				? path + "/chat/completions"
				: path + "/v1/chat/completions";
			this.endpoint = endpointBuilder.Uri;
			this.apiKey = string.IsNullOrWhiteSpace(apiKey) ? null : apiKey.Trim();
			this.model = model.Trim();
			this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
		}

		public async IAsyncEnumerable<string> CompleteAsync(
			LLMRequest request,
			[EnumeratorCancellation] CancellationToken cancellationToken)
		{
			ArgumentNullException.ThrowIfNull(request);

			var messages = new List<object>(request.Messages.Count + 1);
			if (!string.IsNullOrWhiteSpace(request.SystemPrompt))
				messages.Add(new { role = "system", content = request.SystemPrompt });

			foreach (LLMMessage message in request.Messages)
				messages.Add(new { role = message.Role, content = message.Content });

			var payload = new {
				model,
				messages,
				max_tokens = request.MaxTokens,
				temperature = request.Temperature,
				stream = true
			};

			using var requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint) {
				Content = new StringContent(
					JsonSerializer.Serialize(payload),
					Encoding.UTF8,
					"application/json")
			};

			if (apiKey is not null)
				requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
			requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/event-stream"));

			using HttpResponseMessage response = await SendAsync(
				requestMessage,
				cancellationToken).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
				throw await CreateHttpRequestExceptionAsync(response, cancellationToken).ConfigureAwait(false);

			await using Stream stream = await ReadAsStreamAsync(response.Content, cancellationToken).ConfigureAwait(false);
			using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
			var eventData = new StringBuilder();
			bool hasData = false;

			while (true)
			{
				string? line = await ReadLineAsync(reader, cancellationToken).ConfigureAwait(false);
				bool endOfStream = line is null;
				line ??= string.Empty;

				if (line.Length != 0)
				{
					if (line.StartsWith("data:", StringComparison.Ordinal))
					{
						if (hasData)
							eventData.Append('\n');
						ReadOnlySpan<char> value = line.AsSpan(5);
						if (!value.IsEmpty && value[0] == ' ')
							value = value[1..];
						int separatorLength = hasData ? 1 : 0;
						if (value.Length > MaxSseEventLength - eventData.Length - separatorLength)
							throw new HttpRequestException("API SSE event exceeded the maximum supported size.");
						eventData.Append(value);
						hasData = true;
					}

					if (!endOfStream)
						continue;
				}

				if (hasData)
				{
					string data = eventData.ToString();
					eventData.Clear();
					hasData = false;

					if (string.Equals(data.Trim(), "[DONE]", StringComparison.Ordinal))
						yield break;

					if (TryGetContent(data, out string content))
						yield return content;
				}

				if (endOfStream)
					yield break;
			}
		}

		public async Task<bool> TestConnectionAsync(CancellationToken cancellationToken)
		{
			try
			{
				var request = new LLMRequest(
					"You are a test assistant.",
					new[] { new LLMMessage("user", "Say 'Hello'") },
					10);

				await foreach (string _ in CompleteAsync(request, cancellationToken).ConfigureAwait(false))
					return true;

				return false;
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch
			{
				return false;
			}
		}

		private async Task<HttpResponseMessage> SendAsync(
			HttpRequestMessage request,
			CancellationToken cancellationToken)
		{
			try
			{
				return await httpClient.SendAsync(
					request,
					HttpCompletionOption.ResponseHeadersRead,
					cancellationToken).ConfigureAwait(false);
			}
			catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
			{
				throw new OperationCanceledException(cancellationToken);
			}
		}

		private static async Task<Stream> ReadAsStreamAsync(
			HttpContent content,
			CancellationToken cancellationToken)
		{
			try
			{
				return await content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
			}
			catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
			{
				throw new OperationCanceledException(cancellationToken);
			}
		}

		private static async Task<string?> ReadLineAsync(
			StreamReader reader,
			CancellationToken cancellationToken)
		{
			try
			{
				return await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false);
			}
			catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
			{
				throw new OperationCanceledException(cancellationToken);
			}
		}

		private static bool TryGetContent(string data, out string content)
		{
			content = string.Empty;

			try
			{
				using JsonDocument json = JsonDocument.Parse(data);
				JsonElement root = json.RootElement;

				if (root.TryGetProperty("error", out JsonElement error))
				{
					string message = error.ValueKind == JsonValueKind.Object
						&& error.TryGetProperty("message", out JsonElement errorMessage)
						&& errorMessage.ValueKind == JsonValueKind.String
						? errorMessage.GetString() ?? "Unknown streaming error"
						: error.ToString();
					throw new HttpRequestException($"API stream returned an error: {message}");
				}

				if (!root.TryGetProperty("choices", out JsonElement choices)
					|| choices.ValueKind != JsonValueKind.Array
					|| choices.GetArrayLength() == 0)
					return false;

				JsonElement firstChoice = choices[0];
				if (!firstChoice.TryGetProperty("delta", out JsonElement delta)
					|| !delta.TryGetProperty("content", out JsonElement contentProperty)
					|| contentProperty.ValueKind != JsonValueKind.String)
					return false;

				content = contentProperty.GetString() ?? string.Empty;
				return content.Length != 0;
			}
			catch (JsonException)
			{
				return false;
			}
			catch (InvalidOperationException)
			{
				return false;
			}
		}

		private static async Task<HttpRequestException> CreateHttpRequestExceptionAsync(
			HttpResponseMessage response,
			CancellationToken cancellationToken)
		{
			(string body, bool truncated) = await ReadErrorBodyAsync(response.Content, cancellationToken).ConfigureAwait(false);
			string suffix = truncated ? " [truncated]" : string.Empty;
			string message = $"API request failed with status {(int)response.StatusCode} ({response.ReasonPhrase}). {body}{suffix}";
			return new HttpRequestException(message, inner: null, response.StatusCode);
		}

		private static async Task<(string Body, bool Truncated)> ReadErrorBodyAsync(
			HttpContent content,
			CancellationToken cancellationToken)
		{
			try
			{
				await using Stream stream = await content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
				byte[] buffer = new byte[4096];
				using var body = new MemoryStream(capacity: MaxErrorBodyLength + 1);
				int remaining = MaxErrorBodyLength + 1;

				while (remaining > 0)
				{
					int read = await stream.ReadAsync(
						buffer.AsMemory(0, Math.Min(buffer.Length, remaining)),
						cancellationToken).ConfigureAwait(false);
					if (read == 0)
						break;

					body.Write(buffer, 0, read);
					remaining -= read;
				}

				bool truncated = body.Length > MaxErrorBodyLength;
				int length = (int)Math.Min(body.Length, MaxErrorBodyLength);
				return (Encoding.UTF8.GetString(body.GetBuffer(), 0, length), truncated);
			}
			catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
			{
				throw new OperationCanceledException(cancellationToken);
			}
		}
	}
}
