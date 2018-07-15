// Copyright (c) Cristian Civera (cristian@aspitalia.com)
// This code is distributed under the MS-PL (for details please see \doc\MS-PL.txt)

using System;
using System.IO;

namespace Ricciolo.StylesExplorer.MarkupReflection
{
	internal class BamlBinaryReader : BinaryReader
	{
		public BamlBinaryReader(Stream stream)
			: base(stream)
		{
		}

		public virtual double ReadCompressedDouble()
		{
			byte b = this.ReadByte();
			switch (b) {
				case 1:
					return 0;
				case 2:
					return 1;
				case 3:
					return -1;
				case 4:
					return ReadInt32() * 1E-06;
				case 5:
					return this.ReadDouble();
				default:
					throw new BadImageFormatException($"Unexpected byte sequence in ReadCompressedDouble: 0x{b:x}");
			}
		}

		public int ReadCompressedInt32()
		{
			return base.Read7BitEncodedInt();
		}
	}
}