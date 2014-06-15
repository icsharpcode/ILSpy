using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.IL
{
	static class ILTypeExtensions
	{
		/*public static ImmutableArray<StackType> GetStackTypes(this TypeSignatureCollection typeSignatureCollection)
		{
			var result = new StackType[typeSignatureCollection.Count];
			int i = 0;
			foreach (var typeSig in typeSignatureCollection) {
				result[i++] = GetStackType(typeSig);
			}
			return ImmutableArray.Create(result);
		}*/

		public static StackType GetStackType(this TypeReference typeRef)
		{
			return typeRef.SkipModifiers().MetadataType.GetStackType();
        }

		public static TypeReference SkipModifiers(this TypeReference typeRef)
		{
			while (typeRef.IsOptionalModifier || typeRef.IsRequiredModifier)
				typeRef = ((IModifierType)typeRef).ElementType;
			return typeRef;
		}

		public static StackType GetStackType(this MetadataType typeCode)
		{
			switch (typeCode) {
				case MetadataType.Boolean:
				case MetadataType.Char:
				case MetadataType.SByte:
				case MetadataType.Byte:
				case MetadataType.Int16:
				case MetadataType.UInt16:
				case MetadataType.Int32:
				case MetadataType.UInt32:
					return StackType.I4;
				case MetadataType.Int64:
				case MetadataType.UInt64:
					return StackType.I8;
				case MetadataType.IntPtr:
				case MetadataType.UIntPtr:
				case MetadataType.Pointer:
					return StackType.I;
				case MetadataType.Single:
				case MetadataType.Double:
					return StackType.F;
				case MetadataType.ByReference:
					return StackType.Ref;
				case MetadataType.Void:
					return StackType.Void;
				default:
					return StackType.O;
			}
		}

		public static StackType GetStackType(this PrimitiveType primitiveType)
		{
			return ((MetadataType)primitiveType).GetStackType();
		}
	}
}
