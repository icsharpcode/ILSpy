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
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Windows.Media;

using ICSharpCode.Decompiler;

namespace ICSharpCode.ILSpy.TreeNodes
{
	using ICSharpCode.Decompiler.Output;
	using ICSharpCode.Decompiler.TypeSystem;
	using ICSharpCode.ILSpyX;

	/// <summary>
	/// Represents a property in the TreeView.
	/// </summary>
	public sealed class PropertyTreeNode : ILSpyTreeNode, IMemberTreeNode
	{
		public PropertyTreeNode(IProperty property)
		{
			this.PropertyDefinition = property ?? throw new ArgumentNullException(nameof(property));

			if (property.CanGet)
				this.Children.Add(new MethodTreeNode(property.Getter));
			if (property.CanSet)
				this.Children.Add(new MethodTreeNode(property.Setter));
			/*foreach (var m in property.OtherMethods)
				this.Children.Add(new MethodTreeNode(m));*/
		}

		public IProperty PropertyDefinition { get; }

		public override object Text => GetText(GetPropertyDefinition(), Language) + GetSuffixString(PropertyDefinition);

		public override object NavigationText => GetText(GetPropertyDefinition(), Language, includeDeclaringTypeName: true);

		private IProperty GetPropertyDefinition()
		{
			var pd = ((MetadataModule)PropertyDefinition.ParentModule?.MetadataFile
				?.GetTypeSystemWithCurrentOptionsOrNull(SettingsService, AssemblyTreeModel.CurrentLanguageVersion)
				?.MainModule)?.GetDefinition((PropertyDefinitionHandle)PropertyDefinition.MetadataToken);
			return (IProperty)pd?.Specialize(PropertyDefinition.Substitution) ?? PropertyDefinition;
		}

		public static object GetText(IProperty property, Language language, bool includeDeclaringTypeName = false)
		{
			return language.EntityToString(property, includeDeclaringTypeName ? ConversionFlags.ShowDeclaringType : ConversionFlags.None);
		}

		public override object Icon => GetIcon(GetPropertyDefinition());

		public static ImageSource GetIcon(IProperty property)
		{
			IMethod accessor = property.Getter ?? property.Setter;
			Debug.Assert(accessor != null, "Property must have at least one accessor");
			bool isExtension = property.ResolveExtensionInfo()?.InfoOfExtensionMember((IMethod)accessor.MemberDefinition) != null;
			return Images.GetIcon(property.IsIndexer ? MemberIcon.Indexer : MemberIcon.Property,
				Images.GetOverlayIcon(property.Accessibility), property.IsStatic, isExtension);
		}

		public override FilterResult Filter(LanguageSettings settings)
		{
			if (settings.ShowApiLevel == ApiVisibility.PublicOnly && !IsPublicAPI)
				return FilterResult.Hidden;
			if (settings.SearchTermMatches(PropertyDefinition.Name) && (settings.ShowApiLevel == ApiVisibility.All || LanguageService.Language.ShowMember(PropertyDefinition)))
				return FilterResult.Match;
			else
				return FilterResult.Hidden;
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			if (Parent is ExtensionTreeNode && language is CSharpLanguage cs)
				cs.DecompileExtension(PropertyDefinition, output, options);
			else
				language.DecompileProperty(PropertyDefinition, output, options);
		}

		public override bool IsPublicAPI {
			get {
				switch (GetPropertyDefinition().Accessibility)
				{
					case Accessibility.Public:
					case Accessibility.ProtectedOrInternal:
					case Accessibility.Protected:
						return true;
					default:
						return false;
				}
			}
		}

		IEntity IMemberTreeNode.Member => PropertyDefinition;

		public override string ToString()
		{
			return "Property " + LanguageService.ILLanguage.EntityToString(PropertyDefinition, ConversionFlags.None);
		}
	}
}
