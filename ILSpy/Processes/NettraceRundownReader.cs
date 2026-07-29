// Copyright (c) 2026 Christoph Wille
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
using System.Text;

namespace ICSharpCode.ILSpy.Processes
{
	/// <summary>
	/// Extracts the loaded-module list from the nettrace stream of an EventPipe rundown
	/// session (container format: microsoft/perfview, src/TraceEvent/EventPipe).
	/// Deliberately not a general nettrace library: it walks the FastSerialization object
	/// stream far enough to find loader events, decodes those, and skips everything else -
	/// stacks, sequence points, and every other provider's payloads are stepped over by
	/// size without being interpreted.
	/// </summary>
	internal static class NettraceRundownReader
	{
		// FastSerialization tags.
		const byte TagNullReference = 1;
		const byte TagBeginObject = 4;
		const byte TagBeginPrivateObject = 5;
		const byte TagEndObject = 6;

		// The container version the runtime serves for CollectTracing2 with format=NetTrace.
		// V5 only adds tags a V4 reader may ignore; anything beyond that is a format the
		// event and block layouts below are not written for.
		const int MaxSupportedTraceVersion = 5;

		const int CompressedFlagMetadataId = 0x01;
		const int CompressedFlagCaptureThreadAndSequence = 0x02;
		const int CompressedFlagThreadId = 0x04;
		const int CompressedFlagStackId = 0x08;
		const int CompressedFlagActivityId = 0x10;
		const int CompressedFlagRelatedActivityId = 0x20;
		const int CompressedFlagDataLength = 0x80;

		// The CLR's own providers are manifest-based, so their events arrive with an empty
		// name in the stream and must be recognized by provider plus numeric id. The ids
		// overlap between the two providers and mean different things in each, so both parts
		// of the pair matter. Rundown emits the "DC" (data collection) variants at session
		// stop; the runtime provider reports what loads while the session is open.
		const string RundownProviderName = "Microsoft-Windows-DotNETRuntimeRundown";
		const string RuntimeProviderName = "Microsoft-Windows-DotNETRuntime";

		const int RundownModuleDCStart = 153;
		const int RundownModuleDCStop = 154;
		const int RundownAssemblyDCStart = 155;
		const int RundownAssemblyDCStop = 156;
		const int RuntimeModuleLoad = 152;
		const int RuntimeAssemblyLoad = 154;

		sealed record EventMetadata(string ProviderName, int EventId, int Version);

		/// <summary>
		/// A module as reported by the runtime, before assembly names are resolved.
		/// </summary>
		sealed record ModuleRecord(long AssemblyId, string? IlPath, string? NativePath);

		public static IReadOnlyList<ProcessModuleInfo> ReadModules(Stream stream)
		{
			using var reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);
			ReadStreamHeader(reader);

			var metadata = new Dictionary<int, EventMetadata>();
			var modules = new List<ModuleRecord>();
			var assemblyNames = new Dictionary<long, string>();

			while (true)
			{
				byte tag = reader.ReadByte();
				if (tag == TagNullReference)
					break; // End of the object stream.
				if (tag is not (TagBeginObject or TagBeginPrivateObject))
					throw new InvalidDataException($"Unexpected FastSerialization tag 0x{tag:X2} in the Nettrace stream.");

				string typeName = ReadTypeName(reader, out int version);
				switch (typeName)
				{
					case "Trace":
						if (version > MaxSupportedTraceVersion)
							throw new InvalidDataException(
								$"Nettrace version {version} is newer than this reader supports.");
						SkipTraceObject(reader);
						break;
					case "MetadataBlock":
						ReadBlock(reader, (payload, id, _) => ReadMetadataEvent(payload, id, metadata));
						break;
					case "EventBlock":
						ReadBlock(reader, (payload, id, _) => ReadEvent(payload, id, metadata, modules, assemblyNames));
						break;
					default:
						// StackBlock, SPBlock and any block type added later: the block is
						// self-describing in length, so it can be stepped over wholesale.
						SkipBlock(reader);
						break;
				}
				ExpectTag(reader, TagEndObject);
			}

			return BuildModuleList(modules, assemblyNames);
		}

		static void ReadStreamHeader(BinaryReader reader)
		{
			byte[] magic;
			try
			{
				magic = reader.ReadBytes(8);
			}
			catch (EndOfStreamException)
			{
				magic = Array.Empty<byte>();
			}
			if (magic.Length < 8 || Encoding.ASCII.GetString(magic) != "Nettrace")
				throw new InvalidDataException("The stream does not start with the Nettrace magic.");

			string serializer = ReadLengthPrefixedAsciiString(reader);
			if (!serializer.StartsWith("!FastSerialization", StringComparison.Ordinal))
				throw new InvalidDataException($"Unexpected Nettrace serializer '{serializer}'.");
		}

		static string ReadTypeName(BinaryReader reader, out int version)
		{
			// The type of an object is itself an object: begin tag, a null type-of-type,
			// the version pair, the name, and the closing tag.
			ExpectTag(reader, TagBeginObject, TagBeginPrivateObject);
			ExpectTag(reader, TagNullReference);
			version = reader.ReadInt32();
			reader.ReadInt32(); // minimum reader version
			string name = ReadLengthPrefixedAsciiString(reader);
			ExpectTag(reader, TagEndObject);
			return name;
		}

		static string ReadLengthPrefixedAsciiString(BinaryReader reader)
		{
			int length = reader.ReadInt32();
			if (length is < 0 or > 1024)
				throw new InvalidDataException($"Implausible Nettrace string length {length}.");
			return Encoding.ASCII.GetString(reader.ReadBytes(length));
		}

		static void ExpectTag(BinaryReader reader, byte expected)
		{
			byte tag = reader.ReadByte();
			if (tag != expected)
				throw new InvalidDataException($"Expected FastSerialization tag 0x{expected:X2}, found 0x{tag:X2}.");
		}

		static void ExpectTag(BinaryReader reader, byte expected, byte alternative)
		{
			byte tag = reader.ReadByte();
			if (tag != expected && tag != alternative)
				throw new InvalidDataException($"Expected FastSerialization tag 0x{expected:X2}, found 0x{tag:X2}.");
		}

		static void SkipTraceObject(BinaryReader reader)
		{
			// SyncTimeUtc (8 shorts), SyncTimeQpc, QpcFrequency, then four ints describing
			// the pointer size, process, cpu count and sampling rate.
			reader.BaseStream.Seek(8 * sizeof(short) + 2 * sizeof(long) + 4 * sizeof(int), SeekOrigin.Current);
		}

		/// <summary>
		/// Walks the event blobs of a metadata or event block, handing each one's payload to
		/// <paramref name="onEvent"/> together with its metadata id.
		/// </summary>
		static void ReadBlock(BinaryReader reader, Action<BinaryReader, int, int> onEvent)
		{
			long blockEnd = BeginBlock(reader, out bool compressed);

			// The compressed layout omits fields that repeat, so the previous event's values
			// carry forward. The state is per block, since blocks decode independently.
			int metadataId = 0;
			int payloadSize = 0;

			while (reader.BaseStream.Position < blockEnd)
			{
				if (compressed)
					ReadCompressedEventHeader(reader, ref metadataId, ref payloadSize);
				else
					ReadUncompressedEventHeader(reader, out metadataId, out payloadSize);

				long payloadEnd = reader.BaseStream.Position + payloadSize;
				if (payloadEnd > blockEnd)
					throw new InvalidDataException("A Nettrace event payload runs past the end of its block.");
				onEvent(reader, metadataId, payloadSize);
				reader.BaseStream.Seek(payloadEnd, SeekOrigin.Begin);

				if (!compressed)
					AlignTo4(reader);
			}
			reader.BaseStream.Seek(blockEnd, SeekOrigin.Begin);
		}

		static void SkipBlock(BinaryReader reader)
		{
			long blockEnd = BeginBlock(reader, out _);
			reader.BaseStream.Seek(blockEnd, SeekOrigin.Begin);
		}

		/// <summary>
		/// Reads a block's size and header and returns the stream position where the block
		/// ends. Blocks are 4-byte aligned relative to the start of the stream.
		/// </summary>
		static long BeginBlock(BinaryReader reader, out bool compressed)
		{
			int blockSize = reader.ReadInt32();
			if (blockSize < 0)
				throw new InvalidDataException($"Implausible Nettrace block size {blockSize}.");
			AlignTo4(reader);
			long blockEnd = reader.BaseStream.Position + blockSize;

			short headerSize = reader.ReadInt16();
			short flags = reader.ReadInt16();
			reader.ReadInt64(); // minimum timestamp
			reader.ReadInt64(); // maximum timestamp
			const int ReadHeaderBytes = sizeof(short) * 2 + sizeof(long) * 2;
			if (headerSize > ReadHeaderBytes)
				reader.BaseStream.Seek(headerSize - ReadHeaderBytes, SeekOrigin.Current);

			compressed = (flags & 1) != 0;
			return blockEnd;
		}

		static void AlignTo4(BinaryReader reader)
		{
			long padding = (4 - (reader.BaseStream.Position % 4)) % 4;
			if (padding != 0)
				reader.BaseStream.Seek(padding, SeekOrigin.Current);
		}

		static void ReadCompressedEventHeader(BinaryReader reader, ref int metadataId, ref int payloadSize)
		{
			byte flags = reader.ReadByte();
			if ((flags & CompressedFlagMetadataId) != 0)
				metadataId = (int)ReadVarUInt(reader);
			if ((flags & CompressedFlagCaptureThreadAndSequence) != 0)
			{
				ReadVarUInt(reader); // sequence number delta
				ReadVarUInt(reader); // capture thread id
				ReadVarUInt(reader); // capture processor number
			}
			if ((flags & CompressedFlagThreadId) != 0)
				ReadVarUInt(reader);
			if ((flags & CompressedFlagStackId) != 0)
				ReadVarUInt(reader);
			ReadVarUInt(reader); // timestamp delta
			if ((flags & CompressedFlagActivityId) != 0)
				reader.BaseStream.Seek(16, SeekOrigin.Current);
			if ((flags & CompressedFlagRelatedActivityId) != 0)
				reader.BaseStream.Seek(16, SeekOrigin.Current);
			if ((flags & CompressedFlagDataLength) != 0)
				payloadSize = (int)ReadVarUInt(reader);
		}

		static void ReadUncompressedEventHeader(BinaryReader reader, out int metadataId, out int payloadSize)
		{
			reader.ReadInt32(); // event size
			metadataId = reader.ReadInt32();
			reader.ReadInt32(); // sequence number
			reader.ReadInt64(); // thread id
			reader.ReadInt64(); // capture thread id
			reader.ReadInt32(); // processor number
			reader.ReadInt32(); // stack id
			reader.ReadInt64(); // timestamp
			reader.BaseStream.Seek(32, SeekOrigin.Current); // activity + related activity id
			payloadSize = reader.ReadInt32();
		}

		static ulong ReadVarUInt(BinaryReader reader)
		{
			ulong value = 0;
			int shift = 0;
			while (true)
			{
				byte b = reader.ReadByte();
				value |= (ulong)(b & 0x7F) << shift;
				if ((b & 0x80) == 0)
					return value;
				shift += 7;
				if (shift > 63)
					throw new InvalidDataException("Malformed variable-length integer in the Nettrace stream.");
			}
		}

		/// <summary>
		/// An event in a metadata block describes an event type: which provider and event
		/// name a metadata id stands for in the event blocks that follow.
		/// </summary>
		static void ReadMetadataEvent(BinaryReader reader, int metadataId, Dictionary<int, EventMetadata> metadata)
		{
			if (metadataId != 0)
				return; // Only the metadata records themselves are of interest here.
			int id = reader.ReadInt32();
			string providerName = ReadUtf16NullTerminated(reader);
			int eventId = reader.ReadInt32();
			ReadUtf16NullTerminated(reader); // event name, empty for manifest-based providers
			reader.ReadInt64(); // keywords
			int version = reader.ReadInt32();
			metadata[id] = new EventMetadata(providerName, eventId, version);
		}

		static void ReadEvent(BinaryReader reader, int metadataId, Dictionary<int, EventMetadata> metadata,
			List<ModuleRecord> modules, Dictionary<long, string> assemblyNames)
		{
			if (!metadata.TryGetValue(metadataId, out var meta))
				return;

			// ModuleLoad, ModuleDCStart and ModuleDCStop share one payload layout - unlike
			// the similarly named DomainModule* and ModuleRange* events, which carry
			// different fields and are deliberately not matched here.
			if (IsModuleEvent(meta))
			{
				reader.ReadInt64(); // module id
				long assemblyId = reader.ReadInt64();
				reader.ReadInt32(); // module flags
				reader.ReadInt32(); // reserved
				string ilPath = ReadUtf16NullTerminated(reader);
				string nativePath = ReadUtf16NullTerminated(reader);
				modules.Add(new ModuleRecord(assemblyId, ilPath, nativePath));
			}
			else if (IsAssemblyEvent(meta))
			{
				long assemblyId = reader.ReadInt64();
				reader.ReadInt64(); // app domain id
				if (meta.Version >= 1)
					reader.ReadInt64(); // binding id
				reader.ReadInt32(); // assembly flags
				string fullName = ReadUtf16NullTerminated(reader);
				if (fullName.Length > 0)
				{
					// "Foo, Version=1.0.0.0, Culture=..." - the simple name is enough to
					// label a module that has no file on disk.
					int comma = fullName.IndexOf(',');
					assemblyNames[assemblyId] = comma > 0 ? fullName[..comma] : fullName;
				}
			}
		}

		static bool IsModuleEvent(EventMetadata meta) => meta.ProviderName switch {
			RundownProviderName => meta.EventId is RundownModuleDCStart or RundownModuleDCStop,
			RuntimeProviderName => meta.EventId == RuntimeModuleLoad,
			_ => false,
		};

		static bool IsAssemblyEvent(EventMetadata meta) => meta.ProviderName switch {
			RundownProviderName => meta.EventId is RundownAssemblyDCStart or RundownAssemblyDCStop,
			RuntimeProviderName => meta.EventId == RuntimeAssemblyLoad,
			_ => false,
		};

		static string ReadUtf16NullTerminated(BinaryReader reader)
		{
			var builder = new StringBuilder();
			while (true)
			{
				ushort c = reader.ReadUInt16();
				if (c == 0)
					return builder.ToString();
				builder.Append((char)c);
			}
		}

		static IReadOnlyList<ProcessModuleInfo> BuildModuleList(
			List<ModuleRecord> modules, Dictionary<long, string> assemblyNames)
		{
			var result = new List<ProcessModuleInfo>();
			var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

			foreach (var module in modules)
			{
				string? path = FirstExistingPath(module.IlPath, module.NativePath);
				if (path != null)
				{
					if (seen.Add(path))
						result.Add(new ProcessModuleInfo(Path.GetFileName(path), path, IsInMemory: false));
					continue;
				}

				// No file behind it: a byte-array load, a dynamic assembly, or a module
				// whose file the current user cannot see. It can be listed, not opened.
				assemblyNames.TryGetValue(module.AssemblyId, out string? assemblyName);
				string name = assemblyName
					?? NonEmpty(module.IlPath)
					?? NonEmpty(module.NativePath)
					?? "(dynamic module)";
				if (seen.Add("\0" + name))
					result.Add(new ProcessModuleInfo(name, null, IsInMemory: true));
			}

			return result.OrderBy(m => m.Name, StringComparer.OrdinalIgnoreCase).ToList();
		}

		static string? FirstExistingPath(params string?[] candidates)
		{
			foreach (string? candidate in candidates)
			{
				if (string.IsNullOrEmpty(candidate))
					continue;
				try
				{
					if (File.Exists(candidate))
						return Path.GetFullPath(candidate);
				}
				catch (Exception ex) when (ex is ArgumentException or IOException or UnauthorizedAccessException)
				{
					// A path the runtime reports but this process cannot probe.
				}
			}
			return null;
		}

		static string? NonEmpty(string? value) => string.IsNullOrWhiteSpace(value) ? null : value;
	}
}
