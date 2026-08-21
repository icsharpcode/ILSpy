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

using ICSharpCode.ILSpy.AI;

namespace ICSharpCode.ILSpyX.AI.Providers
{
	/// <summary>Anthropic Messages API provider with streaming support.</summary>
	public sealed class AnthropicProvider : ILLMProvider
	{
		const string AnthropicVersion = "2023-06-01";
		const int MaxErrorBodyLength = 4096;
		const int MaxSseEventLength = 256 * 1024;
		const int MaxSseLineLength = MaxSseEventLength + 6;

		readonly Uri endpoint;
		readonly string apiKey;
		readonly string model;
		readonly HttpClient httpClient;

		public AnthropicProvider(string baseUrl, string apiKey, string model, HttpClient httpClient)
		{
			if (string.IsNullOrWhiteSpace(baseUrl))
				throw new ArgumentException("Base URL cannot be empty.", nameof(baseUrl));
			if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out Uri? parsed)
				|| (parsed.Scheme != Uri.UriSchemeHttp && parsed.Scheme != Uri.UriSchemeHttps))
				throw new ArgumentException("Base URL must be an absolute HTTP or HTTPS URI.", nameof(baseUrl));
			if (parsed.Scheme == Uri.UriSchemeHttp && !parsed.IsLoopback)
				throw new ArgumentException("HTTP is only allowed for loopback endpoints.", nameof(baseUrl));
			if (!string.IsNullOrEmpty(parsed.Query) || !string.IsNullOrEmpty(parsed.Fragment))
				throw new ArgumentException("Base URL cannot contain a query or fragment.", nameof(baseUrl));
			if (string.IsNullOrWhiteSpace(apiKey))
				throw new ArgumentException("API key cannot be empty.", nameof(apiKey));
			if (string.IsNullOrWhiteSpace(model))
				throw new ArgumentException("Model cannot be empty.", nameof(model));

			var builder = new UriBuilder(parsed);
			string path = builder.Path.TrimEnd('/');
			builder.Path = path.EndsWith("/v1/messages", StringComparison.OrdinalIgnoreCase)
				? path
				: path.EndsWith("/v1", StringComparison.OrdinalIgnoreCase)
					? path + "/messages"
					: path + "/v1/messages";
			endpoint = builder.Uri;
			this.apiKey = apiKey.Trim();
			this.model = model.Trim();
			this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
		}

		public async IAsyncEnumerable<string> CompleteAsync(
			LLMRequest request,
			[EnumeratorCancellation] CancellationToken cancellationToken)
		{
			ArgumentNullException.ThrowIfNull(request);
			var messages = new List<object>(request.Messages.Count);
			foreach (LLMMessage message in request.Messages)
			{
				if (message.Role == "system")
					continue;
				messages.Add(new { role = message.Role, content = message.Content });
			}

			var payload = new {
				model,
				max_tokens = request.MaxTokens,
				temperature = request.Temperature,
				system = request.SystemPrompt,
				messages,
				stream = true
			};
			using var requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint) {
				Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
			};
			requestMessage.Headers.Add("x-api-key", apiKey);
			requestMessage.Headers.Add("anthropic-version", AnthropicVersion);
			requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/event-stream"));

			using HttpResponseMessage response = await SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);
			if (!response.IsSuccessStatusCode)
				throw await CreateHttpRequestExceptionAsync(response, cancellationToken).ConfigureAwait(false);

			await using Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
			using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
			string? eventName = null;
			var data = new StringBuilder();
			bool hasData = false;
			await foreach (string line in ReadLinesAsync(reader, cancellationToken).ConfigureAwait(false))
			{
				if (line.Length == 0)
				{
					if (hasData)
					{
						if (TryGetContent(eventName, data.ToString(), out string content))
							yield return content;
						if (string.Equals(eventName, "message_stop", StringComparison.Ordinal))
							yield break;
					}
					eventName = null;
					data.Clear();
					hasData = false;
					continue;
				}
				if (line.StartsWith("event:", StringComparison.Ordinal))
				{
					eventName = line[6..].Trim();
					continue;
				}
				if (line.StartsWith("data:", StringComparison.Ordinal))
				{
					ReadOnlySpan<char> value = line.AsSpan(5);
					if (!value.IsEmpty && value[0] == ' ')
						value = value[1..];
					int separatorLength = hasData ? 1 : 0;
					if (value.Length > MaxSseEventLength - data.Length - separatorLength)
						throw new HttpRequestException("API SSE event exceeded the maximum supported size.");
					if (hasData)
						data.Append('\n');
					data.Append(value);
					hasData = true;
				}
			}
			if (hasData && TryGetContent(eventName, data.ToString(), out string finalContent))
				yield return finalContent;
		}

		public async Task<bool> TestConnectionAsync(CancellationToken cancellationToken)
		{
			try
			{
				var request = new LLMRequest("You are a test assistant.", new[] { new LLMMessage("user", "Say 'Hello'") }, 10);
				await foreach (string _ in CompleteAsync(request, cancellationToken).ConfigureAwait(false))
					return true;
				return false;
			}
			catch (OperationCanceledException) { throw; }
			catch { return false; }
		}

		async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			try
			{
				return await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
			}
			catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
			{
				throw new OperationCanceledException(cancellationToken);
			}
		}

		static async IAsyncEnumerable<string> ReadLinesAsync(StreamReader reader, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			char[] buffer = new char[4096];
			var line = new StringBuilder();
			bool skipLineFeed = false;
			while (true)
			{
				int read;
				try
				{ read = await reader.ReadAsync(buffer.AsMemory(), cancellationToken).ConfigureAwait(false); }
				catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested) { throw new OperationCanceledException(cancellationToken); }
				if (read == 0)
					break;
				for (int i = 0; i < read; i++)
				{
					char c = buffer[i];
					if (skipLineFeed)
					{ skipLineFeed = false; if (c == '\n') continue; }
					if (c is '\r' or '\n')
					{ yield return line.ToString(); line.Clear(); skipLineFeed = c == '\r'; continue; }
					if (line.Length >= MaxSseLineLength)
						throw new HttpRequestException("API SSE event exceeded the maximum supported size.");
					line.Append(c);
				}
			}
			if (line.Length != 0)
				yield return line.ToString();
		}

		static bool TryGetContent(string? eventName, string data, out string content)
		{
			content = string.Empty;
			try
			{
				using JsonDocument json = JsonDocument.Parse(data);
				JsonElement root = json.RootElement;
				if (root.TryGetProperty("type", out JsonElement type) && type.GetString() == "error")
				{
					string message = root.TryGetProperty("error", out JsonElement error)
						&& error.TryGetProperty("message", out JsonElement text) ? text.GetString() ?? "Unknown streaming error" : "Unknown streaming error";
					throw new HttpRequestException($"API stream returned an error: {message}");
				}
				if (!string.Equals(eventName, "content_block_delta", StringComparison.Ordinal)
					|| !root.TryGetProperty("delta", out JsonElement delta)
					|| !delta.TryGetProperty("text", out JsonElement textElement)
					|| textElement.ValueKind != JsonValueKind.String)
					return false;
				content = textElement.GetString() ?? string.Empty;
				return content.Length != 0;
			}
			catch (JsonException) { return false; }
			catch (InvalidOperationException) { return false; }
		}

		async Task<HttpRequestException> CreateHttpRequestExceptionAsync(HttpResponseMessage response, CancellationToken cancellationToken)
		{
			(string body, bool truncated) = await ReadErrorBodyAsync(response.Content, cancellationToken).ConfigureAwait(false);
			return new HttpRequestException($"API request failed with status {(int)response.StatusCode} ({response.ReasonPhrase}). {RedactApiKey(body)}{(truncated ? " [truncated]" : string.Empty)}", null, response.StatusCode);
		}

		string RedactApiKey(string body)
		{
			return apiKey.Length < 4
				? body
				: body.Replace(apiKey, "[redacted]", StringComparison.Ordinal);
		}

		static async Task<(string Body, bool Truncated)> ReadErrorBodyAsync(HttpContent content, CancellationToken cancellationToken)
		{
			await using Stream stream = await content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
			byte[] buffer = new byte[4096];
			using var body = new MemoryStream(MaxErrorBodyLength + 1);
			int remaining = MaxErrorBodyLength + 1;
			while (remaining > 0)
			{
				int read = await stream.ReadAsync(buffer.AsMemory(0, Math.Min(buffer.Length, remaining)), cancellationToken).ConfigureAwait(false);
				if (read == 0)
					break;
				body.Write(buffer, 0, read);
				remaining -= read;
			}
			bool truncated = body.Length > MaxErrorBodyLength;
			int length = (int)Math.Min(body.Length, MaxErrorBodyLength);
			return (Encoding.UTF8.GetString(body.GetBuffer(), 0, length), truncated);
		}
	}
}
