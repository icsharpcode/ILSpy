using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

using ICSharpCode.ILSpy.Util;
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