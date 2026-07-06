using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class CS9_TargetTypedConditional
	{
		public class Base
		{
		}

		public class Derived1 : Base
		{
		}

		public class Derived2 : Base
		{
		}

		public interface IShape
		{
		}

		public class Circle : IShape
		{
		}

		public class Square : IShape
		{
		}

		public struct Wrapper
		{
			public string Text;

			public static implicit operator Wrapper(string value)
			{
				return new Wrapper {
					Text = value
				};
			}

			public static implicit operator Wrapper(int value)
			{
				return new Wrapper {
					Text = value.ToString()
				};
			}
		}

		private int? nullableField;
		private long? nullableLongField;
		private Base baseField;
		private Wrapper wrapperField;
		private Action actionField;

		public int? GetNullableInt(bool b)
		{
			return b ? 1 : null;
		}

		public long? GetNullableLong(bool b)
		{
			return b ? 1L : null;
		}

		public Base GetBase(bool b)
		{
			return b ? new Derived1() : new Derived2();
		}

		public IShape GetShape(bool b)
		{
			return b ? new Circle() : new Square();
		}

		public Wrapper GetWrapper(bool b)
		{
			return b ? "yes" : 1;
		}

		public Action GetAction(bool b)
		{
			return b ? M1 : M2;
		}

		public int? GetNested(bool b1, bool b2)
		{
			return b1 ? (b2 ? 1 : 2) : null;
		}

		public void Assignments(bool b)
		{
			nullableField = (b ? 1 : null);
			nullableLongField = (b ? 1L : null);
			baseField = (b ? new Derived1() : new Derived2());
			wrapperField = (b ? "yes" : 1);
			actionField = (b ? M1 : M2);
		}

		public void Arguments(bool b)
		{
			UseNullable(b ? 1 : null);
			UseBase(b ? new Derived1() : new Derived2());
			UseWrapper(b ? "yes" : 1);
			UseAction(b ? M1 : M2);
		}

		private static void M1()
		{
		}

		private static void M2()
		{
		}

		private void UseNullable(int? value)
		{
		}

		private void UseBase(Base value)
		{
		}

		private void UseWrapper(Wrapper value)
		{
		}

		private void UseAction(Action value)
		{
		}
	}
}
