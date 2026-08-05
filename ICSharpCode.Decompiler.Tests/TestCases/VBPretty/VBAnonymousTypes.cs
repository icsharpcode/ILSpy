using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using Microsoft.VisualBasic.CompilerServices;

[StandardModule]
public sealed class VBAnonymousTypes
{
	public static void MutableAnonymousType()
	{
		var anon = new {
			Value = 1,
			Name = "test"
		};
		Console.WriteLine(anon.Value);
		Console.WriteLine(anon.Name);
	}

	public static void KeyAnonymousType()
	{
		var anon = new {
			Value = 1,
			Name = "test"
		};
		Console.WriteLine(anon.Value);
		Console.WriteLine(anon.Name);
	}

	public static void AnonymousTypeAsArgument()
	{
		Console.WriteLine(new {
			Value = 1,
			Name = "test"
		}.ToString());
	}

	public static void SelectAnonymousType(IEnumerable<int> items)
	{
		var enumerable = items.Select([SpecialName] (int i) => new {
			Value = i,
			Square = checked(i * i)
		});
		foreach (var item in enumerable)
		{
			Console.WriteLine(item.Value);
			Console.WriteLine(item.Square);
		}
	}

	public static void LetWhereSelect(IEnumerable<int> items)
	{
		var enumerable = from i in items
						 let square = checked(i * i)
						 where square > 4
						 select new { i, square };
		foreach (var item in enumerable)
		{
			Console.WriteLine(item.i);
			Console.WriteLine(item.square);
		}
	}

	public static void JoinSelect(IEnumerable<int> left, IEnumerable<int> right)
	{
		var enumerable = from x in left
						 join y in right on x equals y
						 select new { x, y };
		foreach (var item in enumerable)
		{
			Console.WriteLine(item.x);
			Console.WriteLine(item.y);
		}
	}

	public static void OrderBySelect(IEnumerable<int> items)
	{
		var enumerable = from i in items
						 let doubled = checked(i * 2)
						 orderby doubled
						 select new { i, doubled };
		foreach (var item in enumerable)
		{
			Console.WriteLine(item.i);
			Console.WriteLine(item.doubled);
		}
	}
}
