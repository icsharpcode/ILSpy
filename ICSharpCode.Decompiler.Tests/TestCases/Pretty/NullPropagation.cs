using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class NullPropagation
	{
		private class MyClass
		{
			public int IntVal;
			public string Text;
			public MyClass Field;
			public MyClass Property {
				get;
				set;
			}
			public MyClass this[int index] {
				get {
					return null;
				}
			}
			public MyClass Method(int arg)
			{
				return null;
			}

			public void Done()
			{
			}
		}

		private struct MyStruct
		{
			public int IntVal;
			public MyClass Field;
			public MyStruct? Property1 {
				get {
					return null;
				}
			}
			public MyStruct Property2 {
				get {
					return default(MyStruct);
				}
			}
			public MyStruct? this[int index] {
				get {
					return null;
				}
			}
			public MyStruct? Method1(int arg)
			{
				return null;
			}
			public MyStruct Method2(int arg)
			{
				return default(MyStruct);
			}

			public void Done()
			{
			}
		}

		private int GetInt()
		{
			return 9;
		}

		private string GetString()
		{
			return null;
		}

		private MyClass GetMyClass()
		{
			return null;
		}

		private MyStruct? GetMyStruct()
		{
			return null;
		}

		public string Substring()
		{
			return this.GetString()?.Substring(this.GetInt());
		}

		private void Use<T>(T t)
		{
		}

#if VOID_SUPPORTED
		public void CallDone()
		{
			this.GetMyClass()?.Done();
			this.GetMyClass()?.Field?.Done();
			this.GetMyClass()?.Field.Done();
			this.GetMyClass()?.Property?.Done();
			this.GetMyClass()?.Property.Done();
			this.GetMyClass()?.Method(GetInt())?.Done();
			this.GetMyClass()?.Method(GetInt()).Done();
			this.GetMyClass()?[GetInt()]?.Done();
			this.GetMyClass()?[GetInt()].Done();
		}

		public void CallDoneStruct()
		{
			this.GetMyStruct()?.Done();
			this.GetMyStruct()?.Field?.Done();
			this.GetMyStruct()?.Field.Done();
			this.GetMyStruct()?.Property1?.Done();
			this.GetMyStruct()?.Property2.Done();
			this.GetMyStruct()?.Method1(GetInt())?.Done();
			this.GetMyStruct()?.Method2(GetInt()).Done();
			this.GetMyStruct()?[GetInt()]?.Done();
		}
#endif

		public void RequiredParentheses()
		{
			(this.GetMyClass()?.Field).Done();
			(this.GetMyClass()?.Method(this.GetInt())).Done();
		//	(GetMyStruct()?.Property2)?.Done();
		}

		public int?[] ChainsOnClass()
		{
			return new int?[9] {
				this.GetMyClass()?.IntVal,
				this.GetMyClass()?.Field.IntVal,
				this.GetMyClass()?.Field?.IntVal,
				this.GetMyClass()?.Property.IntVal,
				this.GetMyClass()?.Property?.IntVal,
				this.GetMyClass()?.Method(this.GetInt()).IntVal,
				this.GetMyClass()?.Method(this.GetInt())?.IntVal,
				this.GetMyClass()?[this.GetInt()].IntVal,
				this.GetMyClass()?[this.GetInt()]?.IntVal
			};
		}

#if STRUCT_SPLITTING_IMPROVED
		public int? SumOfChainsStruct()
		{
			return this.GetMyStruct()?.IntVal
				+ this.GetMyStruct()?.Field.IntVal
				+ this.GetMyStruct()?.Field?.IntVal
				+ this.GetMyStruct()?.Property2.IntVal
				+ this.GetMyStruct()?.Property1?.IntVal
				+ this.GetMyStruct()?.Method2(this.GetInt()).IntVal
				+ this.GetMyStruct()?.Method1(this.GetInt())?.IntVal
				+ this.GetMyStruct()?[this.GetInt()]?.IntVal;
		}
#endif

		public int CoalescingReturn()
		{
			return this.GetMyClass()?.IntVal ?? 1;
		}

		public void Coalescing()
		{
			this.Use(this.GetMyClass()?.IntVal ?? 1);
		}

		public void CoalescingString()
		{
			this.Use(this.GetMyClass()?.Text ?? "Hello");
		}

#if VOID_SUPPORTED
		public void InvokeDelegate(EventHandler eh)
		{
			eh?.Invoke(null, EventArgs.Empty);
		}
#endif

		public int? InvokeDelegate(Func<int> f)
		{
			return f?.Invoke();
		}

		private void NotNullPropagation(MyClass c)
		{
			// don't decompile this to "(c?.IntVal ?? 0) != 0"
			if (c != null && c.IntVal != 0) {
				Console.WriteLine("non-zero");
			}
			if (c == null || c.IntVal == 0) {
				Console.WriteLine("null or zero");
			}
			Console.WriteLine("end of method");
		}
	}
}
