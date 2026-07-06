using System;
using System.Runtime.CompilerServices;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public interface IOrpShape
	{
	}

	public class OrpCircle : IOrpShape
	{
	}

	public static class OrpCircleExtensions
	{
		public static void Grouped(this OrpCircle circle)
		{
			Console.WriteLine("OrpCircleExtensions.Grouped(OrpCircle)");
		}
	}

	public static class OrpShapeExtensions
	{
		[OverloadResolutionPriority(1)]
		public static void Fmt(this IOrpShape shape)
		{
			Console.WriteLine("Fmt(IOrpShape)");
		}

		public static void Fmt(this OrpCircle circle)
		{
			Console.WriteLine("Fmt(OrpCircle)");
		}

		[OverloadResolutionPriority(1)]
		public static void Grouped(this IOrpShape shape)
		{
			Console.WriteLine("OrpShapeExtensions.Grouped(IOrpShape)");
		}
	}

	public class OverloadResolutionPriorityTests
	{
		public class Base
		{
			[OverloadResolutionPriority(5)]
			public void Disambiguate(object x)
			{
				Console.WriteLine("Base.Disambiguate(object)");
			}
		}

		public class Derived : Base
		{
			public void Disambiguate(string x)
			{
				Console.WriteLine("Derived.Disambiguate(string)");
			}
		}

		public class Widget
		{
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

			[OverloadResolutionPriority(1)]
			public Widget(long x)
			{
				Console.WriteLine("Widget(long)");
			}

			public Widget(int x)
			{
				Console.WriteLine("Widget(int)");
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
			Console.WriteLine("Params(params int[])");
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

		public static void CallsWithConstantArguments()
		{
			// Each call binds to the [OverloadResolutionPriority] winner, not to the
			// overload classic betterness would pick. The decompiler does not know
			// about the attribute, so it keeps the arguments explicitly typed; that
			// output still re-resolves to the same winner under C# 13 rules.
#if EXPECTED_OUTPUT
			Integer(5L);
			Obj((object)"s");
			Neg(5L);
			Amb(1, 1L);
			Params(new int[1] { 5 });
			OverloadResolutionPriorityTests.Generic<int>(5);
			Formattable((IFormattable)5);
#else
			Integer(5);
			Obj("s");
			Neg(5);
			Amb(1, 1);
			Params(5);
			Generic(5);
			Formattable(5);
#endif
			// These forms are already unambiguous without priority knowledge and
			// round-trip unchanged.
			Integer(5L);
			Params();
			Params(1, 2);
		}

		public static void CallWithInParameter(int value)
		{
#if EXPECTED_OUTPUT
			In(in value);
#else
			In(value);
#endif
		}

		public static Widget CreateWidget()
		{
#if EXPECTED_OUTPUT
			return new Widget(5L);
#else
			return new Widget(5);
#endif
		}

		public static int UseIndexer(Widget widget)
		{
#if EXPECTED_OUTPUT
			return widget[5L];
#else
			return widget[5];
#endif
		}

		public static void UseExtensions(OrpCircle circle)
		{
#if EXPECTED_OUTPUT
			((IOrpShape)circle).Fmt();
#else
			circle.Fmt();
#endif
			// Priorities are only compared between overloads declared in the same
			// type: OrpShapeExtensions.Grouped(IOrpShape) has priority 1, but it does
			// not displace OrpCircleExtensions.Grouped(OrpCircle), which wins by
			// classic betterness.
			circle.Grouped();
		}

		public static void UseDerived(Derived derived)
		{
			// The high priority on Base.Disambiguate(object) is irrelevant: the
			// applicable candidate in the most-derived type wins as usual.
			derived.Disambiguate("x");
		}
	}
}
