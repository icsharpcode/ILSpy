using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty.StaticAbstractInterfaceMembers
{
	internal class C : I<C>
	{
		private string _s;

		static C I<C>.P { get; set; }
		static event Action I<C>.E {
			add {

			}
			remove {

			}
		}
		public C(string s)
		{
			_s = s;
		}

		static void I<C>.M(object x)
		{
			Console.WriteLine("Implementation");
		}
		static C I<C>.operator +(C l, C r)
		{
			return new C(l._s + " " + r._s);
		}

		static bool I<C>.operator ==(C l, C r)
		{
			return l._s == r._s;
		}

		static bool I<C>.operator !=(C l, C r)
		{
			return l._s != r._s;
		}

		static implicit I<C>.operator C(string s)
		{
			return new C(s);
		}

		static explicit I<C>.operator string(C c)
		{
			return c._s;
		}
	}

	internal interface I<T> where T : I<T>
	{
		static abstract T P { get; set; }
		static abstract event Action E;
		static abstract void M(object x);
		static abstract T operator +(T l, T r);
		static abstract bool operator ==(T l, T r);
		static abstract bool operator !=(T l, T r);
		static abstract implicit operator T(string s);
		static abstract explicit operator string(T t);
	}

	public interface IAmSimple
	{
		static abstract int Capacity { get; }
		static abstract int Count { get; set; }
		static abstract int SetterOnly { set; }
		static abstract event EventHandler E;
		static abstract IAmSimple CreateI();
	}

	internal interface IAmStatic<T> where T : IAmStatic<T>
	{
		static int f;
		static T P { get; set; }
		static event Action E;
		static void M(object x)
		{
		}
		static IAmStatic<T> operator +(IAmStatic<T> l, IAmStatic<T> r)
		{
			throw new NotImplementedException();
		}
		static IAmStatic()
		{
			f = 42;
		}
	}

	internal interface IAmVirtual<T> where T : IAmVirtual<T>
	{
		static virtual T P { get; set; }
		static virtual event Action E;
		static virtual void M(object x)
		{
		}
		static virtual T operator +(T l, T r)
		{
			throw new NotImplementedException();
		}
		static virtual implicit operator T(string s)
		{
			return default(T);
		}
		static virtual explicit operator string(T t)
		{
			return null;
		}
	}

	internal class Uses
	{
		public static T TestVirtualStaticUse<T>(T a, T b) where T : IAmVirtual<T>
		{
			T.P = a;
			a = "World";
			T.E += null;
			T.E -= null;
			T.M("Hello");
			UseString((string)b);
			return a + b;
		}
		public static IAmStatic<T> TestStaticUse<T>(T a, T b) where T : IAmStatic<T>
		{
			IAmStatic<T>.f = 11;
			IAmStatic<T>.P = a;
			IAmStatic<T>.E += null;
			IAmStatic<T>.E -= null;
			IAmStatic<T>.M("Hello");
			return a + b;
		}
		public static I<T> TestAbstractStaticUse<T>(T a, T b) where T : I<T>
		{
			T.P = a;
			a = "World";
			T.E += null;
			T.E -= null;
			T.M("Hello");
			UseString((string)b);
			return a + b;
		}
		private static void UseString(string a)
		{
		}
	}

	public class X : IAmSimple
	{
		public static int Capacity { get; }

		public static int Count { get; set; }

		public static int SetterOnly {
			set {
			}
		}

		public static event EventHandler E;

		public static IAmSimple CreateI()
		{
			return new X();
		}
	}

	public class X2 : IAmSimple
	{
		public static int Capacity {
			get {
				throw new NotImplementedException();
			}
		}

		public static int Count {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		public static int SetterOnly {
			set {
				throw new NotImplementedException();
			}
		}

		public static event EventHandler E {
			add {
				throw new NotImplementedException();
			}
			remove {
				throw new NotImplementedException();
			}
		}

		public static IAmSimple CreateI()
		{
			throw new NotImplementedException();
		}
	}

	internal class ZOperatorTest
	{

		public interface IGetNext<T> where T : IGetNext<T>
		{
			static abstract T operator ++(T other);
		}

		public struct WrappedInteger : IGetNext<WrappedInteger>
		{
			public int Value;

			public static WrappedInteger operator ++(WrappedInteger other)
			{
				WrappedInteger result = other;
				result.Value++;
				return result;
			}
		}

		public void GenericUse<T>(T t) where T : IGetNext<T>
		{
			++t;
		}
	}
}
