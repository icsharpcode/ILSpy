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

#if DEBUG

using System.Linq;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ILSpy.AppEnv;
using ILSpy.Languages;
using ILSpy.ViewModels;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Views;

/// <summary>
/// Pins the Debug Steps wiring at the composition layer. Tests are gated by DEBUG since
/// the feature is itself DEBUG-only — Release builds wouldn't have the types these
/// assertions reference.
/// </summary>
[TestFixture]
public class DebugStepsTests
{
	[AvaloniaTest]
	public Task DebugStepsPaneModel_Is_Exported_As_A_ToolPane()
	{
		// MEF tool-pane registry contract: DebugStepsPaneModel registers under its
		// PaneContentId so DockWorkspace.ShowToolPane(...) can surface it.
		var pane = AppComposition.Current.GetExport<DebugStepsPaneModel>();
		((object?)pane).Should().NotBeNull("DebugStepsPaneModel must resolve as a [Shared] export");
		pane.Id.Should().Be(DebugStepsPaneModel.PaneContentId);
		pane.Title.Should().Be("Debug Steps");
		return Task.CompletedTask;
	}

	[AvaloniaTest]
	public Task DebugStepsPaneModel_WritingOptions_Default_Enables_Field_And_LogicOperation_Sugar()
	{
		// The default writing options match the WPF baseline: field sugar and
		// logic-operation sugar on; IL ranges and child-index-in-block off. The four
		// CheckBoxes in DebugSteps.axaml bind two-way against these defaults.
		var options = DebugStepsPaneModel.WritingOptions;
		options.UseFieldSugar.Should().BeTrue();
		options.UseLogicOperationSugar.Should().BeTrue();
		options.ShowILRanges.Should().BeFalse();
		options.ShowChildIndexInBlock.Should().BeFalse();
		return Task.CompletedTask;
	}

	[AvaloniaTest]
	public Task ILAst_And_TypedIL_Languages_Are_Registered_In_Debug_Builds()
	{
		// Two ILAstLanguage subclasses: BlockILLanguage ("ILAst") drives the stepper,
		// TypedILLanguage ("Typed IL") writes type-annotated raw IL without transforms.
		// Both register via [Export(typeof(Language))]; the language picker uses them
		// in addition to C# and the disassembler-IL language.
		var languageService = AppComposition.Current.GetExport<LanguageService>();
		languageService.Languages.OfType<ILAstLanguage>().Should().HaveCount(2,
			"both BlockIL and TypedIL must be registered when DEBUG is defined");
		languageService.Languages.Should().Contain(l => l.Name == "ILAst");
		languageService.Languages.Should().Contain(l => l.Name == "Typed IL");
		return Task.CompletedTask;
	}
}

#endif
