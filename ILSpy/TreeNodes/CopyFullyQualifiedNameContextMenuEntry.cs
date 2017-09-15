using System.Windows;
using Mono.Cecil;

namespace ICSharpCode.ILSpy.TreeNodes
{
	[ExportContextMenuEntry(Header = "Copy FQ Name", Icon = "images/Copy.png", Order = 9999)]
	public class CopyFullyQualifiedNameContextMenuEntry : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context)
		{
			return GetMemberNodeFromContext(context) != null;
		}

		public bool IsEnabled(TextViewContext context) => true;

		public void Execute(TextViewContext context)
		{
			var member = GetMemberNodeFromContext(context)?.Member;
			if (member == null) return;
			Clipboard.SetText(GetFullyQualifiedName(member));
		}

		private IMemberTreeNode GetMemberNodeFromContext(TextViewContext context)
		{
			return context.SelectedTreeNodes?.Length == 1 ? context.SelectedTreeNodes[0] as IMemberTreeNode : null;
		}

		/// <summary>
		/// Resolve full type name using .NET type representation for nested types.
		/// </summary>
		private string GetFullyQualifiedName(MemberReference member)
		{
			if (member.DeclaringType != null) {
				if (member is TypeReference)
					return GetFullyQualifiedName(member.DeclaringType) + "+" + member.Name;
				else
					return GetFullyQualifiedName(member.DeclaringType) + "." + member.Name;
			}
			return (member is TypeReference t ? t.Namespace + "." : "") + member.Name;
		}
	}
}