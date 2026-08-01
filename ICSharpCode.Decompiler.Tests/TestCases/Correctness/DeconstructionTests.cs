using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.Tests.TestCases.Correctness
{
	static class KeyValuePairExtensions
	{
		public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value)
		{
			key = pair.Key;
			value = pair.Value;
		}
	}

	class DeconstructionTests
	{
		public static void Main()
		{
			new DeconstructionTests().Test();
		}

		public struct MyInt
		{
			public static implicit operator int(MyInt x)
			{
				Console.WriteLine("int op_Implicit(MyInt)");
				return 0;
			}

			public static implicit operator MyInt(int x)
			{
				Console.WriteLine("MyInt op_Implicit(int)");
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
				Console.WriteLine("Deconstruct");
				a = default(T);
				b = default(T2);
			}
		}

		private class AssignmentTargets
		{
			int id;

			public AssignmentTargets(int id)
			{
				this.id = id;
			}

			public int IntField;

			public int? NullableIntField;

			public MyInt MyIntField;

			public MyInt? NullableMyIntField;

			public MyInt My {
				get {
					Console.WriteLine($"{id}.get_My()");
					return default(MyInt);
				}
				set {
					Console.WriteLine($"{id}.set_My({value})");
				}
			}

			public MyInt? NMy {
				get {
					Console.WriteLine($"{id}.get_NMy()");
					return default(MyInt?);
				}
				set {
					Console.WriteLine($"{id}.set_NMy({value})");
				}
			}

			public int IntProperty {
				get {
					Console.WriteLine($"{id}.get_IntProperty()");
					return default(int);
				}
				set {
					Console.WriteLine($"{id}.set_IntProperty({value})");
				}
			}

			public uint UIntProperty {
				get {
					Console.WriteLine($"{id}.get_UIntProperty()");
					return default(uint);
				}
				set {
					Console.WriteLine($"{id}.set_UIntProperty({value})");
				}
			}
		}

		private DeconstructionSource<T, T2> GetSource<T, T2>()
		{
			Console.WriteLine("GetSource()");
			return new DeconstructionSource<T, T2>();
		}

		private (T, T2) GetTuple<T, T2>()
		{
			Console.WriteLine("GetTuple<T, T2>()");
			return default(ValueTuple<T, T2>);
		}

		private (T, T2, T3) GetTuple<T, T2, T3>()
		{
			Console.WriteLine("GetTuple<T, T2, T3>()");
			return default(ValueTuple<T, T2, T3>);
		}

		private AssignmentTargets Get(int i)
		{
			Console.WriteLine($"Get({i})");
			return new AssignmentTargets(i);
		}

		public void Test()
		{
			Property_NoDeconstruction_SwappedAssignments();
			Property_NoDeconstruction_SwappedInits();
			Property_IntToUIntConversion();
			NoDeconstruction_NotUsingConver();
			NoDeconstruction_NotUsingConver_Tuple();
			NullReferenceException_Field_Deconstruction(out _);
			NullReferenceException_RefLocalReferencesField_Deconstruction(out _);
			NullReferenceException_RefLocalReferencesArrayElement_Deconstruction(out _, null);
			DeconstructTupleSameVar(("a", "b"));
			DeconstructTupleListForEachSameVar(new List<(string, string)> { ("a", "b") });
			StructDeconstruction_Assignment(new NestedInner { Value = 7 });
			NestedDeconstruction_Assignment(new NestedOuter { Value = 42 });
			NestedDeconstruction_ForEach(new List<NestedOuter> {
				new NestedOuter { Value = 1 },
				new NestedOuter { Value = 2 }
			});
			NestedDeconstruction_DiscardedElement(new KeyValuePair<object, DiscardData>("key", default(DiscardData)));
		}

		public struct DiscardData
		{
			public void Deconstruct(out object o1, out object o2)
			{
				Console.WriteLine("DiscardData.Deconstruct");
				o1 = 1;
				o2 = 2;
			}
		}

		public void NestedDeconstruction_DiscardedElement(KeyValuePair<object, DiscardData> pair)
		{
			Console.WriteLine("NestedDeconstruction_DiscardedElement:");
			var (key, (value, _)) = pair;
			Console.WriteLine(key);
			Console.WriteLine(value);
		}

		public struct NestedInner
		{
			public int Value;

			public void Deconstruct(out int a, out int b)
			{
				Console.WriteLine("NestedInner.Deconstruct");
				a = Value + 1;
				b = Value + 2;
			}
		}

		public struct NestedOuter
		{
			public int Value;

			public void Deconstruct(out int x, out NestedInner inner)
			{
				Console.WriteLine("NestedOuter.Deconstruct");
				x = Value;
				inner = new NestedInner { Value = Value * 10 };
			}
		}

		public void StructDeconstruction_Assignment(NestedInner s)
		{
			Console.WriteLine("StructDeconstruction_Assignment:");
			var (a, b) = s;
			Console.WriteLine(a);
			Console.WriteLine(b);
		}

		public void NestedDeconstruction_Assignment(NestedOuter o)
		{
			Console.WriteLine("NestedDeconstruction_Assignment:");
			var (x, (a, b)) = o;
			Console.WriteLine(x);
			Console.WriteLine(a);
			Console.WriteLine(b);
		}

		public void NestedDeconstruction_ForEach(IEnumerable<NestedOuter> items)
		{
			Console.WriteLine("NestedDeconstruction_ForEach:");
			foreach (var (x, (a, b)) in items)
			{
				Console.WriteLine(x + a + b);
			}
		}

		public void Property_NoDeconstruction_SwappedAssignments()
		{
			Console.WriteLine("Property_NoDeconstruction_SwappedAssignments:");
			AssignmentTargets customDeconstructionAndConversion = Get(0);
			AssignmentTargets customDeconstructionAndConversion2 = Get(1);
			GetSource<MyInt?, MyInt>().Deconstruct(out MyInt? x, out MyInt y);
			MyInt myInt2 = customDeconstructionAndConversion2.My = y;
			MyInt? myInt4 = customDeconstructionAndConversion.NMy = x;
		}

		public void Property_NoDeconstruction_SwappedInits()
		{
			Console.WriteLine("Property_NoDeconstruction_SwappedInits:");
			AssignmentTargets customDeconstructionAndConversion = Get(1);
			(Get(0).NMy, customDeconstructionAndConversion.My) = GetSource<MyInt?, MyInt>();
		}

		public void Property_IntToUIntConversion()
		{
			Console.WriteLine("Property_IntToUIntConversion:");
			AssignmentTargets t0 = Get(0);
			AssignmentTargets t1 = Get(1);
			int a;
			uint b;
			GetSource<int, uint>().Deconstruct(out a, out b);
			t0.UIntProperty = (uint)a;
			t1.IntProperty = (int)b;
		}

		public void NoDeconstruction_NotUsingConver()
		{
			Console.WriteLine("NoDeconstruction_NotUsingConver:");
			AssignmentTargets t0 = Get(0);
			int a;
			uint b;
			GetSource<int, uint>().Deconstruct(out a, out b);
			long c = a;
			t0.IntProperty = a;
			t0.UIntProperty = b;
			Console.WriteLine(c);
		}

		public void NoDeconstruction_NotUsingConver_Tuple()
		{
			Console.WriteLine("NoDeconstruction_NotUsingConver_Tuple:");
			AssignmentTargets t0 = Get(0);
			var t = GetTuple<int, uint>();
			long c = t.Item1;
			t0.IntProperty = t.Item1;
			t0.UIntProperty = t.Item2;
			Console.WriteLine(c);
		}

		public void NullReferenceException_Field_Deconstruction(out int a)
		{
			try
			{
				AssignmentTargets t0 = null;
				(t0.IntField, a) = GetSource<int, int>();
			}
			catch (Exception ex)
			{
				a = 0;
				Console.WriteLine(ex.GetType().FullName);
			}
		}

		public void NullReferenceException_RefLocalReferencesField_Deconstruction(out int a)
		{
			try
			{
				AssignmentTargets t0 = null;
				ref int i = ref t0.IntField;
				(i, a) = GetSource<int, int>();
			}
			catch (Exception ex)
			{
				a = 0;
				Console.WriteLine(ex.GetType().FullName);
			}
		}

		public void NullReferenceException_RefLocalReferencesArrayElement_Deconstruction(out int a, int[] arr)
		{
			try
			{
				ref int i = ref arr[0];
				(i, a) = GetSource<int, int>();
			}
			catch (Exception ex)
			{
				a = 0;
				Console.WriteLine(ex.GetType().FullName);
			}
		}

		public void DeconstructTupleSameVar((string, string) tuple)
		{
			Console.WriteLine("DeconstructTupleSameVar:");
			string a;
			a = tuple.Item1;
			a = tuple.Item2;
			Console.WriteLine(a);
		}

		public void DeconstructTupleListForEachSameVar(List<(string, string)> tuples)
		{
			Console.WriteLine("DeconstructTupleListForEachSameVar:");
			foreach (var tuple in tuples)
			{
				string a;
				a = tuple.Item1;
				a = tuple.Item2;
				Console.WriteLine(a);
			}
		}
	}
}
