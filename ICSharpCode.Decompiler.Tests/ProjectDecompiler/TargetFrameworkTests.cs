// Copyright (c) 2020 Daniel Grunwald
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
using System.Reflection;

using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.Decompiler.Metadata;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.ProjectDecompiler
{
	[TestFixture]
	public sealed class TargetFrameworkTests
	{
		[TestCase(-1)]
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(99)]
		[TestCase(int.MinValue)]
		public void VerifyThrowsForInvalidVersion(int invalidVersion)
		{
			// Arrange - nothing

			// Act
			void CreateInstance() => new TargetFramework(identifier: null, invalidVersion, profile: null);

			// Assert
			Assert.Throws<ArgumentException>(CreateInstance);
		}

		[TestCase(100, "v1.0")]
		[TestCase(102, "v1.0.2")]
		[TestCase(130, "v1.3")]
		[TestCase(145, "v1.4.5")]
		[TestCase(1670, "v16.7")]
		[TestCase(1800, "v18.0")]
		public void VerifyVersion(int version, string expectedVersion)
		{
			// Arrange - nothing

			// Act
			var targetFramework = new TargetFramework(identifier: null, version, profile: null);

			// Assert
			Assert.That(targetFramework.VersionNumber, Is.EqualTo(version));
			Assert.That(targetFramework.VersionString, Is.EqualTo(expectedVersion));
		}

		[Test]
		public void VerifyPortableLibrary()
		{
			// Arrange
			const string identifier = ".NETPortable";

			// Act
			var targetFramework = new TargetFramework(identifier, 100, profile: null);

			// Assert
			Assert.That(targetFramework.IsPortableClassLibrary);
			Assert.That(targetFramework.Identifier, Is.EqualTo(identifier));
		}

		[Test]
		[Pairwise]
		public void VerifyIdentifierAndProfile(
			[Values(null, "", ".NETFramework")] string identifier,
			[Values(null, "", ".Client")] string profile)
		{
			// Arrange - nothing

			// Act
			var targetFramework = new TargetFramework(identifier, 100, profile);

			// Assert
			Assert.That(targetFramework.Identifier, Is.EqualTo(identifier));
			Assert.That(targetFramework.Profile, Is.EqualTo(profile));
		}

		[TestCase(null, 350, "net35")]
		[TestCase(".NETFramework", 350, "net35")]
		[TestCase(".NETFramework", 400, "net40")]
		[TestCase(".NETFramework", 451, "net451")]
		[TestCase(".NETCoreApp", 200, "netcoreapp2.0")]
		[TestCase(".NETCoreApp", 310, "netcoreapp3.1")]
		[TestCase(".NETStandard", 130, "netstandard1.3")]
		[TestCase(".NETStandard", 200, "netstandard2.0")]
		[TestCase("Silverlight", 400, "sl4")]
		[TestCase("Silverlight", 550, "sl5")]
		[TestCase(".NETCore", 450, "netcore45")]
		[TestCase(".NETCore", 451, "netcore451")]
		[TestCase("WindowsPhone", 700, "wp7")]
		[TestCase("WindowsPhone", 810, "wp81")]
		[TestCase(".NETMicroFramework", 100, "netmf")]
		[TestCase(".NETMicroFramework", 210, "netmf")]
		[TestCase(".NETPortable", 100, null)]
		[TestCase("Unsupported", 100, null)]
		public void VerifyMoniker(string identifier, int version, string expectedMoniker)
		{
			// Arrange - nothing

			// Act
			var targetFramework = new TargetFramework(identifier, version, profile: null);

			// Assert
			Assert.That(targetFramework.Moniker, Is.EqualTo(expectedMoniker));
		}

		[TestCase(".NETCoreApp, Version=v5.0", TargetFrameworkIdentifier.NET, "5.0.0")]
		[TestCase(".NETCoreApp, Version=v10.0", TargetFrameworkIdentifier.NET, "10.0.0")]
		public void VerifyUniversalAssemblyResolverParseTargetFramework(string targetFramework, TargetFrameworkIdentifier identifier, string version)
		{
			var (id, v) = UniversalAssemblyResolver.ParseTargetFramework(targetFramework);
			Assert.That(id, Is.EqualTo(identifier));
			Assert.That(v.ToString(3), Is.EqualTo(version));
		}

		[TestCase(TargetFrameworkIdentifier.NET, true)]
		[TestCase(TargetFrameworkIdentifier.NETCoreApp, true)]
		[TestCase(TargetFrameworkIdentifier.NETStandard, true)]
		[TestCase(TargetFrameworkIdentifier.Silverlight, false)]
		public void VerifyUseOfDotNetCorePathFinder(TargetFrameworkIdentifier identifier, bool expected)
		{
			Assert.That(UniversalAssemblyResolver.UsesDotNetCorePathFinder(identifier), Is.EqualTo(expected));
		}
		[Test]
		public void NetStandardResolvesSharedRuntimeAssembly()
		{
			var reference = AssemblyNameReference.Parse(
				"System.Memory, Version=4.0.1.2, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51");
			var resolver = new UniversalAssemblyResolver(null, false, ".NETStandard,Version=v2.0");

			var file = resolver.FindAssemblyFile(reference);

			Assert.That(file, Is.Not.Null);
			Assert.That(File.Exists(file), Is.True);
		}

		[Test]
		public void NetCoreAppResolvesCompatibleSharedRuntimeAssembly()
		{
			var reference = AssemblyNameReference.Parse(
				"System.Text.Json, Version=10.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51");
			var resolver = new UniversalAssemblyResolver(null, false, ".NETCoreApp,Version=v3.1");

			var file = resolver.FindAssemblyFile(reference);

			Assert.That(file, Is.Not.Null);
			Assert.That(File.Exists(file), Is.True);
			Assert.That(AssemblyName.GetAssemblyName(file!).Version, Is.GreaterThanOrEqualTo(reference.Version));
		}

		[Test]
		public void LaterRuntimeAssemblyResolvesWithoutBecomingImplicitProjectReference()
		{
			var reference = AssemblyNameReference.Parse(
				"System.Net.ServerSentEvents, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
			var resolver = new UniversalAssemblyResolver(null, false, ".NETCoreApp,Version=v8.0");

			Assert.That(resolver.FindAssemblyFile(reference), Is.Not.Null);
			Assert.That(resolver.IsSharedAssembly(reference, out _), Is.False);
		}

		[TestCase(TargetFrameworkIdentifier.NETStandard, ".NETStandard,Version=v2.0", PlatformID.Win32NT, false)]
		[TestCase(TargetFrameworkIdentifier.NET, ".NETFramework,Version=v4.7.2", PlatformID.Win32NT, false)]
		[TestCase(TargetFrameworkIdentifier.NETCoreApp, ".NETCoreApp,Version=v3.1", PlatformID.Win32NT, true)]
		[TestCase(TargetFrameworkIdentifier.NET, ".NETCoreApp,Version=v10.0", PlatformID.Win32NT, true)]
		[TestCase(TargetFrameworkIdentifier.NETStandard, ".NETStandard,Version=v2.0", PlatformID.Unix, true)]
		public void VerifyHostRuntimeFallback(TargetFrameworkIdentifier identifier, string targetFramework,
			PlatformID platform, bool expected)
		{
			Assert.That(UniversalAssemblyResolver.ShouldUseHostRuntimeFallback(identifier, targetFramework, platform),
				Is.EqualTo(expected));
		}
	}
}
