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
using Avalonia.Threading;

using AwesomeAssertions;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class DecompileReferenceContextMenuTests
{
	[AvaloniaTest]
	public async Task Decompile_On_A_Code_Reference_Navigates_To_The_Entity_Definition()
	{
		// Right-clicking a symbol in the decompiled code and choosing "Decompile" (go to definition)
		// must move the assembly tree to that entity's definition node, like clicking the hyperlink.
		var (_, vm) = await TestHarness.BootAsync(3);
		var entry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>()
			.GetEntry(nameof(Resources.Decompile));

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		var entity = (IEntity)typeNode.Member!;
		var context = new TextViewContext { Reference = new ReferenceSegment { Reference = entity } };

		entry.IsVisible(context).Should().BeTrue("a clicked entity reference must surface Decompile/go-to-definition");

		// Park the selection elsewhere so the jump is observable.
		var other = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		vm.AssemblyTreeModel.SelectNode(other);
		ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, other).Should().BeTrue("precondition: parked on another node");

		entry.Execute(context);
		Dispatcher.UIThread.RunJobs();

		var navigatedTo = (vm.AssemblyTreeModel.SelectedItem as IMemberTreeNode)?.Member;
		((object?)navigatedTo).Should().NotBeNull("Decompile must navigate to the entity's definition node");
		navigatedTo!.MetadataToken.Should().Be(entity.MetadataToken,
			"the navigated node must be the clicked entity's definition");
	}
}
