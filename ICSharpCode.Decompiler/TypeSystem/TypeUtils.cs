// Copyright (c) 2015 Siegfried Pammer
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
using System;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.Decompiler
{
	public static class TypeUtils
	{
		static int GetNativeSize(IType type)
		{
			const int NativeIntSize = 6; // between 4 (Int32) and 8 (Int64)
			if (type.Kind == TypeKind.Pointer)
				return NativeIntSize;
			
			var typeForConstant = (type.Kind == TypeKind.Enum) ? type.GetDefinition().EnumUnderlyingType : type;
			var typeDef = typeForConstant.GetDefinition();
			if (typeDef == null)
				return 0;
			switch (typeDef.KnownTypeCode) {
				case KnownTypeCode.Boolean:
				case KnownTypeCode.SByte:
				case KnownTypeCode.Byte:
					return 1;
				case KnownTypeCode.Char:
				case KnownTypeCode.Int16:
				case KnownTypeCode.UInt16:
					return 2;
				case KnownTypeCode.Int32:
				case KnownTypeCode.UInt32:
				case KnownTypeCode.Single:
					return 4;
				case KnownTypeCode.IntPtr:
				case KnownTypeCode.UIntPtr:
					return NativeIntSize;
				case KnownTypeCode.Int64:
				case KnownTypeCode.UInt64:
				case KnownTypeCode.Double:
					return 8;
			}
			return 0;
		}
		
		public static IType GetLargerType(IType type1, IType type2)
		{
			return GetNativeSize(type1) >= GetNativeSize(type2) ? type1 : type2;
		}
		
		public static bool IsSmallIntegerType(this IType type)
		{
			return GetNativeSize(type) < 4;
		}
		
		/// <summary>
		/// Gets whether reading/writing an element of accessType from the pointer
		/// is equivalent to reading/writing an element of the pointer's element type.
		/// </summary>
		public static bool IsCompatibleTypeForMemoryAccess(IType pointerType, IType accessType)
		{
			IType memoryType;
			if (pointerType is PointerType)
				memoryType = ((PointerType)pointerType).ElementType;
			else if (pointerType is ByReferenceType)
				memoryType = ((ByReferenceType)pointerType).ElementType;
			else
				return false;
			ITypeDefinition memoryTypeDef = memoryType.GetDefinition();
			ITypeDefinition accessTypeDef = accessType.GetDefinition();
			if (memoryType.Kind == TypeKind.Enum && memoryTypeDef != null) {
				memoryType = memoryTypeDef.EnumUnderlyingType;
				memoryTypeDef = memoryType.GetDefinition();
			}
			if (accessType.Kind == TypeKind.Enum && accessTypeDef != null) {
				accessType = accessTypeDef.EnumUnderlyingType;
				accessTypeDef = accessType.GetDefinition();
			}
			if (memoryType.Equals(accessType))
				return true;
			// If the types are not equal, the access still might produce equal results:
			if (memoryType.IsReferenceType == true && accessType.IsReferenceType == true)
				return true;
			if (memoryTypeDef != null) {
				switch (memoryTypeDef.KnownTypeCode) {
					case KnownTypeCode.Byte:
					case KnownTypeCode.SByte:
						// Reading small integers of different signs is not equivalent due to sign extension,
						// but writes are equivalent (truncation works the same for signed and unsigned)
						return accessType.IsKnownType(KnownTypeCode.Byte) || accessType.IsKnownType(KnownTypeCode.SByte);
					case KnownTypeCode.Int16:
					case KnownTypeCode.UInt16:
						return accessType.IsKnownType(KnownTypeCode.Int16) || accessType.IsKnownType(KnownTypeCode.UInt16);
					case KnownTypeCode.Int32:
					case KnownTypeCode.UInt32:
						return accessType.IsKnownType(KnownTypeCode.Int32) || accessType.IsKnownType(KnownTypeCode.UInt32);
					case KnownTypeCode.IntPtr:
					case KnownTypeCode.UIntPtr:
						return accessType.IsKnownType(KnownTypeCode.IntPtr) || accessType.IsKnownType(KnownTypeCode.UIntPtr);
					case KnownTypeCode.Int64:
					case KnownTypeCode.UInt64:
						return accessType.IsKnownType(KnownTypeCode.Int64) || accessType.IsKnownType(KnownTypeCode.UInt64);
					case KnownTypeCode.Char:
						return accessType.IsKnownType(KnownTypeCode.Char) || accessType.IsKnownType(KnownTypeCode.UInt16) || accessType.IsKnownType(KnownTypeCode.Int16);
					case KnownTypeCode.Boolean:
						return accessType.IsKnownType(KnownTypeCode.Boolean) || accessType.IsKnownType(KnownTypeCode.Byte) || accessType.IsKnownType(KnownTypeCode.SByte);
				}
			}
			return false;
		}

		public static StackType GetStackType(this IType type)
		{
			switch (type.Kind)
			{
				case TypeKind.Unknown:
					return StackType.Unknown;
				case TypeKind.ByReference:
					return StackType.Ref;
				case TypeKind.Pointer:
					return StackType.I;
			}
			ITypeDefinition typeDef = type.GetDefinition();
			if (typeDef == null)
				return StackType.O;
			if (typeDef.Kind == TypeKind.Enum) {
				typeDef = typeDef.EnumUnderlyingType.GetDefinition();
				if (typeDef == null)
					return StackType.O;
			}
			switch (typeDef.KnownTypeCode) {
				case KnownTypeCode.Boolean:
				case KnownTypeCode.Char:
				case KnownTypeCode.SByte:
				case KnownTypeCode.Byte:
				case KnownTypeCode.Int16:
				case KnownTypeCode.UInt16:
				case KnownTypeCode.Int32:
				case KnownTypeCode.UInt32:
					return StackType.I4;
				case KnownTypeCode.Int64:
				case KnownTypeCode.UInt64:
					return StackType.I8;
				case KnownTypeCode.Single:
				case KnownTypeCode.Double:
					return StackType.F;
				case KnownTypeCode.Void:
					return StackType.Void;
				case KnownTypeCode.IntPtr:
				case KnownTypeCode.UIntPtr:
					return StackType.I;
				default:
					return StackType.O;
			}
		}
		
		public static Sign GetSign(this IType type)
		{
			var typeForConstant = (type.Kind == TypeKind.Enum) ? type.GetDefinition().EnumUnderlyingType : type;
			var typeDef = typeForConstant.GetDefinition();
			if (typeDef == null)
				return Sign.None;
			switch (typeDef.KnownTypeCode) {
				case KnownTypeCode.SByte:
				case KnownTypeCode.Int16:
				case KnownTypeCode.Int32:
				case KnownTypeCode.Int64:
				case KnownTypeCode.IntPtr:
				case KnownTypeCode.Single:
				case KnownTypeCode.Double:
				case KnownTypeCode.Decimal:
					return Sign.Signed;
				case KnownTypeCode.UIntPtr:
				case KnownTypeCode.Char:
				case KnownTypeCode.Boolean:
				case KnownTypeCode.Byte:
				case KnownTypeCode.UInt16:
				case KnownTypeCode.UInt32:
				case KnownTypeCode.UInt64:
					return Sign.Unsigned;
				default:
					return Sign.None;
			}
		}
		
		public static KnownTypeCode ToKnownTypeCode(this PrimitiveType primitiveType)
		{
			switch (primitiveType) {
				case PrimitiveType.I1:
					return KnownTypeCode.SByte;
				case PrimitiveType.I2:
					return KnownTypeCode.Int16;
				case PrimitiveType.I4:
					return KnownTypeCode.Int32;
				case PrimitiveType.I8:
					return KnownTypeCode.Int64;
				case PrimitiveType.R4:
					return KnownTypeCode.Single;
				case PrimitiveType.R8:
					return KnownTypeCode.Double;
				case PrimitiveType.U1:
					return KnownTypeCode.Byte;
				case PrimitiveType.U2:
					return KnownTypeCode.UInt16;
				case PrimitiveType.U4:
					return KnownTypeCode.UInt32;
				case PrimitiveType.U8:
					return KnownTypeCode.UInt64;
				case PrimitiveType.I:
					return KnownTypeCode.IntPtr;
				case PrimitiveType.U:
					return KnownTypeCode.UIntPtr;
				default:
					return KnownTypeCode.None;
			}
		}
		
		public static KnownTypeCode ToKnownTypeCode(this StackType stackType, Sign sign = Sign.None)
		{
			switch (stackType) {
				case StackType.I4:
					return sign == Sign.Unsigned ? KnownTypeCode.UInt32 : KnownTypeCode.Int32;
				case StackType.I8:
					return sign == Sign.Unsigned ? KnownTypeCode.UInt64 : KnownTypeCode.Int64;
				case StackType.I:
					return sign == Sign.Unsigned ? KnownTypeCode.UIntPtr : KnownTypeCode.IntPtr;
				case StackType.F:
					return KnownTypeCode.Double;
				case StackType.O:
					return KnownTypeCode.Object;
				case StackType.Void:
					return KnownTypeCode.Void;
				default:
					return KnownTypeCode.None;
			}
		}
	}
	
	public enum Sign : byte
	{
		None,
		Signed,
		Unsigned
	}
}
