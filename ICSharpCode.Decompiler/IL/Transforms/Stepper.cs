// Copyright (c) 2016 Daniel Grunwald
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
using System.Diagnostics;
using System.Linq;

using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Exception thrown when an IL transform runs into the <see cref="Stepper.StepLimit"/>.
	/// </summary>
	public class StepLimitReachedException : Exception
	{
	}

	/// <summary>
	/// Helper class that manages recording transform steps.
	/// </summary>
	public class Stepper
	{
		/// <summary>
		/// Gets whether stepping of built-in transforms is supported in this build of ICSharpCode.Decompiler.
		/// Usually only debug builds support transform stepping.
		/// </summary>
		public static bool SteppingAvailable {
			get {
#if STEP
				return true;
#else
				return false;
#endif
			}
		}

		public IList<Node> Steps => steps;
		public Node? LimitReachedStep { get; private set; }
		public Node? LastStep { get; private set; }

		public int StepLimit { get; set; } = int.MaxValue;
		public bool IsDebug { get; set; }

		public class Node
		{
			public string Description { get; }
			public ILInstruction? Position { get; set; }
			public object? ModifiedNode { get; set; }
			/// <summary>
			/// Precise identities of the changed node (the node itself, its debug-step marker, and
			/// any node its mutation produced). Resolved first when highlighting the step.
			/// </summary>
			public IList<object> ModifiedNodeCandidates { get; } = new List<object>();
			/// <summary>
			/// Neighbours of the changed node, recorded before the mutation. When the node itself is
			/// gone from the rendered text (a removal), a caret is placed at the gap they leave:
			/// at the start of a following neighbour, or the end of a preceding one (AtEnd).
			/// </summary>
			public IList<(object Anchor, bool AtEnd)> SeamAnchors { get; } = new List<(object, bool)>();
			/// <summary>
			/// The changed node's ancestor chain, resolved as a last resort when neither the node
			/// nor a seam neighbour has a rendered range.
			/// </summary>
			public IList<object> AncestorCandidates { get; } = new List<object>();
			/// <summary>
			/// BeginStep is inclusive.
			/// </summary>
			public int BeginStep { get; set; }
			/// <summary>
			/// EndStep is exclusive.
			/// </summary>
			public int EndStep { get; set; }

			public IList<Node> Children { get; } = new List<Node>();

			public Node(string description)
			{
				Description = description;
			}

			/// <summary>
			/// Records the highlight candidates for this step from an already-navigated node: the
			/// node's identity (optionally a second identity such as a debug-step marker), its
			/// immediate siblings as seam anchors, and its ancestor chain. Callers pass the
			/// neighbours because <see cref="ILInstruction"/> and the C# AST expose navigation
			/// differently; the ordering/dedup/seam strategy lives here so the two languages'
			/// recording can't drift. <paramref name="insertFirst"/> marks a produced-node update
			/// (from EndStep): the node is preferred over existing candidates and the seam and
			/// ancestor anchors are left untouched, since they were captured from the original node.
			/// </summary>
			public void RecordModifiedNode(
				object node,
				object? nextSibling = null,
				object? previousSibling = null,
				IEnumerable<object>? ancestors = null,
				object? extraIdentity = null,
				bool insertFirst = false)
			{
				AddCandidate(node, insertFirst);
				if (extraIdentity != null)
					AddCandidate(extraIdentity, insertFirst: false);
				if (insertFirst)
					return;
				if (nextSibling != null)
					SeamAnchors.Add((nextSibling, false));
				if (previousSibling != null)
					SeamAnchors.Add((previousSibling, true));
				if (ancestors != null)
				{
					foreach (var ancestor in ancestors)
						AncestorCandidates.Add(ancestor);
				}
			}

			void AddCandidate(object candidate, bool insertFirst)
			{
				if (ModifiedNodeCandidates.Contains(candidate))
					return;
				if (insertFirst)
					ModifiedNodeCandidates.Insert(0, candidate);
				else
					ModifiedNodeCandidates.Add(candidate);
			}
		}

		readonly Stack<Node> groups;
		readonly IList<Node> steps;
		int step = 0;

		public Stepper()
		{
			steps = new List<Node>();
			groups = new Stack<Node>();
		}

		/// <summary>
		/// Call this method immediately before performing a transform step.
		/// Used for debugging the IL transforms. Has no effect in release mode.
		/// 
		/// May throw <see cref="StepLimitReachedException"/> in debug mode.
		/// </summary>
		[DebuggerStepThrough]
		public Node Step(string description, ILInstruction? near = null, object? modifiedNode = null)
		{
			return StepInternal(description, near, modifiedNode ?? near);
		}

		[DebuggerStepThrough]
		private Node StepInternal(string description, ILInstruction? near, object? modifiedNode)
		{
			var stepNode = new Node($"{step}: {description}") {
				Position = near,
				ModifiedNode = modifiedNode,
				BeginStep = step,
				EndStep = step + 1
			};
			// Record the IL position, its neighbours, and its ancestor chain as highlight candidates
			// here, before the limit-reached check below can throw: the debug-step view halts the
			// pipeline at the selected step, so that step would otherwise carry no candidates and the
			// "show state before" view could not locate the change. A later transform may detach the
			// exact instruction; a surviving neighbour then anchors a seam caret, and failing that a
			// surviving ancestor (ultimately the ILFunction) still resolves.
			if (near != null)
			{
				object? next = null, prev = null;
				if (near.Parent is { } parent)
				{
					int index = near.ChildIndex;
					if (index + 1 < parent.Children.Count)
						next = parent.Children[index + 1];
					if (index - 1 >= 0)
						prev = parent.Children[index - 1];
				}
				stepNode.RecordModifiedNode(near, next, prev, Ancestors(near));

				static IEnumerable<object> Ancestors(ILInstruction inst)
				{
					for (var node = inst.Parent; node != null; node = node.Parent)
						yield return node;
				}
			}
			if (step == StepLimit)
			{
				LimitReachedStep = stepNode;
				if (IsDebug)
					Debugger.Break();
				else
					throw new StepLimitReachedException();
			}
			var p = groups.PeekOrDefault();
			if (p != null)
				p.Children.Add(stepNode);
			else
				steps.Add(stepNode);
			LastStep = stepNode;
			step++;
			return stepNode;
		}

		[DebuggerStepThrough]
		public Node StartGroup(string description, ILInstruction? near = null, object? modifiedNode = null)
		{
			var stepNode = StepInternal(description, near, modifiedNode ?? near);
			groups.Push(stepNode);
			return stepNode;
		}

		public Node? GetStepByBeginStep(int beginStep)
		{
			return FindStep(steps, beginStep);

			static Node? FindStep(IEnumerable<Node> nodes, int beginStep)
			{
				foreach (var node in nodes)
				{
					if (node.BeginStep == beginStep)
						return node;
					var child = FindStep(node.Children, beginStep);
					if (child != null)
						return child;
				}
				return null;
			}
		}

		public void EndGroup(bool keepIfEmpty = false)
		{
			var node = groups.Pop();
			if (!keepIfEmpty && node.Children.Count == 0)
			{
				var col = groups.PeekOrDefault()?.Children ?? steps;
				Debug.Assert(col.Last() == node);
				col.RemoveAt(col.Count - 1);
			}
			node.EndStep = step;
		}
	}
}
