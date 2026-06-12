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

[TestFixture]
public class ClearAssemblyListCommandTests
{
	[AvaloniaTest]
	public async Task Clear_Assembly_List_Re_Raises_CanExecute_When_The_List_Changes()
	{
		// The native menu only re-reads CanExecute when CanExecuteChanged fires. The command used to
		// never raise it, so it stayed at its startup value -- the list was empty then, leaving the
		// menu item disabled forever. Now CanExecuteChanged is routed through the shared CommandManager,
		// which the assembly-list change invalidates -- so the item re-queries whenever the list changes.
		var (_, vm) = await TestHarness.BootAsync(1);
		var command = AppComposition.Current.GetExport<MainMenuCommandRegistry>()
			.GetCommand(nameof(Resources.ClearAssemblyList));

		command.CanExecute(null).Should().BeTrue("with assemblies loaded the command is enabled");

		bool raised = false;
		// Keep the handler rooted: CommandManager holds subscribers weakly, so an inline lambda with
		// no other reference could be collected before the raise.
		EventHandler handler = (_, _) => raised = true;
		command.CanExecuteChanged += handler;

		vm.AssemblyTreeModel.AssemblyList!.Clear();
		Dispatcher.UIThread.RunJobs();

		raised.Should().BeTrue(
			"clearing the list must re-raise CanExecuteChanged so the menu re-queries the command's enablement");
		command.CanExecute(null).Should().BeFalse("an empty list disables the command");
		GC.KeepAlive(handler);
	}
}
