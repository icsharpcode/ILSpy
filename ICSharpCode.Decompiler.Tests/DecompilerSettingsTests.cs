// Copyright (c) 2026 Siegfried Pammer
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

using ICSharpCode.Decompiler.CSharp;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests
{
	[TestFixture]
	class DecompilerSettingsTests
	{
		[Test]
		public void GetMinimumRequiredVersionReturnsTheHighestEnabledFeatureVersion()
		{
			var settings = new DecompilerSettings(LanguageVersion.CSharp1);
			Assert.That(settings.GetMinimumRequiredVersion(), Is.EqualTo(LanguageVersion.CSharp1));

			settings.AnonymousMethods = true;
			Assert.That(settings.GetMinimumRequiredVersion(), Is.EqualTo(LanguageVersion.CSharp2));

			// Syntax-preference settings participate too: enabled, they emit syntax of their version.
			settings.UseEnhancedUsing = true;
			Assert.That(settings.GetMinimumRequiredVersion(), Is.EqualTo(LanguageVersion.CSharp8_0));

			settings.SwitchOnReadOnlySpanChar = true;
			Assert.That(settings.GetMinimumRequiredVersion(), Is.EqualTo(LanguageVersion.CSharp11_0));

			// The scan must pick the highest enabled feature, not the first match bottom-up.
			settings.ParamsCollections = true;
			Assert.That(settings.GetMinimumRequiredVersion(), Is.EqualTo(LanguageVersion.CSharp13_0));

			settings.AnonymousMethods = false;
			settings.ParamsCollections = false;
			Assert.That(settings.GetMinimumRequiredVersion(), Is.EqualTo(LanguageVersion.CSharp11_0));
		}

		[Test]
		public void CollectionExpressionsRequireCSharp12()
		{
			Assert.That(new DecompilerSettings(LanguageVersion.CSharp11_0).CollectionExpressions, Is.False);
			Assert.That(new DecompilerSettings(LanguageVersion.CSharp12_0).CollectionExpressions, Is.True);
		}
	}
}
