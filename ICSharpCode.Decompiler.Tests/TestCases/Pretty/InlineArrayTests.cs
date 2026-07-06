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

		[InlineArray(4)]
		public struct Nested4
		{
			private Byte16 elem;
		}

		[InlineArray(4)]
		public struct String4
		{
			private string elem;
		}

		[InlineArray(8)]
		public struct WithMethods
		{
			private int elem;

			public int Get(int index)
			{
				return this[index];
			}

			public readonly int GetReadonly(int index)
			{
				return this[index];
			}

			public void Set(int index, int value)
			{
				this[index] = value;
			}
		}

		public struct Wrapper
		{
			public Byte16 Buffer;
		}

		private Byte16 instanceBuffer;
		private readonly Byte16 readonlyBuffer;
		private static Byte16 staticBuffer;
		private Wrapper wrapper;

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

		public byte InstanceFieldRead()
		{
			return instanceBuffer[3];
		}

		public void InstanceFieldWrite(byte value)
		{
			instanceBuffer[3] = value;
		}

		public byte ReadOnlyFieldRead()
		{
			return readonlyBuffer[3];
		}

		public byte StaticFieldRead()
		{
			return staticBuffer[4];
		}

		public void StaticFieldWrite(byte value)
		{
			staticBuffer[4] = value;
		}

		public byte NestedFieldRead()
		{
			return wrapper.Buffer[2];
		}

		public void NestedFieldWrite(byte value)
		{
			wrapper.Buffer[2] = value;
		}

		public byte ArrayElementReceiverRead(Byte16[] array, int index)
		{
			return array[index][3];
		}

		public void ArrayElementReceiverWrite(Byte16[] array, int index, byte value)
		{
			array[index][3] = value;
		}

		public byte RefParamRead(ref Byte16 array)
		{
			return array[1];
		}

		public void RefParamWrite(ref Byte16 array, byte value)
		{
			array[1] = value;
		}

		public byte InParamRead(in Byte16 array)
		{
			return array[1];
		}

		public void CompoundAssign(ref Byte16 array, byte value)
		{
			array[2] += value;
			array[3]++;
		}

		public void PassElementByRef(ref Byte16 array)
		{
			Mutate(ref array[5]);
		}

		public void PassElementByOut(ref Byte16 array)
		{
			Produce(out array[5]);
		}

		public void PassElementByIn(ref Byte16 array)
		{
			Consume(in array[5]);
		}

		public void RefLocalWrite(ref Byte16 array)
		{
#if EXPECTED_OUTPUT
			array[2] = 42;
#else
			ref byte reference = ref array[2];
			reference = 42;
#endif
		}

		public byte RefReadonlyLocalRead(in Byte16 array)
		{
#if EXPECTED_OUTPUT
			return array[6];
#else
			ref readonly byte reference = ref array[6];
			return reference;
#endif
		}

		public byte NestedRead(ref Nested4 array)
		{
			return array[1][2];
		}

		public void NestedWrite(ref Nested4 array, byte value)
		{
			array[1][2] = value;
		}

		public string StringRead(ref String4 array)
		{
			return array[0];
		}

		public void StringWrite(ref String4 array, string value)
		{
			array[0] = value;
		}

		public byte FromEndConst(Byte16 array)
		{
#if EXPECTED_OUTPUT
			return array[15];
#else
			return array[^1];
#endif
		}

		public byte FromEndVariable(Byte16 array, Index index)
		{
#if EXPECTED_OUTPUT
			return array[index.GetOffset(16)];
#else
			return array[index];
#endif
		}

		public Span<byte> RangeConst(ref Byte16 array)
		{
#if EXPECTED_OUTPUT
			return ((Span<byte>)array).Slice(1, 4);
#else
			return array[1..5];
#endif
		}

		public Span<byte> RangeOpenEnd(ref Byte16 array)
		{
#if EXPECTED_OUTPUT && ROSLYN5
			return ((Span<byte>)array).Slice(2);
#elif EXPECTED_OUTPUT
			return ((Span<byte>)array).Slice(2, 14);
#else
			return array[2..];
#endif
		}

		public Span<byte> RangeFull(ref Byte16 array)
		{
#if EXPECTED_OUTPUT
			return array;
#else
			return array[..];
#endif
		}

		public Span<byte> RangeVariableEnd(ref Byte16 array, int end)
		{
#if EXPECTED_OUTPUT
			return ((Span<byte>)array).Slice(0, end);
#else
			return array[..end];
#endif
		}

		public Span<byte> RangeVariable(ref Byte16 array, Range range)
		{
#if EXPECTED_OUTPUT
			Range range2 = range;
			int offset = range2.Start.GetOffset(16);
			int length = range2.End.GetOffset(16) - offset;
			return ((Span<byte>)array).Slice(offset, length);
#else
			return array[range];
#endif
		}

		public byte VariableSplitting(Byte16 array, byte value)
		{
			return array[GetIndex()] = (array[GetIndex() + 1] = value);
		}

		public int SumForeach(Byte16 b)
		{
			int num = 0;
			foreach (byte b2 in b)
			{
				num += b2;
			}
			return num;
		}

		public int SumForeachLocal()
		{
			Byte16 buffer = GetByte16();
			int num = 0;
			foreach (byte b in buffer)
			{
				num += b;
			}
			return num;
		}

		public int SumForeachRvalue()
		{
#if EXPECTED_OUTPUT
			int num = 0;
			Byte16 buffer = GetByte16();
			foreach (byte b in buffer)
			{
				num += b;
			}
			return num;
#else
			int num = 0;
			foreach (byte b in GetByte16())
			{
				num += b;
			}
			return num;
#endif
		}

		public int SumForeachBreak(Byte16 array, int limit)
		{
			int num = 0;
			foreach (byte b in array)
			{
				if (num > limit)
				{
					break;
				}
				num += b;
			}
			return num;
		}

		public int SumForeachMutate(Byte16 array)
		{
			int num = 0;
			foreach (byte b in array)
			{
				array[0] = b;
				num += b;
			}
			return num;
		}

		public void SpanConversionFromField()
		{
			ReceiverSpan(instanceBuffer);
			ReceiverReadOnlySpan(readonlyBuffer);
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

		public void Mutate(ref byte b)
		{
			b++;
		}

		public void Produce(out byte b)
		{
			b = 1;
		}

		public void Consume(in byte b)
		{
		}
	}
}
