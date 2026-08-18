// Copyright (c) 2026 Dr. Masroor Ehsan

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

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ICSharpCode.ILSpyX.AI.Providers
{
	/// <summary>
	/// OpenAI-compatible API provider (supports OpenAI, Ollama, and custom endpoints).
	/// </summary>
	public sealed class OpenAIProvider : ILLMProvider
	{
		private const int MaxErrorBodyLength = 4096;
		private const int MaxSseEventLength = 256 * 1024;
		private const int MaxSseLineLength = MaxSseEventLength + 6;

		private readonly Uri endpoint;
		private readonly string? apiKey;
		private readonly string model;
		private readonly HttpClient httpClient;
		private readonly ILogger logger;

		public OpenAIProvider(string baseUrl, string? apiKey, string model, HttpClient httpClient, ILoggerFactory? loggerFactory = null)
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
			this.logger = loggerFactory?.CreateLogger<OpenAIProvider>() ?? NullLogger<OpenAIProvider>.Instance;

			logger.LogInformation("OpenAIProvider initialized with endpoint: {Endpoint}, model: {Model}", endpoint, model);
		}

		public async IAsyncEnumerable<string> CompleteAsync(
			LLMRequest request,
			[EnumeratorCancellation] CancellationToken cancellationToken)
		{
			ArgumentNullException.ThrowIfNull(request);

			logger.LogDebug("Starting CompleteAsync for model {Model}", model);

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

			string payloadJson = JsonSerializer.Serialize(payload);
			logger.LogTrace("Request payload: {Payload}", payloadJson);

			using var requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint) {
				Content = new StringContent(
					payloadJson,
					Encoding.UTF8,
					"application/json")
			};

			if (apiKey is not null)
				requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
			requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/event-stream"));

			logger.LogInformation("Sending HTTP POST to {Endpoint}", endpoint);

			HttpResponseMessage response;
			try
			{
				response = await SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "HTTP request failed: {ExceptionType} - {Message}", ex.GetType().Name, ex.Message);
				throw;
			}

			using (response)
			{
				logger.LogInformation("Received response with status code: {StatusCode}", response.StatusCode);

				if (!response.IsSuccessStatusCode)
				{
					var exception = await CreateHttpRequestExceptionAsync(response, cancellationToken).ConfigureAwait(false);
					logger.LogError(exception, "HTTP request returned error status {StatusCode}", response.StatusCode);
					throw exception;
				}

				await using Stream stream = await ReadAsStreamAsync(response.Content, cancellationToken).ConfigureAwait(false);
				using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
				var eventData = new StringBuilder();
				bool hasData = false;
				int eventCount = 0;
				int contentChunkCount = 0;

				logger.LogDebug("Starting to read SSE stream");

				await foreach (string line in ReadLinesAsync(reader, cancellationToken).ConfigureAwait(false))
				{
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
							{
								logger.LogError("SSE event exceeded maximum size: {EventLength}", eventData.Length + value.Length);
								throw new HttpRequestException("API SSE event exceeded the maximum supported size.");
							}
							eventData.Append(value);
							hasData = true;
						}
						continue;
					}

					if (!hasData)
						continue;

					string data = TakeEventData(eventData, ref hasData);
					eventCount++;
					logger.LogTrace("Received SSE event #{EventNumber}: {Data}", eventCount, data.Length > 200 ? data.Substring(0, 200) + "..." : data);

					if (string.Equals(data.Trim(), "[DONE]", StringComparison.Ordinal))
					{
						logger.LogInformation("Received [DONE] marker after {EventCount} events and {ContentChunkCount} content chunks", eventCount, contentChunkCount);
						yield break;
					}
					if (TryGetContent(data, out string content))
					{
						contentChunkCount++;
						logger.LogTrace("Yielding content chunk #{ChunkNumber}, length: {Length}", contentChunkCount, content.Length);
						yield return content;
					}
				}

				if (hasData)
				{
					string data = TakeEventData(eventData, ref hasData);
					eventCount++;
					logger.LogTrace("Processing final SSE event #{EventNumber}: {Data}", eventCount, data.Length > 200 ? data.Substring(0, 200) + "..." : data);

					if (!string.Equals(data.Trim(), "[DONE]", StringComparison.Ordinal)
						&& TryGetContent(data, out string content))
					{
						contentChunkCount++;
						logger.LogTrace("Yielding final content chunk #{ChunkNumber}, length: {Length}", contentChunkCount, content.Length);
						yield return content;
					}
				}

				logger.LogInformation("CompleteAsync finished. Total events: {EventCount}, content chunks: {ContentChunkCount}", eventCount, contentChunkCount);
			}
		}

		public async Task<bool> TestConnectionAsync(CancellationToken cancellationToken)
		{
			logger.LogInformation("Starting connection test to {Endpoint}", endpoint);
			try
			{
				var request = new LLMRequest(
					"You are a test assistant.",
					new[] { new LLMMessage("user", "Say 'Hello'") },
					100);

				await foreach (string _ in CompleteAsync(request, cancellationToken).ConfigureAwait(false))
				{
					logger.LogInformation("Connection test successful - received response from provider");
					return true;
				}

				logger.LogWarning("Connection test completed but received no content");
				return true;
			}
			catch (OperationCanceledException)
			{
				logger.LogInformation("Connection test was canceled");
				throw;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Connection test failed: {ExceptionType} - {Message}\nStack trace: {StackTrace}",
					ex.GetType().FullName, ex.Message, ex.StackTrace);
				return false;
			}
		}

		private async Task<HttpResponseMessage> SendAsync(
			HttpRequestMessage request,
			CancellationToken cancellationToken)
		{
			try
			{
				logger.LogDebug("Sending HTTP request to {Uri}", request.RequestUri);
				var response = await httpClient.SendAsync(
					request,
					HttpCompletionOption.ResponseHeadersRead,
					cancellationToken).ConfigureAwait(false);
				logger.LogDebug("Received HTTP response with status {StatusCode}", response.StatusCode);
				return response;
			}
			catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
			{
				logger.LogInformation("HTTP request was canceled");
				throw new OperationCanceledException(cancellationToken);
			}
			catch (TaskCanceledException ex)
			{
				logger.LogError(ex, "HTTP request timed out");
				throw;
			}
			catch (HttpRequestException ex)
			{
				logger.LogError(ex, "HTTP request exception: {Message}", ex.Message);
				throw;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Unexpected exception during HTTP request: {ExceptionType} - {Message}",
					ex.GetType().FullName, ex.Message);
				throw;
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

		private static async IAsyncEnumerable<string> ReadLinesAsync(
			StreamReader reader,
			[EnumeratorCancellation] CancellationToken cancellationToken)
		{
			char[] buffer = new char[4096];
			var line = new StringBuilder();
			bool skipLineFeed = false;

			while (true)
			{
				int read;
				try
				{
					read = await reader.ReadAsync(buffer.AsMemory(), cancellationToken).ConfigureAwait(false);
				}
				catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
				{
					throw new OperationCanceledException(cancellationToken);
				}

				if (read == 0)
					break;

				for (int i = 0; i < read; i++)
				{
					char c = buffer[i];
					if (skipLineFeed)
					{
						skipLineFeed = false;
						if (c == '\n')
							continue;
					}

					if (c is '\r' or '\n')
					{
						yield return line.ToString();
						line.Clear();
						skipLineFeed = c == '\r';
						continue;
					}

					if (line.Length >= MaxSseLineLength)
						throw new HttpRequestException("API SSE event exceeded the maximum supported size.");
					line.Append(c);
				}
			}

			if (line.Length != 0)
				yield return line.ToString();
		}

		private static string TakeEventData(StringBuilder eventData, ref bool hasData)
		{
			string data = eventData.ToString();
			eventData.Clear();
			hasData = false;
			return data;
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
