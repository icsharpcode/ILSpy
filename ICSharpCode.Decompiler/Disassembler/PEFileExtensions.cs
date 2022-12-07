// Copyright (c) 2022 Tom-Englert
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

#nullable enable

using System;
using System.Linq;
using System.Reflection.Metadata;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.Decompiler.Disassembler
{
	public static class PEFileExtensions
	{
		public static TypeDefinitionHandle FindType(this PEFile file, string typeName)
		{
			TypeDefinitionHandle type = file.Metadata.TypeDefinitions.SingleOrDefault(handle =>
				handle.GetFullTypeName(file.Metadata).ToCSharpNameString() == typeName);

			if (type == default)
			{
				throw new InvalidOperationException($"Could not find '{typeName}' in '{file.FileName}'");
			}

			return type;
		}

		public static PropertyDefinitionHandle FindProperty(this PEFile file, string typeName, string propertyName)
		{
			return FindProperty(file, file.FindType(typeName), propertyName);
		}

		public static PropertyDefinitionHandle FindProperty(this PEFile file, TypeDefinitionHandle type,
			string propertyName)
		{
			MetadataReader metadata = file.Metadata;
			TypeDefinition typeDefinition = metadata.GetTypeDefinition(type);

			PropertyDefinitionHandle property = typeDefinition.GetProperties().SingleOrDefault(handle => {
				PropertyDefinition definition = metadata.GetPropertyDefinition(handle);
				string name = metadata.GetString(definition.Name);
				return name == propertyName;
			});

			if (property == default)
			{
				throw new InvalidOperationException(
					$"Could not find '{type.GetFullTypeName(metadata).ToCSharpNameString()}.{propertyName}' in '{file.FileName}'");
			}

			return property;
		}

		public static MethodDefinitionHandle FindMethod(this PEFile file, string typeName, string methodName,
			Func<IMethodSignature, bool>? predicate = null)
		{
			return FindMethod(file, file.FindType(typeName), methodName, predicate);
		}

		public static MethodDefinitionHandle FindMethod(this PEFile file, TypeDefinitionHandle type, string methodName,
			Func<IMethodSignature, bool>? predicate = null)
		{
			MetadataReader metadata = file.Metadata;
			TypeDefinition typeDefinition = metadata.GetTypeDefinition(type);

			MethodDefinitionHandle handle = typeDefinition.GetMethods()
				.Select(handle => handle.GetMethodSignature(file))
				.Where(signature => signature.Name == methodName && predicate?.Invoke(signature) != false)
				.Select(signature => signature.Handle)
				.SingleOrDefault();

			if (handle == default)
			{
				throw new InvalidOperationException(
					$"Could not find '{type.GetFullTypeName(metadata).ToCSharpNameString()}.{methodName}' in '{file.FileName}'");
			}

			return handle;
		}
	}
}