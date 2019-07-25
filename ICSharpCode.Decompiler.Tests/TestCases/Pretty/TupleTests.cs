// Copyright (c) 2018 Daniel Grunwald
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class TupleTests
	{
		private abstract class OverloadResolution
		{
			public abstract void M1((long, long) a);
			public abstract void M1(object a);

			public void UseM1((int, int) a)
			{
				// M1(a); TODO: tuple conversion transform
				// Cast is required to avoid the overload usable via tuple conversion:
				M1((object)a);
			}
		}

		public struct GenericStruct<T>
		{
			public T Field;
			public T Property {
				get;
				set;
			}
		}

		public ValueTuple VT0;
		public ValueTuple<int> VT1;
		public ValueTuple<int, int, int, int, int, int, int, ValueTuple> VT7EmptyRest;

		public (int, uint) Unnamed2;
		public (int, int, int) Unnamed3;
		public (int, int, int, int) Unnamed4;
		public (int, int, int, int, int) Unnamed5;
		public (int, int, int, int, int, int) Unnamed6;
		public (int, int, int, int, int, int, int) Unnamed7;
		public (int, int, int, int, int, int, int, int) Unnamed8;

		public (int a, uint b) Named2;
		public (int a, uint b)[] Named2Array;
		public (int a, int b, int c, int d, int e, int f, int g, int h) Named8;

		public (int, int a, int, int b, int) PartiallyNamed;

		public ((int a, int b) x, (int, int) y, (int c, int d) z) Nested1;
		public ((object a, dynamic b), dynamic, (dynamic c, object d)) Nested2;
		public (ValueTuple a, (int x1, int x2), ValueTuple<int> b, (int y1, int y2), (int, int) c) Nested3;
		public (int a, int b, int c, int d, int e, int f, int g, int h, (int i, int j)) Nested4;

		public Dictionary<(int a, string b), (string c, int d)> TupleDict;
		public List<(int, string)> List;
		public bool HasItems => List.Any(((int, string) a) => a.Item1 > 0);

		public int VT1Member => VT1.Item1;
		public int AccessUnnamed8 => Unnamed8.Item8;
		public int AccessNamed8 => Named8.h;
		public int AccessPartiallyNamed => PartiallyNamed.a + PartiallyNamed.Item3;

		public ValueTuple<int> NewTuple1 => new ValueTuple<int>(1);
		public (int a, int b) NewTuple2 => (1, 2);
		public object BoxedTuple10 => (1, 2, 3, 4, 5, 6, 7, 8, 9, 10);

		public (uint, int) SwapUnnamed => (Unnamed2.Item2, Unnamed2.Item1);
		public (uint, int) SwapNamed2 => (Named2.b, Named2.a);

		public int TupleHash => (1, 2, 3).GetHashCode();
		public int TupleHash2 => Named2.GetHashCode();

		public (int, int) AccessRest => (1, 2, 3, 4, 5, 6, 7, 8, 9).Rest;

		public (string, object, Action) TargetTyping => (null, 1, delegate {
		});

		public object NotTargetTyping => ((string)null, (object)1, (Action)delegate {
		});

		public void UseDict()
		{
			if (TupleDict.Count > 10) {
				TupleDict.Clear();
			}
			// TODO: it would be nice if we could infer the name 'c' for the local
			string item = TupleDict[(1, "abc")].c;
			Console.WriteLine(item);
			Console.WriteLine(item);
			Console.WriteLine(TupleDict.Values.ToList().First().d);
		}

		public void Issue1174()
		{
			Console.WriteLine((1, 2, 3).GetHashCode());
		}

		public void LocalVariables((int, int) a)
		{
			(int, int) valueTuple = (a.Item1 + a.Item2, a.Item1 * a.Item2);
			Console.WriteLine(valueTuple.ToString());
			Console.WriteLine(valueTuple.GetType().FullName);
		}

		public void Foreach(IEnumerable<(int, string)> input)
		{
			foreach (var item in input) {
				Console.WriteLine($"{item.Item1}: {item.Item2}");
			}
		}

		public void ForeachNamedElements(IEnumerable<(int Index, string Data)> input)
		{
			foreach (var item in input) {
				Console.WriteLine($"{item.Index}: {item.Data}");
			}
		}

		public void NonGenericForeach(IEnumerable input)
		{
			foreach ((string, int) item in input) {
				Console.WriteLine($"{item.Item1}: {item.Item2}");
			}
		}

		public void CallForeach()
		{
			Foreach(new List<(int, string)> {
				(1, "a"),
				(2, "b")
			});
		}

		public void DynamicTuple((dynamic A, dynamic B) a)
		{
			a.A.DynamicCall();
			a.B.Dynamic = 42;
		}

		public void GenericStructWithElementNames(GenericStruct<(int A, int B)> s)
		{
			Console.WriteLine(s.Field.A + s.Property.B);
		}
	}
}
