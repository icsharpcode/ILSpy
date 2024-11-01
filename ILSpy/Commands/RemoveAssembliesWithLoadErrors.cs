// Copyright (c) 2018 AlphaSierraPapa for the SharpDevelop Team
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

using System.Composition;
using System.Linq;

using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Properties;

namespace ICSharpCode.ILSpy
{
	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._File), Header = nameof(Resources._RemoveAssembliesWithLoadErrors), MenuCategory = nameof(Resources.Remove), MenuOrder = 2.6)]
	[Shared]
	class RemoveAssembliesWithLoadErrors(AssemblyTreeModel assemblyTreeModel) : SimpleCommand
	{
		public override bool CanExecute(object parameter)
		{
			return assemblyTreeModel.AssemblyList.GetAssemblies().Any(l => l.HasLoadError);
		}

		public override void Execute(object parameter)
		{
			foreach (var assembly in assemblyTreeModel.AssemblyList.GetAssemblies())
			{
				if (!assembly.HasLoadError)
					continue;
				var node = assemblyTreeModel.FindAssemblyNode(assembly);
				if (node != null && node.CanDelete())
					node.Delete();
			}
		}
	}

	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._File), Header = nameof(Resources.ClearAssemblyList), MenuCategory = nameof(Resources.Remove), MenuOrder = 2.6)]
	[Shared]
	class ClearAssemblyList : SimpleCommand
	{
		private readonly AssemblyTreeModel assemblyTreeModel;

		public ClearAssemblyList(AssemblyTreeModel assemblyTreeModel)
		{
			this.assemblyTreeModel = assemblyTreeModel;
		}

		public override bool CanExecute(object parameter)
		{
			return assemblyTreeModel.AssemblyList.Count > 0;
		}

		public override void Execute(object parameter)
		{
			assemblyTreeModel.AssemblyList.Clear();
		}
	}
}
