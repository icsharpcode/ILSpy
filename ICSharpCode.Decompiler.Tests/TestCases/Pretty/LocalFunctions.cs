// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
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
using System.Linq;

namespace LocalFunctions
{
	internal class LocalFunctions
	{
		public class Generic<T1> where T1 : struct, ICloneable, IConvertible
		{
			public int MixedLocalFunction<T2>() where T2 : ICloneable, IConvertible
			{
				T2 t2 = default(T2);
				object z = this;
				for (int j = 0; j < 10; j++) {
					int i = 0;
					i += NonStaticMethod6<object>();
					int NonStaticMethod6<T3>()
					{
						t2 = default(T2);
						int l = 0;
						return NonStaticMethod6_1<T1>() + NonStaticMethod6_1<T2>() + z.GetHashCode();
						int NonStaticMethod6_1<T4>()
						{
							return i + l + NonStaticMethod6<T4>() + StaticMethod1<decimal>();
						}
					}
				}
				return MixedLocalFunction<T1>() + MixedLocalFunction<T2>() + StaticMethod1<decimal>() + StaticMethod1<int>() + NonStaticMethod3() + StaticMethod4<object>(null) + StaticMethod5<T1>();
				int NonStaticMethod3()
				{
					return GetHashCode();
				}
				int StaticMethod1<T3>() where T3 : struct
				{
					return typeof(T1).Name.Length + typeof(T2).Name.Length + typeof(T3).Name.Length + StaticMethod1<float>() + StaticMethod1_1<T3, DayOfWeek>() + StaticMethod2_RepeatT2<T2, T3, DayOfWeek>();
				}
				int StaticMethod1_1<T3, T4>() where T3 : struct where T4 : Enum
				{
					return typeof(T1).Name.Length + typeof(T2).Name.Length + typeof(T3).Name.Length + typeof(T4).Name.Length + StaticMethod1<float>() + StaticMethod1_1<T3, DayOfWeek>();
				}
#pragma warning disable CS8387
				int StaticMethod2_RepeatT2<T2, T3, T4>() where T2 : IConvertible where T3 : struct where T4 : Enum
#pragma warning restore CS8387
				{
					return typeof(T2).Name.Length;
				}
				int StaticMethod4<T>(T dd)
				{
					return 0;
				}
				int StaticMethod5<T3>()
				{
					int k = 0;
					return k + NonStaticMethod5_1<T1>();
					int NonStaticMethod5_1<T4>()
					{
						return k;
					}
				}
			}

			public int MixedLocalFunction2Delegate<T2>() where T2 : ICloneable, IConvertible
			{
				T2 t2 = default(T2);
				object z = this;
				for (int j = 0; j < 10; j++) {
					int i = 0;
					i += StaticInvokeAsFunc(NonStaticMethod6<object>);
					int NonStaticMethod6<T3>()
					{
						t2 = default(T2);
						int l = 0;
						return StaticInvokeAsFunc(NonStaticMethod6_1<T1>) + StaticInvokeAsFunc(NonStaticMethod6_1<T2>) + z.GetHashCode();
						int NonStaticMethod6_1<T4>()
						{
							return i + l + StaticInvokeAsFunc(NonStaticMethod6<T4>) + StaticInvokeAsFunc(StaticMethod1<decimal>);
						}
					}
				}
				return StaticInvokeAsFunc(MixedLocalFunction2Delegate<T1>) + StaticInvokeAsFunc(MixedLocalFunction2Delegate<T2>) + StaticInvokeAsFunc(StaticMethod1<decimal>) + StaticInvokeAsFunc(StaticMethod1<int>) + StaticInvokeAsFunc(NonStaticMethod3) + StaticInvokeAsFunc(StaticMethod5<T1>) + new Func<object, int>(StaticMethod4<object>)(null) + StaticInvokeAsFunc2<object>(StaticMethod4<object>) + new Func<Func<object, int>, int>(StaticInvokeAsFunc2<object>)(StaticMethod4<object>);
				int NonStaticMethod3()
				{
					return GetHashCode();
				}
				int StaticInvokeAsFunc(Func<int> func)
				{
					return func();
				}
				int StaticInvokeAsFunc2<T>(Func<T, int> func)
				{
					return func(default(T));
				}
				int StaticMethod1<T3>() where T3 : struct
				{
					return typeof(T1).Name.Length + typeof(T2).Name.Length + typeof(T3).Name.Length + StaticInvokeAsFunc(StaticMethod1<float>) + StaticInvokeAsFunc(StaticMethod1_1<T3, DayOfWeek>) + StaticInvokeAsFunc(StaticMethod2_RepeatT2<T2, T3, DayOfWeek>);
				}
				int StaticMethod1_1<T3, T4>() where T3 : struct where T4 : Enum
				{
					return typeof(T1).Name.Length + typeof(T2).Name.Length + typeof(T3).Name.Length + typeof(T4).Name.Length + StaticInvokeAsFunc(StaticMethod1<float>) + StaticInvokeAsFunc(StaticMethod1_1<T3, DayOfWeek>);
				}
#pragma warning disable CS8387
				int StaticMethod2_RepeatT2<T2, T3, T4>() where T2 : IConvertible where T3 : struct where T4 : Enum
#pragma warning restore CS8387
				{
					return typeof(T2).Name.Length;
				}
				int StaticMethod4<T>(T dd)
				{
					return 0;
				}
				int StaticMethod5<T3>()
				{
					int k = 0;
					return k + StaticInvokeAsFunc(NonStaticMethod5_1<T1>);
					int NonStaticMethod5_1<T4>()
					{
						return k;
					}
				}
			}

			public static void Test_CaptureT<T2>()
			{
				T2 t2 = default(T2);
				Method1<int>();
				void Method1<T3>()
				{
					t2 = default(T2);
					T2 t2x = t2;
					T3 t3 = default(T3);
					Method1_1();
					void Method1_1()
					{
						t2 = default(T2);
						t2x = t2;
						t3 = default(T3);
					}
				}
			}

			public void TestGenericArgs<T2>() where T2 : List<T2>
			{
				ZZ<T2>(null);
				ZZ2<object>(null);
				void Nop<T>(T data)
				{
				}
				void ZZ<T3>(T3 t3) where T3 : T2
				{
					Nop<List<T2>>(t3);
					ZZ1<T3>(t3);
					ZZ3();
					void ZZ3()
					{
						Nop<List<T2>>(t3);
					}
				}
				void ZZ1<T3>(T3 t3)
				{
					Nop<List<T2>>((List<T2>)(object)t3);
				}
				void ZZ2<T3>(T3 t3)
				{
					Nop<List<T2>>((List<T2>)(object)t3);
				}
			}
		}

		private int field;

		private Lazy<object> nonCapturinglocalFunctionInLambda = new Lazy<object>(delegate {
			return CreateValue();

			object CreateValue()
			{
				return null;
			}
		});

		private Lazy<object> capturinglocalFunctionInLambda = new Lazy<object>(delegate {
			int x = 42;
			return Do();

			object Do()
			{
				return CreateValue();

				int CreateValue()
				{
					return x;
				}
			}
		});

		private static void Test(int x)
		{
		}

		private static int GetInt(string a)
		{
			return a.Length;
		}

		private static string GetString(int a)
		{
			return a.ToString();
		}

		public static void StaticContextNoCapture(int length)
		{
			for (int i = 0; i < length; i++) {
				LocalWrite("Hello " + i);
			}

			void LocalWrite(string s)
			{
				Console.WriteLine(s);
			}
		}

		public static void StaticContextSimpleCapture(int length)
		{
			for (int i = 0; i < length; i++) {
				LocalWrite();
			}

			void LocalWrite()
			{
				Console.WriteLine("Hello " + length);
			}
		}

		public static void StaticContextCaptureForLoopVariable(int length)
		{
			int i;
			for (i = 0; i < length; i++) {
				LocalWrite();
			}
			void LocalWrite()
			{
				Console.WriteLine("Hello " + i + "/" + length);
			}
		}

		public void ContextNoCapture()
		{
			for (int i = 0; i < field; i++) {
				LocalWrite("Hello " + i);
			}

			void LocalWrite(string s)
			{
				Console.WriteLine(s);
			}
		}

		public void ContextSimpleCapture()
		{
			for (int i = 0; i < field; i++) {
				LocalWrite();
			}

			void LocalWrite()
			{
				Console.WriteLine("Hello " + field);
			}
		}

		public void ContextCaptureForLoopVariable()
		{
			int i;
			for (i = 0; i < field; i++) {
				LocalWrite();
			}
			void LocalWrite()
			{
				Console.WriteLine("Hello " + i + "/" + field);
			}
		}

		public void CapturedOutsideLoop()
		{
			int i = 0;
			while (i < field) {
				i = GetInt("asdf");
				LocalWrite();
			}

			void LocalWrite()
			{
				Console.WriteLine("Hello " + i + "/" + field);
			}
		}

		public void CapturedInForeachLoop(IEnumerable<string> args)
		{
			foreach (string arg2 in args) {
				string arg = arg2;
				LocalWrite();
				void LocalWrite()
				{
					Console.WriteLine("Hello " + arg);
				}
			}
		}

		public void Overloading()
		{
			Test(5);
			LocalFunctions.Test(2);

			void Test(int x)
			{
				Console.WriteLine("x: {0}", x);
			}
		}

		private void Name()
		{

		}

		private void LocalFunctionHidingMethod()
		{
			Action action = this.Name;
			Name();
			action();

			void Name()
			{

			}
		}

		public void NamedArgument()
		{
			Use(Get(1), Get(2), Get(3));
			Use(Get(1), c: Get(2), b: Get(3));

			int Get(int i)
			{
				return i;
			}

			void Use(int a, int b, int c)
			{
				Console.WriteLine(a + b + c);
			}
		}

		public static Func<int> LambdaInLocalFunction()
		{
			int x = (int)Math.Pow(2.0, 10.0);
			return Create();

			Func<int> Create()
			{
				return () => x;
			}
		}

		public static Func<int> MethodRef()
		{
			int x = (int)Math.Pow(2.0, 10.0);
			Enumerable.Range(1, 100).Select(LocalFunction);
			return null;

			int LocalFunction(int y)
			{
				return x * y;
			}
		}

		public static int Fib(int i)
		{
			return FibHelper(i);

			int FibHelper(int n)
			{
				if (n <= 0) {
					return 0;
				}

				return FibHelper(n - 1) + FibHelper(n - 2);
			}
		}
		public int MutuallyRecursiveLocalFunctions()
		{
			return B(4) + C(3);

			int A(int i)
			{
				if (i > 0) {
					return A(i - 1) + 2 * B(i - 1) + 3 * C(i - 1);
				}
				return 1;
			}

			int B(int i)
			{
				if (i > 0) {
					return 3 * A(i - 1) + B(i - 1);
				}
				return 1;
			}

			int C(int i)
			{
				if (i > 0) {
					return 2 * A(i - 1) + C(i - 1);
				}
				return 1;
			}
		}

		public static int NestedLocalFunctions(int i)
		{
			return A();

			int A()
			{
				double x = Math.Pow(10.0, 2.0);
				return B();

				int B()
				{
					return i + (int)x;
				}
			}
		}

		public static int LocalFunctionInLambda(IEnumerable<int> xs)
		{
			return xs.First(delegate(int x) {
				return Do();

				bool Do()
				{
					return x == 3;
				}
			});
		}

		public static IEnumerable<int> YieldReturn(int n)
		{
			return GetNumbers();

			IEnumerable<int> GetNumbers()
			{
				for (int i = 0; i < n; i++) {
					yield return i;
				}
			}
		}

		public void WriteCapturedParameter(int i)
		{
			ParamWrite();
			Console.WriteLine(i);

			void ParamWrite()
			{
				i++;
			}
		}

		//public static void LocalFunctionInUsing()
		//{
		//	using (MemoryStream memoryStream = new MemoryStream()) {
		//		Do();

		//		void Do()
		//		{
		//			memoryStream.WriteByte(42);
		//		}
		//	}
		//}

		public void NestedCapture1()
		{
			Method1(null);

			Action<object> Method1(Action<object> action)
			{
				return Method1_1;

				void Method1_1(object containerBuilder)
				{
					Method1_2(containerBuilder);
				}

				void Method1_2(object containerBuilder)
				{
					action(containerBuilder);
				}
			}
		}

#if false
		public int NestedCapture2()
		{
			return Method();
			int Method()
			{
				int t0 = 0;
				return ZZZ_0();
				int ZZZ_0()
				{
					t0 = 0;
					var t1 = t0;
					Func<int> zzz2 = () => {
						t0 = 0;
						t1 = 0;
						return ZZZ_1();
					};
					return zzz2();
				}
				int ZZZ_1()
				{
					t0 = 0;
					var t1 = t0;
					Func<int> zzz = () => {
						t0 = 0;
						t1 = 0;
						return 0;
					};
					return zzz();
				}
			}
		}
#endif
	}
}
