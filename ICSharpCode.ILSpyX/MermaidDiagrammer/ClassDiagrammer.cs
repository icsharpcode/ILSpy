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

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpyX.MermaidDiagrammer
{
	/// <summary>Contains type info and metadata for generating a HTML class diagrammer from a source assembly.
	/// Serialized into JSON by <see cref="GenerateHtmlDiagrammer.SerializeModel(ClassDiagrammer)"/>.</summary>
	public sealed class ClassDiagrammer
	{
		internal const string NewLine = "\n";

		internal string SourceAssemblyName { get; set; } = null!;
		internal string SourceAssemblyVersion { get; set; } = null!;

		/// <summary>Types selectable in the diagrammer, grouped by their
		/// <see cref="System.Type.Namespace"/> to facilitate a structured type selection.</summary>
		internal Dictionary<string, Type[]> TypesByNamespace { get; set; } = null!;

		/// <summary>Types not included in the <see cref="ClassDiagrammer"/>,
		/// but referenced by <see cref="Type"/>s that are.
		/// Contains display names (values; similar to <see cref="Type.Name"/>)
		/// by their referenced IDs (keys; similar to <see cref="Type.Id"/>).</summary>
		internal Dictionary<string, string> OutsideReferences { get; set; } = null!;

		/// <summary>Types excluded from the <see cref="ClassDiagrammer"/>;
		/// used to support <see cref="GenerateHtmlDiagrammer.ReportExludedTypes"/>.</summary>
		internal string[] Excluded { get; set; } = null!;

		/// <summary>A <see cref="Type"/>-like structure with collections
		/// of property relations to one or many other <see cref="Type"/>s.</summary>
		public abstract class Relationships
		{
			/// <summary>Relations to zero or one other instances of <see cref="Type"/>s included in the <see cref="ClassDiagrammer"/>,
			/// with the display member names as keys and the related <see cref="Type.Id"/> as values.
			/// This is because member names must be unique within the owning <see cref="Type"/>,
			/// while the related <see cref="Type"/> may be the same for multiple properties.</summary>
			public Dictionary<string, string>? HasOne { get; set; }

			/// <summary>Relations to zero to infinite other instances of <see cref="Type"/>s included in the <see cref="ClassDiagrammer"/>,
			/// with the display member names as keys and the related <see cref="Type.Id"/> as values.
			/// This is because member names must be unique within the owning <see cref="Type"/>,
			/// while the related <see cref="Type"/> may be the same for multiple properties.</summary>
			public Dictionary<string, string>? HasMany { get; set; }
		}

		/// <summary>The mermaid class diagram definition, inheritance and relationships metadata
		/// and XML documentation for a <see cref="System.Type"/> from the source assembly.</summary>
		public sealed class Type : Relationships
		{
			/// <summary>Uniquely identifies the <see cref="System.Type"/> in the scope of the source assembly
			/// as well as any HTML diagrammer generated from it.
			/// Should match \w+ to be safe to use as select option value and
			/// part of the DOM id of the SVG node rendered for this type.
			/// May be the type name itself.</summary>
			internal string Id { get; set; } = null!;

			/// <summary>The human-readable label for the type, if different from <see cref="Id"/>.
			/// Not guaranteed to be unique in the scope of the <see cref="ClassDiagrammer"/>.</summary>
			public string? Name { get; set; }

			/// <summary>Contains the definition of the type and its own (not inherited) flat members
			/// in mermaid class diagram syntax, see https://mermaid.js.org/syntax/classDiagram.html .</summary>
			public string Body { get; set; } = null!;

			/// <summary>The base type directly implemented by this type, with the <see cref="Id"/> as key
			/// and the (optional) differing display name as value of the single entry
			/// - or null if the base type is <see cref="object"/>.
			/// Yes, Christopher Lambert, there can only be one. For now.
			/// But using the same interface as for <see cref="Interfaces"/> is convenient
			/// and who knows - at some point the .Net bus may roll up with multi-inheritance.
			/// Then this'll look visionary!</summary>
			public Dictionary<string, string?>? BaseType { get; set; }

			/// <summary>Interfaces directly implemented by this type, with their <see cref="Id"/> as keys
			/// and their (optional) differing display names as values.</summary>
			public Dictionary<string, string?[]>? Interfaces { get; set; }

			/// <summary>Contains inherited members by the <see cref="Id"/> of their <see cref="IMember.DeclaringType"/>
			/// for the consumer to choose which of them to display in an inheritance scenario.</summary>
			public IDictionary<string, InheritedMembers>? Inherited { get; set; }

			/// <summary>Contains the XML documentation comments for this type
			/// (using a <see cref="string.Empty"/> key) and its members, if available.</summary>
			public IDictionary<string, string>? XmlDocs { get; set; }

			/// <summary>Members inherited from an ancestor type specified by the Key of <see cref="Inherited"/>.</summary>
			public class InheritedMembers : Relationships
			{
				/// <summary>The simple, non-complex members inherited from another <see cref="Type"/>
				/// in mermaid class diagram syntax.</summary>
				public string? FlatMembers { get; set; }
			}
		}
	}
}