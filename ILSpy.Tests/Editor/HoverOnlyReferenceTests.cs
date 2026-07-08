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

using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.Output;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.TextView;

/// <summary>
/// Sample type decompiled by <see cref="HoverOnlyReferenceTests"/>: a dynamic member access, whose
/// synthesized member the decompiler renders as a hover-only reference.
/// </summary>
public class DynamicMemberSample
{
	public object Get(dynamic d)
	{
		return d.Property;
	}

	public object Call(dynamic d)
	{
		return d.Compute(1, "two", d);
	}
}

/// <summary>
/// Pins the third reference mode. A member synthesized for a dynamic access shows a hover tooltip but
/// is not a navigation target and, unlike a local variable, clicking it does not highlight occurrences
/// (it is a distinct synthetic member at every use).
/// </summary>
[TestFixture]
public class HoverOnlyReferenceTests
{
	[AvaloniaTest]
	public async Task Dynamic_Member_Is_HoverOnly_And_A_Click_Neither_Navigates_Nor_Highlights()
	{
		var (window, vm) = await TestHarness.BootAsync();
		await vm.OpenAssemblyAsync(typeof(DynamicMemberSample).Assembly.Location);
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"ILSpy.Tests",
			"ICSharpCode.ILSpy.Tests.TextView",
			"ICSharpCode.ILSpy.Tests.TextView.DynamicMemberSample");
		vm.AssemblyTreeModel.SelectNode(typeNode);
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		var view = window.GetVisualDescendants().OfType<DecompilerTextView>().First();

		var hoverOnly = tab.References!
			.Where(r => r.Kind == ReferenceMode.HoverOnly)
			.ToList();
		hoverOnly.Should().NotBeEmpty("the dynamic member access 'd.Property' is a hover-only reference");

		var navigated = false;
		tab.NavigateRequested += _ => navigated = true;
		view.OnReferenceClicked(hoverOnly.First());

		navigated.Should().BeFalse("clicking a hover-only reference must not navigate");
		view.LocalReferenceMarks.Should().BeEmpty("a hover-only reference must not highlight occurrences");
	}

	[AvaloniaTest]
	public async Task Dynamic_Invoke_Member_Hover_Renders_Its_Synthesized_Signature()
	{
		var (_, vm) = await TestHarness.BootAsync();
		await vm.OpenAssemblyAsync(typeof(DynamicMemberSample).Assembly.Location);
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"ILSpy.Tests",
			"ICSharpCode.ILSpy.Tests.TextView",
			"ICSharpCode.ILSpy.Tests.TextView.DynamicMemberSample");
		vm.AssemblyTreeModel.SelectNode(typeNode);
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		// The 'Compute' hover target is the member synthesized for the dynamic call d.Compute(1, "two", d).
		var compute = tab.References!
			.Select(r => r.Reference)
			.OfType<IMethod>()
			.First(m => m.Name == "Compute");

		var ambience = new CSharpAmbience {
			ConversionFlags = ConversionFlags.ShowReturnType | ConversionFlags.ShowParameterList,
		};
		ambience.ConvertSymbol(compute).Should().Be(
			"dynamic Compute(int, string, dynamic)",
			"each argument is typed from the callsite: the constants keep their compile-time type, "
			+ "the dynamic argument stays dynamic");
	}
}
