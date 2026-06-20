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
	}
}
