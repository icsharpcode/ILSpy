// Copyright (c) 2026 Siegfried Pammer
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
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using AwesomeAssertions;

using ICSharpCode.ILSpy.NuGetFeeds;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.NuGetFeeds;

[TestFixture]
public class NuGetFeedClientTests
{
	string emptyFolderFeed = null!;

	[SetUp]
	public void CreateEmptyFolderFeed()
	{
		emptyFolderFeed = Path.Combine(Path.GetTempPath(), "ILSpy.Tests", "empty-feed-" + Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(emptyFolderFeed);
	}

	[TearDown]
	public void DeleteEmptyFolderFeed() => Directory.Delete(emptyFolderFeed, recursive: true);

	[TestCase("../../.dotnet/tools/.store/dotnet-ef")]
	[TestCase(@"..\..\evil")]
	[TestCase("/etc/evil")]
	[TestCase("evil/1.0.0")]
	[TestCase("")]
	public async Task GetVersionsAsync_Rejects_Ids_That_Are_Not_Package_Ids(string id)
	{
		var client = new NuGetFeedClient();

		Func<Task> versions = () => client.GetVersionsAsync(
			emptyFolderFeed, id, includePrerelease: false, maxCount: 10, CancellationToken.None);

		(await versions.Should().ThrowAsync<InvalidOperationException>())
			.WithMessage("*not a valid package id*");
	}

	[TestCase("../../.dotnet/tools/.store/dotnet-ef")]
	[TestCase(@"..\..\evil")]
	[TestCase("/etc/evil")]
	[TestCase("evil/1.0.0")]
	[TestCase("")]
	public async Task DownloadToGlobalPackagesFolderAsync_Rejects_Ids_That_Are_Not_Package_Ids(string id)
	{
		var client = new NuGetFeedClient();

		Func<Task> download = () => client.DownloadToGlobalPackagesFolderAsync(
			emptyFolderFeed, id, "1.0.0", CancellationToken.None);

		(await download.Should().ThrowAsync<InvalidOperationException>())
			.WithMessage("*not a valid package id*");
	}

	[Test]
	public async Task GetVersionsAsync_Accepts_A_Well_Formed_Id()
	{
		var client = new NuGetFeedClient();

		var versions = await client.GetVersionsAsync(
			emptyFolderFeed, "Newtonsoft.Json", includePrerelease: false, maxCount: 10, CancellationToken.None);

		versions.Should().BeEmpty("an empty folder feed has nothing to offer, but a proper id must not be refused");
	}
}
