using System.Collections.Generic;

namespace ICSharpCode.Decompiler.Tests.TestCases.Ugly
{
	internal class NoNewOfT<TOnType> where TOnType : new()
	{
		public static TOnType CreateTOnType()
		{
			return new TOnType();
		}

		public static T CreateUnconstrainedT<T>() where T : new()
		{
			return new T();
		}

		public static T CreateClassT<T>() where T : class, new()
		{
			return new T();
		}

		public static T CollectionInitializer<T>() where T : IList<int>, new()
		{
			return new T() { 1, 2, 3, 4, 5 };
		}
	}
}
