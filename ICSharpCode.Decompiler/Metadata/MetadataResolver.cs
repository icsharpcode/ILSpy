using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Decompiler.Util;
using SRM = System.Reflection.Metadata;

namespace ICSharpCode.Decompiler.Metadata
{
	public interface IMetadataResolveContext
	{
		PEFile CurrentModule { get; }
		PEFile ResolveAssembly(IAssemblyReference reference);
	}

	public class SimpleMetadataResolveContext : IMetadataResolveContext
	{
		readonly PEFile mainModule;
		readonly IAssemblyResolver assemblyResolver;
		readonly Dictionary<IAssemblyReference, PEFile> loadedModules;

		public SimpleMetadataResolveContext(PEFile mainModule)
		{
			this.mainModule = mainModule;
			this.assemblyResolver = mainModule.AssemblyResolver;
			this.loadedModules = new Dictionary<IAssemblyReference, PEFile>();
		}

		public SimpleMetadataResolveContext(PEFile mainModule, IMetadataResolveContext parentContext)
		{
			this.mainModule = mainModule;
			this.assemblyResolver = mainModule.AssemblyResolver;
			this.loadedModules = parentContext is SimpleMetadataResolveContext simple ? simple.loadedModules : new Dictionary<IAssemblyReference, PEFile>();
		}

		public PEFile CurrentModule => mainModule;

		public PEFile ResolveAssembly(IAssemblyReference reference)
		{
			if (loadedModules.TryGetValue(reference, out var module))
				return module;
			var resolved = assemblyResolver.Resolve(reference);
			loadedModules.Add(reference, resolved);
			return resolved;
		}
	}

	public static class MetadataResolver
	{
		public static TypeDefinition ResolveType(EntityHandle handle, IMetadataResolveContext context)
		{
			switch (handle.Kind) {
				case HandleKind.TypeDefinition:
					return new TypeDefinition(context.CurrentModule, (TypeDefinitionHandle)handle);
				case HandleKind.TypeReference:
					return Resolve((TypeReferenceHandle)handle, context);
				case HandleKind.TypeSpecification:
					return Resolve((TypeSpecificationHandle)handle, context);
				default:
					throw new NotSupportedException();
			}
		}

		public static MethodDefinition ResolveAsMethod(EntityHandle handle, IMetadataResolveContext context)
		{
			switch (handle.Kind) {
				case HandleKind.MethodDefinition:
					return new MethodDefinition(context.CurrentModule, (MethodDefinitionHandle)handle);
				case HandleKind.MemberReference:
					var resolved = ((MemberReferenceHandle)handle).Resolve(context);
					if (resolved is MethodDefinition m)
						return m;
					return default;
				case HandleKind.MethodSpecification:
					resolved = ((MethodSpecificationHandle)handle).Resolve(context);
					if (resolved is MethodDefinition m2)
						return m2;
					return default;
			}
			throw new NotImplementedException();
		}

		public static FieldDefinition ResolveAsField(EntityHandle handle, IMetadataResolveContext context)
		{
			switch (handle.Kind) {
				case HandleKind.FieldDefinition:
					return new FieldDefinition(context.CurrentModule, (FieldDefinitionHandle)handle);
				case HandleKind.MemberReference:
					var resolved = ((MemberReferenceHandle)handle).Resolve(context);
					if (resolved is FieldDefinition m)
						return m;
					return default;
			}
			throw new NotImplementedException();
		}

		/// <summary>
		/// Implements resolving of TypeReferences to TypeDefinitions as decribed in II.7.3 of ECMA-335 6th edition.
		/// </summary>
		public static TypeDefinition Resolve(this TypeReferenceHandle handle, IMetadataResolveContext context)
		{
			var metadata = context.CurrentModule.Metadata;
			var tr = metadata.GetTypeReference(handle);
			if (tr.ResolutionScope.IsNil) {
				foreach (var h in metadata.ExportedTypes) {
					var exportedType = metadata.GetExportedType(h);
					if (exportedType.Name == tr.Name && exportedType.Namespace == tr.Namespace) {
						switch (exportedType.Implementation.Kind) {
							case HandleKind.AssemblyFile:
								throw new NotSupportedException();
							case HandleKind.AssemblyReference:
								return ResolveTypeInOtherAssembly((AssemblyReferenceHandle)exportedType.Implementation, metadata.GetString(tr.Namespace), metadata.GetString(tr.Name));
							default:
								throw new NotSupportedException();
						}
					}
				}
			}
			switch (tr.ResolutionScope.Kind) {
				case HandleKind.TypeReference:
					var outerType = Resolve((TypeReferenceHandle)tr.ResolutionScope, context);
					if (outerType == null)
						throw new NotSupportedException();
					var td = outerType.Module.Metadata.GetTypeDefinition(outerType.Handle);
					var name = metadata.GetString(tr.Name);
					foreach (var nestedType in td.GetNestedTypes()) {
						var nestedTypeDef = outerType.Module.Metadata.GetTypeDefinition(nestedType);
						if (outerType.Module.Metadata.GetString(nestedTypeDef.Name) == name)
							return new TypeDefinition(outerType.Module, nestedType);
					}
					break;
				case HandleKind.ModuleReference:
					break;
				case HandleKind.AssemblyReference:
					return ResolveTypeInOtherAssembly((AssemblyReferenceHandle)tr.ResolutionScope, metadata.GetString(tr.Namespace), metadata.GetString(tr.Name));
			}
			throw new NotSupportedException();

			TypeDefinition ResolveTypeInOtherAssembly(AssemblyReferenceHandle asm, string ns, string typeName)
			{
				var module = context.ResolveAssembly(new AssemblyReference(context.CurrentModule, (AssemblyReferenceHandle)tr.ResolutionScope));
				var moduleMetadata = module.Metadata;
				var @namespace = ResolveNamespace(moduleMetadata, ns.Split('.'));
				if (@namespace == null)
					throw new NotSupportedException();
				var type = FindTypeInNamespace(moduleMetadata, @namespace.Value, typeName);
				if (type.IsNil)
					throw new NotSupportedException();
				return new TypeDefinition(module, type);
			}
		}

		static NamespaceDefinition? ResolveNamespace(MetadataReader metadata, string[] namespaceParts)
		{
			var currentNamespace = metadata.GetNamespaceDefinitionRoot();
			for (int i = 0; i < namespaceParts.Length; i++) {
				string identifier = namespaceParts[i];
				var next = currentNamespace.NamespaceDefinitions.FirstOrDefault(ns => metadata.StringComparer.Equals(ns, identifier));
				if (next.IsNil)
					return null;
				currentNamespace = metadata.GetNamespaceDefinition(next);
			}
			return currentNamespace;
		}

		static TypeDefinitionHandle FindTypeInNamespace(MetadataReader metadata, NamespaceDefinition @namespace, string name)
		{
			foreach (var type in @namespace.TypeDefinitions) {
				if (metadata.StringComparer.Equals(metadata.GetTypeDefinition(type).Name, name))
					return type;
			}
			return default;
		}

		public static IMetadataEntity Resolve(this MemberReferenceHandle handle, IMetadataResolveContext context)
		{
			var metadata = context.CurrentModule.Metadata;
			var mr = metadata.GetMemberReference(handle);
			SRM.TypeDefinition declaringType;
			MetadataReader targetMetadata;
			PEFile targetModule;
			switch (mr.Parent.Kind) {
				case HandleKind.TypeDefinition:
					declaringType = metadata.GetTypeDefinition((TypeDefinitionHandle)mr.Parent);
					targetMetadata = metadata;
					targetModule = context.CurrentModule;
					break;
				case HandleKind.TypeReference:
					var resolvedTypeReference = Resolve((TypeReferenceHandle)mr.Parent, context);
					targetModule = resolvedTypeReference.Module;
					targetMetadata = targetModule.Metadata;
					declaringType = targetMetadata.GetTypeDefinition(resolvedTypeReference.Handle);
					break;
				case HandleKind.TypeSpecification:
					resolvedTypeReference = Resolve((TypeSpecificationHandle)mr.Parent, context);
					targetModule = resolvedTypeReference.Module;
					targetMetadata = targetModule.Metadata;
					declaringType = targetMetadata.GetTypeDefinition(resolvedTypeReference.Handle);
					break;
				case HandleKind.MethodDefinition:
				case HandleKind.ModuleReference:
					throw new NotImplementedException();
				default:
					throw new NotSupportedException();
			}
			var name = metadata.GetString(mr.Name);
			switch (mr.GetKind()) {
				case MemberReferenceKind.Field:
					foreach (var f in declaringType.GetFields()) {
						var fd = targetMetadata.GetFieldDefinition(f);
						if (targetMetadata.StringComparer.Equals(fd.Name, name))
							return new FieldDefinition(targetModule, f);
					}
					throw new NotSupportedException();
				case MemberReferenceKind.Method:
					var candidates = new List<(MethodDefinitionHandle, BlobHandle)>();
					foreach (var m in declaringType.GetMethods()) {
						var md = targetMetadata.GetMethodDefinition(m);
						if (targetMetadata.StringComparer.Equals(md.Name, name))
							candidates.Add((m, md.Signature));
					}
					if (candidates.Count == 0)
						throw new NotSupportedException();
					foreach (var (method, signature) in candidates) {
						if (SignatureBlobComparer.EqualsMethodSignature(targetMetadata.GetBlobReader(signature), metadata.GetBlobReader(mr.Signature), targetMetadata, metadata))
							return new MethodDefinition(targetModule, method);
					}
					throw new NotSupportedException();
			}
			throw new NotSupportedException();
		}

		public static TypeDefinition Resolve(this TypeSpecificationHandle handle, IMetadataResolveContext context)
		{
			var metadata = context.CurrentModule.Metadata;
			var ts = metadata.GetTypeSpecification(handle);
			var unspecialized = ts.DecodeSignature(new Unspecializer(), default);
			switch (unspecialized.Kind) {
				case HandleKind.TypeDefinition:
					return new TypeDefinition(context.CurrentModule, (TypeDefinitionHandle)unspecialized);
				case HandleKind.TypeReference:
					return Resolve((TypeReferenceHandle)unspecialized, context);
				default:
					throw new NotImplementedException();
			}
		}

		public static MethodDefinition Resolve(this MethodSpecificationHandle handle, IMetadataResolveContext context)
		{
			var metadata = context.CurrentModule.Metadata;
			var ms = metadata.GetMethodSpecification(handle);
			return ResolveAsMethod(ms.Method, context);
		}

		class Unspecializer : ISignatureTypeProvider<EntityHandle, Unit>
		{
			public EntityHandle GetArrayType(EntityHandle elementType, ArrayShape shape)
			{
				return elementType;
			}

			public EntityHandle GetByReferenceType(EntityHandle elementType)
			{
				return elementType;
			}

			public EntityHandle GetFunctionPointerType(MethodSignature<EntityHandle> signature)
			{
				return MetadataTokens.EntityHandle(0);
			}

			public EntityHandle GetGenericInstantiation(EntityHandle genericType, ImmutableArray<EntityHandle> typeArguments)
			{
				return genericType;
			}

			public EntityHandle GetGenericMethodParameter(Unit genericContext, int index)
			{
				return MetadataTokens.EntityHandle(0);
			}

			public EntityHandle GetGenericTypeParameter(Unit genericContext, int index)
			{
				return MetadataTokens.EntityHandle(0);
			}

			public EntityHandle GetModifiedType(EntityHandle modifier, EntityHandle unmodifiedType, bool isRequired)
			{
				return unmodifiedType;
			}

			public EntityHandle GetPinnedType(EntityHandle elementType)
			{
				return elementType;
			}

			public EntityHandle GetPointerType(EntityHandle elementType)
			{
				return elementType;
			}

			public EntityHandle GetPrimitiveType(PrimitiveTypeCode typeCode)
			{
				return MetadataTokens.EntityHandle(0);
			}

			public EntityHandle GetSZArrayType(EntityHandle elementType)
			{
				return MetadataTokens.EntityHandle(0);
			}

			public EntityHandle GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
			{
				return handle;
			}

			public EntityHandle GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
			{
				return handle;
			}

			public EntityHandle GetTypeFromSpecification(MetadataReader reader, Unit genericContext, TypeSpecificationHandle handle, byte rawTypeKind)
			{
				return reader.GetTypeSpecification(handle).DecodeSignature(this, genericContext);
			}
		}
	}

	public static class SignatureBlobComparer
	{
		public static bool EqualsMethodSignature(BlobReader a, BlobReader b, MetadataReader contextForA, MetadataReader contextForB)
		{
			return EqualsMethodSignature(ref a, ref b, contextForA, contextForB);
		}

		static bool EqualsMethodSignature(ref BlobReader a, ref BlobReader b, MetadataReader contextForA, MetadataReader contextForB)
		{
			SignatureHeader header;
			// compare signature headers
			if (a.RemainingBytes == 0 || b.RemainingBytes == 0 || (header = a.ReadSignatureHeader()) != b.ReadSignatureHeader())
				return false;
			if (header.IsGeneric) {
				// read & compare generic parameter count
				if (!IsSameCompressedInteger(ref a, ref b, out _))
					return false;
			}
			// read & compare parameter count
			if (!IsSameCompressedInteger(ref a, ref b, out int totalParameterCount))
				return false;
			if (!IsSameCompressedInteger(ref a, ref b, out int typeCode))
				return false;
			if (!TypesAreEqual(ref a, ref b, contextForA, contextForB, typeCode))
				return false;
			int i = 0;
			for (; i < totalParameterCount; i++) {
				if (!IsSameCompressedInteger(ref a, ref b, out typeCode))
					return false;
				//
				if (typeCode == 65)
					break;
				if (!TypesAreEqual(ref a, ref b, contextForA, contextForB, typeCode))
					return false;
			}
			for (; i < totalParameterCount; i++) {
				if (!IsSameCompressedInteger(ref a, ref b, out typeCode))
					return false;
				if (!TypesAreEqual(ref a, ref b, contextForA, contextForB, typeCode))
					return false;
			}
			return true;
		}

		static bool IsSameCompressedInteger(ref BlobReader a, ref BlobReader b, out int value)
		{
			return a.TryReadCompressedInteger(out value) && b.TryReadCompressedInteger(out int otherValue) && value == otherValue;
		}

		static bool IsSameCompressedSignedInteger(ref BlobReader a, ref BlobReader b, out int value)
		{
			return a.TryReadCompressedSignedInteger(out value) && b.TryReadCompressedSignedInteger(out int otherValue) && value == otherValue;
		}

		static bool TypesAreEqual(ref BlobReader a, ref BlobReader b, MetadataReader contextForA, MetadataReader contextForB, int typeCode)
		{
			switch (typeCode) {
				case 0x1: // ELEMENT_TYPE_VOID
				case 0x2: // ELEMENT_TYPE_BOOLEAN 
				case 0x3: // ELEMENT_TYPE_CHAR 
				case 0x4: // ELEMENT_TYPE_I1 
				case 0x5: // ELEMENT_TYPE_U1
				case 0x6: // ELEMENT_TYPE_I2
				case 0x7: // ELEMENT_TYPE_U2
				case 0x8: // ELEMENT_TYPE_I4
				case 0x9: // ELEMENT_TYPE_U4
				case 0xA: // ELEMENT_TYPE_I8
				case 0xB: // ELEMENT_TYPE_U8
				case 0xC: // ELEMENT_TYPE_R4
				case 0xD: // ELEMENT_TYPE_R8
				case 0xE: // ELEMENT_TYPE_STRING
				case 0x16: // ELEMENT_TYPE_TYPEDBYREF
				case 0x18: // ELEMENT_TYPE_I
				case 0x19: // ELEMENT_TYPE_U
				case 0x1C: // ELEMENT_TYPE_OBJECT
					return true;
				case 0xF: // ELEMENT_TYPE_PTR 
				case 0x10: // ELEMENT_TYPE_BYREF 
				case 0x45: // ELEMENT_TYPE_PINNED
				case 0x1D: // ELEMENT_TYPE_SZARRAY
					if (!IsSameCompressedInteger(ref a, ref b, out typeCode))
						return false;
					if (!TypesAreEqual(ref a, ref b, contextForA, contextForB, typeCode))
						return false;
					return true;
				case 0x1B: // ELEMENT_TYPE_FNPTR 
					if (!EqualsMethodSignature(ref a, ref b, contextForA, contextForB))
						return false;
					return true;
				case 0x14: // ELEMENT_TYPE_ARRAY 
					// element type
					if (!IsSameCompressedInteger(ref a, ref b, out typeCode))
						return false;
					if (!TypesAreEqual(ref a, ref b, contextForA, contextForB, typeCode))
						return false;
					// rank
					if (!IsSameCompressedInteger(ref a, ref b, out _))
						return false;
					// sizes
					if (!IsSameCompressedInteger(ref a, ref b, out int numOfSizes))
						return false;
					for (int i = 0; i < numOfSizes; i++) {
						if (!IsSameCompressedInteger(ref a, ref b, out _))
							return false;
					}
					// lower bounds
					if (!IsSameCompressedInteger(ref a, ref b, out int numOfLowerBounds))
						return false;
					for (int i = 0; i < numOfLowerBounds; i++) {
						if (!IsSameCompressedSignedInteger(ref a, ref b, out _))
							return false;
					}
					return true;
				case 0x1F: // ELEMENT_TYPE_CMOD_REQD 
				case 0x20: // ELEMENT_TYPE_CMOD_OPT 
					// modifier
					if (!TypeHandleEquals(ref a, ref b, contextForA, contextForB))
						return false;
					// unmodified type
					if (!IsSameCompressedInteger(ref a, ref b, out typeCode))
						return false;
					if (!TypesAreEqual(ref a, ref b, contextForA, contextForB, typeCode))
						return false;
					return true;
				case 0x15: // ELEMENT_TYPE_GENERICINST 
					// generic type
					if (!IsSameCompressedInteger(ref a, ref b, out typeCode))
						return false;
					if (!TypesAreEqual(ref a, ref b, contextForA, contextForB, typeCode))
						return false;
					if (!IsSameCompressedInteger(ref a, ref b, out int numOfArguments))
						return false;
					for (int i = 0; i < numOfArguments; i++) {
						if (!IsSameCompressedInteger(ref a, ref b, out typeCode))
							return false;
						if (!TypesAreEqual(ref a, ref b, contextForA, contextForB, typeCode))
							return false;
					}
					return true;
				case 0x13: // ELEMENT_TYPE_VAR
				case 0x1E: // ELEMENT_TYPE_MVAR 
					// index
					if (!IsSameCompressedInteger(ref a, ref b, out _))
						return false;
					return true;
				case 0x11: // ELEMENT_TYPE_VALUETYPE
				case 0x12: // ELEMENT_TYPE_CLASS
					if (!TypeHandleEquals(ref a, ref b, contextForA, contextForB))
						return false;
					return true;
				default:
					return false;
			}
		}

		static bool TypeHandleEquals(ref BlobReader a, ref BlobReader b, MetadataReader contextForA, MetadataReader contextForB)
		{
			var typeA = a.ReadTypeHandle();
			var typeB = b.ReadTypeHandle();
			if (typeA.IsNil || typeB.IsNil)
				return false;
			return typeA.GetFullTypeName(contextForA) == typeB.GetFullTypeName(contextForB);
		}
	}
}
