using System;
using System.Windows;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;

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
			if (member == null || member.IsNil) return;
			Clipboard.SetText(GetFullyQualifiedName(member));
		}

		private IMemberTreeNode GetMemberNodeFromContext(TextViewContext context)
		{
			return context.SelectedTreeNodes?.Length == 1 ? context.SelectedTreeNodes[0] as IMemberTreeNode : null;
		}

		/// <summary>
		/// Resolve full type name using .NET type representation for nested types.
		/// </summary>
		private string GetFullyQualifiedName(IMetadataEntity member)
		{
			string name;
			System.Reflection.Metadata.TypeDefinitionHandle declaringType;
			switch (member.Handle.Kind) {
				case System.Reflection.Metadata.HandleKind.TypeDefinition:
					return ((System.Reflection.Metadata.TypeDefinitionHandle)member.Handle).GetFullTypeName(member.Module.Metadata).ToString();
				case System.Reflection.Metadata.HandleKind.FieldDefinition:
					name = "";
					declaringType = member.Handle.GetDeclaringType(member.Module.Metadata);
					var fd = member.Module.Metadata.GetFieldDefinition((System.Reflection.Metadata.FieldDefinitionHandle)member.Handle);
					if (!declaringType.IsNil) {
						name = declaringType.GetFullTypeName(member.Module.Metadata) + ".";
					}
					return name + member.Module.Metadata.GetString(fd.Name);
				case System.Reflection.Metadata.HandleKind.MethodDefinition:
					name = "";
					declaringType = member.Handle.GetDeclaringType(member.Module.Metadata);
					var md = member.Module.Metadata.GetMethodDefinition((System.Reflection.Metadata.MethodDefinitionHandle)member.Handle);
					if (!declaringType.IsNil) {
						name = declaringType.GetFullTypeName(member.Module.Metadata) + ".";
					}
					return name + member.Module.Metadata.GetString(md.Name);
				case System.Reflection.Metadata.HandleKind.EventDefinition:
					name = "";
					declaringType = member.Handle.GetDeclaringType(member.Module.Metadata);
					var ed = member.Module.Metadata.GetEventDefinition((System.Reflection.Metadata.EventDefinitionHandle)member.Handle);
					if (!declaringType.IsNil) {
						name = declaringType.GetFullTypeName(member.Module.Metadata) + ".";
					}
					return name + member.Module.Metadata.GetString(ed.Name);
				case System.Reflection.Metadata.HandleKind.PropertyDefinition:
					name = "";
					declaringType = member.Handle.GetDeclaringType(member.Module.Metadata);
					var pd = member.Module.Metadata.GetPropertyDefinition((System.Reflection.Metadata.PropertyDefinitionHandle)member.Handle);
					if (!declaringType.IsNil) {
						name = declaringType.GetFullTypeName(member.Module.Metadata) + ".";
					}
					return name + member.Module.Metadata.GetString(pd.Name);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}