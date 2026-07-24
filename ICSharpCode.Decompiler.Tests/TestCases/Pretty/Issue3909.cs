#nullable enable
namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class Issue3909
	{
		public abstract class Base
		{
			public virtual T? Unconstrained<T>(T? value)
			{
				return value;
			}

			public virtual T? ReferenceType<T>(T? value) where T : class
			{
				return value;
			}

			public virtual T? ReferenceTypeNullable<T>(T? value) where T : class?
			{
				return value;
			}

			public virtual T? NotNull<T>(T? value) where T : notnull
			{
				return value;
			}

			public virtual T? ValueType<T>(T? value) where T : struct
			{
				return value;
			}

			public virtual U? ReturnOnly<U>()
			{
				return default(U);
			}

			public virtual T?[] Nested<T>(T?[] values)
			{
				return values;
			}

			public virtual T Identity<T>(T value)
			{
				return value;
			}
		}

		public sealed class Derived : Base
		{
			public override T? Unconstrained<T>(T? value) where T : default
			{
				return value;
			}

			public override T? ReferenceType<T>(T? value) where T : class
			{
				return value;
			}

			public override T? ReferenceTypeNullable<T>(T? value) where T : class
			{
				return value;
			}

			public override T? NotNull<T>(T? value) where T : default
			{
				return value;
			}

			public override T? ValueType<T>(T? value)
			{
				return value;
			}

			public override U? ReturnOnly<U>() where U : default
			{
				return default(U);
			}

			public override T?[] Nested<T>(T?[] values) where T : default
			{
				return values;
			}

			public override T Identity<T>(T value)
			{
				return value;
			}
		}

		public interface IRoundTrip
		{
			T? RoundTrip<T>(T? value);
		}

		public class ExplicitImpl : IRoundTrip
		{
			T? IRoundTrip.RoundTrip<T>(T? value) where T : default
			{
				return value;
			}
		}
	}
}
