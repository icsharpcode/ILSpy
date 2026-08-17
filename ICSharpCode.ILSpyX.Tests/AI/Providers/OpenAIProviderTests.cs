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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.ILSpyX.AI;
using ICSharpCode.ILSpyX.AI.Providers;

using NUnit.Framework;

namespace ICSharpCode.ILSpyX.Tests.AI.Providers
{
	[TestFixture]
	public class OpenAIProviderTests
	{
		[TestCase("")]
		[TestCase("relative")]
		[TestCase("ftp://example.com")]
		public void Constructor_RejectsInvalidBaseUri(string baseUri)
		{
			using var httpClient = new HttpClient(new FakeHttpMessageHandler(_ => CreateStreamingResponse("data: [DONE]\n\n")));

			Assert.That(() => new OpenAIProvider(baseUri, "key", "model", httpClient), Throws.ArgumentException);
		}

		[TestCase("")]
		[TestCase("   ")]
		public void Constructor_RejectsEmptyModel(string model)
		{
			using var httpClient = new HttpClient(new FakeHttpMessageHandler(_ => CreateStreamingResponse("data: [DONE]\n\n")));

			Assert.That(() => new OpenAIProvider("https://example.com", "key", model, httpClient), Throws.ArgumentException);
		}

		[Test]
		public void Constructor_RejectsNullHttpClient()
		{
			Assert.That(() => new OpenAIProvider("https://example.com", "key", "model", null!), Throws.ArgumentNullException);
		}

		[Test]
		public async Task CompleteAsync_SendsOpenAICompatibleRequest()
		{
			HttpRequestMessage? capturedRequest = null;
			string? capturedBody = null;
			var handler = new FakeHttpMessageHandler(async (request, cancellationToken) => {
				capturedRequest = request;
				capturedBody = await request.Content!.ReadAsStringAsync(cancellationToken);
				return CreateStreamingResponse("data: [DONE]\n\n");
			});
			using var httpClient = new HttpClient(handler);
			var provider = new OpenAIProvider("https://example.com/api/", "secret", "test-model", httpClient);
			var request = new LLMRequest(
				"system prompt",
				new[] {
					new LLMMessage("user", "question"),
					new LLMMessage("assistant", "answer")
				},
				123,
				0.25);

			await ConsumeAsync(provider.CompleteAsync(request, CancellationToken.None));

			Assert.That(capturedRequest, Is.Not.Null);
			Assert.That(capturedRequest!.Method, Is.EqualTo(HttpMethod.Post));
			Assert.That(capturedRequest.RequestUri, Is.EqualTo(new Uri("https://example.com/api/v1/chat/completions")));
			Assert.That(capturedRequest.Headers.Authorization?.Scheme, Is.EqualTo("Bearer"));
			Assert.That(capturedRequest.Headers.Authorization?.Parameter, Is.EqualTo("secret"));
			Assert.That(capturedRequest.Content!.Headers.ContentType?.MediaType, Is.EqualTo("application/json"));

			using var json = JsonDocument.Parse(capturedBody!);
			var root = json.RootElement;
			Assert.That(root.GetProperty("model").GetString(), Is.EqualTo("test-model"));
			Assert.That(root.GetProperty("max_tokens").GetInt32(), Is.EqualTo(123));
			Assert.That(root.GetProperty("temperature").GetDouble(), Is.EqualTo(0.25));
			Assert.That(root.GetProperty("stream").GetBoolean(), Is.True);
			var messages = root.GetProperty("messages").EnumerateArray().ToArray();
			Assert.That(messages, Has.Length.EqualTo(3));
			AssertMessage(messages[0], "system", "system prompt");
			AssertMessage(messages[1], "user", "question");
			AssertMessage(messages[2], "assistant", "answer");
		}

		[Test]
		public async Task CompleteAsync_OmitsAuthorizationForEmptyApiKey()
		{
			HttpRequestMessage? capturedRequest = null;
			var handler = new FakeHttpMessageHandler(request => {
				capturedRequest = request;
				return CreateStreamingResponse("data: [DONE]\n\n");
			});
			using var httpClient = new HttpClient(handler);
			var provider = new OpenAIProvider("http://localhost:11434", "", "model", httpClient);

			await ConsumeAsync(provider.CompleteAsync(ValidRequest(), CancellationToken.None));

			Assert.That(capturedRequest!.Headers.Authorization, Is.Null);
		}

		[Test]
		public async Task CompleteAsync_ParsesDataWithOptionalWhitespaceAndStopsAtDone()
		{
			const string responseBody = "event: message\n"
				+ "data:{\"choices\":[{\"delta\":{\"content\":\"Hello\"}}]}\n\n"
				+ "data:   {\"choices\":[{\"delta\":{\"content\":\" world\"}}]}\n\n"
				+ "data:\t[DONE]\n\n"
				+ "data: {\"choices\":[{\"delta\":{\"content\":\"ignored\"}}]}\n\n";
			using var httpClient = new HttpClient(new FakeHttpMessageHandler(_ => CreateStreamingResponse(responseBody)));
			var provider = new OpenAIProvider("https://example.com", null, "model", httpClient);

			var chunks = await ConsumeAsync(provider.CompleteAsync(ValidRequest(), CancellationToken.None));

			Assert.That(chunks, Is.EqualTo(new[] { "Hello", " world" }));
		}

		[Test]
		public async Task CompleteAsync_CombinesMultilineDataEvents()
		{
			const string responseBody = "data: {\"choices\":[\n"
				+ "data: {\"delta\":{\"content\":\"Hello\"}}\n"
				+ "data: ]}\n\n"
				+ "data: [DONE]\n\n";
			using var httpClient = new HttpClient(new FakeHttpMessageHandler(_ => CreateStreamingResponse(responseBody)));
			var provider = new OpenAIProvider("https://example.com", "key", "model", httpClient);

			var chunks = await ConsumeAsync(provider.CompleteAsync(ValidRequest(), CancellationToken.None));

			Assert.That(chunks, Is.EqualTo(new[] { "Hello" }));
		}

		[Test]
		public async Task CompleteAsync_ParsesFinalEventWithoutTrailingBlankLine()
		{
			const string responseBody = "data: {\"choices\":[{\"delta\":{\"content\":\"final\"}}]}";
			using var httpClient = new HttpClient(new FakeHttpMessageHandler(_ => CreateStreamingResponse(responseBody)));
			var provider = new OpenAIProvider("https://example.com", "key", "model", httpClient);

			List<string> chunks = await ConsumeAsync(provider.CompleteAsync(ValidRequest(), CancellationToken.None));

			Assert.That(chunks, Is.EqualTo(new[] { "final" }));
		}

		[Test]
		public void CompleteAsync_ThrowsForStreamingError()
		{
			const string responseBody = "data: {\"error\":{\"message\":\"stream failed\"}}\n\n";
			using var httpClient = new HttpClient(new FakeHttpMessageHandler(_ => CreateStreamingResponse(responseBody)));
			var provider = new OpenAIProvider("https://example.com", "key", "model", httpClient);

			HttpRequestException? exception = Assert.ThrowsAsync<HttpRequestException>(async () =>
				await ConsumeAsync(provider.CompleteAsync(ValidRequest(), CancellationToken.None)));

			Assert.That(exception!.Message, Does.Contain("stream failed"));
		}

		[Test]
		public async Task CompleteAsync_SkipsMalformedOrEmptyEvents()
		{
			const string responseBody = "data: not-json\n\n"
				+ "data: {\"choices\":[]}\n\n"
				+ "data: {\"choices\":[{\"delta\":{}}]}\n\n"
				+ "data: {\"choices\":[{\"delta\":{\"content\":null}}]}\n\n"
				+ "data: {\"choices\":[{\"delta\":{\"content\":\"ok\"}}]}\n\n"
				+ "data: [DONE]\n\n";
			using var httpClient = new HttpClient(new FakeHttpMessageHandler(_ => CreateStreamingResponse(responseBody)));
			var provider = new OpenAIProvider("https://example.com", "key", "model", httpClient);

			var chunks = await ConsumeAsync(provider.CompleteAsync(ValidRequest(), CancellationToken.None));

			Assert.That(chunks, Is.EqualTo(new[] { "ok" }));
		}

		[Test]
		public void CompleteAsync_RejectsNullRequest()
		{
			using var httpClient = new HttpClient(new FakeHttpMessageHandler(_ => CreateStreamingResponse("data: [DONE]\n\n")));
			var provider = new OpenAIProvider("https://example.com", "key", "model", httpClient);

			Assert.ThrowsAsync<ArgumentNullException>(async () => await ConsumeAsync(provider.CompleteAsync(null!, CancellationToken.None)));
		}

		[TestCase(HttpStatusCode.Unauthorized)]
		[TestCase(HttpStatusCode.NotFound)]
		[TestCase(HttpStatusCode.TooManyRequests)]
		[TestCase(HttpStatusCode.InternalServerError)]
		public void CompleteAsync_PreservesErrorStatusAndBoundsBody(HttpStatusCode statusCode)
		{
			string responseBody = new string('x', 100_000);
			using var httpClient = new HttpClient(new FakeHttpMessageHandler(_ => new HttpResponseMessage(statusCode) {
				Content = new StringContent(responseBody)
			}));
			var provider = new OpenAIProvider("https://example.com", "key", "model", httpClient);

			var exception = Assert.ThrowsAsync<HttpRequestException>(async () =>
				await ConsumeAsync(provider.CompleteAsync(ValidRequest(), CancellationToken.None)));

			Assert.That(exception!.StatusCode, Is.EqualTo(statusCode));
			Assert.That(exception.Message.Length, Is.LessThan(20_000));
			Assert.That(exception.Message, Does.Contain("truncated"));
		}

		[Test]
		public void CompleteAsync_PropagatesCancellation()
		{
			var handler = new FakeHttpMessageHandler(async (_, cancellationToken) => {
				await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
				throw new InvalidOperationException();
			});
			using var httpClient = new HttpClient(handler);
			var provider = new OpenAIProvider("https://example.com", "key", "model", httpClient);
			using var cancellationTokenSource = new CancellationTokenSource();
			cancellationTokenSource.Cancel();

			Assert.ThrowsAsync<OperationCanceledException>(async () =>
				await ConsumeAsync(provider.CompleteAsync(ValidRequest(), cancellationTokenSource.Token)));
		}

		[Test]
		public async Task CompleteAsync_DoesNotDisposeCallerOwnedHttpClient()
		{
			var handler = new FakeHttpMessageHandler(_ => CreateStreamingResponse("data: [DONE]\n\n"));
			using var httpClient = new HttpClient(handler);
			var provider = new OpenAIProvider("https://example.com", "key", "model", httpClient);

			await ConsumeAsync(provider.CompleteAsync(ValidRequest(), CancellationToken.None));
			await ConsumeAsync(provider.CompleteAsync(ValidRequest(), CancellationToken.None));

			Assert.That(handler.RequestCount, Is.EqualTo(2));
		}

		[Test]
		public void TestConnectionAsync_DoesNotSwallowCancellation()
		{
			var handler = new FakeHttpMessageHandler(async (_, cancellationToken) => {
				await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
				throw new InvalidOperationException();
			});
			using var httpClient = new HttpClient(handler);
			var provider = new OpenAIProvider("https://example.com", "key", "model", httpClient);
			using var cancellationTokenSource = new CancellationTokenSource();
			cancellationTokenSource.Cancel();

			Assert.ThrowsAsync<OperationCanceledException>(async () =>
				await provider.TestConnectionAsync(cancellationTokenSource.Token));
		}

		[Test]
		public async Task TestConnectionAsync_ReturnsFalseForRequestFailure()
		{
			using var httpClient = new HttpClient(new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.Unauthorized)));
			var provider = new OpenAIProvider("https://example.com", "key", "model", httpClient);

			bool result = await provider.TestConnectionAsync(CancellationToken.None);

			Assert.That(result, Is.False);
		}

		[Test]
		public async Task TestConnectionAsync_ReturnsTrueAfterFirstChunk()
		{
			const string responseBody = "data: {\"choices\":[{\"delta\":{\"content\":\"Hello\"}}]}\n\n"
				+ "data: [DONE]\n\n";
			using var httpClient = new HttpClient(new FakeHttpMessageHandler(_ => CreateStreamingResponse(responseBody)));
			var provider = new OpenAIProvider("https://example.com", "key", "model", httpClient);

			bool result = await provider.TestConnectionAsync(CancellationToken.None);

			Assert.That(result, Is.True);
		}

		private static LLMRequest ValidRequest()
		{
			return new LLMRequest("", new[] { new LLMMessage("user", "Hello") }, 10);
		}

		private static HttpResponseMessage CreateStreamingResponse(string body)
		{
			return new HttpResponseMessage(HttpStatusCode.OK) {
				Content = new StringContent(body, Encoding.UTF8, "text/event-stream")
			};
		}

		private static async Task<List<string>> ConsumeAsync(IAsyncEnumerable<string> source)
		{
			var result = new List<string>();
			await foreach (string item in source)
			{
				result.Add(item);
			}
			return result;
		}

		private static void AssertMessage(JsonElement message, string role, string content)
		{
			Assert.That(message.GetProperty("role").GetString(), Is.EqualTo(role));
			Assert.That(message.GetProperty("content").GetString(), Is.EqualTo(content));
		}

		private sealed class FakeHttpMessageHandler : HttpMessageHandler
		{
			private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> sendAsync;

			public FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> sendAsync)
				: this((request, _) => Task.FromResult(sendAsync(request)))
			{
			}

			public FakeHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> sendAsync)
			{
				this.sendAsync = sendAsync;
			}

			public int RequestCount { get; private set; }

			protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
			{
				RequestCount++;
				return sendAsync(request, cancellationToken);
			}
		}
	}
}
