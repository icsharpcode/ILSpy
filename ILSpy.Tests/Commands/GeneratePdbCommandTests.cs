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

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Commands;

/// <summary>
/// The File-menu "Generate portable PDB" command is wired to the real <see cref="PdbGenerator"/>
/// (it used to be a not-implemented stub). It is enabled exactly when the selection holds at least
/// one valid loaded assembly, the same gate the context-menu entry uses.
/// </summary>
[TestFixture]
public class GeneratePdbCommandTests
{
	[AvaloniaTest]
	public async Task Generate_Pdb_Command_Tracks_The_Assembly_Selection()
	{
		var (_, vm) = await TestHarness.BootAsync(3);

		var command = AppComposition.Current.GetExport<MainMenuCommandRegistry>()
			.GetCommand(nameof(Resources.GeneratePortable));
		command.Should().NotBeNull("the File menu exposes a Generate portable PDB command");

		vm.AssemblyTreeModel.SelectedItems.Clear();
		command.CanExecute(null).Should().BeFalse("with nothing selected there is no assembly to write a PDB for");

		var coreLib = vm.AssemblyTreeModel.FindCoreLib();
		vm.AssemblyTreeModel.SelectedItems.Clear();
		vm.AssemblyTreeModel.SelectedItems.Add(coreLib);

		command.CanExecute(null).Should().BeTrue("a selected valid assembly enables the command");
	}

	[AvaloniaTest]
	public async Task TryGetAssemblies_Picks_Only_Valid_Assembly_Nodes()
	{
		var (_, vm) = await TestHarness.BootAsync(3);
		var coreLib = vm.AssemblyTreeModel.FindCoreLib();

		PdbGenerator.TryGetAssemblies(new[] { coreLib }, out var assemblies).Should().BeTrue();
		assemblies.Should().ContainSingle().Which.Should().BeSameAs(coreLib.LoadedAssembly);

		// A non-assembly node (a child of the assembly) is not a candidate.
		coreLib.Expand();
		var nonAssembly = coreLib.Children[0];
		(nonAssembly is AssemblyTreeNode).Should().BeFalse("a child of the assembly node is not itself an assembly");
		PdbGenerator.TryGetAssemblies(new[] { nonAssembly }, out var none).Should().BeFalse();
		none.Should().BeEmpty();
	}
}
