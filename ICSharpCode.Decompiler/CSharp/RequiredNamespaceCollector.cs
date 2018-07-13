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
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.Decompiler.Util;

using static ICSharpCode.Decompiler.Metadata.ILOpCodeExtensions;

namespace ICSharpCode.Decompiler.CSharp
{
	class RequiredNamespaceCollector
	{
		public static void CollectNamespaces(DecompilerTypeSystem typeSystem, HashSet<string> namespaces)
		{
			foreach (var type in typeSystem.MainAssembly.TypeDefinitions) {
				CollectNamespaces(type, typeSystem, namespaces);
			}
			CollectAttributeNamespaces(typeSystem, namespaces);
		}

		public static void CollectAttributeNamespaces(DecompilerTypeSystem typeSystem, HashSet<string> namespaces)
		{
			HandleAttributes(typeSystem.MainAssembly.GetAssemblyAttributes(), namespaces);
			HandleAttributes(typeSystem.MainAssembly.GetModuleAttributes(), namespaces);
		}

		public static void CollectNamespaces(IEntity entity, DecompilerTypeSystem typeSystem,
			HashSet<string> namespaces, CodeMappingInfo mappingInfo = null, bool scanningFullType = false)
		{
			if (entity == null || entity.MetadataToken.IsNil)
				return;
			switch (entity) {
				case ITypeDefinition td:
					if (mappingInfo == null)
						mappingInfo = CSharpDecompiler.GetCodeMappingInfo(entity.ParentAssembly.PEFile, entity.MetadataToken);
					namespaces.Add(td.Namespace);
					HandleAttributes(td.GetAttributes(), namespaces);

					foreach (var typeParam in td.TypeParameters) {
						HandleAttributes(typeParam.GetAttributes(), namespaces);
					}

					foreach (var baseType in td.DirectBaseTypes) {
						CollectNamespacesForTypeReference(baseType, namespaces);
					}

					foreach (var nestedType in td.NestedTypes) {
						CollectNamespaces(nestedType, typeSystem, namespaces, mappingInfo, scanningFullType: true);
					}

					foreach (var field in td.Fields) {
						CollectNamespaces(field, typeSystem, namespaces, mappingInfo, scanningFullType: true);
					}

					foreach (var property in td.Properties) {
						CollectNamespaces(property, typeSystem, namespaces, mappingInfo, scanningFullType: true);
					}

					foreach (var @event in td.Events) {
						CollectNamespaces(@event, typeSystem, namespaces, mappingInfo, scanningFullType: true);
					}

					foreach (var method in td.Methods) {
						CollectNamespaces(method, typeSystem, namespaces, mappingInfo, scanningFullType: true);
					}
					break;
				case IField field:
					HandleAttributes(field.GetAttributes(), namespaces);
					CollectNamespacesForTypeReference(field.ReturnType, namespaces);
					break;
				case IMethod method:
					HandleAttributes(method.GetAttributes(), namespaces);
					HandleAttributes(method.GetReturnTypeAttributes(), namespaces);
					CollectNamespacesForTypeReference(method.ReturnType, namespaces);
					foreach (var param in method.Parameters) {
						HandleAttributes(param.GetAttributes(), namespaces);
						CollectNamespacesForTypeReference(param.Type, namespaces);
					}
					foreach (var typeParam in method.TypeParameters) {
						HandleAttributes(typeParam.GetAttributes(), namespaces);
					}
					if (!method.MetadataToken.IsNil && method.HasBody) {
						if (mappingInfo == null)
							mappingInfo = CSharpDecompiler.GetCodeMappingInfo(entity.ParentAssembly.PEFile, entity.MetadataToken);
						var reader = typeSystem.ModuleDefinition.Reader;
						var parts = mappingInfo.GetMethodParts((MethodDefinitionHandle)method.MetadataToken).ToList();
						foreach (var part in parts) {
							var methodDef = typeSystem.ModuleDefinition.Metadata.GetMethodDefinition(part);
							var body = reader.GetMethodBody(methodDef.RelativeVirtualAddress);
							CollectNamespacesFromMethodBody(body, reader, typeSystem, namespaces, scanningFullType: scanningFullType);
						}
					}
					break;
				case IProperty property:
					HandleAttributes(property.GetAttributes(), namespaces);
					CollectNamespaces(property.Getter, typeSystem, namespaces);
					CollectNamespaces(property.Setter, typeSystem, namespaces);
					break;
				case IEvent @event:
					HandleAttributes(@event.GetAttributes(), namespaces);
					CollectNamespaces(@event.AddAccessor, typeSystem, namespaces);
					CollectNamespaces(@event.RemoveAccessor, typeSystem, namespaces);
					break;
			}
		}

		static void CollectNamespacesForTypeReference(IType type, HashSet<string> namespaces)
		{
			switch (type) {
				case ArrayType arrayType:
					namespaces.Add(arrayType.Namespace);
					CollectNamespacesForTypeReference(arrayType.ElementType, namespaces);
					break;
				case ParameterizedType parameterizedType:
					namespaces.Add(parameterizedType.Namespace);
					CollectNamespacesForTypeReference(parameterizedType.GenericType, namespaces);
					foreach (var arg in parameterizedType.TypeArguments)
						CollectNamespacesForTypeReference(arg, namespaces);
					break;
				case ByReferenceType byReferenceType:
					CollectNamespacesForTypeReference(byReferenceType.ElementType, namespaces);
					break;
				case PointerType pointerType:
					CollectNamespacesForTypeReference(pointerType.ElementType, namespaces);
					break;
				case TupleType tupleType:
					foreach (var elementType in tupleType.ElementTypes) {
						CollectNamespacesForTypeReference(elementType, namespaces);
					}
					break;
				default:
					namespaces.Add(type.Namespace);
					break;
			}
		}

		public static void CollectNamespaces(EntityHandle entity, DecompilerTypeSystem typeSystem, HashSet<string> namespaces)
		{
			if (entity.IsNil) return;
			if (entity.Kind.IsTypeKind()) {
				CollectNamespaces(typeSystem.ResolveAsType(entity).GetDefinition(), typeSystem, namespaces);
			} else {
				CollectNamespaces(typeSystem.ResolveAsMember(entity), typeSystem, namespaces);
			}
		}

		public static void HandleAttributes(IEnumerable<IAttribute> attributes, HashSet<string> namespaces)
		{
			foreach (var attr in attributes) {
				namespaces.Add(attr.AttributeType.Namespace);
				foreach (var arg in attr.FixedArguments) {
					HandleAttributeValue(arg.Type, arg.Value, namespaces);
				}
				foreach (var arg in attr.NamedArguments) {
					HandleAttributeValue(arg.Type, arg.Value, namespaces);
				}
			}
		}

		static void HandleAttributeValue(IType type, object value, HashSet<string> namespaces)
		{
			CollectNamespacesForTypeReference(type, namespaces);
			if (value is IType typeofType)
				CollectNamespacesForTypeReference(typeofType, namespaces);
			if (value is ImmutableArray<CustomAttributeTypedArgument<IType>> arr) {
				foreach (var element in arr) {
					HandleAttributeValue(element.Type, element.Value, namespaces);
				}
			}
		}

		static void CollectNamespacesFromMethodBody(MethodBodyBlock method, PEReader reader, DecompilerTypeSystem typeSystem, HashSet<string> namespaces, bool scanningFullType = false)
		{
			var instructions = method.GetILReader();
			var metadata = reader.GetMetadataReader();

			if (!method.LocalSignature.IsNil) {
				var localSignature = typeSystem.DecodeLocalSignature(method.LocalSignature);
				foreach (var type in localSignature)
					CollectNamespacesForTypeReference(type, namespaces);
			}

			foreach (var region in method.ExceptionRegions) {
				if (region.CatchType.IsNil) continue;
				CollectNamespacesForTypeReference(typeSystem.ResolveAsType(region.CatchType), namespaces);
			}

			while (instructions.RemainingBytes > 0) {
				var opCode = instructions.DecodeOpCode();
				switch (opCode.GetOperandType()) {
					case Metadata.OperandType.Field:
					case Metadata.OperandType.Method:
					case Metadata.OperandType.Sig:
					case Metadata.OperandType.Tok:
					case Metadata.OperandType.Type:
						var handle = MetadataTokenHelpers.EntityHandleOrNil(instructions.ReadInt32());
						if (handle.IsNil) break;
						switch (handle.Kind) {
							case HandleKind.TypeDefinition:
							case HandleKind.TypeReference:
							case HandleKind.TypeSpecification:
								CollectNamespacesForTypeReference(typeSystem.ResolveAsType(handle), namespaces);
								break;
							case HandleKind.FieldDefinition:
							case HandleKind.MethodDefinition:
							case HandleKind.MethodSpecification:
							case HandleKind.MemberReference:
								CollectNamespacesForMemberReference(typeSystem.ResolveAsMember(handle), typeSystem, namespaces, scanningFullType: scanningFullType);
								break;
							case HandleKind.StandaloneSignature:
								var sig = metadata.GetStandaloneSignature((StandaloneSignatureHandle)handle);
								if (sig.GetKind() == StandaloneSignatureKind.Method) {
									var methodSig = typeSystem.DecodeMethodSignature((StandaloneSignatureHandle)handle);
									CollectNamespacesForTypeReference(methodSig.ReturnType, namespaces);
									foreach (var paramType in methodSig.ParameterTypes) {
										CollectNamespacesForTypeReference(paramType, namespaces);
									}
								}
								break;
						}
						break;
					default:
						instructions.SkipOperand(opCode);
						break;
				}
			}
		}

		static void CollectNamespacesForMemberReference(IMember member, DecompilerTypeSystem typeSystem, HashSet<string> namespaces, bool scanningFullType = false)
		{
			switch (member) {
				case IField field:
					if (!scanningFullType && field.IsCompilerGeneratedOrIsInCompilerGeneratedClass())
						CollectNamespaces(field, typeSystem, namespaces);
					else
						CollectNamespacesForTypeReference(field.DeclaringType, namespaces);
					CollectNamespacesForTypeReference(field.ReturnType, namespaces);
					break;
				case IMethod method:
					if (!scanningFullType && method.IsCompilerGeneratedOrIsInCompilerGeneratedClass())
						CollectNamespaces(method, typeSystem, namespaces);
					else
						CollectNamespacesForTypeReference(method.DeclaringType, namespaces);
					CollectNamespacesForTypeReference(method.ReturnType, namespaces);
					foreach (var param in method.Parameters)
						CollectNamespacesForTypeReference(param.Type, namespaces);
					foreach (var arg in method.TypeArguments)
						CollectNamespacesForTypeReference(arg, namespaces);
					break;
			}
		}
	}
}
