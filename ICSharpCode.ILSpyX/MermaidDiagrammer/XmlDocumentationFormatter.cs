// Copyright (c) 2024 Holger Schmidt
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

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using ICSharpCode.Decompiler.Documentation;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX.MermaidDiagrammer.Extensions;

namespace ICSharpCode.ILSpyX.MermaidDiagrammer
{
	/// <summary>Wraps the <see cref="IDocumentationProvider"/> to prettify XML documentation comments.
	/// Make sure to enable XML documentation output, see
	/// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/xmldoc/#create-xml-documentation-output .</summary>
	public class XmlDocumentationFormatter
	{
		/// <summary>Matches XML indent.</summary>
		protected const string linePadding = @"^[ \t]+|[ \t]+$";

		/// <summary>Matches reference tags including "see href", "see cref" and "paramref name"
		/// with the cref value being prefixed by symbol-specific letter and a colon
		/// including the quotes around the attribute value and the closing slash of the tag containing the attribute.</summary>
		protected const string referenceAttributes = @"(see\s.ref=""(.:)?)|(paramref\sname="")|(""\s/)";

		private readonly IDocumentationProvider docs;
		private readonly Regex noiseAndPadding;

		public XmlDocumentationFormatter(IDocumentationProvider docs, string[]? strippedNamespaces)
		{
			this.docs = docs;
			List<string> regexes = new() { linePadding, referenceAttributes };

			if (strippedNamespaces?.Length > 0)
				regexes.AddRange(strippedNamespaces.Select(ns => $"({ns.Replace(".", "\\.")}\\.)"));

			noiseAndPadding = new Regex(regexes.Join("|"), RegexOptions.Multiline); // builds an OR | combined regex
		}

		internal Dictionary<string, string>? GetXmlDocs(ITypeDefinition type, params IMember[][] memberCollections)
		{
			Dictionary<string, string>? docs = new();
			AddXmlDocEntry(docs, type);

			foreach (IMember[] members in memberCollections)
			{
				foreach (IMember member in members)
					AddXmlDocEntry(docs, member);
			}

			return docs?.Keys.Count != 0 ? docs : default;
		}

		protected virtual string? GetDoco(IEntity entity)
		{
			string? comment = docs.GetDocumentation(entity)?
				.ReplaceAll(["<summary>", "</summary>"], null)
				.ReplaceAll(["<para>", "</para>"], ClassDiagrammer.NewLine).Trim() // to format
				.Replace('<', '[').Replace('>', ']'); // to prevent ugly escaped output

			return comment == null ? null : noiseAndPadding.Replace(comment, string.Empty).NormalizeHorizontalWhiteSpace();
		}

		private void AddXmlDocEntry(Dictionary<string, string> docs, IEntity entity)
		{
			string? doc = GetDoco(entity);

			if (string.IsNullOrEmpty(doc))
				return;

			string key = entity is IMember member ? member.Name : string.Empty;
			docs[key] = doc;
		}
	}
}