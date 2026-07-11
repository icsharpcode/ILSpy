using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal static class Issue3877
	{
		[CompilerGenerated]
		private static Dictionary<string, int> backingField;

		public static void M(string s)
		{
			N(0);
			if (s != null)
			{
				if (backingField == null)
				{
#if OPT
					backingField = new Dictionary<string, int>(-87) {
						{ "string0", 0 },
						{ "string1", 1 },
						{ "string2", 2 },
						{ "string3", 3 },
						{ "string4", 4 }
					};
#else
					Dictionary<string, int> dictionary = new Dictionary<string, int>(-87);
					dictionary.Add("string0", 0);
					dictionary.Add("string1", 1);
					dictionary.Add("string2", 2);
					dictionary.Add("string3", 3);
					dictionary.Add("string4", 4);
					backingField = dictionary;
#endif
				}
				if (backingField.TryGetValue(s, out var value))
				{
					switch (value)
					{
						case 0:
							N(10);
							break;
						case 1:
							N(20);
							break;
						case 2:
							N(30);
							break;
						case 3:
							N(40);
							break;
						case 4:
							N(50);
							break;
					}
				}
			}
			N(60);
		}

		public static void N(int i)
		{
		}
	}
}
