﻿// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Commands
{
	[ExportContextMenuEntry(Header = nameof(Resources.Decompile), Order = 10)]
	[PartCreationPolicy(CreationPolicy.Shared)]
	class DecompileCommand : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context)
		{
			if (context.SelectedTreeNodes == null)
				return context.Reference?.Reference is IEntity;
			return context.SelectedTreeNodes.Length == 1 && context.SelectedTreeNodes.All(n => n is IMemberTreeNode);
		}

		public bool IsEnabled(TextViewContext context)
		{
			if (context.SelectedTreeNodes == null)
				return context.Reference?.Reference is IEntity;
			foreach (IMemberTreeNode node in context.SelectedTreeNodes)
			{
				if (!IsValidReference(node.Member))
					return false;
			}

			return true;
		}

		bool IsValidReference(object reference)
		{
			return reference is IEntity;
		}

		public void Execute(TextViewContext context)
		{
			IEntity selection = null;
			if (context.SelectedTreeNodes?[0] is IMemberTreeNode node)
			{
				selection = node.Member;
			}
			else if (context.Reference?.Reference is IEntity entity)
			{
				selection = entity;
			}
			if (selection != null)
				MessageBus.Send(this, new NavigateToReferenceEventArgs(selection));
		}
	}
}
