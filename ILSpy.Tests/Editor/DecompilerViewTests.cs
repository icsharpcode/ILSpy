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
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.EnsureLazyChildren();
		var methodNode = typeNode.Children.OfType<MethodTreeNode>()
			.Single(m => m.MethodDefinition.Name == "AsEnumerable");
		methodNode.MethodDefinition.IsExtensionMethod.Should().BeTrue();

		vm.AssemblyTreeModel.SelectedItem = methodNode;
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		tab.Text.Should().Contain("AsEnumerable");
		tab.Text.Should().Contain("IEnumerable<TSource>");
		tab.Text.Should().Contain("return source");

		methodNode.Should().Be().CenteredInView();
	}

	[AvaloniaTest]
	public async Task Selecting_References_Folder_Decompiles_References_Listing()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		assemblyNode.EnsureLazyChildren();
		var refFolder = assemblyNode.Children.OfType<ReferenceFolderTreeNode>().Single();

		vm.AssemblyTreeModel.SelectedItem = refFolder;
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		tab.Text.Should().Contain("Detected TargetFramework-Id:");
		tab.Text.Should().Contain("Detected RuntimePack:");
		tab.Text.Should().Contain("Referenced assemblies (in metadata order):");
		tab.Text.Should().Contain("System.Runtime");
		tab.Text.Should().Contain("Assembly load log including transitive references:");
	}

	[AvaloniaTest]
	public async Task Selecting_Type_Node_Decompiles_Type_Definition()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var coreLib = typeof(object).Assembly.GetName().Name!;
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			coreLib, "System", "System.Version");

		vm.AssemblyTreeModel.SelectedItem = typeNode;
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		tab.Text.Should().Contain("class Version");
		tab.Text.Should().Contain("Major");
		tab.Text.Should().Contain("Minor");
	}

	[AvaloniaTest]
	public async Task Selecting_Property_Node_Decompiles_Property()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var coreLib = typeof(object).Assembly.GetName().Name!;
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			coreLib, "System", "System.Version");
		typeNode.EnsureLazyChildren();
		var propertyNode = typeNode.Children.OfType<PropertyTreeNode>()
			.Single(p => p.PropertyDefinition.Name == "Major");

		vm.AssemblyTreeModel.SelectedItem = propertyNode;
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		tab.Text.Should().Contain("public int Major");
	}

	[AvaloniaTest]
	public async Task Selecting_Field_Node_Decompiles_Field()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var coreLib = typeof(object).Assembly.GetName().Name!;
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			coreLib, "System", "System.Math");
		typeNode.EnsureLazyChildren();
		var fieldNode = typeNode.Children.OfType<FieldTreeNode>()
			.Single(f => f.FieldDefinition.Name == "PI");

		vm.AssemblyTreeModel.SelectedItem = fieldNode;
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		tab.Text.Should().Contain("public const double PI");
		tab.Text.Should().Contain("3.14159");
	}

	[AvaloniaTest]
	public async Task Selecting_Event_Node_Decompiles_Event()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var coreLib = typeof(object).Assembly.GetName().Name!;
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			coreLib, "System", "System.AppDomain");
		typeNode.EnsureLazyChildren();
		var eventNode = typeNode.Children.OfType<EventTreeNode>()
			.Single(e => e.EventDefinition.Name == "ProcessExit");

		vm.AssemblyTreeModel.SelectedItem = eventNode;
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		tab.Text.Should().Contain("event EventHandler");
		tab.Text.Should().Contain("ProcessExit");
	}

	[AvaloniaTest]
	public async Task Selecting_Namespace_Node_Decompiles_Namespace()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var coreLib = typeof(object).Assembly.GetName().Name!;
		var namespaceNode = vm.AssemblyTreeModel.FindNode<NamespaceTreeNode>(
			coreLib, "System.Runtime.Versioning");

		vm.AssemblyTreeModel.SelectedItem = namespaceNode;
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		tab.Text.Should().Contain("TargetFrameworkAttribute");
		tab.Text.Should().Contain("SupportedOSPlatformAttribute");
	}

	[AvaloniaTest]
	public async Task Decompiler_Tab_Title_Tracks_Tree_Node_Text_When_Node_Loads_Late()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		// Open a fresh assembly via the command and immediately let the OpenCommand select it,
		// before LoadedAssembly.Text has the rich "(version, tfm)" suffix wired up. This is the
		// timing window where the tab title used to lock in the bare ShortName.
		var newAsmPath = typeof(System.Net.Http.HttpClient).Assembly.Location;
		var registry = AppComposition.Current.GetExport<MainMenuCommandRegistry>();
		var openCommand = registry.Commands
			.Single(c => c.Metadata.Header == nameof(Resources._Open))
			.CreateExport().Value;
		openCommand.Execute(newAsmPath);

		await Waiters.WaitForAsync(() =>
			vm.AssemblyTreeModel.SelectedItem is AssemblyTreeNode n
				&& string.Equals(n.LoadedAssembly.FileName, newAsmPath, System.StringComparison.OrdinalIgnoreCase));

		var node = (AssemblyTreeNode)vm.AssemblyTreeModel.SelectedItem!;
		await node.LoadedAssembly.GetLoadResultAsync();
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		await Waiters.WaitForAsync(() => string.Equals(tab.Title, node.Text?.ToString(), System.StringComparison.Ordinal));
		node.Text!.ToString().Should().NotBe(node.LoadedAssembly.ShortName,
			"the rich form (with version + tfm) must be available, otherwise the test isn't exercising the late-update path");
		tab.Title.Should().Be(node.Text!.ToString());
	}

	[AvaloniaTest]
	public async Task Decompile_Tab_Title_Cycles_Round_Spinner_While_Decompiling()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var firstNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		vm.AssemblyTreeModel.SelectedItem = firstNode;
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		var titles = new List<string>();
		var editorTexts = new List<string>();
		tab.PropertyChanged += OnTabPropertyChanged;

		var coreLib = typeof(object).Assembly.GetName().Name!;
		var second = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(coreLib, "System", "System.Version");
		vm.AssemblyTreeModel.SelectedItem = second;
		await vm.DockWorkspace.WaitForDecompiledTextAsync();

		tab.PropertyChanged -= OnTabPropertyChanged;

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
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.EnsureLazyChildren();
		var asEnumerable = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "AsEnumerable");
		var empty = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Empty");

		vm.AssemblyTreeModel.SelectedItems.Clear();
		vm.AssemblyTreeModel.SelectedItems.Add(asEnumerable);
		vm.AssemblyTreeModel.SelectedItems.Add(empty);

		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		tab.Title.Should().Be($"{asEnumerable.Text}, {empty.Text}");
	}

	[AvaloniaTest]
	public async Task Multi_Selecting_Methods_Decompiles_All_Of_Them_Into_One_View()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.EnsureLazyChildren();
		var asEnumerable = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "AsEnumerable");
		var empty = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Empty");

		vm.AssemblyTreeModel.SelectedItems.Clear();
		vm.AssemblyTreeModel.SelectedItems.Add(asEnumerable);
		vm.AssemblyTreeModel.SelectedItems.Add(empty);

		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		tab.Text.Should().Contain("AsEnumerable");
		tab.Text.Should().Contain("Empty");
		// Body fragment from AsEnumerable to confirm both bodies actually rendered.
		tab.Text.Should().Contain("return source");
	}

	[AvaloniaTest]
	public async Task Selecting_Assembly_Node_Emits_Header_And_Assembly_Attributes()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		vm.AssemblyTreeModel.SelectedItem = assemblyNode;

		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
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
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		// Seed the decompiler tab.
		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		vm.AssemblyTreeModel.SelectedItem = assemblyNode;
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		// Drop in a synthetic XML resource. XmlResourceEntryNode → SyntaxExtension=".xml" →
		// DecompilerTextView.ApplyDocument runs XmlFoldingStrategy.
		var xml = "<root>\n  <child attr=\"v\">\n    text\n  </child>\n  <other>\n  </other>\n</root>";
		var bytes = System.Text.Encoding.UTF8.GetBytes(xml);
		tab.CurrentNode = new XmlResourceEntryNode("test.xml", () => new System.IO.MemoryStream(bytes));
		await Waiters.WaitForAsync(() => tab.SyntaxExtension == ".xml" && tab.Text.Contains("<root>"));

		var view = window.GetVisualDescendants().OfType<DecompilerTextView>().Single();
		// Drain the layout so ApplyDocument's PropertyChanged handler has executed.
		for (int i = 0; i < 5; i++)
		{
			global::Avalonia.Threading.Dispatcher.UIThread.RunJobs();
			await Task.Delay(20);
		}

		// FoldingManager is held in a private field — reflection is the lightest probe; a
		// public accessor purely for tests would leak detail.
		var fmField = typeof(DecompilerTextView).GetField("activeFoldingManager",
			System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		fmField.Should().NotBeNull();
		var fm = fmField!.GetValue(view) as global::AvaloniaEdit.Folding.FoldingManager;
		fm.Should().NotBeNull("XML SyntaxExtension should install AvaloniaEdit's XmlFoldingStrategy");
		fm!.AllFoldings.Should().NotBeEmpty("multi-line XML elements should produce fold ranges");
	}
}
