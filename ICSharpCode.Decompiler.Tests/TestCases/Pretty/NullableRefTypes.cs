#nullable enable
using System;
using System.Collections.Generic;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class T01_NullableRefTypes
	{
		private string field_string;
		private string? field_nullable_string;
		private dynamic? field_nullable_dynamic;

		private Dictionary<string?, string> field_generic;
		private Dictionary<int, string?[]> field_generic2;
		private Dictionary<int?, string?[]> field_generic3;
		private KeyValuePair<string?, string> field_generic_value_type;
		private KeyValuePair<string?, string>? field_generic_nullable_value_type;
		private (string, string?, string) field_tuple;
		private string[]?[] field_array;
		private Dictionary<(string, string?), (int, string[]?, string?[])> field_complex;

		public (string A, dynamic? B) PropertyNamedTuple {
			get {
				throw new NotImplementedException();
			}
		}

		public (string A, dynamic? B) this[(dynamic? C, string D) weirdIndexer] {
			get {
				throw new NotImplementedException();
			}
		}

		public int GetLength1(string[] arr)
		{
			return field_string.Length + arr.Length;
		}

		public int GetLength2(string[]? arr)
		{
			return field_nullable_string!.Length + arr!.Length;
		}

		public int? GetLength3(string[]? arr)
		{
			return field_nullable_string?.Length + arr?.Length;
		}

		public void GenericNullable<T1, T2>((T1?, T1, T2, T2?, T1, T1?) x) where T1 : class where T2 : struct
		{
		}

		public T ByRef<T>(ref T t)
		{
			return t;
		}

		public void CallByRef(ref string a, ref string? b)
		{
			ByRef(ref a).ToString();
			ByRef(ref b)!.ToString();
		}

		public void Constraints<UC, C, CN, NN, S, SN, D, DN, NND>() where C : class where CN : class? where NN : notnull where S : struct where D : IDisposable where DN : IDisposable? where NND : notnull, IDisposable
		{
		}
	}

	public class T02_EverythingIsNullableInHere
	{
		private string? field1;
		private object? field2;
		// value types are irrelevant for the nullability attributes:
		private int field3;
		private int? field4;

		public string? Property {
			get;
			set;
		}
		public event EventHandler? Event;

		public static int? NullConditionalOperator(T02_EverythingIsNullableInHere? x)
		{
			// This code throws if `x != null && x.field1 == null`.
			// But we can't decompile it to the warning-free "x?.field1!.Length",
			// because of https://github.com/dotnet/roslyn/issues/43659
			return x?.field1.Length;
		}
	}

	public class T03_EverythingIsNotNullableInHere
	{
		private string field1;
		private object field2;
		// value types are irrelevant for the nullability attributes:
		private int field3;
		private int? field4;

		public string Property {
			get;
			set;
		}
		public event EventHandler Event;
	}

	public class T04_Dictionary<TKey, TValue> where TKey : notnull
	{
		private struct Entry
		{
			public TKey key;
			public TValue value;
		}

		private int[]? _buckets;
		private Entry[]? _entries;
		private IEqualityComparer<TKey>? _comparer;
	}
}
