#nullable enable
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

using System.Collections.Generic;

namespace ICSharpCode.Decompiler.IL.Patterns
{
	/// <summary>
	/// Data holder for a single list matching operation.
	/// </summary>
	/// <remarks>
	/// Notes on backtracking:
	/// PerformMatch() may save backtracking-savepoints to the ListMatch instance.
	/// Each backtracking-savepoints is a Stack{int} with instructions of how to restore the saved state.
	/// When a savepoint is created by a PerformMatch() call, that call may be nested in several other PerformMatch() calls that operate on
	/// the same list.
	/// When leaving those calls (whether with a successful match or not), the outer PerformMatch() calls may push additional state onto
	/// all of the added backtracking-savepoints.
	/// When the overall list match fails but savepoints exists, the most recently added savepoint is restored by calling PerformMatch()
	/// with listMatch.restoreStack set to that savepoint. Each PerformMatch() call must pop its state from that stack before
	/// recursively calling its child patterns.
	/// </remarks>
	public struct ListMatch
	{
		/// <summary>
		/// The main list matching logic.
		/// </summary>
		/// <returns>Returns whether the list match was successful.
		/// If the method returns true, it adds the capture groups (if any) to the match.
		/// If the method returns false, the match object remains in a partially-updated state and needs to be restored
		/// before it can be reused.</returns>
		internal static bool DoMatch(IReadOnlyList<ILInstruction> patterns, IReadOnlyList<ILInstruction?> syntaxList, ref Match match)
		{
			ListMatch listMatch = new ListMatch(syntaxList);
			do
			{
				if (PerformMatchSequence(patterns, ref listMatch, ref match))
				{
					// If we have a successful match and it matches the whole list,
					// we are done.
					if (listMatch.SyntaxIndex == syntaxList.Count)
						return true;
				}
				// Otherwise, restore a savepoint created by PerformMatch() and resume the matching logic at that savepoint.
			} while (listMatch.RestoreSavePoint(ref match));
			return false;
		}

		/// <summary>
		/// PerformMatch() for a sequence of patterns.
		/// </summary>
		/// <param name="patterns">List of patterns to match.</param>
		/// <param name="listMatch">Stores state about the current list match.</param>
		/// <param name="match">The match object, used to store global state during the match (such as the results of capture groups).</param>
		/// <returns>Returns whether all patterns were matched successfully against a part of the list.
		/// If the method returns true, it updates listMatch.SyntaxIndex to point to the next node that was not part of the match,
		/// and adds the capture groups (if any) to the match.
		/// If the method returns false, the listMatch and match objects remain in a partially-updated state and need to be restored
		/// before they can be reused.</returns>
		internal static bool PerformMatchSequence(IReadOnlyList<ILInstruction> patterns, ref ListMatch listMatch, ref Match match)
		{
			// The patterns may create savepoints, so we need to save the 'i' variable
			// as part of those checkpoints.
			for (int i = listMatch.PopFromSavePoint() ?? 0; i < patterns.Count; i++)
			{
				int startMarker = listMatch.GetSavePointStartMarker();
				bool success = patterns[i].PerformMatch(ref listMatch, ref match);
				listMatch.PushToSavePoints(startMarker, i);
				if (!success)
					return false;
			}
			return true;
		}

		/// <summary>
		/// A savepoint that the list matching operation can be restored from.
		/// </summary>
		struct SavePoint
		{
			internal readonly int CheckPoint;
			internal readonly int SyntaxIndex;
			internal readonly Stack<int> stack;

			public SavePoint(int checkpoint, int syntaxIndex)
			{
				this.CheckPoint = checkpoint;
				this.SyntaxIndex = syntaxIndex;
				this.stack = new Stack<int>();
			}
		}

		/// <summary>
		/// The syntax list we are matching against.
		/// </summary>
		internal readonly IReadOnlyList<ILInstruction?> SyntaxList;

		/// <summary>
		/// The current index in the syntax list.
		/// </summary>
		internal int SyntaxIndex;

		ListMatch(IReadOnlyList<ILInstruction?> syntaxList)
		{
			this.SyntaxList = syntaxList;
			this.SyntaxIndex = 0;
			this.backtrackingStack = null;
			this.restoreStack = null;
		}

		List<SavePoint>? backtrackingStack;
		Stack<int>? restoreStack;

		void AddSavePoint(SavePoint savepoint)
		{
			if (backtrackingStack == null)
				backtrackingStack = new List<SavePoint>();
			backtrackingStack.Add(savepoint);
		}

		internal void AddSavePoint(ref Match match, int data)
		{
			var savepoint = new SavePoint(match.CheckPoint(), this.SyntaxIndex);
			savepoint.stack.Push(data);
			AddSavePoint(savepoint);
		}

		internal int GetSavePointStartMarker()
		{
			return backtrackingStack != null ? backtrackingStack.Count : 0;
		}

		internal void PushToSavePoints(int startMarker, int data)
		{
			if (backtrackingStack == null)
				return;
			for (int i = startMarker; i < backtrackingStack.Count; i++)
			{
				backtrackingStack[i].stack.Push(data);
			}
		}

		internal int? PopFromSavePoint()
		{
			if (restoreStack == null || restoreStack.Count == 0)
				return null;
			return restoreStack.Pop();
		}

		/// <summary>
		/// Restores the listmatch state from a savepoint.
		/// </summary>
		/// <returns>Returns whether a savepoint exists</returns>
		internal bool RestoreSavePoint(ref Match match)
		{
			if (backtrackingStack == null || backtrackingStack.Count == 0)
				return false;
			var savepoint = backtrackingStack[backtrackingStack.Count - 1];
			backtrackingStack.RemoveAt(backtrackingStack.Count - 1);
			match.RestoreCheckPoint(savepoint.CheckPoint);
			this.SyntaxIndex = savepoint.SyntaxIndex;
			restoreStack = savepoint.stack;
			return true;
		}
	}
}
