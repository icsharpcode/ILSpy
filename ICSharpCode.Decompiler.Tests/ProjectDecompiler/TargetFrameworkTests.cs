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
using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests
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
			Assert.AreEqual(version, targetFramework.VersionNumber);
			Assert.AreEqual(expectedVersion, targetFramework.VersionString);
		}

		[Test]
		public void VerifyPortableLibrary()
		{
			// Arrange
			const string identifier = ".NETPortable";

			// Act
			var targetFramework = new TargetFramework(identifier, 100, profile: null);

			// Assert
			Assert.IsTrue(targetFramework.IsPortableClassLibrary);
			Assert.AreEqual(identifier, targetFramework.Identifier);
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
			Assert.AreEqual(identifier, targetFramework.Identifier);
			Assert.AreEqual(profile, targetFramework.Profile);
		}
	}
}
