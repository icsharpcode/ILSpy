// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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
using System.Reflection;
using System.Reflection.Metadata;

namespace ICSharpCode.ILSpy.Metadata
{
	/// <summary>
	/// Classifies a column for the DataGrid view (Phase 2+). Heap-offset cells render with a
	/// hex-padded width and resolve their string content via the relevant heap reader; token
	/// cells render as a hyperlink that navigates to the target table row. The text path
	/// (Phase 1) ignores everything except <c>Format</c>.
	/// </summary>
	public enum ColumnKind
	{
		Other,
		HeapOffset,
		Token,
	}

	/// <summary>
	/// Annotates a row-shape property with rendering hints. <c>Format</c> is consumed by the
	/// text writer; <c>Kind</c> and <c>LinkToTable</c> drive the DataGrid column factory in
	/// Phase 2+ and are inert in Phase 1.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class ColumnInfoAttribute : Attribute
	{
		public string Format { get; }
		public ColumnKind Kind { get; set; }
		public bool LinkToTable { get; set; }

		public ColumnInfoAttribute(string format)
		{
			Format = format;
		}
	}

	/// <summary>
	/// One row in a PE-header / metadata-table dump. <c>Value</c> is rendered hex-formatted
	/// at <c>Size * 2</c> digits when numeric; <c>RowDetails</c> carries optional flag-bit
	/// breakdowns that the DataGrid view (Phase 2) uses to populate row details. The text
	/// path (Phase 1) ignores <c>RowDetails</c>.
	/// </summary>
	public sealed class Entry
	{
		public string Member { get; }
		public int Offset { get; }
		public int Size { get; }
		public object? Value { get; }
		public string Meaning { get; }
		public IList<BitEntry>? RowDetails { get; }

		public Entry(int offset, object? value, int size, string member, string meaning, IList<BitEntry>? rowDetails = null)
		{
			Member = member;
			Offset = offset;
			Size = size;
			Value = value;
			Meaning = meaning;
			RowDetails = rowDetails;
		}
	}

	/// <summary>One bit (or bit-group) entry inside a flags-Entry's row-details strip.</summary>
	public sealed class BitEntry
	{
		public bool Value { get; }
		public string Meaning { get; }

		public BitEntry(bool value, string meaning)
		{
			Value = value;
			Meaning = meaning;
		}
	}

	/// <summary>
	/// Bitmask covering each CLI metadata table. Used as the second argument to
	/// <see cref="MetadataReaderHelpers.ComputeCodedTokenSize"/> when computing how many bytes
	/// a coded-token column occupies in a given metadata blob — same encoding as the WPF
	/// host's <c>TableMask</c>.
	/// </summary>
	[Flags]
	public enum TableMask : ulong
	{
		Module = 0x1,
		TypeRef = 0x2,
		TypeDef = 0x4,
		FieldPtr = 0x8,
		Field = 0x10,
		MethodPtr = 0x20,
		MethodDef = 0x40,
		ParamPtr = 0x80,
		Param = 0x100,
		InterfaceImpl = 0x200,
		MemberRef = 0x400,
		Constant = 0x800,
		CustomAttribute = 0x1000,
		FieldMarshal = 0x2000,
		DeclSecurity = 0x4000,
		ClassLayout = 0x8000,
		FieldLayout = 0x10000,
		StandAloneSig = 0x20000,
		EventMap = 0x40000,
		EventPtr = 0x80000,
		Event = 0x100000,
		PropertyMap = 0x200000,
		PropertyPtr = 0x400000,
		Property = 0x800000,
		MethodSemantics = 0x1000000,
		MethodImpl = 0x2000000,
		ModuleRef = 0x4000000,
		TypeSpec = 0x8000000,
		ImplMap = 0x10000000,
		FieldRva = 0x20000000,
		EnCLog = 0x40000000,
		EnCMap = 0x80000000,
		Assembly = 0x100000000,
		AssemblyRef = 0x800000000,
		File = 0x4000000000,
		ExportedType = 0x8000000000,
		ManifestResource = 0x10000000000,
		NestedClass = 0x20000000000,
		GenericParam = 0x40000000000,
		MethodSpec = 0x80000000000,
		GenericParamConstraint = 0x100000000000,
		Document = 0x1000000000000,
		MethodDebugInformation = 0x2000000000000,
		LocalScope = 0x4000000000000,
		LocalVariable = 0x8000000000000,
		LocalConstant = 0x10000000000000,
		ImportScope = 0x20000000000000,
		StateMachineMethod = 0x40000000000000,
		CustomDebugInformation = 0x80000000000000,
	}

	/// <summary>
	/// Reflection-backed shims onto <c>System.Reflection.Metadata</c> internals — needed to
	/// decode coded-token columns and compute their on-disk widths. The corresponding public
	/// APIs do not exist; the WPF host carries the same reflection infrastructure. Pre-existing
	/// risk: if a future runtime renames <c>TypeDefOrRefTag.ConvertToHandle</c> or the
	/// <c>TableRowCounts</c> field on <c>MetadataReader</c>, these methods will throw and
	/// the affected metadata-table viewers will break.
	/// </summary>
	public static class MetadataReaderHelpers
	{
		static readonly FieldInfo? rowCountsField;
		static readonly MethodInfo? computeCodedTokenSizeMethod;
		static readonly MethodInfo? typeDefOrRefConvert;
		static readonly MethodInfo? hasFieldMarshalConvert;
		static readonly MethodInfo? memberForwardedConvert;

		static MetadataReaderHelpers()
		{
			rowCountsField = typeof(MetadataReader)
				.GetField("TableRowCounts", BindingFlags.NonPublic | BindingFlags.Instance);
			computeCodedTokenSizeMethod = typeof(MetadataReader)
				.GetMethod("ComputeCodedTokenSize", BindingFlags.Instance | BindingFlags.NonPublic);
			var asm = typeof(TypeDefinitionHandle).Assembly;
			typeDefOrRefConvert = asm.GetType("System.Reflection.Metadata.Ecma335.TypeDefOrRefTag")
				?.GetMethod("ConvertToHandle", BindingFlags.Static | BindingFlags.NonPublic);
			hasFieldMarshalConvert = asm.GetType("System.Reflection.Metadata.Ecma335.HasFieldMarshalTag")
				?.GetMethod("ConvertToHandle", BindingFlags.Static | BindingFlags.NonPublic);
			memberForwardedConvert = asm.GetType("System.Reflection.Metadata.Ecma335.MemberForwardedTag")
				?.GetMethod("ConvertToHandle", BindingFlags.Static | BindingFlags.NonPublic);
		}

		public static EntityHandle FromTypeDefOrRefTag(uint tag)
			=> (EntityHandle)typeDefOrRefConvert!.Invoke(null, [tag])!;

		public static EntityHandle FromHasFieldMarshalTag(uint tag)
			=> (EntityHandle)hasFieldMarshalConvert!.Invoke(null, [tag])!;

		public static EntityHandle FromMemberForwardedTag(uint tag)
			=> (EntityHandle)memberForwardedConvert!.Invoke(null, [tag])!;

		public static int ComputeCodedTokenSize(this MetadataReader metadata, int largeRowSize, TableMask mask)
			=> (int)computeCodedTokenSizeMethod!.Invoke(metadata, [largeRowSize, rowCountsField!.GetValue(metadata)!, (ulong)mask])!;

		/// <summary>
		/// Reads a null-terminated UTF-8 string and advances the reader past the terminator.
		/// Throws <see cref="BadImageFormatException"/> when no terminator remains, so callers
		/// parsing structured blobs can degrade to a hex dump on malformed input.
		/// </summary>
		public static string ReadUTF8StringNullTerminated(this ref BlobReader reader)
		{
			int length = reader.IndexOf(0);
			if (length < 0)
				throw new BadImageFormatException("Missing null terminator in blob.");
			string s = reader.ReadUTF8(length);
			reader.ReadByte();
			return s;
		}
	}
}
