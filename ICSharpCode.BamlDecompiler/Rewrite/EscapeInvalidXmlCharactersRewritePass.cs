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

using System.Linq;
using System.Xml.Linq;

using ICSharpCode.BamlDecompiler.Xaml;

namespace ICSharpCode.BamlDecompiler.Rewrite
{
	/// <summary>
	/// Escapes attribute values, text and comments that carry characters XML cannot represent -
	/// a string record from an obfuscated assembly may hold any byte sequence. Without this the
	/// document builds fine and only throws when it is written, taking the resource with it.
	/// Names and namespace URIs are escaped where they are built, so this pass only has to cover
	/// the content.
	/// </summary>
	internal class EscapeInvalidXmlCharactersRewritePass : IRewritePass
	{
		public void Run(XamlContext ctx, XDocument document)
		{
			foreach (var element in document.Descendants())
			{
				foreach (var attribute in element.Attributes())
				{
					// Namespace declarations carry a URI that was escaped when the XNamespace was
					// created; rewriting it here would desync it from the names using it.
					if (!attribute.IsNamespaceDeclaration)
						attribute.Value = XamlUtils.EscapeInvalidXmlCharacters(attribute.Value);
				}
			}

			foreach (var node in document.DescendantNodes().ToList())
			{
				switch (node)
				{
					case XText text:
						text.Value = XamlUtils.EscapeInvalidXmlCharacters(text.Value);
						break;
					case XComment comment:
						comment.Value = XamlUtils.EscapeInvalidXmlCharacters(comment.Value);
						break;
				}
			}
		}
	}
}
