// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Immutable;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;

namespace ICSharpCode.Decompiler
{
	/// <summary>
	/// Class for dealing with .NET 5 single-file bundles.
	/// 
	/// Based on code from Microsoft.NET.HostModel.
	/// </summary>
	public static class SingleFileBundle
	{
		/// <summary>
		/// Check if the memory-mapped data is a single-file bundle
		/// </summary>
		public static unsafe bool IsBundle(MemoryMappedViewAccessor view, out long bundleHeaderOffset)
		{
			var buffer = view.SafeMemoryMappedViewHandle;
			byte* ptr = null;
			buffer.AcquirePointer(ref ptr);
			try
			{
				return IsBundle(ptr, checked((long)buffer.ByteLength), out bundleHeaderOffset);
			}
			finally
			{
				buffer.ReleasePointer();
			}
		}

		public static unsafe bool IsBundle(byte* data, long size, out long bundleHeaderOffset)
		{
			ReadOnlySpan<byte> bundleSignature = new byte[] {
				// 32 bytes represent the bundle signature: SHA-256 for ".net core bundle"
				0x8b, 0x12, 0x02, 0xb9, 0x6a, 0x61, 0x20, 0x38,
				0x72, 0x7b, 0x93, 0x02, 0x14, 0xd7, 0xa0, 0x32,
				0x13, 0xf5, 0xb9, 0xe6, 0xef, 0xae, 0x33, 0x18,
				0xee, 0x3b, 0x2d, 0xce, 0x24, 0xb3, 0x6a, 0xae
			};

			byte* end = data + (size - bundleSignature.Length);
			for (byte* ptr = data; ptr < end; ptr++)
			{
				if (*ptr == 0x8b && bundleSignature.SequenceEqual(new ReadOnlySpan<byte>(ptr, bundleSignature.Length)))
				{
					bundleHeaderOffset = *(long*)(ptr - sizeof(long));
					return true;
				}
			}

			bundleHeaderOffset = 0;
			return false;
		}

		public struct Header
		{
			public uint MajorVersion;
			public uint MinorVersion;
			public int FileCount;
			public string BundleID;

			// Fields introduced with v2:
			public long DepsJsonOffset;
			public long DepsJsonSize;
			public long RuntimeConfigJsonOffset;
			public long RuntimeConfigJsonSize;
			public ulong Flags;

			public ImmutableArray<Entry> Entries;
		}

		/// <summary>
		/// FileType: Identifies the type of file embedded into the bundle.
		///
		/// The bundler differentiates a few kinds of files via the manifest,
		/// with respect to the way in which they'll be used by the runtime.
		/// </summary>
		public enum FileType : byte
		{
			Unknown,           // Type not determined.
			Assembly,          // IL and R2R Assemblies
			NativeBinary,      // NativeBinaries
			DepsJson,          // .deps.json configuration file
			RuntimeConfigJson, // .runtimeconfig.json configuration file
			Symbols            // PDB Files
		};

		public struct Entry
		{
			public long Offset;
			public long Size;
			public FileType Type;
			public string RelativePath; // Path of an embedded file, relative to the Bundle source-directory.
		}

		static UnmanagedMemoryStream AsStream(MemoryMappedViewAccessor view)
		{
			long size = checked((long)view.SafeMemoryMappedViewHandle.ByteLength);
			return new UnmanagedMemoryStream(view.SafeMemoryMappedViewHandle, 0, size);
		}

		/// <summary>
		/// Reads the manifest header from the memory mapping.
		/// </summary>
		public static Header ReadManifest(MemoryMappedViewAccessor view, long bundleHeaderOffset)
		{
			using var stream = AsStream(view);
			stream.Seek(bundleHeaderOffset, SeekOrigin.Begin);
			return ReadManifest(stream);
		}

		/// <summary>
		/// Reads the manifest header from the stream.
		/// </summary>
		public static Header ReadManifest(Stream stream)
		{
			var header = new Header();
			using var reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);
			header.MajorVersion = reader.ReadUInt32();
			header.MinorVersion = reader.ReadUInt32();
			if (header.MajorVersion < 1 || header.MajorVersion > 2)
			{
				throw new InvalidDataException($"Unsupported manifest version: {header.MajorVersion}.{header.MinorVersion}");
			}
			header.FileCount = reader.ReadInt32();
			header.BundleID = reader.ReadString();
			if (header.MajorVersion >= 2)
			{
				header.DepsJsonOffset = reader.ReadInt64();
				header.DepsJsonSize = reader.ReadInt64();
				header.RuntimeConfigJsonOffset = reader.ReadInt64();
				header.RuntimeConfigJsonSize = reader.ReadInt64();
				header.Flags = reader.ReadUInt64();
			}
			var entries = ImmutableArray.CreateBuilder<Entry>(header.FileCount);
			for (int i = 0; i < header.FileCount; i++)
			{
				entries.Add(ReadEntry(reader));
			}
			header.Entries = entries.MoveToImmutable();
			return header;
		}

		private static Entry ReadEntry(BinaryReader reader)
		{
			Entry entry;
			entry.Offset = reader.ReadInt64();
			entry.Size = reader.ReadInt64();
			entry.Type = (FileType)reader.ReadByte();
			entry.RelativePath = reader.ReadString();
			return entry;
		}
	}
}
