// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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

using System;
using System.Collections.Generic;
using System.Diagnostics;

using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.TreeView;

namespace ICSharpCode.ILSpy
{
	[DebuggerDisplay("Nodes = {treeNodes.Count}, State = [{ViewState}]")]
	public class NavigationState : IEquatable<NavigationState>
	{
		private readonly HashSet<SharpTreeNode> treeNodes;

		public IEnumerable<SharpTreeNode> TreeNodes => treeNodes;
		public ViewState ViewState { get; private set; }
		public TabPageModel TabPage { get; private set; }

		public NavigationState(TabPageModel tabPage, ViewState viewState)
		{
			this.TabPage = tabPage;
			this.treeNodes = new HashSet<SharpTreeNode>((IEnumerable<SharpTreeNode>)viewState.DecompiledNodes ?? Array.Empty<SharpTreeNode>());
			ViewState = viewState;
		}

		public NavigationState(TabPageModel tabPage, IEnumerable<SharpTreeNode> treeNodes)
		{
			this.TabPage = tabPage;
			this.treeNodes = new HashSet<SharpTreeNode>(treeNodes);
		}


		public bool Equals(NavigationState other)
		{
			if (!this.treeNodes.SetEquals(other.treeNodes))
				return false;

			if (object.ReferenceEquals(this.ViewState, other.ViewState))
				return true;

			if (this.ViewState == null)
				return false;

			return this.ViewState.Equals(other.ViewState);
		}
	}
}
