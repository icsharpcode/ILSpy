using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.Analyzers;
using ICSharpCode.ILSpy.Controls;
using ICSharpCode.ILSpy.Metadata;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Commands
{
	[ExportContextMenuEntry(Header = "Decompile", Order = 10)]
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
			foreach (IMemberTreeNode node in context.SelectedTreeNodes) {
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
			if (context.SelectedTreeNodes?[0] is IMemberTreeNode node) {
				selection = node.Member;
			} else if (context.Reference?.Reference is IEntity entity) {
				selection = entity;
			}
			if (selection != null)
				MainWindow.Instance.JumpToReference(selection);
		}
	}

	[ExportContextMenuEntry(Header = "Go to token", Order = 10)]
	class GoToToken : IContextMenuEntry
	{
		public void Execute(TextViewContext context)
		{
			int token = GetSelectedToken(context.DataGrid, out PEFile module).Value;
			MainWindow.Instance.JumpToReference(("metadata", module, MetadataTokens.Handle(token)));
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
}
