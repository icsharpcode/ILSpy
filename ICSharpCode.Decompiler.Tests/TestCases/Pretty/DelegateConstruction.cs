﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
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
using System.Threading;
#if CS100
using System.Threading.Tasks;
#endif

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction
{
	public static class DelegateConstruction
	{
		private class InstanceTests
		{
			public struct SomeData
			{
				public string Value;
			}

			private int x;

			public Action CaptureOfThis()
			{
				return delegate {
					CaptureOfThis();
				};
			}

			public Action CaptureOfThisAndParameter(int a)
			{
				return delegate {
					CaptureOfThisAndParameter(a);
				};
			}

			public Action CaptureOfThisAndParameterInForEach(int a)
			{
				foreach (int item in Enumerable.Empty<int>())
				{
					if (item > 0)
					{
						return delegate {
							CaptureOfThisAndParameter(item + a);
						};
					}
				}
				return null;
			}

			public Action CaptureOfThisAndParameterInForEachWithItemCopy(int a)
			{
				foreach (int item in Enumerable.Empty<int>())
				{
					int copyOfItem = item;
					if (item > 0)
					{
						return delegate {
							CaptureOfThisAndParameter(item + a + copyOfItem);
						};
					}
				}
				return null;
			}

			public void LambdaInForLoop()
			{
				for (int i = 0; i < 100000; i++)
				{
					Bar(() => Foo());
				}
			}

			public int Foo()
			{
				return 0;
			}

			public void Bar(Func<int> f)
			{
			}

			private void Bug955()
			{
				new Thread((ThreadStart)delegate {
				});
			}

			public void Bug951(int amount)
			{
				DoAction(delegate {
					if (amount < 0)
					{
						amount = 0;
					}
					DoAction(delegate {
						NoOp(amount);
					});
				});
			}

			public void Bug951b()
			{
				int amount = Foo();
				DoAction(delegate {
					if (amount < 0)
					{
						amount = 0;
					}
					DoAction(delegate {
						NoOp(amount);
					});
				});
			}

			public void Bug951c(SomeData data)
			{
				DoAction(delegate {
					DoAction(delegate {
						DoSomething(data.Value);
					});
				});
			}

			public Func<int, int> Issue2143()
			{
				return (int x) => this.x;
			}

			public Action<object> Bug971_DelegateWithoutParameterList()
			{
				return delegate {
				};
			}

			private void DoAction(Action action)
			{
			}

			private void NoOp(int a)
			{
			}

			private void DoSomething(string text)
			{
			}
		}

		public interface IM3
		{
			void M3();
		}

		public class BaseClass : IM3
		{
			protected virtual void M1()
			{
			}
			protected virtual void M2()
			{
			}
			public virtual void M3()
			{
			}

			public static void StaticMethod()
			{

			}
		}

		public class SubClass : BaseClass
		{
			protected override void M2()
			{
			}
			public new void M3()
			{
			}

			public void Test()
			{
				Noop("M1.base", base.M1);
				Noop("M1", M1);
				Noop("M2.base", base.M2);
				Noop("M2", M2);
				Noop("M3.base", base.M3);
				Noop("M3.base_virt", ((BaseClass)this).M3);
				Noop("M3.base_interface", ((IM3)this).M3);
#if CS70
				Noop("M3", this.M3);
				Noop("M3", M3);

#if CS80
				static void M3()
#else
				void M3()
#endif
				{

				}
#else
				Noop("M3", M3);
#endif
			}

			public void Test2()
			{
				Noop("M3.new", new BaseClass().M3);
				Noop("M3.new", new SubClass().M3);
			}

			private void Noop(string name, Action _)
			{
			}
		}

		public class GenericTest<TNonCaptured, TCaptured>
		{
			public Func<TCaptured> GetFunc(Func<TNonCaptured, TCaptured> f)
			{
				TCaptured captured = f(default(TNonCaptured));
				return delegate {
					Console.WriteLine(captured.GetType().FullName);
					return captured;
				};
			}

			public Func<TNonCaptured, TNonCapturedMP, TCaptured> GetFunc<TNonCapturedMP>(Func<TCaptured> f)
			{
				TCaptured captured = f();
				return delegate (TNonCaptured a, TNonCapturedMP d) {
					Console.WriteLine(a.GetHashCode());
					Console.WriteLine(captured.GetType().FullName);
					return captured;
				};
			}
		}

		private delegate void GenericDelegate<T>();
		public delegate void RefRecursiveDelegate(ref RefRecursiveDelegate d);

		public static Func<string, string, bool> test0 = (string a, string b) => string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b);
		public static Func<string, string, bool> test1 = (string a, string b) => string.IsNullOrEmpty(a) || !string.IsNullOrEmpty(b);
		public static Func<string, string, bool> test2 = (string a, string b) => !string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b);
		public static Func<string, string, bool> test3 = (string a, string b) => !string.IsNullOrEmpty(a) || !string.IsNullOrEmpty(b);
		public static Func<string, string, bool> test4 = (string a, string b) => string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b);
		public static Func<string, string, bool> test5 = (string a, string b) => string.IsNullOrEmpty(a) && !string.IsNullOrEmpty(b);
		public static Func<string, string, bool> test6 = (string a, string b) => !string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b);
		public static Func<string, string, bool> test7 = (string a, string b) => !string.IsNullOrEmpty(a) && !string.IsNullOrEmpty(b);

		public static void Test(this string a)
		{
		}

		public static Predicate<T> And<T>(this Predicate<T> filter1, Predicate<T> filter2)
		{
			if (filter1 == null)
			{
				return filter2;
			}
			if (filter2 == null)
			{
				return filter1;
			}
			return (T m) => filter1(m) && filter2(m);
		}

		public static Action<string> ExtensionMethodUnbound()
		{
			return Test;
		}

		public static Action ExtensionMethodBound()
		{
			return "abc".Test;
		}

		public static Action ExtensionMethodBoundOnNull()
		{
			return ((string)null).Test;
		}

		public static Predicate<int> NoExtensionMethodOnLambda()
		{
			return And((int x) => x >= 0, (int x) => x <= 100);
		}

		public static object StaticMethod()
		{
			return new Func<Action>(ExtensionMethodBound);
		}

		public static object InstanceMethod()
		{
			return new Func<string>("hello".ToUpper);
		}

		public static object InstanceMethodOnNull()
		{
			return new Func<string>(((string)null).ToUpper);
		}

		public static List<Action<int>> AnonymousMethodStoreWithinLoop()
		{
			List<Action<int>> list = new List<Action<int>>();
			for (int i = 0; i < 10; i++)
			{
				int counter;
				list.Add(delegate (int x) {
					counter = x;
				});
			}
			return list;
		}

		public static List<Action<int>> AnonymousMethodStoreOutsideLoop()
		{
			List<Action<int>> list = new List<Action<int>>();
			int counter;
			for (int i = 0; i < 10; i++)
			{
				list.Add(delegate (int x) {
					counter = x;
				});
			}
			return list;
		}

		public static Action StaticAnonymousMethodNoClosure()
		{
			return delegate {
				Console.WriteLine();
			};
		}

		public static void NameConflict()
		{
			// i is local in main method,
			// j is captured variable,
			// k is parameter in anonymous method
			// l is local in anonymous method,
			// Ensure that the decompiler doesn't introduce name conflicts
			List<Action<int>> list = new List<Action<int>>();
			for (int i = 0; i < 10; i++)
			{
				int j;
				for (j = 0; j < 10; j++)
				{
					list.Add(delegate (int k) {
						for (int l = 0; l < j; l += k)
						{
							Console.WriteLine();
						}
					});
				}
			}
		}

		public static void NameConflict2(int j)
		{
			List<Action<int>> list = new List<Action<int>>();
			for (int k = 0; k < 10; k++)
			{
				list.Add(delegate (int i) {
					Console.WriteLine(i);
				});
			}
		}

		public static Action<int> NameConflict3(int i)
		{
			return delegate (int j) {
				for (int k = 0; k < j; k++)
				{
					Console.WriteLine(k);
				}
			};
		}

		public static Func<int, Func<int, int>> CurriedAddition(int a)
		{
			return (int b) => (int c) => a + b + c;
		}

		public static Func<int, Func<int, Func<int, int>>> CurriedAddition2(int a)
		{
			return (int b) => (int c) => (int d) => a + b + c + d;
		}

		public static Func<TCaptured> CapturedTypeParameter1<TNonCaptured, TCaptured>(TNonCaptured a, Func<TNonCaptured, TCaptured> f)
		{
			TCaptured captured = f(a);
			return delegate {
				Console.WriteLine(captured.GetType().FullName);
				return captured;
			};
		}

		public static Func<TCaptured> CapturedTypeParameter2<TNonCaptured, TCaptured>(TNonCaptured a, Func<TNonCaptured, List<TCaptured>> f)
		{
			List<TCaptured> captured = f(a);
			return delegate {
				Console.WriteLine(captured.GetType().FullName);
				return captured.FirstOrDefault();
			};
		}

		public static Func<int> Issue1773(short data)
		{
			int integerData = data;
			return () => integerData;
		}

#if !MCS
		// does not compile with mcs...
		public static Func<int> Issue1773b(object data)
		{
#if ROSLYN
			dynamic dynamicData = data;
			return () => dynamicData.DynamicCall();
#else
			// This is a bug in the old csc: captured dynamic local variables did not have the [DynamicAttribute]
			// on the display-class field.
			return () => ((dynamic)data).DynamicCall();
#endif
		}

		public static Func<int> Issue1773c(object data)
		{
#if ROSLYN
			dynamic dynamicData = data;
			return () => dynamicData;
#else
			return () => (dynamic)data;
#endif
		}
#endif

#if CS70
		public static Func<string> Issue1773d((int Integer, string String) data)
		{
			(int Integer, string RenamedString) valueTuple = data;
			return () => valueTuple.RenamedString;
		}
#endif

		public static Func<T, T> Identity<T>()
		{
			return (T _) => _;
		}

		private static void Use(Action a)
		{

		}

		private static void Use2(Func<Func<int, int>, IEnumerable<int>> a)
		{

		}
		private static void Use2<T>(GenericDelegate<T> a)
		{
		}

		private static void Use3<T>(Func<Func<T, T>> a)
		{
		}

		public static void SimpleDelegateReference()
		{
			Use(SimpleDelegateReference);
#if !MCS2
			Use3(Identity<int>);
#endif
		}

		public static void DelegateReferenceWithStaticTarget()
		{
			Use(NameConflict);
			Use(BaseClass.StaticMethod);
		}

		public static void ExtensionDelegateReference(IEnumerable<int> ints)
		{
			Use2(ints.Select<int, int>);
		}

#if CS70
		public static void LocalFunctionDelegateReference()
		{
			Use(LocalFunction);
			Use2<int>(LocalFunction1<int>);
#if CS80
			static void LocalFunction()
#else
			void LocalFunction()
#endif
			{
			}
#if CS80
			static void LocalFunction1<T>()
#else
			void LocalFunction1<T>()
#endif
			{
			}
		}
#endif

#if CS90
		public static Func<int, int, int, int> LambdaParameterDiscard()
		{
			return (int _, int _, int _) => 0;
		}
#endif

#if CS100
		public static Func<int> LambdaWithAttribute0()
		{
			return [My] () => 0;
		}

		public static Func<int, int> LambdaWithAttribute1()
		{
			return [My] (int x) => 0;
		}

		public static Func<int, int> LambdaWithAttributeOnParam()
		{
			return ([My] int x) => 0;
		}

		public static Func<Task<int>> AsyncLambdaWithAttribute0()
		{
			return [My] async () => 0;
		}
		public static Action StatementLambdaWithAttribute0()
		{
			return [My] () => {
			};
		}

		public static Action<int> StatementLambdaWithAttribute1()
		{
			return [return: My] (int x) => {
				Console.WriteLine(x);
			};
		}
		public static Action<int> StatementLambdaWithAttribute2()
		{
			return ([My] int x) => {
				Console.WriteLine(x);
			};
		}
#endif

		public static void CallRecursiveDelegate(ref RefRecursiveDelegate d)
		{
			d(ref d);
		}
	}

	public class Issue1867
	{
		private int value;

		public Func<bool> TestLambda(Issue1867 x)
		{
			Issue1867 m1;
			Issue1867 m2;
			if (x.value > value)
			{
				m1 = this;
				m2 = x;
			}
			else
			{
				m1 = x;
				m2 = this;
			}

			return () => m1.value + 1 == 4 && m2.value > 5;
		}
	}

	internal class Issue2791
	{
		public void M()
		{
			Run(delegate (object o) {
				try
				{
					List<int> list = o as List<int>;
					Action action = delegate {
						list.Select((int x) => x * 2);
					};
#if OPT && ROSLYN
					Action obj = delegate {
#else
					Action action2 = delegate {
#endif
						list.Select((int x) => x * 2);
					};
					Console.WriteLine();
					action();
					Console.WriteLine();
#if OPT && ROSLYN
					obj();
#else
					action2();
#endif
				}
				catch (Exception)
				{
					Console.WriteLine("catch");
				}
				finally
				{
					Console.WriteLine("finally");
				}
			}, null);
		}

		private void Run(ParameterizedThreadStart del, object x)
		{
			del(x);
		}
	}

	[AttributeUsage(AttributeTargets.All)]
	internal class MyAttribute : Attribute
	{
	}
}
