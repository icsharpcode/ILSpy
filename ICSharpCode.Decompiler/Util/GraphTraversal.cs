// Copyright (c) 2023 Daniel Grunwald
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

using System;
using System.Collections.Generic;

namespace ICSharpCode.Decompiler.Util;

static class GraphTraversal
{
	/// <summary>
	/// Depth-first-search of an graph data structure.
	/// The two callbacks (successorFunc + postorderAction) will be called exactly once for each node reachable from startNodes.
	/// </summary>
	/// <param name="startNodes">The start nodes.</param>
	/// <param name="visitedFunc">Called multiple times per node. The first call should return true, subsequent calls must return false.
	/// The first calls to this function occur in pre-order.
	/// If null, normal Equals/GetHashCode will be used to compare nodes.</param>
	/// <param name="successorFunc">The function that gets the successors of an element. Called in pre-order.</param>
	/// <param name="postorderAction">Called in post-order.</param>
	/// <param name="reverseSuccessors">
	/// With reverse_successors=True, the start_nodes and each list of successors will be handled in reverse order.
	/// This is useful if the post-order will be reversed later (e.g. for a topological sort)
	/// so that blocks which could be output in either order (e.g. then-block and else-block of an if)
	/// will maintain the order of the edges (then-block before else-block).
	/// </param>
	public static void DepthFirstSearch<T>(IEnumerable<T> startNodes, Func<T, bool>? visitedFunc, Func<T, IEnumerable<T>?> successorFunc, Action<T>? postorderAction = null, bool reverseSuccessors = false)
	{
		/*
        Pseudocode:
            def dfs_walk(start_nodes, successor_func, visited, postorder_func, reverse_successors):
                if reverse_successors:
                    start_nodes = reversed(start_nodes)
                for node in start_nodes:
                    if node in visited: continue
                    visited.insert(node)
                    children = successor_func(node)
                    dfs_walk(children, successor_func, visited, postorder_action, reverse_successors)
                    postorder_action(node)
        
        The actual implementation here is equivalent but does not use recursion,
        so that we don't blow the stack on large graphs.
        A single stack holds the "continuations" of work that needs to be done.
        These can be either "visit continuations" (=execute the body of the pseudocode
        loop for the given node) or "postorder continuations" (=execute postorder_action)
        */
		// Use a List as stack (but allowing for the Reverse() usage)
		var worklist = new List<(T node, bool isPostOrderContinuation)>();
		visitedFunc ??= new HashSet<T>().Add;
		foreach (T node in startNodes)
		{
			worklist.Add((node, false));
		}
		if (!reverseSuccessors)
		{
			// Our use of a stack will reverse the order of the nodes.
			// If that's not desired, restore original order by reversing twice.
			worklist.Reverse();
		}
		// Process outstanding continuations:
		while (worklist.Count > 0)
		{
			var (node, isPostOrderContinuation) = worklist.Last();
			if (isPostOrderContinuation)
			{
				// Execute postorder_action
				postorderAction?.Invoke(node);
				worklist.RemoveAt(worklist.Count - 1);
				continue;
			}
			// Execute body of loop
			if (!visitedFunc(node))
			{
				// Already visited
				worklist.RemoveAt(worklist.Count - 1);
				continue;
			}
			// foreach-loop-iteration will end with postorder_func call,
			// so switch the type of continuation for this node
			int oldWorkListSize = worklist.Count;
			worklist[oldWorkListSize - 1] = (node, true);
			// Create "visit continuations" for all successor nodes:
			IEnumerable<T>? children = successorFunc(node);
			if (children != null)
			{
				foreach (T child in children)
				{
					worklist.Add((child, false));
				}
			}
			// Our use of a stack will reverse the order of the nodes.
			// If that's not desired, restore original order by reversing twice.
			if (!reverseSuccessors)
			{
				worklist.Reverse(oldWorkListSize, worklist.Count - oldWorkListSize);
			}
		}
	}
}
