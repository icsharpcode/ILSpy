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
		readonly Dictionary<IAssemblyReference, PEFile> loadedModules = new Dictionary<IAssemblyReference, PEFile>();

		public SimpleMetadataResolveContext(PEFile mainModule)
		{
			this.mainModule = mainModule;
			this.assemblyResolver = mainModule.AssemblyResolver;
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
					var memberRefHandle = (MemberReferenceHandle)handle;
					var metadata = context.CurrentModule.GetMetadataReader();
					var memberRef = metadata.GetMemberReference(memberRefHandle);
					if (memberRef.GetKind() != MemberReferenceKind.Method)
						return default;
					break;
			}
			throw new NotImplementedException();
		}

		public static FieldDefinition ResolveAsField(EntityHandle handle, IMetadataResolveContext context)
		{
			switch (handle.Kind) {
				case HandleKind.FieldDefinition:
					return new FieldDefinition(context.CurrentModule, (FieldDefinitionHandle)handle);
				case HandleKind.MemberReference:
					var memberRefHandle = (MemberReferenceHandle)handle;
					var metadata = context.CurrentModule.GetMetadataReader();
					var memberRef = metadata.GetMemberReference(memberRefHandle);
					if (memberRef.GetKind() != MemberReferenceKind.Field)
						throw new ArgumentException("MemberReferenceKind must be Field!", nameof(handle));
					
					break;
			}
			throw new NotImplementedException();
		}

		/// <summary>
		/// Implements resolving of TypeReferences to TypeDefinitions as decribed in II.7.3 of ECMA-335 6th edition.
		/// </summary>
		public static TypeDefinition Resolve(this TypeReferenceHandle handle, IMetadataResolveContext context)
		{
			var metadata = context.CurrentModule.GetMetadataReader();
			var tr = metadata.GetTypeReference(handle);
			if (tr.ResolutionScope.IsNil) {
				foreach (var h in metadata.ExportedTypes) {
					var exportedType = metadata.GetExportedType(h);
					if (exportedType.Name == tr.Name && exportedType.Namespace == tr.Namespace) {
						// TODO
					}
				}
			}
			switch (tr.ResolutionScope.Kind) {
				case HandleKind.TypeReference:
					//return Resolve((TypeReferenceHandle)tr.ResolutionScope, context).GetNestedType(new );
					break;
				case HandleKind.ModuleReference:
					break;
				case HandleKind.AssemblyReference:
					var module = context.ResolveAssembly(new AssemblyReference(context.CurrentModule, (AssemblyReferenceHandle)tr.ResolutionScope));
					var moduleMetadata = module.GetMetadataReader();
					var @namespace = ResolveNamespace(moduleMetadata, metadata.GetString(tr.Namespace).Split('.'));
					if (@namespace == null)
						throw new NotSupportedException();
					var type = FindTypeInNamespace(moduleMetadata, @namespace.Value, metadata.GetString(tr.Name));
					if (type.IsNil)
						throw new NotSupportedException();
					return new TypeDefinition(module, type);
			}
			throw new NotSupportedException();
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
			return default(TypeDefinitionHandle);
		}

		public static IMetadataEntity Resolve(MemberReferenceHandle handle, IMetadataResolveContext context)
		{
			var metadata = context.CurrentModule.GetMetadataReader();
			var mr = metadata.GetMemberReference(handle);
			TypeDefinition declaringType;
			switch (mr.Parent.Kind) {
				case HandleKind.TypeDefinition:
					declaringType = new TypeDefinition(context.CurrentModule, (TypeDefinitionHandle)mr.Parent);
					break;
				case HandleKind.TypeReference:
					declaringType = Resolve((TypeReferenceHandle)mr.Parent, context);
					break;
				case HandleKind.TypeSpecification:
				case HandleKind.MethodDefinition:
				case HandleKind.ModuleReference:
					throw new NotImplementedException();
				default:
					throw new NotSupportedException();
			}
			/*var name = metadata.GetString(mr.Name);
			switch (mr.GetKind()) {
				case MemberReferenceKind.Field:
					return declaringType.Fields.FirstOrDefault(fd => fd.Name == name);
				case MemberReferenceKind.Method:
					var signature = mr.DecodeMethodSignature(new TypeSystem.Implementation.TypeReferenceSignatureDecoder(), default(Unit));
					return declaringType.Methods.SingleOrDefault(md => MatchMethodDefinition(name, signature, md));
			}*/
			throw new NotSupportedException();
		}

		static bool MatchMethodDefinition(string name, MethodSignature<TypeSystem.ITypeReference> signature, MethodDefinition md)
		{
			throw new NotImplementedException();
			//if (name != md.Name || md.GenericParameters.Count != signature.GenericParameterCount || signature.RequiredParameterCount != md.Parameters.Count)
			//	return false;
			// TODO overload resolution... OMG
			//return true;
		}

		public static TypeDefinition Resolve(TypeSpecificationHandle handle, IMetadataResolveContext context)
		{
			var metadata = context.CurrentModule.GetMetadataReader();
			var ts = metadata.GetTypeSpecification(handle);
			var unspecialized = ts.DecodeSignature(new Unspecializer(), default(Unit));
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
				throw new NotImplementedException();
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
				throw new NotImplementedException();
			}

			public EntityHandle GetSZArrayType(EntityHandle elementType)
			{
				throw new NotImplementedException();
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
}
