using System.Windows;
using Mono.Cecil;

namespace ICSharpCode.ILSpy.TreeNodes
{
	[ExportContextMenuEntry(Header = "Copy FQ Name", Icon = "images/Copy.png", Order = 9999)]
	public class CopyFullyQualifiedTypeNameContextMenuEntry : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context)
		{
			return GetTypeNodeFromContext(context) != null;
		}

		public bool IsEnabled(TextViewContext context) => true;

		public void Execute(TextViewContext context)
		{
			var typeDefinition = GetTypeNodeFromContext(context)?.TypeDefinition;
			if (typeDefinition == null) return;

			Clipboard.SetText(GetFullyQualifiedName(typeDefinition));
		}

		private TypeTreeNode GetTypeNodeFromContext(TextViewContext context)
		{
			return context.SelectedTreeNodes?.Length == 1 ? context.SelectedTreeNodes[0] as TypeTreeNode : null;
		}

		/// <summary>
		/// Resolve full type name using .NET type representation for nested types.
		/// </summary>
		private string GetFullyQualifiedName(TypeDefinition typeDefinition)
		{
			if (typeDefinition.IsNested)
			{
				return $"{GetFullyQualifiedName(typeDefinition.DeclaringType)}+{typeDefinition.Name}";
			}

			return $"{typeDefinition.Namespace}.{typeDefinition.Name}";
		}
	}
}