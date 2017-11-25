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
using System.Xml;

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
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => a);
	}
	
	public void LocalVariable()
	{
		bool a = true;
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => a);
	}
	
	public void LambdaParameter()
	{
		ExpressionTrees.ToCode(ExpressionTrees.X(), (bool a) => a);
	}
	
	public void AddOperator(int x)
	{
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => 1 + x + 2);
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
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => (new int[3] {
			3,
			4,
			5
		})[0 + (int)(DateTime.Now.Ticks % 3)]);
	}
	
	public void ArrayLengthAndDoubles()
	{
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => new double[3] {
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
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => new object() as string);
	}
	
	public void ComplexGenericName()
	{
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => ((Func<int, bool>)((int x) => x > 0))(0));
	}
	
	public void DefaultValue()
	{
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => new TimeSpan(1, 2, 3) == default(TimeSpan));
	}
	
	public void EnumConstant()
	{
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => new object().Equals(MidpointRounding.ToEven));
	}
	
	public void IndexerAccess()
	{
		Dictionary<string, int> dict = Enumerable.Range(1, 20).ToDictionary((int n) => n.ToString());
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => dict["3"] == 3);
	}
	
	public void IsOperator()
	{
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => new object() is string);
	}
	
	public void ListInitializer()
	{
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => new Dictionary<int, int> {
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
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => new List<int>(50) {
			1,
			2,
			3
		}.Count == 3);
	}
	
	public void ListInitializer3()
	{
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => new List<int> {
			1,
			2,
			3
		}.Count == 3);
	}
	
	public void LiteralCharAndProperty()
	{
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => new string(' ', 3).Length == 1);
	}
	
	public void CharNoCast()
	{
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => "abc"[1] == 'b');
	}
	
	public void StringsImplicitCast()
	{
		int i = 1;
		string x = "X";
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => ((("a\n\\b" ?? x) + x).Length == 2) ? false : (true && (1m + (decimal)(-i) > 0m || false)));
	}
	
	public void NotImplicitCast()
	{
		byte z = 42;
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => ~z == 0);
	}
	
	public void MembersBuiltin()
	{
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => 1.23m.ToString());
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => AttributeTargets.All.HasFlag((Enum)AttributeTargets.Assembly));
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => "abc".Length == 3);
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => 'a'.CompareTo('b') < 0);
	}
	
	public void MembersDefault()
	{
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => default(DateTime).Ticks == 0);
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => ((Array)null).Length == 0);
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => ((Type)null).IsLayoutSequential);
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => ((List<int>)null).Count);
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => ((Array)null).Clone() == null);
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => ((Type)null).IsInstanceOfType(new object()));
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => ((List<int>)null).AsReadOnly());
	}
	
	public void DoAssert()
	{
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => this.field != this.C());
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => !object.ReferenceEquals(this, new ExpressionTrees()));
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => this.MyEquals(this) && !this.MyEquals(null));
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
		ExpressionTrees.ToCode(ExpressionTrees.X(), (Expression<Func<Func<bool>>>)(() => ((IEnumerable<int>)new int[4] {
			2000,
			2004,
			2008,
			2012
		}).Any<int>));
	}
	
	public void MethodGroupConstant()
	{
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => Array.TrueForAll(new int[4] {
			2000,
			2004,
			2008,
			2012
		}, DateTime.IsLeapYear));
		
		HashSet<int> set = new HashSet<int>();
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => new int[4] {
			2000,
			2004,
			2008,
			2012
		}.All(set.Add));
		
		Func<Func<object, object, bool>, bool> sink = (Func<object, object, bool> f) => f(null, null);
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => sink(object.Equals));
	}
	
	public void MultipleCasts()
	{
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => 1 == (int)(object)1);
	}
	
	public void MultipleDots()
	{
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => 3.ToString().ToString().Length > 0);
	}
	
	public void NestedLambda()
	{
		Func<Func<int>, int> call = (Func<int> f) => f();
		//no params
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => call(() => 42));
		//one param
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => from x in new int[2] {
			37,
			42
		}
		select x * 2);
		//two params
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => new int[2] {
			37,
			42
		}.Select((int x, int i) => x * 2));
	}
	
	public void CurriedLambda()
	{
		ExpressionTrees.ToCode(ExpressionTrees.X(), (Expression<Func<int, Func<int, Func<int, int>>>>)((int a) => (int b) => (int c) => a + b + c));
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
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => this.Fizz((string x) => x == "a"));
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => this.Fizz((string x) => x != "a"));
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => this.Fizz((Action x) => x == new Action(this.NestedLambda2)));
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => this.Fizz((Action x) => x != new Action(this.NestedLambda2)));
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => this.Fizz((int x) => x == 37));
		
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => this.Fizz((int x) => true));
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => this.Buzz((int x) => true));
	}
	
	public void NewArrayAndExtensionMethod()
	{
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => new double[3] {
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
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => new int[3, 4].Length == 1);
	}
	
	public void NewObject()
	{
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => new object() != new object());
	}
	
	public void NotOperator()
	{
		bool x = true;
		int y = 3;
		byte z = 42;
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => ~z == 0);
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => ~y == 0);
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => !x);
	}
	
	public void ObjectInitializers()
	{
		XmlReaderSettings s = new XmlReaderSettings {
			CloseInput = false,
			CheckCharacters = false
		};
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => new XmlReaderSettings {
			CloseInput = s.CloseInput,
			CheckCharacters = s.CheckCharacters
		}.Equals(s));
	}
	
	public void Quoted()
	{
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => (Expression<Func<int, string, string>>)((int n, string s) => s + n.ToString()) != null);
	}
	
	public void Quoted2()
	{
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => ExpressionTrees.ToCode(ExpressionTrees.X(), () => true).Equals(null));
	}
	
	public void QuotedWithAnonymous()
	{
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => (from o in new[] {
			new {
				X = "a",
				Y = "b"
			}
		}
		select o.X + o.Y).Single());
	}
	
	public void StaticCall()
	{
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => object.Equals(3, 0));
	}
	
	public void ThisCall()
	{
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => !this.Equals(3));
	}
	
	public void ThisExplicit()
	{
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => object.Equals(this, 3));
	}
	
	public void TypedConstant()
	{
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => new Type[2] {
			typeof(int),
			typeof(string)
		});
	}
	
	public void StaticCallImplicitCast()
	{
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => object.Equals(3, 0));
	}
	
	public void StaticMembers()
	{
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => (DateTime.Now > DateTime.Now + TimeSpan.FromMilliseconds(10.001)).ToString() == "False");
	}
	
	public void Strings()
	{
		int i = 1;
		string x = "X";
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => ((("a\n\\b" ?? x) + x).Length == 2) ? false : (true && (1m + (decimal)(-i) > 0m || false)));
	}
	
	public void GenericClassInstance()
	{
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => (double)new GenericClass<int>().InstanceField + new GenericClass<double>().InstanceProperty);
	}
	
	public void GenericClassStatic()
	{
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => (double)GenericClass<int>.StaticField + GenericClass<double>.StaticProperty);
	}
	
	public void InvokeGenericMethod()
	{
		ExpressionTrees.ToCode(ExpressionTrees.X(), () => GenericClass<int>.GenericMethod<double>());
	}
}
