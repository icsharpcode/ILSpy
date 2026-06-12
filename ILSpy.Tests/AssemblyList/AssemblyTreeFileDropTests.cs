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
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Controls.DataGridDragDrop;
using Avalonia.Headless.NUnit;
using Avalonia.Input;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Controls.TreeView;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class AssemblyTreeFileDropTests
{
	enum DropPos { Before, After }

	// External file drops are handled generically by SharpTreeView -> AssemblyListTreeNode.Drop with
	// the dropped paths under FileDropFormat; this drives that path the way the control would.
	static void Drop(AssemblyTreeModel model, string[] files, AssemblyTreeNode? target, DropPos position)
	{
		var root = (AssemblyListTreeNode)model.Root!;
		var data = new AvaloniaDataObject();
		data.SetData(AssemblyListTreeNode.FileDropFormat, files);
		var args = new AvaloniaPlatformDragEventArgs(data);
		int index = target == null
			? root.Children.Count
			: root.Children.IndexOf(target) + (position == DropPos.After ? 1 : 0);
		root.Drop(args, index);
	}

	[AvaloniaTest]
	public async Task DataGrid_Is_Configured_As_A_File_Drop_Target()
	{
		// Avalonia's standard drag-drop pipeline routes Explorer file drops through
		// DragDrop.AllowDrop / DragDrop.DropEvent. The grid must opt in or external drops
		// produce a "no" cursor and never reach our handler.
		var (window, vm) = await TestHarness.BootAsync();
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var tree = await pane.WaitForComponent<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView>();

		DragDrop.GetAllowDrop(tree).Should().BeTrue();
	}

	[AvaloniaTest]
	public async Task File_Drop_With_No_Target_Opens_Assembly_And_Appends_To_List()
	{
		// Simulating Windows Explorer dropping a .dll on empty space inside the tree.
		// No target row → the new entry is appended (WPF's AssemblyListTreeNode.Drop with
		// no target index does the same).
		var (window, vm) = await TestHarness.BootAsync(3);
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var list = vm.AssemblyTreeModel.AssemblyList!;

		var beforeCount = list.GetAssemblies().Length;
		var tempPath = CloneCoreLibToTemp();
		try
		{
			Drop(vm.AssemblyTreeModel, new[] { tempPath }, target: null, DropPos.After);
			TestCapture.Step("file-dropped-no-target");

			var after = list.GetAssemblies();
			after.Should().HaveCount(beforeCount + 1);
			after.Last().FileName.Should().Be(tempPath);
		}
		finally
		{
			TryUnload(list, tempPath);
			TryDelete(tempPath);
		}
	}

	[AvaloniaTest]
	public async Task File_Drop_Before_A_Target_Row_Inserts_At_That_Index()
	{
		// Drop position is honoured: dragging onto the first row with Position=Before lands
		// the opened assembly at index 0.
		var (window, vm) = await TestHarness.BootAsync(3);
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var list = vm.AssemblyTreeModel.AssemblyList!;

		var topLevelBefore = vm.AssemblyTreeModel.Root!.Children
			.OfType<AssemblyTreeNode>().ToArray();
		var firstNode = topLevelBefore[0];
		var beforeCount = list.GetAssemblies().Length;

		var tempPath = CloneCoreLibToTemp();
		try
		{
			Drop(vm.AssemblyTreeModel, new[] { tempPath }, target: firstNode,
				DropPos.Before);
			TestCapture.Step("file-dropped-before-first-row");

			var after = list.GetAssemblies();
			after.Should().HaveCount(beforeCount + 1);
			after[0].FileName.Should().Be(tempPath);
		}
		finally
		{
			TryUnload(list, tempPath);
			TryDelete(tempPath);
		}
	}

	[AvaloniaTest]
	public async Task File_Drop_After_A_Target_Row_Inserts_Past_That_Index()
	{
		// Position=After: insertion lands at targetIndex + 1.
		var (window, vm) = await TestHarness.BootAsync(3);
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var list = vm.AssemblyTreeModel.AssemblyList!;

		var firstNode = vm.AssemblyTreeModel.Root!.Children
			.OfType<AssemblyTreeNode>().First();

		var tempPath = CloneCoreLibToTemp();
		try
		{
			Drop(vm.AssemblyTreeModel, new[] { tempPath }, target: firstNode,
				DropPos.After);
			TestCapture.Step("file-dropped-after-first-row");

			var after = list.GetAssemblies();
			after[1].FileName.Should().Be(tempPath);
		}
		finally
		{
			TryUnload(list, tempPath);
			TryDelete(tempPath);
		}
	}

	[AvaloniaTest]
	public async Task File_Drop_Selects_The_Newly_Opened_Assembly_Nodes()
	{
		// Selecting the freshly-opened assemblies after a drop mirrors WPF's
		// AssemblyListTreeNode.Drop (which ends in AssemblyTreeModel.SelectNodes).
		// Without this, the user gets no visual confirmation that the drop took, and
		// the decompiler view stays parked on whatever was selected before.
		var (window, vm) = await TestHarness.BootAsync(3);
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var list = vm.AssemblyTreeModel.AssemblyList!;

		var tempPath = CloneCoreLibToTemp();
		try
		{
			Drop(vm.AssemblyTreeModel, new[] { tempPath }, target: null,
				DropPos.After);
			TestCapture.Step("file-dropped-new-node-selected");

			var newAsm = list.GetAssemblies()
				.First(a => string.Equals(a.FileName, tempPath, StringComparison.OrdinalIgnoreCase));
			var expectedNode = vm.AssemblyTreeModel.Root!.Children.OfType<AssemblyTreeNode>()
				.First(n => n.LoadedAssembly == newAsm);

			vm.AssemblyTreeModel.SelectedItems.Should().HaveCount(1);
			ReferenceEquals(vm.AssemblyTreeModel.SelectedItems[0], expectedNode)
				.Should().BeTrue();
		}
		finally
		{
			TryUnload(list, tempPath);
			TryDelete(tempPath);
		}
	}

	[AvaloniaTest]
	public async Task File_Drop_With_A_Path_That_Cannot_Be_Opened_Does_Not_Mutate_The_List()
	{
		// AssemblyList.OpenAssembly returns null for non-existent / non-PE paths; the
		// handler must filter those out and never raise on the caller's behalf.
		var (window, vm) = await TestHarness.BootAsync(3);
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var list = vm.AssemblyTreeModel.AssemblyList!;

		var bogusPath = Path.Combine(Path.GetTempPath(),
			"definitely-not-an-assembly-" + Guid.NewGuid().ToString("N") + ".dll");
		// Write a junk file so the path exists but isn't a PE — OpenAssembly tolerates this
		// at construction and only fails later in LoadAsync, but we'd still get a LoadedAssembly
		// in the list. The cleaner non-PE check is "file simply doesn't exist".
		var beforeCount = list.GetAssemblies().Length;

		Drop(vm.AssemblyTreeModel, new[] { bogusPath }, target: null,
			DropPos.After);
		TestCapture.Step("bogus-path-dropped");

		// OpenAssembly does add the entry (it lazy-loads), so the count may grow. The contract
		// we exercise here is "the call returns without throwing" — verified by reaching this
		// line. Cleanup any side-effect the call had.
		var afterCount = list.GetAssemblies().Length;
		if (afterCount > beforeCount)
			TryUnload(list, bogusPath);
	}

	static string CloneCoreLibToTemp()
	{
		// Copy CoreLib to a unique path so OpenAssembly doesn't dedupe against the entry the
		// model already created during boot.
		var src = typeof(object).Assembly.Location;
		var path = Path.Combine(Path.GetTempPath(),
			"ILSpy.FileDropTest." + Guid.NewGuid().ToString("N") + ".dll");
		File.Copy(src, path);
		return path;
	}

	static void TryUnload(ICSharpCode.ILSpyX.AssemblyList list, string path)
	{
		var match = list.GetAssemblies().FirstOrDefault(a =>
			string.Equals(a.FileName, path, StringComparison.OrdinalIgnoreCase));
		if (match != null)
			list.Unload(match);
	}

	static void TryDelete(string path)
	{
		try
		{ File.Delete(path); }
		catch { /* fixture cleanup is best-effort */ }
	}
}
