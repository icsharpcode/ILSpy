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
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.Decompiler.TypeSystem;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Metadata;
using ICSharpCode.ILSpy.TextView;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Metadata;

[TestFixture]
public class ShowInMetadataContextMenuTests
{
	[AvaloniaTest]
	public async Task Right_Click_On_An_Entity_Reference_In_The_Decompiler_Routes_To_Its_Metadata_Row()
	{
		// End-to-end: open a CoreLib type so the decompiler view fills with entity
		// hyperlinks; synthesize a TextViewContext pointing at one of those references;
		// verify the entry's IsVisible/Execute drives the dock workspace into the right
		// metadata table for the entity's MetadataToken.kind.

		var (_, vm) = await TestHarness.BootAsync();

		var assemblyNode = vm.AssemblyTreeModel.FindCoreLib();

		// Resolve System.Object's IEntity via the type system, since the entry only acts
		// on IEntity-bearing references.
		await assemblyNode.LoadedAssembly.GetLoadResultAsync();
		var ts = assemblyNode.LoadedAssembly.GetTypeSystemOrNull()!;
		var entity = ts.MainModule.TypeDefinitions.Single(t => t.FullName == "System.Object");
		entity.MetadataToken.IsNil.Should().BeFalse(
			"System.Object must have a real metadata token in CoreLib");

		var ctx = new TextViewContext {
			TextView = new DecompilerTextView(),
			Reference = new ReferenceSegment { Reference = entity },
		};
		var entry = new ShowInMetadataContextMenuEntry();
		entry.IsVisible(ctx).Should().BeTrue(
			"the entry must surface for entity references in the decompiler view");

		entry.Execute(ctx);

		var expectedTable = (TableIndex)(int)entity.MetadataToken.Kind;
		await Waiters.WaitForAsync(
			() => vm.AssemblyTreeModel.SelectedItem is MetadataTableTreeNode m
				&& m.Kind == expectedTable);
		TestCapture.Step("show-in-metadata-table");
		((MetadataTableTreeNode)vm.AssemblyTreeModel.SelectedItem!).Kind.Should().Be(expectedTable);
	}

	[AvaloniaTest]
	public void IsVisible_False_When_There_Is_No_TextView_Or_Entity_Reference()
	{
		// The entry is decompiler-only. A right-click in a context that has no TextView,
		// or whose Reference is null / not an IEntity (e.g. an opcode link), must not
		// surface the entry — otherwise it would fire an unrelated NavigateToToken.
		var entry = new ShowInMetadataContextMenuEntry();

		entry.IsVisible(new TextViewContext()).Should().BeFalse();
		entry.IsVisible(new TextViewContext { TextView = new DecompilerTextView() })
			.Should().BeFalse("a TextView without a Reference is just empty whitespace");
		entry.IsVisible(new TextViewContext {
			TextView = new DecompilerTextView(),
			Reference = new ReferenceSegment { Reference = "not-an-entity" },
		}).Should().BeFalse();
	}
}
