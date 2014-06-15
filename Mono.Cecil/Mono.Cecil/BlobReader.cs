using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mono.Cecil
{
	/// <summary>
	/// Allows reading from a blob.
	/// </summary>
	public struct BlobReader
	{
		readonly byte[] buffer;
		readonly int start;
		readonly int length;
		int position;
		
		public BlobReader(byte[] buffer, int position, int length)
		{
			this.buffer = buffer ?? Empty<byte>.Array;
			this.start = position;
			this.length = length;
			this.position = position;
		}

		public int Position {
			get { return position - start; }
			set { position = value + start; }
		}

		public int Length
		{
			get { return length; }
		}

		public void Advance(int length)
		{
			position += length;
		}

		public byte ReadByte()
		{
			return buffer[position++];
		}

		public sbyte ReadSByte()
		{
			return (sbyte)ReadByte();
		}

		public byte[] ReadBytes(int length)
		{
			var bytes = new byte[length];
			Buffer.BlockCopy(buffer, position, bytes, 0, length);
			position += length;
			return bytes;
		}

		public ushort ReadUInt16()
		{
			ushort value = (ushort)(buffer[position]
				| (buffer[position + 1] << 8));
			position += 2;
			return value;
		}

		public short ReadInt16()
		{
			return (short)ReadUInt16();
		}

		public uint ReadUInt32()
		{
			uint value = (uint)(buffer[position]
				| (buffer[position + 1] << 8)
				| (buffer[position + 2] << 16)
				| (buffer[position + 3] << 24));
			position += 4;
			return value;
		}

		public int ReadInt32()
		{
			return (int)ReadUInt32();
		}

		public ulong ReadUInt64()
		{
			uint low = ReadUInt32();
			uint high = ReadUInt32();

			return (((ulong)high) << 32) | low;
		}

		public long ReadInt64()
		{
			return (long)ReadUInt64();
		}

		public uint ReadCompressedUInt32()
		{
			byte first = ReadByte();
			if ((first & 0x80) == 0)
				return first;

			if ((first & 0x40) == 0)
				return ((uint)(first & ~0x80) << 8)
					| ReadByte();

			return ((uint)(first & ~0xc0) << 24)
				| (uint)ReadByte() << 16
				| (uint)ReadByte() << 8
				| ReadByte();
		}

		public int ReadCompressedInt32()
		{
			var value = (int)(ReadCompressedUInt32() >> 1);
			if ((value & 1) == 0)
				return value;
			if (value < 0x40)
				return value - 0x40;
			if (value < 0x2000)
				return value - 0x2000;
			if (value < 0x10000000)
				return value - 0x10000000;
			return value - 0x20000000;
		}

		public float ReadSingle()
		{
			if (!BitConverter.IsLittleEndian) {
				var bytes = ReadBytes(4);
				Array.Reverse(bytes);
				return BitConverter.ToSingle(bytes, 0);
			}

			float value = BitConverter.ToSingle(buffer, position);
			position += 4;
			return value;
		}

		public double ReadDouble()
		{
			if (!BitConverter.IsLittleEndian) {
				var bytes = ReadBytes(8);
				Array.Reverse(bytes);
				return BitConverter.ToDouble(bytes, 0);
			}

			double value = BitConverter.ToDouble(buffer, position);
			position += 8;
			return value;
		}
	}
}
