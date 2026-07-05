// Copyright (c) 2018 Siegfried Pammer
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

using System;
using System.IO;
using System.Linq;

using Microsoft.VisualStudio.Shell;

namespace ICSharpCode.ILSpy.AddIn.Commands
{
	class OpenProjectOutputCommand : ILSpyCommand
	{
		static OpenProjectOutputCommand instance;

		public OpenProjectOutputCommand(ILSpyAddInPackage owner)
			: base(owner, PkgCmdIDList.cmdidOpenProjectOutputInILSpy)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
		}

		protected override void OnBeforeQueryStatus(object sender, EventArgs e)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			if (sender is OleMenuCommand menuItem)
			{
				menuItem.Visible = false;

				var selectedItem = owner.DTE.SelectedItems.Item(1);
				menuItem.Visible = (ProjectItemForILSpy.Detect(owner, selectedItem) != null);
			}
		}

		protected override void OnExecute(object sender, EventArgs e)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			if (owner.DTE.SelectedItems.Count != 1)
				return;
			var projectItemWrapper = ProjectItemForILSpy.Detect(owner, owner.DTE.SelectedItems.Item(1));
			if (projectItemWrapper != null)
			{
				OpenAssembliesInILSpy(projectItemWrapper.GetILSpyParameters(owner));
			}
		}

		internal static void Register(ILSpyAddInPackage owner)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			instance = new OpenProjectOutputCommand(owner);
		}
	}
}
