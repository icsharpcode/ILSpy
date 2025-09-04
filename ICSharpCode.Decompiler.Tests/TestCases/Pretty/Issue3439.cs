using System;
using System.Collections.Generic;

internal class VariableScopeTest
{
	private class Item
	{
		public long Key;

		public string Value;
	}

	private void Test(List<string> list1)
	{
		AddAction(delegate (List<Item> list2) {
			long num2 = 1L;
			foreach (string item in list1)
			{
				list2.Add(new Item {
					Key = num2,
					Value = item
				});
				num2++;
			}
		});
		int num = 1;
		foreach (string item2 in list1)
		{
			int preservedName = num;
			num++;
			AddAction(item2, delegate (object x) {
				SetValue(x, preservedName);
			});
		}
	}

	private static void AddAction(Action<List<Item>> action)
	{
	}

	private static void AddAction(string name, Action<object> action)
	{
	}

	private static void SetValue(object obj, int value)
	{
	}
}
