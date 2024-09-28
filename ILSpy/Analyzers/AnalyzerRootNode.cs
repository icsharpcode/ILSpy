// Copyright (c) 2024 Tom Englert for the SharpDevelop Team
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
using System.Collections.Specialized;
using System.Linq;

using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.TreeView;

namespace ICSharpCode.ILSpy.Analyzers;

public sealed class AnalyzerRootNode : AnalyzerTreeNode
{
	public AnalyzerRootNode()
	{
		MessageBus<CurrentAssemblyListChangedEventArgs>.Subscribers += (sender, e) => CurrentAssemblyList_Changed(sender, e);
	}

	void CurrentAssemblyList_Changed(object sender, NotifyCollectionChangedEventArgs e)
	{
		if (e.Action == NotifyCollectionChangedAction.Reset)
		{
			this.Children.Clear();
		}
		else
		{
			var removedAssemblies = e.OldItems?.Cast<LoadedAssembly>().ToArray() ?? [];
			var addedAssemblies = e.NewItems?.Cast<LoadedAssembly>().ToArray() ?? [];

			HandleAssemblyListChanged(removedAssemblies, addedAssemblies);
		}
	}

	public override bool HandleAssemblyListChanged(ICollection<LoadedAssembly> removedAssemblies, ICollection<LoadedAssembly> addedAssemblies)
	{
		this.Children.RemoveAll(
			delegate (SharpTreeNode n) {
				AnalyzerTreeNode an = n as AnalyzerTreeNode;
				return an == null || !an.HandleAssemblyListChanged(removedAssemblies, addedAssemblies);
			});
		return true;
	}
}