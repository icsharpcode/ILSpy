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
			new CustomDeconstructionAndConversion().Test();
		}

		private class CustomDeconstructionAndConversion
		{
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

			public int IntField;

			public int? NullableIntField;

			public MyInt MyIntField;

			public MyInt? NullableMyIntField;
			private MyInt? nMy;
			private MyInt my;

			public int Int {
				get;
				set;
			}

			public int? NInt {
				get;
				set;
			}

			public MyInt My {
				get {
					Console.WriteLine("get_My");
					return my;
				}
				set {
					Console.WriteLine("set_My");
					my = value;
				}
			}

			public MyInt? NMy {
				get {
					Console.WriteLine("get_NMy");
					return nMy;
				}
				set {
					Console.WriteLine("set_NMy");
					nMy = value;
				}
			}

			public static MyInt StaticMy {
				get;
				set;
			}

			public static MyInt? StaticNMy {
				get;
				set;
			}

			public void Deconstruct(out MyInt? x, out MyInt y)
			{
				Console.WriteLine("Deconstruct(x, y)");
				x = null;
				y = default(MyInt);
			}

			public CustomDeconstructionAndConversion GetValue()
			{
				Console.WriteLine($"GetValue()");
				return new CustomDeconstructionAndConversion();
			}

			public CustomDeconstructionAndConversion Get(int i)
			{
				Console.WriteLine($"Get({i})");
				return new CustomDeconstructionAndConversion();
			}

			private MyInt? GetNullableMyInt()
			{
				throw new NotImplementedException();
			}

			public void Test()
			{
				Property_NoDeconstruction_SwappedAssignments();
				Property_NoDeconstruction_SwappedInits();
			}

			public void Property_NoDeconstruction_SwappedAssignments()
			{
				Console.WriteLine("Property_NoDeconstruction_SwappedAssignments:");
				CustomDeconstructionAndConversion customDeconstructionAndConversion = Get(0);
				CustomDeconstructionAndConversion customDeconstructionAndConversion2 = Get(1);
				GetValue().Deconstruct(out MyInt? x, out MyInt y);
				MyInt myInt2 = customDeconstructionAndConversion2.My = y;
				MyInt? myInt4 = customDeconstructionAndConversion.NMy = x;
			}

			public void Property_NoDeconstruction_SwappedInits()
			{
				Console.WriteLine("Property_NoDeconstruction_SwappedInits:");
				CustomDeconstructionAndConversion customDeconstructionAndConversion = Get(1);
				(Get(0).NMy, customDeconstructionAndConversion.My) = GetValue();
			}
		}
	}
}
