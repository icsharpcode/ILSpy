using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.TreeNodes.Analyzer
{
	public static class Helpers
	{
		public static TypeDefinitionHandle GetDeclaringType(this MethodDefinitionHandle method, MetadataReader metadata)
		{
			if (method.IsNil)
				throw new ArgumentNullException(nameof(method));
			return metadata.GetMethodDefinition(method).GetDeclaringType();
		}

		public static TypeDefinitionHandle GetDeclaringType(this FieldDefinitionHandle field, MetadataReader metadata)
		{
			if (field.IsNil)
				throw new ArgumentNullException(nameof(field));
			return metadata.GetFieldDefinition(field).GetDeclaringType();
		}

		public static TypeDefinitionHandle GetDeclaringType(this PropertyDefinitionHandle property, MetadataReader metadata)
		{
			if (property.IsNil)
				throw new ArgumentNullException(nameof(property));
			var accessor = metadata.GetPropertyDefinition(property).GetAccessors().GetAny();
			return metadata.GetMethodDefinition(accessor).GetDeclaringType();
		}

		public static TypeDefinitionHandle GetDeclaringType(this EventDefinitionHandle @event, MetadataReader metadata)
		{
			if (@event.IsNil)
				throw new ArgumentNullException(nameof(@event));
			var accessor = metadata.GetEventDefinition(@event).GetAccessors().GetAny();
			return metadata.GetMethodDefinition(accessor).GetDeclaringType();
		}

		public static bool IsSameMethod(MethodDefinitionHandle method, PEFile definitionModule, EntityHandle entity, PEFile entityModule)
		{
			if (method.IsNil || entity.IsNil)
				return false;
			var md = definitionModule.Metadata.GetMethodDefinition(method);
			if (entity.Kind == HandleKind.MethodSpecification) {
				var ms = entityModule.Metadata.GetMethodSpecification((MethodSpecificationHandle)entity);
				entity = ms.Method;
			}
			switch (entity.Kind) {
				case HandleKind.MethodDefinition:
					return method == entity && definitionModule == entityModule;
				case HandleKind.MemberReference:
					var mr = entityModule.Metadata.GetMemberReference((MemberReferenceHandle)entity);
					if (mr.GetKind() != MemberReferenceKind.Method)
						return false;
					if (!entityModule.Metadata.StringComparer.Equals(mr.Name, definitionModule.Metadata.GetString(md.Name)))
						return false;
					if (mr.Parent.Kind.IsTypeKind()) {
						if (!IsSameType(md.GetDeclaringType(), definitionModule, mr.Parent, entityModule))
							return false;
					} else {
						// TODO ...
					}
					return SignatureBlobComparer.EqualsMethodSignature(
						definitionModule.Metadata.GetBlobReader(md.Signature),
						entityModule.Metadata.GetBlobReader(mr.Signature),
						definitionModule.Metadata, entityModule.Metadata);
				default:
					return false;
			}
		}

		public static bool IsSameType(TypeDefinitionHandle type, PEFile module, EntityHandle entity, PEFile entityModule)
		{
			if (type.IsNil || entity.IsNil)
				return false;
			var td = module.Metadata.GetTypeDefinition(type);
			if (entity.Kind == HandleKind.TypeSpecification) {
				var ts = entityModule.Metadata.GetTypeSpecification((TypeSpecificationHandle)entity);
				entity = ts.DecodeSignature(new Unspecializer(), default);
			}
			switch (entity.Kind) {
				case HandleKind.TypeDefinition:
					return type == entity && module == entityModule;
				case HandleKind.TypeReference:
					return type.GetFullTypeName(module.Metadata) == entity.GetFullTypeName(entityModule.Metadata);
				default:
					return false;
			}
		}
	}
}
