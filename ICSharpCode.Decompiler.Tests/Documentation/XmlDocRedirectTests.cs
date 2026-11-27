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

using ICSharpCode.Decompiler.Documentation;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.Documentation;

[TestFixture]
public class XmlDocRedirectTests
{
	[TestCase(false, TestName = "IgnoresRelativeRedirectFromAssemblySidecar")]
	[TestCase(true, TestName = "IgnoresAbsoluteRedirectFromAssemblySidecar")]
	public void IgnoresRedirectFromAssemblySidecar(bool useAbsolutePath)
	{
		string tempDir = Path.Combine(Path.GetTempPath(), "XmlDocRedirect_" + Guid.NewGuid().ToString("N"));
		string assemblyDir = Path.Combine(tempDir, "assembly");
		string victimDir = Path.Combine(tempDir, "victim");
		Directory.CreateDirectory(assemblyDir);
		Directory.CreateDirectory(victimDir);
		string sidecarPath = Path.Combine(assemblyDir, "Evil.xml");
		string victimPath = Path.Combine(victimDir, "Acme.Billing.xml");
		string redirect = useAbsolutePath ? victimPath : Path.Combine("..", "victim", "Acme.Billing.xml");
		File.WriteAllText(sidecarPath, $"<doc redirect=\"{redirect}\" />");
		File.WriteAllText(victimPath, """
			<doc>
				<members>
					<member name="T:Acme.Billing.LicenseServer">
						<summary>Internal documentation marker.</summary>
					</member>
				</members>
			</doc>
			""");
		try
		{
			var provider = new XmlDocumentationProvider(sidecarPath);

			Assert.That(provider.GetDocumentation("T:Acme.Billing.LicenseServer"), Is.Null);
		}
		finally
		{
			Directory.Delete(tempDir, recursive: true);
		}
	}

	[Test]
	public void FollowsRedirectWithinTrustedDocumentationDirectory()
	{
		string tempDir = Path.Combine(Path.GetTempPath(), "XmlDocRedirect_" + Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(tempDir);
		string sidecarPath = Path.Combine(tempDir, "Stub.xml");
		string targetPath = Path.Combine(tempDir, "Reference.xml");
		File.WriteAllText(sidecarPath, "<doc redirect=\"Reference.xml\" />");
		File.WriteAllText(targetPath, """
			<doc>
				<members>
					<member name="T:Acme.Billing.LicenseServer">
						<summary>Framework reference documentation.</summary>
					</member>
				</members>
			</doc>
			""");
		try
		{
			var provider = new XmlDocumentationProvider(sidecarPath, allowRedirection: true);

			Assert.That(provider.GetDocumentation("T:Acme.Billing.LicenseServer"),
				Does.Contain("Framework reference documentation."));
		}
		finally
		{
			Directory.Delete(tempDir, recursive: true);
		}
	}

	[TestCase(false, TestName = "IgnoresRelativeRedirectOutsideTrustedDirectory")]
	[TestCase(true, TestName = "IgnoresAbsoluteRedirectOutsideTrustedDirectory")]
	public void IgnoresRedirectOutsideTrustedDirectory(bool useAbsolutePath)
	{
		string tempDir = Path.Combine(Path.GetTempPath(), "XmlDocRedirect_" + Guid.NewGuid().ToString("N"));
		string sourceDir = Path.Combine(tempDir, "framework");
		string victimDir = Path.Combine(tempDir, "victim");
		Directory.CreateDirectory(sourceDir);
		Directory.CreateDirectory(victimDir);
		string sidecarPath = Path.Combine(sourceDir, "Stub.xml");
		string victimPath = Path.Combine(victimDir, "Acme.Billing.xml");
		string redirect = useAbsolutePath ? victimPath : Path.Combine("..", "victim", "Acme.Billing.xml");
		File.WriteAllText(sidecarPath, $"<doc redirect=\"{redirect}\" />");
		File.WriteAllText(victimPath, """
			<doc>
				<members>
					<member name="T:Acme.Billing.LicenseServer">
						<summary>Internal documentation marker.</summary>
					</member>
				</members>
			</doc>
			""");
		try
		{
			var provider = new XmlDocumentationProvider(sidecarPath, allowRedirection: true);

			Assert.That(provider.GetDocumentation("T:Acme.Billing.LicenseServer"), Is.Null);
		}
		finally
		{
			Directory.Delete(tempDir, recursive: true);
		}
	}

	[Test]
	public void IgnoresProgramFilesPlaceholderWhenProgramFilesDirectoryIsUnavailable()
	{
		if (!string.IsNullOrEmpty(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)))
			Assert.Ignore("This placeholder resolves to the filesystem root only when Program Files is unavailable.");

		string tempDir = Path.Combine(Path.GetTempPath(), "XmlDocRedirect_" + Guid.NewGuid().ToString("N"));
		string assemblyDir = Path.Combine(tempDir, "assembly");
		string victimDir = Path.Combine(tempDir, "victim");
		Directory.CreateDirectory(assemblyDir);
		Directory.CreateDirectory(victimDir);
		string sidecarPath = Path.Combine(assemblyDir, "Evil.xml");
		string victimPath = Path.Combine(victimDir, "Acme.Billing.xml");
		string targetPath = victimPath.Substring(Path.GetPathRoot(victimPath).Length);
		string redirect = "%PROGRAMFILESDIR%" + targetPath.Replace(Path.DirectorySeparatorChar, '/');
		File.WriteAllText(sidecarPath, $"<doc redirect=\"{redirect}\" />");
		File.WriteAllText(victimPath, """
			<doc>
				<members>
					<member name="T:Acme.Billing.LicenseServer">
						<summary>Internal documentation marker.</summary>
					</member>
				</members>
			</doc>
			""");
		try
		{
			var provider = new XmlDocumentationProvider(sidecarPath, allowRedirection: true);

			Assert.That(provider.GetDocumentation("T:Acme.Billing.LicenseServer"), Is.Null);
		}
		finally
		{
			Directory.Delete(tempDir, recursive: true);
		}
	}
}
