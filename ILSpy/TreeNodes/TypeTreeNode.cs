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
using System.Linq;
using System.Reflection;
using System.Windows.Media;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;

using SRM = System.Reflection.Metadata;

namespace ICSharpCode.ILSpy.TreeNodes
{
	public sealed class TypeTreeNode : ILSpyTreeNode, IMemberTreeNode
	{
		readonly TypeDefinition typeDefinition;
		readonly SRM.TypeDefinition metadataTypeDefinition;

		public TypeTreeNode(TypeDefinition typeDefinition, AssemblyTreeNode parentAssemblyNode)
		{
			if (typeDefinition.IsNil)
				throw new ArgumentNullException(nameof(typeDefinition));
			this.ParentAssemblyNode = parentAssemblyNode ?? throw new ArgumentNullException(nameof(parentAssemblyNode));
			this.typeDefinition = typeDefinition;
			this.metadataTypeDefinition = typeDefinition.This();
			this.LazyLoading = true;
		}

		public TypeDefinition TypeDefinition => typeDefinition;

		public AssemblyTreeNode ParentAssemblyNode { get; }

		public override object Text => HighlightSearchMatch(this.Language.FormatTypeName(TypeDefinition), TypeDefinition.Handle.ToSuffixString());

		public override bool IsPublicAPI {
			get {
				switch (metadataTypeDefinition.Attributes & TypeAttributes.VisibilityMask) {
					case TypeAttributes.Public:
					case TypeAttributes.NestedPublic:
					case TypeAttributes.NestedFamily:
					case TypeAttributes.NestedFamORAssem:
						return true;
					default:
						return false;
				}
			}
		}
		
		public override FilterResult Filter(FilterSettings settings)
		{
			if (!settings.ShowInternalApi && !IsPublicAPI)
				return FilterResult.Hidden;
			if (settings.SearchTermMatches(TypeDefinition.Module.GetMetadataReader().GetString(metadataTypeDefinition.Name))) {
				if (settings.Language.ShowMember(TypeDefinition))
					return FilterResult.Match;
				else
					return FilterResult.Hidden;
			} else {
				return FilterResult.Recurse;
			}
		}
		
		protected override void LoadChildren()
		{
			var metadata = TypeDefinition.Module.GetMetadataReader();
			if (!metadataTypeDefinition.BaseType.IsNil || metadataTypeDefinition.GetInterfaceImplementations().Any())
				this.Children.Add(new BaseTypesTreeNode(TypeDefinition));
			if (!metadataTypeDefinition.HasFlag(TypeAttributes.Sealed))
				this.Children.Add(new DerivedTypesTreeNode(ParentAssemblyNode.AssemblyList, TypeDefinition));
			foreach (var nestedType in metadataTypeDefinition.GetNestedTypes().OrderBy(m => metadata.GetString(metadata.GetTypeDefinition(m).Name), NaturalStringComparer.Instance)) {
				this.Children.Add(new TypeTreeNode(new TypeDefinition(TypeDefinition.Module, nestedType), ParentAssemblyNode));
			}
			foreach (var field in metadataTypeDefinition.GetFields().OrderBy(m => metadata.GetString(metadata.GetFieldDefinition(m).Name), NaturalStringComparer.Instance)) {
				this.Children.Add(new FieldTreeNode(new FieldDefinition(TypeDefinition.Module, field)));
			}
			
			foreach (var property in metadataTypeDefinition.GetProperties().OrderBy(m => metadata.GetString(metadata.GetPropertyDefinition(m).Name), NaturalStringComparer.Instance)) {
				this.Children.Add(new PropertyTreeNode(new PropertyDefinition(TypeDefinition.Module, property)));
			}
			foreach (var ev in metadataTypeDefinition.GetEvents().OrderBy(m => metadata.GetString(metadata.GetEventDefinition(m).Name), NaturalStringComparer.Instance)) {
				this.Children.Add(new EventTreeNode(new EventDefinition(TypeDefinition.Module, ev)));
			}
			foreach (var method in metadataTypeDefinition.GetMethods().OrderBy(m => metadata.GetString(metadata.GetMethodDefinition(m).Name), NaturalStringComparer.Instance)) {
				if (method.GetMethodSemanticsAttributes(metadata) == 0) {
					this.Children.Add(new MethodTreeNode(new MethodDefinition(TypeDefinition.Module, method)));
				}
			}
		}

		public override bool CanExpandRecursively => true;

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.DecompileType(TypeDefinition, output, options);
		}

		public override object Icon => GetIcon(TypeDefinition);

		public static ImageSource GetIcon(TypeDefinition type)
		{
			TypeIcon typeIcon = GetTypeIcon(type);
			AccessOverlayIcon overlayIcon = GetOverlayIcon(type);

			return Images.GetIcon(typeIcon, overlayIcon);
		}

		static TypeIcon GetTypeIcon(TypeDefinition type)
		{
			var metadata = type.Module.GetMetadataReader();
			var typeDefinition = metadata.GetTypeDefinition(type.Handle);
			if (typeDefinition.IsValueType(metadata)) {
				if (typeDefinition.IsEnum(metadata))
					return TypeIcon.Enum;
				else
					return TypeIcon.Struct;
			} else {
				if ((typeDefinition.Attributes & TypeAttributes.ClassSemanticsMask) == TypeAttributes.Interface)
					return TypeIcon.Interface;
				else if (typeDefinition.IsDelegate(metadata))
					return TypeIcon.Delegate;
				else if (IsStaticClass(typeDefinition))
					return TypeIcon.StaticClass;
				else
					return TypeIcon.Class;
			}
		}

		private static AccessOverlayIcon GetOverlayIcon(TypeDefinition type)
		{
			var def = type.This();
			AccessOverlayIcon overlay;
			switch (def.Attributes & TypeAttributes.VisibilityMask) {
				case TypeAttributes.Public:
				case TypeAttributes.NestedPublic:
					overlay = AccessOverlayIcon.Public;
					break;
				case TypeAttributes.NotPublic:
				case TypeAttributes.NestedAssembly:
					overlay = AccessOverlayIcon.Internal;
					break;
				case TypeAttributes.NestedFamANDAssem:
					overlay = AccessOverlayIcon.PrivateProtected;
					break;
				case TypeAttributes.NestedFamily:
				case TypeAttributes.NestedFamORAssem:
					overlay = AccessOverlayIcon.Protected;
					break;
				case TypeAttributes.NestedPrivate:
					overlay = AccessOverlayIcon.Private;
					break;
				default:
					throw new NotSupportedException();
			}
			return overlay;
		}

		static bool IsStaticClass(SRM.TypeDefinition type)
		{
			return type.HasFlag(TypeAttributes.Sealed) && type.HasFlag(TypeAttributes.Abstract);
		}

		IMetadataEntity IMemberTreeNode.Member => TypeDefinition;
	}
}
