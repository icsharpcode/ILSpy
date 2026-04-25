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

using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;

using ICSharpCode.Decompiler.Metadata;

using ILSpy.Images;

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
			this.module = module;
			LazyLoading = true;
		}

		public override object Text {
			get {
				var td = module.Metadata.GetTypeDefinition(handle);
				return module.Metadata.GetString(td.Name);
			}
		}

		public override object Icon {
			get {
				var td = module.Metadata.GetTypeDefinition(handle);
				var attrs = td.Attributes;
				if ((attrs & TypeAttributes.ClassSemanticsMask) == TypeAttributes.Interface)
					return Images.Images.Interface;
				// crude detection: real distinction needs base-type lookups (System.Enum, ValueType,
				// MulticastDelegate). Refining will land with the broader tree-node port.
				return Images.Images.Class;
			}
		}

		protected override void LoadChildren()
		{
			var td = module.Metadata.GetTypeDefinition(handle);

			foreach (var nestedType in td.GetNestedTypes())
				Children.Add(new TypeTreeNode(nestedType, module));

			foreach (var method in td.GetMethods())
			{
				var md = module.Metadata.GetMethodDefinition(method);
				var name = module.Metadata.GetString(md.Name);
				var icon = name == ".ctor" || name == ".cctor"
					? Images.Images.Constructor
					: ((md.Attributes & MethodAttributes.SpecialName) != 0 && name.StartsWith("op_"))
						? Images.Images.Operator
						: Images.Images.Method;
				Children.Add(new MemberTreeNode(name, icon));
			}

			foreach (var field in td.GetFields())
			{
				var fd = module.Metadata.GetFieldDefinition(field);
				Children.Add(new MemberTreeNode(module.Metadata.GetString(fd.Name), Images.Images.Field));
			}

			foreach (var prop in td.GetProperties())
			{
				var pd = module.Metadata.GetPropertyDefinition(prop);
				Children.Add(new MemberTreeNode(module.Metadata.GetString(pd.Name), Images.Images.Property));
			}

			foreach (var evt in td.GetEvents())
			{
				var ed = module.Metadata.GetEventDefinition(evt);
				Children.Add(new MemberTreeNode(module.Metadata.GetString(ed.Name), Images.Images.Event));
			}
		}
	}
}
