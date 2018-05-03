// Copyright (c) 2017 Siegfried Pammer
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
using System.Linq;
using SRM = System.Reflection.Metadata;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Util;
using System.Collections.Generic;

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	public sealed class PinnedType : TypeWithElementType
	{
		public PinnedType(IType elementType)
			: base(elementType)
		{
		}

		public override string NameSuffix => " pinned";

		public override bool? IsReferenceType => elementType.IsReferenceType;

		public override TypeKind Kind => TypeKind.Other;

		public override ITypeReference ToTypeReference()
		{
			return new PinnedTypeReference(elementType.ToTypeReference());
		}

		public override IType VisitChildren(TypeVisitor visitor)
		{
			var newType = elementType.AcceptVisitor(visitor);
			if (newType == elementType)
				return this;
			return new PinnedType(newType);
		}
	}

	public sealed class PinnedTypeReference : ITypeReference
	{
		public ITypeReference ElementType { get; }

		public PinnedTypeReference(ITypeReference elementType)
		{
			ElementType = elementType;
		}

		public IType Resolve(ITypeResolveContext context)
		{
			return new PinnedType(ElementType.Resolve(context));
		}
	}

	public sealed class ModifiedTypeReference : ITypeReference
	{
		public ITypeReference ElementType { get; }
		public ITypeReference ModifierType { get; }
		public bool IsRequired { get; }

		public ModifiedTypeReference(ITypeReference elementType, ITypeReference modifierType, bool isRequired)
		{
			ElementType = elementType;
			ModifierType = modifierType;
			IsRequired = isRequired;
		}

		public IType Resolve(ITypeResolveContext context)
		{
			return ElementType.Resolve(context);
		}
	}

	public sealed class DynamicTypeReference : TypeVisitor, ITypeReference
	{
		readonly ITypeReference reference;
		readonly bool[] dynamicInfo;
		int typeIndex;

		static readonly ITypeResolveContext minimalCorlibContext = new SimpleTypeResolveContext(MinimalCorlib.Instance.CreateCompilation());

		public static ITypeReference Create(ITypeReference reference, SRM.CustomAttributeHandleCollection? customAttributes, SRM.MetadataReader metadata)
		{
			if (HasDynamicAttribute(customAttributes, metadata, out var dynamicInfo))
				return new DynamicTypeReference(reference, dynamicInfo);
			return reference;
		}

		public static bool HasDynamicAttribute(SRM.CustomAttributeHandleCollection? attributes, SRM.MetadataReader metadata, out bool[] mapping)
		{
			mapping = null;
			if (attributes == null)
				return false;

			foreach (var handle in attributes) {
				var a = metadata.GetCustomAttribute(handle);
				var type = a.GetAttributeType(metadata);
				if (type.GetFullTypeName(metadata).ToString() == "System.Runtime.CompilerServices.DynamicAttribute") {
					var ctor = a.DecodeValue(new TypeSystemAttributeTypeProvider(minimalCorlibContext));
					if (ctor.FixedArguments.Length == 1) {
						var arg = ctor.FixedArguments[0];
						if (arg.Type.ReflectionName == "System.Boolean[]" && arg.Value is ImmutableArray<SRM.CustomAttributeTypedArgument<IType>> values) {
							mapping = values.SelectArray(v => (bool)v.Value);
							return true;
						}
					}
					return true;
				}
			}
			return false;
		}

		DynamicTypeReference(ITypeReference reference, bool[] dynamicInfo)
		{
			this.reference = reference;
			this.dynamicInfo = dynamicInfo;
		}

		public IType Resolve(ITypeResolveContext context)
		{
			return reference.Resolve(context).AcceptVisitor(this);
		}

		public override IType VisitPointerType(PointerType type)
		{
			typeIndex++;
			return base.VisitPointerType(type);
		}

		public override IType VisitArrayType(ArrayType type)
		{
			typeIndex++;
			return base.VisitArrayType(type);
		}

		public override IType VisitByReferenceType(ByReferenceType type)
		{
			typeIndex++;
			return base.VisitByReferenceType(type);
		}

		public override IType VisitParameterizedType(ParameterizedType type)
		{
			var genericType = type.GenericType.AcceptVisitor(this);
			bool changed = type.GenericType != genericType;
			var arguments = new IType[type.TypeArguments.Count];
			for (int i = 0; i < type.TypeArguments.Count; i++) {
				typeIndex++;
				arguments[i] = type.TypeArguments[i].AcceptVisitor(this);
				changed = changed || arguments[i] != type.TypeArguments[i];
			}
			if (!changed)
				return type;
			return new ParameterizedType(genericType, arguments);
		}

		public override IType VisitTypeDefinition(ITypeDefinition type)
		{
			if (type.KnownTypeCode == KnownTypeCode.Object) {
				if (dynamicInfo == null || typeIndex >= dynamicInfo.Length)
					return SpecialType.Dynamic;
				if (dynamicInfo[typeIndex])
					return SpecialType.Dynamic;
				return type;
			}
			return type;
		}
	}

	sealed class TypeDefTokenTypeReference : ITypeReference
	{
		readonly SRM.EntityHandle token;

		public TypeDefTokenTypeReference(SRM.EntityHandle token)
		{
			if (token.Kind != SRM.HandleKind.TypeDefinition)
				throw new ArgumentException(nameof(token), "must be TypeDef token");
			this.token = token;
		}

		public IType Resolve(ITypeResolveContext context)
		{
			ITypeDefinition td = context.CurrentAssembly.ResolveTypeDefToken(token);
			if (td != null)
				return td;
			return SpecialType.UnknownType;
		}
	}

	class TypeReferenceSignatureDecoder : SRM.ISignatureTypeProvider<ITypeReference, Unit>
	{
		public static readonly TypeReferenceSignatureDecoder Instance = new TypeReferenceSignatureDecoder();

		private TypeReferenceSignatureDecoder() { }

		public ITypeReference GetArrayType(ITypeReference elementType, SRM.ArrayShape shape)
		{
			return new ArrayTypeReference(elementType, shape.Rank);
		}

		public ITypeReference GetByReferenceType(ITypeReference elementType)
		{
			return new ByReferenceTypeReference(elementType);
		}

		public ITypeReference GetFunctionPointerType(SRM.MethodSignature<ITypeReference> signature)
		{
			return KnownTypeReference.IntPtr;
		}

		public ITypeReference GetGenericInstantiation(ITypeReference genericType, ImmutableArray<ITypeReference> typeArguments)
		{
			return new ParameterizedTypeReference(genericType, typeArguments);
		}

		public ITypeReference GetGenericMethodParameter(Unit genericContext, int index)
		{
			return TypeParameterReference.Create(SymbolKind.Method, index);
		}

		public ITypeReference GetGenericTypeParameter(Unit genericContext, int index)
		{
			return TypeParameterReference.Create(SymbolKind.TypeDefinition, index);
		}

		public ITypeReference GetModifiedType(ITypeReference modifier, ITypeReference unmodifiedType, bool isRequired)
		{
			return new ModifiedTypeReference(unmodifiedType, modifier, isRequired);
		}

		public ITypeReference GetPinnedType(ITypeReference elementType)
		{
			return new PinnedTypeReference(elementType);
		}

		public ITypeReference GetPointerType(ITypeReference elementType)
		{
			return new PointerTypeReference(elementType);
		}

		public ITypeReference GetPrimitiveType(SRM.PrimitiveTypeCode typeCode)
		{
			return KnownTypeReference.Get(typeCode.ToKnownTypeCode());
		}

		public ITypeReference GetSZArrayType(ITypeReference elementType)
		{
			return new ArrayTypeReference(elementType);
		}

		public ITypeReference GetTypeFromDefinition(SRM.MetadataReader reader, SRM.TypeDefinitionHandle handle, byte rawTypeKind)
		{
			return new TypeDefTokenTypeReference(handle);
		}

		public ITypeReference GetTypeFromReference(SRM.MetadataReader reader, SRM.TypeReferenceHandle handle, byte rawTypeKind)
		{
			var asmref = handle.GetDeclaringAssembly(reader);
			if (asmref.IsNil)
				return new GetClassTypeReference(handle.GetFullTypeName(reader), DefaultAssemblyReference.CurrentAssembly);
			var asm = reader.GetAssemblyReference(asmref);
			return new GetClassTypeReference(handle.GetFullTypeName(reader), new DefaultAssemblyReference(reader.GetString(asm.Name)));
		}

		public ITypeReference GetTypeFromSpecification(SRM.MetadataReader reader, Unit genericContext, SRM.TypeSpecificationHandle handle, byte rawTypeKind)
		{
			return reader.GetTypeSpecification(handle)
				.DecodeSignature(this, default);
		}
	}

	public class TypeSystemAttributeTypeProvider : SRM.ICustomAttributeTypeProvider<IType>
	{
		readonly ITypeResolveContext context;

		public static TypeSystemAttributeTypeProvider CreateDefault() => new TypeSystemAttributeTypeProvider(new SimpleTypeResolveContext(MinimalCorlib.Instance.CreateCompilation()));

		public TypeSystemAttributeTypeProvider(ITypeResolveContext context)
		{
			this.context = context;
		}

		public IType GetPrimitiveType(SRM.PrimitiveTypeCode typeCode)
		{
			return context.Compilation.FindType(typeCode.ToKnownTypeCode());
		}

		public IType GetSystemType()
		{
			return context.Compilation.FindType(KnownTypeCode.Type);
		}

		public IType GetSZArrayType(IType elementType)
		{
			return new ArrayType(context.Compilation, elementType);
		}

		public IType GetTypeFromDefinition(SRM.MetadataReader reader, SRM.TypeDefinitionHandle handle, byte rawTypeKind)
		{
			var type = reader.GetTypeDefinition(handle);
			return new DefaultUnresolvedTypeDefinition(type.GetFullTypeName(reader).ToString()).Resolve(context);
		}

		public IType GetTypeFromReference(SRM.MetadataReader reader, SRM.TypeReferenceHandle handle, byte rawTypeKind)
		{
			return new DefaultUnresolvedTypeDefinition(handle.GetFullTypeName(reader).ToString()).Resolve(context);
		}

		public IType GetTypeFromSerializedName(string name)
		{
			return new GetClassTypeReference(new FullTypeName(name)).Resolve(context);
		}

		public SRM.PrimitiveTypeCode GetUnderlyingEnumType(IType type)
		{
			var def = type.GetEnumUnderlyingType().GetDefinition();
			if (def == null)
				throw new InvalidOperationException();
			return def.KnownTypeCode.ToPrimtiveTypeCode();
		}

		public bool IsSystemType(IType type)
		{
			return type.IsKnownType(KnownTypeCode.Type);
		}
	}
}
