using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Util;

using static ICSharpCode.Decompiler.Metadata.ILOpCodeExtensions;

namespace ICSharpCode.Decompiler.CSharp
{
	class RequiredNamespaceCollector : ISignatureTypeProvider<Unit, Unit>
	{
		public HashSet<string> Namespaces { get; }

		HashSet<EntityHandle> visited = new HashSet<EntityHandle>();

		Unit IConstructedTypeProvider<Unit>.GetArrayType(Unit elementType, ArrayShape shape) => default;
		Unit IConstructedTypeProvider<Unit>.GetByReferenceType(Unit elementType) => default;
		Unit ISignatureTypeProvider<Unit, Unit>.GetFunctionPointerType(MethodSignature<Unit> signature) => default;
		Unit IConstructedTypeProvider<Unit>.GetGenericInstantiation(Unit genericType, ImmutableArray<Unit> typeArguments) => default;
		Unit ISignatureTypeProvider<Unit, Unit>.GetGenericMethodParameter(Unit genericContext, int index) => default;
		Unit ISignatureTypeProvider<Unit, Unit>.GetGenericTypeParameter(Unit genericContext, int index) => default;
		Unit ISignatureTypeProvider<Unit, Unit>.GetModifiedType(Unit modifier, Unit unmodifiedType, bool isRequired) => default;
		Unit ISignatureTypeProvider<Unit, Unit>.GetPinnedType(Unit elementType) => default;
		Unit IConstructedTypeProvider<Unit>.GetPointerType(Unit elementType) => default;
		Unit ISimpleTypeProvider<Unit>.GetPrimitiveType(PrimitiveTypeCode typeCode) => default;
		Unit ISZArrayTypeProvider<Unit>.GetSZArrayType(Unit elementType) => default;

		Unit ISimpleTypeProvider<Unit>.GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
		{
			var typeDef = reader.GetTypeDefinition(handle);
			GetNamespaceFrom(typeDef, reader);
			return default;
		}

		Unit ISimpleTypeProvider<Unit>.GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
		{
			var typeRef = reader.GetTypeReference(handle);

			if (!typeRef.Namespace.IsNil)
				Namespaces.Add(reader.GetString(typeRef.Namespace));
			return default;
		}

		Unit ISignatureTypeProvider<Unit, Unit>.GetTypeFromSpecification(MetadataReader reader, Unit genericContext, TypeSpecificationHandle handle, byte rawTypeKind)
		{
			reader.GetTypeSpecification(handle).DecodeSignature(this, default);
			return default;
		}

		void GetNamespaceFrom(TypeDefinition typeDef, MetadataReader reader)
		{
			TypeDefinitionHandle handle;
			while (!(handle = typeDef.GetDeclaringType()).IsNil) {
				typeDef = reader.GetTypeDefinition(handle);
			}
			if (!typeDef.Namespace.IsNil)
				Namespaces.Add(reader.GetString(typeDef.Namespace));
		}

		void GetNamespaceFrom(TypeReference typeRef, MetadataReader reader)
		{
			if (!typeRef.Namespace.IsNil) {
				Namespaces.Add(reader.GetString(typeRef.Namespace));
			} else {
				switch (typeRef.ResolutionScope.Kind) {
					case HandleKind.TypeReference:
						typeRef = reader.GetTypeReference((TypeReferenceHandle)typeRef.ResolutionScope);
						GetNamespaceFrom(typeRef, reader);
						break;
					case HandleKind.ModuleReference:
						break;
				}
			}
		}

		protected RequiredNamespaceCollector(HashSet<string> namespaces)
		{
			this.Namespaces = namespaces;
		}

		public static void CollectNamespaces(EntityHandle handle, PEReader reader, HashSet<string> namespaces)
		{
			switch (handle.Kind) {
				case HandleKind.TypeDefinition:
					CollectNamespaces((TypeDefinitionHandle)handle, reader, namespaces);
					break;
				case HandleKind.MethodDefinition:
					CollectNamespaces((MethodDefinitionHandle)handle, reader, namespaces);
					break;
				case HandleKind.PropertyDefinition:
					CollectNamespaces((PropertyDefinitionHandle)handle, reader, namespaces);
					break;
				case HandleKind.EventDefinition:
					CollectNamespaces((EventDefinitionHandle)handle, reader, namespaces);
					break;
				case HandleKind.FieldDefinition:
					CollectNamespaces((FieldDefinitionHandle)handle, reader, namespaces);
					break;
				default:
					throw new NotSupportedException();
			}
		}

		public static void CollectNamespaces(TypeDefinitionHandle handle, PEReader reader, HashSet<string> namespaces)
		{
			var inst = new RequiredNamespaceCollector(namespaces);
			inst.CollectNamespacesForDefinition(handle, reader);
		}

		public static void CollectNamespaces(MethodDefinitionHandle handle, PEReader reader, HashSet<string> namespaces)
		{
			var inst = new RequiredNamespaceCollector(namespaces);
			inst.CollectNamespacesForDefinition(handle, reader);
		}

		public static void CollectNamespaces(FieldDefinitionHandle handle, PEReader reader, HashSet<string> namespaces)
		{
			var inst = new RequiredNamespaceCollector(namespaces);
			inst.CollectNamespacesForDefinition(handle, reader);
		}

		public static void CollectNamespaces(PropertyDefinitionHandle handle, PEReader reader, HashSet<string> namespaces)
		{
			var inst = new RequiredNamespaceCollector(namespaces);
			inst.CollectNamespacesForDefinition(handle, reader);
		}

		public static void CollectNamespaces(EventDefinitionHandle handle, PEReader reader, HashSet<string> namespaces)
		{
			var inst = new RequiredNamespaceCollector(namespaces);
			inst.CollectNamespacesForDefinition(handle, reader);
		}

		public static void CollectNamespaces(MetadataReader metadata, HashSet<string> namespaces)
		{
			foreach (var h in metadata.TypeDefinitions) {
				var td = metadata.GetTypeDefinition(h);
				if (!td.Namespace.IsNil)
					namespaces.Add(metadata.GetString(td.Namespace));
			}
			foreach (var h in metadata.TypeReferences) {
				var tr = metadata.GetTypeReference(h);
				if (!tr.Namespace.IsNil)
					namespaces.Add(metadata.GetString(tr.Namespace));
			}
		}

		void CollectNamespacesFromAttributes(CustomAttributeHandleCollection attributes, MetadataReader metadata)
		{
			foreach (var handle in attributes) {
				var ca = metadata.GetCustomAttribute(handle);
				CollectNamespacesFromReference(ca.Constructor, metadata);
			}
		}

		void CollectNamespacesFromReference(EntityHandle handle, MetadataReader metadata)
		{
			TypeDefinition declaringType;
			switch (handle.Kind) {
				case HandleKind.TypeDefinition:
					var td = metadata.GetTypeDefinition((TypeDefinitionHandle)handle);
					GetNamespaceFrom(td, metadata);
					break;
				case HandleKind.FieldDefinition:
					var fd = metadata.GetFieldDefinition((FieldDefinitionHandle)handle);
					declaringType = metadata.GetTypeDefinition(fd.GetDeclaringType());
					GetNamespaceFrom(declaringType, metadata);
					break;
				case HandleKind.MethodDefinition:
					var md = metadata.GetMethodDefinition((MethodDefinitionHandle)handle);
					declaringType = metadata.GetTypeDefinition(md.GetDeclaringType());
					GetNamespaceFrom(declaringType, metadata);
					break;
				case HandleKind.TypeReference:
					var tr = metadata.GetTypeReference((TypeReferenceHandle)handle);
					GetNamespaceFrom(tr, metadata);
					break;
				case HandleKind.MemberReference:
					var mr = metadata.GetMemberReference((MemberReferenceHandle)handle);
					CollectNamespacesFromReference(mr.Parent, metadata);
					switch (mr.GetKind()) {
						case MemberReferenceKind.Field:
							mr.DecodeFieldSignature(this, default);
							break;
						case MemberReferenceKind.Method:
							mr.DecodeMethodSignature(this, default);
							break;
					}
					break;
				case HandleKind.MethodSpecification:
					var ms = metadata.GetMethodSpecification((MethodSpecificationHandle)handle);
					CollectNamespacesFromReference(ms.Method, metadata);
					ms.DecodeSignature(this, default);
					break;
				case HandleKind.TypeSpecification:
					var ts = metadata.GetTypeSpecification((TypeSpecificationHandle)handle);
					ts.DecodeSignature(this, default);
					break;
				default:
					throw new NotImplementedException();
			}
		}

		void CollectNamespacesForDefinition(EntityHandle handle, PEReader reader)
		{
			if (!visited.Add(handle))
				return;

			var metadata = reader.GetMetadataReader();

			TypeDefinition declaringType;
			switch (handle.Kind) {
				case HandleKind.TypeDefinition:
					var td = metadata.GetTypeDefinition((TypeDefinitionHandle)handle);
					GetNamespaceFrom(td, metadata);
					CollectNamespacesFromAttributes(td.GetCustomAttributes(), metadata);
					CollectNamespacesFromReference(td.BaseType, metadata);
					foreach (var h in td.GetInterfaceImplementations()) {
						var intf = metadata.GetInterfaceImplementation(h);
						CollectNamespacesFromReference(intf.Interface, metadata);
					}
					foreach (var nestedType in td.GetNestedTypes())
						CollectNamespacesForDefinition(nestedType, reader);
					foreach (var field in td.GetFields())
						CollectNamespacesForDefinition(field, reader);
					foreach (var property in td.GetProperties())
						CollectNamespacesForDefinition(property, reader);
					foreach (var @event in td.GetEvents())
						CollectNamespacesForDefinition(@event, reader);
					foreach (var method in td.GetMethods())
						CollectNamespacesForDefinition(method, reader);
					break;
				case HandleKind.FieldDefinition:
					var fd = metadata.GetFieldDefinition((FieldDefinitionHandle)handle);
					declaringType = metadata.GetTypeDefinition(fd.GetDeclaringType());
					GetNamespaceFrom(declaringType, metadata);
					CollectNamespacesFromAttributes(fd.GetCustomAttributes(), metadata);
					fd.DecodeSignature(this, default);
					if (!fd.GetMarshallingDescriptor().IsNil)
						Namespaces.Add("System.Runtime.InteropServices");
					break;
				case HandleKind.MethodDefinition:
					var md = metadata.GetMethodDefinition((MethodDefinitionHandle)handle);
					declaringType = metadata.GetTypeDefinition(md.GetDeclaringType());
					GetNamespaceFrom(declaringType, metadata);
					CollectNamespacesFromAttributes(md.GetCustomAttributes(), metadata);
					md.DecodeSignature(this, default);
					if (md.RelativeVirtualAddress > 0)
						CollectNamespacesFromMethodBody(reader.GetMethodBody(md.RelativeVirtualAddress), reader);
					break;
				case HandleKind.EventDefinition:
					var ed = metadata.GetEventDefinition((EventDefinitionHandle)handle);
					CollectNamespacesFromAttributes(ed.GetCustomAttributes(), metadata);
					CollectNamespacesFromReference(ed.Type, metadata);
					var edacc = ed.GetAccessors();
					if (!edacc.Adder.IsNil)
						CollectNamespacesForDefinition(edacc.Adder, reader);
					if (!edacc.Remover.IsNil)
						CollectNamespacesForDefinition(edacc.Remover, reader);
					if (!edacc.Raiser.IsNil)
						CollectNamespacesForDefinition(edacc.Raiser, reader);
					break;
				case HandleKind.PropertyDefinition:
					var pd = metadata.GetPropertyDefinition((PropertyDefinitionHandle)handle);
					CollectNamespacesFromAttributes(pd.GetCustomAttributes(), metadata);
					pd.DecodeSignature(this, default);
					var pdacc = pd.GetAccessors();
					if (!pdacc.Getter.IsNil)
						CollectNamespacesForDefinition(pdacc.Getter, reader);
					if (!pdacc.Setter.IsNil)
						CollectNamespacesForDefinition(pdacc.Setter, reader);
					break;
				default:
					break;
			}
		}

		void CollectNamespacesFromMethodBody(MethodBodyBlock method, PEReader reader)
		{
			var instructions = method.GetILReader();
			var metadata = reader.GetMetadataReader();
			while (instructions.RemainingBytes > 0) {
				var opCode = instructions.DecodeOpCode();
				switch (opCode.GetOperandType()) {
					case Metadata.OperandType.Field:
					case Metadata.OperandType.Method:
					case Metadata.OperandType.Sig:
					case Metadata.OperandType.Tok:
					case Metadata.OperandType.Type:
						var handle = MetadataTokens.EntityHandle(instructions.ReadInt32());
						CollectNamespacesFromReference(handle, metadata);
						break;
					default:
						instructions.SkipOperand(opCode);
						break;
				}
			}
		}
	}
}
