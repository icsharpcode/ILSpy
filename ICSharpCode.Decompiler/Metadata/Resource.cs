// Copyright (c) 2018 Daniel Grunwald
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
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;

namespace ICSharpCode.Decompiler.Metadata
{
	public enum ResourceType
	{
		Linked,
		Embedded,
		AssemblyLinked,
	}

	public abstract class Resource
	{
		public virtual ResourceType ResourceType => ResourceType.Embedded;
		public virtual ManifestResourceAttributes Attributes => ManifestResourceAttributes.Public;
		public abstract string Name { get; }
		public abstract Stream? TryOpenStream();
		public abstract long? TryGetLength();
	}

	public class ByteArrayResource : Resource
	{
		public override string Name { get; }
		byte[] data;

		public ByteArrayResource(string name, byte[] data)
		{
			this.Name = name ?? throw new ArgumentNullException(nameof(name));
			this.data = data ?? throw new ArgumentNullException(nameof(data));
		}

		public override Stream TryOpenStream()
		{
			return new MemoryStream(data);
		}

		public override long? TryGetLength()
		{
			return data.Length;
		}
	}

	sealed class MetadataResource : Resource
	{
		public PEFile Module { get; }
		public ManifestResourceHandle Handle { get; }
		public bool IsNil => Handle.IsNil;

		public MetadataResource(PEFile module, ManifestResourceHandle handle)
		{
			this.Module = module ?? throw new ArgumentNullException(nameof(module));
			this.Handle = handle;
		}

		public bool Equals(MetadataResource other)
		{
			return Module == other.Module && Handle == other.Handle;
		}

		public override bool Equals(object? obj)
		{
			if (obj is MetadataResource res)
				return Equals(res);
			return false;
		}

		public override int GetHashCode()
		{
			return unchecked(982451629 * Module.GetHashCode() + 982451653 * MetadataTokens.GetToken(Handle));
		}

		public override string Name => Module.Metadata.GetString(Module.Metadata.GetManifestResource(Handle).Name);

		public override ManifestResourceAttributes Attributes => Module.Metadata.GetManifestResource(Handle).Attributes;
		public bool HasFlag(ManifestResourceAttributes flag) => (Attributes & flag) == flag;
		public override ResourceType ResourceType => GetResourceType();

		ResourceType GetResourceType()
		{
			var resource = Module.Metadata.GetManifestResource(Handle);
			if (resource.Implementation.IsNil)
				return ResourceType.Embedded;
			if (resource.Implementation.Kind == HandleKind.AssemblyReference)
				return ResourceType.AssemblyLinked;
			return ResourceType.Linked;
		}

		unsafe bool TryReadResource(out byte* ptr, out long length)
		{
			ptr = null;
			length = 0;
			if (ResourceType != ResourceType.Embedded)
				return false;
			var headers = Module.Reader.PEHeaders;
			if (headers.CorHeader == null)
				return false;
			var resources = headers.CorHeader.ResourcesDirectory;
			if (resources.RelativeVirtualAddress == 0)
				return false;
			var sectionData = Module.Reader.GetSectionData(resources.RelativeVirtualAddress);
			if (sectionData.Length == 0)
				return false;
			var resource = Module.Metadata.GetManifestResource(Handle);
			if (resource.Offset + 4 > sectionData.Length)
				return false;
			ptr = sectionData.Pointer + resource.Offset;
			length = ptr[0] + (ptr[1] << 8) + (ptr[2] << 16) + (ptr[3] << 24);
			return length >= 0 && length <= sectionData.Length;
		}

		public override unsafe Stream? TryOpenStream()
		{
			if (!TryReadResource(out var ptr, out var length))
				return null;
			return new ResourceMemoryStream(Module.Reader, ptr + sizeof(int), length);
		}

		public unsafe override long? TryGetLength()
		{
			if (!TryReadResource(out _, out var length))
				return null;
			return length;
		}
	}

	sealed unsafe class ResourceMemoryStream : UnmanagedMemoryStream
	{
#pragma warning disable IDE0052 // Remove unread private members
		readonly PEReader peReader;
#pragma warning restore IDE0052 // Remove unread private members

		public ResourceMemoryStream(PEReader peReader, byte* data, long length)
			: base(data, length, length, FileAccess.Read)
		{
			// Keep the PEReader alive while the stream in in use.
			this.peReader = peReader;
		}
	}
}
