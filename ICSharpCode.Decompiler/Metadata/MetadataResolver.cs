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
				var next = currentNamespace.NamespaceDefinitions.FirstOrDefault(ns => metadata.GetString(metadata.GetNamespaceDefinition(ns).Name) == identifier);
				if (next.IsNil)
					return null;
				currentNamespace = metadata.GetNamespaceDefinition(next);
			}
			return currentNamespace;
		}

		static TypeDefinitionHandle FindTypeInNamespace(MetadataReader metadata, NamespaceDefinition @namespace, string name)
		{
			foreach (var type in @namespace.TypeDefinitions) {
				var typeName = metadata.GetString(metadata.GetTypeDefinition(type).Name);
				if (name == typeName)
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
						if (SignatureBlobComparer.EqualsMethodSignature(targetMetadata.GetBlobReader(signature), metadata.GetBlobReader(mr.Signature), new SimpleMetadataResolveContext(targetModule, context), context))
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
		public static bool EqualsMethodSignature(BlobReader a, BlobReader b, IMetadataResolveContext contextForA, IMetadataResolveContext contextForB)
		{
			return EqualsMethodSignature(ref a, ref b, contextForA, contextForB);
		}

		static bool EqualsMethodSignature(ref BlobReader a, ref BlobReader b, IMetadataResolveContext contextForA, IMetadataResolveContext contextForB)
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
			return a.TryReadCompressedInteger(out value) == b.TryReadCompressedInteger(out int otherValue) && value == otherValue;
		}

		static bool IsSameCompressedSignedInteger(ref BlobReader a, ref BlobReader b, out int value)
		{
			return a.TryReadCompressedSignedInteger(out value) == b.TryReadCompressedSignedInteger(out int otherValue) && value == otherValue;
		}

		static bool TypesAreEqual(ref BlobReader a, ref BlobReader b, IMetadataResolveContext contextForA, IMetadataResolveContext contextForB, int typeCode)
		{
			switch (typeCode) {
				case 1:
				case 2:
				case 3:
				case 4:
				case 5:
				case 6:
				case 7:
				case 8:
				case 9:
				case 10:
				case 11:
				case 12:
				case 13:
				case 14:
				case 22:
				case 24:
				case 25:
				case 28: // primitive types
					return true;
				case 15: // pointer type
				case 16: // byref type
				case 69: // pinned type
				case 29: // szarray type
					if (!IsSameCompressedInteger(ref a, ref b, out typeCode))
						return false;
					if (!TypesAreEqual(ref a, ref b, contextForA, contextForB, typeCode))
						return false;
					return true;
				case 27:
					if (!EqualsMethodSignature(ref a, ref b, contextForA, contextForB))
						return false;
					return true;
				case 20: // array type
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
					// lowerBounds
					if (!IsSameCompressedInteger(ref a, ref b, out int numOfLowerBounds))
						return false;
					for (int i = 0; i < numOfLowerBounds; i++) {
						if (!IsSameCompressedSignedInteger(ref a, ref b, out _))
							return false;
					}
					return true;
				case 31: // mod-req type
				case 32: // mod-opt type
					// modifier
					if (!TypeHandleEquals(ref a, ref b, contextForA, contextForB))
						return false;
					// unmodified type
					if (!IsSameCompressedInteger(ref a, ref b, out typeCode))
						return false;
					if (!TypesAreEqual(ref a, ref b, contextForA, contextForB, typeCode))
						return false;
					return true;
				case 21: // generic instance type
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
				case 19: // type parameter
				case 30: // method type parameter
					// index
					if (!IsSameCompressedInteger(ref a, ref b, out _))
						return false;
					return true;
				case 17:
				case 18:
					if (!TypeHandleEquals(ref a, ref b, contextForA, contextForB))
						return false;
					return true;
				default:
					return false;
			}
		}

		static bool TypeHandleEquals(ref BlobReader a, ref BlobReader b, IMetadataResolveContext contextForA, IMetadataResolveContext contextForB)
		{
			var typeA = a.ReadTypeHandle();
			var typeB = b.ReadTypeHandle();
			if (typeA.IsNil || typeB.IsNil)
				return false;
			var resolvedA = MetadataResolver.ResolveType(typeA, contextForA);
			var resolvedB = MetadataResolver.ResolveType(typeB, contextForB);
			if (resolvedA.IsNil || resolvedB.IsNil)
				return false;
			if (resolvedA.Handle != resolvedB.Handle)
				return false;
			if (resolvedA.Module.FullName != resolvedB.Module.FullName)
				return false;
			return true;
		}
	}
}
