using System;
using System.Runtime.CompilerServices;
using Library1;

namespace ICSharpCode.Decompiler.Tests.TestCases.ILPretty
{
	public class Issue3729
	{
		private MyStruct structField;

		public void TestSingleArgCtor()
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			Console.WriteLine("Test new struct: " + ((object)new MyStruct(4)/*cast due to constrained. prefix*/).ToString());
		}

		public void TestParameterlessCtor()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			MyEmptyStruct val = new MyEmptyStruct();
			Console.WriteLine(val);
		}

		public void TestMultiArgCtor()
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			MyBigStruct val = new MyBigStruct(1, "hello", 3.14);
			Console.WriteLine(val);
		}

		public void TestFieldCtor()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			ref MyStruct reference = ref structField;
			reference = new MyStruct(5);
		}

		public unsafe static void TestPointerCtor(void* ptr)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			System.Runtime.CompilerServices.Unsafe.Write(ptr, new MyStruct(6));
		}

		public void TestArrayElemCtor()
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			MyStruct[] array = (MyStruct[])(object)new MyStruct[1] {
				new MyStruct(7)
			};
		}

		public void TestGenericStructCtor()
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			MyGenericStruct<int> val = new MyGenericStruct<int>(4);
			Console.WriteLine(val);
		}

		public void TestRefTypeNewobj()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			MyClass value = new MyClass();
			Console.WriteLine(value);
		}

		public void TestUnresolvedStructMemberCalls()
		{
			MyEnumerator val = default;
			val.MoveNext();
			((IDisposable)val/*cast due to constrained. prefix*/).Dispose();
		}
	}
	public class Issue3729_DerivedFromUnknown : MissingBase
	{
	}
	public class Issue3729_DerivedFromUnknownWithArgs : MissingBase
	{
		public Issue3729_DerivedFromUnknownWithArgs()
			: base(42)
		{
		}
	}
}
