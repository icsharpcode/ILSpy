#nullable enable

using System;
using System.Collections.Immutable;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.Metadata
{
	public enum TargetRuntime
	{
		Unknown,
		Net_1_0,
		Net_1_1,
		Net_2_0,
		Net_4_0
	}

	public enum ResourceType
	{
		Linked,
		Embedded,
		AssemblyLinked,
	}

	public abstract class Resource
	{
		public virtual ResourceType ResourceType => ResourceType.Embedded;
		public virtual ManifestResourceAttributes Attributes => ManifestResourceAttributes.Public;
		public abstract string Name { get; }
		public abstract Stream? TryOpenStream();
	}

	public class ByteArrayResource : Resource
	{
		public override string Name { get; }
		byte[] data;

		public ByteArrayResource(string name, byte[] data)
		{
			this.Name = name ?? throw new ArgumentNullException(nameof(name));
			this.data = data ?? throw new ArgumentNullException(nameof(data));
		}

		public override Stream TryOpenStream()
		{
			return new MemoryStream(data);
		}
	}

	sealed class MetadataResource : Resource
	{
		public PEFile Module { get; }
		public ManifestResourceHandle Handle { get; }
		public bool IsNil => Handle.IsNil;

		public MetadataResource(PEFile module, ManifestResourceHandle handle)
		{
			this.Module = module ?? throw new ArgumentNullException(nameof(module));
			this.Handle = handle;
		}

		ManifestResource This() => Module.Metadata.GetManifestResource(Handle);

		public bool Equals(MetadataResource other)
		{
			return Module == other.Module && Handle == other.Handle;
		}

		public override bool Equals(object obj)
		{
			if (obj is MetadataResource res)
				return Equals(res);
			return false;
		}

		public override int GetHashCode()
		{
			return unchecked(982451629 * Module.GetHashCode() + 982451653 * MetadataTokens.GetToken(Handle));
		}

		public override string Name => Module.Metadata.GetString(This().Name);

		public override ManifestResourceAttributes Attributes => This().Attributes;
		public bool HasFlag(ManifestResourceAttributes flag) => (Attributes & flag) == flag;
		public override ResourceType ResourceType => GetResourceType();

		ResourceType GetResourceType()
		{
			if (This().Implementation.IsNil)
				return ResourceType.Embedded;
			if (This().Implementation.Kind == HandleKind.AssemblyReference)
				return ResourceType.AssemblyLinked;
			return ResourceType.Linked;
		}

		public override unsafe Stream? TryOpenStream()
		{
			if (ResourceType != ResourceType.Embedded)
				return null;
			var headers = Module.Reader.PEHeaders;
			if (headers.CorHeader == null)
				return null;
			var resources = headers.CorHeader.ResourcesDirectory;
			if (resources.RelativeVirtualAddress == 0)
				return null;
			var sectionData = Module.Reader.GetSectionData(resources.RelativeVirtualAddress);
			if (sectionData.Length == 0)
				throw new BadImageFormatException("RVA could not be found in any section!");
			var reader = sectionData.GetReader();
			reader.Offset += (int)This().Offset;
			int length = reader.ReadInt32();
			if (length < 0 || length > reader.RemainingBytes)
				throw new BadImageFormatException("Resource stream length invalid");
			return new ResourceMemoryStream(Module.Reader, reader.CurrentPointer, length);
		}
	}

	sealed unsafe class ResourceMemoryStream : UnmanagedMemoryStream
	{
		readonly PEReader peReader;

		public ResourceMemoryStream(PEReader peReader, byte* data, long length)
			: base(data, length, length, FileAccess.Read)
		{
			// Keep the PEReader alive while the stream in in use.
			this.peReader = peReader;
		}
	}

	public sealed class FullTypeNameSignatureDecoder : ISignatureTypeProvider<FullTypeName, Unit>, ICustomAttributeTypeProvider<FullTypeName>
	{
		readonly MetadataReader metadata;

		public FullTypeNameSignatureDecoder(MetadataReader metadata)
		{
			this.metadata = metadata;
		}

		public FullTypeName GetArrayType(FullTypeName elementType, ArrayShape shape)
		{
			return elementType;
		}

		public FullTypeName GetByReferenceType(FullTypeName elementType)
		{
			return elementType;
		}

		public FullTypeName GetFunctionPointerType(MethodSignature<FullTypeName> signature)
		{
			return default;
		}

		public FullTypeName GetGenericInstantiation(FullTypeName genericType, ImmutableArray<FullTypeName> typeArguments)
		{
			return genericType;
		}

		public FullTypeName GetGenericMethodParameter(Unit genericContext, int index)
		{
			return default;
		}

		public FullTypeName GetGenericTypeParameter(Unit genericContext, int index)
		{
			return default;
		}

		public FullTypeName GetModifiedType(FullTypeName modifier, FullTypeName unmodifiedType, bool isRequired)
		{
			return unmodifiedType;
		}

		public FullTypeName GetPinnedType(FullTypeName elementType)
		{
			return elementType;
		}

		public FullTypeName GetPointerType(FullTypeName elementType)
		{
			return elementType;
		}

		public FullTypeName GetPrimitiveType(PrimitiveTypeCode typeCode)
		{
			var ktr = KnownTypeReference.Get(typeCode.ToKnownTypeCode());
			return new TopLevelTypeName(ktr.Namespace, ktr.Name, ktr.TypeParameterCount);
		}

		public FullTypeName GetSystemType()
		{
			return new TopLevelTypeName("System", "Type");
		}

		public FullTypeName GetSZArrayType(FullTypeName elementType)
		{
			return elementType;
		}

		public FullTypeName GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
		{
			return handle.GetFullTypeName(reader);
		}

		public FullTypeName GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
		{
			return handle.GetFullTypeName(reader);
		}

		public FullTypeName GetTypeFromSerializedName(string name)
		{
			return new FullTypeName(name);
		}

		public FullTypeName GetTypeFromSpecification(MetadataReader reader, Unit genericContext, TypeSpecificationHandle handle, byte rawTypeKind)
		{
			return reader.GetTypeSpecification(handle).DecodeSignature(new FullTypeNameSignatureDecoder(metadata), default);
		}

		public PrimitiveTypeCode GetUnderlyingEnumType(FullTypeName type)
		{
			throw new NotImplementedException();
		}

		public bool IsSystemType(FullTypeName type)
		{
			return type.IsKnownType(KnownTypeCode.Type);
		}
	}

	public class GenericContext
	{
		readonly MetadataReader? metadata;
		readonly TypeDefinitionHandle declaringType;
		readonly MethodDefinitionHandle method;

		public static readonly GenericContext Empty = new GenericContext();

		private GenericContext() { }

		public GenericContext(MethodDefinitionHandle method, PEFile module)
		{
			this.metadata = module.Metadata;
			this.method = method;
			this.declaringType = module.Metadata.GetMethodDefinition(method).GetDeclaringType();
		}

		public GenericContext(MethodDefinitionHandle method, MetadataReader metadata)
		{
			this.metadata = metadata;
			this.method = method;
			this.declaringType = metadata.GetMethodDefinition(method).GetDeclaringType();
		}

		public GenericContext(TypeDefinitionHandle declaringType, PEFile module)
		{
			this.metadata = module.Metadata;
			this.declaringType = declaringType;
		}

		public GenericContext(TypeDefinitionHandle declaringType, MetadataReader metadata)
		{
			this.metadata = metadata;
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
