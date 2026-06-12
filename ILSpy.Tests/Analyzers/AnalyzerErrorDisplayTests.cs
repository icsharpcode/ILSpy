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
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.Analyzers;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Analyzers;

[TestFixture]
public class AnalyzerErrorDisplayTests
{
	[AvaloniaTest]
	public Task AnalyzerErrorNode_Built_From_An_Exception_Carries_The_Full_Stack_For_Copy_And_Tooltip()
	{
		// When an analyser throws, the error node must retain the full ex.ToString() (stack
		// trace included) — not just ex.Message — so users can paste it into a bug report.

		Exception captured;
		try
		{
			throw new InvalidCastException("simulated analyser failure");
		}
		catch (InvalidCastException ex)
		{
			captured = ex;
		}

		var node = new AnalyzerErrorNode(captured);

		((string)node.Text).Should().StartWith("Error: ")
			.And.Contain("simulated analyser failure");
		node.Details.Should().Contain("InvalidCastException",
			"the full payload must include the exception type so users have something to grep for");
		node.Details.Should().Contain("simulated analyser failure");
		node.Details.Should().Contain(nameof(AnalyzerErrorNode_Built_From_An_Exception_Carries_The_Full_Stack_For_Copy_And_Tooltip),
			"ex.ToString() includes a stack trace — the method that threw must show up in it");
		((string?)node.ToolTip).Should().Be(node.Details,
			"the tooltip surface is the user's hover-to-read access to the same payload");

		return Task.CompletedTask;
	}

	[AvaloniaTest]
	public async Task CopyErrorMessage_Entry_Is_Visible_Only_For_Error_Nodes()
	{
		var (_, vm) = await TestHarness.BootAsync();

		var entry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>()
			.GetEntry(nameof(Resources.CopyErrorMessage));

		var errorNode = new AnalyzerErrorNode(new InvalidOperationException("boom"));
		entry.IsVisible(new TextViewContext { SelectedTreeNodes = new SharpTreeNode[] { errorNode } })
			.Should().BeTrue("Copy Error Message must surface for AnalyzerErrorNode rows");

		var unrelatedNode = vm.AssemblyTreeModel.FindCoreLib();
		entry.IsVisible(new TextViewContext { SelectedTreeNodes = new SharpTreeNode[] { unrelatedNode } })
			.Should().BeFalse("non-error rows must not see the Copy Error Message entry");
	}
}
