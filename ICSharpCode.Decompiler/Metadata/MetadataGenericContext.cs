// Copyright (c) 2018 Siegfried Pammer
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

using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace ICSharpCode.Decompiler.Metadata
{
	public readonly struct MetadataGenericContext
	{
		readonly MetadataReader? metadata;
		readonly TypeDefinitionHandle declaringType;
		readonly MethodDefinitionHandle method;

		public MetadataGenericContext(MethodDefinitionHandle method, PEFile module)
		{
			this.metadata = module.Metadata;
			this.method = method;
			this.declaringType = module.Metadata.GetMethodDefinition(method).GetDeclaringType();
		}

		public MetadataGenericContext(MethodDefinitionHandle method, MetadataReader metadata)
		{
			this.metadata = metadata;
			this.method = method;
			this.declaringType = metadata.GetMethodDefinition(method).GetDeclaringType();
		}

		public MetadataGenericContext(TypeDefinitionHandle declaringType, PEFile module)
		{
			this.metadata = module.Metadata;
			this.method = default;
			this.declaringType = declaringType;
		}

		public MetadataGenericContext(TypeDefinitionHandle declaringType, MetadataReader metadata)
		{
			this.metadata = metadata;
			this.method = default;
			this.declaringType = declaringType;
		}

		public string GetGenericTypeParameterName(int index)
		{
			GenericParameterHandle genericParameter = GetGenericTypeParameterHandleOrNull(index);
			if (genericParameter.IsNil || metadata == null)
				return index.ToString();
			return metadata.GetString(metadata.GetGenericParameter(genericParameter).Name);
		}

		public string GetGenericMethodTypeParameterName(int index)
		{
			GenericParameterHandle genericParameter = GetGenericMethodTypeParameterHandleOrNull(index);
			if (genericParameter.IsNil || metadata == null)
				return index.ToString();
			return metadata.GetString(metadata.GetGenericParameter(genericParameter).Name);
		}

		public GenericParameterHandle GetGenericTypeParameterHandleOrNull(int index)
		{
			if (declaringType.IsNil || index < 0 || metadata == null)
				return MetadataTokens.GenericParameterHandle(0);
			var genericParameters = metadata.GetTypeDefinition(declaringType).GetGenericParameters();
			if (index >= genericParameters.Count)
				return MetadataTokens.GenericParameterHandle(0);
			return genericParameters[index];
		}

		public GenericParameterHandle GetGenericMethodTypeParameterHandleOrNull(int index)
		{
			if (method.IsNil || index < 0 || metadata == null)
				return MetadataTokens.GenericParameterHandle(0);
			var genericParameters = metadata.GetMethodDefinition(method).GetGenericParameters();
			if (index >= genericParameters.Count)
				return MetadataTokens.GenericParameterHandle(0);
			return genericParameters[index];
		}
	}
}
