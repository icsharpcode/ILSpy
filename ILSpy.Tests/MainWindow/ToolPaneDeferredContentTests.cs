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

using AwesomeAssertions;

using Dock.Controls.DeferredContentControl;

using ICSharpCode.ILSpy.ViewModels;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// Regression guard: <see cref="ToolPaneModel"/> opts out of Dock's deferred content
/// presentation so panes (search, analyzers, debug steps, etc.) materialise their views
/// eagerly. Without this, tests can't reach descendants of a pane until it's
/// focus-activated by the user — and the search-pane's startup tasks (assembly index
/// scan, etc.) wouldn't kick off until first activation.
/// </summary>
[TestFixture]
public class ToolPaneDeferredContentTests
{
	[Test]
	public void ToolPaneModel_Opts_Out_Of_Dock_Deferred_Content_Presentation()
	{
		var sentinel = new TestToolPane();
		((IDeferredContentPresentation)sentinel).DeferContentPresentation.Should().BeFalse();
	}

	sealed class TestToolPane : ToolPaneModel { }
}
