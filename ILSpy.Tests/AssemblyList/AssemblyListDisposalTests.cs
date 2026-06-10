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

using System.Reflection;

using AwesomeAssertions;

using ICSharpCode.ILSpyX;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// The assembly list must NOT dispose a <see cref="LoadedAssembly"/> (and so its
/// <c>MetadataFile</c>) when it removes one: open document tabs and tree nodes can still hold the
/// MetadataFile, and the list has no safe point to know those references are gone. Disposing
/// unmaps the file out from under a live reader -> use-after-dispose. Removed assemblies are
/// dropped and left to the GC instead.
/// </summary>
[TestFixture]
public class AssemblyListDisposalTests
{
	static bool IsDisposed(LoadedAssembly assembly)
		=> (bool)typeof(LoadedAssembly)
			.GetField("isDisposed", BindingFlags.NonPublic | BindingFlags.Instance)!
			.GetValue(assembly)!;

	[Test]
	public void Unload_Does_Not_Dispose_The_Removed_Assembly()
	{
		var list = new AssemblyList();
		var assembly = list.OpenAssembly(typeof(AssemblyListDisposalTests).Assembly.Location);
		assembly.Should().NotBeNull("precondition: the test assembly opens");
		// Force the load to complete so there's a real MetadataFile that disposal would unmap.
		assembly!.GetMetadataFileAsync().GetAwaiter().GetResult();

		list.Unload(assembly);

		IsDisposed(assembly).Should().BeFalse(
			"Unload must not dispose the LoadedAssembly / MetadataFile -- open tabs or tree nodes "
			+ "may still reference it, so the list leaves it to the GC instead of risking a "
			+ "use-after-dispose");
	}

	[Test]
	public void Clear_Does_Not_Dispose_The_Removed_Assemblies()
	{
		var list = new AssemblyList();
		var assembly = list.OpenAssembly(typeof(AssemblyListDisposalTests).Assembly.Location);
		assembly!.GetMetadataFileAsync().GetAwaiter().GetResult();

		list.Clear();

		IsDisposed(assembly).Should().BeFalse(
			"Clear must not dispose the assemblies it removes -- same use-after-dispose hazard as Unload");
	}
}
