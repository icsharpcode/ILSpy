using System;

namespace ValueTypeCall
{
	public struct MutValueType
	{
		public int val;
		
		public void Increment()
		{
			Console.WriteLine("Inc() called on {0}", val);
			val++;
		}
	}
	
	public class Program
	{
		public static void Main()
		{
			MutValueType m = new MutValueType();
			RefParameter(ref m);
			ValueParameter(m);
			Field();
			Box();
		}
		
		static void RefParameter(ref MutValueType m)
		{
			m.Increment();
			m.Increment();
		}
		
		static void ValueParameter(MutValueType m)
		{
			m.Increment();
			m.Increment();
		}
		
		static readonly MutValueType ReadonlyField = new MutValueType { val = 100 };
		static MutValueType MutableField = new MutValueType { val = 200 };
		
		static void Field()
		{
			ReadonlyField.Increment();
			ReadonlyField.Increment();
			MutableField.Increment();
			MutableField.Increment();
			// Ensure that 'v' isn't incorrectly removed
			// as a compiler-generated temporary
			MutValueType v = MutableField;
			v.Increment();
			Console.WriteLine("Final value in MutableField: " + MutableField.val);
		}
		
		static void Box()
		{
			object o = new MutValueType { val = 300 };
			((MutValueType)o).Increment();
			((MutValueType)o).Increment();
		}
	}
}
