using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Correctness
{
	class NullPropagation
	{
		static void Main()
		{
			new NullPropagation().TestNotCoalescing();
#if CS72
			new NullPropagation().TestRefStructNotCoalescing();
#endif
		}

		class MyClass
		{
			public string Text;
		}

		void TestNotCoalescing()
		{
			Console.WriteLine("TestNotCoalescing:");
			Console.WriteLine(NotCoalescing(null));
			Console.WriteLine(NotCoalescing(new MyClass()));
		}

		string NotCoalescing(MyClass c)
		{
			return c != null ? c.Text : "Hello";
		}

#if CS72
		ref struct ByRefLikeStruct
		{
			public int Value;
		}

		class RefStructHolder
		{
			public ByRefLikeStruct Get()
			{
				ByRefLikeStruct result = default(ByRefLikeStruct);
				result.Value = 42;
				return result;
			}
		}

		void TestRefStructNotCoalescing()
		{
			Console.WriteLine("TestRefStructNotCoalescing:");
			Console.WriteLine(RefStructNotCoalescing(null).Value);
			Console.WriteLine(RefStructNotCoalescing(new RefStructHolder()).Value);
		}

		// A by-ref-like return type cannot be turned into a ?. / ?? null-propagation: a ref struct
		// cannot be wrapped in Nullable<T>, so that form does not compile.
		ByRefLikeStruct RefStructNotCoalescing(RefStructHolder holder)
		{
			return holder != null ? holder.Get() : default(ByRefLikeStruct);
		}
#endif
	}
}
