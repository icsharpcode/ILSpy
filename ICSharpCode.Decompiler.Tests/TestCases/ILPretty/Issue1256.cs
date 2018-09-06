using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.ILPretty
{
	internal class Issue1256
	{
		public void Method(Enum e, object o, string s)
		{
			int num = (int)(object)e;
			object obj = new object();
			int num2 = (int)obj;
			long num3 = (long)o;
			int num4 = (int)(object)s;
			int num5 = (int)num3;
		}
	}
}
