using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Updates;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class UpdateServiceTests
{
	[Test]
	public async Task GetLatestVersionAsync_UsesReleaseTag_WhenReleaseTagIsPresent()
	{
		const string xml = """
		<updateInfo>
		  <band id="stable">
		    <latestVersion>10.0.0.0</latestVersion>
		    <releaseTag>v10.0</releaseTag>
		    <downloadUrl>https://example.com/ignored.zip</downloadUrl>
		  </band>
		</updateInfo>
		""";

		using var client = new HttpClient(new StubHttpMessageHandler(xml));

		var result = await UpdateService.GetLatestVersionAsync(client, new Uri("https://example.com/updates.xml"));

		result.Version.Should().Be(new Version(10, 0, 0, 0));
		result.DownloadUrl.Should().Be("https://github.com/icsharpcode/ILSpy/releases/tag/v10.0");
	}

	[Test]
	public async Task GetLatestVersionAsync_ReturnsNullDownloadUrl_WhenReleaseTagContainsPathTraversalAttempt()
	{
		const string xml = """
		<updateInfo>
		  <band id="stable">
		    <latestVersion>10.0.0.0</latestVersion>
		    <releaseTag>../malicious</releaseTag>
		    <downloadUrl>https://example.com/ignored.zip</downloadUrl>
		  </band>
		</updateInfo>
		""";

		using var client = new HttpClient(new StubHttpMessageHandler(xml));

		var result = await UpdateService.GetLatestVersionAsync(client, new Uri("https://example.com/updates.xml"));

		result.Version.Should().Be(new Version(10, 0, 0, 0));
		result.DownloadUrl.Should().BeNull();
	}

	[Test]
	public async Task GetLatestVersionAsync_UsesDownloadUrl_WhenReleaseTagIsMissing()
	{
		const string xml = """
		<updateInfo>
		  <band id="stable">
		    <latestVersion>10.0.0.0</latestVersion>
		    <downloadUrl>https://github.com/icsharpcode/ILSpy/releases/tag/v10.0</downloadUrl>
		  </band>
		</updateInfo>
		""";

		using var client = new HttpClient(new StubHttpMessageHandler(xml));

		var result = await UpdateService.GetLatestVersionAsync(client, new Uri("https://example.com/updates.xml"));

		result.Version.Should().Be(new Version(10, 0, 0, 0));
		result.DownloadUrl.Should().Be("https://github.com/icsharpcode/ILSpy/releases/tag/v10.0");
	}

	[Test]
	public async Task GetLatestVersionAsync_UsesDownloadUrl_ButFailsBecauseBaseUrlDoesntMatch()
	{
		const string xml = """
		<updateInfo>
		  <band id="stable">
		    <latestVersion>10.0.0.0</latestVersion>
		    <downloadUrl>https://example.com/ilspy.zip</downloadUrl>
		  </band>
		</updateInfo>
		""";

		using var client = new HttpClient(new StubHttpMessageHandler(xml));

		var result = await UpdateService.GetLatestVersionAsync(client, new Uri("https://example.com/updates.xml"));

		result.Version.Should().Be(new Version(10, 0, 0, 0));
		result.DownloadUrl.Should().BeNull();
	}

	sealed class StubHttpMessageHandler(string responseContent) : HttpMessageHandler
	{
		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) {
				Content = new StringContent(responseContent)
			});
		}
	}
}
