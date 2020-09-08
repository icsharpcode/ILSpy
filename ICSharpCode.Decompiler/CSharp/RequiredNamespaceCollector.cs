using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;

using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;

using static ICSharpCode.Decompiler.Metadata.ILOpCodeExtensions;

namespace ICSharpCode.Decompiler.CSharp
{
	class RequiredNamespaceCollector
	{
		static readonly Decompiler.TypeSystem.GenericContext genericContext = default;

		readonly HashSet<string> namespaces;
		readonly HashSet<IType> visitedTypes = new HashSet<IType>();

		public RequiredNamespaceCollector(HashSet<string> namespaces)
		{
			this.namespaces = namespaces;
			for (int i = 0; i < KnownTypeReference.KnownTypeCodeCount; i++)
			{
				var ktr = KnownTypeReference.Get((KnownTypeCode)i);
				if (ktr == null)
					continue;
				namespaces.Add(ktr.Namespace);
			}
		}

		public static void CollectNamespaces(MetadataModule module, HashSet<string> namespaces)
		{
			var collector = new RequiredNamespaceCollector(namespaces);
			foreach (var type in module.TypeDefinitions)
			{
				collector.CollectNamespaces(type, module, (CodeMappingInfo)null);
			}
			collector.HandleAttributes(module.GetAssemblyAttributes());
			collector.HandleAttributes(module.GetModuleAttributes());
		}

		public static void CollectAttributeNamespaces(MetadataModule module, HashSet<string> namespaces)
		{
			var collector = new RequiredNamespaceCollector(namespaces);
			collector.HandleAttributes(module.GetAssemblyAttributes());
			collector.HandleAttributes(module.GetModuleAttributes());
		}

		public static void CollectNamespaces(IEntity entity, MetadataModule module, HashSet<string> namespaces)
		{
			var collector = new RequiredNamespaceCollector(namespaces);
			collector.CollectNamespaces(entity, module);
		}

		void CollectNamespaces(IEntity entity, MetadataModule module, CodeMappingInfo mappingInfo = null)
		{
			if (entity == null || entity.MetadataToken.IsNil)
				return;
			if (mappingInfo == null)
				mappingInfo = CSharpDecompiler.GetCodeMappingInfo(entity.ParentModule.PEFile, entity.MetadataToken);
			switch (entity)
			{
				case ITypeDefinition td:
					namespaces.Add(td.Namespace);
					HandleAttributes(td.GetAttributes());
					HandleTypeParameters(td.TypeParameters);

					foreach (var baseType in td.DirectBaseTypes)
					{
						CollectNamespacesForTypeReference(baseType);
					}

					foreach (var nestedType in td.NestedTypes)
					{
						CollectNamespaces(nestedType, module, mappingInfo);
					}

					foreach (var field in td.Fields)
					{
						CollectNamespaces(field, module, mappingInfo);
					}

					foreach (var property in td.Properties)
					{
						CollectNamespaces(property, module, mappingInfo);
					}

					foreach (var @event in td.Events)
					{
						CollectNamespaces(@event, module, mappingInfo);
					}

					foreach (var method in td.Methods)
					{
						CollectNamespaces(method, module, mappingInfo);
					}
					break;
				case IField field:
					HandleAttributes(field.GetAttributes());
					CollectNamespacesForTypeReference(field.ReturnType);
					break;
				case IMethod method:
					var reader = module.PEFile.Reader;
					var parts = mappingInfo.GetMethodParts((MethodDefinitionHandle)method.MetadataToken).ToList();
					foreach (var part in parts)
					{
						var partMethod = module.ResolveMethod(part, genericContext);
						HandleAttributes(partMethod.GetAttributes());
						HandleAttributes(partMethod.GetReturnTypeAttributes());
						CollectNamespacesForTypeReference(partMethod.ReturnType);
						foreach (var param in partMethod.Parameters)
						{
							HandleAttributes(param.GetAttributes());
							CollectNamespacesForTypeReference(param.Type);
						}
						HandleTypeParameters(partMethod.TypeParameters);
						HandleOverrides(part.GetMethodImplementations(module.metadata), module);
						var methodDef = module.metadata.GetMethodDefinition(part);
						if (method.HasBody)
						{
							MethodBodyBlock body;
							try
							{
								body = reader.GetMethodBody(methodDef.RelativeVirtualAddress);
							}
							catch (BadImageFormatException)
							{
								continue;
							}
							CollectNamespacesFromMethodBody(body, module);
						}
					}
					break;
				case IProperty property:
					HandleAttributes(property.GetAttributes());
					CollectNamespaces(property.Getter, module, mappingInfo);
					CollectNamespaces(property.Setter, module, mappingInfo);
					break;
				case IEvent @event:
					HandleAttributes(@event.GetAttributes());
					CollectNamespaces(@event.AddAccessor, module, mappingInfo);
					CollectNamespaces(@event.RemoveAccessor, module, mappingInfo);
					break;
			}
		}

		void HandleOverrides(ImmutableArray<MethodImplementationHandle> immutableArray, MetadataModule module)
		{
			foreach (var h in immutableArray)
			{
				var methodImpl = module.metadata.GetMethodImplementation(h);
				CollectNamespacesForTypeReference(module.ResolveType(methodImpl.Type, genericContext));
				CollectNamespacesForMemberReference(module.ResolveMethod(methodImpl.MethodBody, genericContext));
				CollectNamespacesForMemberReference(module.ResolveMethod(methodImpl.MethodDeclaration, genericContext));
			}
		}

		void CollectNamespacesForTypeReference(IType type)
		{
			if (!visitedTypes.Add(type))
				return;
			switch (type)
			{
				case ParameterizedType parameterizedType:
					namespaces.Add(parameterizedType.Namespace);
					CollectNamespacesForTypeReference(parameterizedType.GenericType);
					foreach (var arg in parameterizedType.TypeArguments)
						CollectNamespacesForTypeReference(arg);
					return; // no need to collect base types again
				case TypeWithElementType typeWithElementType:
					CollectNamespacesForTypeReference(typeWithElementType.ElementType);
					break;
				case TupleType tupleType:
					foreach (var elementType in tupleType.ElementTypes)
					{
						CollectNamespacesForTypeReference(elementType);
					}
					break;
				case FunctionPointerType fnPtrType:
					CollectNamespacesForTypeReference(fnPtrType.ReturnType);
					foreach (var paramType in fnPtrType.ParameterTypes)
					{
						CollectNamespacesForTypeReference(paramType);
					}
					break;
				default:
					namespaces.Add(type.Namespace);
					break;
			}
			foreach (var baseType in type.GetAllBaseTypes())
			{
				namespaces.Add(baseType.Namespace);
			}
		}

		public static void CollectNamespaces(EntityHandle entity, MetadataModule module, HashSet<string> namespaces)
		{
			if (entity.IsNil)
				return;
			CollectNamespaces(module.ResolveEntity(entity, genericContext), module, namespaces);
		}

		void HandleAttributes(IEnumerable<IAttribute> attributes)
		{
			foreach (var attr in attributes)
			{
				namespaces.Add(attr.AttributeType.Namespace);
				foreach (var arg in attr.FixedArguments)
				{
					HandleAttributeValue(arg.Type, arg.Value);
				}
				foreach (var arg in attr.NamedArguments)
				{
					HandleAttributeValue(arg.Type, arg.Value);
				}
			}
		}

		void HandleAttributeValue(IType type, object value)
		{
			CollectNamespacesForTypeReference(type);
			if (value is IType typeofType)
				CollectNamespacesForTypeReference(typeofType);
			if (value is ImmutableArray<CustomAttributeTypedArgument<IType>> arr)
			{
				foreach (var element in arr)
				{
					HandleAttributeValue(element.Type, element.Value);
				}
			}
		}

		void HandleTypeParameters(IEnumerable<ITypeParameter> typeParameters)
		{
			foreach (var typeParam in typeParameters)
			{
				HandleAttributes(typeParam.GetAttributes());

				foreach (var constraint in typeParam.DirectBaseTypes)
				{
					CollectNamespacesForTypeReference(constraint);
				}
			}
		}

		void CollectNamespacesFromMethodBody(MethodBodyBlock method, MetadataModule module)
		{
			var metadata = module.metadata;
			var instructions = method.GetILReader();

			if (!method.LocalSignature.IsNil)
			{
				ImmutableArray<IType> localSignature;
				try
				{
					localSignature = module.DecodeLocalSignature(method.LocalSignature, genericContext);
				}
				catch (BadImageFormatException)
				{
					// Issue #1211: ignore invalid local signatures
					localSignature = ImmutableArray<IType>.Empty;
				}
				foreach (var type in localSignature)
					CollectNamespacesForTypeReference(type);
			}

			foreach (var region in method.ExceptionRegions)
			{
				if (region.CatchType.IsNil)
					continue;
				IType ty;
				try
				{
					ty = module.ResolveType(region.CatchType, genericContext);
				}
				catch (BadImageFormatException)
				{
					continue;
				}
				CollectNamespacesForTypeReference(ty);
			}

			while (instructions.RemainingBytes > 0)
			{
				ILOpCode opCode;
				try
				{
					opCode = instructions.DecodeOpCode();
				}
				catch (BadImageFormatException)
				{
					return;
				}
				switch (opCode.GetOperandType())
				{
					case OperandType.Field:
					case OperandType.Method:
					case OperandType.Sig:
					case OperandType.Tok:
					case OperandType.Type:
						var handle = MetadataTokenHelpers.EntityHandleOrNil(instructions.ReadInt32());
						if (handle.IsNil)
							break;
						switch (handle.Kind)
						{
							case HandleKind.TypeDefinition:
							case HandleKind.TypeReference:
							case HandleKind.TypeSpecification:
								IType type;
								try
								{
									type = module.ResolveType(handle, genericContext);
								}
								catch (BadImageFormatException)
								{
									break;
								}
								CollectNamespacesForTypeReference(type);
								break;
							case HandleKind.FieldDefinition:
							case HandleKind.MethodDefinition:
							case HandleKind.MethodSpecification:
							case HandleKind.MemberReference:
								IMember member;
								try
								{
									member = module.ResolveEntity(handle, genericContext) as IMember;
								}
								catch (BadImageFormatException)
								{
									break;
								}
								CollectNamespacesForMemberReference(member);
								break;
							case HandleKind.StandaloneSignature:
								StandaloneSignature sig;
								try
								{
									sig = metadata.GetStandaloneSignature((StandaloneSignatureHandle)handle);
								}
								catch (BadImageFormatException)
								{
									break;
								}
								if (sig.GetKind() == StandaloneSignatureKind.Method)
								{
									FunctionPointerType fpt;
									try
									{
										(_, fpt) = module.DecodeMethodSignature((StandaloneSignatureHandle)handle, genericContext);
									}
									catch (BadImageFormatException)
									{
										break;
									}
									CollectNamespacesForTypeReference(fpt);
								}
								break;
						}
						break;
					default:
						try
						{
							instructions.SkipOperand(opCode);
						}
						catch (BadImageFormatException)
						{
							return;
						}
						break;
				}
			}
		}

		void CollectNamespacesForMemberReference(IMember member)
		{
			switch (member)
			{
				case IField field:
					CollectNamespacesForTypeReference(field.DeclaringType);
					CollectNamespacesForTypeReference(field.ReturnType);
					break;
				case IMethod method:
					CollectNamespacesForTypeReference(method.DeclaringType);
					CollectNamespacesForTypeReference(method.ReturnType);
					foreach (var param in method.Parameters)
						CollectNamespacesForTypeReference(param.Type);
					foreach (var arg in method.TypeArguments)
						CollectNamespacesForTypeReference(arg);
					break;
			}
		}
	}
}
