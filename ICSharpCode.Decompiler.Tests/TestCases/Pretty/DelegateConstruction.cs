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
using System.Threading;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public static class DelegateConstruction
	{
		private class InstanceTests
		{
			public struct SomeData
			{
				public string Value;
			}

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
				foreach (int item in Enumerable.Empty<int>()) {
					if (item > 0) {
						return delegate {
							CaptureOfThisAndParameter(item + a);
						};
					}
				}
				return null;
			}

			public Action CaptureOfThisAndParameterInForEachWithItemCopy(int a)
			{
				foreach (int item in Enumerable.Empty<int>()) {
					int copyOfItem = item;
					if (item > 0) {
						return delegate {
							CaptureOfThisAndParameter(item + a + copyOfItem);
						};
					}
				}
				return null;
			}

			public void LambdaInForLoop()
			{
				for (int i = 0; i < 100000; i++) {
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
					if (amount < 0) {
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
					if (amount < 0) {
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

				void M3()
				{

				}
#else
				Noop("M3", M3);
#endif
			}

			private void Noop(string name, Action _)
			{
			}
		}

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
			if (filter1 == null) {
				return filter2;
			}
			if (filter2 == null) {
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
			for (int i = 0; i < 10; i++) {
				int counter;
				list.Add(delegate(int x) {
					counter = x;
				});
			}
			return list;
		}

		public static List<Action<int>> AnonymousMethodStoreOutsideLoop()
		{
			List<Action<int>> list = new List<Action<int>>();
			int counter;
			for (int i = 0; i < 10; i++) {
				list.Add(delegate(int x) {
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
			// i is captured variable,
			// j is parameter in anonymous method
			// l is local in anonymous method,
			// k is local in main method
			// Ensure that the decompiler doesn't introduce name conflicts
			List<Action<int>> list = new List<Action<int>>();
			for (int k = 0; k < 10; k++) {
				int i;
				for (i = 0; i < 10; i++) {
					list.Add(delegate(int j) {
						for (int l = 0; l < i; l += j) {
							Console.WriteLine();
						}
					});
				}
			}
		}

		public static void NameConflict2(int j)
		{
			List<Action<int>> list = new List<Action<int>>();
			for (int k = 0; k < 10; k++) {
				list.Add(delegate(int i) {
					Console.WriteLine(i);
				});
			}
		}

		public static Action<int> NameConflict3(int i)
		{
			return delegate(int j) {
				for (int k = 0; k < j; k++) {
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
	}
}
