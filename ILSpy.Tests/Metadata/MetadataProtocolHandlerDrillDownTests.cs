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
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Metadata;
using ICSharpCode.ILSpy.Metadata.CorTables;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Metadata;

/// <summary>
/// Drill-down half of the <c>metadata://</c> protocol handler: when the supplied handle
/// belongs to a metadata table, the handler should resolve to that specific
/// <see cref="MetadataTableTreeNode"/> rather than just landing on the per-assembly
/// <see cref="MetadataTreeNode"/>. WPF's <c>FindNodeByHandleKind</c> is the helper that
/// turns a <see cref="HandleKind"/> into the matching table tree node; this test fixture
/// pins both the helper and the protocol-handler wiring.
/// </summary>
[TestFixture]
public class MetadataProtocolHandlerDrillDownTests
{
	[AvaloniaTest]
	public async Task FindNodeByHandleKind_Returns_The_TypeDef_Table_For_A_TypeDefinition_Handle()
	{
		// Direct exercise of the helper. CoreLib is guaranteed to contain a TypeDef table —
		// the assert simply checks Kind == TableIndex.TypeDef. The HandleKind→TableIndex cast
		// is what the helper relies on; if the underlying enums ever diverge we want a loud
		// failure here rather than a silent "always returns null".
		var metadataNode = await GetMetadataTreeNodeForCoreLib();

		var result = metadataNode.FindNodeByHandleKind(HandleKind.TypeDefinition);

		((object?)result).Should().NotBeNull();
		result!.Kind.Should().Be(TableIndex.TypeDef, "the TypeDef table backs HandleKind.TypeDefinition");
	}

	[AvaloniaTest]
	public async Task FindNodeByHandleKind_Returns_Null_For_A_Heap_Handle_Kind()
	{
		// Heap handles (UserString / Blob / Guid / String) don't have a backing
		// MetadataTableTreeNode — they live as siblings under the MetadataTreeNode itself.
		// The helper must return null in that case so the caller can fall back to the
		// per-assembly Metadata node instead of drilling into a wrong table.
		var metadataNode = await GetMetadataTreeNodeForCoreLib();

		var result = metadataNode.FindNodeByHandleKind(HandleKind.UserString);

		((object?)result).Should().BeNull();
	}

	[AvaloniaTest]
	public async Task MetadataProtocolHandler_Drills_Into_TypeDef_Table_For_A_TypeDefinition_Handle()
	{
		// Full integration: hand a TypeDefinitionHandle to the protocol handler and expect
		// the resolved node to be the TypeDef MetadataTableTreeNode under the per-assembly
		// Metadata folder. Without the drill-down patch the handler would return the parent
		// MetadataTreeNode itself, which makes the user scroll-find the right table by hand.
		var (handler, module, _) = await SetUpHandler();
		var someTypeDef = module.Metadata.TypeDefinitions.First();

		var result = handler.Resolve("metadata", module, someTypeDef, out _);

		// SharpTreeNode has a custom .Should() extension in this assembly that hides BeOfType,
		// so compare exact types directly.
		((object?)result).Should().NotBeNull();
		result!.GetType().Should().Be(typeof(TypeDefTableTreeNode),
			"TypeDefinition handles must drill into the TypeDef table");
	}

	[AvaloniaTest]
	public async Task MetadataProtocolHandler_Falls_Back_To_MetadataTreeNode_For_Heap_Handles()
	{
		// String / UserString / Blob / Guid handles don't map to a per-table tree node, so
		// the handler stays at the Metadata folder level. Users can still expand to the right
		// heap from there; the alternative (returning null) would suppress navigation entirely.
		var (handler, module, _) = await SetUpHandler();

		// Construct a UserStringHandle via MetadataTokens (the public ctor is internal).
		var heapHandle = MetadataTokens.UserStringHandle(1);

		var result = handler.Resolve("metadata", module, heapHandle, out _);

		((object?)result).Should().NotBeNull();
		result!.GetType().Should().Be(typeof(MetadataTreeNode),
			"heap handles have no table match, so the handler should keep the user at the Metadata folder");
	}

	// --- helpers ---------------------------------------------------------------------------

	static async Task<MetadataTreeNode> GetMetadataTreeNodeForCoreLib()
	{
		var (_, vm) = await TestHarness.BootAsync();

		var metadataNode = vm.AssemblyTreeModel.FindCoreLib().GetChild<MetadataTreeNode>();
		// Drill-down needs the children realised; EnsureLazyChildren cascades only one level.
		var tablesNode = metadataNode.GetChild<MetadataTablesTreeNode>();
		tablesNode.EnsureLazyChildren();
		TestCapture.Step("tables-tree-expanded");
		return metadataNode;
	}

	static async Task<(MetadataProtocolHandler handler, ICSharpCode.Decompiler.Metadata.MetadataFile module, MetadataTreeNode metadataNode)> SetUpHandler()
	{
		var metadataNode = await GetMetadataTreeNodeForCoreLib();
		var vm = (MainWindowViewModel)AppComposition.Current.GetExport<MainWindow>().DataContext!;
		var assemblyNode = vm.AssemblyTreeModel.FindCoreLib();
		var module = assemblyNode.LoadedAssembly.GetMetadataFileOrNull()!;
		// The handler is exported as IProtocolHandler, not as itself — fish out the
		// MetadataProtocolHandler concrete impl from the contract list.
		var handler = AppComposition.Current
			.GetExports<ICSharpCode.ILSpy.Commands.IProtocolHandler>()
			.OfType<MetadataProtocolHandler>()
			.Single();
		return (handler, module, metadataNode);
	}
}
