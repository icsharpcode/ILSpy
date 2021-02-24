using System;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: AssemblyFileVersion("4.0.0.0")]
[assembly: AssemblyInformationalVersion("4.0.0.0")]
[assembly: AssemblyTitle("System.Runtime.CompilerServices.Unsafe")]
[assembly: AssemblyDescription("System.Runtime.CompilerServices.Unsafe")]
[assembly: AssemblyMetadata(".NETFrameworkAssembly", "")]
[assembly: AssemblyMetadata("Serviceable", "True")]
[assembly: AssemblyCopyright("© Microsoft Corporation.  All rights reserved.")]
[assembly: AssemblyCompany("Microsoft Corporation")]
[assembly: AssemblyProduct("Microsoft® .NET Framework")]
[assembly: CLSCompliant(false)]

internal sealed class ExtraUnsafeTests
{
	public unsafe static void PinWithTypeMismatch(ref uint managedPtr)
	{
		fixed (ushort* ptr = &Unsafe.As<uint, ushort>(ref managedPtr))
		{
		}
	}

	public unsafe static uint* RefToPointerWithoutPinning(ref uint managedPtr)
	{
		return (uint*)Unsafe.AsPointer(ref managedPtr);
	}

	public static ref ulong RefAssignTypeMismatch(ref uint a, ref uint b)
	{
		ref ushort reference = ref Unsafe.As<uint, ushort>(ref a);
		if (a != 0)
		{
			reference = ref Unsafe.As<uint, ushort>(ref b);
		}
		Console.WriteLine(reference);
		return ref Unsafe.As<ushort, ulong>(ref reference);
	}

	public unsafe static byte[] Issue1292(int val, byte[] arr)
	{
		fixed (byte* ptr = arr)
		{
			*(int*)ptr = val;
		}
		return arr;
	}

	public unsafe void pin_ptr_test(int[] a, int[] b)
	{
		//The blocks IL_0016 are reachable both inside and outside the pinned region starting at IL_0007. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		ref int reference;
		fixed (int* ptr = &a[0])
		{
			if (*ptr <= 0)
			{
				Unsafe.AddByteOffset(ref *ptr, 4 * 0) = 1;
				return;
			}
			reference = ref *ptr;
		}
		fixed (int* ptr = &b[reference])
		{
			Unsafe.AddByteOffset(ref *ptr, 4 * 0) = 1;
		}
	}

	private static void Issue2148(string[] args)
	{
		for (int/*pinned*/ i = 0; i < 100; i++)
		{
			Console.WriteLine("Hello World!");
		}
	}

	private unsafe static void Issue2189()
	{
		fixed (int* ptr = &Unsafe.AsRef<int>((int*)Unsafe.AsPointer(ref SomeStruct.instance.mtfhist)))
		{
			int num = *ptr;
		}
	}
	private unsafe static void PinUnmanagedPtr(int* A_0)
	{
		fixed (int* ptr = &Unsafe.AsRef<int>(A_0))
		{
			int num = *ptr;
		}
	}
	private static ref float AddressTypeMismatch(ref int A_0)
	{
		return ref Unsafe.As<int, float>(ref A_0);
	}
	private unsafe static ref float AddressTypeMismatch(int* A_0)
	{
		return ref *(float*)A_0;
	}
	private static float LoadWithTypeMismatch(ref int A_0)
	{
		return Unsafe.As<int, float>(ref A_0);
	}
	private unsafe static float LoadWithTypeMismatch(int* A_0)
	{
		return *(float*)A_0;
	}
	private static void StoreWithTypeMismatch(ref int A_0)
	{
		Unsafe.As<int, float>(ref A_0) = 1f;
	}
	private unsafe static void StoreWithTypeMismatch(int* A_0)
	{
		*(float*)A_0 = 1f;
	}
	private static ref float AddressOfFieldTypeMismatch(ref int A_0)
	{
		return ref Unsafe.As<int, SomeStruct>(ref A_0).float_field;
	}
	private unsafe static ref float AddressOfFieldTypeMismatch(int* A_0)
	{
		return ref ((SomeStruct*)A_0)->float_field;
	}
	private static float LoadOfFieldTypeMismatch(ref int A_0)
	{
		return Unsafe.As<int, SomeStruct>(ref A_0).float_field;
	}
	private unsafe static float LoadOfFieldTypeMismatch(int* A_0)
	{
		return ((SomeStruct*)A_0)->float_field;
	}
	private static void StoreOfFieldTypeMismatch(ref int A_0)
	{
		Unsafe.As<int, SomeStruct>(ref A_0).float_field = 1f;
	}
	private unsafe static void StoreOfFieldTypeMismatch(int* A_0)
	{
		((SomeStruct*)A_0)->float_field = 1f;
	}
}
internal struct SomeStruct
{
	public int int_field;
	public float float_field;
}

namespace System.Runtime.CompilerServices
{
	public static class Unsafe
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static T Read<T>(void* source)
		{
			return *(T*)source;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static T ReadUnaligned<T>(void* source)
		{
			return Unsafe.ReadUnaligned<T>(source);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T ReadUnaligned<T>(ref byte source)
		{
			return Unsafe.ReadUnaligned<T>(ref source);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void Write<T>(void* destination, T value)
		{
			*(T*)destination = value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteUnaligned<T>(void* destination, T value)
		{
			Unsafe.WriteUnaligned(destination, value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteUnaligned<T>(ref byte destination, T value)
		{
			Unsafe.WriteUnaligned(ref destination, value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void Copy<T>(void* destination, ref T source)
		{
			*(T*)destination = source;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void Copy<T>(ref T destination, void* source)
		{
			destination = *(T*)source;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void* AsPointer<T>(ref T value)
		{
			return Unsafe.AsPointer(ref value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SkipInit<T>(out T value)
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int SizeOf<T>()
		{
			return sizeof(T);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void CopyBlock(void* destination, void* source, uint byteCount)
		{
			// IL cpblk instruction
			Unsafe.CopyBlock(destination, source, byteCount);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CopyBlock(ref byte destination, ref byte source, uint byteCount)
		{
			// IL cpblk instruction
			Unsafe.CopyBlock(ref destination, ref source, byteCount);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void CopyBlockUnaligned(void* destination, void* source, uint byteCount)
		{
			// IL cpblk instruction
			Unsafe.CopyBlockUnaligned(destination, source, byteCount);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CopyBlockUnaligned(ref byte destination, ref byte source, uint byteCount)
		{
			// IL cpblk instruction
			Unsafe.CopyBlockUnaligned(ref destination, ref source, byteCount);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void InitBlock(void* startAddress, byte value, uint byteCount)
		{
			// IL initblk instruction
			Unsafe.InitBlock(startAddress, value, byteCount);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void InitBlock(ref byte startAddress, byte value, uint byteCount)
		{
			// IL initblk instruction
			Unsafe.InitBlock(ref startAddress, value, byteCount);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void InitBlockUnaligned(void* startAddress, byte value, uint byteCount)
		{
			// IL initblk instruction
			Unsafe.InitBlockUnaligned(startAddress, value, byteCount);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void InitBlockUnaligned(ref byte startAddress, byte value, uint byteCount)
		{
			// IL initblk instruction
			Unsafe.InitBlockUnaligned(ref startAddress, value, byteCount);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T As<T>(object o) where T : class
		{
			return (T)o;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static ref T AsRef<T>(void* source)
		{
			return ref *(T*)source;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T AsRef<T>(in T source)
		{
			return ref source;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref TTo As<TFrom, TTo>(ref TFrom source)
		{
			return ref Unsafe.As<TFrom, TTo>(ref source);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Unbox<T>(object box) where T : struct
		{
			return ref (T)box;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Add<T>(ref T source, int elementOffset)
		{
			return ref Unsafe.Add(ref source, elementOffset);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void* Add<T>(void* source, int elementOffset)
		{
			return (byte*)source + (nint)elementOffset * (nint)sizeof(T);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Add<T>(ref T source, IntPtr elementOffset)
		{
			return ref Unsafe.Add(ref source, elementOffset);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Add<T>(ref T source, UIntPtr elementOffset)
		{
			return ref Unsafe.Add(ref source, elementOffset);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T AddByteOffset<T>(ref T source, IntPtr byteOffset)
		{
			return ref Unsafe.AddByteOffset(ref source, byteOffset);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T AddByteOffset<T>(ref T source, UIntPtr byteOffset)
		{
			return ref Unsafe.AddByteOffset(ref source, byteOffset);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Subtract<T>(ref T source, int elementOffset)
		{
			return ref Unsafe.Subtract(ref source, elementOffset);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void* Subtract<T>(void* source, int elementOffset)
		{
			return (byte*)source - (nint)elementOffset * (nint)sizeof(T);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Subtract<T>(ref T source, IntPtr elementOffset)
		{
			return ref Unsafe.Subtract(ref source, elementOffset);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Subtract<T>(ref T source, UIntPtr elementOffset)
		{
			return ref Unsafe.Subtract(ref source, elementOffset);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T SubtractByteOffset<T>(ref T source, IntPtr byteOffset)
		{
			return ref Unsafe.SubtractByteOffset(ref source, byteOffset);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T SubtractByteOffset<T>(ref T source, UIntPtr byteOffset)
		{
			return ref Unsafe.SubtractByteOffset(ref source, byteOffset);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IntPtr ByteOffset<T>(ref T origin, ref T target)
		{
			return Unsafe.ByteOffset(target: ref target, origin: ref origin);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool AreSame<T>(ref T left, ref T right)
		{
			return Unsafe.AreSame(ref left, ref right);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAddressGreaterThan<T>(ref T left, ref T right)
		{
			return Unsafe.IsAddressGreaterThan(ref left, ref right);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAddressLessThan<T>(ref T left, ref T right)
		{
			return Unsafe.IsAddressLessThan(ref left, ref right);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static bool IsNullRef<T>(ref T source)
		{
			return Unsafe.AsPointer(ref source) == null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static ref T NullRef<T>()
		{
			return ref *(T*)null;
		}
	}
}
