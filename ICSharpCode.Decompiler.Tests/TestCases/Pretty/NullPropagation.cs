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
			public MyClass this[int index] => null;

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
			public MyStruct? Property1 => null;
			public MyStruct Property2 => default(MyStruct);
			public MyStruct? this[int index] => null;

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

		public interface ITest
		{
			int Int();
			ITest Next();
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

		public void CallSubstringAndIgnoreResult()
		{
			this.GetString()?.Substring(this.GetInt());
		}

		private void Use<T>(T t)
		{
		}

		public void CallDone()
		{
			this.GetMyClass()?.Done();
			this.GetMyClass()?.Field?.Done();
			this.GetMyClass()?.Field.Done();
			this.GetMyClass()?.Property?.Done();
			this.GetMyClass()?.Property.Done();
			this.GetMyClass()?.Method(this.GetInt())?.Done();
			this.GetMyClass()?.Method(this.GetInt()).Done();
			this.GetMyClass()?[this.GetInt()]?.Done();
			this.GetMyClass()?[this.GetInt()].Done();
		}

		public void CallDoneStruct()
		{
			this.GetMyStruct()?.Done();
#if STRUCT_SPLITTING_IMPROVED
			this.GetMyStruct()?.Field?.Done();
			this.GetMyStruct()?.Field.Done();
			this.GetMyStruct()?.Property1?.Done();
			this.GetMyStruct()?.Property2.Done();
			this.GetMyStruct()?.Method1(this.GetInt())?.Done();
			this.GetMyStruct()?.Method2(this.GetInt()).Done();
			this.GetMyStruct()?[this.GetInt()]?.Done();
#endif
		}

		public void RequiredParentheses()
		{
			(this.GetMyClass()?.Field).Done();
			(this.GetMyClass()?.Method(this.GetInt())).Done();
#if STRUCT_SPLITTING_IMPROVED
			(GetMyStruct()?.Property2)?.Done();
#endif
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
		public int?[] ChainsStruct()
		{
			return new int?[8] {
				this.GetMyStruct()?.IntVal,
				this.GetMyStruct()?.Field.IntVal,
				this.GetMyStruct()?.Field?.IntVal,
				this.GetMyStruct()?.Property2.IntVal,
				this.GetMyStruct()?.Property1?.IntVal,
				this.GetMyStruct()?.Method2(this.GetInt()).IntVal,
				this.GetMyStruct()?.Method1(this.GetInt())?.IntVal,
				this.GetMyStruct()?[this.GetInt()]?.IntVal
			};
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

		public void InvokeDelegate(EventHandler eh)
		{
			eh?.Invoke(null, EventArgs.Empty);
		}

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

		private void Setter(MyClass c)
		{
			if (c != null) {
				c.IntVal = 1;
			}
			Console.WriteLine();
			if (c != null) {
				c.Property = null;
			}
		}

		private static int? GenericUnconstrainedInt<T>(T t) where T : ITest
		{
			return t?.Int();
		}

		private static int? GenericClassConstraintInt<T>(T t) where T : class, ITest
		{
			return t?.Int();
		}

		private static int? GenericStructConstraintInt<T>(T? t) where T : struct, ITest
		{
			return t?.Int();
		}

		// See also: https://github.com/icsharpcode/ILSpy/issues/1050
		// The C# compiler generates pretty weird code in this case.
		//private static int? GenericRefUnconstrainedInt<T>(ref T t) where T : ITest
		//{
		//	return t?.Int();
		//}

		private static int? GenericRefClassConstraintInt<T>(ref T t) where T : class, ITest
		{
			return t?.Int();
		}

		private static int? GenericRefStructConstraintInt<T>(ref T? t) where T : struct, ITest
		{
			return t?.Int();
		}
	}
}
