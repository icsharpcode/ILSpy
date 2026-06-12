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

using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Headless.NUnit;
using Avalonia.LogicalTree;

using AwesomeAssertions;

using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Documentation;
using ICSharpCode.Decompiler.TypeSystem;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.TextView;

/// <summary>
/// Pins the IL-specific halves of the decompiler-view hover tooltip. In the IL language
/// view, hovering a member reference must show the member's disassembled IL header
/// (".method ...", ".class ...", ".field ..."), not a C#-style one-liner — that is what
/// <see cref="ILLanguage.GetRichTextTooltip"/> produces. Hovering an opcode must show the
/// opcode's XML documentation, which comes from
/// <see cref="XmlDocLoader.MscorlibDocumentation"/> via the
/// "F:System.Reflection.Emit.OpCodes.*" doc ids.
/// </summary>
[TestFixture]
public class ILLanguageTooltipTests
{
	static async Task<TypeTreeNode> LoadCoreLibStringNodeAsync()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);

		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var stringNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(coreLibName, "System", "System.String");
		stringNode.IsExpanded = true;
		return stringNode;
	}

	static ILLanguage GetILLanguage()
		=> (ILLanguage)AppComposition.Current.GetExport<LanguageService>().Languages.Single(l => l.Name == "IL");

	[AvaloniaTest]
	public async Task Method_Tooltip_Is_The_Disassembled_IL_Header()
	{
		var stringNode = await LoadCoreLibStringNodeAsync();
		var concat = stringNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Concat" && m.MethodDefinition.Parameters.Count == 2)
			.MethodDefinition;

		var tooltip = GetILLanguage().GetRichTextTooltip(concat);

		tooltip.Should().NotBeNull();
		tooltip!.Text.Should().StartWith(".method",
			"the IL hover tooltip must show the disassembled method header, not an ambience one-liner");
		tooltip.Text.Should().Contain("Concat");
		tooltip.Text.Should().NotContain("\n",
			"the header must be collapsed to a single line for the tooltip");
	}

	[AvaloniaTest]
	public async Task Type_Tooltip_Is_The_Disassembled_IL_Header()
	{
		var stringNode = await LoadCoreLibStringNodeAsync();
		var stringType = (ITypeDefinition)stringNode.Member!;

		var tooltip = GetILLanguage().GetRichTextTooltip(stringType);

		tooltip.Should().NotBeNull();
		tooltip!.Text.Should().StartWith(".class");
		tooltip.Text.Should().Contain("String");
		tooltip.Text.Should().NotContain("\n");
	}

	[AvaloniaTest]
	public async Task Field_Tooltip_Is_The_Disassembled_IL_Header()
	{
		var stringNode = await LoadCoreLibStringNodeAsync();
		var empty = stringNode.Children.OfType<FieldTreeNode>()
			.First(f => f.FieldDefinition.Name == "Empty")
			.FieldDefinition;

		var tooltip = GetILLanguage().GetRichTextTooltip(empty);

		tooltip.Should().NotBeNull();
		tooltip!.Text.Should().StartWith(".field");
		tooltip.Text.Should().Contain("Empty");
		tooltip.Text.Should().NotContain("\n");
	}

	[AvaloniaTest]
	public async Task OpCode_Hover_Builds_Rich_Content_With_Documentation()
	{
		var stringNode = await LoadCoreLibStringNodeAsync();
		var vm = (MainWindowViewModel)AppComposition.Current.GetExport<MainWindow>().DataContext!;
		vm.AssemblyTreeModel.SelectNode(stringNode);
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		var view = new DecompilerTextView { DataContext = tab };
		var segment = new ReferenceSegment {
			Reference = new OpCodeInfo(System.Reflection.Metadata.ILOpCode.Ldloc_0, "ldloc.0"),
		};

		var content = view.BuildHoverContent(tab, segment);

		content.Should().NotBeNull("hovering an opcode must produce a tooltip");
		content!.Value.IsRich.Should().BeTrue(
			"with opcode documentation available, the tooltip must be the rich documentation popup");
		var text = string.Concat(content.Value.Control.GetLogicalDescendants()
			.OfType<TextBlock>()
			.Select(tb => tb.Inlines is { Count: > 0 }
				? string.Concat(tb.Inlines.OfType<Run>().Select(r => r.Text))
				: tb.Text));
		text.Should().Contain("ldloc.0", "the signature line shows the opcode name");
		text.Should().Contain("local variable", "the OpCodes.Ldloc_0 summary must be rendered");
	}

	[AvaloniaTest]
	public async Task EntityReference_Hover_Resolves_To_A_Rich_Tooltip()
	{
		// IL-view member references arrive as unresolved EntityReferences, not IEntity —
		// this covers the resolve half of the hover chain (BuildHoverContent ->
		// ResolveEntity -> EntityReference.Resolve via the tab's current node).
		var stringNode = await LoadCoreLibStringNodeAsync();
		var vm = (MainWindowViewModel)AppComposition.Current.GetExport<MainWindow>().DataContext!;
		vm.AssemblyTreeModel.SelectNode(stringNode);
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		var concat = stringNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Concat" && m.MethodDefinition.Parameters.Count == 2)
			.MethodDefinition;
		var view = new DecompilerTextView { DataContext = tab };
		var segment = new ReferenceSegment {
			Reference = new EntityReference(concat.ParentModule!.MetadataFile!, concat.MetadataToken),
		};

		var content = view.BuildHoverContent(tab, segment);

		content.Should().NotBeNull("a member reference under the pointer must resolve to a tooltip");
		content!.Value.IsRich.Should().BeTrue("entity tooltips render via the documentation popup");
	}

	[AvaloniaTest]
	public void MscorlibDocumentation_Surfaces_OpCode_Documentation()
	{
		// On modern .NET there are no .NET Framework reference-assembly docs; the loader
		// must fall back to the runtime's parallel ref pack (same layout assumption as
		// XmlDocumentationTests for per-module docs).
		var provider = XmlDocLoader.MscorlibDocumentation;
		((object?)provider).Should().NotBeNull(
			"opcode hover tooltips have no documentation source without MscorlibDocumentation");

		var documentation = provider!.GetDocumentation("F:System.Reflection.Emit.OpCodes.Ldloc_0");
		documentation.Should().NotBeNullOrEmpty(
			"the IL view opcode tooltip looks docs up by the OpCodes field doc id");
		documentation.Should().Contain("<summary");
	}
}
