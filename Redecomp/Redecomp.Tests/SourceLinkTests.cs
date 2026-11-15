// Copyright (c) 2026 Franck
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

using Redecomp.Bench;
using Redecomp.Core.Reconstruction;
using Redecomp.Core.Recovery;

namespace Redecomp.Tests;

public static class SourceLinkTests
{
	[Test]
	public static void MapsDocumentPathsToUrls()
	{
		var map = SourceLinkMap.Parse("""
			{ "documents": {
			    "/build/app/*": "https://raw.example.com/org/repo/abc123/*",
			    "/build/app/vendor/*": "https://raw.example.com/org/vendor/def456/*",
			    "C:\\work\\lib\\*": "https://raw.example.com/org/lib/789/*",
			    "/build/exact.cs": "https://raw.example.com/exact.cs"
			} }
			""");
		Assert.AreEqual("https://raw.example.com/org/repo/abc123/Models/Facture.cs", map.Map("/build/app/Models/Facture.cs")?.ToString(), "wildcard");
		Assert.AreEqual("https://raw.example.com/org/vendor/def456/Json.cs", map.Map("/build/app/vendor/Json.cs")?.ToString(), "longest prefix");
		Assert.AreEqual("https://raw.example.com/org/lib/789/Sub/Util.cs", map.Map("C:\\work\\lib\\Sub\\Util.cs")?.ToString(), "Windows path");
		Assert.AreEqual("https://raw.example.com/exact.cs", map.Map("/build/exact.cs")?.ToString(), "exact entry");
		Assert.AreEqual(null, map.Map("/elsewhere/File.cs"), "unmapped document");
	}

	[Test]
	public static void RecoversOriginalSourcesThroughSourceLink()
	{
		var downloader = new SampleDownloader("Facturation");
		string output = Reconstruct(downloader, out var result);
		Assert.AreEqual(SourceOrigin.SourceLink, result.Origin, "origin");
		Assert.That(downloader.Requests > 0, "nothing was downloaded");
		foreach (string relative in new[] { "Program.cs", "Models/Facture.cs", "Services/CalculateurFacture.cs" })
		{
			byte[] original = File.ReadAllBytes(Path.Combine(SampleBuilder.SourceDirectory("Facturation"), relative));
			Assert.That(original.AsSpan().SequenceEqual(File.ReadAllBytes(Path.Combine(output, relative))), $"{relative} differs from the original");
		}
		ProjectBuilder.BuildProject(output, "Facturation");
	}

	[Test]
	public static void RejectsFilesWhoseChecksumDoesNotMatchThePdb()
	{
		var downloader = new SampleDownloader("Facturation") { Tamper = true };
		string output = Reconstruct(downloader, out var result);
		Assert.AreEqual(SourceOrigin.Decompiled, result.Origin, "origin");
		Assert.That(!File.ReadAllText(Path.Combine(output, "Program.cs")).Contains("// tampered"), "a file failing the checksum was used");
	}

	[Test]
	public static void FallsBackToDecompilationWhenSourcesAreUnreachable()
	{
		string output = Reconstruct(new SampleDownloader("Facturation") { Offline = true }, out var result);
		Assert.AreEqual(SourceOrigin.Decompiled, result.Origin, "origin");
		ProjectBuilder.BuildProject(output, "Facturation");
	}

	static string Reconstruct(ISourceDownloader downloader, out ReconstructionResult result)
	{
		string output = ProjectBuilder.NewTempDirectory();
		var options = ReconstructionOptions.Default with { SourceDownloader = downloader };
		result = ProjectReconstructor.Reconstruct(SampleBuilder.Build("Facturation", BuildVariant.SourceLink), output, options);
		return output;
	}

	/// <summary>Serves the sample's files for the URLs SourceLink maps them to, without network access.</summary>
	sealed class SampleDownloader(string sample) : ISourceDownloader
	{
		public bool Tamper { get; init; }
		public bool Offline { get; init; }
		public int Requests { get; private set; }

		public byte[]? TryDownload(Uri uri)
		{
			Requests++;
			string prefix = SampleBuilder.SourceLinkBaseUrl + "/" + sample + "/";
			if (Offline || !uri.ToString().StartsWith(prefix, StringComparison.Ordinal))
				return null;
			string path = Path.Combine(SampleBuilder.SourceDirectory(sample), Uri.UnescapeDataString(uri.ToString()[prefix.Length..]));
			if (!File.Exists(path))
				return null;
			byte[] content = File.ReadAllBytes(path);
			return Tamper ? [.. content, .. "// tampered\n"u8.ToArray()] : content;
		}
	}
}
