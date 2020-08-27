using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Correctness
{
	class NullPropagation
	{
		static void Main()
		{
			new NullPropagation().TestNotCoalescing();
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
	}
}
