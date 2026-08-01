using System.Collections;

namespace ICSharpCode.Decompiler.Tests.TestCases.ILPretty
{
	public class Issue1638
	{
		private int state;

		public IEnumerator Test(object arg1, object arg2)
		{
			try
			{
				yield return arg1;
				yield return state;
			}
			finally
			{
				state = 0;
			}
			yield return arg2;
		}
	}
}
