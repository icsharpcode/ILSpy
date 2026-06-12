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

using Avalonia;
using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

// Sanity check that TestApp.base.Initialize() populates DataTemplates and Styles from
// ILSpy/App.axaml. If this regresses, every UI-headless test that follows runs against an
// unconfigured Application surface (no ViewLocator, no theme), failing in confusing ways.
[TestFixture]
public class AppInitializationTests
{
	[AvaloniaTest]
	public void TestApp_pulls_DataTemplates_from_App_axaml()
	{
		var app = Application.Current!;
		app.DataTemplates.Should().Contain(t => t is ViewLocator,
			"App.axaml registers ViewLocator; base.Initialize() should propagate it onto TestApp.");
	}

	[AvaloniaTest]
	public void TestApp_pulls_Styles_from_App_axaml()
	{
		var app = Application.Current!;
		app.Styles.Should().NotBeEmpty(
			"App.axaml registers a theme via Application.Styles; TestApp should inherit it.");
	}
}
