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

using System;
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Dependencies;

[TestFixture]
public class UnresolvedReferencesReportUiTests
{
	[AvaloniaTest]
	public async Task Unresolved_References_Report_Lists_The_Missing_Reference_With_Its_Log()
	{
		var (_, vm) = await TestHarness.BootAsync();
		var (consumerPath, missingName) = DependencyFixtures.EmitConsumerWithMissingReference("Ui");
		var orphan = await vm.OpenAssemblyAsync(consumerPath);
		vm.AssemblyTreeModel.SelectNode<AssemblyTreeNode>(orphan.ShortName);

		var registry = AppComposition.Current.GetExport<MainMenuCommandRegistry>();
		registry.Commands.Single(c => c.Metadata.Header == nameof(Resources.UnresolvedReferencesReport)).Metadata.ParentMenuID
			.Should().Be(nameof(Resources._Navigate));
		registry.GetCommand(nameof(Resources.UnresolvedReferencesReport)).Execute(null);

		var dockWorkspace = AppComposition.Current.GetExport<DockWorkspace>();
		string? text = null;
		await Waiters.WaitForAsync(() => {
			var content = dockWorkspace.Documents?.VisibleDockables?.OfType<ContentTabPage>()
				.Select(t => t.Content).OfType<DecompilerTabPageModel>()
				.FirstOrDefault(c => c.Title == Resources.UnresolvedReferencesReport);
			text = content?.Text;
			return text?.Contains(missingName, StringComparison.Ordinal) == true;
		}, description: "unresolved references report text");
		text.Should().Contain(orphan.ShortName);
		text.Should().Contain("Could not find reference");
	}
}
