using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.TreeView;

namespace ICSharpCode.ILSpy
{
	public interface IProtocolHandler
	{
		ILSpyTreeNode Resolve(string protocol, PEFile module, Handle handle, out bool newTabPage);
	}
}
