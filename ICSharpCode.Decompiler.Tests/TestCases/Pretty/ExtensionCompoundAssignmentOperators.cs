namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class CompoundAssignmentClassReceiver
	{
		public int Value;
	}

	internal struct CompoundAssignmentValueReceiver
	{
		public int Value;
	}

	internal static class ExtensionCompoundAssignmentOperators
	{
		extension(CompoundAssignmentClassReceiver c)
		{
			public void operator +=(int amount)
			{
				c.Value += amount;
			}
		}

		extension(ref CompoundAssignmentValueReceiver r)
		{
			public void operator +=(int amount)
			{
				r.Value += amount;
			}

			public void operator -=(int amount)
			{
				r.Value -= amount;
			}

			public void operator ++()
			{
				r.Value++;
			}

			public void operator --()
			{
				r.Value--;
			}

			public void operator checked +=(int amount)
			{
				checked
				{
					r.Value += amount;
				}
			}

			public void operator checked ++()
			{
				checked
				{
					r.Value++;
				}
			}
		}
	}

	internal static class ExtensionCompoundAssignmentOperatorsUseSites
	{
		public static void UseStruct(ref CompoundAssignmentValueReceiver r)
		{
			r += 5;
			r -= 3;
			r++;
			r--;
		}

		public static void UseStructChecked(ref CompoundAssignmentValueReceiver r)
		{
			checked
			{
				r += 5;
				r++;
			}
		}

		public static void UseClass(CompoundAssignmentClassReceiver c)
		{
			c += 5;
		}
	}
}
