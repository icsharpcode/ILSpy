using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text;
using ICSharpCode.Decompiler.Documentation;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.Metadata
{
	using SRMMemberRef = System.Reflection.Metadata.MemberReference;
	using SRMMethod = System.Reflection.Metadata.MethodDefinition;
	using SRMMethodSpec = System.Reflection.Metadata.MethodSpecification;
	using SRMProperty = System.Reflection.Metadata.PropertyDefinition;
	using SRMEvent = System.Reflection.Metadata.EventDefinition;
	using SRMField = System.Reflection.Metadata.FieldDefinition;
	using SRMTypeDef = System.Reflection.Metadata.TypeDefinition;
	using SRMTypeRef = System.Reflection.Metadata.TypeReference;
	using SRMTypeSpec = System.Reflection.Metadata.TypeSpecification;
	using SRMAssemblyReference = System.Reflection.Metadata.AssemblyReference;

	public sealed class AssemblyResolutionException : FileNotFoundException
	{
		public IAssemblyReference Reference { get; }

		public AssemblyResolutionException(IAssemblyReference reference)
			: this(reference, null)
		{
		}

		public AssemblyResolutionException(IAssemblyReference reference, Exception innerException)
			: base($"Failed to resolve assembly: '{reference}'", innerException)
		{
			this.Reference = reference;
		}
	}

	public interface IAssemblyResolver
	{
		PEFile Resolve(IAssemblyReference reference);
	}

	public interface IAssemblyReference
	{
		string Name { get; }
		string FullName { get; }
		Version Version { get; }
		string Culture { get; }
		byte[] PublicKeyToken { get; }

		bool IsWindowsRuntime { get; }
		bool IsRetargetable { get; }
	}

	public interface IAssemblyDocumentationResolver
	{
		XmlDocumentationProvider GetProvider();
	}

	public interface IMetadataEntity
	{
		PEFile Module { get; }
		EntityHandle Handle { get; }
	}

	public struct Variable { }

	public interface IDebugInfoProvider
	{
		IList<SequencePoint> GetSequencePoints(MethodDefinitionHandle method);
		IList<Variable> GetVariables(MethodDefinitionHandle method);
		bool TryGetName(MethodDefinitionHandle method, int index, out string name);
	}

	public enum TargetRuntime
	{
		Net_1_0,
		Net_1_1,
		Net_2_0,
		Net_4_0
	}

	public class PEFile
	{
		public string FileName { get; }
		public PEReader Reader { get; }
		public IAssemblyResolver AssemblyResolver { get; }
		public IAssemblyDocumentationResolver DocumentationResolver { get; set; }
		public IDebugInfoProvider DebugInfo { get; set; }

		public PEFile(string fileName, PEReader reader, IAssemblyResolver resolver)
		{
			this.FileName = fileName;
			this.Reader = reader;
			this.AssemblyResolver = resolver;
		}

		public bool IsAssembly => GetMetadataReader().IsAssembly;
		public string Name => GetName();
		public string FullName => IsAssembly ? GetMetadataReader().GetFullAssemblyName() : Name;

		public TargetRuntime GetRuntime()
		{
			string version = GetMetadataReader().MetadataVersion;
			switch (version[1]) {
				case '1':
					if (version[3] == 1)
						return TargetRuntime.Net_1_0;
					else
						return TargetRuntime.Net_1_1;
				case '2':
					return TargetRuntime.Net_2_0;
				case '4':
					return TargetRuntime.Net_4_0;
				default:
					throw new NotSupportedException($"metadata version {version} is not supported!");
			}

		}

		public MetadataReader GetMetadataReader() => Reader.GetMetadataReader();

		string GetName()
		{
			var metadata = GetMetadataReader();
			if (metadata.IsAssembly)
				return metadata.GetString(metadata.GetAssemblyDefinition().Name);
			return metadata.GetString(metadata.GetModuleDefinition().Name);
		}

		public ImmutableArray<AssemblyReference> AssemblyReferences => GetMetadataReader().AssemblyReferences.Select(r => new AssemblyReference(this, r)).ToImmutableArray();
		public ImmutableArray<ModuleReferenceHandle> ModuleReferences => GetModuleReferences().ToImmutableArray();
		public ImmutableArray<TypeDefinition> TypeDefinitions => Reader.GetMetadataReader().GetTopLevelTypeDefinitions().Select(t => new TypeDefinition(this, t)).ToImmutableArray();
		public ImmutableArray<TypeReference> TypeReferences => Reader.GetMetadataReader().TypeReferences.Select(t => new TypeReference(this, t)).ToImmutableArray();
		public ImmutableArray<Resource> Resources => GetResources().ToImmutableArray();

		IEnumerable<ModuleReferenceHandle> GetModuleReferences()
		{
			var rowCount = GetMetadataReader().GetTableRowCount(TableIndex.ModuleRef);
			for (int row = 1; row <= rowCount; row++) {
				yield return MetadataTokens.ModuleReferenceHandle(row);
			}
		}

		IEnumerable<Resource> GetResources()
		{
			var metadata = GetMetadataReader();
			foreach (var h in metadata.ManifestResources) {
				yield return new Resource(this, h);
			}
		}
	}

	public enum ResourceType
	{
		Linked,
		Embedded,
		AssemblyLinked,
	}

	public struct Resource : IEquatable<Resource>
	{
		public PEFile Module { get; }
		public ManifestResourceHandle Handle { get; }
		public bool IsNil => Handle.IsNil;

		public Resource(PEFile module, ManifestResourceHandle handle) : this()
		{
			this.Module = module ?? throw new ArgumentNullException(nameof(module));
			this.Handle = handle;
		}

		ManifestResource This() => Module.GetMetadataReader().GetManifestResource(Handle);

		public bool Equals(Resource other)
		{
			return Module == other.Module && Handle == other.Handle;
		}

		public override bool Equals(object obj)
		{
			if (obj is Resource res)
				return Equals(res);
			return false;
		}

		public override int GetHashCode()
		{
			return unchecked(982451629 * Module.GetHashCode() + 982451653 * MetadataTokens.GetToken(Handle));
		}

		public static bool operator ==(Resource lhs, Resource rhs) => lhs.Equals(rhs);
		public static bool operator !=(Resource lhs, Resource rhs) => !lhs.Equals(rhs);

		public string Name => Module.GetMetadataReader().GetString(This().Name);

		public ManifestResourceAttributes Attributes => This().Attributes;
		public bool HasFlag(ManifestResourceAttributes flag) => (Attributes & flag) == flag;
		public ResourceType ResourceType => GetResourceType();

		ResourceType GetResourceType()
		{
			if (This().Implementation.IsNil)
				return ResourceType.Embedded;
			if (This().Implementation.Kind == HandleKind.AssemblyReference)
				return ResourceType.AssemblyLinked;
			return ResourceType.Linked;
		}

		public unsafe Stream TryOpenStream()
		{
			if (ResourceType != ResourceType.Embedded)
				return null;
			var headers = Module.Reader.PEHeaders;
			var resources = headers.CorHeader.ResourcesDirectory;
			int index = headers.GetContainingSectionIndex(resources.RelativeVirtualAddress);
			if (index < 0) throw new NotSupportedException();
			var sectionHeader = headers.SectionHeaders[index];
			var sectionData = Module.Reader.GetEntireImage();
			int totalOffset = resources.RelativeVirtualAddress + sectionHeader.PointerToRawData - sectionHeader.VirtualAddress;
			var reader = sectionData.GetReader();
			reader.Offset += totalOffset;
			reader.Offset += (int)This().Offset;
			int length = reader.ReadInt32();
			return new MemoryStream(reader.ReadBytes(length));
		}
	}

	public class AssemblyNameReference : IAssemblyReference
	{
		string fullName;

		public string Name { get; private set; }

		public string FullName {
			get {
				if (fullName != null)
					return fullName;

				const string sep = ", ";

				var builder = new StringBuilder();
				builder.Append(Name);
				builder.Append(sep);
				builder.Append("Version=");
				builder.Append(Version.ToString(fieldCount: 4));
				builder.Append(sep);
				builder.Append("Culture=");
				builder.Append(string.IsNullOrEmpty(Culture) ? "neutral" : Culture);
				builder.Append(sep);
				builder.Append("PublicKeyToken=");

				var pk_token = PublicKeyToken;
				if (pk_token != null && pk_token.Length > 0) {
					for (int i = 0; i < pk_token.Length; i++) {
						builder.Append(pk_token[i].ToString("x2"));
					}
				} else
					builder.Append("null");

				if (IsRetargetable) {
					builder.Append(sep);
					builder.Append("Retargetable=Yes");
				}

				return fullName = builder.ToString();
			}
		}

		public Version Version { get; private set; }

		public string Culture { get; private set; }

		public byte[] PublicKeyToken { get; private set; }

		public bool IsWindowsRuntime { get; private set; }

		public bool IsRetargetable { get; private set; }

		public static AssemblyNameReference Parse(string fullName)
		{
			if (fullName == null)
				throw new ArgumentNullException("fullName");
			if (fullName.Length == 0)
				throw new ArgumentException("Name can not be empty");

			var name = new AssemblyNameReference();
			var tokens = fullName.Split(',');
			for (int i = 0; i < tokens.Length; i++) {
				var token = tokens[i].Trim();

				if (i == 0) {
					name.Name = token;
					continue;
				}

				var parts = token.Split('=');
				if (parts.Length != 2)
					throw new ArgumentException("Malformed name");

				switch (parts[0].ToLowerInvariant()) {
					case "version":
						name.Version = new Version(parts[1]);
						break;
					case "culture":
						name.Culture = parts[1] == "neutral" ? "" : parts[1];
						break;
					case "publickeytoken":
						var pk_token = parts[1];
						if (pk_token == "null")
							break;

						name.PublicKeyToken = new byte[pk_token.Length / 2];
						for (int j = 0; j < name.PublicKeyToken.Length; j++)
							name.PublicKeyToken[j] = Byte.Parse(pk_token.Substring(j * 2, 2), System.Globalization.NumberStyles.HexNumber);

						break;
				}
			}

			return name;
		}
	}

	public struct AssemblyReference : IAssemblyReference, IEquatable<AssemblyReference>
	{
		static readonly SHA1 sha1 = SHA1.Create();

		public PEFile Module { get; }
		public AssemblyReferenceHandle Handle { get; }
		public bool IsNil => Handle.IsNil;

		SRMAssemblyReference This() => Module.GetMetadataReader().GetAssemblyReference(Handle);

		public bool IsWindowsRuntime => (This().Flags & AssemblyFlags.WindowsRuntime) != 0;
		public bool IsRetargetable => (This().Flags & AssemblyFlags.Retargetable) != 0;

		public string Name => Module.GetMetadataReader().GetString(This().Name);
		public string FullName => This().GetFullAssemblyName(Module.GetMetadataReader());
		public Version Version => This().Version;
		public string Culture => Module.GetMetadataReader().GetString(This().Culture);
		byte[] IAssemblyReference.PublicKeyToken => GetPublicKeyToken();

		public byte[] GetPublicKeyToken()
		{
			var inst = This();
			if (inst.PublicKeyOrToken.IsNil)
				return Empty<byte>.Array;
			var bytes = Module.GetMetadataReader().GetBlobBytes(inst.PublicKeyOrToken);
			if ((inst.Flags & AssemblyFlags.PublicKey) != 0) {
				return sha1.ComputeHash(bytes).Skip(12).ToArray();
			}
			return bytes;
		}

		public bool Equals(AssemblyReference other)
		{
			return Module == other.Module && Handle == other.Handle;
		}

		public override bool Equals(object obj)
		{
			if (obj is AssemblyReference reference)
				return Equals(reference);
			return false;
		}

		public override int GetHashCode()
		{
			return unchecked(982451629 * Module.GetHashCode() + 982451653 * MetadataTokens.GetToken(Handle));
		}

		public static bool operator ==(AssemblyReference lhs, AssemblyReference rhs) => lhs.Equals(rhs);
		public static bool operator !=(AssemblyReference lhs, AssemblyReference rhs) => !lhs.Equals(rhs);

		public AssemblyReference(PEFile module, AssemblyReferenceHandle handle)
		{
			Module = module;
			Handle = handle;
		}
	}

	public struct Entity : IEquatable<Entity>, IMetadataEntity
	{
		public PEFile Module { get; }
		public EntityHandle Handle { get; }
		public bool IsNil => Handle.IsNil;

		public Entity(PEFile module, EntityHandle handle) : this()
		{
			this.Module = module ?? throw new ArgumentNullException(nameof(module));
			this.Handle = handle;
		}

		public bool IsType() => Handle.Kind == HandleKind.TypeDefinition || Handle.Kind == HandleKind.TypeReference || Handle.Kind == HandleKind.TypeSpecification;

		public TypeDefinition ResolveAsType()
		{
			return MetadataResolver.ResolveType(Handle, new SimpleMetadataResolveContext(Module));
		}

		public FieldDefinition ResolveAsField()
		{
			return MetadataResolver.ResolveAsField(Handle, new SimpleMetadataResolveContext(Module));
		}

		public MethodDefinition ResolveAsMethod()
		{
			return MetadataResolver.ResolveAsMethod(Handle, new SimpleMetadataResolveContext(Module));
		}

		public bool Equals(Entity other)
		{
			return Module == other.Module && Handle == other.Handle;
		}

		public override bool Equals(object obj)
		{
			if (obj is Entity entity)
				return Equals(entity);
			return false;
		}

		public override int GetHashCode()
		{
			return unchecked(982451629 * Module.GetHashCode() + 982451653 * MetadataTokens.GetToken(Handle));
		}

		public static bool operator ==(Entity lhs, Entity rhs) => lhs.Equals(rhs);
		public static bool operator !=(Entity lhs, Entity rhs) => !lhs.Equals(rhs);

		public static implicit operator Entity(MethodDefinition method)
		{
			return new Entity(method.Module, method.Handle);
		}

		public static implicit operator Entity(PropertyDefinition property)
		{
			return new Entity(property.Module, property.Handle);
		}

		public static implicit operator Entity(EventDefinition @event)
		{
			return new Entity(@event.Module, @event.Handle);
		}

		public static implicit operator Entity(FieldDefinition field)
		{
			return new Entity(field.Module, field.Handle);
		}

		public static implicit operator Entity(TypeDefinition type)
		{
			return new Entity(type.Module, type.Handle);
		}

		public static implicit operator TypeDefinition(Entity entity)
		{
			return new TypeDefinition(entity.Module, (TypeDefinitionHandle)entity.Handle);
		}

		public static implicit operator MethodDefinition(Entity entity)
		{
			return new MethodDefinition(entity.Module, (MethodDefinitionHandle)entity.Handle);
		}

		public static implicit operator PropertyDefinition(Entity entity)
		{
			return new PropertyDefinition(entity.Module, (PropertyDefinitionHandle)entity.Handle);
		}

		public static implicit operator EventDefinition(Entity entity)
		{
			return new EventDefinition(entity.Module, (EventDefinitionHandle)entity.Handle);
		}

		public static implicit operator FieldDefinition(Entity entity)
		{
			return new FieldDefinition(entity.Module, (FieldDefinitionHandle)entity.Handle);
		}
	}

	public struct MethodDefinition : IEquatable<MethodDefinition>, IMetadataEntity
	{
		public PEFile Module { get; }
		public MethodDefinitionHandle Handle { get; }
		public bool IsNil => Handle.IsNil;
		EntityHandle IMetadataEntity.Handle => Handle;

		public MethodDefinition(PEFile module, MethodDefinitionHandle handle) : this()
		{
			this.Module = module ?? throw new ArgumentNullException(nameof(module));
			this.Handle = handle;
		}

		public SRMMethod This() => Module.GetMetadataReader().GetMethodDefinition(Handle);

		public bool Equals(MethodDefinition other)
		{
			return Module == other.Module && Handle == other.Handle;
		}

		public override bool Equals(object obj)
		{
			if (obj is MethodDefinition md)
				return Equals(md);
			return false;
		}

		public override int GetHashCode()
		{
			return unchecked(982451629 * Module.GetHashCode() + 982451653 * MetadataTokens.GetToken(Handle));
		}

		public static bool operator ==(MethodDefinition lhs, MethodDefinition rhs) => lhs.Equals(rhs);
		public static bool operator !=(MethodDefinition lhs, MethodDefinition rhs) => !lhs.Equals(rhs);

		public IList<SequencePoint> GetSequencePoints()
		{
			return Module.DebugInfo?.GetSequencePoints(Handle);
		}

		public IList<Variable> GetVariables()
		{
			return Module.DebugInfo?.GetVariables(Handle);
		}
	}

	public struct PropertyDefinition : IEquatable<PropertyDefinition>, IMetadataEntity
	{
		public PEFile Module { get; }
		public PropertyDefinitionHandle Handle { get; }
		public bool IsNil => Handle.IsNil;
		EntityHandle IMetadataEntity.Handle => Handle;

		public PropertyDefinition(PEFile module, PropertyDefinitionHandle handle) : this()
		{
			this.Module = module ?? throw new ArgumentNullException(nameof(module));
			this.Handle = handle;
		}

		public SRMProperty This() => Module.GetMetadataReader().GetPropertyDefinition(Handle);

		public bool Equals(PropertyDefinition other)
		{
			return Module == other.Module && Handle == other.Handle;
		}

		public override bool Equals(object obj)
		{
			if (obj is PropertyDefinition pd)
				return Equals(pd);
			return false;
		}

		public override int GetHashCode()
		{
			return unchecked(982451629 * Module.GetHashCode() + 982451653 * MetadataTokens.GetToken(Handle));
		}

		public static bool operator ==(PropertyDefinition lhs, PropertyDefinition rhs) => lhs.Equals(rhs);
		public static bool operator !=(PropertyDefinition lhs, PropertyDefinition rhs) => !lhs.Equals(rhs);
	}

	public struct FieldDefinition : IEquatable<FieldDefinition>, IMetadataEntity
	{
		public PEFile Module { get; }
		public FieldDefinitionHandle Handle { get; }
		public bool IsNil => Handle.IsNil;
		EntityHandle IMetadataEntity.Handle => Handle;

		public FieldDefinition(PEFile module, FieldDefinitionHandle handle) : this()
		{
			this.Module = module ?? throw new ArgumentNullException(nameof(module));
			this.Handle = handle;
		}

		public SRMField This() => Module.GetMetadataReader().GetFieldDefinition(Handle);

		public bool Equals(FieldDefinition other)
		{
			return Module == other.Module && Handle == other.Handle;
		}

		public override bool Equals(object obj)
		{
			if (obj is FieldDefinition fd)
				return Equals(fd);
			return false;
		}

		public override int GetHashCode()
		{
			return unchecked(982451629 * Module.GetHashCode() + 982451653 * MetadataTokens.GetToken(Handle));
		}

		public static bool operator ==(FieldDefinition lhs, FieldDefinition rhs) => lhs.Equals(rhs);
		public static bool operator !=(FieldDefinition lhs, FieldDefinition rhs) => !lhs.Equals(rhs);

		public object DecodeConstant()
		{
			var metadata = Module.GetMetadataReader();
			var constant = metadata.GetConstant(This().GetDefaultValue());
			var blob = metadata.GetBlobReader(constant.Value);
			switch (constant.TypeCode) {
				case ConstantTypeCode.Boolean:
					return blob.ReadBoolean();
				case ConstantTypeCode.Char:
					return blob.ReadChar();
				case ConstantTypeCode.SByte:
					return blob.ReadSByte();
				case ConstantTypeCode.Byte:
					return blob.ReadByte();
				case ConstantTypeCode.Int16:
					return blob.ReadInt16();
				case ConstantTypeCode.UInt16:
					return blob.ReadUInt16();
				case ConstantTypeCode.Int32:
					return blob.ReadInt32();
				case ConstantTypeCode.UInt32:
					return blob.ReadUInt32();
				case ConstantTypeCode.Int64:
					return blob.ReadInt64();
				case ConstantTypeCode.UInt64:
					return blob.ReadUInt64();
				case ConstantTypeCode.Single:
					return blob.ReadSingle();
				case ConstantTypeCode.Double:
					return blob.ReadDouble();
				case ConstantTypeCode.String:
					return blob.ReadSerializedString();
				case ConstantTypeCode.NullReference:
					return null;
				default:
					throw new NotSupportedException();
			}
		}
	}

	public struct EventDefinition : IEquatable<EventDefinition>, IMetadataEntity
	{
		public PEFile Module { get; }
		public EventDefinitionHandle Handle { get; }
		public bool IsNil => Handle.IsNil;
		EntityHandle IMetadataEntity.Handle => Handle;

		public EventDefinition(PEFile module, EventDefinitionHandle handle)
		{
			this.Module = module ?? throw new ArgumentNullException(nameof(module));
			this.Handle = handle;
		}

		public SRMEvent This() => Module.GetMetadataReader().GetEventDefinition(Handle);

		public bool Equals(EventDefinition other)
		{
			return Module == other.Module && Handle == other.Handle;
		}

		public override bool Equals(object obj)
		{
			if (obj is EventDefinition ed)
				return Equals(ed);
			return false;
		}

		public override int GetHashCode()
		{
			return unchecked(982451629 * Module.GetHashCode() + 982451653 * MetadataTokens.GetToken(Handle));
		}

		public static bool operator ==(EventDefinition lhs, EventDefinition rhs) => lhs.Equals(rhs);
		public static bool operator !=(EventDefinition lhs, EventDefinition rhs) => !lhs.Equals(rhs);
	}

	public struct TypeDefinition : IEquatable<TypeDefinition>, IMetadataEntity
	{
		public PEFile Module { get; }
		public TypeDefinitionHandle Handle { get; }
		public bool IsNil => Handle.IsNil;

		EntityHandle IMetadataEntity.Handle => Handle;

		public TypeDefinition(PEFile module, TypeDefinitionHandle handle)
		{
			this.Module = module ?? throw new ArgumentNullException(nameof(module));
			this.Handle = handle;
		}

		public SRMTypeDef This() => Module.GetMetadataReader().GetTypeDefinition(Handle);

		public bool Equals(TypeDefinition other)
		{
			return Module == other.Module && Handle == other.Handle;
		}

		public override bool Equals(object obj)
		{
			if (obj is TypeDefinition td)
				return Equals(td);
			return false;
		}

		public override int GetHashCode()
		{
			return unchecked(982451629 * Module.GetHashCode() + 982451653 * MetadataTokens.GetToken(Handle));
		}

		public static bool operator ==(TypeDefinition lhs, TypeDefinition rhs) => lhs.Equals(rhs);
		public static bool operator !=(TypeDefinition lhs, TypeDefinition rhs) => !lhs.Equals(rhs);
	}

	public struct TypeReference
	{
		public PEFile Module { get; }
		public TypeReferenceHandle Handle { get; }
		public bool IsNil => Handle.IsNil;

		public TypeReference(PEFile module, TypeReferenceHandle handle)
		{
			this.Module = module ?? throw new ArgumentNullException(nameof(module));
			this.Handle = handle;
		}

		SRMTypeRef This() => Module.GetMetadataReader().GetTypeReference(Handle);

		public string Name {
			get {
				var reader = Module.GetMetadataReader();
				return reader.GetString(This().Name);
			}
		}

		public FullTypeName FullName {
			get {
				return Handle.GetFullTypeName(Module.GetMetadataReader());
			}
		}

		public string Namespace => throw new NotImplementedException();

		public EntityHandle ResolutionScope => This().ResolutionScope;

		public TypeDefinition GetDefinition()
		{
			return MetadataResolver.Resolve(Handle, new SimpleMetadataResolveContext(Module));
		}
	}

	public struct TypeSpecification
	{
		public PEFile Module { get; }
		public TypeSpecificationHandle Handle { get; }
		public bool IsNil => Handle.IsNil;

		public TypeSpecification(PEFile module, TypeSpecificationHandle handle)
		{
			this.Module = module ?? throw new ArgumentNullException(nameof(module));
			this.Handle = handle;
		}

		SRMTypeSpec This() => Module.GetMetadataReader().GetTypeSpecification(Handle);

		public FullTypeName FullName {
			get {
				return DecodeSignature(new FullTypeNameSignatureDecoder(Module.GetMetadataReader()), default);
			}
		}

		public string Name => FullName.Name;

		public string Namespace => FullName.TopLevelTypeName.Namespace;

		public TypeDefinition GetDefinition()
		{
			return MetadataResolver.Resolve(Handle, new SimpleMetadataResolveContext(Module));
		}

		public TType DecodeSignature<TType, TGenericContext>(ISignatureTypeProvider<TType, TGenericContext> provider, TGenericContext genericContext)
		{
			return This().DecodeSignature(provider, genericContext);
		}
	}

	public struct MethodSpecification
	{
		public PEFile Module { get; }
		public MethodSpecificationHandle Handle { get; }
		public bool IsNil => Handle.IsNil;

		public MethodSpecification(PEFile module, MethodSpecificationHandle handle)
		{
			this.Module = module ?? throw new ArgumentNullException(nameof(module));
			this.Handle = handle;
		}
	}

	public struct MemberReference
	{
		public PEFile Module { get; }
		public MemberReferenceHandle Handle { get; }
		public bool IsNil => Handle.IsNil;

		SRMMemberRef This() => Module.GetMetadataReader().GetMemberReference(Handle);

		public MemberReference(PEFile module, MemberReferenceHandle handle)
		{
			this.Module = module ?? throw new ArgumentNullException(nameof(module));
			this.Handle = handle;
		}
	}

	public sealed class FullTypeNameSignatureDecoder : ISignatureTypeProvider<FullTypeName, Unit>
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
			return new FullTypeName($"System.{typeCode}");
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

		public FullTypeName GetTypeFromSpecification(MetadataReader reader, Unit genericContext, TypeSpecificationHandle handle, byte rawTypeKind)
		{
			return reader.GetTypeSpecification(handle).DecodeSignature(new FullTypeNameSignatureDecoder(metadata), default);
		}
	}

	public class GenericContext
	{
		readonly PEFile module;
		readonly MetadataReader metadata;
		readonly TypeDefinitionHandle declaringType;
		readonly MethodDefinitionHandle method;

		public static readonly GenericContext Empty = new GenericContext();

		private GenericContext() { }

		public GenericContext(MethodDefinitionHandle method, PEFile module)
		{
			this.module = module;
			this.metadata = module.GetMetadataReader();
			this.method = method;
			this.declaringType = metadata.GetMethodDefinition(method).GetDeclaringType();
		}

		public GenericContext(MethodDefinition method)
			: this(method.Handle, method.Module)
		{
		}

		public GenericContext(TypeDefinitionHandle declaringType, PEFile module)
		{
			this.module = module;
			this.metadata = module.GetMetadataReader();
			this.declaringType = declaringType;
		}

		public GenericContext(TypeDefinition declaringType)
			: this(declaringType.Handle, declaringType.Module)
		{
		}

		public string GetGenericTypeParameterName(int index)
		{
			GenericParameterHandle genericParameter = GetGenericTypeParameterHandleOrNull(index);
			if (genericParameter.IsNil)
				return index.ToString();
			return metadata.GetString(metadata.GetGenericParameter(genericParameter).Name);
		}

		public string GetGenericMethodTypeParameterName(int index)
		{
			GenericParameterHandle genericParameter = GetGenericMethodTypeParameterHandleOrNull(index);
			if (genericParameter.IsNil)
				return index.ToString();
			return metadata.GetString(metadata.GetGenericParameter(genericParameter).Name);
		}

		public GenericParameterHandle GetGenericTypeParameterHandleOrNull(int index)
		{
			GenericParameterHandleCollection genericParameters;
			if (declaringType.IsNil || index < 0 || index >= (genericParameters = metadata.GetTypeDefinition(declaringType).GetGenericParameters()).Count)
				return MetadataTokens.GenericParameterHandle(0);
			return genericParameters[index];
		}

		public GenericParameterHandle GetGenericMethodTypeParameterHandleOrNull(int index)
		{
			GenericParameterHandleCollection genericParameters;
			if (method.IsNil || index < 0 || index >= (genericParameters = metadata.GetMethodDefinition(method).GetGenericParameters()).Count)
				return MetadataTokens.GenericParameterHandle(0);
			return genericParameters[index];
		}
	}
}
