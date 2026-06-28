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
			public IList<object> ModifiedNodeCandidates { get; } = new List<object>();
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
			// Record the IL position and its ancestor chain as highlight candidates here, before
			// the limit-reached check below can throw: the debug-step view halts the pipeline at the
			// selected step, so that step would otherwise carry no candidates and the "show state
			// before" view could not locate the change. A later transform may detach the exact
			// instruction, but a surviving ancestor (ultimately the ILFunction) still resolves.
			for (var node = near; node != null; node = node.Parent)
			{
				stepNode.ModifiedNodeCandidates.Add(node);
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
