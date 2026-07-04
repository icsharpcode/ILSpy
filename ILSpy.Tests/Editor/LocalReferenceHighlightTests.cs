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

using System.Linq;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.TextView;

/// <summary>
/// Sample type decompiled by <see cref="LocalReferenceHighlightTests"/>: a generic local
/// function inside a generic method, so that every use site carries a specialized method
/// instance while the declaration carries the unspecialized one (issue #2078).
/// </summary>
public class GenericLocalFunctionSample
{
	public T Run<T>(T t)
	{
		return Echo(Echo(t));

		static T2 Echo<T2>(T2 value) => value;
	}
}

/// <summary>
/// Pins the local-reference highlighting for local functions: clicking any occurrence of a
/// generic local function must paint the definition and every use site as one group.
/// </summary>
[TestFixture]
public class LocalReferenceHighlightTests
{
	[AvaloniaTest]
	public async Task Clicking_A_Generic_Local_Function_Use_Highlights_Definition_And_All_Uses()
	{
		var (window, vm) = await TestHarness.BootAsync();
		await vm.OpenAssemblyAsync(typeof(GenericLocalFunctionSample).Assembly.Location);
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"ILSpy.Tests",
			"ICSharpCode.ILSpy.Tests.TextView",
			"ICSharpCode.ILSpy.Tests.TextView.GenericLocalFunctionSample");
		vm.AssemblyTreeModel.SelectNode(typeNode);
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		var view = window.GetVisualDescendants().OfType<DecompilerTextView>().First();

		var localFunctionSegments = tab.References!
			.Where(r => r.IsLocal && r.Reference is IMethod { IsLocalFunction: true })
			.ToList();
		localFunctionSegments.Should().HaveCount(3, "the local function has one definition and two calls");
		localFunctionSegments.Count(r => r.IsDefinition).Should().Be(1);

		var use = localFunctionSegments.First(r => !r.IsDefinition);
		view.OnReferenceClicked(use);

		view.LocalReferenceMarks.Select(m => m.StartOffset).Should().BeEquivalentTo(
			localFunctionSegments.Select(s => s.StartOffset),
			"clicking a use must highlight the definition and every use");
	}
}
