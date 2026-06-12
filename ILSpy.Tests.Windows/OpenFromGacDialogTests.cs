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

using System.Linq;

using AwesomeAssertions;

using ICSharpCode.Decompiler.Metadata;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Windows;

/// <summary>
/// Smoke test for the Windows-conditional GAC enumeration path used by
/// <c>ICSharpCode.ILSpy.Views.OpenFromGacDialog</c>. The shared
/// <see cref="UniversalAssemblyResolver.EnumerateGac"/> only returns entries when run
/// on Windows with a populated GAC; this test validates the project plumbing more than
/// the GAC contents — i.e. the call returns without throwing and the project compiles
/// against Windows-only BCL APIs.
/// </summary>
[TestFixture]
public class OpenFromGacDialogTests
{
	[Test]
	public void EnumerateGac_Returns_Without_Throwing_On_Windows()
	{
		// On a developer machine with .NET 4.x installed, this should also return a non-empty
		// result. On a stripped CI runner the result may be empty; the test only asserts that
		// the enumeration completes — that's enough to catch a regression where the call
		// throws (e.g. registry permission change, native-method signature drift).
		var result = UniversalAssemblyResolver.EnumerateGac().ToList();
		result.Should().NotBeNull();
	}
}
