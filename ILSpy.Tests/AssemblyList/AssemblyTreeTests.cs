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
using System.Threading.Tasks;
using System.Xml.Linq;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Properties;

using ILSpy;
using ILSpy.AppEnv;
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
}
