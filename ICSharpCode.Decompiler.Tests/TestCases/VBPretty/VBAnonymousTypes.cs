using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
#if LEGACY_VBC
using System.Text;
#endif

using Microsoft.VisualBasic.CompilerServices;

// A VB anonymous type. Its properties are settable and only those declared 'Key'
// take part in Equals and GetHashCode, so it cannot be written as a C# anonymous
// type and is declared here instead.
[DebuggerDisplay("Value={Value}, Name={Name}")]
[CompilerGenerated]
internal sealed class VB_AnonymousType_0<T0, T1>
{
#if !OPT && !LEGACY_VBC
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
	private T0 _Value;

#if !OPT && !LEGACY_VBC
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
	private T1 _Name;

	public T0 Value {
		get {
			return _Value;
		}
		set {
			_Value = value;
		}
	}

	public T1 Name {
		get {
			return _Name;
		}
		set {
			_Name = value;
		}
	}

#if !OPT && !LEGACY_VBC
	[DebuggerHidden]
#endif
	public VB_AnonymousType_0(T0 Value, T1 Name)
	{
		_Value = Value;
		_Name = Name;
	}

#if !OPT && !LEGACY_VBC
	[DebuggerHidden]
#endif
	public override string ToString()
	{
#if LEGACY_VBC
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("{ ");
		stringBuilder.AppendFormat("{0} = {1}, ", "Value", _Value);
		stringBuilder.AppendFormat("{0} = {1} ", "Name", _Name);
		stringBuilder.Append("}");
		return stringBuilder.ToString();
#else
		return string.Format(null, "{{ Value = {0}, Name = {1} }}", new object[2] { _Value, _Name });
#endif
	}
}
[StandardModule]
public sealed class VBAnonymousTypes
{
	public static void MutableAnonymousType()
	{
		VB_AnonymousType_0<int, string> obj = new VB_AnonymousType_0<int, string>(1, "test");
		Console.WriteLine(obj.Value);
		Console.WriteLine(obj.Name);
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
