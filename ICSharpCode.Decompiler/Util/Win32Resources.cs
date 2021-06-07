#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.PortableExecutable;

namespace ICSharpCode.Decompiler.Util
{
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
			if (rva == 0)
				return null;
			byte* pRoot = pe.GetSectionData(rva).Pointer;
			return new Win32ResourceDirectory(pe, pRoot, 0, new Win32ResourceName("Root"));
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

		internal unsafe Win32ResourceDirectory(PEReader pe, byte* pRoot, int offset, Win32ResourceName name)
		{
			var p = (IMAGE_RESOURCE_DIRECTORY*)(pRoot + offset);
			Characteristics = p->Characteristics;
			TimeDateStamp = p->TimeDateStamp;
			MajorVersion = p->MajorVersion;
			MinorVersion = p->MinorVersion;
			NumberOfNamedEntries = p->NumberOfNamedEntries;
			NumberOfIdEntries = p->NumberOfIdEntries;

			Name = name;
			Directories = new List<Win32ResourceDirectory>();
			Datas = new List<Win32ResourceData>();
			var pEntries = (IMAGE_RESOURCE_DIRECTORY_ENTRY*)(p + 1);
			int total = NumberOfNamedEntries + NumberOfIdEntries;
			for (int i = 0; i < total; i++)
			{
				var pEntry = pEntries + i;
				name = new Win32ResourceName(pRoot, pEntry);
				if ((pEntry->OffsetToData & 0x80000000) == 0)
					Datas.Add(new Win32ResourceData(pe, pRoot, (int)pEntry->OffsetToData, name));
				else
					Directories.Add(new Win32ResourceDirectory(pe, pRoot, (int)(pEntry->OffsetToData & 0x7FFFFFFF), name));
			}
		}

		static unsafe string ReadString(byte* pRoot, int offset)
		{
			var pString = (IMAGE_RESOURCE_DIRECTORY_STRING*)(pRoot + offset);
			return new string(pString->NameString, 0, pString->Length);
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

		private readonly void* _pointer;

		public Win32ResourceName Name { get; }

		public byte[] Data {
			get {
				byte[] data = new byte[Size];
				fixed (void* pData = data)
					Buffer.MemoryCopy(_pointer, pData, Size, Size);
				return data;
			}
		}

		internal Win32ResourceData(PEReader pe, byte* pRoot, int offset, Win32ResourceName name)
		{
			var p = (IMAGE_RESOURCE_DATA_ENTRY*)(pRoot + offset);
			OffsetToData = p->OffsetToData;
			Size = p->Size;
			CodePage = p->CodePage;
			Reserved = p->Reserved;

			_pointer = pe.GetSectionData((int)OffsetToData).Pointer;
			Name = name;
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

		internal unsafe Win32ResourceName(byte* pRoot, IMAGE_RESOURCE_DIRECTORY_ENTRY* pEntry)
		{
			_name = (pEntry->Name & 0x80000000) == 0 ? (object)(ushort)pEntry->Name : ReadString(pRoot, (int)(pEntry->Name & 0x7FFFFFFF));

			static string ReadString(byte* pRoot, int offset)
			{
				var pString = (IMAGE_RESOURCE_DIRECTORY_STRING*)(pRoot + offset);
				return new string(pString->NameString, 0, pString->Length);
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
