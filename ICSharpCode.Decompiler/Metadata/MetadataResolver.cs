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

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
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

		public SimpleMetadataResolveContext(PEFile mainModule, IAssemblyResolver assemblyResolver)
		{
			this.mainModule = mainModule ?? throw new ArgumentNullException(nameof(mainModule));
			this.assemblyResolver = assemblyResolver ?? throw new ArgumentNullException(nameof(assemblyResolver));
			this.loadedModules = new Dictionary<IAssemblyReference, PEFile>();
		}

		public SimpleMetadataResolveContext(PEFile mainModule, SimpleMetadataResolveContext parentContext)
		{
			this.mainModule = mainModule ?? throw new ArgumentNullException(nameof(mainModule));
			this.assemblyResolver = parentContext.assemblyResolver;
			this.loadedModules = parentContext.loadedModules;
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
		public static (PEFile Module, TypeDefinitionHandle Handle) ResolveType(EntityHandle handle, IMetadataResolveContext context)
		{
			if (handle.IsNil)
				return default;
			switch (handle.Kind) {
				case HandleKind.TypeDefinition:
					return (context.CurrentModule, (TypeDefinitionHandle)handle);
				case HandleKind.TypeReference:
					return Resolve((TypeReferenceHandle)handle, context);
				case HandleKind.TypeSpecification:
					return Resolve((TypeSpecificationHandle)handle, context);
				default:
					throw new NotSupportedException();
			}
		}

		public static (PEFile Module, MethodDefinitionHandle Handle) ResolveAsMethod(EntityHandle handle, IMetadataResolveContext context)
		{
			if (handle.IsNil)
				return default;
			switch (handle.Kind) {
				case HandleKind.MethodDefinition:
					return (context.CurrentModule, (MethodDefinitionHandle)handle);
				case HandleKind.MemberReference:
					var resolved = ((MemberReferenceHandle)handle).Resolve(context);
					if (resolved.Handle.Kind == HandleKind.MethodDefinition)
						return (resolved.Module, (MethodDefinitionHandle)resolved.Handle);
					return default;
				case HandleKind.MethodSpecification:
					resolved = ((MethodSpecificationHandle)handle).Resolve(context);
					if (resolved.Handle.Kind == HandleKind.MethodDefinition)
						return (resolved.Module, (MethodDefinitionHandle)resolved.Handle);
					return default;
				default:
					throw new NotSupportedException();
			}
		}

		public static (PEFile Module, FieldDefinitionHandle Handle) ResolveAsField(EntityHandle handle, IMetadataResolveContext context)
		{
			if (handle.IsNil)
				return default;
			switch (handle.Kind) {
				case HandleKind.FieldDefinition:
					return (context.CurrentModule, (FieldDefinitionHandle)handle);
				case HandleKind.MemberReference:
					var resolved = ((MemberReferenceHandle)handle).Resolve(context);
					if (resolved.Handle.Kind == HandleKind.FieldDefinition)
						return (resolved.Module, (FieldDefinitionHandle)resolved.Handle);
					return default;
				default:
					throw new NotSupportedException();
			}
		}

		/// <summary>
		/// Implements resolving of TypeReferences to TypeDefinitions as decribed in II.7.3 of ECMA-335 6th edition.
		/// </summary>
		public static (PEFile Module, TypeDefinitionHandle Handle) Resolve(this TypeReferenceHandle handle, IMetadataResolveContext context)
		{
			if (handle.IsNil)
				return default;
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
					if (outerType.Module == null)
						throw new NotSupportedException();
					var td = outerType.Module.Metadata.GetTypeDefinition(outerType.Handle);
					var name = metadata.GetString(tr.Name);
					foreach (var nestedType in td.GetNestedTypes()) {
						var nestedTypeDef = outerType.Module.Metadata.GetTypeDefinition(nestedType);
						if (outerType.Module.Metadata.GetString(nestedTypeDef.Name) == name)
							return (outerType.Module, nestedType);
					}
					break;
				case HandleKind.ModuleReference:
					break;
				case HandleKind.AssemblyReference:
					return ResolveTypeInOtherAssembly((AssemblyReferenceHandle)tr.ResolutionScope, metadata.GetString(tr.Namespace), metadata.GetString(tr.Name));
			}
			throw new NotSupportedException();

			(PEFile Module, TypeDefinitionHandle Handle) ResolveTypeInOtherAssembly(AssemblyReferenceHandle asm, string ns, string typeName)
			{
				var module = context.ResolveAssembly(new AssemblyReference(context.CurrentModule, asm));
				if (module == null)
					return default;
				var @namespace = ResolveNamespace(module.Metadata, ns);
				Debug.Assert(@namespace != null);
				var type = FindTypeInNamespace(module.Metadata, @namespace.Value, typeName);
				Debug.Assert(!type.IsNil);
				return (module, type);
			}
		}

		static NamespaceDefinition? ResolveNamespace(MetadataReader metadata, string @namespace)
		{
			var currentNamespace = metadata.GetNamespaceDefinitionRoot();
			int startIndex = 0;
			int dotIndex;

			do {
				dotIndex = @namespace.IndexOf('.', startIndex);
				string identifier = dotIndex == -1 ? @namespace : @namespace.Substring(0, dotIndex);

				var next = currentNamespace.NamespaceDefinitions.FirstOrDefault(ns => metadata.StringComparer.Equals(ns, identifier));
				if (next.IsNil)
					return null;

				currentNamespace = metadata.GetNamespaceDefinition(next);
				startIndex = dotIndex + 1;
			} while (dotIndex > 0);

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

		static (PEFile Module, EntityHandle Handle) Resolve(this MemberReferenceHandle handle, IMetadataResolveContext context)
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
					if (resolvedTypeReference.Module == null)
						return default;
					targetModule = resolvedTypeReference.Module;
					targetMetadata = targetModule.Metadata;
					declaringType = targetMetadata.GetTypeDefinition(resolvedTypeReference.Handle);
					break;
				case HandleKind.TypeSpecification:
					resolvedTypeReference = Resolve((TypeSpecificationHandle)mr.Parent, context);
					if (resolvedTypeReference.Module == null)
						return default;
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
							return (targetModule, f);
					}
					return default;
				case MemberReferenceKind.Method:
					var candidates = new List<(MethodDefinitionHandle, BlobHandle)>();
					foreach (var m in declaringType.GetMethods()) {
						var md = targetMetadata.GetMethodDefinition(m);
						if (targetMetadata.StringComparer.Equals(md.Name, name))
							candidates.Add((m, md.Signature));
					}
					foreach (var (method, signature) in candidates) {
						if (SignatureBlobComparer.EqualsMethodSignature(targetMetadata.GetBlobReader(signature), metadata.GetBlobReader(mr.Signature), targetMetadata, metadata))
							return (targetModule, method);
					}
					return default;
				default:
					throw new NotSupportedException();
			}
		}

		public static (PEFile Module, TypeDefinitionHandle Handle) Resolve(this TypeSpecificationHandle handle, IMetadataResolveContext context)
		{
			var metadata = context.CurrentModule.Metadata;
			var ts = metadata.GetTypeSpecification(handle);
			var unspecialized = ts.DecodeSignature(new Unspecializer(), default);
			if (unspecialized.IsNil)
				return default;
			switch (unspecialized.Kind) {
				case HandleKind.TypeDefinition:
					return (context.CurrentModule, (TypeDefinitionHandle)unspecialized);
				case HandleKind.TypeReference:
					return Resolve((TypeReferenceHandle)unspecialized, context);
				default:
					throw new NotImplementedException();
			}
		}

		public static (PEFile Module, MethodDefinitionHandle Handle) Resolve(this MethodSpecificationHandle handle, IMetadataResolveContext context)
		{
			var metadata = context.CurrentModule.Metadata;
			var ms = metadata.GetMethodSpecification(handle);
			return ResolveAsMethod(ms.Method, context);
		}
	}

	public class Unspecializer : ISignatureTypeProvider<EntityHandle, Unit>
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
