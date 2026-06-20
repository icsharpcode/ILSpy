// Copyright (c) 2011-2013 AlphaSierraPapa for the SharpDevelop Team
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
using System.Diagnostics;

namespace ICSharpCode.Decompiler.CSharp.Syntax.PatternMatching
{
	/// <summary>
	/// Base class for all patterns.
	/// </summary>
	public abstract class Pattern : INode
	{
		/// <summary>
		/// Gets the string that matches any string.
		/// </summary>
		public static readonly string AnyString = "$any$";

		public static bool MatchString(string? pattern, string? text)
		{
			return pattern == AnyString || pattern == text;
		}

		internal struct PossibleMatch
		{
			public readonly int NextOtherIndex; // index of the next other element after the last matched one
			public readonly int Checkpoint; // checkpoint

			public PossibleMatch(int nextOtherIndex, int checkpoint)
			{
				this.NextOtherIndex = nextOtherIndex;
				this.Checkpoint = checkpoint;
			}
		}

		public abstract bool DoMatch(INode? other, Match match);

		public virtual bool DoMatchCollection(IReadOnlyList<INode> other, int pos, Match match, BacktrackingInfo backtrackingInfo)
		{
			return DoMatch(pos < other.Count ? other[pos] : null, match);
		}

		/// <summary>
		/// Matches the <paramref name="patternChildren"/> collection against the
		/// <paramref name="otherChildren"/> collection, with backtracking over Repeat/OptionalNode.
		/// Each collection is already the per-slot child list, so the match walks them by index.
		/// </summary>
		public static bool DoMatchCollection(IReadOnlyList<INode> patternChildren, IReadOnlyList<INode> otherChildren, Match match)
		{
			BacktrackingInfo backtrackingInfo = new BacktrackingInfo();
			Stack<int> patternStack = new Stack<int>();
			Stack<PossibleMatch> stack = backtrackingInfo.backtrackingStack;
			patternStack.Push(0);
			stack.Push(new PossibleMatch(0, match.CheckPoint()));
			while (stack.Count > 0)
			{
				int patternPos = patternStack.Pop();
				int otherPos = stack.Peek().NextOtherIndex;
				match.RestoreCheckPoint(stack.Pop().Checkpoint);
				bool success = true;
				while (patternPos < patternChildren.Count && success)
				{
					INode cur1 = patternChildren[patternPos];
					Debug.Assert(stack.Count == patternStack.Count);
					success = cur1.DoMatchCollection(otherChildren, otherPos, match, backtrackingInfo);
					Debug.Assert(stack.Count >= patternStack.Count);
					// For every alternative the pattern node pushed, queue the next pattern node so the
					// continuation resumes after it.
					while (stack.Count > patternStack.Count)
						patternStack.Push(patternPos + 1);

					patternPos++;
					otherPos++;
				}
				if (success && otherPos >= otherChildren.Count)
					return true;
			}
			return false;
		}
	}
}
