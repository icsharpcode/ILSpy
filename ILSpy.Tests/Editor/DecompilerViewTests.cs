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

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class DecompilerViewTests
{
	[AvaloniaTest]
	public async Task Selecting_AsEnumerable_Method_Decompiles_Into_Document_View()
	{
		// Selecting a MethodTreeNode in the assembly tree must trigger a decompile and the
		// resulting C# (signature + body) must land in the document tab. Also verifies the
		// row scrolls into view (CenteredInView assertion) so the user sees the selection.

		// Arrange — boot, expand Enumerable, locate AsEnumerable.
		var (_, vm) = await TestHarness.BootAsync(3);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable").Expand();
		var methodNode = typeNode.GetChild<MethodTreeNode>(m => m.MethodDefinition.Name == "AsEnumerable");
		methodNode.MethodDefinition.IsExtensionMethod.Should().BeTrue();

		// Act — select the method node and wait for the decompile.
		vm.AssemblyTreeModel.SelectNode(methodNode);
		TestCapture.Step("selected-asenumerable");
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		TestCapture.Step("asenumerable-decompiled");

		// Assert — signature + body fragments are present and the row centred in the tree.
		tab.Text.Should().Contain("AsEnumerable");
		tab.Text.Should().Contain("IEnumerable<TSource>");
		tab.Text.Should().Contain("return source");

		methodNode.Should().Be().CenteredInView();
	}

	[AvaloniaTest]
	public async Task Selecting_References_Folder_Decompiles_References_Listing()
	{
		// The References folder under each assembly is a synthetic node; selecting it emits a
		// summary listing (target framework, runtime pack, referenced assemblies, load log)
		// rather than decompiled C#.

		// Arrange — boot, expand the System.Linq assembly node, grab its References folder.
		var (_, vm) = await TestHarness.BootAsync(3);

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq").Expand();
		TestCapture.Step("expanded-system-linq");
		var refFolder = assemblyNode.GetChild<ReferenceFolderTreeNode>();

		// Act — select the References folder and wait for its decompile output.
		vm.AssemblyTreeModel.SelectNode(refFolder);
		TestCapture.Step("selected-references");
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		TestCapture.Step("references-listing");

		// Assert — every header and a representative referenced assembly land in the listing.
		tab.Text.Should().Contain("Detected TargetFramework-Id:");
		tab.Text.Should().Contain("Detected RuntimePack:");
		tab.Text.Should().Contain("Referenced assemblies (in metadata order):");
		tab.Text.Should().Contain("System.Runtime");
		tab.Text.Should().Contain("Assembly load log including transitive references:");
	}

	[AvaloniaTest]
	public async Task Selecting_Type_Node_Decompiles_Type_Definition()
	{
		// Selecting a TypeTreeNode (System.Version) must decompile the full class declaration
		// including representative members.

		// Arrange — boot, find System.Version.
		var (_, vm) = await TestHarness.BootAsync(3);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			TreeNavigation.CoreLibName, "System", "System.Version");

		// Act — select the type node, wait for the decompile.
		vm.AssemblyTreeModel.SelectNode(typeNode);
		TestCapture.Step("selected-version");
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		TestCapture.Step("version-class");

		// Assert — class declaration + representative property names land in the output.
		tab.Text.Should().Contain("class Version");
		tab.Text.Should().Contain("Major");
		tab.Text.Should().Contain("Minor");
	}

	[AvaloniaTest]
	public async Task Selecting_Property_Node_Decompiles_Property()
	{
		// PropertyTreeNode → just the single property's declaration (with type prefix), not
		// the full enclosing class.

		// Arrange — boot, expand System.Version, grab Major.
		var (_, vm) = await TestHarness.BootAsync(3);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			TreeNavigation.CoreLibName, "System", "System.Version").Expand();
		var propertyNode = typeNode.GetChild<PropertyTreeNode>(p => p.PropertyDefinition.Name == "Major");

		// Act — select the property, wait for the decompile.
		vm.AssemblyTreeModel.SelectNode(propertyNode);
		TestCapture.Step("selected-major");
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		TestCapture.Step("major-property");

		// Assert — property declaration lands in the output.
		tab.Text.Should().Contain("public int Major");
	}

	[AvaloniaTest]
	public async Task Selecting_Field_Node_Decompiles_Field()
	{
		// FieldTreeNode (System.Math.PI) → decompiled field declaration with its constant
		// initializer.

		// Arrange — boot, expand System.Math, grab PI.
		var (_, vm) = await TestHarness.BootAsync(3);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			TreeNavigation.CoreLibName, "System", "System.Math").Expand();
		var fieldNode = typeNode.GetChild<FieldTreeNode>(f => f.FieldDefinition.Name == "PI");

		// Act — select the field, wait for the decompile.
		vm.AssemblyTreeModel.SelectNode(fieldNode);
		TestCapture.Step("selected-pi");
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		TestCapture.Step("pi-field");

		// Assert — declaration and constant value both land.
		tab.Text.Should().Contain("public const double PI");
		tab.Text.Should().Contain("3.14159");
	}

	[AvaloniaTest]
	public async Task Selecting_Event_Node_Decompiles_Event()
	{
		// EventTreeNode (AppDomain.ProcessExit) → decompiled event declaration.

		// Arrange — boot, expand AppDomain, grab ProcessExit.
		var (_, vm) = await TestHarness.BootAsync(3);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			TreeNavigation.CoreLibName, "System", "System.AppDomain").Expand();
		var eventNode = typeNode.GetChild<EventTreeNode>(e => e.EventDefinition.Name == "ProcessExit");

		// Act — select the event, wait for the decompile.
		vm.AssemblyTreeModel.SelectNode(eventNode);
		TestCapture.Step("selected-processexit");
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		TestCapture.Step("processexit-event");

		// Assert — event keyword + name land in the output.
		tab.Text.Should().Contain("event EventHandler");
		tab.Text.Should().Contain("ProcessExit");
	}

	[AvaloniaTest]
	public async Task Selecting_Namespace_Node_In_IL_Lists_Its_Types()
	{
		// Under the IL disassembler, selecting a NamespaceTreeNode lists the namespace's
		// types — ILLanguage overrides DecompileNamespace to disassemble them, matching the
		// app's IL mode that disassembles the whole namespace.

		// Arrange — boot, switch to IL, locate System.Runtime.Versioning.
		var (_, vm) = await TestHarness.BootAsync(3);

		var languageService = AppComposition.Current.GetExport<LanguageService>();
		languageService.CurrentLanguage = languageService.GetLanguage("IL");
		TestCapture.Step("select-il-language");

		var namespaceNode = vm.AssemblyTreeModel.FindNode<NamespaceTreeNode>(
			TreeNavigation.CoreLibName, "System.Runtime.Versioning");

		// Act — select the namespace, wait for the decompile.
		vm.AssemblyTreeModel.SelectNode(namespaceNode);
		TestCapture.Step("selected-namespace");
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		TestCapture.Step("namespace-il-listing");

		// Assert — the document was produced by the IL language, and representative type
		// names land in the IL output.
		tab.Language.Name.Should().Be("IL");
		tab.Text.Should().Contain("TargetFrameworkAttribute");
		tab.Text.Should().Contain("SupportedOSPlatformAttribute");
	}

	[AvaloniaTest]
	public async Task Selecting_Namespace_Node_In_CSharp_Shows_Only_The_Namespace_Comment()
	{
		// The C# language does NOT override DecompileNamespace, so selecting a namespace node
		// falls through to the base Language.DecompileNamespace, which writes just a
		// "// <namespace>" comment — not the contained types. Keeps a single click from
		// triggering a multi-second decompile of the whole namespace.

		// Arrange — boot, ensure C#, locate System.Runtime.Versioning.
		var (_, vm) = await TestHarness.BootAsync(3);

		var languageService = AppComposition.Current.GetExport<LanguageService>();
		languageService.CurrentLanguage = languageService.GetLanguage("C#");
		TestCapture.Step("select-csharp-language");

		var namespaceNode = vm.AssemblyTreeModel.FindNode<NamespaceTreeNode>(
			TreeNavigation.CoreLibName, "System.Runtime.Versioning");

		// Act — select the namespace, wait for the decompile.
		vm.AssemblyTreeModel.SelectNode(namespaceNode);
		TestCapture.Step("selected-namespace");
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		TestCapture.Step("namespace-csharp-comment");

		// Assert — only the namespace comment, none of the contained types.
		tab.Text.Should().Contain("System.Runtime.Versioning");
		tab.Text.Should().NotContain("TargetFrameworkAttribute");
		tab.Text.Should().NotContain("SupportedOSPlatformAttribute");
	}

	[AvaloniaTest]
	public async Task Decompiler_Tab_Title_Tracks_Tree_Node_Text_When_Node_Loads_Late()
	{
		// Opening a fresh assembly via OpenCommand selects it before LoadedAssembly.Text has
		// the full "(version, tfm)" suffix wired up. The tab title used to lock in the bare
		// ShortName at that moment; it must instead update once Text becomes available.

		// Arrange — boot.
		var (_, vm) = await TestHarness.BootAsync(3);

		// Act 1 — fire OpenCommand on a new path. OpenCommand selects the new node before its
		// rich Text (with version + tfm) settles, exercising the late-update path. We can't use
		// OpenAssemblyAsync here: it awaits the load, which would skip the pre-settle window.
		var newAsmPath = typeof(System.Net.Http.HttpClient).Assembly.Location;
		var openCommand = AppComposition.Current.GetExport<MainMenuCommandRegistry>()
			.GetCommand(nameof(Resources._Open));
		openCommand.Execute(newAsmPath);

		await Waiters.WaitForAsync(() =>
			vm.AssemblyTreeModel.SelectedItem is AssemblyTreeNode n
				&& string.Equals(n.LoadedAssembly.FileName, newAsmPath, System.StringComparison.OrdinalIgnoreCase));
		TestCapture.Step("assembly-selected");

		// Act 2 — wait for the rich form of LoadedAssembly.Text to materialise.
		var node = (AssemblyTreeNode)vm.AssemblyTreeModel.SelectedItem!;
		await node.LoadedAssembly.GetLoadResultAsync();
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		TestCapture.Step("opened-assembly");

		// Assert — once Text is rich, the tab title catches up. Sanity-check the form is rich
		// (otherwise the test isn't exercising the late-update path).
		await Waiters.WaitForAsync(() => string.Equals(tab.Title, node.Text?.ToString(), System.StringComparison.Ordinal));
		node.Text!.ToString().Should().NotBe(node.LoadedAssembly.ShortName,
			"the rich form (with version + tfm) must be available, otherwise the test isn't exercising the late-update path");
		tab.Title.Should().Be(node.Text!.ToString());
	}

	[AvaloniaTest]
	public async Task Decompile_Tab_Title_Cycles_Round_Spinner_While_Decompiling()
	{
		// While a decompile is in flight the tab title should prefix with one of ten Braille
		// spinner glyphs so the user sees activity. The editor's Text must NOT receive the
		// glyph (only the title bar). Once decompilation finishes, the glyph clears.

		// Arrange — boot, run an initial decompile so the second selection produces a real
		// "in flight" period (not the cold-start one).
		var (_, vm) = await TestHarness.BootAsync(3);

		var firstNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		vm.AssemblyTreeModel.SelectNode(firstNode);
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		TestCapture.Step("first-decompile");

		var titles = new List<string>();
		var editorTexts = new List<string>();
		tab.PropertyChanged += OnTabPropertyChanged;

		// Act — kick off a second decompile and observe Title/Text changes throughout. No
		// breakpoint inside the observation window: a capture's RunJobs could perturb the
		// in-flight title/text transitions this test is asserting on.
		var second = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			TreeNavigation.CoreLibName, "System", "System.Version");
		vm.AssemblyTreeModel.SelectNode(second);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();

		tab.PropertyChanged -= OnTabPropertyChanged;
		TestCapture.Step("after-second-decompile");

		// Assert — at least one captured title contained a spinner glyph; no editor Text ever
		// did; the final title is glyph-free.
		var glyphs = new[] { '⠋', '⠙', '⠹', '⠸', '⠼', '⠴', '⠦', '⠧', '⠇', '⠏' };
		titles.Should().Contain(t => glyphs.Any(t.Contains),
			"the tab title must show one of the Braille spinner frames while decompilation is in flight");
		editorTexts.Should().NotContain(t => glyphs.Any(t.Contains),
			"the editor's Text must not be overwritten with the spinner — title prefix only");
		tab.Title.Should().NotContainAny("⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏",
			"the spinner glyph must clear once decompilation finishes");

		void OnTabPropertyChanged(object? s, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(DecompilerTabPageModel.Title))
				titles.Add(tab.Title ?? string.Empty);
			else if (e.PropertyName == nameof(DecompilerTabPageModel.Text))
				editorTexts.Add(tab.Text ?? string.Empty);
		}
	}

	[AvaloniaTest]
	public async Task Multi_Selection_Tab_Title_Joins_All_Selected_Names_With_Comma()
	{
		// Selecting several nodes via Ctrl+Click feeds them all into one decompile. The tab
		// title must list every selected node's display text comma-joined.

		// Arrange — boot, expand Enumerable, grab two methods.
		var (_, vm) = await TestHarness.BootAsync(3);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable").Expand();
		var asEnumerable = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "AsEnumerable");
		var empty = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Empty");

		// Act — replace the selection with both nodes.
		vm.AssemblyTreeModel.SelectedItems.Clear();
		vm.AssemblyTreeModel.SelectedItems.Add(asEnumerable);
		vm.AssemblyTreeModel.SelectedItems.Add(empty);
		TestCapture.Step("multi-selected");

		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		TestCapture.Step("multi-selection-title");

		// Assert — title is the two display texts joined with ", ".
		tab.Title.Should().Be($"{asEnumerable.Text}, {empty.Text}");
	}

	[AvaloniaTest]
	public async Task Multi_Selecting_Methods_Decompiles_All_Of_Them_Into_One_View()
	{
		// Multi-selection isn't just a title concern — both methods' bodies must actually be
		// decompiled into one combined document.

		// Arrange — boot, expand Enumerable, grab two methods.
		var (_, vm) = await TestHarness.BootAsync(3);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable").Expand();
		var asEnumerable = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "AsEnumerable");
		var empty = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Empty");

		// Act — multi-select the two methods.
		vm.AssemblyTreeModel.SelectedItems.Clear();
		vm.AssemblyTreeModel.SelectedItems.Add(asEnumerable);
		vm.AssemblyTreeModel.SelectedItems.Add(empty);
		TestCapture.Step("multi-selected");

		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		TestCapture.Step("multi-selection-decompiled");

		// Assert — both names AND a body-fragment from one of them confirm both bodies
		// actually rendered (not just signatures).
		tab.Text.Should().Contain("AsEnumerable");
		tab.Text.Should().Contain("Empty");
		tab.Text.Should().Contain("return source");
	}

	[AvaloniaTest]
	public async Task Selecting_Assembly_Node_Emits_Header_And_Assembly_Attributes()
	{
		// AssemblyTreeNode → emit a "manifest"-style summary header (file, identity, runtime,
		// architecture, global type) plus the assembly-level attributes block.

		// Arrange — boot, find System.Linq.
		var (_, vm) = await TestHarness.BootAsync(3);

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");

		// Act — select the assembly node, wait for the decompile.
		vm.AssemblyTreeModel.SelectNode(assemblyNode);
		TestCapture.Step("selected-assembly");
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		TestCapture.Step("assembly-manifest");

		// Assert — every documented header line and at least one assembly attribute land.
		tab.Text.Should().Contain("// " + assemblyNode.LoadedAssembly.FileName);
		tab.Text.Should().Contain("// System.Linq, Version=");
		tab.Text.Should().Contain("// Global type: <Module>");
		tab.Text.Should().Contain("// Architecture: ");
		tab.Text.Should().Contain("// Runtime: ");
		tab.Text.Should().Contain("[assembly: AssemblyVersion(");
	}

	[AvaloniaTest]
	public async Task Xml_Resource_Installs_FoldingManager_With_Folds()
	{
		// XmlResourceEntryNode bumps the editor into XML mode (SyntaxExtension=".xml"), and
		// DecompilerTextView.ApplyDocument must wire AvaloniaEdit's XmlFoldingStrategy so
		// multi-line elements become collapsible.

		// Arrange — boot, seed the decompiler tab with an assembly view so the document/editor
		// wiring is realised.
		var (_, vm) = await TestHarness.BootAsync(3);

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		vm.AssemblyTreeModel.SelectNode(assemblyNode);
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		// The dock's deferred content presenter doesn't materialise its templated children
		// in Avalonia.Headless mode, so we construct the view explicitly from the live
		// viewmodel — same shape the live app's DataTemplate produces.
		var view = new DecompilerTextView { DataContext = tab };
		var host = new global::Avalonia.Controls.Window { Content = view, Width = 600, Height = 400 };
		host.Show();
		host.Capture("assembly-in-host");

		// Act — drop a synthetic XML resource into the tab. XmlResourceEntryNode triggers
		// SyntaxExtension=".xml" and the XmlFoldingStrategy install.
		var xml = "<root>\n  <child attr=\"v\">\n    text\n  </child>\n  <other>\n  </other>\n</root>";
		var bytes = System.Text.Encoding.UTF8.GetBytes(xml);
		tab.CurrentNode = new XmlResourceEntryNode("test.xml", () => new System.IO.MemoryStream(bytes));
		await Waiters.WaitForAsync(() => tab.SyntaxExtension == ".xml" && tab.Text.Contains("<root>"));

		// Drain the layout so ApplyDocument's PropertyChanged handler has executed.
		for (int i = 0; i < 5; i++)
		{
			global::Avalonia.Threading.Dispatcher.UIThread.RunJobs();
			await Task.Delay(20);
		}
		host.Capture("xml-folding");

		// Assert — FoldingManager is installed (private field, reflected) and produced fold
		// ranges for the multi-line elements.
		var fmField = typeof(DecompilerTextView).GetField("activeFoldingManager",
			System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		fmField.Should().NotBeNull();
		var fm = fmField!.GetValue(view) as global::AvaloniaEdit.Folding.FoldingManager;
		fm.Should().NotBeNull("XML SyntaxExtension should install AvaloniaEdit's XmlFoldingStrategy");
		fm!.AllFoldings.Should().NotBeEmpty("multi-line XML elements should produce fold ranges");
	}

	[AvaloniaTest]
	public async Task Expand_Display_Settings_Reach_The_Decompiler_Foldings()
	{
		// The Display-options "Expand using declarations / member definitions after decompilation"
		// flags only take effect by riding DecompilerSettings into the decompile: TextTokenWriter
		// sets each fold's DefaultClosed from settings.ExpandUsingDeclarations /
		// ExpandMemberDefinitions. So the live decompile must bridge them in from DisplaySettings;
		// without the bridge they default false and every fold comes back collapsed.
		var (_, vm) = await TestHarness.BootAsync(3);

		var settings = AppComposition.Current.GetExport<ICSharpCode.ILSpy.SettingsService>();
		settings.DisplaySettings.ExpandMemberDefinitions = true;
		settings.DisplaySettings.ExpandUsingDeclarations = true;

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			TreeNavigation.CoreLibName, "System", "System.Version");
		vm.AssemblyTreeModel.SelectNode(typeNode);
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		tab.Foldings.Should().NotBeNull();
		var foldings = tab.Foldings!;
		foldings.Should().NotBeEmpty("decompiling a type must produce foldings");

		var memberFolds = foldings.Where(f => f.IsDefinition).ToList();
		memberFolds.Should().NotBeEmpty("a type with members must produce member-definition foldings");
		memberFolds.Should().OnlyContain(f => !f.DefaultClosed,
			"ExpandMemberDefinitions=true must leave member-definition foldings expanded (DefaultClosed=false)");

		// The top-most fold is the using-declarations block (offset 0, not a member definition).
		var usingFold = foldings.OrderBy(f => f.StartOffset).First();
		usingFold.IsDefinition.Should().BeFalse("the top-most fold is the using block, not a member");
		usingFold.DefaultClosed.Should().BeFalse(
			"ExpandUsingDeclarations=true must leave the using block expanded");
	}

#if DEBUG
	[AvaloniaTest]
	public async Task Switching_Language_With_Selected_Method_Does_Not_Throw_Folding_Out_Of_Bounds()
	{
		// Regression for the unobserved-task AggregateException("Folding must be within document
		// boundary") that fired when the user switched from C# to ILAst while a method was
		// selected. Root cause: DockWorkspace.OnLanguagePropertyChanged toggles
		// CurrentNode = null → CurrentNode = node to force a re-decompile, and the null step
		// short-circuits DecompileAsync to Text = "" while leaving Foldings holding the previous
		// (C#) offsets. ApplyDocument then tried to install C# foldings into an empty document.
		//
		// We trap any unobserved Task exception during the test and fail with its full message
		// so the regression surfaces clearly instead of via the finalizer thread.

		System.Exception? unobserved = null;
		System.EventHandler<System.Threading.Tasks.UnobservedTaskExceptionEventArgs> handler = (_, e) => {
			unobserved ??= e.Exception;
			e.SetObserved();
		};
		System.Threading.Tasks.TaskScheduler.UnobservedTaskException += handler;
		try
		{
			var (_, vm) = await TestHarness.BootAsync(3);

			var method = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
				"System.Linq", "System.Linq", "System.Linq.Enumerable")
				.Expand().GetChild<MethodTreeNode>(m => m.MethodDefinition.Name == "AsEnumerable");
			vm.AssemblyTreeModel.SelectNode(method);
			var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
			tab.Foldings.Should().NotBeNull(
				"C# decompile of a method body should produce at least one fold");
			TestCapture.Step("csharp-method");

			// Switch the language — the buggy path was here.
			var languageService = AppComposition.Current.GetExport<LanguageService>();
			var blockIL = languageService.Languages.OfType<ILAstLanguage>()
				.Single(l => l.Name == "ILAst");
			languageService.CurrentLanguage = blockIL;

			await vm.DockWorkspace.WaitForDecompiledTextAsync();
			TestCapture.Step("ilast-method");

			// Force GC to flush any unobserved Task faults that escaped via the dispatcher.
			System.GC.Collect();
			System.GC.WaitForPendingFinalizers();
			System.GC.Collect();

			unobserved.Should().BeNull(
				$"C# → ILAst language switch must not leak an unobserved task fault; saw: {unobserved}");
		}
		finally
		{
			System.Threading.Tasks.TaskScheduler.UnobservedTaskException -= handler;
		}
	}
#endif
}
