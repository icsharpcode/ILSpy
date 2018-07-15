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
		public static void CollectNamespaces(MetadataModule module, HashSet<string> namespaces)
		{
			foreach (var type in module.TypeDefinitions) {
				CollectNamespaces(type, module, namespaces);
			}
			CollectAttributeNamespaces(module, namespaces);
		}

		public static void CollectAttributeNamespaces(MetadataModule module, HashSet<string> namespaces)
		{
			HandleAttributes(module.GetAssemblyAttributes(), namespaces);
			HandleAttributes(module.GetModuleAttributes(), namespaces);
		}

		static readonly Decompiler.TypeSystem.GenericContext genericContext = default;

		public static void CollectNamespaces(IEntity entity, MetadataModule module,
			HashSet<string> namespaces, CodeMappingInfo mappingInfo = null)
		{
			if (entity == null || entity.MetadataToken.IsNil)
				return;
			switch (entity) {
				case ITypeDefinition td:
					if (mappingInfo == null)
						mappingInfo = CSharpDecompiler.GetCodeMappingInfo(entity.ParentModule.PEFile, entity.MetadataToken);
					namespaces.Add(td.Namespace);
					HandleAttributes(td.GetAttributes(), namespaces);

					foreach (var typeParam in td.TypeParameters) {
						HandleAttributes(typeParam.GetAttributes(), namespaces);
					}

					foreach (var baseType in td.DirectBaseTypes) {
						CollectNamespacesForTypeReference(baseType, namespaces);
					}

					foreach (var nestedType in td.NestedTypes) {
						CollectNamespaces(nestedType, module, namespaces, mappingInfo);
					}

					foreach (var field in td.Fields) {
						CollectNamespaces(field, module, namespaces, mappingInfo);
					}

					foreach (var property in td.Properties) {
						CollectNamespaces(property, module, namespaces, mappingInfo);
					}

					foreach (var @event in td.Events) {
						CollectNamespaces(@event, module, namespaces, mappingInfo);
					}

					foreach (var method in td.Methods) {
						CollectNamespaces(method, module, namespaces, mappingInfo);
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
							mappingInfo = CSharpDecompiler.GetCodeMappingInfo(entity.ParentModule.PEFile, entity.MetadataToken);
						var reader = module.PEFile.Reader;
						var parts = mappingInfo.GetMethodParts((MethodDefinitionHandle)method.MetadataToken).ToList();
						foreach (var part in parts) {
							var methodDef = module.metadata.GetMethodDefinition(part);
							var body = reader.GetMethodBody(methodDef.RelativeVirtualAddress);
							CollectNamespacesFromMethodBody(body, module, namespaces);
						}
					}
					break;
				case IProperty property:
					HandleAttributes(property.GetAttributes(), namespaces);
					CollectNamespaces(property.Getter, module, namespaces);
					CollectNamespaces(property.Setter, module, namespaces);
					break;
				case IEvent @event:
					HandleAttributes(@event.GetAttributes(), namespaces);
					CollectNamespaces(@event.AddAccessor, module, namespaces);
					CollectNamespaces(@event.RemoveAccessor, module, namespaces);
					break;
			}
		}

		static void CollectNamespacesForTypeReference(IType type, HashSet<string> namespaces)
		{
			switch (type) {
				case ParameterizedType parameterizedType:
					namespaces.Add(parameterizedType.Namespace);
					CollectNamespacesForTypeReference(parameterizedType.GenericType, namespaces);
					foreach (var arg in parameterizedType.TypeArguments)
						CollectNamespacesForTypeReference(arg, namespaces);
					break;
				case TypeWithElementType typeWithElementType:
					CollectNamespacesForTypeReference(typeWithElementType.ElementType, namespaces);
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

		public static void CollectNamespaces(EntityHandle entity, MetadataModule module, HashSet<string> namespaces)
		{
			if (entity.IsNil) return;
			CollectNamespaces(module.ResolveEntity(entity, genericContext), module, namespaces);
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

		static void CollectNamespacesFromMethodBody(MethodBodyBlock method, MetadataModule module, HashSet<string> namespaces)
		{
			var metadata = module.metadata;
			var instructions = method.GetILReader();

			if (!method.LocalSignature.IsNil) {
				var localSignature = module.DecodeLocalSignature(method.LocalSignature, genericContext);
				foreach (var type in localSignature)
					CollectNamespacesForTypeReference(type, namespaces);
			}

			foreach (var region in method.ExceptionRegions) {
				if (region.CatchType.IsNil) continue;
				CollectNamespacesForTypeReference(module.ResolveType(region.CatchType, genericContext), namespaces);
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
								CollectNamespacesForTypeReference(module.ResolveType(handle, genericContext), namespaces);
								break;
							case HandleKind.FieldDefinition:
							case HandleKind.MethodDefinition:
							case HandleKind.MethodSpecification:
							case HandleKind.MemberReference:
								CollectNamespacesForMemberReference(module.ResolveEntity(handle, genericContext) as IMember,
									module, namespaces);
								break;
							case HandleKind.StandaloneSignature:
								var sig = metadata.GetStandaloneSignature((StandaloneSignatureHandle)handle);
								if (sig.GetKind() == StandaloneSignatureKind.Method) {
									var methodSig = module.DecodeMethodSignature((StandaloneSignatureHandle)handle, genericContext);
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

		static void CollectNamespacesForMemberReference(IMember member, MetadataModule module, HashSet<string> namespaces)
		{
			switch (member) {
				case IField field:
					CollectNamespacesForTypeReference(field.DeclaringType, namespaces);
					CollectNamespacesForTypeReference(field.ReturnType, namespaces);
					break;
				case IMethod method:
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
