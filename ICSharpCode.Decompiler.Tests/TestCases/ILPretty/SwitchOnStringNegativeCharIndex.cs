namespace ICSharpCode.Decompiler.Tests.TestCases.ILPretty
{
	internal class SwitchOnStringNegativeCharIndex
	{
		public static int M(string s)
		{
			if (s != null)
			{
				int length = s.Length;
				if (length == 1)
				{
					switch (s[-1])
					{
						case 'a':
							return 1;
						case 'b':
							return 2;
						case 'c':
							return 3;
						case 'd':
							return 4;
						case 'e':
							return 5;
						case 'f':
							return 6;
						case 'g':
							return 7;
						case 'h':
							return 8;
					}
				}
			}
			return 0;
		}
	}
}
