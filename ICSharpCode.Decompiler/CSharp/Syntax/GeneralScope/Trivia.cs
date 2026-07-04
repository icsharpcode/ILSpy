// Copyright (c) 2026 Siegfried Pammer
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

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// Content that the output visitor emits verbatim but that is not a semantic child of a node:
	/// comments (<see cref="Comment"/>) and preprocessor directives (<see cref="PreProcessorDirective"/>).
	/// Trivia is attached to the node it annotates as leading or trailing trivia
	/// (<see cref="AstNode.AddLeadingTrivia"/> / <see cref="AstNode.AddTrailingTrivia"/>) rather than
	/// occupying a child slot, so it stays off the child-index space.
	/// </summary>
	public abstract class Trivia : AstNode
	{
		// The Leading or Trailing list of the owning node that currently holds this trivia (null while
		// detached). Lives here rather than on AstNode so that plain nodes, which can never be trivia,
		// do not pay for the field; AstNode's sibling navigation branches on it via a Trivia type test.
		internal List<Trivia>? triviaSiblings;

		TextLocation startLocation;
		TextLocation endLocation;

		protected Trivia()
		{
		}

		protected Trivia(TextLocation startLocation, TextLocation endLocation)
		{
			this.startLocation = startLocation;
			this.endLocation = endLocation;
		}

		public override TextLocation StartLocation {
			get { return startLocation; }
		}

		public override TextLocation EndLocation {
			get { return endLocation; }
		}

		internal void SetStartLocation(TextLocation value)
		{
			this.startLocation = value;
		}

		internal void SetEndLocation(TextLocation value)
		{
			this.endLocation = value;
		}

		internal void SetTriviaParent(AstNode newParent, List<Trivia> siblings, int index)
		{
			SetParent(newParent);
			triviaSiblings = siblings;
			childIndex = index;
		}
	}
}
