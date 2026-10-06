// Copyright (c) 2026 Piero Viano
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

using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Properties;

namespace ICSharpCode.ILSpy.Dependencies
{
	/// <summary>
	/// Navigate -> "Unresolved References Report" for the selected assembly nodes, or the whole list
	/// when no assembly node is selected.
	/// </summary>
	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._Navigate), Header = nameof(Resources.UnresolvedReferencesReport), MenuCategory = "Dependencies", MenuOrder = 620)]
	[Shared]
	[method: ImportingConstructor]
	public sealed class UnresolvedReferencesReportCommand(AssemblyTreeModel assemblyTreeModel) : SimpleCommand
	{
		public override bool CanExecute(object? parameter) => (assemblyTreeModel.AssemblyList?.Count ?? 0) > 0;

		public override void Execute(object? parameter)
		{
			var assemblies = UnresolvedReferencesReportActions.GetTargetAssemblies(assemblyTreeModel.SelectedItems, assemblyTreeModel.AssemblyList,
				includeOwnersOfSelection: false);
			UnresolvedReferencesReportActions.ShowUnresolvedReferencesReportAsync(assemblies).HandleExceptions();
		}
	}
}
