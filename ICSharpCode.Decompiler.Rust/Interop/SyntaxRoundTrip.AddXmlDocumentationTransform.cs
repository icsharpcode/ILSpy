// Copyright (c) 2026 Christoph Wille
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

#nullable enable

using System.IO;
using System.Xml;

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.Rust.Interop
{
	/// <summary>
	/// The oracle answers of the engine's <c>AddXmlDocumentationTransform</c>
	/// (<c>csharp/transforms/add_xml_documentation.rs</c>, which documents the kinds); the
	/// documentation provider of the decompile run stays here.
	/// </summary>
	partial class SyntaxRoundTrip
	{
		const byte QueryXmlDocumentationEnabled = 238;
		const byte QueryXmlDocumentation = 239;

		/// <summary>The answer to a query of <c>AddXmlDocumentationTransform</c>; null where it does not apply.</summary>
		byte[]? AnswerAddXmlDocumentation(byte[] query)
		{
			var provider = context?.Settings.ShowXmlDocumentation == true ? context.DecompileRun.DocumentationProvider : null;
			if (query[0] == QueryXmlDocumentationEnabled)
				return new[] { provider != null ? (byte)1 : (byte)0 };
			int pos = 1;
			int id = (int)ReadUVar(query, ref pos);
			if (provider == null || id < 0 || id >= symbols.Count)
				return null;
			if (symbols[id] is not IEntity entity)
				return new byte[] { 0 };
			var s = new MemoryStream();
			try
			{
				string? doc = provider.GetDocumentation(entity);
				if (doc == null && entity is IMethod { AccessorOwner: IProperty owner } accessor
					&& owner.IsParameterizedProperty() && accessor.Equals(owner.Getter ?? owner.Setter))
				{
					doc = provider.GetDocumentation(owner);
				}
				if (doc == null)
					return new byte[] { 0 };
				s.WriteByte(1);
				WriteUtf16(s, doc);
			}
			catch (XmlException ex)
			{
				s.WriteByte(2);
				WriteUtf16(s, " Exception while reading XmlDoc: " + ex);
			}
			return s.ToArray();
		}
	}
}
