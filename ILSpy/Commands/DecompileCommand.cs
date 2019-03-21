using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
			var (c, selectedItem) = context.GetColumnAndRowFromMousePosition();
			if (c is FilterableGridViewColumn column && column.CellTemplateSelector is HandleTemplate && selectedItem is IMemberTreeNode) {
				var selectedHandle = MetadataTokens.EntityHandle((int)selectedItem.GetType().GetProperty(column.SortBy).GetValue(selectedItem));
				return !selectedHandle.IsNil && (selectedHandle.Kind == HandleKind.TypeDefinition || selectedHandle.Kind == HandleKind.FieldDefinition || selectedHandle.Kind == HandleKind.MethodDefinition || selectedHandle.Kind == HandleKind.PropertyDefinition || selectedHandle.Kind == HandleKind.EventDefinition);
			}
			if (context.SelectedTreeNodes == null)
				return context.Reference?.Reference is IEntity;
			return context.SelectedTreeNodes.Length == 1 && context.SelectedTreeNodes.All(n => n is IMemberTreeNode);
		}

		public bool IsEnabled(TextViewContext context)
		{
			var (c, selectedItem) = context.GetColumnAndRowFromMousePosition();
			if (c is FilterableGridViewColumn column && column.CellTemplateSelector is HandleTemplate && selectedItem is IMemberTreeNode) {
				var selectedHandle = MetadataTokens.EntityHandle((int)selectedItem.GetType().GetProperty(column.SortBy).GetValue(selectedItem));
				return !selectedHandle.IsNil && (selectedHandle.Kind == HandleKind.TypeDefinition || selectedHandle.Kind == HandleKind.FieldDefinition || selectedHandle.Kind == HandleKind.MethodDefinition || selectedHandle.Kind == HandleKind.PropertyDefinition || selectedHandle.Kind == HandleKind.EventDefinition);
			}
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
			var (c, selectedItem) = context.GetColumnAndRowFromMousePosition();
			if (c is FilterableGridViewColumn column && column.CellTemplateSelector is HandleTemplate && selectedItem is IMemberTreeNode semanticContext) {
				var selectedHandle = MetadataTokens.EntityHandle((int)selectedItem.GetType().GetProperty(column.SortBy).GetValue(selectedItem));
				var module = (MetadataModule)semanticContext.Member.ParentModule;
				switch (selectedHandle.Kind) {
					case HandleKind.TypeDefinition:
						selection = module.GetDefinition((TypeDefinitionHandle)selectedHandle);
						break;
					case HandleKind.FieldDefinition:
						selection = module.GetDefinition((FieldDefinitionHandle)selectedHandle);
						break;
					case HandleKind.MethodDefinition:
						selection = module.GetDefinition((MethodDefinitionHandle)selectedHandle);
						break;
					case HandleKind.PropertyDefinition:
						selection = module.GetDefinition((PropertyDefinitionHandle)selectedHandle);
						break;
					case HandleKind.EventDefinition:
						selection = module.GetDefinition((EventDefinitionHandle)selectedHandle);
						break;
				}
			} else if (context.SelectedTreeNodes?[0] is IMemberTreeNode node) {
				selection = node.Member;
			} else if (context.Reference?.Reference is IEntity entity) {
				selection = entity;
			}
			if (selection != null)
				MainWindow.Instance.JumpToReference(selection);
		}
	}
}
