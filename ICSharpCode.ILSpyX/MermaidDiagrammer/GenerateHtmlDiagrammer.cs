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

namespace ICSharpCode.ILSpyX.MermaidDiagrammer
{
	/// <summary>The command for creating an HTML5 diagramming app with an API optimized for binding command line parameters.
	/// To use it outside of that context, set its properties and call <see cref="Run"/>.</summary>
	public partial class GenerateHtmlDiagrammer
	{
		internal const string RepoUrl = "https://github.com/icsharpcode/ILSpy";

		public required string Assembly { get; set; }
		public string? OutputFolder { get; set; }

		public string? Include { get; set; }
		public string? Exclude { get; set; }
		public bool JsonOnly { get; set; }
		public bool ReportExludedTypes { get; set; }
		public string? XmlDocs { get; set; }

		/// <summary>Namespaces to strip from <see cref="XmlDocs"/>.
		/// Implemented as a list of exact replacements instead of a single, more powerful RegEx because replacement in
		/// <see cref="XmlDocumentationFormatter.GetDoco(Decompiler.TypeSystem.IEntity)"/>
		/// happens on the unstructured string where matching and replacing the namespaces of referenced types, members and method parameters
		/// using RegExes would add a lot of complicated RegEx-heavy code for a rather unimportant feature.</summary>
		public IEnumerable<string>? StrippedNamespaces { get; set; }
	}
}