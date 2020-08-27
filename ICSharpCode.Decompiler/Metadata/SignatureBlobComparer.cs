// Copyright (c) 2018 Siegfried Pammer
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

using System.Reflection.Metadata;

namespace ICSharpCode.Decompiler.Metadata
{
	public static class SignatureBlobComparer
	{
		public static bool EqualsMethodSignature(BlobReader a, BlobReader b, MetadataReader contextForA, MetadataReader contextForB)
		{
			return EqualsMethodSignature(ref a, ref b, contextForA, contextForB);
		}

		static bool EqualsMethodSignature(ref BlobReader a, ref BlobReader b, MetadataReader contextForA, MetadataReader contextForB)
		{
			SignatureHeader header;
			// compare signature headers
			if (a.RemainingBytes == 0 || b.RemainingBytes == 0 || (header = a.ReadSignatureHeader()) != b.ReadSignatureHeader())
				return false;
			if (header.IsGeneric)
			{
				// read & compare generic parameter count
				if (!IsSameCompressedInteger(ref a, ref b, out _))
					return false;
			}
			// read & compare parameter count
			if (!IsSameCompressedInteger(ref a, ref b, out int totalParameterCount))
				return false;
			if (!IsSameCompressedInteger(ref a, ref b, out int typeCode))
				return false;
			if (!TypesAreEqual(ref a, ref b, contextForA, contextForB, typeCode))
				return false;
			int i = 0;
			for (; i < totalParameterCount; i++)
			{
				if (!IsSameCompressedInteger(ref a, ref b, out typeCode))
					return false;
				// varargs sentinel
				if (typeCode == 65)
					break;
				if (!TypesAreEqual(ref a, ref b, contextForA, contextForB, typeCode))
					return false;
			}
			for (; i < totalParameterCount; i++)
			{
				if (!IsSameCompressedInteger(ref a, ref b, out typeCode))
					return false;
				if (!TypesAreEqual(ref a, ref b, contextForA, contextForB, typeCode))
					return false;
			}
			return true;
		}

		public static bool EqualsTypeSignature(BlobReader a, BlobReader b, MetadataReader contextForA, MetadataReader contextForB)
		{
			return EqualsTypeSignature(ref a, ref b, contextForA, contextForB);
		}

		static bool EqualsTypeSignature(ref BlobReader a, ref BlobReader b, MetadataReader contextForA, MetadataReader contextForB)
		{
			if (!IsSameCompressedInteger(ref a, ref b, out int typeCode))
				return false;
			return TypesAreEqual(ref a, ref b, contextForA, contextForB, typeCode);
		}

		static bool IsSameCompressedInteger(ref BlobReader a, ref BlobReader b, out int value)
		{
			return a.TryReadCompressedInteger(out value) && b.TryReadCompressedInteger(out int otherValue) && value == otherValue;
		}

		static bool IsSameCompressedSignedInteger(ref BlobReader a, ref BlobReader b, out int value)
		{
			return a.TryReadCompressedSignedInteger(out value) && b.TryReadCompressedSignedInteger(out int otherValue) && value == otherValue;
		}

		static bool TypesAreEqual(ref BlobReader a, ref BlobReader b, MetadataReader contextForA, MetadataReader contextForB, int typeCode)
		{
			switch (typeCode)
			{
				case 0x1: // ELEMENT_TYPE_VOID
				case 0x2: // ELEMENT_TYPE_BOOLEAN 
				case 0x3: // ELEMENT_TYPE_CHAR 
				case 0x4: // ELEMENT_TYPE_I1 
				case 0x5: // ELEMENT_TYPE_U1
				case 0x6: // ELEMENT_TYPE_I2
				case 0x7: // ELEMENT_TYPE_U2
				case 0x8: // ELEMENT_TYPE_I4
				case 0x9: // ELEMENT_TYPE_U4
				case 0xA: // ELEMENT_TYPE_I8
				case 0xB: // ELEMENT_TYPE_U8
				case 0xC: // ELEMENT_TYPE_R4
				case 0xD: // ELEMENT_TYPE_R8
				case 0xE: // ELEMENT_TYPE_STRING
				case 0x16: // ELEMENT_TYPE_TYPEDBYREF
				case 0x18: // ELEMENT_TYPE_I
				case 0x19: // ELEMENT_TYPE_U
				case 0x1C: // ELEMENT_TYPE_OBJECT
					return true;
				case 0xF: // ELEMENT_TYPE_PTR 
				case 0x10: // ELEMENT_TYPE_BYREF 
				case 0x45: // ELEMENT_TYPE_PINNED
				case 0x1D: // ELEMENT_TYPE_SZARRAY
					if (!IsSameCompressedInteger(ref a, ref b, out typeCode))
						return false;
					if (!TypesAreEqual(ref a, ref b, contextForA, contextForB, typeCode))
						return false;
					return true;
				case 0x1B: // ELEMENT_TYPE_FNPTR 
					if (!EqualsMethodSignature(ref a, ref b, contextForA, contextForB))
						return false;
					return true;
				case 0x14: // ELEMENT_TYPE_ARRAY 
						   // element type
					if (!IsSameCompressedInteger(ref a, ref b, out typeCode))
						return false;
					if (!TypesAreEqual(ref a, ref b, contextForA, contextForB, typeCode))
						return false;
					// rank
					if (!IsSameCompressedInteger(ref a, ref b, out _))
						return false;
					// sizes
					if (!IsSameCompressedInteger(ref a, ref b, out int numOfSizes))
						return false;
					for (int i = 0; i < numOfSizes; i++)
					{
						if (!IsSameCompressedInteger(ref a, ref b, out _))
							return false;
					}
					// lower bounds
					if (!IsSameCompressedInteger(ref a, ref b, out int numOfLowerBounds))
						return false;
					for (int i = 0; i < numOfLowerBounds; i++)
					{
						if (!IsSameCompressedSignedInteger(ref a, ref b, out _))
							return false;
					}
					return true;
				case 0x1F: // ELEMENT_TYPE_CMOD_REQD 
				case 0x20: // ELEMENT_TYPE_CMOD_OPT 
						   // modifier
					if (!TypeHandleEquals(ref a, ref b, contextForA, contextForB))
						return false;
					// unmodified type
					if (!IsSameCompressedInteger(ref a, ref b, out typeCode))
						return false;
					if (!TypesAreEqual(ref a, ref b, contextForA, contextForB, typeCode))
						return false;
					return true;
				case 0x15: // ELEMENT_TYPE_GENERICINST 
						   // generic type
					if (!IsSameCompressedInteger(ref a, ref b, out typeCode))
						return false;
					if (!TypesAreEqual(ref a, ref b, contextForA, contextForB, typeCode))
						return false;
					if (!IsSameCompressedInteger(ref a, ref b, out int numOfArguments))
						return false;
					for (int i = 0; i < numOfArguments; i++)
					{
						if (!IsSameCompressedInteger(ref a, ref b, out typeCode))
							return false;
						if (!TypesAreEqual(ref a, ref b, contextForA, contextForB, typeCode))
							return false;
					}
					return true;
				case 0x13: // ELEMENT_TYPE_VAR
				case 0x1E: // ELEMENT_TYPE_MVAR 
						   // index
					if (!IsSameCompressedInteger(ref a, ref b, out _))
						return false;
					return true;
				case 0x11: // ELEMENT_TYPE_VALUETYPE
				case 0x12: // ELEMENT_TYPE_CLASS
					if (!TypeHandleEquals(ref a, ref b, contextForA, contextForB))
						return false;
					return true;
				default:
					return false;
			}
		}

		static bool TypeHandleEquals(ref BlobReader a, ref BlobReader b, MetadataReader contextForA, MetadataReader contextForB)
		{
			var typeA = a.ReadTypeHandle();
			var typeB = b.ReadTypeHandle();
			if (typeA.IsNil || typeB.IsNil)
				return false;
			return typeA.GetFullTypeName(contextForA) == typeB.GetFullTypeName(contextForB);
		}
	}
}
