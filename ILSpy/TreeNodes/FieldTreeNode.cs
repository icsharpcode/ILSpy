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
using System.Windows.Media;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.ILSpy.TreeNodes
{
	/// <summary>
	/// Represents a field in the TreeView.
	/// </summary>
	public sealed class FieldTreeNode : ILSpyTreeNode, IMemberTreeNode
	{
		public FieldDefinition FieldDefinition { get; }

		public FieldTreeNode(FieldDefinition field)
		{
			if (field.IsNil)
				throw new ArgumentNullException(nameof(field));
			this.FieldDefinition = field;
		}

		public override object Text => GetText(FieldDefinition, Language) + FieldDefinition.Handle.ToSuffixString();

		public static object GetText(FieldDefinition field, Language language)
		{
			var metadata = field.Module.GetMetadataReader();
			var fieldDefinition = metadata.GetFieldDefinition(field.Handle);
			string fieldType = fieldDefinition.DecodeSignature(language.CreateSignatureTypeProvider(false), new GenericContext(fieldDefinition.GetDeclaringType(), field.Module));
			return HighlightSearchMatch(metadata.GetString(fieldDefinition.Name), " : " + fieldType);
		}

		public override object Icon => GetIcon(FieldDefinition);

		public static ImageSource GetIcon(FieldDefinition field)
		{
			var metadata = field.Module.GetMetadataReader();
			var fieldDefinition = metadata.GetFieldDefinition(field.Handle);
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

		private static bool IsDecimalConstant(FieldDefinition field)
		{
			var metadata = field.Module.GetMetadataReader();
			var fieldDefinition = metadata.GetFieldDefinition(field.Handle);

			var fieldType = fieldDefinition.DecodeSignature(new FullTypeNameSignatureDecoder(metadata), default(Unit));
			if (fieldType.ToString() == "System.Decimal") {
				var attrs = fieldDefinition.GetCustomAttributes();
				foreach (var h in attrs) {
					var attr = metadata.GetCustomAttribute(h);
					var attrType = attr.GetAttributeType(metadata).GetFullTypeName(metadata);
					if (attrType.ToString() == "System.Runtime.CompilerServices.DecimalConstantAttribute")
						return true;
				}
			}
			return false;
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
			var metadata = FieldDefinition.Module.GetMetadataReader();
			var fieldDefinition = metadata.GetFieldDefinition(FieldDefinition.Handle);
			if (settings.SearchTermMatches(metadata.GetString(fieldDefinition.Name)) && settings.Language.ShowMember(FieldDefinition))
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
				var metadata = FieldDefinition.Module.GetMetadataReader();
				var fieldDefinition = metadata.GetFieldDefinition(FieldDefinition.Handle);

				switch (fieldDefinition.Attributes & FieldAttributes.FieldAccessMask) {
					case FieldAttributes.Public:
					case FieldAttributes.FamORAssem:
					case FieldAttributes.Family:
						return true;
					default:
						return false;
				}
			}
		}

		IMetadataEntity IMemberTreeNode.Member => FieldDefinition;
	}
}
