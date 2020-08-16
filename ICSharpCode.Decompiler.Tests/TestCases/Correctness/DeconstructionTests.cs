using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.Tests.TestCases.Correctness
{
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
					return default;
				}
				set {
					Console.WriteLine($"{id}.set_My({value})");
				}
			}

			public MyInt? NMy {
				get {
					Console.WriteLine($"{id}.get_NMy()");
					return default;
				}
				set {
					Console.WriteLine($"{id}.set_NMy({value})");
				}
			}

			public int IntProperty {
				get {
					Console.WriteLine($"{id}.get_IntProperty()");
					return default;
				}
				set {
					Console.WriteLine($"{id}.set_IntProperty({value})");
				}
			}

			public uint UIntProperty {
				get {
					Console.WriteLine($"{id}.get_UIntProperty()");
					return default;
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
			try {
				AssignmentTargets t0 = null;
				(t0.IntField, a) = GetSource<int, int>();
			} catch (Exception ex) {
				a = 0;
				Console.WriteLine(ex.GetType().FullName);
			}
		}

		public void NullReferenceException_RefLocalReferencesField_Deconstruction(out int a)
		{
			try {
				AssignmentTargets t0 = null;
				ref int i = ref t0.IntField;
				(i, a) = GetSource<int, int>();
			} catch (Exception ex) {
				a = 0;
				Console.WriteLine(ex.GetType().FullName);
			}
		}
	}
}
