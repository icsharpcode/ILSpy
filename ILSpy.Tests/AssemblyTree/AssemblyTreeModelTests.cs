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

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpyX;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;

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
}
