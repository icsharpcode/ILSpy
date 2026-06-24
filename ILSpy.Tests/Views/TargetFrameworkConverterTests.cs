// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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

using AwesomeAssertions;

using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// The Set-Target-Framework dialog accepts the short TFMs users know (net48, net6.0, ...) but
/// ILSpy resolves with the long FrameworkName form (.NETFramework,Version=v4.8). These tests
/// pin that conversion/validation, which is the part of the dialog worth unit-testing in
/// isolation from the window.
/// </summary>
[TestFixture]
public class TargetFrameworkConverterTests
{
	[TestCase("net48", ".NETFramework,Version=v4.8")]
	[TestCase("net472", ".NETFramework,Version=v4.7.2")]
	[TestCase("net6.0", ".NETCoreApp,Version=v6.0")]
	[TestCase("net8.0", ".NETCoreApp,Version=v8.0")]
	[TestCase("netstandard2.0", ".NETStandard,Version=v2.0")]
	[TestCase("netcoreapp3.1", ".NETCoreApp,Version=v3.1")]
	public void Short_Tfm_Parses_To_FrameworkName(string input, string expected)
	{
		TargetFrameworkConverter.TryParseToFrameworkName(input, out var frameworkName, out _)
			.Should().BeTrue();
		frameworkName.Should().Be(expected);
	}

	[Test]
	public void Long_FrameworkName_Is_Also_Accepted()
	{
		TargetFrameworkConverter.TryParseToFrameworkName(".NETCoreApp,Version=v6.0", out var frameworkName, out _)
			.Should().BeTrue();
		frameworkName.Should().Be(".NETCoreApp,Version=v6.0");
	}

	[TestCase("")]
	[TestCase("   ")]
	[TestCase("not-a-tfm")]
	[TestCase("garbage123")]
	public void Invalid_Input_Is_Rejected_With_An_Error(string input)
	{
		TargetFrameworkConverter.TryParseToFrameworkName(input, out var frameworkName, out var error)
			.Should().BeFalse();
		frameworkName.Should().BeNull();
		error.Should().NotBeNullOrEmpty();
	}

	[TestCase(".NETFramework,Version=v4.8", "net48")]
	[TestCase(".NETCoreApp,Version=v6.0", "net6.0")]
	[TestCase(".NETStandard,Version=v2.0", "netstandard2.0")]
	public void FrameworkName_Converts_To_Short_Folder_Name_For_Display(string frameworkName, string expected)
	{
		TargetFrameworkConverter.ToShortFolderName(frameworkName).Should().Be(expected);
	}
}
