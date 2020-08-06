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
	sealed class TypeProvider : ICompilationProvider,
		SRM.ISignatureTypeProvider<IType, GenericContext>,
		SRM.ICustomAttributeTypeProvider<IType>
	{
		readonly MetadataModule module;
		readonly ICompilation compilation;

		public TypeProvider(MetadataModule module)
		{
			this.module = module;
			this.compilation = module.Compilation;
		}

		public TypeProvider(ICompilation compilation)
		{
			this.compilation = compilation;
		}

		public ICompilation Compilation => compilation;

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

		public IType GetGenericMethodParameter(GenericContext genericContext, int index)
		{
			return genericContext.GetMethodTypeParameter(index);
		}

		public IType GetGenericTypeParameter(GenericContext genericContext, int index)
		{
			return genericContext.GetClassTypeParameter(index);
		}

		public IType GetModifiedType(IType modifier, IType unmodifiedType, bool isRequired)
		{
			return new ModifiedType(modifier, unmodifiedType, isRequired);
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

		bool? IsReferenceType(SRM.MetadataReader reader, SRM.EntityHandle handle, byte rawTypeKind)
		{
			switch (reader.ResolveSignatureTypeKind(handle, rawTypeKind)) {
				case SRM.SignatureTypeKind.ValueType:
					return false;
				case SRM.SignatureTypeKind.Class:
					return true;
				default:
					return null;
			}
		}

		public IType GetTypeFromDefinition(SRM.MetadataReader reader, SRM.TypeDefinitionHandle handle, byte rawTypeKind)
		{
			ITypeDefinition td = module?.GetDefinition(handle);
			if (td != null)
				return td;
			bool? isReferenceType = IsReferenceType(reader, handle, rawTypeKind);
			return new UnknownType(handle.GetFullTypeName(reader), isReferenceType);
		}

		public IType GetTypeFromReference(SRM.MetadataReader reader, SRM.TypeReferenceHandle handle, byte rawTypeKind)
		{
			IModule resolvedModule = module.GetDeclaringModule(handle);
			var fullTypeName = handle.GetFullTypeName(reader);
			IType type;
			if (resolvedModule != null) {
				type = resolvedModule.GetTypeDefinition(fullTypeName);
			} else {
				type = GetClassTypeReference.ResolveInAllAssemblies(compilation, in fullTypeName);
			}
			return type ?? new UnknownType(fullTypeName, IsReferenceType(reader, handle, rawTypeKind));
		}

		public IType GetTypeFromSerializedName(string name)
		{
			if (name == null) {
				return null;
			}
			try {
				return ReflectionHelper.ParseReflectionName(name)
					.Resolve(module != null ? new SimpleTypeResolveContext(module) : new SimpleTypeResolveContext(compilation));
			} catch (ReflectionNameParseException ex) {
				throw new BadImageFormatException($"Invalid type name: \"{name}\": {ex.Message}");
			}
		}
		
		public IType GetTypeFromSpecification(SRM.MetadataReader reader, GenericContext genericContext, SRM.TypeSpecificationHandle handle, byte rawTypeKind)
		{
			return reader.GetTypeSpecification(handle).DecodeSignature<IType, GenericContext>(this, genericContext);
		}

		public SRM.PrimitiveTypeCode GetUnderlyingEnumType(IType type)
		{
			var def = type.GetEnumUnderlyingType().GetDefinition();
			if (def == null)
				throw new EnumUnderlyingTypeResolveException();
			return def.KnownTypeCode.ToPrimitiveTypeCode();
		}

		public bool IsSystemType(IType type)
		{
			return type.IsKnownType(KnownTypeCode.Type);
		}
	}
}
