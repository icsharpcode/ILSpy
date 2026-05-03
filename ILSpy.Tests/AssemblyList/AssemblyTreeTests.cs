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

using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.DataGridHierarchical;
using Avalonia.Headless.NUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Properties;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.TreeView;

using ILSpy;
using ILSpy.AppEnv;
using ILSpy.AssemblyTree;
using ILSpy.Commands;
using ILSpy.Images;
using ILSpy.TreeNodes;
using ILSpy.ViewModels;
using ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class AssemblyTreeTests
{
	[AvaloniaTest]
	public async Task Assembly_Node_Has_Resources_Folder_When_Module_Has_Resources()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		// Pick a loaded assembly that ships embedded resources. CoreLib always has at least
		// the localised error-message tables.
		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(coreLibName);
		assemblyNode.EnsureLazyChildren();

		var resources = assemblyNode.Children.OfType<ResourceListTreeNode>().Single();
		resources.Text.Should().Be(Resources._Resources);
		resources.EnsureLazyChildren();
		resources.Children.Should().NotBeEmpty();
		// The factory dispatcher routes typed resources (.xml/.png/.ico/.cur/.xaml) to
		// specialised ResourceEntryNode subclasses; everything else falls back to
		// ResourceTreeNode. Both branches are ILSpyTreeNodes.
		resources.Children.Should().AllBeAssignableTo<ILSpyTreeNode>();
	}

	[AvaloniaTest]
	public async Task DataGrid_Has_Extended_Selection_Mode()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var pane = await window.WaitForComponent<AssemblyListPane>();
		var treeGrid = await pane.WaitForComponent<DataGrid>();
		treeGrid.SelectionMode.Should().Be(DataGridSelectionMode.Extended,
			"the assembly tree must let users Ctrl-click multiple rows");
	}

	[AvaloniaTest]
	public async Task Multi_Selection_Tracks_Multiple_Nodes_And_Marks_Them_IsSelected()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.EnsureLazyChildren();
		var first = typeNode.Children.OfType<MethodTreeNode>()
			.Single(m => m.MethodDefinition.Name == "AsEnumerable");
		var second = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Empty");

		vm.AssemblyTreeModel.SelectedItems.Add(first);
		vm.AssemblyTreeModel.SelectedItems.Add(second);

		vm.AssemblyTreeModel.SelectedItems.Should().Contain(first);
		vm.AssemblyTreeModel.SelectedItems.Should().Contain(second);
		first.IsSelected.Should().BeTrue();
		second.IsSelected.Should().BeTrue();

		vm.AssemblyTreeModel.SelectedItems.Remove(first);
		first.IsSelected.Should().BeFalse();
		second.IsSelected.Should().BeTrue();
	}

	[AvaloniaTest]
	public async Task Loading_Zip_Package_Surfaces_Folders_And_Entries()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var tempDir = Path.Combine(Path.GetTempPath(), "ILSpy.Tests", System.Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(tempDir);
		var zipPath = Path.Combine(tempDir, "fixture.zip");
		using (var zip = ZipFile.Open(zipPath, ZipArchiveMode.Create))
		{
			using (var w = new StreamWriter(zip.CreateEntry("readme.txt").Open()))
				w.Write("hello");
			using (var w = new StreamWriter(zip.CreateEntry("lib/inner.txt").Open()))
				w.Write("inner");
			using (var w = new StreamWriter(zip.CreateEntry("docs/help.html").Open()))
				w.Write("<html />");
		}

		var registry = AppComposition.Current.GetExport<MainMenuCommandRegistry>();
		var openCommand = registry.Commands
			.Single(c => c.Metadata.Header == nameof(Resources._Open))
			.CreateExport().Value;
		openCommand.Execute(zipPath);

		await Waiters.WaitForAsync(() =>
			vm.AssemblyTreeModel.AssemblyList!.GetAssemblies().Any(a =>
				string.Equals(a.FileName, zipPath, System.StringComparison.OrdinalIgnoreCase)));
		var loaded = vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
			.First(a => string.Equals(a.FileName, zipPath, System.StringComparison.OrdinalIgnoreCase));
		await loaded.GetLoadResultAsync();

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(loaded.ShortName);
		assemblyNode.EnsureLazyChildren();

		var folderNames = assemblyNode.Children.OfType<PackageFolderTreeNode>()
			.Select(f => (string)f.Text)
			.ToList();
		folderNames.Should().Contain("lib");
		folderNames.Should().Contain("docs");

		var entryNames = assemblyNode.Children.OfType<ResourceTreeNode>()
			.Select(r => r.Resource.Name)
			.ToList();
		entryNames.Should().Contain("readme.txt");
	}

	[AvaloniaTest]
	public async Task Dot_Resources_File_Resolves_To_ResourcesFileTreeNode()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(coreLibName);
		assemblyNode.EnsureLazyChildren();
		var resources = assemblyNode.Children.OfType<ResourceListTreeNode>().Single();
		resources.EnsureLazyChildren();

		var resourceFileNode = resources.Children.OfType<ResourcesFileTreeNode>().FirstOrDefault();
		((object?)resourceFileNode).Should().NotBeNull(
			"CoreLib must ship at least one embedded .resources file");
		resourceFileNode!.Resource.Name.Should().EndWith(".resources");

		resourceFileNode.EnsureLazyChildren();
		(resourceFileNode.Children.Count > 0 || resourceFileNode.StringTableEntries.Count > 0)
			.Should().BeTrue("a .resources file must surface either inner ResourceEntryNode children or string-table entries");
	}

	[AvaloniaTest]
	public async Task Assembly_Node_Has_References_Folder_Child()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		assemblyNode.EnsureLazyChildren();

		var refFolder = assemblyNode.Children.OfType<ReferenceFolderTreeNode>().Single();
		refFolder.Text.Should().Be(Resources.References);

		refFolder.EnsureLazyChildren();
		refFolder.Children.Should().NotBeEmpty();
		refFolder.Children.Should().AllBeAssignableTo<AssemblyReferenceTreeNode>();
	}

	[AvaloniaTest]
	public async Task Assembly_Reference_Icon_Settles_To_Assembly_Once_Resolved()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		assemblyNode.EnsureLazyChildren();
		var refFolder = assemblyNode.Children.OfType<ReferenceFolderTreeNode>().Single();
		refFolder.EnsureLazyChildren();

		var refNode = refFolder.Children.OfType<AssemblyReferenceTreeNode>()
			.First(n => n.AssemblyReference.Name == "System.Runtime");

		// First access is "loading" — the resolver runs on the background dispatcher.
		var initialIcon = refNode.Icon;
		initialIcon.Should().BeSameAs(global::ILSpy.Images.Images.AssemblyLoading);

		await Waiters.WaitForAsync(() =>
			!ReferenceEquals(refNode.Icon, global::ILSpy.Images.Images.AssemblyLoading));
		refNode.Icon.Should().BeSameAs(global::ILSpy.Images.Images.Assembly);
	}

	[AvaloniaTest]
	public async Task Assembly_Reference_Has_Referenced_Types_Subnode()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		assemblyNode.EnsureLazyChildren();
		var refFolder = assemblyNode.Children.OfType<ReferenceFolderTreeNode>().Single();
		refFolder.EnsureLazyChildren();

		var refNode = refFolder.Children.OfType<AssemblyReferenceTreeNode>()
			.First(n => n.AssemblyReference.Name == "System.Runtime");
		refNode.EnsureLazyChildren();

		var typesNode = refNode.Children.OfType<AssemblyReferenceReferencedTypesTreeNode>().Single();
		typesNode.Text.ToString().Should().Contain(Resources.ReferencedTypes);
		typesNode.EnsureLazyChildren();
		typesNode.Children.Should().NotBeEmpty();
		typesNode.Children.Should().Contain(c => c is TypeReferenceTreeNode);
	}

	[AvaloniaTest]
	public async Task FindNamespaceNode_Returns_The_Tree_Node_For_A_Loaded_Namespace()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		assemblyNode.EnsureLazyChildren();

		var ns = assemblyNode.FindNamespaceNode("System.Linq");
		((object?)ns).Should().NotBeNull();
		ns!.Name.Should().Be("System.Linq");
		assemblyNode.Children.OfType<NamespaceTreeNode>().Should().Contain(ns);
	}

	[AvaloniaTest]
	public async Task FindTypeNode_Returns_The_Tree_Node_For_A_Loaded_Type()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		assemblyNode.EnsureLazyChildren();

		var module = assemblyNode.LoadedAssembly.GetMetadataFileOrNull()!;
		var typeSystem = (MetadataModule)module.GetTypeSystemOrNull()!.MainModule;
		var enumerable = typeSystem.TopLevelTypeDefinitions
			.First(t => t.ReflectionName == "System.Linq.Enumerable");

		var node = assemblyNode.FindTypeNode(enumerable);
		((object?)node).Should().NotBeNull();
		node!.Handle.Should().Be((System.Reflection.Metadata.TypeDefinitionHandle)enumerable.MetadataToken);
	}

	[AvaloniaTest]
	public async Task Member_Reference_Node_Uses_Method_Or_Field_Overlay_Icon()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		assemblyNode.EnsureLazyChildren();
		var refFolder = assemblyNode.Children.OfType<ReferenceFolderTreeNode>().Single();
		refFolder.EnsureLazyChildren();

		// Walk every reference's ReferencedTypes subtree until we find a TypeRef that
		// has at least one MemberReference. System.Linq imports plenty (e.g. delegates,
		// Func ctors) so this should always succeed.
		MemberReferenceTreeNode? memberRefNode = null;
		foreach (var refNode in refFolder.Children.OfType<AssemblyReferenceTreeNode>())
		{
			refNode.EnsureLazyChildren();
			var typesNode = refNode.Children.OfType<AssemblyReferenceReferencedTypesTreeNode>().FirstOrDefault();
			if (typesNode == null)
				continue;
			typesNode.EnsureLazyChildren();
			foreach (var typeRef in typesNode.Children.OfType<TypeReferenceTreeNode>())
			{
				typeRef.EnsureLazyChildren();
				memberRefNode = typeRef.Children.OfType<MemberReferenceTreeNode>().FirstOrDefault();
				if (memberRefNode != null)
					break;
			}
			if (memberRefNode != null)
				break;
		}

		((object?)memberRefNode).Should().NotBeNull(
			"at least one TypeRef under System.Linq's references must have member references");
		memberRefNode!.Icon.Should().BeOneOf(global::ILSpy.Images.Images.MethodReference,
			global::ILSpy.Images.Images.FieldReference);
		memberRefNode.Text.ToString().Should().NotBeNullOrWhiteSpace();
	}

	[AvaloniaTest]
	public async Task Exported_Type_Node_Uses_Export_Overlay_Icon()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		// `mscorlib` is the canonical type-forwarder facade: it reports ExportedTypes that
		// resolve to System.Private.CoreLib. Open it explicitly so the test fixture has a
		// loaded forwarder regardless of what the default startup list includes.
		var coreLibDir = System.IO.Path.GetDirectoryName(typeof(object).Assembly.Location)!;
		var mscorlibPath = System.IO.Path.Combine(coreLibDir, "mscorlib.dll");
		if (!System.IO.File.Exists(mscorlibPath))
		{
			Assert.Inconclusive($"mscorlib.dll not present next to System.Private.CoreLib at {coreLibDir} — runtime layout differs.");
			return;
		}
		var registry = AppComposition.Current.GetExport<MainMenuCommandRegistry>();
		var openCommand = registry.Commands
			.Single(c => c.Metadata.Header == nameof(Resources._Open))
			.CreateExport().Value;
		openCommand.Execute(mscorlibPath);

		await Waiters.WaitForAsync(() =>
			vm.AssemblyTreeModel.AssemblyList!.GetAssemblies().Any(a =>
				string.Equals(a.FileName, mscorlibPath, System.StringComparison.OrdinalIgnoreCase)));
		var loaded = vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
			.First(a => string.Equals(a.FileName, mscorlibPath, System.StringComparison.OrdinalIgnoreCase));
		await loaded.GetLoadResultAsync();

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(loaded.ShortName);
		assemblyNode.EnsureLazyChildren();
		var refFolder = assemblyNode.Children.OfType<ReferenceFolderTreeNode>().Single();
		refFolder.EnsureLazyChildren();

		ExportedTypeTreeNode? exportedNode = null;
		foreach (var refNode in refFolder.Children.OfType<AssemblyReferenceTreeNode>())
		{
			refNode.EnsureLazyChildren();
			var typesNode = refNode.Children.OfType<AssemblyReferenceReferencedTypesTreeNode>().FirstOrDefault();
			if (typesNode == null)
				continue;
			typesNode.EnsureLazyChildren();
			exportedNode = typesNode.Children.OfType<ExportedTypeTreeNode>().FirstOrDefault();
			if (exportedNode != null)
				break;
		}

		((object?)exportedNode).Should().NotBeNull(
			"mscorlib must forward types to System.Private.CoreLib via ExportedType rows");
		exportedNode!.Icon.Should().BeSameAs(global::ILSpy.Images.Images.ExportedType);
		exportedNode.Text.ToString().Should().NotBeNullOrWhiteSpace();
	}

	[AvaloniaTest]
	public async Task Expanding_Assembly_Reference_Reveals_Transitive_References()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		assemblyNode.EnsureLazyChildren();
		var refFolder = assemblyNode.Children.OfType<ReferenceFolderTreeNode>().Single();
		refFolder.EnsureLazyChildren();

		// Many BCL refs resolve to facade assemblies with zero transitive references; iterate
		// until we find one that has its own AssemblyRefs to verify the transitive walk works.
		bool foundTransitive = false;
		foreach (var refNode in refFolder.Children.OfType<AssemblyReferenceTreeNode>())
		{
			refNode.EnsureLazyChildren();
			if (refNode.Children.OfType<AssemblyReferenceTreeNode>().Any())
			{
				foundTransitive = true;
				break;
			}
		}
		foundTransitive.Should().BeTrue("at least one reference must expose its own AssemblyRefs as child nodes");
	}

	[AvaloniaTest]
	public async Task Activating_Assembly_Reference_Selects_Resolved_Assembly_Node()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		assemblyNode.EnsureLazyChildren();
		var refFolder = assemblyNode.Children.OfType<ReferenceFolderTreeNode>().Single();
		refFolder.EnsureLazyChildren();

		var refNode = refFolder.Children.OfType<AssemblyReferenceTreeNode>()
			.Single(n => n.AssemblyReference.Name == "System.Runtime");

		var args = new StubRoutedEventArgs();
		refNode.ActivateItem(args);

		args.Handled.Should().BeTrue();
		await Waiters.WaitForAsync(() =>
			vm.AssemblyTreeModel.SelectedItem is AssemblyTreeNode selected
				&& selected.LoadedAssembly.ShortName == "System.Runtime");
	}

	sealed class StubRoutedEventArgs : ICSharpCode.ILSpyX.TreeView.PlatformAbstractions.IPlatformRoutedEventArgs
	{
		public bool Handled { get; set; }
	}

	[AvaloniaTest]
	public async Task Selecting_Method_Auto_Loads_Referenced_Assemblies()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var newAsmPath = typeof(System.Net.Http.HttpClient).Assembly.Location;
		var registry = AppComposition.Current.GetExport<MainMenuCommandRegistry>();
		var openCommand = registry.Commands
			.Single(c => c.Metadata.Header == nameof(Resources._Open))
			.CreateExport().Value;
		openCommand.Execute(newAsmPath);

		await Waiters.WaitForAsync(() =>
			vm.AssemblyTreeModel.AssemblyList!.GetAssemblies().Any(a =>
				string.Equals(a.FileName, newAsmPath, System.StringComparison.OrdinalIgnoreCase)));

		var loaded = vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
			.First(a => string.Equals(a.FileName, newAsmPath, System.StringComparison.OrdinalIgnoreCase));
		await loaded.GetLoadResultAsync();

		var initialFiles = vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
			.Select(a => a.FileName)
			.ToHashSet(System.StringComparer.OrdinalIgnoreCase);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			loaded.ShortName, "System.Net.Http", "System.Net.Http.HttpClient");
		typeNode.EnsureLazyChildren();
		var methodNode = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "CancelPendingRequests");

		vm.AssemblyTreeModel.SelectedItem = methodNode;
		await vm.DockWorkspace.WaitForDecompiledTextAsync();

		await Waiters.WaitForAsync(() =>
			vm.AssemblyTreeModel.AssemblyList!.GetAssemblies().Any(a => !initialFiles.Contains(a.FileName)));

		var addedAssemblies = vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
			.Where(a => !initialFiles.Contains(a.FileName))
			.ToList();
		addedAssemblies.Should().NotBeEmpty();
		addedAssemblies.Should().OnlyContain(a => a.IsAutoLoaded);

		foreach (var auto in addedAssemblies)
		{
			var node = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(auto.ShortName);
			node.IsAutoLoaded.Should().BeTrue();
		}
	}

	[AvaloniaTest]
	public async Task Auto_Loaded_Assemblies_Are_Not_Persisted()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var newAsmPath = typeof(System.Net.Http.HttpClient).Assembly.Location;
		var registry = AppComposition.Current.GetExport<MainMenuCommandRegistry>();
		var openCommand = registry.Commands
			.Single(c => c.Metadata.Header == nameof(Resources._Open))
			.CreateExport().Value;
		openCommand.Execute(newAsmPath);

		await Waiters.WaitForAsync(() =>
			vm.AssemblyTreeModel.AssemblyList!.GetAssemblies().Any(a =>
				string.Equals(a.FileName, newAsmPath, System.StringComparison.OrdinalIgnoreCase)));

		var loaded = vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
			.First(a => string.Equals(a.FileName, newAsmPath, System.StringComparison.OrdinalIgnoreCase));
		await loaded.GetLoadResultAsync();

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			loaded.ShortName, "System.Net.Http", "System.Net.Http.HttpClient");
		typeNode.EnsureLazyChildren();
		var methodNode = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "CancelPendingRequests");

		vm.AssemblyTreeModel.SelectedItem = methodNode;
		await vm.DockWorkspace.WaitForDecompiledTextAsync();

		await Waiters.WaitForAsync(() =>
			vm.AssemblyTreeModel.AssemblyList!.GetAssemblies().Any(a => a.IsAutoLoaded));

		var settingsService = AppComposition.Current.GetExport<SettingsService>();
		var manager = settingsService.AssemblyListManager;
		manager.SaveList(vm.AssemblyTreeModel.AssemblyList!);

		var settingsPath = ICSharpCode.ILSpyX.Settings.ILSpySettings.SettingsFilePathProvider!();
		var doc = XDocument.Load(settingsPath);
		var savedFiles = doc.Descendants("Assembly")
			.Select(e => e.Value)
			.ToHashSet(System.StringComparer.OrdinalIgnoreCase);

		var assemblies = vm.AssemblyTreeModel.AssemblyList!.GetAssemblies();
		var autoLoaded = assemblies.Where(a => a.IsAutoLoaded).ToList();
		autoLoaded.Should().NotBeEmpty();
		foreach (var asm in autoLoaded)
			savedFiles.Should().NotContain(asm.FileName, "auto-loaded assembly {0} must not be persisted", asm.FileName);

		var manualLoaded = assemblies.Where(a => !a.IsAutoLoaded).ToList();
		foreach (var asm in manualLoaded)
			savedFiles.Should().Contain(asm.FileName, "manually loaded assembly {0} must be persisted", asm.FileName);
	}

	[AvaloniaTest]
	public async Task Opening_Assembly_Adds_It_To_The_Tree()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var newAsmPath = typeof(System.Net.Http.HttpClient).Assembly.Location;
		var initialCount = vm.AssemblyTreeModel.AssemblyList!.Count;

		var registry = AppComposition.Current.GetExport<MainMenuCommandRegistry>();
		var openCommand = registry.Commands
			.Single(c => c.Metadata.Header == nameof(Resources._Open))
			.CreateExport().Value;
		openCommand.Execute(newAsmPath);

		await Waiters.WaitForAsync(() =>
			vm.AssemblyTreeModel.AssemblyList!.GetAssemblies().Any(a =>
				string.Equals(a.FileName, newAsmPath, System.StringComparison.OrdinalIgnoreCase)));

		var loaded = vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
			.First(a => string.Equals(a.FileName, newAsmPath, System.StringComparison.OrdinalIgnoreCase));
		await loaded.GetLoadResultAsync();

		vm.AssemblyTreeModel.AssemblyList!.Count.Should().Be(initialCount + 1);
		var node = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(loaded.ShortName);
		node.LoadedAssembly.Should().BeSameAs(loaded);
		node.IsSelected.Should().BeTrue();
	}

	[AvaloniaTest]
	public async Task User_Click_On_Visible_Row_Does_Not_Recentre_Viewport()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var enumerable = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		enumerable.EnsureLazyChildren();
		var ns = (NamespaceTreeNode)enumerable.Parent!;
		ns.EnsureLazyChildren();
		foreach (var child in ns.Children.OfType<TypeTreeNode>())
			child.EnsureLazyChildren();

		var pane = await window.WaitForComponent<AssemblyListPane>();
		var grid = await pane.WaitForComponent<DataGrid>();

		// Park the viewport mid-list so the next-clicked row sits at a non-zero offset. The
		// regression would re-centre on click and snap the offset back; we want offset to
		// stay put.
		vm.AssemblyTreeModel.SelectedItem = enumerable;
		await Waiters.WaitForAsync(() => ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, enumerable));
		for (int i = 0; i < 8; i++)
		{
			Dispatcher.UIThread.RunJobs();
			await Task.Delay(25);
		}
		grid.UpdateLayout();

		var scrollViewer = grid.GetVisualDescendants().OfType<ScrollViewer>().Single();
		(scrollViewer.Extent.Height - scrollViewer.Viewport.Height).Should().BeGreaterThan(50,
			"the grid must have something to scroll for this test to be meaningful");
		scrollViewer.Offset = new Vector(scrollViewer.Offset.X, 50);
		grid.UpdateLayout();
		Dispatcher.UIThread.RunJobs();
		scrollViewer.Offset.Y.Should().BeGreaterThan(5,
			"the test scenario requires the viewport be parked mid-list");

		// Pick a different visible row to click on; clicking an already-visible row must NOT
		// move the viewport. Use the *bottom-most* visible row for a strict check — if the bug
		// regressed, CenterRowInView would scroll it up to the middle and offset would change.
		var candidateRow = grid.GetVisualDescendants().OfType<DataGridRow>()
			.Where(r => !r.IsSelected
				&& r.TranslatePoint(new Point(0, 0), scrollViewer) is { } p
				&& p.Y >= 0 && p.Y + r.Bounds.Height <= scrollViewer.Viewport.Height)
			.OrderByDescending(r => r.TranslatePoint(new Point(0, 0), scrollViewer)!.Value.Y)
			.FirstOrDefault();
		candidateRow.Should().NotBeNull("the test needs a visible non-selected row to click");

		var offsetBefore = scrollViewer.Offset.Y;

		// Real pointer click — setting SelectedItem programmatically would fire DataGrid's
		// internal ScrollIntoView too, which a real user click does not.
		var rowCentre = candidateRow!.TranslatePoint(
			new Point(candidateRow.Bounds.Width / 2, candidateRow.Bounds.Height / 2),
			window)!.Value;
		global::Avalonia.Headless.HeadlessWindowExtensions.MouseDown(window, rowCentre, global::Avalonia.Input.MouseButton.Left);
		global::Avalonia.Headless.HeadlessWindowExtensions.MouseUp(window, rowCentre, global::Avalonia.Input.MouseButton.Left);

		for (int i = 0; i < 8; i++)
		{
			Dispatcher.UIThread.RunJobs();
			await Task.Delay(25);
		}

		scrollViewer.Offset.Y.Should().BeApproximately(offsetBefore, 1.0,
			"clicking an already-visible row must not move the viewport");
	}

	[AvaloniaTest]
	public async Task Save_Code_Command_Dispatches_Single_Selected_Node_Save_Override()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var probe = new SaveProbeNode();
		vm.AssemblyTreeModel.SelectedItem = probe;

		var registry = AppComposition.Current.GetExport<MainMenuCommandRegistry>();
		var saveCmd = registry.Commands
			.Single(c => c.Metadata.Header == nameof(Resources._SaveCode))
			.CreateExport().Value;
		saveCmd.Execute(null);

		probe.SaveCalled.Should().BeTrue(
			"Save Code on a single ILSpyTreeNode selection must dispatch through ILSpyTreeNode.Save()");
	}

	sealed class SaveProbeNode : ILSpyTreeNode
	{
		public bool SaveCalled { get; private set; }
		public override bool Save()
		{
			SaveCalled = true;
			return true;
		}
		public override object Text => "SaveProbeNode";
	}

	[AvaloniaTest]
	public async Task Save_Code_Command_Writes_Full_Decompilation_To_Picked_Path()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.EnsureLazyChildren();
		var method = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "AsEnumerable");
		vm.AssemblyTreeModel.SelectedItem = method;
		await vm.DockWorkspace.WaitForDecompiledTextAsync();

		var registry = AppComposition.Current.GetExport<MainMenuCommandRegistry>();
		var saveCmd = (global::ILSpy.Commands.SaveCommand)registry.Commands
			.Single(c => c.Metadata.Header == nameof(Resources._SaveCode))
			.CreateExport().Value;

		var tempFile = System.IO.Path.Combine(
			System.IO.Path.GetTempPath(),
			"ilspy-save-" + System.Guid.NewGuid().ToString("N") + ".cs");
		try
		{
			await saveCmd.SaveCodeAsync(tempFile);

			System.IO.File.Exists(tempFile).Should().BeTrue();
			var contents = await System.IO.File.ReadAllTextAsync(tempFile);
			contents.Should().Contain("AsEnumerable",
				"the full decompilation should include the selected method's name");
			contents.Should().Contain("return source",
				"the method body should be present (FullDecompilation = true)");
		}
		finally
		{
			if (System.IO.File.Exists(tempFile))
				System.IO.File.Delete(tempFile);
		}
	}

	[AvaloniaTest]
	public async Task Delete_Key_On_Selected_Assembly_Unloads_It_From_The_List()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		// Pick the assembly we'll evict + a sibling we expect to survive.
		var sacrificialName = typeof(System.Linq.Enumerable).Assembly.GetName().Name!;
		var survivorName = typeof(object).Assembly.GetName().Name!;
		var sacrificialNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(sacrificialName);
		vm.AssemblyTreeModel.SelectedItem = sacrificialNode;
		await Waiters.WaitForAsync(() => ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, sacrificialNode));

		var pane = await window.WaitForComponent<AssemblyListPane>();
		var grid = await pane.WaitForComponent<DataGrid>();
		grid.Focus();
		Dispatcher.UIThread.RunJobs();

		// Headless keyboard event — simulates the user pressing Del while the tree has focus.
		global::Avalonia.Headless.HeadlessWindowExtensions.KeyPress(
			window,
			global::Avalonia.Input.Key.Delete,
			global::Avalonia.Input.RawInputModifiers.None,
			global::Avalonia.Input.PhysicalKey.Delete,
			null);

		await Waiters.WaitForAsync(() =>
			!vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
				.Any(a => string.Equals(a.ShortName, sacrificialName, System.StringComparison.Ordinal)));

		vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
			.Should().Contain(a => a.ShortName == survivorName,
				"only the selected assembly should be removed");
	}

	[AvaloniaTest]
	public async Task Clear_Assembly_List_Command_Empties_The_Active_List()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);
		vm.AssemblyTreeModel.AssemblyList!.Count.Should().BeGreaterThan(0);

		var registry = AppComposition.Current.GetExport<MainMenuCommandRegistry>();
		var clearCmd = registry.Commands
			.Single(c => c.Metadata.Header == nameof(Resources.ClearAssemblyList))
			.CreateExport().Value;
		clearCmd.CanExecute(null).Should().BeTrue("non-empty list must enable Clear");
		clearCmd.Execute(null);

		await Waiters.WaitForAsync(() => vm.AssemblyTreeModel.AssemblyList!.Count == 0);
		vm.AssemblyTreeModel.AssemblyList!.Count.Should().Be(0);
	}

	[AvaloniaTest]
	public async Task Remove_Assemblies_With_Load_Errors_Drops_Only_Broken_Entries()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		// Add a "broken" assembly: a real file that isn't a PE binary. AssemblyList records
		// it but the load fails — that's exactly the state the command targets.
		var brokenPath = System.IO.Path.Combine(
			System.IO.Path.GetTempPath(),
			"ilspy-bogus-" + System.Guid.NewGuid().ToString("N") + ".dll");
		await System.IO.File.WriteAllTextAsync(brokenPath, "not a real PE file");
		try
		{
			vm.AssemblyTreeModel.OpenFiles(new[] { brokenPath });
			await Waiters.WaitForAsync(() =>
				vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
					.Any(a => string.Equals(a.FileName, brokenPath, System.StringComparison.OrdinalIgnoreCase)));
			var broken = vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
				.First(a => string.Equals(a.FileName, brokenPath, System.StringComparison.OrdinalIgnoreCase));
			// GetLoadResultAsync rethrows the load failure; HasLoadError is the safe probe.
			await Waiters.WaitForAsync(() => broken.HasLoadError);

			var validBefore = vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
				.Where(a => !a.HasLoadError)
				.Select(a => a.FileName)
				.ToList();
			validBefore.Should().NotBeEmpty("at least one real assembly must remain to verify it's spared");

			var registry = AppComposition.Current.GetExport<MainMenuCommandRegistry>();
			var cmd = registry.Commands
				.Single(c => c.Metadata.Header == nameof(Resources._RemoveAssembliesWithLoadErrors))
				.CreateExport().Value;
			cmd.CanExecute(null).Should().BeTrue("a broken entry must enable the command");
			cmd.Execute(null);

			await Waiters.WaitForAsync(() =>
				!vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
					.Any(a => string.Equals(a.FileName, brokenPath, System.StringComparison.OrdinalIgnoreCase)));

			var remaining = vm.AssemblyTreeModel.AssemblyList!.GetAssemblies().Select(a => a.FileName).ToList();
			remaining.Should().NotContain(brokenPath, "the broken assembly should be removed");
			foreach (var path in validBefore)
				remaining.Should().Contain(path, "valid assemblies must be preserved");
		}
		finally
		{
			if (System.IO.File.Exists(brokenPath))
				System.IO.File.Delete(brokenPath);
		}
	}
}
