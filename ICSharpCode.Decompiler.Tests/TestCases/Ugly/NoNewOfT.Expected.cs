using System;
using System.Collections.Generic;

namespace ICSharpCode.Decompiler.Tests.TestCases.Ugly
{
	internal class NoNewOfT<TOnType> where TOnType : new()
	{
		public static TOnType CreateTOnType()
		{
#if !ROSLYN
#if OPT
			if (default(TOnType) != null)
			{
				return default(TOnType);
			}
			return Activator.CreateInstance<TOnType>();
#else
			return (default(TOnType) == null) ? Activator.CreateInstance<TOnType>() : default(TOnType);
#endif
#else
			return Activator.CreateInstance<TOnType>();
#endif
		}

		public static T CreateUnconstrainedT<T>() where T : new()
		{
#if !ROSLYN
#if OPT
			if (default(T) != null)
			{
				return default(T);
			}
			return Activator.CreateInstance<T>();
#else
			return (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
#endif
#else
			return Activator.CreateInstance<T>();
#endif
		}

		public static T CreateClassT<T>() where T : class, new()
		{
			return Activator.CreateInstance<T>();
		}

		public static T CollectionInitializer<T>() where T : IList<int>, new()
		{
#if ROSLYN
			T result = Activator.CreateInstance<T>();
			result.Add(1);
			result.Add(2);
			result.Add(3);
			result.Add(4);
			result.Add(5);
			return result;
#else
			T val = ((default(T) == null) ? Activator.CreateInstance<T>() : default(T));
			val.Add(1);
			val.Add(2);
			val.Add(3);
			val.Add(4);
			val.Add(5);
			return val;
#endif
		}
	}
}
