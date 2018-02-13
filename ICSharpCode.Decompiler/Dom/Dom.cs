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
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.Dom
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

	public interface ICustomAttributeProvider
	{
		PEFile Module { get; }
		CustomAttributeHandleCollection CustomAttributes { get; }
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
	}

	public struct Variable { }

	public interface IDebugInfoProvider
	{
		IList<SequencePoint> GetSequencePoints(MethodDefinition method);
		IList<Variable> GetVariables(MethodDefinition method);
	}

	public interface IMemberReference
	{
		PEFile Module { get; }
		string Name { get; }
		IMemberDefinition GetDefinition();
		ITypeReference DeclaringType { get; }
	}

	public interface IMemberDefinition : IMemberReference, ICustomAttributeProvider
	{
		new TypeDefinition DeclaringType { get; }
	}

	public interface IMethodReference : IMemberReference
	{
	}

	public interface ITypeReference : IMemberReference
	{
		PEFile Module { get; }
		FullTypeName FullName { get; }
		string Namespace { get; }
		new TypeDefinition GetDefinition();
	}

	public interface ITypeDefinition : ITypeReference, IMemberDefinition
	{
		new PEFile Module { get; }
		TypeDefinitionHandle Handle { get; }
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

		public MetadataReader GetMetadataReader() => Reader.GetMetadataReader();

		string GetName()
		{
			var metadata = GetMetadataReader();
			if (metadata.IsAssembly)
				return metadata.GetString(metadata.GetAssemblyDefinition().Name) + " (" + metadata.GetAssemblyDefinition().Version + ")";
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
			for (int row = 0; row < rowCount; row++) {
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

	public struct Resource
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

	public struct AssemblyReference : IAssemblyReference
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

		public AssemblyReference(PEFile module, AssemblyReferenceHandle handle)
		{
			Module = module;
			Handle = handle;
		}
	}

	public struct MethodDefinition : IEquatable<MethodDefinition>, IMethodReference, IMemberDefinition
	{
		public PEFile Module { get; }
		public MethodDefinitionHandle Handle { get; }
		public bool IsNil => Handle.IsNil;

		public MethodDefinition(PEFile module, MethodDefinitionHandle handle) : this()
		{
			this.Module = module ?? throw new ArgumentNullException(nameof(module));
			this.Handle = handle;
		}

		SRMMethod This() => Module.GetMetadataReader().GetMethodDefinition(Handle);

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

		public string Name {
			get {
				var reader = Module.GetMetadataReader();
				return reader.GetString(reader.GetMethodDefinition(Handle).Name);
			}
		}

		public TypeDefinition DeclaringType => new TypeDefinition(Module, This().GetDeclaringType());
		ITypeReference IMemberReference.DeclaringType => DeclaringType;

		public MethodAttributes Attributes => This().Attributes;
		public MethodImplAttributes ImplAttributes => This().ImplAttributes;
		public CustomAttributeHandleCollection CustomAttributes => This().GetCustomAttributes();
		public DeclarativeSecurityAttributeHandleCollection DeclarativeSecurityAttributes => This().GetDeclarativeSecurityAttributes();
		public bool HasFlag(MethodAttributes attribute) => (This().Attributes & attribute) == attribute;
		public bool HasPInvokeInfo => !This().GetImport().Module.IsNil || HasFlag(MethodAttributes.PinvokeImpl);
		public bool IsConstructor => This().IsConstructor(Module.GetMetadataReader());
		public bool HasParameters => This().GetParameters().Count > 0;
		public bool HasBody => (Attributes & MethodAttributes.Abstract) == 0 &&
			(Attributes & MethodAttributes.PinvokeImpl) == 0 &&
			(ImplAttributes & MethodImplAttributes.InternalCall) == 0 &&
			(ImplAttributes & MethodImplAttributes.Native) == 0 &&
			(ImplAttributes & MethodImplAttributes.Unmanaged) == 0 &&
			(ImplAttributes & MethodImplAttributes.Runtime) == 0;
		public int RVA => This().RelativeVirtualAddress;
		public MethodBodyBlock Body => Module.Reader.GetMethodBody(RVA);
		public MethodImport Import => This().GetImport();
		public GenericParameterHandleCollection GenericParameters => This().GetGenericParameters();
		public ParameterHandleCollection Parameters => This().GetParameters();

		public bool IsExtensionMethod {
			get {
				if (!HasFlag(MethodAttributes.Static)) {
					var metadata = Module.GetMetadataReader();
					foreach (var attribute in This().GetCustomAttributes()) {
						string typeName = metadata.GetCustomAttribute(attribute).GetAttributeType(Module).FullName.ToString();
						if (typeName == "System.Runtime.CompilerServices.ExtensionAttribute")
							return true;
					}
				}
				return false;
			}
		}

		public unsafe MethodSemanticsAttributes GetMethodSemanticsAttributes()
		{
			var reader = Module.GetMetadataReader();
			byte* startPointer = reader.MetadataPointer;
			int offset = reader.GetTableMetadataOffset(TableIndex.MethodSemantics);
			int rowSize = reader.GetTableRowSize(TableIndex.MethodSemantics);
			int rowCount = reader.GetTableRowCount(TableIndex.MethodSemantics);
			var small = reader.IsSmallReference(TableIndex.MethodDef);

			int methodRowNo = reader.GetRowNumber(Handle);
			for (int row = rowCount - 1; row >= 0; row--) {
				byte* ptr = startPointer + offset + rowSize * row;
				uint rowNo = small ? *(ushort*)(ptr + 2) : *(uint*)(ptr + 2);
				if (methodRowNo == rowNo) {
					return (MethodSemanticsAttributes)(*(ushort*)ptr);
				}
			}
			return 0;
		}

		public unsafe ImmutableArray<MethodImplementationHandle> GetMethodImplementations()
		{
			var reader = Module.GetMetadataReader();
			byte* startPointer = reader.MetadataPointer;
			int offset = reader.GetTableMetadataOffset(TableIndex.MethodImpl);
			int rowSize = reader.GetTableRowSize(TableIndex.MethodImpl);
			int rowCount = reader.GetTableRowCount(TableIndex.MethodImpl);
			var methodDefSize = reader.GetReferenceSize(TableIndex.MethodDef);
			var typeDefSize = reader.GetReferenceSize(TableIndex.TypeDef);

			var containingTypeRow = reader.GetRowNumber(This().GetDeclaringType());
			var methodDefRow = reader.GetRowNumber(Handle);

			var list = new List<MethodImplementationHandle>();

			// TODO : if sorted -> binary search?
			for (int row = 0; row < reader.GetTableRowCount(TableIndex.MethodImpl); row++) {
				byte* ptr = startPointer + offset + rowSize * row;
				uint currentTypeRow = typeDefSize == 2 ? *(ushort*)ptr : *(uint*)ptr;
				if (currentTypeRow != containingTypeRow) continue;
				uint currentMethodRowCoded = methodDefSize == 2 ? *(ushort*)(ptr + typeDefSize) : *(uint*)(ptr + typeDefSize);
				if ((currentMethodRowCoded >> 1) != methodDefRow) continue;
				list.Add(MetadataTokens.MethodImplementationHandle(row + 1));
			}

			return list.ToImmutableArray();
		}

		public IList<SequencePoint> GetSequencePoints()
		{
			return Module.DebugInfo?.GetSequencePoints(this);
		}

		public IList<Variable> GetVariables()
		{
			return Module.DebugInfo?.GetVariables(this);
		}

		public MethodSignature<TType> DecodeSignature<TType, TGenericContext>(ISignatureTypeProvider<TType, TGenericContext> provider, TGenericContext genericContext)
		{
			return This().DecodeSignature(provider, genericContext);
		}

		IMemberDefinition IMemberReference.GetDefinition() => this;
	}

	public struct PropertyDefinition : IEquatable<PropertyDefinition>, IMemberDefinition, ICustomAttributeProvider
	{
		public PEFile Module { get; }
		public PropertyDefinitionHandle Handle { get; }
		public bool IsNil => Handle.IsNil;

		public PropertyDefinition(PEFile module, PropertyDefinitionHandle handle) : this()
		{
			this.Module = module ?? throw new ArgumentNullException(nameof(module));
			this.Handle = handle;
		}

		SRMProperty This() => Module.GetMetadataReader().GetPropertyDefinition(Handle);

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

		public string Name {
			get {
				var reader = Module.GetMetadataReader();
				return reader.GetString(reader.GetPropertyDefinition(Handle).Name);
			}
		}

		public TypeDefinition DeclaringType => GetAccessors().First().Method.DeclaringType;
		ITypeReference IMemberReference.DeclaringType => DeclaringType;

		public MethodDefinition GetMethod => GetAccessors().FirstOrDefault(m => m.Kind == MethodSemanticsAttributes.Getter).Method;
		public MethodDefinition SetMethod => GetAccessors().FirstOrDefault(m => m.Kind == MethodSemanticsAttributes.Setter).Method;
		public ImmutableArray<MethodDefinition> OtherMethods => GetAccessors().Where(a => a.Kind == MethodSemanticsAttributes.Other).Select(a => a.Method).ToImmutableArray();

		public PropertyAttributes Attributes => This().Attributes;
		public bool HasFlag(PropertyAttributes attribute) => (This().Attributes & attribute) == attribute;
		public bool HasParameters => Handle.HasParameters(Module.GetMetadataReader());
		public CustomAttributeHandleCollection CustomAttributes => This().GetCustomAttributes();

		public bool IsIndexer => HasMatchingDefaultMemberAttribute(out var attr);

		public bool HasMatchingDefaultMemberAttribute(out CustomAttributeHandle defaultMemberAttribute)
		{
			var metadata = Module.GetMetadataReader();
			defaultMemberAttribute = default(CustomAttributeHandle);
			if (HasParameters) {
				var accessor = GetAccessors().First(a => a.Kind != MethodSemanticsAttributes.Other).Method;
				PropertyDefinition basePropDef = this;
				var firstOverrideHandle = accessor.GetMethodImplementations().FirstOrDefault();
				if (!firstOverrideHandle.IsNil) {
					// if the property is explicitly implementing an interface, look up the property in the interface:
					var firstOverride = metadata.GetMethodImplementation(firstOverrideHandle);
					var baseAccessor = firstOverride.MethodDeclaration.CoerceMemberReference(Module).GetDefinition() as MethodDefinition?;
					if (baseAccessor != null) {
						foreach (PropertyDefinition baseProp in baseAccessor?.DeclaringType.Properties) {
							if (baseProp.GetMethod == baseAccessor || baseProp.SetMethod == baseAccessor) {
								basePropDef = baseProp;
								break;
							}
						}
					} else
						return false;
				}
				var defaultMemberName = basePropDef.DeclaringType.Handle.GetDefaultMemberName(metadata, out var attr);
				if (defaultMemberName == basePropDef.Name) {
					defaultMemberAttribute = attr;
					return true;
				}
			}
			return false;
		}

		public unsafe ImmutableArray<(MethodSemanticsAttributes Kind, MethodDefinition Method)> GetAccessors()
		{
			var reader = Module.GetMetadataReader();
			byte* startPointer = reader.MetadataPointer;
			int offset = reader.GetTableMetadataOffset(TableIndex.MethodSemantics);

			uint encodedTag = (uint)(MetadataTokens.GetRowNumber(Handle) << 1) | 1;
			var methodDefRefSize = reader.GetReferenceSize(TableIndex.MethodDef);
			(int startRow, int endRow) = reader.BinarySearchRange(TableIndex.MethodSemantics, 2 + methodDefRefSize, encodedTag, reader.IsSmallReference(TableIndex.MethodSemantics));
			if (startRow == -1)
				return ImmutableArray<(MethodSemanticsAttributes Kind, MethodDefinition Method)>.Empty;
			var methods = new(MethodSemanticsAttributes Kind, MethodDefinition Method)[endRow - startRow + 1];
			int rowSize = reader.GetTableRowSize(TableIndex.MethodSemantics);
			for (int row = startRow; row <= endRow; row++) {
				int rowOffset = row * rowSize;
				byte* ptr = startPointer + offset + rowOffset;
				var kind = (MethodSemanticsAttributes)(*(ushort*)ptr);
				var handle = MetadataTokens.MethodDefinitionHandle(*(ushort*)(ptr + 2));
				methods[row - startRow] = (kind, new MethodDefinition(Module, handle));
			}
			return methods.ToImmutableArray();
		}

		public MethodSignature<TType> DecodeSignature<TType, TGenericContext>(ISignatureTypeProvider<TType, TGenericContext> provider, TGenericContext genericContext)
		{
			return This().DecodeSignature(provider, genericContext);
		}

		IMemberDefinition IMemberReference.GetDefinition() => this;
	}

	public struct FieldDefinition : IEquatable<FieldDefinition>, IMemberDefinition, ICustomAttributeProvider
	{
		public PEFile Module { get; }
		public FieldDefinitionHandle Handle { get; }
		public bool IsNil => Handle.IsNil;

		public FieldDefinition(PEFile module, FieldDefinitionHandle handle) : this()
		{
			this.Module = module ?? throw new ArgumentNullException(nameof(module));
			this.Handle = handle;
		}

		SRMField This() => Module.GetMetadataReader().GetFieldDefinition(Handle);

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

		public string Name {
			get {
				var reader = Module.GetMetadataReader();
				return reader.GetString(reader.GetFieldDefinition(Handle).Name);
			}
		}

		public TypeDefinition DeclaringType => new TypeDefinition(Module, This().GetDeclaringType());
		ITypeReference IMemberReference.DeclaringType => DeclaringType;

		public FieldAttributes Attributes => This().Attributes;
		public bool HasFlag(FieldAttributes attribute) => (This().Attributes & attribute) == attribute;
		public CustomAttributeHandleCollection CustomAttributes => This().GetCustomAttributes();
		public int RVA => This().GetRelativeVirtualAddress();
		public int Offset => This().GetOffset();

		public BlobHandle GetMarshallingDescriptor() => This().GetMarshallingDescriptor();
		public ConstantHandle GetDefaultValue() => This().GetDefaultValue();

		public TType DecodeSignature<TType, TGenericContext>(ISignatureTypeProvider<TType, TGenericContext> provider, TGenericContext genericContext)
		{
			return This().DecodeSignature(provider, genericContext);
		}

		public object DecodeConstant()
		{
			var metadata = Module.GetMetadataReader();
			var constant = metadata.GetConstant(GetDefaultValue());
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

		IMemberDefinition IMemberReference.GetDefinition() => this;
	}

	public struct EventDefinition : IEquatable<EventDefinition>, IMemberDefinition, ICustomAttributeProvider
	{
		public PEFile Module { get; }
		public EventDefinitionHandle Handle { get; }
		public bool IsNil => Handle.IsNil;

		public EventDefinition(PEFile module, EventDefinitionHandle handle)
		{
			this.Module = module ?? throw new ArgumentNullException(nameof(module));
			this.Handle = handle;
		}

		SRMEvent This() => Module.GetMetadataReader().GetEventDefinition(Handle);

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

		public string Name {
			get {
				var reader = Module.GetMetadataReader();
				return reader.GetString(reader.GetEventDefinition(Handle).Name);
			}
		}

		public TypeDefinition DeclaringType => GetAccessors().First().Method.DeclaringType;
		ITypeReference IMemberReference.DeclaringType => DeclaringType;

		public EventAttributes Attributes => This().Attributes;
		public bool HasFlag(EventAttributes attribute) => (This().Attributes & attribute) == attribute;
		public CustomAttributeHandleCollection CustomAttributes => This().GetCustomAttributes();

		public MethodDefinition AddMethod => GetAccessors().FirstOrDefault(m => m.Kind == MethodSemanticsAttributes.Adder).Method;
		public MethodDefinition RemoveMethod => GetAccessors().FirstOrDefault(m => m.Kind == MethodSemanticsAttributes.Remover).Method;
		public MethodDefinition InvokeMethod => GetAccessors().FirstOrDefault(m => m.Kind == MethodSemanticsAttributes.Raiser).Method;
		public ImmutableArray<MethodDefinition> OtherMethods => GetAccessors().Where(a => a.Kind == MethodSemanticsAttributes.Other).Select(a => a.Method).ToImmutableArray();

		public unsafe ImmutableArray<(MethodSemanticsAttributes Kind, MethodDefinition Method)> GetAccessors()
		{
			var reader = Module.GetMetadataReader();
			byte* startPointer = reader.MetadataPointer;
			int offset = reader.GetTableMetadataOffset(TableIndex.MethodSemantics);

			uint encodedTag = (uint)(MetadataTokens.GetRowNumber(Handle) << 1) | 0;
			var methodDefRefSize = reader.GetReferenceSize(TableIndex.MethodDef);
			(int startRow, int endRow) = reader.BinarySearchRange(TableIndex.MethodSemantics, 2 + methodDefRefSize, encodedTag, reader.IsSmallReference(TableIndex.MethodSemantics));
			if (startRow == -1)
				return ImmutableArray<(MethodSemanticsAttributes Kind, MethodDefinition Method)>.Empty;
			var methods = new(MethodSemanticsAttributes Kind, MethodDefinition Method)[endRow - startRow + 1];
			int rowSize = reader.GetTableRowSize(TableIndex.MethodSemantics);
			for (int row = startRow; row <= endRow; row++) {
				int rowOffset = row * rowSize;
				byte* ptr = startPointer + offset + rowOffset;
				var kind = (MethodSemanticsAttributes)(*(ushort*)ptr);
				var handle = MetadataTokens.MethodDefinitionHandle(*(ushort*)(ptr + 2));
				methods[row - startRow] = (kind, new MethodDefinition(Module, handle));
			}
			return methods.ToImmutableArray();
		}

		public TType DecodeSignature<TType, TGenericContext>(ISignatureTypeProvider<TType, TGenericContext> provider, TGenericContext genericContext)
		{
			var type = This().Type;
			var reader = Module.GetMetadataReader();
			switch (type.Kind) {
				case HandleKind.TypeDefinition:
					return provider.GetTypeFromDefinition(reader, (TypeDefinitionHandle)type, 0);
				case HandleKind.TypeReference:
					return provider.GetTypeFromReference(reader, (TypeReferenceHandle)type, 0);
				case HandleKind.TypeSpecification:
					return provider.GetTypeFromSpecification(reader, genericContext, (TypeSpecificationHandle)type, 0);
				default:
					throw new NotSupportedException();
			}
		}

		IMemberDefinition IMemberReference.GetDefinition() => this;
	}

	public struct TypeDefinition : IEquatable<TypeDefinition>, ITypeDefinition, ICustomAttributeProvider
	{
		public PEFile Module { get; }
		public TypeDefinitionHandle Handle { get; }
		public bool IsNil => Handle.IsNil;

		public TypeDefinition(PEFile module, TypeDefinitionHandle handle)
		{
			this.Module = module ?? throw new ArgumentNullException(nameof(module));
			this.Handle = handle;
		}

		internal SRMTypeDef This() => Module.GetMetadataReader().GetTypeDefinition(Handle);

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

		public string Name => Module.GetMetadataReader().GetString(This().Name);
		public string Namespace => Module.GetMetadataReader().GetString(This().Namespace);
		public FullTypeName FullName => This().GetFullTypeName(Module.GetMetadataReader());
		public TypeAttributes Attributes => This().Attributes;
		public bool HasFlag(TypeAttributes attribute) => (This().Attributes & attribute) == attribute;
		public TypeDefinition DeclaringType => new TypeDefinition(Module, This().GetDeclaringType());
		ITypeReference IMemberReference.DeclaringType => DeclaringType;
		public GenericParameterHandleCollection GenericParameters => This().GetGenericParameters();
		public CustomAttributeHandleCollection CustomAttributes => This().GetCustomAttributes();
		public DeclarativeSecurityAttributeHandleCollection DeclarativeSecurityAttributes => This().GetDeclarativeSecurityAttributes();
		public TypeLayout GetLayout() => This().GetLayout();

		public ITypeReference BaseType {
			get {
				var baseType = This().BaseType;
				return CreateTypeReference(baseType);
			}
		}

		ITypeReference CreateTypeReference(EntityHandle baseType)
		{
			if (baseType.IsNil)
				return null;
			switch (baseType.Kind) {
				case HandleKind.TypeDefinition:
					return new TypeDefinition(Module, (TypeDefinitionHandle)baseType);
				case HandleKind.TypeReference:
					return new TypeReference(Module, (TypeReferenceHandle)baseType);
				case HandleKind.TypeSpecification:
					return new TypeSpecification(Module, (TypeSpecificationHandle)baseType);
				default:
					throw new NotSupportedException();
			}
		}

		public bool HasInterfaces => This().GetInterfaceImplementations().Count > 0;

		public ImmutableArray<ITypeReference> Interfaces => GetInterfaces().ToImmutableArray();

		IEnumerable<ITypeReference> GetInterfaces()
		{
			var reader = Module.GetMetadataReader();
			foreach (var h in This().GetInterfaceImplementations()) {
				var interfaceImpl = reader.GetInterfaceImplementation(h);
				yield return CreateTypeReference(interfaceImpl.Interface);
			}
		}

		public bool IsValueType => This().IsValueType(Module.GetMetadataReader());
		public bool IsEnum => This().IsEnum(Module.GetMetadataReader());
		public bool IsInterface => (This().Attributes & TypeAttributes.ClassSemanticsMask) == TypeAttributes.Interface;
		public bool IsClass => (This().Attributes & TypeAttributes.ClassSemanticsMask) == TypeAttributes.Class;
		public bool IsNotPublic => (This().Attributes & TypeAttributes.VisibilityMask) == 0;
		public bool IsDelegate {
			get {
				var baseType = This().BaseType;
				return !baseType.IsNil && baseType.GetFullTypeName(Module.GetMetadataReader()).ToString() == typeof(MulticastDelegate).FullName;
			}
		}

		public ImmutableArray<TypeDefinition> NestedTypes {
			get {
				var module = Module;
				return This().GetNestedTypes().Select(nt => new TypeDefinition(module, nt)).ToImmutableArray();
			}
		}
		public ImmutableArray<FieldDefinition> Fields {
			get {
				var module = Module;
				return This().GetFields().Select(f => new FieldDefinition(module, f)).ToImmutableArray();
			}
		}
		public ImmutableArray<PropertyDefinition> Properties {
			get {
				var module = Module;
				return This().GetProperties().Select(p => new PropertyDefinition(module, p)).ToImmutableArray();
			}
		}
		public ImmutableArray<EventDefinition> Events {
			get {
				var module = Module;
				return This().GetEvents().Select(e => new EventDefinition(module, e)).ToImmutableArray();
			}
		}
		public ImmutableArray<MethodDefinition> Methods {
			get {
				var module = Module;
				return This().GetMethods().Select(m => new MethodDefinition(module, m)).ToImmutableArray();
			}
		}

		TypeDefinition ITypeReference.GetDefinition() => this;

		public InterfaceImplementationHandleCollection GetInterfaceImplementations() => This().GetInterfaceImplementations();

		IMemberDefinition IMemberReference.GetDefinition() => this;
	}

	public struct TypeReference : ITypeReference
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

		IMemberDefinition IMemberReference.GetDefinition() => GetDefinition();
		ITypeReference IMemberReference.DeclaringType => new TypeReference(Module, This().GetDeclaringType());
	}

	public struct TypeSpecification : ITypeReference
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
				return DecodeSignature(new FullTypeNameSignatureDecoder(Module.GetMetadataReader()), default(Unit));
			}
		}

		public string Name => FullName.Name;

		public string Namespace => FullName.TopLevelTypeName.Namespace;

		public ITypeReference DeclaringType => GetDefinition().DeclaringType;

		public TypeDefinition GetDefinition()
		{
			return MetadataResolver.Resolve(Handle, new SimpleMetadataResolveContext(Module));
		}

		IMemberDefinition IMemberReference.GetDefinition()
		{
			return GetDefinition();
		}

		public TType DecodeSignature<TType, TGenericContext>(ISignatureTypeProvider<TType, TGenericContext> provider, TGenericContext genericContext)
		{
			return This().DecodeSignature(provider, genericContext);
		}
	}

	public struct MethodSpecification : IMethodReference
	{
		public PEFile Module { get; }
		public MethodSpecificationHandle Handle { get; }
		public bool IsNil => Handle.IsNil;

		public string Name => This().Method.CoerceMemberReference(Module).Name;

		public MethodSpecification(PEFile module, MethodSpecificationHandle handle)
		{
			this.Module = module ?? throw new ArgumentNullException(nameof(module));
			this.Handle = handle;
		}

		SRMMethodSpec This() => Module.GetMetadataReader().GetMethodSpecification(Handle);

		public ImmutableArray<TType> DecodeSignature<TType, TGenericContext>(ISignatureTypeProvider<TType, TGenericContext> provider, TGenericContext genericContext)
		{
			return This().DecodeSignature(provider, genericContext);
		}

		public IMemberDefinition GetDefinition()
		{
			return Method.GetDefinition();
		}

		public ITypeReference DeclaringType => Method.DeclaringType;

		public IMemberReference Method => This().Method.CoerceMemberReference(Module);
	}

	public struct MemberReference : IMemberReference
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

		public string Name {
			get {
				var reader = Module.GetMetadataReader();
				return reader.GetString(reader.GetMemberReference(Handle).Name);
			}
		}

		public MemberReferenceKind Kind => This().GetKind();

		/// <summary>
		/// MethodDef, ModuleRef,TypeDef, TypeRef, or TypeSpec handle.
		/// </summary>
		public EntityHandle Parent => This().Parent;

		public MethodSignature<TType> DecodeMethodSignature<TType, TGenericContext>(ISignatureTypeProvider<TType, TGenericContext> provider, TGenericContext genericContext)
		{
			return This().DecodeMethodSignature(provider, genericContext);
		}

		public TType DecodeFieldSignature<TType, TGenericContext>(ISignatureTypeProvider<TType, TGenericContext> provider, TGenericContext genericContext)
		{
			return This().DecodeFieldSignature(provider, genericContext);
		}

		public IMemberDefinition GetDefinition()
		{
			return MetadataResolver.Resolve(Handle, new SimpleMetadataResolveContext(Module));
		}

		public TypeDefinition DeclaringType => GetDefinition().DeclaringType;

		ITypeReference IMemberReference.DeclaringType => DeclaringType;
	}

	class FullTypeNameSignatureDecoder : ISignatureTypeProvider<FullTypeName, Unit>
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
			throw new NotSupportedException();
		}

		public FullTypeName GetGenericInstantiation(FullTypeName genericType, ImmutableArray<FullTypeName> typeArguments)
		{
			return genericType;
		}

		public FullTypeName GetGenericMethodParameter(Unit genericContext, int index)
		{
			return default(FullTypeName);
		}

		public FullTypeName GetGenericTypeParameter(Unit genericContext, int index)
		{
			return default(FullTypeName);
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
			return handle.GetFullTypeName(reader, omitGenericParamCount: true);
		}

		public FullTypeName GetTypeFromSpecification(MetadataReader reader, Unit genericContext, TypeSpecificationHandle handle, byte rawTypeKind)
		{
			return reader.GetTypeSpecification(handle).DecodeSignature(new FullTypeNameSignatureDecoder(metadata), default(Unit));
		}
	}

	public class GenericContext
	{
		readonly MethodDefinition method;
		readonly TypeDefinition declaringType;
		readonly MetadataReader metadata;

		public static readonly GenericContext Empty = new GenericContext();

		private GenericContext() { }

		public GenericContext(MethodDefinition method)
		{
			this.method = method;
			this.declaringType = method.DeclaringType;
			this.metadata = method.Module.GetMetadataReader();
		}

		public GenericContext(TypeDefinition declaringType)
		{
			this.declaringType = declaringType;
			this.metadata = declaringType.Module.GetMetadataReader();
		}

		public string GetGenericTypeParameterName(int index)
		{
			if (declaringType.IsNil || index < 0 || index >= declaringType.GenericParameters.Count)
				return index.ToString();
			return metadata.GetString(metadata.GetGenericParameter(declaringType.GenericParameters[index]).Name);
		}

		public string GetGenericMethodTypeParameterName(int index)
		{
			if (method.IsNil || index < 0 || index >= method.GenericParameters.Count)
				return index.ToString();
			return metadata.GetString(metadata.GetGenericParameter(method.GenericParameters[index]).Name);
		}

		public GenericParameterHandle GetGenericTypeParameterHandleOrNull(int index)
		{
			if (declaringType.IsNil || index < 0 || index >= declaringType.GenericParameters.Count)
				return MetadataTokens.GenericParameterHandle(0);
			return declaringType.GenericParameters[index];
		}

		public GenericParameterHandle GetGenericMethodTypeParameterHandleOrNull(int index)
		{
			if (method.IsNil || index < 0 || index >= method.GenericParameters.Count)
				return MetadataTokens.GenericParameterHandle(0);
			return method.GenericParameters[index];
		}
	}
}
