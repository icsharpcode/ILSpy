using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Automation;
using System.Windows.Automation.Peers;

namespace ICSharpCode.TreeView
{
	class SharpTreeViewAutomationPeer : FrameworkElementAutomationPeer
	{
		internal SharpTreeViewAutomationPeer(SharpTreeView owner ): base(owner)
		{
		}
		//private SharpTreeView  SharpTreeView { get { return (SharpTreeView)base.Owner; } }
		protected override AutomationControlType GetAutomationControlTypeCore()
		{
			return AutomationControlType.Tree;
		}
	}
}
