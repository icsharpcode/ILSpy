using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Correctness
{
	public struct MutValueType
	{
		public int val;
		
		public void Increment()
		{
			Console.WriteLine("Inc() called on {0}", val);
			val = val + 1;
		}
	}
	
	public struct GenericValueType<T>
	{
		T data;
		int num;
		
		public GenericValueType(T data)
		{
			this.data = data;
			this.num = 1;
		}
		
		public void Call(ref GenericValueType<T> v)
		{
			num++;
			Console.WriteLine("Call #{0}: {1} with v=#{2}", num, data, v.num);
		}
	}
	
	public struct ValueTypeWithReadOnlyMember
	{
		public readonly int Member;
		
		public ValueTypeWithReadOnlyMember(int member)
		{
			this.Member = member;
		}
	}
	
	public class ValueTypeCall
	{
		public static void Main()
		{
			MutValueType m = new MutValueType();
			RefParameter(ref m);
			ValueParameter(m);
			Field();
			Box();
			var gvt = new GenericValueType<string>("Test");
			gvt.Call(ref gvt);
			new ValueTypeCall().InstanceFieldTests();
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
		
		MutValueType instanceField;
		ValueTypeWithReadOnlyMember mutableInstanceFieldWithReadOnlyMember;
		
		void InstanceFieldTests()
		{
			this.instanceField.val = 42;
			Console.WriteLine(this.instanceField.val);
			mutableInstanceFieldWithReadOnlyMember = new ValueTypeWithReadOnlyMember(45);
			Console.WriteLine(this.mutableInstanceFieldWithReadOnlyMember.Member);
		}
	}
}
