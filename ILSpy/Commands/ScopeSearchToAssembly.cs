// Copyright (c) 2021 Siegfried Pammer
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

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy
{
	[ExportContextMenuEntry(Header = nameof(Resources.ScopeSearchToThisAssembly), Category = nameof(Resources.Analyze), Order = 9999)]
	public class ScopeSearchToAssembly : IContextMenuEntry
	{
		public void Execute(TextViewContext context)
		{
			string asmName = GetAssembly(context);
			string searchTerm = MainWindow.Instance.SearchPane.SearchTerm;
			string[] args = NativeMethods.CommandLineToArgumentArray(searchTerm);
			bool replaced = false;
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i].StartsWith("inassembly:", StringComparison.OrdinalIgnoreCase))
				{
					args[i] = "inassembly:" + asmName;
					replaced = true;
					break;
				}
			}
			if (!replaced)
			{
				searchTerm += " inassembly:" + asmName;
			}
			else
			{
				searchTerm = NativeMethods.ArgumentArrayToCommandLine(args);
			}
			MainWindow.Instance.SearchPane.SearchTerm = searchTerm;
		}

		public bool IsEnabled(TextViewContext context)
		{
			return GetAssembly(context) != null;
		}

		public bool IsVisible(TextViewContext context)
		{
			return GetAssembly(context) != null;
		}

		string GetAssembly(TextViewContext context)
		{
			if (context.Reference?.Reference is IEntity entity)
				return entity.ParentModule.AssemblyName;
			if (context.SelectedTreeNodes?.Length != 1)
				return null;
			switch (context.SelectedTreeNodes[0])
			{
				case AssemblyTreeNode tn:
					return tn.LoadedAssembly.ShortName;
				case IMemberTreeNode member:
					return member.Member.ParentModule.AssemblyName;
				default:
					return null;
			}
		}
	}
}