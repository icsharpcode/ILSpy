using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty.StaticAbstractInterfaceMembers
{
	internal interface I<T> where T : I<T>
	{
		static abstract T P { get; set; }
		static abstract event Action E;
		static abstract void M();
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
		static T P { get; set; }
		static event Action E;
		static void M()
		{
		}
		static T operator +(IAmStatic<T> l, T r)
		{
			throw new NotImplementedException();
		}
	}

	internal interface IAmVirtual<T> where T : IAmVirtual<T>
	{
		static virtual T P { get; set; }
		static virtual event Action E;
		static virtual void M()
		{
		}
		static virtual T operator +(T l, T r)
		{
			throw new NotImplementedException();
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
