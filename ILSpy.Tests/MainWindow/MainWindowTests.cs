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

using System.Linq;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ILSpy.AppEnv;
using ILSpy.AssemblyTree;
using ILSpy.ViewModels;
using ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class MainWindowTests
{
	[AvaloniaTest]
	public void MainWindow_Resolves_From_Composition_And_Shows()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();

		window.IsVisible.Should().BeTrue();
		window.Title.Should().Be("ILSpy");
		window.DataContext.Should().BeOfType<MainWindowViewModel>();
	}

	[AvaloniaTest]
	public async Task Assembly_Tree_Pane_Is_Visible_In_Layout()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();

		await Waiters.WaitForAsync(() => window.GetVisualDescendants().OfType<AssemblyListPane>().Any());
		var pane = window.GetVisualDescendants().OfType<AssemblyListPane>().Single();

		pane.IsVisible.Should().BeTrue();
		pane.Bounds.Width.Should().BeGreaterThan(0);
		pane.Bounds.Height.Should().BeGreaterThan(0);
	}
}
