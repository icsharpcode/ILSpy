// Copyright (c) 2018 Daniel Grunwald
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
using System.Collections.Immutable;
using System.Reflection.Metadata.Ecma335;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using SRM = System.Reflection.Metadata;

namespace ICSharpCode.Decompiler.TypeSystem
{
	/// <summary>
	/// Allows decoding signatures using decompiler types.
	/// </summary>
	sealed class TypeProvider : SRM.ISignatureTypeProvider<IType, ITypeResolveContext>, SRM.ICustomAttributeTypeProvider<IType>
	{
		readonly IAssembly assembly;
		readonly ICompilation compilation;

		public TypeProvider(IAssembly assembly)
		{
			this.assembly = assembly;
			this.compilation = assembly.Compilation;
		}

		public IType GetArrayType(IType elementType, SRM.ArrayShape shape)
		{
			return new ArrayType(compilation, elementType, shape.Rank);
		}

		public IType GetByReferenceType(IType elementType)
		{
			return new ByReferenceType(elementType);
		}

		public IType GetFunctionPointerType(SRM.MethodSignature<IType> signature)
		{
			return compilation.FindType(KnownTypeCode.IntPtr);
		}

		public IType GetGenericInstantiation(IType genericType, ImmutableArray<IType> typeArguments)
		{
			return new ParameterizedType(genericType, typeArguments);
		}

		public IType GetGenericMethodParameter(ITypeResolveContext genericContext, int index)
		{
			// Note: returning type parameter, never type argument.
			// Otherwise we risk screwing up the counting for dynamicTypeIndex.
			IMethod method = genericContext.CurrentMember as IMethod;
			if (method != null && index < method.TypeParameters.Count) {
				return method.TypeParameters[index];
			}
			return DummyTypeParameter.GetMethodTypeParameter(index);
		}

		public IType GetGenericTypeParameter(ITypeResolveContext genericContext, int index)
		{
			ITypeDefinition typeDef = genericContext.CurrentTypeDefinition;
			if (typeDef != null && index < typeDef.TypeParameters.Count) {
				return typeDef.TypeParameters[index];
			}
			return DummyTypeParameter.GetClassTypeParameter(index);
		}

		public IType GetModifiedType(IType modifier, IType unmodifiedType, bool isRequired)
		{
			return unmodifiedType;
		}

		public IType GetPinnedType(IType elementType)
		{
			return new PinnedType(elementType);
		}

		public IType GetPointerType(IType elementType)
		{
			return new PointerType(elementType);
		}

		public IType GetPrimitiveType(SRM.PrimitiveTypeCode typeCode)
		{
			return compilation.FindType(typeCode.ToKnownTypeCode());
		}

		public IType GetSystemType()
		{
			return compilation.FindType(KnownTypeCode.Type);
		}

		public IType GetSZArrayType(IType elementType)
		{
			return new ArrayType(compilation, elementType);
		}

		public IType GetTypeFromDefinition(SRM.MetadataReader reader, SRM.TypeDefinitionHandle handle, byte rawTypeKind)
		{
			ITypeDefinition td = assembly.ResolveTypeDefToken(handle);
			if (td != null)
				return td;
			return SpecialType.UnknownType;
		}

		public IType GetTypeFromReference(SRM.MetadataReader reader, SRM.TypeReferenceHandle handle, byte rawTypeKind)
		{
			var asmref = handle.GetDeclaringAssembly(reader);
			IAssemblyReference nrAsmRef;
			if (asmref.IsNil)
				nrAsmRef = DefaultAssemblyReference.CurrentAssembly;
			else
				nrAsmRef = new DefaultAssemblyReference(reader.GetString(reader.GetAssemblyReference(asmref).Name));
			bool? isReferenceType = null;
			switch (reader.ResolveSignatureTypeKind(handle, rawTypeKind)) {
				case SRM.SignatureTypeKind.ValueType:
					isReferenceType = false;
					break;
				case SRM.SignatureTypeKind.Class:
					isReferenceType = true;
					break;
			}
			var gctr = new GetClassTypeReference(handle.GetFullTypeName(reader), nrAsmRef, isReferenceType);
			return gctr.Resolve(new SimpleTypeResolveContext(assembly));
		}

		public IType GetTypeFromSerializedName(string name)
		{
			// TODO: aren't we missing support for assembly-qualified names?
			return new GetClassTypeReference(new FullTypeName(name))
				.Resolve(new SimpleTypeResolveContext(assembly));
		}

		public IType GetTypeFromSpecification(SRM.MetadataReader reader, ITypeResolveContext genericContext, SRM.TypeSpecificationHandle handle, byte rawTypeKind)
		{
			return reader.GetTypeSpecification(handle).DecodeSignature(this, genericContext);
		}

		public SRM.PrimitiveTypeCode GetUnderlyingEnumType(IType type)
		{
			var def = type.GetEnumUnderlyingType().GetDefinition();
			if (def == null)
				throw new InvalidOperationException();
			return def.KnownTypeCode.ToPrimitiveTypeCode();
		}

		public bool IsSystemType(IType type)
		{
			return type.IsKnownType(KnownTypeCode.Type);
		}
	}
}
