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

namespace ICSharpCode.ILSpy.TreeNodes
{
	/// <summary>
	/// Outcome of an <see cref="ILSpyTreeNode.Filter"/> evaluation against the active
	/// <see cref="LanguageSettings"/>. Drives whether the assembly tree shows the node, hides
	/// it outright, or shows it only if any descendant matches. Layout mirrors WPF's
	/// <c>FilterResult</c> for 1:1 portability of <see cref="ILSpyTreeNode.Filter"/> overrides.
	/// </summary>
	public enum FilterResult
	{
		/// <summary>Shows the node (and resets the search term for child nodes).</summary>
		Match,

		/// <summary>
		/// Hides the node only if all children are hidden (and resets the search term for
		/// child nodes). Treated identically to <see cref="Recurse"/> by the cascade — the
		/// "reset search term" distinction is documented for forward-compat with the WPF
		/// behavior contract.
		/// </summary>
		MatchAndRecurse,

		/// <summary>
		/// Hides the node only if all children are hidden (doesn't reset the search term
		/// for child nodes).
		/// </summary>
		Recurse,

		/// <summary>The node and all its descendants are hidden.</summary>
		Hidden,
	}
}
