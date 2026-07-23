using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class LambdaReturnTypes
	{
		public object WideningReturnType()
		{
			var result = object (ref int x) => "boxed";
			Console.WriteLine("no inlining");
			return result;
		}

		public int InferredReturnTypeOmitted()
		{
			var anon = (ref int x) => x + 1;
			Console.WriteLine("no inlining");
			int arg = 21;
			return anon(ref arg);
		}

		public int VoidReturnType()
		{
			var anon = void (ref int x) => x++;
			Console.WriteLine("no inlining");
			int arg = 3;
			anon(ref arg);
			return arg;
		}

		public int RefReturn()
		{
			var anon = (ref int x) => ref x;
			Console.WriteLine("no inlining");
			int arg = 1;
			anon(ref arg) = 5;
			return arg;
		}
	}
}
