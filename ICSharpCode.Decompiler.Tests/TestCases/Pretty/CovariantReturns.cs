namespace ICSharpCode.Decompiler.Tests.TestCases.CovariantReturns
{
	public abstract class Base
	{
		public abstract Base Instance { get; }

		public abstract Base this[int index] { get; }

		public virtual Base Build()
		{
			throw null;
		}

		protected abstract Base SetParent(object parent);
	}

	public class Derived : Base
	{
		public override Derived Instance { get; }

		public override Derived this[int index] {
			get {
				throw null;
			}
		}

		public override Derived Build()
		{
			throw null;
		}

		protected override Derived SetParent(object parent)
		{
			throw null;
		}
	}

	public class UseSites
	{
		public Base Test(Base x)
		{
			return x.Build();
		}

		public Derived Test(Derived x)
		{
			return x.Build();
		}
	}
}