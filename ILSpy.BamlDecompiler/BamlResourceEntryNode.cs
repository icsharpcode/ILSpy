// Copyright (c) 2020 AlphaSierraPapa for the SharpDevelop Team
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
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;

namespace ILSpy.BamlDecompiler
{
	public sealed class BamlResourceEntryNode : ResourceEntryNode
	{
		public BamlResourceEntryNode(string key, Func<Stream> data) : base(key, data)
		{
		}

		public override bool View(TabPageModel tabPage)
		{
			IHighlightingDefinition highlighting = null;

			tabPage.SupportsLanguageSwitching = false;
			tabPage.ShowTextView(textView => textView.RunWithCancellation(
				token => Task.Factory.StartNew(
					() => {
						AvalonEditTextOutput output = new AvalonEditTextOutput();
						try
						{
							LoadBaml(output, token);
							highlighting = HighlightingManager.Instance.GetDefinitionByExtension(".xml");
						}
						catch (Exception ex)
						{
							output.Write(ex.ToString());
						}
						return output;
					}, token))
				.Then(output => textView.ShowNode(output, this, highlighting))
				.HandleExceptions());
			return true;
		}

		void LoadBaml(AvalonEditTextOutput output, CancellationToken cancellationToken)
		{
			var asm = this.Ancestors().OfType<AssemblyTreeNode>().First().LoadedAssembly;
			using var data = OpenStream();
			BamlDecompilerTypeSystem typeSystem = new BamlDecompilerTypeSystem(asm.GetPEFileOrNull(), asm.GetAssemblyResolver());
			var decompiler = new XamlDecompiler(typeSystem, new BamlDecompilerSettings());
			decompiler.CancellationToken = cancellationToken;
			var result = decompiler.Decompile(data);
			output.Write(result.Xaml.ToString());
		}
	}
}
