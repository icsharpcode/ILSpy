// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.Collections.Immutable;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using SRM = System.Reflection.Metadata;

namespace ICSharpCode.ILSpy
{
	class ILSignatureProvider : SRM.ISignatureTypeProvider<string, GenericContext>
	{
		public static readonly ILSignatureProvider WithoutNamespace = new ILSignatureProvider(false);
		public static readonly ILSignatureProvider WithNamespace = new ILSignatureProvider(true);

		bool includeNamespace;

		public ILSignatureProvider(bool includeNamespace)
		{
			this.includeNamespace = includeNamespace;
		}

		public string GetArrayType(string elementType, SRM.ArrayShape shape)
		{
			string printedShape = "";
			for (int i = 0; i < shape.Rank; i++) {
				if (i > 0)
					printedShape += ", ";
				if (i < shape.LowerBounds.Length || i < shape.Sizes.Length) {
					int lower = 0;
					if (i < shape.LowerBounds.Length) {
						lower = shape.LowerBounds[i];
						printedShape += lower.ToString();
					}
					printedShape += "...";
					if (i < shape.Sizes.Length)
						printedShape += (lower + shape.Sizes[i] - 1).ToString();
				}
			}
			return $"{elementType}[{printedShape}]";
		}

		public string GetByReferenceType(string elementType)
		{
			return elementType + "&";
		}

		public string GetFunctionPointerType(SRM.MethodSignature<string> signature)
		{
			return "method " + signature.ReturnType + " *(" + string.Join(", ", signature.ParameterTypes) + ")";
		}

		public string GetGenericInstantiation(string genericType, ImmutableArray<string> typeArguments)
		{
			return genericType + "<" + string.Join(", ", typeArguments) + ">";
		}

		public string GetGenericMethodParameter(GenericContext genericContext, int index)
		{
			return "!!" + genericContext.GetGenericMethodTypeParameterName(index);
		}

		public string GetGenericTypeParameter(GenericContext genericContext, int index)
		{
			return "!" + genericContext.GetGenericTypeParameterName(index);
		}

		public string GetModifiedType(string modifier, string unmodifiedType, bool isRequired)
		{
			string modifierKeyword = isRequired ? "modreq" : "modopt";
			return $"{unmodifiedType} {modifierKeyword}({modifier})";
		}

		public string GetPinnedType(string elementType)
		{
			throw new NotImplementedException();
		}

		public string GetPointerType(string elementType)
		{
			return elementType + "*";
		}

		public string GetPrimitiveType(SRM.PrimitiveTypeCode typeCode)
		{
			switch (typeCode) {
				case SRM.PrimitiveTypeCode.Boolean:
					return "bool";
				case SRM.PrimitiveTypeCode.Byte:
					return "uint8";
				case SRM.PrimitiveTypeCode.SByte:
					return "int8";
				case SRM.PrimitiveTypeCode.Char:
					return "char";
				case SRM.PrimitiveTypeCode.Int16:
					return "int16";
				case SRM.PrimitiveTypeCode.UInt16:
					return "uint16";
				case SRM.PrimitiveTypeCode.Int32:
					return "int32";
				case SRM.PrimitiveTypeCode.UInt32:
					return "uint32";
				case SRM.PrimitiveTypeCode.Int64:
					return "int64";
				case SRM.PrimitiveTypeCode.UInt64:
					return "uint64";
				case SRM.PrimitiveTypeCode.Single:
					return "float32";
				case SRM.PrimitiveTypeCode.Double:
					return "float64";
				case SRM.PrimitiveTypeCode.IntPtr:
					return "native int";
				case SRM.PrimitiveTypeCode.UIntPtr:
					return "native uint";
				case SRM.PrimitiveTypeCode.Object:
					return "object";
				case SRM.PrimitiveTypeCode.String:
					return "string";
				case SRM.PrimitiveTypeCode.TypedReference:
					return "typedref";
				case SRM.PrimitiveTypeCode.Void:
					return "void";
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public string GetSZArrayType(string elementType)
		{
			return elementType + "[]";
		}

		public string GetTypeFromDefinition(SRM.MetadataReader reader, SRM.TypeDefinitionHandle handle, byte rawTypeKind)
		{
			if (!includeNamespace) {
				return Decompiler.Disassembler.DisassemblerHelpers.Escape(handle.GetFullTypeName(reader).Name);
			}

			return handle.GetFullTypeName(reader).ToILNameString();
		}

		public string GetTypeFromReference(SRM.MetadataReader reader, SRM.TypeReferenceHandle handle, byte rawTypeKind)
		{
			if (!includeNamespace) {
				return Decompiler.Disassembler.DisassemblerHelpers.Escape(handle.GetFullTypeName(reader).Name);
			}

			return handle.GetFullTypeName(reader).ToILNameString();
		}

		public string GetTypeFromSpecification(SRM.MetadataReader reader, GenericContext genericContext, SRM.TypeSpecificationHandle handle, byte rawTypeKind)
		{
			return reader.GetTypeSpecification(handle).DecodeSignature(this, genericContext);
		}
	}
}
