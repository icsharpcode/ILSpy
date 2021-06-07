// Copyright (c) 2014 Daniel Grunwald
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

using System.Collections.Generic;

namespace ICSharpCode.Decompiler.Util
{
	/// <summary>
	/// Union-Find data structure.
	/// </summary>
	public class UnionFind<T>
	{
		Dictionary<T, Node> mapping;

		class Node
		{
			public int rank;
			public Node parent;
			public T value;

			internal Node(T value)
			{
				this.value = value;
				this.parent = this;
			}
		}

		public UnionFind()
		{
			mapping = new Dictionary<T, Node>();
		}

		Node GetNode(T element)
		{
			Node node;
			if (!mapping.TryGetValue(element, out node))
			{
				node = new Node(element);
				node.parent = node;
				mapping.Add(element, node);
			}
			return node;
		}

		public T Find(T element)
		{
			return FindRoot(GetNode(element)).value;
		}

		Node FindRoot(Node node)
		{
			if (node.parent != node)
				node.parent = FindRoot(node.parent);
			return node.parent;
		}

		public void Merge(T a, T b)
		{
			var rootA = FindRoot(GetNode(a));
			var rootB = FindRoot(GetNode(b));
			if (rootA == rootB)
				return;
			if (rootA.rank < rootB.rank)
				rootA.parent = rootB;
			else if (rootA.rank > rootB.rank)
				rootB.parent = rootA;
			else
			{
				rootB.parent = rootA;
				rootA.rank++;
			}
		}
	}
}
