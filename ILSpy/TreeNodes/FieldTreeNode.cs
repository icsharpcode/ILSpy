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
using System.Reflection;
using System.Reflection.Metadata;
using System.Windows.Media;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.ILSpy.TreeNodes
{
	/// <summary>
	/// Represents a field in the TreeView.
	/// </summary>
	public sealed class FieldTreeNode : ILSpyTreeNode, IMemberTreeNode
	{
		public IField FieldDefinition { get; }

		public FieldTreeNode(IField field)
		{
			this.FieldDefinition = field ?? throw new ArgumentNullException(nameof(field));
		}

		public override object Text => GetText(FieldDefinition, Language) + FieldDefinition.MetadataToken.ToSuffixString();

		public static object GetText(IField field, Language language)
		{
			return language.FieldToString(field, includeTypeName: false, includeNamespace: false);
		}

		public override object Icon => GetIcon(FieldDefinition);

		public static ImageSource GetIcon(IField field)
		{
			var metadata = ((MetadataAssembly)field.ParentAssembly).PEFile.Metadata;
			var fieldDefinition = metadata.GetFieldDefinition((FieldDefinitionHandle)field.MetadataToken);
			if (fieldDefinition.GetDeclaringType().IsEnum(metadata) && !fieldDefinition.HasFlag(FieldAttributes.SpecialName))
				return Images.GetIcon(MemberIcon.EnumValue, GetOverlayIcon(fieldDefinition.Attributes), false);

			if (fieldDefinition.HasFlag(FieldAttributes.Literal))
				return Images.GetIcon(MemberIcon.Literal, GetOverlayIcon(fieldDefinition.Attributes), false);
			else if (fieldDefinition.HasFlag(FieldAttributes.InitOnly)) {
				if (IsDecimalConstant(field))
					return Images.GetIcon(MemberIcon.Literal, GetOverlayIcon(fieldDefinition.Attributes), false);
				else
					return Images.GetIcon(MemberIcon.FieldReadOnly, GetOverlayIcon(fieldDefinition.Attributes), fieldDefinition.HasFlag(FieldAttributes.Static));
			} else
				return Images.GetIcon(MemberIcon.Field, GetOverlayIcon(fieldDefinition.Attributes), fieldDefinition.HasFlag(FieldAttributes.Static));
		}

		private static bool IsDecimalConstant(IField field)
		{
			return field.IsConst && field.Type.IsKnownType(KnownTypeCode.Decimal) && field.ConstantValue != null;
		}

		private static AccessOverlayIcon GetOverlayIcon(FieldAttributes fieldAttributes)
		{
			switch (fieldAttributes & FieldAttributes.FieldAccessMask) {
				case FieldAttributes.Public:
					return AccessOverlayIcon.Public;
				case FieldAttributes.Assembly:
					return AccessOverlayIcon.Internal;
				case FieldAttributes.FamANDAssem:
					return AccessOverlayIcon.PrivateProtected;
				case FieldAttributes.Family:
					return AccessOverlayIcon.Protected;
				case FieldAttributes.FamORAssem:
					return AccessOverlayIcon.ProtectedInternal;
				case FieldAttributes.Private:
					return AccessOverlayIcon.Private;
				case 0:
					return AccessOverlayIcon.CompilerControlled;
				default:
					throw new NotSupportedException();
			}
		}

		public override FilterResult Filter(FilterSettings settings)
		{
			if (!settings.ShowInternalApi && !IsPublicAPI)
				return FilterResult.Hidden;
			if (settings.SearchTermMatches(FieldDefinition.Name) && settings.Language.ShowMember(FieldDefinition))
				return FilterResult.Match;
			else
				return FilterResult.Hidden;
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.DecompileField(FieldDefinition, output, options);
		}
		
		public override bool IsPublicAPI {
			get {
				switch (FieldDefinition.Accessibility) {
					case Accessibility.Public:
					case Accessibility.Protected:
					case Accessibility.ProtectedOrInternal:
						return true;
					default:
						return false;
				}
			}
		}

		IEntity IMemberTreeNode.Member => FieldDefinition;
	}
}
