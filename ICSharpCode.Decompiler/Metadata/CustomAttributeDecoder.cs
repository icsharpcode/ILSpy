// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Immutable;
using System.Reflection.Metadata;

namespace ICSharpCode.Decompiler.Metadata
{
	/// <summary>
	/// Decodes custom attribute blobs.
	/// </summary>
	internal readonly struct CustomAttributeDecoder<TType>
	{
		// This is a stripped-down copy of SRM's internal CustomAttributeDecoder.
		// We need it to decode security declarations.

		private readonly ICustomAttributeTypeProvider<TType> _provider;
		private readonly MetadataReader _reader;
		private readonly bool _provideBoxingTypeInfo;

		public CustomAttributeDecoder(ICustomAttributeTypeProvider<TType> provider, MetadataReader reader, bool provideBoxingTypeInfo = false)
		{
			_reader = reader;
			_provider = provider;
			_provideBoxingTypeInfo = provideBoxingTypeInfo;
		}

		public ImmutableArray<CustomAttributeNamedArgument<TType>> DecodeNamedArguments(ref BlobReader valueReader, int count)
		{
			var arguments = ImmutableArray.CreateBuilder<CustomAttributeNamedArgument<TType>>(count);
			for (int i = 0; i < count; i++)
			{
				CustomAttributeNamedArgumentKind kind = (CustomAttributeNamedArgumentKind)valueReader.ReadSerializationTypeCode();
				if (kind != CustomAttributeNamedArgumentKind.Field && kind != CustomAttributeNamedArgumentKind.Property)
				{
					throw new BadImageFormatException();
				}

				ArgumentTypeInfo info = DecodeNamedArgumentType(ref valueReader);
				string name = valueReader.ReadSerializedString();
				CustomAttributeTypedArgument<TType> argument = DecodeArgument(ref valueReader, info);
				arguments.Add(new CustomAttributeNamedArgument<TType>(name, kind, argument.Type, argument.Value));
			}

			return arguments.MoveToImmutable();
		}

		private struct ArgumentTypeInfo
		{
			public TType Type;
			public TType ElementType;
			public SerializationTypeCode TypeCode;
			public SerializationTypeCode ElementTypeCode;
		}

		private ArgumentTypeInfo DecodeNamedArgumentType(ref BlobReader valueReader, bool isElementType = false)
		{
			var info = new ArgumentTypeInfo {
				TypeCode = valueReader.ReadSerializationTypeCode(),
			};

			switch (info.TypeCode)
			{
				case SerializationTypeCode.Boolean:
				case SerializationTypeCode.Byte:
				case SerializationTypeCode.Char:
				case SerializationTypeCode.Double:
				case SerializationTypeCode.Int16:
				case SerializationTypeCode.Int32:
				case SerializationTypeCode.Int64:
				case SerializationTypeCode.SByte:
				case SerializationTypeCode.Single:
				case SerializationTypeCode.String:
				case SerializationTypeCode.UInt16:
				case SerializationTypeCode.UInt32:
				case SerializationTypeCode.UInt64:
					info.Type = _provider.GetPrimitiveType((PrimitiveTypeCode)info.TypeCode);
					break;

				case SerializationTypeCode.Type:
					info.Type = _provider.GetSystemType();
					break;

				case SerializationTypeCode.TaggedObject:
					info.Type = _provider.GetPrimitiveType(PrimitiveTypeCode.Object);
					break;

				case SerializationTypeCode.SZArray:
					if (isElementType)
					{
						// jagged arrays are not allowed.
						throw new BadImageFormatException();
					}

					var elementInfo = DecodeNamedArgumentType(ref valueReader, isElementType: true);
					info.ElementType = elementInfo.Type;
					info.ElementTypeCode = elementInfo.TypeCode;
					info.Type = _provider.GetSZArrayType(info.ElementType);
					break;

				case SerializationTypeCode.Enum:
					string typeName = valueReader.ReadSerializedString();
					info.Type = _provider.GetTypeFromSerializedName(typeName);
					info.TypeCode = (SerializationTypeCode)_provider.GetUnderlyingEnumType(info.Type);
					break;

				default:
					throw new BadImageFormatException();
			}

			return info;
		}

		private CustomAttributeTypedArgument<TType> DecodeArgument(ref BlobReader valueReader, ArgumentTypeInfo info)
		{
			var outer = info;
			if (info.TypeCode == SerializationTypeCode.TaggedObject)
			{
				info = DecodeNamedArgumentType(ref valueReader);
			}

			// PERF_TODO: https://github.com/dotnet/corefx/issues/6533
			//   Cache /reuse common arguments to avoid boxing (small integers, true, false).
			object value;
			switch (info.TypeCode)
			{
				case SerializationTypeCode.Boolean:
					value = valueReader.ReadBoolean();
					break;

				case SerializationTypeCode.Byte:
					value = valueReader.ReadByte();
					break;

				case SerializationTypeCode.Char:
					value = valueReader.ReadChar();
					break;

				case SerializationTypeCode.Double:
					value = valueReader.ReadDouble();
					break;

				case SerializationTypeCode.Int16:
					value = valueReader.ReadInt16();
					break;

				case SerializationTypeCode.Int32:
					value = valueReader.ReadInt32();
					break;

				case SerializationTypeCode.Int64:
					value = valueReader.ReadInt64();
					break;

				case SerializationTypeCode.SByte:
					value = valueReader.ReadSByte();
					break;

				case SerializationTypeCode.Single:
					value = valueReader.ReadSingle();
					break;

				case SerializationTypeCode.UInt16:
					value = valueReader.ReadUInt16();
					break;

				case SerializationTypeCode.UInt32:
					value = valueReader.ReadUInt32();
					break;

				case SerializationTypeCode.UInt64:
					value = valueReader.ReadUInt64();
					break;

				case SerializationTypeCode.String:
					value = valueReader.ReadSerializedString();
					break;

				case SerializationTypeCode.Type:
					string typeName = valueReader.ReadSerializedString();
					value = _provider.GetTypeFromSerializedName(typeName);
					break;

				case SerializationTypeCode.SZArray:
					value = DecodeArrayArgument(ref valueReader, info);
					break;

				default:
					throw new BadImageFormatException();
			}

			if (_provideBoxingTypeInfo && outer.TypeCode == SerializationTypeCode.TaggedObject)
			{
				return new CustomAttributeTypedArgument<TType>(outer.Type, new CustomAttributeTypedArgument<TType>(info.Type, value));
			}

			return new CustomAttributeTypedArgument<TType>(info.Type, value);
		}

		private ImmutableArray<CustomAttributeTypedArgument<TType>>? DecodeArrayArgument(ref BlobReader blobReader, ArgumentTypeInfo info)
		{
			int count = blobReader.ReadInt32();
			if (count == -1)
			{
				return null;
			}

			if (count == 0)
			{
				return ImmutableArray<CustomAttributeTypedArgument<TType>>.Empty;
			}

			if (count < 0)
			{
				throw new BadImageFormatException();
			}

			var elementInfo = new ArgumentTypeInfo {
				Type = info.ElementType,
				TypeCode = info.ElementTypeCode,
			};

			var array = ImmutableArray.CreateBuilder<CustomAttributeTypedArgument<TType>>(count);

			for (int i = 0; i < count; i++)
			{
				array.Add(DecodeArgument(ref blobReader, elementInfo));
			}

			return array.MoveToImmutable();
		}
	}
}