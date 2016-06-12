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
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.Decompiler
{
	public static class DecompilerTypeSystemUtils
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
	}
}
