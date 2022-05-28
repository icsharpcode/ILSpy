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

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX.Abstractions;

namespace ICSharpCode.ILSpyX.Search
{
	public interface ISearchResultFactory
	{
		MemberSearchResult Create(IEntity entity);
		ResourceSearchResult Create(PEFile module, Resource resource, ITreeNode node, ITreeNode parent);
		AssemblySearchResult Create(PEFile module);
		NamespaceSearchResult Create(PEFile module, INamespace @namespace);
	}

	public class SearchResult
	{
		public static readonly IComparer<SearchResult> ComparerByName = new SearchResultNameComparer();
		public static readonly IComparer<SearchResult> ComparerByFitness = new SearchResultFitnessComparer();

		public virtual object? Reference => null;

		public float Fitness { get; set; }

		public string Name { get; set; }
		public string Location { get; set; }
		public string Assembly { get; set; }
		public object? ToolTip { get; set; }
		public object Image { get; set; }
		public object LocationImage { get; set; }

		public object AssemblyImage { get; set; }

		public override string ToString()
		{
			return Name;
		}

		class SearchResultNameComparer : IComparer<SearchResult>
		{
			public int Compare(SearchResult? x, SearchResult? y)
			{
				return StringComparer.Ordinal.Compare(x?.Name ?? "", y?.Name ?? "");
			}
		}

		class SearchResultFitnessComparer : IComparer<SearchResult>
		{
			public int Compare(SearchResult? x, SearchResult? y)
			{
				//elements with higher Fitness come first
				return Comparer<float>.Default.Compare(y?.Fitness ?? 0, x?.Fitness ?? 0);
			}
		}
	}

	public class MemberSearchResult : SearchResult
	{
		public IEntity Member { get; set; }
		public override object Reference => Member;
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
	}

	public class NamespaceSearchResult : SearchResult
	{
		public INamespace Namespace { get; set; }
		public override object Reference => Namespace;
	}
}