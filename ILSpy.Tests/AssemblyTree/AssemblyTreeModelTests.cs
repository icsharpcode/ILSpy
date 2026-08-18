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

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpyX;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.AssemblyTree;

// AssemblyTreeModel is the MEF-shared root of the assembly tree. Initialize() loads
// (or creates) the default assembly list from settings, optionally bootstraps it with
// the three .NET runtime assemblies, and wires Root to an AssemblyListTreeNode. If
// either AssemblyList or Root stays null, the entire pane below is blank — which is
// hard to debug from a UI snapshot, so we catch it at the model layer here.
[TestFixture]
public class AssemblyTreeModelTests
{
	[AvaloniaTest]
	public void Initialize_populates_AssemblyList_and_Root()
	{
		var model = AppComposition.Current.GetExport<AssemblyTreeModel>();
		model.Should().NotBeNull("AssemblyTreeModel is [Export][Shared] in ICSharpCode.ILSpy.AssemblyTree.");

		model.Initialize();

		model.AssemblyList.Should().NotBeNull("Initialize loads or creates the default AssemblyList.");
		// Cast through object so the generic AwesomeAssertions Should() resolves, not the
		// TreeNodeAssertionsExtensions.Should(SharpTreeNode) shadow that landed in this commit
		// (its TreeNodeAssertions surface doesn't expose NotBeNull).
		((object?)model.Root).Should().NotBeNull("Initialize wires Root to an AssemblyListTreeNode of the loaded list.");
	}

	[AvaloniaTest]
	public void Initialize_populates_AssemblyLists_and_selects_the_default()
	{
		var model = AppComposition.Current.GetExport<AssemblyTreeModel>();
		model.Initialize();

		model.AssemblyLists.Should().NotBeEmpty(
			"Initialize mirrors AssemblyListManager.AssemblyLists into the toolbar combo's source.");
		model.ActiveListName.Should().Be(AssemblyListManager.DefaultListName,
			"Initialize selects the (Default) list so the tree has something to render at startup.");
	}

	static readonly string TempRoot = Path.Combine(Path.GetTempPath(), "ILSpy.Tests.Packages", Guid.NewGuid().ToString("N"));

	// Two assemblies in sibling folder chains, so a walk that reaches only one of them fails.
	static string CreatePackage()
	{
		var dir = Directory.CreateDirectory(Path.Combine(TempRoot, Guid.NewGuid().ToString("N"))).FullName;
		var zipPath = Path.Combine(dir, "package.zip");
		using var zip = ZipFile.Open(zipPath, ZipArchiveMode.Create);
		zip.CreateEntryFromFile(FixtureAssembly.Emit("Nested"), "lib/net10.0/Nested.dll");
		zip.CreateEntryFromFile(FixtureAssembly.Emit("Sibling"), "runtimes/win-x64/Sibling.dll");
		return zipPath;
	}

	[OneTimeTearDown]
	public void DeleteTempPackages()
	{
		if (Directory.Exists(TempRoot))
			Directory.Delete(TempRoot, recursive: true);
	}

	[AvaloniaTest]
	public async Task EnumerateAllAssemblies_expands_a_package_into_its_entries()
	{
		// The search corpus is this walk, so an assembly it does not yield is an assembly no
		// search can ever match.
		var (_, vm) = await TestHarness.BootAsync();
		await vm.OpenAssemblyAsync(CreatePackage());

		var nested = new List<LoadedAssembly>();
		await foreach (var asm in vm.AssemblyTreeModel.AssemblyList!.EnumerateAllAssemblies())
		{
			if (asm.ParentBundle != null)
				nested.Add(asm);
		}

		nested.Select(a => a.FileName).Should().BeEquivalentTo(
			new[] { "lib/net10.0/Nested.dll", "runtimes/win-x64/Sibling.dll" },
			"every .dll in the package is searchable, and the package-relative path is what "
			+ "distinguishes copies of one assembly built for several targets.");
	}

	[AvaloniaTest]
	public async Task EnumerateAllAssemblies_stops_expanding_a_package_once_cancelled()
	{
		// Both search panes restart on every keystroke, so an abandoned walk that keeps
		// extracting package entries competes with the run the user is waiting for.
		var (_, vm) = await TestHarness.BootAsync();
		await vm.OpenAssemblyAsync(CreatePackage());

		using var cts = new CancellationTokenSource();
		var walk = vm.AssemblyTreeModel.AssemblyList!.EnumerateAllAssemblies(cts.Token).GetAsyncEnumerator();
		try
		{
			while (await walk.MoveNextAsync() && walk.Current.ParentBundle == null)
			{
				// Skip past the list's own assemblies to the package's first entry.
			}
			walk.Current.ParentBundle.Should().NotBeNull("the package's entries come last on the list.");
			cts.Cancel();

			var next = async () => await walk.MoveNextAsync();
			await next.Should().ThrowAsync<OperationCanceledException>(
				"the second entry must not be extracted after the walk was cancelled.");
		}
		finally
		{
			await walk.DisposeAsync();
		}
	}

	[AvaloniaTest]
	public async Task FindTreeNode_resolves_a_type_inside_an_unexpanded_package()
	{
		// Search enumerates the contents of packages whether or not the user ever opened them in
		// the tree, so activating such a result has to reach a node that does not exist yet.
		var (_, vm) = await TestHarness.BootAsync();
		await vm.OpenAssemblyAsync(CreatePackage());

		var nested = (await vm.AssemblyTreeModel.AssemblyList!.GetAllAssemblies())
			.Single(a => a.FileName == "lib/net10.0/Nested.dll");
		var type = nested.GetTypeSystemOrNull()!.MainModule.TypeDefinitions
			.Single(t => t.Name == FixtureAssembly.TypeName);

		var node = vm.AssemblyTreeModel.FindTreeNode(type);

		// Assert the owning module, not just the node type: the fixture's type handle is
		// 0x02000002, which resolves in nearly every assembly on the list, so a lookup that fell
		// back to the first top-level node would still hand back some TypeTreeNode.
		((object?)node).Should().BeOfType<TypeTreeNode>()
			.Which.Module.Should().BeSameAs(nested.GetMetadataFileOrNull(),
				"the lookup must descend into the package's folders, expanding them on the way.");
	}

	[AvaloniaTest]
	public async Task FindTreeNode_leaves_package_folders_off_the_path_unexpanded()
	{
		// Expanding a package folder resolves and extracts every .dll it holds, so a lookup that
		// swept the package depth-first would pay for entries the user never asked about.
		var (_, vm) = await TestHarness.BootAsync();
		var package = await vm.OpenAssemblyAsync(CreatePackage());

		var nested = (await vm.AssemblyTreeModel.AssemblyList!.GetAllAssemblies())
			.Single(a => a.FileName == "lib/net10.0/Nested.dll");
		var type = nested.GetTypeSystemOrNull()!.MainModule.TypeDefinitions
			.Single(t => t.Name == FixtureAssembly.TypeName);

		((object?)vm.AssemblyTreeModel.FindTreeNode(type)).Should().NotBeNull();

		var packageNode = vm.AssemblyTreeModel.FindAssemblyNode(package);
		((object?)packageNode).Should().NotBeNull();
		var sibling = packageNode!.Children.OfType<PackageFolderTreeNode>()
			.Single(f => f.Text as string == "runtimes/win-x64");
		sibling.Children.Should().BeEmpty(
			"only the folders on the path down to the target get expanded.");
	}
}
