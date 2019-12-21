using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Metadata
{
	[Export(typeof(IProtocolHandler))]
	class MetadataProtocolHandler : IProtocolHandler
	{
		public ILSpyTreeNode Resolve(string protocol, PEFile module, Handle handle, out bool newTabPage)
		{
			newTabPage = true;
			if (protocol != "metadata")
				return null;
			var assemblyTreeNode = MainWindow.Instance.FindTreeNode(module) as AssemblyTreeNode;
			if (assemblyTreeNode == null)
				return null;
			var mxNode = assemblyTreeNode.Children.OfType<MetadataTreeNode>().FirstOrDefault();
			if (mxNode != null) {
				mxNode.EnsureLazyChildren();
				return mxNode.FindNodeByHandleKind(handle.Kind);
			}
			return null;
		}
	}
}
