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
using System.Reflection.Metadata;

using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.Decompiler.Disassembler
{
	public class DisassemblerSignatureTypeProvider : ISignatureTypeProvider<Action<ILNameSyntax>, GenericContext>
	{
		readonly PEFile module;
		readonly MetadataReader metadata;
		readonly ITextOutput output;

		public DisassemblerSignatureTypeProvider(PEFile module, ITextOutput output)
		{
			this.module = module ?? throw new ArgumentNullException(nameof(module));
			this.output = output ?? throw new ArgumentNullException(nameof(output));
			this.metadata = module.Metadata;
		}

		public Action<ILNameSyntax> GetArrayType(Action<ILNameSyntax> elementType, ArrayShape shape)
		{
			return syntax => {
				var syntaxForElementTypes = syntax == ILNameSyntax.SignatureNoNamedTypeParameters ? syntax : ILNameSyntax.Signature;
				elementType(syntaxForElementTypes);
				output.Write('[');
				for (int i = 0; i < shape.Rank; i++)
				{
					if (i > 0)
						output.Write(", ");
					if (i < shape.LowerBounds.Length || i < shape.Sizes.Length)
					{
						int lower = 0;
						if (i < shape.LowerBounds.Length)
						{
							lower = shape.LowerBounds[i];
							output.Write(lower.ToString());
						}
						output.Write("...");
						if (i < shape.Sizes.Length)
							output.Write((lower + shape.Sizes[i] - 1).ToString());
					}
				}
				output.Write(']');
			};
		}

		public Action<ILNameSyntax> GetByReferenceType(Action<ILNameSyntax> elementType)
		{
			return syntax => {
				var syntaxForElementTypes = syntax == ILNameSyntax.SignatureNoNamedTypeParameters ? syntax : ILNameSyntax.Signature;
				elementType(syntaxForElementTypes);
				output.Write('&');
			};
		}

		public Action<ILNameSyntax> GetFunctionPointerType(MethodSignature<Action<ILNameSyntax>> signature)
		{
			return syntax => {
				output.Write("method ");
				signature.Header.WriteTo(output);
				signature.ReturnType(syntax);
				output.Write(" *(");
				for (int i = 0; i < signature.ParameterTypes.Length; i++)
				{
					if (i > 0)
						output.Write(", ");
					signature.ParameterTypes[i](syntax);
				}
				output.Write(')');
			};
		}

		public Action<ILNameSyntax> GetGenericInstantiation(Action<ILNameSyntax> genericType, ImmutableArray<Action<ILNameSyntax>> typeArguments)
		{
			return syntax => {
				var syntaxForElementTypes = syntax == ILNameSyntax.SignatureNoNamedTypeParameters ? syntax : ILNameSyntax.Signature;
				genericType(syntaxForElementTypes);
				output.Write('<');
				for (int i = 0; i < typeArguments.Length; i++)
				{
					if (i > 0)
						output.Write(", ");
					typeArguments[i](syntaxForElementTypes);
				}
				output.Write('>');
			};
		}

		public Action<ILNameSyntax> GetGenericMethodParameter(GenericContext genericContext, int index)
		{
			return syntax => {
				output.Write("!!");
				WriteTypeParameter(genericContext.GetGenericMethodTypeParameterHandleOrNull(index), index, syntax);
			};
		}

		public Action<ILNameSyntax> GetGenericTypeParameter(GenericContext genericContext, int index)
		{
			return syntax => {
				output.Write("!");
				WriteTypeParameter(genericContext.GetGenericTypeParameterHandleOrNull(index), index, syntax);
			};
		}

		void WriteTypeParameter(GenericParameterHandle paramRef, int index, ILNameSyntax syntax)
		{
			if (paramRef.IsNil || syntax == ILNameSyntax.SignatureNoNamedTypeParameters)
				output.Write(index.ToString());
			else
			{
				var param = metadata.GetGenericParameter(paramRef);
				if (param.Name.IsNil)
					output.Write(param.Index.ToString());
				else
					output.Write(DisassemblerHelpers.Escape(metadata.GetString(param.Name)));
			}
		}

		public Action<ILNameSyntax> GetModifiedType(Action<ILNameSyntax> modifier, Action<ILNameSyntax> unmodifiedType, bool isRequired)
		{
			return syntax => {
				unmodifiedType(syntax);
				if (isRequired)
					output.Write(" modreq");
				else
					output.Write(" modopt");
				output.Write('(');
				modifier(ILNameSyntax.TypeName);
				output.Write(')');
			};
		}

		public Action<ILNameSyntax> GetPinnedType(Action<ILNameSyntax> elementType)
		{
			return syntax => {
				var syntaxForElementTypes = syntax == ILNameSyntax.SignatureNoNamedTypeParameters ? syntax : ILNameSyntax.Signature;
				elementType(syntaxForElementTypes);
				output.Write(" pinned");
			};
		}

		public Action<ILNameSyntax> GetPointerType(Action<ILNameSyntax> elementType)
		{
			return syntax => {
				var syntaxForElementTypes = syntax == ILNameSyntax.SignatureNoNamedTypeParameters ? syntax : ILNameSyntax.Signature;
				elementType(syntaxForElementTypes);
				output.Write('*');
			};
		}

		public Action<ILNameSyntax> GetPrimitiveType(PrimitiveTypeCode typeCode)
		{
			switch (typeCode)
			{
				case PrimitiveTypeCode.SByte:
					return syntax => output.Write("int8");
				case PrimitiveTypeCode.Int16:
					return syntax => output.Write("int16");
				case PrimitiveTypeCode.Int32:
					return syntax => output.Write("int32");
				case PrimitiveTypeCode.Int64:
					return syntax => output.Write("int64");
				case PrimitiveTypeCode.Byte:
					return syntax => output.Write("uint8");
				case PrimitiveTypeCode.UInt16:
					return syntax => output.Write("uint16");
				case PrimitiveTypeCode.UInt32:
					return syntax => output.Write("uint32");
				case PrimitiveTypeCode.UInt64:
					return syntax => output.Write("uint64");
				case PrimitiveTypeCode.Single:
					return syntax => output.Write("float32");
				case PrimitiveTypeCode.Double:
					return syntax => output.Write("float64");
				case PrimitiveTypeCode.Void:
					return syntax => output.Write("void");
				case PrimitiveTypeCode.Boolean:
					return syntax => output.Write("bool");
				case PrimitiveTypeCode.String:
					return syntax => output.Write("string");
				case PrimitiveTypeCode.Char:
					return syntax => output.Write("char");
				case PrimitiveTypeCode.Object:
					return syntax => output.Write("object");
				case PrimitiveTypeCode.IntPtr:
					return syntax => output.Write("native int");
				case PrimitiveTypeCode.UIntPtr:
					return syntax => output.Write("native uint");
				case PrimitiveTypeCode.TypedReference:
					return syntax => output.Write("typedref");
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public Action<ILNameSyntax> GetSZArrayType(Action<ILNameSyntax> elementType)
		{
			return syntax => {
				var syntaxForElementTypes = syntax == ILNameSyntax.SignatureNoNamedTypeParameters ? syntax : ILNameSyntax.Signature;
				elementType(syntaxForElementTypes);
				output.Write('[');
				output.Write(']');
			};
		}

		public Action<ILNameSyntax> GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
		{
			return syntax => {
				switch (rawTypeKind)
				{
					case 0x00:
						break;
					case 0x11:
						output.Write("valuetype ");
						break;
					case 0x12:
						output.Write("class ");
						break;
					default:
						throw new BadImageFormatException($"Unexpected rawTypeKind: {rawTypeKind} (0x{rawTypeKind:x})");
				}
				((EntityHandle)handle).WriteTo(module, output, GenericContext.Empty);
			};
		}

		public Action<ILNameSyntax> GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
		{
			return syntax => {
				switch (rawTypeKind)
				{
					case 0x00:
						break;
					case 0x11:
						output.Write("valuetype ");
						break;
					case 0x12:
						output.Write("class ");
						break;
					default:
						throw new BadImageFormatException($"Unexpected rawTypeKind: {rawTypeKind} (0x{rawTypeKind:x})");
				}
				((EntityHandle)handle).WriteTo(module, output, GenericContext.Empty);
			};
		}

		public Action<ILNameSyntax> GetTypeFromSpecification(MetadataReader reader, GenericContext genericContext, TypeSpecificationHandle handle, byte rawTypeKind)
		{
			return reader.GetTypeSpecification(handle).DecodeSignature(this, genericContext);
		}
	}
}
