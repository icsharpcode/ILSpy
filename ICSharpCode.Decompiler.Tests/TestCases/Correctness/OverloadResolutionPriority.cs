// Copyright (c) 2026 Siegfried Pammer
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
using System.Runtime.CompilerServices;

namespace ICSharpCode.Decompiler.Tests.TestCases.Correctness
{
	// Exercises C# 13 [OverloadResolutionPriority]: every call site below binds to the
	// priority winner, not to the overload classic betterness would pick. The decompiled
	// output must re-resolve to the same winners when recompiled, otherwise the printed
	// overload names diverge.
	internal static class OverloadResolutionPriorityTest
	{
		internal class Widget
		{
			[OverloadResolutionPriority(1)]
			public Widget(long x)
			{
				Console.WriteLine("Widget(long)");
			}

			public Widget(int x)
			{
				Console.WriteLine("Widget(int)");
			}

			[OverloadResolutionPriority(1)]
			public int this[long x] {
				get {
					Console.WriteLine("Widget.this[long]");
					return 1;
				}
			}

			public int this[int x] {
				get {
					Console.WriteLine("Widget.this[int]");
					return 0;
				}
			}
		}

		internal class Base
		{
			[OverloadResolutionPriority(5)]
			public void Disambiguate(object x)
			{
				Console.WriteLine("Base.Disambiguate(object)");
			}
		}

		internal class Derived : Base
		{
			public void Disambiguate(string x)
			{
				Console.WriteLine("Derived.Disambiguate(string)");
			}
		}

		[OverloadResolutionPriority(1)]
		public static void Integer(long x)
		{
			Console.WriteLine("Integer(long)");
		}

		public static void Integer(int x)
		{
			Console.WriteLine("Integer(int)");
		}

		[OverloadResolutionPriority(1)]
		public static void Obj(object x)
		{
			Console.WriteLine("Obj(object)");
		}

		public static void Obj(string x)
		{
			Console.WriteLine("Obj(string)");
		}

		public static void Neg(long x)
		{
			Console.WriteLine("Neg(long)");
		}

		[OverloadResolutionPriority(-1)]
		public static void Neg(int x)
		{
			Console.WriteLine("Neg(int)");
		}

		[OverloadResolutionPriority(1)]
		public static void Amb(int x, long y)
		{
			Console.WriteLine("Amb(int, long)");
		}

		public static void Amb(long x, int y)
		{
			Console.WriteLine("Amb(long, int)");
		}

		[OverloadResolutionPriority(1)]
		public static void Params(params int[] xs)
		{
			Console.WriteLine("Params(params int[]) " + xs.Length);
		}

		public static void Params(int x)
		{
			Console.WriteLine("Params(int)");
		}

		[OverloadResolutionPriority(1)]
		public static void Generic<T>(T x)
		{
			Console.WriteLine("Generic<T>");
		}

		public static void Generic(int x)
		{
			Console.WriteLine("Generic(int)");
		}

		[OverloadResolutionPriority(1)]
		public static void In(in int x)
		{
			Console.WriteLine("In(in int)");
		}

		public static void In(int x)
		{
			Console.WriteLine("In(int)");
		}

		[OverloadResolutionPriority(1)]
		public static void Formattable(IFormattable x)
		{
			Console.WriteLine("Formattable(IFormattable)");
		}

		public static void Formattable(int x)
		{
			Console.WriteLine("Formattable(int)");
		}

		private static void CallWithLocal()
		{
			int value = int.Parse("5");
			In(value);
			Integer(value);
		}

		private static void Main()
		{
			Integer(5);
			Integer(5L);
			Obj("s");
			Neg(5);
			Amb(1, 1);
			Params();
			Params(5);
			Params(1, 2);
			Generic(5);
			In(5);
			CallWithLocal();
			Formattable(5);
			Widget widget = new Widget(5);
			Console.WriteLine(widget[5]);
			new Derived().Disambiguate("x");
			OrpShape.Circle circle = new OrpShape.Circle();
			circle.Fmt();
			circle.Grouped();
		}
	}

	internal static class OrpShape
	{
		internal interface IShape
		{
		}

		internal class Circle : IShape
		{
		}
	}

	internal static class OrpShapeExtensions
	{
		[OverloadResolutionPriority(1)]
		public static void Fmt(this OrpShape.IShape shape)
		{
			Console.WriteLine("Fmt(IShape)");
		}

		public static void Fmt(this OrpShape.Circle circle)
		{
			Console.WriteLine("Fmt(Circle)");
		}

		[OverloadResolutionPriority(1)]
		public static void Grouped(this OrpShape.IShape shape)
		{
			Console.WriteLine("OrpShapeExtensions.Grouped(IShape)");
		}
	}

	internal static class OrpCircleExtensions
	{
		// Lower priority than OrpShapeExtensions.Grouped, but priorities are only
		// compared between overloads declared in the same type, so this one still
		// wins by classic betterness.
		public static void Grouped(this OrpShape.Circle circle)
		{
			Console.WriteLine("OrpCircleExtensions.Grouped(Circle)");
		}
	}
}
