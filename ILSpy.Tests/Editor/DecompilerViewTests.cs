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

using ILSpy.AppEnv;
using ILSpy.Commands;
using ILSpy.TextView;
using ILSpy.TreeNodes;
using ILSpy.ViewModels;
using ILSpy.Views;

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

		// Arrange — boot the window, wait for assemblies, expand Enumerable, locate AsEnumerable.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.IsExpanded = true;
		var methodNode = typeNode.Children.OfType<MethodTreeNode>()
			.Single(m => m.MethodDefinition.Name == "AsEnumerable");
		methodNode.MethodDefinition.IsExtensionMethod.Should().BeTrue();

		// Act — select the method node and wait for the decompile.
		vm.AssemblyTreeModel.SelectNode(methodNode);
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

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

		// Arrange — boot, wait for assemblies, expand the System.Linq assembly node.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		assemblyNode.IsExpanded = true;
		var refFolder = assemblyNode.Children.OfType<ReferenceFolderTreeNode>().Single();

		// Act — select the References folder and wait for its decompile output.
		vm.AssemblyTreeModel.SelectNode(refFolder);
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

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

		// Arrange — boot, wait for assemblies, find System.Version.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var coreLib = typeof(object).Assembly.GetName().Name!;
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			coreLib, "System", "System.Version");

		// Act — select the type node, wait for the decompile.
		vm.AssemblyTreeModel.SelectNode(typeNode);
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

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

		// Arrange — boot, wait for assemblies, expand System.Version, grab Major.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var coreLib = typeof(object).Assembly.GetName().Name!;
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			coreLib, "System", "System.Version");
		typeNode.IsExpanded = true;
		var propertyNode = typeNode.Children.OfType<PropertyTreeNode>()
			.Single(p => p.PropertyDefinition.Name == "Major");

		// Act — select the property, wait for the decompile.
		vm.AssemblyTreeModel.SelectNode(propertyNode);
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		// Assert — property declaration lands in the output.
		tab.Text.Should().Contain("public int Major");
	}

	[AvaloniaTest]
	public async Task Selecting_Field_Node_Decompiles_Field()
	{
		// FieldTreeNode (System.Math.PI) → decompiled field declaration with its constant
		// initializer.

		// Arrange — boot, wait for assemblies, expand System.Math, grab PI.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var coreLib = typeof(object).Assembly.GetName().Name!;
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			coreLib, "System", "System.Math");
		typeNode.IsExpanded = true;
		var fieldNode = typeNode.Children.OfType<FieldTreeNode>()
			.Single(f => f.FieldDefinition.Name == "PI");

		// Act — select the field, wait for the decompile.
		vm.AssemblyTreeModel.SelectNode(fieldNode);
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		// Assert — declaration and constant value both land.
		tab.Text.Should().Contain("public const double PI");
		tab.Text.Should().Contain("3.14159");
	}

	[AvaloniaTest]
	public async Task Selecting_Event_Node_Decompiles_Event()
	{
		// EventTreeNode (AppDomain.ProcessExit) → decompiled event declaration.

		// Arrange — boot, wait for assemblies, expand AppDomain, grab ProcessExit.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var coreLib = typeof(object).Assembly.GetName().Name!;
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			coreLib, "System", "System.AppDomain");
		typeNode.IsExpanded = true;
		var eventNode = typeNode.Children.OfType<EventTreeNode>()
			.Single(e => e.EventDefinition.Name == "ProcessExit");

		// Act — select the event, wait for the decompile.
		vm.AssemblyTreeModel.SelectNode(eventNode);
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		// Assert — event keyword + name land in the output.
		tab.Text.Should().Contain("event EventHandler");
		tab.Text.Should().Contain("ProcessExit");
	}

	[AvaloniaTest]
	public async Task Selecting_Namespace_Node_Decompiles_Namespace()
	{
		// NamespaceTreeNode → decompiled namespace contents (the small types it contains).

		// Arrange — boot, wait for assemblies, locate System.Runtime.Versioning.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var coreLib = typeof(object).Assembly.GetName().Name!;
		var namespaceNode = vm.AssemblyTreeModel.FindNode<NamespaceTreeNode>(
			coreLib, "System.Runtime.Versioning");

		// Act — select the namespace, wait for the decompile.
		vm.AssemblyTreeModel.SelectNode(namespaceNode);
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		// Assert — representative type names land in the output.
		tab.Text.Should().Contain("TargetFrameworkAttribute");
		tab.Text.Should().Contain("SupportedOSPlatformAttribute");
	}

	[AvaloniaTest]
	public async Task Decompiler_Tab_Title_Tracks_Tree_Node_Text_When_Node_Loads_Late()
	{
		// Opening a fresh assembly via OpenCommand selects it before LoadedAssembly.Text has
		// the full "(version, tfm)" suffix wired up. The tab title used to lock in the bare
		// ShortName at that moment; it must instead update once Text becomes available.

		// Arrange — boot, wait for assemblies.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		// Act 1 — fire OpenCommand on a new path. OpenCommand selects the new node before its
		// rich Text (with version + tfm) settles, exercising the late-update path.
		var newAsmPath = typeof(System.Net.Http.HttpClient).Assembly.Location;
		var registry = AppComposition.Current.GetExport<MainMenuCommandRegistry>();
		var openCommand = registry.Commands
			.Single(c => c.Metadata.Header == nameof(Resources._Open))
			.CreateExport().Value;
		openCommand.Execute(newAsmPath);

		await Waiters.WaitForAsync(() =>
			vm.AssemblyTreeModel.SelectedItem is AssemblyTreeNode n
				&& string.Equals(n.LoadedAssembly.FileName, newAsmPath, System.StringComparison.OrdinalIgnoreCase));

		// Act 2 — wait for the rich form of LoadedAssembly.Text to materialise.
		var node = (AssemblyTreeNode)vm.AssemblyTreeModel.SelectedItem!;
		await node.LoadedAssembly.GetLoadResultAsync();
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

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

		// Arrange — boot, wait for assemblies, run an initial decompile so the second selection
		// produces a real "in flight" period (not the cold-start one).
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var firstNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		vm.AssemblyTreeModel.SelectNode(firstNode);
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		var titles = new List<string>();
		var editorTexts = new List<string>();
		tab.PropertyChanged += OnTabPropertyChanged;

		// Act — kick off a second decompile and observe Title/Text changes throughout.
		var coreLib = typeof(object).Assembly.GetName().Name!;
		var second = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(coreLib, "System", "System.Version");
		vm.AssemblyTreeModel.SelectNode(second);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();

		tab.PropertyChanged -= OnTabPropertyChanged;

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

		// Arrange — boot, wait for assemblies, expand Enumerable, grab two methods.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.IsExpanded = true;
		var asEnumerable = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "AsEnumerable");
		var empty = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Empty");

		// Act — replace the selection with both nodes.
		vm.AssemblyTreeModel.SelectedItems.Clear();
		vm.AssemblyTreeModel.SelectedItems.Add(asEnumerable);
		vm.AssemblyTreeModel.SelectedItems.Add(empty);

		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		// Assert — title is the two display texts joined with ", ".
		tab.Title.Should().Be($"{asEnumerable.Text}, {empty.Text}");
	}

	[AvaloniaTest]
	public async Task Multi_Selecting_Methods_Decompiles_All_Of_Them_Into_One_View()
	{
		// Multi-selection isn't just a title concern — both methods' bodies must actually be
		// decompiled into one combined document.

		// Arrange — boot, wait for assemblies, expand Enumerable, grab two methods.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.IsExpanded = true;
		var asEnumerable = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "AsEnumerable");
		var empty = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Empty");

		// Act — multi-select the two methods.
		vm.AssemblyTreeModel.SelectedItems.Clear();
		vm.AssemblyTreeModel.SelectedItems.Add(asEnumerable);
		vm.AssemblyTreeModel.SelectedItems.Add(empty);

		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

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

		// Arrange — boot, wait for assemblies, find System.Linq.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");

		// Act — select the assembly node, wait for the decompile.
		vm.AssemblyTreeModel.SelectNode(assemblyNode);

		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

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

		// Arrange — boot, wait for assemblies, seed the decompiler tab with an assembly view
		// so the document/editor wiring is realised.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		vm.AssemblyTreeModel.SelectNode(assemblyNode);
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		// Act — drop a synthetic XML resource into the tab. XmlResourceEntryNode triggers
		// SyntaxExtension=".xml" and the XmlFoldingStrategy install.
		var xml = "<root>\n  <child attr=\"v\">\n    text\n  </child>\n  <other>\n  </other>\n</root>";
		var bytes = System.Text.Encoding.UTF8.GetBytes(xml);
		tab.CurrentNode = new XmlResourceEntryNode("test.xml", () => new System.IO.MemoryStream(bytes));
		await Waiters.WaitForAsync(() => tab.SyntaxExtension == ".xml" && tab.Text.Contains("<root>"));

		var view = await window.WaitForComponent<DecompilerTextView>();
		// Drain the layout so ApplyDocument's PropertyChanged handler has executed.
		for (int i = 0; i < 5; i++)
		{
			global::Avalonia.Threading.Dispatcher.UIThread.RunJobs();
			await Task.Delay(20);
		}

		// Assert — FoldingManager is installed (private field, reflected) and produced fold
		// ranges for the multi-line elements.
		var fmField = typeof(DecompilerTextView).GetField("activeFoldingManager",
			System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		fmField.Should().NotBeNull();
		var fm = fmField!.GetValue(view) as global::AvaloniaEdit.Folding.FoldingManager;
		fm.Should().NotBeNull("XML SyntaxExtension should install AvaloniaEdit's XmlFoldingStrategy");
		fm!.AllFoldings.Should().NotBeEmpty("multi-line XML elements should produce fold ranges");
	}
}
