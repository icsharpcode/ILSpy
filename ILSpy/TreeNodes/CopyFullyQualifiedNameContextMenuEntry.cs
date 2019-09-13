using System;
using System.Windows;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.Properties;

namespace ICSharpCode.ILSpy.TreeNodes
{
	[ExportContextMenuEntry(Header = nameof(Resources.CopyName), Icon = "images/Copy", Order = 9999)]
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
			Clipboard.SetText(member.ReflectionName);
		}

		private IMemberTreeNode GetMemberNodeFromContext(TextViewContext context)
		{
			return context.SelectedTreeNodes?.Length == 1 ? context.SelectedTreeNodes[0] as IMemberTreeNode : null;
		}
	}
}