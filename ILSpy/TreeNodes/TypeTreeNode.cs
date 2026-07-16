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
using System.Linq;
using System.Reflection.Metadata;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Output;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Languages;

namespace ICSharpCode.ILSpy.TreeNodes
{
	public sealed class TypeTreeNode : ILSpyTreeNode, IMemberTreeNode
	{
		readonly ITypeDefinition typeDefinition;
		readonly TypeDefinitionHandle handle;
		readonly MetadataFile module;

		public TypeDefinitionHandle Handle => handle;
		public MetadataFile Module => module;

		public ITypeDefinition TypeDefinition => typeDefinition;

		public IEntity Member => typeDefinition;

		/// <summary>
		/// The type is resolved once, by the parent <see cref="AssemblyTreeNode"/>'s single pass over
		/// the module's type system, and held for the node's lifetime. Every member below reads it
		/// without touching the per-module type-system cache again: that cache is keyed on the current
		/// decompiler settings, so re-resolving on each property read would rebuild an effective
		/// settings object and take its lock for every cell the tree paints.
		/// </summary>
		public TypeTreeNode(ITypeDefinition typeDefinition, MetadataFile module)
		{
			this.typeDefinition = typeDefinition ?? throw new ArgumentNullException(nameof(typeDefinition));
			this.module = module ?? throw new ArgumentNullException(nameof(module));
			this.handle = (TypeDefinitionHandle)typeDefinition.MetadataToken;
			LazyLoading = true;
		}

		public override object Text
			=> Language.TypeToString(typeDefinition, ConversionFlags.None) + GetSuffixString(handle);

		public override object Icon {
			get {
				var baseImage = typeDefinition.Kind switch {
					TypeKind.Interface => Images.Interface,
					TypeKind.Struct or TypeKind.Void => Images.Struct,
					TypeKind.Delegate => Images.Delegate,
					TypeKind.Enum => Images.Enum,
					_ => Images.Class,
				};
				return Images.GetIcon(baseImage,
					Images.GetOverlay(typeDefinition.Accessibility), typeDefinition.IsStatic);
			}
		}

		public override bool CanExpandRecursively => true;

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
			=> language.DecompileType(typeDefinition, output, options);

		public override bool IsPublicAPI => typeDefinition.Accessibility switch {
			Accessibility.Public or Accessibility.Protected or Accessibility.ProtectedOrInternal => true,
			_ => false,
		};

		public override FilterResult Filter(LanguageSettings settings)
		{
			if (settings.ShowApiLevel == ApiVisibility.PublicOnly && !IsPublicAPI)
				return FilterResult.Hidden;
			if (settings.SearchTermMatches(typeDefinition.Name))
			{
				if (settings.ShowApiLevel == ApiVisibility.All || LanguageService.CurrentLanguage.ShowMember(typeDefinition))
					return FilterResult.Match;
				else
					return FilterResult.Hidden;
			}
			else
			{
				return FilterResult.Recurse;
			}
		}

		// Stable identity for SessionSettings.ActiveTreeViewPath. ReflectionName is
		// language-independent.
		public override string ToString() => typeDefinition.ReflectionName;

		// Sealed / static / value-type / enum / delegate cannot be the base of another class,
		// so a DerivedTypes child would always show up empty. Suppress it for those kinds.
		static bool CanHaveDerivedTypes(ITypeDefinition typeDef)
		{
			if (typeDef.IsSealed)
				return false;
			return typeDef.Kind switch {
				TypeKind.Class or TypeKind.Interface => true,
				_ => false,
			};
		}

		protected override void LoadChildren()
		{
			var typeDef = typeDefinition;

			// Inheritance-relation siblings come first so they sit above the type's own members.
			// BaseTypes is skipped for System.Object (no upstream chain) and for value types'
			// implicit System.ValueType base when there's nothing else to show — the AddBaseTypes
			// pass produces an empty set, which collapses the node.
			if (typeDef.DirectBaseTypes.Any())
				Children.Add(new BaseTypesTreeNode(module, typeDef));

			// DerivedTypes is meaningful only for non-sealed reference types (and abstract
			// classes / interfaces). Sealed classes can't be derived from; static classes are
			// implicitly sealed.
			var assemblyList = AppComposition.Current.GetExport<AssemblyTreeModel>().AssemblyList;
			if (assemblyList != null && CanHaveDerivedTypes(typeDef))
				Children.Add(new DerivedTypesTreeNode(assemblyList, typeDef));

			foreach (var nestedType in typeDef.NestedTypes
				.OrderBy(t => t.Name, NaturalStringComparer.Instance))
			{
				Children.Add(new TypeTreeNode(nestedType, module));
			}

			// C# 14 explicit-extension declaration blocks surface as their own container nodes
			// — one per (marker, type-params) tuple inside this static class. Filter()-hidden
			// when the user runs an older C# language version that doesn't recognise them, or
			// when DecompilerSettings.ExtensionMembers is off.
			if (typeDef.ExtensionInfo is { } ext)
			{
				var parentAssemblyNode = this.Ancestors().OfType<AssemblyTreeNode>().FirstOrDefault();
				if (parentAssemblyNode != null)
				{
					foreach (var group in ext.ExtensionGroups)
						Children.Add(new ExtensionTreeNode(typeDef, group, parentAssemblyNode));
				}
			}

			// Enums look more useful in declaration order than alphabetical.
			var fields = typeDef.Kind == TypeKind.Enum
				? typeDef.Fields
				: typeDef.Fields.OrderBy(f => f.Name, NaturalStringComparer.Instance);
			foreach (var field in fields)
				Children.Add(new FieldTreeNode(field));

			foreach (var prop in typeDef.Properties.OrderBy(p => p.Name, NaturalStringComparer.Instance))
				Children.Add(new PropertyTreeNode(prop));

			foreach (var ev in typeDef.Events.OrderBy(e => e.Name, NaturalStringComparer.Instance))
				Children.Add(new EventTreeNode(ev));

			foreach (var method in typeDef.Methods.OrderBy(m => m.Name, NaturalStringComparer.Instance))
			{
				if (method.MetadataToken.IsNil || method.IsAccessor)
					continue;
				Children.Add(new MethodTreeNode(method));
			}
		}
	}
}
