// Copyright (c) 2014 Daniel Grunwald
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
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.Decompiler.IL
{
	public static class NRTypeExtensions
	{
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
			var typeDef = type.GetDefinition();
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
				case KnownTypeCode.Byte:
				case KnownTypeCode.UInt16:
				case KnownTypeCode.UInt32:
				case KnownTypeCode.UInt64:
					return Sign.Unsigned;
				default:
					return Sign.None;
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
	
	public enum Sign
	{
		None,
		Signed,
		Unsigned
	}
}
