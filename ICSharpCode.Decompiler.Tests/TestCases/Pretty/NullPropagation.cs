using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	class NullPropagation
	{
		class MyClass
		{
			public int IntVal;
			public MyClass Field;
			public MyClass Property { get; set; }
			public MyClass Method(int arg) { return null; }
			public MyClass this[int index] {
				get { return null; }
			}

			public void Done()
			{
			}
		}

		struct MyStruct
		{
			public int IntVal;
			public MyClass Field;
			public MyStruct? Property1 { get { return null; } }
			public MyStruct Property2 { get { return default; } }
			public MyStruct? Method1(int arg) { return null; }
			public MyStruct Method2(int arg) { return default; }
			public MyStruct? this[int index] {
				get { return null; }
			}

			public void Done()
			{
			}
		}

		int GetInt()
		{
			return 9;
		}

		string GetString()
		{
			return null;
		}

		MyClass GetMyClass()
		{
			return null;
		}

		MyStruct? GetMyStruct()
		{
			return null;
		}

		string Substring()
		{
			return GetString()?.Substring(GetInt());
		}

		void CallDone()
		{
			GetMyClass()?.Done();
			GetMyClass()?.Field?.Done();
			GetMyClass()?.Field.Done();
			GetMyClass()?.Property?.Done();
			GetMyClass()?.Property.Done();
			GetMyClass()?.Method(GetInt())?.Done();
			GetMyClass()?.Method(GetInt()).Done();
			GetMyClass()?[GetInt()]?.Done();
			GetMyClass()?[GetInt()].Done();
		}

		void CallDoneStruct()
		{
			GetMyStruct()?.Done();
			GetMyStruct()?.Field?.Done();
			GetMyStruct()?.Field.Done();
			GetMyStruct()?.Property1?.Done();
			GetMyStruct()?.Property2.Done();
			GetMyStruct()?.Method1(GetInt())?.Done();
			GetMyStruct()?.Method2(GetInt()).Done();
			GetMyStruct()?[GetInt()]?.Done();
		}

		void RequiredParentheses()
		{
			(GetMyClass()?.Field).Done();
			(GetMyClass()?.Method(GetInt())).Done();
			(GetMyStruct()?.Property2)?.Done();
		}

		int? SumOfChains()
		{
			return GetMyClass()?.IntVal
				+ GetMyClass()?.Field.IntVal
				+ GetMyClass()?.Field?.IntVal
				+ GetMyClass()?.Property.IntVal
				+ GetMyClass()?.Property?.IntVal
				+ GetMyClass()?.Method(GetInt()).IntVal
				+ GetMyClass()?.Method(GetInt())?.IntVal
				+ GetMyClass()?[GetInt()].IntVal
				+ GetMyClass()?[GetInt()]?.IntVal;
		}

		int? SumOfChainsStruct()
		{
			return GetMyStruct()?.IntVal
				+ GetMyStruct()?.Field.IntVal
				+ GetMyStruct()?.Field?.IntVal
				+ GetMyStruct()?.Property2.IntVal
				+ GetMyStruct()?.Property1?.IntVal
				+ GetMyStruct()?.Method2(GetInt()).IntVal
				+ GetMyStruct()?.Method1(GetInt())?.IntVal
				+ GetMyStruct()?[GetInt()]?.IntVal;
		}
	}
}
