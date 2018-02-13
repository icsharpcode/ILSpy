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
using System.Collections.Generic;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using SRM = System.Reflection.Metadata;

namespace ICSharpCode.Decompiler.Disassembler
{
	sealed class BlobDecoder
	{
		SRM.BlobReader blob;
		static readonly ITypeResolveContext context = new SimpleTypeResolveContext(MinimalCorlib.Instance.CreateCompilation());

		public BlobDecoder(SRM.BlobReader blob)
		{
			this.blob = blob;
		}

		public int Offset => blob.Offset;

		public ResolveResult ReadFixedArg(IType argType)
		{
			if (argType.Kind == TypeKind.Array) {
				if (((ArrayType)argType).Dimensions != 1) {
					// Only single-dimensional arrays are supported
					return ErrorResolveResult.UnknownError;
				}
				IType elementType = ((ArrayType)argType).ElementType;
				uint numElem = blob.ReadUInt32();
				if (numElem == 0xffffffff) {
					// null reference
					return new ConstantResolveResult(argType, null);
				} else {
					ResolveResult[] elements = new ResolveResult[numElem];
					for (int i = 0; i < elements.Length; i++) {
						elements[i] = ReadElem(elementType);
						// Stop decoding when encountering an error:
						if (elements[i].IsError)
							return ErrorResolveResult.UnknownError;
					}
					IType int32 = context.Compilation.FindType(KnownTypeCode.Int32);
					ResolveResult[] sizeArgs = { new ConstantResolveResult(int32, elements.Length) };
					return new ArrayCreateResolveResult(argType, sizeArgs, elements);
				}
			} else {
				return ReadElem(argType);
			}
		}

		public ResolveResult ReadElem(IType elementType)
		{
			ITypeDefinition underlyingType;
			if (elementType.Kind == TypeKind.Enum) {
				underlyingType = elementType.GetDefinition().EnumUnderlyingType.GetDefinition();
			} else {
				underlyingType = elementType.GetDefinition();
			}
			if (underlyingType == null)
				return ErrorResolveResult.UnknownError;
			KnownTypeCode typeCode = underlyingType.KnownTypeCode;
			if (typeCode == KnownTypeCode.Object) {
				// boxed value type
				IType boxedTyped = ReadCustomAttributeFieldOrPropType();
				ResolveResult elem = ReadFixedArg(boxedTyped);
				if (elem.IsCompileTimeConstant && elem.ConstantValue == null)
					return new ConstantResolveResult(elementType, null);
				else
					return new ConversionResolveResult(elementType, elem, Conversion.BoxingConversion);
			} else if (typeCode == KnownTypeCode.Type) {
				var type = ReadType();
				if (type != null) {
					return new TypeOfResolveResult(underlyingType, type);
				} else {
					return new ConstantResolveResult(underlyingType, null);
				}
			} else {
				return new ConstantResolveResult(elementType, ReadElemValue(typeCode));
			}
		}

		object ReadElemValue(KnownTypeCode typeCode)
		{
			switch (typeCode) {
				case KnownTypeCode.Boolean:
					return blob.ReadBoolean();
				case KnownTypeCode.Char:
					return blob.ReadChar();
				case KnownTypeCode.SByte:
					return blob.ReadSByte();
				case KnownTypeCode.Byte:
					return blob.ReadByte();
				case KnownTypeCode.Int16:
					return blob.ReadInt16();
				case KnownTypeCode.UInt16:
					return blob.ReadUInt16();
				case KnownTypeCode.Int32:
					return blob.ReadInt32();
				case KnownTypeCode.UInt32:
					return blob.ReadUInt32();
				case KnownTypeCode.Int64:
					return blob.ReadInt64();
				case KnownTypeCode.UInt64:
					return blob.ReadUInt64();
				case KnownTypeCode.Single:
					return blob.ReadSingle();
				case KnownTypeCode.Double:
					return blob.ReadDouble();
				case KnownTypeCode.String:
					return blob.ReadSerializedString();
				default:
					throw new NotSupportedException();
			}
		}

		public KeyValuePair<string, ResolveResult> ReadNamedArg()
		{
			SymbolKind memberType;
			var b = blob.ReadByte();
			switch (b) {
				case 0x53:
					memberType = SymbolKind.Field;
					break;
				case 0x54:
					memberType = SymbolKind.Property;
					break;
				default:
					throw new NotSupportedException(string.Format("Custom member type 0x{0:x} is not supported.", b));
			}
			IType type = ReadCustomAttributeFieldOrPropType();
			string name = blob.ReadSerializedString();
			ResolveResult val = ReadFixedArg(type);
			return new KeyValuePair<string, ResolveResult>(name, val);
		}

		IType ReadCustomAttributeFieldOrPropType()
		{
			ICompilation compilation = context.Compilation;
			var b = blob.ReadByte();
			switch (b) {
				case 0x02:
					return compilation.FindType(KnownTypeCode.Boolean);
				case 0x03:
					return compilation.FindType(KnownTypeCode.Char);
				case 0x04:
					return compilation.FindType(KnownTypeCode.SByte);
				case 0x05:
					return compilation.FindType(KnownTypeCode.Byte);
				case 0x06:
					return compilation.FindType(KnownTypeCode.Int16);
				case 0x07:
					return compilation.FindType(KnownTypeCode.UInt16);
				case 0x08:
					return compilation.FindType(KnownTypeCode.Int32);
				case 0x09:
					return compilation.FindType(KnownTypeCode.UInt32);
				case 0x0a:
					return compilation.FindType(KnownTypeCode.Int64);
				case 0x0b:
					return compilation.FindType(KnownTypeCode.UInt64);
				case 0x0c:
					return compilation.FindType(KnownTypeCode.Single);
				case 0x0d:
					return compilation.FindType(KnownTypeCode.Double);
				case 0x0e:
					return compilation.FindType(KnownTypeCode.String);
				case 0x1d:
					return new ArrayType(compilation, ReadCustomAttributeFieldOrPropType());
				case 0x50:
					return compilation.FindType(KnownTypeCode.Type);
				case 0x51: // boxed value type
					return compilation.FindType(KnownTypeCode.Object);
				case 0x55: // enum
					var type = ReadType();
					if (type == null) {
						throw new NotSupportedException("Enum type should not be null.");
					}
					return type;
				default:
					throw new NotSupportedException(string.Format("Custom attribute type 0x{0:x} is not supported.", b));
			}
		}

		IType ReadType()
		{
			string typeName = blob.ReadSerializedString();
			if (typeName == null) {
				return null;
			}
			ITypeReference typeReference = ReflectionHelper.ParseReflectionName(typeName);
			IType typeInCurrentAssembly = typeReference.Resolve(context);
			if (typeInCurrentAssembly.Kind != TypeKind.Unknown)
				return typeInCurrentAssembly;

			// look for the type in mscorlib
			ITypeDefinition systemObject = context.Compilation.FindType(KnownTypeCode.Object).GetDefinition();
			if (systemObject != null) {
				return typeReference.Resolve(new SimpleTypeResolveContext(systemObject.ParentAssembly));
			} else {
				// couldn't find corlib - return the unknown IType for the current assembly
				return typeInCurrentAssembly;
			}
		}
	}
}