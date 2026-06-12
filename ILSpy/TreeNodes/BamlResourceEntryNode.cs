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

using System;
using System.IO;
using System.Linq;

using ICSharpCode.BamlDecompiler;
using ICSharpCode.Decompiler;

using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Baml
{
	/// <summary>
	/// Tree node for a single <c>.baml</c> resource entry inside a WPF/UWP assembly's
	/// resource file. Selecting the node decompiles the BAML stream into XAML on a
	/// background thread and emits the resulting XML into the editor (XML highlighting
	/// kicks in via <c>SyntaxExtensionOverride</c>).
	/// </summary>
	public sealed class BamlResourceEntryNode : ResourceEntryNode
	{
		public BamlResourceEntryNode(string key, Func<Stream> openStream)
			: base(key, openStream)
		{
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			try
			{
				LoadBaml(output, options);
				// Surface the rendered XAML as XML in the editor so the highlighter kicks in
				// and the foldings strategy collapses nested elements. The smart-output sink
				// reads this override before falling back to the language's FileExtension.
				if (output is AvaloniaEditTextOutput edit)
					edit.SyntaxExtensionOverride = ".xml";
			}
			catch (Exception ex)
			{
				language.WriteCommentLine(output, "BAML decompilation failed:");
				output.WriteLine(ex.ToString());
			}
		}

		void LoadBaml(ITextOutput output, DecompilationOptions options)
		{
			var assemblyNode = this.Ancestors().OfType<AssemblyTreeNode>().First();
			var assembly = assemblyNode.LoadedAssembly;
			using var data = OpenStream();
			var typeSystem = new BamlDecompilerTypeSystem(
				assembly.GetMetadataFileOrNull()!,
				assembly.GetAssemblyResolver());
			var decompiler = new XamlDecompiler(typeSystem, new BamlDecompilerSettings {
				ThrowOnAssemblyResolveErrors = options.DecompilerSettings.ThrowOnAssemblyResolveErrors,
			}) {
				CancellationToken = options.CancellationToken,
			};
			var result = decompiler.Decompile(data);
			output.Write(result.Xaml.ToString());
		}
	}
}
