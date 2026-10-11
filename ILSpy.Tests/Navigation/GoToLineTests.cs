// Copyright (c) 2026 Piero Viano
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
using System.Threading.Tasks;

using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Navigation;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.Tests.TextView;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Navigation;

/// <summary>
/// Go to Line (dotPeek Ctrl+G in the code view): a small prompt in the active decompiler view
/// that moves the caret to "line" or "line:column".
/// </summary>
[TestFixture]
public class GoToLineTests
{
	static async Task<(MainWindow Window, DecompilerTextView View)> SetupAsync()
	{
		var (window, vm) = await TestHarness.BootAsync();
		await vm.OpenAssemblyAsync(typeof(MemberHighlightSample).Assembly.Location);
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"ILSpy.Tests",
			"ICSharpCode.ILSpy.Tests.TextView",
			"ICSharpCode.ILSpy.Tests.TextView.MemberHighlightSample");
		vm.AssemblyTreeModel.SelectNode(typeNode);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		var view = window.GetVisualDescendants().OfType<DecompilerTextView>().First();
		return (window, view);
	}

	[TestCase("12", 12, null)]
	[TestCase(" 7 ", 7, null)]
	[TestCase("12:5", 12, 5)]
	[TestCase("3,9", 3, 9)]
	public void Parses_Line_And_Optional_Column(string input, int line, int? column)
	{
		GoToLineInput.TryParse(input, out var parsedLine, out var parsedColumn).Should().BeTrue();
		parsedLine.Should().Be(line);
		parsedColumn.Should().Be(column);
	}

	[TestCase("")]
	[TestCase("abc")]
	[TestCase("0")]
	[TestCase("-3")]
	[TestCase("4:0")]
	[TestCase("4:x")]
	public void Rejects_Invalid_Input(string input)
	{
		GoToLineInput.TryParse(input, out _, out _).Should().BeFalse();
	}

	[AvaloniaTest]
	public async Task GoToLine_Moves_The_Caret_To_The_Line_And_Column()
	{
		var (_, view) = await SetupAsync();
		view.Editor.Document.LineCount.Should().BeGreaterThan(5);

		var target = view.Editor.Document.Lines.Skip(1).First(l => l.Length >= 5).LineNumber;

		view.GoToLine(target, 3).Should().BeTrue();

		view.Editor.TextArea.Caret.Line.Should().Be(target);
		view.Editor.TextArea.Caret.Column.Should().Be(3);
	}

	[AvaloniaTest]
	public async Task GoToLine_Clamps_Out_Of_Range_Positions()
	{
		var (_, view) = await SetupAsync();
		int lineCount = view.Editor.Document.LineCount;

		view.GoToLine(lineCount + 100, 1000).Should().BeTrue();

		view.Editor.TextArea.Caret.Line.Should().Be(lineCount);
		var lastLine = view.Editor.Document.GetLineByNumber(lineCount);
		view.Editor.TextArea.Caret.Column.Should().Be(lastLine.Length + 1, "the column is clamped to the end of the line");
	}

	[AvaloniaTest]
	public async Task Ctrl_G_In_The_Editor_Opens_The_Prompt_And_Enter_Jumps()
	{
		var (window, view) = await SetupAsync();
		view.Editor.TextArea.Focus();
		Dispatcher.UIThread.RunJobs();

		window.KeyPress(Key.G, RawInputModifiers.Control, PhysicalKey.G, null);
		Dispatcher.UIThread.RunJobs();

		var prompt = window.OwnedWindows.OfType<GoToLineWindow>().Should().ContainSingle(
			"Ctrl+G inside the code view opens the Go to Line prompt").Subject;
		prompt.InputText = "5";
		prompt.Accept();
		Dispatcher.UIThread.RunJobs();

		view.Editor.TextArea.Caret.Line.Should().Be(5);
		window.OwnedWindows.OfType<GoToLineWindow>().Should().BeEmpty("accepting closes the prompt");
	}

	[AvaloniaTest]
	public async Task Menu_Command_Opens_The_Prompt_For_The_Active_Code_View()
	{
		var (window, _) = await SetupAsync();

		var command = AppComposition.Current.GetExport<MainMenuCommandRegistry>().GetCommand(nameof(Resources.GoToLine));
		command.CanExecute(null).Should().BeTrue("a decompiled document is showing");
		command.Execute(null);

		var prompt = window.OwnedWindows.OfType<GoToLineWindow>().Should().ContainSingle().Subject;
		prompt.Close();
	}
}
