﻿// Copyright (c) 2024 Siegfried Pammer
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

#nullable enable

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.Metadata
{
	/// <summary>
	/// MetadataFile is the main class the decompiler uses to represent a metadata assembly/module.
	/// Every file on disk can be loaded into a standalone MetadataFile instance.
	/// 
	/// A MetadataFile can be combined with its referenced assemblies/modules to form a type system,
	/// in that case the <see cref="MetadataModule"/> class is used instead.
	/// </summary>
	/// <remarks>
	/// In addition to wrapping a <c>System.Reflection.Metadata.MetadataReader</c>, this class
	/// contains a few decompiler-specific caches to allow efficiently constructing a type
	/// system from multiple MetadataFiles. This allows the caches to be shared across multiple
	/// decompiled type systems.
	/// </remarks>
	[DebuggerDisplay("{Kind}: {FileName}")]
	public class MetadataFile
	{
		public enum MetadataFileKind
		{
			PortableExecutable,
			ProgramDebugDatabase,
			WebCIL,
			Metadata
		}

		public string FileName { get; }
		public MetadataFileKind Kind { get; }
		public MetadataReader Metadata { get; }

		public virtual int MetadataOffset { get; }
		public virtual bool IsEmbedded { get; }
		public virtual bool IsMetadataOnly { get; } = true;

		public bool IsAssembly => Metadata.IsAssembly;

		string? name;

		public string Name {
			get {
				var value = LazyInit.VolatileRead(ref name);
				if (value == null)
				{
					var metadata = Metadata;
					value = metadata.IsAssembly
						? metadata.GetString(metadata.GetAssemblyDefinition().Name)
						: metadata.GetString(metadata.GetModuleDefinition().Name);
					value = LazyInit.GetOrSet(ref name, value);
				}
				return value;
			}
		}

		string? fullName;

		public string FullName {
			get {
				var value = LazyInit.VolatileRead(ref fullName);
				if (value == null)
				{
					var metadata = Metadata;
					value = metadata.IsAssembly ? metadata.GetFullAssemblyName() : Name;
					value = LazyInit.GetOrSet(ref fullName, value);
				}
				return value;
			}
		}

		public TargetRuntime GetRuntime()
		{
			string version = Metadata.MetadataVersion;
			if (version == null || version.Length <= 1)
				return TargetRuntime.Unknown;
			switch (version[1])
			{
				case '1':
					if (version.Length <= 3)
						return TargetRuntime.Unknown;
					if (version[3] == 1)
						return TargetRuntime.Net_1_0;
					else
						return TargetRuntime.Net_1_1;
				case '2':
					return TargetRuntime.Net_2_0;
				case '4':
					return TargetRuntime.Net_4_0;
				default:
					return TargetRuntime.Unknown;
			}
		}

		ImmutableArray<AssemblyReference> assemblyReferences;
		public ImmutableArray<AssemblyReference> AssemblyReferences {
			get {
				var value = assemblyReferences;
				if (value.IsDefault)
				{
					value = Metadata.AssemblyReferences.Select(r => new AssemblyReference(this.Metadata, r)).ToImmutableArray();
					assemblyReferences = value;
				}
				return value;
			}
		}

		ImmutableArray<ModuleReferenceMetadata> moduleReferences;
		public ImmutableArray<ModuleReferenceMetadata> ModuleReferences {
			get {
				var value = moduleReferences;
				if (value.IsDefault)
				{
					value = Metadata.GetModuleReferences()
							.Select(m => new ModuleReferenceMetadata(this.Metadata, m))
							.ToImmutableArray();

					moduleReferences = value;
				}
				return value;
			}
		}

		public ImmutableArray<Resource> Resources => GetResources().ToImmutableArray();

		IEnumerable<Resource> GetResources()
		{
			var metadata = Metadata;
			foreach (var h in metadata.ManifestResources)
			{
				yield return new MetadataResource(this, h);
			}
		}

		Dictionary<TopLevelTypeName, TypeDefinitionHandle>? typeLookup;

		/// <summary>
		/// Finds the top-level-type with the specified name.
		/// </summary>
		public TypeDefinitionHandle GetTypeDefinition(TopLevelTypeName typeName)
		{
			var lookup = LazyInit.VolatileRead(ref typeLookup);
			if (lookup == null)
			{
				lookup = new Dictionary<TopLevelTypeName, TypeDefinitionHandle>();
				foreach (var handle in Metadata.TypeDefinitions)
				{
					var td = Metadata.GetTypeDefinition(handle);
					if (!td.GetDeclaringType().IsNil)
					{
						continue; // nested type
					}
					var nsHandle = td.Namespace;
					string ns = nsHandle.IsNil ? string.Empty : Metadata.GetString(nsHandle);
					string name = ReflectionHelper.SplitTypeParameterCountFromReflectionName(Metadata.GetString(td.Name), out int typeParameterCount);
					lookup[new TopLevelTypeName(ns, name, typeParameterCount)] = handle;
				}
				lookup = LazyInit.GetOrSet(ref typeLookup, lookup);
			}
			if (lookup.TryGetValue(typeName, out var resultHandle))
				return resultHandle;
			else
				return default;
		}

		Dictionary<FullTypeName, ExportedTypeHandle>? typeForwarderLookup;

		/// <summary>
		/// Finds the type forwarder with the specified name.
		/// </summary>
		public ExportedTypeHandle GetTypeForwarder(FullTypeName typeName)
		{
			var lookup = LazyInit.VolatileRead(ref typeForwarderLookup);
			if (lookup == null)
			{
				lookup = new Dictionary<FullTypeName, ExportedTypeHandle>();
				foreach (var handle in Metadata.ExportedTypes)
				{
					var td = Metadata.GetExportedType(handle);
					lookup[td.GetFullTypeName(Metadata)] = handle;
				}
				lookup = LazyInit.GetOrSet(ref typeForwarderLookup, lookup);
			}
			if (lookup.TryGetValue(typeName, out var resultHandle))
				return resultHandle;
			else
				return default;
		}

		MethodSemanticsLookup? methodSemanticsLookup;

		internal MethodSemanticsLookup MethodSemanticsLookup {
			get {
				var r = LazyInit.VolatileRead(ref methodSemanticsLookup);
				if (r != null)
					return r;
				else
					return LazyInit.GetOrSet(ref methodSemanticsLookup, new MethodSemanticsLookup(Metadata));
			}
		}

		public MetadataFile(MetadataFileKind kind, string fileName, MetadataReaderProvider metadata, MetadataReaderOptions metadataOptions = MetadataReaderOptions.Default, int metadataOffset = 0, bool isEmbedded = false)
		{
			this.Kind = kind;
			this.FileName = fileName;
			this.Metadata = metadata.GetMetadataReader(metadataOptions);
			this.MetadataOffset = metadataOffset;
			this.IsEmbedded = isEmbedded;
		}

		private protected MetadataFile(MetadataFileKind kind, string fileName, PEReader reader, MetadataReaderOptions metadataOptions = MetadataReaderOptions.Default)
		{
			this.Kind = kind;
			this.FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
			_ = reader ?? throw new ArgumentNullException(nameof(reader));
			if (!reader.HasMetadata)
				throw new MetadataFileNotSupportedException("PE file does not contain any managed metadata.");
			this.Metadata = reader.GetMetadataReader(metadataOptions);
		}

		public virtual MethodBodyBlock GetMethodBody(int rva)
		{
			throw new BadImageFormatException("This metadata file does not contain method bodies.");
		}

		public virtual SectionData GetSectionData(int rva)
		{
			throw new BadImageFormatException("This metadata file does not support sections.");
		}

		public virtual int GetContainingSectionIndex(int rva)
		{
			throw new BadImageFormatException("This metadata file does not support sections.");
		}

		public virtual ImmutableArray<SectionHeader> SectionHeaders => throw new BadImageFormatException("This metadata file does not support sections.");

		/// <summary>
		/// Gets the CLI header or null if the image does not have one.
		/// </summary>
		public virtual CorHeader? CorHeader => null;

		public IModuleReference WithOptions(TypeSystemOptions options)
		{
			return new MetadataFileWithOptions(this, options);
		}

		private class MetadataFileWithOptions : IModuleReference
		{
			readonly MetadataFile peFile;
			readonly TypeSystemOptions options;

			public MetadataFileWithOptions(MetadataFile peFile, TypeSystemOptions options)
			{
				this.peFile = peFile;
				this.options = options;
			}

			IModule IModuleReference.Resolve(ITypeResolveContext context)
			{
				return new MetadataModule(context.Compilation, peFile, options);
			}
		}
	}

	/// <summary>
	/// Abstraction over PEMemoryBlock
	/// </summary>
	public readonly unsafe struct SectionData
	{
		public byte* Pointer { get; }
		public int Length { get; }

		public SectionData(PEMemoryBlock block)
		{
			Pointer = block.Pointer;
			Length = block.Length;
		}

		public SectionData(byte* startPointer, int length)
		{
			Pointer = startPointer;
			Length = length;
		}

		public BlobReader GetReader()
		{
			return new BlobReader(Pointer, Length);
		}

		internal BlobReader GetReader(int offset, int size)
		{
			return new BlobReader(Pointer + offset, size);
		}
	}

	public struct SectionHeader
	{
		public string Name;
		public uint VirtualSize;
		public uint VirtualAddress;
		public uint RawDataSize;
		public uint RawDataPtr;
	}
}
