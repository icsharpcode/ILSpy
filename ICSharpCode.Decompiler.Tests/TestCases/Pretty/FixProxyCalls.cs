using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.Tests.TestCases.ILPretty
{
	internal class A
	{
		protected internal virtual Task<string> Test(string test)
		{
			return Task.Run((Func<string>)(() => test.ToUpper()));
		}
	}

	internal class B : A
	{
		protected internal override async Task<string> Test(string test)
		{
			return await base.Test(test);
		}
	}

	internal class C
	{
		protected internal virtual string Test(string test)
		{
			return string.Join(test, "fsdf");
		}
	}

	internal class D : C
	{
		protected internal IEnumerable<string> Test2(string test)
		{
			yield return base.Test(test);
		}
	}

	internal class E
	{
		protected internal virtual string Test(string test)
		{
			return string.Join(test, "fsdf");
		}
	}

	internal class F : E
	{
		protected internal override string Test(string test)
		{
			Func<string, string> func = (Func<string, string>)((string a) => base.Test(a));
			test = string.Join(test, "aa");
			return func(test);
		}
	}
}