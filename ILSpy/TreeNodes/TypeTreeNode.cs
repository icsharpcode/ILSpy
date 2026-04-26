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

namespace ILSpy.TreeNodes
{
	sealed class TypeTreeNode : ILSpyTreeNode
	{
		readonly TypeDefinitionHandle handle;
		readonly MetadataFile module;

		public TypeDefinitionHandle Handle => handle;
		public MetadataFile Module => module;

		public TypeTreeNode(TypeDefinitionHandle handle, MetadataFile module)
		{
			this.handle = handle;
			this.module = module ?? throw new ArgumentNullException(nameof(module));
			LazyLoading = true;
		}

		public override object Text {
			get {
				var typeDef = ResolveTypeDefinition();
				if (typeDef != null)
					return Language.TypeToString(typeDef, ConversionFlags.None);
				return module.Metadata.GetString(module.Metadata.GetTypeDefinition(handle).Name);
			}
		}

		public override object Icon {
			get {
				var typeDef = ResolveTypeDefinition();
				if (typeDef == null)
					return Images.Images.Class;
				var baseImage = typeDef.Kind switch {
					TypeKind.Interface => Images.Images.Interface,
					TypeKind.Struct or TypeKind.Void => Images.Images.Struct,
					TypeKind.Delegate => Images.Images.Delegate,
					TypeKind.Enum => Images.Images.Enum,
					_ => Images.Images.Class,
				};
				return Images.Images.GetIcon(baseImage,
					Images.Images.GetOverlay(typeDef.Accessibility), typeDef.IsStatic);
			}
		}

		public override bool CanExpandRecursively => true;

		// Stable identity for SessionSettings.ActiveTreeViewPath. ReflectionName is
		// language-independent.
		public override string ToString()
		{
			var typeDef = ResolveTypeDefinition();
			return typeDef?.ReflectionName ?? module.Metadata.GetString(module.Metadata.GetTypeDefinition(handle).Name);
		}

		ITypeDefinition? ResolveTypeDefinition()
		{
			var typeSystem = module.GetTypeSystemOrNull();
			if (typeSystem == null)
				return null;
			return ((MetadataModule)typeSystem.MainModule).GetDefinition(handle);
		}

		protected override void LoadChildren()
		{
			var typeDef = ResolveTypeDefinition();
			if (typeDef == null)
				return;

			foreach (var nestedType in typeDef.NestedTypes
				.OrderBy(t => t.Name, NaturalStringComparer.Instance))
			{
				Children.Add(new TypeTreeNode((TypeDefinitionHandle)nestedType.MetadataToken, module));
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
