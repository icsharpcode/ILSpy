using System;
using System.Collections.Generic;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class ExtensionMethods
	{
		public struct Value
		{
			public int Field;
		}

		public class HasInstanceMethod
		{
			public void Ambiguous(int i)
			{
			}
		}

		public void Simple(string text)
		{
			text.Print();
		}

		public void NamedArgumentAfterReceiver(List<int> list)
		{
			list.FirstOrLast(last: true);
		}

		public void NullReceiver()
		{
			((string)null).Print();
		}

		public void ExplicitTypeArguments(object o)
		{
			o.As<string>();
		}

		public void RefReceiver(Value value)
		{
			value.Increment();
		}

		public void InReceiver(Value value)
		{
			value.Read();
		}

		public void ParamsExpansion(string text)
		{
			text.Repeat(1, 2, 3);
		}

		public void InstanceMethodWinsSoTheCallStaysStatic(HasInstanceMethod x)
		{
			ExtensionMethodsProvider.Ambiguous(x, 1);
		}

		public Action MethodGroup(string text)
		{
			return text.Print;
		}
	}

	public static class ExtensionMethodsProvider
	{
		public static void Print(this string text)
		{
		}

		public static int FirstOrLast<T>(this List<T> list, bool last)
		{
			return list.Count;
		}

		public static T As<T>(this object o) where T : class
		{
			return o as T;
		}

		public static void Increment(this ref ExtensionMethods.Value value)
		{
			value.Field++;
		}

		public static int Read(this in ExtensionMethods.Value value)
		{
			return value.Field;
		}

		public static int Repeat(this string text, params int[] values)
		{
			return text.Length + values.Length;
		}

		public static void Ambiguous(this ExtensionMethods.HasInstanceMethod x, int i)
		{
		}
	}
}
