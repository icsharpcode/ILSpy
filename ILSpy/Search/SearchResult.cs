// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.Collections.Generic;
using System.Windows.Media;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.Search;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy
{
	public class SearchResult
	{
		public static readonly IComparer<SearchResult> Comparer = new SearchResultComparer();

		public virtual object Reference {
			get {
				return null;
			}
		}

		public float Fitness { get; set; }

		public string Name { get; set; }
		public string Location { get; set; }
		public string Assembly { get; set; }
		public object ToolTip { get; set; }
		public virtual ImageSource Image { get; set; }
		public virtual ImageSource LocationImage { get; set; }

		public ImageSource AssemblyImage {
			get {
				return Images.Assembly;
			}
		}

		public override string ToString()
		{
			return Name;
		}

		class SearchResultComparer : IComparer<SearchResult>
		{
			public int Compare(SearchResult x, SearchResult y)
			{
				return StringComparer.Ordinal.Compare(x?.Name ?? "", y?.Name ?? "");
			}
		}
	}

	public class MemberSearchResult : SearchResult
	{
		public IEntity Member { get; set; }
		public override object Reference => Member;

		public override ImageSource Image {
			get {
				if (base.Image == null)
				{
					base.Image = AbstractEntitySearchStrategy.GetIcon(Member);
				}
				return base.Image;
			}
		}

		public override ImageSource LocationImage {
			get {
				if (base.LocationImage == null)
				{
					base.LocationImage = Member.DeclaringTypeDefinition != null ? TypeTreeNode.GetIcon(Member.DeclaringTypeDefinition) : Images.Namespace;
				}
				return base.LocationImage;
			}
		}
	}

	public class ResourceSearchResult : SearchResult
	{
		public Resource Resource { get; set; }
		public override object Reference => ValueTuple.Create(Resource, Name);
	}

	public class AssemblySearchResult : SearchResult
	{
		public PEFile Module { get; set; }
		public override object Reference => Module;

		public override ImageSource Image => Images.Assembly;
		public override ImageSource LocationImage => Images.Library;
	}

	public class NamespaceSearchResult : SearchResult
	{
		public INamespace Namespace { get; set; }
		public override object Reference => Namespace;

		public override ImageSource Image => Images.Namespace;
		public override ImageSource LocationImage => Images.Assembly;
	}
}