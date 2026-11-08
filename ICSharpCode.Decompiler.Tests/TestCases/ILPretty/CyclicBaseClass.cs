namespace CyclicBaseClass
{
	public abstract class Expanding<T> : Expanding<Expanding<T>>
	{
	}

	public static class Extensions
	{
		public static void Deconstruct(this SelfDerived value, out int a, out int b)
		{
			a = 1;
			b = 2;
		}

		public static void Deconstruct<T>(this Expanding<T> value, out int a, out int b)
		{
			a = 1;
			b = 2;
		}
	}

	public static class NestedDeconstruction
	{
		public static int Sum(Pair<SelfDerived> pair)
		{
			var (num4, (num5, num6)) = pair;
			return num4 + num5 + num6;
		}

		public static int Sum(Pair<Expanding<int>> pair)
		{
			var (num4, (num5, num6)) = pair;
			return num4 + num5 + num6;
		}
	}

	public abstract class Pair<T>
	{
		public abstract void Deconstruct(out int first, out T second);
	}

	public abstract class SelfDerived : SelfDerived
	{
	}
}
