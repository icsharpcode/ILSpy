using System;
using System.Runtime.CompilerServices;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class InlineArrayTests
	{
		[InlineArray(16)]
		public struct Byte16
		{
			private byte elem;
		}

		[InlineArray(16)]
		public struct Generic16<T>
		{
			private T elem;
		}

		public byte Byte0()
		{
			return GetByte16()[0];
		}

		public byte GenericByte0()
		{
			return GetGeneric<byte>()[0];
		}

		public byte Byte5()
		{
			return GetByte16()[5];
		}

		public byte GenericByte5()
		{
			return GetGeneric<byte>()[5];
		}

		public byte ByteN()
		{
			return GetByte16()[GetIndex()];
		}

		public byte GenericByteN()
		{
			return GetGeneric<byte>()[GetIndex()];
		}

		public byte Byte0(Byte16 array, byte value)
		{
			return array[0] = value;
		}

		public byte GenericByte0(Generic16<byte> array, byte value)
		{
			return array[0] = value;
		}

		public byte Byte5(Byte16 array, byte value)
		{
			return array[5] = value;
		}

		public byte GenericByte5(Generic16<byte> array, byte value)
		{
			return array[5] = value;
		}

		public byte ByteN(Byte16 array, byte value)
		{
			return array[GetIndex()] = value;
		}

		public byte GenericByteN(Generic16<byte> array, byte value)
		{
			return array[GetIndex()] = value;
		}

		public void Slice(Byte16 array)
		{
			Receiver(array[..8]);
			Receiver((ReadOnlySpan<byte>)array[..8]);
			ReceiverSpan(array[..8]);
			ReceiverReadOnlySpan(array[..8]);
		}

		// TODO
		//public void Slice(Byte16 array, int end)
		//{
		//	Receiver(array[..end]);
		//	Receiver((ReadOnlySpan<byte>)array[..end]);
		//	ReceiverSpan(array[..end]);
		//	ReceiverReadOnlySpan(array[..end]);
		//}

		public byte VariableSplitting(Byte16 array, byte value)
		{
			return array[GetIndex()] = (array[GetIndex() + 1] = value);
		}

		public void OverloadResolution()
		{
			Receiver(GetByte16());
			Receiver((object)GetByte16());
			Byte16 buffer = GetByte16();
			Receiver((Span<byte>)buffer);
			Byte16 buffer2 = GetByte16();
			Receiver((ReadOnlySpan<byte>)buffer2);
			Byte16 buffer3 = GetByte16();
			ReceiverSpan(buffer3);
			Byte16 buffer4 = GetByte16();
			ReceiverReadOnlySpan(buffer4);
		}

		public Byte16 GetByte16()
		{
			return default(Byte16);
		}

		public Generic16<T> GetGeneric<T>()
		{
			return default(Generic16<T>);
		}

		public int GetIndex()
		{
			return 0;
		}

		public void Receiver(Span<byte> span)
		{
		}

		public void Receiver(ReadOnlySpan<byte> span)
		{
		}

		public void Receiver(Byte16 span)
		{
		}

		public void Receiver(object span)
		{
		}

		public void ReceiverSpan(Span<byte> span)
		{
		}

		public void ReceiverReadOnlySpan(ReadOnlySpan<byte> span)
		{
		}
	}
}
