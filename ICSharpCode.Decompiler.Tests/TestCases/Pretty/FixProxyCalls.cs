using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.Tests.TestCases.ILPretty
{
	internal class AwaitDelegatingHandler
	{
		protected internal virtual Task<string> Test(string test)
		{
			return Task.Run((Func<string>)(() => test.ToUpper()));
		}
	}

	internal class AwaitTestHandler : AwaitDelegatingHandler
	{
		protected internal override async Task<string> Test(string test)
		{
			return await base.Test(test);
		}
	}

	internal class YieldDelegatingHandler
	{
		protected internal virtual string Test(string test)
		{
			return string.Join(test, "fsdf");
		}
	}

	internal class YieldTestHandler : YieldDelegatingHandler
	{
		protected internal IEnumerable<string> Test2(string test)
		{
			yield return base.Test(test);
		}
	}

	internal class LambdaDelegatingHandler
	{
		protected internal virtual string Test(string test)
		{
			return string.Join(test, "fsdf");
		}
	}

	internal class LambdaHandler : LambdaDelegatingHandler
	{
		protected internal override string Test(string test)
		{
			Func<string> func = (Func<string>)(() => base.Test(test));
			return func();
		}
	}
}