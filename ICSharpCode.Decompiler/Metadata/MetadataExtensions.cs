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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.Decompiler.Util;

using SRM = System.Reflection.Metadata;

namespace ICSharpCode.Decompiler.Metadata
{
	public static class MetadataExtensions
	{
		static string CalculatePublicKeyToken(BlobHandle blob, MetadataReader reader)
		{
			// Calculate public key token:
			// 1. hash the public key (the strong-name format mandates SHA-1; a managed
			// implementation is used so this works under restrictive crypto policies).
			byte[] publicKeyTokenBytes = new byte[20];
			Sha1ForNonSecretPurposes.HashData(reader.GetBlobBytes(blob), publicKeyTokenBytes);

			// 2. take the last 8 bytes
			// 3. according to Cecil we need to reverse them, other sources did not mention this.
			return publicKeyTokenBytes.TakeLast(8).Reverse().ToHexString(8);
		}

		public static string GetPublicKeyToken(this MetadataReader reader)
		{
			if (!reader.IsAssembly)
				return string.Empty;
			var asm = reader.GetAssemblyDefinition();
			string publicKey = "null";
			if (!asm.PublicKey.IsNil)
			{
				// AssemblyFlags.PublicKey does not apply to assembly definitions
				publicKey = CalculatePublicKeyToken(asm.PublicKey, reader);
			}
			return publicKey;
		}

		public static string GetFullAssemblyName(this MetadataReader reader)
		{
			if (!reader.IsAssembly)
				return string.Empty;
			var asm = reader.GetAssemblyDefinition();
			string publicKey = reader.GetPublicKeyToken();
			return $"{reader.GetString(asm.Name)}, " +
				$"Version={asm.Version}, " +
				$"Culture={(asm.Culture.IsNil ? "neutral" : reader.GetString(asm.Culture))}, " +
				$"PublicKeyToken={publicKey}";
		}

		public static bool TryGetFullAssemblyName(this MetadataReader reader, out string assemblyName)
		{
			try
			{
				assemblyName = GetFullAssemblyName(reader);
				return true;
			}
			catch (BadImageFormatException)
			{
				assemblyName = null;
				return false;
			}
		}

		public static string GetFullAssemblyName(this SRM.AssemblyReference reference, MetadataReader reader)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(reader.GetString(reference.Name));
			builder.Append(", Version=");
			builder.Append(reference.Version);
			builder.Append(", Culture=");
			if (reference.Culture.IsNil)
			{
				builder.Append("neutral");
			}
			else
			{
				builder.Append(reader.GetString(reference.Culture));
			}

			if (reference.PublicKeyOrToken.IsNil)
			{
				builder.Append(", PublicKeyToken=null");
			}
			else if ((reference.Flags & AssemblyFlags.PublicKey) != 0)
			{
				builder.Append(", PublicKeyToken=");
				builder.Append(CalculatePublicKeyToken(reference.PublicKeyOrToken, reader));
			}
			else
			{
				builder.Append(", PublicKeyToken=");
				builder.AppendHexString(reader.GetBlobReader(reference.PublicKeyOrToken));
			}
			if ((reference.Flags & AssemblyFlags.Retargetable) != 0)
			{
				builder.Append(", Retargetable=true");
			}
			return builder.ToString();
		}

		public static bool TryGetFullAssemblyName(this SRM.AssemblyReference reference, MetadataReader reader, out string assemblyName)
		{
			try
			{
				assemblyName = GetFullAssemblyName(reference, reader);
				return true;
			}
			catch (BadImageFormatException)
			{
				assemblyName = null;
				return false;
			}
		}

		/// <summary>
		/// True when <paramref name="reference"/> names the assembly defined by <paramref name="reader"/>:
		/// same simple name (ignoring case), culture and public key token. The version is not compared,
		/// since binding redirects and unified frameworks satisfy references with other versions.
		/// </summary>
		public static bool IsReferenceTo(this IAssemblyReference reference, MetadataReader reader)
		{
			if (reference == null)
				throw new ArgumentNullException(nameof(reference));
			if (reader == null)
				throw new ArgumentNullException(nameof(reader));
			if (!reader.IsAssembly)
				return false;
			var asm = reader.GetAssemblyDefinition();
			if (!string.Equals(reference.Name, reader.GetString(asm.Name), StringComparison.OrdinalIgnoreCase))
				return false;
			if (!string.Equals(NormalizeCulture(reference.Culture), NormalizeCulture(reader.GetString(asm.Culture)), StringComparison.OrdinalIgnoreCase))
				return false;
			var token = reference.PublicKeyToken is { Length: > 0 } bytes ? bytes.ToHexString(bytes.Length) : "null";
			return string.Equals(token, reader.GetPublicKeyToken(), StringComparison.OrdinalIgnoreCase);

			static string NormalizeCulture(string culture)
				=> string.IsNullOrEmpty(culture) || culture == "neutral" ? string.Empty : culture;
		}

		/// <summary>
		/// True when <paramref name="reference"/> asks for a different version than the assembly
		/// defined by <paramref name="reader"/>. Unknown versions never count as a mismatch.
		/// </summary>
		public static bool IsVersionMismatch(this IAssemblyReference reference, MetadataReader reader)
		{
			if (reference == null)
				throw new ArgumentNullException(nameof(reference));
			if (reader == null)
				throw new ArgumentNullException(nameof(reader));
			return reference.Version != null && reader.IsAssembly && reference.Version != reader.GetAssemblyDefinition().Version;
		}

		public static string ToHexString(this IEnumerable<byte> bytes, int estimatedLength)
		{
			if (bytes == null)
				throw new ArgumentNullException(nameof(bytes));

			StringBuilder sb = new StringBuilder(estimatedLength * 2);
			foreach (var b in bytes)
				sb.AppendFormat("{0:x2}", b);
			return sb.ToString();
		}

		public static void AppendHexString(this StringBuilder builder, BlobReader reader)
		{
			for (int i = 0; i < reader.Length; i++)
			{
				builder.AppendFormat("{0:x2}", reader.ReadByte());
			}
		}

		public static string ToHexString(this BlobReader reader)
		{
			StringBuilder sb = new StringBuilder(reader.Length * 3);
			for (int i = 0; i < reader.Length; i++)
			{
				if (i == 0)
					sb.AppendFormat("{0:X2}", reader.ReadByte());
				else
					sb.AppendFormat("-{0:X2}", reader.ReadByte());
			}
			return sb.ToString();
		}

		public static IEnumerable<TypeDefinitionHandle> GetTopLevelTypeDefinitions(this MetadataReader reader)
		{
			foreach (var handle in reader.TypeDefinitions)
			{
				var td = reader.GetTypeDefinition(handle);
				if (td.GetDeclaringType().IsNil)
					yield return handle;
			}
		}

		public static string ToILNameString(this FullTypeName typeName, bool omitGenerics = false)
		{
			string name;
			if (typeName.IsNested)
			{
				name = typeName.Name;
				if (!omitGenerics)
				{
					int localTypeParameterCount = typeName.GetNestedTypeAdditionalTypeParameterCount(typeName.NestingLevel - 1);
					if (localTypeParameterCount > 0)
						name += "`" + localTypeParameterCount;
				}
				name = Disassembler.DisassemblerHelpers.Escape(name);
				return $"{typeName.GetDeclaringType().ToILNameString(omitGenerics)}/{name}";
			}
			if (!string.IsNullOrEmpty(typeName.TopLevelTypeName.Namespace))
			{
				name = $"{typeName.TopLevelTypeName.Namespace}.{typeName.Name}";
				if (!omitGenerics && typeName.TypeParameterCount > 0)
					name += "`" + typeName.TypeParameterCount;
			}
			else
			{
				name = typeName.Name;
				if (!omitGenerics && typeName.TypeParameterCount > 0)
					name += "`" + typeName.TypeParameterCount;
			}
			return Disassembler.DisassemblerHelpers.Escape(name);
		}

		internal static readonly TypeProvider minimalCorlibTypeProvider =
			new TypeProvider(new SimpleCompilation(MinimalCorlib.Instance));

		/// <summary>
		/// An attribute type provider that can be used to decode attribute signatures
		/// that only mention built-in types.
		/// </summary>
		public static ICustomAttributeTypeProvider<IType> MinimalAttributeTypeProvider {
			get => minimalCorlibTypeProvider;
		}

		public static ISignatureTypeProvider<IType, TypeSystem.GenericContext> MinimalSignatureTypeProvider {
			get => minimalCorlibTypeProvider;
		}

		/// <summary>
		/// Converts <see cref="KnownTypeCode"/> to <see cref="PrimitiveTypeCode"/>.
		/// Returns 0 for known types that are not primitive types (such as <see cref="Span{T}"/>).
		/// </summary>
		public static PrimitiveTypeCode ToPrimitiveTypeCode(this KnownTypeCode typeCode)
		{
			switch (typeCode)
			{
				case KnownTypeCode.Object:
					return PrimitiveTypeCode.Object;
				case KnownTypeCode.Boolean:
					return PrimitiveTypeCode.Boolean;
				case KnownTypeCode.Char:
					return PrimitiveTypeCode.Char;
				case KnownTypeCode.SByte:
					return PrimitiveTypeCode.SByte;
				case KnownTypeCode.Byte:
					return PrimitiveTypeCode.Byte;
				case KnownTypeCode.Int16:
					return PrimitiveTypeCode.Int16;
				case KnownTypeCode.UInt16:
					return PrimitiveTypeCode.UInt16;
				case KnownTypeCode.Int32:
					return PrimitiveTypeCode.Int32;
				case KnownTypeCode.UInt32:
					return PrimitiveTypeCode.UInt32;
				case KnownTypeCode.Int64:
					return PrimitiveTypeCode.Int64;
				case KnownTypeCode.UInt64:
					return PrimitiveTypeCode.UInt64;
				case KnownTypeCode.Single:
					return PrimitiveTypeCode.Single;
				case KnownTypeCode.Double:
					return PrimitiveTypeCode.Double;
				case KnownTypeCode.String:
					return PrimitiveTypeCode.String;
				case KnownTypeCode.Void:
					return PrimitiveTypeCode.Void;
				case KnownTypeCode.TypedReference:
					return PrimitiveTypeCode.TypedReference;
				case KnownTypeCode.IntPtr:
					return PrimitiveTypeCode.IntPtr;
				case KnownTypeCode.UIntPtr:
					return PrimitiveTypeCode.UIntPtr;
				default:
					return 0;
			}
		}

		public static KnownTypeCode ToKnownTypeCode(this PrimitiveTypeCode typeCode)
		{
			switch (typeCode)
			{
				case PrimitiveTypeCode.Boolean:
					return KnownTypeCode.Boolean;
				case PrimitiveTypeCode.Byte:
					return KnownTypeCode.Byte;
				case PrimitiveTypeCode.SByte:
					return KnownTypeCode.SByte;
				case PrimitiveTypeCode.Char:
					return KnownTypeCode.Char;
				case PrimitiveTypeCode.Int16:
					return KnownTypeCode.Int16;
				case PrimitiveTypeCode.UInt16:
					return KnownTypeCode.UInt16;
				case PrimitiveTypeCode.Int32:
					return KnownTypeCode.Int32;
				case PrimitiveTypeCode.UInt32:
					return KnownTypeCode.UInt32;
				case PrimitiveTypeCode.Int64:
					return KnownTypeCode.Int64;
				case PrimitiveTypeCode.UInt64:
					return KnownTypeCode.UInt64;
				case PrimitiveTypeCode.Single:
					return KnownTypeCode.Single;
				case PrimitiveTypeCode.Double:
					return KnownTypeCode.Double;
				case PrimitiveTypeCode.IntPtr:
					return KnownTypeCode.IntPtr;
				case PrimitiveTypeCode.UIntPtr:
					return KnownTypeCode.UIntPtr;
				case PrimitiveTypeCode.Object:
					return KnownTypeCode.Object;
				case PrimitiveTypeCode.String:
					return KnownTypeCode.String;
				case PrimitiveTypeCode.TypedReference:
					return KnownTypeCode.TypedReference;
				case PrimitiveTypeCode.Void:
					return KnownTypeCode.Void;
				default:
					return KnownTypeCode.None;
			}
		}

		public static IEnumerable<ModuleReferenceHandle> GetModuleReferences(this MetadataReader metadata)
		{
			var rowCount = metadata.GetTableRowCount(TableIndex.ModuleRef);
			for (int row = 1; row <= rowCount; row++)
			{
				yield return MetadataTokens.ModuleReferenceHandle(row);
			}
		}

		public static IEnumerable<TypeSpecificationHandle> GetTypeSpecifications(this MetadataReader metadata)
		{
			var rowCount = metadata.GetTableRowCount(TableIndex.TypeSpec);
			for (int row = 1; row <= rowCount; row++)
			{
				yield return MetadataTokens.TypeSpecificationHandle(row);
			}
		}

		public static IEnumerable<MethodSpecificationHandle> GetMethodSpecifications(this MetadataReader metadata)
		{
			var rowCount = metadata.GetTableRowCount(TableIndex.MethodSpec);
			for (int row = 1; row <= rowCount; row++)
			{
				yield return MetadataTokens.MethodSpecificationHandle(row);
			}
		}

		public static uint ReadULEB128(this BinaryReader reader)
		{
			uint val = 0;
			int shift = 0;
			while (true)
			{
				byte b = reader.ReadByte();
				val |= (b & 0b0111_1111u) << shift;
				if ((b & 0b1000_0000) == 0)
					break;
				shift += 7;
				if (shift >= 35)
					throw new OverflowException();
			}
			return val;
		}

	}
}
