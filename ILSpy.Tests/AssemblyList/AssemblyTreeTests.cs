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
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Metadata;
using ICSharpCode.ILSpy.Metadata.CorTables;
using ICSharpCode.ILSpy.Search;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;

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
		var (_, vm) = await TestHarness.BootAsync(3);

		// Act — pick CoreLib (always ships localised error-message tables), expand it.
		var assemblyNode = vm.AssemblyTreeModel.FindCoreLib();
		assemblyNode.Expand();

		// Assert — exactly one Resources folder, named, non-empty, every child an ILSpyTreeNode.
		var resources = assemblyNode.Children.OfType<ResourceListTreeNode>().Single();
		resources.Text.Should().Be(Resources._Resources);
		resources.Expand();
		resources.Children.Should().NotBeEmpty();
		resources.Children.Should().AllBeAssignableTo<ILSpyTreeNode>();
	}

	[AvaloniaTest]
	public async Task DataGrid_Has_Extended_Selection_Mode()
	{
		// The assembly tree DataGrid must run in Extended selection mode so users can
		// Ctrl-click to multi-select rows.

		// Arrange — boot, wait for assemblies + AssemblyListPane to materialise.
		var (window, vm) = await TestHarness.BootAsync(3);

		// Act — locate the realised DataGrid inside the AssemblyListPane.
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var treeGrid = await pane.WaitForComponent<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView>();

		// Assert — SelectionMode is Extended.
		treeGrid.SelectionMode.Should().Be(global::Avalonia.Controls.SelectionMode.Multiple,
			"the assembly tree must let users Ctrl-click multiple rows");
	}

	[AvaloniaTest]
	public async Task Multi_Selection_Tracks_Multiple_Nodes_And_Marks_Them_IsSelected()
	{
		// Adding multiple nodes to AssemblyTreeModel.SelectedItems must reflect on each node's
		// IsSelected flag (used by the tree view template binding) and removing one node must
		// flip its IsSelected back without affecting siblings.

		// Arrange — boot, wait for assemblies, expand Enumerable, grab two methods.
		var (_, vm) = await TestHarness.BootAsync(3);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.Expand();
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
		var (_, vm) = await TestHarness.BootAsync(3);

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
		var loaded = await vm.OpenAssemblyAsync(zipPath);

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(loaded.ShortName);
		assemblyNode.Expand();

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
		var (_, vm) = await TestHarness.BootAsync(3);

		var assemblyNode = vm.AssemblyTreeModel.FindCoreLib();
		assemblyNode.Expand();
		var resources = assemblyNode.Children.OfType<ResourceListTreeNode>().Single();
		resources.Expand();

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
		var (_, vm) = await TestHarness.BootAsync(3);

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		assemblyNode.Expand();

		// Act — find the References folder.
		var refFolder = assemblyNode.Children.OfType<ReferenceFolderTreeNode>().Single();

		// Assert — folder is named, expands to non-empty AssemblyReferenceTreeNode children.
		refFolder.Text.Should().Be(Resources.References);

		refFolder.Expand();
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
		var (_, vm) = await TestHarness.BootAsync(3);

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		var refFolder = assemblyNode.GetChild<ReferenceFolderTreeNode>();
		refFolder.EnsureLazyChildren();

		var refNode = refFolder.Children.OfType<AssemblyReferenceTreeNode>()
			.First(n => n.AssemblyReference.Name == "System.Runtime");

		// Act 1 + Assert 1 — first access triggers the loading state.
		var initialIcon = refNode.Icon;
		initialIcon.Should().BeSameAs(ICSharpCode.ILSpy.Images.AssemblyLoading);

		// Act 2 — wait for resolver to finish.
		await Waiters.WaitForAsync(() =>
			!ReferenceEquals(refNode.Icon, ICSharpCode.ILSpy.Images.AssemblyLoading));

		// Assert 2 — icon settles on the resolved Assembly glyph.
		refNode.Icon.Should().BeSameAs(ICSharpCode.ILSpy.Images.Assembly);
	}

	[AvaloniaTest]
	public async Task Assembly_Reference_Has_Referenced_Types_Subnode()
	{
		// Each AssemblyReferenceTreeNode contains a "Referenced Types" subnode that lists the
		// TypeRef rows the host assembly imports from that reference.

		// Arrange — boot, wait for assemblies, expand System.Linq → References → System.Runtime.
		var (_, vm) = await TestHarness.BootAsync(3);

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		assemblyNode.Expand();
		var refFolder = assemblyNode.Children.OfType<ReferenceFolderTreeNode>().Single();
		refFolder.Expand();

		var refNode = refFolder.Children.OfType<AssemblyReferenceTreeNode>()
			.First(n => n.AssemblyReference.Name == "System.Runtime");
		refNode.Expand();

		// Act — find the ReferencedTypes subnode and expand it.
		var typesNode = refNode.Children.OfType<AssemblyReferenceReferencedTypesTreeNode>().Single();
		typesNode.Expand();

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
		var (_, vm) = await TestHarness.BootAsync(3);

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		assemblyNode.Expand();

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
		var (_, vm) = await TestHarness.BootAsync(3);

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		assemblyNode.Expand();

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
		var (_, vm) = await TestHarness.BootAsync(3);

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		assemblyNode.Expand();
		var refFolder = assemblyNode.Children.OfType<ReferenceFolderTreeNode>().Single();
		refFolder.Expand();

		// Act — walk every reference's ReferencedTypes subtree until we find a TypeRef with
		// at least one MemberReference. System.Linq imports plenty (delegates, Func ctors)
		// so this should always succeed.
		MemberReferenceTreeNode? memberRefNode = null;
		foreach (var refNode in refFolder.Children.OfType<AssemblyReferenceTreeNode>())
		{
			refNode.Expand();
			var typesNode = refNode.Children.OfType<AssemblyReferenceReferencedTypesTreeNode>().FirstOrDefault();
			if (typesNode == null)
				continue;
			typesNode.Expand();
			foreach (var typeRef in typesNode.Children.OfType<TypeReferenceTreeNode>())
			{
				typeRef.Expand();
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
		memberRefNode!.Icon.Should().BeOneOf(ICSharpCode.ILSpy.Images.MethodReference,
			ICSharpCode.ILSpy.Images.FieldReference);
		memberRefNode.Text.ToString().Should().NotBeNullOrWhiteSpace();
	}

	[AvaloniaTest]
	public async Task Exported_Type_Node_Uses_Export_Overlay_Icon()
	{
		// ExportedTypeTreeNode (type forwarders, e.g. mscorlib forwarding to
		// System.Private.CoreLib) renders with the dedicated ExportedType glyph.

		// Arrange — boot, wait for assemblies. Open mscorlib explicitly: it's the canonical
		// type-forwarder facade and we need a known forwarder regardless of default startup.
		var (_, vm) = await TestHarness.BootAsync(3);

		var coreLibDir = System.IO.Path.GetDirectoryName(typeof(object).Assembly.Location)!;
		var mscorlibPath = System.IO.Path.Combine(coreLibDir, "mscorlib.dll");
		if (!System.IO.File.Exists(mscorlibPath))
		{
			Assert.Inconclusive($"mscorlib.dll not present next to System.Private.CoreLib at {coreLibDir} — runtime layout differs.");
			return;
		}
		var loaded = await vm.OpenAssemblyAsync(mscorlibPath);

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(loaded.ShortName);
		assemblyNode.Expand();
		var refFolder = assemblyNode.Children.OfType<ReferenceFolderTreeNode>().Single();
		refFolder.Expand();

		// Act — walk every reference's ReferencedTypes for an ExportedTypeTreeNode.
		ExportedTypeTreeNode? exportedNode = null;
		foreach (var refNode in refFolder.Children.OfType<AssemblyReferenceTreeNode>())
		{
			refNode.Expand();
			var typesNode = refNode.Children.OfType<AssemblyReferenceReferencedTypesTreeNode>().FirstOrDefault();
			if (typesNode == null)
				continue;
			typesNode.Expand();
			exportedNode = typesNode.Children.OfType<ExportedTypeTreeNode>().FirstOrDefault();
			if (exportedNode != null)
				break;
		}

		// Assert — non-null, uses the ExportedType glyph, label is non-empty.
		((object?)exportedNode).Should().NotBeNull(
			"mscorlib must forward types to System.Private.CoreLib via ExportedType rows");
		exportedNode!.Icon.Should().BeSameAs(ICSharpCode.ILSpy.Images.ExportedType);
		exportedNode.Text.ToString().Should().NotBeNullOrWhiteSpace();
	}

	[AvaloniaTest]
	public async Task Expanding_Assembly_Reference_Reveals_Transitive_References()
	{
		// AssemblyReferenceTreeNode shows the reference's *own* AssemblyRef rows as children
		// — building a transitive reference tree the user can drill into.

		// Arrange — boot, wait for assemblies, expand System.Linq → References.
		var (_, vm) = await TestHarness.BootAsync(3);

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		assemblyNode.Expand();
		var refFolder = assemblyNode.Children.OfType<ReferenceFolderTreeNode>().Single();
		refFolder.Expand();

		// Act — many BCL refs resolve to facade assemblies with zero transitive references;
		// iterate until we find one that has its own AssemblyRefs.
		bool foundTransitive = false;
		foreach (var refNode in refFolder.Children.OfType<AssemblyReferenceTreeNode>())
		{
			refNode.Expand();
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
		var (_, vm) = await TestHarness.BootAsync(3);

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		assemblyNode.Expand();
		var refFolder = assemblyNode.Children.OfType<ReferenceFolderTreeNode>().Single();
		refFolder.Expand();

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
		var (_, vm) = await TestHarness.BootAsync(3);

		var newAsmPath = typeof(System.Net.Http.HttpClient).Assembly.Location;
		var loaded = await vm.OpenAssemblyAsync(newAsmPath);

		var initialFiles = vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
			.Select(a => a.FileName)
			.ToHashSet(System.StringComparer.OrdinalIgnoreCase);

		// Act — select CancelPendingRequests, wait for decompile, then for new (auto-loaded)
		// assemblies to appear.
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			loaded.ShortName, "System.Net.Http", "System.Net.Http.HttpClient");
		typeNode.Expand();
		var methodNode = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "CancelPendingRequests");

		vm.AssemblyTreeModel.SelectNode(methodNode);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		TestCapture.Step("method-decompiled");

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
		var (_, vm) = await TestHarness.BootAsync(3);

		var newAsmPath = typeof(System.Net.Http.HttpClient).Assembly.Location;
		var loaded = await vm.OpenAssemblyAsync(newAsmPath);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			loaded.ShortName, "System.Net.Http", "System.Net.Http.HttpClient");
		typeNode.Expand();
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
		var (_, vm) = await TestHarness.BootAsync(3);

		var newAsmPath = typeof(System.Net.Http.HttpClient).Assembly.Location;
		var initialCount = vm.AssemblyTreeModel.AssemblyList!.Count;

		// Act — fire OpenCommand on a new path and wait for the load to complete.
		var loaded = await vm.OpenAssemblyAsync(newAsmPath);

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
		var (window, vm) = await TestHarness.BootAsync(3);

		var enumerable = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		enumerable.Expand();
		var ns = (NamespaceTreeNode)enumerable.Parent!;
		ns.Expand();
		var asm = (AssemblyTreeNode)ns.Parent!;
		asm.IsExpanded = true;
		foreach (var child in ns.Children.OfType<TypeTreeNode>())
		{
			child.Expand();
		}

		var pane = await window.WaitForComponent<AssemblyListPane>();
		var grid = await pane.WaitForComponent<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView>();

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
		TestCapture.Step("viewport-parked-mid-list");

		// Pick the bottom-most visible non-selected row — that's the strictest probe. If the
		// bug regressed, CenterRowInView would scroll it up to the middle and offset would
		// change.
		var candidateRow = grid.GetVisualDescendants().OfType<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeViewItem>()
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
		TestCapture.Step("visible-row-clicked");

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
	public async Task Selecting_A_Visible_Row_Via_The_Model_Does_Not_Recentre_Viewport()
	{
		// "Decompile to new tab" (and any model-driven navigation) selects the node in the tree, which
		// syncs through TreeSelectionBinder -> ScrollIntoNodeView -> CenterNodeInView. Unlike a real
		// mouse click, that path DOES run the centring code, so the in-viewport early-return must also
		// cover it -- otherwise opening an already-visible item in a new tab yanks the tree to the centre.

		var (window, vm) = await TestHarness.BootAsync(3);

		var enumerable = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		enumerable.Expand();
		var ns = (NamespaceTreeNode)enumerable.Parent!;
		ns.Expand();
		var asm = (AssemblyTreeNode)ns.Parent!;
		asm.IsExpanded = true;
		foreach (var child in ns.Children.OfType<TypeTreeNode>())
		{
			child.Expand();
		}

		var pane = await window.WaitForComponent<AssemblyListPane>();
		var grid = await pane.WaitForComponent<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView>();

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

		// Pick the bottom-most fully-visible non-selected row -- the strictest probe for an unwanted recentre.
		var candidateRow = grid.GetVisualDescendants().OfType<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeViewItem>()
			.Where(r => !r.IsSelected
				&& r.TranslatePoint(new Point(0, 0), scrollViewer) is { } p
				&& p.Y >= 0 && p.Y + r.Bounds.Height <= scrollViewer.Viewport.Height)
			.OrderByDescending(r => r.TranslatePoint(new Point(0, 0), scrollViewer)!.Value.Y)
			.FirstOrDefault();
		candidateRow.Should().NotBeNull("the test needs a visible non-selected row to select");
		var candidateNode = candidateRow!.Node;
		((object?)candidateNode).Should().NotBeNull();

		var offsetBefore = scrollViewer.Offset.Y;

		// Act — model-driven selection (the open-in-new-tab path), NOT a mouse click.
		vm.AssemblyTreeModel.SelectNode(candidateNode);
		for (int i = 0; i < 8; i++)
		{
			Dispatcher.UIThread.RunJobs();
			await Task.Delay(25);
		}

		scrollViewer.Offset.Y.Should().BeApproximately(offsetBefore, 1.0,
			"selecting an already-visible row via the model (e.g. Decompile to new tab) must not move the viewport");
	}

	[AvaloniaTest]
	public async Task Expanding_A_Node_Does_Not_Scroll_The_Selection_Back_Into_View()
	{
		// Rule: when the user mutates the tree directly -- here, expanding an unrelated node -- the
		// app must not chase the selection; the viewport follows the user's action, not the selected
		// row. Regression: the ListBox's AutoScrollToSelectedItem yanked the (now off-screen)
		// selection back into view on every expand, fighting the expand's own reveal ("weird
		// scrolling"). Cross-control navigation still reveals (that path goes through the model).

		var (window, vm) = await TestHarness.BootAsync(3);

		// Tall tree: fully expand System.Linq so the list is taller than the viewport.
		var enumerable = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		enumerable.Expand();
		var ns = (NamespaceTreeNode)enumerable.Parent!;
		ns.Expand();
		((AssemblyTreeNode)ns.Parent!).IsExpanded = true;
		foreach (var t in ns.Children.OfType<TypeTreeNode>())
			t.Expand();

		var pane = await window.WaitForComponent<AssemblyListPane>();
		var grid = await pane.WaitForComponent<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView>();
		var scrollViewer = await grid.WaitForComponent<ScrollViewer>();

		// Select + reveal the type, then let it settle on screen.
		vm.AssemblyTreeModel.SelectNode(enumerable);
		await Waiters.WaitForAsync(() => ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, enumerable));
		for (int i = 0; i < 8; i++)
		{
			Dispatcher.UIThread.RunJobs();
			await Task.Delay(25);
		}
		grid.UpdateLayout();

		(scrollViewer.Extent.Height - scrollViewer.Viewport.Height).Should().BeGreaterThan(50,
			"the tree must be taller than the viewport for this test to be meaningful");
		grid.IsNodeFullyVisible(enumerable).Should().BeTrue("the selected type is revealed before the expand");

		// Act: the user expands an unrelated, earlier node (CoreLib, at the top), inserting many
		// rows above the selection and pushing it off-screen.
		var coreLib = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(typeof(object).Assembly.GetName().Name!);
		coreLib.IsExpanded = true;
		for (int i = 0; i < 8; i++)
		{
			Dispatcher.UIThread.RunJobs();
			await Task.Delay(25);
		}
		grid.UpdateLayout();

		// Assert: the app did not chase the selection. The expand reveals the opened node's children;
		// the off-screen selection stays off-screen.
		grid.IsNodeFullyVisible(enumerable).Should().BeFalse(
			"expanding a node the user opened must not auto-scroll the off-screen selection back into view");
	}

	[AvaloniaTest]
	public async Task Save_Code_Command_Dispatches_Single_Selected_Node_Save_Override()
	{
		// SaveCommand on a single ILSpyTreeNode selection must call the node's own Save()
		// method — that's how nodes (e.g. resources) opt into a custom save dialog/format.

		// Arrange — boot, wait for assemblies, plant a probe node that records when Save runs.
		var (_, vm) = await TestHarness.BootAsync(3);

		var probe = new SaveProbeNode();
		vm.AssemblyTreeModel.SelectNode(probe);

		// Act — fire the Save Code main-menu command.
		var saveCmd = AppComposition.Current.GetExport<MainMenuCommandRegistry>().GetCommand(nameof(Resources._SaveCode));
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
		var (_, vm) = await TestHarness.BootAsync(3);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.Expand();
		var method = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "AsEnumerable");
		vm.AssemblyTreeModel.SelectNode(method);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		TestCapture.Step("asenumerable-decompiled");

		// Act — invoke SaveCodeAsync with a temp path (bypassing the file picker so the test
		// is deterministic).
		var registry = AppComposition.Current.GetExport<MainMenuCommandRegistry>();
		var saveCmd = (ICSharpCode.ILSpy.Commands.SaveCommand)registry.Commands
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
		var (window, vm) = await TestHarness.BootAsync(3);

		var sacrificialName = typeof(System.Linq.Enumerable).Assembly.GetName().Name!;
		var survivorName = typeof(object).Assembly.GetName().Name!;
		var sacrificialNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(sacrificialName);
		vm.AssemblyTreeModel.SelectNode(sacrificialNode);
		await Waiters.WaitForAsync(() => ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, sacrificialNode));
		TestCapture.Step("sacrificial-selected");

		var pane = await window.WaitForComponent<AssemblyListPane>();
		var grid = await pane.WaitForComponent<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView>();
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
		TestCapture.Step("delete-key-pressed");

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
	public async Task Delete_Reselects_The_Next_Node_So_Repeated_Delete_Keeps_Working()
	{
		// Regression for the "spamming Delete breaks" bug: Unload cleared the model selection but
		// the grid kept showing a row selected, so model.SelectedItems went empty and the next
		// Delete read an empty selection and no-op'd. Delete must re-select the nearest remaining
		// node (grid + model in sync) so repeated Delete keeps unloading.
		var (window, vm) = await TestHarness.BootAsync(3);
		var model = vm.AssemblyTreeModel;

		var firstAssembly = model.FindNode<AssemblyTreeNode>("System.Linq");
		model.SelectNode(firstAssembly);
		await Waiters.WaitForAsync(() => ReferenceEquals(model.SelectedItem, firstAssembly));

		var pane = await window.WaitForComponent<AssemblyListPane>();
		var grid = await pane.WaitForComponent<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView>();
		grid.Focus();
		Dispatcher.UIThread.RunJobs();

		int loadedBefore = model.AssemblyList!.GetAssemblies().Count();

		window.KeyPress(Key.Delete, RawInputModifiers.None, PhysicalKey.Delete, null);
		await Waiters.WaitForAsync(() => model.AssemblyList!.GetAssemblies().Count() == loadedBefore - 1);
		Dispatcher.UIThread.RunJobs();

		// The bug: after the first Delete the selection must still point at a (different) node.
		model.SelectedItems.Should().NotBeEmpty(
			"after Delete the selection must move to the nearest remaining node, not unset");
		((object?)model.SelectedItem).Should().NotBeNull();
		ReferenceEquals(model.SelectedItem, firstAssembly).Should().BeFalse(
			"the deleted node must not stay selected");

		// Spamming Delete: a second press must unload another assembly (it couldn't before).
		window.KeyPress(Key.Delete, RawInputModifiers.None, PhysicalKey.Delete, null);
		await Waiters.WaitForAsync(() => model.AssemblyList!.GetAssemblies().Count() == loadedBefore - 2,
			description: "a second Delete must unload another assembly; the selection didn't desync");
	}

	[AvaloniaTest]
	public async Task Left_And_Right_Keys_Collapse_Expand_And_Navigate_The_Tree()
	{
		// Standard tree keyboard nav: Right expands a collapsed node then steps into its first
		// child; Left collapses an expanded node, or moves to the parent when already collapsed.
		var (window, vm) = await TestHarness.BootAsync(3);
		var model = vm.AssemblyTreeModel;
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var grid = await pane.WaitForComponent<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView>();
		grid.Focus();
		Dispatcher.UIThread.RunJobs();

		var assembly = model.FindNode<AssemblyTreeNode>("System.Linq");
		assembly.EnsureLazyChildren();
		model.SelectNode(assembly);
		await Waiters.WaitForAsync(() => ReferenceEquals(model.SelectedItem, assembly));
		assembly.IsExpanded.Should().BeFalse("precondition: the assembly starts collapsed");

		// Right expands.
		window.KeyPress(Key.Right, RawInputModifiers.None, PhysicalKey.ArrowRight, null);
		await Waiters.WaitForAsync(() => assembly.IsExpanded, description: "Right expands the collapsed node");

		// Right again steps into the first child.
		window.KeyPress(Key.Right, RawInputModifiers.None, PhysicalKey.ArrowRight, null);
		await Waiters.WaitForAsync(
			() => model.SelectedItem is { } s && !ReferenceEquals(s, assembly) && ReferenceEquals(s.Parent, assembly),
			description: "Right on an expanded node selects its first child");

		// Left on the (collapsed/leaf) child moves selection back to the parent.
		window.KeyPress(Key.Left, RawInputModifiers.None, PhysicalKey.ArrowLeft, null);
		await Waiters.WaitForAsync(() => ReferenceEquals(model.SelectedItem, assembly),
			description: "Left on a collapsed child selects the parent");

		// Left on the expanded parent collapses it.
		window.KeyPress(Key.Left, RawInputModifiers.None, PhysicalKey.ArrowLeft, null);
		await Waiters.WaitForAsync(() => !assembly.IsExpanded, description: "Left collapses the expanded node");
	}

	[AvaloniaTest]
	public async Task Numpad_Add_Subtract_Multiply_Expand_And_Collapse_The_Node()
	{
		// Numpad + expands, - collapses, * expands (recursively where the node allows it).
		var (window, vm) = await TestHarness.BootAsync(3);
		var model = vm.AssemblyTreeModel;
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var grid = await pane.WaitForComponent<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView>();
		grid.Focus();
		Dispatcher.UIThread.RunJobs();

		var assembly = model.FindNode<AssemblyTreeNode>("System.Linq");
		assembly.EnsureLazyChildren();
		model.SelectNode(assembly);
		await Waiters.WaitForAsync(() => ReferenceEquals(model.SelectedItem, assembly));
		assembly.IsExpanded.Should().BeFalse("precondition: starts collapsed");

		window.KeyPress(Key.Add, RawInputModifiers.None, PhysicalKey.NumPadAdd, null);
		await Waiters.WaitForAsync(() => assembly.IsExpanded, description: "Numpad + expands");

		window.KeyPress(Key.Subtract, RawInputModifiers.None, PhysicalKey.NumPadSubtract, null);
		await Waiters.WaitForAsync(() => !assembly.IsExpanded, description: "Numpad - collapses");

		window.KeyPress(Key.Multiply, RawInputModifiers.None, PhysicalKey.NumPadMultiply, null);
		await Waiters.WaitForAsync(() => assembly.IsExpanded, description: "Numpad * expands");
	}

	[AvaloniaTest]
	public async Task Shift_Up_After_Shift_Down_Shrinks_The_Selection_Toward_The_Anchor()
	{
		// Anchor at A, Shift+Down twice -> A,B,C. Shift+Up must SHRINK back to A,B (not keep C).
		var (window, vm) = await TestHarness.BootAsync(3);
		var model = vm.AssemblyTreeModel;
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var grid = await pane.WaitForComponent<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView>();
		grid.Focus();
		Dispatcher.UIThread.RunJobs();

		var flat = (System.Collections.IList)grid.ItemsSource!;
		var a = (SharpTreeNode)flat[0]!;
		var b = (SharpTreeNode)flat[1]!;
		var c = (SharpTreeNode)flat[2]!;

		model.SelectNode(a);
		await Waiters.WaitForAsync(() => ReferenceEquals(model.SelectedItem, a));

		window.KeyPress(Key.Down, RawInputModifiers.Shift, PhysicalKey.ArrowDown, null);
		Dispatcher.UIThread.RunJobs();
		window.KeyPress(Key.Down, RawInputModifiers.Shift, PhysicalKey.ArrowDown, null);
		Dispatcher.UIThread.RunJobs();
		model.SelectedItems.Should().BeEquivalentTo(new[] { a, b, c }, "precondition: A,B,C selected");

		window.KeyPress(Key.Up, RawInputModifiers.Shift, PhysicalKey.ArrowUp, null);
		Dispatcher.UIThread.RunJobs();
		model.SelectedItems.Should().BeEquivalentTo(new[] { a, b },
			"Shift+Up must shrink the range back toward the anchor (A,B), dropping C");
	}

	[AvaloniaTest]
	public async Task Shift_Down_Then_Shift_Up_In_The_Middle_Shrinks_Without_Drifting_The_Anchor()
	{
		// Start mid-list, Shift+Down x3 then Shift+Up x2. The anchor must stay put, so the net is
		// the [anchor..anchor+1] range -- not a range that drifted up past the anchor.
		var (window, vm) = await TestHarness.BootAsync(3);
		var model = vm.AssemblyTreeModel;
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var grid = await pane.WaitForComponent<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView>();
		grid.Focus();
		Dispatcher.UIThread.RunJobs();

		var flat = (System.Collections.IList)grid.ItemsSource!;
		// Expand the first assembly so there are plenty of rows and a real "middle".
		var firstAsm = (SharpTreeNode)flat[0]!;
		firstAsm.EnsureLazyChildren();
		firstAsm.IsExpanded = true;
		Dispatcher.UIThread.RunJobs();
		await Waiters.WaitForAsync(() => flat.Count >= 7);

		var start = (SharpTreeNode)flat[3]!;
		var below = (SharpTreeNode)flat[4]!;
		model.SelectNode(start);
		await Waiters.WaitForAsync(() => ReferenceEquals(model.SelectedItem, start));

		for (int d = 0; d < 3; d++)
		{
			window.KeyPress(Key.Down, RawInputModifiers.Shift, PhysicalKey.ArrowDown, null);
			Dispatcher.UIThread.RunJobs();
		}
		for (int u = 0; u < 2; u++)
		{
			window.KeyPress(Key.Up, RawInputModifiers.Shift, PhysicalKey.ArrowUp, null);
			Dispatcher.UIThread.RunJobs();
		}

		model.SelectedItems.Should().BeEquivalentTo(new[] { start, below },
			"net of +3/-2 from a fixed anchor is the anchor plus one row below it");
	}

	[AvaloniaTest]
	public async Task Type_Ahead_Jumps_To_The_Node_Matching_The_Typed_Text()
	{
		// Typing a name jumps the selection to the visible node whose text matches.
		var (window, vm) = await TestHarness.BootAsync(3);
		var model = vm.AssemblyTreeModel;
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var grid = await pane.WaitForComponent<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView>();
		grid.Focus();
		Dispatcher.UIThread.RunJobs();

		var target = model.FindNode<AssemblyTreeNode>("System.Linq");
		var targetText = target.Text?.ToString()!;
		// Start the selection on a different assembly so the jump is observable.
		var other = model.FindNode<AssemblyTreeNode>(typeof(object).Assembly.GetName().Name!);
		model.SelectNode(other);
		await Waiters.WaitForAsync(() => ReferenceEquals(model.SelectedItem, other));

		foreach (char ch in targetText)
		{
			grid.RaiseEvent(new global::Avalonia.Input.TextInputEventArgs {
				RoutedEvent = global::Avalonia.Input.InputElement.TextInputEvent,
				Text = ch.ToString(),
				Source = grid,
			});
		}
		Dispatcher.UIThread.RunJobs();

		await Waiters.WaitForAsync(
			() => (model.SelectedItem as SharpTreeNode)?.Text?.ToString() == targetText,
			description: "type-ahead must select the node whose text matches what was typed");
	}

	[AvaloniaTest]
	public async Task Clear_Assembly_List_Command_Empties_The_Active_List()
	{
		// The Clear Assembly List menu command must drop every entry from the active list.

		// Arrange — boot, wait for assemblies, baseline the non-empty count.
		var (_, vm) = await TestHarness.BootAsync(3);
		vm.AssemblyTreeModel.AssemblyList!.Count.Should().BeGreaterThan(0);

		// Act — fire the Clear command.
		var clearCmd = AppComposition.Current.GetExport<MainMenuCommandRegistry>().GetCommand(nameof(Resources.ClearAssemblyList));
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
		var (_, vm) = await TestHarness.BootAsync(3);

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
			var cmd = AppComposition.Current.GetExport<MainMenuCommandRegistry>().GetCommand(nameof(Resources._RemoveAssembliesWithLoadErrors));
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
	public async Task Setting_SharpTreeNode_IsExpanded_Reveals_Children_In_The_Tree()
	{
		// SharpTreeView binds the TreeFlattener directly, so toggling SharpTreeNode.IsExpanded
		// reveals the node's children in the flattened row list with no wrapper bookkeeping.
		var (window, vm) = await TestHarness.BootAsync(3);

		var pane = await window.WaitForComponent<AssemblyListPane>();
		var grid = await pane.WaitForComponent<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView>();

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		assemblyNode.IsExpanded.Should().BeFalse("baseline: top-level assembly rows start collapsed");
		var flat = (System.Collections.IList)grid.ItemsSource!;
		int before = flat.Count;

		// Act — production-shaped action: just toggle the model property.
		assemblyNode.Expand();
		TestCapture.Step("assembly-row-expanded");

		// Assert — the node's children now appear in the flattened tree.
		await Waiters.WaitForAsync(() => flat.Count > before,
			description: "setting SharpTreeNode.IsExpanded must reveal its children in the flattened tree");
	}

	[AvaloniaTest]
	public async Task Pane_OpenNodeInNewTab_Spawns_A_Fresh_Decompiler_Tab_And_Selection_Follows_The_New_Active_Tab()
	{
		// MMB on a tree row maps to AssemblyListPane.OpenNodeInNewTab — a new decompiler
		// tab opens with the supplied node decompiled, the existing tab keeps its content,
		// and the assembly-tree selection is pulled across to the new tab's source node
		// (the active tab and the tree are kept in lockstep).
		var (window, vm) = await TestHarness.BootAsync(3);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.IsExpanded = true;
		var pinned = typeNode.Children.OfType<MethodTreeNode>()
			.Single(m => m.MethodDefinition.Name == "AsEnumerable");
		var newTabTarget = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Empty");
		vm.AssemblyTreeModel.SelectNode(pinned);
		var firstTab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		TestCapture.Step("asenumerable-in-first-tab");

		await Waiters.WaitForAsync(() => window.GetVisualDescendants().OfType<AssemblyListPane>().Any());
		var pane = await window.WaitForComponent<AssemblyListPane>();

		var documents = ((ILSpyDockFactory)vm.DockWorkspace.Factory).Documents!;
		var initialCount = documents.VisibleDockables?.Count ?? 0;

		pane.OpenNodeInNewTab(newTabTarget);

		await Waiters.WaitForAsync(
			() => (documents.VisibleDockables?.Count ?? 0) > initialCount);
		var newTab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		TestCapture.Step("empty-spawned-in-new-tab");
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
		var (_, vm) = await TestHarness.BootAsync(3);

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
		TestCapture.Step("second-tab-active");

		// Re-activate the first tab — selection must swing back.
		vm.DockWorkspace.Factory.SetActiveDockable(firstTab!);

		await Waiters.WaitForAsync(() =>
			ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, first));
		TestCapture.Step("switched-back-to-first-tab");
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
		var (_, vm) = await TestHarness.BootAsync();

		var tablesNode = vm.AssemblyTreeModel.FindCoreLib()
			.GetChild<MetadataTreeNode>()
			.GetChild<MetadataTablesTreeNode>();
		var typeDefNode = tablesNode.GetChild<TypeDefTableTreeNode>();

		// First metadata view via tree-selection (active tab gets MetadataTablePageModel content).
		vm.AssemblyTreeModel.SelectNode(typeDefNode);
		var firstMeta = await vm.DockWorkspace.WaitForMetadataTabAsync();
		TestCapture.Step("typedef-metadata-tab");

		var documents = ((ILSpyDockFactory)vm.DockWorkspace.Factory).Documents!;
		var initialCount = documents.VisibleDockables?.Count ?? 0;

		// Second metadata view via the new-tab carve-out — pick a different table so the
		// content is unmistakable.
		var methodTableNode = tablesNode.Children
			.OfType<ICSharpCode.ILSpy.Metadata.CorTables.MethodTableTreeNode>().Single();
		vm.DockWorkspace.OpenNodeInNewTab(methodTableNode);

		await Waiters.WaitForAsync(
			() => (documents.VisibleDockables?.Count ?? 0) > initialCount);
		var secondMeta = await vm.DockWorkspace.WaitForMetadataTabAsync();
		TestCapture.Step("method-table-second-metadata-tab");
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
		var (_, vm) = await TestHarness.BootAsync();

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
		var (_, vm) = await TestHarness.BootAsync();

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
		var (_, vm) = await TestHarness.BootAsync();

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
		settings.ShowApiLevel = ICSharpCode.ILSpyX.ApiVisibility.PublicOnly;
		((int)getter.Filter(settings)).Should().Be((int)ICSharpCode.ILSpy.TreeNodes.FilterResult.Hidden,
			"property accessors must be hidden by default — only ShowAll surfaces them");

		settings.ShowApiLevel = ICSharpCode.ILSpyX.ApiVisibility.All;
		((int)getter.Filter(settings)).Should().NotBe((int)ICSharpCode.ILSpy.TreeNodes.FilterResult.Hidden,
			"flipping to ShowAll must let accessors through");
	}

	[AvaloniaTest]
	public async Task FindTreeNode_Resolves_Property_Accessor_To_Its_MethodTreeNode_Child()
	{
		// MMB on an accessor row in the MethodDef metadata grid resolves the row's token to
		// an IMethod whose AccessorOwner is the IProperty. Without the AccessorOwner-aware
		// branch in FindMemberNode, the lookup falls through to "MethodTreeNode child of the
		// type" and returns null because accessors live under the property, not the type.
		var (_, vm) = await TestHarness.BootAsync();

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
		var (_, vm) = await TestHarness.BootAsync(3);

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

		var (_, vm) = await TestHarness.BootAsync(3);

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

		var (_, vm) = await TestHarness.BootAsync(3);

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

		var (_, vm) = await TestHarness.BootAsync(3);

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

		var (_, vm) = await TestHarness.BootAsync(3);

		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var objectNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			coreLibName, "System", "System.Object");
		objectNode.IsExpanded = true;

		objectNode.Children.OfType<BaseTypesTreeNode>()
			.Should().BeEmpty("System.Object has no base types");
	}

	[AvaloniaTest]
	public void ExitCommand_Is_Exported_To_File_Menu_With_Resources_E_xit_Header()
	{
		// File → Exit must be MEF-discovered and parented to the File menu at MenuOrder=99999
		// (last entry, mirrors WPF). Headless app lifetime isn't IClassicDesktopStyleApplicationLifetime,
		// so Execute() is a safe no-op under tests — we don't actually shut down the test runner,
		// but the metadata + CanExecute path is the regression-worthy surface.

		var registry = AppComposition.Current.GetExport<MainMenuCommandRegistry>();
		var entry = registry.Commands
			.SingleOrDefault(c => c.Metadata.Header == nameof(Resources.E_xit));
		((object?)entry).Should().NotBeNull("File → Exit must be exported via [ExportMainMenuCommand]");

		entry!.Metadata.ParentMenuID.Should().Be(nameof(Resources._File),
			"Exit lives under the File menu");
		entry.Metadata.MenuCategory.Should().Be(nameof(Resources.Exit),
			"Exit's MenuCategory pins it to its own separator group");
		entry.Metadata.MenuOrder.Should().Be(99999,
			"Exit must be the last entry in the File menu (matches WPF MenuOrder)");

		// Resolve the command, confirm it can execute (SimpleCommand default), and that calling
		// Execute under the headless lifetime doesn't blow up — the cast to
		// IClassicDesktopStyleApplicationLifetime returns null in headless and the Shutdown call
		// is a safe no-op.
		var command = entry.CreateExport().Value;
		command.CanExecute(null).Should().BeTrue();
		command.Invoking(c => c.Execute(null)).Should().NotThrow(
			"Execute under headless must be a no-op rather than crash");
	}

	[AvaloniaTest]
	public async Task Active_Search_Term_Does_Not_Hide_Member_Tree_Nodes()
	{
		// Pre-existing port misstep: commit 45461ddde wired the search-pane's term into
		// LanguageSettings.SearchTerm and made SearchTermMatches gate visibility on it.
		// WPF intentionally makes SearchTermMatches a no-op (returns true) so the assembly
		// tree stays independent of the search pane. After fixing parity, FieldTreeNode.Filter
		// must NOT return Hidden purely because the field's name doesn't contain the active
		// SearchTerm — only ShowApiLevel + ShowMember remain valid hiding criteria.

		var (_, vm) = await TestHarness.BootAsync();

		var settings = AppComposition.Current.GetExport<SettingsService>()
			.SessionSettings.LanguageSettings;
		settings.SearchTerm = "ZZZ_DefinitelyNotAnEnumName";

		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var dayOfWeek = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			coreLibName, "System", "System.DayOfWeek");
		Assert.That(dayOfWeek, Is.Not.Null, "System.DayOfWeek must be discoverable in CoreLib");

		dayOfWeek!.EnsureLazyChildren();
		var monday = dayOfWeek.Children.OfType<FieldTreeNode>()
			.FirstOrDefault(f => f.FieldDefinition.Name == "Monday");
		Assert.That(monday, Is.Not.Null,
			"Monday must be present as a FieldTreeNode child of System.DayOfWeek");

		// Direct Filter() invocation bypasses the IsVisible gate that the implicit cascade
		// honours, so the assertion exercises the policy independent of view-tree state.
		monday!.Filter(settings).Should().NotBe(FilterResult.Hidden,
			"an active search term must not hide member tree nodes — WPF's SearchTermMatches is a no-op");
	}

	[AvaloniaTest]
	public async Task Global_Namespace_Types_Appear_Under_Dash_Node()
	{
		// Every PE module carries a <Module> pseudo-type in the global (empty) namespace.
		// To match the long-standing tree shape, those types must live under a
		// NamespaceTreeNode whose Name is the empty string and whose displayed Text is "-",
		// NOT as bare TypeTreeNodes hanging directly off the assembly node.

		// Arrange — boot, wait for assemblies, locate CoreLib.
		var (_, vm) = await TestHarness.BootAsync(3);

		var assemblyNode = vm.AssemblyTreeModel.FindCoreLib();
		assemblyNode.EnsureLazyChildren();

		// Assert — exactly one global-namespace node, labelled "-", non-empty contents.
		// Plain null check rather than .Should().NotBeNull() because the AwesomeAssertions
		// SharpTreeNode-targeted extension hijacks Should() on NamespaceTreeNode? and lacks
		// NotBeNull.
		var globalNs = assemblyNode.Children.OfType<NamespaceTreeNode>()
			.SingleOrDefault(n => n.Name.Length == 0);
		Assert.That(globalNs, Is.Not.Null,
			"types in the global namespace must be grouped under a NamespaceTreeNode with an empty Name");
		globalNs!.Text.Should().Be("-",
			"NamespaceTreeNode renders the empty namespace as '-' to match the long-standing tree shape");

		// And no bare TypeTreeNodes should hang directly off the assembly — every type must
		// route through one of the namespace nodes.
		assemblyNode.Children.OfType<TypeTreeNode>().Should().BeEmpty(
			"global-namespace types must live under the '-' node, not as direct assembly children");

		globalNs.EnsureLazyChildren();
		globalNs.Children.OfType<TypeTreeNode>().Should().NotBeEmpty(
			"<Module> (and any other global types) belong under the '-' node");
	}

	[AvaloniaTest]
	public async Task CtrlA_Selects_Every_Top_Level_Row_On_The_First_Press()
	{
		// Ctrl+A in the assembly tree must select all rows on the FIRST press, not just move
		// the current cell to the last row (the user reported having to press it twice).

		// Arrange — boot, wait for assemblies, realise the grid.
		var (window, vm) = await TestHarness.BootAsync(3);

		var pane = await window.WaitForComponent<AssemblyListPane>();
		var grid = await pane.WaitForComponent<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView>();
		grid.UpdateLayout();

		var topLevelCount = vm.AssemblyTreeModel.Root!.Children.Count;
		topLevelCount.Should().BeGreaterThan(1, "the test needs several top-level rows to be meaningful");

		// Give the grid keyboard focus WITHOUT clicking a cell first — this is the state the
		// tree is in right after the user tabs/clicks into the pane but before establishing a
		// current cell. The DataGrid only handles Ctrl+A when a focused element lives inside it.
		grid.Focus();
		Dispatcher.UIThread.RunJobs();

		// First press.
		window.KeyPress(Key.A, RawInputModifiers.Control, PhysicalKey.A, keySymbol: "a");
		Dispatcher.UIThread.RunJobs();
		await Task.Delay(50);
		Dispatcher.UIThread.RunJobs();
		// What the USER sees is the grid's selection. A feedback loop through the model's
		// singular SelectedItem can collapse the grid back to one row even while the model's
		// SelectedItems still holds all of them — so assert on the grid, not the model.
		int gridAfterFirst = grid.SelectedItems!.Count;
		int modelAfterFirst = vm.AssemblyTreeModel.SelectedItems.Count;
		TestCapture.Step("after-first-ctrl-a");

		// Second press.
		window.KeyPress(Key.A, RawInputModifiers.Control, PhysicalKey.A, keySymbol: "a");
		Dispatcher.UIThread.RunJobs();
		await Task.Delay(50);
		Dispatcher.UIThread.RunJobs();
		int gridAfterSecond = grid.SelectedItems!.Count;

		// Assert — every top-level row is selected in the grid after the FIRST press.
		gridAfterFirst.Should().Be(topLevelCount,
			$"Ctrl+A must select all {topLevelCount} assembly rows on the first press "
			+ $"(grid after 1st={gridAfterFirst}, model after 1st={modelAfterFirst}, grid after 2nd={gridAfterSecond})");
	}

	[AvaloniaTest]
	public async Task Plain_Click_On_A_Row_Reduces_A_Multi_Selection_To_That_Row()
	{
		// With several rows selected (e.g. after Ctrl+A), a plain left-click (no modifier) on
		// one row must collapse the selection down to just that row.

		// Arrange — boot, wait for assemblies, realise the grid, select everything.
		var (window, vm) = await TestHarness.BootAsync(3);

		var pane = await window.WaitForComponent<AssemblyListPane>();
		var grid = await pane.WaitForComponent<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView>();
		grid.UpdateLayout();

		var topLevelCount = vm.AssemblyTreeModel.Root!.Children.Count;
		topLevelCount.Should().BeGreaterThan(1, "the test needs several top-level rows to be meaningful");

		grid.Focus();
		Dispatcher.UIThread.RunJobs();
		window.KeyPress(Key.A, RawInputModifiers.Control, PhysicalKey.A, keySymbol: "a");
		Dispatcher.UIThread.RunJobs();
		await Waiters.WaitForAsync(() => grid.SelectedItems!.Count == topLevelCount);
		TestCapture.Step("all-rows-selected");

		// Act — plain left-click on the second visible row.
		var targetRow = grid.GetVisualDescendants().OfType<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeViewItem>()
			.OrderBy(r => r.TranslatePoint(new Point(0, 0), grid)?.Y ?? double.MaxValue)
			.Skip(1).First();
		var targetNode = targetRow.DataContext as SharpTreeNode;
		Assert.That(targetNode, Is.Not.Null, "the clicked row must wrap a tree node");
		// Tree rows stretch to content width (with horizontal scroll), so a row can be wider than
		// the grid viewport. Click within the visible viewport, not at the (off-screen) row centre.
		var clickX = System.Math.Min(targetRow.Bounds.Width, grid.Bounds.Width) / 2;
		var rowCentre = targetRow.TranslatePoint(
			new Point(clickX, targetRow.Bounds.Height / 2), window)!.Value;
		HeadlessWindowExtensions.MouseDown(window, rowCentre, MouseButton.Left);
		HeadlessWindowExtensions.MouseUp(window, rowCentre, MouseButton.Left);
		Dispatcher.UIThread.RunJobs();
		await Task.Delay(50);
		Dispatcher.UIThread.RunJobs();
		TestCapture.Step("plain-click-collapsed-selection");

		// Assert — selection collapsed to exactly the clicked row, in both grid and model.
		grid.SelectedItems!.Count.Should().Be(1,
			$"a plain click must reduce the selection to one row (grid has {grid.SelectedItems!.Count})");
		var modelSelection = vm.AssemblyTreeModel.SelectedItems;
		modelSelection.Count.Should().Be(1, "the model selection must collapse to the clicked node too");
		ReferenceEquals(modelSelection[0], targetNode).Should().BeTrue(
			"the surviving selection must be the row the user clicked");
	}

	[AvaloniaTest]
	public async Task Load_Dependencies_Resolves_References_And_Keeps_Them_In_The_List()
	{
		// "Load Dependencies" must resolve every reference of the selected assembly and leave
		// the resolved (auto-loaded) assemblies in the list. Regression: the command used to
		// finish with a full list Refresh (F5), which rebuilds the list from persisted state and
		// silently drops the just-resolved auto-loaded dependencies -- so the command appeared
		// to do nothing.

		// Arrange -- boot, open System.Net.Http (it references many assemblies not in the list).
		var (_, vm) = await TestHarness.BootAsync(3);

		var newAsmPath = typeof(System.Net.Http.HttpClient).Assembly.Location;
		var openCommand = AppComposition.Current.GetExport<MainMenuCommandRegistry>().GetCommand(nameof(Resources._Open));
		openCommand.Execute(newAsmPath);
		await Waiters.WaitForAsync(() =>
			vm.AssemblyTreeModel.AssemblyList!.GetAssemblies().Any(a =>
				string.Equals(a.FileName, newAsmPath, System.StringComparison.OrdinalIgnoreCase)));

		var httpNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(
			System.IO.Path.GetFileNameWithoutExtension(newAsmPath));
		await httpNode.LoadedAssembly.GetLoadResultAsync();

		var before = vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
			.Select(a => a.FileName)
			.ToHashSet(System.StringComparer.OrdinalIgnoreCase);

		// Act -- run Load Dependencies on the System.Net.Http node.
		await vm.AssemblyTreeModel.LoadDependenciesAsync(new SharpTreeNode[] { httpNode });
		for (int i = 0; i < 8; i++)
		{
			Dispatcher.UIThread.RunJobs();
			await Task.Delay(25);
		}

		// Assert -- references were resolved AND survive in the list as auto-loaded entries.
		var added = vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
			.Where(a => !before.Contains(a.FileName))
			.ToList();
		added.Should().NotBeEmpty(
			"Load Dependencies must resolve referenced assemblies and keep them in the list");
		added.Should().OnlyContain(a => a.IsAutoLoaded,
			"freshly resolved dependencies are auto-loaded");
	}
}
