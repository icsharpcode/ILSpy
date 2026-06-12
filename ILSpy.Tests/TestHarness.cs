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
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Avalonia.Controls;
using Avalonia.Interactivity;

using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpyX;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// Boots and drives the shared MEF-composed application for headless UI tests, collapsing the
/// four-line prologue (resolve <see cref="MainWindow"/>, show it, cast its DataContext, wait for
/// the assembly list) and the "open an assembly via the production command and wait for it to
/// load" dance that nearly every fixture repeats. The container is rebuilt per test by
/// <see cref="ResetAppStateAttribute"/>, so each call hands back that test's own singletons.
/// </summary>
public static class TestHarness
{
	/// <summary>
	/// Resolves the shared <see cref="MainWindow"/>, shows it, and waits until the default
	/// assembly list has loaded at least <paramref name="minimumAssemblies"/> assemblies.
	/// Returns the window and its <see cref="MainWindowViewModel"/> — the two handles the test
	/// body almost always needs next.
	/// </summary>
	public static async Task<(MainWindow Window, MainWindowViewModel ViewModel)> BootAsync(
		int minimumAssemblies = 1,
		TimeSpan? timeout = null)
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumAssemblies, timeout);
		// First visual breakpoint of every UI test: the booted window with its assembly list loaded.
		window.Capture("booted");
		return (window, vm);
	}

	/// <summary>
	/// Looks up a [ExportMainMenuCommand]-tagged command by its <c>Header</c> metadata (pass the
	/// resource key, e.g. <c>nameof(Resources._Open)</c>) and materialises a fresh instance.
	/// </summary>
	public static ICommand GetCommand(this MainMenuCommandRegistry registry, string header)
	{
		ArgumentNullException.ThrowIfNull(registry);
		return registry.Commands
			.Single(c => string.Equals(c.Metadata.Header, header, StringComparison.Ordinal))
			.CreateExport().Value;
	}

	/// <summary>
	/// Looks up a [ExportContextMenuEntry]-tagged entry by its <c>Header</c> metadata.
	/// </summary>
	public static IContextMenuEntry GetEntry(this ContextMenuEntryRegistry registry, string header)
	{
		ArgumentNullException.ThrowIfNull(registry);
		return registry.Entries
			.Single(e => string.Equals(e.Metadata.Header, header, StringComparison.Ordinal))
			.Value;
	}

	/// <summary>
	/// Finds the <see cref="MenuItem"/> whose <c>Header</c> equals <paramref name="header"/> (pass
	/// the resolved resource string, e.g. <c>Resources._Reload</c>) in a menu built by
	/// <c>AssemblyListPane.BuildContextMenuForCurrentState</c> and raises its <c>Click</c> — the
	/// same routed event the production menu fires when the user picks the item. Collapses the
	/// <c>Items.OfType&lt;MenuItem&gt;().Single(...).RaiseEvent(...)</c> dance every context-menu
	/// test repeats.
	/// </summary>
	public static void ClickItem(this ContextMenu menu, string header)
	{
		ArgumentNullException.ThrowIfNull(menu);
		ArgumentNullException.ThrowIfNull(header);
		var item = menu.Items.OfType<MenuItem>()
			.Single(i => string.Equals((string?)i.Header, header, StringComparison.Ordinal));
		item.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
	}

	/// <summary>
	/// Opens <paramref name="path"/> through the production Open command, waits for it to appear
	/// in the assembly list, awaits its load result, and returns the resulting
	/// <see cref="LoadedAssembly"/>. This is the canonical "add a test assembly to the tree"
	/// sequence — registry lookup, execute, poll-for-appearance, await-load — in one call.
	/// </summary>
	public static async Task<LoadedAssembly> OpenAssemblyAsync(
		this MainWindowViewModel vm,
		string path,
		TimeSpan? timeout = null)
	{
		ArgumentNullException.ThrowIfNull(vm);
		ArgumentNullException.ThrowIfNull(path);

		var open = AppComposition.Current.GetExport<MainMenuCommandRegistry>().GetCommand(nameof(Resources._Open));
		open.Execute(path);

		await Waiters.WaitForAsync(
			() => vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
				.Any(a => string.Equals(a.FileName, path, StringComparison.OrdinalIgnoreCase)),
			timeout,
			$"assembly '{path}' to appear in the active list");

		var loaded = vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
			.First(a => string.Equals(a.FileName, path, StringComparison.OrdinalIgnoreCase));
		await loaded.GetLoadResultAsync();
		return loaded;
	}
}
