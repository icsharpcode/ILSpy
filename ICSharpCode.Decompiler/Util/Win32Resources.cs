// Copyright (c) 2020 文煌
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
using System.Diagnostics;
using System.Reflection.PortableExecutable;

namespace ICSharpCode.Decompiler.Util
{
	/// <summary>
	/// Resolves a resource data RVA to a pointer into the PE image and reports how many bytes remain
	/// from that point to the end of the containing section, so a crafted Size can be bounded against
	/// the data that actually exists.
	/// </summary>
	internal unsafe delegate byte* ResolveResourceData(int rva, out int length);

	/// <summary>
	/// Represents win32 resources
	/// </summary>
	public static class Win32Resources
	{
		/// <summary>
		/// Reads win32 resource root directory
		/// </summary>
		/// <param name="pe"></param>
		/// <returns></returns>
		public static unsafe Win32ResourceDirectory? ReadWin32Resources(this PEReader pe)
		{
			if (pe == null)
			{
				throw new ArgumentNullException(nameof(pe));
			}

			int rva = pe.PEHeaders.PEHeader?.ResourceTableDirectory.RelativeVirtualAddress ?? 0;
			// A crafted header can set the directory RVA negative; GetSectionData throws on a negative
			// RVA, so reject it here rather than let it abort parsing.
			if (rva <= 0)
				return null;
			var block = pe.GetSectionData(rva);
			if (block.Pointer == null || block.Length <= 0)
				return null;

			byte* Resolve(int dataRva, out int length)
			{
				// OffsetToData is a file uint cast to int, so it can be negative; GetSectionData throws
				// on a negative RVA, and a positive RVA outside any section yields an empty block.
				if (dataRva < 0)
				{
					length = 0;
					return null;
				}
				var dataBlock = pe.GetSectionData(dataRva);
				length = dataBlock.Length;
				return dataBlock.Pointer;
			}

			return Win32ResourceDirectory.ReadDirectoryTree(block.Pointer, block.Length, Resolve);
		}

		/// <summary>
		/// Returns true when reading <paramref name="size"/> bytes at <paramref name="offset"/> stays
		/// within a section of <paramref name="length"/> bytes. Subtraction avoids the overflow an
		/// <c>offset + size</c> bound would have on attacker-controlled values.
		/// </summary>
		internal static bool InBounds(int offset, int size, int length)
		{
			return offset >= 0 && size >= 0 && offset <= length && size <= length - offset;
		}

		public static Win32ResourceDirectory? Find(this Win32ResourceDirectory root, Win32ResourceName type)
		{
			if (root is null)
				throw new ArgumentNullException(nameof(root));
			if (!root.Name.HasName || root.Name.Name != "Root")
				throw new ArgumentOutOfRangeException(nameof(root));
			if (type is null)
				throw new ArgumentNullException(nameof(type));

			return root.FindDirectory(type);
		}

		public static Win32ResourceDirectory? Find(this Win32ResourceDirectory root, Win32ResourceName type, Win32ResourceName name)
		{
			if (root is null)
				throw new ArgumentNullException(nameof(root));
			if (!root.Name.HasName || root.Name.Name != "Root")
				throw new ArgumentOutOfRangeException(nameof(root));
			if (type is null)
				throw new ArgumentNullException(nameof(type));
			if (name is null)
				throw new ArgumentNullException(nameof(name));

			return root.FindDirectory(type)?.FindDirectory(name);
		}

		public static Win32ResourceData? Find(this Win32ResourceDirectory root, Win32ResourceName type, Win32ResourceName name, Win32ResourceName langId)
		{
			if (root is null)
				throw new ArgumentNullException(nameof(root));
			if (!root.Name.HasName || root.Name.Name != "Root")
				throw new ArgumentOutOfRangeException(nameof(root));
			if (type is null)
				throw new ArgumentNullException(nameof(type));
			if (name is null)
				throw new ArgumentNullException(nameof(name));
			if (langId is null)
				throw new ArgumentNullException(nameof(langId));

			return root.FindDirectory(type)?.FindDirectory(name)?.FindData(langId);
		}
	}

	[DebuggerDisplay("Directory: {Name}")]
	public sealed class Win32ResourceDirectory
	{
		#region Structure
		public uint Characteristics { get; }
		public uint TimeDateStamp { get; }
		public ushort MajorVersion { get; }
		public ushort MinorVersion { get; }
		public ushort NumberOfNamedEntries { get; }
		public ushort NumberOfIdEntries { get; }
		#endregion

		public Win32ResourceName Name { get; }

		public IList<Win32ResourceDirectory> Directories { get; }

		public IList<Win32ResourceData> Datas { get; }

		// Windows resource trees are conventionally Type -> Name -> Language (three directory levels).
		// This cap sits well above that, so it never rejects a real tree, while keeping a crafted deep
		// chain from exhausting the stack - which the cycle check alone cannot, since a non-repeating
		// chain of distinct offsets is acyclic yet still arbitrarily deep.
		const int MaxDepth = 16;

		internal static unsafe Win32ResourceDirectory ReadDirectoryTree(byte* pRoot, int length, ResolveResourceData resolveData)
		{
			return new Win32ResourceDirectory(resolveData, pRoot, length, 0, new Win32ResourceName("Root"), 0, new HashSet<int>());
		}

		unsafe Win32ResourceDirectory(ResolveResourceData resolveData, byte* pRoot, int length, int offset, Win32ResourceName name, int depth, HashSet<int> visited)
		{
			Name = name;
			Directories = new List<Win32ResourceDirectory>();
			Datas = new List<Win32ResourceData>();

			// A crafted .rsrc tree can be cyclic or arbitrarily deep. The depth cap bounds nesting; the
			// visited set, shared across the whole walk, parses each directory offset at most once,
			// which breaks cycles and caps total work at the distinct offsets in the section. Real
			// resource trees never reference one directory from two branches, so parsing a repeated
			// offset only once is lossless for valid input.
			if (depth > MaxDepth || !visited.Add(offset))
				return;
			if (!Win32Resources.InBounds(offset, sizeof(IMAGE_RESOURCE_DIRECTORY), length))
				return;

			var p = (IMAGE_RESOURCE_DIRECTORY*)(pRoot + offset);
			Characteristics = p->Characteristics;
			TimeDateStamp = p->TimeDateStamp;
			MajorVersion = p->MajorVersion;
			MinorVersion = p->MinorVersion;
			NumberOfNamedEntries = p->NumberOfNamedEntries;
			NumberOfIdEntries = p->NumberOfIdEntries;

			int entriesOffset = offset + sizeof(IMAGE_RESOURCE_DIRECTORY);
			int total = NumberOfNamedEntries + NumberOfIdEntries;
			// Clamp the file-declared entry count to the entries that actually fit before the section
			// end, so the walk below cannot read past it.
			int available = (length - entriesOffset) / sizeof(IMAGE_RESOURCE_DIRECTORY_ENTRY);
			if (available < 0)
				available = 0;
			if (total > available)
				total = available;

			var pEntries = (IMAGE_RESOURCE_DIRECTORY_ENTRY*)(pRoot + entriesOffset);
			for (int i = 0; i < total; i++)
			{
				var pEntry = pEntries + i;
				name = new Win32ResourceName(pRoot, length, pEntry);
				if ((pEntry->OffsetToData & 0x80000000) == 0)
					Datas.Add(new Win32ResourceData(resolveData, pRoot, length, (int)pEntry->OffsetToData, name));
				else
					Directories.Add(new Win32ResourceDirectory(resolveData, pRoot, length, (int)(pEntry->OffsetToData & 0x7FFFFFFF), name, depth + 1, visited));
			}
		}

		public Win32ResourceDirectory? FindDirectory(Win32ResourceName name)
		{
			foreach (var directory in Directories)
			{
				if (directory.Name == name)
					return directory;
			}
			return null;
		}

		public Win32ResourceData? FindData(Win32ResourceName name)
		{
			foreach (var data in Datas)
			{
				if (data.Name == name)
					return data;
			}
			return null;
		}

		public Win32ResourceDirectory? FirstDirectory()
		{
			return Directories.Count != 0 ? Directories[0] : null;
		}

		public Win32ResourceData? FirstData()
		{
			return Datas.Count != 0 ? Datas[0] : null;
		}
	}

	[DebuggerDisplay("Data: {Name}")]
	public sealed unsafe class Win32ResourceData
	{
		#region Structure
		public uint OffsetToData { get; }
		public uint Size { get; }
		public uint CodePage { get; }
		public uint Reserved { get; }
		#endregion

		private readonly byte* _pointer;
		private readonly int _dataLength;

		public Win32ResourceName Name { get; }

		public byte[] Data {
			get {
				// Size is file-controlled (up to 4 GB); bound both the allocation and the copy to the
				// bytes the resolver reports between the data pointer and the end of its section. That
				// length is never negative, so the clamped count fits in an int.
				if (_pointer == null || _dataLength <= 0)
					return Array.Empty<byte>();
				int count = (int)Math.Min((uint)Size, (uint)_dataLength);
				byte[] data = new byte[count];
				fixed (void* pData = data)
					Buffer.MemoryCopy(_pointer, pData, count, count);
				return data;
			}
		}

		internal Win32ResourceData(ResolveResourceData resolveData, byte* pRoot, int length, int offset, Win32ResourceName name)
		{
			Name = name;
			if (!Win32Resources.InBounds(offset, sizeof(IMAGE_RESOURCE_DATA_ENTRY), length))
				return;

			var p = (IMAGE_RESOURCE_DATA_ENTRY*)(pRoot + offset);
			OffsetToData = p->OffsetToData;
			Size = p->Size;
			CodePage = p->CodePage;
			Reserved = p->Reserved;

			// OffsetToData is an RVA that can exceed int range; wrap deliberately (the library builds
			// checked) so an out-of-range value reaches the resolver as a negative RVA it rejects.
			_pointer = resolveData(unchecked((int)OffsetToData), out _dataLength);
		}
	}

	public sealed class Win32ResourceName
	{
		private readonly object _name;

		public bool HasName => _name is string;

		public bool HasId => _name is ushort;

		public string Name => (string)_name;

		public ushort Id => (ushort)_name;

		public Win32ResourceName(string name)
		{
			_name = name ?? throw new ArgumentNullException(nameof(name));
		}

		public Win32ResourceName(int id) : this(checked((ushort)id))
		{
		}

		public Win32ResourceName(ushort id)
		{
			_name = id;
		}

		internal unsafe Win32ResourceName(byte* pRoot, int length, IMAGE_RESOURCE_DIRECTORY_ENTRY* pEntry)
		{
			_name = (pEntry->Name & 0x80000000) == 0 ? (object)(ushort)pEntry->Name : ReadString(pRoot, length, (int)(pEntry->Name & 0x7FFFFFFF));

			static string ReadString(byte* pRoot, int length, int offset)
			{
				// A ushort character count followed by that many UTF-16 chars. Reject a prefix that
				// runs past the section and clamp the character count to the bytes that remain.
				if (!Win32Resources.InBounds(offset, sizeof(ushort), length))
					return string.Empty;
				var pString = (IMAGE_RESOURCE_DIRECTORY_STRING*)(pRoot + offset);
				int charCount = pString->Length;
				int available = (length - (offset + sizeof(ushort))) / sizeof(char);
				if (charCount > available)
					charCount = available;
				return new string(pString->NameString, 0, charCount);
			}
		}

		public static bool operator ==(Win32ResourceName x, Win32ResourceName y)
		{
			if (x.HasName)
			{
				return y.HasName ? string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase) == 0 : false;
			}
			else
			{
				return y.HasId ? x.Id == y.Id : false;
			}
		}

		public static bool operator !=(Win32ResourceName x, Win32ResourceName y)
		{
			return !(x == y);
		}

		public override int GetHashCode()
		{
			return _name.GetHashCode();
		}

		public override bool Equals(object? obj)
		{
			if (!(obj is Win32ResourceName name))
				return false;
			return this == name;
		}

		public override string ToString()
		{
			return HasName ? $"Name: {Name}" : $"Id: {Id}";
		}
	}

	internal struct IMAGE_RESOURCE_DIRECTORY
	{
		public uint Characteristics;
		public uint TimeDateStamp;
		public ushort MajorVersion;
		public ushort MinorVersion;
		public ushort NumberOfNamedEntries;
		public ushort NumberOfIdEntries;
	}

	internal struct IMAGE_RESOURCE_DIRECTORY_ENTRY
	{
		public uint Name;
		public uint OffsetToData;
	}

	internal unsafe struct IMAGE_RESOURCE_DIRECTORY_STRING
	{
		public ushort Length;
		public fixed char NameString[1];
	}

	internal struct IMAGE_RESOURCE_DATA_ENTRY
	{
		public uint OffsetToData;
		public uint Size;
		public uint CodePage;
		public uint Reserved;
	}
}
