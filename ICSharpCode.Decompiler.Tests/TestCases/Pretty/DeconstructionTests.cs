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
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class DeconstructionBase
	{
	}

	public class DeconstructionDerived : DeconstructionBase
	{
	}

	public static class DeconstructionExt
	{
		public static void Deconstruct<K, V>(this KeyValuePair<K, V> pair, out K key, out V value)
		{
			key = pair.Key;
			value = pair.Value;
		}

		public static void Deconstruct<T1, T2>(this Tuple<T1, T2> tuple, out T1 item1, out T2 item2)
		{
			item1 = tuple.Item1;
			item2 = tuple.Item2;
		}

		public static void Deconstruct(this DeconstructionBase b, out int a, out int c)
		{
			a = 1;
			c = 2;
		}

		public static void Deconstruct(this DeconstructionDerived d, out int a, out int c)
		{
			a = 3;
			c = 4;
		}
	}

	public class DeconstructionOuter
	{
		public void Deconstruct(out int x, out DeconstructionDerived d)
		{
			x = 1;
			d = new DeconstructionDerived();
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
			public int Dummy { get; set; }

			public void Deconstruct(out T a, out T2 b)
			{
				a = default(T);
				b = default(T2);
			}
		}

		private class DeconstructionSource<T, T2, T3>
		{
			public int Dummy { get; set; }

			public void Deconstruct(out T a, out T2 b, out T3 c)
			{
				a = default(T);
				b = default(T2);
				c = default(T3);
			}
		}

		public struct StructDeconstructionSource<T, T2>
		{
			public int Dummy { get; set; }

			public void Deconstruct(out T a, out T2 b)
			{
				a = default(T);
				b = default(T2);
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

			public int Int { get; set; }

			public long Long { get; set; }

			public float Float { get; set; }

			public double Double { get; set; }

			public decimal Decimal { get; set; }

			public string String { get; set; }

			public object Object { get; set; }

			public dynamic Dynamic { get; set; }

			public int? NInt { get; set; }

			public MyInt My { get; set; }

			public MyInt? NMy { get; set; }

			public static MyInt StaticMy { get; set; }

			public static MyInt? StaticNMy { get; set; }
		}

		private DeconstructionSource<T, T2> GetSource<T, T2>()
		{
			return null;
		}

		private DeconstructionSource<T, T2, T3> GetSource<T, T2, T3>()
		{
			return null;
		}

		private StructDeconstructionSource<T, T2> GetStructSource<T, T2>()
		{
			return default(StructDeconstructionSource<T, T2>);
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

		private List<T> GetList<T>()
		{
			return null;
		}

		private int GetInt()
		{
			return 0;
		}

		private Tuple<T, T2> GetTupleClass<T, T2>()
		{
			return null;
		}

		private Dictionary<string, T> GetStringDictionary<T>()
		{
			return null;
		}

		private AssignmentTargets Get(int i)
		{
			return null;
		}

		public void LocalVariable_NoConversion_Custom()
		{
			var (myInt3, myInt4) = GetSource<MyInt?, MyInt>();
			Console.WriteLine(myInt3);
			Console.WriteLine(myInt4);
		}

		public void LocalVariable_NoConversion_Custom_UnrelatedAssignmentAfter()
		{
			var (myInt3, myInt4) = GetSource<MyInt?, MyInt>();
			int value = GetInt();
			Console.WriteLine(myInt3);
			Console.WriteLine(myInt4);
			Console.WriteLine(value);
		}

		public void LocalVariable_NoConversion_Tuple()
		{
			var (myInt, myInt2) = GetTuple<MyInt?, MyInt>();
			Console.WriteLine(myInt);
			Console.WriteLine(myInt2);
		}

		public void LocalVariable_NoConversion_Custom_DiscardFirst()
		{
			var (_, myInt3, value) = GetSource<MyInt?, MyInt, int>();
			Console.WriteLine(myInt3);
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
			var (myInt3, myInt4, _) = GetSource<MyInt?, MyInt, int>();
			Console.WriteLine(myInt3);
			Console.WriteLine(myInt4);
		}

		public void LocalVariable_NoConversion_Tuple_DiscardLast()
		{
			var (myInt, myInt2, _) = GetTuple<MyInt?, MyInt, int>();
			Console.WriteLine(myInt);
			Console.WriteLine(myInt2);
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

		public void Issue2378(Tuple<object, object> tuple)
		{
			var (value, value2) = tuple;
			Console.WriteLine(value2);
			Console.WriteLine(value);
		}

		public void Issue2378_IntToLongConversion(Tuple<int, int> tuple)
		{
			int value;
			long value2;
			(value, value2) = tuple;
			Console.WriteLine(value2);
			Console.WriteLine(value);
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
			var (myInt3, myInt4) = new DeconstructionSource<MyInt?, MyInt> {
				Dummy = 3
			};
			Console.WriteLine(myInt3);
			Console.WriteLine(myInt4);
		}

		public void LocalVariable_NoConversion_Struct_Custom()
		{
			var (value, value2) = GetStructSource<string, int>();
			Console.WriteLine(value);
			Console.WriteLine(value2);
		}

		public void LocalVariable_Nested_ClassInner()
		{
			var (myInt3, (myInt4, value)) = GetSource<MyInt?, DeconstructionSource<MyInt, int>>();
			Console.WriteLine(myInt3);
			Console.WriteLine(myInt4);
			Console.WriteLine(value);
		}

		public void LocalVariable_Nested_StructInner()
		{
			var (myInt3, (myInt4, value)) = GetSource<MyInt?, StructDeconstructionSource<MyInt, int>>();
			Console.WriteLine(myInt3);
			Console.WriteLine(myInt4);
			Console.WriteLine(value);
		}

		public void LocalVariable_Nested_StructOuterAndInner()
		{
			var (myInt3, (myInt4, value)) = GetStructSource<MyInt?, StructDeconstructionSource<MyInt, int>>();
			Console.WriteLine(myInt3);
			Console.WriteLine(myInt4);
			Console.WriteLine(value);
		}

		public void LocalVariable_Nested_BothElementsNested()
		{
			var ((myInt3, value), (myInt4, value2)) = GetSource<DeconstructionSource<MyInt?, int>, StructDeconstructionSource<MyInt, int>>();
			Console.WriteLine(myInt3);
			Console.WriteLine(value);
			Console.WriteLine(myInt4);
			Console.WriteLine(value2);
		}

		public void LocalVariable_Nested_StructInnerFirstElement()
		{
			var ((value, value2), value3) = GetSource<StructDeconstructionSource<int, string>, int>();
			Console.WriteLine(value);
			Console.WriteLine(value2);
			Console.WriteLine(value3);
		}

		public void LocalVariable_ElementOfElementRead_ThenDeconstruct()
		{
			((StructDeconstructionSource<int, string>, int), int) tuple = GetTuple<(StructDeconstructionSource<int, string>, int), int>();
			(StructDeconstructionSource<int, string>, int) item = tuple.Item1;
			StructDeconstructionSource<int, string> item2 = item.Item1;
			var (value, value2) = item2;
			Console.WriteLine(value);
			Console.WriteLine(value2);
			Console.WriteLine(item.Item2);
			Console.WriteLine(tuple.Item2);
		}

		public void LocalVariable_Nested_Depth3()
		{
			var (myInt3, (myInt4, (value, value2))) = GetSource<MyInt?, DeconstructionSource<MyInt, StructDeconstructionSource<int, int>>>();
			Console.WriteLine(myInt3);
			Console.WriteLine(myInt4);
			Console.WriteLine(value);
			Console.WriteLine(value2);
		}

		public void LocalVariable_Nested_DiscardInnerElement()
		{
			var (myInt3, (myInt4, _)) = GetSource<MyInt?, StructDeconstructionSource<MyInt, int>>();
			Console.WriteLine(myInt3);
			Console.WriteLine(myInt4);
		}

		public void LocalVariable_Nested_SystemTupleSource()
		{
			var (myInt3, (myInt4, value)) = GetTupleClass<MyInt?, DeconstructionSource<MyInt, int>>();
			Console.WriteLine(myInt3);
			Console.WriteLine(myInt4);
			Console.WriteLine(value);
		}

		public void LocalVariable_Nested_TupleInner()
		{
			var (value, (value2, value3)) = GetTuple<int, (int, int)>();
			Console.WriteLine(value);
			Console.WriteLine(value2);
			Console.WriteLine(value3);
		}

		public void LocalVariable_Nested_TupleInner_Depth3()
		{
			var (value, (value2, (value3, value4))) = GetTuple<int, (int, (int, int))>();
			Console.WriteLine(value);
			Console.WriteLine(value2);
			Console.WriteLine(value3);
			Console.WriteLine(value4);
		}

		public void LocalVariable_Nested_TupleInner_BothElements()
		{
			var ((value, value2), (value3, value4)) = GetTuple<(int, int), (int, int)>();
			Console.WriteLine(value);
			Console.WriteLine(value2);
			Console.WriteLine(value3);
			Console.WriteLine(value4);
		}

		// Both sources are already materialized, so the element stores of the two
		// deconstructions are adjacent with nothing in between. Locating the enclosing
		// designation of the second one must not walk into the first one's stores.
		public void LocalVariable_Nested_TupleInner_AfterAdjacentDeconstruction((int, (int, int)) source, (int, (int, int)) source2)
		{
			var (value, (value2, value3)) = source;
			var (value4, (value5, value6)) = source2;
			Console.WriteLine(value);
			Console.WriteLine(value2);
			Console.WriteLine(value3);
			Console.WriteLine(value4);
			Console.WriteLine(value5);
			Console.WriteLine(value6);
		}

		// A statement that is not part of the designation sits between the temporary and
		// the reads of it, so the enclosing pattern cannot reach them; they have to be
		// reconstructed on their own rather than deferred to a match that never happens.
		public void LocalVariable_Nested_TupleInner_BarrierBeforeInnerReads((int, (int, int)) source)
		{
			(int, int) item = source.Item2;
			Console.WriteLine(source.Item1);
			var (value, value2) = item;
			Console.WriteLine(value);
			Console.WriteLine(value2);
		}

		// Same, but the barrier sits between the temporary and the outer element read.
		public void LocalVariable_Nested_TupleInner_BarrierAfterTemporary((int, (int, int)) source)
		{
			(int, int) item = source.Item2;
			Console.WriteLine(GetInt());
			var (value, _) = source;
			var (value2, value3) = item;
			Console.WriteLine(value);
			Console.WriteLine(value2);
			Console.WriteLine(value3);
		}

		// Same, but the preceding statements are plain element reads that keep the inner
		// tuple whole, so they are element stores without being a deconstruction. Locating
		// the enclosing designation walks back over them; the run they belong to is itself
		// a deconstruction, so both are reconstructed.
		public void LocalVariable_Nested_TupleInner_AfterAdjacentElementReads((int, (int, int)) source, (int, (int, int)) source2)
		{
			var (value, tuple2) = source;
			var (value2, (value3, value4)) = source2;
			Console.WriteLine(value);
			Console.WriteLine(tuple2);
			Console.WriteLine(value2);
			Console.WriteLine(value3);
			Console.WriteLine(value4);
		}

		public void LocalVariable_Nested_TupleInner_Conversions()
		{
			int value;
			long value2;
			long value3;
			(value, (value2, value3)) = GetTuple<int, (int, int)>();
			Console.WriteLine(value);
			Console.WriteLine(value2);
			Console.WriteLine(value3);
		}

		// The element variable escapes the deconstruction, so it must stay a designator
		// leaf instead of becoming a nested designation.
		public void LocalVariable_TupleInner_ElementUsedOutside()
		{
#if OPT
			(int, (int, int)) tuple = GetTuple<int, (int, int)>();
			int item = tuple.Item1;
			(int, int) item2 = tuple.Item2;
			Console.WriteLine(item);
			Console.WriteLine(item2.Item1);
#else
			var (value, tuple2) = GetTuple<int, (int, int)>();
			Console.WriteLine(value);
			Console.WriteLine(tuple2.Item1);
#endif
		}

		// Same, but the escaping element is in the first position. Every leaf of the
		// wrongly nested node precedes the assigned ones there, so the retry that demotes
		// it has to be reached before the pattern is judged to start mid-way.
		public void LocalVariable_TupleInner_FirstElementUsedOutside()
		{
			var (tuple2, value) = GetTuple<(int, int), int>();
			Console.WriteLine(tuple2.Item1);
			Console.WriteLine(value);
		}

		public void ForEach_Nested_TupleInner()
		{
			foreach (var (value, (value2, value3)) in GetList<(int, (int, int))>())
			{
				Console.WriteLine(value);
				Console.WriteLine(value2);
				Console.WriteLine(value3);
			}
		}

		public void LocalVariable_Nested_TypedConversions_UnrelatedCallAfter()
		{
			long value;
			MyInt myInt2;
			long value2;
			(value, (myInt2, value2)) = GetSource<int, DeconstructionSource<MyInt, int>>();
			int value3 = GetInt();
			Console.WriteLine(value);
			Console.WriteLine(myInt2);
			Console.WriteLine(value2);
			Console.WriteLine(value3);
		}

		public void LocalVariable_Nested_IntToLongConversion()
		{
			int value;
			MyInt myInt2;
			long value2;
			(value, (myInt2, value2)) = GetSource<int, DeconstructionSource<MyInt, int>>();
			Console.WriteLine(value);
			Console.WriteLine(myInt2);
			Console.WriteLine(value2);
		}

		public void LocalVariable_Nested_ElementDeconstructedAfterBarrier()
		{
			GetSource<MyInt?, DeconstructionSource<MyInt, int>>().Deconstruct(out var a, out var b);
			Console.WriteLine(a);
			var (myInt2, value) = b;
			Console.WriteLine(myInt2);
			Console.WriteLine(value);
		}

		public void LocalVariable_Nested_OuterElementUsedTwice()
		{
			GetSource<MyInt?, DeconstructionSource<MyInt, int>>().Deconstruct(out var a, out var b);
			var (myInt2, value) = b;
			Console.WriteLine(a);
			Console.WriteLine(a);
			Console.WriteLine(myInt2);
			Console.WriteLine(value);
		}

		public void ForEach_Nested()
		{
			foreach (var (myInt3, (myInt4, value)) in GetList<StructDeconstructionSource<MyInt?, DeconstructionSource<MyInt, int>>>())
			{
				Console.WriteLine(myInt3);
				Console.WriteLine(myInt4);
				Console.WriteLine(value);
			}
		}

		public void ForEach_Nested_KeyValuePair()
		{
			foreach (var (value, (myInt2, value2)) in GetStringDictionary<StructDeconstructionSource<MyInt, int>>())
			{
				Console.WriteLine(value);
				Console.WriteLine(myInt2);
				Console.WriteLine(value2);
			}
		}

		public void Property_Nested_NoConversion()
		{
			(Get(0).Int, (Get(1).My, Get(2).String)) = GetSource<int, DeconstructionSource<MyInt, string>>();
		}

		public void Property_Nested_IntToLongConversion()
		{
			(Get(0).Int, (Get(1).My, Get(2).Long)) = GetSource<int, DeconstructionSource<MyInt, int>>();
		}

		public void Property_Nested_DiscardInnerElement()
		{
			(Get(0).NMy, (_, Get(1).My)) = GetSource<MyInt?, DeconstructionSource<int, MyInt>>();
		}

		public unsafe void Pointer_NoConversion_Tuple(int* p)
		{
			int value;
			(*p, value) = GetTuple<int, int>();
			Console.WriteLine(value);
			Console.WriteLine(value);
		}

		// The store opcode is sign-agnostic - stind.i4 reports int for a uint target and
		// stind.i1 reports sbyte for a byte one - so the element type of the target cannot
		// be taken from it: doing so refuses every one of these deconstructions.
		// The IL calls the extension declared on the base type, forced by the cast. Folding
		// this into a nested designation would rebind Deconstruct on the element's static
		// type, where the extension declared on the derived type wins and returns different
		// values, so the call has to stay explicit.
		public void Nested_CompetingExtensionDeconstruct(DeconstructionOuter o)
		{
			o.Deconstruct(out var x, out var d);
			((DeconstructionBase)d).Deconstruct(out int a, out int c);
			Console.WriteLine(x);
			Console.WriteLine(a);
			Console.WriteLine(c);
		}

		public unsafe void Pointer_NoConversion_Tuple_UInt(uint* p)
		{
			int value;
			(*p, value) = GetTuple<uint, int>();
			Console.WriteLine(value);
			Console.WriteLine(value);
		}

		public unsafe void Pointer_NoConversion_Tuple_Byte(byte* p)
		{
			int value;
			(*p, value) = GetTuple<byte, int>();
			Console.WriteLine(value);
			Console.WriteLine(value);
		}

		public unsafe void Pointer_Nested_Custom(int* p)
		{
			MyInt myInt2;
			int value;
			(*p, (myInt2, value)) = GetSource<int, DeconstructionSource<MyInt, int>>();
			Console.WriteLine(myInt2);
			Console.WriteLine(value);
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

		public void RefLocal_FloatToDoubleConversion_Custom2(out double a)
		{
			(a, GetRef<double>()) = GetSource<float, float>();
		}

		public void RefLocal_FloatToDoubleConversion_Tuple(out double a)
		{
			(a, GetRef<double>()) = GetTuple<double, float>();
		}

		public void RefLocal_NoConversion_Custom(out MyInt? a)
		{
			(a, GetRef<MyInt>()) = GetSource<MyInt?, MyInt>();
		}

		public void RefLocal_IntToLongConversion_Custom(out long a)
		{
			(a, GetRef<long>()) = GetSource<int, int>();
		}

		// dynamic conversion is not supported
		//public void RefLocal_ImplicitReferenceConversion_Custom(out object a)
		//{
		//	(a, GetRef<dynamic>()) = GetSource<string, string>();
		//}

		public void RefLocal_NoConversion_Custom_DiscardFirst()
		{
			(_, GetRef<MyInt>()) = GetSource<MyInt?, MyInt>();
		}

		public void RefLocal_NoConversion_Custom_DiscardLast(out MyInt? a)
		{
			(a, _) = GetSource<MyInt?, MyInt>();
		}

		public void RefLocal_NoConversion_Tuple(out MyInt? a)
		{
			(a, GetRef<MyInt>()) = GetTuple<MyInt?, MyInt>();
		}

		public void RefLocal_NoConversion_Tuple_DiscardLast(out MyInt? a)
		{
			(a, GetRef<MyInt>(), _) = GetTuple<MyInt?, MyInt, int>();
		}

		// currently we detect deconstruction, iff the first element is not discarded
		//public void RefLocal_NoConversion_Tuple_DiscardFirst(out var a)
		//{
		//	(_, GetRef<var>(), GetRef<var>()) = GetTuple<MyInt?, MyInt, int>();
		//}

		public void RefLocal_NoConversion_Custom_DiscardSecond(out MyInt? a)
		{
			(a, _, GetRef<int>()) = GetSource<MyInt?, MyInt, int>();
		}

		public void RefLocal_NoConversion_Tuple_DiscardSecond(out MyInt? a)
		{
			(a, _, GetRef<int>()) = GetTuple<MyInt?, MyInt, int>();
		}

		public void RefLocal_NoConversion_Custom_ReferenceTypes(out string a)
		{
			(a, GetRef<string>()) = GetSource<string, string>();
		}

		public void RefLocal_NoConversion_Tuple_ReferenceTypes(out string a)
		{
			(a, GetRef<string>()) = GetTuple<string, string>();
		}

		public void RefLocal_IntToLongConversion_Tuple(out long a)
		{
			(a, GetRef<long>()) = GetTuple<int, int>();
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
			foreach (var (text2, num2) in dictionary)
			{
				Console.WriteLine(text2 + ": " + num2);
			}
		}

		public void DeconstructTupleListForEach(List<(string, int)> tuples)
		{
			foreach (var (text, num) in tuples)
			{
				Console.WriteLine(text + ": " + num);
			}
		}

		public async Task<int> DeconstructionAssignmentToCapturedLocals(string file)
		{
			int a = 0;
			int b = 0;
			await Task.Run(delegate {
				(a, b) = GetTuple<int, int>();
			});
			return a + b;
		}

		public bool DeconstructStructParameter(StructDeconstructionSource<int, string> point)
		{
			var (num2, value) = point;
			Console.WriteLine(value);
			return num2 >= 0;
		}

		public void DeconstructStructLocal()
		{
			StructDeconstructionSource<int, string> structSource = GetStructSource<int, string>();
			var (num2, text2) = structSource;
			Console.WriteLine(num2 + text2 + structSource.Dummy);
		}
	}
}
