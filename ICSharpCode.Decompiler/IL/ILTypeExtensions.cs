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

using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL
{
	static class ILTypeExtensions
	{
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
		
		public static Sign GetSign(this PrimitiveType primitiveType)
		{
			switch (primitiveType) {
				case PrimitiveType.I1:
				case PrimitiveType.I2:
				case PrimitiveType.I4:
				case PrimitiveType.I8:
				case PrimitiveType.R4:
				case PrimitiveType.R8:
				case PrimitiveType.I:
					return Sign.Signed;
				case PrimitiveType.U1:
				case PrimitiveType.U2:
				case PrimitiveType.U4:
				case PrimitiveType.U8:
				case PrimitiveType.U:
					return Sign.Unsigned;
				default:
					return Sign.None;
			}
		}
		
		/// <summary>
		/// Gets whether the type is a small integer type.
		/// Small integer types are:
		/// * bool, sbyte, byte, char, short, ushort
		/// * any enums that have a small integer type as underlying type
		/// </summary>
		public static int GetSize(this PrimitiveType type)
		{
			switch (type) {
				case PrimitiveType.I1:
				case PrimitiveType.U1:
					return 1;
				case PrimitiveType.I2:
				case PrimitiveType.U2:
					return 2;
				case PrimitiveType.I4:
				case PrimitiveType.U4:
				case PrimitiveType.R4:
					return 4;
				case PrimitiveType.I8:
				case PrimitiveType.R8:
				case PrimitiveType.U8:
					return 8;
				case PrimitiveType.I:
				case PrimitiveType.U:
					return TypeUtils.NativeIntSize;
				default:
					return 0;
			}
		}
		
		/// <summary>
		/// Gets whether the type is a small integer type.
		/// Small integer types are:
		/// * bool, sbyte, byte, char, short, ushort
		/// * any enums that have a small integer type as underlying type
		/// </summary>
		public static bool IsSmallIntegerType(this PrimitiveType type)
		{
			return GetSize(type) < 4;
		}
		
		public static bool IsIntegerType(this PrimitiveType primitiveType)
		{
			return primitiveType.GetStackType().IsIntegerType();
		}
	}
}
