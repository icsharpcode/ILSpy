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
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;
using Avalonia.Threading;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Commands;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// The general fix (routing SimpleCommand.CanExecuteChanged through the shared CommandManager,
/// invalidated on selection changes) must apply to every selection-dependent menu command, not just
/// the one originally reported. Save Code is the canonical case.
/// </summary>
[TestFixture]
public class SaveCommandEnablementTests
{
	[AvaloniaTest]
	public async Task Save_Command_Enables_When_A_Tree_Node_Is_Selected()
	{
		var (_, vm) = await TestHarness.BootAsync(1);
		var command = AppComposition.Current.GetExport<MainMenuCommandRegistry>()
			.GetCommand(nameof(Resources._SaveCode));

		command.CanExecute(null).Should().BeFalse("with nothing selected at startup, Save is disabled");

		bool raised = false;
		EventHandler handler = (_, _) => raised = true; // rooted; CommandManager holds handlers weakly
		command.CanExecuteChanged += handler;

		vm.AssemblyTreeModel.SelectNode(vm.AssemblyTreeModel.Root!.Children[0]);
		Dispatcher.UIThread.RunJobs();

		raised.Should().BeTrue(
			"selecting a node must re-raise CanExecuteChanged so the menu re-queries the command's enablement");
		command.CanExecute(null).Should().BeTrue("a selected tree node enables Save");
		GC.KeepAlive(handler);
	}
}
