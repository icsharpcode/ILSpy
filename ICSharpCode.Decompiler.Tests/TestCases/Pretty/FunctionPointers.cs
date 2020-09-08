using System;
using System.Text;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class FunctionPointerAddressOf
	{
		public static void Overloaded()
		{
		}
		public static void Overloaded(int a)
		{
		}

		public unsafe delegate*<void> GetAddress()
		{
			return &Overloaded;
		}

		public unsafe IntPtr GetAddressAsIntPtr()
		{
			return (IntPtr)(delegate*<void>)(&Overloaded);
		}

		public unsafe void* GetAddressAsVoidPtr()
		{
			return (delegate*<int, void>)(&Overloaded);
		}

		public static string VarianceTest(object o)
		{
			return null;
		}

		public unsafe delegate*<StringBuilder, object> Variance()
		{
			return (delegate*<object, string>)(&VarianceTest);
		}

		public unsafe delegate*<void> AddressOfLocalFunction()
		{
			return &LocalFunction;

			static void LocalFunction()
			{

			}
		}
	}

	internal class FunctionPointersWithDynamicTypes
	{
		public class D<T, U>
		{
		}
		public class A<T>
		{
			public class B<U>
			{
			}
		}

		public unsafe delegate*<dynamic, dynamic, dynamic> F1;
		public unsafe delegate*<object, object, dynamic> F2;
		public unsafe delegate*<dynamic, object, object> F3;
		public unsafe delegate*<object, dynamic, object> F4;
		public unsafe delegate*<object, object, object> F5;
		public unsafe delegate*<object, object, ref dynamic> F6;
		public unsafe delegate*<ref dynamic, object, object> F7;
		public unsafe delegate*<object, ref dynamic, object> F8;
		public unsafe delegate*<ref object, ref object, dynamic> F9;
		public unsafe delegate*<dynamic, ref object, ref object> F10;
		public unsafe delegate*<ref object, dynamic, ref object> F11;
		public unsafe delegate*<object, ref readonly dynamic> F12;
		public unsafe delegate*<in dynamic, object> F13;
		public unsafe delegate*<out dynamic, object> F14;
		public unsafe D<delegate*<dynamic>[], dynamic> F15;
		public unsafe delegate*<A<object>.B<dynamic>> F16;
	}

	internal class FunctionPointersWithNativeIntegerTypes
	{
		public unsafe delegate*<nint, nint, nint> F1;
		public unsafe delegate*<IntPtr, IntPtr, nint> F2;
		public unsafe delegate*<nint, IntPtr, IntPtr> F3;
		public unsafe delegate*<IntPtr, nint, IntPtr> F4;
		public unsafe delegate*<delegate*<IntPtr, IntPtr, IntPtr>, nint> F5;
		public unsafe delegate*<nint, delegate*<IntPtr, IntPtr, IntPtr>> F6;
		public unsafe delegate*<delegate*<IntPtr, IntPtr, nint>, IntPtr> F7;
		public unsafe delegate*<IntPtr, delegate*<IntPtr, nint, IntPtr>> F8;
	}

	internal class FunctionPointersWithRefParams
	{
		public unsafe delegate*<in byte, ref char, out float, ref readonly int> F1;
		public unsafe delegate*<ref char, out float, ref int> F2;

		// TODO: re-enable test after https://github.com/dotnet/roslyn/issues/47487 is fixed
		//public unsafe int CallF1(byte b, char c, out float f)
		//{
		//	return F1(1, ref c, out f);
		//}

		public unsafe void CallF2(byte b, char c, out float f)
		{
			F2(ref c, out f) = b;
		}
	}

	// TODO: the new calling convention syntax isn't yet available in the released Roslyn version
	//internal unsafe class FunctionPointersWithCallingConvention
	//{
	//  public delegate*<void> managed;
	//  public delegate* unmanaged<void> unmanaged;
	//  public delegate* unmanaged[Cdecl]<void> cdecl;
	//}

	internal class FunctionPointerTypeInference
	{
		private static char Test(int i)
		{
			return (char)i;
		}

		public unsafe R GenericMethod<T, R>(delegate*<T, R> f, T arg)
		{
			return f(arg);
		}

		public unsafe void Call()
		{
			delegate*<int, char> f = &Test;
			GenericMethod(f, 0);
			GenericMethod((delegate*<int, char>)(&Test), 1);
			GenericMethod<int, char>(null, 2);
		}
	}
}
