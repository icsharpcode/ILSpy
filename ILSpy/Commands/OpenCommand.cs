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

using System.Composition;
using System.Windows.Input;

using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Properties;

using Microsoft.Win32;

namespace ICSharpCode.ILSpy
{
	[ExportToolbarCommand(ToolTip = nameof(Resources.Open), ToolbarIcon = "Images/Open", ToolbarCategory = nameof(Resources.Open), ToolbarOrder = 0)]
	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._File), Header = nameof(Resources._Open), MenuIcon = "Images/Open", MenuCategory = nameof(Resources.Open), MenuOrder = 0)]
	[Shared]
	sealed class OpenCommand : CommandWrapper
	{
		private readonly AssemblyTreeModel assemblyTreeModel;

		public OpenCommand(AssemblyTreeModel assemblyTreeModel)
			: base(ApplicationCommands.Open)
		{
			this.assemblyTreeModel = assemblyTreeModel;
		}

		protected override void OnExecute(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;
			OpenFileDialog dlg = new OpenFileDialog {
				Filter = ".NET assemblies|*.dll;*.exe;*.winmd;*.wasm|Nuget Packages (*.nupkg)|*.nupkg|Portable Program Database (*.pdb)|*.pdb|All files|*.*",
				Multiselect = true,
				RestoreDirectory = true
			};

			if (dlg.ShowDialog() == true)
			{
				assemblyTreeModel.OpenFiles(dlg.FileNames);
			}
		}
	}
}
