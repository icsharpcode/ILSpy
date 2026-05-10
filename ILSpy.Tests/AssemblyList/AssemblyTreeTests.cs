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
using Avalonia.Headless.NUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpyX;

using ILSpy;
using ILSpy.AppEnv;
using ILSpy.AssemblyTree;
using ILSpy.Commands;
using ILSpy.Docking;
using ILSpy.Metadata;
using ILSpy.Metadata.CorTables;
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
		// Assemblies with embedded resources expose a "Resources" folder under the assembly
		// node. Verifies the folder is present, contains entries, and every entry is an
		// ILSpyTreeNode (typed factory routes specialised handlers, generic falls back to
		// ResourceTreeNode — both are ILSpyTreeNodes).

		// Arrange — boot, wait for assemblies.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		// Act — pick CoreLib (always ships localised error-message tables), expand it.
		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(coreLibName);
		assemblyNode.EnsureLazyChildren();
		assemblyNode.IsExpanded = true;

		// Assert — exactly one Resources folder, named, non-empty, every child an ILSpyTreeNode.
		var resources = assemblyNode.Children.OfType<ResourceListTreeNode>().Single();
		resources.Text.Should().Be(Resources._Resources);
		resources.EnsureLazyChildren();
		resources.IsExpanded = true;
		resources.Children.Should().NotBeEmpty();
		resources.Children.Should().AllBeAssignableTo<ILSpyTreeNode>();
	}

	[AvaloniaTest]
	public async Task DataGrid_Has_Extended_Selection_Mode()
	{
		// The assembly tree DataGrid must run in Extended selection mode so users can
		// Ctrl-click to multi-select rows.

		// Arrange — boot, wait for assemblies + AssemblyListPane to materialise.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		// Act — locate the realised DataGrid inside the AssemblyListPane.
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var treeGrid = await pane.WaitForComponent<DataGrid>();

		// Assert — SelectionMode is Extended.
		treeGrid.SelectionMode.Should().Be(DataGridSelectionMode.Extended,
			"the assembly tree must let users Ctrl-click multiple rows");
	}

	[AvaloniaTest]
	public async Task Multi_Selection_Tracks_Multiple_Nodes_And_Marks_Them_IsSelected()
	{
		// Adding multiple nodes to AssemblyTreeModel.SelectedItems must reflect on each node's
		// IsSelected flag (used by the tree view template binding) and removing one node must
		// flip its IsSelected back without affecting siblings.

		// Arrange — boot, wait for assemblies, expand Enumerable, grab two methods.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.EnsureLazyChildren();
		typeNode.IsExpanded = true;
		var first = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "ElementAt");
		var second = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Empty");

		// Act 1 — add both nodes to the multi-selection.
		vm.AssemblyTreeModel.SelectedItems.Add(first);
		vm.AssemblyTreeModel.SelectedItems.Add(second);

		// Assert 1 — both are tracked and both report IsSelected.
		vm.AssemblyTreeModel.SelectedItems.Should().Contain(first);
		vm.AssemblyTreeModel.SelectedItems.Should().Contain(second);
		first.IsSelected.Should().BeTrue();
		second.IsSelected.Should().BeTrue();

		// Act 2 — remove one.
		vm.AssemblyTreeModel.SelectedItems.Remove(first);

		// Assert 2 — only the removed one flips back.
		first.IsSelected.Should().BeFalse();
		second.IsSelected.Should().BeTrue();
	}

	[AvaloniaTest]
	public async Task Loading_Zip_Package_Surfaces_Folders_And_Entries()
	{
		// Loading a .zip should run the bundled-archive file loader: nested directories become
		// PackageFolderTreeNodes, top-level files become ResourceTreeNodes.

		// Arrange — boot, wait for assemblies, build a fixture zip with a flat file + two
		// folders containing one file each.
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

		// Act — open the zip via the OpenCommand and wait for it to load.
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
		assemblyNode.IsExpanded = true;

		// Assert — both nested folders show as PackageFolderTreeNodes; the flat file shows as
		// a ResourceTreeNode with its original entry name.
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
		// .resources entries get the dedicated ResourcesFileTreeNode handler, which exposes a
		// string-table or inner ResourceEntryNode children (depending on payload type).

		// Arrange — boot, wait for assemblies, expand CoreLib's Resources folder.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(coreLibName);
		assemblyNode.EnsureLazyChildren();
		assemblyNode.IsExpanded = true;
		var resources = assemblyNode.Children.OfType<ResourceListTreeNode>().Single();
		resources.EnsureLazyChildren();
		resources.IsExpanded = true;

		// Act — look up the first .resources file and select it.
		var resourceFileNode = resources.Children.OfType<ResourcesFileTreeNode>().FirstOrDefault();
		((object?)resourceFileNode).Should().NotBeNull(
			"CoreLib must ship at least one embedded .resources file");
		resourceFileNode!.Resource.Name.Should().EndWith(".resources");

		resourceFileNode.EnsureLazyChildren();

		// Assert — either inner children or string-table entries — at least one branch must
		// surface non-empty content.
		(resourceFileNode.Children.Count > 0 || resourceFileNode.StringTableEntries.Count > 0)
			.Should().BeTrue("a .resources file must surface either inner ResourceEntryNode children or string-table entries");
	}

	[AvaloniaTest]
	public async Task Assembly_Node_Has_References_Folder_Child()
	{
		// Each assembly node exposes a "References" folder child whose children are
		// AssemblyReferenceTreeNodes — one per AssemblyRef row.

		// Arrange — boot, wait for assemblies, expand System.Linq.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		assemblyNode.EnsureLazyChildren();
		assemblyNode.IsExpanded = true;

		// Act — find the References folder.
		var refFolder = assemblyNode.Children.OfType<ReferenceFolderTreeNode>().Single();

		// Assert — folder is named, expands to non-empty AssemblyReferenceTreeNode children.
		refFolder.Text.Should().Be(Resources.References);

		refFolder.EnsureLazyChildren();
		refFolder.IsExpanded = true;
		refFolder.Children.Should().NotBeEmpty();
		refFolder.Children.Should().AllBeAssignableTo<AssemblyReferenceTreeNode>();
	}

	[AvaloniaTest]
	public async Task Assembly_Reference_Icon_Settles_To_Assembly_Once_Resolved()
	{
		// AssemblyReferenceTreeNode.Icon starts as the AssemblyLoading glyph and asynchronously
		// flips to the resolved Assembly glyph once the resolver finishes — verifies the
		// async-icon two-stage flow.

		// Arrange — boot, wait for assemblies, force lazy children on assembly + References.
		// Stay model-only (no IsExpanded = true) — visually expanding rows would render the
		// icons, triggering the resolver and losing the lazy "loading" state we're asserting.
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

		// Act 1 + Assert 1 — first access triggers the loading state.
		var initialIcon = refNode.Icon;
		initialIcon.Should().BeSameAs(global::ILSpy.Images.Images.AssemblyLoading);

		// Act 2 — wait for resolver to finish.
		await Waiters.WaitForAsync(() =>
			!ReferenceEquals(refNode.Icon, global::ILSpy.Images.Images.AssemblyLoading));

		// Assert 2 — icon settles on the resolved Assembly glyph.
		refNode.Icon.Should().BeSameAs(global::ILSpy.Images.Images.Assembly);
	}

	[AvaloniaTest]
	public async Task Assembly_Reference_Has_Referenced_Types_Subnode()
	{
		// Each AssemblyReferenceTreeNode contains a "Referenced Types" subnode that lists the
		// TypeRef rows the host assembly imports from that reference.

		// Arrange — boot, wait for assemblies, expand System.Linq → References → System.Runtime.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		assemblyNode.EnsureLazyChildren();
		assemblyNode.IsExpanded = true;
		var refFolder = assemblyNode.Children.OfType<ReferenceFolderTreeNode>().Single();
		refFolder.EnsureLazyChildren();
		refFolder.IsExpanded = true;

		var refNode = refFolder.Children.OfType<AssemblyReferenceTreeNode>()
			.First(n => n.AssemblyReference.Name == "System.Runtime");
		refNode.EnsureLazyChildren();
		refNode.IsExpanded = true;

		// Act — find the ReferencedTypes subnode and expand it.
		var typesNode = refNode.Children.OfType<AssemblyReferenceReferencedTypesTreeNode>().Single();
		typesNode.EnsureLazyChildren();
		typesNode.IsExpanded = true;

		// Assert — header text contains the expected label, and the children include at least
		// one TypeReferenceTreeNode.
		typesNode.Text.ToString().Should().Contain(Resources.ReferencedTypes);
		typesNode.Children.Should().NotBeEmpty();
		typesNode.Children.Should().Contain(c => c is TypeReferenceTreeNode);
	}

	[AvaloniaTest]
	public async Task FindNamespaceNode_Returns_The_Tree_Node_For_A_Loaded_Namespace()
	{
		// AssemblyTreeNode.FindNamespaceNode is the lookup primitive used by navigation and
		// search to land on a specific namespace.

		// Arrange — boot, wait for assemblies, expand System.Linq.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		assemblyNode.EnsureLazyChildren();
		assemblyNode.IsExpanded = true;

		// Act — look up the System.Linq namespace.
		var ns = assemblyNode.FindNamespaceNode("System.Linq");

		// Assert — returns the actual NamespaceTreeNode child, not a clone.
		((object?)ns).Should().NotBeNull();
		ns!.Name.Should().Be("System.Linq");
		assemblyNode.Children.OfType<NamespaceTreeNode>().Should().Contain(ns);
	}

	[AvaloniaTest]
	public async Task FindTypeNode_Returns_The_Tree_Node_For_A_Loaded_Type()
	{
		// AssemblyTreeNode.FindTypeNode is the lookup primitive used by navigation and search
		// to land on a specific type by ITypeDefinition.

		// Arrange — boot, wait for assemblies, expand System.Linq.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		assemblyNode.EnsureLazyChildren();
		assemblyNode.IsExpanded = true;

		// Act — resolve System.Linq.Enumerable's ITypeDefinition, then look up its tree node.
		var module = assemblyNode.LoadedAssembly.GetMetadataFileOrNull()!;
		var typeSystem = (MetadataModule)module.GetTypeSystemOrNull()!.MainModule;
		var enumerable = typeSystem.TopLevelTypeDefinitions
			.First(t => t.ReflectionName == "System.Linq.Enumerable");

		var node = assemblyNode.FindTypeNode(enumerable);

		// Assert — non-null, with the matching metadata token.
		((object?)node).Should().NotBeNull();
		node!.Handle.Should().Be((System.Reflection.Metadata.TypeDefinitionHandle)enumerable.MetadataToken);
	}

	[AvaloniaTest]
	public async Task Member_Reference_Node_Uses_Method_Or_Field_Overlay_Icon()
	{
		// MemberReferenceTreeNode renders with an icon that overlays a "reference arrow" on
		// top of the underlying Method/Field glyph — verifies that overlay routing works.

		// Arrange — boot, wait for assemblies, expand System.Linq → References.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		assemblyNode.EnsureLazyChildren();
		assemblyNode.IsExpanded = true;
		var refFolder = assemblyNode.Children.OfType<ReferenceFolderTreeNode>().Single();
		refFolder.EnsureLazyChildren();
		refFolder.IsExpanded = true;

		// Act — walk every reference's ReferencedTypes subtree until we find a TypeRef with
		// at least one MemberReference. System.Linq imports plenty (delegates, Func ctors)
		// so this should always succeed.
		MemberReferenceTreeNode? memberRefNode = null;
		foreach (var refNode in refFolder.Children.OfType<AssemblyReferenceTreeNode>())
		{
			refNode.EnsureLazyChildren();
			refNode.IsExpanded = true;
			var typesNode = refNode.Children.OfType<AssemblyReferenceReferencedTypesTreeNode>().FirstOrDefault();
			if (typesNode == null)
				continue;
			typesNode.EnsureLazyChildren();
			typesNode.IsExpanded = true;
			foreach (var typeRef in typesNode.Children.OfType<TypeReferenceTreeNode>())
			{
				typeRef.EnsureLazyChildren();
				typeRef.IsExpanded = true;
				memberRefNode = typeRef.Children.OfType<MemberReferenceTreeNode>().FirstOrDefault();
				if (memberRefNode != null)
					break;
			}
			if (memberRefNode != null)
				break;
		}

		// Assert — non-null, icon is one of the two ref glyphs, label is non-empty.
		((object?)memberRefNode).Should().NotBeNull(
			"at least one TypeRef under System.Linq's references must have member references");
		memberRefNode!.Icon.Should().BeOneOf(global::ILSpy.Images.Images.MethodReference,
			global::ILSpy.Images.Images.FieldReference);
		memberRefNode.Text.ToString().Should().NotBeNullOrWhiteSpace();
	}

	[AvaloniaTest]
	public async Task Exported_Type_Node_Uses_Export_Overlay_Icon()
	{
		// ExportedTypeTreeNode (type forwarders, e.g. mscorlib forwarding to
		// System.Private.CoreLib) renders with the dedicated ExportedType glyph.

		// Arrange — boot, wait for assemblies. Open mscorlib explicitly: it's the canonical
		// type-forwarder facade and we need a known forwarder regardless of default startup.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

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
		assemblyNode.IsExpanded = true;
		var refFolder = assemblyNode.Children.OfType<ReferenceFolderTreeNode>().Single();
		refFolder.EnsureLazyChildren();
		refFolder.IsExpanded = true;

		// Act — walk every reference's ReferencedTypes for an ExportedTypeTreeNode.
		ExportedTypeTreeNode? exportedNode = null;
		foreach (var refNode in refFolder.Children.OfType<AssemblyReferenceTreeNode>())
		{
			refNode.EnsureLazyChildren();
			refNode.IsExpanded = true;
			var typesNode = refNode.Children.OfType<AssemblyReferenceReferencedTypesTreeNode>().FirstOrDefault();
			if (typesNode == null)
				continue;
			typesNode.EnsureLazyChildren();
			typesNode.IsExpanded = true;
			exportedNode = typesNode.Children.OfType<ExportedTypeTreeNode>().FirstOrDefault();
			if (exportedNode != null)
				break;
		}

		// Assert — non-null, uses the ExportedType glyph, label is non-empty.
		((object?)exportedNode).Should().NotBeNull(
			"mscorlib must forward types to System.Private.CoreLib via ExportedType rows");
		exportedNode!.Icon.Should().BeSameAs(global::ILSpy.Images.Images.ExportedType);
		exportedNode.Text.ToString().Should().NotBeNullOrWhiteSpace();
	}

	[AvaloniaTest]
	public async Task Expanding_Assembly_Reference_Reveals_Transitive_References()
	{
		// AssemblyReferenceTreeNode shows the reference's *own* AssemblyRef rows as children
		// — building a transitive reference tree the user can drill into.

		// Arrange — boot, wait for assemblies, expand System.Linq → References.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		assemblyNode.EnsureLazyChildren();
		assemblyNode.IsExpanded = true;
		var refFolder = assemblyNode.Children.OfType<ReferenceFolderTreeNode>().Single();
		refFolder.EnsureLazyChildren();
		refFolder.IsExpanded = true;

		// Act — many BCL refs resolve to facade assemblies with zero transitive references;
		// iterate until we find one that has its own AssemblyRefs.
		bool foundTransitive = false;
		foreach (var refNode in refFolder.Children.OfType<AssemblyReferenceTreeNode>())
		{
			refNode.EnsureLazyChildren();
			refNode.IsExpanded = true;
			if (refNode.Children.OfType<AssemblyReferenceTreeNode>().Any())
			{
				foundTransitive = true;
				break;
			}
		}

		// Assert — at least one reference exposes its own AssemblyRefs.
		foundTransitive.Should().BeTrue("at least one reference must expose its own AssemblyRefs as child nodes");
	}

	[AvaloniaTest]
	public async Task Activating_Assembly_Reference_Selects_Resolved_Assembly_Node()
	{
		// Double-clicking (Activate) an AssemblyReferenceTreeNode should jump to the resolved
		// assembly's AssemblyTreeNode in the tree — auto-loading it if necessary.

		// Arrange — boot, wait for assemblies, expand System.Linq → References.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		assemblyNode.EnsureLazyChildren();
		assemblyNode.IsExpanded = true;
		var refFolder = assemblyNode.Children.OfType<ReferenceFolderTreeNode>().Single();
		refFolder.EnsureLazyChildren();
		refFolder.IsExpanded = true;

		var refNode = refFolder.Children.OfType<AssemblyReferenceTreeNode>()
			.Single(n => n.AssemblyReference.Name == "System.Runtime");

		// Act — fire ActivateItem with a stub event-args so we can assert handled.
		var args = new StubRoutedEventArgs();
		refNode.ActivateItem(args);

		// Assert — args.Handled flips and the selection lands on the resolved assembly node.
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
		// Decompiling a method whose body references types from currently-unloaded assemblies
		// must auto-load those assemblies (so the decompiler can resolve them) — and the
		// auto-loaded entries must be flagged IsAutoLoaded.

		// Arrange — boot, wait for assemblies, open System.Net.Http (its CancelPendingRequests
		// pulls in plenty of currently-unreferenced types).
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

		// Act — select CancelPendingRequests, wait for decompile, then for new (auto-loaded)
		// assemblies to appear.
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			loaded.ShortName, "System.Net.Http", "System.Net.Http.HttpClient");
		typeNode.EnsureLazyChildren();
		typeNode.IsExpanded = true;
		var methodNode = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "CancelPendingRequests");

		vm.AssemblyTreeModel.SelectNode(methodNode);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();

		await Waiters.WaitForAsync(() =>
			vm.AssemblyTreeModel.AssemblyList!.GetAssemblies().Any(a => !initialFiles.Contains(a.FileName)));

		// Assert — non-empty set of newly-loaded entries, all flagged IsAutoLoaded both on
		// LoadedAssembly and on the matching tree node.
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
		// Auto-loaded assemblies are an artefact of an in-flight decompile, not of the user's
		// curated list — saving the assembly list must omit them while keeping the ones the
		// user actually opened.

		// Arrange — boot, wait for assemblies, open + select a method that triggers auto-load.
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
		typeNode.IsExpanded = true;
		var methodNode = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "CancelPendingRequests");

		vm.AssemblyTreeModel.SelectNode(methodNode);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();

		await Waiters.WaitForAsync(() =>
			vm.AssemblyTreeModel.AssemblyList!.GetAssemblies().Any(a => a.IsAutoLoaded));

		// Act — save the assembly list to disk, then read the file back and tally the persisted
		// FileName entries.
		var settingsService = AppComposition.Current.GetExport<SettingsService>();
		var manager = settingsService.AssemblyListManager;
		manager.SaveList(vm.AssemblyTreeModel.AssemblyList!);

		var settingsPath = ICSharpCode.ILSpyX.Settings.ILSpySettings.SettingsFilePathProvider!();
		var doc = XDocument.Load(settingsPath);
		var savedFiles = doc.Descendants("Assembly")
			.Select(e => e.Value)
			.ToHashSet(System.StringComparer.OrdinalIgnoreCase);

		// Assert — every IsAutoLoaded assembly was omitted; every manual one is present.
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
		// OpenCommand must add the freshly-opened assembly to the AssemblyList, materialise a
		// matching tree node, and select that node so the user sees their newly-opened file.

		// Arrange — boot, wait for assemblies, snapshot the initial count.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var newAsmPath = typeof(System.Net.Http.HttpClient).Assembly.Location;
		var initialCount = vm.AssemblyTreeModel.AssemblyList!.Count;

		// Act — fire OpenCommand on a new path and wait for the load to complete.
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

		// Assert — count incremented by one; the corresponding tree node points at the
		// freshly-loaded assembly and is selected.
		vm.AssemblyTreeModel.AssemblyList!.Count.Should().Be(initialCount + 1);
		var node = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(loaded.ShortName);
		node.LoadedAssembly.Should().BeSameAs(loaded);
		node.IsSelected.Should().BeTrue();
	}

	[AvaloniaTest]
	public async Task User_Click_On_Visible_Row_Does_Not_Recentre_Viewport()
	{
		// CenterRowInView must skip the recentre when the clicked row is already fully visible.
		// Regression test: the original implementation used a clickInProgress flag that fired
		// during file picker await/resume, dragging the viewport on every click. The fix is an
		// in-viewport early-return.

		// Arrange — boot, wait for assemblies, expand a sub-tree large enough to scroll. Park
		// the viewport mid-list so clicks are at non-zero offset.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var enumerable = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		enumerable.EnsureLazyChildren();
		enumerable.IsExpanded = true;
		var ns = (NamespaceTreeNode)enumerable.Parent!;
		ns.EnsureLazyChildren();
		ns.IsExpanded = true;
		var asm = (AssemblyTreeNode)ns.Parent!;
		asm.IsExpanded = true;
		foreach (var child in ns.Children.OfType<TypeTreeNode>())
		{
			child.EnsureLazyChildren();
			child.IsExpanded = true;
		}

		var pane = await window.WaitForComponent<AssemblyListPane>();
		var grid = await pane.WaitForComponent<DataGrid>();

		vm.AssemblyTreeModel.SelectNode(enumerable);
		await Waiters.WaitForAsync(() => ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, enumerable));
		for (int i = 0; i < 8; i++)
		{
			Dispatcher.UIThread.RunJobs();
			await Task.Delay(25);
		}
		grid.UpdateLayout();

		var scrollViewer = await grid.WaitForComponent<ScrollViewer>();
		(scrollViewer.Extent.Height - scrollViewer.Viewport.Height).Should().BeGreaterThan(50,
			"the grid must have something to scroll for this test to be meaningful");
		scrollViewer.Offset = new Vector(scrollViewer.Offset.X, 50);
		grid.UpdateLayout();
		Dispatcher.UIThread.RunJobs();
		scrollViewer.Offset.Y.Should().BeGreaterThan(5,
			"the test scenario requires the viewport be parked mid-list");

		// Pick the bottom-most visible non-selected row — that's the strictest probe. If the
		// bug regressed, CenterRowInView would scroll it up to the middle and offset would
		// change.
		var candidateRow = grid.GetVisualDescendants().OfType<DataGridRow>()
			.Where(r => !r.IsSelected
				&& r.TranslatePoint(new Point(0, 0), scrollViewer) is { } p
				&& p.Y >= 0 && p.Y + r.Bounds.Height <= scrollViewer.Viewport.Height)
			.OrderByDescending(r => r.TranslatePoint(new Point(0, 0), scrollViewer)!.Value.Y)
			.FirstOrDefault();
		candidateRow.Should().NotBeNull("the test needs a visible non-selected row to click");

		var offsetBefore = scrollViewer.Offset.Y;

		// Act — real pointer click. Setting SelectedItem programmatically would fire DataGrid's
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

		// Assert — viewport offset is unchanged (within 1px tolerance for layout jitter).
		scrollViewer.Offset.Y.Should().BeApproximately(offsetBefore, 1.0,
			"clicking an already-visible row must not move the viewport");
	}

	[AvaloniaTest]
	public async Task Save_Code_Command_Dispatches_Single_Selected_Node_Save_Override()
	{
		// SaveCommand on a single ILSpyTreeNode selection must call the node's own Save()
		// method — that's how nodes (e.g. resources) opt into a custom save dialog/format.

		// Arrange — boot, wait for assemblies, plant a probe node that records when Save runs.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var probe = new SaveProbeNode();
		vm.AssemblyTreeModel.SelectNode(probe);

		// Act — fire the Save Code main-menu command.
		var registry = AppComposition.Current.GetExport<MainMenuCommandRegistry>();
		var saveCmd = registry.Commands
			.Single(c => c.Metadata.Header == nameof(Resources._SaveCode))
			.CreateExport().Value;
		saveCmd.Execute(null);

		// Assert — probe's Save() ran.
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
		// When SaveCommand falls through (no node-specific Save override), it must run a full
		// decompilation (DecompilationOptions.FullDecompilation = true) of the selection and
		// write the resulting C# to the user-picked path.

		// Arrange — boot, wait for assemblies, select a method, wait for the in-tab decompile.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.EnsureLazyChildren();
		typeNode.IsExpanded = true;
		var method = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "AsEnumerable");
		vm.AssemblyTreeModel.SelectNode(method);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();

		// Act — invoke SaveCodeAsync with a temp path (bypassing the file picker so the test
		// is deterministic).
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

			// Assert — file exists; contents include the method name and a body fragment that
			// only appears with FullDecompilation = true.
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
		// Pressing Del on a selected AssemblyTreeNode must unload that assembly from the
		// AssemblyList while leaving siblings intact.

		// Arrange — boot, wait for assemblies, pick the assembly to evict + a sibling we
		// expect to survive, select the sacrificial one, focus the grid.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var sacrificialName = typeof(System.Linq.Enumerable).Assembly.GetName().Name!;
		var survivorName = typeof(object).Assembly.GetName().Name!;
		var sacrificialNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(sacrificialName);
		vm.AssemblyTreeModel.SelectNode(sacrificialNode);
		await Waiters.WaitForAsync(() => ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, sacrificialNode));

		var pane = await window.WaitForComponent<AssemblyListPane>();
		var grid = await pane.WaitForComponent<DataGrid>();
		grid.Focus();
		Dispatcher.UIThread.RunJobs();

		int itemsBefore = (grid.ItemsSource as System.Collections.ICollection)?.Count ?? -1;

		// Act — headless keyboard event simulating the user pressing Del while the tree has
		// focus.
		global::Avalonia.Headless.HeadlessWindowExtensions.KeyPress(
			window,
			global::Avalonia.Input.Key.Delete,
			global::Avalonia.Input.RawInputModifiers.None,
			global::Avalonia.Input.PhysicalKey.Delete,
			null);

		// Assert — the sacrificial assembly is gone from the data list AND the grid's bound
		// ItemsSource drops by one. Both halves matter: a passing AssemblyList assertion alone
		// would mask regressions where the data updates but the grid stays bound to a stale
		// snapshot (which is exactly what happened when the BindTree filter materialised the
		// children into a List instead of forwarding the live ObservableCollection).
		await Waiters.WaitForAsync(() =>
			!vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
				.Any(a => string.Equals(a.ShortName, sacrificialName, System.StringComparison.Ordinal)));

		vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
			.Should().Contain(a => a.ShortName == survivorName,
				"only the selected assembly should be removed");

		await Waiters.WaitForAsync(
			() => (grid.ItemsSource as System.Collections.ICollection)?.Count < itemsBefore,
			description: "DataGrid item count should drop after the assembly is unloaded");
	}

	[AvaloniaTest]
	public async Task Clear_Assembly_List_Command_Empties_The_Active_List()
	{
		// The Clear Assembly List menu command must drop every entry from the active list.

		// Arrange — boot, wait for assemblies, baseline the non-empty count.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);
		vm.AssemblyTreeModel.AssemblyList!.Count.Should().BeGreaterThan(0);

		// Act — fire the Clear command.
		var registry = AppComposition.Current.GetExport<MainMenuCommandRegistry>();
		var clearCmd = registry.Commands
			.Single(c => c.Metadata.Header == nameof(Resources.ClearAssemblyList))
			.CreateExport().Value;
		clearCmd.CanExecute(null).Should().BeTrue("non-empty list must enable Clear");
		clearCmd.Execute(null);

		// Assert — the list goes to zero entries.
		await Waiters.WaitForAsync(() => vm.AssemblyTreeModel.AssemblyList!.Count == 0);
		vm.AssemblyTreeModel.AssemblyList!.Count.Should().Be(0);
	}

	[AvaloniaTest]
	public async Task Remove_Assemblies_With_Load_Errors_Drops_Only_Broken_Entries()
	{
		// The "Remove Assemblies with Load Errors" command must drop every entry whose load
		// failed — and *only* those, leaving valid assemblies in place.

		// Arrange — boot, wait for assemblies, plant a "broken" entry: a real file that isn't
		// a PE binary. AssemblyList records it but the load fails — that's the state the
		// command targets.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

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

			// Act — fire the command.
			var registry = AppComposition.Current.GetExport<MainMenuCommandRegistry>();
			var cmd = registry.Commands
				.Single(c => c.Metadata.Header == nameof(Resources._RemoveAssembliesWithLoadErrors))
				.CreateExport().Value;
			cmd.CanExecute(null).Should().BeTrue("a broken entry must enable the command");
			cmd.Execute(null);

			await Waiters.WaitForAsync(() =>
				!vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
					.Any(a => string.Equals(a.FileName, brokenPath, System.StringComparison.OrdinalIgnoreCase)));

			// Assert — broken path gone; every previously-valid path still present.
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

	[AvaloniaTest]
	public async Task Setting_SharpTreeNode_IsExpanded_Expands_Row_In_Grid()
	{
		// ProDataGrid's HierarchicalOptions.IsExpandedPropertyPath wires SharpTreeNode.IsExpanded
		// to the grid's wrapper expansion state via reflection + INotifyPropertyChanged. Setting
		// IsExpanded on the model must propagate to the grid wrapper without any manual
		// subscription bookkeeping.

		// Arrange — boot, wait for assemblies, locate the realised DataGrid and the
		// HierarchicalModel wrapper for the assembly node.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var pane = await window.WaitForComponent<AssemblyListPane>();
		var grid = await pane.WaitForComponent<DataGrid>();

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		var hm = (global::Avalonia.Controls.DataGridHierarchical.IHierarchicalModel)grid.HierarchicalModel!;
		var hNode = hm.FindNode(assemblyNode);
		hNode.Should().NotBeNull();
		hNode!.IsExpanded.Should().BeFalse("baseline: top-level assembly rows start collapsed");

		// Act — production-shaped action: just toggle the model property.
		assemblyNode.EnsureLazyChildren();
		assemblyNode.IsExpanded = true;

		// Drain the dispatcher so the wiring (PropertyChanged → hm.Expand) settles.
		for (int i = 0; i < 5; i++)
		{
			Dispatcher.UIThread.RunJobs();
			await Task.Delay(20);
		}

		// Assert — the grid wrapper now reports IsExpanded == true.
		hm.FindNode(assemblyNode)!.IsExpanded.Should().BeTrue(
			"setting SharpTreeNode.IsExpanded must propagate to the HierarchicalModel wrapper");
	}

	[AvaloniaTest]
	public async Task Pane_OpenNodeInNewTab_Spawns_A_Fresh_Decompiler_Tab_And_Selection_Follows_The_New_Active_Tab()
	{
		// MMB on a tree row maps to AssemblyListPane.OpenNodeInNewTab — a new decompiler
		// tab opens with the supplied node decompiled, the existing tab keeps its content,
		// and the assembly-tree selection is pulled across to the new tab's source node
		// (the active tab and the tree are kept in lockstep).
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.IsExpanded = true;
		var pinned = typeNode.Children.OfType<MethodTreeNode>()
			.Single(m => m.MethodDefinition.Name == "AsEnumerable");
		var newTabTarget = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Empty");
		vm.AssemblyTreeModel.SelectNode(pinned);
		var firstTab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		await Waiters.WaitForAsync(() => window.GetVisualDescendants().OfType<AssemblyListPane>().Any());
		var pane = await window.WaitForComponent<AssemblyListPane>();

		var documents = ((ILSpyDockFactory)vm.DockWorkspace.Factory).Documents!;
		var initialCount = documents.VisibleDockables?.Count ?? 0;

		pane.OpenNodeInNewTab(newTabTarget);

		await Waiters.WaitForAsync(
			() => (documents.VisibleDockables?.Count ?? 0) > initialCount);
		var newTab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		ReferenceEquals(newTab, firstTab).Should().BeFalse(
			"a fresh decompiler tab must be created instead of reusing the existing one");
		newTab.Text.Should().Contain("Empty");
		firstTab.Text.Should().Contain("AsEnumerable");
		// Selection has moved to the new tab's source node — the active tab and the
		// assembly-tree selection stay in lockstep.
		ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, newTabTarget).Should().BeTrue(
			"the assembly-tree selection must follow the newly-active tab");
	}

	[AvaloniaTest]
	public async Task Switching_Tabs_Pulls_Tree_Selection_To_The_Active_Tabs_Source_Node()
	{
		// Activate two tabs (each carrying a different SourceNode), then flip the active
		// dockable back to the first one and confirm the assembly-tree selection follows.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.IsExpanded = true;
		var first = typeNode.Children.OfType<MethodTreeNode>()
			.Single(m => m.MethodDefinition.Name == "AsEnumerable");
		var second = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Empty");

		// First tab: tree-selection produces the MainTab content.
		vm.AssemblyTreeModel.SelectNode(first);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		var documents = ((ILSpyDockFactory)vm.DockWorkspace.Factory).Documents!;
		var firstTab = documents.ActiveDockable;

		// Second tab: MMB-style carve-out — selection follows the new active tab.
		vm.DockWorkspace.OpenNodeInNewTab(second);
		await Waiters.WaitForAsync(() =>
			ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, second));

		// Re-activate the first tab — selection must swing back.
		vm.DockWorkspace.Factory.SetActiveDockable(firstTab!);

		await Waiters.WaitForAsync(() =>
			ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, first));
		ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, first).Should().BeTrue(
			"clicking back to the previous tab must pull the tree selection with it");
	}

	[AvaloniaTest]
	public async Task OpenNodeInNewTab_Spawns_A_Second_Metadata_Tab_When_Node_Has_Custom_Content()
	{
		// Tree nodes that override CreateTab (metadata tables, resource viewers, …) must be
		// hosted as their custom page-type in the new tab — not forcibly wrapped in a
		// DecompilerTabPageModel. Exercises the regression behind "there can't be multiple
		// metadata views": before the OpenNodeInNewTab consolidation, the new-tab path
		// always built a decompiler tab and the metadata page-type couldn't be carved out.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);

		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(coreLibName);
		assemblyNode.EnsureLazyChildren();
		var metadataNode = assemblyNode.Children.OfType<MetadataTreeNode>().Single();
		metadataNode.EnsureLazyChildren();
		var tablesNode = metadataNode.Children.OfType<MetadataTablesTreeNode>().Single();
		tablesNode.EnsureLazyChildren();
		var typeDefNode = tablesNode.Children.OfType<TypeDefTableTreeNode>().Single();

		// First metadata view via tree-selection (active tab gets MetadataTablePageModel content).
		vm.AssemblyTreeModel.SelectNode(typeDefNode);
		var firstMeta = await vm.DockWorkspace.WaitForMetadataTabAsync();

		var documents = ((ILSpyDockFactory)vm.DockWorkspace.Factory).Documents!;
		var initialCount = documents.VisibleDockables?.Count ?? 0;

		// Second metadata view via the new-tab carve-out — pick a different table so the
		// content is unmistakable.
		var methodTableNode = tablesNode.Children
			.OfType<global::ILSpy.Metadata.CorTables.MethodTableTreeNode>().Single();
		vm.DockWorkspace.OpenNodeInNewTab(methodTableNode);

		await Waiters.WaitForAsync(
			() => (documents.VisibleDockables?.Count ?? 0) > initialCount);
		var secondMeta = await vm.DockWorkspace.WaitForMetadataTabAsync();
		ReferenceEquals(secondMeta, firstMeta).Should().BeFalse(
			"a fresh metadata tab must be created — the previous one keeps its TypeDef state");
	}

	[AvaloniaTest]
	public async Task Property_Tree_Node_Has_Getter_And_Setter_Method_Children()
	{
		// Properties expose their accessors as MethodTreeNode children, mirroring WPF
		// ILSpy. Without this, tree-based navigation can't reach get_X / set_X — and
		// MMB on a property-accessor row in the metadata grid silently no-ops because
		// FindTreeNode can't resolve the IMethod to a tree node.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);

		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var stringTypeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			coreLibName, "System", "System.String");
		stringTypeNode.IsExpanded = true;

		// String.Length is read-only — a property with a getter but no setter.
		var lengthProp = stringTypeNode.Children.OfType<PropertyTreeNode>()
			.First(p => p.PropertyDefinition.Name == "Length");
		lengthProp.IsExpanded = true;

		var accessors = lengthProp.Children.OfType<MethodTreeNode>().ToList();
		accessors.Should().NotBeEmpty(
			"a property tree node must surface its accessors as MethodTreeNode children");
		accessors.Should().Contain(m => m.MethodDefinition.Name == "get_Length",
			"String.Length must expose its getter");
	}

	[AvaloniaTest]
	public async Task Event_Tree_Node_Has_Add_And_Remove_Method_Children()
	{
		// Events surface add_X / remove_X (and invoke_X if present) as MethodTreeNode
		// children, mirroring WPF ILSpy. Same regression-protection as the property
		// case above.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);

		var coreLibName = typeof(object).Assembly.GetName().Name!;
		// AppDomain.AssemblyLoad is a public, well-known event in CoreLib.
		var appDomainNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			coreLibName, "System", "System.AppDomain");
		appDomainNode.IsExpanded = true;
		var loadEvent = appDomainNode.Children.OfType<EventTreeNode>()
			.First(e => e.EventDefinition.Name == "AssemblyLoad");
		loadEvent.IsExpanded = true;

		var accessors = loadEvent.Children.OfType<MethodTreeNode>().ToList();
		accessors.Should().Contain(m => m.MethodDefinition.Name == "add_AssemblyLoad",
			"event tree node must expose its add accessor");
		accessors.Should().Contain(m => m.MethodDefinition.Name == "remove_AssemblyLoad",
			"event tree node must expose its remove accessor");
	}

	[AvaloniaTest]
	public async Task Property_Accessors_Are_Filtered_Out_Of_The_Tree_Under_Default_Settings()
	{
		// Accessors live in Children for navigation (FindTreeNode resolves to them) but
		// the per-node Filter must hide them under the default ShowApiLevel — they're
		// rendered as part of the parent property in the C# surface, so the tree should
		// not duplicate them as standalone rows. ShowAll is the explicit opt-in.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);

		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var stringTypeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			coreLibName, "System", "System.String");
		stringTypeNode.IsExpanded = true;
		var lengthProp = stringTypeNode.Children.OfType<PropertyTreeNode>()
			.First(p => p.PropertyDefinition.Name == "Length");
		var getter = lengthProp.Children.OfType<MethodTreeNode>()
			.Single(m => m.MethodDefinition.Name == "get_Length");

		var settings = AppComposition.Current.GetExport<SettingsService>()
			.SessionSettings.LanguageSettings;
		settings.ShowApiLevel = global::ICSharpCode.ILSpyX.ApiVisibility.PublicOnly;
		((int)getter.Filter(settings)).Should().Be((int)global::ILSpy.TreeNodes.FilterResult.Hidden,
			"property accessors must be hidden by default — only ShowAll surfaces them");

		settings.ShowApiLevel = global::ICSharpCode.ILSpyX.ApiVisibility.All;
		((int)getter.Filter(settings)).Should().NotBe((int)global::ILSpy.TreeNodes.FilterResult.Hidden,
			"flipping to ShowAll must let accessors through");
	}

	[AvaloniaTest]
	public async Task FindTreeNode_Resolves_Property_Accessor_To_Its_MethodTreeNode_Child()
	{
		// MMB on an accessor row in the MethodDef metadata grid resolves the row's token to
		// an IMethod whose AccessorOwner is the IProperty. Without the AccessorOwner-aware
		// branch in FindMemberNode, the lookup falls through to "MethodTreeNode child of the
		// type" and returns null because accessors live under the property, not the type.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);

		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var stringTypeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			coreLibName, "System", "System.String");
		stringTypeNode.IsExpanded = true;
		var lengthProp = stringTypeNode.Children.OfType<PropertyTreeNode>()
			.First(p => p.PropertyDefinition.Name == "Length");
		lengthProp.IsExpanded = true;
		var getter = lengthProp.PropertyDefinition.Getter!;

		var resolved = vm.AssemblyTreeModel.FindTreeNode(getter);
		((object?)resolved).Should().BeOfType<MethodTreeNode>(
			"FindTreeNode must reach into the property to surface its accessor");
		((MethodTreeNode)resolved!).MethodDefinition.MetadataToken
			.Should().Be(getter.MetadataToken);
	}

	[AvaloniaTest]
	public async Task Type_Tree_Node_Exposes_BaseTypes_Subtree_Listing_Object_For_System_Exception()
	{
		// Each type node should surface a "Base Types" sub-tree listing its base classes /
		// interfaces, mirroring the WPF tree. System.Exception extends only System.Object so we
		// expect a single BaseTypesEntryNode whose label ends in "Object".

		// Arrange — boot, wait for assemblies, drill into System.Exception.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			coreLibName, "System", "System.Exception");
		typeNode.IsExpanded = true;

		// Assert — BaseTypes child exists, expanding it yields entries, Object is among them.
		var baseTypes = typeNode.Children.OfType<BaseTypesTreeNode>().Single();
		baseTypes.IsExpanded = true;
		baseTypes.Children.OfType<BaseTypesEntryNode>().Should().NotBeEmpty();
		baseTypes.Children.OfType<BaseTypesEntryNode>()
			.Should().Contain(e => e.Text.ToString()!.EndsWith("Object"),
				"System.Exception's base-type chain must include Object");
	}

	[AvaloniaTest]
	public async Task BaseTypesEntryNode_Activate_Navigates_To_The_Base_Type()
	{
		// Activating a BaseTypesEntryNode jumps the assembly-tree selection to the underlying
		// type node — same gesture the WPF tree uses to walk an inheritance chain.

		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			coreLibName, "System", "System.Exception");
		typeNode.IsExpanded = true;
		var baseTypes = typeNode.Children.OfType<BaseTypesTreeNode>().Single();
		baseTypes.IsExpanded = true;
		var objectEntry = baseTypes.Children.OfType<BaseTypesEntryNode>()
			.First(e => e.Text.ToString()!.EndsWith("Object"));

		// Act — activate via the stub routed-args (mirrors a double-click).
		var args = new StubRoutedEventArgs();
		objectEntry.ActivateItem(args);

		// Assert — selection moved off System.Exception and onto a TypeTreeNode for Object.
		// Cast through object to bypass SharpTreeNodeAssertions' restricted member set.
		((object?)vm.AssemblyTreeModel.SelectedItem).Should().BeOfType<TypeTreeNode>();
		((object?)vm.AssemblyTreeModel.SelectedItem).Should().NotBeSameAs(typeNode);
		((TypeTreeNode)vm.AssemblyTreeModel.SelectedItem!).Text.ToString()
			.Should().Be("Object");
	}

	[AvaloniaTest]
	public async Task Type_Tree_Node_Exposes_DerivedTypes_Subtree_For_Non_Sealed_Class()
	{
		// Non-sealed types get a "Derived Types" sub-tree listing classes that extend them. We
		// scan the assembly list synchronously — for the small fixture loaded in tests this
		// yields a few hits per common base (e.g. SystemException is derived from Exception).

		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			coreLibName, "System", "System.Exception");
		typeNode.IsExpanded = true;

		var derived = typeNode.Children.OfType<DerivedTypesTreeNode>().Single();
		derived.IsExpanded = true;
		derived.Children.OfType<DerivedTypesEntryNode>().Should().NotBeEmpty(
			"the loaded assembly list contains several Exception subclasses (e.g. SystemException, ArgumentException)");
	}

	[AvaloniaTest]
	public async Task Sealed_Class_Has_No_DerivedTypes_Node()
	{
		// Sealed types can't be derived from, so the DerivedTypes sub-tree must not appear.

		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var stringNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			coreLibName, "System", "System.String");
		stringNode.IsExpanded = true;

		stringNode.Children.OfType<DerivedTypesTreeNode>()
			.Should().BeEmpty("System.String is sealed");
	}

	[AvaloniaTest]
	public async Task Object_Has_No_BaseTypes_Node()
	{
		// System.Object has no base types, so the BaseTypes sub-tree must not appear.

		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var objectNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			coreLibName, "System", "System.Object");
		objectNode.IsExpanded = true;

		objectNode.Children.OfType<BaseTypesTreeNode>()
			.Should().BeEmpty("System.Object has no base types");
	}
}
