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
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

public class ExpressionTrees
{
	private class XmlReaderSettings
	{
		internal bool CloseInput;
		internal bool CheckCharacters;
	}

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
	
	private int field;
	
	private static object ToCode<R>(object x, Expression<Func<R>> expr)
	{
		return expr;
	}
	
	private static object ToCode<T, R>(object x, Expression<Func<T, R>> expr)
	{
		return expr;
	}
	
	private static object X()
	{
		return null;
	}
	
	public void Parameter(bool a)
	{
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => a);
	}
	
	public void LocalVariable()
	{
		bool a = true;
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => a);
	}
	
	public void LambdaParameter()
	{
		ExpressionTrees.ToCode<bool, bool>(ExpressionTrees.X(), (bool a) => a);
	}
	
	public void AddOperator(int x)
	{
		ExpressionTrees.ToCode<int>(ExpressionTrees.X(), () => 1 + x + 2);
	}
	
	public void AnonymousClasses()
	{
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => new {
			X = 3,
			A = "a"
		});
	}
	
	public void ArrayIndex()
	{
		ExpressionTrees.ToCode<int>(ExpressionTrees.X(), () => (new int[] {
			3,
			4,
			5
		})[0 + (int)(System.DateTime.Now.Ticks % 3L)]);
	}
	
	public void ArrayLengthAndDoubles()
	{
		ExpressionTrees.ToCode<int>(ExpressionTrees.X(), () => System.Linq.Enumerable.ToArray<double>(System.Linq.Enumerable.Concat<double>(new double[] {
			1.0,
			2.01,
			3.5
		}, new double[] {
			1.0,
			2.0
		})).Length);
	}
	
	public void AsOperator()
	{
		ExpressionTrees.ToCode<string>(ExpressionTrees.X(), () => new object() as string);
	}
	
	public void ComplexGenericName()
	{
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => ((Func<int, bool>)((int x) => x > 0))(0));
	}
	
	public void DefaultValue()
	{
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => new System.TimeSpan(1, 2, 3) == default(TimeSpan));
	}
	
	public void EnumConstant()
	{
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => new object().Equals((object)MidpointRounding.ToEven));
	}
	
	public void IndexerAccess()
	{
		Dictionary<string, int> dict = Enumerable.Range(1, 20).ToDictionary((int n) => n.ToString());
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => dict["3"] == 3);
	}
	
	public void IsOperator()
	{
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => new object() is string);
	}
	
	public void ListInitializer()
	{
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => new Dictionary<int, int> {
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
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => new List<int>(50) {
			1,
			2,
			3
		}.Count == 3);
	}
	
	public void ListInitializer3()
	{
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => new List<int> {
			1,
			2,
			3
		}.Count == 3);
	}
	
	public void LiteralCharAndProperty()
	{
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => new string(' ', 3).Length == 1);
	}
	
	public void CharNoCast()
	{
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => (int)"abc"[1] == 98);
	}
	
	public void StringsImplicitCast()
	{
		int i = 1;
		string x = "X";
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => ((("a\n\\b" ?? x) + x).Length == 2) ? false : (true && (1m + (decimal)(-i) > 0m || false)));
	}
	
	public void NotImplicitCast()
	{
		byte z = 42;
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => ~(int)z == 0);
	}
	
	public void MembersBuiltin()
	{
		ExpressionTrees.ToCode<string>(ExpressionTrees.X(), () => 1.23m.ToString());
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => AttributeTargets.All.HasFlag((Enum)AttributeTargets.Assembly));
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => "abc".Length == 3);
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => 'a'.CompareTo('b') < 0);
	}
	
	public void MembersDefault()
	{
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => default(DateTime).Ticks == 0L);
	}

	public void Test()
	{
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => new int[0].Length == 0);
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => this.GetType().IsLayoutSequential);
		ExpressionTrees.ToCode<int>(ExpressionTrees.X(), () => new List<int>().Count);
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => new int[0].Clone() == null);
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => this.GetType().IsInstanceOfType(new object()));
		ExpressionTrees.ToCode<System.Collections.ObjectModel.ReadOnlyCollection<int>>(ExpressionTrees.X(), () => new List<int>().AsReadOnly());
	}
	
	public void DoAssert()
	{
		this.field = 37;
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => this.field != this.C());
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => !object.ReferenceEquals(this, new ExpressionTrees()));
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => this.MyEquals(this) && !this.MyEquals(new ExpressionTrees()));
	}
	
	private int C()
	{
		return this.field + 5;
	}
	
	private bool MyEquals(ExpressionTrees other)
	{
		if (other != null) {
			return this.field == other.field;
		}
		return false;
	}
	
	public void MethodGroupAsExtensionMethod()
	{
		ExpressionTrees.ToCode<System.Func<bool>>(ExpressionTrees.X(), () => (Func<bool>)new int[] {
			2000,
			2004,
			2008,
			2012
		}.Any);
	}
	
	public void MethodGroupConstant()
	{
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => System.Array.TrueForAll<int>(new int[] {
			2000,
			2004,
			2008,
			2012
		}, DateTime.IsLeapYear));
		
		HashSet<int> set = new HashSet<int>();
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => new int[] {
			2000,
			2004,
			2008,
			2012
		}.All(set.Add));
		
		Func<Func<object, object, bool>, bool> sink = (Func<object, object, bool> f) => f(null, null);
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => sink(int.Equals));
	}
	
	public void MultipleCasts()
	{
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => 1 == (int)(object)1);
	}
	
	public void MultipleDots()
	{
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => 3.ToString().ToString().Length > 0);
	}
	
	public void NestedLambda()
	{
		Func<Func<int>, int> call = (Func<int> f) => f();
		//no params
		ExpressionTrees.ToCode<int>(ExpressionTrees.X(), () => call(() => 42));
		//one param
		ExpressionTrees.ToCode<System.Collections.Generic.IEnumerable<int>>(ExpressionTrees.X(), () => System.Linq.Enumerable.Select<int, int>(new int[] {
			37,
			42
		}, (int x) => x * 2));
		//two params
		ExpressionTrees.ToCode<System.Collections.Generic.IEnumerable<int>>(ExpressionTrees.X(), () => System.Linq.Enumerable.Select<int, int>(new int[] {
			37,
			42
		}, (int x, int i) => x * 2));
	}
	
	public void CurriedLambda()
	{
		ExpressionTrees.ToCode<int, System.Func<int, System.Func<int, int>>>(ExpressionTrees.X(), (int a) => (int b) => (int c) => a + b + c);
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
	
	public void NestedLambda2()
	{
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => this.Fizz((string x) => x == "a"));
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => this.Fizz((int x) => x == 37));
		
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => this.Fizz((int x) => true));
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => this.Buzz((int x) => true));
	}
	
	public void NewArrayAndExtensionMethod()
	{
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => System.Linq.Enumerable.SequenceEqual<double>(new double[] {
			1.0,
			2.01,
			3.5
		}, new double[] {
			1.0,
			2.01,
			3.5
		}));
	}
	
	public void NewMultiDimArray()
	{
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => new int[3, 4].Length == 1);
	}
	
	public void NewObject()
	{
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => new object() != new object());
	}
	
	public void NotOperator()
	{
		bool x = true;
		int y = 3;
		byte z = 42;
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => ~(int)z == 0);
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => ~y == 0);
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => !x);
	}
	
	public void ObjectInitializers()
	{
		XmlReaderSettings s = new XmlReaderSettings {
			CloseInput = false,
			CheckCharacters = false
		};
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => new XmlReaderSettings {
			CloseInput = s.CloseInput,
			CheckCharacters = s.CheckCharacters
		}.Equals(s));
	}
	
	public void Quoted()
	{
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => (Expression<Func<int, string, string>>)((int n, string s) => s + n.ToString()) != null);
	}
	
	public void Quoted2()
	{
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => true).Equals(null));
	}
	
	public void QuotedWithAnonymous()
	{
		ExpressionTrees.ToCode<string>(ExpressionTrees.X(), () => System.Linq.Enumerable.Single<string>(System.Linq.Enumerable.Select(new[] {
			new {
				X = "a",
				Y = "b"
			}
		}, o => o.X + o.Y)));
	}
	
	public void StaticCall()
	{
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => object.Equals((object)3, (object)0));
	}
	
	public void ThisCall()
	{
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => !this.Equals((object)3));
	}
	
	public void ThisExplicit()
	{
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => object.Equals(this, (object)3));
	}
	
	public void TypedConstant()
	{
		ExpressionTrees.ToCode<System.Type[]>(ExpressionTrees.X(), () => new Type[] {
			typeof(int),
			typeof(string)
		});
	}
	
	public void StaticCallImplicitCast()
	{
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => object.Equals((object)3, (object)0));
	}
	
	public void StaticMembers()
	{
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => (System.DateTime.Now > System.DateTime.Now + System.TimeSpan.FromMilliseconds(10.001)).ToString() == "False");
	}
	
	public void Strings()
	{
		int i = 1;
		string x = "X";
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => ((("a\n\\b" ?? x) + x).Length == 2) ? false : (true && (1m + (decimal)(-i) > 0m || false)));
	}
	
	public void StringAccessor()
	{
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => (int)"abc"[1] == 98);
	}
	
	public void GenericClassInstance()
	{
		ExpressionTrees.ToCode<double>(ExpressionTrees.X(), () => (double)new GenericClass<int>().InstanceField + new GenericClass<double>().InstanceProperty);
	}
	
	public void GenericClassStatic()
	{
		ExpressionTrees.ToCode<double>(ExpressionTrees.X(), () => (double)GenericClass<int>.StaticField + GenericClass<double>.StaticProperty);
	}
	
	public void InvokeGenericMethod()
	{
		ExpressionTrees.ToCode<bool>(ExpressionTrees.X(), () => GenericClass<int>.GenericMethod<double>());
	}
}
