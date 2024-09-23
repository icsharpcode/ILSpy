#if !OPT
using System;
#endif

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class VariableNaming
	{
		private enum MyEnum
		{
			VALUE1 = 1,
			VALUE2
		}

		private class C
		{
			public string Name;
			public string Text;
		}

		private void Test(string text, C c)
		{
#if CS70
			_ = c.Name;
#else
			string name = c.Name;
#endif
		}

		private void Test2(string text, C c)
		{
#if CS70
			_ = c.Text;
#else
			string text2 = c.Text;
#endif
		}

#if !OPT
		private void Issue1841()
		{
			C gen1 = new C();
			C gen2 = new C();
			C gen3 = new C();
			C gen4 = new C();
		}

		private void Issue1881()
		{
#pragma warning disable CS0219
			MyEnum enumLocal1 = MyEnum.VALUE1;
			MyEnum enumLocal2 = (MyEnum)0;
			enumLocal2 = MyEnum.VALUE1;
			object enumLocal3 = MyEnum.VALUE2;
			object enumLocal4 = new object();
			enumLocal4 = MyEnum.VALUE2;
			ValueType enumLocal5 = MyEnum.VALUE1;
			ValueType enumLocal6 = (MyEnum)0;
			enumLocal6 = MyEnum.VALUE2;
#pragma warning restore CS0219
		}
#endif

		private static void NestedForLoopTest(int sizeX, int sizeY, int[] array)
		{
			for (int y = 0; y < sizeY; y++)
			{
				for (int x = 0; x < sizeX; x++)
				{
					array[y * sizeX + x] = 0;
				}
			}
#if !EXPECTED_OUTPUT || (LEGACY_CSC && !OPT)
			for (int y = 0; y < sizeY; y++)
			{
				for (int x = 0; x < sizeX; x++)
				{
					array[y * sizeX + x] = 1;
				}
			}
#else
			for (int i = 0; i < sizeY; i++)
			{
				for (int j = 0; j < sizeX; j++)
				{
					array[i * sizeX + j] = 1;
				}
			}
#endif
		}

		private static void NestedForLoopTest2()
		{
			for (int i = 0; i < 10; i++)
			{
				Nop(i);
			}
#if EXPECTED_OUTPUT && !(LEGACY_CSC && !OPT)
			for (int j = 0; j < 10; j++)
			{
				Nop(j);
			}

			for (int k = 0; k < 10; k++)
			{
				Nop(k);
			}

			for (int l = 0; l < 10; l++)
			{
				Nop(l);
			}

			for (int m = 0; m < 10; m++)
			{
				for (int n = 0; n < 10; n++)
				{
					Nop(n);
				}
			}

			for (int num = 0; num < 10; num++)
			{
				for (int num2 = 0; num2 < 10; num2++)
				{
					Nop(num2);
				}
			}
#else
			for (int i = 0; i < 10; i++)
			{
				Nop(i);
			}

			for (int i = 0; i < 10; i++)
			{
				Nop(i);
			}

			for (int i = 0; i < 10; i++)
			{
				Nop(i);
			}

			for (int i = 0; i < 10; i++)
			{
				for (int j = 0; j < 10; j++)
				{
					Nop(j);
				}
			}

			for (int i = 0; i < 10; i++)
			{
				for (int j = 0; j < 10; j++)
				{
					Nop(j);
				}
			}
#endif
		}

		private static void Nop(int v)
		{
		}
	}
}
