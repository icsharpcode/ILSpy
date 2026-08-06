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

	static class TupleClassExtensions
	{
		public static void Deconstruct<T1, T2>(this Tuple<T1, T2> tuple, out T1 item1, out T2 item2)
		{
			item1 = tuple.Item1;
			item2 = tuple.Item2;
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
			NestedDeconstruction_ClassInner(new ClassInnerOuter { Value = 7 });
			NestedDeconstruction_Depth3(new DeepOuter { Value = 3 });
			NestedDeconstruction_LhsSideEffects_DeconstructionOrder_Assignments();
			NestedDeconstruction_Conversions_AfterAllDeconstructCalls();
			NestedDeconstruction_TypedDeclaration_Conversions(new NestedOuter { Value = 5 });
			NestedDeconstruction_DiscardWithSideEffectTargets();
			NestedDeconstruction_HiddenDeconstructMethod(default(HidingOuter));
			NestedDeconstruction_TupleWithCustomElement((7, new NestedInner { Value = 3 }));
			NestedDeconstruction_SystemTupleSource(Tuple.Create(8, new NestedInner { Value = 4 }));
			NestedDeconstruction_CheckedConversions(new NestedOuter { Value = 9 });
			NestedDeconstruction_GenericConstraintSource(new ConstrainedSource { Value = 11 });
			NestedDeconstruction_InParameterSource(new NestedOuter { Value = 12 });
			NestedDeconstruction_ConditionalSource(c: true, new NestedOuter { Value = 13 }, new NestedOuter { Value = 14 });
			NestedDeconstruction_TupleOuterConversions((15, new NestedInner { Value = 6 }));
			NestedDeconstruction_TypedConversions_UnrelatedCallAfter(new NestedOuter { Value = 16 });
			NestedDeconstruction_NullableConversions(new NestedOuter { Value = 17 });
			NestedDeconstruction_MyIntConversionOnNestedLeaves(new NestedOuter { Value = 18 });
			NestedDeconstruction_ForEachDictionary_Conversions(new Dictionary<string, NestedInner> {
				{ "k1", new NestedInner { Value = 19 } }
			});
		}

		public class ConstrainedSource
		{
			public int Value;

			public void Deconstruct(out int a, out NestedInner inner)
			{
				Console.WriteLine("ConstrainedSource.Deconstruct");
				a = Value;
				inner = new NestedInner { Value = Value * 10 };
			}
		}

		public void NestedDeconstruction_SystemTupleSource(Tuple<int, NestedInner> tup)
		{
			Console.WriteLine("NestedDeconstruction_SystemTupleSource:");
			(long x, (long a, long b)) = tup;
			int z = Side();
			Console.WriteLine(x + " " + a + " " + b + " " + z);
		}

		public void NestedDeconstruction_CheckedConversions(NestedOuter o)
		{
			Console.WriteLine("NestedDeconstruction_CheckedConversions:");
			checked
			{
				(long x, (long a, long b)) = o;
				Console.WriteLine(x + " " + a + " " + b);
			}
		}

		public void NestedDeconstruction_GenericConstraintSource<T>(T o) where T : ConstrainedSource
		{
			Console.WriteLine("NestedDeconstruction_GenericConstraintSource:");
			var (a, (c, d)) = o;
			Console.WriteLine(a + " " + c + " " + d);
		}

		public void NestedDeconstruction_InParameterSource(in NestedOuter o)
		{
			Console.WriteLine("NestedDeconstruction_InParameterSource:");
			(long x, (long a, long b)) = o;
			Console.WriteLine(x + " " + a + " " + b);
		}

		public void NestedDeconstruction_ConditionalSource(bool c, NestedOuter o1, NestedOuter o2)
		{
			Console.WriteLine("NestedDeconstruction_ConditionalSource:");
			(long x, (int a, int b)) = c ? o1 : o2;
			int z = Side();
			Console.WriteLine(x + " " + a + " " + b + " " + z);
		}

		public void NestedDeconstruction_TupleOuterConversions((int, NestedInner) tup)
		{
			Console.WriteLine("NestedDeconstruction_TupleOuterConversions:");
			(long x, (long a, long b)) = tup;
			Console.WriteLine(x + " " + a + " " + b);
		}

		public void NestedDeconstruction_TypedConversions_UnrelatedCallAfter(NestedOuter o)
		{
			Console.WriteLine("NestedDeconstruction_TypedConversions_UnrelatedCallAfter:");
			(long x, (long a, long b)) = o;
			int z = Side();
			Console.WriteLine(x + " " + a + " " + b + " " + z);
		}

		public void NestedDeconstruction_NullableConversions(NestedOuter o)
		{
			Console.WriteLine("NestedDeconstruction_NullableConversions:");
			(long? x, (long? a, int? b)) = o;
			Console.WriteLine(x + " " + a + " " + b);
		}

		public void NestedDeconstruction_MyIntConversionOnNestedLeaves(NestedOuter o)
		{
			Console.WriteLine("NestedDeconstruction_MyIntConversionOnNestedLeaves:");
			(MyInt x, (MyInt a, long b)) = o;
			Console.WriteLine(x + " " + a + " " + b);
		}

		public void NestedDeconstruction_ForEachDictionary_Conversions(Dictionary<string, NestedInner> d)
		{
			Console.WriteLine("NestedDeconstruction_ForEachDictionary_Conversions:");
			foreach ((string k, (long a, long b)) in d)
			{
				Console.WriteLine(k + " " + a + " " + b);
			}
		}

		// The evaluation order of a deconstruction-assignment is: (1) all side-effects of
		// the left-hand-side targets, (2) all Deconstruct invocations, (3) conversions,
		// (4) assignments. Get(i), the Deconstruct methods, MyInt's implicit conversions,
		// and the property setters all print, so any phase reordering breaks the output diff.
		public void NestedDeconstruction_LhsSideEffects_DeconstructionOrder_Assignments()
		{
			Console.WriteLine("NestedDeconstruction_LhsSideEffects_DeconstructionOrder_Assignments:");
			(Get(0).IntProperty, (Get(1).IntProperty, Get(2).IntProperty)) = new NestedOuter { Value = 11 };
		}

		public void NestedDeconstruction_Conversions_AfterAllDeconstructCalls()
		{
			Console.WriteLine("NestedDeconstruction_Conversions_AfterAllDeconstructCalls:");
			(Get(0).My, (Get(1).IntProperty, Get(2).My)) = new NestedOuter { Value = 21 };
		}

		public void NestedDeconstruction_TypedDeclaration_Conversions(NestedOuter o)
		{
			Console.WriteLine("NestedDeconstruction_TypedDeclaration_Conversions:");
			(MyInt x, (long a, MyInt b)) = o;
			Console.WriteLine(x);
			Console.WriteLine(a);
			Console.WriteLine(b);
		}

		public void NestedDeconstruction_DiscardWithSideEffectTargets()
		{
			Console.WriteLine("NestedDeconstruction_DiscardWithSideEffectTargets:");
			(Get(0).IntProperty, (_, Get(1).My)) = new NestedOuter { Value = 31 };
		}

		public class HidingBase
		{
			public int Value;

			public void Deconstruct(out string a, out double b)
			{
				Console.WriteLine("HidingBase.Deconstruct");
				a = "base" + Value;
				b = 0.5;
			}
		}

		public class HidingDerived : HidingBase
		{
			public new void Deconstruct(out string a, out double b)
			{
				Console.WriteLine("HidingDerived.Deconstruct");
				a = "derived";
				b = 99.5;
			}
		}

		public struct HidingOuter
		{
			public void Deconstruct(out int x, out HidingDerived d)
			{
				Console.WriteLine("HidingOuter.Deconstruct");
				x = 1;
				d = new HidingDerived { Value = 5 };
			}
		}

		// The base-typed view forces the call to bind to HidingBase.Deconstruct; a nested
		// designation cannot express that, because it rebinds on the element's static type,
		// where the hiding method wins.
		public void NestedDeconstruction_HiddenDeconstructMethod(HidingOuter o)
		{
			Console.WriteLine("NestedDeconstruction_HiddenDeconstructMethod:");
			var (_, d) = o;
			HidingBase b = d;
			var (a, c) = b;
			Console.WriteLine(a);
			Console.WriteLine(c);
		}

		public int Side()
		{
			Console.WriteLine("Side()");
			return 5;
		}

		// A tuple deconstruction whose element is custom-deconstructed, followed by an
		// unrelated assignment: the tuple part must not be consumed into a pattern rooted
		// in the element's Deconstruct call.
		public void NestedDeconstruction_TupleWithCustomElement((int, NestedInner) tup)
		{
			Console.WriteLine("NestedDeconstruction_TupleWithCustomElement:");
			var (x, (_, _)) = tup;
			int z = Side();
			Console.WriteLine(x * x + z);
		}

		public class NestedClassInner
		{
			public int Value;

			public void Deconstruct(out int a, out int b)
			{
				Console.WriteLine("NestedClassInner.Deconstruct");
				a = Value + 1;
				b = Value + 2;
			}
		}

		public struct ClassInnerOuter
		{
			public int Value;

			public void Deconstruct(out int x, out NestedClassInner inner)
			{
				Console.WriteLine("ClassInnerOuter.Deconstruct");
				x = Value;
				inner = new NestedClassInner { Value = Value * 10 };
			}
		}

		public void NestedDeconstruction_ClassInner(ClassInnerOuter o)
		{
			Console.WriteLine("NestedDeconstruction_ClassInner:");
			var (x, (a, b)) = o;
			Console.WriteLine(x);
			Console.WriteLine(a);
			Console.WriteLine(b);
		}

		public struct DeepOuter
		{
			public int Value;

			public void Deconstruct(out int x, out ClassInnerOuter mid)
			{
				Console.WriteLine("DeepOuter.Deconstruct");
				x = Value;
				mid = new ClassInnerOuter { Value = Value * 100 };
			}
		}

		public void NestedDeconstruction_Depth3(DeepOuter o)
		{
			Console.WriteLine("NestedDeconstruction_Depth3:");
			var (x, (y, (a, b))) = o;
			Console.WriteLine(x);
			Console.WriteLine(y);
			Console.WriteLine(a);
			Console.WriteLine(b);
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
