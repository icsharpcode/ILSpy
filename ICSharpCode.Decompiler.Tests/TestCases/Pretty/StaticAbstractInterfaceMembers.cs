using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty.StaticAbstractInterfaceMembers
{
	public interface I
	{
		static abstract int Capacity { get; }
		static abstract int Count { get; set; }
		static abstract int SetterOnly { set; }
		static abstract event EventHandler E;
		static abstract I CreateI();
	}

	public class X : I
	{
		public static int Capacity { get; }

		public static int Count { get; set; }

		public static int SetterOnly {
			set {
			}
		}

		public static event EventHandler E;

		public static I CreateI()
		{
			return new X();
		}
	}

	public class X2 : I
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

		public static I CreateI()
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
