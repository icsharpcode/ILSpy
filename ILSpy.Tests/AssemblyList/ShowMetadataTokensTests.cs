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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class ShowMetadataTokensTests
{
	[AvaloniaTest]
	public async Task Member_Text_Carries_Hex_Or_Decimal_Token_Suffix_Per_DisplaySettings()
	{
		// Display Settings "Show metadata tokens" + "Show metadata tokens in base 10" must
		// reach the tree-node Text values. Mirrors WPF ILSpyTreeNode.GetSuffixString —
		// suffix is " @xNNNNNNNN" (hex, 8 digits) or " @NNNNNNNNN" (decimal) when enabled,
		// empty otherwise.

		var settings = AppComposition.Current.GetExport<SettingsService>().DisplaySettings;
		var (_, vm) = await TestHarness.BootAsync(3);

		// Drill into a known type and grab a method node.
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.IsExpanded = true;
		var method = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "AsEnumerable");

		try
		{
			settings.ShowMetadataTokens = false;
			((string)method.Text).Should().NotContain(" @",
				"with ShowMetadataTokens=false the Text must have no token suffix");

			settings.ShowMetadataTokens = true;
			settings.ShowMetadataTokensInBase10 = false;
			var hexText = (string)method.Text;
			hexText.Should().MatchRegex(@" @[0-9a-fA-F]{8}$",
				"with ShowMetadataTokens=true and Base10=false the suffix is ' @' + 8 hex digits");

			settings.ShowMetadataTokensInBase10 = true;
			var decText = (string)method.Text;
			decText.Should().MatchRegex(@" @\d+$",
				"with Base10=true the suffix switches to ' @' + decimal digits");
			decText.Should().NotMatchRegex(@" @[0-9a-fA-F]{8}$",
				"decimal form must not coincidentally match the hex regex (token > 16777215 makes them distinguishable)");
		}
		finally
		{
			settings.ShowMetadataTokens = false;
			settings.ShowMetadataTokensInBase10 = false;
		}
	}
}
