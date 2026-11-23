// Copyright (c) 2026 Piero Viano
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
using System.Globalization;
using System.Net;
using System.Text;

namespace ICSharpCode.ILSpyX.Dependencies
{
	/// <summary>
	/// Renders an <see cref="AssemblyDependencyGraph"/> as a Mermaid flowchart (<c>graph LR</c>),
	/// either as raw Mermaid text or as a self-contained HTML page that loads mermaid.js from the
	/// same CDN location the HTML class diagrammer (<c>MermaidDiagrammer/html/template.html</c>) uses.
	/// </summary>
	public static class DependencyGraphMermaidWriter
	{
		/// <summary>The mermaid.js build the generated page loads, shared with the class diagrammer.</summary>
		public const string MermaidScriptUrl = "https://cdn.jsdelivr.net/npm/mermaid@11.4.0/dist/mermaid.min.js";

		/// <summary>Mermaid node id of the node at <paramref name="index"/>.</summary>
		public static string NodeId(int index) => "n" + index.ToString(CultureInfo.InvariantCulture);

		/// <summary>
		/// Mermaid flowchart text for <paramref name="graph"/>: one quoted, escaped node per assembly
		/// (name and version), one arrow per reference (labelled with the referenced version when it
		/// differs from the resolved one), and style classes for root and unresolved nodes.
		/// </summary>
		public static string ToMermaid(AssemblyDependencyGraph graph)
		{
			ArgumentNullException.ThrowIfNull(graph);
			var sb = new StringBuilder();
			sb.Append("graph LR\n");
			foreach (var node in graph.Nodes)
			{
				string label = node.Version != null ? node.Name + " " + node.Version : node.Name;
				if (!node.IsResolved)
					label += " (unresolved)";
				sb.Append("    ").Append(NodeId(node.Index)).Append("[\"").Append(EscapeLabel(label)).Append("\"]\n");
			}
			foreach (var edge in graph.Edges)
			{
				sb.Append("    ").Append(NodeId(edge.From)).Append(" -->");
				if (edge.IsVersionMismatch && edge.ReferencedVersion != null)
					sb.Append("|\"").Append(EscapeLabel("v" + edge.ReferencedVersion)).Append("\"|");
				sb.Append(' ').Append(NodeId(edge.To)).Append('\n');
			}
			sb.Append("    classDef root font-weight:bold,stroke-width:3px;\n");
			sb.Append("    classDef unresolved fill:#fde2e2,stroke:#c0392b,stroke-dasharray:4 3,color:#c0392b;\n");
			foreach (var node in graph.Nodes)
			{
				if (node.IsRoot)
					sb.Append("    class ").Append(NodeId(node.Index)).Append(" root;\n");
				if (!node.IsResolved)
					sb.Append("    class ").Append(NodeId(node.Index)).Append(" unresolved;\n");
			}
			return sb.ToString();
		}

		/// <summary>
		/// A standalone HTML page that renders <paramref name="graph"/> with mermaid.js.
		/// <paramref name="title"/> is shown as the page title and heading.
		/// </summary>
		public static string ToHtml(AssemblyDependencyGraph graph, string title)
		{
			ArgumentNullException.ThrowIfNull(graph);
			ArgumentNullException.ThrowIfNull(title);
			string encodedTitle = WebUtility.HtmlEncode(title);
			var sb = new StringBuilder();
			sb.Append("<!DOCTYPE html>\n");
			sb.Append("<html lang=\"en\">\n<head>\n<meta charset=\"utf-8\">\n");
			sb.Append("<title>").Append(encodedTitle).Append("</title>\n");
			sb.Append("<style>body{font-family:sans-serif;margin:16px}pre.mermaid{background:transparent}</style>\n");
			sb.Append("</head>\n<body>\n");
			sb.Append("<h1>").Append(encodedTitle).Append("</h1>\n");
			// mermaid reads the element's text content, so HTML-encoding here is undone before parsing.
			sb.Append("<pre class=\"mermaid\">\n").Append(WebUtility.HtmlEncode(ToMermaid(graph))).Append("</pre>\n");
			sb.Append("<script src=\"").Append(MermaidScriptUrl).Append("\"></script>\n");
			sb.Append("<script>mermaid.initialize({ startOnLoad: true, maxTextSize: 1000000, maxEdges: 100000 });</script>\n");
			sb.Append("</body>\n</html>\n");
			return sb.ToString();
		}

		/// <summary>
		/// Escapes text for use inside a double-quoted Mermaid label. Mermaid decodes
		/// <c>#name;</c> / <c>#code;</c> entity codes, so every character that could end the label,
		/// open markup or start an entity is replaced by one; line breaks become spaces.
		/// </summary>
		public static string EscapeLabel(string text)
		{
			ArgumentNullException.ThrowIfNull(text);
			var sb = new StringBuilder(text.Length);
			foreach (char c in text)
			{
				switch (c)
				{
					case '#':
						sb.Append("#35;");
						break;
					case '"':
						sb.Append("#quot;");
						break;
					case '<':
						sb.Append("#lt;");
						break;
					case '>':
						sb.Append("#gt;");
						break;
					case '&':
						sb.Append("#amp;");
						break;
					case '|':
						sb.Append("#124;");
						break;
					case '`':
						sb.Append("#96;");
						break;
					case '\r':
					case '\n':
					case '\t':
						sb.Append(' ');
						break;
					default:
						sb.Append(c);
						break;
				}
			}
			return sb.ToString();
		}
	}
}
