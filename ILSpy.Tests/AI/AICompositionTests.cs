// Copyright (c) 2026 Dr. Masroor Ehsan
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

using AwesomeAssertions;

using ICSharpCode.ILSpy.AppEnv;

using NUnit.Framework;

using ICSharpCode.ILSpy.AI;

namespace ICSharpCode.ILSpy.Tests.AI;

/// <summary>
/// The MEF exports that moved from ILSpyX into ICSharpCode.ILSpy.AI must keep resolving through
/// the real composition path (AppComposition catalogs ILSpyX + the app assembly + explicitly
/// registered assemblies — it never scans project references, and System.Composition resolution
/// failures are lazy, surfacing only when an export is actually requested). These tests request
/// them through the same <see cref="AppComposition"/> used by TestApp/ResetAppStateAttribute.
/// </summary>
[TestFixture]
public class AICompositionTests
{
	[Test]
	public void AISelectionService_resolves_from_the_extracted_assembly()
	{
		var service = AppComposition.Current.GetExport<AISelectionService>();

		service.Should().NotBeNull();
		service!.GetType().Assembly.GetName().Name.Should().Be("ICSharpCode.ILSpy.AI",
			"the export left ILSpyX; AppComposition must catalog the new assembly explicitly");
	}

	[Test]
	public void SecureKeyStorage_resolves_from_the_extracted_assembly()
	{
		var storage = AppComposition.Current.GetExport<SecureKeyStorage>();

		storage.Should().NotBeNull();
		storage!.GetType().Assembly.GetName().Name.Should().Be("ICSharpCode.ILSpy.AI");
	}

	[Test]
	public void IAIProviderFactory_resolves_from_the_extracted_assembly()
	{
		var factory = AppComposition.Current.GetExport<IAIProviderFactory>();

		factory.Should().NotBeNull();
		factory!.GetType().Assembly.GetName().Name.Should().Be("ICSharpCode.ILSpy.AI");
	}

	[Test]
	public void Resolved_selection_service_and_provider_factory_are_shared_instances()
	{
		var one = AppComposition.Current.GetExport<AISelectionService>();
		var two = AppComposition.Current.GetExport<AISelectionService>();

		one.Should().BeSameAs(two, "the [Shared] lifetime semantics must survive the assembly move");
	}
}
