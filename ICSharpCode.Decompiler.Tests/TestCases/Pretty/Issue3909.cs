using System;
using System.Collections.Generic;

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

		public class Node
		{
		}

		public abstract class ConstrainedBase
		{
			public virtual T? ClassType<T>(T? value) where T : Node
			{
				return value;
			}

			public virtual T? DelegateType<T>(T? value) where T : Delegate
			{
				return value;
			}

			public virtual T? EnumType<T>(T? value) where T : Enum
			{
				return value;
			}

			public virtual T? InterfaceType<T>(T? value) where T : IDisposable
			{
				return value;
			}

			public virtual List<T?> NestedGeneric<T>(List<T?> values)
			{
				return values;
			}

			public virtual TItem? PartiallyAnnotated<TItem, TOther>(TItem? value, TOther other)
			{
				return value;
			}
		}

		public sealed class ConstrainedDerived : ConstrainedBase
		{
			public override T? ClassType<T>(T? value) where T : class
			{
				return value;
			}

			public override T? DelegateType<T>(T? value) where T : class
			{
				return value;
			}

			public override T? EnumType<T>(T? value) where T : default
			{
				return value;
			}

			public override T? InterfaceType<T>(T? value) where T : default
			{
				return value;
			}

			public override List<T?> NestedGeneric<T>(List<T?> values) where T : default
			{
				return values;
			}

			public override TItem? PartiallyAnnotated<TItem, TOther>(TItem? value, TOther other) where TItem : default
			{
				return value;
			}
		}

		public class ClassTypeChainBase<TOuter> where TOuter : Node
		{
			public virtual T? Chained<T>(T? value) where T : TOuter
			{
				return value;
			}
		}

		public sealed class ClassTypeChainDerived<TOuter> : ClassTypeChainBase<TOuter> where TOuter : Node
		{
			public override T? Chained<T>(T? value) where T : class
			{
				return value;
			}
		}

		public class ReferenceTypeChainBase<TOuter> where TOuter : class
		{
			public virtual T? Chained<T>(T? value) where T : TOuter
			{
				return value;
			}
		}

		public sealed class ReferenceTypeChainDerived<TOuter> : ReferenceTypeChainBase<TOuter> where TOuter : class
		{
			public override T? Chained<T>(T? value) where T : default
			{
				return value;
			}
		}

		public class ContainerBase<TOuter> where TOuter : class
		{
			public virtual TOuter? Pick<TItem>(TItem item)
			{
				return null;
			}
		}

		public sealed class ContainerDerived<TOuter> : ContainerBase<TOuter> where TOuter : class
		{
			public override TOuter? Pick<TItem>(TItem item)
			{
				return null;
			}
		}
#if CS130
		public class BaseWithAllowsRefStruct
		{
			public virtual void Annotated<T>(T? value) where T : allows ref struct
			{
			}

			public virtual void Plain<T>(T value) where T : allows ref struct
			{
			}
		}

		public class DerivedWithAllowsRefStruct : BaseWithAllowsRefStruct
		{
			public override void Annotated<T>(T? value) where T : default
			{
			}

			public override void Plain<T>(T value)
			{
			}
		}
#endif
	}
}
