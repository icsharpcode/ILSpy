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

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.Decompiler.CSharp;

using ILSpy.AppEnv;
using ILSpy.Languages;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Languages;

/// <summary>
/// Pins the WPF parity feature: in Debug builds the C# language pipeline contributes one
/// extra language per AST transform, named <c>"C# - no transforms"</c>, <c>"C# - after
/// <em>TransformName</em>"</c>, … so a developer can pick the dropdown entry and see the
/// decompiler's intermediate AST at that stage. Each variant sets <c>showAllMembers</c>
/// so compiler-generated members aren't hidden — visibility into the synthetic ones is
/// the whole point of the feature.
/// </summary>
[TestFixture]
public class CSharpDebugTransformLanguagesTests
{
	[Test]
	public void GetDebugLanguages_Yields_A_Pipeline_Step_Per_Ast_Transform_Plus_The_No_Transforms_Baseline()
	{
		var transforms = CSharpDecompiler.GetAstTransforms().ToList();
		transforms.Should().NotBeEmpty(
			"the decompiler must publish at least one AST transform — otherwise the debug-languages feature would be meaningless");

		var debugLanguages = CSharpLanguage.GetDebugLanguages().ToList();

		// One baseline ("no transforms"), one variant per transform, plus a final "after
		// <last transform>" entry. So count = transforms.Count + 1.
		debugLanguages.Should().HaveCount(transforms.Count + 1,
			"one variant for the baseline + one per transform step (the last yields the fully-transformed AST)");

		debugLanguages.Select(l => l.Name).Should().Contain("C# - no transforms",
			"the first dropdown entry is the baseline before any transform runs");

		// Spot-check the second entry is named after the first transform, matching WPF —
		// each subsequent variant is named after the transform that just ran.
		debugLanguages[1].Name.Should().Be("C# - after " + transforms[0].GetType().Name);
	}

	[AvaloniaTest]
	public void LanguageService_Registers_The_Debug_Transform_Languages_In_The_Dropdown()
	{
		// MEF-side wiring: LanguageService aggregates [Export(Language)]-resolved instances
		// plus the manually-yielded debug variants under #if DEBUG. Without the registration
		// step the variants never reach the toolbar — even though GetDebugLanguages itself
		// would return a populated list.
		var languageService = AppComposition.Current.GetExport<LanguageService>();
		var names = languageService.Languages.Select(l => l.Name).ToList();

		names.Should().Contain("C# - no transforms",
			"LanguageService must include the no-transforms baseline in its Languages list");
		names.Should().Contain(n => n.StartsWith("C# - after "),
			"LanguageService must include at least one 'after <TransformName>' variant");
	}
}

#endif
