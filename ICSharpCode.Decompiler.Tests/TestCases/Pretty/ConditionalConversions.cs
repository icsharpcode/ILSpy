using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class ConditionalConversions
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

		public enum Color
		{
			None,
			Red,
			Green
		}

		private int? nullableField;
		private long? nullableLongField;
		private Base baseField;
		private object objectField;
		private Wrapper wrapperField;
		private Color colorField;
		private Action actionField;

		public void Assignments(bool b)
		{
			nullableField = (b ? new int?(1) : ((int?)null));
			nullableLongField = (b ? new long?(1L) : ((long?)null));
			baseField = (b ? ((Base)new Derived1()) : ((Base)new Derived2()));
			objectField = (b ? ((object)1) : "hello");
			wrapperField = (b ? ((Wrapper)"yes") : ((Wrapper)1));
			colorField = (b ? Color.Red : Color.None);
			actionField = (b ? new Action(M1) : new Action(M2));
		}

		public void Arguments(bool b)
		{
			UseNullable(b ? new int?(1) : ((int?)null));
			UseBase(b ? ((Base)new Derived1()) : ((Base)new Derived2()));
			UseWrapper(b ? ((Wrapper)"yes") : ((Wrapper)1));
			UseAction(b ? new Action(M1) : new Action(M2));
		}

		public int LocalsUsedTwice(bool b)
		{
			int? num = (b ? new int?(1) : ((int?)null));
			Base value = (b ? ((Base)new Derived1()) : ((Base)new Derived2()));
			UseBase(value);
			UseBase(value);
			return num.GetValueOrDefault() + (num.HasValue ? 1 : 0);
		}

		public void NestedConditional(bool b1, bool b2)
		{
			nullableField = (b1 ? new int?(b2 ? 1 : 2) : ((int?)null));
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
