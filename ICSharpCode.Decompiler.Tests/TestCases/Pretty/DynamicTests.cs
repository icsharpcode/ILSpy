using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class DynamicTests
	{

		private class Base
		{
			public Base(object baseObj)
			{
			}
		}

		private class Derived : Base
		{
			public Derived(dynamic d)
				: base((object)d)
			{
			}
		}

		private struct MyValueType
		{
			private readonly dynamic _getOnlyProperty;
			public dynamic Field;
#if CS60
			public dynamic GetOnlyProperty => _getOnlyProperty;
#else
			public dynamic GetOnlyProperty {
				get {
					return _getOnlyProperty;
				}
			}
#endif

			public dynamic Property {
				get;
				set;
			}

			public void Method(dynamic a)
			{

			}
		}

		private static dynamic field;
		private static object objectField;
		public dynamic Property {
			get;
			set;
		}

		public DynamicTests()
		{
		}

		public DynamicTests(dynamic test)
		{
		}

		public DynamicTests(DynamicTests test)
		{
		}

		private static void CallWithOut(out dynamic d)
		{
			d = null;
		}

#if CS70
		private static void CallWithIn(in dynamic d)
		{
		}
#endif

		private static void CallWithRef(ref dynamic d)
		{
		}

		private static void RefCallSiteTests()
		{
#if CS70
			CallWithOut(out dynamic d);
			CallWithIn(in d);
#else
			dynamic d;
			CallWithOut(out d);
#endif
			CallWithRef(ref d);
			d.SomeCall();
		}

		private static void InvokeConstructor()
		{
			DynamicTests dynamicTests = new DynamicTests();
			dynamic val = new DynamicTests();
			val.Test(new UnauthorizedAccessException());
			dynamic val2 = new DynamicTests(val);
			val2.Get(new DynamicTests((DynamicTests)val));
			val2.Call(new DynamicTests((dynamic)dynamicTests));
		}

		private static dynamic InlineAssign(object a, out dynamic b)
		{
			return b = ((dynamic)a).Test;
		}

		private static dynamic SelfReference(dynamic d)
		{
			return d[d, d] = d;
		}

		private static dynamic LongArgumentListFunc(dynamic d)
		{
			// Func`13
			return d(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
		}

		private static void LongArgumentListAction(dynamic d)
		{
			// Action`13
			d(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11);
		}

		private static void DynamicThrow()
		{
			try {
				throw (Exception)field;
			} catch (Exception ex) {
				Console.WriteLine(ex.ToString());
				throw;
			}
		}

		private static void MemberAccess(dynamic a)
		{
			a.Test1();
			a.GenericTest<int, int>();
			a.Test2(1);
			a.Test3(a.InnerTest(1, 2, 3, 4, 5));
			a.Test4(2, null, a.Index[0]);
			a.Test5(a, a.Number, a.String);
			a[0] = 3;
			a.Index[a.Number] = 5;
			a.Index[a.Number] += 5;
			a.Setter = new DynamicTests();
			a.Setter2 = 5;
		}

		private static void StructMemberAccess(MyValueType valueType)
		{
			valueType.Field = 0;
			valueType.Field += 5;
			valueType.Field[1] = 5;
			valueType.Field.CallMe();
			DynamicTests.Casts(valueType.GetOnlyProperty);
			valueType.GetOnlyProperty.CallMe();
			valueType.Property = 0;
			valueType.Property += 5;
			valueType.Property[1] = 5;
			valueType.Property.CallMe(5.ToDynamic((object)valueType.Property.Call()));
			valueType.Method(valueType.GetOnlyProperty + valueType.Field);
		}

		private static void RequiredCasts()
		{
			((dynamic)objectField).A = 5;
			((dynamic)objectField).B += 5;
			((dynamic)objectField).Call();
			((object)field).ToString();
			field.Call("Hello World");
			field.Call((object)"Hello World");
			field.Call((dynamic)"Hello World");
		}

		private static void DynamicCallWithString()
		{
			field.Call("Hello World");
		}

		private static void DynamicCallWithNamedArgs()
		{
			field.Call(a: "Hello World");
		}

		private static void DynamicCallWithRefOutArg(int a, out int b)
		{
			field.Call(ref a, out b);
		}

		private static void DynamicCallWithStringCastToObj()
		{
			field.Call((object)"Hello World");
		}

		private static void DynamicCallWithStringCastToDynamic()
		{
			field.Call((dynamic)"Hello World");
		}

		private static void DynamicCallWithStringCastToDynamic2()
		{
			field.Call((dynamic)"Hello World", 5, null);
		}

		private static void DynamicCallWithStringCastToDynamic3()
		{
			field.Call((dynamic)"Hello World", 5u, null);
		}

		private static void Invocation(dynamic a, dynamic b)
		{
			a(null, b.Test());
		}

		private static dynamic Test1(dynamic a)
		{
			dynamic val = a.IndexedProperty;
			return val[0];
		}

		private static dynamic Test2(dynamic a)
		{
			return a.IndexedProperty[0];
		}

		private static void ArithmeticBinaryOperators(dynamic a, dynamic b)
		{
			DynamicTests.MemberAccess(a + b);
			DynamicTests.MemberAccess(a + 1);
			DynamicTests.MemberAccess(a + null);
			DynamicTests.MemberAccess(a - b);
			DynamicTests.MemberAccess(a - 1);
			DynamicTests.MemberAccess(a - null);
			DynamicTests.MemberAccess(a * b);
			DynamicTests.MemberAccess(a * 1);
			DynamicTests.MemberAccess(a * null);
			DynamicTests.MemberAccess(a / b);
			DynamicTests.MemberAccess(a / 1);
			DynamicTests.MemberAccess(a / null);
			DynamicTests.MemberAccess(a % b);
			DynamicTests.MemberAccess(a % 1);
			DynamicTests.MemberAccess(a % null);
		}

		private static void CheckedArithmeticBinaryOperators(dynamic a, dynamic b)
		{
			checked {
				DynamicTests.MemberAccess(a + b);
				DynamicTests.MemberAccess(a + 1);
				DynamicTests.MemberAccess(a + null);
				DynamicTests.MemberAccess(a - b);
				DynamicTests.MemberAccess(a - 1);
				DynamicTests.MemberAccess(a - null);
				DynamicTests.MemberAccess(a * b);
				DynamicTests.MemberAccess(a * 1);
				DynamicTests.MemberAccess(a * null);
				DynamicTests.MemberAccess(a / b);
				DynamicTests.MemberAccess(a / 1);
				DynamicTests.MemberAccess(a / null);
				DynamicTests.MemberAccess(a % b);
				DynamicTests.MemberAccess(a % 1);
				DynamicTests.MemberAccess(a % null);
			}
		}

		private static void UncheckedArithmeticBinaryOperators(dynamic a, dynamic b)
		{
			checked {
				DynamicTests.MemberAccess(a + b);
				DynamicTests.MemberAccess(a + 1);
				DynamicTests.MemberAccess(a + null);
				DynamicTests.MemberAccess(unchecked(a - b));
				DynamicTests.MemberAccess(a - 1);
				DynamicTests.MemberAccess(a - null);
				DynamicTests.MemberAccess(unchecked(a * b));
				DynamicTests.MemberAccess(a * 1);
				DynamicTests.MemberAccess(a * null);
				DynamicTests.MemberAccess(a / b);
				DynamicTests.MemberAccess(a / 1);
				DynamicTests.MemberAccess(a / null);
				DynamicTests.MemberAccess(a % b);
				DynamicTests.MemberAccess(a % 1);
				DynamicTests.MemberAccess(a % null);
			}
		}

		private static void RelationalOperators(dynamic a, dynamic b)
		{
			DynamicTests.MemberAccess(a == b);
			DynamicTests.MemberAccess(a == 1);
			DynamicTests.MemberAccess(a == null);
			DynamicTests.MemberAccess(a != b);
			DynamicTests.MemberAccess(a != 1);
			DynamicTests.MemberAccess(a != null);
			DynamicTests.MemberAccess(a < b);
			DynamicTests.MemberAccess(a < 1);
			DynamicTests.MemberAccess(a < null);
			DynamicTests.MemberAccess(a > b);
			DynamicTests.MemberAccess(a > 1);
			DynamicTests.MemberAccess(a > null);
			DynamicTests.MemberAccess(a >= b);
			DynamicTests.MemberAccess(a >= 1);
			DynamicTests.MemberAccess(a >= null);
			DynamicTests.MemberAccess(a <= b);
			DynamicTests.MemberAccess(a <= 1);
			DynamicTests.MemberAccess(a <= null);
		}

		private static void Casts(dynamic a)
		{
			Console.WriteLine();
			MemberAccess((int)a);
			MemberAccess(checked((int)a));
		}

		private static void M(object o)
		{
		}

		private static void M2(dynamic d)
		{
		}

		private static void M3(int i)
		{
		}

		private static void NotDynamicDispatch(dynamic d)
		{
			DynamicTests.M(d);
			M((object)d);
			DynamicTests.M2(d);
			M2((object)d);
		}

		private static void CompoundAssignment(dynamic a, dynamic b)
		{
			a.Setter2 += 5;
			a.Setter2 -= 1;
			a.Setter2 *= 2;
			a.Setter2 /= 5;
			a.Setter2 += b;
			a.Setter2 -= b;
			a.Setter2 *= b;
			a.Setter2 /= b;
			field.Setter += 5;
			field.Setter -= 5;
		}

		private static void InlineCompoundAssignment(dynamic a, dynamic b)
		{
			Console.WriteLine(a.Setter2 += 5);
			Console.WriteLine(a.Setter2 -= 1);
			Console.WriteLine(a.Setter2 *= 2);
			Console.WriteLine(a.Setter2 /= 5);
			Console.WriteLine(a.Setter2 += b);
			Console.WriteLine(a.Setter2 -= b);
			Console.WriteLine(a.Setter2 *= b);
			Console.WriteLine(a.Setter2 /= b);
		}

		private static void UnaryOperators(dynamic a)
		{
			// TODO : beautify inc/dec on locals and fields
			//a--;
			//a++;
			//--a;
			//++a;
			DynamicTests.Casts(-a);
			DynamicTests.Casts(+a);
		}

		private static void Loops(dynamic list)
		{
			foreach (dynamic item in list) {
				DynamicTests.UnaryOperators(item);
			}
		}

		private static void If(dynamic a, dynamic b)
		{
			if (a == b) {
				Console.WriteLine("Equal");
			}
		}

		private static void If2(dynamic a, dynamic b)
		{
			if (a == null || b == null) {
				Console.WriteLine("One is null");
			}
		}

		private static void If3(dynamic a, dynamic b)
		{
			if (a == null && b == null) {
				Console.WriteLine("Both are null");
			}
		}

		private static void If4(dynamic a, dynamic b)
		{
			if ((a == null || b == null) && GetDynamic(1) && !(GetDynamic(2) && GetDynamic(3))) {
				Console.WriteLine("then");
			} else {
				Console.WriteLine("else");
			}
		}

		private static bool ConstantTarget(dynamic a)
		{
			return true.Equals(a);
		}

		private static dynamic GetDynamic(int i)
		{
			return null;
		}

		private static bool GetBool(int i)
		{
			return false;
		}

		private static dynamic LogicAnd()
		{
			return GetDynamic(1) && GetDynamic(2);
		}

		private static dynamic LogicAnd(dynamic a, dynamic b)
		{
			return a && b;
		}

		private static void LogicAndExtended(int i, dynamic d)
		{
			Console.WriteLine(GetDynamic(1) && GetDynamic(2));
			Console.WriteLine(GetDynamic(1) && GetBool(2));
			Console.WriteLine(GetBool(1) && GetDynamic(2));
			Console.WriteLine(i == 1 && d == null);
		}

		private static dynamic LogicOr()
		{
			return GetDynamic(1) || GetDynamic(2);
		}

		private static dynamic LogicOr(dynamic a, dynamic b)
		{
			return a || b;
		}

		private static void LogicOrExtended(int i, dynamic d)
		{
			Console.WriteLine(GetDynamic(1) || GetDynamic(2));
			Console.WriteLine(GetDynamic(1) || GetBool(2));
			Console.WriteLine(GetBool(1) || GetDynamic(2));
			Console.WriteLine(i == 1 || d == null);
		}

		private static int ImplicitCast(object o)
		{
			return (dynamic)o;
		}

		private static int ExplicitCast(object o)
		{
			return (int)(dynamic)o;
		}
	}

	internal static class Extension
	{
		public static dynamic ToDynamic(this int i, dynamic info)
		{
			throw null;
		}
	}
}
