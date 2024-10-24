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

using System;
using System.Composition;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Windows;
using System.Windows.Controls;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpy.Metadata;
using ICSharpCode.ILSpy.Properties;

using TomsToolbox.Wpf;

namespace ICSharpCode.ILSpy.Commands
{
	[ExportContextMenuEntry(Header = nameof(Resources.GoToToken), Order = 10)]
	[Shared]
	class GoToTokenCommand : IContextMenuEntry
	{
		public void Execute(TextViewContext context)
		{
			int token = GetSelectedToken(context.DataGrid, out MetadataFile module).Value;
			MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(module, MetadataTokens.Handle(token), protocol: "metadata")));
		}

		public bool IsEnabled(TextViewContext context)
		{
			return true;
		}

		public bool IsVisible(TextViewContext context)
		{
			return context.DataGrid?.Name == "MetadataView" && GetSelectedToken(context.DataGrid, out _) != null;
		}

		private int? GetSelectedToken(DataGrid grid, out MetadataFile module)
		{
			module = null;
			if (grid == null)
				return null;
			var cell = grid.CurrentCell;
			if (!cell.IsValid)
				return null;
			Type type = cell.Item.GetType();
			var property = type.GetProperty(cell.Column.Header.ToString());
			var moduleField = type.GetField("module", BindingFlags.NonPublic | BindingFlags.Instance);
			if (property == null || property.PropertyType != typeof(int) || !property.GetCustomAttributes(false).Any(a => a is ColumnInfoAttribute { Kind: ColumnKind.Token } c))
				return null;
			module = (MetadataFile)moduleField.GetValue(cell.Item);
			return (int)property.GetValue(cell.Item);
		}
	}

	[ExportContextMenuEntry(Header = nameof(Resources.Copy), Order = 10)]
	[Shared]
	class CopyCommand : IContextMenuEntry
	{
		public void Execute(TextViewContext context)
		{
			string content = GetSelectedCellContent(context.OriginalSource);
			Clipboard.SetText(content);
		}

		public bool IsEnabled(TextViewContext context)
		{
			return true;
		}

		public bool IsVisible(TextViewContext context)
		{
			return context.DataGrid?.Name == "MetadataView"
				&& GetSelectedCellContent(context.OriginalSource) != null;
		}

		private static string GetSelectedCellContent(DependencyObject originalSource)
		{
			var cell = originalSource.AncestorsAndSelf().OfType<DataGridCell>().FirstOrDefault();

			return cell?.Column.OnCopyingCellClipboardContent(cell.DataContext).ToString();
		}
	}
}
