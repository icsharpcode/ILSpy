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

using ILSpy;
using ILSpy.Languages;

namespace ILSpy.TreeNodes
{
	sealed class PropertyTreeNode : ILSpyTreeNode, IMemberTreeNode
	{
		public IProperty PropertyDefinition { get; }

		public IEntity? Member => PropertyDefinition;

		public PropertyTreeNode(IProperty property)
		{
			PropertyDefinition = property ?? throw new ArgumentNullException(nameof(property));
			if (property.CanGet)
				Children.Add(new MethodTreeNode(property.Getter));
			if (property.CanSet)
				Children.Add(new MethodTreeNode(property.Setter));
		}

		public override object Text => Language.EntityToString(PropertyDefinition, ConversionFlags.None) + GetSuffixString(PropertyDefinition);

		public override object NavigationText => Language.EntityToString(PropertyDefinition, ConversionFlags.ShowDeclaringType);

		public override object Icon => Images.Images.GetIcon(Images.Images.Property,
			Images.Images.GetOverlay(PropertyDefinition.Accessibility), PropertyDefinition.IsStatic);

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
			=> language.DecompileProperty(PropertyDefinition, output, options);

		public override bool IsPublicAPI => PropertyDefinition.Accessibility switch {
			Accessibility.Public or Accessibility.Protected or Accessibility.ProtectedOrInternal => true,
			_ => false,
		};

		public override FilterResult Filter(LanguageSettings settings)
		{
			if (settings.ShowApiLevel == ApiVisibility.PublicOnly && !IsPublicAPI)
				return FilterResult.Hidden;
			if (settings.SearchTermMatches(PropertyDefinition.Name) && (settings.ShowApiLevel == ApiVisibility.All || LanguageService.CurrentLanguage.ShowMember(PropertyDefinition)))
				return FilterResult.Match;
			else
				return FilterResult.Hidden;
		}

		public override string ToString()
			=> "Property " + new ICSharpCode.Decompiler.IL.ILAmbience {
				ConversionFlags = ConversionFlags.ShowTypeParameterList
					| ConversionFlags.PlaceReturnTypeAfterParameterList
					| ConversionFlags.ShowReturnType
					| ConversionFlags.ShowParameterList
					| ConversionFlags.ShowParameterModifiers,
			}.ConvertSymbol(PropertyDefinition);
	}
}
