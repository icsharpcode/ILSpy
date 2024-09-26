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

using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;

using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TextView;

namespace ICSharpCode.ILSpy
{
	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._File), Header = nameof(Resources._SaveCode), MenuIcon = "Images/Save", MenuCategory = nameof(Resources.Save), MenuOrder = 0)]
	[PartCreationPolicy(CreationPolicy.Shared)]
	sealed class SaveCommand : CommandWrapper
	{
		private AssemblyTreeModel assemblyTreeModel;

		[ImportingConstructor]
		public SaveCommand(AssemblyTreeModel assemblyTreeModel)
			: base(ApplicationCommands.Save)
		{
			this.assemblyTreeModel = assemblyTreeModel;
		}

		protected override void OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;
			e.CanExecute = SaveCodeContextMenuEntry.CanExecute(assemblyTreeModel.SelectedNodes.ToList());
		}

		protected override void OnExecute(object sender, ExecutedRoutedEventArgs e)
		{
			SaveCodeContextMenuEntry.Execute(assemblyTreeModel.SelectedNodes.ToList());
		}
	}
}
