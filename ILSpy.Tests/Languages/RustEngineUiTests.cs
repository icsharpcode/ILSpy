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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Headless.NUnit;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Rust.Interop;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;
using ICSharpCode.ILSpyX.TreeView;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Languages;

/// <summary>
/// Sample type decompiled by <see cref="RustEngineUiTests"/>: two constructors and a field with and
/// one without an initializer for the constructor and field selection, a call of a specialized
/// member, and a method the forced-panic test makes the engine fail on.
/// </summary>
public class RustEngineUiSample
{
	public int Initialized = 42;

	public int Plain;

	public RustEngineUiSample(int value)
	{
		Plain = value;
	}

	public RustEngineUiSample(string text)
	{
		Plain = text.Length;
	}

	public void Fill(List<int> list)
	{
		list.Add(Initialized);
	}

	public int Panics()
	{
		return Plain + 1;
	}
}

/// <summary>
/// The UI features that sit on top of the decompiler, run on the Rust engine. Test runs are strict
/// (<c>ILSPY_RUST_STRICT=1</c>), where a request the engine cannot serve becomes the per-member
/// error comment instead of a fallback, so each test asserts both the feature and the absence of
/// that comment, and that the engine's counters moved. The Debug Steps pane, which runs managed by
/// design, and body bookmarks are covered by <c>DebugStepsTests</c> and
/// <c>BookmarkDebugInfoCaptureTests</c>.
/// </summary>
[TestFixture]
public class RustEngineUiTests
{
	const string ErrorComment = "ILSpy could not decompile this member";

	static (long Seam1, long Seam2Bodies, long Seam2Requests) Counters()
		=> (RustSeam1.Served, RustSeam2.Served, RustSeam2.RequestsServed);

	static void ShouldHaveServed((long Seam1, long Seam2Bodies, long Seam2Requests) before, string text)
	{
		text.Should().NotContain(ErrorComment, "under strict mode a fallback would have become an error comment");
		RustSeam1.Served.Should().BeGreaterThan(before.Seam1, "the engine read and transformed the IL");
		RustSeam2.Served.Should().BeGreaterThan(before.Seam2Bodies, "the engine translated the bodies to C#");
		RustSeam2.RequestsServed.Should().BeGreaterThan(before.Seam2Requests, "the engine ran the AST transforms");
	}

	static async Task<(MainWindow Window, MainWindowViewModel Vm, TypeTreeNode Node)> OpenSampleAsync()
	{
		var (window, vm) = await TestHarness.BootAsync();
		var node = await vm.OpenSampleAsync(typeof(RustEngineUiSample));
		node.IsExpanded = true;
		return (window, vm, node);
	}

	static async Task<DecompilerTabPageModel> ShowAsync(MainWindowViewModel vm, SharpTreeNode node)
	{
		vm.AssemblyTreeModel.SelectNode(node);
		return await vm.DockWorkspace.WaitForDecompiledTextAsync();
	}

	static MethodTreeNode Method(TypeTreeNode type, Func<IMethod, bool> predicate)
		=> type.Children.OfType<MethodTreeNode>().Single(m => predicate(m.MethodDefinition));

	[AvaloniaTest]
	public async Task Constructor_Selection_Shows_Only_The_Selected_Constructor_And_The_Initialized_Fields()
	{
		var (_, vm, type) = await OpenSampleAsync();
		var ctor = Method(type, m => m.IsConstructor && m.Parameters is [{ Type.Name: "Int32" }]);
		var before = Counters();

		var tab = await ShowAsync(vm, ctor);

		var text = tab.Text!;
		ShouldHaveServed(before, text);
		text.Should().Contain("public RustEngineUiSample(int value)");
		text.Should().Contain("public int Initialized = 42;", "a field with an initializer belongs to every constructor");
		text.Should().NotContain("string text", "SelectCtorTransform removes the other constructor");
		text.Should().NotContain("public int Plain;", "SelectCtorTransform removes a field without an initializer");
	}

	[AvaloniaTest]
	public async Task Field_Selection_Shows_Only_The_Selected_Field()
	{
		var (_, vm, type) = await OpenSampleAsync();
		var field = type.Children.OfType<FieldTreeNode>().Single(f => f.FieldDefinition.Name == nameof(RustEngineUiSample.Initialized));
		var before = Counters();

		var tab = await ShowAsync(vm, field);

		var text = tab.Text!;
		ShouldHaveServed(before, text);
		text.Should().Contain("public int Initialized = 42;");
		text.Should().NotContain("RustEngineUiSample(", "SelectFieldTransform removes the constructors");
		text.Should().NotContain("Plain", "SelectFieldTransform removes the other field");
	}

	[AvaloniaTest]
	public async Task A_Specialized_Member_Reference_Has_Its_Tooltip_And_Navigates_To_Its_Definition()
	{
		var (window, vm, type) = await OpenSampleAsync();
		var fill = Method(type, m => m.Name == nameof(RustEngineUiSample.Fill));
		var before = Counters();

		var tab = await ShowAsync(vm, fill);
		ShouldHaveServed(before, tab.Text!);

		// The call's parentheses carry the same reference; the name is what the pointer hovers.
		var segment = tab.References!.Single(r => r.Reference is IMethod { Name: "Add" } && tab.Text!.Substring(r.StartOffset, r.Length) == "Add");
		var add = (IMethod)segment.Reference!;
		add.Should().BeOfType<SpecializedMethod>("the call targets the member of the constructed type List<int>");
		add.DeclaringType.FullName.Should().Be("System.Collections.Generic.List");
		add.DeclaringType.TypeArguments.Should().ContainSingle().Which.FullName.Should().Be("System.Int32");

		var view = window.GetVisualDescendants().OfType<DecompilerTextView>().First();
		var content = view.BuildHoverContent(tab, segment);
		content.Should().NotBeNull("the reference under the pointer has a tooltip");
		var tooltip = string.Concat(content!.Value.Control.GetLogicalDescendants()
			.OfType<TextBlock>()
			.Select(tb => tb.Inlines is { Count: > 0 }
				? string.Concat(tb.Inlines.OfType<Run>().Select(r => r.Text))
				: tb.Text));
		tooltip.Should().Contain("List<int>.Add(int", "the tooltip shows the specialized signature");

		view.OnReferenceClicked(segment);
		await Waiters.WaitForAsync(() => vm.AssemblyTreeModel.SelectedItem is MethodTreeNode { MethodDefinition.Name: "Add" },
			description: "go to definition to select List<T>.Add");
		var target = ((MethodTreeNode)vm.AssemblyTreeModel.SelectedItem!).MethodDefinition;
		// The tree has its own type system, so the definitions compare by module and token.
		target.ParentModule!.MetadataFile.Should().BeSameAs(add.MemberDefinition.ParentModule!.MetadataFile);
		target.MetadataToken.Should().Be(add.MemberDefinition.MetadataToken,
			"go to definition lands on the generic definition of the specialized member");
	}

	[AvaloniaTest]
	public async Task Portable_Pdb_Generation_Writes_Sequence_Points_From_The_Engine()
	{
		var (_, vm) = await TestHarness.BootAsync();
		var loaded = await vm.OpenFixtureAsync("PdbFixture");
		var file = (PEFile)loaded.GetMetadataFileOrNull()!;
		var settings = new DecompilerSettings();
		var before = Counters();

		using var pdb = new MemoryStream();
		// The emitted fixture has no CodeView entry to take the id from.
		new PortablePdbWriter { PdbId = new BlobContentId(Guid.NewGuid(), 1) }
			.WritePdb(file, new CSharpDecompiler(file, loaded.GetAssemblyResolver(), settings), settings, pdb);

		RustSeam1.Served.Should().BeGreaterThan(before.Seam1, "the engine read the fixture's method bodies");
		RustSeam2.Served.Should().BeGreaterThan(before.Seam2Bodies, "the engine translated the bodies to C#");
		pdb.Position = 0;
		using var provider = MetadataReaderProvider.FromPortablePdbStream(pdb);
		var reader = provider.GetMetadataReader();
		var add = file.Metadata.MethodDefinitions
			.Single(h => file.Metadata.GetString(file.Metadata.GetMethodDefinition(h).Name) == "Add");
		var points = reader.GetMethodDebugInformation(add).GetSequencePoints().Where(p => !p.IsHidden).ToList();
		points.Should().NotBeEmpty("the decompiled body of Widget.Add carries sequence points");
		reader.GetString(reader.GetDocument(points[0].Document).Name).Should().EndWith("Widget.cs");
	}

	[AvaloniaTest]
	public async Task Without_The_Assembly_References_Loaded_A_Method_Falls_Back_To_The_Managed_Pipeline()
	{
		var (_, vm) = await TestHarness.BootAsync();
		// Leaves System.Runtime, the sample's only path to a corlib, unresolved, so the known types
		// come from MinimalCorlib, which the engine does not model.
		AppComposition.Current.GetExport<SettingsService>().DecompilerSettings.AutoLoadAssemblyReferences = false;
		var type = await vm.OpenSampleAsync(typeof(DecompileSample));
		type.IsExpanded = true;
		var describe = Method(type, m => m.Name == nameof(DecompileSample.Describe));
		long fallbacks = RustSeam1.Fallbacks;
		string? strict = Environment.GetEnvironmentVariable("ILSPY_RUST_STRICT");
		Environment.SetEnvironmentVariable("ILSPY_RUST_STRICT", "0");
		string text;
		try
		{
			text = (await ShowAsync(vm, describe)).Text!;
		}
		finally
		{
			Environment.SetEnvironmentVariable("ILSPY_RUST_STRICT", strict);
		}

		RustSeam1.Fallbacks.Should().BeGreaterThan(fallbacks, "the request is routed to the managed pipeline and counted");
		text.Should().NotContain(ErrorComment);
		// With EventHandler unresolved, the managed pipeline keeps the null check explicit.
		text.Should().Contain(".Invoke((object)this, EventArgs.Empty);");
	}

	[AvaloniaTest]
	public async Task Cancelling_A_Long_Decompile_Stops_The_Engine_And_Leaves_The_View_Usable()
	{
		var (_, vm) = await TestHarness.BootAsync(3);
		var enumerable = vm.AssemblyTreeModel.FindNode<TypeTreeNode>("System.Linq", "System.Linq", "System.Linq.Enumerable");
		int methodCount = enumerable.TypeDefinition.Methods.Count();
		long start = RustSeam1.Served;

		vm.AssemblyTreeModel.SelectNode(enumerable);
		await Waiters.WaitForAsync(() => RustSeam1.Served - start >= 5, description: "the engine to serve the first members");
		var tab = vm.DockWorkspace.ActiveDecompilerTab!;
		tab.IsDecompiling.Should().BeTrue("Enumerable is long enough to still be decompiling");
		tab.CancelDecompilationCommand.Execute(null);
		// IsDecompiling drops in the run's finally, after its background work has returned.
		await Waiters.WaitForAsync(() => !tab.IsDecompiling, description: "the cancelled decompile to unwind");

		long served = RustSeam1.Served - start;
		served.Should().BeLessThan(methodCount, "the cancelled decompile stopped before the end of the type");
		tab.IsDecompiling.Should().BeFalse();
		(tab.Text ?? "").Should().NotContain("class Enumerable", "a cancelled decompile does not show its partial output");

		var (_, after) = await vm.ShowSampleAsync(typeof(DecompileSample));
		after.Text.Should().Contain("class DecompileSample").And.NotContain(ErrorComment);
	}

	[AvaloniaTest]
	public async Task A_Forced_Engine_Panic_Becomes_The_Error_Comment_And_The_View_Stays_Usable()
	{
		var (_, vm, type) = await OpenSampleAsync();
		var panics = Method(type, m => m.Name == nameof(RustEngineUiSample.Panics));
		uint token = (uint)MetadataTokens.GetToken(panics.MethodDefinition.MetadataToken);
		if (NativeMethods.ilspy_debug_panic_on_read(token) == 0)
			Assert.Ignore("the engine was built without debug assertions, which have no panic hook");
		string text;
		try
		{
			text = (await ShowAsync(vm, panics)).Text!;
		}
		finally
		{
			NativeMethods.ilspy_debug_panic_on_read(0);
		}

		text.Should().Contain(ErrorComment, "the panic surfaces through the per-member error comment");
		text.Should().Contain("forced panic", "the comment carries the engine's panic message");
		text.Should().Contain("int Panics()", "the member keeps its signature");

		var fill = Method(type, m => m.Name == nameof(RustEngineUiSample.Fill));
		var after = await ShowAsync(vm, fill);
		after.Text.Should().Contain("list.Add(Initialized);").And.NotContain(ErrorComment);
	}
}
