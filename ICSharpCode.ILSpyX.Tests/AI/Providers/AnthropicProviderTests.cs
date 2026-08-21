// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.ILSpy.AI;
using ICSharpCode.ILSpyX.AI;
using ICSharpCode.ILSpyX.AI.Providers;

using NUnit.Framework;

namespace ICSharpCode.ILSpyX.Tests.AI.Providers
{
	[TestFixture]
	public sealed class AnthropicProviderTests
	{
		[Test]
		public async Task CompleteAsync_SendsMessagesRequestAndParsesContentDeltas()
		{
			HttpRequestMessage? capturedRequest = null;
			string? capturedBody = null;
			var handler = new FakeHttpMessageHandler(async (request, cancellationToken) => {
				capturedRequest = request;
				capturedBody = await request.Content!.ReadAsStringAsync(cancellationToken);
				return Response("event: message_start\ndata: {}\n\n"
					+ "event: content_block_delta\ndata: {\"type\":\"content_block_delta\",\"delta\":{\"text\":\"Hello\"}}\n\n"
					+ "event: content_block_delta\ndata: {\"delta\":{\"text\":\" world\"}}\n\n"
					+ "event: message_stop\ndata: {}\n\n");
			});
			using var httpClient = new HttpClient(handler);
			var provider = new AnthropicProvider("https://api.example.test/v1/", "secret", "claude-test", httpClient);

			List<string> chunks = await ConsumeAsync(provider.CompleteAsync(
				new LLMRequest("system prompt", new[] { new LLMMessage("user", "question") }, 321, 0.2), CancellationToken.None));

			Assert.That(chunks, Is.EqualTo(new[] { "Hello", " world" }));
			Assert.That(capturedRequest!.RequestUri, Is.EqualTo(new Uri("https://api.example.test/v1/messages")));
			Assert.That(capturedRequest.Headers.GetValues("x-api-key"), Is.EqualTo(new[] { "secret" }));
			Assert.That(capturedRequest.Headers.GetValues("anthropic-version"), Is.EqualTo(new[] { "2023-06-01" }));
			using JsonDocument json = JsonDocument.Parse(capturedBody!);
			JsonElement root = json.RootElement;
			Assert.That(root.GetProperty("model").GetString(), Is.EqualTo("claude-test"));
			Assert.That(root.GetProperty("max_tokens").GetInt32(), Is.EqualTo(321));
			Assert.That(root.GetProperty("temperature").GetDouble(), Is.EqualTo(0.2));
			Assert.That(root.GetProperty("system").GetString(), Is.EqualTo("system prompt"));
			Assert.That(root.GetProperty("stream").GetBoolean(), Is.True);
			Assert.That(root.GetProperty("messages")[0].GetProperty("role").GetString(), Is.EqualTo("user"));
		}

		[Test]
		public void CompleteAsync_PreservesErrorStatusAndBoundsBody()
		{
			using var httpClient = new HttpClient(new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.TooManyRequests) {
				Content = new StringContent(new string('x', 100_000))
			}));
			var provider = new AnthropicProvider("https://api.example.test", "secret", "model", httpClient);

			var exception = Assert.ThrowsAsync<HttpRequestException>(async () =>
				await ConsumeAsync(provider.CompleteAsync(ValidRequest(), CancellationToken.None)));

			Assert.That(exception!.StatusCode, Is.EqualTo(HttpStatusCode.TooManyRequests));
			Assert.That(exception.Message.Length, Is.LessThan(20_000));
			Assert.That(exception.Message, Does.Contain("truncated"));
		}

		[Test]
		public void CompleteAsync_RedactsApiKeyFromErrorBody()
		{
			const string secretKey = "sk-ant-key-marker-9f8e7d6c";
			using var httpClient = new HttpClient(new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.BadRequest) {
				Content = new StringContent("{\"error\":{\"message\":\"authentication failed for key " + secretKey + "\"}}")
			}));
			var provider = new AnthropicProvider("https://api.example.test", secretKey, "model", httpClient);

			var exception = Assert.ThrowsAsync<HttpRequestException>(async () =>
				await ConsumeAsync(provider.CompleteAsync(ValidRequest(), CancellationToken.None)));

			Assert.That(exception!.Message, Does.Not.Contain(secretKey));
			Assert.That(exception.Message, Does.Contain("authentication failed"));
		}

		[Test]
		public void CompleteAsync_RejectsStreamingErrors()
		{
			using var httpClient = new HttpClient(new FakeHttpMessageHandler(_ => Response(
				"event: error\ndata: {\"type\":\"error\",\"error\":{\"message\":\"stream failed\"}}\n\n")));
			var provider = new AnthropicProvider("https://api.example.test", "secret", "model", httpClient);

			var exception = Assert.ThrowsAsync<HttpRequestException>(async () =>
				await ConsumeAsync(provider.CompleteAsync(ValidRequest(), CancellationToken.None)));

			Assert.That(exception!.Message, Does.Contain("stream failed"));
		}

		private static LLMRequest ValidRequest() => new("", new[] { new LLMMessage("user", "Hello") }, 10);

		private static HttpResponseMessage Response(string body) => new(HttpStatusCode.OK) {
			Content = new StringContent(body, Encoding.UTF8, "text/event-stream")
		};

		private static async Task<List<string>> ConsumeAsync(IAsyncEnumerable<string> source)
		{
			var result = new List<string>();
			await foreach (string item in source)
				result.Add(item);
			return result;
		}

		sealed class FakeHttpMessageHandler : HttpMessageHandler
		{
			readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> sendAsync;

			public FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> sendAsync)
				: this((request, _) => Task.FromResult(sendAsync(request))) { }

			public FakeHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> sendAsync)
			{
				this.sendAsync = sendAsync;
			}

			protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
				=> sendAsync(request, cancellationToken);
		}
	}
}
