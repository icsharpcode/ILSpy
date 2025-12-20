using System.Linq;
using System.Windows;

using ICSharpCode.ILSpyX.TreeView.PlatformAbstractions;

namespace ICSharpCode.ILSpy.TreeNodes
{
	public partial class AssemblyListTreeNode
	{
		public override bool CanDrop(IPlatformDragEventArgs e, int index)
		{
			e.Effects = XPlatDragDropEffects.Move | XPlatDragDropEffects.Copy | XPlatDragDropEffects.Link;
			if (e.Data.GetDataPresent(AssemblyTreeNode.DataFormat))
				return true;
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				return true;
			e.Effects = XPlatDragDropEffects.None;
			return false;
		}

		public override void Drop(IPlatformDragEventArgs e, int index)
		{
			string[] files = e.Data.GetData(AssemblyTreeNode.DataFormat) as string[];
			if (files == null)
				files = e.Data.GetData(DataFormats.FileDrop) as string[];
			if (files != null)
			{
				var assemblies = files
					.Where(file => file != null)
					.Select(file => assemblyList.OpenAssembly(file))
					.Where(asm => asm != null)
					.Distinct()
					.ToArray();
				assemblyList.Move(assemblies, index);
				var nodes = assemblies.SelectArray(AssemblyTreeModel.FindTreeNode);
				AssemblyTreeModel.SelectNodes(nodes);
			}
		}
	}
}