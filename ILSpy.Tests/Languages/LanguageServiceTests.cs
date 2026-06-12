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

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Languages;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Languages;

// LanguageService is a [Shared] aggregator of [Export(typeof(Language))] members. If MEF
// can't discover any Language exports — bad assembly scan, missing [Export(typeof(Language))]
// attribute, etc. — Languages stays empty, CurrentLanguage's "first language" fallback throws,
// and tree-node labels render as raw metadata names (the user-facing symptom). The MEF-level
// assertion here catches the regression before that surface symptom.
[TestFixture]
public class LanguageServiceTests
{
	[AvaloniaTest]
	public void LanguageService_exposes_CSharp_and_IL_with_CSharp_as_default()
	{
		var svc = AppComposition.Current.GetExport<LanguageService>();

		svc.Languages.Select(l => l.Name).Should().Contain(["C#", "IL"],
			"CSharpLanguage and ILLanguage are [Export(typeof(Language))] in ICSharpCode.ILSpy.Languages.");
		svc.CurrentLanguage.Name.Should().Be("C#",
			"LanguageService picks 'C#' over the alphabetical-first language when no SessionSettings preference exists.");
	}
}
