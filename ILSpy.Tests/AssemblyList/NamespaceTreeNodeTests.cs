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

using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class NamespaceTreeNodeTests
{
	[AvaloniaTest]
	public async Task Text_Escapes_Characters_That_Cannot_Be_Displayed()
	{
		// Namespace names come straight out of the metadata string heap, which permits whitespace
		// and control characters that would corrupt the tree row if rendered raw. The label is
		// escaped for display; Name and FullName stay raw because they are the lookup keys used
		// against metadata.

		var (_, vm) = await TestHarness.BootAsync(3);
		var module = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq")
			.LoadedAssembly.GetMetadataFileOrNull()!;

		var node = new NamespaceTreeNode("Weird\tNamespace", module);

		Assert.That(node.Text.ToString(), Is.EqualTo("Weird\\u0009Namespace"));
		Assert.That(node.Name, Is.EqualTo("Weird\tNamespace"), "Name stays raw -- it is a lookup key");
		Assert.That(node.FullName, Is.EqualTo("Weird\tNamespace"), "FullName stays raw -- it is a lookup key");
	}

	[AvaloniaTest]
	public async Task Text_Renders_The_Global_Namespace_As_Dash()
	{
		var (_, vm) = await TestHarness.BootAsync(3);
		var module = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq")
			.LoadedAssembly.GetMetadataFileOrNull()!;

		var node = new NamespaceTreeNode(string.Empty, module);

		Assert.That(node.Text.ToString(), Is.EqualTo("-"));
	}
}
