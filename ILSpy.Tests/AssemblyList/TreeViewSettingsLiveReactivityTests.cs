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

using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class TreeViewSettingsLiveReactivityTests
{
	[AvaloniaTest]
	public async Task Toggling_ShowMetadataTokens_Raises_PropertyChanged_On_Tree_Node_Text()
	{
		// Live re-render contract: when the user flips Display Settings → "Show metadata
		// tokens" the visible tree nodes must re-fetch their Text (which now includes the
		// suffix). Verified by subscribing to a tree node's PropertyChanged and asserting
		// 'Text' fires when the setting flips.

		var settings = AppComposition.Current.GetExport<SettingsService>().DisplaySettings;
		var (_, vm) = await TestHarness.BootAsync(3);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");

		try
		{
			settings.ShowMetadataTokens = false;
			settings.ShowMetadataTokensInBase10 = false;

			int textFiredCount = 0;
			PropertyChangedEventHandler handler = (_, e) => {
				if (e.PropertyName == nameof(SharpTreeNode.Text))
					textFiredCount++;
			};
			typeNode.PropertyChanged += handler;

			try
			{
				settings.ShowMetadataTokens = true;
				textFiredCount.Should().BeGreaterThan(0,
					"flipping ShowMetadataTokens must raise PropertyChanged(Text) on visible tree nodes");

				int baseline = textFiredCount;
				settings.ShowMetadataTokensInBase10 = true;
				textFiredCount.Should().BeGreaterThan(baseline,
					"flipping Base10 must also raise PropertyChanged(Text)");
			}
			finally
			{
				typeNode.PropertyChanged -= handler;
			}
		}
		finally
		{
			settings.ShowMetadataTokens = false;
			settings.ShowMetadataTokensInBase10 = false;
		}
	}
}
