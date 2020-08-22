// Copyright (c) 2020 Siegfried Pammer
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public static class DeconstructionExt
	{
		public static void Deconstruct<K, V>(this KeyValuePair<K, V> pair, out K key, out V value)
		{
			key = pair.Key;
			value = pair.Value;
		}
	}

	internal class DeconstructionTests
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct MyInt
		{
			public static implicit operator int(MyInt x)
			{
				return 0;
			}

			public static implicit operator MyInt(int x)
			{
				return default(MyInt);
			}
		}

		private class DeconstructionSource<T, T2>
		{
			public int Dummy {
				get;
				set;
			}

			public void Deconstruct(out T a, out T2 b)
			{
				a = default(T);
				b = default(T2);
			}
		}

		private class DeconstructionSource<T, T2, T3>
		{
			public int Dummy {
				get;
				set;
			}

			public void Deconstruct(out T a, out T2 b, out T3 c)
			{
				a = default(T);
				b = default(T2);
				c = default(T3);
			}
		}

		private class AssignmentTargets
		{
			public int IntField;
			public long LongField;
			public float FloatField;
			public double DoubleField;
			public decimal DecimalField;

			public MyInt MyField;
			public MyInt? NMyField;

			public string StringField;
			public object ObjectField;
			public dynamic DynamicField;

			public int? NullableIntField;

			public MyInt MyIntField;

			public MyInt? NullableMyIntField;

			public int Int {
				get;
				set;
			}

			public long Long {
				get;
				set;
			}

			public float Float {
				get;
				set;
			}

			public double Double {
				get;
				set;
			}

			public decimal Decimal {
				get;
				set;
			}

			public string String {
				get;
				set;
			}

			public object Object {
				get;
				set;
			}

			public dynamic Dynamic {
				get;
				set;
			}

			public int? NInt {
				get;
				set;
			}

			public MyInt My {
				get;
				set;
			}

			public MyInt? NMy {
				get;
				set;
			}

			public static MyInt StaticMy {
				get;
				set;
			}

			public static MyInt? StaticNMy {
				get;
				set;
			}
		}

		private DeconstructionSource<T, T2> GetSource<T, T2>()
		{
			return null;
		}

		private DeconstructionSource<T, T2, T3> GetSource<T, T2, T3>()
		{
			return null;
		}

		private ref T GetRef<T>()
		{
			throw new NotImplementedException();
		}

		private (T, T2) GetTuple<T, T2>()
		{
			return default((T, T2));
		}

		private (T, T2, T3) GetTuple<T, T2, T3>()
		{
			return default((T, T2, T3));
		}

		private AssignmentTargets Get(int i)
		{
			return null;
		}

		public void LocalVariable_NoConversion_Custom()
		{
			var (myInt3, x) = GetSource<MyInt?, MyInt>();
			Console.WriteLine(myInt3);
			Console.WriteLine(x);
		}

		public void LocalVariable_NoConversion_Tuple()
		{
			var (myInt, x) = GetTuple<MyInt?, MyInt>();
			Console.WriteLine(myInt);
			Console.WriteLine(x);
		}

		public void LocalVariable_NoConversion_Custom_DiscardFirst()
		{
			var (_, x, value) = GetSource<MyInt?, MyInt, int>();
			Console.WriteLine(x);
			Console.WriteLine(value);
		}

		// currently we detect deconstruction, iff the first element is not discarded
		//public void LocalVariable_NoConversion_Tuple_DiscardFirst()
		//{
		//	var (_, x, value) = GetTuple<MyInt?, MyInt, int>();
		//	Console.WriteLine(x);
		//	Console.WriteLine(value);
		//}

		public void LocalVariable_NoConversion_Custom_DiscardLast()
		{
			var (myInt3, x, _) = GetSource<MyInt?, MyInt, int>();
			Console.WriteLine(myInt3);
			Console.WriteLine(x);
		}

		public void LocalVariable_NoConversion_Tuple_DiscardLast()
		{
			var (myInt, x, _) = GetTuple<MyInt?, MyInt, int>();
			Console.WriteLine(myInt);
			Console.WriteLine(x);
		}

		public void LocalVariable_NoConversion_Custom_DiscardSecond()
		{
			var (myInt3, _, value) = GetSource<MyInt?, MyInt, int>();
			Console.WriteLine(myInt3);
			Console.WriteLine(value);
		}

		public void LocalVariable_NoConversion_Tuple_DiscardSecond()
		{
			var (myInt, _, value) = GetTuple<MyInt?, MyInt, int>();
			Console.WriteLine(myInt);
			Console.WriteLine(value);
		}

		public void LocalVariable_NoConversion_Custom_ReferenceTypes()
		{
			var (value, value2) = GetSource<string, string>();
			Console.WriteLine(value);
			Console.WriteLine(value2);
		}

		public void LocalVariable_NoConversion_Tuple_ReferenceTypes()
		{
			var (value, value2) = GetTuple<string, string>();
			Console.WriteLine(value);
			Console.WriteLine(value2);
		}

		public void LocalVariable_IntToLongConversion_Custom()
		{
			int value;
			long value2;
			(value, value2) = GetSource<int, int>();
			Console.WriteLine(value);
			Console.WriteLine(value2);
		}

		public void LocalVariable_IntToLongConversion_Tuple()
		{
			int value;
			long value2;
			(value, value2) = GetTuple<int, int>();
			Console.WriteLine(value);
			Console.WriteLine(value2);
		}

		public void LocalVariable_FloatToDoubleConversion_Custom()
		{
			int value;
			double value2;
			(value, value2) = GetSource<int, float>();
			Console.WriteLine(value);
			Console.WriteLine(value2);
		}

		public void LocalVariable_FloatToDoubleConversion_Tuple()
		{
			int value;
			double value2;
			(value, value2) = GetTuple<int, float>();
			Console.WriteLine(value);
			Console.WriteLine(value2);
		}

		// dynamic conversion is currently not supported
		//public void LocalVariable_ImplicitReferenceConversion_Custom()
		//{
		//	object value;
		//	dynamic value2;
		//	(value, value2) = GetSource<string, string>();
		//	Console.WriteLine(value);
		//	value2.UseMe();
		//}

		//public void LocalVariable_ImplicitReferenceConversion_Tuple()
		//{
		//	object value;
		//	dynamic value2;
		//	(value, value2) = GetTuple<string, string>();
		//	Console.WriteLine(value);
		//	value2.UseMe();
		//}

		public void LocalVariable_NoConversion_ComplexValue_Custom()
		{
			var (myInt3, x) = new DeconstructionSource<MyInt?, MyInt> {
				Dummy = 3
			};
			Console.WriteLine(myInt3);
			Console.WriteLine(x);
		}

		public void Property_NoConversion_Custom()
		{
			(Get(0).NMy, Get(1).My) = GetSource<MyInt?, MyInt>();
		}

		public void Property_IntToLongConversion_Custom()
		{
			(Get(0).Int, Get(1).Long) = GetSource<int, int>();
		}

		public void Property_FloatToDoubleConversion_Custom()
		{
			(Get(0).Int, Get(1).Double) = GetSource<int, float>();
		}

		// dynamic conversion is not supported
		//public void Property_ImplicitReferenceConversion_Custom()
		//{
		//	(Get(0).Object, Get(1).Dynamic) = GetSource<string, string>();
		//}

		public void Property_NoConversion_Custom_DiscardFirst()
		{
			(_, Get(1).My) = GetSource<MyInt?, MyInt>();
		}

		public void Property_NoConversion_Custom_DiscardLast()
		{
			(Get(0).NMy, _) = GetSource<MyInt?, MyInt>();
		}

		public void Property_NoConversion_Tuple()
		{
			(Get(0).NMy, Get(1).My) = GetTuple<MyInt?, MyInt>();
		}

		public void Property_NoConversion_Tuple_DiscardLast()
		{
			(Get(0).NMy, Get(1).My, _) = GetTuple<MyInt?, MyInt, int>();
		}

		// currently we detect deconstruction, iff the first element is not discarded
		//public void Property_NoConversion_Tuple_DiscardFirst()
		//{
		//	(_, Get(1).My, Get(2).Int) = GetTuple<MyInt?, MyInt, int>();
		//}

		public void Property_NoConversion_Custom_DiscardSecond()
		{
			(Get(0).NMy, _, Get(2).Int) = GetSource<MyInt?, MyInt, int>();
		}

		public void Property_NoConversion_Tuple_DiscardSecond()
		{
			(Get(0).NMy, _, Get(2).Int) = GetTuple<MyInt?, MyInt, int>();
		}

		public void Property_NoConversion_Custom_ReferenceTypes()
		{
			(Get(0).String, Get(1).String) = GetSource<string, string>();
		}

		public void Property_NoConversion_Tuple_ReferenceTypes()
		{
			(Get(0).String, Get(1).String) = GetTuple<string, string>();
		}

		public void Property_IntToLongConversion_Tuple()
		{
			(Get(0).Int, Get(1).Long) = GetTuple<int, int>();
		}

		public void Property_FloatToDoubleConversion_Tuple()
		{
			(Get(0).Int, Get(1).Double) = GetTuple<int, float>();
		}

		public void RefLocal_NoConversion_Custom(out double a)
		{
			(a, GetRef<float>()) = GetSource<double, float>();
		}

		public void RefLocal_NoConversion_Tuple(out double a)
		{
			(a, GetRef<float>()) = GetTuple<double, float>();
		}

		public void RefLocal_FloatToDoubleConversion_Custom(out double a)
		{
			(a, GetRef<double>()) = GetSource<double, float>();
		}

		public void RefLocal_FloatToDoubleConversion_Tuple(out double a)
		{
			(a, GetRef<double>()) = GetTuple<double, float>();
		}

		//public void ArrayAssign_FloatToDoubleConversion_Custom(double[] arr)
		//{
		//	(arr[0], arr[1], arr[2]) = GetSource<double, float, double>();
		//}

		public void Field_NoConversion_Custom()
		{
			(Get(0).IntField, Get(1).IntField) = GetSource<int, int>();
		}

		public void Field_NoConversion_Tuple()
		{
			(Get(0).IntField, Get(1).IntField) = GetTuple<int, int>();
		}

		public void Field_IntToLongConversion_Custom()
		{
			(Get(0).IntField, Get(1).LongField) = GetSource<int, int>();
		}

		public void Field_IntToLongConversion_Tuple()
		{
			(Get(0).IntField, Get(1).LongField) = GetTuple<int, int>();
		}

		public void Field_FloatToDoubleConversion_Custom()
		{
			(Get(0).DoubleField, Get(1).DoubleField) = GetSource<double, float>();
		}

		public void Field_FloatToDoubleConversion_Tuple()
		{
			(Get(0).DoubleField, Get(1).DoubleField) = GetTuple<double, float>();
		}

		// dynamic conversion is not supported
		//public void Field_ImplicitReferenceConversion_Custom()
		//{
		//	(Get(0).ObjectField, Get(1).DynamicField) = GetSource<string, string>();
		//}

		public void Field_NoConversion_Custom_DiscardFirst()
		{
			(_, Get(1).MyField) = GetSource<MyInt?, MyInt>();
		}

		public void Field_NoConversion_Custom_DiscardLast()
		{
			(Get(0).NMyField, _) = GetSource<MyInt?, MyInt>();
		}

		public void Field_NoConversion_Tuple_DiscardLast()
		{
			(Get(0).NMyField, Get(1).MyField, _) = GetTuple<MyInt?, MyInt, int>();
		}

		// currently we detect deconstruction, iff the first element is not discarded
		//public void Field_NoConversion_Tuple_DiscardFirst()
		//{
		//	(_, Get(1).MyField, Get(2).IntField) = GetTuple<MyInt?, MyInt, int>();
		//}

		public void Field_NoConversion_Custom_DiscardSecond()
		{
			(Get(0).NMyField, _, Get(2).IntField) = GetSource<MyInt?, MyInt, int>();
		}

		public void Field_NoConversion_Tuple_DiscardSecond()
		{
			(Get(0).NMyField, _, Get(2).IntField) = GetTuple<MyInt?, MyInt, int>();
		}

		public void Field_NoConversion_Custom_ReferenceTypes()
		{
			(Get(0).StringField, Get(1).StringField) = GetSource<string, string>();
		}

		public void Field_NoConversion_Tuple_ReferenceTypes()
		{
			(Get(0).StringField, Get(1).StringField) = GetTuple<string, string>();
		}

		public void DeconstructDictionaryForEach(Dictionary<string, int> dictionary)
		{
			foreach (var (str, num2) in dictionary) {
				Console.WriteLine(str + ": " + num2);
			}
		}

		public void DeconstructTupleListForEach(List<(string, int)> tuples)
		{
			foreach (var (str, num) in tuples) {
				Console.WriteLine(str + ": " + num);
			}
		}
	}
}
