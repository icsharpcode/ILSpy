#pragma warning disable format
// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class ExpressionTrees
	{
		private class GenericClass<X>
		{
			public static X StaticField;
			public X InstanceField;

			public static X StaticProperty {
				get;
				set;
			}

			public X InstanceProperty {
				get;
				set;
			}

			public static bool GenericMethod<Y>()
			{
				return false;
			}
		}

		internal class GenericClassWithCtor<T>
		{
		}

		internal class GenericClassWithMultipleCtors<T>
		{
			public GenericClassWithMultipleCtors()
			{
			}

			public GenericClassWithMultipleCtors(int x)
			{
			}
		}

		private class AssertTest
		{
			private struct DataStruct
			{
				private int dummy;
			}

			private struct WrapperStruct
			{
				internal DataStruct Data;
			}

			private class SomeClass
			{
				internal WrapperStruct DataWrapper;
			}

			private SomeClass someClass;

			public void Test()
			{
				GetMember(() => someClass.DataWrapper.Data);
			}

			public static MemberInfo GetMember<T>(Expression<Func<T>> p)
			{
				return null;
			}
		}

		public class Administrator
		{
			public int ID {
				get;
				set;
			}

			public string TrueName {
				get;
				set;
			}

			public string Phone {
				get;
				set;
			}
		}

		public class Contract
		{
			public int ID {
				get;
				set;
			}

			public string ContractNo {
				get;
				set;
			}

			public string HouseAddress {
				get;
				set;
			}

			public DateTime SigningTime {
				get;
				set;
			}

			public string BuyerName {
				get;
				set;
			}

			public string BuyerTelephone {
				get;
				set;
			}

			public string Customer {
				get;
				set;
			}

			public string CustTelephone {
				get;
				set;
			}

			public int AdminID {
				get;
				set;
			}

			public int StoreID {
				get;
				set;
			}
		}

		public class Database
		{
			public IQueryable<Contract> Contracts {
				get;
				set;
			}

			public IQueryable<Loan> Loan {
				get;
				set;
			}

			public IQueryable<Administrator> Administrator {
				get;
				set;
			}

			public IQueryable<Store> Store {
				get;
				set;
			}
		}

		public class Loan
		{
			public string ContractNo {
				get;
				set;
			}

			public DateTime? ShenDate {
				get;
				set;
			}

			public DateTime? LoanDate {
				get;
				set;
			}

			public string Credit {
				get;
				set;
			}

			public string LoanBank {
				get;
				set;
			}

			public string Remarks {
				get;
				set;
			}
		}

		public class Store
		{
			public int ID {
				get;
				set;
			}

			public string Name {
				get;
				set;
			}
		}

		internal class MyClass
		{
			public static MyClass operator +(MyClass a, MyClass b)
			{
				return new MyClass();
			}
		}

		internal class SimpleType
		{
			public const int ConstField = 1;

			public static readonly int StaticReadonlyField = 2;

			public static int StaticField = 3;

			public readonly int ReadonlyField = 2;

			public int Field = 3;

#if CS60
			public static int StaticReadonlyProperty => 0;
#else
		public static int StaticReadonlyProperty {
			get {
				return 0;
			}
		}
#endif

			public static int StaticProperty {
				get;
				set;
			}

#if CS60
			public int ReadonlyProperty => 0;
#else
		public int ReadonlyProperty {
			get {
				return 0;
			}
		}
#endif

			public int Property {
				get;
				set;
			}
		}

		internal class SimpleTypeWithCtor
		{
			public SimpleTypeWithCtor(int i)
			{
			}
		}

		internal class SimpleTypeWithMultipleCtors
		{
			public SimpleTypeWithMultipleCtors()
			{
			}

			public SimpleTypeWithMultipleCtors(int i)
			{
			}
		}

		private int field;
		private Database db;
		private dynamic ViewBag;

		public static readonly object[] SupportedMethods = new object[2] {
			ToCode(null, () => ((IQueryable<object>)null).Aggregate((object o1, object o2) => null)),
			ToCode(null, () => ((IEnumerable<object>)null).Aggregate((object o1, object o2) => null))
		};

		public static readonly object[] SupportedMethods2 = new object[4] {
			ToCode(null, () => ((IQueryable<object>)null).Aggregate(null, (object o1, object o2) => null)),
			ToCode(null, () => ((IQueryable<object>)null).Aggregate((object)null, (Expression<Func<object, object, object>>)((object o1, object o2) => null), (Expression<Func<object, object>>)((object o) => null))),
			ToCode(null, () => ((IEnumerable<object>)null).Aggregate(null, (object o1, object o2) => null)),
			ToCode(null, () => ((IEnumerable<object>)null).Aggregate((object)null, (Func<object, object, object>)((object o1, object o2) => null), (Func<object, object>)((object o) => null)))
		};

		public static void TestCall(object a)
		{

		}

		public static void TestCall(ref object a)
		{

		}

		private void Issue1249(int ID)
		{
			if (ID == 0)
			{
				ViewBag.data = "''";
				return;
			}
			var model = (from a in db.Contracts
						 where a.ID == ID
						 select new {
							 ID = a.ID,
							 ContractNo = a.ContractNo,
							 HouseAddress = a.HouseAddress,
							 AdminID = (from b in db.Administrator
										where b.ID == a.AdminID
										select b.TrueName).FirstOrDefault(),
							 StoreID = (from b in db.Store
										where b.ID == a.StoreID
										select b.Name).FirstOrDefault(),
							 SigningTime = a.SigningTime,
							 YeWuPhone = (from b in db.Administrator
										  where b.ID == a.AdminID
										  select b.Phone).FirstOrDefault(),
							 BuyerName = a.BuyerName,
							 BuyerTelephone = a.BuyerTelephone,
							 Customer = a.Customer,
							 CustTelephone = a.CustTelephone,
							 Credit = (from b in db.Loan
									   where b.ContractNo == a.ContractNo
									   select b.Credit).FirstOrDefault(),
							 LoanBank = (from b in db.Loan
										 where b.ContractNo == a.ContractNo
										 select b.LoanBank).FirstOrDefault(),
							 Remarks = (from b in db.Loan
										where b.ContractNo == a.ContractNo
										select b.Remarks).FirstOrDefault()
						 }).FirstOrDefault();
			ViewBag.data = model.ToJson();
			DateTime? dateTime = (from b in db.Loan
								  where b.ContractNo == model.ContractNo
								  select b.ShenDate).FirstOrDefault();
			DateTime? dateTime2 = (from b in db.Loan
								   where b.ContractNo == model.ContractNo
								   select b.LoanDate).FirstOrDefault();
			ViewBag.ShenDate = ((!dateTime.HasValue) ? "" : dateTime.ParseDateTime().ToString("yyyy-MM-dd"));
			ViewBag.LoanDate = ((!dateTime2.HasValue) ? "" : dateTime2.ParseDateTime().ToString("yyyy-MM-dd"));
		}

		private static object ToCode<R>(object x, Expression<Action<R>> expr)
		{
			return expr;
		}

		private static object ToCode<R>(object x, Expression<Func<R>> expr)
		{
			return expr;
		}

		private static object ToCode<T, R>(object x, Expression<Func<T, R>> expr)
		{
			return expr;
		}

		private static object ToCode<T, T2, R>(object x, Expression<Func<T, T2, R>> expr)
		{
			return expr;
		}

		private static object X()
		{
			return null;
		}

		public void Parameter(bool a)
		{
			ToCode(X(), () => a);
		}

		public void LocalVariable()
		{
			bool a = true;
			ToCode(X(), () => a);
		}

		public void LambdaParameter()
		{
			ToCode(X(), (bool a) => a);
		}

		public void AddOperator(int x)
		{
			ToCode(X(), () => 1 + x + 2);
		}

		public void AnonymousClasses()
		{
			ToCode(X(), () => new {
				X = 3,
				A = "a"
			});
		}

		public void ArrayIndex()
		{
			ToCode(X(), () => (new int[3] {
			3,
			4,
			5
		})[0 + (int)(DateTime.Now.Ticks % 3)]);
		}

		public void ArrayLengthAndDoubles()
		{
			ToCode(X(), () => new double[3] {
			1.0,
			2.01,
			3.5
		}.Concat(new double[2] {
			1.0,
			2.0
		}).ToArray().Length);
		}

		public void AsOperator()
		{
			ToCode(X(), () => new object() as string);
		}

		public void ComplexGenericName()
		{
			ToCode(X(), () => ((Func<int, bool>)((int x) => x > 0))(0));
		}

		public void DefaultValue()
		{
			ToCode(X(), () => new TimeSpan(1, 2, 3) == default(TimeSpan));
		}

		public void EnumConstant()
		{
			ToCode(X(), () => new object().Equals(MidpointRounding.ToEven));
		}

		public void IndexerAccess()
		{
			Dictionary<string, int> dict = Enumerable.Range(1, 20).ToDictionary((int n) => n.ToString());
			ToCode(X(), () => dict["3"] == 3);
		}

		public void IsOperator()
		{
			ToCode(X(), () => new object() is string);
		}

		public void ListInitializer()
		{
			ToCode(X(), () => new Dictionary<int, int> {
			{
				1,
				1
			},
			{
				2,
				2
			},
			{
				3,
				4
			}
		}.Count == 3);
		}

		public void ListInitializer2()
		{
			ToCode(X(), () => new List<int>(50) {
			1,
			2,
			3
		}.Count == 3);
		}

		public void ListInitializer3()
		{
			ToCode(X(), () => new List<int> {
			1,
			2,
			3
		}.Count == 3);
		}

		public void LiteralCharAndProperty()
		{
			ToCode(X(), () => new string(' ', 3).Length == 1);
		}

		public void CharNoCast()
		{
			ToCode(X(), () => "abc"[1] == 'b');
		}

		public void StringsImplicitCast()
		{
			int i = 1;
			string x = "X";
			ToCode(X(), () => ((("a\n\\b" ?? x) + x).Length == 2) ? false : (true && (1m + (decimal)(-i) > 0m || false)));
		}

		public void NotImplicitCast()
		{
			byte z = 42;
			ToCode(X(), () => ~z == 0);
		}

		public void MembersBuiltin()
		{
			ToCode(X(), () => 1.23m.ToString());
			ToCode(X(), () => AttributeTargets.All.HasFlag(AttributeTargets.Assembly));
			ToCode(X(), () => "abc".Length == 3);
			ToCode(X(), () => 'a'.CompareTo('b') < 0);
		}

		public void MembersDefault()
		{
			ToCode(X(), () => default(DateTime).Ticks == 0);
			ToCode(X(), () => ((Array)null).Length == 0);
			ToCode(X(), () => ((Type)null).IsLayoutSequential);
			ToCode(X(), () => ((List<int>)null).Count);
			ToCode(X(), () => ((Array)null).Clone() == null);
			ToCode(X(), () => ((Type)null).IsInstanceOfType(new object()));
			ToCode(X(), () => ((List<int>)null).AsReadOnly());
		}

		public void DoAssert()
		{
			ToCode(X(), () => field != C());
			ToCode(X(), () => !object.ReferenceEquals(this, new ExpressionTrees()));
			ToCode(X(), () => MyEquals(this) && !MyEquals(null));
		}

		private int C()
		{
			throw new NotImplementedException();
		}

		private bool MyEquals(ExpressionTrees other)
		{
			throw new NotImplementedException();
		}

		public void MethodGroupAsExtensionMethod()
		{
			ToCode(X(), (Expression<Func<Func<bool>>>)(() => ((IEnumerable<int>)new int[4] {
			2000,
			2004,
			2008,
			2012
		}).Any));
		}

		public void MethodGroupConstant()
		{
			ToCode(X(), () => Array.TrueForAll(new int[4] {
			2000,
			2004,
			2008,
			2012
		}, DateTime.IsLeapYear));

			HashSet<int> set = new HashSet<int>();
			ToCode(X(), () => new int[4] {
			2000,
			2004,
			2008,
			2012
		}.All(set.Add));

			Func<Func<object, object, bool>, bool> sink = (Func<object, object, bool> f) => f(null, null);
			ToCode(X(), () => sink(object.Equals));
		}

		public void MultipleCasts()
		{
			ToCode(X(), () => 1 == (int)(object)1);
		}

		public void MultipleDots()
		{
			ToCode(X(), () => 3.ToString().ToString().Length > 0);
		}

		public void NestedLambda()
		{
			Func<Func<int>, int> call = (Func<int> f) => f();
			//no params
			ToCode(X(), () => call(() => 42));
			//one param
			ToCode(X(), () => new int[2] {
				37,
				42
			}.Select((int x) => x * 2));
			//two params
			ToCode(X(), () => new int[2] {
				37,
				42
			}.Select((int x, int i) => x * 2));
		}

		public void CurriedLambda()
		{
			ToCode(X(), (Expression<Func<int, Func<int, Func<int, int>>>>)((int a) => (int b) => (int c) => a + b + c));
		}

		private bool Fizz(Func<int, bool> a)
		{
			return a(42);
		}

		private bool Buzz(Func<int, bool> a)
		{
			return a(42);
		}

		private bool Fizz(Func<string, bool> a)
		{
			return a("42");
		}

		private bool Fizz(Func<Action, bool> a)
		{
			return a(null);
		}

		public void NestedLambda2()
		{
			ToCode(X(), () => Fizz((string x) => x == "a"));
			ToCode(X(), () => Fizz((string x) => x != "a"));
			ToCode(X(), () => Fizz((Action x) => x == new Action(NestedLambda2)));
			ToCode(X(), () => Fizz((Action x) => x != new Action(NestedLambda2)));
			ToCode(X(), () => Fizz((int x) => x == 37));

			ToCode(X(), () => Fizz((int x) => true));
			ToCode(X(), () => Buzz((int x) => true));
		}

		public void NewArrayAndExtensionMethod()
		{
			ToCode(X(), () => new double[3] {
			1.0,
			2.01,
			3.5
		}.SequenceEqual(new double[3] {
			1.0,
			2.01,
			3.5
		}));
		}

		public void NewMultiDimArray()
		{
			ToCode(X(), () => new int[3, 4].Length == 1);
		}

		public void NewObject()
		{
			ToCode(X(), () => new object() != new object());
		}

		public void NotOperator()
		{
			bool x = true;
			int y = 3;
			byte z = 42;
			ToCode(X(), () => ~z == 0);
			ToCode(X(), () => ~y == 0);
			ToCode(X(), () => !x);
		}

		public void ObjectInitializers()
		{
			XmlReaderSettings s = new XmlReaderSettings {
				CloseInput = false,
				CheckCharacters = false
			};
			ToCode(X(), () => new XmlReaderSettings {
				CloseInput = s.CloseInput,
				CheckCharacters = s.CheckCharacters
			}.Equals(s));
		}

		public void Quoted()
		{
			ToCode(X(), () => (Expression<Func<int, string, string>>)((int n, string s) => s + n.ToString()) != null);
		}

		public void Quoted2()
		{
			ToCode(X(), () => ToCode(X(), () => true).Equals(null));
		}

		public void QuotedWithAnonymous()
		{
			ToCode(X(), () => new[] {
				new {
					X = "a",
					Y = "b"
				}
			}.Select(o => o.X + o.Y).Single());
		}

		public void StaticCall()
		{
			ToCode(X(), () => object.Equals(3, 0));
		}

		public void ThisCall()
		{
			ToCode(X(), () => !Equals(3));
		}

		public void ThisExplicit()
		{
			ToCode(X(), () => object.Equals(this, 3));
		}

		public void TypedConstant()
		{
			ToCode(X(), () => new Type[2] {
				typeof(int),
				typeof(string)
			});
		}

		public void StaticCallImplicitCast()
		{
			ToCode(X(), () => object.Equals(3, 0));
		}

		public void StaticMembers()
		{
			ToCode(X(), () => (DateTime.Now > DateTime.Now + TimeSpan.FromMilliseconds(10.001)).ToString() == "False");
		}

		public void Strings()
		{
			int i = 1;
			string x = "X";
			ToCode(X(), () => ((("a\n\\b" ?? x) + x).Length == 2) ? false : (true && (1m + (decimal)(-i) > 0m || false)));
		}

		public void GenericClassInstance()
		{
			ToCode(X(), () => (double)new GenericClass<int>().InstanceField + new GenericClass<double>().InstanceProperty);
		}

		public void GenericClassStatic()
		{
			ToCode(X(), () => (double)GenericClass<int>.StaticField + GenericClass<double>.StaticProperty);
		}

		public void InvokeGenericMethod()
		{
			ToCode(X(), () => GenericClass<int>.GenericMethod<double>());
		}

		private static void Test<T>(T delegateExpression, Expression<T> expressionTree)
		{
		}

		public static void ArrayIndexer()
		{
			Test<Func<int[], int>>((int[] array) => array[0], (int[] array) => array[0]);
			Test<Func<int[], int, int>>((int[] array, int index) => array[index], (int[] array, int index) => array[index]);
			Test<Func<int[,], int>>((int[,] array) => array[0, 5], (int[,] array) => array[0, 5]);
			Test<Func<int[,], int, int>>((int[,] array, int index) => array[index, 7], (int[,] array, int index) => array[index, 7]);
			Test<Func<int[][], int, int>>((int[][] array, int index) => array[index][7], (int[][] array, int index) => array[index][7]);
		}

		public static void ArrayLength()
		{
			Test<Func<int[], int>>((int[] array) => array.Length, (int[] array) => array.Length);
			Test<Func<int>>(() => ((Array)null).Length, () => ((Array)null).Length);
		}

		public static void NewObj()
		{
			Test<Func<object>>(() => new SimpleType(), () => new SimpleType());
			Test<Func<object>>(() => new SimpleTypeWithCtor(5), () => new SimpleTypeWithCtor(5));
			Test<Func<object>>(() => new SimpleTypeWithMultipleCtors(), () => new SimpleTypeWithMultipleCtors());
			Test<Func<object>>(() => new SimpleTypeWithMultipleCtors(5), () => new SimpleTypeWithMultipleCtors(5));
			Test<Func<object>>(() => new GenericClass<int>(), () => new GenericClass<int>());
			Test<Func<object>>(() => new GenericClassWithCtor<int>(), () => new GenericClassWithCtor<int>());
			Test<Func<object>>(() => new GenericClassWithMultipleCtors<int>(5), () => new GenericClassWithMultipleCtors<int>(5));
		}

		public unsafe static void TypeOfExpr()
		{
			Test<Func<Type>>(() => typeof(int), () => typeof(int));
			Test<Func<Type>>(() => typeof(object), () => typeof(object));
			Test<Func<Type>>(() => typeof(List<>), () => typeof(List<>));
			Test<Func<Type>>(() => typeof(List<int>), () => typeof(List<int>));
			Test<Func<Type>>(() => typeof(int*), () => typeof(int*));
		}

		public static void AsTypeExpr()
		{
			Test<Func<object, MyClass>>((object obj) => obj as MyClass, (object obj) => obj as MyClass);
			Test<Func<object, int?>>((object obj) => obj as int?, (object obj) => obj as int?);
			Test<Func<object, GenericClass<object>>>((object obj) => obj as GenericClass<object>, (object obj) => obj as GenericClass<object>);
		}

		public static void IsTypeExpr()
		{
			Test<Func<object, bool>>((object obj) => obj is MyClass, (object obj) => obj is MyClass);
			Test<Func<object, bool>>((object obj) => obj is int?, (object obj) => obj is int?);
		}

		public static void UnaryLogicalOperators()
		{
			Test<Func<bool, bool>>((bool a) => !a, (bool a) => !a);
		}

		public static void ConditionalOperator()
		{
			ToCode(null, (bool a) => a ? 5 : 10);
			ToCode(null, (object a) => a ?? new MyClass());
		}

		public static void ComparisonOperators()
		{
			ToCode(null, (int a, int b) => a == b);
			ToCode(null, (int a, int b) => a != b);
			ToCode(null, (int a, int b) => a < b);
			ToCode(null, (int a, int b) => a <= b);
			ToCode(null, (int a, int b) => a > b);
			ToCode(null, (int a, int b) => a >= b);
			ToCode(null, (int a, int b) => a == 1 && b == 2);
			ToCode(null, (int a, int b) => a == 1 || b == 2);
			ToCode(null, (int a, short b) => a == b);
			ToCode(null, (ushort a, int b) => a != b);
			ToCode(null, (int a, long b) => (long)a < b);
			ToCode(null, (ulong a, uint b) => a <= (ulong)b);
			ToCode(null, (int a, uint b) => (long)a <= (long)b);
			ToCode(null, (int a, long b) => (long)a > b);
			ToCode(null, (short a, long b) => (long)a >= b);
			ToCode(null, (int a, int b) => a == 1 && b == 2);
			ToCode(null, (int a, int b) => a == 1 || b == 2);
		}

		public static void LiftedComparisonOperators()
		{
			ToCode(X(), (int? a, int? b) => a == b);
			ToCode(X(), (int? a, int? b) => a != b);
			ToCode(X(), (int? a, int? b) => a < b);
			ToCode(X(), (int? a, int? b) => a <= b);
			ToCode(X(), (int? a, int? b) => a > b);
			ToCode(X(), (int? a, int? b) => a >= b);
		}

		public static void UnaryArithmeticOperators()
		{
			Test<Func<int, int>>((int a) => a, (int a) => a);
			Test<Func<int, int>>((int a) => -a, (int a) => -a);
		}

		public static void BinaryArithmeticOperators()
		{
			Test<Func<int, int, int>>((int a, int b) => a + b, (int a, int b) => a + b);
			Test<Func<int, int, int>>((int a, int b) => a - b, (int a, int b) => a - b);
			Test<Func<int, int, int>>((int a, int b) => a * b, (int a, int b) => a * b);
			Test<Func<int, int, int>>((int a, int b) => a / b, (int a, int b) => a / b);
			Test<Func<int, int, int>>((int a, int b) => a % b, (int a, int b) => a % b);
			Test<Func<long, int, long>>((long a, int b) => a + b, (long a, int b) => a + (long)b);
			Test<Func<long, int, long>>((long a, int b) => a - b, (long a, int b) => a - (long)b);
			Test<Func<long, int, long>>((long a, int b) => a * b, (long a, int b) => a * (long)b);
			Test<Func<long, int, long>>((long a, int b) => a / b, (long a, int b) => a / (long)b);
			Test<Func<long, int, long>>((long a, int b) => a % b, (long a, int b) => a % (long)b);
			Test<Func<short, int, int>>((short a, int b) => a + b, (short a, int b) => a + b);
			Test<Func<int, short, int>>((int a, short b) => a - b, (int a, short b) => a - b);
			Test<Func<short, int, int>>((short a, int b) => a * b, (short a, int b) => a * b);
			Test<Func<int, short, int>>((int a, short b) => a / b, (int a, short b) => a / b);
			Test<Func<short, int, int>>((short a, int b) => a % b, (short a, int b) => a % b);
		}

		public static void BitOperators()
		{
			Test<Func<int, int>>((int a) => ~a, (int a) => ~a);
			Test<Func<int, int, int>>((int a, int b) => a & b, (int a, int b) => a & b);
			Test<Func<int, int, int>>((int a, int b) => a | b, (int a, int b) => a | b);
			Test<Func<int, int, int>>((int a, int b) => a ^ b, (int a, int b) => a ^ b);
		}

		public static void ShiftOperators()
		{
			Test<Func<int, int>>((int a) => a >> 2, (int a) => a >> 2);
			Test<Func<int, int>>((int a) => a << 2, (int a) => a << 2);
			Test<Func<long, long>>((long a) => a >> 2, (long a) => a >> 2);
			Test<Func<long, long>>((long a) => a << 2, (long a) => a << 2);
		}

		public static void SimpleExpressions()
		{
			Test<Func<int>>(() => 0, () => 0);
			Test<Func<int, int>>((int a) => a, (int a) => a);
		}

		public static void Capturing()
		{
			int captured = 5;
			Test<Func<int>>(() => captured, () => captured);
		}

		public static void FieldAndPropertyAccess()
		{
			ToCode(null, () => 1);
			ToCode(null, () => SimpleType.StaticField);
			ToCode(null, () => SimpleType.StaticReadonlyField);
			ToCode(null, () => SimpleType.StaticProperty);
			ToCode(null, () => SimpleType.StaticReadonlyProperty);
			ToCode(null, (SimpleType a) => a.Field);
			ToCode(null, (SimpleType a) => a.Property);
			ToCode(null, (SimpleType a) => a.ReadonlyField);
			ToCode(null, (SimpleType a) => a.ReadonlyProperty);
		}

		public static void Call()
		{
			ToCode(null, (string a) => Console.WriteLine(a));
			Test<Func<string, string>>((string a) => a.ToString(), (string a) => a.ToString());
			Test<Func<int, string>>((int a) => a.ToString(), (int a) => a.ToString());
			Test<Func<string, char[]>>((string a) => a.ToArray(), (string a) => a.ToArray());
			Test<Func<bool>>(() => 'a'.CompareTo('b') < 0, () => 'a'.CompareTo('b') < 0);
			Test<Action<object, bool>>(delegate (object lockObj, bool lockTaken) {
				Monitor.Enter(lockObj, ref lockTaken);
			}, (object lockObj, bool lockTaken) => Monitor.Enter(lockObj, ref lockTaken));
			Test<Func<string, int, bool>>((string str, int num) => int.TryParse(str, out num), (string str, int num) => int.TryParse(str, out num));
			Test<Func<string, SimpleType, bool>>((string str, SimpleType t) => int.TryParse(str, out t.Field), (string str, SimpleType t) => int.TryParse(str, out t.Field));
			Test<Action<object>>(delegate (object o) {
				TestCall(o);
			}, (object o) => TestCall(o));
			Test<Action<object>>(delegate (object o) {
				TestCall(ref o);
			}, (object o) => TestCall(ref o));
		}

		public static void Quote()
		{
			Test<Func<bool>>(() => (Expression<Func<int, string, string>>)((int n, string s) => s + n.ToString()) != null, () => (Expression<Func<int, string, string>>)((int n, string s) => s + n.ToString()) != null);
		}

		public static void ArrayInitializer()
		{
			Test<Func<int[]>>(() => new int[3] {
				1,
				2,
				3
			}, () => new int[3] {
				1,
				2,
				3
			});
			Test<Func<int[]>>(() => new int[3], () => new int[3]);
			Test<Func<int[,]>>(() => new int[3, 5], () => new int[3, 5]);
			Test<Func<int[][]>>(() => new int[3][], () => new int[3][]);
			Test<Func<int[][]>>(() => new int[1][] {
				new int[3] {
					1,
					2,
					3
				}
			}, () => new int[1][] {
				new int[3] {
					1,
					2,
					3
				}
			});
		}

		public static void AnonymousTypes()
		{
			Test<Func<object>>(() => new {
				A = 5,
				B = "Test"
			}, () => new {
				A = 5,
				B = "Test"
			});
		}

		public static void ObjectInit()
		{
			ToCode(null, () => new SimpleType {
				Property = 4,
				Field = 3
			});
		}

		public static void StringConcat()
		{
			Test<Func<string, object, string>>(null, (string a, object b) => a + b);
			Test<Func<string, object, string>>(null, (string a, object b) => a + b.ToString());
			Test<Func<string, int, string>>(null, (string a, int b) => a + b);
			Test<Func<string, int, string>>(null, (string a, int b) => a + b.ToString());
		}

		public async Task Issue1524(string str)
		{
			await Task.Delay(100);
#if CS70
			if (string.IsNullOrEmpty(str) && int.TryParse(str, out var id))
			{
#else
			int id;
			if (string.IsNullOrEmpty(str) && int.TryParse(str, out id))
			{
#endif
				(from a in new List<int>().AsQueryable()
				 where a == id
				 select a).FirstOrDefault();
			}
		}

		public void NullCoalescing()
		{
			Test<Func<string, string, string>>((string a, string b) => a ?? b, (string a, string b) => a ?? b);
			Test<Func<int?, int>>((int? a) => a ?? 1, (int? a) => a ?? 1);
		}
	}

	internal static class Extensions
	{
		public static dynamic ToJson(this object o)
		{
			return null;
		}

		public static DateTime ParseDateTime(this object str)
		{
			return default(DateTime);
		}
	}
}