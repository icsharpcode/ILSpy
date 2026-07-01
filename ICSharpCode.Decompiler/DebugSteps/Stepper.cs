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

namespace ICSharpCode.Decompiler.DebugSteps
{
	/// <summary>
	/// Exception thrown when a debug-step-enabled transform pipeline runs into the <see cref="Stepper.StepLimit"/>.
	/// </summary>
	public class StepLimitReachedException : Exception
	{
	}

	/// <summary>
	/// Language-specific node identities and navigation data captured before a transform mutation.
	/// </summary>
	public readonly struct DebugStepNodeInfo
	{
		public object Node { get; }
		public object? NextSibling { get; }
		public object? PreviousSibling { get; }
		public IEnumerable<object>? Ancestors { get; }
		public object? ExtraIdentity { get; }

		public DebugStepNodeInfo(
			object node,
			object? nextSibling = null,
			object? previousSibling = null,
			IEnumerable<object>? ancestors = null,
			object? extraIdentity = null)
		{
			Node = node ?? throw new ArgumentNullException(nameof(node));
			NextSibling = nextSibling;
			PreviousSibling = previousSibling;
			Ancestors = ancestors;
			ExtraIdentity = extraIdentity;
		}
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
			public object? Position { get; set; }
			public object? ModifiedNode { get; set; }
			/// <summary>
			/// Precise identities of the changed node (the node itself, its debug-step marker, and
			/// any node its mutation produced). Resolved first when highlighting the step.
			/// </summary>
			public IList<object> ModifiedNodeCandidates { get; } = new List<object>();
			/// <summary>
			/// Neighbors of the changed node, recorded before the mutation. When the node itself is
			/// gone from the rendered text (a removal), a caret is placed at the gap they leave:
			/// at the start of a following neighbor, or the end of a preceding one (AtEnd).
			/// </summary>
			public IList<(object Anchor, bool AtEnd)> SeamAnchors { get; } = new List<(object, bool)>();
			/// <summary>
			/// The changed node's ancestor chain, resolved as a last resort when neither the node
			/// nor a seam neighbor has a rendered range.
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
			/// neighbors because each transform representation exposes navigation differently;
			/// the ordering/dedup/seam strategy lives here so languages'
			/// recording can't drift. <paramref name="insertFirst"/> marks a produced-node update
			/// (from EndStep): the node is preferred over existing candidates and the seam and
			/// ancestor anchors are left untouched, since they were captured from the original node.
			/// </summary>
			public void RecordModifiedNode(DebugStepNodeInfo modifiedNode, bool insertFirst = false)
			{
				RecordModifiedNode(
					modifiedNode.Node,
					modifiedNode.NextSibling,
					modifiedNode.PreviousSibling,
					modifiedNode.Ancestors,
					modifiedNode.ExtraIdentity,
					insertFirst);
			}

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
		public Node Step(string description, DebugStepNodeInfo? modifiedNode = null)
		{
			object? node = modifiedNode?.Node;
			var stepNode = new Node($"{step}: {description}") {
				Position = node,
				ModifiedNode = node,
				BeginStep = step,
				EndStep = step + 1
			};
			// Record highlight candidates before the limit-reached check below can throw: the debug-step
			// view halts the pipeline at the selected step, so that step would otherwise carry no
			// candidates and the "show state before" view could not locate the change.
			if (modifiedNode is { } info)
			{
				stepNode.RecordModifiedNode(info);
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
		public Node StartGroup(string description, DebugStepNodeInfo? modifiedNode = null)
		{
			var stepNode = Step(description, modifiedNode);
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

	/// <summary>
	/// Annotation attached to a step's modified node so the debug-step highlighter can still locate
	/// that node's rendered range after a later transform copies its annotations onto a replacement
	/// (via CopyAnnotationsFrom). This is the only annotation the UI bridges to a text range; indexing
	/// every annotation would add dead keys and let shared ones collide by identity.
	/// </summary>
	public sealed class DebugStepMarker
	{
	}
}
