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
using System.Threading;
using System.Threading.Tasks;

using Avalonia.Threading;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Docking;

/// <summary>
/// A long-running operation started via <c>DockWorkspace.RunInNewTabAsync</c> runs in its own
/// frozen tab, so selecting tree nodes (which re-decompiles the preview tab) does not cancel it —
/// unlike <c>RunWithCancellation</c>, which runs on the preview tab navigation cancels.
/// </summary>
[TestFixture]
public class RunInNewTabTests
{
	[AvaloniaTest]
	public async Task A_Long_Op_In_A_New_Tab_Is_Not_Cancelled_By_Tree_Navigation()
	{
		var (_, vm) = await TestHarness.BootAsync(3);
		var dock = vm.DockWorkspace;

		var started = new TaskCompletionSource();
		var release = new TaskCompletionSource();
		CancellationToken capturedToken = default;

		int tabsBefore = dock.Documents!.VisibleDockables!.OfType<ContentTabPage>().Count();

		var runTask = dock.RunInNewTabAsync("Long op", async token => {
			capturedToken = token;
			started.SetResult();
			await release.Task;
			var o = new AvaloniaEditTextOutput { Title = "Long op report" };
			o.Write("complete");
			o.WriteLine();
			return o;
		});

		await started.Task;

		dock.Documents!.VisibleDockables!.OfType<ContentTabPage>().Count()
			.Should().BeGreaterThan(tabsBefore, "the long op opens its own tab");

		// Navigate while the op is running: select a node -> the preview tab decompiles. A C#
		// namespace node decompiles to just its "// Some.Name.Space" comment line, which keeps
		// this inside the headless decompile-wait budget on slow CI runners (a full type decompile
		// would not).
		var navNode = vm.AssemblyTreeModel.FindNode<NamespaceTreeNode>(
			TreeNavigation.CoreLibName, "System.Runtime.Versioning");
		vm.AssemblyTreeModel.SelectNode(navNode);
		await dock.WaitForDecompiledTextAsync();
		for (int i = 0; i < 6; i++)
		{
			Dispatcher.UIThread.RunJobs();
			await Task.Delay(20);
		}

		capturedToken.IsCancellationRequested.Should().BeFalse(
			"navigating the tree must NOT cancel a long op running in its own frozen tab");

		// Let it finish; its report lands in its tab.
		release.SetResult();
		await runTask;

		dock.Documents!.VisibleDockables!.OfType<ContentTabPage>()
			.Any(t => t.Content is DecompilerTabPageModel d && d.Text != null && d.Text.Contains("complete"))
			.Should().BeTrue("the finished op shows its report in its tab");
	}
}
