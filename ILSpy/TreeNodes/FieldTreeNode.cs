// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Output;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.Languages;

namespace ICSharpCode.ILSpy.TreeNodes
{
	public sealed class FieldTreeNode : ILSpyTreeNode, IMemberTreeNode
	{
		public IField FieldDefinition { get; }

		public IEntity? Member => FieldDefinition;

		public FieldTreeNode(IField field)
		{
			FieldDefinition = field ?? throw new ArgumentNullException(nameof(field));
		}

		public override object Text => Language.EntityToString(FieldDefinition, ConversionFlags.None) + GetSuffixString(FieldDefinition);

		public override object NavigationText => Language.EntityToString(FieldDefinition, ConversionFlags.ShowDeclaringType);

		public override object Icon => GetIcon(FieldDefinition);

		// Mirrors WPF's discriminator: enum value, const literal, readonly, plain field.
		public static Avalonia.Media.IImage GetIcon(IField field)
		{
			// EnumValue: declaring type is an enum and the return type is the enum itself --
			// this excludes the synthesised int32 'value__' backing field.
			if (field.DeclaringType.Kind == TypeKind.Enum && field.ReturnType.Kind == TypeKind.Enum)
				return Images.GetIcon(Images.EnumValue,
					Images.GetOverlay(field.Accessibility));

			if (field.IsConst)
				return Images.GetIcon(Images.Literal,
					Images.GetOverlay(field.Accessibility));

			if (field.IsReadOnly)
				return Images.GetIcon(Images.FieldReadOnly,
					Images.GetOverlay(field.Accessibility), field.IsStatic);

			return Images.GetIcon(Images.Field,
				Images.GetOverlay(field.Accessibility), field.IsStatic);
		}
		public override bool ShowExpander => false;

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
			=> language.DecompileField(FieldDefinition, output, options);

		public override bool IsPublicAPI => FieldDefinition.Accessibility switch {
			Accessibility.Public or Accessibility.Protected or Accessibility.ProtectedOrInternal => true,
			_ => false,
		};

		public override FilterResult Filter(LanguageSettings settings)
		{
			if (settings.ShowApiLevel == ApiVisibility.PublicOnly && !IsPublicAPI)
				return FilterResult.Hidden;
			if (settings.SearchTermMatches(FieldDefinition.Name) && (settings.ShowApiLevel == ApiVisibility.All || LanguageService.CurrentLanguage.ShowMember(FieldDefinition)))
				return FilterResult.Match;
			else
				return FilterResult.Hidden;
		}

		public override string ToString() => "Field " + FieldDefinition.Name;
	}
}
