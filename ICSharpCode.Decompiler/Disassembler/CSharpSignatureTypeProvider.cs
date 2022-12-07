// Copyright (c) 2022 Tom-Englert
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
using System.Reflection.Metadata;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.Decompiler.Disassembler
{
	public class CSharpSignatureTypeProvider : ISignatureTypeProvider<Action<ITextOutput>, MetadataGenericContext>
	{
		public Action<ITextOutput> GetArrayType(Action<ITextOutput> elementType, ArrayShape shape)
		{
			return output => {
				elementType(output);
				output.Write('[');
				for (int i = 1; i < shape.Rank; i++)
				{
					output.Write(",");
				}
				output.Write(']');
			};
		}

		public Action<ITextOutput> GetByReferenceType(Action<ITextOutput> elementType)
		{
			return output => {
				output.Write("ref ");
				elementType(output);
			};
		}

		public Action<ITextOutput> GetFunctionPointerType(MethodSignature<Action<ITextOutput>> signature)
		{
			return output => {
				signature.ReturnType(output);
				output.Write(" *(");
				for (int i = 0; i < signature.ParameterTypes.Length; i++)
				{
					if (i > 0)
						output.Write(", ");
					signature.ParameterTypes[i](output);
				}
				output.Write(')');
			};
		}

		public Action<ITextOutput> GetGenericInstantiation(Action<ITextOutput> genericType, ImmutableArray<Action<ITextOutput>> typeArguments)
		{
			return output => {
				genericType(output);
				output.Write('<');
				for (int i = 0; i < typeArguments.Length; i++)
				{
					if (i > 0)
						output.Write(", ");
					typeArguments[i](output);
				}
				output.Write('>');
			};
		}

		public Action<ITextOutput> GetGenericMethodParameter(MetadataGenericContext genericContext, int index)
		{
			return output => WriteTypeParameter(genericContext.Metadata, genericContext.GetGenericMethodTypeParameterHandleOrNull(index), index, output);
		}

		public Action<ITextOutput> GetGenericTypeParameter(MetadataGenericContext genericContext, int index)
		{
			return output => WriteTypeParameter(genericContext.Metadata, genericContext.GetGenericTypeParameterHandleOrNull(index), index, output);
		}

		private static void WriteTypeParameter(MetadataReader metadata, GenericParameterHandle paramRef, int index, ITextOutput output)
		{
			if (paramRef.IsNil)
			{
				output.Write(index.ToString());
			}
			else
			{
				var param = metadata.GetGenericParameter(paramRef);
				output.Write(param.Name.IsNil ? param.Index.ToString() : DisassemblerHelpers.Escape(metadata.GetString(param.Name)));
			}
		}

		public Action<ITextOutput> GetModifiedType(Action<ITextOutput> modifier, Action<ITextOutput> unmodifiedType, bool isRequired)
		{
			return unmodifiedType;
		}

		public Action<ITextOutput> GetPinnedType(Action<ITextOutput> elementType)
		{
			return elementType;
		}

		public Action<ITextOutput> GetPointerType(Action<ITextOutput> elementType)
		{
			return output => {
				elementType(output);
				output.Write('*');
			};
		}

		public Action<ITextOutput> GetPrimitiveType(PrimitiveTypeCode typeCode)
		{
			switch (typeCode)
			{
				case PrimitiveTypeCode.SByte:
					return output => output.Write("sbyte");
				case PrimitiveTypeCode.Int16:
					return output => output.Write("short");
				case PrimitiveTypeCode.Int32:
					return output => output.Write("int");
				case PrimitiveTypeCode.Int64:
					return output => output.Write("long");
				case PrimitiveTypeCode.Byte:
					return output => output.Write("byte");
				case PrimitiveTypeCode.UInt16:
					return output => output.Write("ushort");
				case PrimitiveTypeCode.UInt32:
					return output => output.Write("uint");
				case PrimitiveTypeCode.UInt64:
					return output => output.Write("ulong");
				case PrimitiveTypeCode.Single:
					return output => output.Write("float");
				case PrimitiveTypeCode.Double:
					return output => output.Write("double");
				case PrimitiveTypeCode.Void:
					return output => output.Write("void");
				case PrimitiveTypeCode.Boolean:
					return output => output.Write("bool");
				case PrimitiveTypeCode.String:
					return output => output.Write("string");
				case PrimitiveTypeCode.Char:
					return output => output.Write("char");
				case PrimitiveTypeCode.Object:
					return output => output.Write("object");
				case PrimitiveTypeCode.IntPtr:
					return output => output.Write("IntPtr");
				case PrimitiveTypeCode.UIntPtr:
					return output => output.Write("UIntPtr");
				case PrimitiveTypeCode.TypedReference:
					return output => output.Write("typedref");
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public Action<ITextOutput> GetSZArrayType(Action<ITextOutput> elementType)
		{
			return output => {
				elementType(output);
				output.Write("[]");
			};
		}

		public Action<ITextOutput> GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
		{
			return output => output.Write(handle.GetFullTypeName(reader).ToCSharpNameString(true));
		}

		public Action<ITextOutput> GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
		{
			return output => output.Write(handle.GetFullTypeName(reader).ToCSharpNameString(true));
		}

		public Action<ITextOutput> GetTypeFromSpecification(MetadataReader reader, MetadataGenericContext genericContext, TypeSpecificationHandle handle, byte rawTypeKind)
		{
			return reader.GetTypeSpecification(handle).DecodeSignature(this, genericContext);
		}
	}
}
