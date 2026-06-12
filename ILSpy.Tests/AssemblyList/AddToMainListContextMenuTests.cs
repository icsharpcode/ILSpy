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

using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// The "Add to main list" context entry promotes an auto-loaded (on-demand resolved) dependency into
/// a permanent member of the assembly list. It is visible only when an auto-loaded assembly is
/// selected. (Regression: the entry was dropped in the Avalonia port.)
/// </summary>
[TestFixture]
public class AddToMainListContextMenuTests
{
	[AvaloniaTest]
	public async Task AddToMainList_Is_Hidden_For_A_Normal_List_Member()
	{
		var (_, vm) = await TestHarness.BootAsync(3);
		var entry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>().GetEntry(nameof(Resources._AddMainList));

		var asm = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		Assert.That(asm, Is.Not.Null);

		entry.IsVisible(new TextViewContext { SelectedTreeNodes = new[] { (SharpTreeNode)asm! } })
			.Should().BeFalse("a normal (non auto-loaded) list member must not show 'Add to main list'");
	}

	[AvaloniaTest]
	public async Task AddToMainList_Promotes_An_AutoLoaded_Assembly()
	{
		var (_, vm) = await TestHarness.BootAsync(3);
		var entry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>().GetEntry(nameof(Resources._AddMainList));

		var asm = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		Assert.That(asm, Is.Not.Null);
		asm!.LoadedAssembly.IsAutoLoaded = true;
		try
		{
			var context = new TextViewContext { SelectedTreeNodes = new[] { (SharpTreeNode)asm } };
			entry.IsVisible(context).Should().BeTrue("an auto-loaded assembly is selected");

			entry.Execute(context);

			asm.IsAutoLoaded.Should().BeFalse("the assembly was promoted into the main list");
		}
		finally
		{
			asm.LoadedAssembly.IsAutoLoaded = false;
		}
	}
}
