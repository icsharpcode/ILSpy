using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class Generics
	{
		private class GenericClass<T>
		{
			public void M(out GenericClass<T> self)
			{
				self = this;
			}
		}

		public class BaseClass
		{
		}

		public class DerivedClass : BaseClass
		{
		}

		public T CastToTypeParameter<T>(DerivedClass d) where T : BaseClass
		{
			return (T)(BaseClass)d;
		}

		public T New<T>() where T : new()
		{
			return new T();
		}

		public bool IsNull<T>(T t)
		{
			return t == null;
		}
	}
}
