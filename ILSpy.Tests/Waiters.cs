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
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Threading;
using Avalonia.VisualTree;

using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Tests;

public static class Waiters
{
	// Generous so the slow Windows CI runner can finish a cold first decompile (JIT + building a
	// CoreLib-scale type system) or a large/whole-assembly decompile. A fast machine completes well
	// under this and never waits the full window; only a genuine hang pays the price.
	static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(60);
	static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(25);

	public static async Task WaitForAsync(
		Func<bool> predicate,
		TimeSpan? timeout = null,
		[CallerArgumentExpression(nameof(predicate))] string? description = null)
	{
		ArgumentNullException.ThrowIfNull(predicate);
		var deadline = DateTime.UtcNow + (timeout ?? DefaultTimeout);
		while (DateTime.UtcNow < deadline)
		{
			if (predicate())
				return;
			Dispatcher.UIThread.RunJobs();
			await Task.Delay(PollInterval);
		}
		throw new TimeoutException(
			$"Timed out after {(timeout ?? DefaultTimeout).TotalSeconds:0.#}s waiting for: {description}");
	}

	public static async Task WaitForAssembliesAsync(
		this AssemblyTreeModel atm,
		int minimumCount = 1,
		TimeSpan? timeout = null)
	{
		ArgumentNullException.ThrowIfNull(atm);
		await WaitForAsync(
			() => (atm.AssemblyList?.GetAssemblies().Length ?? 0) >= minimumCount,
			timeout,
			$"AssemblyList to contain >= {minimumCount} assemblies");
		var assemblies = atm.AssemblyList!.GetAssemblies();
		await Task.WhenAll(assemblies.Select(a => a.GetLoadResultAsync()));
	}

	/// <summary>
	/// Waits until exactly one descendant of <typeparamref name="T"/> exists in the
	/// <paramref name="root"/>'s visual tree, then returns it. Replaces the brittle
	/// <c>root.GetVisualDescendants().OfType&lt;T&gt;().Single()</c> pattern that races against
	/// the layout/composition cycle - Avalonia.Headless tests routinely query the visual tree
	/// before lazily-templated panes (DataGrid, dock panes, etc.) have materialised.
	/// </summary>
	public static async Task<T> WaitForComponent<T>(this Visual root, TimeSpan? timeout = null)
		where T : Visual
	{
		ArgumentNullException.ThrowIfNull(root);
		await WaitForAsync(
			() => root.GetVisualDescendants().OfType<T>().Any(),
			timeout,
			$"a {typeof(T).Name} descendant to materialise in the visual tree");
		return root.GetVisualDescendants().OfType<T>().First();
	}

	public static async Task<DecompilerTabPageModel> WaitForDecompiledTextAsync(
		this DockWorkspace dock,
		TimeSpan? timeout = null)
	{
		ArgumentNullException.ThrowIfNull(dock);
		var documents = ((ILSpyDockFactory)dock.Factory).Documents
			?? throw new InvalidOperationException("DockWorkspace has no document dock yet.");

		await WaitForAsync(
			() => documents.ActiveDockable is ContentTabPage { Content: DecompilerTabPageModel { IsDecompiling: false } tab }
				&& !string.IsNullOrEmpty(tab.Text),
			timeout,
			"active decompiler tab to finish decompiling and produce text");

		return (DecompilerTabPageModel)((ContentTabPage)documents.ActiveDockable!).Content!;
	}

	public static async Task<MetadataTablePageModel> WaitForMetadataTabAsync(
		this DockWorkspace dock,
		TimeSpan? timeout = null)
	{
		ArgumentNullException.ThrowIfNull(dock);
		var documents = ((ILSpyDockFactory)dock.Factory).Documents
			?? throw new InvalidOperationException("DockWorkspace has no document dock yet.");

		await WaitForAsync(
			() => documents.ActiveDockable is ContentTabPage { Content: MetadataTablePageModel { Items.Count: > 0 } },
			timeout,
			"active dockable to be a metadata table tab with populated rows");

		return (MetadataTablePageModel)((ContentTabPage)documents.ActiveDockable!).Content!;
	}
}
