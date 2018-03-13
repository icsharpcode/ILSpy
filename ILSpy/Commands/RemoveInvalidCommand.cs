using System.Collections.Generic;
using System.Linq;
using ICSharpCode.TreeView;

namespace ICSharpCode.ILSpy.Commands
{
	[ExportMainMenuCommand(Menu = "_File", Header = "Remove Invalid", MenuCategory = "Remove", MenuOrder = 2.6)]

	sealed class RemoveInvalidCommand:SimpleCommand
	{
		private static IEnumerable<LoadedAssembly> InvalidAssemblys()
		{
			return MainWindow.Instance?.CurrentAssemblyList?.assemblies.Where(o => o.HasLoadError);
		}

		private static List<SharpTreeNode> GetNodes()
		{
			var result = InvalidAssemblys()
				.Select(assm =>
					MainWindow.Instance.AssemblyListTreeNode.Children.FirstOrDefault(n => n.ToString().Equals(assm.FileName)))
				.Where(o => o != null)
				.ToList();
			return result;
		}
		public override bool CanExecute(object parameter)
		{
			var invalid = InvalidAssemblys();
			return invalid != null && invalid.Any();
		}

		public override void Execute(object parameter)
		{
			var nodes = GetNodes();
			for (var i = 0; i < nodes.Count; i++) {
				var node = nodes[i];
				if(node.CanDelete())
					node.Delete();
			}
		}
	}
}
