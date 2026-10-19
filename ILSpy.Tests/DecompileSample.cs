// Copyright (c) 2026 Christoph Wille
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
using System.Threading.Tasks;

using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// Base of <see cref="DecompileSample"/>. Being the first entity the decompiled sample refers
/// to, it is where "follow the first link in the document" tests land, which keeps that
/// navigation inside this assembly instead of decompiling a framework type.
/// </summary>
public class DecompileSampleBase
{
}

/// <summary>
/// The type UI tests decompile when they need a real document with real members but nothing
/// specific to a framework type: a constant, a field, a property, an event, and two methods with
/// several statements each, one calling the other (so analyzers find a use). Decompiling it is
/// an order of magnitude cheaper than System.String or System.Linq.Enumerable, which is what the
/// tests used before and what made them the slowest in the suite.
/// Open it with <see cref="DecompileSampleExtensions.ShowSampleAsync"/>.
/// </summary>
public class DecompileSample : DecompileSampleBase
{
	public const int Limit = 3;

	public int Count;

	public string Name { get; set; } = "";

	public event EventHandler? Changed;

	public string Describe()
	{
		Count++;
		if (Count > Limit)
			Count = 0;
		Changed?.Invoke(this, EventArgs.Empty);
		return Name + Count;
	}

	public int Twice(int value, int extra)
	{
		string text = Describe();
		int result = value * 2;
		result += extra;
		return result + text.Length;
	}

	public void Reset()
	{
		Count = 0;
		Name = "";
	}
}

/// <summary>
/// A second, unrelated sample type for tests that need "another document" from the same
/// assembly, e.g. to check that a bookmark anchored in <see cref="DecompileSample"/> does not
/// resolve here.
/// </summary>
public class DecompileSampleOther
{
	public int Value;

	public int Next()
	{
		Value++;
		return Value;
	}
}

public static class DecompileSampleExtensions
{
	/// <summary>
	/// Adds the test assembly to the assembly list (a no-op when it is already there) and returns
	/// the tree node of <paramref name="sample"/>, one of the sample types above. Unlike the Open
	/// command, this leaves the tree selection alone, so a test can arrange its own selection
	/// state around the call.
	/// </summary>
	public static async Task<TypeTreeNode> OpenSampleAsync(this MainWindowViewModel vm, Type sample)
	{
		ArgumentNullException.ThrowIfNull(vm);
		ArgumentNullException.ThrowIfNull(sample);
		var loaded = vm.AssemblyTreeModel.AssemblyList!.OpenAssembly(sample.Assembly.Location);
		await loaded.GetLoadResultAsync();
		return vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			sample.Assembly.GetName().Name!, sample.Namespace!, sample.FullName!);
	}

	/// <summary>
	/// Opens the test assembly, selects <paramref name="sample"/> in the tree and waits for its
	/// decompiled text to land in the active decompiler tab.
	/// </summary>
	public static async Task<(TypeTreeNode Node, DecompilerTabPageModel Tab)> ShowSampleAsync(this MainWindowViewModel vm, Type sample)
	{
		var node = await vm.OpenSampleAsync(sample);
		vm.AssemblyTreeModel.SelectNode(node);
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		return (node, tab);
	}
}
