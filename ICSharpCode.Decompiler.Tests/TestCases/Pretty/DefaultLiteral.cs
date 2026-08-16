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

using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class DefaultLiteral
	{
		public struct Data
		{
			public int Field;

			public static Data operator +(Data a, Data b)
			{
				return new Data {
					Field = a.Field + b.Field
				};
			}
		}

		public struct OtherData
		{
			public int Field;
		}

		public int this[Data d] => d.Field;

		public int DeclarationWithInitializer()
		{
			Data data = default;
			return data.Field + data.Field;
		}

		public int Assignment(Data data)
		{
			int field = data.Field;
			data = default;
			return field + data.Field;
		}

		public Data Return()
		{
			return default;
		}

		public T ReturnGeneric<T>()
		{
			return default;
		}

		public async Task<Data> ReturnAsync()
		{
			await Task.Yield();
			return default;
		}

		public string AmbiguousArgumentStaysTyped()
		{
			return Overloaded(default(Data));
		}

		public string BetterConversionTargetArgument()
		{
			return OverloadedNullable(default);
		}

		public int UnambiguousArgument()
		{
			return Single(default);
		}

		public string BoxedArgumentStaysTyped()
		{
			return Boxed(default(Data));
		}

		public int IndexerArgument()
		{
			return this[default];
		}

		public Data OperatorOperandStaysTyped(Data data)
		{
			return data + default(Data);
		}

		public string NonIdentityConversionsStayTyped()
		{
			object obj = default(Data);
			return obj.ToString() + obj.ToString();
		}

		private int Single(Data data)
		{
			return data.Field;
		}

		private string Boxed(object o)
		{
			return o.ToString();
		}

		private string Overloaded(Data x)
		{
			return "Data";
		}

		private string Overloaded(OtherData x)
		{
			return "OtherData";
		}

		private string OverloadedNullable(Data x)
		{
			return "Data";
		}

		private string OverloadedNullable(Data? x)
		{
			return "Data?";
		}
	}
}
