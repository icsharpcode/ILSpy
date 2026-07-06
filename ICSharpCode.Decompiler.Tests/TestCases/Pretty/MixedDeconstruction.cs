using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class MixedDeconstruction
	{
		private class DeconstructionSource<T, T2>
		{
			public void Deconstruct(out T a, out T2 b)
			{
				a = default(T);
				b = default(T2);
			}
		}

		private class DeconstructionSource<T, T2, T3>
		{
			public void Deconstruct(out T a, out T2 b, out T3 c)
			{
				a = default(T);
				b = default(T2);
				c = default(T3);
			}
		}

		public int IntField;

		public int IntProperty { get; set; }

		private DeconstructionSource<T, T2> GetSource<T, T2>()
		{
			return null;
		}

		private DeconstructionSource<T, T2, T3> GetSource<T, T2, T3>()
		{
			return null;
		}

		private (T, T2) GetTuple<T, T2>()
		{
			return default((T, T2));
		}

		private (T, T2, T3) GetTuple<T, T2, T3>()
		{
			return default((T, T2, T3));
		}

		private ((T, T2), T3) GetNestedTuple<T, T2, T3>()
		{
			return default(((T, T2), T3));
		}

		public void ParameterAndDeclaration_Custom(int x)
		{
			(x, string value) = GetSource<int, string>();
			Console.WriteLine(x);
			Console.WriteLine(value);
		}

		public void ParameterAndDeclaration_Tuple(int x)
		{
			(x, string value) = GetTuple<int, string>();
			Console.WriteLine(x);
			Console.WriteLine(value);
		}

		public void DeclarationAndParameter_Custom(string s)
		{
			(int value, s) = GetSource<int, string>();
			Console.WriteLine(value);
			Console.WriteLine(s);
		}

		public void DeclarationAndParameter_Tuple(string s)
		{
			(int value, s) = GetTuple<int, string>();
			Console.WriteLine(value);
			Console.WriteLine(s);
		}

		public void LocalAndDeclaration_Custom()
		{
			int num = Console.Read();
			Console.WriteLine(num);
			(num, double value) = GetSource<int, double>();
			Console.WriteLine(num);
			Console.WriteLine(value);
		}

		public void LocalAndDeclaration_Tuple()
		{
			int num = Console.Read();
			Console.WriteLine(num);
			(num, double value) = GetTuple<int, double>();
			Console.WriteLine(num);
			Console.WriteLine(value);
		}

		public void OutParameterAndDeclaration_Custom(out int x)
		{
			(x, string value) = GetSource<int, string>();
			Console.WriteLine(value);
		}

		public void OutParameterAndDeclaration_Tuple(out int x)
		{
			(x, string value) = GetTuple<int, string>();
			Console.WriteLine(value);
		}

		public void DiscardSecond_Custom(int x)
		{
			(x, _, double value) = GetSource<int, string, double>();
			Console.WriteLine(x);
			Console.WriteLine(value);
		}

		public void DiscardSecond_Tuple(int x)
		{
			(x, _, double value) = GetTuple<int, string, double>();
			Console.WriteLine(x);
			Console.WriteLine(value);
		}

		public void FieldAndDeclaration_Custom()
		{
			(IntField, string value) = GetSource<int, string>();
			Console.WriteLine(value);
		}

		public void FieldAndDeclaration_Tuple()
		{
			(IntField, string value) = GetTuple<int, string>();
			Console.WriteLine(value);
		}

		public void PropertyAndDeclaration_Custom()
		{
			(IntProperty, string value) = GetSource<int, string>();
			Console.WriteLine(value);
		}

		public void PropertyAndDeclaration_Tuple()
		{
			(IntProperty, string value) = GetTuple<int, string>();
			Console.WriteLine(value);
		}

		public void IntToLongConversion_Custom(long l)
		{
			(l, string value) = GetSource<int, string>();
			Console.WriteLine(l);
			Console.WriteLine(value);
		}

		public void IntToLongConversion_Tuple(long l)
		{
			(l, string value) = GetTuple<int, string>();
			Console.WriteLine(l);
			Console.WriteLine(value);
		}

		public void Nested_Tuple(int x)
		{
			((x, string value), double value2) = GetNestedTuple<int, string, double>();
			Console.WriteLine(x);
			Console.WriteLine(value);
			Console.WriteLine(value2);
		}
	}
}
