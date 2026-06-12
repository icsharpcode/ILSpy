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

using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.Decompiler.Metadata;

using ICSharpCode.ILSpy.Metadata;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Metadata;

/// <summary>
/// Navigating to a metadata token (the "Go to token" / token-cell path) must push a back/forward
/// history entry, so Back returns the user to where they were before the jump.
/// </summary>
[TestFixture]
public class GoToTokenHistoryTests
{
	[AvaloniaTest]
	public async Task Navigating_To_A_Token_Records_A_History_Entry_And_Back_Returns()
	{
		var (_, vm) = await TestHarness.BootAsync();
		var ws = vm.DockWorkspace;

		// Start somewhere concrete: a decompiled type. This becomes the entry Back should return to.
		// A small CoreLib type keeps the decompile inside the headless 15s wait on slow CI runners
		// (a full System.Linq.Enumerable decompile overruns it); the token jump below lands on the
		// metadata table, a distinct node either way.
		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var startNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(coreLibName, "System", "System.Object");
		Assert.That(startNode, Is.Not.Null);
		vm.AssemblyTreeModel.SelectNode(startNode);
		await ws.WaitForDecompiledTextAsync();

		int backBefore = ws.BackHistory.Count;

		// Jump to a metadata token (a CoreLib TypeDef). This lands on the metadata TypeDef table —
		// a different node than the starting type, so it must record a new history entry.
		var coreLib = vm.AssemblyTreeModel.FindCoreLib();
		var metadataFile = coreLib.LoadedAssembly.GetMetadataFileOrNull();
		Assert.That(metadataFile, Is.Not.Null);
		ws.NavigateToToken(new MetadataTokenReference(metadataFile!, MetadataTokens.EntityHandle(0x02000002)));

		await Waiters.WaitForAsync(
			() => vm.AssemblyTreeModel.SelectedItem is MetadataTableTreeNode);

		ws.BackHistory.Count.Should().BeGreaterThan(backBefore,
			"go-to-token must record a history entry so Back returns to the previous location");
		ws.NavigateBackCommand.CanExecute(null).Should().BeTrue();

		// Back returns to the starting type.
		ws.NavigateBackCommand.Execute(null);
		await Waiters.WaitForAsync(
			() => ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, startNode));
	}
}
