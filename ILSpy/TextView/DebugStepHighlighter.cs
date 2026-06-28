// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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

using ICSharpCode.Decompiler.IL.Transforms;

namespace ICSharpCode.ILSpy.TextView
{
	/// <summary>
	/// Maps a debug-stepper step to the character range of the node it changed, using the
	/// <see cref="NodeLookup"/> built while rendering. Language-agnostic: it resolves a step's
	/// <see cref="Stepper.Node.ModifiedNodeCandidates"/> (and <see cref="Stepper.Node.ModifiedNode"/>)
	/// against the lookup, so it serves both the C# AST and ILAst views.
	/// </summary>
	internal static class DebugStepHighlighter
	{
		/// <summary>
		/// Resolves the range to highlight for a step-limited decompile. <paramref name="stepLimit"/>
		/// and <paramref name="highlightStep"/> come from the re-decompile request; the step is
		/// looked up in <paramref name="stepper"/> (the instance the transforms populated).
		/// Returns false for a full run (no step limit) or when no candidate has a recorded range.
		/// </summary>
		public static bool TryResolve(Stepper stepper, int stepLimit, int? highlightStep, NodeLookup nodeLookup, out TextRange range)
		{
			range = default;
			if (stepLimit == int.MaxValue)
				return false;
			if (highlightStep is { } step)
			{
				if (TryGetRange(stepper.GetStepByBeginStep(step), nodeLookup, out range))
					return true;
				if (stepper.LimitReachedStep is { BeginStep: var limitStep } reachedStep
					&& limitStep == step)
				{
					return TryGetRange(reachedStep, nodeLookup, out range);
				}
				return false;
			}
			if (TryGetRange(stepper.LimitReachedStep, nodeLookup, out range))
				return true;
			if (stepLimit > 0)
				return TryGetRange(stepper.GetStepByBeginStep(stepLimit - 1), nodeLookup, out range);
			return false;
		}

		static bool TryGetRange(Stepper.Node? step, NodeLookup nodeLookup, out TextRange range)
		{
			range = default;
			if (step == null)
				return false;
			foreach (var candidate in step.ModifiedNodeCandidates)
			{
				if (nodeLookup.TryGetRange(candidate, out range))
					return true;
			}
			return step.ModifiedNode != null && nodeLookup.TryGetRange(step.ModifiedNode, out range);
		}
	}
}
