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

#pragma warning disable 660, 661

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class OverloadResolution
	{
		public struct Data
		{
			public int Field;

			public static bool operator ==(Data a, Data b)
			{
				return a.Field == b.Field;
			}

			public static bool operator !=(Data a, Data b)
			{
				return a.Field != b.Field;
			}
		}

		public struct OtherData
		{
			public int Field;
		}

		public void IntegerOverloads()
		{
			Integer(1);
			Integer((short)1);
			Integer(1L);
		}

		public void ReferenceTypeOverloads()
		{
			RefType("string");
			RefType((object)"string");
			RefType(null);
			RefType((object)null);
		}

		public void NullableOverloads()
		{
			NullableInt(1);
			NullableInt((int?)1);
			NullableInt(null);
		}

		public void ParamsOverloads(int n)
		{
			Params(1);
			Params(1, 2);
			Params(default, default, default);
			Params(new int[n]);
		}

		public void GenericOverloads()
		{
			Generic(1);
			Generic<int>(1);
		}

		public string AmbiguousDefaultArgumentStaysTyped()
		{
			return Ambiguous(default(Data));
		}

		public string DefaultArgumentWithBetterConversionTarget()
		{
			return WithNullable(default);
		}

		public bool EqualityWithDefaultStaysTyped(Data data)
		{
			return data == default(Data);
		}

		private static void Integer(short s)
		{
		}

		private static void Integer(int i)
		{
		}

		private static void Integer(long l)
		{
		}

		private static void RefType(object o)
		{
		}

		private static void RefType(string s)
		{
		}

		private static void NullableInt(int i)
		{
		}

		private static void NullableInt(int? i)
		{
		}

		private static void Params(int i)
		{
		}

		private static void Params(params int[] xs)
		{
		}

		private static void Generic(int i)
		{
		}

		private static void Generic<T>(T a)
		{
		}

		private static string Ambiguous(Data x)
		{
			return "Data";
		}

		private static string Ambiguous(OtherData x)
		{
			return "OtherData";
		}

		private static string WithNullable(Data x)
		{
			return "Data";
		}

		private static string WithNullable(Data? x)
		{
			return "Data?";
		}
	}
}
