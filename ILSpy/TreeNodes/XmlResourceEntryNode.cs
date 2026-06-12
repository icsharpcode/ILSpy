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
using System.Composition;
using System.IO;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpyX.Abstractions;

using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.TextView;

namespace ICSharpCode.ILSpy.TreeNodes
{
	[Export(typeof(IResourceNodeFactory))]
	[Shared]
	sealed class XmlResourceNodeFactory : IResourceNodeFactory
	{
		static readonly string[] xmlFileExtensions = { ".xml", ".xsd", ".xslt" };

		public ITreeNode? CreateNode(Resource resource)
		{
			foreach (var ext in xmlFileExtensions)
			{
				if (resource.Name.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
					return new XmlResourceEntryNode(resource.Name, resource.TryOpenStream);
			}
			return null;
		}
	}

	sealed class XmlResourceEntryNode : ResourceEntryNode
	{
		public XmlResourceEntryNode(string key, Func<Stream?> openStream)
			: base(key, () => openStream() ?? Stream.Null)
		{
		}

		// TODO: ship Resource{Xml,Xsd,Xslt}.svg assets and pick by extension. Until then the
		// generic resource glyph is fine — the file name in the tree disambiguates.
		public override object Icon => Images.Resource;

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			using var data = OpenStream();
			if (data == Stream.Null)
			{
				output.WriteLine("ILSpy: Failed opening resource stream.");
				return;
			}
			using var reader = new StreamReader(data);
			output.Write(reader.ReadToEnd());
			if (output is AvaloniaEditTextOutput aeto)
			{
				// Force XML highlighting regardless of the active language. AvaloniaEdit's bundled
				// XmlHighlighting.xshd handles .xml / .xsd / .xslt identically.
				aeto.SyntaxExtensionOverride = ".xml";
			}
		}
	}
}
