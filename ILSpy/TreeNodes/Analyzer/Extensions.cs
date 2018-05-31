using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Decompiler;

namespace ICSharpCode.ILSpy.TreeNodes.Analyzer
{
    public static class Extensions
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
	}
}
