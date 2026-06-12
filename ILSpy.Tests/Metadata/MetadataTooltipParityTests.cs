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
using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Metadata;
using ICSharpCode.ILSpy.Metadata.CorTables;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Metadata;

/// <summary>
/// End-to-end check that the per-cell metadata tooltips restored across the table entries actually
/// resolve against a real assembly: a heap-offset string tooltip, a token tooltip routed through
/// <c>GenerateTooltip</c>, and a rich per-bit <see cref="FlagsTooltip"/> rendered as a control.
/// </summary>
[TestFixture]
public class MetadataTooltipParityTests
{
	[AvaloniaTest]
	public async Task TypeDef_Row_Tooltips_Resolve_For_System_Object()
	{
		var (_, vm) = await TestHarness.BootAsync();

		var typeDefNode = vm.AssemblyTreeModel.FindCoreLib()
			.GetChild<MetadataTreeNode>()
			.GetChild<MetadataTablesTreeNode>()
			.GetChild<TypeDefTableTreeNode>();

		vm.AssemblyTreeModel.SelectNode(typeDefNode);
		var tab = await vm.DockWorkspace.WaitForMetadataTabAsync();

		var objectRow = tab.Items.Cast<TypeDefTableTreeNode.TypeDefEntry>()
			.Single(e => e.Name == "Object" && e.Namespace == "System");

		// Heap-offset string tooltip on the Name column.
		MetadataCellTooltip.Resolve(objectRow, "Name").Should().BeOfType<string>()
			.Which.Should().Contain("Object");

		// Token tooltip routed through GenerateTooltip (System.Object's first method).
		MetadataCellTooltip.Resolve(objectRow, "MethodList").Should().BeOfType<string>()
			.Which.Should().NotBeNullOrWhiteSpace();

		// Rich per-bit flags breakdown renders as a control, not a plain string.
		MetadataCellTooltip.Resolve(objectRow, "Attributes").Should().BeAssignableTo<Control>();
	}
}
