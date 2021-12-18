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
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpy.Metadata;
using ICSharpCode.ILSpy.Properties;

namespace ICSharpCode.ILSpy.Commands
{
	[ExportContextMenuEntry(Header = nameof(Resources.GoToToken), Order = 10)]
	class GoToTokenCommand : IContextMenuEntry
	{
		public void Execute(TextViewContext context)
		{
			int token = GetSelectedToken(context.DataGrid, out PEFile module).Value;
			MainWindow.Instance.JumpToReference(new EntityReference(module, MetadataTokens.Handle(token), protocol: "metadata"));
		}

		public bool IsEnabled(TextViewContext context)
		{
			return true;
		}

		public bool IsVisible(TextViewContext context)
		{
			return context.DataGrid?.Name == "MetadataView" && GetSelectedToken(context.DataGrid, out _) != null;
		}

		private int? GetSelectedToken(DataGrid grid, out PEFile module)
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
			if (property == null || property.PropertyType != typeof(int) || !property.GetCustomAttributes(false).Any(a => a is StringFormatAttribute sf && sf.Format == "X8"))
				return null;
			module = (PEFile)moduleField.GetValue(cell.Item);
			return (int)property.GetValue(cell.Item);
		}
	}

	[ExportContextMenuEntry(Header = nameof(Resources.Copy), Order = 10)]
	class CopyCommand : IContextMenuEntry
	{
		public void Execute(TextViewContext context)
		{
			string content = GetSelectedCellContent(context.DataGrid, context.MousePosition);
			Clipboard.SetText(content);
		}

		public bool IsEnabled(TextViewContext context)
		{
			return true;
		}

		public bool IsVisible(TextViewContext context)
		{
			return context.DataGrid?.Name == "MetadataView"
				&& GetSelectedCellContent(context.DataGrid, context.MousePosition) != null;
		}

		private string GetSelectedCellContent(DataGrid grid, Point position)
		{
			position = grid.PointFromScreen(position);
			var hit = VisualTreeHelper.HitTest(grid, position);
			if (hit == null)
				return null;
			var cell = hit.VisualHit.GetParent<DataGridCell>();
			if (cell == null)
				return null;
			return cell.DataContext.GetType()
				.GetProperty(cell.Column.Header.ToString(), BindingFlags.Instance | BindingFlags.Public)
				.GetValue(cell.DataContext).ToString();
		}
	}
}
