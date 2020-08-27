// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
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

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;

namespace ICSharpCode.Decompiler.Documentation
{
	/// <summary>
	/// A type reference of the form 'Some.Namespace.TopLevelType.NestedType`n'.
	/// We do not know the boundary between namespace name and top level type, so we have to try
	/// all possibilities.
	/// The type parameter count only applies to the innermost type, all outer types must be non-generic.
	/// </summary>
	[Serializable]
	public class GetPotentiallyNestedClassTypeReference : ITypeReference
	{
		readonly string typeName;
		readonly int typeParameterCount;

		public GetPotentiallyNestedClassTypeReference(string typeName, int typeParameterCount)
		{
			this.typeName = typeName;
			this.typeParameterCount = typeParameterCount;
		}

		public IType Resolve(ITypeResolveContext context)
		{
			string[] parts = typeName.Split('.');
			var assemblies = new[] { context.CurrentModule }.Concat(context.Compilation.Modules);
			for (int i = parts.Length - 1; i >= 0; i--)
			{
				string ns = string.Join(".", parts, 0, i);
				string name = parts[i];
				int topLevelTPC = (i == parts.Length - 1 ? typeParameterCount : 0);
				foreach (var asm in assemblies)
				{
					if (asm == null)
						continue;
					ITypeDefinition typeDef = asm.GetTypeDefinition(new TopLevelTypeName(ns, name, topLevelTPC));
					for (int j = i + 1; j < parts.Length && typeDef != null; j++)
					{
						int tpc = (j == parts.Length - 1 ? typeParameterCount : 0);
						typeDef = typeDef.NestedTypes.FirstOrDefault(n => n.Name == parts[j] && n.TypeParameterCount == tpc);
					}
					if (typeDef != null)
						return typeDef;
				}
			}
			int idx = typeName.LastIndexOf('.');
			if (idx < 0)
				return new UnknownType("", typeName, typeParameterCount);
			// give back a guessed namespace/type name
			return new UnknownType(typeName.Substring(0, idx), typeName.Substring(idx + 1), typeParameterCount);
		}

		/// <summary>
		/// Resolves the type reference within the context of the given PE file.
		/// </summary>
		/// <returns>Either TypeDefinitionHandle, if the type is defined in the module or ExportedTypeHandle,
		/// if the module contains a type forwarder. Returns a nil handle, if the type was not found.</returns>
		public EntityHandle ResolveInPEFile(PEFile module)
		{
			string[] parts = typeName.Split('.');
			for (int i = parts.Length - 1; i >= 0; i--)
			{
				string ns = string.Join(".", parts, 0, i);
				string name = parts[i];
				int topLevelTPC = (i == parts.Length - 1 ? typeParameterCount : 0);
				var topLevelName = new TopLevelTypeName(ns, name, topLevelTPC);
				var typeHandle = module.GetTypeDefinition(topLevelName);

				for (int j = i + 1; j < parts.Length && !typeHandle.IsNil; j++)
				{
					int tpc = (j == parts.Length - 1 ? typeParameterCount : 0);
					var typeDef = module.Metadata.GetTypeDefinition(typeHandle);
					string lookupName = parts[j] + (tpc > 0 ? "`" + tpc : "");
					typeHandle = typeDef.GetNestedTypes().FirstOrDefault(n => IsEqualShortName(n, module.Metadata, lookupName));
				}

				if (!typeHandle.IsNil)
					return typeHandle;
				FullTypeName typeName = topLevelName;
				for (int j = i + 1; j < parts.Length; j++)
				{
					int tpc = (j == parts.Length - 1 ? typeParameterCount : 0);
					typeName = typeName.NestedType(parts[j], tpc);
				}

				var exportedType = module.GetTypeForwarder(typeName);
				if (!exportedType.IsNil)
					return exportedType;
			}

			return default;

			bool IsEqualShortName(TypeDefinitionHandle h, MetadataReader metadata, string name)
			{
				var nestedType = metadata.GetTypeDefinition(h);
				return metadata.StringComparer.Equals(nestedType.Name, name);
			}
		}
	}
}
